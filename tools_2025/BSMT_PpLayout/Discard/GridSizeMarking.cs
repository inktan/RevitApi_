//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//using Autodesk.Revit.UI;
//using Autodesk.Revit.DB;
//using Autodesk.Revit.UI.Selection;
//using System.Diagnostics;
//using PubFuncWt;
//using g3;
//using goa.Common;
//using goa.Common.g3InterOp;

//namespace BSMT_PpLayout
//{

//    class GridSizeMarking : RequestMethod
//    {
//        public GridSizeMarking(UIApplication uiapp) : base(uiapp)
//        {
//        }
//        /// <summary>
//        /// 批量打断车道中心线
//        /// </summary>
//        internal override void Execute()
//        {
//            "轴网尺寸标注，该功能正在优化，待释放".TaskDialogErrorMessage();
//            return;

//            InitialUIinter initialUIinter = new InitialUIinter(this.uiApp);
//            List<ElementId> selBaseWallIds = initialUIinter.SelBsmtWallIds(); // UI获取所有地库填充区域id
//            List<ElemsViewLevel> elemsViewLevels = initialUIinter.ElemsViewLevels(); //一个视图对应一个层级

//            foreach (ElemsViewLevel elemsViewLevel in elemsViewLevels)// 遍历单个视图
//            {
//                View nowView = elemsViewLevel.View;
//                foreach (Bsmt bsmt in elemsViewLevel.Bsmts)// 遍历单个地库
//                {
//                    if (!selBaseWallIds.Contains(bsmt.BsmtBound.Id))// 判断是否UI选择
//                        continue;

//                    bsmt.Computer_Ps_Fr_Col_SubExit_Area();
//                    List<RevitEleCol> allColumnUnitExits = bsmt.InBoundEleCols;
//                    List<Segment2d> segment2ds = bsmt.InBoundEleLines.Select(p => p.BoundSeg.Segment2d).ToList();

//                    Dictionary<Segment2d, List<RevitEleCol>> keyValuePairs = new Dictionary<Segment2d, List<RevitEleCol>>();           
//                    foreach (Segment2d segment2d in bsmt.Polygon2dInward.SegmentItr())
//                    {
//                        Vector2d segDirection = segment2d.Direction;
//                        double extent = segment2d.Length / 2;
//                        Line2d line2d = segment2d.ToLine2d();
//                        List<RevitEleCol> columnUnitExits = new List<RevitEleCol>();
//                        //【】规则，柱子中心距离边界的距离为柱子宽度的一半
//                        foreach (RevitEleCol eleCol in allColumnUnitExits)
//                        {
//                            //【】首先判断是否与基准线的方向是否平行
//                            Vector2d handOrientation = eleCol.FamilyInstance.HandOrientation.ToVector2d();
//                            double dot = Math.Abs(handOrientation.Dot(segDirection)) ;
//                            if (dot.EqualPrecision(1)|| dot.EqualPrecision(0))// 平行/垂直
//                            {
//                                Vector2d vector2d01 = eleCol.LocVector2d;
//                                double projectDistance = segment2d.Project(vector2d01);
//                                if(Math.Abs(projectDistance) <= extent)
//                                {
//                                    Segment2d pedalSegment2d = vector2d01.PedalSegment2d(segment2d);

//                                    // 判断垂线上是否存在车道中心线
//                                    bool isBreak02 = false;
//                                    foreach (Segment2d segment2d02 in segment2ds)
//                                    {
//                                        if (pedalSegment2d.Intersects(segment2d02))
//                                        {
//                                            isBreak02 = true;
//                                            break;
//                                        }
//                                    }
//                                    if(isBreak02)
//                                    {
//                                        continue;
//                                    }
//                                    else
//                                    {
//                                        // 判断垂线上是否存在其它柱子
//                                        bool isBreak01 = false;
//                                        foreach (RevitEleCol eleCol02 in allColumnUnitExits)
//                                        {
//                                            if (eleCol.Id != eleCol02.Id)
//                                            {
//                                                if (eleCol02.Polygon2d().Intersects(pedalSegment2d))
//                                                {
//                                                    isBreak01 = true;
//                                                    break;
//                                                }
//                                            }
//                                        }
//                                        if (isBreak01 || isBreak02)
//                                        {
//                                            continue;
//                                        }
//                                        else
//                                        {
//                                            columnUnitExits.Add(eleCol);
//                                        }
//                                    }
//                                }
//                            }
//                        }
//                        keyValuePairs.Add(segment2d, columnUnitExits);
//                    }

//                    //【】添加标注
//                    Options options = new Options();
//                    options.View = view;
//                    options.ComputeReferences = true;

//                    foreach (KeyValuePair<Segment2d ,List<RevitEleCol>> keyValuePair in keyValuePairs)
//                    {
//                        Segment2d segment2d = keyValuePair.Key;
//                        List<RevitEleCol> _columnUnitExits = keyValuePair.Value;

//                        if (_columnUnitExits.Count < 2) continue;

//                        List<FamilyInstance> _familyInstances = _columnUnitExits.Select(p => p.FamilyInstance).ToList();
//                        GetDimension(doc, view, _familyInstances, options, bsmt.Polygon2dInward, segment2d);

//                    }
//                }
//            }
//        }
//        internal void GetDimension(Document doc,View actiView, List<FamilyInstance> familyInstances, Options options, Polygon2d polygon2d, Segment2d segment2dCopy)
//        {
//            //【】判断方向，如果是竖线，采用Y值进行排序
//            XYZ direction = (familyInstances[0].Location as LocationPoint).Point - (familyInstances[1].Location as LocationPoint).Point;
//            if(Math.Abs(direction.Normalize().Y).EqualPrecision(1))
//                familyInstances = familyInstances.OrderBy(p => (p.Location as LocationPoint).Point.Y).ToList();
//            else
//                familyInstances = familyInstances.OrderBy(p => (p.Location as LocationPoint).Point.X).ToList();

//            //【】求出中点连线
//            FamilyInstance familyInstanceFirst = familyInstances.First();
//            FamilyInstance familyInstanceLast = familyInstances.Last();

//            List<Line> lineFirstRefes = GetVerLineReference(familyInstanceFirst, options);
//            List<Line> lineLastRefes = GetVerLineReference(familyInstanceLast, options);

//            // 族实例中几何引用的几何体位置源于族类型，需要transform到族实例所在的位置
//            Line lineFirst = null;
//            Line lineLast = null;
//            Vector2d directionBase = segment2dCopy.Direction;
//            foreach (Line line1 in lineFirstRefes)
//            {
//                Transform transformFirst = familyInstanceFirst.GetTransform();
//                lineFirst = line1.CreateTransformed(transformFirst) as Line;
//                Vector2d direction01 = lineFirst.Direction.ToVector2d();
//                if(Math.Abs(directionBase.Dot(direction01)).EqualZreo())
//                {
//                    break;
//                }
//            }
//            foreach (Line line1 in lineLastRefes)
//            {
//                Transform transformFirst = familyInstanceLast.GetTransform();
//                lineLast = line1.CreateTransformed(transformFirst) as Line;
//                Vector2d direction01 = lineLast.Direction.ToVector2d();
//                if (Math.Abs(directionBase.Dot(direction01)).EqualZreo())
//                {
//                    break;
//                }
//            }

//            //【】对尺寸标注的位置，做进一步处理；求出平行地库边界的定位线段

//            Line2d line2dBase = segment2dCopy.ToLine2d();
//            Line2d line2dFirst = lineFirst.ToLine2d();
//            Line2d line2dLast = lineLast.ToLine2d();

//            IntrLine2Line2 intrLine2First = new IntrLine2Line2(line2dBase, line2dFirst);
//            intrLine2First.Compute();
//            XYZ xYZFirst = intrLine2First.Point.ToXYZ();

//            IntrLine2Line2 intrLine2Last = new IntrLine2Line2(line2dBase, line2dLast);
//            intrLine2Last.Compute();
//            XYZ xYZLast = intrLine2Last.Point.ToXYZ();

//            Line line = Line.CreateBound(xYZFirst, xYZLast);

//            Segment2d _segment2d = line.ToSegment2d();
//            //【】旋转90°

//            Segment2d segment2d = _segment2d.Rotate(_segment2d.Center, Math.PI / 2);

//            Vector2d p0 = segment2d.P0;
//            Vector2d p1 = segment2d.P1;
//            Vector2d _center = segment2d.Center;
//            Vector2d direciton = p0 - _center;
//            if (polygon2d.Contains(p0))
//            {
//                direciton = p1 - _center;
//            }
//            else if (polygon2d.Contains(p1))
//            {
//                direciton = p0 - _center;
//            }
//            _segment2d = _segment2d.Move(direciton.Normalized, 10000.0.MilliMeterToFeet());
//            line = _segment2d.ToLine();
//            //【】求出reference

//            ReferenceArray references = new ReferenceArray();
//            foreach (FamilyInstance familyInstance in familyInstances)
//            {
//                List<Line> line01Refes = GetVerLineReference(familyInstance, options);
//                foreach (Line line1 in line01Refes)
//                {
//                    Transform transformCopy = familyInstance.GetTransform();
//                    Line lineCopy = line1.CreateTransformed(transformCopy) as Line;
//                    Vector2d direction01 = lineCopy.Direction.ToVector2d();
//                    if (Math.Abs(directionBase.Dot(direction01)).EqualZreo())
//                    {
//                        references.Append(line1.Reference);
//                        break;
//                    }
//                }
//            }

//            //【】寻找边界柱子
//            using (Transaction trans = new Transaction(doc, "添加塔楼间距标注"))
//            {
//                trans.Start();
//                Dimension dimension = doc.Create.NewDimension(actiView, line, references);

//                trans.Commit();
//            }
//        }
//        /// <summary>
//        /// 寻找单个柱子的尺寸生成标注参照线（dimension的创建依赖于GetSymbolGeometry（））
//        /// 寻找单个柱子的实际标注参照线（GetInstanceGeometry()获取的为实际几何位置信息）
//        /// </summary>
//        internal List<Line> GetVerLineReference(FamilyInstance familyInstance, Options options)
//        {
//            GeometryInstance geometryInstance = familyInstance.get_Geometry(options).First() as GeometryInstance;
//            GeometryElement geometryElement = geometryInstance.GetSymbolGeometry();// 当前获取的为族类型的几何参照，不是族实例的几何参照
//            List<Line> lines = new List<Line>();
//            foreach (var item in geometryElement)
//            {
//                if (item is Line)
//                {
//                    Line line = item as Line;
//                    XYZ xYZ = line.GetEndPoint(0);
//                    if (xYZ.X.EqualZreo())
//                    {
//                        lines.Add(line);
//                    }
//                    else if(xYZ.Y.EqualZreo())
//                    {
//                        lines.Add(line);
//                    }
//                }
//            }
//            return lines;
//        }


//    }
//}
