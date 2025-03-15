using PubFuncWt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MountSiteDesignAnaly
{
    internal class GreenRateDataStatistics
    {

        internal List<FloorInfo> coverFloorInfos { get; set; }
        public GreenRateDataStatistics(List<FloorInfo> _coverFloorInfos)
        {
            this.coverFloorInfos = _coverFloorInfos;

        }

        Dictionary<double, SingleGreenStatistics> CellDatas { get; set; }

        internal void Statistics()
        {
            this.CellDatas = new Dictionary<double, SingleGreenStatistics>();

            foreach (var floorInfo in coverFloorInfos)
            {
                foreach (var singleGreenInfor in floorInfo.SingleGreenInfors)
                {
                    if (this.CellDatas.ContainsKey(singleGreenInfor.Id))
                    {
                        this.CellDatas[singleGreenInfor.Id].Area += singleGreenInfor.EffectiveArea;
                    }
                    else
                    {
                        SingleGreenStatistics cellData = new SingleGreenStatistics();
                        cellData.Id = singleGreenInfor.Id;
                        cellData.GreenProperity = ViewModel.Instance.CoverThickGreenFactors[cellData.Id].GreenProperity;
                        cellData.CoverThicknessTop = ViewModel.Instance.CoverThickGreenFactors[cellData.Id].CoverThicknessTop;
                        cellData.CoverThicknessBottom = ViewModel.Instance.CoverThickGreenFactors[cellData.Id].CoverThicknessBottom;
                        cellData.GreenFactor = ViewModel.Instance.CoverThickGreenFactors[cellData.Id].GreenFactor;

                        cellData.Area = singleGreenInfor.EffectiveArea;

                        this.CellDatas[singleGreenInfor.Id] = cellData;
                    }
                }
            }
        }
        internal void ToCSV()
        {
            // 输出表格
            Variable_.CsvInfo = new List<object[]>() { new object[] { DateTime.Now } };
            Variable_.CsvInfo.Add(new object[] { "-", "ID", "绿地性质", "覆土厚度-上限(m)", "覆土厚度-下限(m)", "有效系数", "面积(㎡)" });

            foreach (var item in this.CellDatas.OrderBy(p => p.Key))
            {
                Variable_.CsvInfo.Add(
                    new object[]
                    { 
                        "-", 
                        item.Value.Id, 
                        item.Value.GreenProperity,
                        item.Value.CoverThicknessTop, 
                        item.Value.CoverThicknessBottom, 
                        item.Value.GreenFactor,
                        item.Value.Area.SQUARE_FEETtoSQUARE_METERS()});

            }

            Variable_.CsvInfo.Add(new object[] { DateTime.Now });
        }
    }

    internal class SingleGreenStatistics : CoverThickGreenFactor
    {
        internal double Area { get; set; }

    }

}
