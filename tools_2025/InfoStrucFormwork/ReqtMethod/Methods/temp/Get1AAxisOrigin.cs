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
using System.Text.RegularExpressions;

namespace InfoStrucFormwork
{
    internal class Get1AAxisOrigin : RequestMethod
    {
        internal Get1AAxisOrigin(UIApplication _uiApp) : base(_uiApp)
        {

        }

        internal override void Execute()
        {
            // 方式一
            // teighaNet模块 直接读取链接cad信息

            // 拾取cad
            Reference reference = this.sel.PickObject(ObjectType.Element, new SelPickFilter_ImportInstance() { Doc = this.doc });

            ImportInstance importInstance = this.doc.GetElement(reference) as ImportInstance;
            CMD.PositonMoveDis = importInstance.GetTotalTransform().Origin;// Cad链接实例的相对移动位置

            CADLinkType type = CMD.Doc.GetElement(importInstance.GetTypeId()) as CADLinkType;
            string pathfile = ModelPathUtils.ConvertModelPathToUserVisiblePath(type.GetExternalFileReference().GetAbsolutePath());

            TeighaServices teighaServices = new TeighaServices(uiApp);
            teighaServices.TempDwgPath = pathfile;
            teighaServices.ParseDwg();

            DwgParser dwgParser = teighaServices.DwgParser;

            string pattern = @"s-wall";

            foreach (var item in dwgParser.LayerNames)
            {
                if (Regex.IsMatch(item, pattern, RegexOptions.IgnoreCase))
                {
                    List<LineInfo> lineInfos = dwgParser.LineLayerInfos[item];
                }
            }

            // 区分 竖向轴网

            //lineInfos.Select(p=>p.Line.Direction.)

            // 区分 横向轴网

            //throw new NotImplementedException();

        }
    }
}
