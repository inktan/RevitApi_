using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSMT_PpLayout
{
    internal enum FollowPathCutType:int
    {
        None,
        AllBoundary,
        Just_no_BsmeWall,// 非地库外墙边界，皆可停车
        Just_x_Lane,// X方向边界，皆可停车
        Just_y_Lane,// Y方向边界，皆可停车
        Just_Lane,
    }

    internal enum BigOrSmallColumn :int
    { 
        None,
        Big,
        Small,
    }

    internal enum CurveLoopShapeType : int
    {
        None,
        Rectangle,
        Trapezoid,//梯形
    }
    /// <summary>
    /// 端头车位是否位于尽端式车道
    /// </summary>
    internal enum CarType : int
    {
        None,

        #region 是否为一排车的尽端
        /// <summary>
        /// 尽端式 >=2600mm
        /// </summary>
        EndType,
        /// <summary>
        /// 需要设置联通道 杭州要求 >=8500mm
        /// </summary>
        NoEndType,
        #endregion

        #region self
        /// <summary>
        /// 车尾
        /// </summary>
        CarTail,
        /// <summary>
        /// 车头
        /// </summary>
        CarHead,
        /// <summary>
        /// 侧边界
        /// </summary>
        Broadside,
        #endregion
    }

    /// <summary>
    /// 所有线属性 枚举值
    /// </summary>
    internal enum EleProperty : int
    {
        None,

        #region 填充区域
        /// <summary>
        /// 地库外墙线
        /// </summary>
        BsmtWall,
        /// <summary>
        /// 核心筒
        /// </summary>
        CoreTube,
        /// <summary>
        /// 设备用房
        /// </summary>
        EquRoom,
        /// <summary>
        /// 防火隔墙
        /// </summary>
        FireWall,
        /// <summary>
        /// 采光井
        /// </summary>
        LightWell,
        /// <summary>
        /// 锁定车位
        /// </summary>
        LockParking,
        /// <summary>
        /// 主楼非停车区域
        /// </summary>
        MainBuildingNonParkingArea,
        /// <summary>
        /// 非机动车车库
        /// </summary>
        NonVehicleGarage,
        /// <summary>
        /// 非机动车车库 夹层
        /// </summary>
        NonVehicleGarage_Mezzanine,
        /// <summary>
        /// 障碍物性质
        /// </summary>
        Obstructive,
        /// <summary>
        /// 塔楼结构投影 考虑进深 需要进一步细化 东西南北进深
        /// </summary>
        ResidenOpenRoom,
        /// <summary>
        /// 塔楼结构投影
        /// </summary>
        ResidenStruRegion,
        /// <summary>
        /// 住宅公用变电所
        /// </summary>
        ResidentialUtilitySubstation,
        /// <summary>
        /// 剪力墙
        /// </summary>
        ShearWall,
        /// <summary>
        /// 当前已经存在的可停车填充区域，需要找到落在各个停车边界区域内的未锁定停车位族实例
        /// </summary>
        SubPsAreaExit,
        /// <summary>
        /// 下沉庭院
        /// </summary>
        SinkingCourtyard,
        /// <summary>
        /// 结构柱
        /// </summary>
        StruColumn,
        /// <summary>
        /// 工具箱
        /// </summary>
        ToolRoom,
        /// <summary>
        /// 单元门厅
        /// </summary>
        UnitFoyer,
        /// <summary>
        /// 出入口坡道
        /// </summary>
        Ramp,
        /// <summary>
        /// 储藏间
        /// </summary>
        Storeroom,
        /// <summary>
        /// 人防分区
        /// </summary>
        AirDefenseDivision,
        #endregion

        #region 线
        /// <summary>
        /// 通车道
        /// </summary>
        Circle,      
        /// <summary>
        /// 通车道
        /// </summary>
        Lane,
        /// <summary>
        /// 主通车道
        /// </summary>
        PriLane,
        /// <summary>
        /// 次通车道
        /// </summary>
        SecLane,
        /// <summary>
        /// 自定义宽度车道
        /// </summary>
        CusLane,
        #endregion

        #region 族实例 停车位
        /// <summary>
        /// 子母车位
        /// </summary>
        AttachedPP, 
        /// <summary>
        /// 无障碍车位
        /// </summary>
        BarrierFreePP,
        /// <summary>
        /// 大车位
        /// </summary>
        BigParkSpace,
        /// <summary>
        /// 快充电位
        /// </summary>
        FastChargePP, 
        /// <summary>
        /// 机械车位
        /// </summary>
        MechanicalPP,
        /// <summary>
        /// 大车位
        /// </summary>
        MiniParkSpace,
        /// <summary>
        /// 普通停车位 大与小
        /// </summary>
        ParkSpace,
        /// <summary>
        /// 尽端回车位
        /// </summary>
        EndPP,
        /// <summary>
        /// 公共泊车位
        /// </summary>
        PublicPP,
        /// <summary>
        /// 慢充电位
        /// </summary>
        SlowChargePP,
        #endregion

        #region 族实例 地库坡道出入口
        /// <summary>
        /// 直线坡道
        /// </summary>
        VehicleRamp,
        /// <summary>
        /// 弧线坡道 弧段到直段
        /// </summary>
        VehicleRamp_Arc_A,
        /// <summary>
        /// 弧线坡道 直段到弧段到直段
        /// </summary>
        VehicleRamp_Arc_B,
        /// <summary>
        /// 上下坡道
        /// </summary>
        VehicleRamp_UpDown,
        /// <summary>
        /// 上下坡道-弧段
        /// </summary>
        VehicleRamp_UpDown_Arc,
        #endregion

        #region 族实例 其他
        /// <summary>
        /// 基于排好的车位生成的柱子，与属于填充类型的结构柱不同·
        /// </summary>
        ColumnUnit,
        #endregion


    }

    internal enum TowerDistanceJudgment : int
    {
        [Description("数据异常，请手动查看")]
        None,
        //[Description("优劣判断：较为经济的间距区域" + "情况描述：可设置1条车道 + 2排车位")]
        [Description("可设置1条车道 + 2排车位")]
        First,
        //[Description("优劣判断：不经济的间距区域" + "情况描述：可设置2条车道 + 3排车位")]
        [Description("可设置2条车道 + 3排车位")]
        Second,
        //[Description("优劣判断：较为经济的间距区域" + "情况描述：可设置2条车道 + 4排车位")]
        [Description("可设置2条车道 + 4排车位")]
        Third,
        //[Description("优劣判断：不经济的间距区域" + "情况描述：可设置3条车道 + 5排车位")]
        [Description("可设置3条车道 + 5排车位")]
        Fourth,
        //[Description("优劣判断：较为经济的间距区域" + "情况描述：可设置3条车道 + 6排车位")]
        [Description("可设置3条车道 + 6排车位")]
        Fifth,
        //[Description("数据异常，请手动查看")]
        //Sixth,
        //[Description("数据异常，请手动查看")]
        //Seventh,
        //[Description("数据异常，请手动查看")]
        //Eighth,
        //[Description("数据异常，请手动查看")]
        //Ninth,
        //[Description("数据异常，请手动查看")]
        //Tenth,

    }

}
