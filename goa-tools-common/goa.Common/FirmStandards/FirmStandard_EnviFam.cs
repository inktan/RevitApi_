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
        public static string EnviCompHeightParamName = "窗顶到FL"; //OBSOLETE
        public static Guid EnviCompHeightParam = new Guid("1bd715a2-2c7e-4564-bdef-92780f0ba2c8"); //窗顶到FL OBSOLETE

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
}
