using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Selection;
using System.Diagnostics;
using PubFuncWt;
using g3;
using goa.Common;

namespace BSMT_PpLayout
{
    class TableHandle
    {
        Document _doc;

        DataCollection _firstDC = new DataCollection();
        DataCollection _secondDC = new DataCollection();
        DataCollection _thirdDC = new DataCollection();

        internal TableHandle(Document document)
        {
            this._doc = document;
        }
        internal void Execute(List<DataCollection> dataCollections)
        {
            if(dataCollections.Count==1)// 只有一层
            {
                _firstDC = dataCollections[0];
            }
            else if (dataCollections.Count > 1)// 有两层
            {
                foreach (var item in dataCollections)
                {
                    if(item.DseignName.Contains("A")
                        || item.DseignName.Contains("1")
                        || item.DseignName.Contains("I")
                        || item.DseignName.Contains("i")
                        || item.DseignName.Contains("一层"))
                    {
                        _firstDC = item;
                    }
                    if (item.DseignName.Contains("B")
                        || item.DseignName.Contains("2")
                        || item.DseignName.Contains("II")
                        || item.DseignName.Contains("ii")
                        || item.DseignName.Contains("二层"))
                    {
                        _secondDC = item;
                    }
                    if (item.DseignName.Contains("C")
                        || item.DseignName.Contains("3")
                        || item.DseignName.Contains("III")
                        || item.DseignName.Contains("iii")
                        || item.DseignName.Contains("三层"))
                    {
                        _thirdDC = item;
                    }
                }
            }

            // 图元id信息
            string elementIdNames = "";
            foreach (var item in dataCollections.Select(p=>p.ElementId))
            {
                elementIdNames += item.ToString() + "_";
            }

            // 寻找表格
            List<Element> groups = new FilteredElementCollector(this._doc).OfCategory(BuiltInCategory.OST_IOSDetailGroups).ToElements().ToList();

            // 判断有没有含有目标id的表格组类型 地库图面数据汇总_
            GroupType mapDataStatisticIndexGroupType = groups
                .Where(p => p is GroupType)
                .Where(p => p.Name.Contains("地库图面数据汇总_"))
                .Where(p => p.Name.Contains(elementIdNames))// 是否包含目标id
                .FirstOrDefault() as GroupType;

            GroupType singleCarAreaIndexGroupType = groups
                .Where(p => p is GroupType)
                .Where(p => p.Name.Contains("地下车库单车面积指标_"))
                .Where(p => p.Name.Contains(elementIdNames))// 是否包含目标id
                .FirstOrDefault() as GroupType;

            if (mapDataStatisticIndexGroupType != null)
            {
                using (Transaction transaction = new Transaction(this._doc, "删除已有的组类型"))
                {
                    transaction.Start();
                    this._doc.Delete(mapDataStatisticIndexGroupType.Id);
                    transaction.Commit();
                }
            }
            if (singleCarAreaIndexGroupType != null)
            {
                using (Transaction transaction = new Transaction(this._doc, "删除已有的组类型"))
                {
                    transaction.Start();
                    this._doc.Delete(singleCarAreaIndexGroupType.Id);
                    transaction.Commit();
                }
            }

            // 创建新的目标表格组group

   
            string mapDataStatisticIndexGroupTypeNewName = "地库图面数据汇总_" + elementIdNames;
            string singleCarAreaIndexGroupTypeNewName = "地下车库单车面积指标_" + elementIdNames;

            // 如何区分一层与二层

            HandleMapDataStatisticIndexGroupType(mapDataStatisticIndexGroupTypeNewName);
            HandleSingleCarAreaIndexGroupType(singleCarAreaIndexGroupTypeNewName);

        }
        /// <summary>
        /// 处理地库图面数据
        /// </summary>
        /// <param name="groupTypeName"></param>
        internal Group HandleMapDataStatisticIndexGroupType(string groupTypeName)
        {
            // 基于模板创建一个实例

            double xSpace = 98.425196850394;
            double ySpace = 22.9658792650882;

            double xLength = xSpace * 6.5;// 表格的横向长度
            double yLength = ySpace * 6;// 表格的高度
            double secondYLength = ySpace * 5;// 表格的高度

            // 表格分割线
            // 7根与x轴平行的线
            Line line01x = Line.CreateBound(new XYZ(0, 0, 0), new XYZ(xLength, 0, 0));
            double yPositionLine = ySpace;
            Line line02x = Line.CreateBound(new XYZ(0, yPositionLine, 0), new XYZ(xLength, yPositionLine, 0));
            yPositionLine += ySpace ;
            Line line03x = Line.CreateBound(new XYZ(0, yPositionLine, 0), new XYZ(xLength, yPositionLine, 0));
            yPositionLine += ySpace ;
            Line line04x = Line.CreateBound(new XYZ(0, yPositionLine, 0), new XYZ(xLength, yPositionLine, 0));
            yPositionLine += ySpace ;
            Line line05x = Line.CreateBound(new XYZ(0, yPositionLine, 0), new XYZ(xLength, yPositionLine, 0));
            yPositionLine += ySpace ;
            Line line06x = Line.CreateBound(new XYZ(0, yPositionLine, 0), new XYZ(xLength, yPositionLine, 0));
            yPositionLine += ySpace ;
            Line line07x = Line.CreateBound(new XYZ(0, yPositionLine, 0), new XYZ(xLength, yPositionLine, 0));
            // 7根与y轴平行的线 由于标题的原因 最左最右两根线段比中间的长
            Line line01y = Line.CreateBound(new XYZ(0, 0, 0), new XYZ(0, yLength, 0));
            double xPositionLine = xSpace * 0.5;
            Line line02y = Line.CreateBound(new XYZ(xPositionLine, 0, 0), new XYZ(xPositionLine, secondYLength, 0));
            xPositionLine += xSpace;
            Line line03y = Line.CreateBound(new XYZ(xPositionLine, 0, 0), new XYZ(xPositionLine, secondYLength, 0));
            xPositionLine += xSpace;
            Line line04y = Line.CreateBound(new XYZ(xPositionLine, 0, 0), new XYZ(xPositionLine, secondYLength, 0));
            xPositionLine += xSpace;
            Line line05y = Line.CreateBound(new XYZ(xPositionLine, 0, 0), new XYZ(xPositionLine, secondYLength, 0));
            xPositionLine += xSpace;
            Line line06y = Line.CreateBound(new XYZ(xPositionLine, 0, 0), new XYZ(xPositionLine, secondYLength, 0));
            xPositionLine += xSpace;
            Line line07y = Line.CreateBound(new XYZ(xPositionLine, 0, 0), new XYZ(xPositionLine, secondYLength, 0));
            xPositionLine += xSpace;
            Line line08y = Line.CreateBound(new XYZ(xPositionLine, 0, 0), new XYZ(xPositionLine, yLength, 0));

            List<Line> lines = new List<Line>() { line01x, line02x, line03x, line04x, line05x, line06x, line07x, line01y, line02y, line03y, line04y, line05y, line06y, line07y, line08y };

            List<ElementId> elementIds = new List<ElementId>();
            DetailCurveArray detailCurveArray = new DetailCurveArray();
            using (Transaction transaction = new Transaction(this._doc, "创建表格定位线"))
            {
                transaction.Start();
                detailCurveArray = this._doc.Create.NewDetailCurveArray(this._doc.ActiveView, lines.ToCurveArray());
                transaction.Commit();
            }
            foreach (DetailCurve detailCurve in detailCurveArray)
            {
                elementIds.Add(detailCurve.Id);
            }

            // 文字
            // 判断 仿宋_5.0mm 是否存在
            TextNoteType fangSong5 = new FilteredElementCollector(this._doc).OfClass(typeof(TextNoteType)).Where(p => p.Name.Contains("仿宋_5.0")).FirstOrDefault() as TextNoteType;
       
            if (fangSong5==null)
            {
                // 文字类型确认
                ElementId defaultTypeId = this._doc.GetDefaultElementTypeId(ElementTypeGroup.TextNoteType);// 默认文字类型为 仿宋_3.5mm
                TextNoteType textNoteType = this._doc.GetElement(defaultTypeId) as TextNoteType;
                // 修改文字大小 透明度
                using (Transaction creatNewTextNoteType = new Transaction(this._doc, "设置目标文字类型"))
                {
                    creatNewTextNoteType.Start();
                    fangSong5 = textNoteType.Duplicate("仿宋_5.0mm") as TextNoteType;
                    fangSong5.get_Parameter(BuiltInParameter.TEXT_SIZE).SetValue(5.0.MilliMeterToFeet());
                    int transparency = 1;
                    fangSong5.get_Parameter(BuiltInParameter.TEXT_BACKGROUND).SetValue(transparency);
                    creatNewTextNoteType.Commit();
                }
            }
            else
            {
                // 修改文字大小
                using (Transaction creatNewTextNoteType = new Transaction(this._doc, "设置目标文字类型"))
                {
                    creatNewTextNoteType.Start();
                    int transparency = 1;
                    fangSong5.get_Parameter(BuiltInParameter.TEXT_BACKGROUND).SetValue(transparency); 
                    creatNewTextNoteType.Commit();
                }
            }
            // ==>
            string text61 = "图面统计汇总";
            XYZ positionTemp = new XYZ(xLength / 2, yLength, 0);// 创建文字的position指的是文字编辑栏的右上角
            TextNote note = null;
            using (Transaction tran = new Transaction(this._doc, "Creating a Text note"))
            {
                tran.Start();
                note = TextNote.Create(this._doc, this._doc.ActiveView.Id, positionTemp, text61, fangSong5.Id);
                tran.Commit();
            }
            if (note != null)
            {
                elementIds.Add(note.Id);
            }
            // ==>
            
            string text11 = "限值";
            positionTemp = new XYZ(0.0, ySpace, 0);// 创建文字的position指的是文字编辑栏的右上角
            note = null;
            using (Transaction tran = new Transaction(this._doc, "Creating a Text note"))
            {
                tran.Start();
                note = TextNote.Create(this._doc, this._doc.ActiveView.Id, positionTemp, text11, fangSong5.Id);
                tran.Commit();
            }
            if (note != null)
            {
                elementIds.Add(note.Id);
            }

            // ==>
            string text21 = "合计";

            positionTemp = new XYZ(0.0, ySpace*2, 0);// 创建文字的position指的是文字编辑栏的右上角
            note = null;
            using (Transaction tran = new Transaction(this._doc, "Creating a Text note"))
            {
                tran.Start();
                note = TextNote.Create(this._doc, this._doc.ActiveView.Id, positionTemp, text21, fangSong5.Id);
                tran.Commit();
            }
            if (note != null)
            {
                elementIds.Add(note.Id);
            }
            // ==>

            string text31 = "地下二层";
            positionTemp = new XYZ(0.0, ySpace * 3, 0);// 创建文字的position指的是文字编辑栏的右上角
            note = null;
            using (Transaction tran = new Transaction(this._doc, "Creating a Text note"))
            {
                tran.Start();
                note = TextNote.Create(this._doc, this._doc.ActiveView.Id, positionTemp, text31, fangSong5.Id);
                tran.Commit();
            }
            if (note != null)
            {
                elementIds.Add(note.Id);
            }
            // ==>
            string text41 = "地下一层";
            positionTemp = new XYZ(0.0, ySpace * 4, 0);// 创建文字的position指的是文字编辑栏的右上角
            note = null;
            using (Transaction tran = new Transaction(this._doc, "Creating a Text note"))
            {
                tran.Start();
                note = TextNote.Create(this._doc, this._doc.ActiveView.Id, positionTemp, text41, fangSong5.Id);
                tran.Commit();
            }
            if (note != null)
            {
                elementIds.Add(note.Id);
            }

            // ==> 所在列 微型车位

            string text52 = "微型车位";
            positionTemp = new XYZ(xSpace * 0.5, ySpace * 5, 0);// 创建文字的position指的是文字编辑栏的右上角
            note = null;
            using (Transaction tran = new Transaction(this._doc, "Creating a Text note"))
            {
                tran.Start();
                note = TextNote.Create(this._doc, this._doc.ActiveView.Id, positionTemp, text52, fangSong5.Id);
                tran.Commit();
            }
            if (note != null)
            {
                elementIds.Add(note.Id);
            }

            string text42 = this._firstDC.N_MiniParkSpace.ToString();
            positionTemp = new XYZ(xSpace * 0.5, ySpace * 4, 0);// 创建文字的position指的是文字编辑栏的右上角
            note = null;
            using (Transaction tran = new Transaction(this._doc, "Creating a Text note"))
            {
                tran.Start();
                note = TextNote.Create(this._doc, this._doc.ActiveView.Id, positionTemp, text42, fangSong5.Id);
                tran.Commit();
            }
            if (note != null)
            {
                elementIds.Add(note.Id);
            }

            string text32 = this._secondDC.N_MiniParkSpace.ToString();
            positionTemp = new XYZ(xSpace * 0.5, ySpace * 3, 0);// 创建文字的position指的是文字编辑栏的右上角
            note = null;
            using (Transaction tran = new Transaction(this._doc, "Creating a Text note"))
            {
                tran.Start();
                note = TextNote.Create(this._doc, this._doc.ActiveView.Id, positionTemp, text32, fangSong5.Id);
                tran.Commit();
            }
            if (note != null)
            {
                elementIds.Add(note.Id);
            }

            string text22 = (this._firstDC.N_MiniParkSpace + this._secondDC.N_MiniParkSpace).ToString();
            positionTemp = new XYZ(xSpace * 0.5, ySpace * 2, 0);// 创建文字的position指的是文字编辑栏的右上角
            note = null;
            using (Transaction tran = new Transaction(this._doc, "Creating a Text note"))
            {
                tran.Start();
                note = TextNote.Create(this._doc, this._doc.ActiveView.Id, positionTemp, text22, fangSong5.Id);
                tran.Commit();
            }
            if (note != null)
            {
                elementIds.Add(note.Id);
            }

            // ==>
            string text53 = "子母车位";
            positionTemp = new XYZ(xSpace * 1.5, ySpace * 5, 0);// 创建文字的position指的是文字编辑栏的右上角
            note = null;
            using (Transaction tran = new Transaction(this._doc, "Creating a Text note"))
            {
                tran.Start();
                note = TextNote.Create(this._doc, this._doc.ActiveView.Id, positionTemp, text53, fangSong5.Id);
                tran.Commit();
            }
            if (note != null)
            {
                elementIds.Add(note.Id);
            }

            string text43 = this._firstDC.N_AttachedPP.ToString();
            positionTemp = new XYZ(xSpace * 1.5, ySpace * 4, 0);// 创建文字的position指的是文字编辑栏的右上角
            note = null;
            using (Transaction tran = new Transaction(this._doc, "Creating a Text note"))
            {
                tran.Start();
                note = TextNote.Create(this._doc, this._doc.ActiveView.Id, positionTemp, text43, fangSong5.Id);
                tran.Commit();
            }
            if (note != null)
            {
                elementIds.Add(note.Id);
            }
            string text33 = this._secondDC.N_AttachedPP.ToString();
            positionTemp = new XYZ(xSpace * 1.5, ySpace * 3, 0);// 创建文字的position指的是文字编辑栏的右上角
            note = null;
            using (Transaction tran = new Transaction(this._doc, "Creating a Text note"))
            {
                tran.Start();
                note = TextNote.Create(this._doc, this._doc.ActiveView.Id, positionTemp, text33, fangSong5.Id);
                tran.Commit();
            }
            if (note != null)
            {
                elementIds.Add(note.Id);
            }
            string text23 = (this._firstDC.N_AttachedPP + this._secondDC.N_AttachedPP).ToString();
            positionTemp = new XYZ(xSpace * 1.5, ySpace * 2, 0);// 创建文字的position指的是文字编辑栏的右上角
            note = null;
            using (Transaction tran = new Transaction(this._doc, "Creating a Text note"))
            {
                tran.Start();
                note = TextNote.Create(this._doc, this._doc.ActiveView.Id, positionTemp, text23, fangSong5.Id);
                tran.Commit();
            }
            if (note != null)
            {
                elementIds.Add(note.Id);
            }

            // ==>
            string text54 = "大车位";
            positionTemp = new XYZ(xSpace * 2.5, ySpace * 5, 0);// 创建文字的position指的是文字编辑栏的右上角
            note = null;
            using (Transaction tran = new Transaction(this._doc, "Creating a Text note"))
            {
                tran.Start();
                note = TextNote.Create(this._doc, this._doc.ActiveView.Id, positionTemp, text54, fangSong5.Id);
                tran.Commit();
            }
            if (note != null)
            {
                elementIds.Add(note.Id);
            }
            string text44 = this._firstDC.N_BigParkSpace.ToString();
            positionTemp = new XYZ(xSpace * 2.5, ySpace * 4, 0);// 创建文字的position指的是文字编辑栏的右上角
            note = null;
            using (Transaction tran = new Transaction(this._doc, "Creating a Text note"))
            {
                tran.Start();
                note = TextNote.Create(this._doc, this._doc.ActiveView.Id, positionTemp, text44, fangSong5.Id);
                tran.Commit();
            }
            if (note != null)
            {
                elementIds.Add(note.Id);
            }
            string text34 = this._secondDC.N_BigParkSpace.ToString();
            positionTemp = new XYZ(xSpace * 2.5, ySpace * 3, 0);// 创建文字的position指的是文字编辑栏的右上角
            note = null;
            using (Transaction tran = new Transaction(this._doc, "Creating a Text note"))
            {
                tran.Start();
                note = TextNote.Create(this._doc, this._doc.ActiveView.Id, positionTemp, text34, fangSong5.Id);
                tran.Commit();
            }
            if (note != null)
            {
                elementIds.Add(note.Id);
            }
            string text24 = (this._firstDC.N_BigParkSpace + this._secondDC.N_BigParkSpace).ToString();
            positionTemp = new XYZ(xSpace * 2.5, ySpace * 2, 0);// 创建文字的position指的是文字编辑栏的右上角
            note = null;
            using (Transaction tran = new Transaction(this._doc, "Creating a Text note"))
            {
                tran.Start();
                note = TextNote.Create(this._doc, this._doc.ActiveView.Id, positionTemp, text24, fangSong5.Id);
                tran.Commit();
            }
            if (note != null)
            {
                elementIds.Add(note.Id);
            }
            // ==>
            string text55 = "无障碍车位";
            positionTemp = new XYZ(xSpace * 3.5, ySpace * 5, 0);// 创建文字的position指的是文字编辑栏的右上角
            note = null;
            using (Transaction tran = new Transaction(this._doc, "Creating a Text note"))
            {
                tran.Start();
                note = TextNote.Create(this._doc, this._doc.ActiveView.Id, positionTemp, text55, fangSong5.Id);
                tran.Commit();
            }
            if (note != null)
            {
                elementIds.Add(note.Id);
            }
            string text45 = this._firstDC.N_BarrierFreePP.ToString();
            positionTemp = new XYZ(xSpace * 3.5, ySpace * 4, 0);// 创建文字的position指的是文字编辑栏的右上角
            note = null;
            using (Transaction tran = new Transaction(this._doc, "Creating a Text note"))
            {
                tran.Start();
                note = TextNote.Create(this._doc, this._doc.ActiveView.Id, positionTemp, text45, fangSong5.Id);
                tran.Commit();
            }
            if (note != null)
            {
                elementIds.Add(note.Id);
            }
            string text35 = this._secondDC.N_BarrierFreePP.ToString();
            positionTemp = new XYZ(xSpace * 3.5, ySpace * 3, 0);// 创建文字的position指的是文字编辑栏的右上角
            note = null;
            using (Transaction tran = new Transaction(this._doc, "Creating a Text note"))
            {
                tran.Start();
                note = TextNote.Create(this._doc, this._doc.ActiveView.Id, positionTemp, text35, fangSong5.Id);
                tran.Commit();
            }
            if (note != null)
            {
                elementIds.Add(note.Id);
            }
            string text25 = (this._firstDC.N_BarrierFreePP + this._secondDC.N_BarrierFreePP).ToString();
            positionTemp = new XYZ(xSpace * 3.5, ySpace * 2, 0);// 创建文字的position指的是文字编辑栏的右上角
            note = null;
            using (Transaction tran = new Transaction(this._doc, "Creating a Text note"))
            {
                tran.Start();
                note = TextNote.Create(this._doc, this._doc.ActiveView.Id, positionTemp, text25, fangSong5.Id);
                tran.Commit();
            }
            if (note != null)
            {
                elementIds.Add(note.Id);
            }
            // ==>
            string text56 = "普通车位";
            positionTemp = new XYZ(xSpace * 4.5, ySpace * 5, 0);// 创建文字的position指的是文字编辑栏的右上角
            note = null;
            using (Transaction tran = new Transaction(this._doc, "Creating a Text note"))
            {
                tran.Start();
                note = TextNote.Create(this._doc, this._doc.ActiveView.Id, positionTemp, text56, fangSong5.Id);
                tran.Commit();
            }
            if (note != null)
            {
                elementIds.Add(note.Id);
            }
            string text46 = this._firstDC.N_.ToString();
            positionTemp = new XYZ(xSpace * 4.5, ySpace * 4, 0);// 创建文字的position指的是文字编辑栏的右上角
            note = null;
            using (Transaction tran = new Transaction(this._doc, "Creating a Text note"))
            {
                tran.Start();
                note = TextNote.Create(this._doc, this._doc.ActiveView.Id, positionTemp, text46, fangSong5.Id);
                tran.Commit();
            }
            if (note != null)
            {
                elementIds.Add(note.Id);
            }
            string text36 = this._secondDC.N_.ToString();
            positionTemp = new XYZ(xSpace * 4.5, ySpace * 3, 0);// 创建文字的position指的是文字编辑栏的右上角
            note = null;
            using (Transaction tran = new Transaction(this._doc, "Creating a Text note"))
            {
                tran.Start();
                note = TextNote.Create(this._doc, this._doc.ActiveView.Id, positionTemp, text36, fangSong5.Id);
                tran.Commit();
            }
            if (note != null)
            {
                elementIds.Add(note.Id);
            }
            string text26 = (this._firstDC.N_ + this._secondDC.N_BarrierFreePP).ToString();
            positionTemp = new XYZ(xSpace * 4.5, ySpace * 2, 0);// 创建文字的position指的是文字编辑栏的右上角
            note = null;
            using (Transaction tran = new Transaction(this._doc, "Creating a Text note"))
            {
                tran.Start();
                note = TextNote.Create(this._doc, this._doc.ActiveView.Id, positionTemp, text26, fangSong5.Id);
                tran.Commit();
            }
            if (note != null)
            {
                elementIds.Add(note.Id);
            }
            // ==>
            string text57 = "合计";
            positionTemp = new XYZ(xSpace * 5.5, ySpace * 5, 0);// 创建文字的position指的是文字编辑栏的右上角
            note = null;
            using (Transaction tran = new Transaction(this._doc, "Creating a Text note"))
            {
                tran.Start();
                note = TextNote.Create(this._doc, this._doc.ActiveView.Id, positionTemp, text57, fangSong5.Id);
                tran.Commit();
            }
            if (note != null)
            {
                elementIds.Add(note.Id);
            }
            string text47 = this._firstDC.N_.ToString();
            positionTemp = new XYZ(xSpace * 5.5, ySpace * 4, 0);// 创建文字的position指的是文字编辑栏的右上角
            note = null;
            using (Transaction tran = new Transaction(this._doc, "Creating a Text note"))
            {
                tran.Start();
                note = TextNote.Create(this._doc, this._doc.ActiveView.Id, positionTemp, text47, fangSong5.Id);
                tran.Commit();
            }
            if (note != null)
            {
                elementIds.Add(note.Id);
            }
            string text37 = this._secondDC.N_.ToString();
            positionTemp = new XYZ(xSpace * 5.5, ySpace * 3, 0);// 创建文字的position指的是文字编辑栏的右上角
            note = null;
            using (Transaction tran = new Transaction(this._doc, "Creating a Text note"))
            {
                tran.Start();
                note = TextNote.Create(this._doc, this._doc.ActiveView.Id, positionTemp, text37, fangSong5.Id);
                tran.Commit();
            }
            if (note != null)
            {
                elementIds.Add(note.Id);
            }
            string text27 = (this._firstDC.N_ + this._secondDC.N_).ToString();
            positionTemp = new XYZ(xSpace * 5.5, ySpace * 2, 0);// 创建文字的position指的是文字编辑栏的右上角
            note = null;
            using (Transaction tran = new Transaction(this._doc, "Creating a Text note"))
            {
                tran.Start();
                note = TextNote.Create(this._doc, this._doc.ActiveView.Id, positionTemp, text27, fangSong5.Id);
                tran.Commit();
            }
            if (note != null)
            {
                elementIds.Add(note.Id);
            }

            // ==>

            // 成组 修改组名 移动
            Group group = null;
            using (Transaction creatNewGroup = new Transaction(this._doc, "创建新组"))
            {
                creatNewGroup.Start();
                group = this._doc.Create.NewGroup(elementIds);
                group.GroupType.Name = groupTypeName;
                ElementTransformUtils.MoveElement(this._doc, group.Id, new XYZ(0, ySpace * 7, 0));
                ElementTransformUtils.MoveElement(this._doc, group.Id,this._firstDC.Location);
                creatNewGroup.Commit();
            }

            return group;
        }
        /// <summary>
        /// 处理地下车库单车面积指标
        /// </summary>
        internal Group HandleSingleCarAreaIndexGroupType(string groupTypeName)
        {
            // 基于模板创建一个实例

            double xSpace = 98.425196850394;
            double ySpace = 22.9658792650882;

            double xLength = xSpace * 7.5;// 表格的横向长度
            double yLength = ySpace * 5;// 表格的高度
            double secondXLength = xSpace * 5.5;// 表格的长度
            double secondYLength = ySpace * 4;// 表格的高度

            // 表格分割线
            // 5根与x轴平行的线
            Line line01x = Line.CreateBound(new XYZ(0, 0, 0), new XYZ(xLength, 0, 0));
            double yPositionLine = ySpace;
            Line line02x = Line.CreateBound(new XYZ(0, yPositionLine, 0), new XYZ(secondXLength, yPositionLine, 0));
            yPositionLine += ySpace;
            Line line03x = Line.CreateBound(new XYZ(0, yPositionLine, 0), new XYZ(secondXLength, yPositionLine, 0));
            yPositionLine += ySpace;
            Line line04x = Line.CreateBound(new XYZ(0, yPositionLine, 0), new XYZ(xLength, yPositionLine, 0));
            yPositionLine += ySpace;
            Line line05x = Line.CreateBound(new XYZ(0, yPositionLine, 0), new XYZ(xLength, yPositionLine, 0));
            yPositionLine += ySpace;
            Line line06x = Line.CreateBound(new XYZ(0, yPositionLine, 0), new XYZ(xLength, yPositionLine, 0));
            // 7根与y轴平行的线 由于标题的原因 最左最右两根线段比中间的长
            Line line01y = Line.CreateBound(new XYZ(0, 0, 0), new XYZ(0, yLength, 0));
            double xPositionLine = xSpace * 0.5;
            Line line02y = Line.CreateBound(new XYZ(xPositionLine, 0, 0), new XYZ(xPositionLine, secondYLength, 0));
            xPositionLine += xSpace;
            Line line03y = Line.CreateBound(new XYZ(xPositionLine, 0, 0), new XYZ(xPositionLine, secondYLength, 0));
            xPositionLine += xSpace;
            Line line04y = Line.CreateBound(new XYZ(xPositionLine, 0, 0), new XYZ(xPositionLine, secondYLength, 0));
            xPositionLine += xSpace;
            Line line05y = Line.CreateBound(new XYZ(xPositionLine, 0, 0), new XYZ(xPositionLine, secondYLength, 0));
            xPositionLine += xSpace;
            Line line06y = Line.CreateBound(new XYZ(xPositionLine, 0, 0), new XYZ(xPositionLine, secondYLength, 0));
            xPositionLine += xSpace;
            Line line07y = Line.CreateBound(new XYZ(xPositionLine, 0, 0), new XYZ(xPositionLine, secondYLength, 0));
            xPositionLine += xSpace;
            Line line08y = Line.CreateBound(new XYZ(xPositionLine, 0, 0), new XYZ(xPositionLine, secondYLength, 0));
            xPositionLine += xSpace;
            Line line09y = Line.CreateBound(new XYZ(xPositionLine, 0, 0), new XYZ(xPositionLine, yLength, 0));

            List<Line> lines = new List<Line>() { line01x, line02x, line03x, line04x, line05x, line06x, line01y, line02y, line03y, line04y, line05y, line06y, line07y, line08y, line09y };

            List<ElementId> elementIds = new List<ElementId>();
            DetailCurveArray detailCurveArray = new DetailCurveArray();
            using (Transaction transaction = new Transaction(this._doc, "创建表格定位线"))
            {
                transaction.Start();
                detailCurveArray = this._doc.Create.NewDetailCurveArray(this._doc.ActiveView, lines.ToCurveArray());
                transaction.Commit();
            }
            foreach (DetailCurve detailCurve in detailCurveArray)
            {
                elementIds.Add(detailCurve.Id);
            }

            // 文字
            // 判断 仿宋_5.0mm 是否存在
            TextNoteType fangSong5 = new FilteredElementCollector(this._doc).OfClass(typeof(TextNoteType)).Where(p => p.Name.Contains("仿宋_5.0")).FirstOrDefault() as TextNoteType;

            if (fangSong5 == null)
            {
                // 文字类型确认
                ElementId defaultTypeId = this._doc.GetDefaultElementTypeId(ElementTypeGroup.TextNoteType);// 默认文字类型为 仿宋_3.5mm
                TextNoteType textNoteType = this._doc.GetElement(defaultTypeId) as TextNoteType;
                // 修改文字大小
                using (Transaction creatNewTextNoteType = new Transaction(this._doc, "设置目标文字类型"))
                {
                    creatNewTextNoteType.Start();
                    fangSong5 = textNoteType.Duplicate("仿宋_5.0mm") as TextNoteType;
                    fangSong5.get_Parameter(BuiltInParameter.TEXT_SIZE).SetValue(5.0.MilliMeterToFeet());
                    creatNewTextNoteType.Commit();
                }
            }
            // ==>
            string text51 = "地下室单车面积指标";
            XYZ positionTemp = new XYZ(xLength / 2, yLength, 0);// 创建文字的position指的是文字编辑栏的右上角
            TextNote note = null;
            using (Transaction tran = new Transaction(this._doc, "Creating a Text note"))
            {
                tran.Start();
                note = TextNote.Create(this._doc, this._doc.ActiveView.Id, positionTemp, text51, fangSong5.Id);
                tran.Commit();
            }
            if (note != null)
            {
                elementIds.Add(note.Id);
            }
           
            // ==>
            string text11 = "合计";

            positionTemp = new XYZ(0.0, ySpace * 1, 0);// 创建文字的position指的是文字编辑栏的右上角
            note = null;
            using (Transaction tran = new Transaction(this._doc, "Creating a Text note"))
            {
                tran.Start();
                note = TextNote.Create(this._doc, this._doc.ActiveView.Id, positionTemp, text11, fangSong5.Id);
                tran.Commit();
            }
            if (note != null)
            {
                elementIds.Add(note.Id);
            }
            // ==>

            string text21 = "地下二层";
            positionTemp = new XYZ(0.0, ySpace * 2, 0);// 创建文字的position指的是文字编辑栏的右上角
            note = null;
            using (Transaction tran = new Transaction(this._doc, "Creating a Text note"))
            {
                tran.Start();
                note = TextNote.Create(this._doc, this._doc.ActiveView.Id, positionTemp, text21, fangSong5.Id);
                tran.Commit();
            }
            if (note != null)
            {
                elementIds.Add(note.Id);
            }
            // ==>
            string text31 = "地下一层";
            positionTemp = new XYZ(0.0, ySpace * 3, 0);// 创建文字的position指的是文字编辑栏的右上角
            note = null;
            using (Transaction tran = new Transaction(this._doc, "Creating a Text note"))
            {
                tran.Start();
                note = TextNote.Create(this._doc, this._doc.ActiveView.Id, positionTemp, text31, fangSong5.Id);
                tran.Commit();
            }
            if (note != null)
            {
                elementIds.Add(note.Id);
            }

            // ==>
            string text42 = "面积";
            positionTemp = new XYZ(xSpace * 0.5, ySpace * 4, 0);// 创建文字的position指的是文字编辑栏的右上角
            note = null;
            using (Transaction tran = new Transaction(this._doc, "Creating a Text note"))
            {
                tran.Start();
                note = TextNote.Create(this._doc, this._doc.ActiveView.Id, positionTemp, text42, fangSong5.Id);
                tran.Commit();
            }
            if (note != null)
            {
                elementIds.Add(note.Id);
            }

            string text32 = this._firstDC.S_.SQUARE_FEETtoSQUARE_METERS().NumDecimal(2).ToString();
            positionTemp = new XYZ(xSpace * 0.5, ySpace * 3, 0);// 创建文字的position指的是文字编辑栏的右上角
            note = null;
            using (Transaction tran = new Transaction(this._doc, "Creating a Text note"))
            {
                tran.Start();
                note = TextNote.Create(this._doc, this._doc.ActiveView.Id, positionTemp, text32, fangSong5.Id);
                tran.Commit();
            }
            if (note != null)
            {
                elementIds.Add(note.Id);
            }
            string text22 = this._secondDC.S_.SQUARE_FEETtoSQUARE_METERS().NumDecimal(2).ToString();
            positionTemp = new XYZ(xSpace * 0.5, ySpace * 2, 0);// 创建文字的position指的是文字编辑栏的右上角
            note = null;
            using (Transaction tran = new Transaction(this._doc, "Creating a Text note"))
            {
                tran.Start();
                note = TextNote.Create(this._doc, this._doc.ActiveView.Id, positionTemp, text22, fangSong5.Id);
                tran.Commit();
            }
            if (note != null)
            {
                elementIds.Add(note.Id);
            }
            string text12 = (this._secondDC.S_ + this._secondDC.S_).SQUARE_FEETtoSQUARE_METERS().NumDecimal(2).ToString();
            positionTemp = new XYZ(xSpace * 0.5, ySpace * 1, 0);// 创建文字的position指的是文字编辑栏的右上角
            note = null;
            using (Transaction tran = new Transaction(this._doc, "Creating a Text note"))
            {
                tran.Start();
                note = TextNote.Create(this._doc, this._doc.ActiveView.Id, positionTemp, text12, fangSong5.Id);
                tran.Commit();
            }
            if (note != null)
            {
                elementIds.Add(note.Id);
            }

            // ==>
            string text43 = "叠墅赠送面积";
            positionTemp = new XYZ(xSpace * 1.5, ySpace * 4, 0);// 创建文字的position指的是文字编辑栏的右上角
            note = null;
            using (Transaction tran = new Transaction(this._doc, "Creating a Text note"))
            {
                tran.Start();
                note = TextNote.Create(this._doc, this._doc.ActiveView.Id, positionTemp, text43, fangSong5.Id);
                tran.Commit();
            }
            if (note != null)
            {
                elementIds.Add(note.Id);
            }



            // ==>
            string text44 = "大型机房面积";
            positionTemp = new XYZ(xSpace * 2.5, ySpace * 4, 0);// 创建文字的position指的是文字编辑栏的右上角
            note = null;
            using (Transaction tran = new Transaction(this._doc, "Creating a Text note"))
            {
                tran.Start();
                note = TextNote.Create(this._doc, this._doc.ActiveView.Id, positionTemp, text44, fangSong5.Id);
                tran.Commit();
            }
            if (note != null)
            {
                elementIds.Add(note.Id);
            }


            string text34 = this._firstDC.S3_.SQUARE_FEETtoSQUARE_METERS().NumDecimal(2).ToString();
            positionTemp = new XYZ(xSpace * 2.5, ySpace * 3, 0);// 创建文字的position指的是文字编辑栏的右上角
            note = null;
            using (Transaction tran = new Transaction(this._doc, "Creating a Text note"))
            {
                tran.Start();
                note = TextNote.Create(this._doc, this._doc.ActiveView.Id, positionTemp, text34, fangSong5.Id);
                tran.Commit();
            }
            if (note != null)
            {
                elementIds.Add(note.Id);
            }
            string text24 = this._secondDC.S3_.SQUARE_FEETtoSQUARE_METERS().NumDecimal(2).ToString();
            positionTemp = new XYZ(xSpace * 2.5, ySpace * 2, 0);// 创建文字的position指的是文字编辑栏的右上角
            note = null;
            using (Transaction tran = new Transaction(this._doc, "Creating a Text note"))
            {
                tran.Start();
                note = TextNote.Create(this._doc, this._doc.ActiveView.Id, positionTemp, text24, fangSong5.Id);
                tran.Commit();
            }
            if (note != null)
            {
                elementIds.Add(note.Id);
            }
            string text14 = (this._secondDC.S3_.SQUARE_FEETtoSQUARE_METERS() + this._secondDC.S_.SQUARE_FEETtoSQUARE_METERS()).NumDecimal(2).ToString();
            positionTemp = new XYZ(xSpace * 2.5, ySpace * 1, 0);// 创建文字的position指的是文字编辑栏的右上角
            note = null;
            using (Transaction tran = new Transaction(this._doc, "Creating a Text note"))
            {
                tran.Start();
                note = TextNote.Create(this._doc, this._doc.ActiveView.Id, positionTemp, text14, fangSong5.Id);
                tran.Commit();
            }
            if (note != null)
            {
                elementIds.Add(note.Id);
            }

            // ==>
            string text45 = "高层投影面积";
            positionTemp = new XYZ(xSpace * 3.5, ySpace * 4, 0);// 创建文字的position指的是文字编辑栏的右上角
            note = null;
            using (Transaction tran = new Transaction(this._doc, "Creating a Text note"))
            {
                tran.Start();
                note = TextNote.Create(this._doc, this._doc.ActiveView.Id, positionTemp, text45, fangSong5.Id);
                tran.Commit();
            }
            if (note != null)
            {
                elementIds.Add(note.Id);
            }

            string text35 = this._firstDC.S0_.SQUARE_FEETtoSQUARE_METERS().NumDecimal(2).ToString();
            positionTemp = new XYZ(xSpace * 3.5, ySpace * 3, 0);// 创建文字的position指的是文字编辑栏的右上角
            note = null;
            using (Transaction tran = new Transaction(this._doc, "Creating a Text note"))
            {
                tran.Start();
                note = TextNote.Create(this._doc, this._doc.ActiveView.Id, positionTemp, text35, fangSong5.Id);
                tran.Commit();
            }
            if (note != null)
            {
                elementIds.Add(note.Id);
            }
            string text25 = this._secondDC.S0_.SQUARE_FEETtoSQUARE_METERS().NumDecimal(2).ToString();
            positionTemp = new XYZ(xSpace * 3.5, ySpace * 2, 0);// 创建文字的position指的是文字编辑栏的右上角
            note = null;
            using (Transaction tran = new Transaction(this._doc, "Creating a Text note"))
            {
                tran.Start();
                note = TextNote.Create(this._doc, this._doc.ActiveView.Id, positionTemp, text25, fangSong5.Id);
                tran.Commit();
            }
            if (note != null)
            {
                elementIds.Add(note.Id);
            }
            string text15 = (this._secondDC.S0_.SQUARE_FEETtoSQUARE_METERS() + this._secondDC.S0_.SQUARE_FEETtoSQUARE_METERS()).NumDecimal(2).ToString();
            positionTemp = new XYZ(xSpace * 3.5, ySpace * 1, 0);// 创建文字的position指的是文字编辑栏的右上角
            note = null;
            using (Transaction tran = new Transaction(this._doc, "Creating a Text note"))
            {
                tran.Start();
                note = TextNote.Create(this._doc, this._doc.ActiveView.Id, positionTemp, text15, fangSong5.Id);
                tran.Commit();
            }
            if (note != null)
            {
                elementIds.Add(note.Id);
            }

            // ==>
            string text46 = "机动车位数";
            positionTemp = new XYZ(xSpace * 4.5, ySpace * 4, 0);// 创建文字的position指的是文字编辑栏的右上角
            note = null;
            using (Transaction tran = new Transaction(this._doc, "Creating a Text note"))
            {
                tran.Start();
                note = TextNote.Create(this._doc, this._doc.ActiveView.Id, positionTemp, text46, fangSong5.Id);
                tran.Commit();
            }
            if (note != null)
            {
                elementIds.Add(note.Id);
            }

            string text36 = this._firstDC.N_.NumDecimal(2).ToString();
            positionTemp = new XYZ(xSpace * 4.5, ySpace * 3, 0);// 创建文字的position指的是文字编辑栏的右上角
            note = null;
            using (Transaction tran = new Transaction(this._doc, "Creating a Text note"))
            {
                tran.Start();
                note = TextNote.Create(this._doc, this._doc.ActiveView.Id, positionTemp, text36, fangSong5.Id);
                tran.Commit();
            }
            if (note != null)
            {
                elementIds.Add(note.Id);
            }

            string text26 = this._secondDC.N_.NumDecimal(2).ToString();
            positionTemp = new XYZ(xSpace * 4.5, ySpace * 2, 0);// 创建文字的position指的是文字编辑栏的右上角
            note = null;
            using (Transaction tran = new Transaction(this._doc, "Creating a Text note"))
            {
                tran.Start();
                note = TextNote.Create(this._doc, this._doc.ActiveView.Id, positionTemp, text26, fangSong5.Id);
                tran.Commit();
            }
            if (note != null)
            {
                elementIds.Add(note.Id);
            }

            string text16 = (this._firstDC.N_+ this._secondDC.N_).NumDecimal(2).ToString();
            positionTemp = new XYZ(xSpace * 4.5, ySpace * 1, 0);// 创建文字的position指的是文字编辑栏的右上角
            note = null;
            using (Transaction tran = new Transaction(this._doc, "Creating a Text note"))
            {
                tran.Start();
                note = TextNote.Create(this._doc, this._doc.ActiveView.Id, positionTemp, text16, fangSong5.Id);
                tran.Commit();
            }
            if (note != null)
            {
                elementIds.Add(note.Id);
            }

            // ==>
            string text47 = "车地比（净）";
            positionTemp = new XYZ(xSpace * 5.5, ySpace *4, 0);// 创建文字的position指的是文字编辑栏的右上角
            note = null;
            using (Transaction tran = new Transaction(this._doc, "Creating a Text note"))
            {
                tran.Start();
                note = TextNote.Create(this._doc, this._doc.ActiveView.Id, positionTemp, text47, fangSong5.Id);
                tran.Commit();
            }
            if (note != null)
            {
                elementIds.Add(note.Id);
            }
            string text27 = ((this._firstDC.S1_.SQUARE_FEETtoSQUARE_METERS() + this._secondDC.S1_.SQUARE_FEETtoSQUARE_METERS()) / (this._firstDC.N_ + this._secondDC.N_)).NumDecimal(2).ToString();
            positionTemp = new XYZ(xSpace * 5.5, ySpace * 2, 0);// 创建文字的position指的是文字编辑栏的右上角
            note = null;
            using (Transaction tran = new Transaction(this._doc, "Creating a Text note"))
            {
                tran.Start();
                note = TextNote.Create(this._doc, this._doc.ActiveView.Id, positionTemp, text27, fangSong5.Id);
                tran.Commit();
            }
            if (note != null)
            {
                elementIds.Add(note.Id);
            }
            // ==>
            string text48 = "车地比（毛）";
            positionTemp = new XYZ(xSpace * 6.5, ySpace * 4, 0);// 创建文字的position指的是文字编辑栏的右上角
            note = null;
            using (Transaction tran = new Transaction(this._doc, "Creating a Text note"))
            {
                tran.Start();
                note = TextNote.Create(this._doc, this._doc.ActiveView.Id, positionTemp, text48, fangSong5.Id);
                tran.Commit();
            }
            if (note != null)
            {
                elementIds.Add(note.Id);
            }

            string text28 = ((this._firstDC.S_.SQUARE_FEETtoSQUARE_METERS() + this._secondDC.S_.SQUARE_FEETtoSQUARE_METERS()) / (this._firstDC.N_ + this._secondDC.N_)).NumDecimal(2).ToString();
            positionTemp = new XYZ(xSpace * 6.5, ySpace * 2, 0);// 创建文字的position指的是文字编辑栏的右上角
            note = null;
            using (Transaction tran = new Transaction(this._doc, "Creating a Text note"))
            {
                tran.Start();
                note = TextNote.Create(this._doc, this._doc.ActiveView.Id, positionTemp, text28, fangSong5.Id);
                tran.Commit();
            }
            if (note != null)
            {
                elementIds.Add(note.Id);
            }
            // ==>

            // 成组 修改组名 移动
            Group group = null;
            using (Transaction creatNewGroup = new Transaction(this._doc, "创建新组"))
            {
                creatNewGroup.Start();
                group = this._doc.Create.NewGroup(elementIds);
                group.GroupType.Name = groupTypeName;
                ElementTransformUtils.MoveElement(this._doc, group.Id, this._firstDC.Location);
                creatNewGroup.Commit();
            }
            return group;
        }

    }
}
