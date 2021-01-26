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
        public static string EnviCompHeightParamName = "窗顶到FL"; //OBSOLETE
        public static string GrooveFamilyLengthParamName = "切割长度"; //OBSOLETE

        //OBSOLETE
        public static List<Guid> FacadeCompLengthParam =
            new List<Guid>()
            {
                new Guid("df6f4df7-fc6f-4137-bf34-b1d0adafa2a4"), //长度 //OBSOLETE
                new Guid("2869b986-c80d-4b3c-843c-32034a8004d9"), //长度2 //OBSOLETE
                new Guid("634e4f00-a99d-4c7d-9841-b2d1f7602d31"), //长度3 //OBSOLETE
            };

        public static List<Guid> EnviFamParam =
            new List<Guid>()
            {
                new Guid("8ca79d78-0d1e-49b8-9d6a-098de3844b64"), //0,构件宽度(已废止)
                new Guid("b805339a-4d2f-4588-95d4-a672f12674ae"), //1,深度（阳台、设备平台）
                new Guid("2e8d2f0e-2633-4814-af23-0b5e12d03b62"), //2,侧面楼板进深（转角阳台）
                new Guid("6925630a-2488-4d51-86aa-0e1da29c07e0"), //3,左侧墙垛宽度
                new Guid("f2308ef4-7358-4abe-beab-57ebcf6eb207"), //4,右侧墙垛宽度
                new Guid("beee9ce6-220d-47c9-9c0b-0767ec4a0b86"), //5,转角连接 左
                new Guid("e15567df-ea07-4069-9bac-dd98d06ef35e"), //6,转角连接 右
                new Guid("c614682e-2a2d-4d07-8188-619193e42ee2"), //7,洞口宽度
                new Guid("ef0e9919-c77b-4fe8-97da-c620f6e40448"), //8,洞口高度（类型）
                new Guid("6b69cf42-7519-4d48-bc68-c92d0e0b890b"), //9,窗分隔 上（类型）
                new Guid("d57c18ba-191f-448d-9f4e-5e3a37214951"), //10, 窗分隔 下（类型）
                new Guid("58a7bcb0-cb63-4647-99ed-b1d56d23b193"), //11, 开启扇宽度（类型）
                new Guid("609ce7d5-b6c7-4f16-9fae-833fdb6467b3"), //12, 横向附框宽度（类型）
                new Guid("4e174055-9c22-4916-9be1-18876d1658ef"), //13, 纵向附框宽度（类型）
                new Guid("3adf993a-464e-4afd-8b49-408c278f1374"), //14, 窗分隔方式
                new Guid("beee9ce6-220d-47c9-9c0b-0767ec4a0b86"), //15, 转角连接 左
                new Guid("e15567df-ea07-4069-9bac-dd98d06ef35e"), //16, 转角连接 右
                new Guid("689ef905-4d1a-4870-b62e-e60962925a71"), //17, 窗到外墙面距离（类型）
            };

        public static Guid FacadeCompHeightParam = new Guid("15ac0566-5670-418d-a258-163ac7e39fc2"); //OBSOLETE
        public static Guid FacadeCompHeightParam2 = new Guid("bfb0015d-b9a0-4362-aaf9-c6f22170f851"); //OBSOLEE
        public static Guid EnviCompHeightParam = new Guid("1bd715a2-2c7e-4564-bdef-92780f0ba2c8"); //窗顶到FL
        public static Guid FamilyTypeIDForAPI = new Guid("679a258b-764c-481b-bd7d-0f543c298849");

        public static Dictionary<string, EnviFamilyType> EnviFamilyTypeID
            = new Dictionary<string, EnviFamilyType>()
            {
                //v1-v7, OBSOLETE
                {"ENVI-WINDOW", EnviFamilyType.Window },
                {"ENVI-BAYWINDOW", EnviFamilyType.BayWindow },
                {"ENVI-BALCONY", EnviFamilyType.Balcony },
                {"ENVI-BALCONY-L", EnviFamilyType.Balcony_L },
                {"ENVI-EQUIPMENT", EnviFamilyType.Equipment },
                //v8
                {"ENVI-WB-WINDOW-B", EnviFamilyType.wb_Window_B}, //窗B
                {"ENVI-WB-WINDOW-C", EnviFamilyType.wb_Window_C}, //窗C
                {"ENVI-WB-BAYWINDOW-B", EnviFamilyType.wb_BayWindow_B}, //飘窗B
                {"ENVI-WB-BAYWINDOW-C", EnviFamilyType.wb_BayWindow_C}, //飘窗C
                {"ENVI-WB-BALCDOOR", EnviFamilyType.wb_BalcDoor}, //阳台门
                {"ENVI-WB-EQUIPMENT", EnviFamilyType.wb_Equipment}, //设备平台百叶
                //{"ENVI-WB-CUSTOMWINDOW", EnviFamilyType.wb_CustomWindow}, //自定义窗
            };
        #endregion

        #region 立面构件
        public static List<Guid> SuperFacadeFamParam =
                new List<Guid>()
            {
                new Guid("15ac0566-5670-418d-a258-163ac7e39fc2"), //0,高度
                new Guid("166b1067-0b30-496f-a282-8f0977b61bc7"), //1,起点端平头
                new Guid("d24cd0f7-9c77-4874-ad31-6b0c05e9ba04"), //2,终点端平头
                new Guid("961acbe0-1290-49fa-a1e3-1f8a4fbfb3a2"), //3,起点端切换
                new Guid("dc483ab4-8328-4e66-ac3c-318804ca3db2"), //4,终点端切换
                new Guid("d55dbc24-b12f-4705-9f6e-799f5f50c26f"), //5,起点端三维
                new Guid("78f8c911-6fb8-4707-a81a-51209a49595e"), //6,终点端三维
                new Guid("97fa7302-a50f-4afd-9551-5c189f527fa6"), //7,均分
                new Guid("808883e0-bc45-485a-8e86-c2d58b94169e"), //8,输入个数
                new Guid("aba6b675-882f-43a1-9e85-4f02f7e99f52"), //9,输入间距
                new Guid("7e1b937d-91db-4a5f-bc35-2cffa786acda"), //10,起点偏移
                new Guid("3a493107-3355-4f58-88e3-942abad8a50a"), //11,主体偏移（类型）
                new Guid("8eac5912-3a20-483c-8577-d44859091fda"), //12,起点端延长
                new Guid("2a3e6c2e-0cf5-4ab2-9c49-eaabc5803bc6"), //13,终点端延长
                new Guid("75fe6a6a-c1e9-4b12-81f0-7d26993ff40e"), //14,首尾可见
            };
        public static Dictionary<string, FacadeFamilyType> FacadeFamilyTypeID
            = new Dictionary<string, FacadeFamilyType>()
            {
                //v1 OBSOLETE
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
                //V2
                {"FACADE-S-DECO", FacadeFamilyType.SuperCornice }, //超级线脚
                {"FACADE-S-SKIN", FacadeFamilyType.SuperSkin }, //超级表皮
                {"FACADE-ARRAY-ONFACE", FacadeFamilyType.ArrayOnFace },//基于线阵列-垂直面
                {"FACADE-ARRAY-ONLEVEL", FacadeFamilyType.ArrayOnLevel },//基于线阵列-楼层
                {"VOID_FACEBASED", FacadeFamilyType.VoidFaceBased }, //基于面开洞

                //{"FACADE-S-REVEAL", FacadeFamilyType.SuperReveal }, //超级开槽-矩形截面 OBSOLETE
                //{"FACADE-S-REVEAL-COMP", FacadeFamilyType.SuperRevealComp }, //超级开槽-复杂截面 OBSOLETE
                //{"FACADE-ARRAY-REVEAL", FacadeFamilyType.RevealArray_fb }, //开槽阵列-矩形截面 OBSOLETE
                //{"FACADE-ARRAY-MULLION", FacadeFamilyType.MullionArray_fb },//开槽阵列-竖铤 OBSOLETE
                //{"FACADE-ARRAY-SEAM", FacadeFamilyType.SeamArray_fb },//开槽阵列-分缝 OBSOLETE
            };

        public readonly static HashSet<string> ParametersNeedSync
                = new HashSet<string>(new List<string>()
            {
                    SuperFacadeFamParam[0].ToString(), //高度
                    SuperFacadeFamParam[1].ToString(), //起点端平头
                    SuperFacadeFamParam[2].ToString(), //终点端平头
                    SuperFacadeFamParam[3].ToString(), //起点端切换
                    SuperFacadeFamParam[4].ToString(), //终点端切换
                    SuperFacadeFamParam[5].ToString(), //起点端三维
                    SuperFacadeFamParam[6].ToString(), //终点端三维
                    SuperFacadeFamParam[7].ToString(), //均分
                    SuperFacadeFamParam[8].ToString(), //输入个数
                    SuperFacadeFamParam[9].ToString(), //输入间距
                    SuperFacadeFamParam[10].ToString(),//起点偏移
                    SuperFacadeFamParam[12].ToString(),//起点端延长
                    SuperFacadeFamParam[13].ToString(),//终点端延长
                    SuperFacadeFamParam[14].ToString(),//首尾可见
                    EnviFamParam[7].ToString(), //洞口宽度
                    EnviFamParam[8].ToString(), //洞口高度
                    EnviFamParam[14].ToString(), //窗分隔方式
                    EnviFamParam[15].ToString(), //转角连接 左
                    EnviFamParam[16].ToString(), //转角连接 右

            });
        #endregion

        #region 总图
        public static Dictionary<string, SitePlanFamilyType> SitePlanFamilyTypeID
            = new Dictionary<string, SitePlanFamilyType>()
            {
                {"SITE-HIGHRISE", SitePlanFamilyType.Block_Highrise }, //高层
                {"SITE-STACK", SitePlanFamilyType.Block_Stack }, //叠拼
                {"SITE-CALCPOINT", SitePlanFamilyType.CalcPoint }, //计算点
                {"SITE-SUNPATH", SitePlanFamilyType.SunPath }, //棒影图
                {"SITE-CODE-DISTANCE", SitePlanFamilyType.DistanceCheck }, //间距控制
            };

        public static List<Guid> SitePlanFamilyParam = new List<Guid>()
        {
                new Guid("111e1c2e-5bbe-4ad6-b493-cfd669fd9901"), //0, 总图体块-总建筑面积
                new Guid("8e371b1f-313e-4143-899f-30d59c28e25c"), //1, 总图体块-层数
                new Guid("e88dbee5-7ce4-46be-8f1f-e32699c18e1e"), //2, 住宅体块-体块族类型
        };

        public static bool IsSiteBlockFamily(Element _elem)
        {
            if (_elem is FamilyInstance == false)
                return false;
            var fi = _elem as FamilyInstance;
            var type = SitePlanFamilyType.Block_Highrise;
            return IsSiteBlockFamilySymbol(fi.Symbol, ref type);
        }
        public static bool IsSiteBlockFamily(Element _elem, ref SitePlanFamilyType _type)
        {
            if (_elem is FamilyInstance == false)
                return false;
            var fi = _elem as FamilyInstance;
            return IsSiteBlockFamilySymbol(fi.Symbol, ref _type);
        }
        public static bool IsSiteBlockFamilySymbol(FamilySymbol _fs, ref SitePlanFamilyType _type)
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
        public static SitePlanFamilyType GetSitePlanFamilyType(Element _elem)
        {
            if (_elem is FamilyInstance == false)
                return SitePlanFamilyType.INVALID;
            var fi = _elem as FamilyInstance;
            return GetSitePlanFamilyType(fi.Symbol);
        }
        public static SitePlanFamilyType GetSitePlanFamilyType(FamilySymbol _fs)
        {
            var p = _fs.get_Parameter(FamilyTypeIDForAPI);
            if (p == null)
                return SitePlanFamilyType.INVALID;
            var typeString = p.AsString();
            if (SitePlanFamilyTypeID.ContainsKey(typeString) == false)
                return SitePlanFamilyType.INVALID;
            else
                return SitePlanFamilyTypeID[typeString];
        }
        public static FacadeFamilyType GetFacadeFamilyType(this FamilyInstance _fi)
        {
            var fs = _fi.Symbol;
            return fs.GetFacadeFamilyType();
        }
        public static FacadeFamilyType GetFacadeFamilyType(this FamilySymbol fs)
        {
            var p = fs.get_Parameter(FamilyTypeIDForAPI);
            if (p == null)
                return FacadeFamilyType.INVALID;
            var key = p.AsString();
            if (key == null
                || FacadeFamilyTypeID.ContainsKey(key) == false)
                return FacadeFamilyType.INVALID;
            return FacadeFamilyTypeID[key];
        }
        public static EnviFamilyType GetEnviFamilyType(this FamilyInstance _fi)
        {
            var fs = _fi.Symbol;
            return fs.GetEnviFamilyType();
        }
        public static EnviFamilyType GetEnviFamilyType(this FamilySymbol fs)
        {
            var p = fs.get_Parameter(FamilyTypeIDForAPI);
            if (p == null)
                return EnviFamilyType.INVALID;
            var key = p.AsString();
            if (key == null
                || EnviFamilyTypeID.ContainsKey(key) == false)
                return EnviFamilyType.INVALID;
            return EnviFamilyTypeID[key];
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
                {"SPACE-MARK",  OtherFamilyType.SpaceMark}, //使用面积标记
                 { "VOID_FACEBASED", OtherFamilyType.VoidFaceBased } //基于面的空心
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
        DistanceCheck,
        INVALID,
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
        //V1 - V7
        Window,
        BayWindow,
        Balcony,
        Balcony_L,
        Equipment,
        //V8
        wb_Window_B,
        wb_Window_C,
        wb_BayWindow_B,
        wb_BayWindow_C,
        wb_BalcDoor,
        wb_Equipment,
        wb_CustomWindow,
        INVALID,
    }
    public enum FacadeFamilyType
    {
        //old facade families
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
        //line-based super series
        SuperCornice,
        SuperSkin,
        SuperReveal,
        SuperRevealComp,

        //face-based array OBSOLETE
        MullionArray_fb,
        RevealArray_fb,
        SeamArray_fb,

        //line-based array
        ArrayOnFace,
        ArrayOnLevel,

        //void 
        VoidFaceBased,

        INVALID,
    }
    public enum OtherFamilyType
    {
        Cutter,
        SpaceMark,
        VoidFaceBased,
    }
    #endregion
}
