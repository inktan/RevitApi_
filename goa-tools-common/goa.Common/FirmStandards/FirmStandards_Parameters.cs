using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace goa.Common
{
    public static partial class FirmStandards
    {
        #region Sheet Titleblock Parameters
        public static readonly Dictionary<string, string> SheetParameterNameMap =
                new Dictionary<string, string>()
        {
                { "工程名称", "BORD-01.Project"},
                { "工程编号", "BORD-02.Pro NO."},
                {"建设单位", "BORD-03.Owner" },
                {"子项名称", "BORD-04.Subtitle" },
                {"子项编号", "BORD-05.Subtitle NO." },
                {"图别", "BORD-06.Category" },
                {"审定", "BORD-07.Authorized" },
                {"审核", "BORD-08.Reviewed" },
                {"工程负责", "BORD-09.Project Director" },
                {"专业负责", "BORD-10.Discipline Responsible" },
                {"校对", "BORD-11.Checked" },
                {"设计", "BORD-12.Designed" },
                {"绘图", "BORD-13.Drawn" },
                {"序号", "BORD-14.Serial NO." },
                {"比例", "BORD-15.Scale" },
                {"图幅", "BORD-16.Size" },
                {"版本", "BORD-17.Version" },
                {"说明", "BORD-18.Description" },
        };

        public static readonly Dictionary<string, BuiltInParameter> SheetBuiltInParameterMap =
            new Dictionary<string, BuiltInParameter>()
            {
                { "图名", BuiltInParameter.SHEET_NAME},
                { "图号", BuiltInParameter.SHEET_NUMBER},
            };
        #endregion


    }
}
