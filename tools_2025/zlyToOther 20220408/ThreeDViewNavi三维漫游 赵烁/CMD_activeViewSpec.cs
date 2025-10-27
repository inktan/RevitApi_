using Autodesk.Revit;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using goa.Common;
using goa.Common.Exceptions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace ThreeDViewNavi
{
    [TransactionAttribute(TransactionMode.Manual)]
    [RegenerationAttribute(RegenerationOption.Manual)]
    public class CMD_activeViewSpec : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData,
                      ref string message,
                      ElementSet elements)
        {
            try
            {
                showActiveViewSpecs(commandData.Application.ActiveUIDocument);
            }
            catch (CommonUserExceptions ex)
            {
                UserMessages.ShowMessage(ex.Message);
            }
            catch (Exception ex)
            {
                UserMessages.ShowErrorMessage(ex, null);
            }
            return Result.Succeeded;
        }


        private void showActiveViewSpecs(UIDocument _uidoc)
        {
            var view3d = _uidoc.ActiveView as View3D;
            if (view3d == null || view3d.IsPerspective == false)
                throw new CommonUserExceptions("请在三维透视视图中运行。");

            double width3d, height3d, dist;
            if(view3d.CropBoxActive)
            {
                getViewSpecCroped(view3d, out dist, out width3d, out height3d);
            }
            else
            {                
                getViewSpecNoneCrop(_uidoc, out dist, out width3d, out height3d);
            }

            string s = "";
            /*
            s += "width:" + UnitUtils.ConvertFromInternalUnits(width3d, DisplayUnitType.DUT_MILLIMETERS).ToStringDigits(2) + "\r\n";
            s += "height:" + UnitUtils.ConvertFromInternalUnits(height3d, DisplayUnitType.DUT_MILLIMETERS).ToStringDigits(2) + "\r\n";
            s += "dist:" + UnitUtils.ConvertFromInternalUnits(dist, DisplayUnitType.DUT_MILLIMETERS).ToStringDigits(2) + "\r\n";
            */
            s += "水平视角:" + Methods.RadToDegree(Math.Atan(width3d * 0.5 / dist) * 2.0).ToStringDigits(2) + "°\r\n";
            s += "垂直视角:" + Methods.RadToDegree(Math.Atan(height3d * 0.5 / dist) * 2.0).ToStringDigits(2) + "°";
            UserMessages.ShowMessage(s);
        }

        private void getViewSpecCroped(View3D view3d, out double dist, out double width3d, out double height3d)
        {
            var cropMgr = view3d.GetCropRegionShapeManager();
            var crop = cropMgr.GetCropShape();
            var vertices = crop.SelectMany(x => x.Select(c => c.GetEndPoint(0))).ToList();
            var mean = XYZ.Zero;
            foreach (var p in vertices)
                mean += p;
            mean /= vertices.Count;
            var right = view3d.RightDirection;
            var up = view3d.UpDirection;
            double minH = double.MaxValue, minV = double.MaxValue;
            double maxH = double.MinValue, maxV = double.MinValue;
            foreach(var p in vertices)
            {
                var v = p - mean;
                var dotH = v.DotProduct(right);
                var dotV = v.DotProduct(up);
                minH = Math.Min(dotH, minH);
                minV = Math.Min(dotV, minV);
                maxH = Math.Max(dotH, maxH);
                maxV = Math.Max(dotV, maxV);
            }
            width3d = maxH - minH;
            height3d = maxV - minV;
            var eyePos = view3d.GetOrientation().EyePosition;
            dist = Math.Abs((mean - eyePos).DotProduct(view3d.ViewDirection));
        }

        private void getViewSpecNoneCrop(UIDocument _uidoc, out double dist, out double width3d, out double height3d)
        {
            var uiView = GetActiveUiView(_uidoc);
            var view3d = _uidoc.ActiveView as View3D;
            IList<XYZ> corners = uiView.GetZoomCorners();
            XYZ c1 = corners[0];
            XYZ c2 = corners[1];
            XYZ diagonal = c2 - c1;
            XYZ up = view3d.UpDirection;
            XYZ right = view3d.RightDirection;
            width3d = diagonal.DotProduct(right);
            height3d = diagonal.DotProduct(up);
            var eyePos = view3d.GetOrientation().EyePosition; //eye position is behind view rectangle
            dist = Math.Abs((c1 - eyePos).DotProduct(view3d.ViewDirection));
        }

        /// <summary>
        /// Return currently active UIView or null.
        /// </summary>
        private UIView GetActiveUiView(UIDocument uidoc)
        {
            Document doc = uidoc.Document;
            Autodesk.Revit.DB.View view = doc.ActiveView;
            IList<UIView> uiviews = uidoc.GetOpenUIViews();
            UIView uiview = null;

            foreach (UIView uv in uiviews)
            {
                if (uv.ViewId.Equals(view.Id))
                {
                    uiview = uv;
                    break;
                }
            }
            return uiview;
        }
    }
}
