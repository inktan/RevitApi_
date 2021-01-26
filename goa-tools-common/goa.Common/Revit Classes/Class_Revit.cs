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
            if (_parent != null && _parent.IsDisposed == false)
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

        public int Count { get { return this.walls.Count; } }

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

    /// <summary>
    /// Store editing information for individual element,
    /// for commit transaltion later.
    /// </summary>
    public class ElementEditRecorder
    {
        public Element Elem;
        public ElementId ElemType = ElementId.InvalidElementId;
        public XYZ LocationPoint; //for record
        public XYZ Translation;
        public Curve LocationCurve;
        public Curve NewLocationCurve;
        public Dictionary<string, ParameterEditRecorder> ParametersToChange
                = new Dictionary<string, ParameterEditRecorder>();
        public XYZ HandOrientation;
        public XYZ FacingOrientation;

        public ElementEditRecorder(Element _elem)
        {
            this.Elem = _elem;
            if (this.Elem.Location is LocationCurve)
            {
                this.NewLocationCurve = this.Elem.LocationCurve();
            }
            if (_elem is FamilyInstance)
            {
                var fi = _elem as FamilyInstance;
                this.HandOrientation = fi.HandOrientation;
                this.FacingOrientation = fi.FacingOrientation;
            }
        }

        public void ApplyChange()
        {
            if (this.NewLocationCurve != null)
            {
                var lc = this.Elem.Location as LocationCurve;
                lc.Curve = this.NewLocationCurve;
            }
            if (this.Translation != null)
                ElementTransformUtils.MoveElement(this.Elem.Document, this.Elem.Id, this.Translation);
            foreach (var pcm in this.ParametersToChange.Values)
                pcm.ApplyChange();
            if (this.Elem is FamilyInstance)
            {
                var fi = this.Elem as FamilyInstance;
                if (fi.HandOrientation.IsAlmostEqualToByDifference(this.HandOrientation, 0.0001) == false)
                    fi.flipHand();
                if (fi.FacingOrientation.IsAlmostEqualToByDifference(this.FacingOrientation, 0.0001) == false)
                    fi.flipFacing();
            }
            if (this.ElemType != ElementId.InvalidElementId)
            {
                var typeP = this.Elem.get_Parameter(BuiltInParameter.ELEM_TYPE_PARAM);
                typeP.Set(this.ElemType);
            }
        }

        public override bool Equals(object obj)
        {
            if (obj is Element)
            {
                var elem = obj as Element;
                return this.Elem.Id == elem.Id;
            }
            else
            {
                return false;
            }
        }
    }

    /// <summary>
    /// Store new value of a parameter to commit in transaction later.
    /// </summary>
    public class ParameterEditRecorder
    {
        public Parameter Param;
        public string Key;
        public StorageType StorageType;
        public object Value;

        public ParameterEditRecorder(Parameter _p, object _value)
        {
            this.Param = _p;
            this.Key = _p.GetId();
            this.Value = _value;
            this.StorageType = _p.StorageType;
        }

        public ParameterEditRecorder(string _key, StorageType _type, object _value)
        {
            this.Key = _key;
            this.StorageType = _type;
            this.Value = _value;
        }
        public ParameterEditRecorder(string _key, double _value)
        {
            this.Key = _key;
            this.StorageType = StorageType.Double;
            this.Value = _value;
        }

        public void ApplyChange()
        {
            this.Param.SetValue(this.Value);
        }
        public void ApplyChange(Element elem)
        {
            var param = elem.GetParameterById(this.Key);
            param.SetValue(this.Value);
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
        public List<ElementId> DetachedIds = new List<ElementId>();
        public FailureProcessingResult PreprocessFailures
            (FailuresAccessor a)
        {
            IList<FailureMessageAccessor> failures
                = a.GetFailureMessages();

            FailureProcessingResult result = FailureProcessingResult.Continue;

            foreach (FailureMessageAccessor f in failures)
            {
                string s = f.GetDescriptionText();
                var severity = f.GetSeverity();
                //auto resolve detach error
                if (severity == FailureSeverity.Error
                    && f.HasResolutionOfType(FailureResolutionType.DetachElements))
                {
                    DetachedIds.AddRange(f.GetFailingElementIds());
                    f.SetCurrentResolutionType(FailureResolutionType.DetachElements);
                    a.ResolveFailure(f);
                    result = FailureProcessingResult.ProceedWithCommit;
                }
                //serious errors: waiting for user input
                else if (severity == FailureSeverity.Error
                    || severity == FailureSeverity.DocumentCorruption)
                {
                    result = FailureProcessingResult.WaitForUserInput;
                    break;
                }
                //ignore all warnings
                else
                {
                    a.DeleteWarning(f);
                }
            }
            return result;
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
