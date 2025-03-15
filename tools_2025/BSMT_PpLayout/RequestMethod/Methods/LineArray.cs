using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Selection;
using g3;
using goa.Common.g3InterOp;
using ClipperLib;
using goa.Common;
using PubFuncWt;
using System.Diagnostics;

namespace BSMT_PpLayout
{
    class LineArray : RequestMethod
    {

        public LineArray(UIApplication uiapp) : base(uiapp)
        {
        }
        /// <summary>
        /// 线性阵列
        /// </summary>
        internal override void Execute()
        {
            //throw new NotImplementedException();

            // 鼠标两点划线
            Segment2d seg2d = CreatSeg2d();

            Polygon2d selRegion = CreatRegion(seg2d);

            Func<Frame3d> func = () =>
            {
                Vector3d origin = seg2d.P0.ToVector3d();
                Vector3d x = seg2d.Direction.Normalized.ToVector3d();
                Vector3d y = x.Rotate(new Vector3d(0, 0, 1), 90);
                Vector3d z = x.Cross(y).Normalized;// 叉积符合右手定则

                return new Frame3d(origin, x, y, z);
            };
            Frame3d frame3d = func();
            selRegion = func().ToFrame2dPoly(selRegion);

            PsCutClipPattern psArrCutClipLinearAuto_GerV = new PsArrCutClipLinearAuto_GerV();

            // 机械车位 普通车位
            if (GlobalData.Instance.wheMechanicalPs)
            {
                SelMechan selMechan = new SelMechan();

                selMechan.floors.ItemsSource = new List<string> { "2", "3" };
                selMechan.floors.SelectedIndex = 0;

                selMechan.ShowDialog();

                int _floor = Convert.ToInt32(selMechan.floors.SelectedItem as string);

                psArrCutClipLinearAuto_GerV = new PsArrCutClipLinearAuto_GerV_Mechan();
                psArrCutClipLinearAuto_GerV.MechanPsName += " " + _floor.ToString() + "F";

                GlobalData.Instance.pSWidth_num = GlobalData.Instance.pSWidth_num * 3;// 宽度设定为组合体包含的单元体个数
            }

            List<PPLocPoint> pPLocPoints = psArrCutClipLinearAuto_GerV.Sweep(selRegion, selRegion.OutwardOffeet(100.0), new List<Polygon2d>());
            pPLocPoints = PPLocPoint.FromFrame2d(pPLocPoints, frame3d).ToList();

            (new PPLocPoint()).PointProjecToRevit(pPLocPoints, doc, this.view);
        }

        Segment2d CreatSeg2d()
        {
            // UI交互 选择一个区域，选择一个边，filledregion的curveLoop顺序，在后台中，默认为逆时针。
            Vector2d selPoint01 = sel.PickPoint().ToUV().ToVector2d(); // 第一个点决定了，柱子的起点
            Vector2d selPoint02 = sel.PickPoint().ToUV().ToVector2d();
            return new Segment2d(selPoint01, selPoint02);

        }
        Polygon2d CreatRegion(Segment2d seg2d)
        {
            // 基于两点划线，创造可停车区域，该处不与停车方案产生关系

            Vector2d vecStart = seg2d.P0;
            Vector2d vecEnd = seg2d.P1;
            Vector2d pedl = vecStart.Rotate(vecEnd, -Math.PI / 2);
            Vector2d direction = (pedl - vecEnd).Normalized;
            Vector2d vecEnd01 = vecEnd + direction * GlobalData.Instance.pSHeight_num;
            Vector2d vecStart01 = vecStart + direction * GlobalData.Instance.pSHeight_num;
            Polygon2d rec = new Polygon2d(new List<Vector2d>() { vecStart, vecEnd, vecEnd01, vecStart01 });
            return rec;
        }

    }
}
