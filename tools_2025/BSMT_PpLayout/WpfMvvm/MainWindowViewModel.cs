using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WpfMvvm.Infrastruct;

namespace WpfMvvm
{
    public class MainWindowViewModel : ObservableObject
    {


        //单个属性
        private string strValue;

        public string StrValue
        {
            get { return strValue; }
            set
            {
                strValue = value;
                RaisePropertyChanged("StrValue");
            }
        }
        //多个属性
        public ObservableCollection<string> CbItemsSource { get; set; }

        private string _cbSelectedItem;

        public string CbSelectedItem
        {
            get { return _cbSelectedItem; }
            set
            {
                _cbSelectedItem = value;
                RaisePropertyChanged("CbSelectedItem");
            }
        }

        public MainWindowViewModel()
        {
            CbItemsSource = new ObservableCollection<string>()
            {
                 "红", "黄", "蓝"
            };

            BtnExecCommand = new RelayCommand(() =>
            {
                MessageBox.Show(CbSelectedItem);

            });
        }

        //命令

        public RelayCommand BtnExecCommand { get; set; }

    }
}
