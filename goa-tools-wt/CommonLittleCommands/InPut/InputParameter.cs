using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Collections.ObjectModel;
using Autodesk.Revit.DB;

namespace CommonLittleCommands
{
    public class InputParameter : ObservableObject
    {
        // 需要注意 mvvm模式下的binding需要暴露属性或字段为public级别
        // 采用单例模式
        private static InputParameter _instance;
        /// <summary>
        /// 通过属性实现单例模式 
        /// </summary>
        public static InputParameter Instance
        {
            get
            {
                if (ReferenceEquals(_instance, null))
                {
                    _instance = new InputParameter();
                }
                return _instance;
            }
        }

        #region 声明全局变量

        public int intTransparency = 100;

        #endregion

        #region MVVM
        public ObservableCollection<string> tempStrings = new ObservableCollection<string>() { "计算结果如下：" };// Mvvm数据显示


        // 单个属性
        private string _strBackgroundMonitorDta;// 即时计算指标显示
        public string strBackgroundMonitorDta { get { return _strBackgroundMonitorDta; } set { _strBackgroundMonitorDta = value; RaisePropertyChanged("strBackgroundMonitorDta"); } }
        #endregion

    }
}
