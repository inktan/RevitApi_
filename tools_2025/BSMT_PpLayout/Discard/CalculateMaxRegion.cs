//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//using g3;
//using ClipperLib;

//using Autodesk.Revit.DB;
//using CommonMethod_g3;
//using PublicProjectMethods_;

//namespace ParkingLayoutEfficientNewStructual
//{
//    using cInt = Int64;

//    using Path = List<IntPoint>;
//    using Paths = List<List<IntPoint>>;
//    using Path_Vector2d = List<Vector2d>;
//    using Paths_Vector2d = List<List<Vector2d>>;

//    /// <summary>
//    /// 基于停车区域各属性边，自动求出最大可停车区域
//    /// </summary>
//    class CalculateMaxRegion
//    {
//        #region Class 内部全局变量
//        internal Autodesk.Revit.DB.Document  this.Doc;
//        internal Polygon2d Polygon2d{ get; set; }

//        //【】分割最大可停车矩形区域的条件依据
//        internal List<Polygon2d> ObstructioPolygon2ds { get; set; }
//        internal SingleLayoutRegion SingleLayoutRegion { get; set; }
//        #endregion

//        //【】记录所有面积区域
//        internal List<MaxAreaPolygon> MaxAreaPolyogns { get; set; }

//        /// <summary>
//        /// 扫描 SingleLayoutRegion 的可停车矩形区域
//        /// </summary>
//        public CalculateMaxRegion(Document doc, SingleLayoutRegion singleLayoutRegion)
//        {
//            this. this.Doc = doc;
//            this.SingleLayoutRegion = singleLayoutRegion;

//            // 这里只寻找与求最大面积相关的区域 塔楼投影-需要注意 塔楼投影需不需要考虑开间退让的问题 设备用房
//            this.ObstructioPolygon2ds = singleLayoutRegion.RelatedObstructiveBoundaryLoops
//                .Where(p=> p.LineStyleId== LineStyleId.OutlineResidenStructRegionId
//                || p.LineStyleId == LineStyleId.OutlineResidenStructRegionDepthId
//                || p.LineStyleId == LineStyleId.EquipmentRoomId )
//                .Select(p => p.polygon2d)
//                .ToList();
//        }

//        /// <summary>
//        /// 第一次处理 singleLayoutRegion父子区域自身
//        /// </summary>
//        internal void Calculate(List<BoundarySegment> allAttributeLines)
//        {
//            this.Polygon2d = this.SingleLayoutRegion.Polygon2d;
//            //【】区分的 BasementWallId PrimaryLaneId
//            List<BoundarySegment> primaryLaneSegments = this.SingleLayoutRegion.SelfBoundarySegments.Where(p=>p.LineStyleId==LineStyleId.PrimaryLaneId).ToList();
//            GetMaxAreaRectangle(primaryLaneSegments, this.Polygon2d, this.ObstructioPolygon2ds, allAttributeLines);
//        }
//        /// <summary>
//        /// 第二次开始 进入循环裁剪计算——注意和第一次的区别
//        /// </summary>
//        internal void Calculate(MaxAreaPolygon maxAreaRectangle, List<BoundarySegment> allAttributeLines)
//        {
//            this.Polygon2d = maxAreaRectangle.Polygon2d;
//            //【】BasementWallId PrimaryLaneId
//            List<BoundarySegment> primaryLaneSegments = maxAreaRectangle.SelfBoundarySegments.Where(p => p.LineStyleId == LineStyleId.PrimaryLaneId).ToList();
//            GetMaxAreaRectangle(primaryLaneSegments, this.Polygon2d, this.ObstructioPolygon2ds, allAttributeLines);
//        }

//        /// <summary>
//        /// 循环一个线圈的各个边界，对每个投影线段进行sweep，得到面积最大的矩形区域
//        /// </summary>
//        /// <returns></returns>
//        internal void GetMaxAreaRectangle(List<BoundarySegment> primaryLaneSegments, Polygon2d _polygon2d, List<Polygon2d> obstructioPolygon2ds, List<BoundarySegment> allAttributeLines)
//        {
//            List<MaxAreaPolygon> maxAreaPolyogns = new List<MaxAreaPolygon>();
//            //【】设定旋转基准点
//            Vector2d vector2dZreo = Vector2d.Zero;
//            Path_Vector2d vertices = _polygon2d.VerticesItr(false).ToList();

//            //【】只循环 旋转 属性为PrimaryLaneId的边界
//            int i = 0;
//            foreach (BoundarySegment boundarySegment in primaryLaneSegments)
//            {
//                //【】求旋转至水平方向的角度——目的：将需要处理的sweep边界，转换到水平x轴上
//                Segment2d segment2d = boundarySegment.segment2d;
//                if (segment2d.Length < InputParameter.pSHeight-Precision_.Precison)
//                    continue;
//                double rotateAngle = segment2d.AngleBetweenHorizontal();

//                if (i == -1)// 单个子区域调试
//                {// 单个子区域调试     

//                    IEnumerable<Line> lines = new List<Line>() { segment2d.ToLine() };
//                    double ele =  this.Doc.ActiveView.GenLevel.ProjectElevation;
//                     this.Doc.CreateDirectShapeWithNewTransaction(lines);
//                }// 单个子区域调试


//                Polygon2d polygon2d = _polygon2d.Rotate(vector2dZreo, rotateAngle);
//                List<Polygon2d> _obstructioPolygon2ds = obstructioPolygon2ds.Rotate(vector2dZreo, rotateAngle).ToList();
//                segment2d = segment2d.Rotate(vector2dZreo,rotateAngle);

//                List<MaxAreaPolygon> _maxAreaPolyogns = GetAreaRegionBySingleRegionBoundary(segment2d, polygon2d, _obstructioPolygon2ds);
//                if (_maxAreaPolyogns.Count>0)
//                {
//                    _maxAreaPolyogns = _maxAreaPolyogns.RotateTransform(-rotateAngle, vector2dZreo).ToList();
//                    _maxAreaPolyogns.ForEach(p => {
//                        p.SetLeftRightSelfBoundarySegments(allAttributeLines);
//                        p.BottomScanLineAngle = rotateAngle;
//                        p.TopScanLineAngle = p.TopBoundarySegment.segment2d.AngleBetweenHorizontal();
//                        p.LeftScanLineAngle = p.LeftBoundarySegment.segment2d.AngleBetweenHorizontal();
//                        p.RightScanLineAngle = p.RightBoundarySegment.segment2d.AngleBetweenHorizontal();


//                    });
//                    maxAreaPolyogns.AddRange(_maxAreaPolyogns);
//                }

//                if (i == -1)// 单个子区域调试
//                {// 单个子区域调试     

//                    foreach (var item in _maxAreaPolyogns)
//                    {
//                        List<Vector2d> path_Vector2D = item.Polygon2d.VerticesItr(false).ToList();
//                        IEnumerable<XYZ> xYZs = path_Vector2D.ToXyzs();
//                        IEnumerable<Line> lines = xYZs.ToLines();
//                        double ele =  this.Doc.ActiveView.GenLevel.ProjectElevation;
//                         this.Doc.CreateDirectShapeWithNewTransaction(lines);
//                    }
//                }// 单个子区域调试
//                i++;

//            }        
//            this.MaxAreaPolyogns = maxAreaPolyogns;           
//        }
      
//        /// <summary>
//        /// 将底部一根水平边界线段投影多段后，得到各段对应的面积矩形 底部参考线已经transform为水平状态
//        /// </summary>
//        /// <returns></returns>
//        internal List<MaxAreaPolygon> GetAreaRegionBySingleRegionBoundary(Segment2d segment2d, Polygon2d regionPolygon2d, List<Polygon2d> obstructioPolygon2ds)
//        {
//            List<MaxAreaPolygon> maxAreaRectangles = new List<MaxAreaPolygon>();
//            IEnumerable<Segment2d> allSubsections = GetsProjectedSegment(segment2d, regionPolygon2d, obstructioPolygon2ds);
 
//            foreach (Segment2d subsection in allSubsections)
//            {
//                //【】对单根投影线做判断，是否可以进行面积投影 
//                if (subsection.Length <= InputParameter.pSWidth - Precision_.Precison)
//                    continue;

//                MaxAreaPolygon maxAreaRectangle1 = GetsMaxAreaPolygonBySubsection(subsection, regionPolygon2d, obstructioPolygon2ds);
//                if (maxAreaRectangle1.Polygon2d != null && maxAreaRectangle1.Polygon2d.Area != 0)
//                {
//                    maxAreaRectangles.Add(maxAreaRectangle1);
//                }
//            }
          
//            return maxAreaRectangles;
//        }
//        /// <summary>
//        /// 将基准线打断——该处需要进一步思考
//        /// </summary>
//        /// <returns></returns>
//        internal IEnumerable<Segment2d> GetsProjectedSegment(Segment2d segment2d, Polygon2d regionPolygon2d, List<Polygon2d> obstructioPolygon2ds)
//        {
//            List<Vector2d> vector2ds = GetAllObstakVecesToProjectCut(regionPolygon2d, obstructioPolygon2ds);
//            List<Vector2d> _vector2ds = InsideSegment(segment2d, vector2ds).ToList();

//            Vector2d center = segment2d.Center;
//            Vector2d direction = segment2d.Direction;

//            for (int i = 0; i < _vector2ds.Count(); i++)
//            {
//                _vector2ds[i] = _vector2ds[i].Pedal(center, direction);
//            }

//            int count = _vector2ds.Count;
//            for (int i = 0; i < count - 1; i++)
//            {
//                Vector2d left = _vector2ds[i];
//                for (int j = 0; j < count; j++)
//                {
//                    if (j > i)
//                    {
//                        Vector2d right = _vector2ds[j];
//                        yield return new Segment2d(left, right);
//                    }
//                }
//            }
//        }
//        /// <summary>
//        /// 【1】所有障碍物边界顶点；【2】当前计算区域的边界点——为了投影异性的上部角点
//        /// </summary>
//        internal List<Vector2d> GetAllObstakVecesToProjectCut(Polygon2d regionPolygon2d, List<Polygon2d> obstructioPolygon2ds)
//        {
//            List<Vector2d> nourthVector2ds = regionPolygon2d.NourthVertices();// 停车区域边界 北侧的点
//            foreach (Polygon2d _polygon2d in obstructioPolygon2ds)
//            {
//                List<Vector2d> sourthVector2ds = _polygon2d.SourthVertices();
//                nourthVector2ds.AddRange(sourthVector2ds);
//            }
//            return nourthVector2ds;
//        }
//        /// <summary>
//        /// 取出在线段x轴范围上的点，不包括首尾端点，首尾端点强制为当前线段的端点，确保没有重合点
//        /// </summary>
//        internal IEnumerable<Vector2d> InsideSegment(Segment2d segment2d, List<Vector2d> vector2ds)
//        {
//            List<Vector2d> _vector2ds = new Path_Vector2d();
//            double start = segment2d.P0.x + Precision_.Precison;
//            double end = segment2d.P1.x - Precision_.Precison;
//            Interval interval = new Interval(start, end);
//            foreach (Vector2d vector2d in vector2ds)
//            {
//                double x = vector2d.x;
//                if (interval.InsideWithOutEndpoint(x))
//                {
//                    _vector2ds.Add(vector2d);
//                }
//            }
//            _vector2ds.Add(segment2d.P0);
//            _vector2ds.Add(segment2d.P1);
//            if (_vector2ds.Count() > 1)
//                _vector2ds = _vector2ds.OrderBy(p => p.x).ToList();//从小到大-从左至右

//            return _vector2ds.DelDuplicate();
//        }
//        /// <summary>
//        /// 取最低水平边界线进行投影分段中的一段，方向为y轴正方向，自下而上求sweep面积
//        /// </summary>
//        /// <returns></returns>
//        internal MaxAreaPolygon GetsMaxAreaPolygonBySubsection(Segment2d subsection, Polygon2d regionPolygon2d, List<Polygon2d> obstructioPolygon2ds)
//        {
//            //【】01 首先判断落在线段上的点 
//            //【】02 基于线段的两个端点，做sweep line找到与区域上部边界的交点，取y值较大的点
//            //【】03 比较左、右sweep line与上部边界交点，取y值较小的点
//            Vector2d p0 = subsection.P0;
//            Vector2d p1 = subsection.P1;
//            Vector2d p_0 = subsection.P0;// 这里做重复赋值，源于下方要二次调用该数据
//            Vector2d p_1 = subsection.P1;
//            if (subsection.P0.x > subsection.P1.x)// 该处进行判断，确保左右点位置正确
//            {
//                p0 = subsection.P1;
//                p1 = subsection.P0;
//                p_0 = subsection.P1;// 这里做重复赋值，源于下方要二次调用该数据
//                p_1 = subsection.P0;
//            }
//            p0 += new Vector2d(Precision_.Precison, 0);// 线段向内收缩，是为了解决IntrSegment2Segment2中相等无理数计算不相交的情况
//            p1 -= new Vector2d(Precision_.Precison, 0);// 新问题，该处辅助线的移动，会影响所求面积区域的边界线位置

//            Vector2d direction = new Vector2d(0, 1).Normalized;
//            Line2d segment2d_p0 = new Line2d(p0, direction);
//            Line2d segment2d_p1 = new Line2d(p1, direction);

//            double p0y = p0.y + Precision_.Precison;// 限定，扫描线与边界区域的交点位于底部基准线的上方Y轴正方向

//            Path_Vector2d vector2ds01 = segment2d_p0.IntrLine2Polygon(regionPolygon2d).Where(p => p.y >= p0y + Precision_.Precison).ToList();// 取在底部基准线上的交点
//            Path_Vector2d vector2ds02 = segment2d_p1.IntrLine2Polygon(regionPolygon2d).Where(p => p.y >= p0y + Precision_.Precison).ToList();

//            if(vector2ds01.Count <1|| vector2ds02.Count<1)// 该处判断为三角形的角点处的相交点，在y值上均低于底部基准线（源于计算误差0.000001）
//            {
//                return new MaxAreaPolygon(new Polygon2d());
//            }

//            Vector2d vector2d01 = vector2ds01.OrderBy(p => p.y).ToList().First();// 取比自身所在线段y值大的第一个交点
//            Vector2d vector2d02 = vector2ds02.OrderBy(p => p.y).ToList().First();
//            Vector2d topVector2d = new List<Vector2d>() { vector2d01, vector2d02 }.OrderByDescending(p => p.y).ToList().Last();// 取两个交点的较小点

//            // 创建切割矩形
//            Vector2d _p_0 = new Vector2d(p_0.x, topVector2d.y);
//            Vector2d _p_1 = new Vector2d(p_1.x, topVector2d.y);

//            // 该处注意创建矩形的四个角点顺序，首尾点为线圈的底部线段端点。这里默认为逆时针，手动创建的polygon，点集顺序会被记录
//            Polygon2d polygon2d = new Polygon2d(new List<Vector2d>() { p_0, p_1, _p_1, _p_0 });

//            // 继续判断矩形高度，当前矩形内是否存在障碍物角点
//            double polygonHeight = 0;
//            polygon2d = ReSetPolyHeight(polygon2d, obstructioPolygon2ds, out polygonHeight, ref  _p_1, ref  _p_0);

//            if (polygonHeight <= InputParameter.pSWidth - Precision_.Precison)
//                return new MaxAreaPolygon(new Polygon2d());

//            // 记录当前最大矩形面积的信息，线圈、基准投影线、（需要属性说明）
//            MaxAreaPolygon maxAreaRectangle = new MaxAreaPolygon(polygon2d)
//            {
//                BottomBoundarySegment = new BoundarySegment(subsection, LineStyleId.PrimaryLaneId),
//                TopBoundarySegment = new BoundarySegment(new Segment2d(_p_1, _p_0), LineStyleId.BroadsideId),
//                LeftBoundarySegment = new BoundarySegment(new Segment2d(_p_0, p_0), LineStyleId.BroadsideId),
//                RightBoundarySegment = new BoundarySegment(new Segment2d(p_1, _p_1), LineStyleId.BroadsideId),
//                PolygonHeight = polygonHeight,
//            };

//            return maxAreaRectangle;
//        }
//        /// <summary>
//        /// 进一步设定当前面积最大区域的Y轴高度
//        /// </summary>
//        /// <param name="polygon2d">该矩形的第一个点为矩形底布边长的一个端点</param>
//        /// <param name="obstructioPolygon2ds">原始区域相关的所有障碍物区域</param>
//        /// <param name="height">输出求出的矩形高度</param>
//        /// <returns></returns>
//        internal Polygon2d ReSetPolyHeight(Polygon2d polygon2d,List<Polygon2d> obstructioPolygon2ds,out double height,ref Vector2d _p_1,ref  Vector2d _p_0)
//        {
//            Path_Vector2d path_Vector2d = polygon2d.VerticesItr(false).ToList();
//            Vector2d p_0 = path_Vector2d[0];// 矩形点集顺序，请查看上方创建polygon时的排列顺序
//            Vector2d p_1 = path_Vector2d[1];

//            height = Math.Abs(_p_0.y - p_0.y);

//            // 1 判断当前矩形最下侧线段是否与障碍物有相交关系；该线段需要进行端点收缩
//            Segment2d segment2d = new Segment2d(p_0,p_1);
//            foreach (var item in obstructioPolygon2ds)
//            {
//                if (item.Intersects(segment2d) || item.Contains(segment2d))
//                {
//                    height = 0;
//                    return new Polygon2d();
//                }
//            }

//            List<Vector2d> vector2ds = GetAllObstakVeces( obstructioPolygon2ds);//有个问题，点落在边界上
//            List<Vector2d> insideVector2ds = GetAllObstakVecesInRectangle(polygon2d, vector2ds);// 如何保证点落在目标区域内

//            if (insideVector2ds.Count > 0)
//            {
//                Vector2d topVector2d = insideVector2ds.OrderByDescending(p => p.y).ToList().Last();
//                height = Math.Abs(topVector2d.y - p_0.y);
//                if (height < InputParameter.pSWidth)// 这里直接使用车宽进行判断
//                {
//                    height = 0;
//                    return new Polygon2d();
//                }

//                _p_0 = new Vector2d(p_0.x, topVector2d.y);
//                _p_1 = new Vector2d(p_1.x, topVector2d.y);

//                polygon2d = new Polygon2d(new List<Vector2d>() { p_0, p_1, _p_1, _p_0 });
//            }

//            return polygon2d;
//        }
//        /// <summary>
//        /// 获取落在目标区域的所有障碍物边界顶点
//        /// </summary>
//        internal List<Vector2d> GetAllObstakVecesInRectangle(Polygon2d _polygon2D, Path_Vector2d path_Vector2d)
//        {
//            List<Vector2d> vector2Ds = new Path_Vector2d();
//            foreach (Vector2d _vector2d in path_Vector2d)
//            {
//                if (_polygon2D.Contains(_vector2d))// 由于线圈被收缩，因此，为了判断底部的点，需要将点往上移动一个小的距离
//                {
//                    vector2Ds.Add(_vector2d);
//                }
//            }
//            return vector2Ds;
//        }
//        /// <summary>
//        /// 获取所有障碍物角点
//        /// </summary>
//        internal List<Vector2d> GetAllObstakVeces(List<Polygon2d> obstructioPolygon2ds)
//        {
//            //【】向内偏移距离用来满足，解决边界点落在边界上
//            List<Vector2d> vector2ds = new Path_Vector2d();
//            foreach (Polygon2d _polygon2d in obstructioPolygon2ds)
//            {
//                Polygon2d polygon2D = _polygon2d.InwardOffeet(Precision_.Precison);

//                vector2ds.AddRange(polygon2D.Vertices);
//            }
//            return vector2ds;
//        }
     
//    }// class
//}// namespace
