//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//using Autodesk.Revit.UI;
//using Autodesk.Revit.UI.Selection;
//using Autodesk.Revit.DB;
//using PubFuncWt;
//using g3;
//using System.Diagnostics;
//using goa.Common;

//namespace BSMT_PpLayout
//{
//    class LookForEndPS : RequestMethod
//    {
//        internal LookForEndPS(UIApplication uIApplication):base(uIApplication)
//        {

//        }

//        internal override void Execute()
//        {
//            #region 找到停车位族类型                       
//            FamilySymbol familySymbol = doc.ReturnCarType();
//            #endregion

//            InitialUIinter initialUIinter = new InitialUIinter(this.uiApp);
//            List<ElementId> selBaseWallIds = initialUIinter.SelBsmtWallIds(); // UI获取所有地库填充区域id
//            List<ElemsViewLevel> elemsViewLevels = initialUIinter.ElemsViewLevels(); //一个视图对应一个层级

//            foreach (ElemsViewLevel elemsViewLevel in elemsViewLevels)// 遍历单个视图
//            {
//                View nowView = elemsViewLevel.View;
//                // 找到每个地库外墙边界内的所有尽端车位
//                foreach (Bsmt bsmt in elemsViewLevel.Bsmts)// 遍历单个地库
//                {
//                    if (!selBaseWallIds.Contains(bsmt.BsmtBound.Id))// 判断是否UI选择
//                        continue;
//                    bsmt.Computer();
//                    //【】首先将停车位的类型刷新
//                    foreach (RevitElePS parkingUnitExit in bsmt.InBoundElePses)
//                    {
//                        FamilyInstance familyInstance = parkingUnitExit.Ele as FamilyInstance;
//                        FamilySymbol fs = familyInstance.Symbol;
//                        string fsName = fs.Name;
//                        string faName = fs.FamilyName;

//                        if (fsName.Contains("回车"))
//                        {
//                            int firstIndex = fsName.IndexOf('回');
//                            string newFsName01 = fsName.Substring(0, firstIndex);
//                            string newFsName02 = fsName.Substring(firstIndex+3, fsName.Count()- firstIndex-3);

//                            string newFsName = newFsName01 + newFsName02;

//                            FamilySymbol newFs  = doc.FamilySymbolByName(newFsName, newFsName01);//寻找目标停车位族类型 

//                            using (Transaction trans = new Transaction(doc, "changeFs"))
//                            {
//                                trans.Start();
//                                familyInstance.Symbol = newFs;
//                                trans.Commit();
//                            }
//                        }
//                    }

//                    EndParkingHandling endParkingHandling = new EndParkingHandling(bsmt);
//                    List<PairEndPS> pairEndPs = endParkingHandling.FindRowCars();

//                    List<FamilyInstance> endTypeFamilyInstances = new List<FamilyInstance>();
//                    if (pairEndPs.Count > 0)
//                    {

//                        foreach (PairEndPS pairEndPS in pairEndPs)
//                        {
//                            // 判断当前排属于尽端情况的停车位是否贴边与地库外墙线
//                            bool isContine = false;
//                            Polygon2d polygon2dBaseMent = bsmt.Polygon2dInward;
//                            foreach (Segment2d segment2d in polygon2dBaseMent.SegmentItr())
//                            {
//                                Vector2d vector2d01 = pairEndPS.ParkingUnitExit01.LocVector2d;
//                                Vector2d vector2d02 = pairEndPS.ParkingUnitExit02.LocVector2d;

//                                double distance01 = vector2d01.CalcPedalDistanceToLine(segment2d.ToLine2d());
//                                double distance02 = vector2d02.CalcPedalDistanceToLine(segment2d.ToLine2d());

//                                double difference01 = (distance01) - GlobalData.pSHeight / 2 - GlobalData.bsmtWallThickness;
//                                double difference02 = (distance02) - GlobalData.pSHeight / 2 - GlobalData.bsmtWallThickness;

//                                if (Math.Abs(difference01) <= Precision_.Precison && Math.Abs(difference02) <= Precision_.Precison)
//                                {
//                                    isContine = true;
//                                    break;
//                                }
//                            }
//                            if (isContine) continue;

//                            // 找到其中所有需要替换族类型的停车位，需要对这些回车位进行二次筛选处理
//                            if (pairEndPS.NowRowCarEndType == CarType.EndType)
//                            {
//                                // 尽端回车问题
//                                List<FamilyInstance> familyInstances = pairEndPS.FindTarInReturnEnd();
//                                endTypeFamilyInstances.AddRange(familyInstances);                    
//                            }
//                            else if (pairEndPS.NowRowCarEndType == CarType.NoEndType)
//                            {
//                                // 设置联通道问题
//                                List<FamilyInstance> familyInstances = pairEndPS.FindTarInLoopReturn();
//                                using (Transaction trans = new Transaction(doc, "changeFs"))
//                                {
//                                    trans.Start();
//                                    familyInstances.ForEach(p => {

//                                        p.Symbol = familySymbol;
//                                    });
//                                    trans.Commit();
//                                }
//                            }
//                        }
//                    }

//                    #region 对求出的尽端车位进行二次处理
//                    List<List<FamilyInstance>> needEndTypeFamilyInstancesList = GetRowEndTypeFamilyInstance(endTypeFamilyInstances);
//                    List<FamilyInstance> needEndTypeFamilyInstances = new List<FamilyInstance>();
//                    foreach (List<FamilyInstance> familyInstances1 in needEndTypeFamilyInstancesList)
//                    {
//                        needEndTypeFamilyInstances.AddRange(GetFinalEndTypeFamilyInstance(familyInstances1)) ;
//                    }
//                    using (Transaction trans = new Transaction(doc, "changeFs"))
//                    {
//                        trans.Start();
//                        needEndTypeFamilyInstances.ForEach(p => {
//                            p.Symbol = familySymbol;
//                        });
//                        trans.Commit();
//                    }
//                    #endregion
//                    //sw.Stop();
//                    //TaskDialog.Show("error", "程序执行所用时间:" + sw.ElapsedMilliseconds.ToString() + " 毫秒");

//                }
//            }

//            //throw new NotImplementedException();
//        }
//        /// <summary>
//        /// 找到共线的尽端回车停车位
//        /// </summary>
//        /// <param name="familyInstances"></param>
//        /// <returns></returns>
//        internal List<List<FamilyInstance>> GetRowEndTypeFamilyInstance(List<FamilyInstance> _familyInstances)
//        {
//            List<List<FamilyInstance>> familyInstancesList = new List<List<FamilyInstance>>();
//            List<ElementId> collectionEleIds = new List<ElementId>();// 收集器用于数据循环使用
//            foreach (FamilyInstance familyInstance in _familyInstances)
//            {
//                if (collectionEleIds.Contains(familyInstance.Id)) continue;
//                collectionEleIds.Add(familyInstance.Id);

//                List<FamilyInstance> familyInstances = new List<FamilyInstance>() { familyInstance };
//                foreach (FamilyInstance _familyInstance in _familyInstances)
//                {
//                    #region 避免重复循环
//                    if (familyInstance.Id == _familyInstance.Id) continue;
//                    if (collectionEleIds.Contains(_familyInstance.Id)) continue;
//                    #endregion
//                    Vector2d facingOri = familyInstance.FacingOrientation.ToVector2d();
//                    // 共线
//                    Vector2d location01 = (familyInstance.Location as LocationPoint).Point.ToVector2d();
//                    Vector2d location02 = (_familyInstance.Location as LocationPoint).Point.ToVector2d();
//                    Line2d line2D = new Line2d(location01, facingOri);
//                    double distance = Math.Sqrt(line2D.DistanceSquared(location02));
//                    if (distance > Precision_.Precison) continue;

//                    familyInstances.Add(_familyInstance);
//                    collectionEleIds.Add(_familyInstance.Id);
//                }
//                familyInstancesList.Add(familyInstances);
//            }
//            return familyInstancesList;
//        }

//        /// <summary>
//        /// 在一条线上的尽端回车保留问题
//        /// </summary>
//        /// <param name="familyInstances"></param>
//        /// <returns></returns>
//        internal List<FamilyInstance> GetFinalEndTypeFamilyInstance(List<FamilyInstance> familyInstances)
//        {
 
//            if(familyInstances.Count<=1)
//            {
//                return familyInstances;
//            }
//            else
//            {
//                List<FamilyInstance> _familyInstances = new List<FamilyInstance>();
//                // 对面尽端车位 需要删除一个
//                List<ElementId> collectionEleIds = new List<ElementId>();
//                // 判断头对头停车位 1 方向 2 距离
//                foreach (FamilyInstance familyInstance in familyInstances)
//                {
//                    if (collectionEleIds.Contains(familyInstance.Id)) continue;
//                    collectionEleIds.Add(familyInstance.Id);

//                    int time = 0;
//                    foreach (FamilyInstance _familyInstance in familyInstances)
//                    {
//                        time++;
//                        #region 避免重复循环
//                        //if (familyInstance.Id == _familyInstance.Id) continue;
//                        //if (collectionEleIds.Contains(_familyInstance.Id)) continue;
//                        #endregion

//                        Vector2d location01 = (familyInstance.Location as LocationPoint).Point.ToVector2d();
//                        Vector2d location02 = (_familyInstance.Location as LocationPoint).Point.ToVector2d();
//                        // 距离
//                        double _distance = location01.Distance(location02);
//                        double missDistance = Math.Abs(_distance - GlobalData.pSHeight - GlobalData.Wd_sec);

//                        double bufferValue = 500.0.MilliMeterToFeet();
//                        if (missDistance < Precision_.Precison + bufferValue)
//                        {
//                            _familyInstances.Add(familyInstance);
//                            collectionEleIds.Add(_familyInstance.Id);
//                            time = 0;
//                            break;
//                        }
//                        if (time == familyInstances.Count)
//                        {
//                            _familyInstances.Add(familyInstance);
//                            time = 0;
//                        }
//                    }
//                }
//                return _familyInstances;
//            }
//        }

//        /// <summary>
//        /// 将所有的停车位族类型复原
//        /// </summary>
//        /// <param name="baseMent"></param>
//        internal void ReSetParkingFsSymbol (Document doc,Bsmt baseMent,FamilySymbol familySymbol)
//        {
//            List<RevitElePS> parkingUnitExits = baseMent.InBoundElePses;

//            List<FamilyInstance> familyInstances = new List<FamilyInstance>();

//            foreach (RevitElePS parkingUnitExit in parkingUnitExits)
//            {
//                FamilyInstance familyInstance = parkingUnitExit.FamilyInstance;

//                string fsName = familyInstance.Symbol.Name;
//                if(fsName!= "停车位_")
//                {
//                    familyInstances.Add(familyInstance);
//                }
//            }

//            using (Transaction trans = new Transaction(doc, "changeFs"))
//            {
//                trans.Start();
//                familyInstances.ForEach(p => {
//                    p.Symbol = familySymbol;
//                });
//                trans.Commit();
//            }

//        }//method



//    }
//}
