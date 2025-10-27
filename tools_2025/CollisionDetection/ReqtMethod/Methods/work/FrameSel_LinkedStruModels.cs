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
//using NetTopologySuite.Geometries;

namespace CollisionDetection
{
    internal class FrameSel_LinkedStruModels : RequestMethod
    {
        internal FrameSel_LinkedStruModels(UIApplication _uiApp) : base(_uiApp)
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
            //Dictionary<ElementId, List<ElementId>> collisionPairs = new Dictionary<ElementId, List<ElementId>>();

            //for (int i = 0; i < elementsToCheck.Count; i++)
            //{
            //    Element elem1 = elementsToCheck[i];
            //    BoundingBoxXYZ bb1 = elem1.get_BoundingBox(null);

            //    for (int j = i + 1; j < elementsToCheck.Count; j++)
            //    {
            //        Element elem2 = elementsToCheck[j];
            //        BoundingBoxXYZ bb2 = elem2.get_BoundingBox(null);

            //        // 首先检查边界框是否相交
            //        if (DoBoundingBoxesIntersect(bb1, bb2))
            //        {
            //            List<Solid> solids01 = elem1.GetAllSolids();
            //            List<Solid> solids02 = elem2.GetAllSolids();

            //            // 如果边界框相交，进行更精确的几何检查
            //            if (DoElementsIntersect(solids01, solids02))
            //            {
            //                // 记录碰撞对
            //                if (!collisionPairs.ContainsKey(elem1.Id))
            //                {
            //                    collisionPairs[elem1.Id] = new List<ElementId>();
            //                }
            //                collisionPairs[elem1.Id].Add(elem2.Id);

            //                if (!collisionPairs.ContainsKey(elem2.Id))
            //                {
            //                    collisionPairs[elem2.Id] = new List<ElementId>();
            //                }
            //                collisionPairs[elem2.Id].Add(elem1.Id);
            //            }
            //        }
            //    }
            //}

            //return;

            // 获取文档中所有的三维l链接模型
            List<RevitLinkInstance> allRvtLinks = new FilteredElementCollector(this.doc).WhereElementIsNotElementType().OfCategory(BuiltInCategory.OST_RvtLinks).Cast<RevitLinkInstance>().ToList();
            //这里要进一步处理，判断链接模型为结构专业的链接模型
            List<RevitLinkInstance> rvtLinks = new List<RevitLinkInstance>();
            foreach (RevitLinkInstance link in allRvtLinks)
            {
                int structuralFramingCount = new FilteredElementCollector(link.GetLinkDocument())
                    .OfCategory(BuiltInCategory.OST_StructuralFraming)
                    .WhereElementIsNotElementType()
                    .Count();
                if (structuralFramingCount > 5)
                {
                    rvtLinks.Add(link);
                }
                //elements.Count.ToString().TaskDialogErrorMessage();
            }

            List<ElementId> collisionIds = new List<ElementId>();
            List<ElementId> collisionRvtLinkIds = new List<ElementId>();

            foreach (RevitLinkInstance rvtLink in rvtLinks)
            {
                Document linkDoc = rvtLink.GetLinkDocument();
                List<Element> rvtLinkEles = new FilteredElementCollector(linkDoc).WhereElementIsNotElementType()
                    .Where(p => p.GetAllSolids().Count > 0)
                    .ToList();
                Transform transform = rvtLink.GetTotalTransform();

                foreach (Element elem2 in rvtLinkEles)
                {
                    // 获取元素的BoundingBox
                    BoundingBoxXYZ bbox = elem2.get_BoundingBox(null);
                    if (bbox == null) continue;

                    // 创建变换后的BoundingBox
                    BoundingBoxXYZ bb2 = new BoundingBoxXYZ();

                    // 变换最小点和最大点
                    bb2.Min = transform.OfPoint(bbox.Min);
                    bb2.Max = transform.OfPoint(bbox.Max);

                    // 如果需要，也可以变换Transform属性（如果BoundingBox本身有变换）
                    if (bbox.Transform != null && !bbox.Transform.IsIdentity)
                    {
                        bb2.Transform = transform.Multiply(bbox.Transform);
                    }
                    else
                    {
                        bb2.Transform = transform;
                    }

                    foreach (Element elem1 in elementsToCheck)
                    {
                        List<Solid> solids1 = elem1.GetAllSolids();
                        BoundingBoxXYZ bb1 = elem1.get_BoundingBox(null);

                        // 首先检查边界框是否相交
                        if (DoBoundingBoxesIntersect(bb1, bb2))
                        {
                            List<Solid> solids02 = elem2.GetAllSolids().Select(p => SolidUtils.CreateTransformed(p, transform)).ToList();

                            // 如果边界框相交，进行更精确的几何检查
                            if (DoElementsIntersect(solids1, solids02))
                            {
                                collisionIds.Add(elem1.Id);
                                //将碰撞的链接模型id添加进去
                                if (!collisionRvtLinkIds.Contains(rvtLink.Id))
                                {
                                    collisionRvtLinkIds.Add(rvtLink.Id);
                                }
                                // 高亮显示碰撞元素
                                //using (Transaction t = new Transaction(this.doc, "Highlight Collisions"))
                                //{
                                //    t.Start();
                                //    this.sel.SetElementIds(new List<ElementId>() { elem1.Id });
                                //    this.uiDoc.ShowElements(new List<ElementId>() { elem1.Id });
                                //this.view.IsolateElementsTemporary(new List<ElementId>() { elem1.Id });
                                //    t.Commit();
                                //}
                                //return;
                            }
                        }
                    }
                }
            }

            //collisionIds.AddRange(collisionPairs.Keys);
            if (collisionIds.Count > 0)
            {
                // 高亮显示碰撞元素
                using (Transaction t = new Transaction(this.doc, "Highlight Collisions"))
                {
                    t.Start();
                    this.view.DisableTemporaryViewMode(TemporaryViewMode.TemporaryHideIsolate);
                    this.sel.SetElementIds(collisionIds);
                    this.uiDoc.ShowElements(collisionIds);
                    collisionIds.AddRange(collisionRvtLinkIds);
                    this.view.IsolateElementsTemporary(collisionIds);
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
        private bool DoElementsIntersect(List<Solid> solids01, List<Solid> solids02)
        {
            //Options options = new Options();
            //options.ComputeReferences = true;
            //options.DetailLevel = ViewDetailLevel.Fine;

            //GeometryElement geom1 = elem1.get_Geometry(options);
            //GeometryElement geom2 = elem2.get_Geometry(options);

            //if (geom1 == null || geom2 == null) return false;

            foreach (Solid solid1 in solids01)
            {
                foreach (Solid solid2 in solids02)
                {
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
