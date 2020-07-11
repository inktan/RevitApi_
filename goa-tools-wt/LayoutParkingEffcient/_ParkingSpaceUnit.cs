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
    using cInt = Int64;

    using Path = List<IntPoint>;
    using Paths = List<List<IntPoint>>;
    using Path_xyz = List<XYZ>;
    using Paths_xyz = List<List<XYZ>>;

    class ParkingSpaceUnit
    {
        //从中心点算起
        //public XYZ leftDown { get; set; }
        //public XYZ leftUp { get; set; }
        //public XYZ rightUp { get; set; }
        //public XYZ rightDown { get; set; }
        //public XYZ middlePoint { get; set; }

        //public Path path { get; set; }
        //public Path_xyz path_xyz { get; set; }

        //// 构造函数
        //public ParkingSpaceUnit(XYZ middleXyz, double height, double wight)
        //{
        //    this.leftDown = new XYZ(middleXyz.X - wight / 2, middleXyz.Y - height / 2, 0);
        //    this.leftUp = new XYZ(middleXyz.X - wight / 2, middleXyz.Y + height / 2, 0);
        //    this.rightUp = new XYZ(middleXyz.X + wight / 2, middleXyz.Y + height / 2, 0);
        //    this.rightDown = new XYZ(middleXyz.X + wight / 2, middleXyz.Y - height / 2, 0);
        //    this.middlePoint = middleXyz;

        //    this.path_xyz = new List<XYZ>() { leftDown , leftUp , rightUp , rightDown };
        //    this.path = clipper_methods.Path_xyzToPath(path_xyz);
        //}

        //从车位左下角点算起

        public XYZ leftDown { get; set; }
        public XYZ leftUp { get; set; }
        public XYZ rightUp { get; set; }
        public XYZ rightDown { get; set; }
        public XYZ middlePoint { get; set; }
        public bool hasColumn { get; set; }
        public XYZ upColunmPoint { get; set; }
        public XYZ bottomColunmPoint { get; set; }

        public Path path { get; set; }
        public Path_xyz path_xyz { get; set; }

        // 构造函数
        public ParkingSpaceUnit()
        {
            this.leftDown = XYZ.Zero;
            this.leftUp = new XYZ(10, 0, 0);
            this.rightUp = new XYZ(10, 10, 0);
            this.rightDown = new XYZ(10, 0, 0);
            this.middlePoint = new XYZ(5, 5, 0);

            this.path_xyz = new List<XYZ>() { leftDown, leftUp, rightUp, rightDown };
            this.path = clipper_methods.Path_xyzToPath(path_xyz);

        }
        public ParkingSpaceUnit(XYZ leftDown, double height, double wight, double columnHeight, double columnWidth, double columnBurfferDistance, bool isHasCloumn)
        {
            if (!isHasCloumn)//无柱距
            {
                this.leftDown = leftDown;
                this.leftUp = new XYZ(leftDown.X, leftDown.Y + height, 0);
                this.rightUp = new XYZ(leftDown.X + wight, leftDown.Y + height, 0);
                this.rightDown = new XYZ(leftDown.X + wight, leftDown.Y, 0);
                this.middlePoint = new XYZ(leftDown.X + wight / 2, leftDown.Y + height / 2, 0);

            }
            else if (isHasCloumn)//有柱距
            {
                this.leftDown = leftDown;
                this.leftUp = new XYZ(leftDown.X, leftDown.Y + height, 0);
                this.rightUp = new XYZ(leftDown.X + wight + columnWidth + columnBurfferDistance * 2, leftDown.Y + height, 0);
                this.rightDown = new XYZ(leftDown.X + wight + columnWidth + columnBurfferDistance * 2, leftDown.Y, 0);
                this.middlePoint = new XYZ(leftDown.X + wight / 2 + columnWidth + columnBurfferDistance * 2, leftDown.Y + height / 2, 0);
            }

            this.bottomColunmPoint = leftDown + new XYZ(columnWidth / 2 + columnBurfferDistance, columnHeight / 2, 0);
            this.upColunmPoint = leftDown + new XYZ(columnWidth / 2 + columnBurfferDistance, height - columnHeight / 2, 0);

            this.path_xyz = new List<XYZ>() { leftDown - new XYZ(0, 1, 0) - new XYZ(1, 0, 0), leftUp - new XYZ(1, 0, 0), rightUp, rightDown - new XYZ(0, 1, 0) };//为了满足能够完美切除，底部往下做一个缓冲值
            this.path = clipper_methods.Path_xyzToPath(path_xyz);
        }
        public ParkingSpaceUnit(XYZ leftDown, double height, double wight, double mainRoad, double columnHeight, double columnWidth, double columnBurfferDistance, bool isHasCloumn)
        {
            if (!isHasCloumn)//无柱距
            {
                this.leftDown = leftDown;
                this.leftUp = new XYZ(leftDown.X, leftDown.Y + height + mainRoad, 0);
                this.rightUp = new XYZ(leftDown.X + wight, leftDown.Y + height + mainRoad, 0);
                this.rightDown = new XYZ(leftDown.X + wight, leftDown.Y, 0);

                this.middlePoint = new XYZ(leftDown.X + wight / 2, leftDown.Y + height / 2, 0);
            }
            else if (isHasCloumn)//有柱距
            {
                this.leftDown = leftDown;
                this.leftUp = new XYZ(leftDown.X, leftDown.Y + height + mainRoad, 0);
                this.rightUp = new XYZ(leftDown.X + wight + columnWidth + columnBurfferDistance * 2, leftDown.Y + height + mainRoad, 0);
                this.rightDown = new XYZ(leftDown.X + wight + columnWidth + columnBurfferDistance * 2, leftDown.Y, 0);

                this.middlePoint = new XYZ(leftDown.X + wight / 2 + columnWidth + columnBurfferDistance * 2, leftDown.Y + height / 2, 0);
            }

            this.bottomColunmPoint = leftDown + new XYZ(columnWidth / 2 + columnBurfferDistance, columnHeight / 2, 0);
            this.upColunmPoint = leftDown + new XYZ(columnWidth / 2 + columnBurfferDistance, height - columnHeight / 2, 0);

            this.path_xyz = new List<XYZ>() { leftDown - new XYZ(0, 1, 0) - new XYZ(1, 0, 0), leftUp - new XYZ(1, 0, 0), rightUp, rightDown - new XYZ(0, 1, 0) };//为了满足能够完美切除，底部往下做一个缓冲值
            this.path = clipper_methods.Path_xyzToPath(path_xyz);
        }
    }
}
