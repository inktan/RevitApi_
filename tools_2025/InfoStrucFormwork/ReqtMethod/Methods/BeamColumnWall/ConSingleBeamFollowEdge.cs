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

    internal class ConSingleBeamFollowEdge : RequestMethod
    {
        internal ConSingleBeamFollowEdge(UIApplication _uiApp) : base(_uiApp)
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
