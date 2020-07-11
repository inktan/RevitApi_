using System;
using System.Collections.Generic;
using Form_ = System.Windows.Forms;
using System.Linq;

using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB.Events;
using Autodesk.Revit.DB.IFC;

using goa.Common;
using ClipperLib;
using wt_Common;

namespace LayoutParkingEffcient
{
    class ParkingPlaceVertical : IDisposable
    {
        public int MyProperty { get; set; }

        //public Line boundaryTop { get; set; }
        //public Line boundaryLeft { get; set; }
        //public Line boundaryBottom { get; set; }
        //public Line boundaryRight { get; set; }
        public XYZ leftDown { get; }
        public XYZ leftUp { get; }
        public XYZ rightUp { get; }
        public XYZ rightDown { get; }
        public XYZ middlePoint { get; }

        //车位中心点已经落在目标区域内
        public ParkingPlaceVertical(ParkingSpaceUnit parkingSpaceUnit)
        {
            this.leftDown = parkingSpaceUnit.leftDown;
            this.leftUp = parkingSpaceUnit.leftUp;
            this.rightUp = parkingSpaceUnit.rightUp;
            this.rightDown = parkingSpaceUnit.rightDown;
            this.middlePoint = parkingSpaceUnit.middlePoint;
        }

        public bool isCarInsideCanPlacedRegion(CurveArray curveArray, CurveArrArray curveArrArray)//由于clipper传递后，边界线会产生误差值，这里设置为点与线的重合容差为10mm
        {
            bool isIN = isCarMiddlePointInsideCanPlacedRegion(curveArray, curveArrArray);
            bool _isIN = isCarFourcornerInCanPlacedRegion(curveArray, curveArrArray);

            return isIN && _isIN;
        }
        public bool isCarMiddlePointInsideCanPlacedRegion(CurveArray curveArray, CurveArrArray curveArrArray)//由于clipper传递后，边界线会产生误差值，这里设置为点与线的重合容差为0.5feet
        {
            //判断 车位中点 是否在目标轮廓区域内 不可压边界线
            bool middle = _Methods.IsInsidePolygon(this.middlePoint, curveArray);
            bool _middle = false;
            foreach (CurveArray _curevs in curveArrArray)
            {
                _middle = _Methods.IsInsideOrOnEdgeOfPolygon(this.middlePoint, _curevs);
                if (_middle)
                {
                    _middle = true;
                    break;
                }
            }

            return middle && !_middle;
        }

        public bool isCarFourcornerInCanPlacedRegion(CurveArray curveArray, CurveArrArray nowObstacleCurveArrArray)//由于clipper传递后，边界线会产生误差值，这里设置为点与线的重合容差为0.5feet
        {
            //判断是否在目标轮廓区域内 可压线
            bool ld = _Methods.IsInsideOrOnEdgeOfPolygon(this.leftDown, curveArray);
            if (!ld) return false;
            bool lu = _Methods.IsInsideOrOnEdgeOfPolygon(this.leftUp, curveArray);
            if (!lu) return false;
            bool ru = _Methods.IsInsideOrOnEdgeOfPolygon(this.rightUp, curveArray);
            if (!ru) return false;
            bool rd = _Methods.IsInsideOrOnEdgeOfPolygon(this.rightDown, curveArray);
            if (!rd) return false;
            bool mid = _Methods.IsInsideOrOnEdgeOfPolygon(this.middlePoint, curveArray);//该处中心点会出现误差值
            if (!mid) return false;

            //判断是否在障碍物区域外 可压线
            foreach (CurveArray _curevs in nowObstacleCurveArrArray)
            {
                bool _ld = _Methods.IsInsidePolygon(this.leftDown, _curevs);
                if (_ld) return false;
                bool _lu = _Methods.IsInsidePolygon(this.leftUp, _curevs);
                if (_lu) return false;
                bool _ru = _Methods.IsInsidePolygon(this.rightUp, _curevs);
                if (_ru) return false;
                bool _rd = _Methods.IsInsidePolygon(this.rightDown, _curevs);
                if (_rd) return false;
                bool _mid = _Methods.IsInsidePolygon(this.middlePoint, _curevs);
                if (_mid) return false;
            }
            return true;
        }
        /// <summary>
        /// 由于已经是对目标region进行clipper裁剪，因此，该判断只需要处理与障碍物线圈的关系，即可。
        /// </summary>
        public bool isCarFourcornerInCanPlacedRegion(CurveArrArray nowObstacleCurveArrArray)//由于clipper传递后，边界线会产生误差值，这里设置为点与线的重合容差为0.5feet
        {
            //四个角点不位于目标区域边界与障碍物边界之间，角点可以压线
            bool isNeeded = false;

            //判断是否在障碍物区域外 可压线
            bool isInsideObstalRegion = false;
            foreach (CurveArray _curevs in nowObstacleCurveArrArray)
            {
                bool _ld = _Methods.IsInsidePolygon(this.leftDown, _curevs);
                bool _lu = _Methods.IsInsidePolygon(this.leftUp, _curevs);
                bool _ru = _Methods.IsInsidePolygon(this.rightUp, _curevs);
                bool _rd = _Methods.IsInsidePolygon(this.rightDown, _curevs);
                bool _mid = _Methods.IsInsidePolygon(this.middlePoint, _curevs);
                if (_ld || _lu || _ru || _rd || _mid)
                {
                    isInsideObstalRegion = true;
                    break;
                }
            }
            if (!isInsideObstalRegion)
            {
                isNeeded = true;
            }
            return isNeeded;
        }

        public void Dispose()
        {
            //throw new NotImplementedException();
        }
    }
}
