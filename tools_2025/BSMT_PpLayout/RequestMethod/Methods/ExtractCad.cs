
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PubFuncWt;
using goa.Common;
using g3;

namespace BSMT_PpLayout
{

    class ExtractCad : RequestMethod
    {
        public ExtractCad(UIApplication uiapp) : base(uiapp)
        {
        }

        internal override void Execute()
        {
            ImportInstance importInstance = this.doc.GetElement(sel.PickObject(ObjectType.Element, new SelPickFilter_ImportInstance() { Doc = this.doc })) as ImportInstance;

            CategoryNameMap categoryNameMap = importInstance.Category.SubCategories;

            List<string> layerNames = new List<string>();
            foreach (var item in categoryNameMap)
            {
                Category category = item as Category;
                layerNames.Add(category.Name);
            }

            SelLayer selLayer = new SelLayer();

            selLayer.fillRegionTypeNames.ItemsSource = doc.FillRegionTypeNames();
            selLayer.fillRegionTypeNames.SelectedIndex = 0;

            selLayer.comboBox01.ItemsSource = layerNames;
            selLayer.comboBox01.SelectedIndex = 0;

            selLayer.ShowDialog();

            string layerName = selLayer.comboBox01.SelectedItem as string;

            List<PolyLine> polyLines = PolylineInCAD(importInstance, layerName);

            if (polyLines.Count < 1)
            {
                throw new NotImplementedException("在当前所选链接CAD的目标图层中，未找到闭合多段线。");
                //throw new NotImplementedException("未在当前选择区域中，找到链接Cad文件中的闭合多段线。");
            }

            string filledTypeName = selLayer.fillRegionTypeNames.SelectedItem as string;
            foreach (var item in polyLines)
            {
                var points = item.GetCoordinates().DelAlmostEqualPoint();
                if (points.Count() < 3)
                {
                    continue;
                }
                List<Line> lines = new List<Line>();
                for (int i = 0; i < points.Count(); ++i)
                    lines.Add(Line.CreateBound(points.ElementAt(i), points.ElementAt((i + 1) % points.Count())));

                if (lines.Count > 0)
                {
                    CurveLoop cl = lines.ToCurveLoop();
                    if (!cl.IsOpen())
                    {
                        try
                        {
                            this.view.CreatFilledRegoins(this.doc, new List<CurveLoop>() { cl }, filledTypeName, 0);
                        }
                        catch (Exception)
                        {
                            //throw;
                        }
                    }
                }
            }
        }
        /// <summary>
        /// 直接提取
        /// </summary>
        /// <returns></returns>
        internal List<PolyLine> PolylineInCAD(ImportInstance importInstance, string layerName)
        {
            List<PolyLine> result = new List<PolyLine>();

            Options opt = new Options();
            opt.IncludeNonVisibleObjects = false;
            List<PolyLine> temp = importInstance.GetGeometryObjectOfType<PolyLine>(opt);

            foreach (var item in temp)
            {
                GraphicsStyle graphicsStyle = this.doc.GetElement(item.GraphicsStyleId) as GraphicsStyle;
                if (graphicsStyle.GraphicsStyleCategory.Name == layerName)
                {
                    result.Add(item);
                }
            }
            return result;
        }
        /// <summary>
        /// 根据选择框，选择链接cad中的多段线
        /// </summary>
        /// <returns></returns>
        internal List<PolyLine> _PolylineInCAD()
        {

            List<PolyLine> result = new List<PolyLine>();
            // 更改操作方式
            var pickBox = this.sel.PickBox(PickBoxStyle.Crossing);
            Polygon2d region = pickBox.ToBoundingBox().ToPolygon2d();

            List<ImportInstance> cadlinks = new FilteredElementCollector(this.doc).OfClass(typeof(ImportInstance)).Cast<ImportInstance>().ToList();

            foreach (ImportInstance cadlink in cadlinks)
            {
                BoundingBoxXYZ boundingBoxXYZ = cadlink.get_BoundingBox(this.view);
                Polygon2d cadLinkPoly2d = boundingBoxXYZ.ToPolygon2d();
                if (region.Contains(cadLinkPoly2d) || region.Intersects(cadLinkPoly2d) || cadLinkPoly2d.Contains(region))
                {
                    Options opt = new Options();
                    opt.IncludeNonVisibleObjects = false;

                    List<PolyLine> temp = cadlink.GetGeometryObjectOfType<PolyLine>(opt).Where(x => isPolylineInsideBox(x, pickBox)).ToList();
                    result.AddRange(temp);
                }
            }
            return result;
        }
        /// <summary>
        /// polyling是否选择框内
        /// </summary>
        /// <returns></returns>
        private bool isPolylineInsideBox(PolyLine _pl, PickedBox _pb)
        {
            var bb = _pb.ToBoundingBoxUV();
            var coords = _pl.GetCoordinates().Select(x => x.ProjectToXY().ToUV());
            //return coords.All(x => bb.IsInside(x));
            foreach (var item in coords)
            {
                if (bb.IsInside(item))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
