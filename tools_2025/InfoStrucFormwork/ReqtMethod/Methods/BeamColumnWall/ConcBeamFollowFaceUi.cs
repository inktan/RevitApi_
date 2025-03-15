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
    internal class ConcBeamFollowFaceUi : RequestMethod
    {
        internal ConcBeamFollowFaceUi(UIApplication _uiApp) : base(_uiApp)
        {
        }

        internal override void Execute()
        {
            PickCad pickCad = new PickCad(uiApp);
            pickCad.Execute();

            EleCreatInfo eleInfo;
            List<ElementId> elementIds = new List<ElementId>();
            // 混凝土-梁
            eleInfo = new ConBeamFollowFace
            {
                DwgParser = pickCad.DwgParser
            };
            eleInfo.Execute();
            if (eleInfo.ElementIds != null)
            {
                elementIds.AddRange(eleInfo.ElementIds);
            }

            //throw new NotImplementedException();

            this.sw.Stop();
            //$"{sw.ElapsedMilliseconds} + 毫秒".TaskDialogErrorMessage();
        }
    }
}
