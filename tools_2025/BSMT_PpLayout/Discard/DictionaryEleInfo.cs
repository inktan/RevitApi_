//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//using Autodesk.Revit.DB;
//using ClipperLib;
//using goa.Common.g3InterOp;
//using goa.Common;
//using g3;
//using PubFuncWt;

//namespace BSMT_PpLayout
//{

//    using cInt = Int64;
//    using Path = List<IntPoint>;
//    using Paths = List<List<IntPoint>>;
//    using Path_xyz = List<XYZ>;
//    using Paths_xyz = List<List<XYZ>>;

//    /// <summary>
//    /// 基于 BoundingBox 创建 Dictionary ,元素id与元素对应的polygon2d
//    /// </summary>
//    class DictionaryEleInfo
//    {
//        #region 需要增加一个 g3 字典处理
//        private Dictionary<string, Element> _dicIdvsEles;// elementId Vs element
//        internal Dictionary<string, Element> DicIdvsEles { get { return _dicIdvsEles; } }

//        private Dictionary<string, Polygon2d> _dicIdvsPolygon2ds;// elementId Vs g3_plygon2D
//        internal Dictionary<string, Polygon2d> DicIdvsPolygon2ds { get { return _dicIdvsPolygon2ds; } }
//        #endregion

//        internal DictionaryEleInfo(List<Element> allEles, View view)
//        {
//            CreatDicIdvs(allEles, view);
//        }

//        /// <summary>
//        /// 将元素列表转化为 eleIdString 与 element Path_xyz CurveArray 的字典
//        /// </summary>
//        /// <returns></returns>
//        public void CreatDicIdvs(List<Element> allEles, View view)
//        {
//            Dictionary<string, Element> keyValueIdvsEles = new Dictionary<string, Element>();
//            Dictionary<string, Polygon2d> keyValueIdvsPolygon2ds = new Dictionary<string, Polygon2d>();

//            #region 拿出信息字典，key为elementId string

//            foreach (Element ele in allEles)
//            {
//                if (!ele.IsValidObject) continue;
//                BoundingBoxXYZ boundingBoxXYZ = ele.get_BoundingBox(view);
//                if (boundingBoxXYZ == null) continue;

//                if (ele is FamilyInstance)//族实例 默认放置位置为当前视图的切割平面，二维停车位详图族实例的放置位置位于剖切面，剖切面位置高于视图标高
//                {
//                    if (ele.OwnerViewId != view.Id)//根据所属视图进行判断
//                        continue;
//                }
//                else if (ele is Group)//根据物体 Z 值，进行判断，之所以不用OwnerViewId进行判断，源于在小于0.0标高视图创建的模型线组的参照标高默认为0.0标高
//                {
//                    if (Math.Abs(boundingBoxXYZ.Min.Z - view.GenLevel.ProjectElevation) > Precision_.Precison)//找到当前可见视图元素的 z 值，该处需要注意Revit的数值比较尽量不使用绝对相等进行对比
//                        continue;
//                }
//                List<Curve> curves= boundingBoxXYZ.BottomLines().Cast<Curve>().ToList();

//                #region 字典赋值
//                string strEldId = ele.Id.ToString();
//                keyValueIdvsEles[strEldId] = ele;
//                keyValueIdvsPolygon2ds[strEldId] = curves.ToPolygon2d();
//                #endregion
//            }

//            #region 字段赋值
//            this._dicIdvsEles = keyValueIdvsEles;
//            this._dicIdvsPolygon2ds = keyValueIdvsPolygon2ds;
//            #endregion

//            #endregion


//        }
//    }
//}
