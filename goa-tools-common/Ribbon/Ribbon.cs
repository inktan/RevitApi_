using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace Ribbon
{
    public class Ribbon : IExternalApplication
    {
        private static string assemblyDirectory
        {
            get
            {
                string codeBase = Assembly.GetExecutingAssembly().CodeBase;
                UriBuilder uri = new UriBuilder(codeBase);
                string path = Uri.UnescapeDataString(uri.Path);
                return Path.GetDirectoryName(path);
            }
        }

        private static string assemblyName
        {
            get
            {
                return Assembly.GetExecutingAssembly().GetName().Name + ".dll";
            }
        }

        /// <summary>
        /// input assembly file name with extension
        /// </summary>
        private static string projectAssemblyFullPath(string _assemblyFile)
        {
            return assemblyDirectory + "\\" + _assemblyFile;
        }

        /// <summary>
        /// input image file name with extension
        /// </summary>
        private static string iconFullPath(string _imageFile)
        {
            return assemblyDirectory + "\\Resources\\" + _imageFile;
        }

        public Result OnStartup(UIControlledApplication application)
        {
            //tab
            application.CreateRibbonTab("goa tools 模型");
            application.CreateRibbonTab("goa tools 制图");
            application.CreateRibbonTab("goa tools 组");

            //var stopwatch = new Stopwatch();
            //stopwatch.Start();

            //ANLS panel
            RibbonPanel rp_anls = application.CreateRibbonPanel("goa tools 模型", "设计分析");
            //ANLS_SOLA button 
            PushButton pb_anls_sola = rp_anls.AddItem(new PushButtonData("ANLS_SOLA", "日照分析", projectAssemblyFullPath("ANLS_SOLA.dll"), "ANLS_SOLA.CMD")) as PushButton;
            pb_anls_sola.LargeImage = new BitmapImage(new Uri(iconFullPath("ANLS_SOLA_96.png")));
            //Surface Area
            PushButtonData pbd_surfaceAreaSingle = new PushButtonData("surfaceAreaSingle", "单个表面", projectAssemblyFullPath("GeometricSurfaceArea.dll"), "GeometricSurfaceArea.CMD");
            PushButtonData pbd_surfaceAreaMultiple = new PushButtonData("surfaceAreaMultiple", "多个表面", projectAssemblyFullPath("GeometricSurfacesArea.dll"), "GeometricSurfacesArea.CMD");
            PulldownButtonData pdbd_surfaceArea = new PulldownButtonData("surfaceArea", "表面面积");
            PulldownButton pdb_surfaceArea = rp_anls.AddItem(pdbd_surfaceArea) as PulldownButton;
            pdb_surfaceArea.AddPushButton(pbd_surfaceAreaSingle);
            pdb_surfaceArea.AddPushButton(pbd_surfaceAreaMultiple);
            pdb_surfaceArea.LargeImage = new BitmapImage(new Uri(iconFullPath("FACE_AREA_96.png")));

            //Dwelling_Assembly
            RibbonPanel rp_Dwellinglayout = application.CreateRibbonPanel("goa tools 模型", "户型库");
            PushButton Dwelling_Assembly = rp_Dwellinglayout.AddItem(new PushButtonData("Dwelling_Assembly", "户型库", projectAssemblyFullPath("Dwelling_Assembly.dll"), "Dwelling_Assembly.CMD")) as PushButton;

            //GRID panel
            RibbonPanel rp_grid = application.CreateRibbonPanel("goa tools 制图", "轴网工具");
            //GRID_IDEN button
            PushButton pb_grid_iden = rp_grid.AddItem(new PushButtonData("GRID_IDEN", "标头线", projectAssemblyFullPath("GRID_IDEN.dll"), "GRID_IDEN.CMD")) as PushButton;
            pb_grid_iden.LargeImage = new BitmapImage(new Uri(iconFullPath("GRID_IDEN_96.png")));
            //DATM_ON_OFF button
            PushButton pb_datm_onoff = rp_grid.AddItem(new PushButtonData("DATM_ON_OFF", "开/关", projectAssemblyFullPath("DATM_ON_OFF.dll"), "DATM_ON_OFF.CMD")) as PushButton;
            pb_datm_onoff.LargeImage = new BitmapImage(new Uri(iconFullPath("DATM_ON_OFF_96.png")));
            //GRID_RNAM button
            PushButton pb_grid_rnam = rp_grid.AddItem(new PushButtonData("GRID_Number", "重命名", projectAssemblyFullPath("GRID_Number.dll"), "GRID_Number.CMD")) as PushButton;
            pb_grid_rnam.LargeImage = new BitmapImage(new Uri(iconFullPath("GRID_RNAM_96.png")));


            //ANNO panel
            RibbonPanel rp_anno = application.CreateRibbonPanel("goa tools 制图", "快捷标注");
            //ANNO_AREA button
            PushButton pb_anno_area = rp_anno.AddItem(new PushButtonData("ANNO_AREA", "分区面积", projectAssemblyFullPath("ANNO_AREA.dll"), "ANNO_AREA.CMD")) as PushButton;
            pb_anno_area.LargeImage = new BitmapImage(new Uri(iconFullPath("ANNO_AREA_96.png")));
            //ANNO_DIST button
            PushButton pb_anno_dist = rp_anno.AddItem(new PushButtonData("ANNO_DIST", "疏散距离", projectAssemblyFullPath("ANNO_DIST.dll"), "ANNO_DIST.CMD")) as PushButton;
            pb_anno_dist.LargeImage = new BitmapImage(new Uri(iconFullPath("ANNO_DIST_96.png")));
            //ANNO_ELEV button
            PushButton pb_anno_elev = rp_anno.AddItem(new PushButtonData("ANNO_ELEV", "降板标高", projectAssemblyFullPath("ANNO_ELEV.dll"), "ANNO_ELEV.CMD")) as PushButton;
            pb_anno_elev.LargeImage = new BitmapImage(new Uri(iconFullPath("ANNO_ELEV_96.png")));
            //ANNO_ELEV_ABSL button
            PushButton pb_anno_elev_absl = rp_anno.AddItem(new PushButtonData("ANNO_ELEV_ABSL", "绝对标高", projectAssemblyFullPath("ANNO_ELEV_ABSL.dll"), "ANNO_ELEV_ABSL.CMD")) as PushButton;
            pb_anno_elev_absl.LargeImage = new BitmapImage(new Uri(iconFullPath("ANNO_ELEV_ABSL_96.png")));
            //ANNO_AVOD button
            PushButton pb_anno_avod = rp_anno.AddItem(new PushButtonData("ANNO_AVOD", "尺寸避让", projectAssemblyFullPath("ANNO_AVOD.dll"), "ANNO_AVOD.CMD")) as PushButton;
            pb_anno_avod.LargeImage = new BitmapImage(new Uri(iconFullPath("ANNO_AVOD_96.png")));
            //ANNO_ELEV_MULT button
            PushButton pb_anno_elev_mult = rp_anno.AddItem(new PushButtonData("ANNO_ELEV_MULT", "多重标高", projectAssemblyFullPath("ANNO_ELEV_MULT.dll"), "ANNO_ELEV_MULT.CMD")) as PushButton;
            pb_anno_elev_mult.LargeImage = new BitmapImage(new Uri(iconFullPath("ANNO_ELEV_MULT_96.png")));
            //ANNO_ELEV_MULT button
            PushButton pb_fakeElev_Refresh = rp_anno.AddItem(new PushButtonData("FakeElev_Refresh", "假标高计算", projectAssemblyFullPath("FakeElev_Refresh.dll"), "FakeElev_Refresh.CMD")) as PushButton;

            //COMP panel
            RibbonPanel rp_comp = application.CreateRibbonPanel("goa tools 模型", "构件管理");
            //COMP_MANG button
            PushButton pb_comp_mang = rp_comp.AddItem(new PushButtonData("COMP_MANG", "组件管理", projectAssemblyFullPath("COMP_MANG.dll"), "COMP_MANG.CMD")) as PushButton;
            pb_comp_mang.LargeImage = new BitmapImage(new Uri(iconFullPath("COMP_MANG_96.png")));
            //FAMI_REPL button
            PushButton pb_fami_repl = rp_comp.AddItem(new PushButtonData("FAMI_REPL", "族库替换", projectAssemblyFullPath("FAMI_REPL.dll"), "FAMI_REPL.CMD")) as PushButton;
            pb_fami_repl.LargeImage = new BitmapImage(new Uri(iconFullPath("FAMI_REPL_96.png")));
            //PARA_COPY
            PushButton pb_para_copy = rp_comp.AddItem(new PushButtonData("PARA_COPY", "参数复制", projectAssemblyFullPath("PARA_COPY.dll"), "PARA_COPY.CMD")) as PushButton;
            pb_para_copy.LargeImage = new BitmapImage(new Uri(iconFullPath("PARA_COPY_96.png")));
            //TransGroupParas
            PushButton pb_trans_groupParas = rp_comp.AddItem(new PushButtonData("TransGroupParas", "组内族参数传递", projectAssemblyFullPath("TransGroupParas.dll"), "TransGroupParas.CMD")) as PushButton;
            pb_trans_groupParas.LargeImage = new BitmapImage(new Uri(iconFullPath("TransGroupParas_96.png")));

            //MODELLING panel
            RibbonPanel rp_model = application.CreateRibbonPanel("goa tools 模型", "建模工具");
            //stacked items group 1
            var pbd_selc_dependents = new PushButtonData("SELC_DPNT", "选择附属图元", projectAssemblyFullPath("SELC_DPNT.dll"), "SELC_DPNT.CMD");
            var pbd_selc_group_of_same_level = new PushButtonData("selectAllGroup", "选择同层模型组", projectAssemblyFullPath("selectAllGroup.dll"), "selectAllGroup.CMD");
            var pbd_join_mult = new PushButtonData("JOIN_MULT", "批量连接", projectAssemblyFullPath("JOIN_MULT.dll"), "JOIN_MULT.CMD");
            rp_model.AddStackedItems(
                pbd_selc_dependents,
                pbd_selc_group_of_same_level,
                pbd_join_mult);
            //stacked items group 2
            var pbd_move_3d = new PushButtonData("pointToPoint3DMove", "三维移动", projectAssemblyFullPath("pointToPoint3DMove.dll"), "pointToPoint3DMove.CMD");
            var pbd_wall_miter = new PushButtonData("wallMiterConnect", "墙批量改斜接", projectAssemblyFullPath("wallMiterConnect.dll"), "wallMiterConnect.CMD");
            var pbd_arry_tool = new PushButtonData("arryTool", "沿路径阵列", projectAssemblyFullPath("ARRY_TOOL.dll"), "ARRY_TOOL.CMD");
            rp_model.AddStackedItems(
                pbd_move_3d,
                pbd_wall_miter,
                pbd_arry_tool);
            //PROF_EDIT
            PushButtonData pbd_prof_edit_facade = new PushButtonData("PROF_EDIT_FACADE", "造型构件", projectAssemblyFullPath("PROF_EDIT.dll"), "PROF_EDIT.CMD");
            PushButtonData pbd_prof_edit_sitePlan = new PushButtonData("PROF_EDIT_SITEPLAN", "总图体块", projectAssemblyFullPath("PROF_EDIT.dll"), "PROF_EDIT.CMD_SitePlan");
            PulldownButtonData pdbd_prof_edit = new PulldownButtonData("PROF_EDIT", "轮廓编辑");
            PulldownButton pdb_prof_edit = rp_model.AddItem(pdbd_prof_edit) as PulldownButton;
            pdb_prof_edit.AddPushButton(pbd_prof_edit_facade);
            pdb_prof_edit.AddPushButton(pbd_prof_edit_sitePlan);
            pdb_prof_edit.LargeImage = new BitmapImage(new Uri(iconFullPath("PROF_EDIT_96.png")));

            //LEVL_EDIT
            //PushButton pb_levl_edit = rp_model.AddItem(new PushButtonData("LEVL_EDIT", "修改层高", projectAssemblyFullPath("LEVL_EDIT.dll"), "LEVL_EDIT.CMD")) as PushButton;
            // need icon
            //PLAN_EDIT
            //PushButton pb_plan_edit = rp_model.AddItem(new PushButtonData("PLAN_EDIT", "修改平面", projectAssemblyFullPath("PLAN_EDIT.dll"), "PLAN_EDIT.CMD")) as PushButton;
            // need icon

            //DRAW panel
            RibbonPanel rp_draw = application.CreateRibbonPanel("goa tools 制图", "制图工具");
            //DRAW_PATT 
            PushButton pb_draw_patt = rp_draw.AddItem(new PushButtonData("DRAW_PATT", "截面填充", projectAssemblyFullPath("DRAW_PATT.dll"), "DRAW_PATT.CMD")) as PushButton;
            pb_draw_patt.LargeImage = new BitmapImage(new Uri(iconFullPath("DRAW_PATT_96.png")));
            //FAKE_DIMS
            PushButton pb_fake_dims = rp_draw.AddItem(new PushButtonData("FAKE_DIMS", "尺寸标注", projectAssemblyFullPath("FAKE_DIMS.dll"), "FAKE_DIMS.CMD")) as PushButton;
            pb_fake_dims.LargeImage = new BitmapImage(new Uri(iconFullPath("FAKE_DIMS_96.png")));
            //LINE_ROAD
            PushButton pd_line_road = rp_draw.AddItem(new PushButtonData("LINE_ROAD", "单线变路", projectAssemblyFullPath("LINE_ROAD.dll"), "LINE_ROAD.CMD")) as PushButton;
            pd_line_road.LargeImage = new BitmapImage(new Uri(iconFullPath("LINE_ROAD_96.png")));
            //COLR_SWIT
            PushButton pb_colr_swit = rp_draw.AddItem(new PushButtonData("COLR_SWIT", "颜色切换", projectAssemblyFullPath("COLR_SWIT.dll"), "COLR_SWIT.CMD")) as PushButton;
            pb_colr_swit.LargeImage = new BitmapImage(new Uri(iconFullPath("COLR_SWIT_96.png")));
            //WIND_NUMB
            PushButton pb_wind_numb = rp_draw.AddItem(new PushButtonData("WIND_NUMB", "门窗编号", projectAssemblyFullPath("WIND_NUMB.dll"), "WIND_NUMB.CMD")) as PushButton;
            pb_wind_numb.LargeImage = new BitmapImage(new Uri(iconFullPath("WIND_NUMB_96.png")));

            //VIEW panel
            RibbonPanel rp_view = application.CreateRibbonPanel("goa tools 制图", "视图工具");
            //VIEW_INTF button
            PushButton pb_view_intf = rp_view.AddItem(new PushButtonData("VIEW_INTF", "提资视图", projectAssemblyFullPath("VIEW_INTF.dll"), "VIEW_INTF.CMD")) as PushButton;
            pb_view_intf.LargeImage = new BitmapImage(new Uri(iconFullPath("VIEW_INTF_96.png")));
            PushButton pb_type_on_off = rp_view.AddItem(new PushButtonData("TYPE_ON_OFF", "类型开关", projectAssemblyFullPath("TYPE_ON_OFF.dll"), "TYPE_ON_OFF.CMD")) as PushButton;
            pb_type_on_off.LargeImage = new BitmapImage(new Uri(iconFullPath("TYPE_ON_OFF_96.png")));
            PushButton pb_west_on_off = rp_view.AddItem(new PushButtonData("WSET_ON_OFF", "工作集开关", projectAssemblyFullPath("WSET_ON_OFF.dll"), "WSET_ON_OFF.CMD")) as PushButton;
            pb_west_on_off.LargeImage = new BitmapImage(new Uri(iconFullPath("WSET_ON_OFF_96.png")));
            //VIEW_Duplicate
            PushButton pb_view_duplicate = rp_view.AddItem(new PushButtonData("VIEW_Duplicate", "视图复制", projectAssemblyFullPath("VIEW_Duplicate.dll"), "VIEW_Duplicate.CMD")) as PushButton;

            //SHET panel
            RibbonPanel rp_shet = application.CreateRibbonPanel("goa tools 制图", "布图工具");
            //SHET_LOCA button
            PushButton pb_shet_loca = rp_shet.AddItem(new PushButtonData("SHET_LOCA", "区位图", projectAssemblyFullPath("SHET_LOCA.dll"), "SHET_LOCA.CMD")) as PushButton;
            pb_shet_loca.LargeImage = new BitmapImage(new Uri(iconFullPath("SHET_LOCA_96.png")));
            //SHET_INFO button
            PushButton pb_shet_info = rp_shet.AddItem(new PushButtonData("SHET_INFO", "图签信息", projectAssemblyFullPath("SHET_INFO.dll"), "SHET_INFO.CMD")) as PushButton;
            pb_shet_info.LargeImage = new BitmapImage(new Uri(iconFullPath("SHET_INFO_96.png")));
            //SHET_NEW button
            PushButton pb_shet_new = rp_shet.AddItem(new PushButtonData("SHET_NEW", "创建图纸", projectAssemblyFullPath("SHET_NEW.dll"), "SHET_NEW.CMD")) as PushButton;
            pb_shet_new.LargeImage = new BitmapImage(new Uri(iconFullPath("SHET_NEW_96.png")));

            //stopwatch.Stop();
            //TaskDialog.Show("test", stopwatch.ElapsedMilliseconds.ToString());

            return Result.Succeeded;
        }

        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }
    }
}
