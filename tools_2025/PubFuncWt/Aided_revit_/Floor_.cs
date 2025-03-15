using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PubFuncWt
{
    public static class Floor_
    {
    

        public static List<FloorType> FloorTypes(this Document doc)
        {
            return (new FilteredElementCollector(doc))
                .OfCategory(BuiltInCategory.OST_Floors)
                .OfClass(typeof(FloorType))
                .Cast<FloorType>()
                .ToList();
        }

    }
}
