using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.Revit.DB;

namespace goa.Common
{
    public static class FirmStandards
    {
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

        public static List<Guid> FacadeCompLengthParam =
            new List<Guid>()
            {
                        new Guid("df6f4df7-fc6f-4137-bf34-b1d0adafa2a4"), //长度
                        new Guid("2869b986-c80d-4b3c-843c-32034a8004d9"), //长度2
                        new Guid("634e4f00-a99d-4c7d-9841-b2d1f7602d31"), //长度3
            };

        public static List<Guid> EnviCompLengthParam =
            new List<Guid>()
            {
                new Guid("8ca79d78-0d1e-49b8-9d6a-098de3844b64"), //构件宽度
                new Guid("e89df7ba-3726-413c-8876-d605424c4ee9"), //侧边宽度
            };

        public static Guid FacadeCompHeightParam = new Guid("15ac0566-5670-418d-a258-163ac7e39fc2");
        public static Guid FacadeCompHeightParam2 = new Guid("bfb0015d-b9a0-4362-aaf9-c6f22170f851");
        public static Guid EnviCompHeightParam = new Guid("372e3126-f59f-48c7-9081-b2ff4cc9e530");
        public static Guid FamilyTypeIDForAPI = new Guid("679a258b-764c-481b-bd7d-0f543c298849");

        public static Dictionary<string, FacadeFamilyType> FacadeFamilyTypeID
            = new Dictionary<string, FacadeFamilyType>()
            {
                {"FACADE-H-LINE", FacadeFamilyType.Horizontal_Line }, //水平直线
                {"FACADE-H-L", FacadeFamilyType.Horizontal_L }, //水平L
                {"FACADE-H-U", FacadeFamilyType.Horizontal_U }, //水平U
                {"FACADE-V-LINE", FacadeFamilyType.Vertical_Line}, //垂直直线
                {"FACADE-V-L", FacadeFamilyType.Vertical_L }, //垂直L
                {"FACADE-V-REC", FacadeFamilyType.Vertical_Rec }, //垂直矩形
                {"FACADE-V-U-DN", FacadeFamilyType.Vertical_U_Down }, //垂直U上
                {"FACADE-V-U-UP", FacadeFamilyType.Vertical_U_Up }, //垂直U下
                {"FACADE-3D-L-UP", FacadeFamilyType.Spatial_L_Up }, //三维L上
                {"FACADE-3D-L-DN", FacadeFamilyType.Spatial_L_Down }, //三维L下
                {"FACADE-3D-C", FacadeFamilyType.Spatial_C }, //三维C形
            };

        public static Dictionary<string, EnviFamilyType> EnviFamilyTypeID
            = new Dictionary<string, EnviFamilyType>()
            {
                {"ENVI-WINDOW", EnviFamilyType.Window }, //窗
                {"ENVI-BAYWINDOW", EnviFamilyType.BayWindow }, //飘窗
                {"ENVI-BALCONY", EnviFamilyType.Balcony }, //阳台
                {"ENVI-BALCONY-L", EnviFamilyType.Balcony_L }, //转角阳台
                {"ENVI-BAYWINDOW-L", EnviFamilyType.BayWindow_L }, //转角飘窗
                {"ENVI-EQUIPMENT", EnviFamilyType.Equipment }, //设备平台
            };
    }

    public enum EnviFamilyType
    {
        Window,
        BayWindow,
        Balcony,
        BayWindow_L,
        Balcony_L,
        Equipment,
    }

    public enum FacadeFamilyType
    {
        Horizontal_Line,
        Horizontal_L,
        Horizontal_U,
        Vertical_Line,
        Vertical_L,
        Vertical_Rec,
        Vertical_U_Down,
        Vertical_U_Up,
        Spatial_L_Down,
        Spatial_L_Up,
        Spatial_C,
        INVALID,
    }

}
