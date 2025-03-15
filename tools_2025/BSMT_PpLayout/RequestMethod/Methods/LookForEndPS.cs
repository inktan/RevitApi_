using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using Autodesk.Revit.DB;
using PubFuncWt;
using g3;
using System.Diagnostics;
using goa.Common;
using goa.Common.g3InterOp;

namespace BSMT_PpLayout
{
    class LookForEndPS : RequestMethod
    {
        internal LookForEndPS(UIApplication uIApplication) : base(uIApplication)
        {

        }

        internal override void Execute()
        {
            InitialUIinter initialUIinter = new InitialUIinter(this.uiApp);
            List<ElementId> selBaseWallIds = initialUIinter.SelBsmtWallIds(); // UI获取所有地库填充区域id
            List<ElemsViewLevel> elemsViewLevels = initialUIinter.ElemsViewLevels(); //一个视图对应一个层级

            foreach (ElemsViewLevel elemsViewLevel in elemsViewLevels)// 遍历单个视图
            {
                View nowView = elemsViewLevel.View;
                // 找到每个地库外墙边界内的所有尽端车位
                foreach (Bsmt bsmt in elemsViewLevel.Bsmts)// 遍历单个地库
                {
                    if (!selBaseWallIds.Contains(bsmt.BsmtBound.Id))// 判断是否UI选择
                        continue;
                    bsmt.Computer_Ps_Fr_Col_SubExit_Area();
                    //【】首先将停车位的类型刷新

                    foreach (RevitElePS parkingUnitExit in bsmt.InBoundElePses.Where(p => p.EleProperty == EleProperty.EndPP))
                    {
                        FamilyInstance familyInstance = parkingUnitExit.Ele as FamilyInstance;
                        FamilySymbol fs = familyInstance.Symbol;
                        string fsName = fs.Name;
                        string faName = fs.FamilyName;

                        int firstIndex = fsName.IndexOf('回');
                        string newFsName01 = fsName.Substring(0, firstIndex);
                        string newFsName02 = fsName.Substring(firstIndex + 3, fsName.Count() - firstIndex - 3);

                        string newFsName = newFsName01 + newFsName02;

                        FamilySymbol newFs = doc.FamilySymbolByName(newFsName, newFsName01);//寻找目标停车位族类型 

                        using (Transaction trans = new Transaction(doc, "changeFs"))
                        {
                            trans.Start();
                            familyInstance.Symbol = newFs;
                            trans.Commit();
                        }
                    }

                    List<Vector2d> vector2Ds = bsmt.InBoundElePses.Select(p => p.LocVector2d).ToList();
                    alglib.kdtree kdTree = KdTree.ConstructTree<RevitElePS>(bsmt.InBoundElePses, vector2Ds);

                    List<FamilyInstance> needEndTypeFamilyInstances = new List<FamilyInstance>();

                    //【】 取出所有行车道中心线交点和端点，将尽端点和其最近的交点求出
                    OldRS oldRS = new OldRS(bsmt);
                    oldRS.UpdatePathProperties();
                    foreach (var item in oldRS.Routes)
                    {
                        item.CalEndLocation();

                        if (item.PathType == PathType.SingleEnd || item.PathType == PathType.BothEnds)// 回车示意
                        {
                            Vector2d vector2d = item.EndReturnLocation;// 回车位置

                            RevitElePS revitElePS = KdTree.SearchByCoord<RevitElePS>(vector2d, bsmt.InBoundElePses, kdTree, 1).FirstOrDefault();
                            needEndTypeFamilyInstances.Add(revitElePS.Ele as FamilyInstance);
                        }
                        else if (item.PathType == PathType.None)// 85米示意
                        {
                            Vector2d vector2d = item.LoopReturnLocation;// 回车位置

                            List<RevitElePS> revitElePs = KdTree.SearchByCoord<RevitElePS>(vector2d, bsmt.InBoundElePses, kdTree, 1);
                            needEndTypeFamilyInstances.AddRange(revitElePs.Select(p => p.Ele as FamilyInstance));
                        }
                    }
                    using (Transaction trans = new Transaction(doc, "changeFs"))
                    {
                        trans.Start();
                        needEndTypeFamilyInstances.ForEach(p =>
                        {
                           // 在该处，打圈划标记




                        });
                        trans.Commit();
                    }

                }
            }

            //throw new NotImplementedException();
        }

    }
}
