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
using goa.Common.g3InterOp;
using g3;


namespace InfoStrucFormwork
{

    internal class BeamFliter : ISelectionFilter
    {
        bool ISelectionFilter.AllowElement(Element elem)
        {
            if (elem is FamilyInstance fi)
            {
                //return true;
                if (fi.Symbol.FamilyName.Contains("梁"))
                {
                    return true;
                }
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
    internal class ConcSingleBeamFollowFaceUi : RequestMethod
    {
        internal ConcSingleBeamFollowFaceUi(UIApplication _uiApp) : base(_uiApp)
        {
        }

        internal override void Execute()
        {
            // 获取当前文档和活动视图
            Document doc = this.doc;
            ViewSection sectionView = doc.ActiveView as ViewSection;

            if (sectionView == null)
            {
                throw new NotImplementedException("错误，当前视图不是剖面视图。");
            }

            //选一个基准线
            Reference reference02 = CMD.Sel.PickObject(ObjectType.Edge);
            Element element02 = CMD.Doc.GetElement(reference02.ElementId);
            Edge edge = element02.GetGeometryObjectFromReference(reference02) as Edge;
            Curve baseLine = edge.AsCurve();

            // 获取剖面视图的变换矩阵
            Transform transform = sectionView.CropBox.Transform;

            // 将线的起点和终点转换到剖面视图的坐标系
            XYZ startPoint = transform.Inverse.OfPoint(baseLine.GetEndPoint(0));
            XYZ endPoint = transform.Inverse.OfPoint(baseLine.GetEndPoint(1));

            Line2d baseL2d = Line.CreateBound(startPoint, endPoint).ToLine2d();
            //将坐标系转为剖面后，计算Y的差值
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
                            Curve curve = fi.LocationCurve();

                            Vector2d p0 = transform.Inverse.OfPoint(curve.GetEndPoint(0)).ToVector2d();

                            double width = fi.LookupParameter("宽度").AsDouble();

                            Line2d baseL2d01 = new Line2d(new Vector2d(p0.x + width * 0.5, p0.y), new Vector2d(0, 1));
                            Line2d baseL2d02 = new Line2d(new Vector2d(p0.x - width * 0.5, p0.y), new Vector2d(0, 1));

                            double tarHeight = 0; // 高度差
                            IntrLine2Line2 intrLine2Line201 = new IntrLine2Line2(baseL2d, baseL2d01);
                            intrLine2Line201.Compute();
                            if (intrLine2Line201.Quantity == 1)
                            {
                                tarHeight = intrLine2Line201.Point.y - p0.y;
                            }

                            intrLine2Line201 = new IntrLine2Line2(baseL2d, baseL2d02);
                            intrLine2Line201.Compute();
                            if (intrLine2Line201.Quantity == 1)
                            {
                                if (tarHeight > intrLine2Line201.Point.y - p0.y)
                                {
                                    tarHeight = intrLine2Line201.Point.y - p0.y;
                                }
                            }

                            double s_height = fi.get_Parameter(BuiltInParameter.STRUCTURAL_BEAM_END0_ELEVATION).AsDouble();
                            double end_height = fi.get_Parameter(BuiltInParameter.STRUCTURAL_BEAM_END1_ELEVATION).AsDouble();

                            using (Transaction _trans = new Transaction(this.doc, "修改梁高与柱高"))
                            {
                                _trans.Start();
                                _trans.DeleteErrOrWaringTaskDialog();
                                fi.get_Parameter(BuiltInParameter.STRUCTURAL_BEAM_END0_ELEVATION).Set(s_height+ tarHeight);
                                fi.get_Parameter(BuiltInParameter.STRUCTURAL_BEAM_END1_ELEVATION).Set(end_height+ tarHeight);
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
                            double tarHeight = 0;

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
        internal void Execute_()
        {
            //选一根梁
            Reference reference01 = this.sel.PickObject(ObjectType.Element, new BeamFliter());
            FamilyInstance familyInstance01 = this.doc.GetElement(reference01) as FamilyInstance;
            if (familyInstance01 == null)
                throw new NotImplementedException("请选择一根梁");

            //选一个面
            List<Face> roofFaces = new List<Face>();
            Reference reference02 = CMD.Sel.PickObject(ObjectType.PointOnElement);
            Element element02 = CMD.Doc.GetElement(reference02.ElementId);

            Transform transform = Transform.Identity;
            if (element02 is RevitLinkInstance)
            {
                RevitLinkInstance revitLinkInstance = element02 as RevitLinkInstance;
                Document linkDoc = revitLinkInstance.GetLinkDocument();
                Element linkEle = linkDoc.GetElement(reference02.LinkedElementId);

                try
                {
                    //FamilyInstance familyInstance = linkEle as FamilyInstance;
                    //transform = familyInstance.GetTotalTransform();
                    //transform = Transform.CreateTranslation(familyInstance.GetTotalTransform().Origin) ;
                    //transform = familyInstance.GetTransform();
                    transform = Transform.CreateTranslation(new XYZ(0, 0, 57.9174302094503));

                }
                catch (Exception)
                {

                    throw;
                }

                Reference linkRefer = reference02.CreateReferenceInLink();
                Face face = linkEle.GetGeometryObjectFromReference(linkRefer) as Face;//通过reference获取几何

                roofFaces.Add(face);
            }
            else
            {
                roofFaces.Add(element02.GetGeometryObjectFromReference(reference02) as Face);
            }
            if (roofFaces.Count < 0)
                throw new NotImplementedException("请选择一个顶面");

            //计算投影线
            Curve curve = familyInstance01.LocationCurve();
            //将投影线与链接模型中的图元坐标系保持一致
            //curve = curve.CreateTransformed(transform);

            curve = ConBeamFollowFace.ProjectToRoof(curve, roofFaces);
            //将投影后的转回原模型中的坐标系
            curve = curve.CreateTransformed(transform);

            if (curve == null)
                throw new NotImplementedException("计算投影线失败");

            //生成新的梁
            try
            {
                using (Transaction trans = new Transaction(CMD.Doc, "修改标高"))
                {
                    trans.DeleteErrOrWaringTaskDialog();
                    trans.Start();

                    FamilyInstance familyInstance = CMD.Doc.Create.NewFamilyInstance(curve, familyInstance01.Symbol, CMD.Doc.ActiveView.GenLevel, StructuralType.Beam); ;

                    familyInstance.LookupParameter("宽度").Set(familyInstance01.LookupParameter("宽度").AsDouble());
                    familyInstance.LookupParameter("高度").Set(familyInstance01.LookupParameter("高度").AsDouble());

                    // 结构分析
                    Parameter parameter = familyInstance.get_Parameter(BuiltInParameter.STRUCTURAL_ANALYTICAL_MODEL);
                    if (parameter != null && !parameter.IsReadOnly)
                        parameter.Set(0);
                    // 全部刷成无连接模式
                    parameter = familyInstance.get_Parameter(BuiltInParameter.STRUCT_FRAM_JOIN_STATUS);
                    if (parameter != null && !parameter.IsReadOnly)
                        parameter.Set(1);
                    trans.Commit();
                }
            }
            catch (Exception)
            {
                throw new NotImplementedException("创建面吸附梁失败");
                //throw;
            }
            //删除旧梁
            using (Transaction trans = new Transaction(CMD.Doc, "修改标高"))
            {
                trans.DeleteErrOrWaringTaskDialog();
                trans.Start();
                CMD.Doc.Delete(familyInstance01.Id);
                trans.Commit();
            }
            //throw new NotImplementedException();

            this.sw.Stop();
            //$"{sw.ElapsedMilliseconds} + 毫秒".TaskDialogErrorMessage();
        }
    }
}
