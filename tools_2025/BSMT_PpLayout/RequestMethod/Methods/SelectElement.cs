using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using Autodesk.Revit.DB;
using Autodesk.Revit.ApplicationServices;
using PubFuncWt;
using g3;

using goa.Common;
using QuadTrees;
using System.Drawing;
using goa.Common.UserOperation;

namespace BSMT_PpLayout
{
    class SelectElement : RequestMethod
    {

        public SelectElement(UIApplication uiapp) : base(uiapp)
        {
        }

        internal override void Execute()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 选择地库设计区域 详图填充区域类型 为 地库_地库外墙线
        /// </summary>
        internal void SelBasementExteriorWallOutLine()
        {
            if (GlobalData.Instance.isAddRemove)//添加
            {
                // 首次选择详图填充区域
                var _filledRegion = doc.GetElement(sel.PickObject(ObjectType.Element, new SelPickFilter_FilledRegion_BaseMent() { Doc = doc }, "请点选详图区域，类别为 地库_外墙范围")) as FilledRegion;

                ViewPlan viewPlan = doc.GetElement(_filledRegion.OwnerViewId) as ViewPlan;
                ElementId elementId = _filledRegion.Id;

                // 判断id是否存在UI界面的方案列表中
                InitialUIinter initialUIinter = new InitialUIinter(this.uiApp);
                List<ElementId> uibsmtEleIds = initialUIinter.UiBsmtWallIds();

                if (!uibsmtEleIds.Contains(elementId))// 如果存在id则不进行命名设置
                {
                    SetBasementName(viewPlan, _filledRegion);
                }
            }
            else if (!GlobalData.Instance.isAddRemove)// 删除
            {
                // 使用listBox进行删除
                if (GlobalData.Instance.selViewNames.Count > 0)
                {
                    List<string> selBasementNames = new List<string>();
                    foreach (var item in GlobalData.Instance.selViewNames)
                    {
                        selBasementNames.Add(item.ToString());
                    }
                    foreach (var item in selBasementNames)
                    {
                        if (GlobalData.basementWallOutlineNames.Contains(item))
                        {
                            GlobalData.basementWallOutlineNames.Remove(item);
                            // 删除填充区域的标记值
                            string[] strs = item.Split(':');
                            string strElementId = strs.Last();
                            strElementId = strElementId.Remove(strElementId.Length - 1, 1);
                            int elementInt = Convert.ToInt32(strElementId);

                            ElementId elementId = new ElementId(elementInt);
                            Element element = doc.GetElement(elementId);
                            using (Transaction trans = new Transaction(doc))
                            {
                                trans.Start("命名地库方案名称");
                                element.get_Parameter(BuiltInParameter.DOOR_NUMBER).Set("");
                                trans.Commit();
                            }
                        }
                    }
                }
                else
                {
                    // 提醒窗口，可取消
                    ("选择项数为 0").TaskDialogErrorMessage();
                }
            }

            //【】初始化设置
            InitialCMD initialSetup = new InitialCMD(this.uiApp);
            initialSetup.Execute();
        }
        /// <summary>
        /// 将UI界面选中的地库外墙线中的元素进行重置，重装需要删除与所有地库外墙线范围无关的停车位族，地库_子区域
        /// </summary>
        internal void RestBasementWallElements()
        {
            InitialUIinter initialUIinter = new InitialUIinter(this.uiApp);
            List<ElementId> selBaseWallIds = initialUIinter.SelBsmtWallIds(); // UI获取所有地库填充区域id
            List<ElemsViewLevel> elemsViewLevels = initialUIinter.ElemsViewLevels(); //一个视图对应一个层级

            foreach (ElemsViewLevel elemsViewLevel in elemsViewLevels)// 遍历单个视图
            {
                elemsViewLevel.DelUnUsefulEles();// 清除与地库边界围合区域无关的图元

                foreach (Bsmt bsmt in elemsViewLevel.Bsmts)// 遍历单个地库
                {

                    if (!selBaseWallIds.Contains(bsmt.BsmtBound.Id))// 判断是否UI选择
                        continue;
                    bsmt.DelUnUsefulParkSpaces();// 清除与地库边界暧昧不清的图元
                    bsmt.DelUnUsefulSubPsAreaExits();// 清除与地库边界暧昧不清的图元
                    bsmt.DelUnUsefulPillar();

                    // 抓取坡道
                    bsmt.Computer_VeRa();

                    BsmtClipByRoad bsmtClipByRoad = new BsmtClipByRoad(bsmt);// 算法启动 1- 划分区域     
                    bsmtClipByRoad.CreatFilledRegion();// 创建新的子停车区域
                }
            }
        }
        /// <summary>
        /// 首次记录Id需要为地库填充区域设置方案名称
        /// </summary>
        public void SetBasementName(ViewPlan viewPlan, FilledRegion _filledRegion)
        {
            //【】判断已经设置好的方案名，防止重新输入方案命名重复问题 -  这里使用元素的标记值进行追踪
            List<string> names =
                new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_DetailComponents).OfClass(typeof(FilledRegion)).WhereElementIsNotElementType()
                .Where(p =>
                {
                    FilledRegionType tempFilledRegionType = doc.GetElement(p.GetTypeId()) as FilledRegionType;
                    return tempFilledRegionType.Name == "地库外墙范围" || tempFilledRegionType.Name == "地库_外墙范围";
                })
                .Select(p => p.get_Parameter(BuiltInParameter.DOOR_NUMBER).AsString())
                .ToList();

        // 弹窗设置底裤填充区域的标记值作为其方案名称
        reOpenSettingWindow:
            SetDesignNamed setDesignNamed = new SetDesignNamed();
            setDesignNamed.ShowDialog();
            string nameSetting = setDesignNamed.textBox01.Text;
            if (names.Contains(nameSetting))
            {
                ("标记值不可以重复，请重新输入").TaskDialogErrorMessage();
                goto reOpenSettingWindow;
            }
            if (nameSetting == "" || nameSetting == null)
            {
                ("方案名称不可以为空值，请重新输入").TaskDialogErrorMessage();
                goto reOpenSettingWindow;
            }
            else
            {
                string id = _filledRegion.Id.ToString();
                string basementName = $"方案:{nameSetting} (id:{id})";

                foreach (string str in GlobalData.basementWallOutlineNames)
                {
                    string[] strs = str.Split('+');
                    string strElementId = strs.First();
                    if (strElementId == "方案：" + nameSetting)
                    {
                        ("方案名称不可以重复，请重新输入").TaskDialogErrorMessage();
                        goto reOpenSettingWindow;
                    }
                }
                GlobalData.basementWallOutlineNames.Add(basementName);
                using (Transaction trans = new Transaction(doc))
                {
                    trans.Start("命名地库方案名称");
                    _filledRegion.get_Parameter(BuiltInParameter.DOOR_NUMBER).Set(nameSetting);
                    trans.Commit();
                }
            }
        }
        /// <summary>
        /// 框选柱子族实例
        /// </summary>
        internal void SelColumsFs()
        {
            MulPointsSelection selection_ = new MulPointsSelection(this.uiDoc);
            selection_.RegionUsrForEvent += Selection_RegionUsrForEvent;

            selection_.PickPolygon();
            selection_.RegionUsrFor();
        }

        private void Selection_RegionUsrForEvent(object sender, EventArgs e)
        {
            QuadTreeRectF<QTreeRevitEleCtrl> qtree = new QuadTreeRectF<QTreeRevitEleCtrl>();
            IList<Element> elements = new FilteredElementCollector(this.doc, this.view.Id).OfCategory(BuiltInCategory.OST_DetailComponents).WhereElementIsNotElementType().ToElements();

            foreach (var item in elements)
            {
                if (item.Name.Contains("柱子_"))
                {
                    RectangleF rectangleF = item.get_BoundingBox(this.view).ToRectangleF();
                    RevitEleCtrl revitEleCtrl = new RevitEleCtrl(item);
                    QTreeRevitEleCtrl qTreeRevitEleCtrl = new QTreeRevitEleCtrl(rectangleF, revitEleCtrl);
                    qtree.Add(qTreeRevitEleCtrl);
                }
            }

            RegionPoints region_ = sender as RegionPoints;
            Polygon2d polygon2d = new Polygon2d(region_.Points.ToVector2ds());

            List<QTreeRevitEleCtrl> qTreeRevitEleCtrls = qtree.GetObjects(polygon2d.ToRectangle());

            List<ElementId> elementIds = new List<ElementId>();
            foreach (var item in qTreeRevitEleCtrls)
            {
                Polygon2d temp = item.Rect.ToPolygon2d();
                if (polygon2d.Contains(temp) || polygon2d.Intersects(temp))
                {
                    elementIds.Add(item.RevitEleCtrl.Ele.Id);
                }
            }

            this.sel.SetElementIds(elementIds);

            // throw new NotImplementedException();
        }
        internal void SelPSByLine()
        {
            MulPointsSelection mulPointsSelection = new MulPointsSelection(this.uiDoc);
            mulPointsSelection.RegionUsrForEvent += Selection__RegionUsrForEvent1; ;

            List<XYZ> pickPoints = mulPointsSelection.PickPolygon();
            mulPointsSelection.RegionUsrFor();
        }

        private void Selection__RegionUsrForEvent1(object sender, EventArgs e)
        {
            QuadTreeRectF<QTreeRevitEleCtrl> qtree = new QuadTreeRectF<QTreeRevitEleCtrl>();
            IList<Element> elements = new FilteredElementCollector(this.doc, this.view.Id).OfCategory(BuiltInCategory.OST_DetailComponents).WhereElementIsNotElementType().ToElements();

            foreach (var item in elements)
            {
                if (item.Name.Contains("车位_"))
                {
                    RectangleF rectangleF = item.get_BoundingBox(this.view).ToRectangleF();
                    RevitEleCtrl revitEleCtrl = new RevitEleCtrl(item);
                    QTreeRevitEleCtrl qTreeRevitEleCtrl = new QTreeRevitEleCtrl(rectangleF, revitEleCtrl);
                    qtree.Add(qTreeRevitEleCtrl);
                }
            }

            RegionPoints region_ = sender as RegionPoints;
            Polygon2d polygon2d = new Polygon2d(region_.Points.ToVector2ds());

            List<QTreeRevitEleCtrl> qTreeRevitEleCtrls = qtree.GetObjects(polygon2d.ToRectangle());

            List<ElementId> elementIds = new List<ElementId>();
            foreach (var item in qTreeRevitEleCtrls)
            {
                Polygon2d temp = item.Rect.ToPolygon2d();
                if (polygon2d.Contains(temp) || polygon2d.Intersects(temp))
                {
                    elementIds.Add(item.RevitEleCtrl.Ele.Id);
                }
            }

            this.sel.SetElementIds(elementIds);

            // throw new NotImplementedException();
        }

        /// <summary>
        /// 框选停车位族实例 进行解锁
        /// </summary>
        internal void SetunFixedParkingFs()
        {

            List<Element> selEles = sel.PickElementsByRectangle(new SelPickFilter_FamilyInstance(), "请框选停车位族实例")
             .Where(p => p.Name.Contains("停车位_")
                || p.Name.Contains("大车位_")
                || p.Name.Contains("微型车位_")
                || p.Name.Contains("机械车位_")
                || p.Name.Contains("无障碍车位_")
                || p.Name.Contains("子母车位_")
                || p.Name.Contains("公共泊车位_")
                || p.Name.Contains("慢充电车位")
                || p.Name.Contains("快充电车位"))
                .ToList();

            using (Transaction transSetFixedParam = new Transaction(doc))
            {
                transSetFixedParam.Start("transSetFixedParam");
                selEles.ForEach(p =>
                {
                    Parameter parameter = p.LookupParameter("锁定_");
                    parameter.Set(0);
                });
                transSetFixedParam.Commit();
            }

            //List<Element> unfixedParkingFs = selEles.Where(p=>p.get_Parameter(CMD.parkingFixedGid).AsValueString() == "否").ToList();

            sel.SetElementIds(selEles.ToElementIds().ToList());
        }
        /// <summary>
        /// 框选停车位族实例 进行锁定
        /// </summary>
        internal void SetFixedParkingFs()
        {
            #region 与revit文档交互入口
            UIDocument uidoc = this.uiApp.ActiveUIDocument;
            Application app = uiApp.Application;
            Document doc = uidoc.Document;
            Selection sel = uidoc.Selection;
            View acvtiView = doc.ActiveView;
            #endregion

            List<Element> selEles = sel.PickElementsByRectangle(new SelPickFilter_FamilyInstance(), "请框选停车位族实例")
                .Where(p => p.Name.Contains("停车位_")
                || p.Name.Contains("大车位_")
                || p.Name.Contains("微型车位_")
                || p.Name.Contains("机械车位_")
                || p.Name.Contains("无障碍车位_")
                || p.Name.Contains("子母车位_")
                || p.Name.Contains("公共泊车位_")
                || p.Name.Contains("慢充电车位")
                || p.Name.Contains("快充电车位"))
                .ToList();

            using (Transaction transSetFixedParam = new Transaction(doc))
            {
                transSetFixedParam.Start("transSetFixedParam");
                selEles.ForEach(p =>
                {
                    Parameter parameter = p.LookupParameter("锁定_");
                    parameter.Set(1);
                });
                transSetFixedParam.Commit();
            }
            //List<Element> fixedParkingFs = selEles.Where(p => p.get_Parameter(CMD.parkingFixedGid).AsValueString() == "是").ToList();

            sel.SetElementIds(selEles.ToElementIds().ToList());
        }
    }
}
