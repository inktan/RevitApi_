using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.Revit.DB;
using g3;
using PubFuncWt;
using QuadTrees;

namespace BSMT_PpLayout
{
    /// <summary>
    /// 详图区域
    /// 当前存在的子停车区域
    /// </summary>
    class SubPsAreaExist
    {
        private RevitEleFR _bound { get; }
        internal View View => this._bound.View;
        private List<QTreeRevitEleCtrl> _qTreeRevitEleCtrls;
        internal SubPsAreaExist(RevitEleFR _bound, List<QTreeRevitEleCtrl> _eles )
        {
            this._bound = _bound;
            this._qTreeRevitEleCtrls = _eles;
        }
        internal void Computer()
        {
            this._InBoundElePses = inBoundElePses().ToList();
        }
        internal List<RevitElePS> _InBoundElePses { get; set; }
        internal List<RevitElePS> InBoundElePses { get { return _InBoundElePses; } }
        /// <summary>
        /// 绝对位于子停车区域的 车位 族实例
        /// </summary>
        internal IEnumerable<RevitElePS> inBoundElePses()
        {
            foreach (var item in this._qTreeRevitEleCtrls)
            {
                if (item.RevitEleCtrl is RevitElePS)
                {
                    RevitElePS revitElePS = item.RevitEleCtrl as RevitElePS;
                    //Polygon2d polygon2d02 = revitElePS.Polygon2d();
                    if (this.Polygon2d.Contains(revitElePS.LocVector2d))
                    {
                        yield return revitElePS;
                    }
                }
            }
        }
        internal Polygon2d Polygon2d => this._bound.BoundOs().First().polygon2d;
        /// <summary>
        /// 面积
        /// </summary>
        internal double Area => this._bound.Area();
        /// <summary>
        /// 停车位数量
        /// </summary>
        internal double Count()
        {
            double count = 0;
            this.InBoundElePses.ForEach(p =>
            {
                count += p.Count();
            });
            return count;
        }

        /// <summary>
        /// 当前停车效率 英制
        /// </summary>
        internal double PsIndex=> this.Area / this.Count();
        
        /// <summary>
        /// 低于指定停车效率进行填充区域红色提示
        /// </summary>
        internal void SetSubstandard(double referenceIndex)
        {
            if (this.PsIndex > referenceIndex)
            {
                // 将填充区域设置为浅红色
                OverrideGraphicSettings ogs = new OverrideGraphicSettings();
                ogs.SetSurfaceForegroundPatternColor(new Autodesk.Revit.DB.Color(255, 0, 0));
                ogs.SetSurfaceTransparency(75);
                this.View.SetElementOverrides(this._bound.Id, ogs);
            }
            else
            {
                // 将填充区域设置为透明
                OverrideGraphicSettings ogs = new OverrideGraphicSettings();
                ogs.SetSurfaceForegroundPatternColor(new Autodesk.Revit.DB.Color(255, 255, 255));
                ogs.SetSurfaceTransparency(100);
                this.View.SetElementOverrides(this._bound.Id, ogs);
            }

            // 这里需要对平行车位进行高亮处理


        }
    }
}
