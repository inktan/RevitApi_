
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PubFuncWt;

namespace BSMT_PpLayout
{

    class AddSequenceNum : RequestMethod
    {
        public AddSequenceNum(UIApplication uiapp) : base(uiapp)
        {
        }


        internal override void Execute()
        {

            this.doc.DeleteInvalidDetailGroupTypes();

            InitialUIinter initialUIinter = new InitialUIinter(this.uiApp);
            List<ElementId> selBaseWallIds = initialUIinter.SelBsmtWallIds(); // UI获取所有地库填充区域id
            List<ElemsViewLevel> elemsViewLevels = initialUIinter.ElemsViewLevels(); //一个视图对应一个层级

            TextNoteType textNoteType = (new FilteredElementCollector(doc)).OfClass(typeof(TextNoteType)).WhereElementIsElementType().ToElements().FirstOrDefault(p => p.Name == "序号文字大小") as TextNoteType;
            if (textNoteType == null)
            {
                using (Transaction trans = new Transaction(this.doc, "创建详图线组"))
                {
                    trans.Start();
                    textNoteType = (new FilteredElementCollector(doc)).OfClass(typeof(TextNoteType)).WhereElementIsElementType().ToElements().FirstOrDefault() as TextNoteType;
                    textNoteType = textNoteType.Duplicate("序号文字大小") as TextNoteType;
                    textNoteType.get_Parameter(BuiltInParameter.TEXT_SIZE).Set(GlobalData.Instance .NumTextSize_num);
                    trans.Commit();
                }
            }
            else
            {
                using (Transaction trans = new Transaction(this.doc, "创建详图线组"))
                {
                    trans.Start();
                    textNoteType.get_Parameter(BuiltInParameter.TEXT_SIZE).Set(GlobalData.Instance.NumTextSize_num);
                    trans.Commit();
                }
            }


            foreach (ElemsViewLevel elemsViewLevel in elemsViewLevels)// 遍历单个视图
            {
                View nowView = elemsViewLevel.View;

                foreach (Bsmt bsmt in elemsViewLevel.Bsmts)// 遍历单个地库
                {
                    if (!selBaseWallIds.Contains(bsmt.BsmtBound.Id))// 判断是否UI选择
                        continue;

                    bsmt.Computer_Ps_Fr_Col_SubExit_Area();
                    List<RevitElePS> parkingUnitExits = bsmt.InBoundElePses.OrderBy(p => p.LocVector2d.x).OrderByDescending(p => p.LocVector2d.y).ToList();
                    int allCount = parkingUnitExits.Count;

                    GraphicsStyle graphicsStyle = this.doc.GetGraphicsStyleByName("细线");

                    CurveArray curveArray = new CurveArray();

                    using (Transaction trans = new Transaction(doc, "creatText"))
                    {
                        trans.Start();
                        List<ElementId> eleIds = new List<ElementId>();
                        for (int i = 0; i < allCount; i++)
                        {
                            string str = (i + 1).ToString();
                            XYZ xYZlocation = parkingUnitExits[i].LocVector2d.ToXYZ();

                            xYZlocation += parkingUnitExits[i].FamilyInstance.FacingOrientation * GlobalData.Instance.pSHeight_num / 2;

                            Arc circle = Arc.Create(xYZlocation, GlobalData.Instance.NumCircleRadiu_num, 0, Math.PI * 2, XYZ.BasisX, XYZ.BasisY);// 序号圈圈半径
                            curveArray.Append(circle);

                            TextNote textNode = TextNote.Create(doc, nowView.Id, xYZlocation + new XYZ(-2, 2, 0), str, textNoteType.Id);
                            eleIds.Add(textNode.Id);

                        }

                        doc.Create.NewGroup(eleIds);

                        trans.Commit();
                    }

                    if (GlobalData.Instance.AddCircle)
                    {
                        DetailCurveArray detailCurveArray = this.doc.DrawDetailCurvesWithNewTrans(curveArray, graphicsStyle);

                        List<ElementId> detailCurveIds = new List<ElementId>();
                        foreach (DetailCurve detailCurve in detailCurveArray)
                        {
                            detailCurveIds.Add(detailCurve.Id);
                        }
                        using (Transaction trans = new Transaction(this.doc, "创建详图线组"))
                        {
                            trans.Start();
                            doc.Create.NewGroup(detailCurveIds);
                            trans.Commit();
                        }
                    }

                    //throw new NotImplementedException();
                }
            }
        }
    }
}
