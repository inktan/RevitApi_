using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PubFuncWt         
{
    public static class FamilyInstance_
    {
        #region 放置二维族实例
        /// <summary>
        /// 放置多个二维族实例
        /// </summary>
        public static List<FamilyInstance> Place2DFamilyInstancesWithTrans(this IEnumerable<XYZ> xYZs, Document doc, View view,FamilySymbol familySymbol)
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
            FamilyInstance fi  = null;
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
        public static void RotateWithTrans(this IEnumerable<FamilyInstance> familyInstances, Document doc, double _angle)
        {
            using (Transaction _rotateTrans = new Transaction(doc))
            {
                _rotateTrans.Start("_rotateTrans");
                familyInstances.Rotate(_angle);
                _rotateTrans.Commit();
            }
        }

        /// <summary>
        /// 旋转族实例, 旋转轴线段方向为Z轴正方向，当前测试为顺时针旋转
        /// </summary>
        public static void Rotate(this IEnumerable<FamilyInstance> familyInstances, double _angle)
        {
            foreach (FamilyInstance fs in familyInstances)
            {
                fs.Rotate(_angle);
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
                fs.Rotate(_angle);
                _rotateTrans.Commit();
            }
        }
        public static void RotateWithTrans(this FamilyInstance fs, Document doc, XYZ origin, double _angle)
        {
            using (Transaction _rotateTrans = new Transaction(doc))
            {
                _rotateTrans.Start("_rotateTrans");
                fs.Rotate(origin, _angle);
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
        public static void Rotate(this FamilyInstance fs,XYZ origin, double _angle)
        {
            LocationPoint _point = fs.Location as LocationPoint;
            if (_point != null)
            {
                Line _axis = Line.CreateBound(origin, origin + new XYZ(0, 0, 1));
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

    }
}
