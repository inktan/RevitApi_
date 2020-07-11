using System;
using System.Collections.Generic;
using Form_ = System.Windows.Forms;
using System.Linq;

using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB.Events;
using Autodesk.Revit.DB.IFC;

using goa.Common;
using ClipperLib;
using wt_Common;

namespace LayoutParkingEffcient
{
    using cInt = Int64;
    using Path = List<IntPoint>;
    using Paths = List<List<IntPoint>>;
    using Path_xyz = List<XYZ>;
    using Paths_xyz = List<List<XYZ>>;

    class TransactionNow
    {
        public Document doc { get; set; }
        public View acvtiView { get; set; }
        /// <summary>
        /// 构造函数
        /// </summary>
        public TransactionNow(Document _doc)
        {
            doc = _doc;
            acvtiView = _doc.ActiveView;
        }

        #region 开启放置车位事务
        /// <summary>
        /// 排布车位
        /// </summary>
        public void LayoutParking(List<Point> tar_placeXYZs, List<Point> tar_columnplaceXYZs, double rotateAngle, string layoutMethod)//全部转移到clipper中进行处理
        {
            FamilySymbol parkingType = null;
            FamilySymbol columnType = null;

            string parkingTypeName = "停车位_详图线";//设定目标停车位族类型名字
            string parkingFamilyFilePath = @"W:\BIM_ARCH\03.插件\地库强排\族\停车位_详图线.rfa";//停车位族文件所在位置

            string columnTypeName = "柱子_详图线";//设定目标停车位族类型名字
            string columnFamilyFilePath = @"W:\BIM_ARCH\03.插件\地库强排\族\柱子_详图线.rfa";//停车位族文件所在位置

            parkingType = _FindFamilySymbolNeeded(parkingFamilyFilePath, parkingTypeName);
            columnType = _FindFamilySymbolNeeded(columnFamilyFilePath, columnTypeName);

            #region 开展事务 调整停车位大小 按照区域内的点，将车位族布置进去
            modifyParking_H_W(parkingType, CMD.parkingPlaceHeight, CMD.parkingPlaceWight);//调整车位的尺寸
            modifyParking_H_W(columnType, CMD.columnWidth, CMD.columnWidth);//调整车位的尺寸

            List<FamilyInstance> southParkingInstances = new List<FamilyInstance>();
            List<FamilyInstance> northParkingInstances = new List<FamilyInstance>();
            List<FamilyInstance> columnplaceInstances = new List<FamilyInstance>();
            if (layoutMethod == "水平")
            {
                // 车头在南侧
                List<Point> southParking = tar_placeXYZs.Where(p => p.GraphicsStyleId == new ElementId(-122543112)).ToList();
                southParkingInstances = PlaceParkingPlaces_2D(parkingType, southParking, acvtiView);//开启事务 放置车位
                RotateFamilyInstances(southParkingInstances, Math.PI);//对所有车位族实例进行旋转
                RotateFamilyInstances(southParkingInstances, rotateAngle);//对所有车位族实例进行旋转

                //车头在北侧
                List<Point> northParking = tar_placeXYZs.Where(p => p.GraphicsStyleId == new ElementId(-21136)).ToList();
                northParkingInstances = PlaceParkingPlaces_2D(parkingType, northParking, acvtiView);//开启事务 放置车位
                RotateFamilyInstances(northParkingInstances, rotateAngle);//对所有车位族实例进行旋转

                //柱子，不需要特殊处理
                //columnplaceInstances = PlaceParkingPlaces_2D(columnType, tar_columnplaceXYZs, acvtiView);//开启事务 放置车位

            }
            else if (layoutMethod == "垂直")
            {
                // 车头在南侧
                List<Point> southParking = tar_placeXYZs.Where(p => p.GraphicsStyleId == new ElementId(-122543112)).ToList();
                southParkingInstances = PlaceParkingPlaces_2D(parkingType, southParking, acvtiView);//开启事务 放置车位
                RotateFamilyInstances(southParkingInstances, Math.PI / 2);//对所有车位族实例进行旋转
                RotateFamilyInstances(southParkingInstances, Math.PI);//对所有车位族实例进行旋转
                RotateFamilyInstances(southParkingInstances, rotateAngle);//对所有车位族实例进行旋转

                //车头在北侧
                List<Point> northParking = tar_placeXYZs.Where(p => p.GraphicsStyleId == new ElementId(-21136)).ToList();
                northParkingInstances = PlaceParkingPlaces_2D(parkingType, northParking, acvtiView);//开启事务 放置车位
                RotateFamilyInstances(northParkingInstances, Math.PI / 2);//对所有车位族实例进行旋转
                RotateFamilyInstances(northParkingInstances, rotateAngle);//对所有车位族实例进行旋转

                //柱子，不需要特殊处理
                //columnplaceInstances = PlaceParkingPlaces_2D(columnType, tar_columnplaceXYZs, acvtiView);//开启事务 放置车位

            }
            else
            {
                TaskDialog.Show("error", "未选择停车方式");
            }
            #endregion
        }
        /// <summary>
        /// 排布车位
        /// </summary>
        public void LayoutParking(List<Point> tar_placeXYZs, List<Point> tar_columnplaceXYZs, string layoutMethod)//全部转移到clipper中进行处理
        {
            FamilySymbol parkingType = null;
            FamilySymbol columnType = null;

            string parkingTypeName = "停车位_详图线";//设定目标停车位族类型名字
            string parkingFamilyFilePath = @"W:\BIM_ARCH\03.插件\地库强排\族\停车位_详图线.rfa";//停车位族文件所在位置

            string columnTypeName = "柱子_详图线";//设定目标停车位族类型名字
            string columnFamilyFilePath = @"W:\BIM_ARCH\03.插件\地库强排\族\柱子_详图线.rfa";//停车位族文件所在位置

            parkingType = _FindFamilySymbolNeeded(parkingFamilyFilePath, parkingTypeName);
            columnType = _FindFamilySymbolNeeded(columnFamilyFilePath, columnTypeName);

            #region 开展事务 调整停车位大小 按照区域内的点，将车位族布置进去
            modifyParking_H_W(parkingType, CMD.parkingPlaceHeight, CMD.parkingPlaceWight);//调整车位的尺寸
            modifyParking_H_W(columnType, CMD.columnWidth, CMD.columnWidth);//调整车位的尺寸

            List<FamilyInstance> southParkingInstances = new List<FamilyInstance>();
            List<FamilyInstance> northParkingInstances = new List<FamilyInstance>();
            List<FamilyInstance> columnplaceInstances = new List<FamilyInstance>();
            if (layoutMethod == "水平")
            {
                // 车头在南侧
                List<Point> southParking = tar_placeXYZs.Where(p => p.GraphicsStyleId == new ElementId(-122543112)).ToList();
                southParkingInstances = PlaceParkingPlaces_2D(parkingType, southParking, acvtiView);//开启事务 放置车位
                RotateFamilyInstances(southParkingInstances, Math.PI);//对所有车位族实例进行旋转

                //车头在北侧
                List<Point> northParking = tar_placeXYZs.Where(p => p.GraphicsStyleId == new ElementId(-21136)).ToList();
                northParkingInstances = PlaceParkingPlaces_2D(parkingType, northParking, acvtiView);//开启事务 放置车位

                //柱子，不需要特殊处理
                //columnplaceInstances = PlaceParkingPlaces_2D(columnType, tar_columnplaceXYZs, acvtiView);//开启事务 放置车位

            }
            else if (layoutMethod == "垂直")
            {
                // 车头在南侧
                List<Point> southParking = tar_placeXYZs.Where(p => p.GraphicsStyleId == new ElementId(-122543112)).ToList();
                southParkingInstances = PlaceParkingPlaces_2D(parkingType, southParking, acvtiView);//开启事务 放置车位
                RotateFamilyInstances(southParkingInstances, Math.PI / 2);//对所有车位族实例进行旋转
                RotateFamilyInstances(southParkingInstances, Math.PI);//对所有车位族实例进行旋转

                //车头在北侧
                List<Point> northParking = tar_placeXYZs.Where(p => p.GraphicsStyleId == new ElementId(-21136)).ToList();
                northParkingInstances = PlaceParkingPlaces_2D(parkingType, northParking, acvtiView);//开启事务 放置车位
                RotateFamilyInstances(northParkingInstances, Math.PI / 2);//对所有车位族实例进行旋转

                //柱子，不需要特殊处理
                //columnplaceInstances = PlaceParkingPlaces_2D(columnType, tar_columnplaceXYZs, acvtiView);//开启事务 放置车位

            }
            else
            {
                TaskDialog.Show("error", "未选择停车方式");
            }
            #endregion
        }
        /// <summary>
        /// 创建停车位族实例2D
        /// </summary>
        private List<FamilyInstance> PlaceParkingPlaces_2D(FamilySymbol parkingType, List<Point> placeXYZs, View activeview)
        {
            List<FamilyInstance> familyInstances = new List<FamilyInstance>();
            using (Transaction creatNewGroup = new Transaction(doc))
            {
                creatNewGroup.Start("placeParkingPlace");
                if (!parkingType.IsActive)//判断族类型是否被激活
                {
                    parkingType.Activate();
                }
                foreach (Point _point in placeXYZs)
                {
                    FamilyInstance parkingPlace = doc.Create.NewFamilyInstance(_point.Coord, parkingType, activeview);
                    //System.Windows.Forms.Application.DoEvents();
                    familyInstances.Add(parkingPlace);
                }
                creatNewGroup.Commit();
            }
            return familyInstances;
        }

        #endregion

        #region 判断 停车位族 是否存在于 当前文档中，不存在，则载入
        /// <summary>
        /// 通过两层方法(如果当前文档不存在目标name族，则载入族文件)，确定当前文档，存在停车位族
        /// </summary>
        private FamilySymbol _FindFamilySymbolNeeded(string FamilyFilePath, string parkingTypeName)
        {
            FamilySymbol parkingType = null;
            bool symbolFound = FindFamilySymbolNeeded(parkingTypeName, out parkingType);//寻找目标停车位族类型
            if (!symbolFound)
            {
                Family parkFamily = null;
                bool loadFamily = reLoadFamily(FamilyFilePath, out parkFamily);
                ICollection<ElementId> eleIds = parkFamily.GetValidTypes();//目前该函数无效，未查明原因
                if (loadFamily)
                {
                    symbolFound = FindFamilySymbolNeeded(parkingTypeName, out parkingType);
                }
                else
                {
                    throw new NotImplementedException("基础停车位族载入失败。");
                    //TaskDialog.Show("error","基础停车位族载入失败。");
                }
            }
            return parkingType;
        }
        /// <summary>
        /// 通过name进行索引需要的族类型
        /// </summary>
        private bool FindFamilySymbolNeeded(string TargetSymbolName, out FamilySymbol targetFamilySymbal)
        {
            ElementFilter parkingCategoryFilter = new ElementCategoryFilter(BuiltInCategory.OST_DetailComponents);
            //ElementFilter parkingCategoryFilter = new ElementCategoryFilter(BuiltInCategory.OST_GenericAnnotation);
            ElementFilter familySymbolFilter = new ElementClassFilter(typeof(FamilySymbol));
            LogicalAndFilter andFilter = new LogicalAndFilter(parkingCategoryFilter, familySymbolFilter);

            FilteredElementCollector parkingSymbols = new FilteredElementCollector(doc);
            parkingSymbols.WherePasses(andFilter);
            bool symbolFound = false;
            targetFamilySymbal = null;
            foreach (FamilySymbol element in parkingSymbols)
            {
                if (element.Name == TargetSymbolName)
                {
                    symbolFound = true;
                    targetFamilySymbal = element;
                    break;
                }
            }
            return symbolFound;
        }
        #endregion

        #region 调整族 类型参数 ， 以及载入处理
        /// <summary>
        /// 旋转族实例
        /// </summary>
        private void RotateFamilyInstances(List<FamilyInstance> parkingplaceInstances, double _angle)
        {
            using (Transaction _rotateTrans = new Transaction(doc))
            {
                _rotateTrans.Start("_rotateTrans");
                foreach (FamilyInstance fs in parkingplaceInstances)
                {
                    LocationPoint _point = fs.Location as LocationPoint;
                    if (_point != null)
                    {
                        XYZ aa = _point.Point;
                        XYZ cc = new XYZ(aa.X, aa.Y, aa.Z + 10);
                        Line _axis = Line.CreateBound(aa, cc);
                        _point.Rotate(_axis, _angle);
                    }
                }
                _rotateTrans.Commit();
            }
        }
        /// <summary>
        /// 修改停车位尺寸
        /// </summary>
        private void modifyParking_H_W(FamilySymbol parkingType, double Height, double Width)
        {
            using (Transaction modifyParking_H_W = new Transaction(doc))
            {
                modifyParking_H_W.Start("modifyParking_H_W");
                Parameter DWparametericfloor_length = parkingType.LookupParameter("Height");//查找族类型参数
                Parameter DWparametericfloor_width = parkingType.LookupParameter("Width");
                DWparametericfloor_length.Set(Height);//修改族类型参数
                DWparametericfloor_width.Set(Width);
                modifyParking_H_W.Commit();
            }
        }
        /// <summary>
        /// 通过文件路径字符串，将族载入到当前文档
        /// </summary>
        private bool reLoadFamily(string FamilyFilePath, out Family family)
        {
            family = null;
            bool loadSuccess = false;
            using (Transaction loadFamily = new Transaction(doc))
            {
                loadFamily.Start("loadFamily");
                projectFamLoadOption pjflo = new projectFamLoadOption();
                loadSuccess = doc.LoadFamily(FamilyFilePath, pjflo, out family);//经过测试
                if (loadSuccess)
                {
                    foreach (ElementId parkingTypeId in family.GetValidTypes())//该函数无效，获取不出一个family的族类型；
                    {
                        FamilySymbol parkingTypeName = doc.GetElement(parkingTypeId) as FamilySymbol;
                        if (parkingTypeName != null)
                        {
                            //CMD.TestList.Add(parkingTypeName.Name);
                        }
                    }
                }
                loadFamily.Commit();
            }
            return loadSuccess;
        }
        /// <summary>
        /// 载入族提示是否要覆盖族参数
        /// </summary>
        private class projectFamLoadOption : IFamilyLoadOptions
        {
            bool IFamilyLoadOptions.OnFamilyFound(bool familyInUse, out bool overwriteParameterValues)
            {
                overwriteParameterValues = true;
                throw new NotImplementedException();
            }

            bool IFamilyLoadOptions.OnSharedFamilyFound(Family sharedFamily, bool familyInUse, out FamilySource source, out bool overwriteParameterValues)
            {
                source = FamilySource.Project;
                overwriteParameterValues = true;
                throw new NotImplementedException();
            }
        }
        #endregion
    }
}
