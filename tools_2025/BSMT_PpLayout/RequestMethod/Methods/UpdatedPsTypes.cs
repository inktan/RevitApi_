using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace BSMT_PpLayout
{
    /// <summary>
    /// 该功能用于批量替换停车位族 
    /// </summary>
    class UpdatedPsTypes : RequestMethod
    {
        internal UpdatedPsTypes(UIApplication uiapp) : base(uiapp) { }

        internal override void Execute()
        {
            GlobalData.psTypeNames.Clear();

            List<string> names = (new FilteredElementCollector(this.doc))
                 .OfCategory(BuiltInCategory.OST_DetailComponents)
                 .OfClass(typeof(FamilySymbol))
                 .WhereElementIsElementType()
                 .Where(p => p.Name.Contains("位_"))
                 .Select(p => p.Name).ToList();

            names.Sort();

            /*
             * 源于设计师手动添加停车位类型后，界面列表不能自动更改的情况——解决方案，采用documentchange事件，进行监控
             */
            foreach (var item in names)
            {
                string[] strArr = item.Split('_');
                if (strArr.Length == 2)
                {
                    if (strArr.Last() != "")
                    {
                        GlobalData.psTypeNames.Add(item);
                    }
                }
            }

        }
    }
}
