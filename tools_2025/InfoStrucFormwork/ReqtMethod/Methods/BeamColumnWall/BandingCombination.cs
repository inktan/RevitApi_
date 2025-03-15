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
    internal class BandingCombination : RequestMethod
    {
        internal BandingCombination(UIApplication _uiApp) : base(_uiApp)
        {
        }

        List<FamilySymbol> FamilySymbols { get; set; }

        internal override void Execute()
        {
            //01 -结构梁
            Family fa = (new FilteredElementCollector(CMD.Doc))
                  .OfClass(typeof(Family))
                  .First(p => p.Name == "混凝土-矩形梁") as Family;

            FamilySymbols = fa.GetFamilySymbolIds().Select(p => CMD.Doc.GetElement(p) as FamilySymbol).ToList();


            //02- 求默认梁宽
            FamilySymbol familySymbol = this.FamilySymbols.First();
            familySymbol = this.FamilySymbols.Where(_p => _p.Name == "矩形梁").FirstOrDefault();

            Line line = Line.CreateBound(XYZ.Zero, XYZ.Zero + new XYZ(100, 0, 0));

            FamilyInstance familyInstance = null;
            using (Transaction trans = new Transaction(this.doc, "-"))
            {
                trans.Start();

                if (!familySymbol.IsActive)
                {
                    familySymbol.Activate();
                }
                familyInstance = CMD.Doc.Create.NewFamilyInstance(line, familySymbol, CMD.Doc.ActiveView.GenLevel, StructuralType.Beam); ;
                trans.Commit();
            }
            double width = familyInstance.LookupParameter("宽度").AsDouble();

            this.doc.DelEleId(familyInstance.Id);

            // 寻找定位线
            Reference reference = this.sel.PickObject(ObjectType.Element);
            Element element = this.doc.GetElement(reference);
            Face face = element.BottomPlanarFace();

            List<Curve> curves = face.GetAllEdges().Select(p => p.AsCurve()).ToList();

            // 计算合适的Z值
            double z = 0.0;
            if (element.Name.Contains("CONC"))
            {
                if (element.Name.Contains("FNSH"))
                {
                    z += (element as Floor).FloorType.GetCompoundStructure().GetLayers()[1].Width;
                }
                else
                {
                    z += element.LookupParameter("厚度").AsDouble();
                }
            }

            Transform transform = Transform.CreateTranslation(new XYZ(0,0,z));
            curves = curves.Select(p => p.CreateTransformed(transform)).ToList();

            CurveLoop curveLoop = CurveLoop.Create(curves);

            if (ViewModel.Instance.BeamWidth > 1)
            {
                curveLoop = CurveLoop.CreateViaOffset(curveLoop, (ViewModel.Instance.BeamWidth * (0.5)).MilliMeterToFeet(), XYZ.BasisZ);
            }
            else
            {
                curveLoop = CurveLoop.CreateViaOffset(curveLoop, width * (0.5), XYZ.BasisZ);
            }

            OpenTrans(curveLoop.ToList());
            SetLevel(element);

            //throw new NotImplementedException();

            //$"{sw.ElapsedMilliseconds} + 毫秒".TaskDialogErrorMessage();
        }
        List<FamilyInstance> FamilyInstances { get; set; }
        List<ElementId> ElementIds { get; set; }

        internal void OpenTrans(List<Curve> curves)
        {
            this.FamilyInstances = new List<FamilyInstance>();
            this.ElementIds = new List<ElementId>();

            using (Transaction trans = new Transaction(CMD.Doc, "-"))
            {
                trans.DeleteErrOrWaringTaskDialog();
                trans.Start();

                foreach (var item in curves)
                {

                    FamilySymbol familySymbol = this.FamilySymbols.First();
                    familySymbol = this.FamilySymbols.Where(_p => _p.Name == "矩形梁").FirstOrDefault();

                    if (!familySymbol.IsActive)
                    {
                        familySymbol.Activate();
                    }
                    FamilyInstance familyInstance = CMD.Doc.Create.NewFamilyInstance(item, familySymbol, CMD.Doc.ActiveView.GenLevel, StructuralType.Beam); ;
                    this.FamilyInstances.Add(familyInstance);
                    this.ElementIds.Add(familyInstance.Id);
                }

                trans.Commit();
            }
        }
        internal void SetLevel(Element ele)
        {
            //需要判断一下是否为楼板
            //if (ele is Floor)
            //{
            //    return;
            //}

            using (Transaction trans = new Transaction(CMD.Doc, "修改标高"))
            {
                trans.DeleteErrOrWaringTaskDialog();
                trans.Start();

                foreach (var familyInstance in this.FamilyInstances)
                {
                    if (ViewModel.Instance.BeamWidth > 1)
                    {
                        familyInstance.LookupParameter("宽度").Set(ViewModel.Instance.BeamWidth.MilliMeterToFeet());
                    }
                    if (ViewModel.Instance.BeamHeight > 1)
                    {
                        familyInstance.LookupParameter("高度").Set(ViewModel.Instance.BeamHeight.MilliMeterToFeet());
                    }

                    // 结构分析
                    Parameter parameter = familyInstance.get_Parameter(BuiltInParameter.STRUCTURAL_ANALYTICAL_MODEL);
                    if (parameter != null && !parameter.IsReadOnly)
                        parameter.Set(0);
                }

                trans.Commit();
            }
        }

    }
}
