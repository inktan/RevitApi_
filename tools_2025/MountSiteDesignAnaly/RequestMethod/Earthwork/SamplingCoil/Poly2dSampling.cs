using Autodesk.Revit.DB;
using g3;
using goa.Common;
using goa.Common.g3InterOp;
using Octree;
using PubFuncWt;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MountSiteDesignAnaly
{
    /// <summary>
    /// 取采样点
    /// </summary>
    class Poly2dSampling
    {
        internal Polygon2d polygon2d;
        internal BaseFace baseFace;

        internal double elevation;

        internal Vector3d center;

        internal Poly2dSampling(Polygon2d _polygon2d, BaseFace _baseFace, double _elevation, Vector3d _center)
        {
            this.polygon2d = _polygon2d;
            this.baseFace = _baseFace;
            this.elevation = _elevation;
            this.center = _center;
        }

        internal List<List<Rad3dBounds>> SplitSelf(int _maxSize)
        {
            //CMD.Doc.CreateDirectShapeWithNewTransaction(SamplingRay3d.Select(p => p.origin.ToXYZ().VerticalLineUp()));
            return this.SamplingRay3d.DivideListMaxSize(_maxSize);
        }
        internal List<List<Rad3dBounds>> SplitSelf_RetainWall(int _maxSize, List<Polygon2d> polygon2ds)
        {
            //CMD.Doc.CreateDirectShapeWithNewTransaction(SamplingRay3d.Select(p => p.origin.ToXYZ().VerticalLineUp()));
            return this.SamplingRay3d_RetainWall(polygon2ds).DivideListMaxSize(_maxSize);
        }
        internal List<List<Rad3dBounds>> SplitSelf_RetainWall_Site(int _maxSize, List<Polygon2d> polygon2ds)
        {
            //CMD.Doc.CreateDirectShapeWithNewTransaction(SamplingRay3d.Select(p => p.origin.ToXYZ().VerticalLineUp()));
            return this.SamplingRay3d_RetainWall_Site(polygon2ds).DivideListMaxSize(_maxSize);
        }
        /// <summary>
        /// 
        /// </summary>
        internal List<Rad3dBounds> SamplingRay3d => this.samplingPoints().Select(p => new Rad3dBounds(p)).ToList();
        internal List<Rad3dBounds> SamplingRay3d_RetainWall(List<Polygon2d> polygon2ds)
        {
            return this.samplingPoints_RetainWall(polygon2ds).Select(p => new Rad3dBounds(p)).ToList();
        }
        internal List<Rad3dBounds> SamplingRay3d_RetainWall_Site(List<Polygon2d> polygon2ds)
        {
            return this.samplingPoints_RetainWall_Site(polygon2ds).Select(p => new Rad3dBounds(p)).ToList();
        }

        /// <summary>
        /// 土方计算的采样点
        /// </summary>
        /// <returns></returns>
        internal IEnumerable<Vector3d> samplingPoints()
        {
            if (!ReferenceEquals(this.baseFace, null) && this.baseFace.planeType == PlaneType.Inclined)
            {
                return this.inclinedSamplingPoints();
            }
            else
            {
                return this.levelSamplingPoint;
            }
        }
        /// <summary>
        /// 挡土墙计算的采样点，输入所有的线圈
        /// </summary>
        /// <returns></returns>
        internal IEnumerable<Vector3d> samplingPoints_RetainWall(List<Polygon2d> polygon2ds)
        {
            List<Segment2d> segment2Ds = new List<Segment2d>();
            List<Vector2d> vector2ds = new List<Vector2d>();
            foreach (var item in this.polygon2d.OutwardOffeet(ViewModel.Instance.RetainingWallDis.MilliMeterToFeet()).SegmentItr())
            {
                bool orNot = false;
                foreach (var poly in polygon2ds)
                {
                    Polygon2d polygon2d = poly.InwardOffeet(0.1);
                    if (polygon2d.Contains(item.Center))
                    {
                        orNot = true;
                        break;
                    }
                }
                //如果区域边界的取样点落在所有区域合并后的区域（向内收缩）里，则剔除
                if (orNot) continue;
                //在收集到的与山体相交的边界段去点
                vector2ds.AddRange(item.DivideByDis(ViewModel.Instance.SamplingAccuracy.MilliMeterToFeet()));

                segment2Ds.Add(item);
            }
            //CMD.Doc.CreateDirectShapeWithNewTransaction(segment2Ds.ToLines());

            if (!ReferenceEquals(this.baseFace, null) && this.baseFace.planeType == PlaneType.Inclined)
            {
                List<Vector3d> vector3ds = new List<Vector3d>();

                Mesh mesh = this.baseFace.baseFace.Triangulate();
                List<Triangle3d> triangle3ds = mesh.GetAllTriangles().Select(p => p.ToTriangle3d()).ToList();

                foreach (var item in vector2ds.DelDuplicate())
                {
                    Ray3d ray3d = new Ray3d(item.ToVector3d(-10000.0), new Vector3d(0, 0, 1));
                    // 斜面采样问题
                    foreach (var tri in triangle3ds)
                    {
                        IntrRay3Triangle3 intrRay3Triangle3 = new IntrRay3Triangle3(ray3d, tri);
                        intrRay3Triangle3.Compute();
                        if (intrRay3Triangle3.Quantity == 1)
                        {
                            // 倾斜楼板
                            vector3ds.Add(tri.PointAt(intrRay3Triangle3.TriangleBaryCoords));
                            break;
                        }
                    }
                }
                return vector3ds;
            }
            else
            {
                return vector2ds.DelDuplicate().Select(p => p.ToVector3d(this.elevation)).ToList();
            }
        }
        internal IEnumerable<Vector3d> samplingPoints_RetainWall_Site(List<Polygon2d> polygon2ds)
        {
            foreach (var item in this.polygon2d.LDpOfBox2d().GetMatrixVerts(this.polygon2d.Width(), this.polygon2d.Height(), ViewModel.Instance.SamplingAccuracy.MilliMeterToFeet(), ViewModel.Instance.SamplingAccuracy.MilliMeterToFeet()))
            {
                if (this.polygon2d.Contains(item))
                {
                    bool orNot = false;
                    foreach (var poly in polygon2ds)
                    {
                        if (poly.Contains(item))
                        {
                            orNot = true;
                            break;
                        }
                    }
                    if (!orNot)                    
                    {
                        yield return item.ToVector3d(this.elevation);
                    }
                }
            }
        }
        IEnumerable<Vector3d> inclinedSamplingPoints()
        {
            // 在实际高程面上进行取采样点
            Mesh mesh = this.baseFace.baseFace.Triangulate();
            List<Triangle3d> triangle3ds = mesh.GetAllTriangles().Select(p => p.ToTriangle3d()).ToList();
            foreach (var item in this.HorizontalSamplingPoint)
            {
                Ray3d ray3d = new Ray3d(item.ToVector3d(-10000.0), new Vector3d(0, 0, 1));
                // 斜面采样问题
                foreach (var tri in triangle3ds)
                {
                    IntrRay3Triangle3 intrRay3Triangle3 = new IntrRay3Triangle3(ray3d, tri);
                    intrRay3Triangle3.Compute();
                    if (intrRay3Triangle3.Quantity == 1)
                    {
                        // 倾斜楼板
                        yield return tri.PointAt(intrRay3Triangle3.TriangleBaryCoords);
                        break;
                    }
                }
            }
        }
        /// <summary>
        /// 首先在水平投影面上取点
        /// </summary>
        IEnumerable<Vector2d> HorizontalSamplingPoint
        {
            get
            {
                foreach (var item in this.polygon2d.LDpOfBox2d().GetMatrixVerts(this.polygon2d.Width(), this.polygon2d.Height(), ViewModel.Instance.SamplingAccuracy.MilliMeterToFeet(), ViewModel.Instance.SamplingAccuracy.MilliMeterToFeet()))
                {
                    if (this.polygon2d.Contains(item))
                    {
                        yield return item;
                    }
                }
            }
        }
        /// <summary>
        /// 获取实际高程采样点
        /// </summary>
        /// <returns></returns>
        List<Vector3d> levelSamplingPoint => this.HorizontalSamplingPoint.Select(p => p.ToVector3d(this.elevation)).ToList();
    }
    /// <summary>
    /// 射线包围盒
    /// </summary>
    class Rad3dBounds
    {
        internal Ray3d ray3dUp;
        internal Ray3d ray3dDown;

        internal Vector3d origin;

        internal Bounds bounds;
        internal RectangleF rectangleF => ToRectangleF(this.bounds);
        /// <summary>
        /// 一个采样网格划分为两个三角面
        /// </summary>
        internal List<Triangle3d> triangle3ds;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="_ray3d"></param>
        /// <param name="_precision">包围盒的胖瘦</param>
        /// <param name="_depth"></param>
        //internal Rad3dBounds(Ray3d _ray3d, double _precision, double _depth = 40000.0)
        //{
        //    this.ray3d = _ray3d;
        //    double _scale = 1.1;
        //    this.bounds = new Bounds(_ray3d.Origin.ToVector3() + (new Vector3(0, 0, _depth / 2)) * _ray3d.Direction.z, new Vector3(_precision * _scale, _precision * _scale, _depth));
        //}

        internal Rad3dBounds(Vector3d _origin, double _depth = 40000.0)
        {
            this.origin = _origin;

            this.ray3dUp = new Ray3d(origin, new Vector3d(0, 0, 1));
            this.ray3dDown = new Ray3d(origin, new Vector3d(0, 0, -1));
            double _scale = 1.1;
            this.bounds = new Bounds(origin.ToVector3(), new Vector3(ViewModel.Instance.SamplingAccuracy.MilliMeterToFeet() * _scale, ViewModel.Instance.SamplingAccuracy.MilliMeterToFeet() * _scale, _depth));

            this.triangle3ds = Polygon2d.MakeRectangle(_origin.ToVector2d(), ViewModel.Instance.SamplingAccuracy.MilliMeterToFeet(), ViewModel.Instance.SamplingAccuracy.MilliMeterToFeet()).Triangle3dsByRectangle(_origin.z).ToList();
        }
        internal RectangleF ToRectangleF(Bounds boundingBoxXYZ)
        {
            Vector3 min = boundingBoxXYZ.Min;
            Vector3 max = boundingBoxXYZ.Max;
            return new RectangleF((float)min.X, (float)min.Y, (float)(max.X - min.X), (float)(max.Y - min.Y));
        }
    }
}
