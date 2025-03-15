using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PubFuncWt;
using goa.Common;
using Autodesk.Revit.UI.Selection;
using System.Reflection;

namespace MountSiteDesignAnaly
{
    internal class GenerateFloor : RequestMethod
    {
        internal static bool CancelCommand { get; set; }
        internal GenerateFloor(UIApplication _uiApp) : base(_uiApp)
        {

        }

        internal override void Execute()
        {
            // 拾取cad
            Reference reference = this.sel.PickObject(ObjectType.Element, new SelPickFilter_ImportInstance() { Doc = this.doc });
            ImportInstance importInstance = this.doc.GetElement(reference) as ImportInstance;

            // 获取楼板类型
            List<FloorType> floorTypes = doc.FloorTypes();

            DwgInfo dwgInfo = new DwgInfo(importInstance);
            dwgInfo.Excute(this.uiApp);

            SelLayer selLayer = new SelLayer();

            // 楼板类型名字
            selLayer.floorTypeNames.ItemsSource = floorTypes.Select(p => p.Name);
            selLayer.floorTypeNames.SelectedIndex = 0;
            // 图层名字-用于选择楼板类型，也可以包含文字
            selLayer.cadLayers.ItemsSource = dwgInfo.LayerNames;
            selLayer.cadLayers.SelectedIndex = 0;
            // 图层名字-用于选择文字-当前代表楼板高度
            selLayer.textLayers.ItemsSource = dwgInfo.LayerNames;
            selLayer.textLayers.SelectedIndex = 0;

            // 显示选择窗口
            selLayer.ShowDialog();
            if (CancelCommand) return;

            // 选择楼板类型
            string floorTypeName = selLayer.floorTypeNames.SelectedItem as string;
            // 选择楼板多段线图层 可包含对应高度文字
            string floorPolyLineLayerName = selLayer.cadLayers.SelectedItem as string;
            // 选择高度文字图层
            string heightTextLayerName = selLayer.textLayers.SelectedItem as string;

            // 选择图层与楼板

            FloorType floorType = floorTypes.Where(p => p.Name == floorTypeName).FirstOrDefault();
            Level level = (new FilteredElementCollector(this.doc))
                .OfCategory(BuiltInCategory.OST_Levels)
                .WhereElementIsNotElementType().OfClass(typeof(Level))
                .Cast<Level>()
                .Where(p => p.Elevation >= 0.0)
                .First();

            dwgInfo.floorPolylingLayer = floorPolyLineLayerName;
            dwgInfo.heightTextLayer = heightTextLayerName;
            List<PolyLineInfo> polyLineInfos = dwgInfo.PolyLinesByLayerName().ToList();
            if (polyLineInfos.Count < 1)
            {
                throw new NotImplementedException("在当前所选链接CAD的目标图层中，未找到闭合多段线。");
                //throw new NotImplementedException("未在当前选择区域中，找到链接Cad文件中的闭合多段线。");
            }

            foreach (var item in polyLineInfos)
            {
                item.CreatFloor(floorType, level);
            }

            //throw new NotImplementedException();
        }

    }
}
