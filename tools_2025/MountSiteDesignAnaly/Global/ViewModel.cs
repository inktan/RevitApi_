using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Collections.ObjectModel;
using Autodesk.Revit.DB;
using goa.Common;
using g3;
using PubFuncWt;

using GalaSoft.MvvmLight;

namespace MountSiteDesignAnaly
{
    public class ViewModel : ViewModelBase
    {

        // 需要注意 mvvm模式下的binding需要暴露属性或字段为public级别
        // 采用单例模式
        private static ViewModel _instance;
        /// <summary>
        /// 通过属性实现单例模式 
        /// </summary>
        public static ViewModel Instance
        {
            get
            {
                if (ReferenceEquals(_instance, null))
                {
                    _instance = new ViewModel();
                }
                return _instance;
            }
        }
        internal ViewModel()
        { }
        #region 声明全局变量

        // ==> MVVM
        private bool _CalGM = true;// 计算常规模型
        public bool CalGM { get { return _CalGM; } set { _CalGM = value; RaisePropertyChanged("CalGM"); } }
        // ==> MVVM
        private bool _CalFloor = true;// 计算楼板
        public bool CalFloor { get { return _CalFloor; } set { _CalFloor = value; RaisePropertyChanged("CalFloor"); } }
        // ==> MVVM
        private bool _CalPad = true;// 计算建筑地坪
        public bool CalPad { get { return _CalPad; } set { _CalPad = value; RaisePropertyChanged("CalPad"); } }
        // ==> 挡土墙距离
        private double retainingWallDis = 0.0;
        public double RetainingWallDis { get { return retainingWallDis; } set { retainingWallDis = value; RaisePropertyChanged("RetainingWallDis"); } }

        // ==> 采样精度
        private double samplingAccuracy = 1000.0;
        public double SamplingAccuracy { get { return samplingAccuracy; } set { samplingAccuracy = value; RaisePropertyChanged("SamplingAccuracy"); } }
        // ==> 底板厚度
        private double bottomPlateThickness = 0.0;
        public double BottomPlateThickness { get { return bottomPlateThickness; } set { bottomPlateThickness = value; RaisePropertyChanged("BottomPlateThickness"); } }

        // ==> 顶板厚度
        private double topPlateThickness = 200.0;
        public double TopPlateThickness { get { return topPlateThickness; } set { topPlateThickness = value; RaisePropertyChanged("TopPlateThickness"); } }

        // ==> 垫层厚度
        private double cushionThickness = 0.0;
        public double CushionThickness { get { return cushionThickness; } set { cushionThickness = value; RaisePropertyChanged("CushionThickness"); } }

        // ==> 净高要求
        private double minimumClearHeight = 3900.0;
        public double MinimumClearHeight { get { return minimumClearHeight; } set { minimumClearHeight = value; RaisePropertyChanged("MinimumClearHeight"); } }

        // ==> MVVM
        private bool _ShowSamplingLine = false;// 
        public bool ShowSamplingLine { get { return _ShowSamplingLine; } set { _ShowSamplingLine = value; RaisePropertyChanged("ShowSamplingLine"); } }
        // ==> MVVM
        private bool _ShowSamplingTris = false;// 
        public bool ShowSamplingTris { get { return _ShowSamplingTris; } set { _ShowSamplingTris = value; RaisePropertyChanged("ShowSamplingTris"); } }

        // ==> MVVM
        //private bool _CreatFloor = false;// 创建楼板
        //public bool CreatFloor { get { return _CreatFloor; } set { _CreatFloor = value; RaisePropertyChanged("CreatFloor"); } }


        /// <summary>
        /// 覆土厚度列表 == 绿化系数列表
        /// </summary>
        ObservableCollection<CoverThickGreenFactor> coverThickGreenFactors = new ObservableCollection<CoverThickGreenFactor>();
        public ObservableCollection<CoverThickGreenFactor> CoverThickGreenFactors
        {
            get { return coverThickGreenFactors; }
            set { coverThickGreenFactors = value; }
        }


        /// <summary>
        /// 是否计算绿地率
        /// </summary>
        public bool CalCoverGreen { get; internal set; }

        #endregion

    }

}
