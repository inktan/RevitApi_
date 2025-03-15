
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Selection;
using g3;
using System.Diagnostics;
using PubFuncWt;
using System.Windows.Media;
using goa.Common;

namespace BSMT_PpLayout
{
    internal class DataCollection
    {

        internal DataCollection()
        {
        }

        internal XYZ Location = XYZ.Zero;// 表格定位点
        internal ElementId ElementId = ElementId.InvalidElementId;
        internal string DseignName = null;

        internal double L_ = 0;// 地下室外墙周长之和
        internal double S_ = 0;// 地下室建筑面积
        internal double S0_ = 0;// 塔楼投影面积之和
        internal double S1_ = 0;// 机动车库面积
        internal double S2_ = 0;// 主楼非停车区域面积 主楼轮廓内停车区以外的面积，主要包含门厅、核心筒、主楼内储藏等
        internal double S3_ = 0;// 大型设备用房面积
        internal double S4_ = 0;// 非机动车库面积-不含夹层
        internal double S5_ = 0;// 非机动车库面积-夹层
        internal double S6_ = 0;// 工具间
        internal double S7_ = 0;// 公变所
        internal double S8_ = 0;// 储藏间
        internal double S9_ = 0;// 人防分区
        internal double S10_ = 0;// 核心筒

        internal double N_ = 0;
        // 平层所有停车数量

        // 子母车位_
        internal double N_AttachedPP = 0.0;
        // 无障碍车位_
        internal double N_BarrierFreePP = 0.0;
        // 大车位_
        internal double N_BigParkSpace = 0.0;
        // 快充电位_
        internal double N_FastChargePP = 0.0;
        // 机械车位_
        internal double N_MechanicalPP = 0.0;
        // 微型车位_
        internal double N_MiniParkSpace = 0.0;
        // 停车位_
        internal double N_ParkSpace = 0.0;
        // 回车_
        internal double N_EndPP = 0.0;
        // 公共泊车位_
        internal double N_PublicPP = 0.0;
        // 慢充电位_
        internal double N_SlowChargePP = 0.0;

        /// <summary>
        /// 统计当前区域对应地库的各项指标
        /// </summary>
        internal void Calculate(Bsmt bsmt)
        {
            this.ElementId = bsmt.BsmtBound.Id;
            this.DseignName = bsmt.BsmtBound.DesignName;

            Vector2d ld = bsmt.BsmtBound.Polygon2d.LDpOfBox2d();
            this.Location = ld.ToXYZ() + new XYZ(0, -100000.0.MilliMeterToFeet(), 0);// 表格定位点，基于地库所在位置矩形的左下角点，进行放置

            List<RevitElePS> inBoundElePses = bsmt.InBoundElePses;

            #region 统计车位数量

            foreach (var item in inBoundElePses)
            {
                if (item.EleProperty == EleProperty.AttachedPP)
                {
                    N_ += item.Count();
                    N_AttachedPP += 1;
                }
                else if (item.EleProperty == EleProperty.BarrierFreePP)
                {
                    N_ += item.Count();
                    N_BarrierFreePP += item.Count();
                }
                else if (item.EleProperty == EleProperty.BigParkSpace)
                {
                    N_ += item.Count();
                    N_BigParkSpace += item.Count();
                }
                else if (item.EleProperty == EleProperty.FastChargePP)
                {
                    N_ += item.Count();
                    N_FastChargePP += item.Count();
                }
                else if (item.EleProperty == EleProperty.MechanicalPP)
                {
                    Parameter parameter = item.Ele.GetParaByName_("机械车位数");
                    int count = parameter.AsInteger();

                    N_ += count;
                    N_MechanicalPP += count;
                }
                else if (item.EleProperty == EleProperty.MiniParkSpace)
                {
                    N_ += item.Count();
                    N_MiniParkSpace += 1;
                }
                else if (item.EleProperty == EleProperty.ParkSpace)
                {
                    N_ += item.Count();
                    N_ParkSpace += item.Count();
                }
                else if (item.EleProperty == EleProperty.EndPP)
                {
                    // 回车位不被统计到有效停车位数量上
                    //N_ += item.Count();
                    N_EndPP += item.Count();
                }
                else if (item.EleProperty == EleProperty.PublicPP)
                {
                    N_ += item.Count();
                    N_PublicPP += item.Count();
                }
                else if (item.EleProperty == EleProperty.SlowChargePP)
                {
                    N_ += item.Count();
                    N_SlowChargePP += item.Count();
                }

            }

            #endregion
            #region 统计各项面积、周长……

            List<BoundO> revitEleFRs = bsmt.InBoundEleFRes;
            Polygon2d bsmntPolygon2d = bsmt.Polygon2dInward;

            L_ = bsmt.BsmtBound.Length;
            S_ = bsmt.BsmtBound.Area;
            S1_ = bsmt.BsmtBound.Area;

            foreach (var item in revitEleFRs)
            {
                // 加上下沉庭院的周长
                if (item.EleProperty == EleProperty.SinkingCourtyard)
                {
                    Polygon2d polygon2d01 = item.polygon2d;
                    S_ -= polygon2d01.IntersectionArea(bsmntPolygon2d);
                    S1_ -= polygon2d01.IntersectionArea(bsmntPolygon2d);
                    L_ += polygon2d01.ArcLength;
                }
                // 塔楼投影
                else if (item.EleProperty == EleProperty.ResidenStruRegion)
                {
                    S0_ += item.Area;
                }
                // 主楼非停车区域
                else if (item.EleProperty == EleProperty.MainBuildingNonParkingArea)
                {
                    S2_ += item.Area;
                    S1_ -= item.Area;
                }
                // 单元门厅
                else if (item.EleProperty == EleProperty.UnitFoyer)
                {
                    S2_ += item.Area;
                    S1_ -= item.Area;
                }
                // 核心筒
                else if (item.EleProperty == EleProperty.CoreTube)
                {
                    S2_ += item.Area;
                    S1_ -= item.Area;
                }
                // 设备用房
                else if (item.EleProperty == EleProperty.EquRoom)
                {
                    S3_ += item.Area;
                    S1_ -= item.Area;
                }
                // 非机动车库（不含夹层）
                else if (item.EleProperty == EleProperty.NonVehicleGarage)
                {
                    S4_ += item.Area;
                    S1_ -= item.Area;
                }
                // 非机动车库（夹层）
                else if (item.EleProperty == EleProperty.NonVehicleGarage_Mezzanine)
                {
                    S5_ += item.Area;
                }
                // 工具间
                else if (item.EleProperty == EleProperty.ToolRoom)
                {
                    S6_ += item.Area;
                }
                // 工具间
                else if (item.EleProperty == EleProperty.ToolRoom)
                {
                    S6_ += item.Area;
                }
                // 住宅公用变电所
                else if (item.EleProperty == EleProperty.ResidentialUtilitySubstation)
                {
                    S7_ += item.Area;
                }
                // 储藏间
                else if (item.EleProperty == EleProperty.Storeroom)
                {
                    S8_ += item.Area;
                } // 人防分区
                else if (item.EleProperty == EleProperty.AirDefenseDivision)
                {
                    S9_ += item.Area;
                } // 核心筒
                else if (item.EleProperty == EleProperty.CoreTube)
                {
                    S10_ += item.Area;
                }
            }
            #endregion

        }

    }//
}//
