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
    internal class AlignToBoardBottom : RequestMethod
    {
        internal AlignToBoardBottom(UIApplication _uiApp) : base(_uiApp)
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
                tarHeight = floor.get_Parameter(BuiltInParameter.STRUCTURAL_ELEVATION_AT_BOTTOM).AsDouble();
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
                tarHeight = baseLevelHeight + baseOffset;
            }
            else if (element is FamilyInstance familyInstance01)
            {
                if (familyInstance01.Symbol.FamilyName.Contains("墙") || familyInstance01.Symbol.FamilyName.Contains("梁"))
                {
                    //顶部高程
                    tarHeight = familyInstance01.get_Parameter(BuiltInParameter.STRUCTURAL_ELEVATION_AT_BOTTOM).AsDouble();
                }
                // 柱
                else if (familyInstance01.Symbol.FamilyName.Contains("柱"))
                {
                    //底部标高
                    ElementId topLevelId = familyInstance01.get_Parameter(BuiltInParameter.FAMILY_BASE_LEVEL_PARAM).AsElementId();
                    Level topLevel = this.doc.GetElement(topLevelId) as Level;
                    double topLevelHeight = topLevel.Elevation;

                    //底部偏移
                    double baseOffset = familyInstance01.get_Parameter(BuiltInParameter.SCHEDULE_BASE_LEVEL_OFFSET_PARAM).AsDouble();

                    //顶部高程
                    tarHeight = topLevelHeight + baseOffset;
                }
            }

            //tarHeight.FeetToMilliMeter().ToString().TaskDialogErrorMessage();

            while (true)
            {
                try
                {
                    Reference fiReference = this.sel.PickObject(ObjectType.Element, new Fliter());
                    Element ele = this.doc.GetElement(fiReference);
                    if (ele is FamilyInstance)
                    {
                        FamilyInstance fi = this.doc.GetElement(fiReference) as FamilyInstance;
                        string faName = fi.Symbol.FamilyName;
                        if (faName.Contains("梁"))
                        {
                            //获取梁的底部高程
                            double beamBotHeight = fi.get_Parameter(BuiltInParameter.STRUCTURAL_ELEVATION_AT_BOTTOM).AsDouble();
                            double moveDis = tarHeight - beamBotHeight;

                            double beamEndOffset01 = fi.get_Parameter(BuiltInParameter.STRUCTURAL_BEAM_END0_ELEVATION).AsDouble();

                            moveDis += beamEndOffset01;

                            using (Transaction _trans = new Transaction(this.doc, "修改梁高"))
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
                            ElementId levelId = fi.get_Parameter(BuiltInParameter.FAMILY_BASE_LEVEL_PARAM).AsElementId();
                            Level level = this.doc.GetElement(levelId) as Level;
                            double levelHeight = level.Elevation;

                            double moveDis = tarHeight - levelHeight;
                            using (Transaction _trans = new Transaction(this.doc, "修改柱底与梁底"))
                            {
                                _trans.Start();
                                _trans.DeleteErrOrWaringTaskDialog();
                                //顶部偏移
                                fi.get_Parameter(BuiltInParameter.FAMILY_BASE_LEVEL_OFFSET_PARAM).Set(moveDis);
                                fi.get_Parameter(BuiltInParameter.STRUCTURAL_ANALYTICAL_MODEL).Set(0.0);
                                _trans.Commit();
                            }

                        }
                    }
                    else if (ele is Wall)
                    {
                        Wall fi = ele as Wall;

                        //底部标高
                        ElementId baseLevelId = fi.get_Parameter(BuiltInParameter.WALL_BASE_CONSTRAINT).AsElementId();
                        Level baseLevel = this.doc.GetElement(baseLevelId) as Level;
                        double baseLevelHeight = baseLevel.Elevation;
                        double baseOffset = fi.get_Parameter(BuiltInParameter.WALL_BASE_OFFSET).AsDouble();

                        double moveDis = tarHeight - baseLevelHeight;
                        using (Transaction _trans = new Transaction(this.doc, "修改墙高与柱高"))
                        {
                            _trans.Start();
                            _trans.DeleteErrOrWaringTaskDialog();
                            //设置无连接高度
                            fi.get_Parameter(BuiltInParameter.WALL_BASE_OFFSET).Set(moveDis);
                            fi.get_Parameter(BuiltInParameter.WALL_STRUCTURAL_SIGNIFICANT).Set(0.0);
                            _trans.Commit();
                        }

                        //顶部约束标高
                        ElementId topLevelId = fi.get_Parameter(BuiltInParameter.WALL_HEIGHT_TYPE).AsElementId();
                        if (topLevelId.IntegerValue == -1)
                        {
                            //不存在约束标高的情况，要设置无连接高度
                            //topLevelId.IntegerValue.ToString().TaskDialogErrorMessage();
                            double unHeight = fi.get_Parameter(BuiltInParameter.WALL_USER_HEIGHT_PARAM).AsDouble();
                            double nowTopHeight = unHeight + baseLevelHeight + baseOffset;

                            //设置无连接高度
                            using (Transaction _trans = new Transaction(this.doc, "修改墙高与柱高"))
                            {
                                _trans.Start();
                                _trans.DeleteErrOrWaringTaskDialog();
                                //设置无连接高度
                                fi.get_Parameter(BuiltInParameter.WALL_USER_HEIGHT_PARAM).Set(nowTopHeight - tarHeight);
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

    internal class Fliter : ISelectionFilter
    {
        bool ISelectionFilter.AllowElement(Element elem)
        {
            if (elem is FamilyInstance fi)
            {
                //return true;
                if (fi.Symbol.FamilyName.Contains("墙") || fi.Symbol.FamilyName.Contains("柱") || fi.Symbol.FamilyName.Contains("梁"))
                {
                    return true;
                }
            }
            else if (elem is Floor || elem is Wall)
            {
                return true;
            }
            return false;
            //throw new NotImplementedException();
        }
        bool ISelectionFilter.AllowReference(Reference reference, XYZ position)
        {
            return true;
            //throw new NotImplementedException();
        }
    }
    internal class FloorFliter : ISelectionFilter
    {
        bool ISelectionFilter.AllowElement(Element elem)
        {
            if (elem is Floor)
            {
                return true;
            }
            return false;
            //throw new NotImplementedException();
        }

        bool ISelectionFilter.AllowReference(Reference reference, XYZ position)
        {
            return true;
            //throw new NotImplementedException();
        }
    }
}
