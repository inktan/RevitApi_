using System;
using System.Collections.Generic;
using System.Linq;

using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB.Events;

using goa.Common;
using ClipperLib;
using wt_Common;

namespace LINE_ROAD
{
    using cInt = Int64;
    using Path = List<IntPoint>;
    using Paths = List<List<IntPoint>>;
    using Path_xyz = List<XYZ>;
    using Paths_xyz = List<List<XYZ>>;

    public class MainWindowRequestHandler : IExternalEventHandler
    {
        public Request Request = new Request();

        public string GetName()
        {
            return "Overlapping Elements Clean Up Request Handler";
        }
        public void Execute(UIApplication uiapp)
        {
            var window = APP.MainWindow;
            try
            {
                switch (Request.Take())//Request.Take()数据提取 只能有一次
                {
                    case RequestId.None:
                        {
                            break;
                        }
                    case RequestId._LINE_ROAD:
                        {
                            _LINE_ROAD(uiapp);
                            break;
                        }
                    default:
                        {
                            break;
                        }
                }
            }
            catch (Exception ex)
            {
                //UserMessages.ShowErrorMessage(ex, window);
                TaskDialog.Show("error", ex.Message);
            }
            finally
            {
                window.WakeUp();
                window.Activate();
            }
        }//execute
        //外部事件方法建立
        /// <summary>
        /// main method
        /// </summary>
        /// <param name="uiapp"></param>
        public void _LINE_ROAD(UIApplication uiapp)
        {
            #region 与revit文档交互入口
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;
            Selection sel = uidoc.Selection;
            View acvtiView = doc.ActiveView;
            #endregion

            bool _isHaveDetailLine = isHaveDetailLine(doc, "地库_主车道中心线");//判断当前视图是否存在详图线 viewId视图过滤器，可以穿透group

            List<Element> _detailLineEls = sel.PickElementsByRectangle(new SelPickFilter_DetailLine(), "请选择详图线元素").ToList();//在UI界面 选择需要偏移的线 框选
            List<Line> allSingleLines = _Methods.ConvetDocLinesTolines(_detailLineEls);//将所有详图线的Line提取出来

            //第一步 基于交点 打断所有相交线段  （可解决 L V 型 线段 偏移两侧中 存在 不相交的一侧）
            List<Line> allCutOffSingleLines = _Methods.CutOffAllIntersetionLines(allSingleLines);//将所有相交线段，基于相交点，进行打断 此处判断为线段相交

            double radius = CMD.Radius;
            double offset = CMD.Offset;

            //第二步  处理已经存在的V、L型相交线段 倒角圆弧 之后 再 偏移所有线段
            List<Arc> _Arces = _Methods.ChamferAll_LorVLines(allCutOffSingleLines, radius);

            List<Line> _listLine_temp = new List<Line>();
            foreach (Line line in allCutOffSingleLines)//偏移所有的线段
            {
                _listLine_temp.Add(line.CreateOffset(offset, new XYZ(0, 0, -1)) as Line);
                _listLine_temp.Add(line.CreateOffset(offset, new XYZ(0, 0, 1)) as Line);
            }
            allCutOffSingleLines = _listLine_temp;

            // _Methods.TaskDialogShowMessage(_Arces.Count().ToString());
            List<Arc> _listArc_temp = new List<Arc>();
            if (_Arces.Count > 0)
            {
                foreach (Arc arc in _Arces)//偏移所有的圆弧
                {
                    _listArc_temp.Add(arc.CreateOffset(offset, new XYZ(0, 0, -1)) as Arc);
                    _listArc_temp.Add(arc.CreateOffset(offset, new XYZ(0, 0, 1)) as Arc);
                }
                _Arces = _listArc_temp;
            }
            //_Methods.TaskDialogShowMessage(_Arces.Count().ToString());

            //第三步 再次 处理V、L型相交线段

            for (int i = 0; i < allCutOffSingleLines.Count; i++)
            {
                for (int j = 0; j < allCutOffSingleLines.Count; j++)
                {
                    if (i == j)//过滤到重复线段索引的计算
                    {
                        continue;
                    }
                    //排除没有交点的线
                    SetComparisonResult _SetComparisonResult = allCutOffSingleLines[i].Intersect(allCutOffSingleLines[j]);
                    if (_SetComparisonResult == SetComparisonResult.Overlap)//判断 线段相交
                    {
                        int intersectionCount = _Methods.is_L_or_V_twolinesoverlap(allCutOffSingleLines[i], allCutOffSingleLines[j]); //排除首尾相接的线.(因为之前单线打断了以后才偏移,存在很多首尾相接的线)
                        if (intersectionCount == 0)
                        {
                            Line _newline1;
                            Line _newline2;
                            Arc _arc = _Methods.Chamfer(allCutOffSingleLines[i], allCutOffSingleLines[j], radius, out _newline1, out _newline2);
                            if (_arc == null)
                            {
                                continue;
                            }
                            _Arces.Add(_arc);
                            allCutOffSingleLines[i] = _newline1;
                            allCutOffSingleLines[j] = _newline2;
                        }
                    }
                }
            }

            //第四步 开展 绘制线条事务

            using (Transaction offset_line = new Transaction(doc))
            {
                offset_line.Start("start");
                foreach (Line _line in allCutOffSingleLines)
                {
                    DetailCurve dc1 = doc.Create.NewDetailCurve(acvtiView, _line);
                    dc1.LineStyle = CMD._tarGraphicsStyle;
                }
                foreach (Arc _arc in _Arces)
                {
                    DetailCurve dc1 = doc.Create.NewDetailCurve(acvtiView, _arc);
                    dc1.LineStyle = CMD._tarGraphicsStyle;
                }
                offset_line.Commit();
            }

            #region 重新加载线样式
            CMD.LineStyles.Clear();//线样式数据复原
            List<GraphicsStyle> lineStyles = _Methods.getAllLineGraphicsStyl(doc);//重新加载线样式
            foreach (GraphicsStyle gs in lineStyles)
            {
                CMD.LineStyles.Add(gs);
            }
            #endregion 
            string _str_final = "测试成功";
            _Methods.TaskDialogShowMessage(_str_final);
        }
        //以下为各种method---------------------------------分割线---------------------------------
        private bool isHaveDetailLine(Document doc, string _tarEleName)
        {
            bool isHave = false;
            View activeView = doc.ActiveView;
            //判断当前视图是否存在详图线 viewId视图过滤器，可以穿透group
            ICollection<ElementId> _detailLines_check = (new FilteredElementCollector(doc, activeView.Id)).OfCategory(BuiltInCategory.OST_Lines).WhereElementIsNotElementType().ToElementIds();
            int _count_detailLineIds = _detailLines_check.Count;

            //_Methods.TaskDialogShowMessage(_count_detailLineIds.ToString());
            if (_count_detailLineIds <= 0)
            {
                throw new NotImplementedException("当前活动视图不存在详图元素");
            }
            else
            {
                isHave = true;
            }
            return isHave;
        }

    }  // public class RequestHandler : IExternalEventHandler
} // namespace

