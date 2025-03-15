//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//using g3;
//using Autodesk.Revit.DB;

//namespace ParkingLayoutEfficientNewStructual
//{
//    /// <summary>
//    /// 用来对当前扫描出的最大面积区域进行临时车位排布，进一步优化车位排布空间
//    /// </summary>
//    abstract class MaxAreaPolygonCalculateTemp
//    {
//        internal double count =0;// 计算出的可停车数量
//        internal MaxAreaPolygon OldMaxAreaRectangle;
//        internal MaxAreaPolygon NewMaxAreaRectangle;
//        internal Document  this.Doc;

//        public MaxAreaPolygonCalculateTemp(MaxAreaPolygon maxAreaRectangle, Document doc)
//        {
//            this.OldMaxAreaRectangle = maxAreaRectangle;
//            this. this.Doc = doc;
//        }
//        internal void ReCalculate()
//        {
//            double tempCount = 0;
//            this.NewMaxAreaRectangle = ReCalculateMaxAreaRectangle_Normal(this.OldMaxAreaRectangle, ref tempCount);
//            this.count = tempCount;
//        }

//        /// <summary>
//        /// 将车道方向设置为水平/垂直方向，进行临时排车位计算，进行算量
//        /// </summary>
//        /// <param name="maxAreaRectangle"></param>
//        /// <param name="count"></param>
//        /// <returns></returns>
//        internal abstract MaxAreaPolygon ReCalculateMaxAreaRectangle_Normal(MaxAreaPolygon maxAreaRectangle, ref double count);
//        /// <summary>
//        /// 预先排车位的前置条件
//        /// </summary>
//        /// <returns></returns>
//        internal abstract MaxAreaPolygon Preconditions(double bottomScanLineAngle, BoundarySegment bottomBoundarySegment, BoundarySegment topBoundarySegment, BoundarySegment leftBoundarySegment, BoundarySegment rightBoundarySegment,
//            Vector2d vector2dZreo, double carHeight, double carWidth, double carRoadWidth, double columnWidth, double columnBurfferDistance, ref double bottomCount);
//    }
//}
