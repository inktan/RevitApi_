using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PubFuncWt;

namespace InfoStrucFormwork
{
    internal class AllowJoin : RequestMethod
    {
        public AllowJoin(UIApplication _uiApp) : base(_uiApp)
        {

        }

        internal override void Execute()
        {

            ElementClassFilter filter = new ElementClassFilter(typeof(FamilyInstance));
            FilteredElementCollector collector = new FilteredElementCollector(this.doc);

            foreach (var item in collector.WherePasses(filter).ToElements())
            {
                if (!(item is FamilyInstance fi)) continue;

                string faName = fi.Symbol.FamilyName;
                if (faName.Contains("梁"))
                {
                    //这里使用共享参数 结构-梁-梁底相对偏移
                    using (Transaction _trans = new Transaction(this.doc, "修改共享参数"))
                    {
                        _trans.Start();
                        _trans.DeleteErrOrWaringTaskDialog();
                        try
                        {

                            StructuralFramingUtils.AllowJoinAtEnd(fi, 0);
                            StructuralFramingUtils.AllowJoinAtEnd(fi, 1);
                        }
                        catch (Exception)
                        {
                            continue;
                            //throw;
                        }
                        _trans.Commit();
                    }

                    //throw new NotImplementedException();
                }
            }
        }
    }
}
