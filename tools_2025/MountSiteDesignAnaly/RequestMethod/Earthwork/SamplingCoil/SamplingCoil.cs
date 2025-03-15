using Autodesk.Revit.DB;
using g3;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PubFuncWt;
using goa.Common;
using goa.Common.g3InterOp;
using Octree;
using System.Drawing;

namespace MountSiteDesignAnaly
{
    class SamplingCoil
    {
        internal Element coilEle;
        internal BaseFace face;
        internal double elevation;

        internal List<Curve> curves;
        internal Vector3d center;

        internal virtual List<Curve> GetCurves() { return new List<Curve>(); }
        /// <summary>
        /// 要确定楼板高度
        /// </summary>
        internal CurveArray FloorCurveArray
        {
            get
            {
                CurveArray curveArray = new CurveArray();
                this.curves.ForEach(p =>
                {
                    //XYZ p0 = p.GetEndPoint(0);
                    //XYZ p1 = p.GetEndPoint(1);

                    //curveArray.Append(Line.CreateBound(new XYZ(p0.X, p0.Y, this.elevation), new XYZ(p1.X, p1.Y, this.elevation)));
                    curveArray.Append(p);
                });
                return curveArray;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal SamplingCoil(Element _ele)
        {
            this.coilEle = _ele;

            this.elevation = this.coilEle.LookupParameter("底板高程").AsDouble();
            // 计算净高，需要减去
            this.elevation -= ViewModel.Instance.BottomPlateThickness.MilliMeterToFeet();// 底板厚度
            this.elevation -= ViewModel.Instance.CushionThickness.MilliMeterToFeet();// 垫层厚度
        }
        //internal SamplingCoil(Element _ele)
        //{
        //    this.coilEle = _ele;

        //    this.elevation = this.coilEle.LookupParameter("底板高程").AsDouble();
        //}

        internal SamplingCoil(BaseFace _face)
        {
            this.face = _face;

            this.elevation = this.face.baseFace.GetEdgeCurves().First().GetEndPoint(0).Z;
            // 计算净高，需要减去
            this.elevation -= ViewModel.Instance.BottomPlateThickness.MilliMeterToFeet();// 底板厚度
            this.elevation -= ViewModel.Instance.CushionThickness.MilliMeterToFeet();// 垫层厚度

            XYZ _center = this.face.baseFace.GetCentroid();
            this.center = new Vector3d(_center.X, _center.Y, _center.Z);
        }

        internal Polygon2d Polygon2d()
        {
            return this.curves.ToPolygon2d();
        }

        internal Poly2dSampling GetPoly2dSampling()
        {

            return new Poly2dSampling(this.Polygon2d(), this.face, this.elevation, this.center);
        }

        internal IEnumerable<Line> SamplingLine(List<Vector3d> vector3ds)
        {
            foreach (var item in vector3ds)
            {
                yield return (new XYZ(item.x, item.y, item.z)).VerticalLine();
            }
        }

        internal void SetVolumn(Document doc, double volumn)
        {
            using (Transaction trans = new Transaction(doc, "填写土方量值"))
            {
                trans.Start();
                this.coilEle.LookupParameter("土方量").Set(volumn.CUBIC_METERStoCUBIC_FEET());

                trans.Commit();
            }
        }

        internal Floor floor;
        internal void CreatFloor(Document doc)
        {
            using (Transaction trans = new Transaction(doc, "创建楼板"))
            {
                trans.Start();
                this.floor = doc.Create.NewFloor(this.FloorCurveArray, false);
                trans.Commit();
            }
        }
    }

}
