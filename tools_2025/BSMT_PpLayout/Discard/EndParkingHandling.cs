//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Autodesk.Revit.DB;
//using g3;
//using goa.Common.g3InterOp;
//using PubFuncWt;

//namespace BSMT_PpLayout
//{
//    /// <summary>
//    /// 在一个地库边界范围内，寻找尽端车位
//    /// </summary>
//    class EndParkingHandling
//    {
//        internal Bsmt BaseMent;
//        internal List<RevitElePS> ElePses { get; }
//        //internal List<Vector2d> LocationVector2Ds { get; }
//        internal List<PairEndPS> PairEndPs { get; set; }
//        internal alglib.kdtree kDTree { get; }
//        internal EndParkingHandling()
//        {        }

//        internal EndParkingHandling(Bsmt baseMent)
//        {
//            this.BaseMent = baseMent;
//            this.ElePses = this.BaseMent.InBoundElePses;
//            //this.LocationVector2Ds = this.ParkingUnitExits.Select(p=>p.LocationVector2d).ToList();

//            // 创建kDtree 创建一次用于检索即可
//            List<Vector2d> vector2Ds = this.ElePses.Select(p => p.LocVector2d).ToList();
//            this.kDTree = KdTree.ConstructTree<RevitElePS>(this.ElePses, vector2Ds);
//        }

//        /// <summary>
//        /// 每排车端头车配对，补全每排车
//        /// </summary>
//        /// <returns></returns>
//        internal List<PairEndPS> FindRowCars()
//        {
//            List<PairEndPS> pairEndPses = LookForPSEndOfARow();
//            List<PairEndPS> _pairEndPses =new List<PairEndPS>();

//            foreach (PairEndPS _pairEndPS in pairEndPses)
//            {
//                PairEndPS pairEndPS = SetPairEndPSRowCarEndType(_pairEndPS);
//                if (pairEndPS.NowRowCarEndType == CarType.EndType)
//                {
//                    // 尽端回车问题
//                    if (pairEndPS.Length >= GlobalData.returnLengthEnd)
//                    {
//                        _pairEndPses.Add(FindOneRowCars(pairEndPS));
//                    }
//                }
//                else if(pairEndPS.NowRowCarEndType == CarType.NoEndType)
//                {
//                    // 设置联通道问题
//                    if (pairEndPS.Length >= GlobalData.loopReturnLength)
//                    {
//                        _pairEndPses.Add(FindOneRowCars(pairEndPS));
//                    }
//                }
//            }
//            return _pairEndPses;
//        }
//        /// <summary>
//        /// 寻找由一对端头停车位限定的一排停车位
//        /// </summary>
//        /// <param name="pairEndPS"></param>
//        /// <returns></returns>
//        internal PairEndPS FindOneRowCars(PairEndPS pairEndPS)
//        {
//            List<RevitElePS> parkingUnitExits = new List<RevitElePS>();

//            RevitElePS parkingUnitExit01 = pairEndPS.ParkingUnitExit01;
//            RevitElePS parkingUnitExit02 = pairEndPS.ParkingUnitExit02;
//            parkingUnitExits.Add(parkingUnitExit01);
//            parkingUnitExits.Add(parkingUnitExit02);

//            Vector2d vector2D01 = parkingUnitExit01.LocVector2d;
//            Vector2d vector2D02 = parkingUnitExit02.LocVector2d;
//            Vector2d direction = pairEndPS.Direction;

//            double x01 = vector2D01.x;
//            double y01 = vector2D01.y;
//            double x02 = vector2D02.x;
//            double y02 = vector2D02.y;

//            Interval interval_x = new Interval(x01, x02);
//            Interval interval_y = new Interval(y01, y02);

//            Line2d line2D = new Line2d(vector2D01, direction);// 获取当前配对端头停车位所在直线

//            List<ElementId> collectElementIds = new List<ElementId>();
//            foreach (RevitElePS parkingUnitExit in this.ElePses)
//            {
//                collectElementIds.Add(parkingUnitExit01.Id);
//                collectElementIds.Add(parkingUnitExit02.Id);
//                if (collectElementIds.Contains(parkingUnitExit.Id)) continue;

//                Vector2d vector2d = parkingUnitExit.LocVector2d;

//                // 使用 x y 区间 进行界定
//                double x = vector2d.x;
//                double y = vector2d.y;

//                if (interval_x.Inside(x) && interval_y.Inside(y))
//                {
//                    double distance = line2D.DistanceSquared(vector2d);// 其余端头停车位距离上述直线的距离

//                    if (distance > Precision_.Precison)// 通过点到直线的距离判断共线问题
//                        continue;
//                    parkingUnitExits.Add(parkingUnitExit);
//                    collectElementIds.Add(parkingUnitExit.Id);// 该eleIdsList用于循环判断

//                }
//                else
//                    continue;
//            }
//            if (direction.x == 0)
//            {
//                parkingUnitExits = parkingUnitExits.OrderBy(p => p.LocVector2d.y).ToList();// 次轮 y值
//            }
//            else
//            {
//                parkingUnitExits = parkingUnitExits.OrderBy(p => p.LocVector2d.x).ToList();// 首轮 x值
//            }
//            pairEndPS.RowParkingUnitExits = parkingUnitExits;// 排序结果为 从左至右 如果是垂直情况 则是 从下至上

//            return pairEndPS;
//        }

//        /// <summary>
//        /// 每排车端头车配对，当前PairEndPS实例只包含两端头停车位
//        /// </summary>
//        /// <returns></returns>
//        internal List<PairEndPS> LookForPSEndOfARow()
//        {
//            // 收集器-
//            List<PairEndPS> pairEndPs = new List<PairEndPS>();
//            List<RevitElePS> endParkings = FindEndPSes();// 找出所有的端头停车位
//            // 1 配对
//            foreach (RevitElePS parkingUnitExit in endParkings)
//            {
//                // 过滤重复elementId
//                bool whether2Skip = false;
//                foreach (PairEndPS pairEndPS in pairEndPs)
//                {
//                    if (pairEndPs.Count < 1)
//                        break;
//                    if(parkingUnitExit.Id == pairEndPS.ParkingUnitExit01.Id|| parkingUnitExit.Id == pairEndPS.ParkingUnitExit02.Id)
//                    {
//                        whether2Skip = true;
//                        break;
//                    }
//                }
//                if (whether2Skip) continue;
//                // 端头配对问题
//                List<RevitElePS> parkingUnitExits = EndMatched(parkingUnitExit, endParkings);
//                if(parkingUnitExits.Count ==1)
//                {
//                    PairEndPS pairEndPS = new PairEndPS(parkingUnitExit, parkingUnitExits.First());
//                    pairEndPs.Add(pairEndPS);
//                }
//            }

//            if (pairEndPs.Count < 1)
//                return new List<PairEndPS>();

//            return pairEndPs;
//        }
//        /// <summary>
//        /// 判断车侧边延长线 的第一个交点的线属性 相交物体（地库边界线 停车位边界线 车道中心线）
//        /// </summary>
//        /// <param name="pairEndPS1"></param>
//        /// <returns></returns>
//        internal PairEndPS SetPairEndPSRowCarEndType(PairEndPS pairEndPS1)
//        {
//            pairEndPS1.ParkingUnitExit01 = SetParkingUnitExitAdjacentAttribute(pairEndPS1.ParkingUnitExit01);
//            pairEndPS1.ParkingUnitExit02 = SetParkingUnitExitAdjacentAttribute(pairEndPS1.ParkingUnitExit02);

//            if(pairEndPS1.ParkingUnitExit01.RowEndType == CarType.EndType|| pairEndPS1.ParkingUnitExit02.RowEndType == CarType.EndType)
//            {
//                pairEndPS1.NowRowCarEndType = CarType.EndType;
//            }
//            else
//            {
//                pairEndPS1.NowRowCarEndType = CarType.NoEndType;
//            }

//            return pairEndPS1;
//        }
//        /// <summary>
//        /// 判断端头停车位属性，是否为尽端 判断方法，模拟出端头车位前的车道中心线，判断与中心线是否两端是否与车道相连
//        /// </summary>
//        /// <param name="parkingUnitExit"></param>
//        /// <returns></returns>
//        internal RevitElePS SetParkingUnitExitAdjacentAttribute(RevitElePS parkingUnitExit)
//        {
//            Vector2d vector2D_direction = parkingUnitExit.RowDirection * (-1);
//            Vector2d locationVector2d = parkingUnitExit.LocVector2d;
//            // 车头方向
//            Vector2d facingOrientation = parkingUnitExit.FamilyInstance.FacingOrientation.ToVector2d();
//            // 求出停车位车头前的车道中心线(方法：将停车位中心点移动到半个车长+1/3路宽，取1/3的原因是侧边边缘车道中心线只出头道路的一半)
//            Vector2d roadStartPoint = locationVector2d + facingOrientation * (GlobalData.pSHeight/2+GlobalData.Wd_sec/3);
//            Segment2d segment2dRay = new Segment2d(roadStartPoint, roadStartPoint + vector2D_direction*4000);// *4000

//            List<Vector2d> vector2DsPolygon2d = new List<Vector2d>();// 收集所有与停车位线圈的交点
//            parkingUnitExit.Last2PPsExit.Where(p=>p.Id!= parkingUnitExit.Id).Select(p => p.Polygon2d()).ToList();
//            // 基于假定线段起始点寻找最近的10个车位
//            List<Polygon2d> allCarpolygon2Ds = KdTree.SearchByCoord<RevitElePS>(roadStartPoint, this.ElePses, this.kDTree, 10).Where(p => p.Id != parkingUnitExit.Id).Select(p => p.Polygon2d()).ToList();

//            // 这里需要加入障碍物线圈（ 注意bounddingbox会放大障碍物的边界 ）
//            allCarpolygon2Ds.AddRange(this.BaseMent.InBoundEleFRes.Where(p => p.EleProperty != EleProperty.Lane && p.EleProperty != EleProperty.PriLane && p.EleProperty != EleProperty.SecLane && p.EleProperty != EleProperty.CusLane).Select(p => p.polygon2d)); 
//            foreach (Polygon2d polygon2 in allCarpolygon2Ds)
//            {
//                foreach (Segment2d segment2d in polygon2.SegmentItr())
//                {
//                    IntrSegment2Segment2 intrSegment2Segment2 = new IntrSegment2Segment2(segment2dRay, segment2d);
//                    intrSegment2Segment2.Compute();
//                    if(intrSegment2Segment2.Quantity ==1)
//                    {
//                        vector2DsPolygon2d.Add(intrSegment2Segment2.Point0);
//                    }
//                }
//            }
//            // 找到最近点
//            double distance_polygon = 1e10; //默认交点距离为最大值
//            if (vector2DsPolygon2d.Count>0)
//            {
//                foreach (Vector2d vector2d in vector2DsPolygon2d)
//                {
//                    double tempDis = locationVector2d.Distance(vector2d);
//                    if(tempDis< distance_polygon)
//                    {
//                        distance_polygon = tempDis;
//                    }
//                }
//            }

//            // 所有的车道中心线
//            List<Vector2d> vector2DsRoad = new List<Vector2d>();// 收集所有与车道中心线的交点
//            List<Segment2d> roadSegs = this.BaseMent.InBoundEleLines.Select(p => p.BoundSeg.Segment2d).ToList();
//            foreach (Segment2d segment2d in roadSegs)
//            {
//                IntrSegment2Segment2 intrSegment2Segment2 = new IntrSegment2Segment2(segment2dRay, segment2d);
//                intrSegment2Segment2.Compute();
//                if (intrSegment2Segment2.Quantity == 1)
//                {
//                    vector2DsRoad.Add(intrSegment2Segment2.Point0);
//                }
//            }
//            // 找到最近点
//            double distance_road = 1e10; //默认交点距离为最大值
//            if (vector2DsRoad.Count > 0)
//            {
//                foreach (Vector2d vector2d in vector2DsRoad)
//                {
//                    double tempDis = locationVector2d.Distance(vector2d);
//                    if (tempDis < distance_road)
//                    {
//                        distance_road = tempDis;
//                    }
//                }
//            }

//            // 判断端头停车位是否为尽端属性
//            if(distance_road< distance_polygon)
//            {
//                parkingUnitExit.RowEndType = CarType.NoEndType;
//            }
//            else
//            {
//                parkingUnitExit.RowEndType = CarType.EndType;
//            }
//            return parkingUnitExit;
//        }
//        /// <summary>
//        /// 单个端头停车位配对
//        /// </summary>
//        /// <param name="parkingUnitExit">需要配对的目标端头停车位</param>
//        /// <param name="parkingUnitExits">所有的端头停车位</param>
//        /// <returns></returns>
//        internal List<RevitElePS> EndMatched(RevitElePS parkingUnitExit, List<RevitElePS> parkingUnitExits)
//        {
//            List<RevitElePS> mayParkingUnitExits = new List<RevitElePS>();
//            //FamilyInstance fi = parkingUnitExit.FamilyInstance;

//            //XYZ hangOrientation = fi.HandOrientation;
//            Vector2d locationPoint = parkingUnitExit.LocVector2d;

//            Segment2d segment2D = new Segment2d(locationPoint, locationPoint+ parkingUnitExit.RowDirection*40000);// 获取当前端头停车位所在直线
//            // 从所有端头停车位中进行配对
//            foreach (RevitElePS _parkingUnitExit in parkingUnitExits)
//            {
//                if (parkingUnitExit.Id == _parkingUnitExit.Id)
//                    continue;

//                Vector2d vector2d = _parkingUnitExit.LocVector2d;
//                double distance = segment2D.DistanceSquared(vector2d);// 其余端头停车位距离上述直线的距离

//                if (distance > Precision_.Precison * Precision_.Precison)// 通过点到直线的距离判断共线问题
//                    continue;

//                mayParkingUnitExits.Add(_parkingUnitExit);
//            }

//            if (mayParkingUnitExits.Count <= 1)
//                return mayParkingUnitExits;
//            else
//            {
//                double distance = locationPoint.DistanceSquared(mayParkingUnitExits[0].LocVector2d);
//                int serialNum = 0;
//                for (int i = 1; i < mayParkingUnitExits.Count; i++)
//                {
//                    double _distance = locationPoint.DistanceSquared(mayParkingUnitExits[i].LocVector2d);
//                    if (distance> _distance)
//                        serialNum = i;
//                }
//                return new List<RevitElePS>() { mayParkingUnitExits[serialNum]};
//            }
//        }


//        #region 确定所有端头车位

//        /// <summary>
//        /// 基于停车位中心点的距离关系，判断是否为端头车位 但是该函数，同时计算出所有端头停车位的指向问题，指向原则设定为
//        /// </summary>
//        /// <returns></returns>
//        internal List<RevitElePS> FindEndPSes()
//        {
//            List<RevitElePS> parkingUnitExits = new List<RevitElePS>();
//            foreach (RevitElePS parkingUnitExit in this.ElePses)
//            {
//                int tempCount = FindEndPSDistance(parkingUnitExit);
//                if(tempCount == 1)
//                {
//                    Vector2d vecSidePS = parkingUnitExit.Last2PPsExit.First().LocVector2d;

//                    parkingUnitExit.RowDirection =(vecSidePS- parkingUnitExit.LocVector2d).Normalized ;
//                    parkingUnitExits.Add(parkingUnitExit);
//                }
//            }

//            return parkingUnitExits;
//        }

//        /// <summary>
//        /// 判断一个停车位中心点距离其他停车位中心点的距离 统计距离等于 一个车宽/一个车宽+柱距 的数量 2 需要判断
//        /// </summary>
//        /// <param name="parkingUnitExit"></param>
//        /// <returns></returns>
//        internal int FindEndPSDistance(RevitElePS parkingUnitExit)
//        {
//            int tempCount = 0;
//            // 使用KDTree进行检索
//            List<RevitElePS> parkingUnitExits = KdTree.SearchByCoord<RevitElePS>(parkingUnitExit.LocVector2d, this.ElePses, this.kDTree, 3);

//            if (parkingUnitExits.Count >= 2)
//            {
//                parkingUnitExits = parkingUnitExits.Where(p => p.Id != parkingUnitExit.Id).ToList();

//            }

//            foreach (RevitElePS _parkingUnitExit in parkingUnitExits)
//            {
//                double distance = parkingUnitExit.LocVector2d.Distance(_parkingUnitExit.LocVector2d);
//                double wight = GlobalData.pSWidth;
//                double columnWidth = GlobalData.ColumnWidth + GlobalData.ColumnBurfferDistance;
//                // 判断车宽
//                if ((distance <= wight + Precision_.Precison
//                    && distance >= wight - Precision_.Precison)
//                    // 判断 车宽 + 柱宽
//                    || (distance <= wight + columnWidth + Precision_.Precison
//                    && distance >= wight + columnWidth - Precision_.Precison))
//                {
//                    tempCount++;
//                }
//            }
//            parkingUnitExit.Last2PPsExit = parkingUnitExits;
//            return tempCount;
//        }
//        #endregion
              
//    }
//}
