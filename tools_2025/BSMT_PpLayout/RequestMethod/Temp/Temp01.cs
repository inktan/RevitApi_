
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PubFuncWt;
using g3;
using System.Collections.Concurrent;
using goa.Common.g3InterOp;
using Autodesk.Revit.DB.ExternalService;
using goa.Revit.DirectContext3D;
using Autodesk.Revit.DB.ExtensibleStorage;
using goa.Common;

namespace BSMT_PpLayout
{
    class Temp01 : RequestMethod
    {
        public Temp01(UIApplication uiapp) : base(uiapp)
        {


        }

        internal void _Execute()
        {
            FilteredElementCollector parkingSymbols = new FilteredElementCollector(doc).OfClass(typeof(FamilySymbol));
            FamilySymbol fs = null;
            foreach (Element element in parkingSymbols)
            {
                if (element is FamilySymbol)
                {
                    if (element.Name == "柱子_")
                    {
                        FamilySymbol familySymbol = element as FamilySymbol;

                        familySymbol.ToString().TaskDialogErrorMessage();
                        //if (familySymbol.FamilyName == faName)
                        //{
                        //    fs = element as FamilySymbol;
                        //    break;
                        //}
                    }
                }
            }

        }
        internal override void Execute()
        {
            FilteredElementCollector parkingSymbols = new FilteredElementCollector(doc).OfClass(typeof(Family));
            Family fs = null;
            foreach (Element element in parkingSymbols)
            {
                if (element is Family)
                {
                    if (element.Name == "柱子_")
                    {
                        Family familySymbol = element as Family;

                        familySymbol.GetFamilySymbolIds().Count.ToString().TaskDialogErrorMessage();
                        //if (familySymbol.FamilyName == faName)
                        //{
                        //    fs = element as FamilySymbol;
                        //    break;
                        //}
                    }
                }
            }

        }
    }
}
