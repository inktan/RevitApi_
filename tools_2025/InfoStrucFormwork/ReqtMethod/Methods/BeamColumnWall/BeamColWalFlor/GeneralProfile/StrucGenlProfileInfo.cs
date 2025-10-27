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
using System.IO;

namespace InfoStrucFormwork
{
    class StrucGenlProfileInfo
    {
        internal PolyLineInfo PolyLineInfo { get; set; }
        internal CircleInfo CircleInfo { get; set; }

        internal FamilySymbol FamilySymbol { get; set; }
        internal FamilyInstance FamilyInstance { get; set; }

        /// <summary>
        /// 使用数模分离的轮廓线圈
        /// </summary>
        internal void SetCoordinates()
        {
            if (PolyLineInfo != null)
            {
                List<Vector2d> vs = PolyLineInfo.Polygon2d.Vertices.ToList();
                if (vs.Count <= 32)
                {
                    using (Transaction trans = new Transaction(CMD.Doc, "创建"))
                    {
                        trans.DeleteErrOrWaringTaskDialog();
                        trans.Start();
                        Set32Coordinates(vs);
                        trans.Commit();
                    }
                }
                else
                {
                    SetCoordinates_Profile();
                }
            }
            // 还缺一个圆形柱子
            if (CircleInfo != null)
            {
                SetCoordinates_Profile();
            }
            //throw new NotImplementedException();
        }

        void Set32Coordinates(List<Vector2d> vs)
        {
            if (vs.Count < 32)
            {
                Vector2d p_end = vs.Last();
                Vector2d p_start = vs.First();

                Segment2d segment2d = new Segment2d(p_end, p_start);
                int count = 32 - vs.Count;
                double length = segment2d.Length / (count + 1);

                for (int i = 1; i < count + 1; i++)
                {
                    vs.Add(p_end + segment2d.Direction * i * length);
                }
            }

            //Polygon2d pol = new Polygon2d(points);
            //CMD.Doc.CreateDirectShape(pol.ToCurveLoop().ToList());

            Parameter parameter;
            XYZ xYZ = this.PolyLineInfo.PolyLine.GetOutline().MinimumPoint;
            for (int i = 1; i <= 32; i++)
            {
                parameter = FamilyInstance.LookupParameter("x" + i);
                if (!parameter.IsReadOnly)
                {
                    parameter.Set(vs[i - 1].x - xYZ.X);
                }
                parameter = FamilyInstance.LookupParameter("y" + i);
                if (!parameter.IsReadOnly)
                {
                    parameter.Set(vs[i - 1].y - xYZ.Y);
                }
            }
            parameter = FamilyInstance.LookupParameter("V4端点个数");
            if (!parameter.IsReadOnly)
            {
                parameter.Set(32);
            }
            parameter = FamilyInstance.LookupParameter("轮廓形式");
            if (!parameter.IsReadOnly)
            {
                parameter.Set(2);
            }
        }


        /// <summary>
        /// 单纯的使用重载轮廓线圈
        /// </summary>
        internal void SetCoordinates_Profile()
        {
            // 创建文件夹，使用临时文件夹
            //string filePath = Environment.GetEnvironmentVariable("TEMP");
            string filePath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            //string filePath = CMD.Doc.PathName;
            //filePath = filePath.Replace(CMD.Doc.PathName.Split('\\').Last(), @"\结构墙_轮廓");

            //if (!Directory.Exists(filePath))   
            //{
            //    Directory.CreateDirectory(filePath);
            //}

            //DirectoryInfo dir = new DirectoryInfo(filePath);

            // 数量用于计数，重命名
            //FileInfo[] files = dir.GetFiles();

            // 1 打开 结构墙_轮廓 族文档

            Document strucFaDoc = CMD.Doc.EditFamily(CMD.Doc.FamilyByName("结构墙_轮廓"));
            // 找到 _轮廓 族
            Family profileFa = (new FilteredElementCollector(strucFaDoc)).OfClass(typeof(Family)).FirstOrDefault(p => p.Name == "轮廓_") as Family;
            // 2 打开 最底层 轮廓 族文档
            Document proFileFaDoc = strucFaDoc.EditFamily(profileFa);
            // 清除现有轮廓线
            proFileFaDoc.DelEleIds((new FilteredElementCollector(proFileFaDoc)).OfCategory(BuiltInCategory.OST_Lines).ToElementIds());
            // 创建新的轮廓线
            CurveArray curveArray = new CurveArray();
            if (PolyLineInfo != null)
            {
                for (int i = 0; i < PolyLineInfo.PtsOriginByMin.Count(); ++i)
                {
                    curveArray.Append(Line.CreateBound(PolyLineInfo.PtsOriginByMin[i], PolyLineInfo.PtsOriginByMin[(i + 1) % PolyLineInfo.PtsOriginByMin.Count()]));
                }
            }
            else if (CircleInfo != null)
            {
                curveArray.Append(CircleInfo.Arc);
            }

            // 找到视图
            ViewPlan view = (new FilteredElementCollector(proFileFaDoc)).OfClass(typeof(ViewPlan)).First() as ViewPlan;
            using (Transaction trans = new Transaction(proFileFaDoc, "画轮廓"))
            {
                trans.Start();
                DetailCurveArray detailCurveArray = proFileFaDoc.FamilyCreate.NewDetailCurveArray(view, curveArray);
                trans.Commit();
            }
            SaveAsOptions saveAsOptions = new SaveAsOptions();
            saveAsOptions.MaximumBackups = 1;
            saveAsOptions.OverwriteExistingFile = true;

            string proFileFaDocPath = filePath + @"\轮廓_.rfa";
            if (!proFileFaDoc.PathName.IsNullOrEmpty())
            {
                proFileFaDocPath = filePath + @"\" + proFileFaDoc.PathName.Split('\\').Last();
            }
            proFileFaDoc.SaveAs(proFileFaDocPath , saveAsOptions);
            // 3 载入族轮廓
            strucFaDoc.ReLoadFamily(filePath + @"\" + proFileFaDoc.PathName.Split('\\').Last());
            proFileFaDoc.Close();

            // 保存 新的结构柱 族文件
            string newPfoName = @"结构墙_轮廓" + (Guid.NewGuid()).ToString();

            strucFaDoc.SaveAs(filePath + @"\" + newPfoName + @".rfa", saveAsOptions);
            strucFaDoc.Close();
            // 4 在途竖向构件族

            CMD.Doc.ReLoadFamily(filePath + @"\" + newPfoName + @".rfa");

            using (Transaction trans = new Transaction(CMD.Doc, "修改族类型"))
            {
                trans.Start();
                this.FamilyInstance.Symbol = CMD.Doc.FamilySymbolByName(newPfoName, newPfoName);
                trans.Commit();
            }

            //throw new NotImplementedException();
        }

    }
}
