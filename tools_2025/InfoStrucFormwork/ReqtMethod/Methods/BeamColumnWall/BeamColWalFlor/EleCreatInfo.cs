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
using g3;

namespace InfoStrucFormwork
{
    internal abstract class EleCreatInfo
    {
        // cad
        internal DwgParser DwgParser { get; set; }
        internal List<ElementId> ElementIds { get; set; }

        public EleCreatInfo()
        {
            //System.Windows.Forms.Application.DoEvents();// 窗体重绘
        }
        internal abstract void Move();

        internal abstract void ExGeoInfo();
        /// <summary>
        /// 创建梁
        /// </summary>
        internal abstract void Execute();
        /// <summary>
        /// 创建物体、设定物体高度
        /// </summary>
        internal abstract void OpenTrans();

        /// <summary>
        /// 设置所有物体的对应标高
        /// </summary>
        internal abstract void SetLevel();

        /// <summary>
        /// 创建梁类型
        /// </summary>
        internal abstract void GetFamilySymbols();
    }
}

