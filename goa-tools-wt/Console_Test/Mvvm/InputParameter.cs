using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Collections.ObjectModel;


namespace WpfMvvm.Infrastruct
{
    public class InputParameter : ObservableObject
    {
        #region 单例模式固定写法，勿改

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
        #endregion

        #region 字段即时更新

        private string _strCalculationResult;// 即时计算指标显示
        public string strCalculationResult { get { return _strCalculationResult; } set { _strCalculationResult = value; RaisePropertyChanged("strCalculationResult"); } }

        #endregion

    }
}
