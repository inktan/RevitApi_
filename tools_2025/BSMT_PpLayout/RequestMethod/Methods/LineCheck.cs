using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.ExtensibleStorage;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using g3;
using goa.Common;
using goa.Common.g3InterOp;
using PubFuncWt;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSMT_PpLayout
{
    internal class LineCheck : RequestMethod
    {
        internal LineCheck(UIApplication uiapp) : base(uiapp)
        {

        }
        internal override void Execute()
        {
            InitialUIinter initialUIinter = new InitialUIinter(this.uiApp);
            List<ElementId> selBaseWallIds = initialUIinter.SelBsmtWallIds(); // UI获取所有地库填充区域id
            List<ElemsViewLevel> elemsViewLevels = initialUIinter.ElemsViewLevels(); //一个视图对应一个层级

            foreach (ElemsViewLevel elemsViewLevel in elemsViewLevels)// 遍历单个视图
            {
                View nowView = elemsViewLevel.View;

                foreach (Bsmt bsmt in elemsViewLevel.Bsmts)// 遍历单个地库
                {
                    if (!selBaseWallIds.Contains(bsmt.BsmtBound.Id))// 判断是否UI选择
                        continue;

                    List<RevitEleCurve> revitEleLines = bsmt.GetLines();

                    int count = revitEleLines.Count;
                    for (int i = 0; i < count; i++)
                    {
                        for (int j = 0; j < count; j++)
                        {
                            if (j > i)
                            {

                                Curve curve01 = revitEleLines[i].Curve;
                                Curve curve02 = revitEleLines[j].Curve;

                                XYZ p01 = curve02.GetEndPoint(0);
                                XYZ p02 = curve02.GetEndPoint(1);

                                XYZ p11 = curve01.GetEndPoint(0);
                                XYZ p12 = curve01.GetEndPoint(1);

                                double dis01 = curve01.Distance(p01);
                                double dis02 = curve01.Distance(p02);

                                double dis03 = curve02.Distance(p11);
                                double dis04 = curve02.Distance(p12);


                                double buffer01 = 5.0.MilliMeterToFeet();
                                double buffer02 = GlobalData.Instance.lineCheckDis_num;

                                // 线头相交问题
                                if ((dis01 > buffer01 && dis01 < buffer02) ||
                                    (dis02 > buffer01 && dis02 < buffer02) ||
                                    (dis03 > buffer01 && dis03 < buffer02) ||
                                    (dis04 > buffer01 && dis04 < buffer02))
                                {
                                    using (Transaction trans = new Transaction(this.doc))
                                    {
                                        trans.Start("检查线头");

                                        this.view.IsolateElementsTemporary(new List<ElementId>() { revitEleLines[i].Id, revitEleLines[j].Id });
                                        trans.Commit();
                                    }
                                }

                            }
                        }
                    }

                }
            }
            // throw new NotImplementedException();
        }
    }
}
