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

namespace InfoStrucFormwork
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

        private double beamWidth = 300.0;
        public double BeamWidth { get { return beamWidth; } set { beamWidth = value; RaisePropertyChanged("BeamWidth"); } }

        private double beamHeight = 600.0;
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

        public RelayCommand StoreySteelBeamDoubleLine => new RelayCommand(() =>
        {
            makeRequest(RequestId.StoreySteelBeamDoubleLine);
        });
        public RelayCommand RoofSteelBeamDoubleLine => new RelayCommand(() =>
        {
            makeRequest(RequestId.RoofSteelBeamDoubleLine);
        });
        public RelayCommand StoreySteelBeamSingleLine => new RelayCommand(() =>
        {
            makeRequest(RequestId.StoreySteelBeamSingleLine);
        });
        public RelayCommand RoofStelBeamSingleLine => new RelayCommand(() =>
        {
            makeRequest(RequestId.RoofStelBeamSingleLine);
        });

        public RelayCommand BandingWhole => new RelayCommand(() =>
        {
            makeRequest(RequestId.BandingWhole);
        });
        public RelayCommand BandingSel => new RelayCommand(() =>
        {
            makeRequest(RequestId.BandingSel);
        });
        public RelayCommand BandingCombination => new RelayCommand(() =>
        {
            makeRequest(RequestId.BandingCombination);
        });
        public RelayCommand StoreyFloor => new RelayCommand(() =>
        {
            makeRequest(RequestId.StoreyFloor);
        });
        public RelayCommand ConcWall => new RelayCommand(() =>
        {
            makeRequest(RequestId.ConcWall);
        });
        public RelayCommand ConcCol => new RelayCommand(() =>
        {
            makeRequest(RequestId.ConcCol);
        });
        public RelayCommand ConcBeamUiSingle => new RelayCommand(() =>
        {
            makeRequest(RequestId.ConcBeamUiSingle);
        });
        public RelayCommand ConcBeamUiMultiline => new RelayCommand(() =>
        {
            makeRequest(RequestId.ConcBeamUiMultiline);
        });
        public RelayCommand ConcBeamFollowFaceUi => new RelayCommand(() =>
        {
            makeRequest(RequestId.ConcBeamFollowFaceUi);
        });
        public RelayCommand ConcSingleBeamFollowFaceUi => new RelayCommand(() =>
        {
            makeRequest(RequestId.ConcSingleBeamFollowFaceUi);
        });
        public RelayCommand ConcSingleColFollowFaceUi => new RelayCommand(() =>
        {
            makeRequest(RequestId.ConcSingleColFollowFaceUi);
        });
        public RelayCommand ConSingleBeamFollowEdge => new RelayCommand(() =>
        {
            makeRequest(RequestId.ConSingleBeamFollowEdge);
        });
        public RelayCommand ConSingleColFollowEdge => new RelayCommand(() =>
        {
            makeRequest(RequestId.ConSingleColFollowEdge);
        });
        public RelayCommand ConcColFollowFaceUi => new RelayCommand(() =>
        {
            makeRequest(RequestId.ConcColFollowFaceUi);
        });
        public RelayCommand SlopeRoofBeam => new RelayCommand(() =>
        {
            makeRequest(RequestId.SlopeRoofBeam);
        });
        public RelayCommand FloorDivision => new RelayCommand(() =>
        {
            makeRequest(RequestId.FloorDivision);
        });
        public RelayCommand AlignToBoardTop => new RelayCommand(() =>
        {
            makeRequest(RequestId.AlignToBoardTop);
        });
        public RelayCommand AlignToBoardBottom => new RelayCommand(() =>
        {
            makeRequest(RequestId.AlignToBoardBottom);
        });
        public RelayCommand BbCoverFb => new RelayCommand(() =>
        {
            makeRequest(RequestId.BbCoverFb);
        });
        public RelayCommand CalRelativeH => new RelayCommand(() =>
        {
            makeRequest(RequestId.CalRelativeH);
        });
        public RelayCommand ClearStruAna => new RelayCommand(() =>
        {
            makeRequest(RequestId.ClearStruAna);
        });
        public RelayCommand EleSeparate => new RelayCommand(() =>
        {
            makeRequest(RequestId.EleSeparate);
        });
        public RelayCommand AllowJoin => new RelayCommand(() =>
        {
            makeRequest(RequestId.AllowJoin);
        });
        public RelayCommand DisallowJoin => new RelayCommand(() =>
        {
            makeRequest(RequestId.DisallowJoin);
        });
        public RelayCommand DelDupCols => new RelayCommand(() =>
        {
            makeRequest(RequestId.DelDupCols);
        });
        public RelayCommand DelDupBeams => new RelayCommand(() =>
        {
            makeRequest(RequestId.DelDupBeams);
        });
        public RelayCommand BrokenNumRepair => new RelayCommand(() =>
        {
            makeRequest(RequestId.BrokenNumRepair);
        });
        public RelayCommand TestCmd01 => new RelayCommand(() => { makeRequest(RequestId.Test01); }, CanExcute);
        public RelayCommand TestCmd02 => new RelayCommand(() => { makeRequest(RequestId.Test02); }, CanExcute);
        public RelayCommand TestCmd03 => new RelayCommand(() => { makeRequest(RequestId.Test03); }, CanExcute);
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
