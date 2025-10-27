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
    internal class ConcWall : RequestMethod
    {
        internal ConcWall(UIApplication _uiApp) : base(_uiApp)
        {
        }

        internal override void Execute()
        {
            PickCad pickCad = new PickCad(uiApp);
            pickCad.Execute();

            EleCreatInfo eleInfo;
            List<ElementId> elementIds = new List<ElementId>();
      
            // 剪力墙 
            eleInfo = new StrucGenlProfileWall();
            eleInfo.DwgParser = pickCad.DwgParser;
            eleInfo.Execute();// 将不可识别的竖向构件图形，处理为通用 结构墙_轮廓 轮廓族
            if (eleInfo.ElementIds != null)
            {
                elementIds.AddRange(eleInfo.ElementIds);
            }
   

            // 剪力墙 直线 L 算法不成熟 忽略
            //eleInfo = new StrucWall();
            //eleInfo.DwgParser = dwgParser;
            //eleInfo.Execute();
            //elementIds.AddRange(eleInfo.ElementIds);

            //if (elementIds.Count > 0)
            //{
            //    using (Transaction trans = new Transaction(this.doc, "创建组"))
            //    {
            //        trans.Start();
            //        Group group = this.doc.Create.NewGroup(elementIds);
            //        trans.Commit();
            //    }
            //}
            //else
            //{
                //"未生成任何模型构件".TaskDialogErrorMessage();
            //}


            //throw new NotImplementedException();

            this.sw.Stop();
            //$"{sw.ElapsedMilliseconds} + 毫秒".TaskDialogErrorMessage();
        }
    }
}
