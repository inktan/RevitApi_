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

    internal class ConSingleColFollowEdge : RequestMethod
    {
        internal ConSingleColFollowEdge(UIApplication _uiApp) : base(_uiApp)
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
            //转为剖面坐标系
            while (true)
            {
                try
                {
                    //选一个基准线
                    Reference colReference = CMD.Sel.PickObject(ObjectType.Edge);
                    Element col = CMD.Doc.GetElement(colReference.ElementId);

                    if (col is FamilyInstance fi)
                    {
                        string faName = fi.Symbol.FamilyName;
                        if (faName.Contains("柱") || faName.Contains("墙"))
                        {
                            Edge colEdge = col.GetGeometryObjectFromReference(colReference) as Edge;
                            Curve colBaseLine = colEdge.AsCurve();

                            Transform colTransform = fi.GetTransform();
                            colBaseLine = colBaseLine.CreateTransformed(colTransform);

                            // 将线的起点和终点转换到剖面视图的坐标系
                            XYZ p0 = transform.Inverse.OfPoint(colBaseLine.GetEndPoint(0));
                            XYZ p1 = transform.Inverse.OfPoint(colBaseLine.GetEndPoint(1));
                            //(startPoint.ToString() + "\n" + endPoint.ToString() + "\n" + p0.ToString() + "\n" + p1.ToString()).TaskDialogErrorMessage();
                            Line2d colBaseL2d02 = new Line2d(new Vector2d(p1.X, p1.Y), new Vector2d(0, 1));

                            double tarHeight = 0; // 高度差
                            IntrLine2Line2 intrLine2Line201 = new IntrLine2Line2(baseL2d, colBaseL2d02);
                            intrLine2Line201.Compute();
                            if (intrLine2Line201.Quantity == 1)
                            {
                                tarHeight = intrLine2Line201.Point.y - p0.Y;
                                if (p1.Y > p0.Y)
                                {
                                    tarHeight = intrLine2Line201.Point.y - p1.Y;
                                }
                            }

                            //顶部偏移
                            double baseOffset = fi.get_Parameter(BuiltInParameter.SCHEDULE_TOP_LEVEL_OFFSET_PARAM).AsDouble();

                            using (Transaction _trans = new Transaction(this.doc, "修改墙高与柱高"))
                            {
                                _trans.Start();
                                _trans.DeleteErrOrWaringTaskDialog();
                                //顶部偏移
                                fi.get_Parameter(BuiltInParameter.FAMILY_TOP_LEVEL_OFFSET_PARAM).Set(baseOffset + tarHeight);
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

    }
}
