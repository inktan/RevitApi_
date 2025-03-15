using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

using BSMT_PpLayout;
using goa.Common.g3InterOp;
using ClipperLib;
using PubFuncWt;
using Autodesk.Revit.DB.Structure;
using goa.Common;

namespace BSMT_PpLayout
{
    public static class Transcation_mehod
    {

        /// <summary>
        /// 柱子
        /// </summary>
        internal static List<FamilyInstance> LayoutColumn(this Document doc, View nowView, List<ColLocPoint> columnLocationPoints)//全部转移到clipper中进行处理
        {
            FamilySymbol columnType = doc.GetFs("柱子_"+ MainWindow.Instance.columnWidth.Text + "*"+MainWindow.Instance.columnWidth.Text);

            Dictionary<FamilyInstance, double> fsRotateAnglePairs = new Dictionary<FamilyInstance, double>();

            List<FamilyInstance> columns = new List<FamilyInstance>();
            using (Transaction creatNewGroup = new Transaction(doc))
            {
                creatNewGroup.Start("placeParkingPlace");
                if (!columnType.IsActive)//判断族类型是否被激活
                {
                    columnType.Activate();
                }

                foreach (ColLocPoint columnLocationPoint in columnLocationPoints)
                {
                    XYZ location = columnLocationPoint.Vector2d.ToXYZ();
                    FamilyInstance parkingPlace = doc.Create.NewFamilyInstance(location, columnType, nowView);
                    fsRotateAnglePairs.Add(parkingPlace, columnLocationPoint.RotateAngle);
                    columns.Add(parkingPlace);
                }
                creatNewGroup.Commit();
            }

            // 旋转

            using (Transaction _rotateTrans = new Transaction(doc))
            {
                _rotateTrans.Start("_rotateTrans");
                foreach (var item in fsRotateAnglePairs)
                {
                    item.Key.Rotate(item.Value);
                }
                _rotateTrans.Commit();
            }
            return columns;
        }
        /// <summary>
        /// 普通停车位
        /// </summary>
        internal static void LayoutParking(this Document doc, View nowView, List<PPLocPoint> tar_placeXYZs, double rotateAngle, FamilySymbol parkingType)//全部转移到clipper中进行处理
        {
            #region 开展事务 调整停车位大小 按照区域内的点，将车位族布置进去
            List<FamilyInstance> parkingInstances = tar_placeXYZs.Select(p => p.Vector2d.ToXYZ()).Place2DFamilyInstancesWithTrans(doc, nowView, parkingType);//开启事务 放置车位
            parkingInstances.RotateWithTrans(doc, rotateAngle);//对所有车位族实例进行旋转
            #endregion
        }

        public static FamilySymbol GetFs(this Document doc, string oriFsName)
        {
            string oriFaPath = App.FamilyFilePath + @"机动车\停车位_.rfa";//停车位族文件所在位置
            string faName = "停车位_";//设定目标停车位族类型名字

            if (oriFsName.Contains("停车位_") && !oriFsName.Contains("停车位_回车_"))
            {
            }
            else if (oriFsName.Contains("停车位_回车_"))
            {
                oriFaPath = App.FamilyFilePath + @"机动车\停车位_回车_.rfa";
                faName = "停车位_回车_";//设定目标停车位族类型名字
            }
            else if (oriFsName.Contains("机械车位_"))
            {
                oriFaPath = App.FamilyFilePath + @"机动车\机械车位_.rfa";
                faName = "机械车位_";//设定目标停车位族类型名字
            }
            else if (oriFsName.Contains("无障碍车位_"))
            {
                oriFaPath = App.FamilyFilePath + @"机动车\无障碍车位_.rfa";
                faName = "无障碍车位_";//设定目标停车位族类型名字
            }
            else if (oriFsName.Contains("子母车位_"))
            {
                oriFaPath = App.FamilyFilePath + @"机动车\子母车位_.rfa";
                faName = "子母车位_";//设定目标停车位族类型名字
            }
            else if (oriFsName.Contains("公共泊车位_"))
            {
                oriFaPath = App.FamilyFilePath + @"机动车\公共泊车位_.rfa";
                faName = "公共泊车位_";//设定目标停车位族类型名字
            }
            else if (oriFsName.Contains("慢充电位_"))
            {
                oriFaPath = App.FamilyFilePath + @"机动车\慢充电位_.rfa";
                faName = "慢充电位_";//设定目标停车位族类型名字
            }
            else if (oriFsName.Contains("快充电位_"))
            {
                oriFaPath = App.FamilyFilePath + @"机动车\快充电位_.rfa";
                faName = "慢充电位_";//设定目标停车位族类型名字
            }
            else if (oriFsName.Contains("柱子_"))
            {
                oriFaPath = App.FamilyFilePath + @"柱子\柱子_.rfa";
                faName = "柱子_";//设定目标停车位族类型名字
            }

            FamilySymbol oeiFs = doc.FamilySymbolByPath(oriFaPath, oriFsName, faName);

            if (faName == "机械车位_")
            {
                if (oeiFs.Name != oriFsName)
                {
                    throw new NotImplementedException(("族类型，" + oriFsName + ",不存在，请联系BIM协调员。"));
                }
            }
            else if (oeiFs.Name != oriFsName && faName != "机械车位_")// 首先判断族类型名称是否符合要求
            {
                using (Transaction dupFS = new Transaction(doc))
                {
                    dupFS.Start("DuplicateFS");
                    oeiFs = oeiFs.Duplicate(oriFsName) as FamilySymbol;

                    Parameter para_width = oeiFs.LookupParameter("Width");
                    Parameter para_height = oeiFs.LookupParameter("Height");

                    string[] parkPlaceParas = oriFsName.Split(new char[] { '_', '*' });
                    double width = Convert.ToDouble(parkPlaceParas[parkPlaceParas.Length - 2]);
                    double height = Convert.ToDouble(parkPlaceParas[parkPlaceParas.Length - 1]);

                    width = width.MilliMeterToFeet();
                    height = height.MilliMeterToFeet();

                    para_width.Set(width);
                    para_height.Set(height);

                    dupFS.Commit();
                }
            }
            else
            {
                string[] parkPlaceParas = oriFsName.Split(new char[] { '_', '*' });
                double width = Convert.ToDouble(parkPlaceParas[parkPlaceParas.Length - 2]);
                double height = Convert.ToDouble(parkPlaceParas[parkPlaceParas.Length - 1]);

                width = width.MilliMeterToFeet();
                height = height.MilliMeterToFeet(); ;

                Parameter para_width = oeiFs.LookupParameter("Width");
                Parameter para_height = oeiFs.LookupParameter("Height");

                using (Transaction DuplicateFS = new Transaction(doc))
                {
                    DuplicateFS.Start("JudgmentParameters");
                    if (width != para_width.AsDouble())
                    {
                        para_width.Set(width);
                    }
                    if (height != para_height.AsDouble())
                    {
                        para_height.Set(height);
                    }
                    DuplicateFS.Commit();
                }
            }
            return oeiFs;
        }

    }
}
