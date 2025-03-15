using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MountSiteDesignAnaly
{
    internal class GreenRateDatabase
    {

        /// <summary>
        /// 输入覆土绿地计算数据
        /// </summary>
        internal void CreatDatabase()
        {
            ViewModel.Instance.CoverThickGreenFactors.Clear();

            ViewModel.Instance.CoverThickGreenFactors.Add(new CoverThickGreenFactor() { Id = 0, GreenProperity = "覆土厚度", CoverThicknessTop = 9999, CoverThicknessBottom = 1.5, GreenFactor = 1.00 });
            ViewModel.Instance.CoverThickGreenFactors.Add(new CoverThickGreenFactor() { Id = 1, GreenProperity = "覆土厚度", CoverThicknessTop = 1.5, CoverThicknessBottom = 1.0, GreenFactor = 0.8 });
            ViewModel.Instance.CoverThickGreenFactors.Add(new CoverThickGreenFactor() { Id = 2, GreenProperity = "覆土厚度", CoverThicknessTop = 1.0, CoverThicknessBottom = 0.5, GreenFactor = 0.5 });
            ViewModel.Instance.CoverThickGreenFactors.Add(new CoverThickGreenFactor() { Id = 3, GreenProperity = "覆土厚度", CoverThicknessTop = 0.5, CoverThicknessBottom = 0.3, GreenFactor = 0.3 });
            ViewModel.Instance.CoverThickGreenFactors.Add(new CoverThickGreenFactor() { Id = 4, GreenProperity = "覆土厚度", CoverThicknessTop = 0.3, CoverThicknessBottom = 0.1, GreenFactor = 0.1 });
            ViewModel.Instance.CoverThickGreenFactors.Add(new CoverThickGreenFactor() { Id = 5, GreenProperity = "覆土厚度", CoverThicknessTop = 0.1, CoverThicknessBottom = 0, GreenFactor = 0 });

            // 数据加载失败
            GreenRateCalBasis greenRateCalBasis = new GreenRateCalBasis();
            greenRateCalBasis.ShowDialog();

        }
    }
    public class CoverThickGreenFactor : ViewModelBase
    {
        // ID
        [Description("ID")]
        public int Id { get; set; }
        // 绿地性质
        [Description("绿地性质")]
        public string GreenProperity { get; set; }
        // 覆土厚度(m)
        [Description("覆土厚度-上限(m)")]
        public double CoverThicknessTop { get; set; }
        [Description("覆土厚度-下限(m)")]
        public double CoverThicknessBottom { get; set; }
        // 有效系数
        [Description("有效系数")]
        public double GreenFactor { get; set; }

    }
}
