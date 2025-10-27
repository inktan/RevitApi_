using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Collections.ObjectModel;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System.Windows;
using Autodesk.Revit.UI;

namespace DimensioningTools
{
    public class ViewModel : ViewModelBase
    {
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

        private ExternalEvent m_ExEvent;
        private MainWindowRequestHandler m_Handler;
        internal ViewModel()
        {
            this.m_Handler = new MainWindowRequestHandler();
            this.m_ExEvent = ExternalEvent.Create(m_Handler);
        }

        #region 声明变量

        // ==> MVVM

        private double textBox_levelHeight = 3.0;
        public double TextBox_levelHeight { get { return textBox_levelHeight; } set { textBox_levelHeight = value; RaisePropertyChanged("TextBox_levelHeight"); } }
        private double textBox_startElevation = 0.0;
        public double TextBox_startElevation { get { return textBox_startElevation; } set { textBox_startElevation = value; RaisePropertyChanged("TextBox_startElevation"); } }
        private int textBox_numLevel = 16;
        public int TextBox_numLevel { get { return textBox_numLevel; } set { textBox_numLevel = value; RaisePropertyChanged("TextBox_numLevel"); } }

        // 额外添加的混凝土尺寸图层
        //private string concLayernames = "";// 
        //public string ConcLayernames { get { return concLayernames; } set { concLayernames = value; RaisePropertyChanged("ConcLayernames"); } }
        //private string otherLayernames = "";// 
        //public string OtherLayernames { get { return otherLayernames; } set { otherLayernames = value; RaisePropertyChanged("OtherLayernames"); } }

        // 需要注意 mvvm模式下的binding需要暴露属性或字段为public级别
        // 采用单例模式
        // 选择楼板类型
        ObservableCollection<string> floorTypeNames = new ObservableCollection<string>();
        public ObservableCollection<string> FloorTypeNames
        {
            get { return floorTypeNames; }
            set { floorTypeNames = value; }
        }

        /// <summary>
        /// 是否使用轮廓竖向构件
        /// </summary>
        //private bool useProfileVertical = true;
        //public bool UseProfileVerticals { get { return useProfileVertical; } set { useProfileVertical = value; RaisePropertyChanged("UseProfileVerticals"); } }
        #endregion

        #region 依赖命令
        public bool ConsiderRoof = false;

        //ConvertCurrentView,
        //ConvertMultiViews,
        //ConvertSelected,
        //FrameSelectFakeDims,
        public RelayCommand ConvertCurrentView => new RelayCommand(() =>
        {
            makeRequest(RequestId.ConvertCurrentView);
        });
        public RelayCommand ConvertMultiViews => new RelayCommand(() =>
        {
            makeRequest(RequestId.ConvertMultiViews);
        });
        public RelayCommand ConvertSelected => new RelayCommand(() =>
        {
            makeRequest(RequestId.ConvertSelected);
        });
        public RelayCommand FrameSelectFakeDims => new RelayCommand(() =>
        {
            makeRequest(RequestId.FrameSelectFakeDims);
        });
        public RelayCommand DimClosestPointInView => new RelayCommand(() =>
        {
            makeRequest(RequestId.DimClosestPointInView);
        });

        public RelayCommand DimAvoid => new RelayCommand(() =>
        {
            makeRequest(RequestId.DimAvoid);
        });
        public RelayCommand FakeDimAvoid => new RelayCommand(() =>
        {
            makeRequest(RequestId.FakeDimAvoid);
        });
        public RelayCommand WindowSpotElevDim => new RelayCommand(() =>
        {
            makeRequest(RequestId.WindowSpotElevDim);
        });
        public RelayCommand PickElevationFamily => new RelayCommand(() =>
        {
            makeRequest(RequestId.PickElevationFamily);
        });
        public RelayCommand AutoFillUp => new RelayCommand(() =>
        {
            makeRequest(RequestId.AutoFillUp);
        });

        public RelayCommand FakeElev_Refresh_allviews => new RelayCommand(() =>
        {
            makeRequest(RequestId.FakeElev_Refresh_allviews);
        });
        public RelayCommand FakeElev_Refresh_activeview => new RelayCommand(() =>
        {
            makeRequest(RequestId.FakeElev_Refresh_activeview);
        });
        public RelayCommand DimOnFloors => new RelayCommand(() =>
        {
            makeRequest(RequestId.DimOnFloors);
        });
        public RelayCommand AbsElev => new RelayCommand(() =>
        {
            makeRequest(RequestId.AbsElev);
        });

        public RelayCommand TestCmd => new RelayCommand(() => { makeRequest(RequestId.Test); }, CanExcute);
        // 第二参数为bool Func，决定是否启动绑定的按钮
        private bool CanExcute()
        {
            return true;
        }

        #endregion

        #region 外部事件
        internal void makeRequest(RequestId request)
        {
            SetData();

            m_Handler.Request.Make(request);
            m_ExEvent.Raise();
        }

        private void SetData()
        {
            //throw new NotImplementedException();
        }

        #endregion

    }
}
