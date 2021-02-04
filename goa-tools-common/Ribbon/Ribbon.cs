using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

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
            application.CreateRibbonTab("_revitApi_ tools");

            //application.CreateRibbonTab("goa tools 模型");
            //application.CreateRibbonTab("goa tools 制图");
            //application.CreateRibbonTab("goa tools 模块");



            /*
            #region model tab
            //SITE PLANNING panel
            RibbonPanel rp_sitePlan = application.CreateRibbonPanel("goa tools 模型", "总图规划");
            //ANLS_SOLA 
            PushButton pb_anls_sola = rp_sitePlan.AddItem(new PushButtonData("ANLS_SOLA", "日照分析", projectAssemblyFullPath("ANLS_SOLA.dll"), "ANLS_SOLA.CMD")) as PushButton;
            pb_anls_sola.LargeImage = new BitmapImage(new Uri(iconFullPath("ANLS_SOLA_96.png")));
            //Stack items
            PushButtonData pbd_site_family_edit = new PushButtonData("SitePlanFamilyEditor", "体块编辑", projectAssemblyFullPath("SitePlanFamilyEditor.dll"), "SitePlanFamilyEditor.CMD");
            PushButtonData pbd_site_family_select = new PushButtonData("SitePlanBuildingBlockSelector", "体块选择", projectAssemblyFullPath("SitePlanBuildingBlockSelector.dll"), "SitePlanBuildingBlockSelector.CMD");
            PushButtonData pbd_color_pallete = new PushButtonData("FastColorPalette", "调色板增强", projectAssemblyFullPath("FastColorPalette.dll"), "FastColorPalette.CMD");
            rp_sitePlan.AddStackedItems(
                pbd_site_family_edit,
                pbd_site_family_select,
                pbd_color_pallete);

            //ANLS panel
            RibbonPanel rp_anls = application.CreateRibbonPanel("goa tools 模型", "设计分析");
            //Surface Area
            PushButtonData pbd_surfaceAreaSingle = new PushButtonData("surfaceAreaSingle", "单个表面", projectAssemblyFullPath("GeometricSurfaceArea.dll"), "GeometricSurfaceArea.CMD");
            PushButtonData pbd_surfaceAreaMultiple = new PushButtonData("surfaceAreaMultiple", "多个表面", projectAssemblyFullPath("GeometricSurfacesArea.dll"), "GeometricSurfacesArea.CMD");
            PulldownButtonData pdbd_surfaceArea = new PulldownButtonData("surfaceArea", "表面面积");
            PulldownButton pdb_surfaceArea = rp_anls.AddItem(pdbd_surfaceArea) as PulldownButton;
            pdb_surfaceArea.AddPushButton(pbd_surfaceAreaSingle);
            pdb_surfaceArea.AddPushButton(pbd_surfaceAreaMultiple);
            pdb_surfaceArea.LargeImage = new BitmapImage(new Uri(iconFullPath("FACE_AREA_96.png")));

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
            var pbd_wall_miter = new PushButtonData("wallMiterConnect", "墙批量改斜接", projectAssemblyFullPath("wallMiterConnect.dll"), "wallMiterConnect.CMD");
            var pbd_comment_mgr = new PushButtonData("commentMgr", "注释管理", projectAssemblyFullPath("CommentMgr.dll"), "CommentMgr.CMD");
            var pbd_arry_tool = new PushButtonData("arryTool", "沿路径阵列", projectAssemblyFullPath("ARRY_TOOL.dll"), "ARRY_TOOL.CMD");
            rp_model.AddStackedItems(
                pbd_wall_miter,
                pbd_comment_mgr,
                pbd_arry_tool);
            //stacked items group 3
            var pbd_move_3d = new PushButtonData("pointToPoint3DMove", "三维移动", projectAssemblyFullPath("pointToPoint3DMove.dll"), "pointToPoint3DMove.CMD");
            var pbd_cut_geometry = new PushButtonData("CutGeometry", "切割形体", projectAssemblyFullPath("CutGeometry.dll"), "CutGeometry.CMD");

            rp_model.AddStackedItems(
                pbd_move_3d,
                pbd_cut_geometry);

            //stacked items group 4
            var pbd_super_mirror = new PushButtonData("superMirror", "超级镜像", projectAssemblyFullPath("GROUP_CREATE.dll"), "GROUP_CREATE.Mirror_Pick");
            var pbd_fractionChecker = new PushButtonData("fractionChecker", "碎数检查", projectAssemblyFullPath("FractionNumberChecker.dll"), "FractionNumberChecker.CMD");

            rp_model.AddStackedItems(
                pbd_super_mirror,
                pbd_fractionChecker);

            //FAM_CVTR
            PushButton pb_fam_cvtr = rp_comp.AddItem(new PushButtonData("familyConvertor", "族转换", projectAssemblyFullPath("FAMI_CVTR.dll"), "FAMI_CVTR.CMD")) as PushButton;
            //need icon?

            //LEVL_EDIT
            PushButton pb_levl_edit = rp_model.AddItem(new PushButtonData("LEVL_EDIT", "修改层高", projectAssemblyFullPath("LEVL_EDIT.dll"), "LEVL_EDIT.CMD")) as PushButton;
            pb_levl_edit.LargeImage = new BitmapImage(new Uri(iconFullPath("LEVL_EDIT_96.png")));
            //PLAN_EDIT
            PushButton pb_smart_stretch = rp_model.AddItem(new PushButtonData("SmartStretch", "智能拉伸", projectAssemblyFullPath("SmartStretch.dll"), "SmartStretch.CMD")) as PushButton;
            pb_smart_stretch.LargeImage = new BitmapImage(new Uri(iconFullPath("SmartStretch_96.png")));

            //Layout Parking Effcient
            RibbonPanel rp_ParkingLayout = application.CreateRibbonPanel("goa tools 模型", "地库强排");
            PushButton Parking_Layout = rp_ParkingLayout.AddItem(new PushButtonData("ParkingLayoutEfficientNewStructual", "地库强排", projectAssemblyFullPath("ParkingLayoutEfficientNewStructual.dll"), "ParkingLayoutEfficientNewStructual.CMD")) as PushButton;
            Parking_Layout.LargeImage = new BitmapImage(new Uri(iconFullPath("LayoutParking_96.png")));

            //basement design toolset
            var pbd_bsmt_buildingBlocks = new PushButtonData("BSMT_BuildingBlocks", "地上楼栋", projectAssemblyFullPath("BSMT_BuildingBlocks.dll"), "BSMT_BuildingBlocks.CMD");
            var pbd_bsmt_excavation = new PushButtonData("BSMT_Excavation", "土方估算", projectAssemblyFullPath("BSMT_Excavation.dll"), "BSMT_Excavation.CMD");
            var pbd_bsmt_regions = new PushButtonData("BSMT_Regions", "分区面积", projectAssemblyFullPath("BSMT_Regions.dll"), "BSMT_Regions.CMD");
            rp_ParkingLayout.AddStackedItems(
                pbd_bsmt_buildingBlocks,
                pbd_bsmt_excavation,
                pbd_bsmt_regions);

            //Dwelling_Assembly
            RibbonPanel rp_Dwellinglayout = application.CreateRibbonPanel("goa tools 模型", "户型库");
            PushButton Dwelling_Assembly = rp_Dwellinglayout.AddItem(new PushButtonData("Dwelling_Assembly", "户型库", projectAssemblyFullPath("Dwelling_Assembly.dll"), "Dwelling_Assembly.CMD")) as PushButton;

            //user assistant panel
            RibbonPanel rp_userAssistant = application.CreateRibbonPanel("goa tools 模型", "智能助手");
            PushButton pb_userAssistant = rp_userAssistant.AddItem(new PushButtonData("userAssistant", "设置", projectAssemblyFullPath("UserAssistant.dll"), "UserAssistant.CMD")) as PushButton;

            //building modeling panel
            RibbonPanel rp_highrise = application.CreateRibbonPanel("goa tools 模型", "楼栋建模");
            //PLAN_DSGN_TOOLS
            PushButton pb_plan_tools = rp_highrise.AddItem(new PushButtonData("PLAN_DSGN_TOOLS", "户型工具", projectAssemblyFullPath("PLAN_DSGN_TOOLS.dll"), "PLAN_DSGN_TOOLS.CMD")) as PushButton;
            pb_plan_tools.LargeImage = new BitmapImage(new Uri(iconFullPath("PLAN_EDIT_96.png")));
            //FACD_DSGN_TOOLS
            PushButton pb_facd_tools = rp_highrise.AddItem(new PushButtonData("FACD_DSGN_TOOLS", "造型工具", projectAssemblyFullPath("FACD_DSGN_TOOLS.dll"), "FACD_DSGN_TOOLS.CMD")) as PushButton;
            pb_facd_tools.LargeImage = new BitmapImage(new Uri(iconFullPath("PROF_EDIT_96.png")));

            //FACD_DSGN_TOOLS_RibbonButtons
            RibbonPanel rp_facade_hotKeys = application.CreateRibbonPanel("goa tools 模型", "造型工具快捷键");
            var pbd_facade_dyMoStartEnd = new PushButtonData("dyMoStartEnd", "动态模块", projectAssemblyFullPath("FACD_DSGN_TOOLS.dll"), "FACD_DSGN_TOOLS.CMD_callFromRibbon_DymoSessionStartEnd");
            var pbd_facade_dyMoDispose = new PushButtonData("dyMoDispose", "放弃模块", projectAssemblyFullPath("FACD_DSGN_TOOLS.dll"), "FACD_DSGN_TOOLS.CMD_callFromRibbon_DymoSessionDispose");
            rp_facade_hotKeys.AddStackedItems(
                pbd_facade_dyMoStartEnd,
                pbd_facade_dyMoDispose);

            var pbd_facade_dyMoMode1 = new PushButtonData("dymoMode1", "模式P", projectAssemblyFullPath("FACD_DSGN_TOOLS.dll"), "FACD_DSGN_TOOLS.CMD_callFromRibbon_DymoMode_1");
            var pbd_facade_dyMoMode2 = new PushButtonData("dymoMode2", "模式E", projectAssemblyFullPath("FACD_DSGN_TOOLS.dll"), "FACD_DSGN_TOOLS.CMD_callFromRibbon_DymoMode_2");
            var pbd_facade_dyMoMode3 = new PushButtonData("dymoMode3", "模式M", projectAssemblyFullPath("FACD_DSGN_TOOLS.dll"), "FACD_DSGN_TOOLS.CMD_callFromRibbon_DymoMode_3");
            rp_facade_hotKeys.AddStackedItems(
                pbd_facade_dyMoMode1,
                pbd_facade_dyMoMode2,
                pbd_facade_dyMoMode3);

            var pbd_autoSwitchEnd = new PushButtonData("autoSwitchEnd", "端头切换", projectAssemblyFullPath("FACD_DSGN_TOOLS.dll"), "FACD_DSGN_TOOLS.CMD_callFromRibbon_AutoSwitchEnd");
            var pbd_facade_switchTransparency = new PushButtonData("switchTransparency", "切换透明", projectAssemblyFullPath("FACD_DSGN_TOOLS.dll"), "FACD_DSGN_TOOLS.CMD_callFromRibbon_switchTransparency");
            var pbd_facade_pushFace = new PushButtonData("pushFace", "推拉形体", projectAssemblyFullPath("FACD_DSGN_TOOLS.dll"), "FACD_DSGN_TOOLS.CMD_callFromRibbon_pushFace");
            rp_facade_hotKeys.AddStackedItems(
                pbd_autoSwitchEnd,
                pbd_facade_switchTransparency,
                pbd_facade_pushFace);

            var pbd_facade_trimExt = new PushButtonData("facade_trimExt", "修剪/延伸", projectAssemblyFullPath("FACD_DSGN_TOOLS.dll"), "FACD_DSGN_TOOLS.CMD_callFromRibbon_trimExt");
            var pbd_facade_trimCorner = new PushButtonData("facade_trimCorner", "修剪为角", projectAssemblyFullPath("FACD_DSGN_TOOLS.dll"), "FACD_DSGN_TOOLS.CMD_callFromRibbon_trimCorner");
            var pbd_facade_split = new PushButtonData("facade_split", "截断", projectAssemblyFullPath("FACD_DSGN_TOOLS.dll"), "FACD_DSGN_TOOLS.CMD_callFromRibbon_split");
            rp_facade_hotKeys.AddStackedItems(
                pbd_facade_trimExt,
                pbd_facade_trimCorner,
                pbd_facade_split);

            var pbd_facade_alignMove = new PushButtonData("facade_alignMove", "对齐移动", projectAssemblyFullPath("FACD_DSGN_TOOLS.dll"), "FACD_DSGN_TOOLS.CMD_callFromRibbon_alignMove");
            var pbd_facade_alignCopy = new PushButtonData("facade_alignCopy", "对齐复制", projectAssemblyFullPath("FACD_DSGN_TOOLS.dll"), "FACD_DSGN_TOOLS.CMD_callFromRibbon_alignCopy");
            rp_facade_hotKeys.AddStackedItems(
                pbd_facade_alignMove,
                pbd_facade_alignCopy);

            var pbd_facade_selectCompsFiltered = new PushButtonData("facade_selectCompsFiltered", "选择/过滤", projectAssemblyFullPath("FACD_DSGN_TOOLS.dll"), "FACD_DSGN_TOOLS.CMD_callFromRibbon_selectCompsFiltered");
            var pbd_facade_mirrorComps = new PushButtonData("facade_mirrorComps", "镜像", projectAssemblyFullPath("FACD_DSGN_TOOLS.dll"), "FACD_DSGN_TOOLS.CMD_callFromRibbon_mirrorComps");
            rp_facade_hotKeys.AddStackedItems(
                pbd_facade_selectCompsFiltered,
                pbd_facade_mirrorComps);
            #endregion model tab
        
            #region drafting tab
            //GRID panel
            RibbonPanel rp_grid = application.CreateRibbonPanel("goa tools 制图", "轴网工具");
            //GRID_IDEN button
            PushButton pb_grid_iden = rp_grid.AddItem(new PushButtonData("GRID_IDEN", "标头线", projectAssemblyFullPath("GRID_IDEN.dll"), "GRID_IDEN.CMD")) as PushButton;
            pb_grid_iden.LargeImage = new BitmapImage(new Uri(iconFullPath("GRID_IDEN_96.png")));
            //DATM_ON_OFF button
            PushButton pb_datm_onoff = rp_grid.AddItem(new PushButtonData("DATM_ON_OFF", "开/关", projectAssemblyFullPath("DATM_ON_OFF.dll"), "DATM_ON_OFF.CMD")) as PushButton;
            pb_datm_onoff.LargeImage = new BitmapImage(new Uri(iconFullPath("DATM_ON_OFF_96.png")));
            //GRID_RNAM button
            PushButtonData pdb_grid_rnam_true = new PushButtonData("GRID_Number", "真轴号", projectAssemblyFullPath("GRID_Number.dll"), "GRID_Number.CMD");
            PushButtonData pdb_grid_rnam_falseC = new PushButtonData("FakeGRID_NumberCreat", "假轴号创建", projectAssemblyFullPath("FakeGRID_NumberCreat.dll"), "FakeGRID_NumberCreat.CMD");
            PushButtonData pdb_grid_rnam_falseR = new PushButtonData("replaeFakeGridNum", "假轴号替换", projectAssemblyFullPath("replaeFakeGridNum.dll"), "replaeFakeGridNum.CMD");
            PulldownButtonData pdbd_grid_rnam = new PulldownButtonData("GridNumber", "重命名");
            PulldownButton pdb_GridName = rp_grid.AddItem(pdbd_grid_rnam) as PulldownButton;
            pdb_GridName.AddPushButton(pdb_grid_rnam_true);
            pdb_GridName.AddPushButton(pdb_grid_rnam_falseC);
            pdb_GridName.AddPushButton(pdb_grid_rnam_falseR);
            pdb_GridName.LargeImage = new BitmapImage(new Uri(iconFullPath("GRID_RNAM_96.png")));

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
            PushButton pb_anno_openingElevation = rp_anno.AddItem(new PushButtonData("ANNO_OPEN_ELEV", "窗高标高", projectAssemblyFullPath("SportDimensionDemo.dll"), "SportDimensionDemo.WindowsAndDoors")) as PushButton;
            //need icon?
            //stack items 1
            PushButtonData pbd_fakeElev_Refresh = new PushButtonData("FakeElev_Refresh", "假标高计算", projectAssemblyFullPath("FakeElev_Refresh.dll"), "FakeElev_Refresh.CMD");
            PushButtonData pbd_autoFillUpElevations = new PushButtonData("AutoFillUpLevelHeightAnnotation", "填写多重标高", projectAssemblyFullPath("AutoFillUpLevelHeightAnnotation.dll"), "AutoFillUpLevelHeightAnnotation.CMD");
            rp_anno.AddStackedItems
                (pbd_fakeElev_Refresh,
                pbd_autoFillUpElevations);



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
            PushButton pb_wind_numb = rp_draw.AddItem(new PushButtonData("WIND_NUMB_new", "门窗编号", projectAssemblyFullPath("WIND_NUMB_new.dll"), "WIND_NUMB_new.CMD")) as PushButton;
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
            //stacked items 1
            PushButtonData pbd_view_duplicate = new PushButtonData("VIEW_Duplicate", "视图复制", projectAssemblyFullPath("VIEW_Duplicate.dll"), "VIEW_Duplicate.CMD");
            PushButtonData pbd_detailGroupVisibility = new PushButtonData("DetailGroupVisibility", "详图组可见", projectAssemblyFullPath("DetailGroupVisibility.dll"), "DetailGroupVisibility.CMD");
            PushButtonData pbd_checkShareAnnotation = new PushButtonData("CheckShareAnnotation", "检查问题尺寸", projectAssemblyFullPath("CheckShareAnnotation.dll"), "CheckShareAnnotation.CMD");
            rp_view.AddStackedItems(pbd_view_duplicate, pbd_detailGroupVisibility, pbd_checkShareAnnotation);

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

            //SHET_Size_SerialNo button
            PushButton pb_shet_size_serialNo = rp_shet.AddItem(new PushButtonData("SHET_Size_SerialNo", "图序图幅", projectAssemblyFullPath("SHET_Size_SerialNo.dll"), "SHET_Size_SerialNo.CMD")) as PushButton;
            //pb_shet_size_serialNo.LargeImage = new BitmapImage(new Uri(iconFullPath("SHET_Size_SerialNo_96.png")));

            //Excel panel
            RibbonPanel rp_excel = application.CreateRibbonPanel("goa tools 制图", "Excel");
            PushButtonData pbd_excel_fast_paste = new PushButtonData("Excel_Fast_Paste", "粘贴", projectAssemblyFullPath("Excel_Fast_Paste.dll"), "Excel_Fast_Paste.FastPaste");
            PushButtonData pbd_excel_fast_reload = new PushButtonData("Excel_Fast_Reload", "重载", projectAssemblyFullPath("Excel_Fast_Paste.dll"), "Excel_Fast_Paste.FastReload");
            rp_excel.AddStackedItems(
                pbd_excel_fast_paste,
                pbd_excel_fast_reload);
            #endregion drafting tab

            #region module tab

            #region 基本操作
            //BASIC OPERTATION   panel
            RibbonPanel rp_group_basic = application.CreateRibbonPanel("goa tools 模块", "基本操作");

            //GROUP_CREATE
            PushButton pb_group_create = rp_group_basic.AddItem(new PushButtonData("MODULE_CREATE", "新建(GN)", projectAssemblyFullPath("Module.dll"), "Module.CreateCMD")) as PushButton;
            pb_group_create.LargeImage = new BitmapImage(new Uri(iconFullPath("GROUP_CREATE_96.png")));
            //GROUP_SELECT
            PushButton pb_group_select = rp_group_basic.AddItem(new PushButtonData("MODULE_SELECT", "选择(GS)", projectAssemblyFullPath("Module.dll"), "Module.CMD")) as PushButton;
            pb_group_select.LargeImage = new BitmapImage(new Uri(iconFullPath("GROUP_SELECT_96.png")));
            //GROUP_ISOLATE
            //PushButton pb_group_isolate = rp_group_basic.AddItem(new PushButtonData("MODULE_ISOLATE", "隔离选中模块", projectAssemblyFullPath("Module.dll"), "Module.Isolate_CMD")) as PushButton;
            //pb_group_isolate.LargeImage = new BitmapImage(new Uri(iconFullPath("GROUP_ISOLATE_96.png")));
            //GROUP_DELETEMODULE
            PushButton pb_module_delete = rp_group_basic.AddItem(new PushButtonData("MODULE_DELETE", "删除(GA)", projectAssemblyFullPath("Module.dll"), "Module.DeleteModule_CMD")) as PushButton;
            pb_module_delete.LargeImage = new BitmapImage(new Uri(iconFullPath("GROUP_DELETE_MODULE_96.png")));

            //GROUP_UNGROUP
            PushButton pb_group_explode = rp_group_basic.AddItem(new PushButtonData("MODULE_EXPLODE", "炸开(GX)", projectAssemblyFullPath("Module.dll"), "Module.Explode_CMD")) as PushButton;
            pb_group_explode.LargeImage = new BitmapImage(new Uri(iconFullPath("GROUP_EXPLODE_96.png")));

            //rename module
            //PushButton pb_group_rename = rp_group_basic.AddItem(new PushButtonData("MODULE_RENAME", "由当前创建新类型", projectAssemblyFullPath("Module.dll"), "Module.Rename_CMD")) as PushButton;
            //pb_group_rename.LargeImage = new BitmapImage(new Uri(iconFullPath("GROUP_RENAME_96.jpg")));
            //GROUP_MODIFY

            var pb_group_manage = new PushButtonData("MODULE_MANAGE", "模块管理", projectAssemblyFullPath("Module.dll"), "Module.Manage_CMD");
            var pb_group_isolate = new PushButtonData("MODULE_ISOLATE", "隔离模块", projectAssemblyFullPath("Module.dll"), "Module.Isolate_CMD");
            rp_group_basic.AddStackedItems(
                pb_group_manage,
                pb_group_isolate);
            #endregion

            #region 模块编辑
            //GROUP EDIT  panel
            RibbonPanel rp_group_edit = application.CreateRibbonPanel("goa tools 模块", "模块编辑");

            //GROUP_MODIFY
            PushButton pb_group_modify = rp_group_edit.AddItem(new PushButtonData("MODULE_MODIFY", "编辑模块", projectAssemblyFullPath("Module.dll"), "Module.Edit_CMD")) as PushButton;
            pb_group_modify.LargeImage = new BitmapImage(new Uri(iconFullPath("GROUP_MODIFY_96.png")));

            var pb_module_rename = new PushButtonData("MODULE_RENAME", "由当前创建新类型", projectAssemblyFullPath("Module.dll"), "Module.Rename_CMD");
            var pbd_module_changeType = new PushButtonData("MODULE_CHANGE_TYPE", "改变模块类型", projectAssemblyFullPath("Module.dll"), "Module.ChangeType_CMD");
            rp_group_edit.AddStackedItems(
                pb_module_rename,
                pbd_module_changeType);

            var pbd_module_oneClickSync = new PushButtonData("MODULE_ONECLICK_SYNC", "一键同步", projectAssemblyFullPath("Module.dll"), "Module.OneClickSync_CMD");
            var pbd_module_forceSync = new PushButtonData("MODULE_FORCE_SYNC", "强制同步", projectAssemblyFullPath("Module.dll"), "Module.ForceSync_CMD");

            rp_group_edit.AddStackedItems(
                pbd_module_oneClickSync,
                pbd_module_forceSync);

            #endregion

            #region 模块操作
            //GROUP OPERATION  panel
            RibbonPanel rp_group_operate = application.CreateRibbonPanel("goa tools 模块", "模块操作");
            //GROUP_MIRROR
            PushButton pb_group_mirror_draw = rp_group_operate.AddItem(new PushButtonData("MODULE_MIRROR_DRAW", "画线镜像", projectAssemblyFullPath("Module.dll"), "Module.MirrorDraw_CMD")) as PushButton;
            pb_group_mirror_draw.LargeImage = new BitmapImage(new Uri(iconFullPath("GROUP_MIRROR_DRAWN_96.png")));
            //GROUP_MIRROR_PICK
            PushButton pb_group_mirror_pick = rp_group_operate.AddItem(new PushButtonData("MODULE_MIRROR_PICK", "拾取镜像", projectAssemblyFullPath("Module.dll"), "Module.MirrorPick_CMD")) as PushButton;
            pb_group_mirror_pick.LargeImage = new BitmapImage(new Uri(iconFullPath("GROUP_MIRROR_PICK_96.png")));

            //GROUP_COPY_TOLEVEL
            PushButton pb_group_copyToLevel = rp_group_operate.AddItem(new PushButtonData("MODULE_COPY_TOLEVEL", "复制到标高", projectAssemblyFullPath("Module.dll"), "Module.Lv_CMD")) as PushButton;
            pb_group_copyToLevel.LargeImage = new BitmapImage(new Uri(iconFullPath("GROUP_COPY_TOLEVEL_96.png")));

            PushButton pb_module_copyPaste = rp_group_operate.AddItem(new PushButtonData("MODULE_COPY_PASTE", "其他复制", projectAssemblyFullPath("Module.dll"), "Module.CopyPaste_CMD")) as PushButton;
            pb_module_copyPaste.LargeImage = new BitmapImage(new Uri(iconFullPath("GROUP_COPY_SAMELEVEL_96.png")));

            //不常用(复制、移动、旋转)
            //stacked items
            var pbd_module_copySameLevel = new PushButtonData("MODULE_COPY_SAMELEVEL", "同层复制", projectAssemblyFullPath("Module.dll"), "Module.Copy_CMD");
            var pbd_module_move = new PushButtonData("MODULE_MOVE", "移动模块", projectAssemblyFullPath("Module.dll"), "Module.Move_CMD");
            var pbd_module_rotate = new PushButtonData("MODULE_ROTATE", "旋转模块", projectAssemblyFullPath("Module.dll"), "Module.Rotate_CMD");
            rp_group_operate.AddStackedItems(
                pbd_module_copySameLevel,
                pbd_module_move,
                pbd_module_rotate);

            #endregion

            #region 跨模型操作
            //doc  panel
            RibbonPanel rp_module_doc = application.CreateRibbonPanel("goa tools 模块", "跨模型工具");


            PushButton pbd_module_syncDoc = rp_module_doc.AddItem(new PushButtonData("MODULE_DOC_SYNC", "跨模型同步", projectAssemblyFullPath("Module.dll"), "Module.DocSync_CMD")) as PushButton;
            pbd_module_syncDoc.LargeImage = new BitmapImage(new Uri(iconFullPath("GROUP_SYNC_DOC_96.png")));
            #endregion

            #region 附加操作
            //attach operation  panel
            RibbonPanel rp_group_attach = application.CreateRibbonPanel("goa tools 模块", "附加操作");

            var pb_module_convert = new PushButtonData("CONVERT_ALL", "全部组转模块", projectAssemblyFullPath("Module.dll"), "Module.Convert_CMD");
            var pb_module_pickConvert = new PushButtonData("CONVERT_PICK", "点选组转模块", projectAssemblyFullPath("Module.dll"), "Module.PickConvert_CMD");
            rp_group_attach.AddStackedItems(
                pb_module_convert,
                pb_module_pickConvert);

            //stacked items
            var pbd_module_deleteMark = new PushButtonData("MODULE_DELETE_MARK", "清除图元标记", projectAssemblyFullPath("Module.dll"), "Module.DeleteMarkOnElems");
            var pbd_module_offset = new PushButtonData("MODULE_OFFSET", "垂直移动", projectAssemblyFullPath("Module.dll"), "Module.Offset_CMD");

            rp_group_attach.AddStackedItems(
                pbd_module_deleteMark,
                pbd_module_offset);
            #endregion

            #region 选择工具
            //select  panel
            RibbonPanel rp_group_select = application.CreateRibbonPanel("goa tools 模块", "选择工具");
            //stacked items group 1
            var pbd_sel_module_all = new PushButtonData("SEL_ALL_MODULE", "选中同类型模块", projectAssemblyFullPath("Module.dll"), "Module.selectAllSameTypeModule");
            var pbd_sel_module_sameLevel = new PushButtonData("SEL_LEVEL_MODULE", "选中同层模块", projectAssemblyFullPath("Module.dll"), "Module.selectSameLvModule");
            var pbd_sel_module_pick = new PushButtonData("SEL_PICK_MODULE", "选中当前模块", projectAssemblyFullPath("Module.dll"), "Module.selectModuleByPick");
            var pbd_sel_module_shift = new PushButtonData("SEL_SHIFT_MODULE", "减选模块", projectAssemblyFullPath("Module.dll"), "Module.shiftSelect");

            rp_group_select.AddStackedItems(
                pbd_sel_module_sameLevel,
                pbd_sel_module_pick);
            rp_group_select.AddStackedItems(
                pbd_sel_module_all,
                pbd_sel_module_shift);
            #endregion

            //其他同步方法
            RibbonPanel rp_group_sync = application.CreateRibbonPanel("goa tools 模块", "修复工具");
            PushButtonData pbd_module_resetView = new PushButtonData("MODULE_RESETVIEW", "重置视图", projectAssemblyFullPath("Module.dll"), "Module.ResetStorageFiLocation");
            var pbd_module_color = new PushButtonData("DEL_COLOR_OVERRIDE", "颜色复原", projectAssemblyFullPath("Module.dll"), "Module.CleanOverrideColor");
            var pbd_module_window = new PushButtonData("MODULE_CLOSEWINDOW", "关闭窗口", projectAssemblyFullPath("Module.dll"), "Module.CloseWindow");

            rp_group_sync.AddStackedItems(
                pbd_module_resetView,
                pbd_module_color,
                pbd_module_window);

            RibbonPanel rp_group_developer = application.CreateRibbonPanel("goa tools 模块", "开发工具");
            PushButtonData pbd_module_developer = new PushButtonData("MODULE_DEVELOPER", "读取模块信息", projectAssemblyFullPath("Module.dll"), "Module.DeveloperTool");
            PushButtonData pbd_debug_showBoundingBox = new PushButtonData("debug_showBoundingBox", "显示范围框", projectAssemblyFullPath("goa.Common.dll"), "goa.Common.CMD_debug_showBoundingBox");
            rp_group_developer.AddStackedItems
                (pbd_module_developer,
                pbd_debug_showBoundingBox);
            #endregion module tab
            */
            return Result.Succeeded;
        }

        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }
    }
}

