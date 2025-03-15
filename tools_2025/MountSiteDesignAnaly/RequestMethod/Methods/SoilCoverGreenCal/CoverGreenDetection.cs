using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PubFuncWt;
using goa.Common;
using g3;
using goa.Revit.DirectContext3D;
using System.ComponentModel;

namespace MountSiteDesignAnaly
{
    internal class CoverGreenDetection : RequestMethod
    {
        internal CoverGreenDetection(UIApplication _uiApp) : base(_uiApp)
        {
        }

        internal override void Execute()
        {
            GeometryDrawServersMgr.ClearAllServers();

            // 提取覆土楼板

            List<Floor> coverFloors = new FilteredElementCollector(this.doc)
                   .OfCategory(BuiltInCategory.OST_Floors)
                   .WhereElementIsNotElementType()
                   .Cast<Floor>()
                   .Where(p => p.Name.Contains("覆土"))
                   .ToList();

            List<FloorInfo> coverFloorInfos = coverFloors.Select(p => new FloorInfo(p)).ToList();

            // 提取地下室顶板

            List<Floor> bsmtTopFloors = new FilteredElementCollector(this.doc)
                   .OfCategory(BuiltInCategory.OST_Floors)
                   .WhereElementIsNotElementType()
                   .Cast<Floor>()
                   .Where(p => p.Name.Contains("地下室顶板"))
                   .ToList();

            List<FloorInfo> bsmtTopFloorInfos = bsmtTopFloors.Select(p => new FloorInfo(p)).ToList();

            // 获取绿地退界区域

            List<FilledRegion> filledRegions = new FilteredElementCollector(this.doc, this.view.Id)
                .OfClass(typeof(FilledRegion))
                .Cast<FilledRegion>()
                .ToList();

            List<FilledRegionInfo> filledRegionInfos = filledRegions.Select(p => new FilledRegionInfo(p)).ToList();

            foreach (var item in coverFloorInfos)
            {
                item.CalGreenInfosByBsmtFloor(bsmtTopFloorInfos);
                item.CalGreenInfosByRetreatFilledRegion(filledRegionInfos);

            }

            DisplayDomAndWpf workOverlayLegendGrids = new DisplayDomAndWpf(coverFloorInfos.SelectMany(p => p.SingleGreenInfors).ToList());
            workOverlayLegendGrids.Excute();

            GreenRateDatabase greenRateDatabase = new GreenRateDatabase();
            greenRateDatabase.CreatDatabase();

            // 按照类型创建楼板
            foreach (var floorInfo in coverFloorInfos)
            {
                foreach (var singleGreenInfor in floorInfo.SingleGreenInfors)
                {
                    singleGreenInfor.AssignEffectiveGreenValue();
                }
            }

            GreenRateDataStatistics greenRateDataStatistics = new GreenRateDataStatistics(coverFloorInfos);
            greenRateDataStatistics.Statistics();
            greenRateDataStatistics.ToCSV();

            // 输出测算表格
            this.doc.SaveCsv(Variable_.CsvInfo, "覆土绿化计算");

            if (ViewModel.Instance.CalCoverGreen)
            {
                // 按照类型创建楼板
                CreatFloorType();
                foreach (var floorInfo in coverFloorInfos)
                {
                    foreach (var singleGreenInfor in floorInfo.SingleGreenInfors)
                    {
                        singleGreenInfor.CreatFloor(greenFloorTypes);
                    }
                }
            }
        }

        List<FloorType> greenFloorTypes = new List<FloorType>();
        /// <summary>
        /// 查看可计算的楼板类型是否存在，不存在则创建
        /// </summary>
        internal void CreatFloorType()
        {
            List<FloorType> floorTypes = (new FilteredElementCollector(this.doc)).OfCategory(BuiltInCategory.OST_Floors).OfClass(typeof(FloorType)).Cast<FloorType>().ToList();

            FloorType floorType = floorTypes.Where(p => p.Name.Contains("覆土楼板")).FirstOrDefault();
            if (floorType == null)
            {
                throw new NotImplementedException("当前文档不存在-覆土楼板-楼板类型，请检查。");
            }

            using (Transaction tarns = new Transaction(this.doc, "创建绿化系数楼板类型"))
            {
                tarns.DeleteErrOrWaringTaskDialog();

                tarns.Start();
                foreach (var item in ViewModel.Instance.CoverThickGreenFactors)
                {
                    FloorType tmpFloorType = floorTypes.Where(p => p.Name.Contains("绿化系数" + item.GreenFactor.ToString())).FirstOrDefault();
                    if (tmpFloorType == null)
                    {
                        tmpFloorType = floorType.Duplicate("覆土楼板 绿化系数" + item.GreenFactor.ToString()) as FloorType;
                    }
                    greenFloorTypes.Add(tmpFloorType);
                }
                tarns.Commit();
            }
        }
    }
}
