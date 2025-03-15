//using Autodesk.Revit.DB;
//using Autodesk.Revit.UI;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//using PubFuncWt;
//using Autodesk.Revit.UI.Selection;
//using System.Reflection;
//using System.IO;
//using TeighaNet;
//using System.Diagnostics;
//using Autodesk.Revit.DB.Structure;

//using goa.Common;
//using g3;
//using System.Text.RegularExpressions;
////using DataStructures.Trees;
//using System.Threading;

//namespace InfoStrucFormwork
//{
//    internal class StrucFloorAux
//    {

//        public StrucFloorAux()
//        {
//        }

//        internal void Execute()
//        {
//            OpenTrans();
//            //throw new NotImplementedException();
//            System.Windows.Forms.Application.DoEvents();// 窗体重绘
//        }
//        List<ElementId> ElementIds { get; set; }
//        List<FamilyInstance> FamilyInstances { get; set; }
//        internal void OpenTrans()
//        {
//            this.ElementIds = new List<ElementId>();
//            this.FamilyInstances = new List<FamilyInstance>();
//            // 创建二级洞口
//            using (Transaction transaction = new Transaction(CMD.Doc, "创建降板及洞口"))
//            {
//                transaction.Start();
//                //transaction.DeleteErrOrWaringTaskDialog();

//                foreach (var p in StrucFloor.floorDescendInfos)
//                {
//                    foreach (var _p in p.Holes)
//                    {
//                        if (_p.CurveArray.Size > 3)
//                        {
//                            try
//                            {
//                                XYZ position = _p.PolyLineInfo.PolyLine.GetOutline().MinimumPoint;
//                                FamilyInstance familyInstance = StrucFloor.PlaeUniverslHole(p.FamilyInstance, position, _p.PolyLineInfo.Polygon2d);
//                                _p.FamilyInstance = familyInstance;
//                                this.ElementIds.Add(familyInstance.Id);
//                                this.FamilyInstances.Add(familyInstance);
//                                //this.ElementIds.Add(_p.FamilyInstance.Id);
//                            }
//                            catch (Exception)
//                            {
//                                //throw;
//                            }
//                        }
//                    }
//                }
//                transaction.Commit();
//            }
//            using (Transaction transaction = new Transaction(CMD.Doc, "移动"))
//            {
//                transaction.Start();
//                transaction.DeleteErrOrWaringTaskDialog();

//                //List<ElementId> elementIds = this.floorInfos.Where(p => p.Floor != null && p.Floor.IsValidObject).Select(p => p.Floor.Id).ToList();

//                //this.floorInfos.ForEach(_p =>
//                //{
//                //    elementIds.AddRange(_p.DescendFloors.Where(p => p.Descend != null && p.Descend.IsValidObject).Select(p => p.Descend.Id));
//                //    elementIds.AddRange(_p.Holes.Where(p => p.Hole != null && p.Hole.IsValidObject).Select(p => p.Hole.Id));
//                //});

//                if (this.ElementIds.Count() > 0)// 移动楼板
//                {
//                    ElementTransformUtils.MoveElements(CMD.Doc, this.ElementIds.Where(p => CMD.Doc.GetElement(p) != null && CMD.Doc.GetElement(p).IsValidObject).ToList(), CMD.PositonMoveDis);
//                }
//                // 移动 楼板-开洞 族

//                transaction.Commit();
//            }
//            using (Transaction transaction = new Transaction(CMD.Doc, "-"))
//            {
//                transaction.Start();
//                transaction.DeleteErrOrWaringTaskDialog();

//                foreach (var item in this.FamilyInstances)
//                {
//                    item.LookupParameter("建筑板厚度").Set(150.0.MilliMeterToFeet());
//                }
//                transaction.Commit();
//            }
//            using (Transaction transaction = new Transaction(CMD.Doc, "-"))
//            {
//                transaction.Start();
//                transaction.DeleteErrOrWaringTaskDialog();

//                foreach (var item in this.FamilyInstances)
//                {
//                    item.LookupParameter("建筑板厚度").Set(0.0);
//                }
//                transaction.Commit();
//            }
//        }
//    }
//}
