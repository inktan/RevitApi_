using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;

using goa.Common;
using goa.Common.Exceptions;

namespace ThreeDViewNavi
{
    internal static class Navi
    {
        internal static ViewNaviHistory History;
        internal static bool Fly = true;

        private static Timer timer = new Timer();
        private static readonly int minTimerInterval = 10;
        internal static int timerInterval = minTimerInterval; //millisecond
        private static XYZ prevCursor = XYZ.Zero;
        private static ReferenceIntersector ri;
        private static Document riDoc;
        private static XYZ prevPos = XYZ.Zero;
        internal static void Start(UIDocument _uidoc)
        {
            RequestMaker.Init();

            //check active view is 3d view
            if (_uidoc == null)
                return;
            var doc = _uidoc.Document;
            var view = _uidoc.ActiveView;
            var view3d = view as View3D;
            if (view3d == null
                || view3d.IsPerspective == false)
            {
                UserMessages.ShowMessage("当前活动视图必须为三维透视视图。");
                return;
            }
            if (view3d.IsLocked)
            {
                UserMessages.ShowMessage("当前视图被锁定，请先解锁。");
                return;
            }

            Navi.History = new ViewNaviHistory(doc, view.Id);
            Registry.Register(true);

            /*
            //check opened window
            MainWindow form = APP.MainWindow;
            if (null != form && form.IsDisposed == false)
            {
                form.CloseForm();
            }
            //show new window
            goa.Common.APP.RevitWindow = Methods.GetRevitWindow(APP.UIApp);
            form = new MainWindow(_uidoc);
            APP.MainWindow = form;
            form.Show(goa.Common.APP.RevitWindow);
            */

            timer.Interval = timerInterval;
            timer.Tick += onTimerTick;
            timer.Start();
        }

        internal static void Stop()
        {
            timer.Stop();
            timer.Tick -= onTimerTick;
            KeyBoardMouseMonitor.RegisterSystemEvents(false);
            RequestMaker.makeRequest(RequestId.close);
        }

        internal static void Apply()
        {
            if(KeyBoardMouseMonitor.KeysHoldingDown.Contains(Keys.G))
            {
                FeetOnGround();
                return;
            }
            if (History == null)
                return;
            var uidoc = APP.UIApp.ActiveUIDocument;
            if (uidoc == null)
                return;
            var doc = uidoc.Document;
            if (History.Doc.Equals(doc) == false)
                return;
            var view = uidoc.ActiveView;
            if (view is View3D == false
                || History.ViewId != view.Id)
                return;

            var view3d = view as View3D;
            var uiview = uidoc.GetOpenUIViews().First(x => x.ViewId == view3d.Id);

            //setup ri
            if (ri == null
                || riDoc == null
                || riDoc.IsValidObject == false
                || riDoc.Equals(doc) == false
                || ri.ViewId != view3d.Id)
            {
                ri = new ReferenceIntersector(GroundElemFilter, FindReferenceTarget.All, view3d);
                ri.FindReferencesInRevitLinks = true;
                riDoc = doc;
            }

            var orientation = view3d.GetOrientation();

            //filter ri around eye pos.
            //set elem filter does not improve much.
            //need to filter out elem ids.
            if (Fly == false)
                ri.SetTargetElementIds(getAllElemIdsCloseTo(orientation.EyePosition, 8.0, doc));

            XYZ anchor = XYZ.Zero, current = XYZ.Zero;
            if (KeyBoardMouseMonitor.mouseDown)
            {
                anchor = prevCursor;
                current = new XYZ(Cursor.Position.X, Cursor.Position.Y, 0);
            }
            prevCursor = new XYZ(Cursor.Position.X, Cursor.Position.Y, 0);

            double step = KeyBoardMouseMonitor.KeysHoldingDown.Contains(Keys.ControlKey)
                ? Settings.SuperBigStep : KeyBoardMouseMonitor.KeysHoldingDown.Contains(Keys.ShiftKey)
                ? Settings.BigStep
                : Settings.RegularStep;

            var newOrientation = getNewViewOrientation(KeyBoardMouseMonitor.KeysHoldingDown, anchor, current, orientation, step, Settings.rotateSpeed);
            if (newOrientation == null)
                return;

            //did not move?
            if (orientation.EyePosition.IsAlmostEqualToByDifference(newOrientation.EyePosition)
                && orientation.ForwardDirection.IsAlmostEqualToByDifference(newOrientation.ForwardDirection))
                return;

            view3d.SetOrientation(newOrientation);
            History.Save(newOrientation);
            uiview.Zoom(0.99);
            uiview.Zoom(1.0 / 0.99);
        }

        internal static void FeetOnGround()
        {
            var uidoc = APP.UIApp.ActiveUIDocument;
            if (uidoc == null)
                return;
            var doc = uidoc.Document;
            var view = uidoc.ActiveView;
            if (view is View3D == false)
                return;

            var view3d = view as View3D;
            var oldVo = view3d.GetOrientation();
            var oldEye = oldVo.EyePosition;

            var localRi = new ReferenceIntersector(view3d);
            localRi.FindReferencesInRevitLinks = true;

            var rwc = localRi.FindNearest(oldEye, -XYZ.BasisZ);
            XYZ tr = XYZ.Zero;
            var height = UnitUtils.ConvertToInternalUnits(1.6, DisplayUnitType.DUT_METERS);
            if (rwc != null)
            {
                var dist = rwc.Proximity;
                var delta = height - dist;
                tr += delta * XYZ.BasisZ;
            }

            var newEye = oldEye + tr;
            var newVo = new ViewOrientation3D(newEye, oldVo.UpDirection, oldVo.ForwardDirection);
            view3d.SetOrientation(newVo);

            var uiview = uidoc.GetOpenUIViews().First(x => x.ViewId == view3d.Id);
            uiview.Zoom(0.99);
            uiview.Zoom(1.0 / 0.99);

        }
        private static List<ElementId> getAllElemIdsCloseTo(XYZ _pos, double _diag, Document _doc)
        {
            var diagonalV = new XYZ(7.0, 7.0, 7.0);
            var bb = Methods.GetBoundingBox(new XYZ[2] { _pos - diagonalV, _pos + diagonalV });
            var filter = Methods.GetBBIntersectFilter(bb, 0.0);
            var ids = new FilteredElementCollector(_doc)
                .WherePasses(filter)
                .WhereElementIsNotElementType()
                .Select(x => x.Id)
                .ToList();
            return ids;
        }

        private static ViewOrientation3D getNewViewOrientation(HashSet<Keys> _keys, XYZ _anchor, XYZ _current, ViewOrientation3D _vo, double _step, double _roSpeed)
        {
            //reverse?
            if (_keys.Contains(Keys.R))
                return History.Reverse();
            //move and look
            var eyeTr = getEyeTranslation(_keys, _vo, _step);
            var lookTf = getLookTransform(_anchor, _current, _vo, _roSpeed);
            var newEye = _vo.EyePosition + eyeTr;
            var newUp = lookTf.OfVector(_vo.UpDirection);
            var newFor = lookTf.OfVector(_vo.ForwardDirection);
            return new ViewOrientation3D(newEye, newUp, newFor);
        }

        private static XYZ getEyeTranslation(HashSet<Keys> _keys, ViewOrientation3D _vo, double _step)
        {
            var oldForward = _vo.ForwardDirection;
            var oldLeft = _vo.UpDirection.CrossProduct(_vo.ForwardDirection).Normalize();

            XYZ forward = _keys.Contains(Keys.W)
                ? oldForward * _step
                : XYZ.Zero;

            XYZ backward = _keys.Contains(Keys.S)
                ? oldForward * -_step
                : XYZ.Zero;

            XYZ left = _keys.Contains(Keys.A)
                ? oldLeft * _step
                : XYZ.Zero;

            XYZ right = _keys.Contains(Keys.D)
                ? oldLeft * -_step
                : XYZ.Zero;

            XYZ up = _keys.Contains(Keys.E)
                ? XYZ.BasisZ * _step
                : XYZ.Zero;

            XYZ down = _keys.Contains(Keys.Q)
                ? XYZ.BasisZ * -_step
                : XYZ.Zero;

            XYZ move = forward + backward + left + right + up + down;
            if (Fly == false)
            {
                XYZ newPos = _vo.EyePosition + move;
                move += getObstructTranslation(_vo.EyePosition, newPos);
            }

            return move;
        }

        private static ElementMulticategoryFilter GroundElemFilter = new ElementMulticategoryFilter
            (new List<BuiltInCategory>()
            {
                BuiltInCategory.OST_Walls,
                BuiltInCategory.OST_Floors,
                BuiltInCategory.OST_Roofs,
                BuiltInCategory.OST_Topography,
                BuiltInCategory.OST_Stairs,
            });

        private static XYZ getObstructTranslation(XYZ _oldPos, XYZ _newPos)
        {
            XYZ tr = XYZ.Zero;
            //find obstruction between old and new pos
            var dir = (_newPos - _oldPos).Normalize();
            var distOldNew = _newPos.DistanceTo(_oldPos);
            var rwc = ri.FindNearest(_oldPos, dir);
            if (rwc != null && rwc.Proximity < distOldNew)
            {
                tr += (distOldNew - rwc.Proximity + 0.3) * dir.Negate();
                _newPos += tr;
            }
            //find ground
            rwc = ri.FindNearest(_newPos, -XYZ.BasisZ);
            if (rwc != null)
            {
                var dist = rwc.Proximity;
                var delta = 5.0 - dist;
                tr += delta * XYZ.BasisZ;
            }
            return tr;
        }

        private static Transform getLookTransform(XYZ _anchor, XYZ _current, ViewOrientation3D _vo, double _roSpeed)
        {
            double deltaX = _current.X - _anchor.X;
            double deltaY = _current.Y - _anchor.Y;
            deltaX *= -_roSpeed;
            deltaY *= -_roSpeed;

            var oldLeft = _vo.UpDirection.CrossProduct(_vo.ForwardDirection).Normalize();
            var up = Transform.CreateRotation(oldLeft, deltaY).Inverse;
            var left = Transform.CreateRotation(XYZ.BasisZ, deltaX);
            var tf = left * up;
            return tf;
        }

        private static bool processing = false;
        private static void onTimerTick(object sender, EventArgs e)
        {

            if (processing)
            {
                //timerInterval *= 2;
                //timer.Interval = timerInterval;
                return;
            }

            processing = true;

            /*
            if(timerInterval > minTimerInterval)
            {
                timerInterval *= (int)0.5;
                timer.Interval = timerInterval;
            }  
            */
            try
            {
                Navi.Apply();
            }
            catch (Exception ex)
            {
                Stop();
                UserMessages.ShowErrorMessage(ex, null);
            }
            processing = false;
        }
    }
}
