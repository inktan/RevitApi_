using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using PubFuncWt;

namespace BSMT_PpLayout
{
    /// <summary>
    /// 该功能用于批量替换停车位族 
    /// </summary>
    class BrushLayer : RequestMethod
    {
        internal BrushLayer(UIApplication uiapp) : base(uiapp) { }

        internal override void Execute()
        {

            // 找到所有的填充区域，更改其线样式
            IEnumerable<FilledRegion> filledRegions =
                new FilteredElementCollector(this.doc, this.view.Id)
                .OfCategory(BuiltInCategory.OST_DetailComponents)
                .OfClass(typeof(FilledRegion))
                .WhereElementIsNotElementType()
                .Cast<FilledRegion>();

            List<GraphicsStyle> graphics = this.doc.GetAllLineStyleIdsFromSetting();

            foreach (var item in filledRegions)
            {
                string typeName = this.doc.GetElement(item.GetTypeId()).Name;

                if (typeName == "地库外墙范围" || typeName == "地库_外墙范围")
                    ChangeFRLinestyle(item, graphics, "地库_外墙范围");
                else if (typeName == "地库_核心筒")
                    ChangeFRLinestyle(item, graphics, "地库_核心筒");
                else if (typeName == "设备用房" || typeName == "地库_设备用房")
                    ChangeFRLinestyle(item, graphics, "地库_设备用房");
                else if (typeName == "地库_采光井")
                    ChangeFRLinestyle(item, graphics, "地库_采光井");
                else if (typeName == "地库_主楼非停车区域")
                    ChangeFRLinestyle(item, graphics, "地库_主楼非停车区域");
                else if (typeName == "地库_非机动车库" && !typeName.Contains("_夹层"))
                    ChangeFRLinestyle(item, graphics, "地库_非机动车库");
                else if (typeName == "地库_非机动车库_夹层")
                    ChangeFRLinestyle(item, graphics, "地库_非机动车库_夹层");
                else if (typeName == "地库_子停车区域")
                    ChangeFRLinestyle(item, graphics, "地库_子停车区域");
                else if (typeName == "地库_坡道")
                    ChangeFRLinestyle(item, graphics, "地库_坡道");
                else if (typeName == "塔楼开间区域" || typeName == "地库_塔楼开间区域")
                    ChangeFRLinestyle(item, graphics, "地库_塔楼开间区域");
                else if (typeName == "塔楼结构轮廓" || typeName == "地库_塔楼结构轮廓")
                    ChangeFRLinestyle(item, graphics, "地库_塔楼结构轮廓");
                else if (typeName == "公变所" || typeName == "地库_公变所")
                    ChangeFRLinestyle(item, graphics, "地库_公变所");
                else if (typeName == "地库_下沉庭院")
                    ChangeFRLinestyle(item, graphics, "地库_下沉庭院");
                else if (typeName == "地库_结构柱")
                    ChangeFRLinestyle(item, graphics, "地库_结构柱");
                else if (typeName == "地库_工具间")
                    ChangeFRLinestyle(item, graphics, "地库_工具间");
                else if (typeName == "地库_单元门厅")
                    ChangeFRLinestyle(item, graphics, "地库_单元门厅");
                else if (typeName == "地库_储藏间" || typeName == "地库_储藏间_已分隔")
                    ChangeFRLinestyle(item, graphics, "地库_储藏间");
                else if (typeName == "地库_障碍物")
                    ChangeFRLinestyle(item, graphics, "地库_障碍物");
                else if (typeName == "地库_人防分区")
                    ChangeFRLinestyle(item, graphics, "地库_人防分区");
                else if (typeName == "地库_防火分区")
                    ChangeFRLinestyle(item, graphics, "地库_防火分区");
            }
        }
        void ChangeFRLinestyle(FilledRegion filledRegion, List<GraphicsStyle> graphics, string typeName)
        {
            foreach (var item in graphics)
            {
                if (item.Name == typeName)
                {
                    using (Transaction transaction = new Transaction(this.doc, "修改详图区域填充的颜色"))
                    {
                        transaction.Start();
                        filledRegion.SetLineStyleId(item.Id);
                        transaction.Commit();
                    }
                    break;
                }
            }
        }
    }
}
