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
    class SubAreaLaneGeneration : RequestMethod
    {

        public SubAreaLaneGeneration(UIApplication uiapp) : base(uiapp)
        {
        }

        internal override void Execute()
        {
            "指定起点和终点，进行子区域，水平车道最优排布，该功能正在优化，待释放".TaskDialogErrorMessage();
            //Computer01();
            //throw new NotImplementedException();
        }

        internal void Computer01()
        {

            #region UI交互 选择一个区域，选择一个边，filledregion的curveLoop顺序，在后台中，默认为逆时针。
            Reference reference = sel.PickObject(ObjectType.Element, new SelPickFilter_FilledRegion(), "请选择 地库_子停车区域");
            FilledRegion filledRegion = doc.GetElement(reference) as FilledRegion;
            CurveLoop curveLoop = filledRegion.GetBoundaries().First();

            List<Curve> curves = curveLoop.ToCurves().ToList();
            Polygon2d selPolygon2d = curves.ToPolygon2d();
            #endregion

            #region 
            ElemsViewLevel _elemsViewLevel = new ElemsViewLevel(view);
            View nowView = _elemsViewLevel.View;

            ElementId selEleId = new ElementId(-1);
            foreach (Bsmt bsmt in _elemsViewLevel.Bsmts)
            {
                // 判断点选矩形所在 Basement
                Polygon2d polygon2d = bsmt.BsmtBound.Polygon2d;
                if (!polygon2d.Contains(selPolygon2d) && !polygon2d.Intersects(selPolygon2d))
                {
                    continue;  // 如果所选线段，与当前视图的所有外墙区域均不发生关系
                }
                bsmt.Computer_Ps_Fr_Col_SubExit_Area();
                bsmt.Computer_VeRa();

                selEleId = bsmt.BsmtBound.Id;

                BsmtClipByRoad bsmtClipperByRoad = new BsmtClipByRoad(bsmt);// 对每个BaseMent进行，停车区域智能划分
                List<SubParkArea> subParkAreas = bsmtClipperByRoad.SubParkAreasByBacktrack(selPolygon2d).ToList();

            

            }


            #endregion
        }
    }
}
