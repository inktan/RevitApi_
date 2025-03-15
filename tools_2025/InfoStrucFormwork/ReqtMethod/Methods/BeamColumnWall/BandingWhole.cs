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
    internal class BandingWhole : RequestMethod
    {
        internal BandingWhole(UIApplication _uiApp) : base(_uiApp)
        {
        }

        List<FamilySymbol> FamilySymbols { get; set; }
        Element Element { get; set; }
        Level Level { get; set; }

        internal override void Execute()
        {
            TransactionGroup transactionGroup = new TransactionGroup(CMD.Doc, "-圈梁-");
            transactionGroup.Start();

            Family fa = (new FilteredElementCollector(CMD.Doc))
           .OfClass(typeof(Family))
           .First(p => p.Name == "圈梁") as Family;
            FamilySymbols = fa.GetFamilySymbolIds().Select(p => CMD.Doc.GetElement(p) as FamilySymbol).ToList();

            // 寻找定位线
            //Reference reference = this.sel.PickObject(ObjectType.Element, new SelFloor());
            Reference reference = this.sel.PickObject(ObjectType.Element);
            this.Element = this.doc.GetElement(reference);

            this.Level = this.doc.GetElement(this.Element.LevelId) as Level;

            List<PlanarFace> faces = this.Element.BottomPlanarFaces().ToList();

            foreach (var item in faces)
            {
                foreach (var loop in item.GetEdgesAsCurveLoops())
                {
                    ComputeLoop(loop.ToList());
                }
            }

            transactionGroup.Assimilate();
        }
        internal void ComputeLoop(List<Curve> curves)
        {
            // 01 圈梁
            List<XYZ> xYZs = new List<XYZ>();
            foreach (var item in curves)
            {
                xYZs.Add(item.GetEndPoint(0));
            }

            Polygon2d polygon2d = new Polygon2d(xYZs.ToVector2ds());
            //polygon2d = polygon2d.InwardOffeet(200.0.MilliMeterToFeet());

            double z = xYZs.First().Z;
            // 计算合适的Z值
            if (this.Element.Name.Contains("CONC"))
            {
                if (this.Element.Name.Contains("FNSH"))
                {
                    z += (this.Element as Floor).FloorType.GetCompoundStructure().GetLayers()[1].Width;
                }
                else
                {
                    z += this.Element.LookupParameter("厚度").AsDouble();
                }
            }


            XYZ position = polygon2d.Bounds.Min.ToXYZ(z);

            this.FamilyInstances = new List<FamilyInstance>();
            using (Transaction transaction = new Transaction(CMD.Doc, "-圈梁-"))
            {
                transaction.Start();
                transaction.DeleteErrOrWaringTaskDialog();
                FamilyInstance familyInstance = PlaeUniverslHole(position, polygon2d);

                this.FamilyInstances.Add(familyInstance);

                transaction.Commit();
            }

            //throw new NotImplementedException();

            //$"{sw.ElapsedMilliseconds} + 毫秒".TaskDialogErrorMessage();
        }

        List<FamilyInstance> FamilyInstances { get; set; }
        List<ElementId> ElementIds { get; set; }

        /// <summary>
        /// 放置圈梁
        /// </summary>
        /// <returns></returns>
        FamilyInstance PlaeUniverslHole(XYZ position, Polygon2d polygon2d)
        {
            FamilySymbol symbol = FamilySymbols.First().Duplicate(FamilySymbols.First().FamilyName + Guid.NewGuid().ToString()) as FamilySymbol;

            symbol.Name = "圈梁 " + symbol.Id.ToString();

            symbol.Activate();
            FamilyInstance familyInstance;

            Set16Points(position, symbol, polygon2d);

            familyInstance = CMD.Doc.Create.NewFamilyInstance(position, symbol, Level, StructuralType.Beam);

            familyInstance.LookupParameter("偏移x_2_轮廓").Set(0.0.MilliMeterToFeet());
            familyInstance.LookupParameter("偏移y_2_轮廓").Set(0.0.MilliMeterToFeet());

            if (ViewModel.Instance.BeamWidth > 1)
            {
                familyInstance.LookupParameter("梁宽").Set(ViewModel.Instance.BeamWidth.MilliMeterToFeet());
            }
            if (ViewModel.Instance.BeamHeight > 1)
            {
                familyInstance.LookupParameter("梁高").Set(ViewModel.Instance.BeamHeight.MilliMeterToFeet());
            }



            //if (this.Element is Floor)
            //{
            //    double temp = Math.Abs(this.Element.LookupParameter("自标高的高度偏移").AsDouble());
            //    symbol.LookupParameter("尺寸y_2_轮廓").Set(temp);
            //    // 设置圈梁的线圈高低偏移值
            //    symbol.LookupParameter("偏移y_2_轮廓").Set(0.0);

            //    familyInstance.LookupParameter("主体中的偏移").Set(temp * 0.5);
            //    return familyInstance;
            //}

            //if (this.Element.LookupParameter("板顶与基面距离").AsDouble() < 0)
            //{
            //    // 设置圈梁的自身高度
            //    double temp = this.Element.LookupParameter("自标高的高度偏移").AsDouble();
            //    symbol.LookupParameter("尺寸y_2_轮廓").Set(temp);
            //    // 设置圈梁的线圈高低偏移值
            //    symbol.LookupParameter("偏移y_2_轮廓").Set(temp * 0.5);
            //}
            //else
            //{
            //    // 设置圈梁的自身高度
            //    double temp = this.Element.LookupParameter("板顶与基面距离").AsDouble();
            //    symbol.LookupParameter("尺寸y_2_轮廓").Set(temp);
            //    // 设置圈梁的线圈高低偏移值
            //    symbol.LookupParameter("偏移y_2_轮廓").Set(temp * (-0.5) + this.Element.LookupParameter("建筑板厚度").AsDouble());
            //}


            return familyInstance;
        }
        /// <summary>
        /// 设置控制点坐标
        /// </summary>
        void Set16Points(XYZ position, FamilySymbol familySymbol, Polygon2d polygon2d)
        {

            List<Vector2d> points = polygon2d.Vertices.ToList();
            if (points.Count < 16)
            {
                Vector2d p_end = points.Last();
                Vector2d p_start = points.First();

                Segment2d segment2d = new Segment2d(p_end, p_start);
                int count = 16 - points.Count;
                double length = segment2d.Length / (count + 1);

                for (int i = 1; i < count + 1; i++)
                {
                    points.Add(p_end + segment2d.Direction * i * length);
                }
            }

            //Polygon2d pol = new Polygon2d(points);
            //CMD.Doc.CreateDirectShape(pol.ToCurveLoop().ToList());

            for (int i = 1; i <= 16; i++)
            {
                Parameter parameter = familySymbol.LookupParameter("x" + i + "_1_路径");
                if (!parameter.IsReadOnly)
                {
                    parameter.Set(points[i - 1].x - position.X);
                }
                parameter = familySymbol.LookupParameter("y" + i + "_1_路径");
                if (!parameter.IsReadOnly)
                {
                    parameter.Set(points[i - 1].y - position.Y);
                }
            }
        }
    }
}
