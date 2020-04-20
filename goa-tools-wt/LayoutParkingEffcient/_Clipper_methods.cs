using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Autodesk.Revit.UI;
using Autodesk.Revit.DB;


using wt_Common;
namespace ClipperLib
{
    using cInt = Int64;

    using Path = List<IntPoint>;
    using Paths = List<List<IntPoint>>;
    using Path_xyz = List<XYZ>;
    using Paths_xyz = List<List<XYZ>>;

    public class clipper_methods
    {
        /// <summary>
        /// 对一个点集路径进行偏移 需要注意 对于一个闭合路径 使用ClipperOffset 得到的结果，会有两个点集，第一个结果为外圈点集，第二结果为内圈点集
        /// </summary>
        /// <param name="_Path_xyz_RedLine"></param>
        /// <param name="_offset"></param>
        /// <returns></returns>
        public static List<List<IntPoint>> _GetOffsetPonts_clipper_Path(Path_xyz _Path_xyz_RedLine, double _offset)
        {
            Paths solution = new Paths();
            ClipperOffset _Co = new ClipperOffset();//ClipperOffset构造函数。其包含可选参数
            Path _Path_RedLine = Path_xyzToPath(_Path_xyz_RedLine);
            _Co.AddPath(_Path_RedLine, JoinType.jtSquare, EndType.etClosedLine);
            _offset = _offset * 1000;//放大倍数与clipper_methods中放大倍数保持一致
            _Co.Execute(ref solution, _offset);
            return solution;//一根线偏移后，应该具有四个端点，为矩形
        }
        /// <summary>
        /// 使用clipper对 单根个线段进行偏移，其结果点集是一个矩形路径
        /// </summary>
        /// <param name="_line"></param>
        /// <param name="_offset"></param>
        /// <returns></returns>
        public static List<IntPoint> _GetOffsetPonts_clipper_line(Line _line, double _offset)
        {
            //拿出线段断点转化为 Intpont
            XYZ _startPoint = _line.GetEndPoint(0);
            XYZ _endPoint = _line.GetEndPoint(1);
            Path_xyz _Path_xyz = new Path_xyz() { _startPoint, _endPoint };
            Path _Path = Path_xyzToPath(_Path_xyz);
            Paths solution = new Paths();
            //开展偏移工作
            ClipperOffset _Co = new ClipperOffset();//ClipperOffset构造函数。其包含可选参数
            _Co.AddPath(_Path, JoinType.jtSquare, EndType.etClosedLine);
            _offset = _offset * 1000;//放大倍数与clipper_methods中放大倍数保持一致
            _Co.Execute(ref solution, _offset);
            return solution[0]; //一根线偏移后，应该具有四个端点，为矩形
        }
        /// <summary>
        /// 将list<>list xyz 转化为 intpoint list<>list
        /// </summary>
        /// <param name="_XYZslsts"></param>
        /// <returns></returns>
        public static Paths_xyz PathsToPaths_xyz(Paths _Paths)
        {
            Paths_xyz _Paths_xyz = new Paths_xyz();
            foreach (Path _Path in _Paths)
            {
                Path_xyz _XYZs = PathToPath_xyz(_Path);
                _Paths_xyz.Add(_XYZs);
            }
            return _Paths_xyz;
        }
        /// <summary>
        /// 将intpoint list 转化为 list xyz
        /// </summary>
        /// <param name="_XYZs"></param>
        /// <returns></returns>
        public static Path_xyz PathToPath_xyz(Path _Path)
        {
            Path_xyz _Path_xyz = new Path_xyz();
            foreach (IntPoint _IntPoint in _Path)
            {
                XYZ _xyz = IntPointToXYZ(_IntPoint);
                _Path_xyz.Add(_xyz);
            }
            return _Path_xyz;
        }
        /// <summary>
        /// 将list<>list xyz 转化为 intpoint list<>list
        /// </summary>
        /// <param name="_XYZslsts"></param>
        /// <returns></returns>
        public static Paths Paths_xyzToPaths(Paths_xyz _Paths_xyz)
        {
            Paths _Paths = new Paths();
            foreach (Path_xyz _Path_xyz in _Paths_xyz)
            {
                Path _Path = Path_xyzToPath(_Path_xyz);
                _Paths.Add(_Path);
            }
            return _Paths;
        }
        /// <summary>
        /// 将list xyz 转化为 intpoint list
        /// </summary>
        /// <param name="_XYZs"></param>
        /// <returns></returns>
        public static Path Path_xyzToPath(Path_xyz _Path_xyz)
        {
            Path _Path = new Path();
            foreach (XYZ _xyz in _Path_xyz)
            {
                IntPoint _IntPoint = XYZToIntPoint(_xyz);
                _Path.Add(_IntPoint);
            }
            return _Path;
        }
        /// <summary>
        /// Rveit XYZ convert to Clipper IntPoint
        /// </summary>
        /// <param name="_xyz"></param>
        /// <returns></returns>
        public static IntPoint XYZToIntPoint(XYZ _xyz)
        {
            double x = _xyz.X * 1000.0;
            double y = _xyz.Y * 1000.0;
            return new IntPoint((cInt)x, (cInt)y);
        }
        /// <summary>
        /// Clipper IntPoint convert toRveit XYZ 
        /// </summary>
        /// <param name="_intpoint"></param>
        /// <returns></returns>
        public static XYZ IntPointToXYZ(IntPoint _intpoint)
        {
            double x = (double)_intpoint.X / 1000;
            double y = (double)_intpoint.Y / 1000;
            return new XYZ(x, y, 0.0);
        }
    }
}
