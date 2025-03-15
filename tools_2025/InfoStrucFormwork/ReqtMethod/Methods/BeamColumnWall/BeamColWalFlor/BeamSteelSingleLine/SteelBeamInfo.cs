using Autodesk.Revit.DB;
using g3;
using goa.Common.g3InterOp;
using goa.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TeighaNet;
using PubFuncWt;

namespace InfoStrucFormwork
{
    /// <summary>
    /// 处理直线钢梁
    /// </summary>
    class SteelBeamInfo
    {
        /// <summary>
        /// 创建的梁
        /// </summary>
        public FamilyInstance FamilyInstance { get; internal set; }
        /// <summary>
        /// 钢梁 全段 基准线
        /// </summary>
        public LineInfo BaseLineInfo { get; set; }
        /// <summary>
        /// 加腋节点基准线
        /// </summary>
        public List<SteelBeamInfo> HaunchedNodes { get; set; }
        public ArcInfo ArcInfo { get; set; }
        public PolyLineInfo PolyLineInfo { get; set; }
        /// <summary>
        /// 翻边高度
        /// </summary>
        public double zValue { get; set; }
        public string typeName { get; set; }
        public List<TextInfo> TextInfos { get; internal set; }
        public Polygon2d Polygon2d { get; internal set; }

        /// <summary>
        /// 该数据用于计算加腋
        /// </summary>
        public double HaunchedNodeSizeDifference { get; set; }

        /// <summary>
        /// 识别梁基准线对应的文字信息
        /// </summary>
        /// <param name="familySymbols"></param>
        internal void Excute(List<FamilySymbol> familySymbols)
        {
            zValue = 0.0;
            typeName = "";

            this.HaunchedNodes = new List<SteelBeamInfo>();
            //return;
            string pattern00 = @"^\d{99}";// 获取翻高数据

            string pattern01 = @"^\[KD\].*/.*";
            string pattern02 = @"^\[KC\].*/.*";
            string pattern03 = @"^\[SA\].*/.*";

            string pattern04 = @"^变h\(.+\)\*\d+\*\d+\*\d+";

            // 获取翻高数据
            // 获取梁类型
            foreach (var item in TextInfos)
            {
                string text = Regex.Replace(item.Text, @"\s", "");// 去掉所有空格

                if (Regex.IsMatch(text, pattern00, RegexOptions.IgnoreCase))// 翻高数据以括号为结尾
                {
                    double temp = 0.0;
                    if (double.TryParse(item.Text.Substring(item.Text.Length - 6, 5), out temp))
                    {
                        if (item.Text[item.Text.Length - 7] == '-')
                        {
                            this.zValue *= -1000 * temp;
                        }
                        else
                        {
                            this.zValue *= 1000 * temp;
                        }
                    }
                }
                else if (Regex.IsMatch(text, pattern01, RegexOptions.IgnoreCase)
                    || (Regex.IsMatch(text, pattern02, RegexOptions.IgnoreCase))
                    || (Regex.IsMatch(text, pattern03, RegexOptions.IgnoreCase)))
                {
                    char[] tmp = { '(', '/', ')' };
                    String[] result = text.Split(tmp);
                    double length = 0.0;
                    double haunchedNodeSizeDifference = 0.0;

                    if (result.Count() == 4)
                    {
                        length = Convert.ToDouble(result[1]).MilliMeterToFeet();
                        haunchedNodeSizeDifference = Convert.ToDouble(result[2]).MilliMeterToFeet();
                    }
                    else if (result.Count() == 5)
                    {
                        length = Convert.ToDouble(result[1]).MilliMeterToFeet() + Convert.ToDouble(result[2]).MilliMeterToFeet();
                        haunchedNodeSizeDifference = Convert.ToDouble(result[3]).MilliMeterToFeet();
                    }
                    if (length < 0.01) continue;// 太短则不创建

                    char[] tmp01 = { '[', ']' };
                    result = text.Split(tmp01);
                    string haunchedNodeTypeName = string.Join("", result);

                    Vector2d position = item.Position.ToVector2d();
                    int count01 = 0;
                    int count02 = 0;
                    if (position.DistanceSquared(BaseLineInfo.Segment2d.P1) < position.DistanceSquared(BaseLineInfo.Segment2d.P0))
                    {
                        if (count01 == 1) continue;// 只计算一次

                        BaseLineInfo.Segment2d = new Segment2d(BaseLineInfo.Segment2d.P0, BaseLineInfo.Segment2d.P1 - length * BaseLineInfo.Segment2d.Direction);
                        BaseLineInfo.Line = BaseLineInfo.Segment2d.ToLine();

                        // 求出加腋基准线
                        LineInfo lineInfo = new LineInfo
                        {
                            Segment2d = new Segment2d(BaseLineInfo.Segment2d.P1, BaseLineInfo.Segment2d.P1 + length * BaseLineInfo.Segment2d.Direction)
                        };
                        lineInfo.Line = lineInfo.Segment2d.ToLine();
                        SteelBeamInfo steelBeamInfo = new SteelBeamInfo();
                        steelBeamInfo.BaseLineInfo = lineInfo;
                        steelBeamInfo.typeName = haunchedNodeTypeName;
                        steelBeamInfo.HaunchedNodeSizeDifference = haunchedNodeSizeDifference;
                        steelBeamInfo.Polygon2d = lineInfo.Segment2d.ToRenct2d(2.0);
                        this.HaunchedNodes.Add(steelBeamInfo);
                        count01++;
                    }
                    else
                    {
                        if (count02 == 1) continue;// 只计算一次

                        BaseLineInfo.Segment2d = new Segment2d(BaseLineInfo.Segment2d.P0 + length * BaseLineInfo.Segment2d.Direction, BaseLineInfo.Segment2d.P1);
                        BaseLineInfo.Line = BaseLineInfo.Segment2d.ToLine();

                        // 求出加腋基准线
                        LineInfo lineInfo = new LineInfo
                        {
                            Segment2d = new Segment2d(BaseLineInfo.Segment2d.P0, BaseLineInfo.Segment2d.P0 - length * BaseLineInfo.Segment2d.Direction)
                        };
                        lineInfo.Line = lineInfo.Segment2d.ToLine();

                        SteelBeamInfo steelBeamInfo = new SteelBeamInfo();
                        steelBeamInfo.BaseLineInfo = lineInfo;
                        steelBeamInfo.typeName = haunchedNodeTypeName;
                        steelBeamInfo.HaunchedNodeSizeDifference = HaunchedNodeSizeDifference;
                        steelBeamInfo.Polygon2d = lineInfo.Segment2d.ToRenct2d(2.0);
                        this.HaunchedNodes.Add(steelBeamInfo);
                        count02++;
                    }
                }
                else if (Regex.IsMatch(text, pattern04, RegexOptions.IgnoreCase))
                {
                    char[] tmp = { '(', '~', '～', ')' };
                    String[] result = text.Split(tmp);
                    double length = 0.0;
                    double nowNodeDifference = 0.0;

                    try
                    {
                        nowNodeDifference = Math.Abs(Convert.ToDouble(result[1]) - Convert.ToDouble(result[2])).MilliMeterToFeet();
                        // 过程版使用， 使用1：6进行放坡
                        length = nowNodeDifference * 6;
                    }
                    catch (Exception)
                    {
                        //throw;
                    }
                    if (length < 0.01) continue;// 太短则不创建

                    string haunchedNodeTypeName = text;

                    Vector2d position = item.Position.ToVector2d();
                    int count01 = 0;
                    int count02 = 0;
                    if (position.DistanceSquared(BaseLineInfo.Segment2d.P1) < position.DistanceSquared(BaseLineInfo.Segment2d.P0))
                    {
                        if (count01 == 1) continue;// 只计算一次

                        BaseLineInfo.Segment2d = new Segment2d(BaseLineInfo.Segment2d.P0, BaseLineInfo.Segment2d.P1 - length * BaseLineInfo.Segment2d.Direction);
                        BaseLineInfo.Line = BaseLineInfo.Segment2d.ToLine();

                        // 求出加腋基准线
                        LineInfo lineInfo = new LineInfo
                        {
                            Segment2d = new Segment2d(BaseLineInfo.Segment2d.P1, BaseLineInfo.Segment2d.P1 + length * BaseLineInfo.Segment2d.Direction)
                        };
                        lineInfo.Line = lineInfo.Segment2d.ToLine();
                        SteelBeamInfo steelBeamInfo = new SteelBeamInfo();
                        steelBeamInfo.BaseLineInfo = lineInfo;
                        steelBeamInfo.typeName = haunchedNodeTypeName;
                        steelBeamInfo.HaunchedNodeSizeDifference = nowNodeDifference;
                        steelBeamInfo.Polygon2d = lineInfo.Segment2d.ToRenct2d(2.0);
                        this.HaunchedNodes.Add(steelBeamInfo);
                        count01++;
                    }
                    else
                    {
                        if (count02 == 1) continue;// 只计算一次

                        BaseLineInfo.Segment2d = new Segment2d(BaseLineInfo.Segment2d.P0 + length * BaseLineInfo.Segment2d.Direction, BaseLineInfo.Segment2d.P1);
                        BaseLineInfo.Line = BaseLineInfo.Segment2d.ToLine();

                        // 求出加腋基准线
                        LineInfo lineInfo = new LineInfo
                        {
                            Segment2d = new Segment2d(BaseLineInfo.Segment2d.P0, BaseLineInfo.Segment2d.P0 - length * BaseLineInfo.Segment2d.Direction)
                        };
                        lineInfo.Line = lineInfo.Segment2d.ToLine();

                        SteelBeamInfo steelBeamInfo = new SteelBeamInfo();
                        steelBeamInfo.BaseLineInfo = lineInfo;
                        steelBeamInfo.typeName = haunchedNodeTypeName;
                        steelBeamInfo.HaunchedNodeSizeDifference = nowNodeDifference;
                        steelBeamInfo.Polygon2d = lineInfo.Segment2d.ToRenct2d(2.0);
                        this.HaunchedNodes.Add(steelBeamInfo);
                        count02++;
                    }
                }
                else
                {
                    foreach (var fs in familySymbols)
                    {
                        string pat01 = text + @"\s+";
                        string pat02 = @"^" + text;
                        if (Regex.IsMatch(fs.Name, pat01, RegexOptions.IgnoreCase)
                            || Regex.IsMatch(fs.Name, pat02, RegexOptions.IgnoreCase))// 加空格是为了族类型的识别
                        {
                            this.typeName = fs.Name;
                        }
                    }
                }
            }
            if (BaseLineInfo == null)
            {
                this.Curve = ArcInfo.Arc;
            }
            else
            {
                this.Curve = BaseLineInfo.Line;
            }
            //throw new NotImplementedException();
        }
        public Curve Curve { get; set; }
        public Wall Wall { get; set; }
        public double END0 { get; internal set; }
        public double END1 { get; internal set; }

        internal void ProjectToRoof(List<Face> roofFaces)
        {
            if (roofFaces == null || roofFaces.Count < 1 || this.Curve == null)
            {
                return;
            }

            // 基于基准线创造一个face
            Element element = new FilteredElementCollector(CMD.Doc).OfCategory(BuiltInCategory.OST_Walls).OfClass(typeof(WallType)).Cast<WallType>().Where(p => p.Kind == WallKind.Basic).FirstOrDefault();
            if (element == null)
            {
                throw new NotImplementedException("请创建厚度为200mm的基本墙类型，并命名为 常规 - 200mm");
            }
            else
            {
                // 将基准线收到相交面域内~
                //roofFaces.First().GetEdgesAsCurveLoops();
                //foreach (var item in collection)
                //{

                //}

                using (Transaction trans = new Transaction(CMD.Doc, "---"))
                {
                    trans.DeleteErrOrWaringTaskDialog();
                    trans.Start();

                    // 需要将创建墙的基准线进行处理
                    Wall = Wall.Create(CMD.Doc, this.Curve, element.Id, CMD.Level.Id, 1600, 0.0, false, false);
                    Wall.get_Parameter(BuiltInParameter.WALL_BASE_OFFSET).Set(-800);

                    Transform transform = Transform.CreateRotation(new XYZ(0, 0, 1), Math.PI * 0.5);
                    if (BaseLineInfo != null)
                    {
                        ElementTransformUtils.MoveElement(CMD.Doc, Wall.Id, transform.OfVector(BaseLineInfo.Line.Direction) * (element as WallType).Width * 0.5);
                    }
                    WallUtils.DisallowWallJoinAtEnd(Wall, 0);
                    WallUtils.DisallowWallJoinAtEnd(Wall, 1);
                    trans.Commit();
                }

                if (Wall != null && Wall.IsValidObject)
                {
                    if (BaseLineInfo != null)
                    {
                        XYZ center = BaseLineInfo.Line.GetCenter();

                        List<Face> tmpFaces = Wall.GetAllFaces().OrderByDescending(p => p.Area).ToList();
                        List<Face> faces = new List<Face>();
                        faces.Add(tmpFaces[0]);
                        faces.Add(tmpFaces[1]);

                        foreach (var item in faces)
                        {
                            Autodesk.Revit.DB.IntersectionResult intersection = item.Project(center);
                            if (intersection != null && intersection.Distance.EqualZreo())
                            {
                                //HermiteFace 要完全包裹相交面才可以
                                item.Intersect(roofFaces.First(), out Curve tmp);
                                // 判断是否需要旋转方向
                                if (tmp == null)
                                {
                                    continue;
                                }

                                Segment2d segment2d01 = BaseLineInfo.Line.ToSegment2d();
                                Segment2d segment2d02 = new Segment2d(tmp.GetEndPoint(0).ToVector2d(), tmp.GetEndPoint(1).ToVector2d());

                                // 判断曲面投影线是否基准线进行配对
                                // 使用两根线段所在小矩形，是否相交
                                if (!segment2d01.ToRenct2d(0.005).Intersects(segment2d02.ToRenct2d(0.005)))
                                {
                                    continue;
                                }

                                Curve = tmp.CreateReversed();

                                //if (tmp is HermiteSpline)
                                //{
                                //    HermiteSpline hermiteSpline = tmp as HermiteSpline;
                                //    Curve = HermiteSpline.Create(hermiteSpline.ControlPoints, true);
                                //}
                                //else
                                //{
                                //    Curve = tmp.CreateReversed();
                                //}

                                try
                                {
                                    //List<GeometryObject> geometryObjects = new List<GeometryObject>();
                                    //geometryObjects.Add(Curve);
                                    //geometryObjects.Add((Curve)BaseLineInfo.Line);
                                    //geometryObjects.AddRange(item.GetEdgeCurves().Where(p => p.ApproximateLength > 0.01));

                                    //CMD.Doc.CreateDirectShapeWithNewTransaction(geometryObjects);
                                }
                                catch (Exception)
                                {

                                    //throw;
                                }
                                return;
                            }
                        }
                    }
                    else
                    {
                        XYZ center = ArcInfo.Arc.Center;
                        Wall.GetAllFaces().OrderByDescending(p => p.Area).First().Intersect(roofFaces.First(), out Curve tmp);
                        if (tmp != null)
                        {
                            if (tmp is HermiteSpline)
                            {
                                HermiteSpline hermiteSpline = tmp as HermiteSpline;

                                Curve = HermiteSpline.Create(hermiteSpline.ControlPoints, true);
                            }
                            else 
                            {
                                Curve = tmp.CreateReversed();
                            }
                            //List<GeometryObject> geometryObjects = new List<GeometryObject>();
                            //geometryObjects.Add(Curve);
                            //CMD.Doc.CreateDirectShapeWithNewTransaction(geometryObjects);
                            return;
                        }
                    }
                    // 未返回，则设置未空值
                    this.Curve = null;
                }
            }
        }
    }
}
