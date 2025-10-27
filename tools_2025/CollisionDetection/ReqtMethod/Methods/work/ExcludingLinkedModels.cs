using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PubFuncWt;
using Autodesk.Revit.UI.Selection;
using System.Reflection;
using System.IO;
using TeighaNet;
using System.Diagnostics;
using Autodesk.Revit.DB.Structure;

using goa.Common;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB.Electrical;
using System.Text.RegularExpressions;
using goa.Revit.DirectContext3D;
using System.Windows;
using g3;
using Autodesk.Revit.Creation;
using System.Xml.Linq;
//using NetTopologySuite.Geometries;

namespace CollisionDetection
{
    internal class ExcludingLinkedModels : RequestMethod
    {
        internal ExcludingLinkedModels(UIApplication _uiApp) : base(_uiApp)
        {
        }

        internal override void Execute()
        {
            // 获取文档中所有的三维实体元素
            List<Element> elementsToCheck = this.sel.PickElementsByRectangle()
                   .Where(p => p.GetAllSolids().Count > 0)
                   .ToList();

            // 清楚高亮选择元素的状态
            using (Transaction t = new Transaction(this.doc, "Highlight Collisions"))
            {
                t.Start();
                this.sel.SetElementIds(new List<ElementId>() { });
                t.Commit();
            }
            // 检测碰撞
            Dictionary<ElementId, List<ElementId>> collisionPairs = new Dictionary<ElementId, List<ElementId>>();

            for (int i = 0; i < elementsToCheck.Count; i++)
            {
                Element elem1 = elementsToCheck[i];
                BoundingBoxXYZ bb1 = elem1.get_BoundingBox(null);

                for (int j = i + 1; j < elementsToCheck.Count; j++)
                {
                    Element elem2 = elementsToCheck[j];
                    BoundingBoxXYZ bb2 = elem2.get_BoundingBox(null);

                    // 首先检查边界框是否相交
                    if (DoBoundingBoxesIntersect(bb1, bb2))
                    {
                        // 如果边界框相交，进行更精确的几何检查
                        if (DoElementsIntersect(elem1, elem2))
                        {
                            // 记录碰撞对
                            if (!collisionPairs.ContainsKey(elem1.Id))
                            {
                                collisionPairs[elem1.Id] = new List<ElementId>();
                            }
                            collisionPairs[elem1.Id].Add(elem2.Id);

                            if (!collisionPairs.ContainsKey(elem2.Id))
                            {
                                collisionPairs[elem2.Id] = new List<ElementId>();
                            }
                            collisionPairs[elem2.Id].Add(elem1.Id);

                        }
                    }
                }
            }
            // 高亮显示碰撞元素
            if (collisionPairs.Count > 0)
            {
                using (Transaction t = new Transaction(this.doc, "Highlight Collisions"))
                {
                    t.Start();
                    this.view.DisableTemporaryViewMode(TemporaryViewMode.TemporaryHideIsolate);
                    this.sel.SetElementIds(collisionPairs.Keys);
                    this.uiDoc.ShowElements(collisionPairs.Keys);
                    this.view.IsolateElementsTemporary(collisionPairs.Keys);
                    t.Commit();
                }
            }
        }

        private bool DoBoundingBoxesIntersect(BoundingBoxXYZ bb1, BoundingBoxXYZ bb2)
        {
            if (bb1 == null || bb2 == null) return false;

            XYZ min1 = bb1.Min;
            XYZ max1 = bb1.Max;
            XYZ min2 = bb2.Min;
            XYZ max2 = bb2.Max;

            // 检查三个轴上的投影是否重叠
            bool xOverlap = (min1.X <= max2.X) && (max1.X >= min2.X);
            bool yOverlap = (min1.Y <= max2.Y) && (max1.Y >= min2.Y);
            bool zOverlap = (min1.Z <= max2.Z) && (max1.Z >= min2.Z);

            return xOverlap && yOverlap && zOverlap;
        }
        /// <summary>
        /// 当前判断条件为实体交错体积大于1立方厘米
        /// </summary>
        /// <param name="elem1"></param>
        /// <param name="elem2"></param>
        /// <returns></returns>
        private bool DoElementsIntersect(Element elem1, Element elem2)
        {
            Options options = new Options();
            options.ComputeReferences = true;
            options.DetailLevel = ViewDetailLevel.Fine;

            GeometryElement geom1 = elem1.get_Geometry(options);
            GeometryElement geom2 = elem2.get_Geometry(options);

            if (geom1 == null || geom2 == null) return false;

            foreach (GeometryObject obj1 in geom1)
            {
                Solid solid1 = obj1 as Solid;
                if (solid1 == null || solid1.Faces.Size == 0) continue;

                foreach (GeometryObject obj2 in geom2)
                {
                    Solid solid2 = obj2 as Solid;
                    if (solid2 == null || solid2.Faces.Size == 0) continue;

                    // 使用SolidUtils检查实体相交
                    try
                    {
                        Solid intersection = BooleanOperationsUtils.ExecuteBooleanOperation(solid1, solid2, BooleanOperationsType.Intersect);

                        // 10mm位置容差 1英尺= 0.3048米
                        // 10mm位置容差 1英尺= 304.8mm
                        // 10mm位置容差 0.1英尺= 30.48mm
                        // 10mm位置容差 0.033尺= 10.0584mm 0.033*0.033*0.033=0.000035937 对应1立方厘米
                        // 10mm位置容差 0.01英尺= 3.048mm
                        if (intersection != null && intersection.Volume > 0.000035937)
                        {
                            return true;
                        }
                    }
                    catch
                    {
                        // 布尔运算可能失败，忽略此情况
                    }
                }
            }

            return false;
        }
    }
}
