using Autodesk.Revit.DB;
using PubFuncWt;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSMT_PpLayout
{
    class EleCtrlFactory
    {
        private List<Element> _elements;
        private List<QTreeRevitEleCtrl> _qTreeRevitEleCtrls;
        internal List<QTreeRevitEleCtrl> QTreeRevitEleCtrls => this._qTreeRevitEleCtrls;
        internal Document Doc { get; }
        internal View View { get; }
        internal EleCtrlFactory(List<Element> _qTreeRevitEles, Document document, View view)
        {
            this._elements = _qTreeRevitEles;
            this.Doc = document;
            this.View = view;
        }

        internal void Computer()
        {
            this._qTreeRevitEleCtrls = GetQTreeRevitEleCtrls().ToList();
        }
        private IEnumerable<QTreeRevitEleCtrl> GetQTreeRevitEleCtrls()
        {
            List<ElementId> elementIds = new List<ElementId>();// 剔除与group相关的元素
            foreach (var item in this._elements)
            {
                if (item is Group)
                {
                    elementIds.AddRange((item as Group).GetMemberIds());
                    elementIds.Add(item.Id);
                }
            }

            foreach (var item in this._elements)
            {
                if (elementIds.Contains(item.Id)) continue;

                RectangleF rectangleF = item.get_BoundingBox(this.View).ToRectangleF();

                if (item is FamilyInstance)
                {
                    string name = item.Name;

                    if (name.Contains("子母车位_"))
                        yield return new QTreeRevitEleCtrl(rectangleF, new RevitElePS(item, EleProperty.AttachedPP));
                    else if (name.Contains("无障碍车位_"))
                        yield return new QTreeRevitEleCtrl(rectangleF, new RevitElePS(item, EleProperty.BarrierFreePP));
                    else if (name.Contains("大车位_"))
                        yield return new QTreeRevitEleCtrl(rectangleF, new RevitElePS(item, EleProperty.BigParkSpace));
                    else if (name.Contains("快充电位_"))
                        yield return new QTreeRevitEleCtrl(rectangleF, new RevitElePS(item, EleProperty.FastChargePP));
                    else if (name.Contains("机械车位_"))
                        yield return new QTreeRevitEleCtrl(rectangleF, new RevitElePS(item, EleProperty.MechanicalPP));
                    else if (name.Contains("微型车位_"))
                        yield return new QTreeRevitEleCtrl(rectangleF, new RevitElePS(item, EleProperty.MiniParkSpace));
                    else if (name.Contains("停车位_") && !name.Contains("回车_"))
                        yield return new QTreeRevitEleCtrl(rectangleF, new RevitElePS(item, EleProperty.ParkSpace));
                    else if (name.Contains("回车_"))
                        yield return new QTreeRevitEleCtrl(rectangleF, new RevitElePS(item, EleProperty.EndPP));
                    else if (name.Contains("公共泊车位_"))
                        yield return new QTreeRevitEleCtrl(rectangleF, new RevitElePS(item, EleProperty.PublicPP));
                    else if (name.Contains("慢充电位_"))
                        yield return new QTreeRevitEleCtrl(rectangleF, new RevitElePS(item, EleProperty.SlowChargePP));

                    else if (name.Contains("坡道-直线"))
                        yield return new QTreeRevitEleCtrl(rectangleF, new RevitEleVeRa(item, EleProperty.VehicleRamp));
                    //else if (name.Contains("坡道-弧线A"))
                    //    yield return new QTreeRevitEleCtrl(rectangleF, new RevitEleVeRa(item, EleProperty.VehicleRamp_Arc_A));
                    else if (name.Contains("坡道-弧线B"))
                        yield return new QTreeRevitEleCtrl(rectangleF, new RevitEleVeRa(item, EleProperty.VehicleRamp_Arc_B));
                    else if (name.Contains("上下坡道") && !name.Contains("填充"))
                        yield return new QTreeRevitEleCtrl(rectangleF, new RevitEleVeRa(item, EleProperty.VehicleRamp_UpDown));
                    else if ((item as FamilyInstance).Symbol.FamilyName == "上下坡道-弧线")
                        yield return new QTreeRevitEleCtrl(rectangleF, new RevitEleVeRa(item, EleProperty.VehicleRamp_UpDown_Arc));

                    else if (name.Contains("柱子_"))
                        yield return new QTreeRevitEleCtrl(rectangleF, new RevitEleCol(item, EleProperty.ColumnUnit));

                    yield return new QTreeRevitEleCtrl(rectangleF, new RevitEleCtrl(item, EleProperty.None));
                }
                else if (item is FilledRegion)
                {
                    string typeName = item.Document.GetElement(item.GetTypeId()).Name;

                    if (typeName == "地库外墙范围" || typeName == "地库_外墙范围")
                        yield return new QTreeRevitEleCtrl(rectangleF, new BsmtBound(item, EleProperty.BsmtWall));
                    else if (typeName == "地库_核心筒")
                        yield return new QTreeRevitEleCtrl(rectangleF, new RevitEleFR(item, EleProperty.CoreTube));
                    else if (typeName == "设备用房" || typeName == "地库_设备用房")
                        yield return new QTreeRevitEleCtrl(rectangleF, new RevitEleFR(item, EleProperty.EquRoom));
                    else if (typeName == "地库_采光井")
                        yield return new QTreeRevitEleCtrl(rectangleF, new RevitEleFR(item, EleProperty.LightWell));
                    else if (typeName == "地库_主楼非停车区域")
                        yield return new QTreeRevitEleCtrl(rectangleF, new RevitEleFR(item, EleProperty.MainBuildingNonParkingArea));
                    else if (typeName == "地库_非机动车库" && !typeName.Contains("_夹层"))
                        yield return new QTreeRevitEleCtrl(rectangleF, new RevitEleFR(item, EleProperty.NonVehicleGarage));
                    else if (typeName == "地库_非机动车库_夹层")
                        yield return new QTreeRevitEleCtrl(rectangleF, new RevitEleFR(item, EleProperty.NonVehicleGarage_Mezzanine));

                    else if (typeName == "地库_子停车区域")
                        yield return new QTreeRevitEleCtrl(rectangleF, new RevitEleFR(item, EleProperty.SubPsAreaExit));// 地库_子停车区域 

                    else if (typeName == "地库_坡道")
                        yield return new QTreeRevitEleCtrl(rectangleF, new RevitEleFR(item, EleProperty.Ramp));// 出入口坡道 

                    else if (typeName == "塔楼开间区域" || typeName == "地库_塔楼开间区域")
                        yield return new QTreeRevitEleCtrl(rectangleF, new RevitEleFR(item, EleProperty.ResidenOpenRoom));
                    else if (typeName == "塔楼结构轮廓" || typeName == "地库_塔楼结构轮廓")
                        yield return new QTreeRevitEleCtrl(rectangleF, new RevitEleFR(item, EleProperty.ResidenStruRegion));
                    else if (typeName == "公变所" || typeName == "地库_公变所")
                        yield return new QTreeRevitEleCtrl(rectangleF, new RevitEleFR(item, EleProperty.ResidentialUtilitySubstation));
                    else if (typeName == "地库_下沉庭院")
                        yield return new QTreeRevitEleCtrl(rectangleF, new RevitEleFR(item, EleProperty.SinkingCourtyard));
                    else if (typeName == "地库_结构柱")
                        yield return new QTreeRevitEleCtrl(rectangleF, new RevitEleFR(item, EleProperty.StruColumn));
                    else if (typeName == "地库_工具间")
                        yield return new QTreeRevitEleCtrl(rectangleF, new RevitEleFR(item, EleProperty.ToolRoom));
                    else if (typeName == "地库_单元门厅")
                        yield return new QTreeRevitEleCtrl(rectangleF, new RevitEleFR(item, EleProperty.UnitFoyer));
                    else if (typeName == "地库_储藏间")
                        yield return new QTreeRevitEleCtrl(rectangleF, new RevitEleFR(item, EleProperty.Storeroom));
                    else if (typeName == "地库_人防分区")
                        yield return new QTreeRevitEleCtrl(rectangleF, new RevitEleFR(item, EleProperty.AirDefenseDivision));
                    // 没有被归纳的其他类型的填充区域--障碍物
                    else if (typeName.Contains("障碍"))
                        yield return new QTreeRevitEleCtrl(rectangleF, new RevitEleFR(item, EleProperty.Obstructive));
                    else
                        yield return new QTreeRevitEleCtrl(rectangleF, new RevitEleCtrl(item, EleProperty.None));
                }
                else if (item is DetailCurve || item is ModelCurve)
                {
                    // 对curve进行清洗，保留非圆形曲线

                    Curve curve = (item as CurveElement).GeometryCurve;
                    if (curve is Arc)
                    {
                        if (!(curve as Arc).IsBound)// 圆弧非圆形的判断方式
                        {
                            yield return new QTreeRevitEleCtrl(rectangleF, new RevitEleCurve(item, EleProperty.Circle));
                        }
                    }

                    string typeName = (item as CurveElement).LineStyle.Name;

                    if (typeName == "地库_主车道中心线")
                        yield return new QTreeRevitEleCtrl(rectangleF, new RevitEleCurve(item, EleProperty.PriLane));
                    else if (typeName == "地库_次车道中心线")
                        yield return new QTreeRevitEleCtrl(rectangleF, new RevitEleCurve(item, EleProperty.SecLane));
                    else if (typeName == "地库_自定义宽度车道中心线")
                        yield return new QTreeRevitEleCtrl(rectangleF, new RevitEleCurve(item, EleProperty.CusLane));
                }
                else if (item is Wall)
                {
                    string typeName = item.Name;

                    if (typeName.Contains("地库_剪力墙"))
                        yield return new QTreeRevitEleCtrl(rectangleF, new RevitEleWall(item, EleProperty.ShearWall));
                    else if (typeName.Contains("地库_防火墙"))
                        yield return new QTreeRevitEleCtrl(rectangleF, new RevitEleWall(item, EleProperty.FireWall));
                }
                else
                    yield return new QTreeRevitEleCtrl(rectangleF, new RevitEleCtrl(item, EleProperty.None));
            }




        }

    }
}
