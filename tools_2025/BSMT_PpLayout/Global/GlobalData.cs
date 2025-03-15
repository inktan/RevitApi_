using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Collections.ObjectModel;
using Autodesk.Revit.DB;
using PubFuncWt;
using WpfMvvm.Infrastruct;
using g3;
using PubFuncWt;

namespace BSMT_PpLayout
{
    public class GlobalData : ObservableObject
    {


        // 需要注意 mvvm模式下的binding需要暴露属性或字段为public级别
        // 采用单例模式
        private static GlobalData _instance;
        /// <summary>
        /// 通过属性实现单例模式 
        /// </summary>
        public static GlobalData Instance
        {
            get
            {
                if (ReferenceEquals(_instance, null))
                {
                    _instance = new GlobalData();
                }
                return _instance;
            }
        }
        internal GlobalData()
        {

        }
        #region 声明全局变量

        //Revit中数据均为 feet

        internal Document Document;

        //
        private string _pSHeight = (6000).ToString();// 停车位 族类型 高度
        [PanelDataAttri]
        public string pSHeight { get { return _pSHeight; } set { _pSHeight = value; RaisePropertyChanged("pSHeight"); } }
        public double pSHeight_num = UnitUtils_.MilliMeterToFeet(6000);
        //
        private string _pSWidth = (2500).ToString();// 停车位 族类型 宽度
        [PanelDataAttri]
        public string pSWidth { get { return _pSWidth; } set { _pSWidth = value; RaisePropertyChanged("pSWidth"); } }
        public double pSWidth_num = UnitUtils_.MilliMeterToFeet(2500);// 停车位 族类型 宽度
        //
        private string _miniPSHeight = (4500).ToString();// 微型
        [PanelDataAttri]
        public string miniPSHeight { get { return _miniPSHeight; } set { _miniPSHeight = value; RaisePropertyChanged("miniPSHeight"); } }
        public double miniPSHeight_num = UnitUtils_.MilliMeterToFeet(4500);
        //
        private string _miniPSWidth = (2300).ToString();// 微型
        [PanelDataAttri]
        public string miniPSWidth { get { return _miniPSWidth; } set { _miniPSWidth = value; RaisePropertyChanged("miniPSWidth"); } }
        public double miniPSWidth_num = UnitUtils_.MilliMeterToFeet(2300);
        //
        private string _pSHeight_Hor = (6000).ToString();// 平行式停车.ToString()
        [PanelDataAttri]
        public string pSHeight_Hor { get { return _pSHeight_Hor; } set { _pSHeight_Hor = value; RaisePropertyChanged("pSHeight_Hor"); } }
        public double pSHeight_Hor_num = UnitUtils_.MilliMeterToFeet(6000);
        //
        private string _pSWidth_Hor = (2400).ToString();// 平行式停车    
        [PanelDataAttri]
        public string pSWidth_Hor { get { return _pSWidth_Hor; } set { _pSWidth_Hor = value; RaisePropertyChanged("pSWidth_Hor"); } }
        public double pSWidth_Hor_num = UnitUtils_.MilliMeterToFeet(2400);
        //
        internal bool wheMechanicalPs; // 划线阵列
        //
        private string _Wd_pri = (6000).ToString();// 停车位 主车道 宽度
        [PanelDataAttri]
        public string Wd_pri { get { return _Wd_pri; } set { _Wd_pri = value; RaisePropertyChanged("Wd_pri"); } }
        public double Wd_pri_num = UnitUtils_.MilliMeterToFeet(6000);
        //
        private string _Wd_sec = (6000).ToString();// 停车位 次车道 宽度
        [PanelDataAttri]
        public string Wd_sec { get { return _Wd_sec; } set { _Wd_sec = value; RaisePropertyChanged("Wd_sec"); } }
        public double Wd_sec_num = UnitUtils_.MilliMeterToFeet(6000);
        //
        private string _Wd_CustomWidth = (6000).ToString();// 停车位 次车道 宽度
        [PanelDataAttri]
        public string Wd_CustomWidth { get { return _Wd_CustomWidth; } set { _Wd_CustomWidth = value; RaisePropertyChanged("Wd_CustomWidth"); } }
        public double Wd_CustomWidth_num = UnitUtils_.MilliMeterToFeet(6000);
        //
        private string _ColumnWidth = (500).ToString();// 柱宽
        [PanelDataAttri]
        public string ColumnWidth { get { return _ColumnWidth; } set { _ColumnWidth = value; RaisePropertyChanged("ColumnWidth"); } }
        public double ColumnWidth_num = UnitUtils_.MilliMeterToFeet(500);
        //
        private string _ColumnBurfferDistance = (100).ToString();//柱子 粉刷 宽度
        [PanelDataAttri]
        public string ColumnBurfferDistance { get { return _ColumnBurfferDistance; } set { _ColumnBurfferDistance = value; RaisePropertyChanged("ColumnBurfferDistance"); } }
        public double ColumnBurfferDistance_num = UnitUtils_.MilliMeterToFeet(100);
        //
        private string _bsmtWallThickness = (400).ToString();//地库墙体厚度
        [PanelDataAttri]
        public string bsmtWallThickness { get { return _bsmtWallThickness; } set { _bsmtWallThickness = value; RaisePropertyChanged("bsmtWallThickness"); } }
        public double bsmtWallThickness_num = UnitUtils_.MilliMeterToFeet(400);
        //
        private string _NumOfIntervalPP = (3.0).ToString();// 间隔车位数量 2/3，对应大小柱网问题
        [PanelDataAttri]
        public string NumOfIntervalPP { get { return _NumOfIntervalPP; } set { _NumOfIntervalPP = value; RaisePropertyChanged("NumOfIntervalPP"); } }
        public double NumOfIntervalPP_num = 3.0;
        //
        private string _ColumnBackwardDistance = (700).ToString();//柱子 后退距离
        [PanelDataAttri]
        public string ColumnBackwardDistance { get { return _ColumnBackwardDistance; } set { _ColumnBackwardDistance = value; RaisePropertyChanged("ColumnBackwardDistance"); } }
        public double ColumnBackwardDistance_num = UnitUtils_.MilliMeterToFeet(700);
        //
        private string _tarParkingEfficiency = (40).ToString();// 目标停车效率
        [PanelDataAttri]
        public string tarParkingEfficiency { get { return _tarParkingEfficiency; } set { _tarParkingEfficiency = value; RaisePropertyChanged("tarParkingEfficiency"); } }
        public double tarParkingEfficiency_num = 40.0.SQUARE_METERStoSQUARE_FEET();
        //
        private string _WalkwayWidth = (1200).ToString();// 走道宽度
        [PanelDataAttri]
        public string WalkwayWidth { get { return _WalkwayWidth; } set { _WalkwayWidth = value; RaisePropertyChanged("WalkwayWidth"); } }
        public double WalkwayWidth_num = UnitUtils_.MilliMeterToFeet(1200);
        //
        private string _MinArea = (10.0).ToString();// 最小面积
        [PanelDataAttri]
        public string MinArea { get { return _MinArea; } set { _MinArea = value; RaisePropertyChanged("MinArea"); } }
        public double MinArea_num = 10.0.SQUARE_METERStoSQUARE_FEET();
        //
        private string _MinDepth = (1200).ToString();// 最小进深
        [PanelDataAttri]
        public string MinDepth { get { return _MinDepth; } set { _MinDepth = value; RaisePropertyChanged("MinDepth"); } }
        public double MinDepth_num = UnitUtils_.MilliMeterToFeet(1200);
        //
        private string _MaxDepth = (1200).ToString();// 最大进深
        [PanelDataAttri]
        public string MaxDepth { get { return _MaxDepth; } set { _MaxDepth = value; RaisePropertyChanged("MaxDepth"); } }
        public double MaxDepth_num = UnitUtils_.MilliMeterToFeet(1200);
        //
        private string _MinWidth = (1200).ToString();//最小开间
        [PanelDataAttri]
        public string MinWidth { get { return _MinWidth; } set { _MinWidth = value; RaisePropertyChanged("MinWidth"); } }
        public double MinWidth_num = UnitUtils_.MilliMeterToFeet(1200);
        //
        private string _MaxWidth = (1500).ToString();//最大开间
        [PanelDataAttri]
        public string MaxWidth { get { return _MaxWidth; } set { _MaxWidth = value; RaisePropertyChanged("MaxWidth"); } }
        public double MaxWidth_num = UnitUtils_.MilliMeterToFeet(1500);
        //
        private string _numberCarsAllocated = (0).ToString();// 应配车位数
        [PanelDataAttri]
        public string numberCarsAllocated { get { return _numberCarsAllocated; } set { _numberCarsAllocated = value; RaisePropertyChanged("numberCarsAllocated"); } }
        public double numberCarsAllocated_num = 0;
        //
        private string _perOfDamagedPs = (10).ToString();// 折损车位比例
        [PanelDataAttri]
        public string perOfDamagedPs { get { return _perOfDamagedPs; } set { _perOfDamagedPs = value; RaisePropertyChanged("perOfDamagedPs"); } }
        public double perOfDamagedPs_num = 10;
        //
        private string _endReturnLength = (26000).ToString();// 尽端回车长度
        [PanelDataAttri]
        public string endReturnLength { get { return _endReturnLength; } set { _endReturnLength = value; RaisePropertyChanged("endReturnLength"); } }
        public double endReturnLength_num = UnitUtils_.MilliMeterToFeet(26000);
        //
        private string _loopReturnLength = (85000).ToString();// 环道回车长度
        [PanelDataAttri]
        public string loopReturnLength { get { return _loopReturnLength; } set { _loopReturnLength = value; RaisePropertyChanged("loopReturnLength"); } }
        public double loopReturnLength_num = UnitUtils_.MilliMeterToFeet(8500);
        //
        private string _fROffsetDis = (1000.0).ToString();// 详图区域偏移距离
        [PanelDataAttri]
        public string fROffsetDis { get { return _fROffsetDis; } set { _fROffsetDis = value; RaisePropertyChanged("fROffsetDis"); } }
        public double fROffsetDis_num = 1000.0;
        //
        private string _lineCheckDis = (500.0).ToString();// 线头检查
        //[PanelDataAttri]
        public string lineCheckDis { get { return _lineCheckDis; } set { _lineCheckDis = value; RaisePropertyChanged("lineCheckDis"); } }
        public double lineCheckDis_num = 500.0;
        //
        internal BigOrSmallColumn BigOrsmallColumn = BigOrSmallColumn.Big;// 大小柱网的选择
        //
        internal bool stopAlgorithm = true;//是否停止计算
        internal bool isAddRemove = true;// 添加/减少方案
        internal bool isAutoTowerDistanceCheck = false;// 是否自动检查塔楼间距
        internal bool wheChamfer = true;// 是否倒角复原
        internal bool wheGenerateTable = true;// 是否倒角复原

        internal List<string> alreadyExistsBaseMentWalNames = new List<string>();// 地库外墙线填充区域标记值
        internal List<string> selViewNames = new List<string>();// UI互动-所选方案名称

        // 局部寻路 坐标值

        internal Vector2d PartAnchorPoint = new Vector2d();
        internal Vector2d SelPointToAdjustStartPoint = new Vector2d();
        // 插车系数
        private string _NorthVehicleInsertionCoefficient = (0.4).ToString();// 北向插车
        [PanelDataAttri]
        public string NorthVehicleInsertionCoefficient { get { return _NorthVehicleInsertionCoefficient; } set { _NorthVehicleInsertionCoefficient = value; RaisePropertyChanged("NorthVehicleInsertionCoefficient"); } }
        public double NorthVehicleInsertionCoefficient_num = 0.4;
        //
        private string _SorthVehicleInsertionCoefficient = (0.0).ToString();// 南向插车
        [PanelDataAttri]
        public string SorthVehicleInsertionCoefficient { get { return _SorthVehicleInsertionCoefficient; } set { _SorthVehicleInsertionCoefficient = value; RaisePropertyChanged("SorthVehicleInsertionCoefficient"); } }
        public double SorthVehicleInsertionCoefficient_num = 0.0;

        public double LeftVehicleInsertionCoefficient_num = 0.0;
        public double RightVehicleInsertionCoefficient_num = 0.0;

        // 出图表达
        private string _ChamferRadius = (5500.0).ToString();// 倒角半径
        [PanelDataAttri]
        public string ChamferRadius { get { return _ChamferRadius; } set { _ChamferRadius = value; RaisePropertyChanged("ChamferRadius"); } }
        public double ChamferRadius_num = UnitUtils_.MilliMeterToFeet(5500);
        //
        private string _EndCirclePosition = (1500.0).ToString();// 圈圈定位
        [PanelDataAttri]
        public string EndCirclePosition { get { return _EndCirclePosition; } set { _EndCirclePosition = value; RaisePropertyChanged("EndCirclePosition"); } }
        public double EndCirclePosition_num = UnitUtils_.MilliMeterToFeet(1500);
        //
        private string _EndCircleRadius = (800.0).ToString();// 圈圈半径
        [PanelDataAttri]
        public string EndCircleRadius { get { return _EndCircleRadius; } set { _EndCircleRadius = value; RaisePropertyChanged("EndCircleRadius"); } }
        public double EndCircleRadius_num = UnitUtils_.MilliMeterToFeet(800.0);

        // 序号 是否加圆圈
        internal bool AddCircle = true;
        //
        private string _NumCircleRadiu = (1200.0).ToString();// 圈圈半径
        [PanelDataAttri]
        public string NumCircleRadiu { get { return _NumCircleRadiu; } set { _NumCircleRadiu = value; RaisePropertyChanged("NumCircleRadiu"); } }
        public double NumCircleRadiu_num = UnitUtils_.MilliMeterToFeet(1200.0);
        //
        private string _NumTextSize = (1.5).ToString();// 文字大小
        [PanelDataAttri]
        public string NumTextSize { get { return _NumTextSize; } set { _NumTextSize = value; RaisePropertyChanged("NumTextSize"); } }
        public double NumTextSize_num = UnitUtils_.MilliMeterToFeet(1.5);

        // 坡道计算结果参数
        internal double TotalRampLength_num = 0.0;
        internal double CurrentLength_num = 0.0;

        // 坡道计算输入参数
        private string _StoreyHeight = (3600.0).ToString();// 层高
        [PanelDataAttri]
        public string StoreyHeight { get { return _StoreyHeight; } set { _StoreyHeight = value; RaisePropertyChanged("StoreyHeight"); } }
        public double StoreyHeight_num = UnitUtils_.MilliMeterToFeet(3600.0);
        //
        private string _GentleSlopeDeg = (0.075).ToString();// 缓坡坡度
        [PanelDataAttri]
        public string GentleSlopeDeg { get { return _GentleSlopeDeg; } set { _GentleSlopeDeg = value; RaisePropertyChanged("GentleSlopeDeg"); } }
        public double GentleSlopeDeg_num = 0.075;
        //
        private string _GentleSlopeLength = (0.075).ToString();// 缓坡坡长
        [PanelDataAttri]
        public string GentleSlopeLength { get { return _GentleSlopeLength; } set { _GentleSlopeLength = value; RaisePropertyChanged("GentleSlopeLength"); } }
        public double GentleSlopeLength_num = 0.075;
        //
        private string _RampSlopeDeg = (0.075).ToString();// 坡度
        [PanelDataAttri]
        public string RampSlopeDeg { get { return _RampSlopeDeg; } set { _RampSlopeDeg = value; RaisePropertyChanged("RampSlopeDeg"); } }
        public double RampSlopeDeg_num = 0.075;
        //
        private string _MinHeightParking = (2100.0).ToString();// 可停车最低净高
        [PanelDataAttri]
        public string MinHeightParking { get { return _MinHeightParking; } set { _MinHeightParking = value; RaisePropertyChanged("MinHeightParking"); } }
        public double MinHeightParking_num = 0.075;
        //


        // 调整自动计算的起点
        internal bool AdjustStartPoint = false;

        #endregion

        #region MVVM

        private string _strBackgroundMonitorDta;// 监控面板
        public string strBackgroundMonitorDta { get { return _strBackgroundMonitorDta; } set { _strBackgroundMonitorDta = value; RaisePropertyChanged("strBackgroundMonitorDta"); } }

        // 测试进度条
        private double _progressBarValue;
        public double ProgreaaBarVlaue { get { return _progressBarValue; } set { _progressBarValue = value; RaisePropertyChanged("ProgreaaBarVlaue"); } }

        public static ObservableCollection<string> basementWallOutlineNames = new ObservableCollection<string>();  // 方案列表
        public static ObservableCollection<string> psTypeNames = new ObservableCollection<string>();  // 车位类型列表

        // S 地下室外墙周长之和
        private string _l;
        [PanelDataAttri]
        public string L { get { return _l; } set { _l = value; RaisePropertyChanged("L"); } }
        // S 地下室建筑面积
        private string _s;
        [PanelDataAttri]
        public string S { get { return _s; } set { _s = value; RaisePropertyChanged("S"); } }
        // S0 地下室周长系数
        private string _k2;
        [PanelDataAttri]
        public string K2 { get { return _k2; } set { _k2 = value; RaisePropertyChanged("K2"); } }
        // S0 塔楼投影面积之和
        private string _s0;
        [PanelDataAttri]
        public string S0 { get { return _s0; } set { _s0 = value; RaisePropertyChanged("S0"); } }
        // S1 机动车库面积
        private string _s1;
        [PanelDataAttri]
        public string S1 { get { return _s1; } set { _s1 = value; RaisePropertyChanged("S1"); } }
        // S2 主楼非停车区建筑面积
        private string _s2;
        [PanelDataAttri]
        public string S2 { get { return _s2; } set { _s2 = value; RaisePropertyChanged("S2"); } }
        // S3 大型设备用房面积
        private string _s3;
        [PanelDataAttri]
        public string S3 { get { return _s3; } set { _s3 = value; RaisePropertyChanged("S3"); } }
        // S4 非机动车库面积（不含夹层）
        private string _s4;
        [PanelDataAttri]
        public string S4 { get { return _s4; } set { _s4 = value; RaisePropertyChanged("S4"); } }
        // S5 非机动车库面积（夹层）
        private string _s5;
        [PanelDataAttri]
        public string S5 { get { return _s5; } set { _s5 = value; RaisePropertyChanged("S5"); } }
        // S6 非机动车库面积（夹层）
        private string _s6;
        [PanelDataAttri]
        public string S6 { get { return _s6; } set { _s6 = value; RaisePropertyChanged("S6"); } }
        // S7 住宅公共变电所
        private string _s7;
        [PanelDataAttri]
        public string S7 { get { return _s7; } set { _s7 = value; RaisePropertyChanged("S7"); } }
        // S8 储藏间
        private string _s8;
        [PanelDataAttri]
        public string S8 { get { return _s8; } set { _s8 = value; RaisePropertyChanged("S8"); } }
        // S9 人防分区间
        private string _s9;
        [PanelDataAttri]
        public string S9 { get { return _s9; } set { _s9 = value; RaisePropertyChanged("S9"); } }
        // S10 核心筒
        private double _s10;
        [PanelDataAttri]
        public double S10 { get { return _s10; } set { _s10 = value; RaisePropertyChanged("S10"); } }
        // S11 采光井
        private double _s11;
        [PanelDataAttri]
        public double S11 { get { return _s11; } set { _s11 = value; RaisePropertyChanged("S11"); } }
        // S12 下沉庭院
        private double _s12;
        [PanelDataAttri]
        public double S12 { get { return _s12; } set { _s12 = value; RaisePropertyChanged("S12"); } }
        // S13 单元门厅
        private double _s13;
        [PanelDataAttri]
        public double S13 { get { return _s13; } set { _s13 = value; RaisePropertyChanged("S13"); } }

        // Pe 设备用房面积占比
        private string _pe;
        [PanelDataAttri]
        public string Pe { get { return _pe; } set { _pe = value; RaisePropertyChanged("Pe"); } }

        //图面数据
        // N 平层停车数量
        private string _n;
        [PanelDataAttri]
        public string N { get { return _n; } set { _n = value; RaisePropertyChanged("N"); } }
        // N 机械车位数量
        private string _men;
        [PanelDataAttri]
        public string MeN { get { return _men; } set { _men = value; RaisePropertyChanged("MeN"); } }
        // P 毛车地比
        private string _p;
        [PanelDataAttri]
        public string P { get { return _p; } set { _p = value; RaisePropertyChanged("P"); } }
        // P 含设备及主楼车地比
        private string _p1;
        [PanelDataAttri]
        public string P1 { get { return _p1; } set { _p1 = value; RaisePropertyChanged("P1"); } }
        // P 含设备车地比
        private string _p2;
        [PanelDataAttri]
        public string P2 { get { return _p2; } set { _p2 = value; RaisePropertyChanged("P2"); } }
        // P 仅含主楼车地比
        private string _p3;
        [PanelDataAttri]
        public string P3 { get { return _p3; } set { _p3 = value; RaisePropertyChanged("P3"); } }
        // P 净车地比
        private string _p4;
        [PanelDataAttri]
        public string P4 { get { return _p4; } set { _p4 = value; RaisePropertyChanged("P4"); } }

        //应配数据
        // N 平层停车数量

        // P 毛车地比
        private string _pAllocated;
        [PanelDataAttri]
        public string PAllocated { get { return _pAllocated; } set { _pAllocated = value; RaisePropertyChanged("PAllocated"); } }
        // P 含设备及主楼车地比
        private string _p1Allocated;
        [PanelDataAttri]
        public string P1Allocated { get { return _p1Allocated; } set { _p1Allocated = value; RaisePropertyChanged("P1Allocated"); } }
        // P 含设备车地比
        private string _p2Allocated;
        [PanelDataAttri]
        public string P2Allocated { get { return _p2Allocated; } set { _p2Allocated = value; RaisePropertyChanged("P2Allocated"); } }
        // P 仅含主楼车地比
        private string _p3Allocated;
        [PanelDataAttri]
        public string P3Allocated { get { return _p3Allocated; } set { _p3Allocated = value; RaisePropertyChanged("P3Allocated"); } }
        // P 净车地比
        private string _p4Allocated;
        [PanelDataAttri]
        public string P4Allocated { get { return _p4Allocated; } set { _p4Allocated = value; RaisePropertyChanged("P4Allocated"); } }

        // 坡道计算
        // 缓坡高度
        private string _gentleSlopeHeight;
        [PanelDataAttri]
        public string GentleSlopeHeight { get { return _gentleSlopeHeight; } set { _gentleSlopeHeight = value; RaisePropertyChanged("GentleSlopeHeight"); } }
        // 坡道理论总长
        private string _middleRampLength;
        [PanelDataAttri]
        public string MiddleRampLength { get { return _middleRampLength; } set { _middleRampLength = value; RaisePropertyChanged("MiddleRampLength"); } }
        // 坡道理论总长
        private string _totalRampLength;
        [PanelDataAttri]
        public string TotalRampLength { get { return _totalRampLength; } set { _totalRampLength = value; RaisePropertyChanged("TotalRampLength"); } }
        // 当前长度
        private string _currentLength;
        [PanelDataAttri]
        public string CurrentLength { get { return _currentLength; } set { _currentLength = value; RaisePropertyChanged("CurrentLength"); } }
        // 还需长度
        private string _needLength;
        [PanelDataAttri]
        public string NeedLength { get { return _needLength; } set { _needLength = value; RaisePropertyChanged("NeedLength"); } }
        // 坡道底部不可停车长度
        private string _noStopLengthBottomOfRamp;
        [PanelDataAttri]
        public string NoStopLengthBottomOfRamp { get { return _noStopLengthBottomOfRamp; } set { _noStopLengthBottomOfRamp = value; RaisePropertyChanged("NoStopLengthBottomOfRamp"); } }

        // 其它面板数据
        private bool _IsItOrdinary = true;// 是否启动普通停车位
        [PanelDataAttri]
        public bool IsItOrdinary { get { return _IsItOrdinary; } set { _IsItOrdinary = value; RaisePropertyChanged("IsItOrdinary"); } }
        //
        private bool _IsItMiNi = false;// 是否启动迷你停车位
        [PanelDataAttri]
        public bool IsItMiNi { get { return _IsItMiNi; } set { _IsItMiNi = value; RaisePropertyChanged("IsItMiNi"); } }
        //
        private bool _IsItHor = true;// 是否启动平行停车位
        [PanelDataAttri]
        public bool IsItHor { get { return _IsItHor; } set { _IsItHor = value; RaisePropertyChanged("IsItHor"); } }
        //


        #endregion

    }
}
