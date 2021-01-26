using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

using goa.Common.Exceptions;

namespace goa.Common
{
    public enum OperationCancelledCause
    {
        Other,
        InactiveDesignOption,
    }

    /// <summary>
    /// base class of all editor, creator and remover.
    /// </summary>
    public partial class ElementOperator
    {
        /// <summary>
        /// TRANSACTION GROUP INSIDE.
        /// Apply recorded edits, check location line failure
        /// after initial commit. Force re-create failed instances and
        /// commit again.
        /// </summary>
        public static void ExecuteOpsTransGroup
            (IEnumerable<ElementOperator> _ops,
            Document _doc,
            bool _forceRecreate,
            out List<ElementOperator> _reCreated,
            out List<ElementOperator> _notExecuted,
            string _transName = "应用修改")
        {
            //apply edits
            using (TransactionGroup tg = new TransactionGroup(_doc, _transName))
            {
                tg.Start();
                ExecuteSingleLoop(_doc, _ops, _forceRecreate, out _reCreated, out _notExecuted);
                tg.Assimilate();
            }
        }

        /// <summary>
        /// TRANSACTIONS INSIDE
        /// </summary>
        public static void ExecuteSingleLoop
            (Document _doc,
            IEnumerable<ElementOperator> _ops,
            bool _forceRecreate,
            out List<ElementOperator> _reCreated,
            out List<ElementOperator> _notExecuted)
        {
            _reCreated = new List<ElementOperator>();
            _notExecuted = new List<ElementOperator>();

            //commit operations
            Operation123(_doc, _ops);

            if (_forceRecreate)
            {
                //check if any failed ops
                var needToRecreate = new List<LineBasedFamilyEditor>();
                foreach (var op in _ops)
                {
                    //line-based family might be draged by each other
                    if (op is LineBasedFamilyEditor)
                    {
                        var lbEdtr = op as LineBasedFamilyEditor;
                        //if ops are not finished due to other cause, skip
                        if (lbEdtr.OperationFinished == false)
                            continue;
                        var offsetLocLine = LineBasedFamilyUtils.GetOffsetLocLine(lbEdtr.FI);
                        Debug.WriteLine("id: " + lbEdtr.FI.Id);
                        Debug.WriteLine("oll: " + offsetLocLine.ToStringCenterLength(3));
                        var expected = lbEdtr.OffsetLocLine;
                        Debug.WriteLine("exp: " + expected.ToStringCenterLength(3));
                        if (expected != null
                            && expected.ToStringCenterLength(3) != offsetLocLine.ToStringCenterLength(3))
                        {
                            var similarCtr = lbEdtr.GetSimilarCreator();
                            similarCtr.RecreatedFrom = lbEdtr.Elem.Id;
                            _reCreated.Add(similarCtr);
                            needToRecreate.Add(lbEdtr);
                        }
                    }
                }
                if (needToRecreate.Count != 0)
                {
                    //re-create failed
                    Operation123(_doc, _reCreated);

                    using (Transaction trans = new Transaction(_doc, "delete dup"))
                    {
                        trans.Start();
                        foreach (var edtr in needToRecreate)
                        {
                            edtr.OperationFinished = true;
                            _doc.Delete(edtr.FI.Id);
                        }
                        trans.Commit();
                    }
                }
            }
            //get unfinished ops, due to design options (or other causes)
            _notExecuted = _ops.Where(x => x.OperationFinished == false).ToList();
            return;
        }

        /// <summary>
        /// TRANSACTIONS INSIDE.
        /// Execute all operators in three steps. 
        /// Also checks host problem, if any, fix and prompt user
        /// to restart operation.
        /// </summary>
        public static void Operation123(Document _doc, IEnumerable<ElementOperator> _ops)
        {
            var removers = _ops.Where(x => x is ElementRemover);
            var nonRemovers = _ops.Where(x => x is ElementRemover == false);
            var ds = new DialogSuppressor();
            string errorMsg = "操作未完成。";
            using (Transaction trans = new Transaction(_doc, "preprocess"))
            {
                ds.UseInTransaction(trans);
                trans.Start();
                foreach (var op in nonRemovers)
                    op.PreProcess();
                trans.Commit();
                if (trans.GetStatus() != TransactionStatus.Committed)
                    throw new TransactionNotCommitedException(errorMsg);
            }
        execute:
            using (Transaction trans = new Transaction(_doc, "execute"))
            {
                ds.UseInTransaction(trans);
                trans.Start();
                foreach (var op in nonRemovers)
                {
                    if (!op.Cancel)
                        op.Execute();
                }
                if (FamilyCreator.HostsWithInstanceFaceProblem.Count > 0)
                {
                    trans.RollBack();
                }
                else
                {
                    trans.Commit();
                    if (trans.GetStatus() != TransactionStatus.Committed)
                        throw new TransactionNotCommitedException(errorMsg);
                }
            }

            //the geometry face retrieved from it might be in one of two states: 
            //A, directly retrieved from instance. The stable reference string does not contain "INSTANCE", or;
            //B, retrieved from symbol geometry first, then apply instance transform. The stable reference string contains "INSTANCE".
            //State B might result in failure of creating new instance on face, even when the line stays 
            //perfectly inside face. 
            //To avoid this problem, a face with state A must be ensured.
            //A join operation of the host will make sure that faces retrieve from it
            //will always be in state A.
            if (FamilyCreator.HostsWithInstanceFaceProblem.Count > 0)
            {
                UserMessages.ShowMessage("某些主体图元导致本次操作失败。已修复这些图元，请重新进行操作。");
                using (Transaction trans = new Transaction(_doc, "fix"))
                {
                    ds.UseInTransaction(trans);
                    trans.Start();
                    foreach (var host in FamilyCreator.HostsWithInstanceFaceProblem)
                    {
                        HostUtils.FixInstanceFaceProblem(host);
                    }
                    trans.Commit();
                }
            }
            FamilyCreator.HostsWithInstanceFaceProblem.Clear();

            using (Transaction trans = new Transaction(_doc, "postprocess"))
            {
                ds.UseInTransaction(trans);
                trans.Start();
                foreach (var op in nonRemovers)
                {
                    if (!op.Cancel)
                        op.PostProcess();
                }
                trans.Commit();
                if (trans.GetStatus() != TransactionStatus.Committed)
                    throw new TransactionNotCommitedException(errorMsg);
            }

            //hand and facing need second round of set.
            var ctrs = nonRemovers
                .Where(x => x is WallBasedFamilyCreator
                    || x is FaceBasedFamilyCreator).ToList();
            if (ctrs.Count > 0)
            {
                using (Transaction trans = new Transaction(_doc, "postprocess 2"))
                {
                    ds.UseInTransaction(trans);
                    trans.Start();
                    foreach (var ctr in ctrs)
                    {
                        if (!ctr.Cancel)
                            ctr.PostProcess();
                    }
                    trans.Commit();
                    if (trans.GetStatus() != TransactionStatus.Committed)
                        throw new TransactionNotCommitedException(errorMsg);
                }
            }

            //delete elements
            using (Transaction trans = new Transaction(_doc, "delete"))
            {
                trans.Start();
                foreach (var op in removers)
                    op.Execute();
                trans.Commit();
            }
        }
        /// <summary>
        /// TRANSACTION INSIDE.
        /// </summary>
        /// <param name="ops"></param>
        /// <param name="doc"></param>
        public static void FixHostProblem(IEnumerable<ElementOperator> ops, Document doc)
        {
            var newFis = ops.Where(x => x is FamilyCreator)
                .Cast<FamilyCreator>()
                .Where(x => x.OperationFinished)
                .Select(x => x.NewFI);
            using (Transaction trans = new Transaction(doc, "fix"))
            {
                trans.Start();
                foreach (var newFI in newFis)
                {
                    HostUtils.FixInstanceFaceProblem(newFI);
                }
                trans.Commit();
            }
        }
    }
}
