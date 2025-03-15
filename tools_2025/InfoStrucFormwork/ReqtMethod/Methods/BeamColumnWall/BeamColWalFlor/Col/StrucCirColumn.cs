using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PubFuncWt;
using Autodesk.Revit.UI.Selection;
using System.Reflection;
using System.IO;
using TeighaNet;
using System.Diagnostics;
using Autodesk.Revit.DB.Structure;

using goa.Common;
using g3;
using System.Text.RegularExpressions;

namespace InfoStrucFormwork
{
    internal class StrucCirColumn : EleCreatInfo
    {
        // 柱子的所有类型

        public StrucCirColumn() : base()
        {
        }
        List<ColumnInfo> ColumnInfos { get; set; }
        internal override void Execute()
        {
            ExGeoInfo();
            if (ColumnInfos.Count < 1) return;
            GetFamilySymbols();
            OpenTrans();
            SetLevel();
            Move();

            System.Windows.Forms.Application.DoEvents();// 窗体重绘
        }
        internal override void ExGeoInfo()
        {
            List<string> patternNames = new List<string>() { @"a\d*-cols", @"s\d*-cols" };

            // 寻找圆形柱
            List<CircleInfo> circleInfos = new List<CircleInfo>();
            foreach (var patternName in patternNames)
            {
                foreach (var item in this.DwgParser.LayerNames)
                {
                    if (Regex.IsMatch(item, patternName, RegexOptions.IgnoreCase))
                    {
                        if (this.DwgParser.CircleLayerInfos.ContainsKey(item))
                        {
                            circleInfos.AddRange(this.DwgParser.CircleLayerInfos[item].Where(p => p.Arc != null));
                        }
                    }
                }
            }

            ColumnInfos = new List<ColumnInfo>();
            ColumnInfos.AddRange(circleInfos.Select(p => new ColumnInfo(p)));

            ColumnInfos.ForEach(p => p.Excute());

            //throw new NotImplementedException();
        }

        internal override void OpenTrans()
        {
            this.ElementIds = new List<ElementId>();
            using (Transaction trans = new Transaction(CMD.Doc, "创建结构柱"))
            {
                trans.DeleteErrOrWaringTaskDialog();
                trans.Start();

                this.ColumnInfos.Where(p => p.positon != null).ToList().ForEach(p =>
                    {
                        FamilySymbol familySymbol = this.CirColType;
                        if (!familySymbol.IsActive)
                        {
                            familySymbol.Activate();
                        }
                        p.FamilyInstance = CMD.Doc.Create.NewFamilyInstance(p.positon, familySymbol, CMD.Doc.ActiveView.GenLevel, StructuralType.Column);

                        p.FamilyInstance.LookupParameter("直径").Set(p.CircleInfo.Radius * 2);

                        // 结构分析
                        Parameter parameter = p.FamilyInstance.get_Parameter(BuiltInParameter.STRUCTURAL_ANALYTICAL_MODEL);
                        if (parameter != null && !parameter.IsReadOnly)
                            parameter.Set(0);


                        this.ElementIds.Add(p.FamilyInstance.Id);
                    });

                trans.Commit();
            }
            //throw new NotImplementedException();
        }
        /// <summary>
        /// 设定标高，同时取消结构分析
        /// </summary>
        internal override void SetLevel()
        {
            using (Transaction trans = new Transaction(CMD.Doc, "修改标高"))
            {
                trans.DeleteErrOrWaringTaskDialog();
                trans.Start();

                this.ColumnInfos.Where(p => p.FamilyInstance != null).ToList().ForEach(p =>
                {
                    if (CMD.DownLevel != null)
                    {
                        Parameter parameter = p.FamilyInstance.get_Parameter(BuiltInParameter.SCHEDULE_TOP_LEVEL_PARAM);
                        if (!parameter.IsReadOnly)
                        {
                            parameter.Set(CMD.Level.Id);
                        }
                        parameter = p.FamilyInstance.get_Parameter(BuiltInParameter.FAMILY_TOP_LEVEL_PARAM);

                        if (!parameter.IsReadOnly)
                        {
                            parameter.Set(CMD.Level.Id);
                        }
                        parameter = p.FamilyInstance.get_Parameter(BuiltInParameter.SCHEDULE_BASE_LEVEL_PARAM);

                        if (!parameter.IsReadOnly)
                        {
                            parameter.Set(CMD.DownLevel.Id);
                        }
                        parameter = p.FamilyInstance.get_Parameter(BuiltInParameter.FAMILY_BASE_LEVEL_PARAM);

                        if (!parameter.IsReadOnly)
                        {
                            parameter.Set(CMD.DownLevel.Id);
                        }
                    }
                    else
                    {
                        Parameter parameter = p.FamilyInstance.get_Parameter(BuiltInParameter.SCHEDULE_TOP_LEVEL_PARAM);
                        if (!parameter.IsReadOnly)
                        {
                            parameter.Set(CMD.Level.Id);
                        }
                        parameter = p.FamilyInstance.get_Parameter(BuiltInParameter.FAMILY_TOP_LEVEL_PARAM);

                        if (!parameter.IsReadOnly)
                        {
                            parameter.Set(CMD.Level.Id);
                        }
                        parameter = p.FamilyInstance.get_Parameter(BuiltInParameter.SCHEDULE_BASE_LEVEL_PARAM);

                        if (!parameter.IsReadOnly)
                        {
                            parameter.Set(CMD.Level.Id);
                        }
                        parameter = p.FamilyInstance.get_Parameter(BuiltInParameter.FAMILY_BASE_LEVEL_PARAM);

                        if (!parameter.IsReadOnly)
                        {
                            parameter.Set(CMD.Level.Id);
                        }
                    }
                });

                trans.Commit();
            }

            using (Transaction trans = new Transaction(CMD.Doc, "修改标高"))
            {
                trans.DeleteErrOrWaringTaskDialog();
                trans.Start();

                this.ColumnInfos.Where(p => p.FamilyInstance != null).ToList().ForEach(p =>
                {
                    if (CMD.DownLevel != null)
                    {
                        Parameter parameter = p.FamilyInstance.get_Parameter(BuiltInParameter.SCHEDULE_BASE_LEVEL_OFFSET_PARAM);

                        if (!parameter.IsReadOnly)
                        {
                            parameter.Set(0.0);
                        }
                        parameter = p.FamilyInstance.get_Parameter(BuiltInParameter.FAMILY_BASE_LEVEL_OFFSET_PARAM);

                        if (!parameter.IsReadOnly)
                        {
                            parameter.Set(0.0);
                        }
                        parameter = p.FamilyInstance.get_Parameter(BuiltInParameter.SCHEDULE_TOP_LEVEL_OFFSET_PARAM);

                        if (!parameter.IsReadOnly)
                        {
                            parameter.Set(0.0);
                        }
                        parameter = p.FamilyInstance.get_Parameter(BuiltInParameter.FAMILY_TOP_LEVEL_OFFSET_PARAM);

                        if (!parameter.IsReadOnly)
                        {
                            parameter.Set(0.0);
                        }

                    }
                    else
                    {
                        Parameter parameter = p.FamilyInstance.get_Parameter(BuiltInParameter.SCHEDULE_BASE_LEVEL_OFFSET_PARAM);

                        if (!parameter.IsReadOnly)
                        {
                            parameter.Set(-4000.0.MilliMeterToFeet());
                        }
                        parameter = p.FamilyInstance.get_Parameter(BuiltInParameter.FAMILY_BASE_LEVEL_OFFSET_PARAM);

                        if (!parameter.IsReadOnly)
                        {
                            parameter.Set(-4000.0.MilliMeterToFeet());
                        }
                        parameter = p.FamilyInstance.get_Parameter(BuiltInParameter.SCHEDULE_TOP_LEVEL_OFFSET_PARAM);

                        if (!parameter.IsReadOnly)
                        {
                            parameter.Set(0.0);
                        }
                        parameter = p.FamilyInstance.get_Parameter(BuiltInParameter.FAMILY_TOP_LEVEL_OFFSET_PARAM);

                        if (!parameter.IsReadOnly)
                        {
                            parameter.Set(0.0);
                        }

                    }
                });

                trans.Commit();
            }

            using (Transaction trans = new Transaction(CMD.Doc, "取消分析模型"))
            {
                trans.DeleteErrOrWaringTaskDialog();
                trans.Start();

                this.ColumnInfos.Where(p => p.FamilyInstance != null).ToList().ForEach(p =>
                {
                    if (CMD.DownLevel != null)
                    {
                        Parameter parameter = p.FamilyInstance.get_Parameter(BuiltInParameter.STRUCTURAL_ANALYTICAL_MODEL);

                        if (!parameter.IsReadOnly)
                        {
                            parameter.Set(0);
                        }
                    }
                });
                trans.Commit();
            }
            //throw new NotImplementedException();
        }
        FamilySymbol CirColType { get; set; }

        /// <summary>
        /// 创建柱子类型
        /// </summary>
        internal override void GetFamilySymbols()
        {
            try
            {
                Family fa = (new FilteredElementCollector(CMD.Doc))
                    .OfClass(typeof(Family))
                    .First(p => p.Name == "混凝土-圆形柱") as Family;
                this.CirColType = fa.GetFamilySymbolIds().Select(p => CMD.Doc.GetElement(p) as FamilySymbol).Where(p => p.Name == "圆形柱").FirstOrDefault();

                if (this.CirColType == null)
                {
                    throw new NotImplementedException("请联系BIM协调员，载入GOA专属 混凝土-圆形柱");
                }
            }
            catch (Exception)
            {
                throw new NotImplementedException("请联系BIM协调员，载入GOA专属 混凝土-圆形柱");
            }

            //throw new NotImplementedException();
        }
        internal override void Move()
        {
            using (Transaction transaction = new Transaction(CMD.Doc, "移动"))
            {
                transaction.Start();
                transaction.DeleteErrOrWaringTaskDialog();

                if (this.ColumnInfos.Where(p => p.FamilyInstance != null).Count() > 0)
                {
                    ElementTransformUtils.MoveElements(CMD.Doc, this.ColumnInfos.Where(p => p.FamilyInstance != null).Select(p => p.FamilyInstance.Id).ToList(), CMD.PositonMoveDis);
                }
                transaction.Commit();
            }
            //throw new NotImplementedException();
        }
    }
}
