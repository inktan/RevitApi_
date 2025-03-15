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
using Autodesk.Revit.ApplicationServices;

namespace InfoStrucFormwork
{
    /// <summary>
    /// 用于在粘贴操作期间遇到的重复类型名称的自定义处理程序的接口。当目标文档包含与复制的类型具有相同名称但内部机制不同的类型时，必须决定如何继续——是取消操作还是继续，但只复制具有唯一名称的类型。
    /// </summary>
    public class CopyEventHandler : IDuplicateTypeNamesHandler
    {
        public DuplicateTypeAction OnDuplicateTypeNamesFound(DuplicateTypeNamesHandlerArgs args)
        {
            return DuplicateTypeAction.UseDestinationTypes;
        }
    }
    internal class EleSeparate : RequestMethod
    {
        internal EleSeparate(UIApplication _uiApp) : base(_uiApp)
        {
        }

        public string PathName { get; set; }

        internal override void Execute()
        {
            this.PathName = this.doc.PathName;
            try
            {
                ModelPath modelPath = this.doc.GetWorksharingCentralModelPath();
                if (!modelPath.Empty)
                {
                    this.PathName = ModelPathUtils.ConvertModelPathToUserVisiblePath(modelPath);
                }
            }
            catch (Exception)
            {
                //throw;
            }
            if (this.PathName.Contains('\\'))
            {
                this.PathName.TaskDialogErrorMessage();
            }
            else
            {
                throw new NotImplementedException("请先保存模型后，再运行该插件");
            }

            //return;

            ExecuteWallCol();
            ExecuteBeam();
            ExecuteFloor();
        }

        internal void ExecuteWallCol()
        {
            Document wallColDoc = this.app.NewProjectDocument(UnitSystem.Metric);

            using (Transaction trans = new Transaction(wallColDoc))
            {
                trans.DeleteErrOrWaringTaskDialog();
                trans.Start("-设置规程-");

                ElementCategoryFilter vieCategoryFilter = new ElementCategoryFilter(BuiltInCategory.OST_Views);
                FilteredElementCollector collector = new FilteredElementCollector(wallColDoc);
                ICollection<Element> viewElements = collector.OfCategory(BuiltInCategory.OST_Views).ToElements();
                foreach (var item in viewElements)
                {
                    if (item is View3D view3D)
                    {
                        view3D.get_Parameter(BuiltInParameter.VIEW_DISCIPLINE).Set(1);
                    }
                    else if (item is ViewPlan view)
                    {
                        view.get_Parameter(BuiltInParameter.VIEW_DISCIPLINE).Set(1);
                    }
                }

                trans.Commit();
            }

            using (Transaction trans = new Transaction(wallColDoc))
            {
                trans.DeleteErrOrWaringTaskDialog();
                trans.Start("-提取墙元素-");
                CopyPasteOptions opts = new CopyPasteOptions();
                opts.SetDuplicateTypeNamesHandler(new CopyEventHandler());

                List<Element> wallEles = new FilteredElementCollector(this.doc).OfClass(typeof(Wall)).Where(p => p is Wall).ToList();
                List<FamilyInstance> wallFis = new FilteredElementCollector(this.doc)
                    .OfClass(typeof(FamilyInstance))
                    .Select(p => p as FamilyInstance)
                    .Where(p => p.Symbol.FamilyName.Contains("墙"))
                    .ToList();

                //wallEles.Count.ToString().TaskDialogErrorMessage();
                //wallFis.Count.ToString().TaskDialogErrorMessage();
                if (wallEles.Count > 0)
                {
                    ElementTransformUtils.CopyElements(this.doc, wallEles.Select(p => p.Id).ToList(), wallColDoc, null, opts);
                }
                if (wallFis.Count > 0)
                {
                    ElementTransformUtils.CopyElements(this.doc, wallFis.Select(p => p.Id).ToList(), wallColDoc, null, opts);
                }
                trans.Commit();
            }

            using (Transaction trans = new Transaction(wallColDoc))
            {
                trans.DeleteErrOrWaringTaskDialog();
                trans.Start("-提取柱元素-");
                CopyPasteOptions opts = new CopyPasteOptions();
                opts.SetDuplicateTypeNamesHandler(new CopyEventHandler());

                List<FamilyInstance> fis = new FilteredElementCollector(this.doc)
                    .OfClass(typeof(FamilyInstance))
                    .Select(p => p as FamilyInstance)
                    .Where(p => p.Symbol.FamilyName.Contains("柱"))
                    .ToList();

                //fis.Count.ToString().TaskDialogErrorMessage();
                if (fis.Count > 0)
                {
                    ElementTransformUtils.CopyElements(this.doc, fis.Select(p => p.Id).ToList(), wallColDoc, null, opts);
                }

                trans.Commit();
            }

            SaveAsOptions saveAsOptions = new SaveAsOptions
            {
                MaximumBackups = 1,
                OverwriteExistingFile = true
            };

            string wallFilePath = this.PathName.Replace(".rvt", "-墙柱.rvt");
            wallColDoc.SaveAs(wallFilePath, saveAsOptions);
            wallColDoc.Close();
        }

        internal void ExecuteBeam()
        {
            Document beamDoc = this.app.NewProjectDocument(UnitSystem.Metric);

            using (Transaction trans = new Transaction(beamDoc))
            {
                trans.DeleteErrOrWaringTaskDialog();
                trans.Start("-设置规程-");

                ElementCategoryFilter vieCategoryFilter = new ElementCategoryFilter(BuiltInCategory.OST_Views);
                FilteredElementCollector collector = new FilteredElementCollector(beamDoc);
                ICollection<Element> viewElements = collector.OfCategory(BuiltInCategory.OST_Views).ToElements();
                foreach (var item in viewElements)
                {
                    if (item is View3D view3D)
                    {
                        view3D.get_Parameter(BuiltInParameter.VIEW_DISCIPLINE).Set(1);
                    }
                    else if (item is ViewPlan view)
                    {
                        view.get_Parameter(BuiltInParameter.VIEW_DISCIPLINE).Set(1);
                    }
                }

                trans.Commit();
            }

            using (Transaction trans = new Transaction(beamDoc))
            {
                trans.DeleteErrOrWaringTaskDialog();
                trans.Start("-提取梁元素-");
                CopyPasteOptions opts = new CopyPasteOptions();
                opts.SetDuplicateTypeNamesHandler(new CopyEventHandler());

                List<FamilyInstance> fis = new FilteredElementCollector(this.doc)
                    .OfClass(typeof(FamilyInstance))
                    .Select(p => p as FamilyInstance)
                    .Where(p => p.Symbol.FamilyName.Contains("梁"))
                    .ToList();

                //fis.Count.ToString().TaskDialogErrorMessage();
                if (fis.Count > 0)
                {
                    ElementTransformUtils.CopyElements(this.doc, fis.Select(p => p.Id).ToList(), beamDoc, null, opts);
                }

                trans.Commit();
            }

            SaveAsOptions saveAsOptions = new SaveAsOptions
            {
                MaximumBackups = 1,
                OverwriteExistingFile = true
            };

            string filePath = this.PathName.Replace(".rvt", "-梁.rvt");
            beamDoc.SaveAs(filePath, saveAsOptions);
            beamDoc.Close();

        }

        internal void ExecuteFloor()
        {
            Document beamDoc = this.app.NewProjectDocument(UnitSystem.Metric);

            using (Transaction trans = new Transaction(beamDoc))
            {
                trans.DeleteErrOrWaringTaskDialog();
                trans.Start("-设置规程-");

                ElementCategoryFilter vieCategoryFilter = new ElementCategoryFilter(BuiltInCategory.OST_Views);
                FilteredElementCollector collector = new FilteredElementCollector(beamDoc);
                ICollection<Element> viewElements = collector.OfCategory(BuiltInCategory.OST_Views).ToElements();
                foreach (var item in viewElements)
                {
                    if (item is View3D view3D)
                    {
                        view3D.get_Parameter(BuiltInParameter.VIEW_DISCIPLINE).Set(1);
                    }
                    else if (item is ViewPlan view)
                    {
                        view.get_Parameter(BuiltInParameter.VIEW_DISCIPLINE).Set(1);
                    }
                }

                trans.Commit();
            }

            using (Transaction trans = new Transaction(beamDoc))
            {
                trans.DeleteErrOrWaringTaskDialog();
                trans.Start("-提取梁元素-");
                CopyPasteOptions opts = new CopyPasteOptions();
                opts.SetDuplicateTypeNamesHandler(new CopyEventHandler());

                List<Element> fis = new FilteredElementCollector(this.doc).OfClass(typeof(Floor)).ToList();

                //fis.Count.ToString().TaskDialogErrorMessage();
                if (fis.Count > 0)
                {
                    ElementTransformUtils.CopyElements(this.doc, fis.Select(p => p.Id).ToList(), beamDoc, null, opts);
                }

                trans.Commit();
            }

            SaveAsOptions saveAsOptions = new SaveAsOptions
            {
                MaximumBackups = 1,
                OverwriteExistingFile = true
            };

            string filePath = this.PathName.Replace(".rvt", "-楼板.rvt");
            beamDoc.SaveAs(filePath, saveAsOptions);
            beamDoc.Close();

        }

    }
}
