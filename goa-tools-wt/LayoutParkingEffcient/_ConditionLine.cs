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

    public enum LineStyleId : int
    {
        None = 0,
        RedLineId = 1,//地库_场地控制红线
        OffsetRedLineId = 2,//地库_停车区域边界线
        ObstacleCurveId = 3,//地库_障碍物边界线
        MainRoadId = 4,//地库_主车道中心线
        CarTailId = 5,
        basementWallId = 6,
    }

    class ConditionLine
    {
        public LineStyleId lineStyleId { get; set; }
        public Line line { get; set; }

        public ConditionPoint conditionPoint { get; set; }

        public ConditionLine()
        {
            lineStyleId = LineStyleId.ObstacleCurveId;
        }
        public ConditionLine(Document doc, Line _l)
        {
            line = _l;

            lineStyleId = LineStyleId.MainRoadId;
            if (_l.GraphicsStyleId != new ElementId(-1))
            {
                string _lineStyleName = doc.GetElement(_l.GraphicsStyleId).Name;//提取详图线的线样式

                if (_lineStyleName == "地库_场地控制红线")
                {
                    lineStyleId = LineStyleId.RedLineId;
                }
                else if (_lineStyleName == "地库_地库外墙线")
                {
                    lineStyleId = LineStyleId.basementWallId;
                }
                else if (_lineStyleName == "地库_障碍物边界线")
                {
                    lineStyleId = LineStyleId.ObstacleCurveId;
                }
                else if (_lineStyleName == "地库_红线退距线")
                {
                    lineStyleId = LineStyleId.OffsetRedLineId;
                }
            }
        }

        //public ConditionLine( Document doc, Curve _c)//在line找不到重合详图属性线的时候，默认为车道属性
        //{
        //    lineStyleId = LineStyleId.MainRoadId;

        //    if (_c.GraphicsStyleId != new ElementId(-1))
        //    {
        //        string _lineStyleName = doc.GetElement(_c.GraphicsStyleId).Name;//提取详图线的线样式

        //        if (_lineStyleName == "地库_场地控制红线")
        //        {
        //            lineStyleId = LineStyleId.RedLineId;
        //        }
        //        else if (_lineStyleName == "地库_地库外墙线")
        //        {
        //            lineStyleId = LineStyleId.basementWallId;
        //        }
        //        else if (_lineStyleName == "地库_障碍物边界线")
        //        {
        //            lineStyleId = LineStyleId.ObstacleCurveId;
        //        }
        //        else if (_lineStyleName == "地库_红线退距线")
        //        {
        //            lineStyleId = LineStyleId.OffsetRedLineId;
        //        }
        //    }
        //}

        /// <summary>
        /// 构造函数，提取实体线的line和lineStyle
        /// </summary>
        //public ConditionLine(DetailLine detailLine)
        //{
        //    string _lineStyleName = detailLine.LineStyle.Name;//提取详图线的线样式

        //    if (_lineStyleName == "地库_场地控制红线")
        //    {
        //        this.lineStyleId = LineStyleId.RedLineId;
        //    }
        //    else if (_lineStyleName == "地库_地库外墙线")
        //    {
        //        lineStyleId = LineStyleId.basementWallId;
        //    }
        //    else if (_lineStyleName == "地库_障碍物边界线")
        //    {
        //        lineStyleId = LineStyleId.ObstacleCurveId;
        //    }
        //    else if (_lineStyleName == "地库_红线退距线")
        //    {
        //        lineStyleId = LineStyleId.OffsetRedLineId;
        //    }
        //}
    }
}
