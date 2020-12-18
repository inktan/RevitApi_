using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.Revit.DB;
using System.DirectoryServices;
using Autodesk.Revit.UI.Selection;

namespace goa.Common
{
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
            if (elem is Panel)
                return true;
            else
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
        private static string ADPath = "LDAP://10.1.2.2:389";


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
}
