using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PubFuncWt;
using Autodesk.Revit.UI.Selection;
using System.Reflection;
using System.IO;
using TeighaNet;
using System.Diagnostics;
using Autodesk.Revit.DB.Structure;

using goa.Common;

namespace InfoStrucFormwork
{
    internal class StoreySteelBeamSingleLine : RequestMethod
    {
        internal StoreySteelBeamSingleLine(UIApplication _uiApp) : base(_uiApp)
        {

        }

        internal override void Execute()
        {
            PickCad pickCad = new PickCad(uiApp);
            pickCad.Execute();

            EleCreatInfo eleInfo;
            List<ElementId> elementIds = new List<ElementId>();

            // 梁-钢
            eleInfo = new SteelBeamSingleLine();
            eleInfo.DwgParser = pickCad.DwgParser;
            eleInfo.Execute();
            elementIds.AddRange(eleInfo.ElementIds);

            if (elementIds.Count > 0)
            {
                //using (Transaction trans = new Transaction(this.doc, "创建组"))
                //{
                //    trans.Start();
                //    Group group = this.doc.Create.NewGroup(elementIds);
                //    trans.Commit();
                //}
            }
            else
            {
                //"未生成任何模型构件".TaskDialogErrorMessage();
            }


            //throw new NotImplementedException();

            this.sw.Stop();
            //$"{sw.ElapsedMilliseconds} + 毫秒".TaskDialogErrorMessage();
        }
    }
}
