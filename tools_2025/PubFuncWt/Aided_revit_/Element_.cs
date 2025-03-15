using Autodesk.Revit.DB;
using goa.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PubFuncWt
{
    public static class Element_
    {
        /// <summary>
        /// 当前考虑的都是单线圈
        /// </summary>
        /// <param name="ele"></param>
        /// <returns></returns>
        public static List<Curve> SketchLines(this Element ele)
        {
            Document doc = ele.Document;
            ElementClassFilter filter = new ElementClassFilter(typeof(CurveElement));
            List<ElementId> dependentIds = ele.GetDependentElements(filter).ToList();
            var curves = dependentIds
                .Select(x => (doc.GetElement(x).Location as LocationCurve).Curve)
                .ToList();
            return curves;
        }
        /// <summary>
        /// 获取元素垂直向上的水平面
        /// </summary>
        /// <param name="_elem"></param>
        /// <returns></returns>
        public static PlanarFace TopPlanarFace(this Element _elem)
        {
            List<Solid> solids = new List<Solid>();
            if (_elem is FilledRegion)
            {
                Options options = new Options();
                options.ComputeReferences = true;
                GeometryElement geometryElement = _elem.get_Geometry(options);
                foreach (GeometryObject geomObj in geometryElement)
                {
                    if (geomObj is Solid)
                    {
                        solids.Add(geomObj as Solid);
                    }
                }
            }
            else
            {
                solids = _elem.GetAllSolids();
            }

            foreach (var p in solids.SelectMany(p0 => p0.Faces.ToIEnumerable()))
            {
                if (p is PlanarFace)
                {
                    PlanarFace planarFace = p as PlanarFace;
                    if (planarFace.IsFaceVerticalUp())// 找到垂直向上的水平面
                    {
                        return planarFace;
                    }
                }
            }
            return null;
        }
        public static IEnumerable<PlanarFace> TopPlanarFaces(this Element _elem)
        {
            List<Solid> solids = new List<Solid>();
            if (_elem is FilledRegion)
            {
                Options options = new Options();
                options.ComputeReferences = true;
                GeometryElement geometryElement = _elem.get_Geometry(options);
                foreach (GeometryObject geomObj in geometryElement)
                {
                    if (geomObj is Solid)
                    {
                        solids.Add(geomObj as Solid);
                    }
                }
            }
            else
            {
                solids = _elem.GetAllSolids();
            }

            foreach (var p in solids.SelectMany(p0 => p0.Faces.ToIEnumerable()))
            {
                if (p is PlanarFace)
                {
                    PlanarFace planarFace = p as PlanarFace;
                    if (planarFace.IsFaceVerticalUp())// 找到垂直向上的水平面
                    {
                        yield return planarFace;
                    }
                }
            }
            //return null;
        }
        /// <summary>
        /// 获取元素垂直向上的水平面
        /// </summary>
        /// <param name="_elem"></param>
        /// <returns></returns>
        public static IEnumerable<PlanarFace> BottomPlanarFaces(this Element _elem)
        {
            List<Solid> solids = new List<Solid>();
            if (_elem is FilledRegion)
            {
                Options options = new Options();
                options.ComputeReferences = true;
                GeometryElement geometryElement = _elem.get_Geometry(options);
                foreach (GeometryObject geomObj in geometryElement)
                {
                    if (geomObj is Solid)
                    {
                        solids.Add(geomObj as Solid);
                    }
                }
            }
            else
            {
                solids = _elem.GetAllSolids();
            }

            foreach (var p in solids.SelectMany(p0 => p0.Faces.ToIEnumerable()))
            {
                if (p is PlanarFace)
                {
                    PlanarFace planarFace = p as PlanarFace;
                    if (planarFace.IsFaceVerticalDown())// 找到垂直向上的水平面
                    {
                        yield return planarFace;
                    }
                }
            }
            //return null;
        }
        public static PlanarFace BottomPlanarFace(this Element _elem)
        {
            List<Solid> solids = new List<Solid>();
            if (_elem is FilledRegion)
            {
                Options options = new Options();
                options.ComputeReferences = true;
                GeometryElement geometryElement = _elem.get_Geometry(options);
                foreach (GeometryObject geomObj in geometryElement)
                {
                    if (geomObj is Solid)
                    {
                        solids.Add(geomObj as Solid);
                    }
                }
            }
            else
            {
                solids = _elem.GetAllSolids();
            }

            foreach (var p in solids.SelectMany(p0 => p0.Faces.ToIEnumerable()))
            {
                if (p is PlanarFace)
                {
                    PlanarFace planarFace = p as PlanarFace;
                    if (planarFace.IsFaceVerticalDown())// 找到垂直向上的水平面
                    {
                        return planarFace;
                    }
                }
            }
            return null;
        }
        /// <summary>
        /// 返回图元形体所有朝上的非曲面
        /// </summary>
        /// <param name="_elem"></param>
        /// <returns></returns>
        public static List<PlanarFace> GetAllFacesFacingUp(this Element _elem)
        {
            List<PlanarFace> faces = GetAllPlanarFaces(_elem);
            List<PlanarFace> facingUpFaces = new List<PlanarFace>();
            foreach (var f in faces)
            {
                bool facingUp = f.IsFaceFacingUp();
                if (facingUp)
                {
                    facingUpFaces.Add(f);
                }
            }
            return facingUpFaces;
        }
        /// <summary>
        /// 返回图元形体所有朝下的非曲面
        /// </summary>
        /// <param name="_elem"></param>
        /// <returns></returns>
        public static List<PlanarFace> GetAllFacesFacingDown(this Element _elem)
        {
            List<PlanarFace> faces = GetAllPlanarFaces(_elem);
            List<PlanarFace> facingDownFaces = new List<PlanarFace>();
            foreach (var f in faces)
            {
                bool facingUp = (f as PlanarFace).IsFaceFacingUp();
                if (!facingUp)
                {
                    facingDownFaces.Add(f);
                }
            }
            return facingDownFaces;
        }

        /// <summary>
        /// 返回图元形体的所有非曲面
        /// </summary>
        /// <param name="_elem"></param>
        /// <returns></returns>
        public static List<PlanarFace> GetAllPlanarFaces(Element _elem)
        {
            var solids = _elem.GetAllSolids();
            var faces = solids.SelectMany(x => x.Faces.Cast<Face>())
                .Where(x => x is PlanarFace)
                .Cast<PlanarFace>()
                .ToList();
            return faces;
        }

        #region ElementIds Elements Convert 
        /// <summary>
        /// 元素Id列表转换为元素列表
        /// </summary>
        public static IEnumerable<Element> ToElements(this IEnumerable<ElementId> _EleIds, Document doc)
        {
            List<Element> _Eles = new List<Element>();
            foreach (ElementId _EleId in _EleIds)
            {
                Element _Ele = doc.GetElement(_EleId);
                _Eles.Add(_Ele);
            }
            return _Eles;
        }
        /// <summary>
        /// 元素列表转换为元素Id列表
        /// </summary>
        public static List<ElementId> ToElementIds(this IEnumerable<Element> _Eles)
        {
            List<ElementId> _EleIds = new List<ElementId>();
            foreach (Element _Ele in _Eles)
            {
                _EleIds.Add(_Ele.Id);
            }
            return _EleIds;
        }
        #endregion
    }
}
