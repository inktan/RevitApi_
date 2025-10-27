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

namespace CollisionDetection
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

        private double beamWidth = 0.0;
        public double BeamWidth { get { return beamWidth; } set { beamWidth = value; RaisePropertyChanged("BeamWidth"); } }

        private double beamHeight = 0.0;
        public double BeamHeight { get { return beamHeight; } set { beamHeight = value; RaisePropertyChanged("BeamHeight"); } }

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
        public RelayCommand IncludingLinkedModels => new RelayCommand(() =>
        {
            makeRequest(RequestId.IncludingLinkedModels);
        });
        public RelayCommand ExcludingLinkedModels => new RelayCommand(() =>
        {
            makeRequest(RequestId.ExcludingLinkedModels);
        });
        public RelayCommand FrameSel_LinkedStruModels => new RelayCommand(() =>
        {
            makeRequest(RequestId.FrameSel_LinkedStruModels);
        });

        public RelayCommand IncludingLinkedModelsAll => new RelayCommand(() =>
        {
            makeRequest(RequestId.IncludingLinkedModelsAll);
        });
        public RelayCommand ExcludingLinkedModelsAll => new RelayCommand(() =>
        {
            makeRequest(RequestId.ExcludingLinkedModelsAll);
        });
        public RelayCommand AllSel_LinkedStruModels => new RelayCommand(() =>
        {
            makeRequest(RequestId.AllSel_LinkedStruModels);
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
