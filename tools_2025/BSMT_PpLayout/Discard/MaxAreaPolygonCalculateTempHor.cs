//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//using g3;
//using Autodesk.Revit.DB;
//using CommonMethod_g3;
//using PublicProjectMethods_;

//namespace ParkingLayoutEfficientNewStructual
//{
//    /// <summary>
//    /// 用来对当前扫描出的最大面积区域进行临时车位排布，进一步优化车位排布空间
//    /// </summary>
//    class MaxAreaPolygonCalculateTempHor: MaxAreaPolygonCalculateTemp
//    {
//        public MaxAreaPolygonCalculateTempHor(MaxAreaPolygon maxAreaRectangle, Document doc):base(maxAreaRectangle,  doc)
//        {
//            this.OldMaxAreaRectangle = maxAreaRectangle;
//            this. this.Doc = doc;
//        }

//        /// <summary>
//        /// 将车道方向设置为垂直方向，进行临时排车位计算，进行算量
//        /// </summary>
//        /// <param name="maxAreaRectangle"></param>
//        /// <param name="count"></param>
//        /// <returns></returns>
//        internal override MaxAreaPolygon ReCalculateMaxAreaRectangle_Normal(MaxAreaPolygon maxAreaRectangle, ref double count)
//        {
//            // 由于是停车子区域内部计算，使用次车道宽度
//            double carRoadWidth = InputParameter.Wd;
//            double carHeight = InputParameter.pSHeight;
//            double carWidth = InputParameter.pSWidth;
//            double columnWidth = InputParameter.columnWidth;
//            double columnBurfferDistance = InputParameter.columnBurfferDistance;

//            #region 需要对四条边进行判断，是否满足停车要求
//            //【】将图形旋转到基准扫描线位置
//            double bottomScanLineAngle = maxAreaRectangle.BottomScanLineAngle;
//            Vector2d vector2dZreo = maxAreaRectangle.Polygon2d.Start;// 该处可以省略
//            MaxAreaPolygon maxAreaRectangle1 = maxAreaRectangle.RotateTransform(bottomScanLineAngle, vector2dZreo);

//            // 数据输入: 判断扫描线属性，为车道，还是障碍物（当前情况，底部属性线为车道空间相关的线）
//            BoundarySegment bottomBoundarySegment = maxAreaRectangle1.BottomBoundarySegment;
//            BoundarySegment topBoundarySegment = maxAreaRectangle1.TopBoundarySegment;
//            BoundarySegment leftBoundarySegment = maxAreaRectangle1.LeftBoundarySegment;
//            BoundarySegment rightBoundarySegment = maxAreaRectangle1.RightBoundarySegment;

//            MaxAreaPolygon maxAreaPolygon = Preconditions(bottomScanLineAngle, bottomBoundarySegment, topBoundarySegment, leftBoundarySegment, rightBoundarySegment,
//         vector2dZreo, carHeight, carWidth, carRoadWidth, columnWidth, columnBurfferDistance, ref count);

//            return maxAreaPolygon;
//            #endregion
//        }
//        /// <summary>
//        /// 预先排车位的前置条件
//        /// </summary>
//        /// <returns></returns>
//        internal override MaxAreaPolygon Preconditions(double bottomScanLineAngle, BoundarySegment bottomBoundarySegment, BoundarySegment topBoundarySegment, BoundarySegment leftBoundarySegment, BoundarySegment rightBoundarySegment,
//            Vector2d vector2dZreo ,double carHeight, double carWidth, double carRoadWidth, double columnWidth, double columnBurfferDistance, ref double bottomCount)
//        {
//            Segment2d bottomSegment = bottomBoundarySegment.segment2d;
//            Segment2d topSegment = topBoundarySegment.segment2d;

//            Vector2d p_0 = bottomSegment.P0;
//            Vector2d p_1 = bottomSegment.P1;
//            Vector2d _p_0 = topSegment.P1;
//            Vector2d _p_1 = topSegment.P0;
//            double regionHeight = Math.Abs(_p_0.y - p_0.y);
//            double regionWidth = bottomSegment.Length;

//            if(topBoundarySegment.LineStyleId == LineStyleId.PrimaryLaneId)
//            {
//                if (leftBoundarySegment.LineStyleId == LineStyleId.PrimaryLaneId || rightBoundarySegment.LineStyleId == LineStyleId.PrimaryLaneId)
//                {
//                    bottomCount = ArrangeParkingInAdvanceWithPrimaryLaneId(regionHeight, regionWidth, carHeight, carWidth, carRoadWidth, columnWidth, columnBurfferDistance, ref p_0, ref p_1, ref _p_0, ref _p_1, ref topBoundarySegment, ref rightBoundarySegment);
//                }
//                else if (leftBoundarySegment.LineStyleId != LineStyleId.PrimaryLaneId && rightBoundarySegment.LineStyleId != LineStyleId.PrimaryLaneId)
//                {
//                    //bottomCount = ArrangeParkingInAdvanceWithPrimaryLaneId(regionHeight, regionWidth, carHeight, carWidth, carRoadWidth, columnWidth, columnBurfferDistance, ref p_0, ref p_1, ref _p_0, ref _p_1, ref topBoundarySegment, ref rightBoundarySegment);
//                    bottomCount = ArrangeParkingInAdvanceWithPrimaryLaneIdWithLeftRightObstal(regionHeight, regionWidth, carHeight, carWidth, carRoadWidth, columnWidth, columnBurfferDistance, ref p_0, ref p_1, ref _p_0, ref _p_1, ref topBoundarySegment, ref rightBoundarySegment);
//                }
//            }
//            else if(topBoundarySegment.LineStyleId != LineStyleId.PrimaryLaneId)
//            {
//                if (leftBoundarySegment.LineStyleId == LineStyleId.PrimaryLaneId || rightBoundarySegment.LineStyleId == LineStyleId.PrimaryLaneId)
//                {
//                    bottomCount = ArrangeParkingInAdvanceWithTopObstal(regionHeight, regionWidth, carHeight, carWidth, carRoadWidth, columnWidth, columnBurfferDistance, ref p_0, ref p_1, ref _p_0, ref _p_1, ref topBoundarySegment, ref rightBoundarySegment);
//                }
//                else if (leftBoundarySegment.LineStyleId != LineStyleId.PrimaryLaneId && rightBoundarySegment.LineStyleId != LineStyleId.PrimaryLaneId)
//                {
//                    //bottomCount = ArrangeParkingInAdvanceWithTopObstal(regionHeight, regionWidth, carHeight, carWidth, carRoadWidth, columnWidth, columnBurfferDistance, ref p_0, ref p_1, ref _p_0, ref _p_1, ref topBoundarySegment, ref rightBoundarySegment);
//                    bottomCount = ArrangeParkingInAdvanceWithTopObstalWithLeftRightObstal(regionHeight, regionWidth, carHeight, carWidth, carRoadWidth, columnWidth, columnBurfferDistance, ref p_0, ref p_1, ref _p_0, ref _p_1, ref topBoundarySegment, ref rightBoundarySegment);
//                }
//            }

//            Polygon2d polygon2d = new Polygon2d(new List<Vector2d>() { p_0, p_1, _p_1, _p_0 });
//            // 侧边界

//            //leftBoundarySegment = new BoundarySegment(new Segment2d(_p_0, _p_0), leftBoundarySegment.LineStyleId);
//            rightBoundarySegment = new BoundarySegment(new Segment2d(p_1, _p_1), rightBoundarySegment.LineStyleId);

//            // 记录当前最大矩形面积的信息，线圈、基准投影线、（需要属性说明）
//            MaxAreaPolygon newMaxAreaRectangle = new MaxAreaPolygon(polygon2d)
//            {
//                BottomBoundarySegment = bottomBoundarySegment,
//                TopBoundarySegment = topBoundarySegment,
//                LeftBoundarySegment = leftBoundarySegment,
//                RightBoundarySegment = rightBoundarySegment,
//                PolygonHeight = Math.Abs(_p_0.y - p_0.y),
//                ScanLineAngle = bottomScanLineAngle,// 此处的角度需要注意
//                SelfBoundarySegments = new List<BoundarySegment>() { bottomBoundarySegment, topBoundarySegment, leftBoundarySegment, rightBoundarySegment },
//            };

//            // 这里需要判断，如果count为0，则返回空值
//            if(bottomCount!=0)
//            {
//                return newMaxAreaRectangle.RotateTransform(-bottomScanLineAngle, vector2dZreo);
//            }
//            else
//            {
//                return new MaxAreaPolygon(new Polygon2d());
//            }
//        }

//        /// <summary>
//        /// 车位预排布-顶部为车道空间 左右边界线属性至少满足其中一边不是障碍物属性
//        /// </summary>
//        internal  double ArrangeParkingInAdvanceWithPrimaryLaneId(double regionHeight,double regionWidth, double carHeight, double carWidth, double carRoadWidth,double columnWidth,double columnBurfferDistance,
//             ref Vector2d p_0, ref Vector2d p_1, ref Vector2d _p_0, ref Vector2d _p_1, ref BoundarySegment topBoundarySegment, ref BoundarySegment rightBoundarySegment)
//        {
//            // 基于当前尺寸的高度与宽度计算可停车数量
//            #region  几行车
//            double needHeight = 0;
//            int i = 0;// i代表的是第几排车
//            int rows = 0;
//            while (needHeight <= regionHeight + Precision_.Precison)
//            {
//                if (i % 2 == 0 && i != 0)
//                {
//                    needHeight += carHeight;
//                    if (needHeight - regionHeight > Precision_.Precison) break;
//                    else
//                    {
//                        //if (Math.Abs(needHeight - regionHeight) < carHeight + InputParameter.PRECISION)
//                        //{
//                        //    needHeight = maxAreaRectangle.PolygonHeight;
//                        //}
//                        _p_0 = new Vector2d(p_0.x, p_0.y + needHeight);
//                        _p_1 = new Vector2d(p_1.x, p_1.y + needHeight);
//                        topBoundarySegment = new BoundarySegment(new Segment2d(_p_1, _p_0), LineStyleId.PrimaryLaneId);// 两排车对应的是道路
//                        rows += 1;
//                    }
//                }
//                else if (i % 2 == 1)
//                {
//                    if(i==1)
//                    {
//                        needHeight += carHeight;
//                    }
//                    else
//                    {
//                        needHeight += carHeight + carRoadWidth;
//                    }
//                    // 这里需要注意，计算的最后一排车，本身假定情况就是面对主车道，因此，这里直接将底部与顶部的实际距离，提取出来赋给最新生成区域；
//                    if (needHeight - regionHeight > Precision_.Precison)
//                    {
//                        // 这里的判断逻辑不可以更改，特殊情况，车头距离主车道有一定距离，但是该距离不满足（车长+次通车道宽度），该距离空间不可以继续停车
//                        _p_0 = new Vector2d(p_0.x, p_0.y + regionHeight);
//                        _p_1 = new Vector2d(p_1.x, p_1.y + regionHeight);
//                        topBoundarySegment = new BoundarySegment(new Segment2d(_p_1, _p_0), LineStyleId.PrimaryLaneId);//虽然该单数排没有出现，但是该区域需要统计，但是车的排数不需要统计
//                        break;
//                    }
//                    else
//                    {
//                        _p_0 = new Vector2d(p_0.x, p_0.y + needHeight);
//                        _p_1 = new Vector2d(p_1.x, p_1.y + needHeight);
//                        topBoundarySegment = new BoundarySegment(new Segment2d(_p_1, _p_0), LineStyleId.CarTailId);// 三排车对应的是车尾
//                        rows += 1;
//                    }
//                }
//                i++;
//            }
//            #endregion

//            #region 几列车 判断机制，自动加入通车道宽度
//            int columns = CalColumns(regionWidth, carWidth, columnWidth, columnBurfferDistance, ref p_0, ref p_1, ref _p_0, ref _p_1, ref rightBoundarySegment);
//            #endregion
//            return rows * columns;
//        }
//        /// <summary>
//        /// 车位预排布-顶部为障碍物属性 左右边界线属性至少满足其中一边不是障碍物属性
//        /// </summary>
//        internal  double ArrangeParkingInAdvanceWithTopObstal(double regionHeight, double regionWidth, double carHeight, double carWidth, double carRoadWidth, double columnWidth, double columnBurfferDistance,
//       ref Vector2d p_0, ref Vector2d p_1, ref Vector2d _p_0, ref Vector2d _p_1, ref BoundarySegment topBoundarySegment, ref BoundarySegment rightBoundarySegment)
//        {
//            // 基于当前尺寸的高度与宽度计算可停车数量
//            #region  几行车
//            double needHeight = 0;
//            int i = 0;// i代表的是第几排车
//            int rows = 0;
//            while (needHeight <= regionHeight + Precision_.Precison)
//            {
//                if (i % 2 == 0 && i != 0)
//                {
//                    needHeight += carHeight + carRoadWidth;
//                    if (needHeight - regionHeight > Precision_.Precison) break;
//                    else
//                    {
//                        //if (Math.Abs(needHeight - regionHeight) < carHeight + InputParameter.PRECISION)
//                        //{
//                        //    needHeight = maxAreaRectangle.PolygonHeight;
//                        //}
//                        _p_0 = new Vector2d(p_0.x, p_0.y + needHeight);
//                        _p_1 = new Vector2d(p_1.x, p_1.y + needHeight);
//                        topBoundarySegment = new BoundarySegment(new Segment2d(_p_1, _p_0), LineStyleId.PrimaryLaneId);// 两排车对应的是道路
//                        rows += 1;
//                    }
//                }
//                else if (i % 2 == 1)
//                {
//                    needHeight += carHeight;
//                    if (needHeight - regionHeight > Precision_.Precison) break;
//                    else
//                    {
//                        _p_0 = new Vector2d(p_0.x, p_0.y + needHeight);
//                        _p_1 = new Vector2d(p_1.x, p_1.y + needHeight);
//                        topBoundarySegment = new BoundarySegment(new Segment2d(_p_1, _p_0), LineStyleId.CarTailId);// 三排车对应的是车尾
//                        rows += 1;
//                    }
//                }
//                i++;
//            }
//            #endregion

//            #region 几列车 判断机制，自动加入通车道宽度
//            int columns = CalColumns(regionWidth, carWidth, columnWidth, columnBurfferDistance, ref p_0, ref p_1, ref _p_0, ref _p_1, ref rightBoundarySegment);
//            #endregion
//            return rows * columns;
//        }
//        /// <summary>
//        /// 车位预排布-顶部为车道空间 左右边界线属性均为障碍物属性 需要考虑流出通车道的距离
//        /// </summary>
//        internal  double ArrangeParkingInAdvanceWithPrimaryLaneIdWithLeftRightObstal(double regionHeight, double regionWidth, double carHeight, double carWidth, double carRoadWidth, double columnWidth, double columnBurfferDistance,
//             ref Vector2d p_0, ref Vector2d p_1, ref Vector2d _p_0, ref Vector2d _p_1, ref BoundarySegment topBoundarySegment, ref BoundarySegment rightBoundarySegment)
//        {
//            // 基于当前尺寸的高度与宽度计算可停车数量
//            #region  几行车
//            double needHeight = 0;
//            int i = 0;// i代表的是第几排车
//            int rows = 0;

//            //regionWidth的距离不能放置一个通车道宽度，则只填一个车位
//            if (regionWidth < InputParameter.Wd_main + carWidth)// 该情况不留通车道空间
//            {
//                rows = 1;
//                _p_0 = new Vector2d(p_0.x, p_0.y + carHeight);
//                _p_1 = new Vector2d(p_1.x, p_1.y + carHeight);
//                topBoundarySegment = new BoundarySegment(new Segment2d(_p_1, _p_0), LineStyleId.CarTailId);// 三排车对应的是车尾
//            }
//            else
//            {
//                while (needHeight <= regionHeight + Precision_.Precison)
//                {
//                    if (i % 2 == 0 && i != 0)
//                    {
//                        needHeight += carHeight;
//                        if (needHeight - regionHeight > Precision_.Precison) break;
//                        else
//                        {
//                            //if (Math.Abs(needHeight - regionHeight) < carHeight + InputParameter.PRECISION)
//                            //{
//                            //    needHeight = maxAreaRectangle.PolygonHeight;
//                            //}
//                            _p_0 = new Vector2d(p_0.x, p_0.y + needHeight);
//                            _p_1 = new Vector2d(p_1.x, p_1.y + needHeight);
//                            topBoundarySegment = new BoundarySegment(new Segment2d(_p_1, _p_0), LineStyleId.PrimaryLaneId);// 两排车对应的是道路
//                            rows += 1;
//                        }
//                    }
//                    else if (i % 2 == 1)
//                    {
//                        if (i == 1)
//                        {
//                            needHeight += carHeight;
//                        }
//                        else
//                        {
//                            needHeight += carHeight + carRoadWidth;
//                        }
//                        // 这里需要注意，计算的最后一排车，本身假定情况就是面对主车道，因此，这里直接将底部与顶部的实际距离，提取出来赋给最新生成区域；
//                        if (needHeight - regionHeight > Precision_.Precison)
//                        {
//                            // 这里的判断逻辑不可以更改，特殊情况，车头距离主车道有一定距离，但是该距离不满足（车长+次通车道宽度），该距离空间不可以继续停车
//                            _p_0 = new Vector2d(p_0.x, p_0.y + regionHeight);
//                            _p_1 = new Vector2d(p_1.x, p_1.y + regionHeight);
//                            topBoundarySegment = new BoundarySegment(new Segment2d(_p_1, _p_0), LineStyleId.PrimaryLaneId);//虽然该单数排没有出现，但是该区域需要统计，但是车的排数不需要统计
//                            break;
//                        }
//                        else
//                        {
//                            _p_0 = new Vector2d(p_0.x, p_0.y + needHeight);
//                            _p_1 = new Vector2d(p_1.x, p_1.y + needHeight);
//                            topBoundarySegment = new BoundarySegment(new Segment2d(_p_1, _p_0), LineStyleId.CarTailId);// 三排车对应的是车尾
//                            rows += 1;
//                        }
//                    }
//                    i++;
//                }

//            }
//            #endregion

//            #region 几列车 判断机制，自动加入通车道宽度
//            int columns = CalColumnsWithTopObstalWithLeftRightObstal(regionWidth, carWidth, columnWidth, columnBurfferDistance, ref p_0, ref p_1, ref _p_0, ref _p_1, ref rightBoundarySegment);
//            #endregion
//            return rows * columns;
//        }
//        /// <summary>
//        /// 车位预排布-顶部为障碍物属性 左右边界线属性均为障碍物属性 需要考虑流出通车道的距离
//        /// </summary>
//        internal  double ArrangeParkingInAdvanceWithTopObstalWithLeftRightObstal(double regionHeight, double regionWidth, double carHeight, double carWidth, double carRoadWidth, double columnWidth, double columnBurfferDistance,
//       ref Vector2d p_0, ref Vector2d p_1, ref Vector2d _p_0, ref Vector2d _p_1, ref BoundarySegment topBoundarySegment, ref BoundarySegment rightBoundarySegment)
//        {
//            if (true)
//            {

//            }
//            // 基于当前尺寸的高度与宽度计算可停车数量
//            #region  几行车
//            double needHeight = 0;
//            int i = 0;// i代表的是第几排车
//            int rows = 0;

//            //regionWidth的距离不能放置一个通车道宽度，则只填一个车位
//            if (regionWidth < InputParameter.Wd_main + carWidth)// 该情况不留通车道空间
//            {
//                rows = 1;
//                _p_0 = new Vector2d(p_0.x, p_0.y + carHeight);
//                _p_1 = new Vector2d(p_1.x, p_1.y + carHeight);
//                topBoundarySegment = new BoundarySegment(new Segment2d(_p_1, _p_0), LineStyleId.CarTailId);// 三排车对应的是车尾
//            }
//            else
//            {
//                while (needHeight <= regionHeight + Precision_.Precison)
//                {
//                    if (i % 2 == 0 && i != 0)
//                    {
//                        needHeight += carHeight + carRoadWidth;
//                        if (needHeight - regionHeight > Precision_.Precison) break;
//                        else
//                        {
//                            //if (Math.Abs(needHeight - regionHeight) < carHeight + InputParameter.PRECISION)
//                            //{
//                            //    needHeight = maxAreaRectangle.PolygonHeight;
//                            //}
//                            _p_0 = new Vector2d(p_0.x, p_0.y + needHeight);
//                            _p_1 = new Vector2d(p_1.x, p_1.y + needHeight);
//                            topBoundarySegment = new BoundarySegment(new Segment2d(_p_1, _p_0), LineStyleId.PrimaryLaneId);// 两排车对应的是道路
//                            rows += 1;
//                        }
//                    }
//                    else if (i % 2 == 1)
//                    {
//                        needHeight += carHeight;
//                        if (needHeight - regionHeight > Precision_.Precison) break;
//                        else
//                        {
//                            _p_0 = new Vector2d(p_0.x, p_0.y + needHeight);
//                            _p_1 = new Vector2d(p_1.x, p_1.y + needHeight);
//                            topBoundarySegment = new BoundarySegment(new Segment2d(_p_1, _p_0), LineStyleId.CarTailId);// 三排车对应的是车尾
//                            rows += 1;
//                        }
//                    }
//                    i++;
//                }

//            }

//            #endregion

//            int columns = CalColumnsWithTopObstalWithLeftRightObstal(regionWidth, carWidth, columnWidth, columnBurfferDistance, ref p_0, ref p_1, ref _p_0, ref _p_1, ref rightBoundarySegment);

//            return rows * columns;
//        }
//        /// <summary>
//        ///  几列车 判断机制，自动加入通车道宽度
//        /// </summary>
//        internal  int CalColumnsWithTopObstalWithLeftRightObstal(double regionWidth, double carWidth,  double columnWidth, double columnBurfferDistance,ref Vector2d p_0, ref Vector2d p_1, ref Vector2d _p_0, ref Vector2d _p_1, ref BoundarySegment rightBoundarySegment)
//        {
//            #region 几列车 判断机制，自动加入通车道宽度
//            double needWidth = 0;// 默认留一个主车道宽度
//            int j = 0;
//            int columns = 0;
//            //regionWidth的距离不能放置一个通车道宽度，则只填一个车位
//            if (regionWidth < InputParameter.Wd_main + carWidth)// 该情况不留通车道空间
//            {
//                while (needWidth <= regionWidth + Precision_.Precison)// 这里使用的是与次车道垂直的主车道宽度
//                {
//                    if (j > 0)
//                    {
//                        if (j % 3 == 1)// 1-4-7-10余数为1时，添加柱子空间
//                        {
//                            needWidth += columnWidth + columnBurfferDistance * 2 + carWidth;
//                            if (needWidth - regionWidth > Precision_.Precison) break;
//                            {
//                                columns += 1;
//                                p_1 = new Vector2d(p_0.x + needWidth, p_0.y);
//                                _p_1 = new Vector2d(_p_0.x + needWidth, _p_0.y);
//                                rightBoundarySegment = new BoundarySegment(new Segment2d(p_1, _p_1), LineStyleId.BroadsideId);
//                            }
//                        }
//                        else
//                        {
//                            needWidth += carWidth;
//                            if (needWidth - regionWidth > Precision_.Precison) break;
//                            {
//                                columns += 1;
//                                p_1 = new Vector2d(p_0.x + needWidth, p_0.y);
//                                _p_1 = new Vector2d(_p_0.x + needWidth, _p_0.y);
//                                rightBoundarySegment = new BoundarySegment(new Segment2d(p_1, _p_1), LineStyleId.BroadsideId);
//                            }
//                        }
//                    }
//                    j++;
//                }
//            }
//            else
//            {
//                // 矛盾的地方，到底留不留路宽 留路宽，但是不统计该路宽空间上的车位数量
//                //needWidth += InputParameter.Wd_main;
//                //needWidth += InputParameter.Wd_main;
//                while (needWidth  <= regionWidth + Precision_.Precison)// 这里使用的是与次车道垂直的主车道宽度
//                {
//                    if (j > 0)
//                    {
//                        if (j % 3 == 1)// 1-4-7-10余数为1时，添加柱子空间
//                        {
//                            needWidth += columnWidth + columnBurfferDistance * 2 + carWidth;
//                            if (needWidth- regionWidth > Precision_.Precison) break;
//                            {
//                                columns += 1;
//                                p_1 = new Vector2d(p_0.x + needWidth , p_0.y);
//                                _p_1 = new Vector2d(_p_0.x + needWidth , _p_0.y);
//                                //p_1 = new Vector2d(p_0.x + needWidth - InputParameter.Wd_main, p_0.y);
//                                //_p_1 = new Vector2d(_p_0.x + needWidth - InputParameter.Wd_main, _p_0.y);
//                                rightBoundarySegment = new BoundarySegment(new Segment2d(p_1, _p_1), LineStyleId.BroadsideId);
//                            }
//                        }
//                        else
//                        {
//                            needWidth += carWidth;
//                            if (needWidth - regionWidth > Precision_.Precison) break;
//                            {
//                                columns += 1;
//                                p_1 = new Vector2d(p_0.x + needWidth , p_0.y);
//                                _p_1 = new Vector2d(_p_0.x + needWidth , _p_0.y);
//                                //p_1 = new Vector2d(p_0.x + needWidth - InputParameter.Wd_main, p_0.y);
//                                //_p_1 = new Vector2d(_p_0.x + needWidth - InputParameter.Wd_main, _p_0.y);
//                                rightBoundarySegment = new BoundarySegment(new Segment2d(p_1, _p_1), LineStyleId.BroadsideId);
//                            }
//                        }
//                    }
//                    j++;
//                }
//            }
//            #endregion
//            return columns;
//        }
//        /// <summary>
//        ///  几列车
//        /// </summary>
//        internal  int CalColumns(double regionWidth, double carWidth, double columnWidth, double columnBurfferDistance,ref Vector2d p_0, ref Vector2d p_1, ref Vector2d _p_0, ref Vector2d _p_1, ref BoundarySegment rightBoundarySegment)
//        {
//            #region 几列车 判断机制，自动加入通车道宽度
//            double needWidth = 0;
//            int j = 0;
//            int columns = 0;
//            while (needWidth <= regionWidth + Precision_.Precison)
//            {
//                if (j > 0)
//                {
//                    if (j % 3 == 1)// 1-4-7-10余数为1时，添加柱子空间
//                    {
//                        needWidth += columnWidth + columnBurfferDistance * 2 + carWidth;
//                        if (needWidth - regionWidth > Precision_.Precison) break;
//                        {
//                            columns += 1;
//                            p_1 = new Vector2d(p_0.x + needWidth, p_0.y);
//                            _p_1 = new Vector2d(_p_0.x + needWidth, _p_0.y);
//                            rightBoundarySegment = new BoundarySegment(new Segment2d(p_1, _p_1), LineStyleId.BroadsideId);
//                        }
//                    }
//                    else
//                    {
//                        needWidth += carWidth;
//                        if (needWidth - regionWidth > Precision_.Precison) break;
//                        {
//                            columns += 1;
//                            p_1 = new Vector2d(p_0.x + needWidth, p_0.y);
//                            _p_1 = new Vector2d(_p_0.x + needWidth, _p_0.y);
//                            rightBoundarySegment = new BoundarySegment(new Segment2d(p_1, _p_1), LineStyleId.BroadsideId);
//                        }
//                    }
//                }
//                j++;
//            }
//            #endregion
//            return columns;
//        }

//    }
//}
