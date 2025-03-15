using Autodesk.Revit.DB;
using g3;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeighaNet;
using PubFuncWt;

using goa.Common;
using Autodesk.Revit.DB.Structure;
using System.Text.RegularExpressions;

namespace InfoStrucFormwork
{
    class StrucGenlProfileCol : EleCreatInfo
    {

        internal override void Execute()
        {
            ExGeoInfo();
            if (polyLineInfos.Count < 1) return;

            GetFamilySymbols();
            OpenTrans();
            SetLevel();
            Move();

            //System.Windows.Forms.Application.DoEvents();// 窗体重绘
            //throw new NotImplementedException();
        }

        internal override void Move()
        {
            using (Transaction transaction = new Transaction(CMD.Doc, "移动"))
            {
                transaction.Start();
                transaction.DeleteErrOrWaringTaskDialog();
                if (this.strucGenlProfileInfos.Where(p => p.FamilyInstance != null).Count() > 0)
                {
                    ElementTransformUtils.MoveElements(CMD.Doc, this.strucGenlProfileInfos.Where(p => p.FamilyInstance != null).Select(p => p.FamilyInstance.Id).ToList(), CMD.PositonMoveDis);
                }
                transaction.Commit();
            }
            //throw new NotImplementedException();
        }

        /// <summary>
        /// 提取不能被规则分析的竖向构件
        /// </summary>
        List<PolyLineInfo> polyLineInfos { get; set; }

        internal override void ExGeoInfo()
        {
            this.polyLineInfos = new List<PolyLineInfo>();

            List<string> patternNames = new List<string>() { @"a\d*-cols", @"s\d*-cols" };
            foreach (var patternName in patternNames)
            {
                foreach (var item in this.DwgParser.LayerNames)
                {
                    if (Regex.IsMatch(item, patternName, RegexOptions.IgnoreCase))
                    {
                        if (this.DwgParser.PolyLineLayerInfos.ContainsKey(item))// 墙体全部使用轮廓族
                        {
                            polyLineInfos.AddRange(this.DwgParser.PolyLineLayerInfos[item].Where(p => p.PolyLine != null));
                        }
                    }
                }
            }

            // 剔除形状相同的线圈
            List<PolyLineInfo> temp = new List<PolyLineInfo>();
            foreach (var item in this.polyLineInfos)
            {
                List<Polygon2d> polygon2ds = temp.Select(p => p.Polygon2d).ToList();

                if (!item.Polygon2d.IsSameIn(polygon2ds))
                {
                    temp.Add(item);
                }
            }

            this.polyLineInfos = temp;

            //throw new NotImplementedException();
        }

        List<StrucGenlProfileInfo> strucGenlProfileInfos { get; set; }
        internal override void OpenTrans()
        {
            this.ElementIds = new List<ElementId>();
            strucGenlProfileInfos = new List<StrucGenlProfileInfo>();
            using (Transaction trans = new Transaction(CMD.Doc, "创建"))
            {
                trans.DeleteErrOrWaringTaskDialog();
                trans.Start();

                this.polyLineInfos.ForEach(p =>
                {
                    StrucGenlProfileInfo strucGenlProfileInfo = new StrucGenlProfileInfo();
                    strucGenlProfileInfo.PolyLineInfo = p;

                    if (p.Polygon2d.VertexCount < 33)
                    {
                        strucGenlProfileInfo.FamilySymbol = this.fsCol;
                    }
                    if (!strucGenlProfileInfo.FamilySymbol.IsActive)
                    {
                        strucGenlProfileInfo.FamilySymbol.Activate();
                    }
                    strucGenlProfileInfo.FamilyInstance = CMD.Doc.Create.NewFamilyInstance(p.PolyLine.GetOutline().MinimumPoint, strucGenlProfileInfo.FamilySymbol, CMD.Doc.ActiveView.GenLevel, StructuralType.NonStructural);
                    this.ElementIds.Add(strucGenlProfileInfo.FamilyInstance.Id);
                    strucGenlProfileInfos.Add(strucGenlProfileInfo);

                    // 结构分析
                    Parameter parameter = strucGenlProfileInfo.FamilyInstance.get_Parameter(BuiltInParameter.STRUCTURAL_ANALYTICAL_MODEL);
                    if (parameter != null && !parameter.IsReadOnly)
                        parameter.Set(0);

                });

                trans.Commit();
            }

            strucGenlProfileInfos.ForEach(p =>
            {
                // 1、设置坐标点
                // 2、重载线圈
                p.SetCoordinates();
            });

            //throw new NotImplementedException();
        }

        internal override void SetLevel()
        {
            using (Transaction trans = new Transaction(CMD.Doc, "修改标高"))
            {
                trans.DeleteErrOrWaringTaskDialog();
                trans.Start();

                this.strucGenlProfileInfos.Where(p => p.FamilyInstance != null).ToList().ForEach(p =>
                {
                    if (CMD.DownLevel != null)
                    {
                        p.FamilyInstance.get_Parameter(BuiltInParameter.FAMILY_TOP_LEVEL_PARAM).Set(CMD.Level.Id);
                        p.FamilyInstance.get_Parameter(BuiltInParameter.SCHEDULE_BASE_LEVEL_PARAM).Set(CMD.DownLevel.Id);

                        p.FamilyInstance.get_Parameter(BuiltInParameter.FAMILY_TOP_LEVEL_OFFSET_PARAM).Set(0.0);
                        p.FamilyInstance.get_Parameter(BuiltInParameter.FAMILY_BASE_LEVEL_OFFSET_PARAM).Set(0.0);

                    }
                    else
                    {
                        p.FamilyInstance.get_Parameter(BuiltInParameter.FAMILY_TOP_LEVEL_PARAM).Set(CMD.Level.Id);
                        p.FamilyInstance.get_Parameter(BuiltInParameter.SCHEDULE_BASE_LEVEL_PARAM).Set(CMD.Level.Id);

                        p.FamilyInstance.get_Parameter(BuiltInParameter.FAMILY_BASE_LEVEL_OFFSET_PARAM).Set(-4000.0.MilliMeterToFeet());
                    }
                });

                trans.Commit();
            }
            //throw new NotImplementedException();
        }

        FamilySymbol fsCol { get; set; }

        internal override void GetFamilySymbols()
        {
            this.fsCol = CMD.Doc.FamilySymbolByName("结构柱", "结构柱");
        }
    }
}
