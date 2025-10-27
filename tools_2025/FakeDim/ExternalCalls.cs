using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
//using goa.Common;
//using goa.Common.Exceptions;

namespace FAKE_DIMS
{
    public static class ExternalCalls
    {
        /// <summary>
        /// throw if not found.
        /// </summary>
        /// <param name="doc"></param>
        /// <returns></returns>
        public static Family FindFamilyInDoc(Document doc)
        {
            return MainWindowRequestHandler.findFamilyInDoc(doc);
        }
        /// <summary>
        /// TRANSACTIONS INSIDE.
        /// </summary>
        /// <param name="_view"></param>
        /// <param name="_dimCtrls"></param>
        public static void ConvertDims(View _view, Family _fa, IEnumerable<DimCtrl> _dimCtrls)
        {
            MainWindowRequestHandler.convertDims(_view, _fa, _dimCtrls);
        }
    }
}
