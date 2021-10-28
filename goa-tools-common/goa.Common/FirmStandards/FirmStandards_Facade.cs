using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.Revit.DB;

namespace goa.Common
{
    public static partial class FirmStandards
    {
        public static string GrooveFamilyLengthParamName = "切割长度"; //OBSOLETE

        //OBSOLETE
        public static List<Guid> FacadeCompLengthParam =
            new List<Guid>()
            {
                new Guid("df6f4df7-fc6f-4137-bf34-b1d0adafa2a4"), //长度 //OBSOLETE
                new Guid("2869b986-c80d-4b3c-843c-32034a8004d9"), //长度2 //OBSOLETE
                new Guid("634e4f00-a99d-4c7d-9841-b2d1f7602d31"), //长度3 //OBSOLETE
            };

        public static Guid FacadeCompHeightParam = new Guid("15ac0566-5670-418d-a258-163ac7e39fc2"); //OBSOLETE
        public static Guid FacadeCompHeightParam2 = new Guid("bfb0015d-b9a0-4362-aaf9-c6f22170f851"); //OBSOLETE

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
                new Guid("97fa7302-a50f-4afd-9551-5c189f527fa6"), //7,均分 (7至16已失效)
                new Guid("808883e0-bc45-485a-8e86-c2d58b94169e"), //8,输入个数
                new Guid("aba6b675-882f-43a1-9e85-4f02f7e99f52"), //9,输入间距
                new Guid("7e1b937d-91db-4a5f-bc35-2cffa786acda"), //10,起点偏移
                new Guid("3a493107-3355-4f58-88e3-942abad8a50a"), //11,主体偏移（类型）
                new Guid("8eac5912-3a20-483c-8577-d44859091fda"), //12,起点端延长
                new Guid("2a3e6c2e-0cf5-4ab2-9c49-eaabc5803bc6"), //13,终点端延长
                new Guid("75fe6a6a-c1e9-4b12-81f0-7d26993ff40e"), //14,首尾可见
                new Guid("7cf72140-5f9e-4988-ae4b-520b4d1b884d"), //15,起点端角度
                new Guid("25b8580e-0672-4b9e-b086-446c74e59328"), //16,终点端角度
            };
        public static List<Guid> FacadeFamV3Param =
            new List<Guid>()
            {
                new Guid("4fa42ea6-a6aa-431c-894c-cfda221c99da"),    //0,构件长度
                new Guid("7ed3d50f-4bdd-4b29-9227-ef60e52443d5"),    //1,构件角度
                new Guid("a1f8b01b-7ae8-4b6f-a9af-2ff6f103ef80"),    //2,平面剪切S
                new Guid("22cfd207-e7a7-4b33-b4fd-2557da448e0b"),    //3,平面剪切E
                new Guid("ab8561bf-6c7a-4b23-af2d-798e811fa8c5"),    //4,三维剪切S
                new Guid("e733ee55-1320-4e59-972e-da77ce93aeee"),    //5,三维剪切E
                new Guid("7d69421d-1b60-4a97-b700-39df2091274a"),    //6,轮廓x边缘1剪切S
                new Guid("da33aeff-fe44-4cab-8d3a-cfbbc27dec7e"),    //7,轮廓x边缘1剪切E
                new Guid("e1833a50-dad1-4fe2-a835-416b4cb77341"),    //8,轮廓x边缘2剪切S
                new Guid("de4fa009-071f-4b54-873f-e1176c7e286d"),    //9,轮廓x边缘2剪切E
                new Guid("dd0e5183-434f-43da-b90f-42d5dca5930b"),    //10,轮廓y边缘1剪切S
                new Guid("ef3f29f0-39f0-4bd8-924d-0945d169e482"),    //11,轮廓y边缘1剪切E
                new Guid("a159a56c-0bbc-46b1-84c1-635352ac558e"),    //12,轮廓y边缘2剪切S
                new Guid("43867236-11e3-4188-b38c-9ccf3e00e4ab"),    //13,轮廓y边缘2剪切E
                new Guid("3db8f2a3-14e8-4f2b-a8e5-a179dc00c24c"),    //14,profXEdge1cutCalcS
                new Guid("f0e47488-995f-4852-931a-fb51df951231"),    //15,profXEdge1cutCalcE
                new Guid("a02c937b-dfcf-4a59-94ec-73265b936076"),    //16,profXEdge2cutCalcS
                new Guid("d188729d-5cde-49e4-b75d-8186bf0559d9"),    //17,profXEdge2cutCalcE
                new Guid("3629d814-24c5-45cc-af74-1894568de98c"),    //18,profYEdge1cutCalcS
                new Guid("3bcfcbb5-0d58-46ad-8c28-730f02cbd0d3"),    //19,profYEdge1cutCalcE
                new Guid("6f1f7f46-0143-4c75-9be0-9985555edf68"),    //20,profYEdge2cutCalcS
                new Guid("5578ea9f-4c8a-423a-9247-837d47c86344"),    //21,profYEdge2cutCalcE    
                new Guid("35aa16c1-4485-4be6-a2fd-9f4e81574a46"),   //22,阵列方向切换
                new Guid("8e02b381-3d43-4dd6-bb0e-88bccc1ebefb"),   //23,构件宽度
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
                //V2 OBSOLETE
                {"FACADE-S-DECO", FacadeFamilyType.SuperCornice }, //超级线脚
                {"FACADE-S-SKIN", FacadeFamilyType.SuperSkin }, //超级表皮
                {"FACADE-ARRAY-ONFACE", FacadeFamilyType.ArrayOnFace },//基于线阵列-垂直面
                {"FACADE-ARRAY-ONLEVEL", FacadeFamilyType.ArrayOnLevel },//基于线阵列-楼层
                {"VOID_FACEBASED", FacadeFamilyType.VoidFaceBased }, //基于面开洞
                {"VOID_LINEBASED", FacadeFamilyType.VoidLineBased }, //基于线空心
                //V3
                {"FACADE-V3-CORNICE", FacadeFamilyType.CorniceV3}, //线脚v3
                {"FACADE-V3-CORNICE-HOST", FacadeFamilyType.CorniceV3OnHost}, //线脚v3基于面
                {"FACADE-V3-SKIN", FacadeFamilyType.SkinV3 }, //表皮v3
                {"FACADE-V3-SKIN-MULTI",  FacadeFamilyType.Skinv3Multi}, //表皮v3多功能
                {"FACADE-V3-ARRAY", FacadeFamilyType.ArrayV3 }, //阵列v3
                {"FACADE-V3-VOID", FacadeFamilyType.VoidV3 }, //开洞v3
                {"FACADE-V3-FRAMING", FacadeFamilyType.FramingV3 }, //框v3

                //{"FACADE-S-REVEAL", FacadeFamilyType.SuperReveal }, //超级开槽-矩形截面 OBSOLETE
                //{"FACADE-S-REVEAL-COMP", FacadeFamilyType.SuperRevealComp }, //超级开槽-复杂截面 OBSOLETE
                //{"FACADE-ARRAY-REVEAL", FacadeFamilyType.RevealArray_fb }, //开槽阵列-矩形截面 OBSOLETE
                //{"FACADE-ARRAY-MULLION", FacadeFamilyType.MullionArray_fb },//开槽阵列-竖铤 OBSOLETE
                //{"FACADE-ARRAY-SEAM", FacadeFamilyType.SeamArray_fb },//开槽阵列-分缝 OBSOLETE

                //customized families
                {"FACADE-V3-CUSTOMIZED", FacadeFamilyType.Customized  }, //customized families
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
                    SuperFacadeFamParam[15].ToString(),//起点端角度
                    SuperFacadeFamParam[16].ToString(),//终点端角度

                    //FacadeFamV3Param[0].ToString(),//构件长度
                    FacadeFamV3Param[1].ToString(),//构件角度
                    FacadeFamV3Param[2].ToString(),//成角剪切参数系列
                    FacadeFamV3Param[3].ToString(),
                    FacadeFamV3Param[4].ToString(),
                    FacadeFamV3Param[5].ToString(),
                    FacadeFamV3Param[6].ToString(),
                    FacadeFamV3Param[7].ToString(),
                    FacadeFamV3Param[8].ToString(),
                    FacadeFamV3Param[9].ToString(),
                    FacadeFamV3Param[10].ToString(),
                    FacadeFamV3Param[11].ToString(),
                    FacadeFamV3Param[12].ToString(),
                    FacadeFamV3Param[13].ToString(),
                    FacadeFamV3Param[14].ToString(),
                    FacadeFamV3Param[15].ToString(),
                    FacadeFamV3Param[16].ToString(),
                    FacadeFamV3Param[17].ToString(),
                    FacadeFamV3Param[18].ToString(),
                    FacadeFamV3Param[19].ToString(),
                    FacadeFamV3Param[20].ToString(),
                    FacadeFamV3Param[21].ToString(),
                    FacadeFamV3Param[22].ToString(),//阵列方向切换
                    FacadeFamV3Param[23].ToString(),//构件宽度
                    "上部线脚剪切",
                    "下部线脚剪切",
                    "左侧线脚剪切",
                    "右侧线脚剪切",

                    //some family built-in parameters
                    //((int)BuiltInParameter.FAMILY_BASE_LEVEL_PARAM).ToString(),
                    //((int)BuiltInParameter.FAMILY_BASE_LEVEL_OFFSET_PARAM).ToString(),
                    //((int)BuiltInParameter.FAMILY_TOP_LEVEL_PARAM).ToString(),
                    //((int)BuiltInParameter.FAMILY_TOP_LEVEL_OFFSET_PARAM).ToString(),

                    EnviFamParam[7].ToString(), //洞口宽度
                    EnviFamParam[8].ToString(), //洞口高度
                    EnviFamParam[14].ToString(), //窗分隔方式
                    EnviFamParam[15].ToString(), //转角连接 左
                    EnviFamParam[16].ToString(), //转角连接 右
            });

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
    }
    
    public enum FacadeFamilyType
    {
        //old facade families (v1) OBSOLETE
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

        //line-based super series (v2) OBSOLETE
        SuperCornice,
        SuperSkin,
        SuperReveal,
        SuperRevealComp,

        //face-based array OBSOLETE
        MullionArray_fb,
        RevealArray_fb,
        SeamArray_fb,

        //line-based array (v2) OBSOLETE
        ArrayOnFace,
        ArrayOnLevel,

        //void (v2) OBSOLETE
        VoidFaceBased,
        VoidLineBased,

        //facade comp v3
        CorniceV3,
        CorniceV3OnHost,
        SkinV3,
        Skinv3Multi,
        ArrayV3,
        VoidV3,
        FramingV3,

        //customized family
        Customized,

        INVALID,
    }    
}
