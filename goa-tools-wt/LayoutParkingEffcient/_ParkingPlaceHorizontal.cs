//using System;
//using System.Collections.Generic;
//using Form_ = System.Windows.Forms;
//using System.Linq;

//using Autodesk.Revit.UI;
//using Autodesk.Revit.UI.Selection;
//using Autodesk.Revit.ApplicationServices;
//using Autodesk.Revit.DB;
//using Autodesk.Revit.DB.Structure;
//using Autodesk.Revit.Attributes;
//using Autodesk.Revit.DB.Events;
//using Autodesk.Revit.DB.IFC;

//using goa.Common;
//using ClipperLib;
//using wt_Common;

//namespace LayoutParkingEffcient
//{
//    class ParkingPlaceHorizontal
//    {
//        public int MyProperty { get; set; }

//        //public Line boundaryTop { get; set; }
//        //public Line boundaryLeft { get; set; }
//        //public Line boundaryBottom { get; set; }
//        //public Line boundaryRight { get; set; }
//        public XYZ leftDown { get; }
//        public XYZ leftUp { get; }
//        public XYZ rightUp { get; }
//        public XYZ rightDown { get; }
//        public XYZ middlePoint { get; }

//        //车位中心点已经落在目标区域内
//        public ParkingPlaceHorizontal(XYZ middleXyz, double height, double wight)
//        {
//            this.leftDown = new XYZ(middleXyz.X - height / 2, middleXyz.Y - wight / 2, 0);
//            this.leftUp = new XYZ(middleXyz.X - height / 2, middleXyz.Y + wight / 2, 0);
//            this.rightUp = new XYZ(middleXyz.X + height / 2, middleXyz.Y + wight / 2, 0);
//            this.rightDown = new XYZ(middleXyz.X + height / 2, middleXyz.Y - wight / 2, 0);
//            this.middlePoint = middleXyz;
//        }

//        public bool isCarInsideCanPlacedRegion(CurveArray curveArray, CurveArrArray curveArrArray)
//        {
//            //判断 车位中点 是否在目标轮廓区域内 不可压边界线
//            bool isIN = isCarMiddlePointInsideCanPlacedRegion(curveArray, curveArrArray);
//            //bool _isIN = isCarFourcornerInCanPlacedRegion(curveArray, curveArrArray);

//            //return isIN && _isIN;
//            return isIN ;
//        }
//        public bool isCarMiddlePointInsideCanPlacedRegion(CurveArray curveArray, CurveArrArray curveArrArray)
//        {
//            //判断 车位中点 是否在目标轮廓区域内 不可压边界线
//            bool middle = _Methods.IsInsidePolygon(this.middlePoint, curveArray);
//            bool _middle = false;
//            foreach (CurveArray _curevs in curveArrArray)
//            {
//                _middle = _Methods.IsInsideOrOnEdgeOfPolygon(this.middlePoint, _curevs);
//                if (_middle)
//                {
//                    _middle = true;
//                    break;
//                }
//            }

//            return middle && !_middle;
//        }
//        public bool isCarFourcornerInCanPlacedRegion(CurveArray curveArray, CurveArrArray curveArrArray)
//        {
//            //四个角点不位于目标区域边界与障碍物边界之间，角点可以压线
//            bool isNeeded = false;
//            //判断是否在目标轮廓区域内 可压线
//            bool ld = _Methods.IsInsideOrOnEdgeOfPolygon(this.leftDown, curveArray);
//            bool lu = _Methods.IsInsideOrOnEdgeOfPolygon(this.leftUp, curveArray);
//            bool ru = _Methods.IsInsideOrOnEdgeOfPolygon(this.rightUp, curveArray);
//            bool rd = _Methods.IsInsideOrOnEdgeOfPolygon(this.rightDown, curveArray);
//            //判断是否在障碍物区域外 可压线
//            bool isInsideObstalRegion = false;
//            foreach (CurveArray _curevs in curveArrArray)
//            {
//                bool _ld = _Methods.IsInsidePolygon(this.leftDown, _curevs);
//                bool _lu = _Methods.IsInsidePolygon(this.leftUp, _curevs);
//                bool _ru = _Methods.IsInsidePolygon(this.rightUp, _curevs);
//                bool _rd = _Methods.IsInsidePolygon(this.rightDown, _curevs);
//                if (_ld || _lu || _ru || _rd)
//                {
//                    isInsideObstalRegion = true;
//                    break;
//                }
//            }
//            //
//            if ((ld && lu && ru && rd) && (!isInsideObstalRegion))
//            {
//                isNeeded = true;
//            }
//            return isNeeded;
//        }
//    }
//}
