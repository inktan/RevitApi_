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
    internal class ColFliter : ISelectionFilter
    {
        bool ISelectionFilter.AllowElement(Element elem)
        {
            if (elem is FamilyInstance fi)
            {
                //return true;
                if (fi.Symbol.FamilyName.Contains("柱"))
                {
                    return true;
                }
            }
            return false;
            //throw new NotImplementedException();
        }

        bool ISelectionFilter.AllowReference(Reference reference, XYZ position)
        {
            return true;
            //throw new NotImplementedException();
        }
    }
    internal class ConcSingleColFollowFaceUi : RequestMethod
    {
        internal ConcSingleColFollowFaceUi(UIApplication _uiApp) : base(_uiApp)
        {
        }

        internal override void Execute()
        {
            //选一根柱
            Reference reference01 = this.sel.PickObject(ObjectType.Element, new ColFliter());
            FamilyInstance familyInstance01 = this.doc.GetElement(reference01) as FamilyInstance;
            if (familyInstance01 == null)
                throw new NotImplementedException("请选择一根柱");

            //选一个面
            List<Face> roofFaces = new List<Face>();
            Reference reference02 = CMD.Sel.PickObject(ObjectType.PointOnElement);
            Element element02 = CMD.Doc.GetElement(reference02.ElementId);
            if (element02 is RevitLinkInstance)
            {
                RevitLinkInstance revitLinkInstance = element02 as RevitLinkInstance;
                Document linkDoc = revitLinkInstance.GetLinkDocument();
                Element linkEle = linkDoc.GetElement(reference02.LinkedElementId);

                Reference linkRefer = reference02.CreateReferenceInLink();
                Face face = linkEle.GetGeometryObjectFromReference(linkRefer) as Face;//通过reference获取几何

                roofFaces.Add(face);
            }
            else
            {
                roofFaces.Add(element02.GetGeometryObjectFromReference(reference02) as Face);
            }
            if (roofFaces.Count < 0)
                throw new NotImplementedException("请选择一个顶面");

            //计算顶点高度
            XYZ point = familyInstance01.LocationPoint();
            Line line = Line.CreateBound(point, point + new XYZ(0, 0, 1) * 1600);
            SetComparisonResult setComparisonResult = roofFaces.First().Intersect(line, out IntersectionResultArray intersectionResultArray);
            if (intersectionResultArray == null || intersectionResultArray.IsEmpty)
                throw new NotImplementedException("当前选择柱子不在面覆盖范围内");
            else
            {
                XYZ tarPoint = intersectionResultArray.get_Item(0).XYZPoint;
                //tarPoint.ToString().TaskDialogErrorMessage();

                //修改柱的高度
                try
                {
                    using (Transaction trans = new Transaction(CMD.Doc, "修改标高"))
                    {
                        trans.DeleteErrOrWaringTaskDialog();
                        trans.Start();

                        //顶部标高
                        ElementId topLevelId = familyInstance01.get_Parameter(BuiltInParameter.SCHEDULE_TOP_LEVEL_PARAM).AsElementId();
                        Level topLevel = this.doc.GetElement(topLevelId) as Level;
                        double topLevelHeight = topLevel.Elevation;

                        //计算顶部偏移距离
                        double moveDis = tarPoint.Z - topLevelHeight;
                        //顶部偏移
                        familyInstance01.get_Parameter(BuiltInParameter.FAMILY_TOP_LEVEL_OFFSET_PARAM).Set(moveDis);
                        familyInstance01.get_Parameter(BuiltInParameter.STRUCTURAL_ANALYTICAL_MODEL).Set(0.0);

                        trans.Commit();
                    }
                }
                catch (Exception)
                {
                    throw new NotImplementedException("创建面吸附梁失败");
                    //throw;
                }
            }
            //throw new NotImplementedException();

            this.sw.Stop();
            //$"{sw.ElapsedMilliseconds} + 毫秒".TaskDialogErrorMessage();
        }
    }
}
