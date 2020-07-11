using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.Revit.DB;
using System.DirectoryServices;
using Autodesk.Revit.UI.Selection;
using System.Windows.Forms;

namespace goa.Common
{
    #region Design Option
    public class DesignOptionSet
    {
        public Element DesignOptionSet_revit;
        public string Name { get { return DesignOptionSet_revit.Name; } }
        public IList<DesignOption> DesignOptions { get; set; }

        public static IList<DesignOptionSet> GetDesignOptionSets(Document document)
        {
            Dictionary<ElementId, List<DesignOption>> dic = new Dictionary<ElementId, List<DesignOption>>();
            var allDesignOptions = new FilteredElementCollector(document).OfClass(typeof(DesignOption)).Cast<DesignOption>();
            foreach (DesignOption dOpt in allDesignOptions)
            {
                Element dosElem = document.GetElement(dOpt.get_Parameter(BuiltInParameter.OPTION_SET_ID).AsElementId());
                dic.TryAddValue(dosElem.Id, dOpt);
            }
            List<DesignOptionSet> list = new List<DesignOptionSet>();
            foreach (var pair in dic)
            {
                var dos = new DesignOptionSet();
                dos.DesignOptionSet_revit = document.GetElement(pair.Key);
                dos.DesignOptions = pair.Value.Cast<DesignOption>().ToList();
                list.Add(dos);
            }
            return list;
        }
    }
    public class DesignOptionWrapper
    {
        public DesignOptionSet DesignOptionSet;
        public DesignOption DesignOption;
        public string LongName { get { return DesignOptionSet.Name + " : " + DesignOption.Name; } }
        public DesignOptionWrapper(DesignOptionSet _set, DesignOption _deOp)
        {
            this.DesignOptionSet = _set;
            this.DesignOption = _deOp;
        }
    }
    #endregion

    #region Geometry
    public class UVLine
    {
        public UV p0;
        public UV p1;
        public UV Dir;
        public UV Center;
        public double Length;

        public UVLine(UV _p0, UV _p1)
        {
            this.p0 = _p0;
            this.p1 = _p1;
            this.Center = (_p1 + _p0) / 2;
            this.Dir = (_p1 - _p0).Normalize();
            this.Length = _p0.DistanceTo(_p1);
        }

        /// <summary>
        /// Evaluate with normalized parameter
        /// </summary>
        public UV Evaluate(double _f)
        {
            var length = this.Length;
            var origin = this.p0;
            var dir = this.Dir;
            return origin + dir * _f * length;
        }

        public override string ToString()
        {
            string st = this.p0.ToStringDigits(5);
            string ed = this.p1.ToStringDigits(5);
            return st + "||" + ed;
        }
    }
    #endregion

    #region Selection Filter
    public class CurveElementSelectionFilter : ISelectionFilter
    {
        public bool AllowElement(Element elem)
        {
            if (elem is CurveElement)
                return true;
            else
                return false;
        }

        public bool AllowReference(Reference reference, XYZ position)
        {
            return false;
        }
    }
    public class FilledRegionSelectionFilter : ISelectionFilter
    {
        public bool AllowElement(Element elem)
        {
            if (elem is FilledRegion)
                return true;
            else
                return false;
        }

        public bool AllowReference(Reference reference, XYZ position)
        {
            return false;
        }
    }
    public class CurtainGridLineSelectionFilter : ISelectionFilter
    {
        public bool AllowElement(Element elem)
        {
            if (elem is CurtainGridLine)
                return true;
            else
                return false;
        }

        public bool AllowReference(Reference reference, XYZ position)
        {
            return false;
        }
    }
    public class CurtainPanelSelectionFilter : ISelectionFilter
    {
        public bool AllowElement(Element elem)
        {
            if (elem is Autodesk.Revit.DB.Panel)
                return true;
            else
                return false;
        }

        public bool AllowReference(Reference reference, XYZ position)
        {
            return false;
        }
    }
    public class LineElementSelectionFilter : ISelectionFilter
    {
        public bool AllowElement(Element elem)
        {
            if (elem is CurveElement)
            {
                var ce = elem as CurveElement;
                if (ce.GeometryCurve is Line)
                    return true;
            }
            return false;
        }
        public bool AllowReference(Reference reference, XYZ position)
        {
            return false;
        }
    }

    public class GroupSelectionFilter : ISelectionFilter
    {
        public bool AllowElement(Element elem)
        {
            if (elem is Group)
                return true;
            else
                return false;
        }

        public bool AllowReference(Reference reference, XYZ position)
        {
            return false;
        }
    }
    public class FamilyLoadOptions : IFamilyLoadOptions
    {
        public bool OnFamilyFound(
          bool familyInUse,
          out bool overwriteParameterValues)
        {
            overwriteParameterValues = true;
            return true;
        }

        public bool OnSharedFamilyFound(
          Family sharedFamily,
          bool familyInUse,
          out FamilySource source,
          out bool overwriteParameterValues)
        {
            source = FamilySource.Family;
            overwriteParameterValues = true;
            return true;
        }
    }
    public class WallSelectionFilter : ISelectionFilter
    {
        public bool AllowElement(Element elem)
        {
            if (elem is Wall)
                return true;
            else
                return false;
        }

        public bool AllowReference(Reference reference, XYZ position)
        {
            return false;
        }
    }
    public class DimensionSelectionFilter : ISelectionFilter
    {
        public bool AllowElement(Element elem)
        {
            if (elem is Dimension)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool AllowReference(Reference reference, XYZ position)
        {
            return false;
        }
    }
    public class FamilyInstanceSelectionFilter : ISelectionFilter
    {
        public bool AllowElement(Element elem)
        {
            if (elem is FamilyInstance)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool AllowReference(Reference reference, XYZ position)
        {
            return false;
        }
    }
    public class SitePlanFamilySelectionFilter : ISelectionFilter
    {
        public bool AllowElement(Element elem)
        {
            SitePlanFamilyType type = SitePlanFamilyType.Block_Highrise;
            return elem is FamilyInstance
                && FirmStandards.IsSitePlanFamily(elem, ref type);
        }

        public bool AllowReference(Reference reference, XYZ position)
        {
            return false;
        }
    }
    #endregion

    #region Transaction
    /// <summary>
    /// Record all failure messages for use after transaction.
    /// </summary>
    public class TransactionRecorder : IFailuresPreprocessor
    {
        public HashSet<FailureSeverity> AllSeverities = new HashSet<FailureSeverity>();
        public string FailureMessages = "";
        public FailureProcessingResult PreprocessFailures(
          FailuresAccessor a)
        {
            IList<FailureMessageAccessor> failures
              = a.GetFailureMessages();

            foreach (FailureMessageAccessor f in failures)
            {
                var severity = f.GetSeverity();
                this.AllSeverities.Add(severity);
                this.FailureMessages +=
                    severity.ToString() + " :"
                    + Environment.NewLine
                    + f.GetDescriptionText()
                    + Environment.NewLine;
            }
            return FailureProcessingResult.Continue;
        }
        public void UseInTransaction(Transaction _trans)
        {
            var opt = _trans.GetFailureHandlingOptions();
            opt.SetFailuresPreprocessor(this);
            _trans.SetFailureHandlingOptions(opt);
        }
    }
    #endregion

    #region goa UserValidation
    /// 活动目录辅助类。封装一系列活动目录操作相关的方法。
    public sealed class ADValidationCheck
    {
        /// 域名
        private static string DomainName = "goa.com.cn";

        /// LDAP 地址
        private static string LDAPDomain = "dc=goa,dc=com,dc=cn";

        /// LDAP绑定路径
        private static string ADPath = "LDAP://10.1.2.90:389";


        /// 根据用户公共名称取得用户的 对象
        /// 用户公共名称
        /// 如果找到该用户，则返回用户的对象；否则返回 null
        public static DirectoryEntry GetDirectoryEntryForCurrentUser()
        {
            string commonName = Environment.UserName;
            DirectoryEntry de = new DirectoryEntry(ADPath);

            DirectorySearcher deSearch = new DirectorySearcher(de);
            deSearch.PropertiesToLoad.AddRange(new string[] { "name", "Path", "displayname", "samaccountname", "mail" });

            deSearch.Filter = "(&(&(objectCategory=person)(objectClass=user))(samaccountname=" + commonName + "))";
            deSearch.SearchScope = SearchScope.Subtree;

            try
            {
                SearchResult result = deSearch.FindOne();
                de = new DirectoryEntry(result.Path);
                return de;

            }
            catch
            {
                return null;
            }

        }

    }
    #endregion

    #region UserMessages
    public static class UserMessages
    {
        public static string DefaultMessageCaption = "消息";
        public static string ErrorMessageTechnical(Exception ex)
        {
            string s = "--- Error Type ---\r\n" + ex.GetType().ToString()
                        + "\r\n--- Error Message ---\r\n" + ex.Message
                        + "\r\n--- Source ---\r\n" + ex.Source
                        + "\r\n--- TargetSite ---\r\n" + ex.TargetSite
                        + "\r\n--- StackTrace ---\r\n" + ex.StackTrace;
            return s;
        }
        public static void ShowMessage(string _message)
        {
            var newForm = new System.Windows.Forms.Form();//ensure messagebox will be on top
            newForm.TopMost = true;
            MessageBox.Show(newForm,
                _message,
                DefaultMessageCaption,
                MessageBoxButtons.OK,
                MessageBoxIcon.Information,
                MessageBoxDefaultButton.Button1);
            newForm.Dispose();
        }
        public static void ShowErrorMessage(Exception ex, System.Windows.Forms.Form _parent)
        {
            var emt = ErrorMessageTechnical(ex);
            var form = new Form_Error(emt);
            form.TopMost = true;
            if (_parent != null)
                form.ShowDialog(_parent);
            else
                form.ShowDialog();
        }
        public static void ShowErrorMessage(string _errorMessage, string _mainInstruction, System.Windows.Forms.Form _parent)
        {
            var form = new Form_Error(_mainInstruction, _errorMessage);
            form.TopMost = true;
            form.ShowDialog(_parent);
        }
        public static DialogResult ShowYesNoDialog(string message)
        {
            using (var form = new System.Windows.Forms.Form())
            {
                form.TopMost = true;
                var result = MessageBox.Show(message, DefaultMessageCaption, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                form.Dispose();
                return result;
            }
        }
    }
    #endregion

    #region Utility Classes
    /// <summary>
    /// Unjoin and re-join a list of walls.
    /// </summary>
    public class WallEndJoinCtrl
    {
        private Dictionary<ElementId, bool[]> original = new Dictionary<ElementId, bool[]>();
        private List<Wall> walls;

        public WallEndJoinCtrl(List<Wall> _walls)
        {
            this.walls = _walls;
            foreach (var w in _walls)
            {
                var j0 = WallUtils.IsWallJoinAllowedAtEnd(w, 0);
                var j1 = WallUtils.IsWallJoinAllowedAtEnd(w, 1);
                original[w.Id] = new bool[] { j0, j1 };
            }
        }

        public void UnjoinAll()
        {
            foreach (var w in this.walls)
            {
                WallUtils.DisallowWallJoinAtEnd(w, 0);
                WallUtils.DisallowWallJoinAtEnd(w, 1);
            }
        }

        public void Restore()
        {
            foreach (var w in this.walls)
            {
                if (w == null || w.IsValidObject == false)
                    continue;
                var values = this.original[w.Id];
                if (values[0])
                    WallUtils.AllowWallJoinAtEnd(w, 0);
                if (values[1])
                    WallUtils.AllowWallJoinAtEnd(w, 1);
            }
        }
    }

    /// <summary>
    /// Find and remember the join-relations among a list of elements,
    /// unjoin all, and retore later.
    /// </summary>
    public class JoinedGeometryCtrl
    {
        private Dictionary<ElementId, List<ElementId>> joinMap = new Dictionary<ElementId, List<ElementId>>();
        private List<Element> elements;

        public JoinedGeometryCtrl(List<Element> _elems)
        {
            this.elements = _elems;
            var count = _elems.Count;
            var doc = _elems.First().Document;
            for (int i = 0; i < count - 1; i++)
            {
                var e1 = _elems[i];
                for (int j = i + 1; j < count; j++)
                {
                    var e2 = _elems[j];
                    if (JoinGeometryUtils.AreElementsJoined(doc, e1, e2))
                    {
                        if (joinMap.ContainsKey(e1.Id) == false)
                        {
                            joinMap[e1.Id] = new List<ElementId>() { e2.Id };
                        }
                        else
                        {
                            joinMap[e1.Id].Add(e2.Id);
                        }
                    }
                }
            }
        }

        public void UnjoinAll()
        {
            var doc = this.elements.First().Document;
            foreach (var p in this.joinMap)
            {
                var e1 = doc.GetElement(p.Key);
                var ids = p.Value;
                foreach (var id in ids)
                {
                    var e2 = doc.GetElement(id);
                    JoinGeometryUtils.UnjoinGeometry(doc, e1, e2);
                }
            }
        }

        public void Restore()
        {
            var doc = this.elements.First().Document;
            foreach (var p in this.joinMap)
            {
                var e1 = doc.GetElement(p.Key);
                var ids = p.Value;
                foreach (var id in ids)
                {
                    var e2 = doc.GetElement(id);
                    try
                    {
                        JoinGeometryUtils.JoinGeometry(doc, e1, e2);
                    }
                    catch
                    {

                    }
                }
            }
        }
    }
    #endregion

    #region Dialog supress
    /// <summary>
    /// Suppress warnings that can be ignored, continue translaction
    /// without prompting user.
    /// Will not ignore serious errors.
    /// </summary>
    public class DialogSuppressor : IFailuresPreprocessor
    {
        public FailureProcessingResult PreprocessFailures
            (FailuresAccessor a)
        {
            IList<FailureMessageAccessor> failures
                = a.GetFailureMessages();

            bool canContinue = true;
            foreach (FailureMessageAccessor f in failures)
            {
                var severity = f.GetSeverity();
                if (severity == FailureSeverity.Error
                    || severity == FailureSeverity.DocumentCorruption)
                {
                    canContinue = false;
                    break;
                }
                else
                {
                    a.DeleteWarning(f);
                }
            }
            if (canContinue)
                return FailureProcessingResult.Continue;
            else
                return FailureProcessingResult.WaitForUserInput;
        }

        public void UseInTransaction(Transaction _trans)
        {
            var opt = _trans.GetFailureHandlingOptions();
            opt.SetFailuresPreprocessor(this);
            _trans.SetFailureHandlingOptions(opt);
        }
    }
    #endregion
}
