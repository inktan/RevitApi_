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

    internal class BeamFliter : ISelectionFilter
    {
        bool ISelectionFilter.AllowElement(Element elem)
        {
            if (elem is FamilyInstance fi)
            {
                //return true;
                if (fi.Symbol.FamilyName.Contains("梁"))
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
    internal class ConcSingleBeamFollowFaceUi : RequestMethod
    {
        internal ConcSingleBeamFollowFaceUi(UIApplication _uiApp) : base(_uiApp)
        {
        }

        internal override void Execute()
        {
            //选一根梁
            Reference reference01 = this.sel.PickObject(ObjectType.Element, new BeamFliter());
            FamilyInstance familyInstance01 = this.doc.GetElement(reference01) as FamilyInstance;
            if (familyInstance01 == null)
                throw new NotImplementedException("请选择一根梁");

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

            //计算投影线
            Curve curve = familyInstance01.LocationCurve();
            curve = ConBeamFollowFace.ProjectToRoof(curve, roofFaces);
            if (curve == null)
                throw new NotImplementedException("计算投影线失败");

            //生成新的梁
            try
            {
                using (Transaction trans = new Transaction(CMD.Doc, "修改标高"))
                {
                    trans.DeleteErrOrWaringTaskDialog();
                    trans.Start();

                    FamilyInstance familyInstance = CMD.Doc.Create.NewFamilyInstance(curve, familyInstance01.Symbol, CMD.Doc.ActiveView.GenLevel, StructuralType.Beam); ;

                    familyInstance.LookupParameter("宽度").Set(familyInstance01.LookupParameter("宽度").AsDouble());
                    familyInstance.LookupParameter("高度").Set(familyInstance01.LookupParameter("高度").AsDouble());

                    // 结构分析
                    Parameter parameter = familyInstance.get_Parameter(BuiltInParameter.STRUCTURAL_ANALYTICAL_MODEL);
                    if (parameter != null && !parameter.IsReadOnly)
                        parameter.Set(0);
                    // 全部刷成无连接模式
                    parameter = familyInstance.get_Parameter(BuiltInParameter.STRUCT_FRAM_JOIN_STATUS);
                    if (parameter != null && !parameter.IsReadOnly)
                        parameter.Set(1);
                    trans.Commit();
                }
            }
            catch (Exception)
            {
                throw new NotImplementedException("创建面吸附梁失败");
                //throw;
            }
            //删除旧梁
            using (Transaction trans = new Transaction(CMD.Doc, "修改标高"))
            {
                trans.DeleteErrOrWaringTaskDialog();
                trans.Start();
                CMD.Doc.Delete(familyInstance01.Id);
                trans.Commit();
            }
            //throw new NotImplementedException();

            this.sw.Stop();
            //$"{sw.ElapsedMilliseconds} + 毫秒".TaskDialogErrorMessage();
        }
    }
}
