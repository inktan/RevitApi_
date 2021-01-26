using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.DB.Events;
using Autodesk.Revit.DB.ExtensibleStorage;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Forms;
using g3;
using goa.RevitUI;

namespace goa.RevitUI
{
    public static class Methods
    {
        internal static FamilyInstance tempFi1;
        internal static FamilyInstance tempFi2;
        internal static XYZ pos1;
        internal static XYZ face1;

        internal static MainWindowRequestHandler m_Handler;
        internal static ExternalEvent m_ExEvent;
        public static void makeRequest(RequestId request)
        {
            if (m_Handler == null)
            {
                m_Handler = new MainWindowRequestHandler();
                m_ExEvent = ExternalEvent.Create(m_Handler);
            }

            m_Handler.Request.Make(request);
            m_ExEvent.Raise();
        }


        public static FamilyInstance PlaceTempFi(Document doc, IList<ElementId> elemIds)
        {
            var bbs = elemIds.Where(x => doc.GetElement(x) != null)
                        .Select(x => doc.GetElement(x)
                        .get_BoundingBox(doc.ActiveView)).ToList();

            IList<double> xlist = new List<double>();
            IList<double> yList = new List<double>();

            foreach (var bb in bbs)
            {
                if (bb != null)
                {
                    xlist.Add(bb.Min.X);
                    xlist.Add(bb.Max.X);
                    yList.Add(bb.Min.Y);
                    yList.Add(bb.Max.Y);
                }
            }

            double L = xlist.Max() - xlist.Min();
            double H = yList.Max() - yList.Min();
            XYZ pos = new XYZ(xlist.Min(), yList.Min(), 0);

            //放置tempFi
            FamilyInstance tempFi;
            using (Transaction trans = new Transaction(doc))
            {
                trans.Start("placeFi");

                FamilySymbol fs = LoadFamilyAndActive(doc, 4);
                tempFi = doc.Create.NewFamilyInstance(
                    pos, fs, doc.ActiveView);
                Parameter height = tempFi.LookupParameter("H");
                Parameter length = tempFi.LookupParameter("L");
                height.Set(H);
                length.Set(L);
                trans.Commit();
            }
            return tempFi;
        }

        public static FamilySymbol LoadFamilyAndActive(Autodesk.Revit.DB.Document doc, int fiType)
        {
            FamilySymbol fs = GetSpecialSymbol(doc, fiType);
            if (fs == null)
            {
                //载入族，激活symbol
                string file;
                if (fiType == 1)
                {
                    file = @"W:\BIM_ARCH\02.族库\特殊族\设计模块插件专用\DoNotDeleteWarning.rfa";
                }
                else if (fiType == 2)
                {
                    file = @"W:\BIM_ARCH\02.族库\特殊族\设计模块插件专用\DoNotDeleteWarning2.rfa";
                }
                else if (fiType == 3)
                {
                    file = @"W:\BIM_ARCH\02.族库\特殊族\设计模块插件专用\DoNotDeleteWarning3.rfa";
                }
                else
                {
                    file = @"W:\BIM_ARCH\02.族库\特殊族\设计模块插件专用\TempFi.rfa";
                }
                Family family;
                bool loadSuccess = doc.LoadFamily(file, out family);
                if (loadSuccess)
                {
                    ISet<ElementId> familySymbolIds = family.GetFamilySymbolIds();
                    if (familySymbolIds.Count != 0)
                    {
                        // Get family symbols which is contained in this family
                        foreach (ElementId id in familySymbolIds)
                        {
                            FamilySymbol familySymbol = family.Document.GetElement(id) as FamilySymbol;
                            if (familySymbol != null)
                            {
                                fs = familySymbol;
                                break;
                            }
                        }
                    }
                }
                else MessageBox.Show("载入族失败");

                if (!fs.IsActive)
                    fs.Activate();
            }
            return fs;
        }

        public static FamilySymbol GetSpecialSymbol(Autodesk.Revit.DB.Document doc, int fiType)
        {
            //找family
            FilteredElementCollector families = new FilteredElementCollector(doc);
            IList<Element> fams = null;
            //3D Module
            if (fiType == 1)
            {
                fams = families.OfClass(typeof(Family)).
                    Where(x => (x.Name == "DoNotDeleteWarning")).ToList();
            }
            //2D Module
            if (fiType == 2)
            {
                fams = families.OfClass(typeof(Family)).
                    Where(x => (x.Name == "DoNotDeleteWarning2")).ToList();
            }
            //tempFi
            if (fiType == 4)
            {
                fams = families.OfClass(typeof(Family)).
                    Where(x => (x.Name == "TempFi")).ToList();
            }

            if (fams.Count() != 0)
            {
                //找familySymbol
                FamilySymbolFilter filter = new FamilySymbolFilter(fams.First().Id);
                FilteredElementCollector famSymbols = new FilteredElementCollector(doc);
                var famSyms = famSymbols.WherePasses(filter).ToElements();
                if (famSyms.Count != 0)
                {
                    FamilySymbol fs = famSyms.First() as FamilySymbol;
                    return fs;
                }
                else { return null; }
            }
            else { return null; }


        }

        /// <summary>
        /// 监控新生成的tempFi
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static void OnDocumentChanged(object sender, DocumentChangedEventArgs e)
        {
            Document doc = e.GetDocument();
            foreach (ElementId id in e.GetAddedElementIds())
            {
                Element elem = doc.GetElement(id);
                if (elem is FamilyInstance &&
                    (elem as FamilyInstance).Symbol.FamilyName == "TempFi")
                {
                    tempFi2 = elem as FamilyInstance;
                }
            }
        }

        #region 替代原生move
        public static void Move(Document doc, IList<ElementId> elemIds)
        {
            //创建临时操作fi         
            tempFi1 = PlaceTempFi(doc, elemIds);
            pos1 = (tempFi1.Location as LocationPoint).Point;

            makeRequest(RequestId.PostCmd_move);
        }
        public static void PostCmd_move(UIApplication uiapp)
        {
            Selection sel = uiapp.ActiveUIDocument.Selection;
            sel.SetElementIds(new List<ElementId> { tempFi1.Id });

            var commandId = RevitCommandId.LookupPostableCommandId(PostableCommand.Move);
            if (uiapp.CanPostCommand(commandId))
            {
                uiapp.PostCommand(commandId);
            }
        }
        public static void API_move(Document doc)
        {
            XYZ pos2 = (tempFi1.Location as LocationPoint).Point;
            XYZ moveVc = pos2 - pos1;
            using (Transaction trans = new Transaction(doc))
            {
                trans.Start("delete tempFi");
                doc.Delete(tempFi1.Id);
                trans.Commit();
            }
            if (moveVc.IsAlmostEqualTo(XYZ.Zero))
            {
                return;
            }
            else
            {
                //执行模块的移动
                //ModuleMove.MoveOperate(doc, CMD.allFi, moveVc);
            }
        }
        #endregion

        #region  替代原生的copy
        public static void Copy(Document doc, IList<ElementId> elemIds)
        {
            tempFi1 = PlaceTempFi(doc, elemIds);
            makeRequest(RequestId.PostCmd_copy);
        }
        public static void PostCmd_copy(UIApplication uiapp)
        {
            //用以监控新生成的tempFi
            uiapp.Application.DocumentChanged +=
                new EventHandler<DocumentChangedEventArgs>(OnDocumentChanged);

            Selection sel = uiapp.ActiveUIDocument.Selection;
            sel.SetElementIds(new List<ElementId> { tempFi1.Id });
            var commandId = RevitCommandId.LookupPostableCommandId(PostableCommand.Copy);
            if (uiapp.CanPostCommand(commandId))
            {
                uiapp.PostCommand(commandId);
            }
        }
        public static void API_copy(UIApplication uiapp)
        {
            uiapp.Application.DocumentChanged -=
                new EventHandler<DocumentChangedEventArgs>(OnDocumentChanged);
            Document doc = uiapp.ActiveUIDocument.Document;
            if (tempFi1 != null && tempFi2 != null)
            {
                XYZ pos1 = (tempFi1.Location as LocationPoint).Point;
                XYZ pos2 = (tempFi2.Location as LocationPoint).Point;
                XYZ copyVc = pos2 - pos1;

                using (Transaction trans = new Transaction(doc))
                {
                    trans.Start("delete tempFi");
                    doc.Delete(new List<ElementId> { tempFi1.Id, tempFi2.Id });
                    trans.Commit();
                }
                using (Transaction trans = new Transaction(doc))
                {
                    trans.Start("执行模块的复制");
                    //ModuleCopy.CopyMuliFiInSameLv(doc, Copy_CMD.allFi, copyVc);
                    trans.Commit();
                }
            }
            else
            {
                using (Transaction trans = new Transaction(doc))
                {
                    trans.Start("delete tempFi");
                    if (tempFi1 != null)
                    {
                        doc.Delete(new List<ElementId> { tempFi1.Id });
                    }
                    if (tempFi2 != null)
                    {
                        doc.Delete(new List<ElementId> { tempFi2.Id });
                    }
                    trans.Commit();
                }
            }
        }
        #endregion

        #region 替代原生拾取镜像
        public static void MirrorPick(Document doc, IList<ElementId> elemIds)
        {
            //创建临时操作fi         
            tempFi1 = PlaceTempFi(doc, elemIds);
            makeRequest(RequestId.PostCmd_MirrorPick);
        }
        public static void PostCmd_MirrorPick(UIApplication uiapp)
        {
            uiapp.Application.DocumentChanged +=
                new EventHandler<DocumentChangedEventArgs>(OnDocumentChanged);

            Selection sel = uiapp.ActiveUIDocument.Selection;
            sel.SetElementIds(new List<ElementId> { tempFi1.Id });

            var commandId = RevitCommandId.LookupPostableCommandId(
                PostableCommand.MirrorPickAxis);
            if (uiapp.CanPostCommand(commandId))
            {
                uiapp.PostCommand(commandId);
            }
        }
        public static void API_mirrorPick(UIApplication uiapp)
        {
            uiapp.Application.DocumentChanged -=
                new EventHandler<DocumentChangedEventArgs>(OnDocumentChanged);
            Document doc = uiapp.ActiveUIDocument.Document;
            if (tempFi1 != null && tempFi2 != null)
            {
                XYZ pos1 = (tempFi1.Location as LocationPoint).Point;
                XYZ pos2 = (tempFi2.Location as LocationPoint).Point;
                Plane mirrorPlane = null;
                using (Transaction trans = new Transaction(doc))
                {
                    trans.Start("create mirrorPlane");
                    XYZ mid = (pos2 + pos1) * 0.5;

                    if (tempFi1.FacingOrientation.
                        IsAlmostEqualTo(tempFi2.FacingOrientation, 0.0001))
                    {
                        mirrorPlane = Plane.CreateByNormalAndOrigin(
                            tempFi1.HandOrientation, mid);
                    }
                    if (tempFi1.HandOrientation.
                        IsAlmostEqualTo(tempFi2.HandOrientation, 0.0001))
                    {
                        mirrorPlane = Plane.CreateByNormalAndOrigin(
                           tempFi1.FacingOrientation, mid);
                    }
                    //删除tempFi
                    doc.Delete(new List<ElementId> { tempFi1.Id, tempFi2.Id });
                    trans.Commit();
                }

                //执行模块的镜像操作
                //ModuleMirror.MirrorMultFi(doc, MirrorPick_CMD.allFi, mirrorPlane, true);
            }
            else
            {
                using (Transaction trans = new Transaction(doc))
                {
                    trans.Start("delete tempFi");
                    if (tempFi1 != null)
                    {
                        doc.Delete(new List<ElementId> { tempFi1.Id });
                    }
                    if (tempFi2 != null)
                    {
                        doc.Delete(new List<ElementId> { tempFi2.Id });
                    }
                    trans.Commit();
                }
            }
        }
        #endregion

        #region 替代原生旋转
        internal static void Rotate(Document doc, IList<ElementId> elemIds)
        {
            //创建临时操作fi         
            tempFi1 = PlaceTempFi(doc, elemIds);
            pos1 = (tempFi1.Location as LocationPoint).Point;
            face1 = tempFi1.FacingOrientation;

            makeRequest(RequestId.PostCmd_Rotate);
        }

        public static void PostCmd_rotate(UIApplication uiapp)
        {
            Selection sel = uiapp.ActiveUIDocument.Selection;
            sel.SetElementIds(new List<ElementId> { tempFi1.Id });

            var commandId = RevitCommandId.LookupPostableCommandId(PostableCommand.Rotate);
            if (uiapp.CanPostCommand(commandId))
            {
                uiapp.PostCommand(commandId);
            }
        }

        public static void API_rotate(Document doc)
        {
            XYZ pos2 = (tempFi1.Location as LocationPoint).Point;
            XYZ face2 = tempFi1.FacingOrientation;
            using (Transaction trans = new Transaction(doc))
            {
                trans.Start("rotate");
                doc.Delete(tempFi1.Id);
                trans.Commit();
            }
            if (pos1.IsAlmostEqualTo(pos2))
            {
                return;
            }
            else
            {
                //求旋转中心
                XYZ rotateCenter = GetRotateCenter(pos1, face1, pos2, face2);

                double angle = face1.AngleOnPlaneTo(face2, new XYZ(0, 0, 1));

                using (Transaction trans = new Transaction(doc))
                {
                    trans.Start("rotate");
                    //执行模块打的旋转操作
                    //ModuleRotate.RotateOperation(doc, Rotate_CMD.allFi, rotateCenter, angle);
                    trans.Commit();
                }
            }

        }

        public static Line GetPerpendicularBisector(XYZ p1, XYZ p2)
        {
            XYZ mid = (p1 + p2) * 0.5;
            XYZ vc1 = p1 - p2;
            XYZ normal = vc1.CrossProduct(new XYZ(0, 0, 1));
            XYZ mid2 = mid + normal;
            Line pb = Line.CreateBound(mid, mid2);
            return pb;
        }
        /// <summary>
        /// 求两个familyinstance的旋转中心
        /// </summary>
        /// <param name="pos1"></param>
        /// <param name="faceOri1"></param>
        /// <param name="pos2"></param>
        /// <param name="faceOri2"></param>
        /// <returns></returns>
        public static XYZ GetRotateCenter(XYZ pos1, XYZ faceOri1, XYZ pos2, XYZ faceOri2)
        {
            //求垂直平分线1
            Line pb1 = GetPerpendicularBisector(pos1, pos2);
            //求垂直平分线2
            XYZ pos3 = pos1 + faceOri1;
            XYZ pos4 = pos2 + faceOri2;
            Line pb2 = GetPerpendicularBisector(pos3, pos4);

            Line2d line1 = pb1.ToLine2d();
            Line2d line2 = pb2.ToLine2d();

            //求垂直平分线的交点
            IntrLine2Line2 inter = new IntrLine2Line2(line1, line2);
            inter.Compute();
            if (inter.Quantity != 0)
            {
                Vector2d p = inter.Point;
                XYZ _p = new XYZ(p.x, p.y, pos1.Z);
                return _p;
            }
            return null;
        }
        public static Vector2d ToVector2d(this XYZ xyz)
        {
            return new Vector2d(xyz.X, xyz.Y);
        }
        public static Line2d ToLine2d(this Line line)
        {
            return new Line2d(line.GetEndPoint(0).ToVector2d(), line.Direction.ToVector2d());
        }
        #endregion

    }
}
