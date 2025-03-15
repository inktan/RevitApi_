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
using goa.Common.NtsInterOp;
using NetTopologySuite.Geometries;

namespace InfoStrucFormwork
{
    internal class AlignToBoardTop : RequestMethod
    {
        internal AlignToBoardTop(UIApplication _uiApp) : base(_uiApp)
        {
        }


        internal override void Execute()
        {
            //获取墙柱梁板的最高定位高度
            double tarHeight = 0.0;

            Reference reference = this.sel.PickObject(ObjectType.Element, new Fliter());
            Element element = this.doc.GetElement(reference);
            // 楼板
            if (element is Floor floor)
            {
                tarHeight = floor.get_Parameter(BuiltInParameter.STRUCTURAL_ELEVATION_AT_TOP).AsDouble();
            }
            // 墙
            else if (element is Wall wall)
            {
                //底部标高
                ElementId baseLevelId = wall.get_Parameter(BuiltInParameter.WALL_BASE_CONSTRAINT).AsElementId();
                Level baseLevel = this.doc.GetElement(baseLevelId) as Level;
                double baseLevelHeight = baseLevel.Elevation;
                //底部偏移
                double baseOffset = wall.get_Parameter(BuiltInParameter.WALL_BASE_OFFSET).AsDouble();
                //底部定位高度
                double basePositonHeight = baseLevelHeight + baseOffset;
                //无连接高度
                double noCOnHeight = wall.get_Parameter(BuiltInParameter.WALL_USER_HEIGHT_PARAM).AsDouble();

                tarHeight = noCOnHeight + basePositonHeight;
            }
            else if (element is FamilyInstance familyInstance01)
            {
                if (familyInstance01.Symbol.FamilyName.Contains("墙"))
                {
                    //顶部高程
                    tarHeight = familyInstance01.get_Parameter(BuiltInParameter.STRUCTURAL_ELEVATION_AT_TOP).AsDouble();
                }
                // 梁
                else if (familyInstance01.Symbol.FamilyName.Contains("梁"))
                {
                    //顶部高程
                    tarHeight = familyInstance01.get_Parameter(BuiltInParameter.STRUCTURAL_ELEVATION_AT_TOP).AsDouble();
                }
                // 柱
                else if (familyInstance01.Symbol.FamilyName.Contains("柱"))
                {
                    //顶部标高
                    ElementId topLevelId = familyInstance01.get_Parameter(BuiltInParameter.SCHEDULE_TOP_LEVEL_PARAM).AsElementId();
                    Level topLevel = this.doc.GetElement(topLevelId) as Level;
                    double topLevelHeight = topLevel.Elevation;

                    //顶部偏移
                    double baseOffset = familyInstance01.get_Parameter(BuiltInParameter.SCHEDULE_TOP_LEVEL_OFFSET_PARAM).AsDouble();

                    //顶部高程
                    tarHeight = topLevelHeight + baseOffset;
                }
            }

            //tarHeight.FeetToMilliMeter().ToString().TaskDialogErrorMessage();

            while (true)
            {
                try
                {
                    Reference fiReference = this.sel.PickObject(ObjectType.Element);
                    Element ele = this.doc.GetElement(fiReference);
                    if (ele is FamilyInstance)
                    {
                        FamilyInstance fi = this.doc.GetElement(fiReference) as FamilyInstance;
                        string faName = fi.Symbol.FamilyName;
                        if (faName.Contains("梁"))
                        {
                            ElementId fiLevelId = fi.get_Parameter(BuiltInParameter.INSTANCE_REFERENCE_LEVEL_PARAM).AsElementId();
                            Level level = this.doc.GetElement(fiLevelId) as Level;
                            double fiLevelHeight = level.Elevation;
                            double moveDis = tarHeight - fiLevelHeight;
                            using (Transaction _trans = new Transaction(this.doc, "修改梁高与柱高"))
                            {
                                _trans.Start();
                                _trans.DeleteErrOrWaringTaskDialog();
                                fi.get_Parameter(BuiltInParameter.STRUCTURAL_BEAM_END0_ELEVATION).Set(moveDis);
                                fi.get_Parameter(BuiltInParameter.STRUCTURAL_BEAM_END1_ELEVATION).Set(moveDis);
                                fi.get_Parameter(BuiltInParameter.STRUCTURAL_ANALYTICAL_MODEL).Set(0.0);
                                fi.get_Parameter(BuiltInParameter.Z_OFFSET_VALUE).Set(0.0);
                                _trans.Commit();
                            }
                        }
                        else if (faName.Contains("柱") || faName.Contains("墙"))
                        {
                            //顶部标高
                            ElementId topLevelId = fi.get_Parameter(BuiltInParameter.SCHEDULE_TOP_LEVEL_PARAM).AsElementId();
                            Level topLevel = this.doc.GetElement(topLevelId) as Level;
                            double topLevelHeight = topLevel.Elevation;

                            //底部标高
                            ElementId baseLevelId = fi.get_Parameter(BuiltInParameter.SCHEDULE_BASE_LEVEL_PARAM).AsElementId();
                            Level baseLevel = this.doc.GetElement(baseLevelId) as Level;
                            double baseLevelHeight = baseLevel.Elevation;

                            //底部偏移
                            double baseOffset = fi.get_Parameter(BuiltInParameter.SCHEDULE_BASE_LEVEL_OFFSET_PARAM).AsDouble();

                            //计算顶部偏移距离
                            //double moveDis = tarHeight - (topLevelHeight - baseLevelHeight + baseOffset);
                            double moveDis = tarHeight - topLevelHeight;
                            using (Transaction _trans = new Transaction(this.doc, "修改墙高与柱高"))
                            {
                                _trans.Start();
                                _trans.DeleteErrOrWaringTaskDialog();
                                //顶部偏移
                                fi.get_Parameter(BuiltInParameter.FAMILY_TOP_LEVEL_OFFSET_PARAM).Set(moveDis);
                                fi.get_Parameter(BuiltInParameter.STRUCTURAL_ANALYTICAL_MODEL).Set(0.0);
                                _trans.Commit();
                            }
                        }
                    }
                    else if (ele is Wall)
                    {
                        Wall fi = ele as Wall;

                        //判断是否有顶部约束
                        double wallTopLevelHeight = 0.0;
                        ElementId topLevelId = fi.get_Parameter(BuiltInParameter.WALL_HEIGHT_TYPE).AsElementId();
                        try
                        {
                            //顶部标高
                            var levelId = fi.get_Parameter(BuiltInParameter.WALL_HEIGHT_TYPE).AsElementId();
                            wallTopLevelHeight = (this.doc.GetElement(levelId) as Level).Elevation;
                        }
                        catch (Exception)
                        {
                            //throw;
                        }

                        if (topLevelId.IntegerValue == -1)
                        {
                            //底部标高
                            ElementId baseLevelId = fi.get_Parameter(BuiltInParameter.WALL_BASE_CONSTRAINT).AsElementId();
                            Level baseLevel = this.doc.GetElement(baseLevelId) as Level;
                            double baseLevelHeight = baseLevel.Elevation;
                            double baseOffset = fi.get_Parameter(BuiltInParameter.WALL_BASE_OFFSET).AsDouble();

                            double moveDis = tarHeight - (baseLevelHeight + baseOffset);
                            using (Transaction _trans = new Transaction(this.doc, "修改墙高与柱高"))
                            {
                                _trans.Start();
                                _trans.DeleteErrOrWaringTaskDialog();
                                //设置无连接高度
                                fi.get_Parameter(BuiltInParameter.WALL_USER_HEIGHT_PARAM).Set(moveDis);
                                fi.get_Parameter(BuiltInParameter.WALL_STRUCTURAL_SIGNIFICANT).Set(0.0);
                                _trans.Commit();
                            }
                        }
                        else
                        {
                            //底部偏移
                            //计算顶部偏移距离
                            double moveDis = tarHeight - wallTopLevelHeight;
                            using (Transaction _trans = new Transaction(this.doc, "修改墙高与柱高"))
                            {
                                _trans.Start();
                                _trans.DeleteErrOrWaringTaskDialog();
                                //顶部偏移
                                //moveDis.ToString().TaskDialogErrorMessage();
                                fi.get_Parameter(BuiltInParameter.WALL_TOP_OFFSET).Set(moveDis);
                                fi.get_Parameter(BuiltInParameter.WALL_STRUCTURAL_SIGNIFICANT).Set(0.0);
                                _trans.Commit();
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    if (ex is Autodesk.Revit.Exceptions.OperationCanceledException)//用户取消异常，不抛出异常信息
                    {
                        break;
                    }
                }
            }
        }


    }

}


