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
using System.Text.RegularExpressions;
using g3;
using System.Windows;

namespace InfoStrucFormwork
{
    internal class SlopeBeamInfo
    {
        public PolyLineInfo PolyLineInfo { get; set; }
        public TextInfo TextInfo  { get; set; }
    }
    internal class SlopeRoofBeam : RequestMethod
    {
        internal SlopeRoofBeam(UIApplication _uiApp) : base(_uiApp)
        {
        }

        internal override void Execute()
        {
            PickCad pickCad = new PickCad(uiApp);
            pickCad.Execute();

            List<string> patterns = new List<string>()
            {
                @".*A-SECT-STRU\z",
            };
            List<PolyLineInfo> polyLineInfos = new List<PolyLineInfo>();
            List<TextInfo> textInfos = new List<TextInfo>();
            
            foreach (var pattern in patterns)
            {
                foreach (var item in pickCad.DwgParser.LayerNames)
                {
                    if (Regex.IsMatch(item, pattern, RegexOptions.IgnoreCase))
                    {
                        if (pickCad.DwgParser.PolyLineLayerInfos.ContainsKey(item))
                        {
                            polyLineInfos.AddRange(pickCad.DwgParser.PolyLineLayerInfos[item]);
                        }
                        if (pickCad.DwgParser.PolyLineLayerInfos.ContainsKey(item))
                        {
                            textInfos.AddRange(pickCad.DwgParser.TexLayertInfos[item]);
                        }
                    }
                }
            }

            //polyLineInfos.Count.ToString().TaskDialogErrorMessage();

            // 创建文件夹，使用临时文件夹
            //string filePath = Environment.GetEnvironmentVariable("TEMP");
            //this.doc.PathName.ToString().TaskDialogErrorMessage();
            //string filePath = Path.GetDirectoryName(this.doc.PathName);
            //filePath += @"\temp";
            //string filePath = @"C:\set";
            string filePath = @"C:\Temp";
            if (!Directory.Exists(filePath))
            {
                Directory.CreateDirectory(filePath);
            }

            foreach (PolyLineInfo polyLineInfo in polyLineInfos)
            {
                try
                {

                    string newPfoName = @"结构梁_轮廓" + (Guid.NewGuid()).ToString();

                    foreach (var item in textInfos)
                    {
                        string text = item.Text;
                        Vector2d p0 = new Vector2d(item.MinPoint.X, item.MinPoint.Y);
                        Vector2d p1 = new Vector2d(item.MaxPoint.X, item.MaxPoint.Y);

                        Segment2d segment2d = new Segment2d(p0, p1);
                        if (polyLineInfo.Polygon2d.Contains(segment2d) || polyLineInfo.Polygon2d.Intersects(segment2d))
                        {
                            newPfoName ="折梁_" + item.Text;
                            break;
                        }
                    }

                    // 1 打开 结构梁_轮廓 族文档
                    Document strucFaDoc = CMD.Doc.EditFamily(CMD.Doc.FamilyByName("结构梁_轮廓"));
                    // 1.1 找到 _轮廓 嵌套族
                    Family profileFa = (new FilteredElementCollector(strucFaDoc)).OfClass(typeof(Family)).FirstOrDefault(p => p.Name == "_轮廓") as Family;
                    // 2 打开 最底层 轮廓 族文档
                    Document proFileFaDoc = strucFaDoc.EditFamily(profileFa);
                    // 2.1 清除现有轮廓线
                    proFileFaDoc.DelEleIds((new FilteredElementCollector(proFileFaDoc)).OfCategory(BuiltInCategory.OST_Lines).ToElementIds());
                    // 2.2 创建新的轮廓线
                    if (polyLineInfo == null || polyLineInfo.CurveArray.IsEmpty) continue;
                    // 2.3 找到视图
                    ViewPlan view = (new FilteredElementCollector(proFileFaDoc)).OfClass(typeof(ViewPlan)).First() as ViewPlan;
                    // 2.4 基于原点位置创建新的轮廓线
                    CurveArray curveArray = new CurveArray();
                    for (int i = 0; i < polyLineInfo.PtsOriginByMin.Count(); ++i)
                    {
                        curveArray.Append(Line.CreateBound(polyLineInfo.PtsOriginByMin[i], polyLineInfo.PtsOriginByMin[(i + 1) % polyLineInfo.PtsOriginByMin.Count()]));
                    }
                    using (Transaction _trans = new Transaction(proFileFaDoc, "画轮廓"))
                    {
                        _trans.Start();
                        DetailCurveArray detailCurveArray = proFileFaDoc.FamilyCreate.NewDetailCurveArray(view, curveArray);
                        _trans.Commit();
                    }
                    SaveAsOptions saveAsOptions = new SaveAsOptions
                    {
                        MaximumBackups = 1,
                        OverwriteExistingFile = true
                    };
                    //MessageBox.Show(filePath + @"\" + proFileFaDoc.PathName.Split('\\').Last());
                    string outLinePath = filePath + @"\_轮廓.rfa";
                    proFileFaDoc.SaveAs(outLinePath, saveAsOptions);
                    proFileFaDoc.Close();

                    // 3 载入族轮廓
                    strucFaDoc.ReLoadFamily(outLinePath);

                    // 3.1 保存 新的结构梁_轮廓 族文件
                    //string newPfoName = @"结构梁_轮廓" + (Guid.NewGuid()).ToString();
                    //MessageBox.Show(filePath + @"\" + newPfoName + @".rfa");
                    strucFaDoc.SaveAs(filePath + @"\" + newPfoName + @".rfa", saveAsOptions);
                    strucFaDoc.Close();

                    // 4 载入 新的结构梁_框架 族文档
                    this.doc.ReLoadFamily(filePath + @"\" + newPfoName + @".rfa");

                    FamilySymbol familySymbol = this.doc.FamilySymbolByName(newPfoName, newPfoName);
                    using (Transaction trans = new Transaction(CMD.Doc, "创建"))
                    {
                        trans.DeleteErrOrWaringTaskDialog();
                        trans.Start();
                        // 5 创建 结构梁_框架 实例
                        if (!familySymbol.IsActive)
                        {
                            familySymbol.Activate();
                        }

                        XYZ location = polyLineInfo.PolyLine.GetOutline().MinimumPoint;

                        //Line line = Line.CreateBound(location, location + new XYZ(0, 10, 0));

                        //FamilyInstance familyInstance = this.doc.Create.NewFamilyInstance(line, familySymbol, this.doc.ActiveView.GenLevel, StructuralType.Beam);
                        FamilyInstance familyInstance = this.doc.Create.NewFamilyInstance(location, familySymbol, this.doc.ActiveView.GenLevel, StructuralType.Beam);
                        trans.Commit();
                    }

                }
                catch (Exception e)
                {
                    MessageBox.Show(e.ToString());
                    //throw new NotImplementedException(e.ToString());
                    continue;
                    //throw;
                }
            }

            //throw new NotImplementedException();
            //$"{sw.ElapsedMilliseconds} + 毫秒".TaskDialogErrorMessage();
        }
    }
}
