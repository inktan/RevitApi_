using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Selection;

using goa.Common;
using goa.Common.Exceptions;

namespace DimensioningTools
{
    internal  class WindowSpotElevDim : RequestMethod
    {
        internal WindowSpotElevDim(UIApplication _uiApp) : base(_uiApp)
        {
        }
        internal class SelectionLinkFilter : ISelectionFilter
        {
            public RevitLinkInstance instance = null;
            public bool AllowElement(Element elem)
            {
                instance = elem as RevitLinkInstance;
                if (instance != null)
                {
                    return true;
                }
                return false;
            }
            public bool AllowReference(Reference reference, XYZ position)
            {
                if (instance == null)
                {
                    return false;
                }
                else
                {
                    Document linkDoc = instance.GetLinkDocument();
                    Element element = linkDoc.GetElement(reference.LinkedElementId);
                    return element is FamilyInstance;
                }
            }
        }

        internal override void Execute()
        {
            //获取当前文档
            UIDocument uidoc = goa.Common.APP.UIApp.ActiveUIDocument;
            Document doc = uidoc.Document;
            Selection sel = uidoc.Selection;
            View view = doc.ActiveView;
            //获取所选元素集
            IList<Reference> refElems = sel.PickObjects(ObjectType.LinkedElement, new SelectionLinkFilter());
            foreach(Reference refElem in refElems)
            {
                Element elem = doc.GetElement(refElem);
                RevitLinkInstance linkInst = elem as RevitLinkInstance;
                Document linkDoc = linkInst.GetLinkDocument();
                //获取链接模型中的构件
                Element linkElem = linkDoc.GetElement(refElem.LinkedElementId);
                List<PlanarFace> faceList = new List<PlanarFace>();
                Options opt = new Options();
                opt.ComputeReferences = true;
                //获取所选元素的几何元素
                GeometryElement geoElem = linkElem.get_Geometry(opt);
                foreach (GeometryObject xx in geoElem)
                {
                    GeometryInstance geoIns = xx as GeometryInstance;
                    if (null != geoIns)
                    {
                        foreach (GeometryObject instObj in geoIns.GetInstanceGeometry())
                        {
                            Solid solid = instObj as Solid;
                            //将空实体，没有面也没有边的实体剔除
                            if (null == solid || 0 == solid.Faces.Size || 0 == solid.Edges.Size) continue;

                            //从实体获取面转换形成的点
                            foreach (Face face in solid.Faces)
                            {
                                PlanarFace planarFace = face as PlanarFace;
                                if (planarFace == null) continue;
                                //确保垂直向上或向下
                                if (planarFace.FaceNormal.IsAlmostEqualTo(new XYZ(0, 0, -1)) || planarFace.FaceNormal.IsAlmostEqualTo(new XYZ(0, 0, 1)))
                                {
                                    faceList.Add(planarFace);
                                }
                                faceList.Add(planarFace);
                            }
                        }
                    }
                }

                Transform transform = linkInst.GetTotalTransform();
                XYZ origin = transform.Origin;
                if ((linkElem as FamilyInstance).Category.Name == "窗")
                {
                    //找到原点z坐标最接近窗底的面           
                    double bottomZ = GetBottom(linkElem);
                    IList<PlanarFace> bfList = FindFace(bottomZ, faceList);
                    XYZ bottomO = bfList[0].Origin + origin;
                    Reference bRef = (linkElem as FamilyInstance).GetReferences(FamilyInstanceReferenceType.Bottom).FirstOrDefault().CreateLinkReference(linkInst);
                    //创建底部高程点
                    using (Transaction tran = new Transaction(doc, "CreateSpotDimension"))
                    {
                        tran.Start();
                        SpotDimension bottomSd = doc.Create.NewSpotElevation(view, bRef, bottomO, bottomO, bottomO, bottomO, false);
                        tran.Commit();
                    }
                }


                if ((linkElem as FamilyInstance).Category.Name == "窗" || (linkElem as FamilyInstance).Category.Name == "门")
                {
                    //找到原点z坐标最接近窗顶的面
                    double topZ = GetTop(linkElem);
                    IList<PlanarFace> tfList = FindFace(topZ, faceList);
                    XYZ topO = tfList[0].Origin + origin;
                    Reference tRef = (linkElem as FamilyInstance).GetReferences(FamilyInstanceReferenceType.Top).FirstOrDefault().CreateLinkReference(linkInst);
                    //创建顶部高程点
                    using (Transaction tran = new Transaction(doc, "CreateSpotDimension"))
                    {
                        tran.Start();
                        SpotDimension topSd = doc.Create.NewSpotElevation(view, tRef, topO, topO, topO, topO, false);
                        tran.Commit();
                    }
                }
            }
        }
       
        private static IList<PlanarFace> FindFace(double _zValue, List<PlanarFace> _faces)
        {
            IList<PlanarFace> pfList = new List<PlanarFace>();
            foreach (PlanarFace plFace in _faces)
            {
                if (plFace!=null && Math.Abs(plFace.Origin.Z - _zValue) < 0.00001)
                {
                    pfList.Add(plFace);
                }
                else continue;
            }
            return pfList;
        }
        private static double GetTop(Element elem)
        {
            Parameter para = elem.get_Parameter(BuiltInParameter.INSTANCE_HEAD_HEIGHT_PARAM);
            Level level = elem.Document.GetElement(elem.LevelId) as Level;
            double lHigh = level.Elevation;
            double topHigh = para.AsDouble()+lHigh;
            return topHigh;
        }
        private static double GetBottom(Element elem)
        {
            Parameter para = elem.get_Parameter(BuiltInParameter.INSTANCE_SILL_HEIGHT_PARAM);
            Level level = elem.Document.GetElement(elem.LevelId) as Level;
            double lHigh = level.Elevation;
            double bottomHigh = para.AsDouble()+lHigh;
            return bottomHigh;
        }
    }
}
