using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.Revit.DB;

namespace _new
{
    class _CreateDimensionsBasedGMs
    {
        internal Document document { get; set; }
        private Color _color = new Color(0, 0, 0);
        internal Color color { get { return _color; } set { _color = value; } }
        internal DimensionType dimensionType { get; set; }

        internal _CreateDimensionsBasedGMs(Document document)
        {
            this.document = document;
        }
        /// <summary>
        /// 输入行参：familyInstances-所有常规模型, minDistance-最小距离, _above-尺寸标注上部文字, _below-尺寸标注底部文字, _prefix-前缀, _suffix-后缀
        /// </summary>
        /// <param name="familyInstances"></param>
        /// <param name="minDistance"></param>
        /// <param name="_above"></param>
        /// <param name="_below"></param>
        /// <param name="_prefix"></param>
        /// <param name="_suffix"></param>
        internal void Execute(List<FamilyInstance> familyInstances, double minDistance, string _above, string _below, string _prefix, string _suffix)
        {
            if (familyInstances.Count >= 2)
            {
                GetDimensions(this.document, familyInstances, minDistance, _above, _below, _prefix, _suffix);
            }
        }
        /// <summary>
        /// 对多个塔楼的相邻对边进行标注计算
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="filledRegions"></param>
        internal void GetDimensions(Document doc, List<FamilyInstance> familyInstances, double minDistance, string _above, string _below, string _prefix, string _suffix)
        {
            int count = familyInstances.Count;
            for (int i = 0; i < count; i++)
            {
                List<Face> fourFaces01 = FindFourFaces(familyInstances[i], doc).ToList();
                for (int j = 0; j < count; j++)
                {
                    if (j > i)
                    {
                        IEnumerable<Face> fourFaces02 = FindFourFaces(familyInstances[j], doc);
                        fourFaces01.AddRange(fourFaces02);
                        //【】对8个面进行分析，基于平行关系进行分组
                        List<List<Face>> faceses = FindParallelPlane(fourFaces01);
                        foreach (List<Face> faces in faceses)
                        {
                            int _count = faces.Count();
                            List<Dimension> dimensions = new List<Dimension>();
                            for (int _i = 0; _i < _count; _i++)
                            {
                                for (int _j = 0; _j < _count; _j++)
                                {
                                    if (_j > _i)
                                    {
                                        if (faces[_i].Reference.ElementId == faces[_j].Reference.ElementId)
                                            continue;
                                        Dimension dimension = GetDimension(faces[_i], faces[_j], doc);
                                        dimensions.Add(dimension);
                                    }
                                }
                            }
                            // 需要对距离进行停车可行性分析
                            HandleDimensions(dimensions, minDistance, _above, _below, _prefix, _suffix);
                        }
                    }
                }
            }
        }
        /// <summary>
        /// 规范可行性分析
        /// </summary>
        internal void HandleDimensions(List<Dimension> dimensions, double minDistance, string _above, string _below, string _prefix, string _suffix)
        {
            Document doc = this.document;
            //【1】取出最小值，删除其余尺寸标注实例
            dimensions = dimensions.OrderBy(p => p.Value).ToList();//由小变大
            Dimension minDimension = dimensions.First();

            int dimensionsCount = dimensions.Count;
            IEnumerable<ElementId> delDimensionIds = dimensions.GetRange(1, dimensionsCount - 1).Select(p => p.Id);
            using (Transaction trans = new Transaction(doc, "删除多余尺寸标注"))
            {
                trans.Start();
                doc.Delete(delDimensionIds.ToList());
                trans.Commit();
            }
            //【2】对标注进一步处理
            if (minDimension.Value > UnitUtils.ConvertToInternalUnits(minDistance, DisplayUnitType.DUT_MILLIMETERS))
            {
                using (Transaction trans = new Transaction(doc, "删除符合规范的标注"))
                {
                    trans.Start();
                    doc.Delete(minDimension.Id);
                    trans.Commit();
                }
            }
            else
            {
                using (Transaction trans = new Transaction(doc, "对不符合规范的标注进行注释"))
                {
                    trans.Start();

                    string _Above = _above;// 上部
                    string _Below = _below;// 底部
                    string _Prefix = _prefix;// 前缀
                    string _Suffix = _suffix;// 后缀

                    minDimension.Above = _Above;
                    minDimension.Below = _Below;
                    minDimension.Prefix = _Prefix;
                    minDimension.Suffix = _Suffix;

                    OverrideGraphicSettings ogs = new OverrideGraphicSettings();//设置投影线、截面线颜色
                    ogs.SetProjectionLineColor(this._color);// 修改颜色
                    doc.ActiveView.SetElementOverrides(minDimension.Id, ogs);

                    trans.Commit();
                }
            }
        }
        /// <summary>
        /// 对8个面基于平行关系 分成列表
        /// </summary>
        /// <param name="twoFaces"></param>
        /// <returns></returns>
        internal List<List<Face>> FindParallelPlane(List<Face> eightFaces)
        {
            List<Face> faces01 = new List<Face>();
            List<Face> faces02 = new List<Face>();

            Face face01 = eightFaces.First();
            XYZ normalXyz = (face01 as PlanarFace).FaceNormal;
            foreach (Face face in eightFaces)
            {
                XYZ _normalXyz = (face as PlanarFace).FaceNormal;
                double dotResult = normalXyz.DotProduct(_normalXyz);
                if (EqualPrecision(dotResult, 1) || EqualPrecision(dotResult, -1))
                {
                    faces01.Add(face);
                }
                else
                {
                    faces02.Add(face);
                }
            }
            // 求平行且距离最短的两个对面faces
            return new List<List<Face>>() { faces01, faces02 };
        }

        /// <summary>
        /// 获取非顶面的四边面 目标优化：由矩形到多边形的过渡
        /// </summary>
        /// <returns></returns>
        internal IEnumerable<Face> FindFourFaces(FamilyInstance _familyInstance, Document doc)
        {
            List<Face> faces = Faces(_familyInstance, doc.ActiveView);

            // 找到非顶面的face
            foreach (Face face in faces)
            {
                XYZ faceNormal = (face as PlanarFace).FaceNormal;
                if (!EqualPrecision(Math.Abs(faceNormal.Z), 1))
                    yield return face;
            }
        }
        /// <summary>
        /// 基于reference创建尺寸标注
        /// </summary>
        /// <returns></returns>
        internal Dimension GetDimension(Face face01, Face face02, Document doc)
        {
            ReferenceArray referenceArray = new ReferenceArray();
            referenceArray.Append((face01 as PlanarFace).Reference);
            referenceArray.Append((face02 as PlanarFace).Reference);

            Element element01 = doc.GetElement(face01.Reference.ElementId);
            Element element02 = doc.GetElement(face02.Reference.ElementId);
            XYZ locationPoint01 = (element01.Location as LocationPoint).Point;
            XYZ locationPoint02 = (element02.Location as LocationPoint).Point;

            Line dimensionLine = Line.CreateBound(locationPoint01, locationPoint02);
            Dimension dimension = null;
            using (Transaction trans = new Transaction(doc, "添加塔楼间距标注"))
            {
                trans.Start();
                if (this.dimensionType == null)
                {
                    dimension = doc.Create.NewDimension(doc.ActiveView, dimensionLine, referenceArray);
                }
                else
                {
                    dimension = doc.Create.NewDimension(doc.ActiveView, dimensionLine, referenceArray, this.dimensionType);
                }
                trans.Commit();
            }
            return dimension;
        }

        #region Revit                 
        /// <summary>
        /// 获取FamilyInstance的Edges
        /// </summary>
        public static List<Face> Faces(FamilyInstance familyInstance, View _view)
        {
            Options options = new Options();
            options.View = _view;
            options.ComputeReferences = true;
            GeometryInstance geometryInstance = familyInstance.get_Geometry(options).First() as GeometryInstance;
            GeometryElement geometryElement = geometryInstance.GetSymbolGeometry();
            foreach (var item in geometryElement)
            {
                if (item is Solid)
                {
                    Solid solid = item as Solid;
                    return ToIEnumerable(solid.Faces).ToList();
                }
            }
            return new List<Face>();
        }
        public static IEnumerable<Face> ToIEnumerable(FaceArray faceArray)
        {
            foreach (var item in faceArray)
            {
                yield return item as Face;
            }
        }

        #endregion

        #region 数据判断
        /// <summary>
        /// 判断两个数据是否相等 误差精确度设置为 1e-6
        /// </summary>
        /// <returns></returns>
        public bool EqualPrecision(double _d01, double _d02)
        {
            if (Math.Abs(_d01 - _d02) <= 1e-6)
                return true;
            else
                return false;
        }
        #endregion
    }
}
