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
    internal class BandingSel : RequestMethod
    {
        internal BandingSel(UIApplication _uiApp) : base(_uiApp)
        {
        }

        FamilySymbol FamilySymbol { get; set; }

        internal override void Execute()
        {
            //01 -结构梁
            Family fa = (new FilteredElementCollector(CMD.Doc))
                  .OfClass(typeof(Family))
                  .First(p => p.Name == "混凝土-矩形梁") as Family;
            List<FamilySymbol> familySymbols = fa.GetFamilySymbolIds().Select(p => CMD.Doc.GetElement(p) as FamilySymbol).ToList();

            //02- 求默认梁宽
            this.FamilySymbol = familySymbols.Where(_p => _p.Name == "矩形梁").FirstOrDefault();

            FamilyInstance tempFi = null;
            using (Transaction trans = new Transaction(this.doc, "-"))
            {
                trans.Start();
                if (!this.FamilySymbol.IsActive)
                {
                    this.FamilySymbol.Activate();
                }
                Line temp = Line.CreateBound(XYZ.Zero, XYZ.Zero + new XYZ(100, 0, 0));
                tempFi = CMD.Doc.Create.NewFamilyInstance(temp, this.FamilySymbol, CMD.Doc.ActiveView.GenLevel, StructuralType.Beam);
                trans.Commit();
            }
            double width = tempFi.LookupParameter("宽度").AsDouble();
            this.doc.DelEleId(tempFi.Id);

            // 寻找定位线
            Reference reference = this.sel.PickObject(ObjectType.Edge);
            Element element = CMD.Doc.GetElement(reference.ElementId);
            Edge edge = element.GetGeometryObjectFromReference(reference) as Edge;
            Curve curve = edge.AsCurve();

            FamilyInstance familyInstance = null;
            if (curve is Line line)
            {
                try
                {
                    if (line.Direction.Z.EqualZreo())
                    {
                        Face face = element.BottomPlanarFace();
                        List<Curve> curves = face.GetAllEdges().Select(p => p.AsCurve()).ToList();

                        //this.doc.CreateDirectShapeWithNewTransaction(curves);
                        //return;
                        //矩形梁的底面线圈不是按顺序排列的，需要使用几何算法，重新进行排序
                        CurveLoop curveLoop = CurveLoop.Create(curves);
                        if (ViewModel.Instance.BeamWidth > 1)
                        {
                            curveLoop = CurveLoop.CreateViaOffset(curveLoop, (ViewModel.Instance.BeamWidth * (0.5)).MilliMeterToFeet(), XYZ.BasisZ);
                        }
                        else
                        {
                            curveLoop = CurveLoop.CreateViaOffset(curveLoop, width * (0.5), XYZ.BasisZ);
                        }

                        Curve temp = curveLoop.ToList().OrderBy(p => p.Distance(reference.GlobalPoint)).First();

                        familyInstance = OpenTrans(temp);
                    }
                }
                catch (Exception)
                {
                    //throw;
                }
            }
            if (familyInstance == null)
            {
                using (Transaction trans = new Transaction(CMD.Doc, "-"))
                {
                    trans.DeleteErrOrWaringTaskDialog();
                    trans.Start();

                    if (!this.FamilySymbol.IsActive)
                    {
                        this.FamilySymbol.Activate();
                    }
                    familyInstance = CMD.Doc.Create.NewFamilyInstance(curve, this.FamilySymbol, CMD.Doc.ActiveView.GenLevel, StructuralType.Beam); ;

                    trans.Commit();
                }
            }
            SetLevelHW(familyInstance);
            //throw new NotImplementedException();
        }

        internal FamilyInstance OpenTrans(Curve curve)
        {
            FamilyInstance familyInstance = null;
            using (Transaction trans = new Transaction(CMD.Doc, "-"))
            {
                trans.DeleteErrOrWaringTaskDialog();
                trans.Start();

                if (!this.FamilySymbol.IsActive)
                {
                    this.FamilySymbol.Activate();
                }
                familyInstance = CMD.Doc.Create.NewFamilyInstance(curve, this.FamilySymbol, CMD.Doc.ActiveView.GenLevel, StructuralType.Beam); ;

                trans.Commit();
            }
            return familyInstance;
        }
        internal void SetLevelHW(FamilyInstance familyInstance)
        {
            using (Transaction trans = new Transaction(CMD.Doc, "修改标高"))
            {
                trans.DeleteErrOrWaringTaskDialog();
                trans.Start();

                if (ViewModel.Instance.BeamWidth > 10)
                {
                    familyInstance.LookupParameter("宽度").Set(ViewModel.Instance.BeamWidth.MilliMeterToFeet());
                }
                if (ViewModel.Instance.BeamHeight > 10)
                {
                    familyInstance.LookupParameter("高度").Set(ViewModel.Instance.BeamHeight.MilliMeterToFeet());
                }

                // 结构分析
                Parameter parameter = familyInstance.get_Parameter(BuiltInParameter.STRUCTURAL_ANALYTICAL_MODEL);
                if (parameter != null && !parameter.IsReadOnly)
                    parameter.Set(0);

                trans.Commit();
            }
        }
    }
}
