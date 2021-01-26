using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PublicProjectMethods_
{
    public static class FamilyInstance_
    {
        #region 放置二维族实例
        /// <summary>
        /// 放置多个二维族实例
        /// </summary>
        public static List<FamilyInstance> Place2DFamilyInstancesWithTrans(this IEnumerable<XYZ> xYZs, Document doc, View view, FamilySymbol familySymbol)
        {
            List<FamilyInstance> familyInstances = new List<FamilyInstance>();
            using (Transaction palceFs = new Transaction(doc))
            {
                palceFs.Start("placeParkingPlace");
                if (!familySymbol.IsActive)//判断族类型是否被激活
                {
                    familySymbol.Activate();
                }
                foreach (XYZ xYZ in xYZs)
                {
                    FamilyInstance parkingPlace = doc.Create.NewFamilyInstance(xYZ, familySymbol, view);
                    familyInstances.Add(parkingPlace);
                }
                palceFs.Commit();
            }
            return familyInstances;
        }
        /// <summary>
        /// 放置单个二维族实例
        /// </summary>
        public static FamilyInstance Place2DFamilyInstanceWithTrans(this XYZ xYZ, Document doc, View view, FamilySymbol familySymbol)
        {
            FamilyInstance fi = null;
            using (Transaction palceFs = new Transaction(doc))
            {
                palceFs.Start("placeParkingPlace");
                if (!familySymbol.IsActive)//判断族类型是否被激活
                {
                    familySymbol.Activate();
                }
                fi = doc.Create.NewFamilyInstance(xYZ, familySymbol, view);
                palceFs.Commit();
            }
            return fi;
        }
        #endregion
        #region Rotate
        /// <summary>
        /// 旋转族实例, 旋转轴线段方向为Z轴正方向，当前测试为顺时针旋转
        /// </summary>
        public static void RotateWithTrans(this List<FamilyInstance> familyInstances, Document doc, double _angle)
        {
            using (Transaction _rotateTrans = new Transaction(doc))
            {
                _rotateTrans.Start("_rotateTrans");
                foreach (FamilyInstance fs in familyInstances)
                {
                    LocationPoint _point = fs.Location as LocationPoint;
                    if (_point != null)
                    {
                        XYZ aa = _point.Point;
                        XYZ cc = _point.Point + new XYZ(0, 0, 1);
                        Line _axis = Line.CreateBound(aa, cc);
                        _point.Rotate(_axis, _angle);
                    }
                }
                _rotateTrans.Commit();
            }
        }
        /// <summary>
        /// 旋转族实例, 旋转轴线段方向为Z轴正方向，当前测试为顺时针旋转
        /// </summary>
        public static void Rotate(this List<FamilyInstance> familyInstances, double _angle)
        {
            foreach (FamilyInstance fs in familyInstances)
            {
                LocationPoint _point = fs.Location as LocationPoint;
                if (_point != null)
                {
                    XYZ aa = _point.Point;
                    XYZ cc = _point.Point + new XYZ(0, 0, 1);
                    Line _axis = Line.CreateBound(aa, cc);
                    _point.Rotate(_axis, _angle);
                }
            }
        }
        /// <summary>
        /// 旋转族实例, 旋转轴线段方向为Z轴正方向，当前测试为顺时针旋转-包含事务
        /// </summary>
        public static void RotateWithTrans(this FamilyInstance fs, Document doc, double _angle)
        {
            using (Transaction _rotateTrans = new Transaction(doc))
            {
                _rotateTrans.Start("_rotateTrans");
                LocationPoint _point = fs.Location as LocationPoint;
                if (_point != null)
                {
                    XYZ aa = _point.Point;
                    XYZ cc = _point.Point + new XYZ(0, 0, 1);
                    Line _axis = Line.CreateBound(aa, cc);
                    _point.Rotate(_axis, _angle);
                }
                _rotateTrans.Commit();
            }
        }
        /// <summary>
        /// 旋转族实例, 旋转轴线段方向为Z轴正方向，当前测试为顺时针旋转-不包含事务
        /// </summary>
        public static void Rotate(this FamilyInstance fs, double _angle)
        {
            LocationPoint _point = fs.Location as LocationPoint;
            if (_point != null)
            {
                XYZ aa = _point.Point;
                XYZ cc = _point.Point + new XYZ(0, 0, 1);
                Line _axis = Line.CreateBound(aa, cc);
                _point.Rotate(_axis, _angle);
            }
        }
        #endregion
        #region FamilyInstance Face
        /// <summary>
        /// 获取FamilyInstance的Edges
        /// </summary>
        public static List<Face> Faces(this FamilyInstance familyInstance, View _view)
        {
            Options options = new Options();
            options.View = _view;
            options.ComputeReferences = true;
            GeometryInstance geometryInstance = familyInstance.get_Geometry(options).First() as GeometryInstance;
            // 这里需要明确参照几何的来源
            // 使用symbolGeometry是族类型的几何引用
            GeometryElement geometryElement = geometryInstance.GetSymbolGeometry();
            // 使用instanceGeometry是族实例的几何信息，但是无法提取reference
            //GeometryElement geometryElement = geometryInstance.GetInstanceGeometry();
            foreach (var item in geometryElement)
            {
                if (item is Solid)
                {
                    Solid solid = geometryElement.First() as Solid;
                    return solid.Faces.ToIEnumerable().ToList();
                }
            }
            return new List<Face>();
        }
        /// <summary>
        /// 获取FamilyInstance的Edges
        /// </summary>
        public static List<Edge> Edges(this FamilyInstance familyInstance, View _view)
        {
            Options options = new Options();
            options.View = _view;
            options.ComputeReferences = true;
            GeometryInstance geometryInstance = familyInstance.get_Geometry(options).First() as GeometryInstance;
            // 这里需要明确参照几何的来源
            //GeometryElement geometryElement = geometryInstance.GetSymbolGeometry();
            GeometryElement geometryElement = geometryInstance.GetInstanceGeometry();
            foreach (var item in geometryElement)
            {
                if (item is Solid)
                {
                    Solid solid = geometryElement.First() as Solid;
                    return solid.Edges.ToIEnumerable().ToList();
                }
            }
            return new List<Edge>();
        }
        #endregion

        /// <summary>
        /// 判断族实例是否镜像 默认 HandOrientation 逆时针旋转90°为 FacingOrientation
        /// </summary>
        /// <param name="familyInstance"></param>
        /// <returns></returns>
        public static bool IsMirriored(this FamilyInstance familyInstance)
        {
            XYZ facingOrientation = familyInstance.FacingOrientation;
            XYZ handOrientation = familyInstance.HandOrientation;

            Transform transform = Transform.CreateRotation(new XYZ(0, 0, 1), Math.PI / 2);

            // 将hanging逆时针旋转90

            XYZ _hand = transform.OfPoint(handOrientation);

            double distance = facingOrientation.DistanceTo(_hand);

            if (distance < Precision_.Precison)
                return false;
            else
                return true;

        }
        /// <summary>
        /// 族实例的原地镜像平面 如果是镜像关系，则得到的transform的baxicy与facing反向
        /// </summary>
        /// <param name="familyInstance"></param>
        /// <returns></returns>
        public static Plane MirrorPlane(this FamilyInstance familyInstance)
        {
            // 粗略归纳为 transform 只是忽略了facing的镜像，对应 new xyz（0,1,0）
            XYZ normal = new XYZ(0, 1, 0);
            XYZ origin = XYZ.Zero;
            Transform transform = familyInstance.GetTransform();
            XYZ _normal = transform.OfPoint(normal);
            XYZ _origin = transform.OfPoint(origin);

            return Plane.CreateByNormalAndOrigin(_normal - _origin, _origin);
        }

    }
}
