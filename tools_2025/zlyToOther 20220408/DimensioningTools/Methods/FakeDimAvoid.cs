using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;

using goa.Common;
using PubFuncWt;

namespace DimensioningTools
{
    internal static partial class Methods
    {
        internal static void FakeDimAvoid()
        {
            UIDocument activeUIDocument = goa.Common.APP.UIApp.ActiveUIDocument;
            Document document = activeUIDocument.Document;
            Parameter viewScale = GetViewScale(activeUIDocument.ActiveView);
            double num = Convert.ToDouble(viewScale.AsInteger()) / 100;
            using (TransactionGroup tg = new TransactionGroup(document, "尺寸避让"))
            {
                tg.Start();

                while (true)
                {
                    try
                    {
                        List<FamilyInstance> dimensions = activeUIDocument.Selection.PickElementsByRectangle(new SelFakeDimdnsion()).Cast<FamilyInstance>().ToList();

                        int size = dimensions.Count;
                        for (int i = 0; i <= size - 2; i++)
                        {
                            XYZ xYZ01 = dimensions[i].LocationCurve().GetCenter();
                            XYZ xYZ02 = dimensions[i + 1].LocationCurve().GetCenter();

                            double dis = xYZ01.DistanceTo(xYZ02);
                            if (dis < 1.640419948 * num)
                            {
                                using (Transaction transaction1 = new Transaction(document))
                                {
                                    transaction1.Start("自动避让线性标注.");
                                    Parameter parameter = dimensions[i + 1].LookupParameter("文字纵向距离");
                                    parameter.Set(parameter.AsDouble() + 400.0.MilliMeterToFeet());
                                    transaction1.Commit();
                                }
                                i++;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        if (ex is Autodesk.Revit.Exceptions.OperationCanceledException)//用户取消异常，不抛出异常信息
                        {
                            tg.Assimilate();
                            break;
                        }
                    }
                }
            }
        }

    }
    public class SelFakeDimdnsion : ISelectionFilter
    {
        public bool AllowElement(Element elem)
        {
            if (elem is FamilyInstance)
            {
                //if ((elem as FamilyInstance).Symbol.FamilyName.Contains("尺寸标注-毫米"))
                if ((elem as FamilyInstance).Symbol.FamilyName.Contains("尺寸标注-毫米"))
                    {
                    return true;
                }
            }
            return false;
        }

        public bool AllowReference(Reference reference, XYZ position)
        {
            throw new NotImplementedException();
        }
    }
}
