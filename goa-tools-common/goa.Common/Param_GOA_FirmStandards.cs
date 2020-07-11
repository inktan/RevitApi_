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
        public static string InvalidFileNameCharacters = @"\<>|:*?/";

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

        #region 围护构件 立面构件
        public static string EnviCompHeightParamName = "窗顶到FL";
        public static string GrooveFamilyLengthParamName = "切割长度";

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
                new Guid("b805339a-4d2f-4588-95d4-a672f12674ae"), //深度（阳台、设备平台）
            };

        public static Guid FacadeCompHeightParam = new Guid("15ac0566-5670-418d-a258-163ac7e39fc2");
        public static Guid FacadeCompHeightParam2 = new Guid("bfb0015d-b9a0-4362-aaf9-c6f22170f851");
        public static Guid EnviCompHeightParam = new Guid("1bd715a2-2c7e-4564-bdef-92780f0ba2c8"); //窗顶到FL
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
                {"FACADE-V-U-UP", FacadeFamilyType.Vertical_U_Up }, //垂直U上
                {"FACADE-V-U-DN", FacadeFamilyType.Vertical_U_Down }, //垂直U下
                {"FACADE-3D-L-UP", FacadeFamilyType.Spatial_L_Up }, //三维L上
                {"FACADE-3D-L-DN", FacadeFamilyType.Spatial_L_Down }, //三维L下
                {"FACADE-3D-C", FacadeFamilyType.Spatial_C }, //三维C形
            };

        public static Dictionary<string, EnviFamilyType> EnviFamilyTypeID
            = new Dictionary<string, EnviFamilyType>()
            {
                {"ENVI-WINDOW", EnviFamilyType.Window },
                {"ENVI-BAYWINDOW", EnviFamilyType.BayWindow },
                {"ENVI-BALCONY", EnviFamilyType.Balcony },
                {"ENVI-EQUIPMENT", EnviFamilyType.Equipment },
            };
        #endregion

        #region 总图
        public static Dictionary<string, SitePlanFamilyType> SitePlanFamilyTypeID
            = new Dictionary<string, SitePlanFamilyType>()
            {
                            {"SITE-HIGHRISE", SitePlanFamilyType.Block_Highrise }, //高层
                            {"SITE-STACK", SitePlanFamilyType.Block_Stack }, //叠拼
                            {"SITE-CALCPOINT", SitePlanFamilyType.CalcPoint }, //计算点
                            {"SITE-SUNPATH", SitePlanFamilyType.SunPath } //棒影图
            };
        public static Guid SitePlanFamilyAreaParam = new Guid("111e1c2e-5bbe-4ad6-b493-cfd669fd9901"); //总图体块-总建筑面积
        public static Guid SitePlanFamilyLevelParam = new Guid("8e371b1f-313e-4143-899f-30d59c28e25c"); //总图体块-层数
        public static Guid SitePlanFamilyDepthParam = new Guid("01efcb88-509b-4942-9717-1f073d0915ae"); //总图体块-进深
        public static Guid SitePlanFamilyWidthParam = new Guid("52a0b75a-49df-4a63-b387-2ce8e592f83b"); //总图体块-面宽

        public static bool IsSitePlanFamily(Element _elem)
        {
            if (_elem is FamilyInstance == false)
                return false;
            var fi = _elem as FamilyInstance;
            var type = SitePlanFamilyType.Block_Highrise;
            return IsSitePlanFamilySymbol(fi.Symbol, ref type);
        }
        public static bool IsSitePlanFamily(Element _elem, ref SitePlanFamilyType _type)
        {
            if (_elem is FamilyInstance == false)
                return false;
            var fi = _elem as FamilyInstance;
            return IsSitePlanFamilySymbol(fi.Symbol, ref _type);
        }
        public static bool IsSitePlanFamilySymbol(FamilySymbol _fs, ref SitePlanFamilyType _type)
        {
            var p = _fs.get_Parameter(FamilyTypeIDForAPI);
            if (p == null)
                return false;
            var typeString = p.AsString();
            if (SitePlanFamilyTypeID.ContainsKey(typeString) == false)
                return false;
            _type = SitePlanFamilyTypeID[typeString];
            return
                _type == SitePlanFamilyType.Block_Highrise
                || _type == SitePlanFamilyType.Block_Stack;
        }
        #endregion

        #region 地下室
        public static Dictionary<string, BasementFamilyType> BasementFamilytypeID
            = new Dictionary<string, BasementFamilyType>()
            {
                {"BASEMENT-TRANSRADIUS", BasementFamilyType.TransformerRadious }, //公变所服务半径
                {"BASEMENT-TRANSREGION", BasementFamilyType.TransformerServiceRegionTag }, //公变所服务区域标签
                {"BASEMENT-PARKINGSPACE", BasementFamilyType.ParkingSpace }, //停车位
                {"BASEMENT-RAMP", BasementFamilyType.Ramp }, //坡道
                {"BASEMENT-RAMP-STACKED", BasementFamilyType.RampStacked }, //重叠坡道
            };
        public static Guid TransformerRadiousParam_vila = new Guid("1817ccd9-e8c4-4075-a56a-dcc3d7fee750"); //服务半径-别墅
        public static Guid TransformerRadiousParam_highrise = new Guid("bf84337c-3913-4724-8e74-ecd4de55d905"); //服务半径-大高
        public static Guid TransformerRadiousparam_midHighrise = new Guid("b2f013f1-a759-415d-b4c7-39c36db835fb"); //服务半径-小高
        public static Guid TransformerRadiousparam_flat = new Guid("58f5b438-971d-45e8-8483-20e8946c358e"); //服务半径-洋房
        public static Guid TransformerServiceArea_vila = new Guid("85d0518e-60cc-43c5-bfe5-de33cbb979e3"); //服务面积_别墅
        public static Guid TransformerServiceArea_flat = new Guid("4c73614d-c48a-48ed-8cec-0dde111678bd"); //服务面积_洋房
        public static Guid TransformerServiceArea_midHigh = new Guid("c8eb73c4-7989-48ae-afd2-494da0887fbe"); //服务面积_小高
        public static Guid TransformerServiceArea_high = new Guid("97b479c8-7889-44b0-bf93-cc6750d24408"); //服务面积_大高
        #endregion

        #region 其它族和参数
        public static Dictionary<string, OtherFamilyType> OtherFamilytypeID
             = new Dictionary<string, OtherFamilyType>()
        {
            {"OTHER-CUTTER", OtherFamilyType.Cutter }, //剪切形体族
        };
        #endregion
    }
    #region family type ids
    public enum SitePlanFamilyType
    {
        Block_Highrise,
        Block_Stack,
        CalcPoint,
        SunPath,
    }
    public enum BasementFamilyType
    {
        ParkingSpace,
        TransformerRadious,
        TransformerServiceRegionTag,
        Ramp,
        RampStacked,
    }
    public enum EnviFamilyType
    {
        Window,
        BayWindow,
        Balcony,
        Equipment,
        INVALID,
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
    public enum OtherFamilyType
    {
        Cutter,
    }
    #endregion
}
