using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using g3;
using ClipperLib;
using PubFuncWt;
using Autodesk.Revit.DB;
using goa.Common;
using System.Windows.Threading;
using System.Threading;
using Autodesk.Revit.UI;
using goa.Common.g3InterOp;

namespace BSMT_PpLayout
{

    using cInt = Int64;

    using Path = List<IntPoint>;
    using Paths = List<List<IntPoint>>;
    using Path_Vector2d = List<Vector2d>;
    using Paths_Vector2d = List<List<Vector2d>>;

    /// <summary>
    /// 通过算法得到的停车位点
    /// </summary>
    struct PPLocPoint
    {
        internal static bool isStop = false;
        internal Vector2d Vector2d { get; set; }
        internal Vector2d Direction { get; set; }// 该direction用来判断停车位的选装方向
        internal String PsTypeName { get; set; }
        internal double RotateAngle { get { return this.Direction.AngleRadToX() - Math.PI / 2; } }

        internal double Width { get; set; }
        internal double Height { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="vector2d"></param>
        /// <param name="headOfCarFacing"></param>
        internal PPLocPoint(Vector2d _vector2d, string _psTypeName, double _width, double _height)
        {
            this.Vector2d = _vector2d;
            this.Direction = new Vector2d(0, 1);
            this.PsTypeName = _psTypeName;

            this.Width = _width;
            this.Height = _height;
        }

        /// <summary>
        /// 对 List<ParkingLocationPoint> 进行 rotate transform，角度正值为逆时针旋转
        /// </summary>
        internal static IEnumerable<PPLocPoint> Mirror(IEnumerable<PPLocPoint> ppLocPoints, Vector2d origin, Vector2d direction)
        {
            foreach (var item in ppLocPoints)
            {
                yield return Mirror(item, origin, direction);
            }
        }
        internal static PPLocPoint Mirror(PPLocPoint ppLocPoints, Vector2d origin, Vector2d direction)
        {
            ppLocPoints.Vector2d = ppLocPoints.Vector2d.Mirror(origin, direction);
            ppLocPoints.Direction = ppLocPoints.Direction.Mirror(Vector2d.Zero, direction);// 该处为镜像向量，而非旋转点

            return ppLocPoints;
        }

        /// <summary>
        /// 对 List<ParkingLocationPoint> 进行 rotate transform，角度正值为逆时针旋转
        /// </summary>
        internal static IEnumerable<PPLocPoint> Rotate(IEnumerable<PPLocPoint> ppLocPoints, Vector2d origin, double angle)
        {
            foreach (var item in ppLocPoints)
            {
                yield return Rotate(item, origin, angle);
            }
        }
        internal static PPLocPoint Rotate(PPLocPoint ppLocPoints, Vector2d origin, double angle)
        {
            ppLocPoints.Vector2d = ppLocPoints.Vector2d.Rotate(origin, angle);
            ppLocPoints.Direction = ppLocPoints.Direction.Rotate(Vector2d.Zero, angle);// 该处为旋转向量，而非旋转点

            return ppLocPoints;
        }
        internal static IEnumerable<PPLocPoint> FromFrame2d(IEnumerable<PPLocPoint> ppLocPoints, Frame3d frame3d)
        {
            foreach (var item in ppLocPoints)
            {
                yield return FromFrame2d(item, frame3d);
            }
        }
        internal static PPLocPoint FromFrame2d(PPLocPoint ppLocPoints, Frame3d frame3d)
        {

            ppLocPoints.Vector2d = frame3d.FromFrameP(ppLocPoints.Vector2d).ToVector2d();
            ppLocPoints.Direction = frame3d.FromFrameV(ppLocPoints.Direction.ToVector3d()).ToVector2d();
            return ppLocPoints;
        }
        /// <summary>
        /// 属性点转换为停车位 需要区分 族类型 旋转角度
        /// </summary>
        /// <param name="psLocPoints"></param>
        /// <param name="doc"></param>
        internal void PointProjecToRevit(IEnumerable<PPLocPoint> psLocPoints, Document doc, Autodesk.Revit.DB.View nowView)
        {
            //在该函数内部，开启进度窗口显示

            ProgressBar_ progressBar_ = new ProgressBar_();
            progressBar_.Show();
            progressBar_.Computer("计算已经完成，请等待Revit界面更新", 0, psLocPoints.Count());
            GlobalData.Instance.ProgreaaBarVlaue = 0.0;

            List<string> typeNames = psLocPoints.Select(p => p.PsTypeName).Where(p => p != "").Distinct().ToList();
            foreach (string typeName in typeNames)
            {
                List<PPLocPoint> psLocPointsByType = psLocPoints.Where(p => p.PsTypeName == typeName).ToList();
                FamilySymbol parkingType = doc.GetFs(typeName);

                List<double> rotateAngles = psLocPointsByType.Select(p => p.RotateAngle).Distinct().ToList();

                foreach (double angle in rotateAngles)
                {
                    List<PPLocPoint> tarPsLocPointsByAngle = psLocPointsByType.Where(p => p.RotateAngle == angle).ToList();

                    GlobalData.Instance.ProgreaaBarVlaue += tarPsLocPointsByAngle.Count;
                    progressBar_.BringIntoView();

                    System.Windows.Forms.Application.DoEvents();// 窗体重绘

                    //uIDocument.UpdateAllOpenViews();
                    if (progressBar_.IsBreak)
                    {
                        progressBar_.Close();
                        break;
                    }

                    Transcation_mehod.LayoutParking(doc, nowView, tarPsLocPointsByAngle, angle, parkingType);// 当前放置族实例和旋转族实例分两步进行，不分开，会出现不明原因问题
                }
            }

            progressBar_.Close();

        }
        /// <summary>
        /// 创建道路中心线
        /// </summary>
        /// <param name="routes"></param>
        /// <param name="doc"></param>
        /// <param name="nowView"></param>
        internal void CreatingRoads(List<Route> routes, Document doc, Autodesk.Revit.DB.View nowView)
        {
            if (routes.Count == 0)
            {
                return;
            }
            CurveArray curveArray = routes.Select(p => p.Segment2d).ToLines().ToCurveArray();
            GraphicsStyle graphicsStyle = doc.GetGraphicsStyleByName("地库_主车道中心线");

            using (Transaction trans = new Transaction(doc, "creatLines"))
            {
                trans.Start();
                DetailCurveArray detailCurveArray = doc.Create.NewDetailCurveArray(nowView, curveArray);
                foreach (DetailCurve detailCurve in detailCurveArray)
                {
                    detailCurve.LineStyle = graphicsStyle;
                }
                trans.Commit();
            }
        }
    }
}
