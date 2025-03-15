using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Selection;
using System.Diagnostics;
using PubFuncWt;
using System;

namespace BSMT_PpLayout
{
    abstract class RequestMethod
    {
        internal UIApplication uiApp { get; }
        internal UIDocument uiDoc => this.uiApp.ActiveUIDocument;
        internal Document doc => this.uiDoc.Document;
        internal Selection sel => this.uiDoc.Selection;
        internal View view => this.uiDoc.ActiveView;

        // 辅助信息
        internal Stopwatch sw;// 用于计时


        public RequestMethod(UIApplication _uiApp)
        {
            this.uiApp = _uiApp;
            this.sw = new Stopwatch();
        }

        internal abstract void Execute();

        internal void JudgeWheInputDataIsWrong()
        {
            if (MainWindow.Instance.autoGeneral_CheckBox.IsChecked == true)
            {
                if (GlobalData.Instance.pSHeight_num < 100.0.MilliMeterToFeet() || GlobalData.Instance.pSWidth_num < 100.0.MilliMeterToFeet())
                {
                    throw new NotImplementedException("垂直式-普通车位尺寸异常");
                }
            }
            if (MainWindow.Instance.autoMini_CheckBox.IsChecked == true)// 是否启动
            {
                if (GlobalData.Instance.miniPSHeight_num < 100.0.MilliMeterToFeet() || GlobalData.Instance.miniPSWidth_num < 100.0.MilliMeterToFeet())
                {
                    throw new NotImplementedException("垂直式-微型车位尺寸异常");
                }
            }
            if (MainWindow.Instance.autoGeneralHor_CheckBox.IsChecked == true)// 是否启动
            {
                if (GlobalData.Instance.pSHeight_Hor_num < 100.0.MilliMeterToFeet() || GlobalData.Instance.pSWidth_Hor_num < 100.0.MilliMeterToFeet())
                {
                    throw new NotImplementedException("水平式-普通车位尺寸异常"); 
                }
            }
        }
    }
}
