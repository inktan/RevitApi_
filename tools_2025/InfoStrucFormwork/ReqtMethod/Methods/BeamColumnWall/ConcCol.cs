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
    internal class ConcCol : RequestMethod
    {
        internal ConcCol(UIApplication _uiApp) : base(_uiApp)
        {
        }

        internal override void Execute()
        {
            PickCad pickCad = new PickCad(uiApp);
            pickCad.Execute();

            EleCreatInfo eleInfo;
            List<ElementId> elementIds = new List<ElementId>();
            // 柱子 (使用线圈)
            eleInfo = new StrucGenlProfileCol();
            eleInfo.DwgParser = pickCad.DwgParser;
            eleInfo.Execute();// 将不可识别的竖向构件图形，处理为通用 结构柱_轮廓 轮廓族
            if (eleInfo.ElementIds != null)
            {
                elementIds.AddRange(eleInfo.ElementIds);
            }
            //// 生成柱子 圆形
            eleInfo = new StrucCirColumn();
            eleInfo.DwgParser = pickCad.DwgParser;
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
