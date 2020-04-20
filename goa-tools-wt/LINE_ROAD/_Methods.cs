using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Selection;

namespace wt_Common
{
    /// <summary>
    /// 个人常用方法梳理
    /// </summary>
    public class _Methods
    {

        #region 曲线处理 将曲线转为闭合或者非闭合多段线
        /// <summary>
        /// 将原弧线转化为多段线
        /// </summary>
        /// <param name="_ele"></param>
        /// <param name="Numberofsegments"></param>
        /// <returns></returns>
        public static List<Line> GetClosedLinesFromArc(Curve _Curve)
        {
            int Numberofsegments = 50;
            List<Line> _Lines = new List<Line>();
            for (int i = 0; i < Numberofsegments; i++)
            {
                double _Spacing = 1.0 / Numberofsegments;
                Line _LIne = null;
                if (i == Numberofsegments - 1)
                {
                    double _NumSeg_0 = i * _Spacing;
                    double _NumSeg_1 = (0) * _Spacing;
                    XYZ _YXZ_0 = _Curve.Evaluate(_NumSeg_0, true);
                    XYZ _YXZ_1 = _Curve.Evaluate(_NumSeg_1, true);
                    _LIne = Line.CreateBound(_YXZ_0, _YXZ_1);
                }
                else
                {
                    double _NumSeg_0 = i * _Spacing;
                    double _NumSeg_1 = (i + 1) * _Spacing;
                    XYZ _YXZ_0 = _Curve.Evaluate(_NumSeg_0, true);
                    XYZ _YXZ_1 = _Curve.Evaluate(_NumSeg_1, true);
                    _LIne = Line.CreateBound(_YXZ_0, _YXZ_1);
                }
                _Lines.Add(_LIne);
            }
            return _Lines;
        }
        /// <summary>
        /// 将原弧线转化为多段线
        /// </summary>
        /// <param name="_ele"></param>
        /// <param name="Numberofsegments"></param>
        /// <returns></returns>
        public static List<Line> GetunClosedLinesFromArc(Curve _Curve)
        {
            int Numberofsegments = 50;
            List<Line> _Lines = new List<Line>();
            for (int i = 0; i < Numberofsegments; i++)
            {
                double _Spacing = 1.0 / Numberofsegments;
                Line _LIne = null;
                if (i == Numberofsegments - 1)
                {
                    continue;
                }
                else
                {
                    double _NumSeg_0 = i * _Spacing;
                    double _NumSeg_1 = (i + 1) * _Spacing;
                    XYZ _YXZ_0 = _Curve.Evaluate(_NumSeg_0, true);
                    XYZ _YXZ_1 = _Curve.Evaluate(_NumSeg_1, true);
                    _LIne = Line.CreateBound(_YXZ_0, _YXZ_1);
                }
                _Lines.Add(_LIne);
            }
            return _Lines;
        }
        #endregion

        #region 判断两根线是否首位相连 或 闭合
        /// <summary>
        /// 判断两根相交线段 结果为1，则是 L型 or V型；为0，则是X型 or T型， 或不相交； 如果是方向一致的线，1则为首尾相连 线段，2则为重叠
        /// </summary>
        /// <param name="_line1"></param>
        /// <param name="_line2"></param>
        /// <returns></returns>
        public static int is_L_or_V_twolinesoverlap(Line _line1, Line _line2)
        {
            int _intersectCount = 0;
            XYZ _line1_start = _line1.GetEndPoint(0);
            XYZ _line1_end = _line1.GetEndPoint(1);
            XYZ _line2_start = _line2.GetEndPoint(0);
            XYZ _line2_end = _line2.GetEndPoint(1);
            if (_line1_start.IsAlmostEqualTo(_line2_start, 0.0001))
            {
                _intersectCount += 1;
            }
            if (_line1_end.IsAlmostEqualTo(_line2_start, 0.0001))
            {
                _intersectCount += 1;
            }
            if (_line1_start.IsAlmostEqualTo(_line2_end, 0.0001))
            {
                _intersectCount += 1;
            }
            if (_line1_end.IsAlmostEqualTo(_line2_end, 0.0001))
            {
                _intersectCount += 1;
            }

            return _intersectCount;
        }
        #endregion

        #region 打断相交线段 相交线段（V字型、L型）倒角

        /// <summary>
        /// 对所有相交线段 中 处于V字型和L型的 线段进行倒角 输出倒角圆弧 线段列表引用不变
        /// </summary>
        /// <param name="_Lines"></param>
        /// <returns></returns>
        public static List<Arc> ChamferAll_LorVLines(List<Line> _Lines, double radius)
        {
            List<Arc> _Arces = new List<Arc>();
            int _count = _Lines.Count;
            for (int i = 0; i < _count; i++)
            {
                for (int j = 0; j < 2; j++)//判断一根线与其他所有线段的关系，对两个端点，分别进行判断
                {
                    int temp = i;
                    XYZ _i_xyz = _Lines[i].GetEndPoint(j);
                    List<int> list_int = new List<int>();
                    int coincide_count = IsXYZcoincideAllLines(_i_xyz, _Lines, out list_int);//找到一个点与所有线段的重合端点线段，并输出重合端点的线段索引值
                    if (coincide_count == 1)//当目标点与所有线段只有一个重合的情况时，进行倒角，并输出倒角后的裁剪线段
                    {
                        int k = list_int[0];//此处需要判断 索引值是否相同
                        if (k != i)
                        {
                            Line _newline1 = null;
                            Line _newline2 = null;
                            Arc _arc = Chamfer(_Lines[i], _Lines[k], radius, out _newline1, out _newline2);
                            _Arces.Add(_arc);
                            _Lines[i] = _newline1;
                            _Lines[k] = _newline2;
                            i--;//当前 i 索引线段被更新 所以回滚重新判断 
                            continue;
                        }
                    }
                    else if (coincide_count == 2)//与自身端点相交一次，比其它线段相交一次
                    {
                        foreach (int m in list_int)
                        {
                            if (m != i)
                            {
                                Line _newline1 = null;
                                Line _newline2 = null;
                                Arc _arc = Chamfer(_Lines[i], _Lines[m], radius, out _newline1, out _newline2);
                                _Arces.Add(_arc);
                                _Lines[i] = _newline1;
                                _Lines[m] = _newline2;
                                i--;//当前 i 索引线段被更新 所以回滚重新判断 
                                continue;
                            }
                        }

                    }
                    if (temp != i)//此处需要注意：不管是 j 重合一次，还是重合两次，都只能回滚一次，因为，一根线段具有两个端点
                    {
                        break;
                    }
                }
            }

            return _Arces;
        }
        /// <summary>
        /// 找到一个点与所有线段的重合端点线段，并输出重合端点的线段索引值
        /// </summary>
        /// <param name="_i_xyz"></param>
        /// <param name="_Lines"></param>
        /// <param name="Lines"></param>
        /// <returns></returns>
        public static int IsXYZcoincideAllLines(XYZ _i_xyz, List<Line> _Lines, out List<int> list_int)
        {
            list_int = new List<int>();//收集与 i 线段的一个端点重合线段的索引值
            int _count = _Lines.Count;
            for (int k = 0; k < _count; k++)
            {
                XYZ _k_xyz_0 = _Lines[k].GetEndPoint(0);
                XYZ _k_xyz_1 = _Lines[k].GetEndPoint(1);

                if (_i_xyz.IsAlmostEqualTo(_k_xyz_0, 0.0001) || _i_xyz.IsAlmostEqualTo(_k_xyz_1, 0.0001))//判断线段端点是否重合
                {
                    list_int.Add(k);
                }
            }
            return list_int.Count;
        }
        /// <summary>
        /// 把两个相交L、V型线段，如果不是，则基于交点进行裁剪 只留下L、V型，进行倒角处理
        /// </summary>
        /// <param name="_line1"></param>
        /// <param name="_line2"></param>
        /// <param name="radius"></param>
        /// <param name="_newline1"></param>
        /// <param name="_newline2"></param>
        /// <returns></returns>
        public static Arc Chamfer(Line _line1, Line _line2, double radius, out Line _newline1, out Line _newline2)
        {
            List<Line> lines = CutOffTwoLineByIntersectionPoint(_line1, _line2);//把两个相交线段，基于交点进行裁剪 只留下L、V型，进行倒角处理 该处处理，可以保证线的起点与终点的先后位置
            XYZ intersectionPoint = GetIntersetionPointFromTwoLines(_line1, _line2);//求出L or V交点
            XYZ direction_line1 = lines[0].Direction;//求方向，再求角度
            XYZ direction_line2 = lines[1].Direction;
            double angle = direction_line1.AngleTo(direction_line2);
            if (angle > Math.PI)
            {
                angle = Math.PI * 2 - angle;
            }
            //求圆弧的三角函数问题，需要深究——————————————————————————————————————————————————————————————————————————————————————————————————————————————————————————————————————————————————————————————————————————
            XYZ point_arc_start = intersectionPoint.Add(direction_line1.Multiply(radius / (Math.Tan(angle / 2))));//使用正切三角函数，求出，圆弧与倒角直线相切点的位置
            XYZ point_arc_end = intersectionPoint.Add(direction_line2.Multiply(radius / (Math.Tan(angle / 2))));

            XYZ direction_line3 = direction_line1.Add(direction_line2).Normalize();

            XYZ point_arc_middle = intersectionPoint.Add(direction_line3.Multiply((radius / Math.Sin(angle / 2)) - radius));// 使用正弦三角函数，求出，圆弧与倒角直线相切点的位置

            Arc arc = Arc.Create(point_arc_start, point_arc_end, point_arc_middle);
            _newline1 = Line.CreateBound(point_arc_start, lines[0].GetEndPoint(1));
            _newline2 = Line.CreateBound(point_arc_end, lines[1].GetEndPoint(1));
            return arc;
        }
        /// <summary>
        /// 将所有相交线段 基于交点 打断
        /// </summary>
        /// <param name="_Lines"></param>
        /// <returns></returns>
        public static List<Line> CutOffAllIntersetionLines(List<Line> _Lines)
        {
            for (int i = 0; i < _Lines.Count; i++)
            {
                for (int j = 0; j < _Lines.Count; j++)
                {
                    if (i != j)
                    {
                        SetComparisonResult _SetComparisonResult = _Lines[i].Intersect(_Lines[j]);//判断两根线段间的关系
                        if (_SetComparisonResult == SetComparisonResult.Overlap)
                        {
                            List<Line> temps = CutOffTwoIntersetionLines(_Lines[i], _Lines[j]);
                            if (temps.Count == 0)
                            {
                                continue;
                            }
                            else
                            {
                                _Lines.RemoveAt(j);
                                _Lines.RemoveAt(i);
                                _Lines.AddRange(temps);
                                i--;
                                break;
                            }
                        }
                    }
                }
            }
            return _Lines;
        }
        /// <summary>
        /// 基于相交点，切断相交直线，各自留下长的那一段  将相交线打断，留下长的那一半，十字型相交线，切割成L V型相交线段，该处选择留下长的那一部分
        /// </summary>
        /// <param name="_line1"></param>
        /// <param name="_line2"></param>
        /// <returns></returns>
        public static List<Line> CutOffTwoLineByIntersectionPoint(Line _line1, Line _line2)
        {
            List<Line> lines = new List<Line>();
            XYZ intersection = GetIntersetionPointFromTwoLines(_line1, _line2);//获取两个线的交点
            if (intersection == null)
            {
                return lines;
            }
            XYZ xyz_start1 = _line1.GetEndPoint(0);
            XYZ xyz_end1 = _line1.GetEndPoint(1);
            XYZ xyz_start2 = _line2.GetEndPoint(0);
            XYZ xyz_end2 = _line2.GetEndPoint(1);
            if (intersection.DistanceTo(xyz_start1) > intersection.DistanceTo(xyz_end1))
            {
                Line _newline = Line.CreateBound(intersection, xyz_start1);
                lines.Add(_newline);
            }
            else
            {
                Line _newline = Line.CreateBound(intersection, xyz_end1);
                lines.Add(_newline);
            }
            if (intersection.DistanceTo(xyz_start2) > intersection.DistanceTo(xyz_end2))
            {
                Line _newline = Line.CreateBound(intersection, xyz_start2);
                lines.Add(_newline);
            }
            else
            {
                Line _newline = Line.CreateBound(intersection, xyz_end2);
                lines.Add(_newline);
            }
            return lines;
        }
        /// <summary>
        /// 打断两根相交线段 L型 V型除外
        /// </summary>
        /// <param name="_Lines"></param>
        /// <returns></returns>
        public static List<Line> CutOffTwoIntersetionLines(Line _line1, Line _line2)
        {
            List<Line> lines = new List<Line>();

            int _intersectCount = is_L_or_V_twolinesoverlap(_line1, _line2);// 判断两根相交线段 结果为1，则是 L型 or V型；为0，则是X型 or T型
            if (_intersectCount == 0)
            {
                lines = CutOffTwoIntersetionLines_X_T(_line1, _line2);
            }
            return lines;
        }
        /// <summary>
        /// 打断两根 X型 T型 相交线段
        /// </summary>
        /// <param name="_line1"></param>
        /// <param name="_line2"></param>
        /// <returns></returns>
        public static List<Line> CutOffTwoIntersetionLines_X_T(Line _line1, Line _line2)
        {
            List<Line> lines = new List<Line>();
            XYZ _line1_start = _line1.GetEndPoint(0);
            XYZ _line1_end = _line1.GetEndPoint(1);
            XYZ _line2_start = _line2.GetEndPoint(0);
            XYZ _line2_end = _line2.GetEndPoint(1);
            XYZ intersectionPint = GetIntersetionPointFromTwoLines(_line1, _line2);
            //该处需要判断是源于 T型 的存在
            if (!intersectionPint.IsAlmostEqualTo(_line1_start, 0.0001))
            {
                lines.Add(Line.CreateBound(intersectionPint, _line1_start));
            }
            if (!intersectionPint.IsAlmostEqualTo(_line1_end, 0.0001))
            {
                lines.Add(Line.CreateBound(intersectionPint, _line1_end));
            }
            if (!intersectionPint.IsAlmostEqualTo(_line2_start, 0.0001))
            {
                lines.Add(Line.CreateBound(intersectionPint, _line2_start));
            }
            if (!intersectionPint.IsAlmostEqualTo(_line2_end, 0.0001))
            {
                lines.Add(Line.CreateBound(intersectionPint, _line2_end));
            }
            return lines;
        }
        #endregion

        #region  线段处理 求围合面积 修改线样式 获取线的端点列表 求线段交点
        /// <summary>
        /// 获取两根线段的交点
        /// </summary>
        /// <param name="line1"></param>
        /// <param name="line2"></param>
        /// <returns></returns>
        public static XYZ GetIntersetionPointFromTwoLines(Line _line1, Line _line2)
        {
            IntersectionResultArray intersectionResultArray;
            XYZ intersection = null;
            SetComparisonResult _SetComparisonResult = _line1.Intersect(_line2, out intersectionResultArray);

            if (_SetComparisonResult == SetComparisonResult.Overlap)
            {
                intersection = intersectionResultArray.get_Item(0).XYZPoint;
            }
            else
            {
                return null;
                throw new NotImplementedException("线段无非水平相交点");
            }
            return intersection;
        }
        /// <summary>
        /// Shoelace公式  鞋带法计算面积，需要进行进一步研究
        /// </summary>
        /// <param name="_Lines"></param>
        /// <returns></returns>
        public static double GetAreafromLines(IList<Line> _Lines)
        {
            //获取线段断点列表，并去重
            IList<XYZ> _XYZs = GetUniqueXYZFromLines(_Lines);
            //求面积
            return GetAreafromPoints(_XYZs);
        }
        /// <summary>
        /// 修改模型线样式
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="_tarLineStyleName"></param>
        /// <param name="_modelLine"></param>
        public static void SetLineStyle(Document doc, string _tarLineStyleName, ModelLine _modelLine)
        {
            using (Transaction trans = new Transaction(doc))
            {
                trans.Start("trans");
                ICollection<ElementId> LineStyleIds = GetAllLineStyleIdsFromSetting(doc);
                foreach (ElementId _eleId in LineStyleIds)
                {
                    GraphicsStyle _lineStyle = doc.GetElement(_eleId) as GraphicsStyle;
                    string _name = _lineStyle.Name;
                    if (_name == _tarLineStyleName)
                    {
                        _modelLine.LineStyle = _lineStyle;
                    }
                }
                trans.Commit();
            }
        }
        /// <summary>
        /// 修改详图线样式
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="_tarLineStyleName"></param>
        /// <param name="_modelLine"></param>
        public static void SetLineStyle(Document doc, string _tarLineStyleName, DetailLine _DetailLine)
        {
            using (Transaction trans = new Transaction(doc))
            {
                trans.Start("trans");
                ICollection<ElementId> LineStyleIds = GetAllLineStyleIdsFromSetting(doc);
                foreach (ElementId _eleId in LineStyleIds)
                {
                    GraphicsStyle _lineStyle = doc.GetElement(_eleId) as GraphicsStyle;
                    string _name = _lineStyle.Name;
                    if (_name == _tarLineStyleName)
                    {
                        _DetailLine.LineStyle = _lineStyle;
                    }
                }
                trans.Commit();
            }
        }

        /// <summary>
        /// 获取所有线的排序后的端点，并去重
        /// </summary>
        /// <param name="_Lines"></param>
        /// <returns></returns>
        public static List<XYZ> GetUniqueXYZFromLines(IList<Line> _Lines)
        {
            IList<XYZ> _XYZs = new List<XYZ>();
            foreach (Line _Line in _Lines)
            {
                _XYZs.Add(_Line.GetEndPoint(0));
                _XYZs.Add(_Line.GetEndPoint(1));
            }
            //列表去重
            return Pointlistdeduplication(_XYZs).ToList();
        }
        #endregion  

        #region 图元获取 过滤锁定元素 图元与图元Id之间的转换

        /// <summary>
        /// 从单个 线组 元素中读取LineList 如果组中含有曲线，会把曲线转化为多线段
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="sel_groups_obstacle"></param>
        /// <returns></returns>
        public static List<Line> GetLinesFromLineGroup(Group LineGroup)
        {
            //基于组的族类型name 来判断是模型组， 还是详图组 
            string categoryName = LineGroup.get_Parameter(BuiltInParameter.ELEM_CATEGORY_PARAM).AsValueString();
            List<Line> _Lines = new List<Line>();
            if (categoryName == "详图组")
            {
                _Lines = GetLinesFromDetailLineGroup(LineGroup);
            }
            else if (categoryName == "模型组")
            {
                _Lines = GetLinesFromModelLineGroup(LineGroup);
            }
            return _Lines;
        }
        /// <summary>
        /// 从单个 模型线组 元素中读取LineList 如果组中含有曲线，会把曲线转化为多线段
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="sel_groups_obstacle"></param>
        /// <returns></returns>
        public static List<Line> GetLinesFromModelLineGroup(Group ModelLineGroup)
        {
            List<Line> _Lines = new List<Line>();
            Document doc = ModelLineGroup.Document;
            List<ElementId> _groupEleIds = ModelLineGroup.GetMemberIds().ToList();
            List<Element> _groupEles = EleIdsToEles(doc, _groupEleIds);

            _Lines = ConvetDocLinesTolines(_groupEles);

            return _Lines;
        }
        /// <summary>
        /// 从单个 详图线组 元素中读取LineList 如果组中含有曲线，会把曲线转化为多线段
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="sel_groups_obstacle"></param>
        /// <returns></returns>
        public static List<Line> GetLinesFromDetailLineGroup(Group DetailLineGroup)
        {
            List<Line> _Lines = new List<Line>();
            Document doc = DetailLineGroup.Document;
            List<ElementId> _groupEleIds = DetailLineGroup.GetMemberIds().ToList();
            List<Element> _groupEles = EleIdsToEles(doc, _groupEleIds);

            _Lines = ConvetDocLinesTolines(_groupEles);

            return _Lines;
        }
        /// <summary>
        /// 提取 详图线 或者 模型线 的 lines
        /// </summary>
        /// <param name="_Eles"></param>
        /// <returns></returns>
        public static List<Line> ConvetDocLinesTolines(List<Element> _Eles)
        {
            List<Line> _Lines = new List<Line>();
            foreach (Element _ele in _Eles)
            {
                if (_ele is DetailLine)
                {
                    DetailLine _DetailLine = _ele as DetailLine;
                    Curve _Curve = _DetailLine.GeometryCurve;
                    Line _Line = _Curve as Line;
                    _Lines.Add(_Line);
                }
                else if (_ele is ModelLine)
                {
                    ModelLine _ModelLine = _ele as ModelLine;
                    Curve _Curve = _ModelLine.GeometryCurve;
                    Line _Line = _Curve as Line;
                    _Lines.Add(_Line);
                }
                else if (_ele is DetailCurve)//将圆弧转化为多段线，取列表并集
                {
                    DetailCurve _DetaillArc = _ele as DetailCurve;
                    Curve _Curve = _DetaillArc.GeometryCurve;
                    List<Line> __Lines = GetunClosedLinesFromArc(_Curve);//将圆弧划分成一定段数的线段相连
                    _Lines.AddRange(__Lines);
                }
                else if (_ele is ModelCurve)//将圆弧转化为多段线，取列表并集
                {
                    ModelCurve _ModelArc = _ele as ModelCurve;
                    Curve _Curve = _ModelArc.GeometryCurve;
                    List<Line> __Lines = GetunClosedLinesFromArc(_Curve);//将圆弧划分成一定段数的线段相连
                    _Lines.AddRange(__Lines);
                }
            }
            return _Lines;
        }
        /// <summary>
        /// 剔除锁定图元
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="allEleIds"></param>
        /// <returns></returns>
        public static ICollection<ElementId> FileterPinnedElement(Document doc, ICollection<ElementId> allEleIds)
        {
            ICollection<ElementId> _eleIds = new List<ElementId>();
            List<Element> allEles = EleIdsToEles(doc, allEleIds.ToList());
            foreach (Element ele in allEles)
            {
                bool isPinned = ele.Pinned;
                if (!isPinned)
                {
                    _eleIds.Add(ele.Id);
                }
            }
            return _eleIds;
        }
        /// <summary>
        /// 元素Id列表转换为元素列表
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="_EleIds"></param>
        /// <returns></returns>
        public static List<Element> EleIdsToEles(Document doc, List<ElementId> _EleIds)
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
        /// <param name="doc"></param>
        /// <param name="_Eles"></param>
        /// <returns></returns>
        public static List<ElementId> ElesToEleIds(List<Element> _Eles)
        {
            List<ElementId> _EleIds = new List<ElementId>();
            foreach (Element _Ele in _Eles)
            {
                _EleIds.Add(_Ele.Id);
            }
            return _EleIds;
        }
        #endregion

        #region 点处理 求围合面积 求闭合或者非闭合多段线
        /// <summary>
        /// 将顺序点集 连接成闭合多段线
        /// </summary>
        /// <param name="_XYZs"></param>
        /// <returns></returns>
        public static List<Line> GetClosedLinesfromPoints(List<XYZ> _XYZs)
        {
            List<Line> _Lines = new List<Line>();
            int _count = _XYZs.Count;
            for (int i = 0; i < _count; i++)
            {
                Line _LIne = null;
                if (i == _count - 1)
                {
                    _LIne = Line.CreateBound(_XYZs[i], _XYZs[0]);
                }
                else
                {
                    _LIne = Line.CreateBound(_XYZs[i], _XYZs[i + 1]);
                }
                _Lines.Add(_LIne);
            }
            return _Lines;
        }
        /// <summary>
        /// 将顺序点集 连接成非闭合多段线
        /// </summary>
        /// <param name="_XYZs"></param>
        /// <returns></returns>
        public static List<Line> GetunClosedLinesfromPoints(IList<XYZ> _XYZs)
        {
            List<Line> _Lines = new List<Line>();
            int _count = _XYZs.Count;
            for (int i = 0; i < _count; i++)
            {
                Line _LIne = null;
                if (i == _count - 1)
                {
                    continue;
                }
                else
                {
                    _LIne = Line.CreateBound(_XYZs[i], _XYZs[0]);
                }
                _Lines.Add(_LIne);
            }
            return _Lines;
        }
        /// <summary>
        ///  Shoelace公式  鞋带法计算面积，需要进行进一步研究
        /// </summary>
        /// <param name="_XYZs"></param>
        /// <returns></returns>
        public static double GetAreafromPoints(IList<XYZ> _XYZs)
        {
            int count = _XYZs.Count;
            double area = _XYZs[count - 1].X * _XYZs[0].Y - _XYZs[0].X * _XYZs[count - 1].Y;
            for (int i = 1; i < count; i++)
            {
                int j = i - 1;
                area += _XYZs[j].X * _XYZs[i].Y;
                area -= _XYZs[i].X * _XYZs[j].Y;
            }
            return Math.Abs(0.5 * area);
        }

        /// <summary>
        /// 点列表去重
        /// </summary>
        /// <param name="_XYZs"></param>
        /// <returns></returns>
        public static IList<XYZ> Pointlistdeduplication(IList<XYZ> _XYZs)
        {
            //列表去重
            IList<XYZ> __XYZs = new List<XYZ>();
            foreach (XYZ _XYZ in _XYZs)
            {
                if (!IsInXYZlist(_XYZ, __XYZs))
                {
                    __XYZs.Add(_XYZ);
                }
            }
            return __XYZs;
        }
        /// <summary>
        /// 与一个列表的数据都不相等
        /// </summary>
        /// <param name="_XYZ"></param>
        /// <param name="_XYZs"></param>
        /// <returns></returns>
        public static bool IsInXYZlist(XYZ _XYZ, IList<XYZ> _XYZs)
        {
            bool isIn = false;

            foreach (XYZ __XYZ in _XYZs)
            {
                if (_XYZ.IsAlmostEqualTo(__XYZ, 0.001))
                {
                    isIn = true;
                    break;
                }
            }
            return isIn;
        }
        #endregion

        #region 线样式 获取文档所有线样式 新建线样式
        /// <summary>
        /// 拿出所有的线型元素list 传递给combox需要GraphicsStyle的list
        /// </summary>
        /// <param name="doc"></param>
        /// <returns></returns>
        public static List<GraphicsStyle> getAllLineGraphicsStyl(Document doc)
        {
            List<GraphicsStyle> lineStyles = new List<GraphicsStyle>();
            ICollection<ElementId> lineStyleIds = GetAllLineStyleIdsFromSetting(doc);
            foreach (ElementId _eleId in lineStyleIds)
            {
                GraphicsStyle LineSyle = doc.GetElement(_eleId) as GraphicsStyle;
                lineStyles.Add(LineSyle);
            }
            return lineStyles;
        }
        /// <summary>
        /// 获取所有线性类别 方法一：通过创建实体线事务，通过事务回滚方式，获取
        /// </summary>
        /// <param name="doc"></param>
        /// <returns></returns>
        public static ICollection<ElementId> GetAllLineStyleIdsFromTransaction(Document doc)
        {
            ICollection<ElementId> styles = new List<ElementId>();
            Transaction transaction = new Transaction(doc);
            transaction.Start("Create detail line");
            try
            {
                View view = doc.ActiveView;
                DetailCurve detailCurve = doc.Create.NewDetailCurve(view, Line.CreateBound(new XYZ(0, 0, 0), new XYZ(10, 0, 0)));
                styles = detailCurve.GetLineStyleIds();

                transaction.RollBack();
            }
            catch (Exception ex)
            {
                transaction.RollBack();
            }
            return styles;
        }
        /// <summary>
        /// 获取所有线型类别 方法二：通过document setting
        /// </summary>
        /// <param name="doc"></param>
        /// <returns></returns>
        public static ICollection<ElementId> GetAllLineStyleIdsFromSetting(Document doc)
        {
            ICollection<ElementId> styles = new List<ElementId>();
            Settings documentSettings = doc.Settings;
            Categories ParentCategoyry = doc.Settings.Categories;
            Category ParentLineCategoyry = ParentCategoyry.get_Item(BuiltInCategory.OST_Lines);
            CategoryNameMap _CategoryNameMap = ParentLineCategoyry.SubCategories;
            foreach (Category lineStyle in _CategoryNameMap)
            {
                GraphicsStyle _GraphicsStyle = lineStyle.GetGraphicsStyle(GraphicsStyleType.Projection);
                styles.Add(_GraphicsStyle.Id);
            }
            return styles;
        }
        /// <summary>
        /// 创建新的线样式,线宽为整数1-16
        /// </summary>
        /// <param name="doc"></param>
        /// <returns></returns>
        public static Category CreatLineStyle(Document doc, string LineStyleName, int LineWeight, Color newColor)
        {
            Category newCategory = null;
            Categories categories = doc.Settings.Categories;
            Category lineCategory = categories.get_Item(BuiltInCategory.OST_Lines);
            using (Transaction _CreatNewLineCategory = new Transaction(doc))//创建新的线样式
            {
                _CreatNewLineCategory.Start("_CreatNewLineCategory");
                newCategory = doc.Settings.Categories.NewSubcategory(lineCategory, LineStyleName);//
                newCategory.LineColor = newColor;
                newCategory.SetLineWeight(LineWeight, GraphicsStyleType.Projection);
                _CreatNewLineCategory.Commit();
            }
            return newCategory;
        }
        /// <summary>
        /// 创建线型图案 CreatLinePattern
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="patternName"></param>
        /// <returns></returns>
        public static LinePatternElement CreatLinePatternElement(Document doc, string patternName)
        {
            List<LinePatternSegment> lstSegments = new List<LinePatternSegment>();
            lstSegments.Add(new LinePatternSegment(LinePatternSegmentType.Dot, 0.0));
            lstSegments.Add(new LinePatternSegment(LinePatternSegmentType.Space, 0.02));
            lstSegments.Add(new LinePatternSegment(LinePatternSegmentType.Dash, 0.03));
            lstSegments.Add(new LinePatternSegment(LinePatternSegmentType.Space, 0.02));

            LinePattern _linePattern = new LinePattern(patternName);
            _linePattern.SetSegments(lstSegments);

            using (Transaction trans = new Transaction(doc))
            {
                trans.Start("Create a linepattern element");
                LinePatternElement linePatternElement = LinePatternElement.Create(doc, _linePattern);
                trans.Commit();
                return linePatternElement;
            }
        }

        #endregion

        #region 监测消息 测试代码 弹窗处理
        /// <summary>
        /// Revit消息窗口弹出
        /// </summary>
        /// <param name="_str"></param>
        public static void TaskDialogShowMessage(string _str)
        {
            string _title = "Error";
            TaskDialog.Show(_title, _str);
        }
        #endregion

    }
}
