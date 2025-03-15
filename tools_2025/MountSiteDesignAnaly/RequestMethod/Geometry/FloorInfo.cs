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

namespace MountSiteDesignAnaly
{
    class FloorInfo : RevitEleInfo
    {
        /// <summary>
        /// 上部楼板的结构面标高
        /// </summary>
        internal double elevation => this.level.Elevation + this.floor.get_Parameter(BuiltInParameter.FLOOR_HEIGHTABOVELEVEL_PARAM).AsDouble();

        internal Floor floor;

        /// <summary>
        /// 上部楼板，待输入覆土厚度
        /// </summary>
        /// <param name="_floor"></param>
        internal FloorInfo(Element _floor) : base(_floor)
        {
            this.floor = _floor as Floor;
        }
        internal Level level => this.floor.Document.GetElement(this.floor.LevelId) as Level;

        internal double Thickness => this.floor.GetParameterByBuiltInParameter(BuiltInParameter.FLOOR_ATTR_THICKNESS_PARAM).AsDouble();

        /// <summary>
        /// 有对应地下室顶板的覆土楼板范围，目的是为了求出交错区域
        /// </summary>
        internal List<SingleGreenInfor> SingleGreenInfors { get; set; }

        /// <summary>
        /// 去掉退距后的绿化面积
        /// 求每个楼板对应的有效绿化线圈
        /// </summary>
        internal void CalGreenInfosByBsmtFloor(List<FloorInfo> _bsmtTopFloors)
        {
            this.SingleGreenInfors = new List<SingleGreenInfor>();

            if (this.Poly.Area.EqualZreo())
                return;

            Polygon2d thisPoly = this.Poly.InwardOffeet(Precision_.TheShortestDistance);

            int count = 0;
            foreach (var bsmtTopFloor in _bsmtTopFloors)
            {
                Polygon2d frPoly = bsmtTopFloor.Poly;
                if (frPoly.Area.EqualZreo())
                    continue;

                // polygon2d的相关判断，不包含大圈包小圈的情况
                if (thisPoly.Intersects(frPoly)
                    || frPoly.Intersects(thisPoly)
                    || thisPoly.Contains(frPoly)
                    || frPoly.Contains(thisPoly))
                {
                    // 减去覆土区域上，不符合绿地规范的范围

                    SingleGreenInfor greenInfor = new SingleGreenInfor(this, bsmtTopFloor);
                    count++;
                    this.SingleGreenInfors.Add(greenInfor);
                }
            }

            if (count == 0)
            {
                this.SingleGreenInfors = new List<SingleGreenInfor>() { new SingleGreenInfor(this, null) };
            }
        }


        /// <summary>
        /// 判断当前绿地区域中，是否包含退界区域
        /// </summary>
        /// <param name="filledRegionInfos"></param>
        internal void CalGreenInfosByRetreatFilledRegion(List<FilledRegionInfo> filledRegionInfos)
        {
            //throw new NotImplementedException();
            foreach (var item in this.SingleGreenInfors)
            {
                Polygon2d coverPoly = item.coverFloor.Poly;
                foreach (var filledRegionInfo in filledRegionInfos)
                {
                    Polygon2d filledRegionPoly = filledRegionInfo.Poly;

                    if (coverPoly.Intersects(filledRegionPoly)
                      || filledRegionPoly.Intersects(coverPoly)
                      || coverPoly.Contains(coverPoly)
                      || filledRegionPoly.Contains(coverPoly))
                    {
                        item.filledRegionInfo = filledRegionInfo;
                    }
                }
            }
        }
    }
}
