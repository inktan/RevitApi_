using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PublicProjectMethods_
{
    public static class Dimension_
    {
        #region Dimension

        /// <summary>
        /// 尺寸标注单根直线元素-墙体
        /// </summary>
        public static Dimension DimensionSingeleWall(this Document doc, Wall wall)
        {
            Dimension dimension = null;
            // 【】求出尺寸线所在位置
            Line line = (wall.Location as LocationCurve).Curve as Line;
            XYZ wallDirection = line.Direction;

            ReferenceArray referenceArray = new ReferenceArray();

            // 【】求出dimension可参照几何体
            Options options = new Options();
            options.View = doc.ActiveView;
            options.ComputeReferences = true;
            GeometryElement geometryElement = wall.get_Geometry(options);
            Solid solid = geometryElement.First() as Solid;

            // 【方法一】求出墙体两侧边的Edge-reference
            //List<Edge> edges = solid.Edges.ToIEnumerable().ToList();
            //edges = edges.OrderBy(p => p.ApproximateLength).ToList();// 边长排序
            //edges = edges.GetRange(0, 4);
            //edges = edges.OrderBy(p => p.AsCurve().GetEndPoint(0).Z).ToList();// y轴排序
            //edges = edges.GetRange(0, 2);
            //foreach (Edge edge in edges)
            //    referenceArray.Append(edge.Reference);

            // 【方法二】求出墙体两侧边的Face
            List<Face> faces = solid.Faces.ToIEnumerable().ToList();
            foreach (Face face in faces)
            {
                XYZ normalXyz = face.ComputeNormal(new UV());
                if (Math.Abs(normalXyz.DotProduct(wallDirection)).EqualPrecision(1))
                    referenceArray.Append(face.Reference);
            }

            // 【】移动标注线的位置
            //double distanceMove = 2000.0.MilliMeterToFeet();
            //distanceMove = distanceMove.MilliMeterToFeet();

            //XYZ xyzDirection = line.Direction.CrossProduct(new XYZ(0, 0, 1)).Normalize();

            //line = line.CreateOffset(distanceMove, xyzDirection) as Line;

            using (Transaction trans = new Transaction(doc, "轴网端头尺寸标注"))
            {
                trans.Start();
                dimension = doc.Create.NewDimension(doc.ActiveView, line, referenceArray);
                trans.Commit();
            }
            return dimension;
        }
        /// <summary>
        /// 尺寸标注单根直线元素-详图线 模型线
        /// </summary>
        public static Dimension DimensionLineElement(this Document doc, CurveElement curveElement)
        {
            Dimension dimension = null;
            if (!(curveElement is DetailLine) && !(curveElement is ModelLine))
                return dimension;

            ReferenceArray referenceArray = new ReferenceArray();

            // 方法一 
            //Options options = new Options();
            //options.View = doc.ActiveView;
            //options.ComputeReferences = true;
            //GeometryElement geometryElement = curveElement.get_Geometry(options);
            //Line line = geometryElement.First() as Line;

            // 方法二
            Line line = curveElement.GeometryCurve as Line;
            referenceArray.Append(line.GetEndPointReference(0));
            referenceArray.Append(line.GetEndPointReference(1));
            // 方法三 无效
            // Line line = (curveElement.Location as LocationCurve).Curve as Line;

            // 【】移动标注线的位置
            //double distanceMove = 2000.0.MilliMeterToFeet();
            //distanceMove = distanceMove.MilliMeterToFeet();

            //XYZ xyzDirection = line.Direction.CrossProduct(new XYZ(0,0,1)).Normalize();

            //line = line.CreateOffset(distanceMove, xyzDirection) as Line;

            using (Transaction trans = new Transaction(doc, "轴网端头尺寸标注"))
            {
                trans.Start();
                dimension = doc.Create.NewDimension(doc.ActiveView, line, referenceArray);
                trans.Commit();
            }
            return dimension;
        }
        /// <summary>
        /// 尺寸标注双根直线元素间距 -详图线、模型线，要求：方向相同，起点共线方向与轴网方向垂直
        /// </summary>
        public static Dimension DimensionBetweenCurveElements(this Document doc, CurveElement curveElement01, CurveElement curveElement02)
        {
            Dimension dimension = null;
            if (!(((curveElement01 is DetailLine) || (curveElement01 is ModelLine)) && ((curveElement02 is DetailLine) || (curveElement02 is ModelLine))))
                return dimension;
            ReferenceArray referenceArray = new ReferenceArray();

            referenceArray.Append(curveElement01.GeometryCurve.Reference);
            referenceArray.Append(curveElement02.GeometryCurve.Reference);

            Curve curve01 = curveElement01.GeometryCurve;
            Curve curve02 = curveElement02.GeometryCurve;
            Line line = Line.CreateBound(curve01.GetEndPoint(1), curve02.GetEndPoint(1));

            // 【】移动标注线的位置
            //double distanceMove = 2000.0.MilliMeterToFeet();
            //distanceMove = distanceMove.MilliMeterToFeet();
            //line = line.CreateOffset(distanceMove, (curve01 as Line).Direction) as Line;

            using (Transaction trans = new Transaction(doc, "轴网端头尺寸标注"))
            {
                trans.Start();
                dimension = doc.Create.NewDimension(doc.ActiveView, line, referenceArray);
                trans.Commit();
            }
            return dimension;
        }
        /// <summary>
        /// 尺寸标注双根直线元素间距--轴网，要求：轴网方向相同，起点共线方向与轴网方向垂直
        /// </summary>
        public static Dimension DimensionBetweenGrids(this Document doc, Grid grid01, Grid grid02)
        {
            Dimension dimension = null;
            ReferenceArray referenceArray = new ReferenceArray();
            Reference reference01 = new Reference(grid01);
            Reference reference02 = new Reference(grid02);
            referenceArray.Append(reference01);
            referenceArray.Append(reference02);

            Curve curve01 = grid01.Curve;
            Curve curve02 = grid02.Curve;
            Line line = Line.CreateBound(curve01.GetEndPoint(1), curve02.GetEndPoint(1));
            // 【】移动标注线的位置
            double distanceMove = 2000.0.MilliMeterToFeet();
            line = line.CreateOffset(distanceMove, (curve01 as Line).Direction) as Line;
            using (Transaction trans = new Transaction(doc, "轴网端头尺寸标注"))
            {
                trans.Start();
                dimension = doc.Create.NewDimension(doc.ActiveView, line, referenceArray);
                trans.Commit();
            }
            return dimension;
        }

        #endregion
    }
}
