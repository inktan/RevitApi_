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

namespace ReadCadText
{
    internal class PickCad : RequestMethod
    {
        internal PickCad(UIApplication _uiApp) : base(_uiApp)
        {

        }
        /// <summary>
        /// 读各个图框
        /// </summary>
        internal void ReadFloorFrame()
        {

        }

        // 一层功能封装

        // 找到 1A 轴的交点

        internal DwgParser DwgParser { get; set; }
        internal override void Execute()
        {
            // 方式一
            // teighaNet模块 直接读取链接cad信息

            // 拾取cad
            Reference reference = this.sel.PickObject(ObjectType.Element, new SelPickFilter_ImportInstance() { Doc = this.doc });

            this.sw.Start();

            List<Level> levels =
                new FilteredElementCollector(CMD.Doc)
                .OfCategory(BuiltInCategory.OST_Levels)
                .OfClass(typeof(Level))
                .Cast<Level>()
                .ToList();

            CMD.UpLevel = levels.Where(p => p.Elevation > CMD.Doc.ActiveView.GenLevel.Elevation).OrderBy(p => p.Elevation).FirstOrDefault();
            CMD.Level = levels.Where(p => p.Elevation == CMD.Doc.ActiveView.GenLevel.Elevation).FirstOrDefault();
            CMD.DownLevel = levels.Where(p => p.Elevation < CMD.Doc.ActiveView.GenLevel.Elevation).OrderByDescending(p => p.Elevation).FirstOrDefault();

            ImportInstance importInstance = this.doc.GetElement(reference) as ImportInstance;
            CMD.PositonMoveDis = importInstance.GetTotalTransform().Origin;// Cad链接实例的相对移动位置

            CADLinkType type = CMD.Doc.GetElement(importInstance.GetTypeId()) as CADLinkType;
            string pathfile = ModelPathUtils.ConvertModelPathToUserVisiblePath(type.GetExternalFileReference().GetAbsolutePath());

            TeighaServices teighaServices = new TeighaServices(uiApp);
            teighaServices.TempDwgPath = pathfile;
            teighaServices.ParseDwg();

            DwgParser = teighaServices.DwgParser;


            // 方式二
            // 通讯功能模块

            //throw new NotImplementedException();

            this.sw.Stop();
            //$"{sw.ElapsedMilliseconds} + 毫秒".TaskDialogErrorMessage();
        }
    }
}
