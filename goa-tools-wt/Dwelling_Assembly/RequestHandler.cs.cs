using System;
using System.Collections.Generic;
using Form_ = System.Windows.Forms;
using System.Linq;

using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB.Events;

using goa.Common;

namespace Dwelling_Assembly
{
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
                    case RequestId.Creat_1T2H_aa_:
                        {
                            Creat_HXZH_1T2H_aa_1DY_A_(uiapp);
                            break;
                        }
                    case RequestId.Creat_1T2H_aaaa_:
                        {
                            Creat_HXZH_1T2H_aaaa_2DY_A_(uiapp);
                            break;
                        }
                    case RequestId.Creat_1T2H_aaaaaa_:
                        {
                            Creat_HXZH_1T2H_aaaaaa_3DY_A_(uiapp);
                            break;
                        }
                    case RequestId.Creat_1T2H_ab_:
                        {
                            Creat_HXZH_1T2H_ab_1DY_AB_(uiapp);
                            break;
                        }
                    case RequestId.Creat_1T2H_abba_:
                        {
                            Creat_HXZH_1T2H_abba_2DY_AB_(uiapp);
                            break;
                        }
                    case RequestId.Creat_1T2H_aaab_:
                        {
                            Creat_HXZH_1T2H_aaab_2DY_AB_(uiapp);
                            break;
                        }
                    case RequestId.Creat_1T2H_aabbaa_:
                        {
                            Creat_HXZH_1T2H_aabbaa_3DY_AB_(uiapp);
                            break;
                        }
                    case RequestId.Creat_1T2H_abbbba_:
                        {
                            Creat_HXZH_1T2H_abbbba_3DY_AB_(uiapp);
                            break;
                        }
                    case RequestId.Creat_1T2H_aaaaab_:
                        {
                            Creat_HXZH_1T2H_aaaaab_3DY_AB_(uiapp);
                            break;
                        }
                    case RequestId.Creat_1T2H_abbc_:
                        {
                            Creat_HXZH_1T2H_abbc_2DY_ABC_(uiapp);
                            break;
                        }
                    case RequestId.Creat_1T2H_aabbac_:
                        {
                            Creat_HXZH_1T2H_aabbac_3DY_ABC_(uiapp);
                            break;
                        }
                    case RequestId.Creat_1T2H_abbbbc_:
                        {
                            Creat_HXZH_1T2H_abbbbc_3DY_ABC_(uiapp);
                            break;
                        }
                    case RequestId.Creat_1T2H_abccba_:
                        {
                            Creat_HXZH_1T2H_abccba_3DY_ABC_(uiapp);
                            break;
                        }
                    case RequestId.Creat_1T2H_abccbd_:
                        {
                            Creat_HXZH_1T2H_abccbd_3DY_ABCD_(uiapp);
                            break;
                        }
                    case RequestId.Creat_1T2H_abaaba_:
                        {
                            Creat_HXZH_1T2H_abaaba_3DY_AB_(uiapp);
                            break;
                        }
                    case RequestId.Creat_1T2H_abaabc_:
                        {
                            Creat_HXZH_1T2H_abaabc_3DY_ABC_(uiapp);
                            break;
                        }
                    case RequestId.Creat_2T4H_abba_:
                        {
                            Creat_HXZU_2T4H_abba_1DY_AB_(uiapp);
                            break;
                        }
                    case RequestId.Creat_2T4H_abbaabba_:
                        {
                            Creat_HXZU_2T4H_abbaabba_2DY_AB_(uiapp);
                            break;
                        }
                    case RequestId.Creat_2T4H_abbc_:
                        {
                            Creat_HXZU_2T4H_abbc_1DY_ABC_(uiapp);
                            break;
                        }
                    case RequestId.Creat_2T4H_abbccbba_:
                        {
                            Creat_HXZU_2T4H_abbccbba_2DY_ABC_(uiapp);
                            break;
                        }
                    case RequestId.Creat_2T4H_abbaabbc_:
                        {
                            Creat_HXZU_2T4H_abbaabbc_2DY_ABC_(uiapp);
                            break;
                        }
                    case RequestId.Creat_2T4H_abbccbbd_:
                        {
                            Creat_HXZU_2T4H_abbccbbd_2DY_ABCD_(uiapp);
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
            }
            finally
            {
                window.WakeUp();
                window.Activate();
            }
        }//execute

        //外部事件方法建立
        /// <summary>
        /// 新建户型_1T2H_aa_1DY 函数架构 文档开启 在前 文档关闭 在后
        /// </summary>
        /// <param name="uiapp"></param>
        public void Creat_HXZH_1T2H_aa_1DY_A_(UIApplication uiapp)
        {
            #region all code
            #region ***初始化与revit模型进行交互的辅助变量 time_20200312
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Application app = uiapp.Application;
            Document doc = uidoc.Document;
            View actView = doc.ActiveView;
            View FL01_view = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Views).OfClass(typeof(ViewPlan)).FirstOrDefault(x => x.Name == "FL01") as View;
            Plane ori_plane = Plane.CreateByNormalAndOrigin(new XYZ(1, 0, 0), new XYZ(0, 0, 0));//该处为原点镜像轴
            Plane mid_plane = null;//该处mid定义为整体户型组合模式的中心对称轴位置
            LogicalOrFilter orFilter = CreatorFilter(uiapp);
            #endregion

            #region ***声明 all 变量 time_20200312
            Document _TXA_ = null;
            Document _TXB_ = null;
            Document _TXC_ = null;
            Document _TXD_ = null;
            Document _TXA_Core_ = null;
            Document _TXB_Core_ = null;
            Document _TXC_Core_ = null;
            Document _TXD_Core_ = null;
            ICollection<ElementId> group_TXA_ids = new List<ElementId>();//各个基础模型3D元素group的id_list
            ICollection<ElementId> group_TXB_ids = new List<ElementId>();
            ICollection<ElementId> group_TXC_ids = new List<ElementId>();
            ICollection<ElementId> group_TXD_ids = new List<ElementId>();
            ICollection<ElementId> group_TXA_Core_ids = new List<ElementId>();
            ICollection<ElementId> group_TXB_Core_ids = new List<ElementId>();
            ICollection<ElementId> group_TXC_Core_ids = new List<ElementId>();
            ICollection<ElementId> group_TXD_Core_ids = new List<ElementId>();
            ICollection<ElementId> group_TXA_textnote_ids = new List<ElementId>();//各个基础模型2D文字注释group的id_list
            ICollection<ElementId> group_TXB_textnote_ids = new List<ElementId>();
            ICollection<ElementId> group_TXC_textnote_ids = new List<ElementId>();
            ICollection<ElementId> group_TXD_textnote_ids = new List<ElementId>();
            ICollection<ElementId> group_TXA_Core_textnote_ids = new List<ElementId>();
            ICollection<ElementId> group_TXB_Core_textnote_ids = new List<ElementId>();
            ICollection<ElementId> group_TXC_Core_textnote_ids = new List<ElementId>();
            ICollection<ElementId> group_TXD_Core_textnote_ids = new List<ElementId>();
            ICollection<ElementId> _TXA_dimension_ids = new List<ElementId>();//由于2D标注元素，不可以成group，所以此处为各个标注元素的list
            ICollection<ElementId> _TXB_dimension_ids = new List<ElementId>();
            ICollection<ElementId> _TXC_dimension_ids = new List<ElementId>();
            ICollection<ElementId> _TXD_dimension_ids = new List<ElementId>();
            ICollection<ElementId> _TXA_Core_dimension_ids = new List<ElementId>();//注意，目前核心筒基础模型，不涉及标注元素,此外初始化，是为了满足函数参数的空值调用
            ICollection<ElementId> _TXB_Core_dimension_ids = new List<ElementId>();
            ICollection<ElementId> _TXC_Core_dimension_ids = new List<ElementId>();
            ICollection<ElementId> _TXD_Core_dimension_ids = new List<ElementId>();
            #endregion

            #region ***判断是否需要打开源文档 time_20200312
            //OpenTarDocument(uiapp, _TXA_, _TXB_, _TXC_, _TXD_, _TXA_Core_, _TXB_Core_, _TXC_Core_, _TXD_Core_);
            if (CMD._TXA_rvt != "null_path")
            {
                _TXA_ = app.OpenDocumentFile(CMD._TXA_rvt);
            }
            if (CMD._TXB_rvt != "null_path")
            {
                _TXB_ = app.OpenDocumentFile(CMD._TXB_rvt);
            }
            if (CMD._TXC_rvt != "null_path")
            {
                _TXC_ = app.OpenDocumentFile(CMD._TXC_rvt);
            }
            if (CMD._TXD_rvt != "null_path")
            {
                _TXD_ = app.OpenDocumentFile(CMD._TXD_rvt);
            }
            if (CMD._TXA_HXT_rvt != "null_path")
            {
                _TXA_Core_ = app.OpenDocumentFile(CMD._TXA_HXT_rvt);
            }
            if (CMD._TXB_HXT_rvt != "null_path")
            {
                _TXB_Core_ = app.OpenDocumentFile(CMD._TXB_HXT_rvt);
            }
            if (CMD._TXC_HXT_rvt != "null_path")
            {
                _TXC_Core_ = app.OpenDocumentFile(CMD._TXC_HXT_rvt);
            }
            if (CMD._TXD_HXT_rvt != "null_path")
            {
                _TXD_Core_ = app.OpenDocumentFile(CMD._TXD_HXT_rvt);
            }
            #endregion

            #region 根据户型组合模式 在各个源文件 定位元素 在这里不对核心筒定位做处理 原因：revit后台多次打开同一个文件，属于无效操作 time_20200312

            if (CMD._TXA_rvt != "null_path")
            {
                //判断是否需要对基础模型进行定位操作
            }
            if (CMD._TXB_rvt != "null_path")
            {
                //判断是否需要对基础模型进行定位操作
            }
            if (CMD._TXC_rvt != "null_path")
            {
                //判断是否需要对基础模型进行定位操作
            }
            if (CMD._TXD_rvt != "null_path")
            {
                //判断是否需要对基础模型进行定位操作
            }
            #endregion

            using (TransactionGroup transGroupCreateDwellingses = new TransactionGroup(doc, "创建户型零件组_group"))//开展事务组
            {
                if (transGroupCreateDwellingses.Start() == TransactionStatus.Started)//开启事务组
                {
                    #region ***将定位好的基础模型数据，复制到当前工作视图 并creat group 标注元素只有list，不可以creat group time_20200312
                    if (CMD._TXA_rvt != "null_path")
                    {
                        group_TXA_ids = Creat_Unit_GroupId_copyFrom_Ori(_TXA_, doc, orFilter, actView, out group_TXA_textnote_ids, out _TXA_dimension_ids, "_TXA_", "_TXA_textnote_");
                    }
                    if (CMD._TXB_rvt != "null_path")
                    {
                        group_TXB_ids = Creat_Unit_GroupId_copyFrom_Ori(_TXB_, doc, orFilter, actView, out group_TXB_textnote_ids, out _TXB_dimension_ids, "_TXB_", "_TXB_textnote_");
                    }
                    if (CMD._TXC_rvt != "null_path")
                    {
                        group_TXC_ids = Creat_Unit_GroupId_copyFrom_Ori(_TXC_, doc, orFilter, actView, out group_TXC_textnote_ids, out _TXC_dimension_ids, "_TXC_", "_TXC_textnote_");
                    }
                    if (CMD._TXD_rvt != "null_path")
                    {
                        group_TXD_ids = Creat_Unit_GroupId_copyFrom_Ori(_TXD_, doc, orFilter, actView, out group_TXD_textnote_ids, out _TXD_dimension_ids, "_TXD_", "_TXD_textnote_");
                    }
                    if (CMD._TXA_HXT_rvt != "null_path")
                    {
                        group_TXA_Core_ids = Creat_Unit_GroupId_copyFrom_Ori(_TXA_Core_, doc, orFilter, actView, out group_TXA_Core_textnote_ids, out _TXA_Core_dimension_ids, "_TXA_Core_", "_TXA_Core_textnote_");
                    }
                    if (CMD._TXB_HXT_rvt != "null_path")
                    {
                        group_TXB_Core_ids = Creat_Unit_GroupId_copyFrom_Ori(_TXB_Core_, doc, orFilter, actView, out group_TXB_Core_textnote_ids, out _TXB_Core_dimension_ids, "_TXB_Core_", "_TXB_Core_textnote_");
                    }
                    if (CMD._TXC_HXT_rvt != "null_path")
                    {
                        group_TXC_Core_ids = Creat_Unit_GroupId_copyFrom_Ori(_TXC_Core_, doc, orFilter, actView, out group_TXC_Core_textnote_ids, out _TXC_Core_dimension_ids, "_TXC_Core_", "_TXC_Core_textnote_");
                    }
                    if (CMD._TXD_HXT_rvt != "null_path")
                    {
                        group_TXD_Core_ids = Creat_Unit_GroupId_copyFrom_Ori(_TXD_Core_, doc, orFilter, actView, out group_TXD_Core_textnote_ids, out _TXD_Core_dimension_ids, "_TXD_Core_", "_TXD_Core_textnote_");
                    }
                    #endregion

                    #region 在当前视图，对户型进行排列组合 注意模型复制到当前文档时，所处的位置 time_20200312
                    //abccbd 根据字母组合进行判断
                    if (CMD._TXA_rvt != "null_path")
                    {
                        //判断是否需要对复制进来的模型数据进行二次操作
                        ICollection<ElementId> mi_group_TXA_ids = _MirrorElements_ChangeHandler(doc, group_TXA_ids, ori_plane, true);
                        ICollection<ElementId> mi_group_TXA_textnote_ids = _MirrorElements_ChangeHandler(doc, group_TXA_textnote_ids, ori_plane, true);
                        ICollection<ElementId> mi_TXA_dimension_ids = _MirrorElements_ChangeHandler(doc, _TXA_dimension_ids, ori_plane, true);
                    }
                    if (CMD._TXB_rvt != "null_path")
                    {
                        //判断是否需要对复制进来的模型数据进行二次操作
                    }
                    if (CMD._TXC_rvt != "null_path")
                    {
                        //判断是否需要对复制进来的模型数据进行二次操作
                    }
                    if (CMD._TXD_rvt != "null_path")
                    {
                        //判断是否需要对复制进来的模型数据进行二次操作
                    }
                    if (CMD._TXA_HXT_rvt != "null_path")
                    {
                        //判断是否需要对复制进来的模型数据进行二次操作
                    }
                    if (CMD._TXB_HXT_rvt != "null_path")
                    {
                        //判断是否需要对复制进来的模型数据进行二次操作
                    }
                    if (CMD._TXC_HXT_rvt != "null_path")
                    {
                        //判断是否需要对复制进来的模型数据进行二次操作
                    }
                    if (CMD._TXD_HXT_rvt != "null_path")
                    {
                        //判断是否需要对复制进来的模型数据进行二次操作
                    }
                    #endregion
                    transGroupCreateDwellingses.Assimilate();
                }
                else
                {
                    transGroupCreateDwellingses.RollBack();
                }
            }
            #region ***关闭所有源文档，但是不保存  time_20200312
            ResetDocument(_TXA_, _TXB_, _TXC_, _TXD_, _TXA_Core_, _TXB_Core_, _TXC_Core_, _TXD_Core_);
            #endregion
            #endregion
        }
        /// <summary>
        /// 新建户型_1T2H_aaaa_2DY
        /// </summary>
        /// <param name="uiapp"></param>
        public void Creat_HXZH_1T2H_aaaa_2DY_A_(UIApplication uiapp)
        {
            #region all code
            #region ***初始化与revit模型进行交互的辅助变量 time_20200312
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Application app = uiapp.Application;
            Document doc = uidoc.Document;
            View actView = doc.ActiveView;
            View FL01_view = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Views).OfClass(typeof(ViewPlan)).FirstOrDefault(x => x.Name == "FL01") as View;
            Plane ori_plane = Plane.CreateByNormalAndOrigin(new XYZ(1, 0, 0), new XYZ(0, 0, 0));//该处为原点镜像轴
            Plane mid_plane = null;//该处mid定义为整体户型组合模式的中心对称轴位置
            LogicalOrFilter orFilter = CreatorFilter(uiapp);
            #endregion

            #region ***声明 all 变量 time_20200312
            Document _TXA_ = null;
            Document _TXB_ = null;
            Document _TXC_ = null;
            Document _TXD_ = null;
            Document _TXA_Core_ = null;
            Document _TXB_Core_ = null;
            Document _TXC_Core_ = null;
            Document _TXD_Core_ = null;
            ICollection<ElementId> group_TXA_ids = new List<ElementId>();//各个基础模型3D元素group的id_list
            ICollection<ElementId> group_TXB_ids = new List<ElementId>();
            ICollection<ElementId> group_TXC_ids = new List<ElementId>();
            ICollection<ElementId> group_TXD_ids = new List<ElementId>();
            ICollection<ElementId> group_TXA_Core_ids = new List<ElementId>();
            ICollection<ElementId> group_TXB_Core_ids = new List<ElementId>();
            ICollection<ElementId> group_TXC_Core_ids = new List<ElementId>();
            ICollection<ElementId> group_TXD_Core_ids = new List<ElementId>();
            ICollection<ElementId> group_TXA_textnote_ids = new List<ElementId>();//各个基础模型2D文字注释group的id_list
            ICollection<ElementId> group_TXB_textnote_ids = new List<ElementId>();
            ICollection<ElementId> group_TXC_textnote_ids = new List<ElementId>();
            ICollection<ElementId> group_TXD_textnote_ids = new List<ElementId>();
            ICollection<ElementId> group_TXA_Core_textnote_ids = new List<ElementId>();
            ICollection<ElementId> group_TXB_Core_textnote_ids = new List<ElementId>();
            ICollection<ElementId> group_TXC_Core_textnote_ids = new List<ElementId>();
            ICollection<ElementId> group_TXD_Core_textnote_ids = new List<ElementId>();
            ICollection<ElementId> _TXA_dimension_ids = new List<ElementId>();//由于2D标注元素，不可以成group，所以此处为各个标注元素的list
            ICollection<ElementId> _TXB_dimension_ids = new List<ElementId>();
            ICollection<ElementId> _TXC_dimension_ids = new List<ElementId>();
            ICollection<ElementId> _TXD_dimension_ids = new List<ElementId>();
            ICollection<ElementId> _TXA_Core_dimension_ids = new List<ElementId>();//注意，目前核心筒基础模型，不涉及标注元素,此外初始化，是为了满足函数参数的空值调用
            ICollection<ElementId> _TXB_Core_dimension_ids = new List<ElementId>();
            ICollection<ElementId> _TXC_Core_dimension_ids = new List<ElementId>();
            ICollection<ElementId> _TXD_Core_dimension_ids = new List<ElementId>();
            #endregion

            #region ***判断是否需要打开源文档 time_20200312
            //OpenTarDocument(uiapp, _TXA_, _TXB_, _TXC_, _TXD_, _TXA_Core_, _TXB_Core_, _TXC_Core_, _TXD_Core_);
            if (CMD._TXA_rvt != "null_path")
            {
                _TXA_ = app.OpenDocumentFile(CMD._TXA_rvt);
            }
            if (CMD._TXB_rvt != "null_path")
            {
                _TXB_ = app.OpenDocumentFile(CMD._TXB_rvt);
            }
            if (CMD._TXC_rvt != "null_path")
            {
                _TXC_ = app.OpenDocumentFile(CMD._TXC_rvt);
            }
            if (CMD._TXD_rvt != "null_path")
            {
                _TXD_ = app.OpenDocumentFile(CMD._TXD_rvt);
            }
            if (CMD._TXA_HXT_rvt != "null_path")
            {
                _TXA_Core_ = app.OpenDocumentFile(CMD._TXA_HXT_rvt);
            }
            if (CMD._TXB_HXT_rvt != "null_path")
            {
                _TXB_Core_ = app.OpenDocumentFile(CMD._TXB_HXT_rvt);
            }
            if (CMD._TXC_HXT_rvt != "null_path")
            {
                _TXC_Core_ = app.OpenDocumentFile(CMD._TXC_HXT_rvt);
            }
            if (CMD._TXD_HXT_rvt != "null_path")
            {
                _TXD_Core_ = app.OpenDocumentFile(CMD._TXD_HXT_rvt);
            }
            #endregion

            #region 根据户型组合模式 在各个源文件 定位元素 在这里不对核心筒定位做处理 原因：revit后台多次打开同一个文件，属于无效操作 time_20200312

            if (CMD._TXA_rvt != "null_path")
            {
                //判断是否需要对基础模型进行定位操作
            }
            if (CMD._TXB_rvt != "null_path")
            {
                //判断是否需要对基础模型进行定位操作
            }
            if (CMD._TXC_rvt != "null_path")
            {
                //判断是否需要对基础模型进行定位操作
            }
            if (CMD._TXD_rvt != "null_path")
            {
                //判断是否需要对基础模型进行定位操作
            }
            #endregion

            using (TransactionGroup transGroupCreateDwellingses = new TransactionGroup(doc, "创建户型零件组_group"))//开展事务组
            {
                if (transGroupCreateDwellingses.Start() == TransactionStatus.Started)//开启事务组
                {
                    #region ***将定位好的基础模型数据，复制到当前工作视图 并creat group 标注元素只有list，不可以creat group time_20200312
                    if (CMD._TXA_rvt != "null_path")
                    {
                        group_TXA_ids = Creat_Unit_GroupId_copyFrom_Ori(_TXA_, doc, orFilter, actView, out group_TXA_textnote_ids, out _TXA_dimension_ids, "_TXA_", "_TXA_textnote_");
                    }
                    if (CMD._TXB_rvt != "null_path")
                    {
                        group_TXB_ids = Creat_Unit_GroupId_copyFrom_Ori(_TXB_, doc, orFilter, actView, out group_TXB_textnote_ids, out _TXB_dimension_ids, "_TXB_", "_TXB_textnote_");
                    }
                    if (CMD._TXC_rvt != "null_path")
                    {
                        group_TXC_ids = Creat_Unit_GroupId_copyFrom_Ori(_TXC_, doc, orFilter, actView, out group_TXC_textnote_ids, out _TXC_dimension_ids, "_TXC_", "_TXC_textnote_");
                    }
                    if (CMD._TXD_rvt != "null_path")
                    {
                        group_TXD_ids = Creat_Unit_GroupId_copyFrom_Ori(_TXD_, doc, orFilter, actView, out group_TXD_textnote_ids, out _TXD_dimension_ids, "_TXD_", "_TXD_textnote_");
                    }
                    if (CMD._TXA_HXT_rvt != "null_path")
                    {
                        group_TXA_Core_ids = Creat_Unit_GroupId_copyFrom_Ori(_TXA_Core_, doc, orFilter, actView, out group_TXA_Core_textnote_ids, out _TXA_Core_dimension_ids, "_TXA_Core_", "_TXA_Core_textnote_");
                    }
                    if (CMD._TXB_HXT_rvt != "null_path")
                    {
                        group_TXB_Core_ids = Creat_Unit_GroupId_copyFrom_Ori(_TXB_Core_, doc, orFilter, actView, out group_TXB_Core_textnote_ids, out _TXB_Core_dimension_ids, "_TXB_Core_", "_TXB_Core_textnote_");
                    }
                    if (CMD._TXC_HXT_rvt != "null_path")
                    {
                        group_TXC_Core_ids = Creat_Unit_GroupId_copyFrom_Ori(_TXC_Core_, doc, orFilter, actView, out group_TXC_Core_textnote_ids, out _TXC_Core_dimension_ids, "_TXC_Core_", "_TXC_Core_textnote_");
                    }
                    if (CMD._TXD_HXT_rvt != "null_path")
                    {
                        group_TXD_Core_ids = Creat_Unit_GroupId_copyFrom_Ori(_TXD_Core_, doc, orFilter, actView, out group_TXD_Core_textnote_ids, out _TXD_Core_dimension_ids, "_TXD_Core_", "_TXD_Core_textnote_");
                    }
                    #endregion

                    #region 在当前视图，对户型进行排列组合 注意模型复制到当前文档时，所处的位置 time_20200312
                    //abccbd 根据字母组合进行判断
                    Plane _mid_plane = ori_plane;//该处为原点镜像轴

                    if (CMD._TXA_rvt != "null_path")
                    {
                        //判断是否需要对复制进来的模型数据进行二次操作
                        ICollection<ElementId> mi_group_TXA_ids = _MirrorElements_ChangeHandler(doc, group_TXA_ids, ori_plane, true);
                        ICollection<ElementId> mi_group_TXA_textnote_ids = _MirrorElements_ChangeHandler(doc, group_TXA_textnote_ids, ori_plane, true);
                        ICollection<ElementId> mi_TXA_dimension_ids = _MirrorElements_ChangeHandler(doc, _TXA_dimension_ids, ori_plane, true);

                        double _aAaa_max_X;
                        double _aAaa_min_X = GetLeftRgiht_X_ingrid_fromGroup(doc, mi_group_TXA_ids, out _aAaa_max_X);
                        mid_plane = Plane.CreateByNormalAndOrigin(new XYZ(_aAaa_max_X + 1, 0, 0), new XYZ(_aAaa_max_X, 0, 0));//该处为原点镜像轴
                        ICollection<ElementId> _mi_group_TXA_ids = _MirrorElements_ChangeHandler(doc, mi_group_TXA_ids, mid_plane, true);
                        ICollection<ElementId> _mi_group_TXA_textnote_ids = _MirrorElements_ChangeHandler(doc, mi_group_TXA_textnote_ids, mid_plane, true);
                        ICollection<ElementId> _mi_TXA_dimension_ids = _MirrorElements_ChangeHandler(doc, mi_TXA_dimension_ids, mid_plane, true);

                        _aAaa_min_X = GetLeftRgiht_X_ingrid_fromGroup(doc, _mi_group_TXA_ids, out _aAaa_max_X);
                        _mid_plane = Plane.CreateByNormalAndOrigin(new XYZ(_aAaa_max_X + 1, 0, 0), new XYZ(_aAaa_max_X, 0, 0));//该处为原点镜像轴
                        ICollection<ElementId> __mi_group_TXA_ids = _MirrorElements_ChangeHandler(doc, _mi_group_TXA_ids, _mid_plane, true);
                        ICollection<ElementId> __mi_group_TXA_textnote_ids = _MirrorElements_ChangeHandler(doc, _mi_group_TXA_textnote_ids, _mid_plane, true);
                        ICollection<ElementId> __mi_TXA_dimension_ids = _MirrorElements_ChangeHandler(doc, _mi_TXA_dimension_ids, _mid_plane, true);
                    }
                    if (CMD._TXB_rvt != "null_path")
                    {
                        //判断是否需要对复制进来的模型数据进行二次操作
                    }
                    if (CMD._TXC_rvt != "null_path")
                    {
                        //判断是否需要对复制进来的模型数据进行二次操作
                    }
                    if (CMD._TXD_rvt != "null_path")
                    {
                        //判断是否需要对复制进来的模型数据进行二次操作
                    }
                    if (CMD._TXA_HXT_rvt != "null_path")
                    {
                        //判断是否需要对复制进来的模型数据进行二次操作
                        ICollection<ElementId> mi_group_TXA_Core_ids = _MirrorElements_ChangeHandler(doc, group_TXA_Core_ids, mid_plane, true);
                        ICollection<ElementId> mi_group_TXA_Core_textnote_ids = _MirrorElements_ChangeHandler(doc, group_TXA_Core_textnote_ids, mid_plane, true);
                    }
                    if (CMD._TXB_HXT_rvt != "null_path")
                    {
                        //判断是否需要对复制进来的模型数据进行二次操作
                    }
                    if (CMD._TXC_HXT_rvt != "null_path")
                    {
                        //判断是否需要对复制进来的模型数据进行二次操作
                    }
                    if (CMD._TXD_HXT_rvt != "null_path")
                    {
                        //判断是否需要对复制进来的模型数据进行二次操作
                    }
                    #endregion
                    transGroupCreateDwellingses.Assimilate();
                }
                else
                {
                    transGroupCreateDwellingses.RollBack();
                }
            }
            #region ***关闭所有源文档，但是不保存  time_20200312
            ResetDocument(_TXA_, _TXB_, _TXC_, _TXD_, _TXA_Core_, _TXB_Core_, _TXC_Core_, _TXD_Core_);
            #endregion
            #endregion
        }
        /// <summary>
        /// 新建户型_1T2H_aaaaaa_3DY
        /// </summary>
        /// <param name="uiapp"></param>
        public void Creat_HXZH_1T2H_aaaaaa_3DY_A_(UIApplication uiapp)
        {
            #region all code
            #region ***初始化与revit模型进行交互的辅助变量 time_20200312
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Application app = uiapp.Application;
            Document doc = uidoc.Document;
            View actView = doc.ActiveView;
            View FL01_view = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Views).OfClass(typeof(ViewPlan)).FirstOrDefault(x => x.Name == "FL01") as View;
            Plane ori_plane = Plane.CreateByNormalAndOrigin(new XYZ(1, 0, 0), new XYZ(0, 0, 0));//该处为原点镜像轴
            Plane mid_plane = null;//该处mid定义为整体户型组合模式的中心对称轴位置
            LogicalOrFilter orFilter = CreatorFilter(uiapp);
            #endregion

            #region ***声明 all 变量 time_20200312
            Document _TXA_ = null;
            Document _TXB_ = null;
            Document _TXC_ = null;
            Document _TXD_ = null;
            Document _TXA_Core_ = null;
            Document _TXB_Core_ = null;
            Document _TXC_Core_ = null;
            Document _TXD_Core_ = null;
            ICollection<ElementId> group_TXA_ids = new List<ElementId>();//各个基础模型3D元素group的id_list
            ICollection<ElementId> group_TXB_ids = new List<ElementId>();
            ICollection<ElementId> group_TXC_ids = new List<ElementId>();
            ICollection<ElementId> group_TXD_ids = new List<ElementId>();
            ICollection<ElementId> group_TXA_Core_ids = new List<ElementId>();
            ICollection<ElementId> group_TXB_Core_ids = new List<ElementId>();
            ICollection<ElementId> group_TXC_Core_ids = new List<ElementId>();
            ICollection<ElementId> group_TXD_Core_ids = new List<ElementId>();
            ICollection<ElementId> group_TXA_textnote_ids = new List<ElementId>();//各个基础模型2D文字注释group的id_list
            ICollection<ElementId> group_TXB_textnote_ids = new List<ElementId>();
            ICollection<ElementId> group_TXC_textnote_ids = new List<ElementId>();
            ICollection<ElementId> group_TXD_textnote_ids = new List<ElementId>();
            ICollection<ElementId> group_TXA_Core_textnote_ids = new List<ElementId>();
            ICollection<ElementId> group_TXB_Core_textnote_ids = new List<ElementId>();
            ICollection<ElementId> group_TXC_Core_textnote_ids = new List<ElementId>();
            ICollection<ElementId> group_TXD_Core_textnote_ids = new List<ElementId>();
            ICollection<ElementId> _TXA_dimension_ids = new List<ElementId>();//由于2D标注元素，不可以成group，所以此处为各个标注元素的list
            ICollection<ElementId> _TXB_dimension_ids = new List<ElementId>();
            ICollection<ElementId> _TXC_dimension_ids = new List<ElementId>();
            ICollection<ElementId> _TXD_dimension_ids = new List<ElementId>();
            ICollection<ElementId> _TXA_Core_dimension_ids = new List<ElementId>();//注意，目前核心筒基础模型，不涉及标注元素,此外初始化，是为了满足函数参数的空值调用
            ICollection<ElementId> _TXB_Core_dimension_ids = new List<ElementId>();
            ICollection<ElementId> _TXC_Core_dimension_ids = new List<ElementId>();
            ICollection<ElementId> _TXD_Core_dimension_ids = new List<ElementId>();
            #endregion

            #region ***判断是否需要打开源文档 time_20200312
            //OpenTarDocument(uiapp, _TXA_, _TXB_, _TXC_, _TXD_, _TXA_Core_, _TXB_Core_, _TXC_Core_, _TXD_Core_);
            if (CMD._TXA_rvt != "null_path")
            {
                _TXA_ = app.OpenDocumentFile(CMD._TXA_rvt);
            }
            if (CMD._TXB_rvt != "null_path")
            {
                _TXB_ = app.OpenDocumentFile(CMD._TXB_rvt);
            }
            if (CMD._TXC_rvt != "null_path")
            {
                _TXC_ = app.OpenDocumentFile(CMD._TXC_rvt);
            }
            if (CMD._TXD_rvt != "null_path")
            {
                _TXD_ = app.OpenDocumentFile(CMD._TXD_rvt);
            }
            if (CMD._TXA_HXT_rvt != "null_path")
            {
                _TXA_Core_ = app.OpenDocumentFile(CMD._TXA_HXT_rvt);
            }
            if (CMD._TXB_HXT_rvt != "null_path")
            {
                _TXB_Core_ = app.OpenDocumentFile(CMD._TXB_HXT_rvt);
            }
            if (CMD._TXC_HXT_rvt != "null_path")
            {
                _TXC_Core_ = app.OpenDocumentFile(CMD._TXC_HXT_rvt);
            }
            if (CMD._TXD_HXT_rvt != "null_path")
            {
                _TXD_Core_ = app.OpenDocumentFile(CMD._TXD_HXT_rvt);
            }
            #endregion

            #region 根据户型组合模式 在各个源文件 定位元素 在这里不对核心筒定位做处理 原因：revit后台多次打开同一个文件，属于无效操作 time_20200312

            if (CMD._TXA_rvt != "null_path")
            {
                //判断是否需要对基础模型进行定位操作
            }
            if (CMD._TXB_rvt != "null_path")
            {
                //判断是否需要对基础模型进行定位操作
            }
            if (CMD._TXC_rvt != "null_path")
            {
                //判断是否需要对基础模型进行定位操作
            }
            if (CMD._TXD_rvt != "null_path")
            {
                //判断是否需要对基础模型进行定位操作
            }
            #endregion

            using (TransactionGroup transGroupCreateDwellingses = new TransactionGroup(doc, "创建户型零件组_group"))//开展事务组
            {
                if (transGroupCreateDwellingses.Start() == TransactionStatus.Started)//开启事务组
                {
                    #region ***将定位好的基础模型数据，复制到当前工作视图 并creat group 标注元素只有list，不可以creat group time_20200312
                    if (CMD._TXA_rvt != "null_path")
                    {
                        group_TXA_ids = Creat_Unit_GroupId_copyFrom_Ori(_TXA_, doc, orFilter, actView, out group_TXA_textnote_ids, out _TXA_dimension_ids, "_TXA_", "_TXA_textnote_");
                    }
                    if (CMD._TXB_rvt != "null_path")
                    {
                        group_TXB_ids = Creat_Unit_GroupId_copyFrom_Ori(_TXB_, doc, orFilter, actView, out group_TXB_textnote_ids, out _TXB_dimension_ids, "_TXB_", "_TXB_textnote_");
                    }
                    if (CMD._TXC_rvt != "null_path")
                    {
                        group_TXC_ids = Creat_Unit_GroupId_copyFrom_Ori(_TXC_, doc, orFilter, actView, out group_TXC_textnote_ids, out _TXC_dimension_ids, "_TXC_", "_TXC_textnote_");
                    }
                    if (CMD._TXD_rvt != "null_path")
                    {
                        group_TXD_ids = Creat_Unit_GroupId_copyFrom_Ori(_TXD_, doc, orFilter, actView, out group_TXD_textnote_ids, out _TXD_dimension_ids, "_TXD_", "_TXD_textnote_");
                    }
                    if (CMD._TXA_HXT_rvt != "null_path")
                    {
                        group_TXA_Core_ids = Creat_Unit_GroupId_copyFrom_Ori(_TXA_Core_, doc, orFilter, actView, out group_TXA_Core_textnote_ids, out _TXA_Core_dimension_ids, "_TXA_Core_", "_TXA_Core_textnote_");
                    }
                    if (CMD._TXB_HXT_rvt != "null_path")
                    {
                        group_TXB_Core_ids = Creat_Unit_GroupId_copyFrom_Ori(_TXB_Core_, doc, orFilter, actView, out group_TXB_Core_textnote_ids, out _TXB_Core_dimension_ids, "_TXB_Core_", "_TXB_Core_textnote_");
                    }
                    if (CMD._TXC_HXT_rvt != "null_path")
                    {
                        group_TXC_Core_ids = Creat_Unit_GroupId_copyFrom_Ori(_TXC_Core_, doc, orFilter, actView, out group_TXC_Core_textnote_ids, out _TXC_Core_dimension_ids, "_TXC_Core_", "_TXC_Core_textnote_");
                    }
                    if (CMD._TXD_HXT_rvt != "null_path")
                    {
                        group_TXD_Core_ids = Creat_Unit_GroupId_copyFrom_Ori(_TXD_Core_, doc, orFilter, actView, out group_TXD_Core_textnote_ids, out _TXD_Core_dimension_ids, "_TXD_Core_", "_TXD_Core_textnote_");
                    }
                    #endregion

                    #region 在当前视图，对户型进行排列组合 注意模型复制到当前文档时，所处的位置 time_20200312
                    //abccbd 根据字母组合进行判断
                    Plane _mid_plane = ori_plane;
                    Plane __mid_plane = ori_plane;
                    Plane ___mid_plane = ori_plane;

                    if (CMD._TXA_rvt != "null_path")
                    {
                        //判断是否需要对复制进来的模型数据进行二次操作
                        ICollection<ElementId> mi_group_TXA_ids = _MirrorElements_ChangeHandler(doc, group_TXA_ids, ori_plane, true);
                        ICollection<ElementId> mi_group_TXA_textnote_ids = _MirrorElements_ChangeHandler(doc, group_TXA_textnote_ids, ori_plane, true);
                        ICollection<ElementId> mi_TXA_dimension_ids = _MirrorElements_ChangeHandler(doc, _TXA_dimension_ids, ori_plane, true);

                        double _aAaa_max_X;
                        double _aAaa_min_X = GetLeftRgiht_X_ingrid_fromGroup(doc, mi_group_TXA_ids, out _aAaa_max_X);
                        mid_plane = Plane.CreateByNormalAndOrigin(new XYZ(_aAaa_max_X + 1, 0, 0), new XYZ(_aAaa_max_X, 0, 0));//该处为原点镜像轴
                        ICollection<ElementId> _mi_group_TXA_ids = _MirrorElements_ChangeHandler(doc, mi_group_TXA_ids, mid_plane, true);
                        ICollection<ElementId> _mi_group_TXA_textnote_ids = _MirrorElements_ChangeHandler(doc, mi_group_TXA_textnote_ids, mid_plane, true);
                        ICollection<ElementId> _mi_TXA_dimension_ids = _MirrorElements_ChangeHandler(doc, mi_TXA_dimension_ids, mid_plane, true);

                        _aAaa_min_X = GetLeftRgiht_X_ingrid_fromGroup(doc, _mi_group_TXA_ids, out _aAaa_max_X);
                        _mid_plane = Plane.CreateByNormalAndOrigin(new XYZ(_aAaa_max_X + 1, 0, 0), new XYZ(_aAaa_max_X, 0, 0));//该处为原点镜像轴
                        ICollection<ElementId> __mi_group_TXA_ids = _MirrorElements_ChangeHandler(doc, _mi_group_TXA_ids, _mid_plane, true);
                        ICollection<ElementId> __mi_group_TXA_textnote_ids = _MirrorElements_ChangeHandler(doc, _mi_group_TXA_textnote_ids, _mid_plane, true);
                        ICollection<ElementId> __mi_TXA_dimension_ids = _MirrorElements_ChangeHandler(doc, _mi_TXA_dimension_ids, _mid_plane, true);

                        _aAaa_min_X = GetLeftRgiht_X_ingrid_fromGroup(doc, __mi_group_TXA_ids, out _aAaa_max_X);
                        __mid_plane = Plane.CreateByNormalAndOrigin(new XYZ(_aAaa_max_X + 1, 0, 0), new XYZ(_aAaa_max_X, 0, 0));//该处为原点镜像轴
                        ICollection<ElementId> ___mi_group_TXA_ids = _MirrorElements_ChangeHandler(doc, __mi_group_TXA_ids, __mid_plane, true);
                        ICollection<ElementId> ___mi_group_TXA_textnote_ids = _MirrorElements_ChangeHandler(doc, __mi_group_TXA_textnote_ids, __mid_plane, true);
                        ICollection<ElementId> ___mi_TXA_dimension_ids = _MirrorElements_ChangeHandler(doc, __mi_TXA_dimension_ids, __mid_plane, true);

                        _aAaa_min_X = GetLeftRgiht_X_ingrid_fromGroup(doc, ___mi_group_TXA_ids, out _aAaa_max_X);
                        ___mid_plane = Plane.CreateByNormalAndOrigin(new XYZ(_aAaa_max_X + 1, 0, 0), new XYZ(_aAaa_max_X, 0, 0));//该处为原点镜像轴
                        ICollection<ElementId> ____mi_group_TXA_ids = _MirrorElements_ChangeHandler(doc, ___mi_group_TXA_ids, ___mid_plane, true);
                        ICollection<ElementId> ____mi_group_TXA_textnote_ids = _MirrorElements_ChangeHandler(doc, ___mi_group_TXA_textnote_ids, ___mid_plane, true);
                        ICollection<ElementId> ____mi_TXA_dimension_ids = _MirrorElements_ChangeHandler(doc, ___mi_TXA_dimension_ids, ___mid_plane, true);
                    }
                    if (CMD._TXB_rvt != "null_path")
                    {
                        //判断是否需要对复制进来的模型数据进行二次操作
                    }
                    if (CMD._TXC_rvt != "null_path")
                    {
                        //判断是否需要对复制进来的模型数据进行二次操作
                    }
                    if (CMD._TXD_rvt != "null_path")
                    {
                        //判断是否需要对复制进来的模型数据进行二次操作
                    }
                    if (CMD._TXA_HXT_rvt != "null_path")
                    {
                        //判断是否需要对复制进来的模型数据进行二次操作
                        ICollection<ElementId> mi_group_TXA_Core_ids = _MirrorElements_ChangeHandler(doc, group_TXA_Core_ids, mid_plane, true);
                        ICollection<ElementId> mi_group_TXA_Core_textnote_ids = _MirrorElements_ChangeHandler(doc, group_TXA_Core_textnote_ids, mid_plane, true);

                        ICollection<ElementId> _mi_group_TXA_Core_ids = _MirrorElements_ChangeHandler(doc, group_TXA_Core_ids, _mid_plane, true);
                        ICollection<ElementId> _mi_group_TXA_Core_textnote_ids = _MirrorElements_ChangeHandler(doc, group_TXA_Core_textnote_ids, _mid_plane, true);
                    }
                    if (CMD._TXB_HXT_rvt != "null_path")
                    {
                        //判断是否需要对复制进来的模型数据进行二次操作
                    }
                    if (CMD._TXC_HXT_rvt != "null_path")
                    {
                        //判断是否需要对复制进来的模型数据进行二次操作
                    }
                    if (CMD._TXD_HXT_rvt != "null_path")
                    {
                        //判断是否需要对复制进来的模型数据进行二次操作
                    }
                    #endregion
                    transGroupCreateDwellingses.Assimilate();
                }
                else
                {
                    transGroupCreateDwellingses.RollBack();
                }
            }
            #region ***关闭所有源文档，但是不保存  time_20200312
            ResetDocument(_TXA_, _TXB_, _TXC_, _TXD_, _TXA_Core_, _TXB_Core_, _TXC_Core_, _TXD_Core_);
            #endregion
            #endregion
        }
        //新建户型_1T2H_ab_ 1DY
        public void Creat_HXZH_1T2H_ab_1DY_AB_(UIApplication uiapp)
        {
            #region all code
            #region ***初始化与revit模型进行交互的辅助变量 time_20200312
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Application app = uiapp.Application;
            Document doc = uidoc.Document;
            View actView = doc.ActiveView;
            View FL01_view = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Views).OfClass(typeof(ViewPlan)).FirstOrDefault(x => x.Name == "FL01") as View;
            Plane ori_plane = Plane.CreateByNormalAndOrigin(new XYZ(1, 0, 0), new XYZ(0, 0, 0));//该处为原点镜像轴
            Plane mid_plane = null;//该处mid定义为整体户型组合模式的中心对称轴位置
            LogicalOrFilter orFilter = CreatorFilter(uiapp);
            #endregion

            #region ***声明 all 变量 time_20200312
            Document _TXA_ = null;
            Document _TXB_ = null;
            Document _TXC_ = null;
            Document _TXD_ = null;
            Document _TXA_Core_ = null;
            Document _TXB_Core_ = null;
            Document _TXC_Core_ = null;
            Document _TXD_Core_ = null;
            ICollection<ElementId> group_TXA_ids = new List<ElementId>();//各个基础模型3D元素group的id_list
            ICollection<ElementId> group_TXB_ids = new List<ElementId>();
            ICollection<ElementId> group_TXC_ids = new List<ElementId>();
            ICollection<ElementId> group_TXD_ids = new List<ElementId>();
            ICollection<ElementId> group_TXA_Core_ids = new List<ElementId>();
            ICollection<ElementId> group_TXB_Core_ids = new List<ElementId>();
            ICollection<ElementId> group_TXC_Core_ids = new List<ElementId>();
            ICollection<ElementId> group_TXD_Core_ids = new List<ElementId>();
            ICollection<ElementId> group_TXA_textnote_ids = new List<ElementId>();//各个基础模型2D文字注释group的id_list
            ICollection<ElementId> group_TXB_textnote_ids = new List<ElementId>();
            ICollection<ElementId> group_TXC_textnote_ids = new List<ElementId>();
            ICollection<ElementId> group_TXD_textnote_ids = new List<ElementId>();
            ICollection<ElementId> group_TXA_Core_textnote_ids = new List<ElementId>();
            ICollection<ElementId> group_TXB_Core_textnote_ids = new List<ElementId>();
            ICollection<ElementId> group_TXC_Core_textnote_ids = new List<ElementId>();
            ICollection<ElementId> group_TXD_Core_textnote_ids = new List<ElementId>();
            ICollection<ElementId> _TXA_dimension_ids = new List<ElementId>();//由于2D标注元素，不可以成group，所以此处为各个标注元素的list
            ICollection<ElementId> _TXB_dimension_ids = new List<ElementId>();
            ICollection<ElementId> _TXC_dimension_ids = new List<ElementId>();
            ICollection<ElementId> _TXD_dimension_ids = new List<ElementId>();
            ICollection<ElementId> _TXA_Core_dimension_ids = new List<ElementId>();//注意，目前核心筒基础模型，不涉及标注元素,此外初始化，是为了满足函数参数的空值调用
            ICollection<ElementId> _TXB_Core_dimension_ids = new List<ElementId>();
            ICollection<ElementId> _TXC_Core_dimension_ids = new List<ElementId>();
            ICollection<ElementId> _TXD_Core_dimension_ids = new List<ElementId>();
            #endregion

            #region ***判断是否需要打开源文档 time_20200312
            //OpenTarDocument(uiapp, _TXA_, _TXB_, _TXC_, _TXD_, _TXA_Core_, _TXB_Core_, _TXC_Core_, _TXD_Core_);
            if (CMD._TXA_rvt != "null_path")
            {
                _TXA_ = app.OpenDocumentFile(CMD._TXA_rvt);
            }
            if (CMD._TXB_rvt != "null_path")
            {
                _TXB_ = app.OpenDocumentFile(CMD._TXB_rvt);
            }
            if (CMD._TXC_rvt != "null_path")
            {
                _TXC_ = app.OpenDocumentFile(CMD._TXC_rvt);
            }
            if (CMD._TXD_rvt != "null_path")
            {
                _TXD_ = app.OpenDocumentFile(CMD._TXD_rvt);
            }
            if (CMD._TXA_HXT_rvt != "null_path")
            {
                _TXA_Core_ = app.OpenDocumentFile(CMD._TXA_HXT_rvt);
            }
            if (CMD._TXB_HXT_rvt != "null_path")
            {
                _TXB_Core_ = app.OpenDocumentFile(CMD._TXB_HXT_rvt);
            }
            if (CMD._TXC_HXT_rvt != "null_path")
            {
                _TXC_Core_ = app.OpenDocumentFile(CMD._TXC_HXT_rvt);
            }
            if (CMD._TXD_HXT_rvt != "null_path")
            {
                _TXD_Core_ = app.OpenDocumentFile(CMD._TXD_HXT_rvt);
            }
            #endregion

            #region 根据户型组合模式 在各个源文件 定位元素 在这里不对核心筒定位做处理 原因：revit后台多次打开同一个文件，属于无效操作 time_20200312

            if (CMD._TXA_rvt != "null_path")
            {
                //判断是否需要对基础模型进行定位操作
            }
            if (CMD._TXB_rvt != "null_path")
            {
                ICollection<ElementId> _TXB_3D_EleIds_temp = (new FilteredElementCollector(_TXB_)).WherePasses(orFilter).WhereElementIsNotElementType().ToElementIds();
                FL01_view = new FilteredElementCollector(_TXB_).OfCategory(BuiltInCategory.OST_Views).OfClass(typeof(ViewPlan)).FirstOrDefault(x => x.Name == "FL01") as View;
                ICollection<ElementId> _TXB_TextNotes_temp = (new FilteredElementCollector(_TXB_, FL01_view.Id)).OwnedByView(FL01_view.Id).OfCategory(BuiltInCategory.OST_TextNotes).WhereElementIsNotElementType().ToElementIds();
                ICollection<ElementId> _TXB_Dimensions_temp = (new FilteredElementCollector(_TXB_, FL01_view.Id)).OwnedByView(FL01_view.Id).OfCategory(BuiltInCategory.OST_Dimensions).WhereElementIsNotElementType().ToElementIds();
                //判断是否需要对基础模型进行定位操作

                ICollection<ElementId> mi_TXB_3D_EleIds_temp = _MirrorElements_ChangeHandler(_TXB_, _TXB_3D_EleIds_temp, ori_plane, true);
                ICollection<ElementId> mi_TXB_TextNotes_temp = _MirrorElements_ChangeHandler(_TXB_, _TXB_TextNotes_temp, ori_plane, true);
                ICollection<ElementId> mi_TXB_Dimensions_temp = _MirrorElements_ChangeHandler(_TXB_, _TXB_Dimensions_temp, ori_plane, true);
                _deleteEles(_TXB_, _TXB_3D_EleIds_temp);
                _deleteEles(_TXB_, _TXB_TextNotes_temp);
            }
            if (CMD._TXC_rvt != "null_path")
            {
                //判断是否需要对基础模型进行定位操作
            }
            if (CMD._TXD_rvt != "null_path")
            {
                //判断是否需要对基础模型进行定位操作
            }
            #endregion

            using (TransactionGroup transGroupCreateDwellingses = new TransactionGroup(doc, "创建户型零件组_group"))//开展事务组
            {
                if (transGroupCreateDwellingses.Start() == TransactionStatus.Started)//开启事务组
                {
                    #region ***将定位好的基础模型数据，复制到当前工作视图 并creat group 标注元素只有list，不可以creat group time_20200312
                    if (CMD._TXA_rvt != "null_path")
                    {
                        group_TXA_ids = Creat_Unit_GroupId_copyFrom_Ori(_TXA_, doc, orFilter, actView, out group_TXA_textnote_ids, out _TXA_dimension_ids, "_TXA_", "_TXA_textnote_");
                    }
                    if (CMD._TXB_rvt != "null_path")
                    {
                        group_TXB_ids = Creat_Unit_GroupId_copyFrom_Ori(_TXB_, doc, orFilter, actView, out group_TXB_textnote_ids, out _TXB_dimension_ids, "_TXB_", "_TXB_textnote_");
                    }
                    if (CMD._TXC_rvt != "null_path")
                    {
                        group_TXC_ids = Creat_Unit_GroupId_copyFrom_Ori(_TXC_, doc, orFilter, actView, out group_TXC_textnote_ids, out _TXC_dimension_ids, "_TXC_", "_TXC_textnote_");
                    }
                    if (CMD._TXD_rvt != "null_path")
                    {
                        group_TXD_ids = Creat_Unit_GroupId_copyFrom_Ori(_TXD_, doc, orFilter, actView, out group_TXD_textnote_ids, out _TXD_dimension_ids, "_TXD_", "_TXD_textnote_");
                    }
                    if (CMD._TXA_HXT_rvt != "null_path")
                    {
                        group_TXA_Core_ids = Creat_Unit_GroupId_copyFrom_Ori(_TXA_Core_, doc, orFilter, actView, out group_TXA_Core_textnote_ids, out _TXA_Core_dimension_ids, "_TXA_Core_", "_TXA_Core_textnote_");
                    }
                    if (CMD._TXB_HXT_rvt != "null_path")
                    {
                        group_TXB_Core_ids = Creat_Unit_GroupId_copyFrom_Ori(_TXB_Core_, doc, orFilter, actView, out group_TXB_Core_textnote_ids, out _TXB_Core_dimension_ids, "_TXB_Core_", "_TXB_Core_textnote_");
                    }
                    if (CMD._TXC_HXT_rvt != "null_path")
                    {
                        group_TXC_Core_ids = Creat_Unit_GroupId_copyFrom_Ori(_TXC_Core_, doc, orFilter, actView, out group_TXC_Core_textnote_ids, out _TXC_Core_dimension_ids, "_TXC_Core_", "_TXC_Core_textnote_");
                    }
                    if (CMD._TXD_HXT_rvt != "null_path")
                    {
                        group_TXD_Core_ids = Creat_Unit_GroupId_copyFrom_Ori(_TXD_Core_, doc, orFilter, actView, out group_TXD_Core_textnote_ids, out _TXD_Core_dimension_ids, "_TXD_Core_", "_TXD_Core_textnote_");
                    }
                    #endregion

                    #region 在当前视图，对户型进行排列组合 注意模型复制到当前文档时，所处的位置 time_20200312
                    //abccbd 根据字母组合进行判断
                    Plane _mid_plane = ori_plane;
                    Plane __mid_plane = ori_plane;
                    Plane ___mid_plane = ori_plane;

                    if (CMD._TXA_rvt != "null_path")
                    {
                        //判断是否需要对复制进来的模型数据进行二次操作
                    }
                    if (CMD._TXB_rvt != "null_path")
                    {
                        //判断是否需要对复制进来的模型数据进行二次操作
                    }
                    if (CMD._TXC_rvt != "null_path")
                    {
                        //判断是否需要对复制进来的模型数据进行二次操作
                    }
                    if (CMD._TXD_rvt != "null_path")
                    {
                        //判断是否需要对复制进来的模型数据进行二次操作
                    }
                    if (CMD._TXA_HXT_rvt != "null_path")
                    {
                        //判断是否需要对复制进来的模型数据进行二次操作
                    }
                    if (CMD._TXB_HXT_rvt != "null_path")
                    {
                        //判断是否需要对复制进来的模型数据进行二次操作
                    }
                    if (CMD._TXC_HXT_rvt != "null_path")
                    {
                        //判断是否需要对复制进来的模型数据进行二次操作
                    }
                    if (CMD._TXD_HXT_rvt != "null_path")
                    {
                        //判断是否需要对复制进来的模型数据进行二次操作
                    }
                    #endregion
                    transGroupCreateDwellingses.Assimilate();
                }
                else
                {
                    transGroupCreateDwellingses.RollBack();
                }
            }
            #region ***关闭所有源文档，但是不保存  time_20200312
            ResetDocument(_TXA_, _TXB_, _TXC_, _TXD_, _TXA_Core_, _TXB_Core_, _TXC_Core_, _TXD_Core_);
            #endregion
            #endregion
        }
        //新建户型_1T2H_abba_2DY
        public void Creat_HXZH_1T2H_abba_2DY_AB_(UIApplication uiapp)
        {
            #region all code
            #region ***初始化与revit模型进行交互的辅助变量 time_20200312
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Application app = uiapp.Application;
            Document doc = uidoc.Document;
            View actView = doc.ActiveView;
            View FL01_view = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Views).OfClass(typeof(ViewPlan)).FirstOrDefault(x => x.Name == "FL01") as View;
            Plane ori_plane = Plane.CreateByNormalAndOrigin(new XYZ(1, 0, 0), new XYZ(0, 0, 0));//该处为原点镜像轴
            Plane mid_plane = null;//该处mid定义为整体户型组合模式的中心对称轴位置
            LogicalOrFilter orFilter = CreatorFilter(uiapp);
            #endregion

            #region ***声明 all 变量 time_20200312
            Document _TXA_ = null;
            Document _TXB_ = null;
            Document _TXC_ = null;
            Document _TXD_ = null;
            Document _TXA_Core_ = null;
            Document _TXB_Core_ = null;
            Document _TXC_Core_ = null;
            Document _TXD_Core_ = null;
            ICollection<ElementId> group_TXA_ids = new List<ElementId>();//各个基础模型3D元素group的id_list
            ICollection<ElementId> group_TXB_ids = new List<ElementId>();
            ICollection<ElementId> group_TXC_ids = new List<ElementId>();
            ICollection<ElementId> group_TXD_ids = new List<ElementId>();
            ICollection<ElementId> group_TXA_Core_ids = new List<ElementId>();
            ICollection<ElementId> group_TXB_Core_ids = new List<ElementId>();
            ICollection<ElementId> group_TXC_Core_ids = new List<ElementId>();
            ICollection<ElementId> group_TXD_Core_ids = new List<ElementId>();
            ICollection<ElementId> group_TXA_textnote_ids = new List<ElementId>();//各个基础模型2D文字注释group的id_list
            ICollection<ElementId> group_TXB_textnote_ids = new List<ElementId>();
            ICollection<ElementId> group_TXC_textnote_ids = new List<ElementId>();
            ICollection<ElementId> group_TXD_textnote_ids = new List<ElementId>();
            ICollection<ElementId> group_TXA_Core_textnote_ids = new List<ElementId>();
            ICollection<ElementId> group_TXB_Core_textnote_ids = new List<ElementId>();
            ICollection<ElementId> group_TXC_Core_textnote_ids = new List<ElementId>();
            ICollection<ElementId> group_TXD_Core_textnote_ids = new List<ElementId>();
            ICollection<ElementId> _TXA_dimension_ids = new List<ElementId>();//由于2D标注元素，不可以成group，所以此处为各个标注元素的list
            ICollection<ElementId> _TXB_dimension_ids = new List<ElementId>();
            ICollection<ElementId> _TXC_dimension_ids = new List<ElementId>();
            ICollection<ElementId> _TXD_dimension_ids = new List<ElementId>();
            ICollection<ElementId> _TXA_Core_dimension_ids = new List<ElementId>();//注意，目前核心筒基础模型，不涉及标注元素,此外初始化，是为了满足函数参数的空值调用
            ICollection<ElementId> _TXB_Core_dimension_ids = new List<ElementId>();
            ICollection<ElementId> _TXC_Core_dimension_ids = new List<ElementId>();
            ICollection<ElementId> _TXD_Core_dimension_ids = new List<ElementId>();
            #endregion

            #region ***判断是否需要打开源文档 time_20200312
            //OpenTarDocument(uiapp, _TXA_, _TXB_, _TXC_, _TXD_, _TXA_Core_, _TXB_Core_, _TXC_Core_, _TXD_Core_);
            if (CMD._TXA_rvt != "null_path")
            {
                _TXA_ = app.OpenDocumentFile(CMD._TXA_rvt);
            }
            if (CMD._TXB_rvt != "null_path")
            {
                _TXB_ = app.OpenDocumentFile(CMD._TXB_rvt);
            }
            if (CMD._TXC_rvt != "null_path")
            {
                _TXC_ = app.OpenDocumentFile(CMD._TXC_rvt);
            }
            if (CMD._TXD_rvt != "null_path")
            {
                _TXD_ = app.OpenDocumentFile(CMD._TXD_rvt);
            }
            if (CMD._TXA_HXT_rvt != "null_path")
            {
                _TXA_Core_ = app.OpenDocumentFile(CMD._TXA_HXT_rvt);
            }
            if (CMD._TXB_HXT_rvt != "null_path")
            {
                _TXB_Core_ = app.OpenDocumentFile(CMD._TXB_HXT_rvt);
            }
            if (CMD._TXC_HXT_rvt != "null_path")
            {
                _TXC_Core_ = app.OpenDocumentFile(CMD._TXC_HXT_rvt);
            }
            if (CMD._TXD_HXT_rvt != "null_path")
            {
                _TXD_Core_ = app.OpenDocumentFile(CMD._TXD_HXT_rvt);
            }
            #endregion

            #region 根据户型组合模式 在各个源文件 定位元素 在这里不对核心筒定位做处理 原因：revit后台多次打开同一个文件，属于无效操作 time_20200312

            if (CMD._TXA_rvt != "null_path")
            {
                //判断是否需要对基础模型进行定位操作
            }
            if (CMD._TXB_rvt != "null_path")
            {
                ICollection<ElementId> _TXB_3D_EleIds_temp = (new FilteredElementCollector(_TXB_)).WherePasses(orFilter).WhereElementIsNotElementType().ToElementIds();
                FL01_view = new FilteredElementCollector(_TXB_).OfCategory(BuiltInCategory.OST_Views).OfClass(typeof(ViewPlan)).FirstOrDefault(x => x.Name == "FL01") as View;
                ICollection<ElementId> _TXB_TextNotes_temp = (new FilteredElementCollector(_TXB_, FL01_view.Id)).OwnedByView(FL01_view.Id).OfCategory(BuiltInCategory.OST_TextNotes).WhereElementIsNotElementType().ToElementIds();
                ICollection<ElementId> _TXB_Dimensions_temp = (new FilteredElementCollector(_TXB_, FL01_view.Id)).OwnedByView(FL01_view.Id).OfCategory(BuiltInCategory.OST_Dimensions).WhereElementIsNotElementType().ToElementIds();
                //判断是否需要对基础模型进行定位操作

                ICollection<ElementId> mi_TXB_3D_EleIds_temp = _MirrorElements_ChangeHandler(_TXB_, _TXB_3D_EleIds_temp, ori_plane, true);
                ICollection<ElementId> mi_TXB_TextNotes_temp = _MirrorElements_ChangeHandler(_TXB_, _TXB_TextNotes_temp, ori_plane, true);
                ICollection<ElementId> mi_TXB_Dimensions_temp = _MirrorElements_ChangeHandler(_TXB_, _TXB_Dimensions_temp, ori_plane, true);
                _deleteEles(_TXB_, _TXB_3D_EleIds_temp);
                _deleteEles(_TXB_, _TXB_TextNotes_temp);
            }
            if (CMD._TXC_rvt != "null_path")
            {
                //判断是否需要对基础模型进行定位操作
            }
            if (CMD._TXD_rvt != "null_path")
            {
                //判断是否需要对基础模型进行定位操作
            }
            #endregion

            using (TransactionGroup transGroupCreateDwellingses = new TransactionGroup(doc, "创建户型零件组_group"))//开展事务组
            {
                if (transGroupCreateDwellingses.Start() == TransactionStatus.Started)//开启事务组
                {
                    #region ***将定位好的基础模型数据，复制到当前工作视图 并creat group 标注元素只有list，不可以creat group time_20200312
                    if (CMD._TXA_rvt != "null_path")
                    {
                        group_TXA_ids = Creat_Unit_GroupId_copyFrom_Ori(_TXA_, doc, orFilter, actView, out group_TXA_textnote_ids, out _TXA_dimension_ids, "_TXA_", "_TXA_textnote_");
                    }
                    if (CMD._TXB_rvt != "null_path")
                    {
                        group_TXB_ids = Creat_Unit_GroupId_copyFrom_Ori(_TXB_, doc, orFilter, actView, out group_TXB_textnote_ids, out _TXB_dimension_ids, "_TXB_", "_TXB_textnote_");
                    }
                    if (CMD._TXC_rvt != "null_path")
                    {
                        group_TXC_ids = Creat_Unit_GroupId_copyFrom_Ori(_TXC_, doc, orFilter, actView, out group_TXC_textnote_ids, out _TXC_dimension_ids, "_TXC_", "_TXC_textnote_");
                    }
                    if (CMD._TXD_rvt != "null_path")
                    {
                        group_TXD_ids = Creat_Unit_GroupId_copyFrom_Ori(_TXD_, doc, orFilter, actView, out group_TXD_textnote_ids, out _TXD_dimension_ids, "_TXD_", "_TXD_textnote_");
                    }
                    if (CMD._TXA_HXT_rvt != "null_path")
                    {
                        group_TXA_Core_ids = Creat_Unit_GroupId_copyFrom_Ori(_TXA_Core_, doc, orFilter, actView, out group_TXA_Core_textnote_ids, out _TXA_Core_dimension_ids, "_TXA_Core_", "_TXA_Core_textnote_");
                    }
                    if (CMD._TXB_HXT_rvt != "null_path")
                    {
                        group_TXB_Core_ids = Creat_Unit_GroupId_copyFrom_Ori(_TXB_Core_, doc, orFilter, actView, out group_TXB_Core_textnote_ids, out _TXB_Core_dimension_ids, "_TXB_Core_", "_TXB_Core_textnote_");
                    }
                    if (CMD._TXC_HXT_rvt != "null_path")
                    {
                        group_TXC_Core_ids = Creat_Unit_GroupId_copyFrom_Ori(_TXC_Core_, doc, orFilter, actView, out group_TXC_Core_textnote_ids, out _TXC_Core_dimension_ids, "_TXC_Core_", "_TXC_Core_textnote_");
                    }
                    if (CMD._TXD_HXT_rvt != "null_path")
                    {
                        group_TXD_Core_ids = Creat_Unit_GroupId_copyFrom_Ori(_TXD_Core_, doc, orFilter, actView, out group_TXD_Core_textnote_ids, out _TXD_Core_dimension_ids, "_TXD_Core_", "_TXD_Core_textnote_");
                    }
                    #endregion

                    #region 在当前视图，对户型进行排列组合 注意模型复制到当前文档时，所处的位置 time_20200312
                    //abccbd 根据字母组合进行判断
                    Plane _mid_plane = ori_plane;
                    Plane __mid_plane = ori_plane;
                    Plane ___mid_plane = ori_plane;

                    if (CMD._TXA_rvt != "null_path")
                    {
                        //判断是否需要对复制进来的模型数据进行二次操作
                        double _aBba_max_X;
                        double _aBba_min_X = GetLeftRgiht_X_ingrid_fromGroup(doc, group_TXB_ids, out _aBba_max_X);
                        mid_plane = Plane.CreateByNormalAndOrigin(new XYZ(_aBba_max_X + 1, 0, 0), new XYZ(_aBba_max_X, 0, 0));//该处为原点镜像轴

                        ICollection<ElementId> mi_group_TXA_ids = _MirrorElements_ChangeHandler(doc, group_TXA_ids, mid_plane, true);
                        ICollection<ElementId> mi_group_TXA_textnote_ids = _MirrorElements_ChangeHandler(doc, group_TXA_textnote_ids, mid_plane, true);
                        ICollection<ElementId> mi_TXA_dimension_ids = _MirrorElements_ChangeHandler(doc, _TXA_dimension_ids, mid_plane, true);
                    }
                    if (CMD._TXB_rvt != "null_path")
                    {
                        //判断是否需要对复制进来的模型数据进行二次操作
                        ICollection<ElementId> mi_group_TXB_ids = _MirrorElements_ChangeHandler(doc, group_TXB_ids, mid_plane, true);
                        ICollection<ElementId> mi_group_TXB_textnote_ids = _MirrorElements_ChangeHandler(doc, group_TXB_textnote_ids, mid_plane, true);
                        ICollection<ElementId> mi_TXB_dimension_ids = _MirrorElements_ChangeHandler(doc, _TXB_dimension_ids, mid_plane, true);
                    }
                    if (CMD._TXC_rvt != "null_path")
                    {
                        //判断是否需要对复制进来的模型数据进行二次操作
                    }
                    if (CMD._TXD_rvt != "null_path")
                    {
                        //判断是否需要对复制进来的模型数据进行二次操作
                    }
                    if (CMD._TXA_HXT_rvt != "null_path")
                    {
                        //判断是否需要对复制进来的模型数据进行二次操作
                        ICollection<ElementId> mi_group_TXA_Core_ids = _MirrorElements_ChangeHandler(doc, group_TXA_Core_ids, mid_plane, true);
                        ICollection<ElementId> mi_group_TXA_Core_textnote_ids = _MirrorElements_ChangeHandler(doc, group_TXA_Core_textnote_ids, mid_plane, true);
                    }
                    if (CMD._TXB_HXT_rvt != "null_path")
                    {
                        //判断是否需要对复制进来的模型数据进行二次操作
                    }
                    if (CMD._TXC_HXT_rvt != "null_path")
                    {
                        //判断是否需要对复制进来的模型数据进行二次操作
                    }
                    if (CMD._TXD_HXT_rvt != "null_path")
                    {
                        //判断是否需要对复制进来的模型数据进行二次操作
                    }
                    #endregion
                    transGroupCreateDwellingses.Assimilate();
                }
                else
                {
                    transGroupCreateDwellingses.RollBack();
                }
            }
            #region ***关闭所有源文档，但是不保存  time_20200312
            ResetDocument(_TXA_, _TXB_, _TXC_, _TXD_, _TXA_Core_, _TXB_Core_, _TXC_Core_, _TXD_Core_);
            #endregion
            #endregion
        }
        //新建户型_1T2H_aaab_2DY
        public void Creat_HXZH_1T2H_aaab_2DY_AB_(UIApplication uiapp)
        {
            #region all code
            #region ***初始化与revit模型进行交互的辅助变量 time_20200312
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Application app = uiapp.Application;
            Document doc = uidoc.Document;
            View actView = doc.ActiveView;
            View FL01_view = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Views).OfClass(typeof(ViewPlan)).FirstOrDefault(x => x.Name == "FL01") as View;
            Plane ori_plane = Plane.CreateByNormalAndOrigin(new XYZ(1, 0, 0), new XYZ(0, 0, 0));//该处为原点镜像轴
            Plane mid_plane = null;//该处mid定义为整体户型组合模式的中心对称轴位置
            LogicalOrFilter orFilter = CreatorFilter(uiapp);
            #endregion

            #region ***声明 all 变量 time_20200312
            Document _TXA_ = null;
            Document _TXB_ = null;
            Document _TXC_ = null;
            Document _TXD_ = null;
            Document _TXA_Core_ = null;
            Document _TXB_Core_ = null;
            Document _TXC_Core_ = null;
            Document _TXD_Core_ = null;
            ICollection<ElementId> group_TXA_ids = new List<ElementId>();//各个基础模型3D元素group的id_list
            ICollection<ElementId> group_TXB_ids = new List<ElementId>();
            ICollection<ElementId> group_TXC_ids = new List<ElementId>();
            ICollection<ElementId> group_TXD_ids = new List<ElementId>();
            ICollection<ElementId> group_TXA_Core_ids = new List<ElementId>();
            ICollection<ElementId> group_TXB_Core_ids = new List<ElementId>();
            ICollection<ElementId> group_TXC_Core_ids = new List<ElementId>();
            ICollection<ElementId> group_TXD_Core_ids = new List<ElementId>();
            ICollection<ElementId> group_TXA_textnote_ids = new List<ElementId>();//各个基础模型2D文字注释group的id_list
            ICollection<ElementId> group_TXB_textnote_ids = new List<ElementId>();
            ICollection<ElementId> group_TXC_textnote_ids = new List<ElementId>();
            ICollection<ElementId> group_TXD_textnote_ids = new List<ElementId>();
            ICollection<ElementId> group_TXA_Core_textnote_ids = new List<ElementId>();
            ICollection<ElementId> group_TXB_Core_textnote_ids = new List<ElementId>();
            ICollection<ElementId> group_TXC_Core_textnote_ids = new List<ElementId>();
            ICollection<ElementId> group_TXD_Core_textnote_ids = new List<ElementId>();
            ICollection<ElementId> _TXA_dimension_ids = new List<ElementId>();//由于2D标注元素，不可以成group，所以此处为各个标注元素的list
            ICollection<ElementId> _TXB_dimension_ids = new List<ElementId>();
            ICollection<ElementId> _TXC_dimension_ids = new List<ElementId>();
            ICollection<ElementId> _TXD_dimension_ids = new List<ElementId>();
            ICollection<ElementId> _TXA_Core_dimension_ids = new List<ElementId>();//注意，目前核心筒基础模型，不涉及标注元素,此外初始化，是为了满足函数参数的空值调用
            ICollection<ElementId> _TXB_Core_dimension_ids = new List<ElementId>();
            ICollection<ElementId> _TXC_Core_dimension_ids = new List<ElementId>();
            ICollection<ElementId> _TXD_Core_dimension_ids = new List<ElementId>();
            #endregion

            #region ***判断是否需要打开源文档 time_20200312
            //OpenTarDocument(uiapp, _TXA_, _TXB_, _TXC_, _TXD_, _TXA_Core_, _TXB_Core_, _TXC_Core_, _TXD_Core_);
            if (CMD._TXA_rvt != "null_path")
            {
                _TXA_ = app.OpenDocumentFile(CMD._TXA_rvt);
            }
            if (CMD._TXB_rvt != "null_path")
            {
                _TXB_ = app.OpenDocumentFile(CMD._TXB_rvt);
            }
            if (CMD._TXC_rvt != "null_path")
            {
                _TXC_ = app.OpenDocumentFile(CMD._TXC_rvt);
            }
            if (CMD._TXD_rvt != "null_path")
            {
                _TXD_ = app.OpenDocumentFile(CMD._TXD_rvt);
            }
            if (CMD._TXA_HXT_rvt != "null_path")
            {
                _TXA_Core_ = app.OpenDocumentFile(CMD._TXA_HXT_rvt);
            }
            if (CMD._TXB_HXT_rvt != "null_path")
            {
                _TXB_Core_ = app.OpenDocumentFile(CMD._TXB_HXT_rvt);
            }
            if (CMD._TXC_HXT_rvt != "null_path")
            {
                _TXC_Core_ = app.OpenDocumentFile(CMD._TXC_HXT_rvt);
            }
            if (CMD._TXD_HXT_rvt != "null_path")
            {
                _TXD_Core_ = app.OpenDocumentFile(CMD._TXD_HXT_rvt);
            }
            #endregion

            #region 根据户型组合模式 在各个源文件 定位元素 在这里不对核心筒定位做处理 原因：revit后台多次打开同一个文件，属于无效操作 time_20200312

            if (CMD._TXA_rvt != "null_path")
            {
                ICollection<ElementId> _TXA_3D_EleIds_temp = (new FilteredElementCollector(_TXA_)).WherePasses(orFilter).WhereElementIsNotElementType().ToElementIds();
                FL01_view = new FilteredElementCollector(_TXA_).OfCategory(BuiltInCategory.OST_Views).OfClass(typeof(ViewPlan)).FirstOrDefault(x => x.Name == "FL01") as View;
                ICollection<ElementId> _TXA_TextNotes_temp = (new FilteredElementCollector(_TXA_, FL01_view.Id)).OwnedByView(FL01_view.Id).OfCategory(BuiltInCategory.OST_TextNotes).WhereElementIsNotElementType().ToElementIds();
                ICollection<ElementId> _TXA_Dimensions_temp = (new FilteredElementCollector(_TXA_, FL01_view.Id)).OwnedByView(FL01_view.Id).OfCategory(BuiltInCategory.OST_Dimensions).WhereElementIsNotElementType().ToElementIds();
                //判断是否需要对基础模型进行定位操作
                double _aAab_max_X;
                double _aAad_min_X = GetLeftRgiht_X_ingrid(_TXA_, _TXA_3D_EleIds_temp, out _aAab_max_X);

                double mid_plane_x = _aAab_max_X - _aAad_min_X;
                mid_plane = Plane.CreateByNormalAndOrigin(new XYZ(mid_plane_x + 1, 0, 0), new XYZ(mid_plane_x, 0, 0));//该处为原点镜像轴


            }
            if (CMD._TXB_rvt != "null_path")
            {
                ICollection<ElementId> _TXB_3D_EleIds_temp = (new FilteredElementCollector(_TXB_)).WherePasses(orFilter).WhereElementIsNotElementType().ToElementIds();
                FL01_view = new FilteredElementCollector(_TXB_).OfCategory(BuiltInCategory.OST_Views).OfClass(typeof(ViewPlan)).FirstOrDefault(x => x.Name == "FL01") as View;
                ICollection<ElementId> _TXB_TextNotes_temp = (new FilteredElementCollector(_TXB_, FL01_view.Id)).OwnedByView(FL01_view.Id).OfCategory(BuiltInCategory.OST_TextNotes).WhereElementIsNotElementType().ToElementIds();
                ICollection<ElementId> _TXB_Dimensions_temp = (new FilteredElementCollector(_TXB_, FL01_view.Id)).OwnedByView(FL01_view.Id).OfCategory(BuiltInCategory.OST_Dimensions).WhereElementIsNotElementType().ToElementIds();
                //判断是否需要对基础模型进行定位操作
                ICollection<ElementId> mi_TXB_3D_EleIds_temp = _MirrorElements_ChangeHandler(_TXB_, _TXB_3D_EleIds_temp, mid_plane, true);
                ICollection<ElementId> mi_TXB_TextNotes_temp = _MirrorElements_ChangeHandler(_TXB_, _TXB_TextNotes_temp, mid_plane, true);
                ICollection<ElementId> mi_TXB_Dimensions_temp = _MirrorElements_ChangeHandler(_TXB_, _TXB_Dimensions_temp, mid_plane, true);
                _deleteEles(_TXB_, _TXB_3D_EleIds_temp);
                _deleteEles(_TXB_, _TXB_TextNotes_temp);
            }
            if (CMD._TXC_rvt != "null_path")
            {
                //判断是否需要对基础模型进行定位操作
            }
            if (CMD._TXD_rvt != "null_path")
            {
                //判断是否需要对基础模型进行定位操作
            }
            #endregion

            using (TransactionGroup transGroupCreateDwellingses = new TransactionGroup(doc, "创建户型零件组_group"))//开展事务组
            {
                if (transGroupCreateDwellingses.Start() == TransactionStatus.Started)//开启事务组
                {
                    #region ***将定位好的基础模型数据，复制到当前工作视图 并creat group 标注元素只有list，不可以creat group time_20200312
                    if (CMD._TXA_rvt != "null_path")
                    {
                        group_TXA_ids = Creat_Unit_GroupId_copyFrom_Ori(_TXA_, doc, orFilter, actView, out group_TXA_textnote_ids, out _TXA_dimension_ids, "_TXA_", "_TXA_textnote_");
                    }
                    if (CMD._TXB_rvt != "null_path")
                    {
                        group_TXB_ids = Creat_Unit_GroupId_copyFrom_Ori(_TXB_, doc, orFilter, actView, out group_TXB_textnote_ids, out _TXB_dimension_ids, "_TXB_", "_TXB_textnote_");
                    }
                    if (CMD._TXC_rvt != "null_path")
                    {
                        group_TXC_ids = Creat_Unit_GroupId_copyFrom_Ori(_TXC_, doc, orFilter, actView, out group_TXC_textnote_ids, out _TXC_dimension_ids, "_TXC_", "_TXC_textnote_");
                    }
                    if (CMD._TXD_rvt != "null_path")
                    {
                        group_TXD_ids = Creat_Unit_GroupId_copyFrom_Ori(_TXD_, doc, orFilter, actView, out group_TXD_textnote_ids, out _TXD_dimension_ids, "_TXD_", "_TXD_textnote_");
                    }
                    if (CMD._TXA_HXT_rvt != "null_path")
                    {
                        group_TXA_Core_ids = Creat_Unit_GroupId_copyFrom_Ori(_TXA_Core_, doc, orFilter, actView, out group_TXA_Core_textnote_ids, out _TXA_Core_dimension_ids, "_TXA_Core_", "_TXA_Core_textnote_");
                    }
                    if (CMD._TXB_HXT_rvt != "null_path")
                    {
                        group_TXB_Core_ids = Creat_Unit_GroupId_copyFrom_Ori(_TXB_Core_, doc, orFilter, actView, out group_TXB_Core_textnote_ids, out _TXB_Core_dimension_ids, "_TXB_Core_", "_TXB_Core_textnote_");
                    }
                    if (CMD._TXC_HXT_rvt != "null_path")
                    {
                        group_TXC_Core_ids = Creat_Unit_GroupId_copyFrom_Ori(_TXC_Core_, doc, orFilter, actView, out group_TXC_Core_textnote_ids, out _TXC_Core_dimension_ids, "_TXC_Core_", "_TXC_Core_textnote_");
                    }
                    if (CMD._TXD_HXT_rvt != "null_path")
                    {
                        group_TXD_Core_ids = Creat_Unit_GroupId_copyFrom_Ori(_TXD_Core_, doc, orFilter, actView, out group_TXD_Core_textnote_ids, out _TXD_Core_dimension_ids, "_TXD_Core_", "_TXD_Core_textnote_");
                    }
                    #endregion

                    #region 在当前视图，对户型进行排列组合 注意模型复制到当前文档时，所处的位置 time_20200312
                    //abccbd 根据字母组合进行判断
                    Plane _mid_plane = ori_plane;
                    Plane __mid_plane = ori_plane;
                    Plane ___mid_plane = ori_plane;

                    if (CMD._TXA_rvt != "null_path")
                    {
                        //判断是否需要对复制进来的模型数据进行二次操作
                        ICollection<ElementId> mi_group_TXA_ids = _MirrorElements_ChangeHandler(doc, group_TXA_ids, ori_plane, true);
                        ICollection<ElementId> mi_group_TXA_textnote_ids = _MirrorElements_ChangeHandler(doc, group_TXA_textnote_ids, ori_plane, true);
                        ICollection<ElementId> mi_TXA_dimension_ids = _MirrorElements_ChangeHandler(doc, _TXA_dimension_ids, ori_plane, true);

                        ICollection<ElementId> _mi_group_TXA_ids = _MirrorElements_ChangeHandler(doc, mi_group_TXA_ids, mid_plane, true);
                        ICollection<ElementId> _mi_group_TXA_textnote_ids = _MirrorElements_ChangeHandler(doc, mi_group_TXA_textnote_ids, mid_plane, true);
                        ICollection<ElementId> _mi_TXA_dimension_ids = _MirrorElements_ChangeHandler(doc, mi_TXA_dimension_ids, mid_plane, true);
                    }
                    if (CMD._TXB_rvt != "null_path")
                    {
                        //判断是否需要对复制进来的模型数据进行二次操作
                    }
                    if (CMD._TXC_rvt != "null_path")
                    {
                        //判断是否需要对复制进来的模型数据进行二次操作
                    }
                    if (CMD._TXD_rvt != "null_path")
                    {
                        //判断是否需要对复制进来的模型数据进行二次操作
                    }
                    if (CMD._TXA_HXT_rvt != "null_path")
                    {
                        //判断是否需要对复制进来的模型数据进行二次操作
                        ICollection<ElementId> mi_group_TXA_Core_ids = _MirrorElements_ChangeHandler(doc, group_TXA_Core_ids, mid_plane, true);
                        ICollection<ElementId> mi_group_TXA_Core_textnote_ids = _MirrorElements_ChangeHandler(doc, group_TXA_Core_textnote_ids, mid_plane, true);
                    }
                    if (CMD._TXB_HXT_rvt != "null_path")
                    {
                        //判断是否需要对复制进来的模型数据进行二次操作
                    }
                    if (CMD._TXC_HXT_rvt != "null_path")
                    {
                        //判断是否需要对复制进来的模型数据进行二次操作
                    }
                    if (CMD._TXD_HXT_rvt != "null_path")
                    {
                        //判断是否需要对复制进来的模型数据进行二次操作
                    }
                    #endregion
                    transGroupCreateDwellingses.Assimilate();
                }
                else
                {
                    transGroupCreateDwellingses.RollBack();
                }
            }
            #region ***关闭所有源文档，但是不保存  time_20200312
            ResetDocument(_TXA_, _TXB_, _TXC_, _TXD_, _TXA_Core_, _TXB_Core_, _TXC_Core_, _TXD_Core_);
            #endregion
            #endregion

        }
        //新建户型_1T2H_aabbaa_3DY
        public void Creat_HXZH_1T2H_aabbaa_3DY_AB_(UIApplication uiapp)
        {
            #region all code
            #region ***初始化与revit模型进行交互的辅助变量 time_20200312
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Application app = uiapp.Application;
            Document doc = uidoc.Document;
            View actView = doc.ActiveView;
            View FL01_view = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Views).OfClass(typeof(ViewPlan)).FirstOrDefault(x => x.Name == "FL01") as View;
            Plane ori_plane = Plane.CreateByNormalAndOrigin(new XYZ(1, 0, 0), new XYZ(0, 0, 0));//该处为原点镜像轴
            Plane mid_plane = null;//该处mid定义为整体户型组合模式的中心对称轴位置
            LogicalOrFilter orFilter = CreatorFilter(uiapp);
            #endregion

            #region ***声明 all 变量 time_20200312
            Document _TXA_ = null;
            Document _TXB_ = null;
            Document _TXC_ = null;
            Document _TXD_ = null;
            Document _TXA_Core_ = null;
            Document _TXB_Core_ = null;
            Document _TXC_Core_ = null;
            Document _TXD_Core_ = null;
            ICollection<ElementId> group_TXA_ids = new List<ElementId>();//各个基础模型3D元素group的id_list
            ICollection<ElementId> group_TXB_ids = new List<ElementId>();
            ICollection<ElementId> group_TXC_ids = new List<ElementId>();
            ICollection<ElementId> group_TXD_ids = new List<ElementId>();
            ICollection<ElementId> group_TXA_Core_ids = new List<ElementId>();
            ICollection<ElementId> group_TXB_Core_ids = new List<ElementId>();
            ICollection<ElementId> group_TXC_Core_ids = new List<ElementId>();
            ICollection<ElementId> group_TXD_Core_ids = new List<ElementId>();
            ICollection<ElementId> group_TXA_textnote_ids = new List<ElementId>();//各个基础模型2D文字注释group的id_list
            ICollection<ElementId> group_TXB_textnote_ids = new List<ElementId>();
            ICollection<ElementId> group_TXC_textnote_ids = new List<ElementId>();
            ICollection<ElementId> group_TXD_textnote_ids = new List<ElementId>();
            ICollection<ElementId> group_TXA_Core_textnote_ids = new List<ElementId>();
            ICollection<ElementId> group_TXB_Core_textnote_ids = new List<ElementId>();
            ICollection<ElementId> group_TXC_Core_textnote_ids = new List<ElementId>();
            ICollection<ElementId> group_TXD_Core_textnote_ids = new List<ElementId>();
            ICollection<ElementId> _TXA_dimension_ids = new List<ElementId>();//由于2D标注元素，不可以成group，所以此处为各个标注元素的list
            ICollection<ElementId> _TXB_dimension_ids = new List<ElementId>();
            ICollection<ElementId> _TXC_dimension_ids = new List<ElementId>();
            ICollection<ElementId> _TXD_dimension_ids = new List<ElementId>();
            ICollection<ElementId> _TXA_Core_dimension_ids = new List<ElementId>();//注意，目前核心筒基础模型，不涉及标注元素,此外初始化，是为了满足函数参数的空值调用
            ICollection<ElementId> _TXB_Core_dimension_ids = new List<ElementId>();
            ICollection<ElementId> _TXC_Core_dimension_ids = new List<ElementId>();
            ICollection<ElementId> _TXD_Core_dimension_ids = new List<ElementId>();
            #endregion

            #region ***判断是否需要打开源文档 time_20200312
            //OpenTarDocument(uiapp, _TXA_, _TXB_, _TXC_, _TXD_, _TXA_Core_, _TXB_Core_, _TXC_Core_, _TXD_Core_);
            if (CMD._TXA_rvt != "null_path")
            {
                _TXA_ = app.OpenDocumentFile(CMD._TXA_rvt);
            }
            if (CMD._TXB_rvt != "null_path")
            {
                _TXB_ = app.OpenDocumentFile(CMD._TXB_rvt);
            }
            if (CMD._TXC_rvt != "null_path")
            {
                _TXC_ = app.OpenDocumentFile(CMD._TXC_rvt);
            }
            if (CMD._TXD_rvt != "null_path")
            {
                _TXD_ = app.OpenDocumentFile(CMD._TXD_rvt);
            }
            if (CMD._TXA_HXT_rvt != "null_path")
            {
                _TXA_Core_ = app.OpenDocumentFile(CMD._TXA_HXT_rvt);
            }
            if (CMD._TXB_HXT_rvt != "null_path")
            {
                _TXB_Core_ = app.OpenDocumentFile(CMD._TXB_HXT_rvt);
            }
            if (CMD._TXC_HXT_rvt != "null_path")
            {
                _TXC_Core_ = app.OpenDocumentFile(CMD._TXC_HXT_rvt);
            }
            if (CMD._TXD_HXT_rvt != "null_path")
            {
                _TXD_Core_ = app.OpenDocumentFile(CMD._TXD_HXT_rvt);
            }
            #endregion

            #region 根据户型组合模式 在各个源文件 定位元素 在这里不对核心筒定位做处理 原因：revit后台多次打开同一个文件，属于无效操作 time_20200312
            double distance_TXA_X = 0;
            double distance_TXB_X = 0;
            double distance_move_aaBbaa = 0;

            if (CMD._TXA_rvt != "null_path")
            {
                ICollection<ElementId> _TXA_3D_EleIds_temp = (new FilteredElementCollector(_TXA_)).WherePasses(orFilter).WhereElementIsNotElementType().ToElementIds();
                FL01_view = new FilteredElementCollector(_TXA_).OfCategory(BuiltInCategory.OST_Views).OfClass(typeof(ViewPlan)).FirstOrDefault(x => x.Name == "FL01") as View;
                ICollection<ElementId> _TXA_TextNotes_temp = (new FilteredElementCollector(_TXA_, FL01_view.Id)).OwnedByView(FL01_view.Id).OfCategory(BuiltInCategory.OST_TextNotes).WhereElementIsNotElementType().ToElementIds();
                ICollection<ElementId> _TXA_Dimensions_temp = (new FilteredElementCollector(_TXA_, FL01_view.Id)).OwnedByView(FL01_view.Id).OfCategory(BuiltInCategory.OST_Dimensions).WhereElementIsNotElementType().ToElementIds();
                //判断是否需要对基础模型进行定位操作
                double _Aabbaa_max_X;
                double _Aabbaa_min_X = GetLeftRgiht_X_ingrid(_TXA_, _TXA_3D_EleIds_temp, out _Aabbaa_max_X);
                distance_TXA_X = _Aabbaa_max_X - _Aabbaa_min_X;
            }
            if (CMD._TXB_rvt != "null_path")
            {
                ICollection<ElementId> _TXB_3D_EleIds_temp = (new FilteredElementCollector(_TXB_)).WherePasses(orFilter).WhereElementIsNotElementType().ToElementIds();
                FL01_view = new FilteredElementCollector(_TXB_).OfCategory(BuiltInCategory.OST_Views).OfClass(typeof(ViewPlan)).FirstOrDefault(x => x.Name == "FL01") as View;
                ICollection<ElementId> _TXB_TextNotes_temp = (new FilteredElementCollector(_TXB_, FL01_view.Id)).OwnedByView(FL01_view.Id).OfCategory(BuiltInCategory.OST_TextNotes).WhereElementIsNotElementType().ToElementIds();
                ICollection<ElementId> _TXB_Dimensions_temp = (new FilteredElementCollector(_TXB_, FL01_view.Id)).OwnedByView(FL01_view.Id).OfCategory(BuiltInCategory.OST_Dimensions).WhereElementIsNotElementType().ToElementIds();
                //判断是否需要对基础模型进行定位操作
                double _aaBbaa_max_X;
                double _aaBbaa_min_X = GetLeftRgiht_X_ingrid(_TXB_, _TXB_3D_EleIds_temp, out _aaBbaa_max_X);
                distance_TXB_X = _aaBbaa_max_X - _aaBbaa_min_X;

                distance_move_aaBbaa = distance_TXA_X + distance_TXB_X;

                MoveGrouIds(_TXB_, distance_move_aaBbaa, _TXB_3D_EleIds_temp);
                MoveGrouIds(_TXB_, distance_move_aaBbaa, _TXB_TextNotes_temp);

                _aaBbaa_min_X = GetLeftRgiht_X_ingrid(_TXB_, _TXB_3D_EleIds_temp, out _aaBbaa_max_X);
                mid_plane = Plane.CreateByNormalAndOrigin(new XYZ(_aaBbaa_max_X + 1, 0, 0), new XYZ(_aaBbaa_max_X, 0, 0));
            }
            if (CMD._TXC_rvt != "null_path")
            {
                //判断是否需要对基础模型进行定位操作
            }
            if (CMD._TXD_rvt != "null_path")
            {
                //判断是否需要对基础模型进行定位操作
            }
            #endregion

            using (TransactionGroup transGroupCreateDwellingses = new TransactionGroup(doc, "创建户型零件组_group"))//开展事务组
            {
                if (transGroupCreateDwellingses.Start() == TransactionStatus.Started)//开启事务组
                {
                    #region ***将定位好的基础模型数据，复制到当前工作视图 并creat group 标注元素只有list，不可以creat group time_20200312
                    if (CMD._TXA_rvt != "null_path")
                    {
                        group_TXA_ids = Creat_Unit_GroupId_copyFrom_Ori(_TXA_, doc, orFilter, actView, out group_TXA_textnote_ids, out _TXA_dimension_ids, "_TXA_", "_TXA_textnote_");
                    }
                    if (CMD._TXB_rvt != "null_path")
                    {
                        group_TXB_ids = Creat_Unit_GroupId_copyFrom_Ori(_TXB_, doc, orFilter, actView, out group_TXB_textnote_ids, out _TXB_dimension_ids, "_TXB_", "_TXB_textnote_");
                    }
                    if (CMD._TXC_rvt != "null_path")
                    {
                        group_TXC_ids = Creat_Unit_GroupId_copyFrom_Ori(_TXC_, doc, orFilter, actView, out group_TXC_textnote_ids, out _TXC_dimension_ids, "_TXC_", "_TXC_textnote_");
                    }
                    if (CMD._TXD_rvt != "null_path")
                    {
                        group_TXD_ids = Creat_Unit_GroupId_copyFrom_Ori(_TXD_, doc, orFilter, actView, out group_TXD_textnote_ids, out _TXD_dimension_ids, "_TXD_", "_TXD_textnote_");
                    }
                    if (CMD._TXA_HXT_rvt != "null_path")
                    {
                        group_TXA_Core_ids = Creat_Unit_GroupId_copyFrom_Ori(_TXA_Core_, doc, orFilter, actView, out group_TXA_Core_textnote_ids, out _TXA_Core_dimension_ids, "_TXA_Core_", "_TXA_Core_textnote_");
                    }
                    if (CMD._TXB_HXT_rvt != "null_path")
                    {
                        group_TXB_Core_ids = Creat_Unit_GroupId_copyFrom_Ori(_TXB_Core_, doc, orFilter, actView, out group_TXB_Core_textnote_ids, out _TXB_Core_dimension_ids, "_TXB_Core_", "_TXB_Core_textnote_");
                    }
                    if (CMD._TXC_HXT_rvt != "null_path")
                    {
                        group_TXC_Core_ids = Creat_Unit_GroupId_copyFrom_Ori(_TXC_Core_, doc, orFilter, actView, out group_TXC_Core_textnote_ids, out _TXC_Core_dimension_ids, "_TXC_Core_", "_TXC_Core_textnote_");
                    }
                    if (CMD._TXD_HXT_rvt != "null_path")
                    {
                        group_TXD_Core_ids = Creat_Unit_GroupId_copyFrom_Ori(_TXD_Core_, doc, orFilter, actView, out group_TXD_Core_textnote_ids, out _TXD_Core_dimension_ids, "_TXD_Core_", "_TXD_Core_textnote_");
                    }
                    #endregion

                    #region 在当前视图，对户型进行排列组合 注意模型复制到当前文档时，所处的位置 time_20200312
                    //abccbd 根据字母组合进行判断

                    if (CMD._TXA_rvt != "null_path")
                    {
                        //判断是否需要对复制进来的模型数据进行二次操作
                        ICollection<ElementId> mi_group_TXA_ids = _MirrorElements_ChangeHandler(doc, group_TXA_ids, ori_plane, true);
                        ICollection<ElementId> mi_group_TXA_textnote_ids = _MirrorElements_ChangeHandler(doc, group_TXA_textnote_ids, ori_plane, true);
                        ICollection<ElementId> mi_TXA_dimension_ids = _MirrorElements_ChangeHandler(doc, _TXA_dimension_ids, ori_plane, true);

                        ICollection<ElementId> __mi_group_TXA_ids = _MirrorElements_ChangeHandler(doc, group_TXA_ids, mid_plane, true);
                        ICollection<ElementId> __mi_group_TXA_textnote_ids = _MirrorElements_ChangeHandler(doc, group_TXA_textnote_ids, mid_plane, true);
                        ICollection<ElementId> __mi_TXA_dimension_ids = _MirrorElements_ChangeHandler(doc, _TXA_dimension_ids, mid_plane, true);

                        ICollection<ElementId> _mi_group_TXA_ids = _MirrorElements_ChangeHandler(doc, mi_group_TXA_ids, mid_plane, true);
                        ICollection<ElementId> _mi_group_TXA_textnote_ids = _MirrorElements_ChangeHandler(doc, mi_group_TXA_textnote_ids, mid_plane, true);
                        ICollection<ElementId> _mi_TXA_dimension_ids = _MirrorElements_ChangeHandler(doc, mi_TXA_dimension_ids, mid_plane, true);
                    }
                    if (CMD._TXB_rvt != "null_path")
                    {
                        //判断是否需要对复制进来的模型数据进行二次操作
                        ICollection<ElementId> mi_group_TXB_ids = _MirrorElements_ChangeHandler(doc, group_TXB_ids, mid_plane, true);
                        ICollection<ElementId> mi_group_TXB_textnote_ids = _MirrorElements_ChangeHandler(doc, group_TXB_textnote_ids, mid_plane, true);
                        ICollection<ElementId> mi_TXB_dimension_ids = _MirrorElements_ChangeHandler(doc, _TXB_dimension_ids, mid_plane, true);
                    }
                    if (CMD._TXC_rvt != "null_path")
                    {
                        //判断是否需要对复制进来的模型数据进行二次操作
                    }
                    if (CMD._TXD_rvt != "null_path")
                    {
                        //判断是否需要对复制进来的模型数据进行二次操作
                    }
                    if (CMD._TXA_HXT_rvt != "null_path")
                    {
                        //判断是否需要对复制进来的模型数据进行二次操作
                        ICollection<ElementId> mi_group_TXA_Core_ids = _MirrorElements_ChangeHandler(doc, group_TXA_Core_ids, mid_plane, true);
                        ICollection<ElementId> mi_group_TXA_Core_textnote_ids = _MirrorElements_ChangeHandler(doc, group_TXA_Core_textnote_ids, mid_plane, true);
                    }
                    if (CMD._TXB_HXT_rvt != "null_path")
                    {
                        //判断是否需要对复制进来的模型数据进行二次操作
                        MoveGrouIds(doc, distance_move_aaBbaa, group_TXB_Core_ids);
                        MoveGrouIds(doc, distance_move_aaBbaa, group_TXB_Core_textnote_ids);
                    }
                    if (CMD._TXC_HXT_rvt != "null_path")
                    {
                        //判断是否需要对复制进来的模型数据进行二次操作
                    }
                    if (CMD._TXD_HXT_rvt != "null_path")
                    {
                        //判断是否需要对复制进来的模型数据进行二次操作
                    }
                    #endregion
                    transGroupCreateDwellingses.Assimilate();
                }
                else
                {
                    transGroupCreateDwellingses.RollBack();
                }
            }
            #region ***关闭所有源文档，但是不保存  time_20200312
            ResetDocument(_TXA_, _TXB_, _TXC_, _TXD_, _TXA_Core_, _TXB_Core_, _TXC_Core_, _TXD_Core_);
            #endregion
            #endregion
        }
        //新建户型_1T2H_abbbba_3DY
        public void Creat_HXZH_1T2H_abbbba_3DY_AB_(UIApplication uiapp)
        {
            #region all code
            #region ***初始化与revit模型进行交互的辅助变量 time_20200312
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Application app = uiapp.Application;
            Document doc = uidoc.Document;
            View actView = doc.ActiveView;
            View FL01_view = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Views).OfClass(typeof(ViewPlan)).FirstOrDefault(x => x.Name == "FL01") as View;
            Plane ori_plane = Plane.CreateByNormalAndOrigin(new XYZ(1, 0, 0), new XYZ(0, 0, 0));//该处为原点镜像轴
            Plane mid_plane = null;//该处mid定义为整体户型组合模式的中心对称轴位置
            LogicalOrFilter orFilter = CreatorFilter(uiapp);
            #endregion

            #region ***声明 all 变量 time_20200312
            Document _TXA_ = null;
            Document _TXB_ = null;
            Document _TXC_ = null;
            Document _TXD_ = null;
            Document _TXA_Core_ = null;
            Document _TXB_Core_ = null;
            Document _TXC_Core_ = null;
            Document _TXD_Core_ = null;
            ICollection<ElementId> group_TXA_ids = new List<ElementId>();//各个基础模型3D元素group的id_list
            ICollection<ElementId> group_TXB_ids = new List<ElementId>();
            ICollection<ElementId> group_TXC_ids = new List<ElementId>();
            ICollection<ElementId> group_TXD_ids = new List<ElementId>();
            ICollection<ElementId> group_TXA_Core_ids = new List<ElementId>();
            ICollection<ElementId> group_TXB_Core_ids = new List<ElementId>();
            ICollection<ElementId> group_TXC_Core_ids = new List<ElementId>();
            ICollection<ElementId> group_TXD_Core_ids = new List<ElementId>();
            ICollection<ElementId> group_TXA_textnote_ids = new List<ElementId>();//各个基础模型2D文字注释group的id_list
            ICollection<ElementId> group_TXB_textnote_ids = new List<ElementId>();
            ICollection<ElementId> group_TXC_textnote_ids = new List<ElementId>();
            ICollection<ElementId> group_TXD_textnote_ids = new List<ElementId>();
            ICollection<ElementId> group_TXA_Core_textnote_ids = new List<ElementId>();
            ICollection<ElementId> group_TXB_Core_textnote_ids = new List<ElementId>();
            ICollection<ElementId> group_TXC_Core_textnote_ids = new List<ElementId>();
            ICollection<ElementId> group_TXD_Core_textnote_ids = new List<ElementId>();
            ICollection<ElementId> _TXA_dimension_ids = new List<ElementId>();//由于2D标注元素，不可以成group，所以此处为各个标注元素的list
            ICollection<ElementId> _TXB_dimension_ids = new List<ElementId>();
            ICollection<ElementId> _TXC_dimension_ids = new List<ElementId>();
            ICollection<ElementId> _TXD_dimension_ids = new List<ElementId>();
            ICollection<ElementId> _TXA_Core_dimension_ids = new List<ElementId>();//注意，目前核心筒基础模型，不涉及标注元素,此外初始化，是为了满足函数参数的空值调用
            ICollection<ElementId> _TXB_Core_dimension_ids = new List<ElementId>();
            ICollection<ElementId> _TXC_Core_dimension_ids = new List<ElementId>();
            ICollection<ElementId> _TXD_Core_dimension_ids = new List<ElementId>();
            #endregion

            #region ***判断是否需要打开源文档 time_20200312
            //OpenTarDocument(uiapp, _TXA_, _TXB_, _TXC_, _TXD_, _TXA_Core_, _TXB_Core_, _TXC_Core_, _TXD_Core_);
            if (CMD._TXA_rvt != "null_path")
            {
                _TXA_ = app.OpenDocumentFile(CMD._TXA_rvt);
            }
            if (CMD._TXB_rvt != "null_path")
            {
                _TXB_ = app.OpenDocumentFile(CMD._TXB_rvt);
            }
            if (CMD._TXC_rvt != "null_path")
            {
                _TXC_ = app.OpenDocumentFile(CMD._TXC_rvt);
            }
            if (CMD._TXD_rvt != "null_path")
            {
                _TXD_ = app.OpenDocumentFile(CMD._TXD_rvt);
            }
            if (CMD._TXA_HXT_rvt != "null_path")
            {
                _TXA_Core_ = app.OpenDocumentFile(CMD._TXA_HXT_rvt);
            }
            if (CMD._TXB_HXT_rvt != "null_path")
            {
                _TXB_Core_ = app.OpenDocumentFile(CMD._TXB_HXT_rvt);
            }
            if (CMD._TXC_HXT_rvt != "null_path")
            {
                _TXC_Core_ = app.OpenDocumentFile(CMD._TXC_HXT_rvt);
            }
            if (CMD._TXD_HXT_rvt != "null_path")
            {
                _TXD_Core_ = app.OpenDocumentFile(CMD._TXD_HXT_rvt);
            }
            #endregion

            #region 根据户型组合模式 在各个源文件 定位元素 在这里不对核心筒定位做处理 原因：revit后台多次打开同一个文件，属于无效操作 time_20200312
            double distance_TXB_X = 0;
            Plane _mid_plane = null;

            if (CMD._TXA_rvt != "null_path")
            {
                //判断是否需要对基础模型进行定位操作
            }
            if (CMD._TXB_rvt != "null_path")
            {
                ICollection<ElementId> _TXB_3D_EleIds_temp = (new FilteredElementCollector(_TXB_)).WherePasses(orFilter).WhereElementIsNotElementType().ToElementIds();
                FL01_view = new FilteredElementCollector(_TXB_).OfCategory(BuiltInCategory.OST_Views).OfClass(typeof(ViewPlan)).FirstOrDefault(x => x.Name == "FL01") as View;
                ICollection<ElementId> _TXB_TextNotes_temp = (new FilteredElementCollector(_TXB_, FL01_view.Id)).OwnedByView(FL01_view.Id).OfCategory(BuiltInCategory.OST_TextNotes).WhereElementIsNotElementType().ToElementIds();
                ICollection<ElementId> _TXB_Dimensions_temp = (new FilteredElementCollector(_TXB_, FL01_view.Id)).OwnedByView(FL01_view.Id).OfCategory(BuiltInCategory.OST_Dimensions).WhereElementIsNotElementType().ToElementIds();
                //判断是否需要对基础模型进行定位操作
                ICollection<ElementId> mi_TXB_3D_EleIds_temp = _MirrorElements_ChangeHandler(_TXB_, _TXB_3D_EleIds_temp, ori_plane, true);
                ICollection<ElementId> mi_TXB_TextNotes_temp = _MirrorElements_ChangeHandler(_TXB_, _TXB_TextNotes_temp, ori_plane, true);
                ICollection<ElementId> mi_TXB_Dimensions_temp = _MirrorElements_ChangeHandler(_TXB_, _TXB_Dimensions_temp, ori_plane, true);
                _deleteEles(_TXB_, _TXB_3D_EleIds_temp);
                _deleteEles(_TXB_, _TXB_TextNotes_temp);

                double _aBbbba_max_X;
                double _aBbbba_min_X = GetLeftRgiht_X_ingrid(_TXB_, mi_TXB_3D_EleIds_temp, out _aBbbba_max_X);
                distance_TXB_X = _aBbbba_max_X - _aBbbba_min_X;

                mid_plane = Plane.CreateByNormalAndOrigin(new XYZ(_aBbbba_max_X + 1, 0, 0), new XYZ(_aBbbba_max_X, 0, 0));
                _mid_plane = Plane.CreateByNormalAndOrigin(new XYZ(distance_TXB_X * 2 + 1, 0, 0), new XYZ(distance_TXB_X * 2, 0, 0));
            }
            if (CMD._TXC_rvt != "null_path")
            {
                //判断是否需要对基础模型进行定位操作
            }
            if (CMD._TXD_rvt != "null_path")
            {
                //判断是否需要对基础模型进行定位操作
            }
            #endregion

            using (TransactionGroup transGroupCreateDwellingses = new TransactionGroup(doc, "创建户型零件组_group"))//开展事务组
            {
                if (transGroupCreateDwellingses.Start() == TransactionStatus.Started)//开启事务组
                {
                    #region ***将定位好的基础模型数据，复制到当前工作视图 并creat group 标注元素只有list，不可以creat group time_20200312
                    if (CMD._TXA_rvt != "null_path")
                    {
                        group_TXA_ids = Creat_Unit_GroupId_copyFrom_Ori(_TXA_, doc, orFilter, actView, out group_TXA_textnote_ids, out _TXA_dimension_ids, "_TXA_", "_TXA_textnote_");
                    }
                    if (CMD._TXB_rvt != "null_path")
                    {
                        group_TXB_ids = Creat_Unit_GroupId_copyFrom_Ori(_TXB_, doc, orFilter, actView, out group_TXB_textnote_ids, out _TXB_dimension_ids, "_TXB_", "_TXB_textnote_");
                    }
                    if (CMD._TXC_rvt != "null_path")
                    {
                        group_TXC_ids = Creat_Unit_GroupId_copyFrom_Ori(_TXC_, doc, orFilter, actView, out group_TXC_textnote_ids, out _TXC_dimension_ids, "_TXC_", "_TXC_textnote_");
                    }
                    if (CMD._TXD_rvt != "null_path")
                    {
                        group_TXD_ids = Creat_Unit_GroupId_copyFrom_Ori(_TXD_, doc, orFilter, actView, out group_TXD_textnote_ids, out _TXD_dimension_ids, "_TXD_", "_TXD_textnote_");
                    }
                    if (CMD._TXA_HXT_rvt != "null_path")
                    {
                        group_TXA_Core_ids = Creat_Unit_GroupId_copyFrom_Ori(_TXA_Core_, doc, orFilter, actView, out group_TXA_Core_textnote_ids, out _TXA_Core_dimension_ids, "_TXA_Core_", "_TXA_Core_textnote_");
                    }
                    if (CMD._TXB_HXT_rvt != "null_path")
                    {
                        group_TXB_Core_ids = Creat_Unit_GroupId_copyFrom_Ori(_TXB_Core_, doc, orFilter, actView, out group_TXB_Core_textnote_ids, out _TXB_Core_dimension_ids, "_TXB_Core_", "_TXB_Core_textnote_");
                    }
                    if (CMD._TXC_HXT_rvt != "null_path")
                    {
                        group_TXC_Core_ids = Creat_Unit_GroupId_copyFrom_Ori(_TXC_Core_, doc, orFilter, actView, out group_TXC_Core_textnote_ids, out _TXC_Core_dimension_ids, "_TXC_Core_", "_TXC_Core_textnote_");
                    }
                    if (CMD._TXD_HXT_rvt != "null_path")
                    {
                        group_TXD_Core_ids = Creat_Unit_GroupId_copyFrom_Ori(_TXD_Core_, doc, orFilter, actView, out group_TXD_Core_textnote_ids, out _TXD_Core_dimension_ids, "_TXD_Core_", "_TXD_Core_textnote_");
                    }
                    #endregion

                    #region 在当前视图，对户型进行排列组合 注意模型复制到当前文档时，所处的位置 time_20200312
                    //abccbd 根据字母组合进行判断

                    if (CMD._TXA_rvt != "null_path")
                    {
                        //判断是否需要对复制进来的模型数据进行二次操作
                        ICollection<ElementId> mi_group_TXA_ids = _MirrorElements_ChangeHandler(doc, group_TXA_ids, _mid_plane, true);
                        ICollection<ElementId> mi_group_TXA_textnote_ids = _MirrorElements_ChangeHandler(doc, group_TXA_textnote_ids, _mid_plane, true);
                        ICollection<ElementId> mi_TXA_dimension_ids = _MirrorElements_ChangeHandler(doc, _TXA_dimension_ids, _mid_plane, true);
                    }
                    if (CMD._TXB_rvt != "null_path")
                    {
                        //判断是否需要对复制进来的模型数据进行二次操作
                        ICollection<ElementId> mi_group_TXB_ids = _MirrorElements_ChangeHandler(doc, group_TXB_ids, mid_plane, true);
                        ICollection<ElementId> mi_group_TXB_textnote_ids = _MirrorElements_ChangeHandler(doc, group_TXB_textnote_ids, mid_plane, true);
                        ICollection<ElementId> mi_TXB_dimension_ids = _MirrorElements_ChangeHandler(doc, _TXB_dimension_ids, mid_plane, true);

                        ICollection<ElementId> _mi_group_TXB_ids = _MirrorElements_ChangeHandler(doc, mi_group_TXB_ids, _mid_plane, true);
                        ICollection<ElementId> _mi_group_TXB_textnote_ids = _MirrorElements_ChangeHandler(doc, mi_group_TXB_textnote_ids, _mid_plane, true);
                        ICollection<ElementId> _mi_TXB_dimension_ids = _MirrorElements_ChangeHandler(doc, mi_TXB_dimension_ids, _mid_plane, true);

                        ICollection<ElementId> __mi_group_TXB_ids = _MirrorElements_ChangeHandler(doc, group_TXB_ids, _mid_plane, true);
                        ICollection<ElementId> __mi_group_TXB_textnote_ids = _MirrorElements_ChangeHandler(doc, group_TXB_textnote_ids, _mid_plane, true);
                        ICollection<ElementId> __mi_TXB_dimension_ids = _MirrorElements_ChangeHandler(doc, _TXB_dimension_ids, _mid_plane, true);
                    }
                    if (CMD._TXC_rvt != "null_path")
                    {
                        //判断是否需要对复制进来的模型数据进行二次操作
                    }
                    if (CMD._TXD_rvt != "null_path")
                    {
                        //判断是否需要对复制进来的模型数据进行二次操作
                    }
                    if (CMD._TXA_HXT_rvt != "null_path")
                    {
                        //判断是否需要对复制进来的模型数据进行二次操作
                        ICollection<ElementId> mi_group_TXA_Core_ids = _MirrorElements_ChangeHandler(doc, group_TXA_Core_ids, mid_plane, true);
                        ICollection<ElementId> mi_group_TXA_Core_textnote_ids = _MirrorElements_ChangeHandler(doc, group_TXA_Core_textnote_ids, mid_plane, true);

                        ICollection<ElementId> _mi_group_TXA_Core_ids = _MirrorElements_ChangeHandler(doc, group_TXA_Core_ids, _mid_plane, true);
                        ICollection<ElementId> _mi_group_TXA_Core_textnote_ids = _MirrorElements_ChangeHandler(doc, group_TXA_Core_textnote_ids, _mid_plane, true);
                    }
                    if (CMD._TXB_HXT_rvt != "null_path")
                    {
                        //判断是否需要对复制进来的模型数据进行二次操作
                    }
                    if (CMD._TXC_HXT_rvt != "null_path")
                    {
                        //判断是否需要对复制进来的模型数据进行二次操作
                    }
                    if (CMD._TXD_HXT_rvt != "null_path")
                    {
                        //判断是否需要对复制进来的模型数据进行二次操作
                    }
                    #endregion
                    transGroupCreateDwellingses.Assimilate();
                }
                else
                {
                    transGroupCreateDwellingses.RollBack();
                }
            }
            #region ***关闭所有源文档，但是不保存  time_20200312
            ResetDocument(_TXA_, _TXB_, _TXC_, _TXD_, _TXA_Core_, _TXB_Core_, _TXC_Core_, _TXD_Core_);
            #endregion
            #endregion
        }
        //新建户型_1T2H_aaaaab_3DY
        public void Creat_HXZH_1T2H_aaaaab_3DY_AB_(UIApplication uiapp)
        {
            #region all code
            #region ***初始化与revit模型进行交互的辅助变量 time_20200312
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Application app = uiapp.Application;
            Document doc = uidoc.Document;
            View actView = doc.ActiveView;
            View FL01_view = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Views).OfClass(typeof(ViewPlan)).FirstOrDefault(x => x.Name == "FL01") as View;
            Plane ori_plane = Plane.CreateByNormalAndOrigin(new XYZ(1, 0, 0), new XYZ(0, 0, 0));//该处为原点镜像轴
            Plane mid_plane = null;//该处mid定义为整体户型组合模式的中心对称轴位置
            LogicalOrFilter orFilter = CreatorFilter(uiapp);
            #endregion

            #region ***声明 all 变量 time_20200312
            Document _TXA_ = null;
            Document _TXB_ = null;
            Document _TXC_ = null;
            Document _TXD_ = null;
            Document _TXA_Core_ = null;
            Document _TXB_Core_ = null;
            Document _TXC_Core_ = null;
            Document _TXD_Core_ = null;
            ICollection<ElementId> group_TXA_ids = new List<ElementId>();//各个基础模型3D元素group的id_list
            ICollection<ElementId> group_TXB_ids = new List<ElementId>();
            ICollection<ElementId> group_TXC_ids = new List<ElementId>();
            ICollection<ElementId> group_TXD_ids = new List<ElementId>();
            ICollection<ElementId> group_TXA_Core_ids = new List<ElementId>();
            ICollection<ElementId> group_TXB_Core_ids = new List<ElementId>();
            ICollection<ElementId> group_TXC_Core_ids = new List<ElementId>();
            ICollection<ElementId> group_TXD_Core_ids = new List<ElementId>();
            ICollection<ElementId> group_TXA_textnote_ids = new List<ElementId>();//各个基础模型2D文字注释group的id_list
            ICollection<ElementId> group_TXB_textnote_ids = new List<ElementId>();
            ICollection<ElementId> group_TXC_textnote_ids = new List<ElementId>();
            ICollection<ElementId> group_TXD_textnote_ids = new List<ElementId>();
            ICollection<ElementId> group_TXA_Core_textnote_ids = new List<ElementId>();
            ICollection<ElementId> group_TXB_Core_textnote_ids = new List<ElementId>();
            ICollection<ElementId> group_TXC_Core_textnote_ids = new List<ElementId>();
            ICollection<ElementId> group_TXD_Core_textnote_ids = new List<ElementId>();
            ICollection<ElementId> _TXA_dimension_ids = new List<ElementId>();//由于2D标注元素，不可以成group，所以此处为各个标注元素的list
            ICollection<ElementId> _TXB_dimension_ids = new List<ElementId>();
            ICollection<ElementId> _TXC_dimension_ids = new List<ElementId>();
            ICollection<ElementId> _TXD_dimension_ids = new List<ElementId>();
            ICollection<ElementId> _TXA_Core_dimension_ids = new List<ElementId>();//注意，目前核心筒基础模型，不涉及标注元素,此外初始化，是为了满足函数参数的空值调用
            ICollection<ElementId> _TXB_Core_dimension_ids = new List<ElementId>();
            ICollection<ElementId> _TXC_Core_dimension_ids = new List<ElementId>();
            ICollection<ElementId> _TXD_Core_dimension_ids = new List<ElementId>();
            #endregion

            #region ***判断是否需要打开源文档 time_20200312
            //OpenTarDocument(uiapp, _TXA_, _TXB_, _TXC_, _TXD_, _TXA_Core_, _TXB_Core_, _TXC_Core_, _TXD_Core_);
            if (CMD._TXA_rvt != "null_path")
            {
                _TXA_ = app.OpenDocumentFile(CMD._TXA_rvt);
            }
            if (CMD._TXB_rvt != "null_path")
            {
                _TXB_ = app.OpenDocumentFile(CMD._TXB_rvt);
            }
            if (CMD._TXC_rvt != "null_path")
            {
                _TXC_ = app.OpenDocumentFile(CMD._TXC_rvt);
            }
            if (CMD._TXD_rvt != "null_path")
            {
                _TXD_ = app.OpenDocumentFile(CMD._TXD_rvt);
            }
            if (CMD._TXA_HXT_rvt != "null_path")
            {
                _TXA_Core_ = app.OpenDocumentFile(CMD._TXA_HXT_rvt);
            }
            if (CMD._TXB_HXT_rvt != "null_path")
            {
                _TXB_Core_ = app.OpenDocumentFile(CMD._TXB_HXT_rvt);
            }
            if (CMD._TXC_HXT_rvt != "null_path")
            {
                _TXC_Core_ = app.OpenDocumentFile(CMD._TXC_HXT_rvt);
            }
            if (CMD._TXD_HXT_rvt != "null_path")
            {
                _TXD_Core_ = app.OpenDocumentFile(CMD._TXD_HXT_rvt);
            }
            #endregion

            #region 根据户型组合模式 在各个源文件 定位元素 在这里不对核心筒定位做处理 原因：revit后台多次打开同一个文件，属于无效操作 time_20200312
            double distance_TXA_X = 0;
            Plane _mid_plane = null;

            if (CMD._TXA_rvt != "null_path")
            {
                ICollection<ElementId> _TXA_3D_EleIds_temp = (new FilteredElementCollector(_TXA_)).WherePasses(orFilter).WhereElementIsNotElementType().ToElementIds();
                FL01_view = new FilteredElementCollector(_TXA_).OfCategory(BuiltInCategory.OST_Views).OfClass(typeof(ViewPlan)).FirstOrDefault(x => x.Name == "FL01") as View;
                ICollection<ElementId> _TXA_TextNotes_temp = (new FilteredElementCollector(_TXA_, FL01_view.Id)).OwnedByView(FL01_view.Id).OfCategory(BuiltInCategory.OST_TextNotes).WhereElementIsNotElementType().ToElementIds();
                ICollection<ElementId> _TXA_Dimensions_temp = (new FilteredElementCollector(_TXA_, FL01_view.Id)).OwnedByView(FL01_view.Id).OfCategory(BuiltInCategory.OST_Dimensions).WhereElementIsNotElementType().ToElementIds();
                //判断是否需要对基础模型进行定位操作
                double _Aabbaa_max_X;
                double _Aabbaa_min_X = GetLeftRgiht_X_ingrid(_TXA_, _TXA_3D_EleIds_temp, out _Aabbaa_max_X);
                distance_TXA_X = _Aabbaa_max_X - _Aabbaa_min_X;
                mid_plane = Plane.CreateByNormalAndOrigin(new XYZ(distance_TXA_X + 1, 0, 0), new XYZ(distance_TXA_X, 0, 0));
                _mid_plane = Plane.CreateByNormalAndOrigin(new XYZ(distance_TXA_X + 1, 0, 0), new XYZ(distance_TXA_X, 0, 0));
            }
            if (CMD._TXB_rvt != "null_path")
            {
                ICollection<ElementId> _TXB_3D_EleIds_temp = (new FilteredElementCollector(_TXB_)).WherePasses(orFilter).WhereElementIsNotElementType().ToElementIds();
                FL01_view = new FilteredElementCollector(_TXB_).OfCategory(BuiltInCategory.OST_Views).OfClass(typeof(ViewPlan)).FirstOrDefault(x => x.Name == "FL01") as View;
                ICollection<ElementId> _TXB_TextNotes_temp = (new FilteredElementCollector(_TXB_, FL01_view.Id)).OwnedByView(FL01_view.Id).OfCategory(BuiltInCategory.OST_TextNotes).WhereElementIsNotElementType().ToElementIds();
                ICollection<ElementId> _TXB_Dimensions_temp = (new FilteredElementCollector(_TXB_, FL01_view.Id)).OwnedByView(FL01_view.Id).OfCategory(BuiltInCategory.OST_Dimensions).WhereElementIsNotElementType().ToElementIds();
                //判断是否需要对基础模型进行定位操作
                ICollection<ElementId> mi_TXB_3D_EleIds_temp = _MirrorElements_ChangeHandler(_TXB_, _TXB_3D_EleIds_temp, _mid_plane, true);
                ICollection<ElementId> mi_TXB_TextNotes_temp = _MirrorElements_ChangeHandler(_TXB_, _TXB_TextNotes_temp, _mid_plane, true);
                ICollection<ElementId> mi_TXB_Dimensions_temp = _MirrorElements_ChangeHandler(_TXB_, _TXB_Dimensions_temp, _mid_plane, true);
                _deleteEles(_TXB_, _TXB_3D_EleIds_temp);
                _deleteEles(_TXB_, _TXB_TextNotes_temp);
            }
            if (CMD._TXC_rvt != "null_path")
            {
                //判断是否需要对基础模型进行定位操作
            }
            if (CMD._TXD_rvt != "null_path")
            {
                //判断是否需要对基础模型进行定位操作
            }
            #endregion

            using (TransactionGroup transGroupCreateDwellingses = new TransactionGroup(doc, "创建户型零件组_group"))//开展事务组
            {
                if (transGroupCreateDwellingses.Start() == TransactionStatus.Started)//开启事务组
                {
                    #region ***将定位好的基础模型数据，复制到当前工作视图 并creat group 标注元素只有list，不可以creat group time_20200312
                    if (CMD._TXA_rvt != "null_path")
                    {
                        group_TXA_ids = Creat_Unit_GroupId_copyFrom_Ori(_TXA_, doc, orFilter, actView, out group_TXA_textnote_ids, out _TXA_dimension_ids, "_TXA_", "_TXA_textnote_");
                    }
                    if (CMD._TXB_rvt != "null_path")
                    {
                        group_TXB_ids = Creat_Unit_GroupId_copyFrom_Ori(_TXB_, doc, orFilter, actView, out group_TXB_textnote_ids, out _TXB_dimension_ids, "_TXB_", "_TXB_textnote_");
                    }
                    if (CMD._TXC_rvt != "null_path")
                    {
                        group_TXC_ids = Creat_Unit_GroupId_copyFrom_Ori(_TXC_, doc, orFilter, actView, out group_TXC_textnote_ids, out _TXC_dimension_ids, "_TXC_", "_TXC_textnote_");
                    }
                    if (CMD._TXD_rvt != "null_path")
                    {
                        group_TXD_ids = Creat_Unit_GroupId_copyFrom_Ori(_TXD_, doc, orFilter, actView, out group_TXD_textnote_ids, out _TXD_dimension_ids, "_TXD_", "_TXD_textnote_");
                    }
                    if (CMD._TXA_HXT_rvt != "null_path")
                    {
                        group_TXA_Core_ids = Creat_Unit_GroupId_copyFrom_Ori(_TXA_Core_, doc, orFilter, actView, out group_TXA_Core_textnote_ids, out _TXA_Core_dimension_ids, "_TXA_Core_", "_TXA_Core_textnote_");
                    }
                    if (CMD._TXB_HXT_rvt != "null_path")
                    {
                        group_TXB_Core_ids = Creat_Unit_GroupId_copyFrom_Ori(_TXB_Core_, doc, orFilter, actView, out group_TXB_Core_textnote_ids, out _TXB_Core_dimension_ids, "_TXB_Core_", "_TXB_Core_textnote_");
                    }
                    if (CMD._TXC_HXT_rvt != "null_path")
                    {
                        group_TXC_Core_ids = Creat_Unit_GroupId_copyFrom_Ori(_TXC_Core_, doc, orFilter, actView, out group_TXC_Core_textnote_ids, out _TXC_Core_dimension_ids, "_TXC_Core_", "_TXC_Core_textnote_");
                    }
                    if (CMD._TXD_HXT_rvt != "null_path")
                    {
                        group_TXD_Core_ids = Creat_Unit_GroupId_copyFrom_Ori(_TXD_Core_, doc, orFilter, actView, out group_TXD_Core_textnote_ids, out _TXD_Core_dimension_ids, "_TXD_Core_", "_TXD_Core_textnote_");
                    }
                    #endregion

                    #region 在当前视图，对户型进行排列组合 注意模型复制到当前文档时，所处的位置 time_20200312
                    //abccbd 根据字母组合进行判断

                    if (CMD._TXA_rvt != "null_path")
                    {
                        //判断是否需要对复制进来的模型数据进行二次操作
                        ICollection<ElementId> mi_group_TXA_ids = _MirrorElements_ChangeHandler(doc, group_TXA_ids, ori_plane, true);
                        ICollection<ElementId> mi_group_TXA_textnote_ids = _MirrorElements_ChangeHandler(doc, group_TXA_textnote_ids, ori_plane, true);
                        ICollection<ElementId> mi_TXA_dimension_ids = _MirrorElements_ChangeHandler(doc, _TXA_dimension_ids, ori_plane, true);

                        ICollection<ElementId> _mi_group_TXA_ids = _MirrorElements_ChangeHandler(doc, mi_group_TXA_ids, mid_plane, true);
                        ICollection<ElementId> _mi_group_TXA_textnote_ids = _MirrorElements_ChangeHandler(doc, mi_group_TXA_textnote_ids, mid_plane, true);
                        ICollection<ElementId> _mi_TXA_dimension_ids = _MirrorElements_ChangeHandler(doc, mi_TXA_dimension_ids, mid_plane, true);

                        ICollection<ElementId> __mi_group_TXA_ids = _MirrorElements_ChangeHandler(doc, _mi_group_TXA_ids, _mid_plane, true);
                        ICollection<ElementId> __mi_group_TXA_textnote_ids = _MirrorElements_ChangeHandler(doc, _mi_group_TXA_textnote_ids, _mid_plane, true);
                        ICollection<ElementId> __mi_TXA_dimension_ids = _MirrorElements_ChangeHandler(doc, _mi_TXA_dimension_ids, _mid_plane, true);

                        ICollection<ElementId> ___mi_group_TXA_ids = _MirrorElements_ChangeHandler(doc, mi_group_TXA_ids, _mid_plane, true);
                        ICollection<ElementId> ___mi_group_TXA_textnote_ids = _MirrorElements_ChangeHandler(doc, mi_group_TXA_textnote_ids, _mid_plane, true);
                        ICollection<ElementId> ___mi_TXA_dimension_ids = _MirrorElements_ChangeHandler(doc, mi_TXA_dimension_ids, _mid_plane, true);
                    }
                    if (CMD._TXB_rvt != "null_path")
                    {
                        //判断是否需要对复制进来的模型数据进行二次操作
                    }
                    if (CMD._TXC_rvt != "null_path")
                    {
                        //判断是否需要对复制进来的模型数据进行二次操作
                    }
                    if (CMD._TXD_rvt != "null_path")
                    {
                        //判断是否需要对复制进来的模型数据进行二次操作
                    }
                    if (CMD._TXA_HXT_rvt != "null_path")
                    {
                        //判断是否需要对复制进来的模型数据进行二次操作
                        ICollection<ElementId> mi_group_TXA_Core_ids = _MirrorElements_ChangeHandler(doc, group_TXA_Core_ids, mid_plane, true);
                        ICollection<ElementId> mi_group_TXA_Core_textnote_ids = _MirrorElements_ChangeHandler(doc, group_TXA_Core_textnote_ids, mid_plane, true);

                        ICollection<ElementId> _mi_group_TXA_Core_ids = _MirrorElements_ChangeHandler(doc, group_TXA_Core_ids, _mid_plane, true);
                        ICollection<ElementId> _mi_group_TXA_Core_textnote_ids = _MirrorElements_ChangeHandler(doc, group_TXA_Core_textnote_ids, _mid_plane, true);
                    }
                    if (CMD._TXB_HXT_rvt != "null_path")
                    {
                        //判断是否需要对复制进来的模型数据进行二次操作
                    }
                    if (CMD._TXC_HXT_rvt != "null_path")
                    {
                        //判断是否需要对复制进来的模型数据进行二次操作
                    }
                    if (CMD._TXD_HXT_rvt != "null_path")
                    {
                        //判断是否需要对复制进来的模型数据进行二次操作
                    }
                    #endregion
                    transGroupCreateDwellingses.Assimilate();
                }
                else
                {
                    transGroupCreateDwellingses.RollBack();
                }
            }
            #region ***关闭所有源文档，但是不保存  time_20200312
            ResetDocument(_TXA_, _TXB_, _TXC_, _TXD_, _TXA_Core_, _TXB_Core_, _TXC_Core_, _TXD_Core_);
            #endregion
            #endregion
        }
        //新建户型_1T2H_abbc_2DY
        public void Creat_HXZH_1T2H_abbc_2DY_ABC_(UIApplication uiapp)
        {
            #region all code
            #region ***初始化与revit模型进行交互的辅助变量 time_20200312
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Application app = uiapp.Application;
            Document doc = uidoc.Document;
            View actView = doc.ActiveView;
            View FL01_view = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Views).OfClass(typeof(ViewPlan)).FirstOrDefault(x => x.Name == "FL01") as View;
            Plane ori_plane = Plane.CreateByNormalAndOrigin(new XYZ(1, 0, 0), new XYZ(0, 0, 0));//该处为原点镜像轴
            Plane mid_plane = null;//该处mid定义为整体户型组合模式的中心对称轴位置
            LogicalOrFilter orFilter = CreatorFilter(uiapp);
            #endregion

            #region ***声明 all 变量 time_20200312
            Document _TXA_ = null;
            Document _TXB_ = null;
            Document _TXC_ = null;
            Document _TXD_ = null;
            Document _TXA_Core_ = null;
            Document _TXB_Core_ = null;
            Document _TXC_Core_ = null;
            Document _TXD_Core_ = null;
            ICollection<ElementId> group_TXA_ids = new List<ElementId>();//各个基础模型3D元素group的id_list
            ICollection<ElementId> group_TXB_ids = new List<ElementId>();
            ICollection<ElementId> group_TXC_ids = new List<ElementId>();
            ICollection<ElementId> group_TXD_ids = new List<ElementId>();
            ICollection<ElementId> group_TXA_Core_ids = new List<ElementId>();
            ICollection<ElementId> group_TXB_Core_ids = new List<ElementId>();
            ICollection<ElementId> group_TXC_Core_ids = new List<ElementId>();
            ICollection<ElementId> group_TXD_Core_ids = new List<ElementId>();
            ICollection<ElementId> group_TXA_textnote_ids = new List<ElementId>();//各个基础模型2D文字注释group的id_list
            ICollection<ElementId> group_TXB_textnote_ids = new List<ElementId>();
            ICollection<ElementId> group_TXC_textnote_ids = new List<ElementId>();
            ICollection<ElementId> group_TXD_textnote_ids = new List<ElementId>();
            ICollection<ElementId> group_TXA_Core_textnote_ids = new List<ElementId>();
            ICollection<ElementId> group_TXB_Core_textnote_ids = new List<ElementId>();
            ICollection<ElementId> group_TXC_Core_textnote_ids = new List<ElementId>();
            ICollection<ElementId> group_TXD_Core_textnote_ids = new List<ElementId>();
            ICollection<ElementId> _TXA_dimension_ids = new List<ElementId>();//由于2D标注元素，不可以成group，所以此处为各个标注元素的list
            ICollection<ElementId> _TXB_dimension_ids = new List<ElementId>();
            ICollection<ElementId> _TXC_dimension_ids = new List<ElementId>();
            ICollection<ElementId> _TXD_dimension_ids = new List<ElementId>();
            ICollection<ElementId> _TXA_Core_dimension_ids = new List<ElementId>();//注意，目前核心筒基础模型，不涉及标注元素,此外初始化，是为了满足函数参数的空值调用
            ICollection<ElementId> _TXB_Core_dimension_ids = new List<ElementId>();
            ICollection<ElementId> _TXC_Core_dimension_ids = new List<ElementId>();
            ICollection<ElementId> _TXD_Core_dimension_ids = new List<ElementId>();
            #endregion

            #region ***判断是否需要打开源文档 time_20200312
            //OpenTarDocument(uiapp, _TXA_, _TXB_, _TXC_, _TXD_, _TXA_Core_, _TXB_Core_, _TXC_Core_, _TXD_Core_);
            if (CMD._TXA_rvt != "null_path")
            {
                _TXA_ = app.OpenDocumentFile(CMD._TXA_rvt);
            }
            if (CMD._TXB_rvt != "null_path")
            {
                _TXB_ = app.OpenDocumentFile(CMD._TXB_rvt);
            }
            if (CMD._TXC_rvt != "null_path")
            {
                _TXC_ = app.OpenDocumentFile(CMD._TXC_rvt);
            }
            if (CMD._TXD_rvt != "null_path")
            {
                _TXD_ = app.OpenDocumentFile(CMD._TXD_rvt);
            }
            if (CMD._TXA_HXT_rvt != "null_path")
            {
                _TXA_Core_ = app.OpenDocumentFile(CMD._TXA_HXT_rvt);
            }
            if (CMD._TXB_HXT_rvt != "null_path")
            {
                _TXB_Core_ = app.OpenDocumentFile(CMD._TXB_HXT_rvt);
            }
            if (CMD._TXC_HXT_rvt != "null_path")
            {
                _TXC_Core_ = app.OpenDocumentFile(CMD._TXC_HXT_rvt);
            }
            if (CMD._TXD_HXT_rvt != "null_path")
            {
                _TXD_Core_ = app.OpenDocumentFile(CMD._TXD_HXT_rvt);
            }
            #endregion

            #region 根据户型组合模式 在各个源文件 定位元素 在这里不对核心筒定位做处理 原因：revit后台多次打开同一个文件，属于无效操作 time_20200312

            if (CMD._TXA_rvt != "null_path")
            {
                //判断是否需要对基础模型进行定位操作
            }
            if (CMD._TXB_rvt != "null_path")
            {
                ICollection<ElementId> _TXB_3D_EleIds_temp = (new FilteredElementCollector(_TXB_)).WherePasses(orFilter).WhereElementIsNotElementType().ToElementIds();
                FL01_view = new FilteredElementCollector(_TXB_).OfCategory(BuiltInCategory.OST_Views).OfClass(typeof(ViewPlan)).FirstOrDefault(x => x.Name == "FL01") as View;
                ICollection<ElementId> _TXB_TextNotes_temp = (new FilteredElementCollector(_TXB_, FL01_view.Id)).OwnedByView(FL01_view.Id).OfCategory(BuiltInCategory.OST_TextNotes).WhereElementIsNotElementType().ToElementIds();
                ICollection<ElementId> _TXB_Dimensions_temp = (new FilteredElementCollector(_TXB_, FL01_view.Id)).OwnedByView(FL01_view.Id).OfCategory(BuiltInCategory.OST_Dimensions).WhereElementIsNotElementType().ToElementIds();
                //判断是否需要对基础模型进行定位操作
                ICollection<ElementId> mi_TXB_3D_EleIds_temp = _MirrorElements_ChangeHandler(_TXB_, _TXB_3D_EleIds_temp, ori_plane, true);
                ICollection<ElementId> mi_TXB_TextNotes_temp = _MirrorElements_ChangeHandler(_TXB_, _TXB_TextNotes_temp, ori_plane, true);
                ICollection<ElementId> mi_TXB_Dimensions_temp = _MirrorElements_ChangeHandler(_TXB_, _TXB_Dimensions_temp, ori_plane, true);
                _deleteEles(_TXB_, _TXB_3D_EleIds_temp);
                _deleteEles(_TXB_, _TXB_TextNotes_temp);

                double _aBbc_max_X;
                double _aBbc_min_X = GetLeftRgiht_X_ingrid(_TXB_, mi_TXB_3D_EleIds_temp, out _aBbc_max_X);
                mid_plane = Plane.CreateByNormalAndOrigin(new XYZ(_aBbc_max_X + 1, 0, 0), new XYZ(_aBbc_max_X, 0, 0));
            }
            if (CMD._TXC_rvt != "null_path")
            {
                ICollection<ElementId> _TXC_3D_EleIds_temp = (new FilteredElementCollector(_TXC_)).WherePasses(orFilter).WhereElementIsNotElementType().ToElementIds();
                FL01_view = new FilteredElementCollector(_TXC_).OfCategory(BuiltInCategory.OST_Views).OfClass(typeof(ViewPlan)).FirstOrDefault(x => x.Name == "FL01") as View;
                ICollection<ElementId> _TXC_TextNotes_temp = (new FilteredElementCollector(_TXC_, FL01_view.Id)).OwnedByView(FL01_view.Id).OfCategory(BuiltInCategory.OST_TextNotes).WhereElementIsNotElementType().ToElementIds();
                ICollection<ElementId> _TXC_Dimensions_temp = (new FilteredElementCollector(_TXC_, FL01_view.Id)).OwnedByView(FL01_view.Id).OfCategory(BuiltInCategory.OST_Dimensions).WhereElementIsNotElementType().ToElementIds();
                //判断是否需要对基础模型进行定位操作
                ICollection<ElementId> mi_TXC_3D_EleIds_temp = _MirrorElements_ChangeHandler(_TXC_, _TXC_3D_EleIds_temp, mid_plane, true);
                ICollection<ElementId> mi_TXC_TextNotes_temp = _MirrorElements_ChangeHandler(_TXC_, _TXC_TextNotes_temp, mid_plane, true);
                ICollection<ElementId> mi_TXC_Dimensions_temp = _MirrorElements_ChangeHandler(_TXC_, _TXC_Dimensions_temp, mid_plane, true);
                _deleteEles(_TXC_, _TXC_3D_EleIds_temp);
                _deleteEles(_TXC_, _TXC_TextNotes_temp);
            }
            if (CMD._TXD_rvt != "null_path")
            {
                //判断是否需要对基础模型进行定位操作
            }
            #endregion

            using (TransactionGroup transGroupCreateDwellingses = new TransactionGroup(doc, "创建户型零件组_group"))//开展事务组
            {
                if (transGroupCreateDwellingses.Start() == TransactionStatus.Started)//开启事务组
                {
                    #region ***将定位好的基础模型数据，复制到当前工作视图 并creat group 标注元素只有list，不可以creat group time_20200312
                    if (CMD._TXA_rvt != "null_path")
                    {
                        group_TXA_ids = Creat_Unit_GroupId_copyFrom_Ori(_TXA_, doc, orFilter, actView, out group_TXA_textnote_ids, out _TXA_dimension_ids, "_TXA_", "_TXA_textnote_");
                    }
                    if (CMD._TXB_rvt != "null_path")
                    {
                        group_TXB_ids = Creat_Unit_GroupId_copyFrom_Ori(_TXB_, doc, orFilter, actView, out group_TXB_textnote_ids, out _TXB_dimension_ids, "_TXB_", "_TXB_textnote_");
                    }
                    if (CMD._TXC_rvt != "null_path")
                    {
                        group_TXC_ids = Creat_Unit_GroupId_copyFrom_Ori(_TXC_, doc, orFilter, actView, out group_TXC_textnote_ids, out _TXC_dimension_ids, "_TXC_", "_TXC_textnote_");
                    }
                    if (CMD._TXD_rvt != "null_path")
                    {
                        group_TXD_ids = Creat_Unit_GroupId_copyFrom_Ori(_TXD_, doc, orFilter, actView, out group_TXD_textnote_ids, out _TXD_dimension_ids, "_TXD_", "_TXD_textnote_");
                    }
                    if (CMD._TXA_HXT_rvt != "null_path")
                    {
                        group_TXA_Core_ids = Creat_Unit_GroupId_copyFrom_Ori(_TXA_Core_, doc, orFilter, actView, out group_TXA_Core_textnote_ids, out _TXA_Core_dimension_ids, "_TXA_Core_", "_TXA_Core_textnote_");
                    }
                    if (CMD._TXB_HXT_rvt != "null_path")
                    {
                        group_TXB_Core_ids = Creat_Unit_GroupId_copyFrom_Ori(_TXB_Core_, doc, orFilter, actView, out group_TXB_Core_textnote_ids, out _TXB_Core_dimension_ids, "_TXB_Core_", "_TXB_Core_textnote_");
                    }
                    if (CMD._TXC_HXT_rvt != "null_path")
                    {
                        group_TXC_Core_ids = Creat_Unit_GroupId_copyFrom_Ori(_TXC_Core_, doc, orFilter, actView, out group_TXC_Core_textnote_ids, out _TXC_Core_dimension_ids, "_TXC_Core_", "_TXC_Core_textnote_");
                    }
                    if (CMD._TXD_HXT_rvt != "null_path")
                    {
                        group_TXD_Core_ids = Creat_Unit_GroupId_copyFrom_Ori(_TXD_Core_, doc, orFilter, actView, out group_TXD_Core_textnote_ids, out _TXD_Core_dimension_ids, "_TXD_Core_", "_TXD_Core_textnote_");
                    }
                    #endregion

                    #region 在当前视图，对户型进行排列组合 注意模型复制到当前文档时，所处的位置 time_20200312
                    //abccbd 根据字母组合进行判断

                    if (CMD._TXA_rvt != "null_path")
                    {
                        //判断是否需要对复制进来的模型数据进行二次操作
                    }
                    if (CMD._TXB_rvt != "null_path")
                    {
                        //判断是否需要对复制进来的模型数据进行二次操作
                        ICollection<ElementId> mi_group_TXB_ids = _MirrorElements_ChangeHandler(doc, group_TXB_ids, mid_plane, true);
                        ICollection<ElementId> mi_group_TXB_textnote_ids = _MirrorElements_ChangeHandler(doc, group_TXB_textnote_ids, mid_plane, true);
                        ICollection<ElementId> mi_TXB_dimension_ids = _MirrorElements_ChangeHandler(doc, _TXB_dimension_ids, mid_plane, true);
                    }
                    if (CMD._TXC_rvt != "null_path")
                    {
                        //判断是否需要对复制进来的模型数据进行二次操作
                    }
                    if (CMD._TXD_rvt != "null_path")
                    {
                        //判断是否需要对复制进来的模型数据进行二次操作
                    }
                    if (CMD._TXA_HXT_rvt != "null_path")
                    {
                        //判断是否需要对复制进来的模型数据进行二次操作
                        ICollection<ElementId> mi_group_TXA_Core_ids = _MirrorElements_ChangeHandler(doc, group_TXA_Core_ids, mid_plane, true);
                        ICollection<ElementId> mi_group_TXA_Core_textnote_ids = _MirrorElements_ChangeHandler(doc, group_TXA_Core_textnote_ids, mid_plane, true);
                    }
                    if (CMD._TXB_HXT_rvt != "null_path")
                    {
                        //判断是否需要对复制进来的模型数据进行二次操作
                    }
                    if (CMD._TXC_HXT_rvt != "null_path")
                    {
                        //判断是否需要对复制进来的模型数据进行二次操作
                    }
                    if (CMD._TXD_HXT_rvt != "null_path")
                    {
                        //判断是否需要对复制进来的模型数据进行二次操作
                    }
                    #endregion
                    transGroupCreateDwellingses.Assimilate();
                }
                else
                {
                    transGroupCreateDwellingses.RollBack();
                }
            }
            #region ***关闭所有源文档，但是不保存  time_20200312
            ResetDocument(_TXA_, _TXB_, _TXC_, _TXD_, _TXA_Core_, _TXB_Core_, _TXC_Core_, _TXD_Core_);
            #endregion
            #endregion
        }
        //新建户型_1T2H_aabbac_3DY
        public void Creat_HXZH_1T2H_aabbac_3DY_ABC_(UIApplication uiapp)
        {
            #region all code
            #region ***初始化与revit模型进行交互的辅助变量 time_20200312
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Application app = uiapp.Application;
            Document doc = uidoc.Document;
            View actView = doc.ActiveView;
            View FL01_view = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Views).OfClass(typeof(ViewPlan)).FirstOrDefault(x => x.Name == "FL01") as View;
            Plane ori_plane = Plane.CreateByNormalAndOrigin(new XYZ(1, 0, 0), new XYZ(0, 0, 0));//该处为原点镜像轴
            Plane mid_plane = null;//该处mid定义为整体户型组合模式的中心对称轴位置
            LogicalOrFilter orFilter = CreatorFilter(uiapp);
            #endregion

            #region ***声明 all 变量 time_20200312
            Document _TXA_ = null;
            Document _TXB_ = null;
            Document _TXC_ = null;
            Document _TXD_ = null;
            Document _TXA_Core_ = null;
            Document _TXB_Core_ = null;
            Document _TXC_Core_ = null;
            Document _TXD_Core_ = null;
            ICollection<ElementId> group_TXA_ids = new List<ElementId>();//各个基础模型3D元素group的id_list
            ICollection<ElementId> group_TXB_ids = new List<ElementId>();
            ICollection<ElementId> group_TXC_ids = new List<ElementId>();
            ICollection<ElementId> group_TXD_ids = new List<ElementId>();
            ICollection<ElementId> group_TXA_Core_ids = new List<ElementId>();
            ICollection<ElementId> group_TXB_Core_ids = new List<ElementId>();
            ICollection<ElementId> group_TXC_Core_ids = new List<ElementId>();
            ICollection<ElementId> group_TXD_Core_ids = new List<ElementId>();
            ICollection<ElementId> group_TXA_textnote_ids = new List<ElementId>();//各个基础模型2D文字注释group的id_list
            ICollection<ElementId> group_TXB_textnote_ids = new List<ElementId>();
            ICollection<ElementId> group_TXC_textnote_ids = new List<ElementId>();
            ICollection<ElementId> group_TXD_textnote_ids = new List<ElementId>();
            ICollection<ElementId> group_TXA_Core_textnote_ids = new List<ElementId>();
            ICollection<ElementId> group_TXB_Core_textnote_ids = new List<ElementId>();
            ICollection<ElementId> group_TXC_Core_textnote_ids = new List<ElementId>();
            ICollection<ElementId> group_TXD_Core_textnote_ids = new List<ElementId>();
            ICollection<ElementId> _TXA_dimension_ids = new List<ElementId>();//由于2D标注元素，不可以成group，所以此处为各个标注元素的list
            ICollection<ElementId> _TXB_dimension_ids = new List<ElementId>();
            ICollection<ElementId> _TXC_dimension_ids = new List<ElementId>();
            ICollection<ElementId> _TXD_dimension_ids = new List<ElementId>();
            ICollection<ElementId> _TXA_Core_dimension_ids = new List<ElementId>();//注意，目前核心筒基础模型，不涉及标注元素,此外初始化，是为了满足函数参数的空值调用
            ICollection<ElementId> _TXB_Core_dimension_ids = new List<ElementId>();
            ICollection<ElementId> _TXC_Core_dimension_ids = new List<ElementId>();
            ICollection<ElementId> _TXD_Core_dimension_ids = new List<ElementId>();
            #endregion

            #region ***判断是否需要打开源文档 time_20200312
            //OpenTarDocument(uiapp, _TXA_, _TXB_, _TXC_, _TXD_, _TXA_Core_, _TXB_Core_, _TXC_Core_, _TXD_Core_);
            if (CMD._TXA_rvt != "null_path")
            {
                _TXA_ = app.OpenDocumentFile(CMD._TXA_rvt);
            }
            if (CMD._TXB_rvt != "null_path")
            {
                _TXB_ = app.OpenDocumentFile(CMD._TXB_rvt);
            }
            if (CMD._TXC_rvt != "null_path")
            {
                _TXC_ = app.OpenDocumentFile(CMD._TXC_rvt);
            }
            if (CMD._TXD_rvt != "null_path")
            {
                _TXD_ = app.OpenDocumentFile(CMD._TXD_rvt);
            }
            if (CMD._TXA_HXT_rvt != "null_path")
            {
                _TXA_Core_ = app.OpenDocumentFile(CMD._TXA_HXT_rvt);
            }
            if (CMD._TXB_HXT_rvt != "null_path")
            {
                _TXB_Core_ = app.OpenDocumentFile(CMD._TXB_HXT_rvt);
            }
            if (CMD._TXC_HXT_rvt != "null_path")
            {
                _TXC_Core_ = app.OpenDocumentFile(CMD._TXC_HXT_rvt);
            }
            if (CMD._TXD_HXT_rvt != "null_path")
            {
                _TXD_Core_ = app.OpenDocumentFile(CMD._TXD_HXT_rvt);
            }
            #endregion

            #region 根据户型组合模式 在各个源文件 定位元素 在这里不对核心筒定位做处理 原因：revit后台多次打开同一个文件，属于无效操作 time_20200312
            double distance_TXA_X = 0;
            double distance_TXB_X = 0;
            double distance_move_aabbac = 0;

            if (CMD._TXA_rvt != "null_path")
            {
                ICollection<ElementId> _TXA_3D_EleIds_temp = (new FilteredElementCollector(_TXA_)).WherePasses(orFilter).WhereElementIsNotElementType().ToElementIds();
                FL01_view = new FilteredElementCollector(_TXA_).OfCategory(BuiltInCategory.OST_Views).OfClass(typeof(ViewPlan)).FirstOrDefault(x => x.Name == "FL01") as View;
                ICollection<ElementId> _TXA_TextNotes_temp = (new FilteredElementCollector(_TXA_, FL01_view.Id)).OwnedByView(FL01_view.Id).OfCategory(BuiltInCategory.OST_TextNotes).WhereElementIsNotElementType().ToElementIds();
                ICollection<ElementId> _TXA_Dimensions_temp = (new FilteredElementCollector(_TXA_, FL01_view.Id)).OwnedByView(FL01_view.Id).OfCategory(BuiltInCategory.OST_Dimensions).WhereElementIsNotElementType().ToElementIds();
                //判断是否需要对基础模型进行定位操作
                double _Aabbaa_max_X;
                double _Aabbaa_min_X = GetLeftRgiht_X_ingrid(_TXA_, _TXA_3D_EleIds_temp, out _Aabbaa_max_X);
                distance_TXA_X = _Aabbaa_max_X - _Aabbaa_min_X;
            }
            if (CMD._TXB_rvt != "null_path")
            {
                ICollection<ElementId> _TXB_3D_EleIds_temp = (new FilteredElementCollector(_TXB_)).WherePasses(orFilter).WhereElementIsNotElementType().ToElementIds();
                FL01_view = new FilteredElementCollector(_TXB_).OfCategory(BuiltInCategory.OST_Views).OfClass(typeof(ViewPlan)).FirstOrDefault(x => x.Name == "FL01") as View;
                ICollection<ElementId> _TXB_TextNotes_temp = (new FilteredElementCollector(_TXB_, FL01_view.Id)).OwnedByView(FL01_view.Id).OfCategory(BuiltInCategory.OST_TextNotes).WhereElementIsNotElementType().ToElementIds();
                ICollection<ElementId> _TXB_Dimensions_temp = (new FilteredElementCollector(_TXB_, FL01_view.Id)).OwnedByView(FL01_view.Id).OfCategory(BuiltInCategory.OST_Dimensions).WhereElementIsNotElementType().ToElementIds();
                //判断是否需要对基础模型进行定位操作
                double _aaBbaa_max_X;
                double _aaBbaa_min_X = GetLeftRgiht_X_ingrid(_TXB_, _TXB_3D_EleIds_temp, out _aaBbaa_max_X);
                distance_TXB_X = _aaBbaa_max_X - _aaBbaa_min_X;

                distance_move_aabbac = distance_TXA_X + distance_TXB_X;

                MoveGrouIds(_TXB_, distance_move_aabbac, _TXB_3D_EleIds_temp);
                MoveGrouIds(_TXB_, distance_move_aabbac, _TXB_TextNotes_temp);

                _aaBbaa_min_X = GetLeftRgiht_X_ingrid(_TXB_, _TXB_3D_EleIds_temp, out _aaBbaa_max_X);
                mid_plane = Plane.CreateByNormalAndOrigin(new XYZ(_aaBbaa_max_X + 1, 0, 0), new XYZ(_aaBbaa_max_X, 0, 0));
            }
            if (CMD._TXC_rvt != "null_path")
            {
                ICollection<ElementId> _TXC_3D_EleIds_temp = (new FilteredElementCollector(_TXC_)).WherePasses(orFilter).WhereElementIsNotElementType().ToElementIds();
                FL01_view = new FilteredElementCollector(_TXC_).OfCategory(BuiltInCategory.OST_Views).OfClass(typeof(ViewPlan)).FirstOrDefault(x => x.Name == "FL01") as View;
                ICollection<ElementId> _TXC_TextNotes_temp = (new FilteredElementCollector(_TXC_, FL01_view.Id)).OwnedByView(FL01_view.Id).OfCategory(BuiltInCategory.OST_TextNotes).WhereElementIsNotElementType().ToElementIds();
                ICollection<ElementId> _TXC_Dimensions_temp = (new FilteredElementCollector(_TXC_, FL01_view.Id)).OwnedByView(FL01_view.Id).OfCategory(BuiltInCategory.OST_Dimensions).WhereElementIsNotElementType().ToElementIds();
                //判断是否需要对基础模型进行定位操作
                ICollection<ElementId> mi_TXC_3D_EleIds_temp = _MirrorElements_ChangeHandler(_TXC_, _TXC_3D_EleIds_temp, mid_plane, true);
                ICollection<ElementId> mi_TXC_TextNotes_temp = _MirrorElements_ChangeHandler(_TXC_, _TXC_TextNotes_temp, mid_plane, true);
                ICollection<ElementId> mi_TXC_Dimensions_temp = _MirrorElements_ChangeHandler(_TXC_, _TXC_Dimensions_temp, mid_plane, true);
                _deleteEles(_TXC_, _TXC_3D_EleIds_temp);
                _deleteEles(_TXC_, _TXC_TextNotes_temp);
            }
            if (CMD._TXD_rvt != "null_path")
            {
                //判断是否需要对基础模型进行定位操作
            }
            #endregion

            using (TransactionGroup transGroupCreateDwellingses = new TransactionGroup(doc, "创建户型零件组_group"))//开展事务组
            {
                if (transGroupCreateDwellingses.Start() == TransactionStatus.Started)//开启事务组
                {
                    #region ***将定位好的基础模型数据，复制到当前工作视图 并creat group 标注元素只有list，不可以creat group time_20200312
                    if (CMD._TXA_rvt != "null_path")
                    {
                        group_TXA_ids = Creat_Unit_GroupId_copyFrom_Ori(_TXA_, doc, orFilter, actView, out group_TXA_textnote_ids, out _TXA_dimension_ids, "_TXA_", "_TXA_textnote_");
                    }
                    if (CMD._TXB_rvt != "null_path")
                    {
                        group_TXB_ids = Creat_Unit_GroupId_copyFrom_Ori(_TXB_, doc, orFilter, actView, out group_TXB_textnote_ids, out _TXB_dimension_ids, "_TXB_", "_TXB_textnote_");
                    }
                    if (CMD._TXC_rvt != "null_path")
                    {
                        group_TXC_ids = Creat_Unit_GroupId_copyFrom_Ori(_TXC_, doc, orFilter, actView, out group_TXC_textnote_ids, out _TXC_dimension_ids, "_TXC_", "_TXC_textnote_");
                    }
                    if (CMD._TXD_rvt != "null_path")
                    {
                        group_TXD_ids = Creat_Unit_GroupId_copyFrom_Ori(_TXD_, doc, orFilter, actView, out group_TXD_textnote_ids, out _TXD_dimension_ids, "_TXD_", "_TXD_textnote_");
                    }
                    if (CMD._TXA_HXT_rvt != "null_path")
                    {
                        group_TXA_Core_ids = Creat_Unit_GroupId_copyFrom_Ori(_TXA_Core_, doc, orFilter, actView, out group_TXA_Core_textnote_ids, out _TXA_Core_dimension_ids, "_TXA_Core_", "_TXA_Core_textnote_");
                    }
                    if (CMD._TXB_HXT_rvt != "null_path")
                    {
                        group_TXB_Core_ids = Creat_Unit_GroupId_copyFrom_Ori(_TXB_Core_, doc, orFilter, actView, out group_TXB_Core_textnote_ids, out _TXB_Core_dimension_ids, "_TXB_Core_", "_TXB_Core_textnote_");
                    }
                    if (CMD._TXC_HXT_rvt != "null_path")
                    {
                        group_TXC_Core_ids = Creat_Unit_GroupId_copyFrom_Ori(_TXC_Core_, doc, orFilter, actView, out group_TXC_Core_textnote_ids, out _TXC_Core_dimension_ids, "_TXC_Core_", "_TXC_Core_textnote_");
                    }
                    if (CMD._TXD_HXT_rvt != "null_path")
                    {
                        group_TXD_Core_ids = Creat_Unit_GroupId_copyFrom_Ori(_TXD_Core_, doc, orFilter, actView, out group_TXD_Core_textnote_ids, out _TXD_Core_dimension_ids, "_TXD_Core_", "_TXD_Core_textnote_");
                    }
                    #endregion

                    #region 在当前视图，对户型进行排列组合 注意模型复制到当前文档时，所处的位置 time_20200312
                    //abccbd 根据字母组合进行判断

                    if (CMD._TXA_rvt != "null_path")
                    {
                        //判断是否需要对复制进来的模型数据进行二次操作
                        ICollection<ElementId> mi_group_TXA_ids = _MirrorElements_ChangeHandler(doc, group_TXA_ids, ori_plane, true);
                        ICollection<ElementId> mi_group_TXA_textnote_ids = _MirrorElements_ChangeHandler(doc, group_TXA_textnote_ids, ori_plane, true);
                        ICollection<ElementId> mi_TXA_dimension_ids = _MirrorElements_ChangeHandler(doc, _TXA_dimension_ids, ori_plane, true);

                        ICollection<ElementId> _mi_group_TXA_ids = _MirrorElements_ChangeHandler(doc, mi_group_TXA_ids, mid_plane, true);
                        ICollection<ElementId> _mi_group_TXA_textnote_ids = _MirrorElements_ChangeHandler(doc, mi_group_TXA_textnote_ids, mid_plane, true);
                        ICollection<ElementId> _mi_TXA_dimension_ids = _MirrorElements_ChangeHandler(doc, mi_TXA_dimension_ids, mid_plane, true);
                    }
                    if (CMD._TXB_rvt != "null_path")
                    {
                        //判断是否需要对复制进来的模型数据进行二次操作
                        ICollection<ElementId> mi_group_TXB_ids = _MirrorElements_ChangeHandler(doc, group_TXB_ids, mid_plane, true);
                        ICollection<ElementId> mi_group_TXB_textnote_ids = _MirrorElements_ChangeHandler(doc, group_TXB_textnote_ids, mid_plane, true);
                        ICollection<ElementId> mi_TXB_dimension_ids = _MirrorElements_ChangeHandler(doc, _TXB_dimension_ids, mid_plane, true);
                    }
                    if (CMD._TXC_rvt != "null_path")
                    {
                        //判断是否需要对复制进来的模型数据进行二次操作
                    }
                    if (CMD._TXD_rvt != "null_path")
                    {
                        //判断是否需要对复制进来的模型数据进行二次操作
                    }
                    if (CMD._TXA_HXT_rvt != "null_path")
                    {
                        //判断是否需要对复制进来的模型数据进行二次操作
                        ICollection<ElementId> mi_group_TXA_Core_ids = _MirrorElements_ChangeHandler(doc, group_TXA_Core_ids, mid_plane, true);
                        ICollection<ElementId> mi_group_TXA_Core_textnote_ids = _MirrorElements_ChangeHandler(doc, group_TXA_Core_textnote_ids, mid_plane, true);
                    }
                    if (CMD._TXB_HXT_rvt != "null_path")
                    {
                        //判断是否需要对复制进来的模型数据进行二次操作
                        MoveGrouIds(doc, distance_move_aabbac, group_TXB_Core_ids);
                        MoveGrouIds(doc, distance_move_aabbac, group_TXB_Core_textnote_ids);
                    }
                    if (CMD._TXC_HXT_rvt != "null_path")
                    {
                        //判断是否需要对复制进来的模型数据进行二次操作
                    }
                    if (CMD._TXD_HXT_rvt != "null_path")
                    {
                        //判断是否需要对复制进来的模型数据进行二次操作
                    }
                    #endregion
                    transGroupCreateDwellingses.Assimilate();
                }
                else
                {
                    transGroupCreateDwellingses.RollBack();
                }
            }
            #region ***关闭所有源文档，但是不保存  time_20200312
            ResetDocument(_TXA_, _TXB_, _TXC_, _TXD_, _TXA_Core_, _TXB_Core_, _TXC_Core_, _TXD_Core_);
            #endregion
            #endregion
        }
        //新建户型_1T2H_abbbbc_3DY
        public void Creat_HXZH_1T2H_abbbbc_3DY_ABC_(UIApplication uiapp)
        {
            #region all code
            #region ***初始化与revit模型进行交互的辅助变量 time_20200312
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Application app = uiapp.Application;
            Document doc = uidoc.Document;
            View actView = doc.ActiveView;
            View FL01_view = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Views).OfClass(typeof(ViewPlan)).FirstOrDefault(x => x.Name == "FL01") as View;
            Plane ori_plane = Plane.CreateByNormalAndOrigin(new XYZ(1, 0, 0), new XYZ(0, 0, 0));//该处为原点镜像轴
            Plane mid_plane = null;//该处mid定义为整体户型组合模式的中心对称轴位置
            LogicalOrFilter orFilter = CreatorFilter(uiapp);
            #endregion

            #region ***声明 all 变量 time_20200312
            Document _TXA_ = null;
            Document _TXB_ = null;
            Document _TXC_ = null;
            Document _TXD_ = null;
            Document _TXA_Core_ = null;
            Document _TXB_Core_ = null;
            Document _TXC_Core_ = null;
            Document _TXD_Core_ = null;
            ICollection<ElementId> group_TXA_ids = new List<ElementId>();//各个基础模型3D元素group的id_list
            ICollection<ElementId> group_TXB_ids = new List<ElementId>();
            ICollection<ElementId> group_TXC_ids = new List<ElementId>();
            ICollection<ElementId> group_TXD_ids = new List<ElementId>();
            ICollection<ElementId> group_TXA_Core_ids = new List<ElementId>();
            ICollection<ElementId> group_TXB_Core_ids = new List<ElementId>();
            ICollection<ElementId> group_TXC_Core_ids = new List<ElementId>();
            ICollection<ElementId> group_TXD_Core_ids = new List<ElementId>();
            ICollection<ElementId> group_TXA_textnote_ids = new List<ElementId>();//各个基础模型2D文字注释group的id_list
            ICollection<ElementId> group_TXB_textnote_ids = new List<ElementId>();
            ICollection<ElementId> group_TXC_textnote_ids = new List<ElementId>();
            ICollection<ElementId> group_TXD_textnote_ids = new List<ElementId>();
            ICollection<ElementId> group_TXA_Core_textnote_ids = new List<ElementId>();
            ICollection<ElementId> group_TXB_Core_textnote_ids = new List<ElementId>();
            ICollection<ElementId> group_TXC_Core_textnote_ids = new List<ElementId>();
            ICollection<ElementId> group_TXD_Core_textnote_ids = new List<ElementId>();
            ICollection<ElementId> _TXA_dimension_ids = new List<ElementId>();//由于2D标注元素，不可以成group，所以此处为各个标注元素的list
            ICollection<ElementId> _TXB_dimension_ids = new List<ElementId>();
            ICollection<ElementId> _TXC_dimension_ids = new List<ElementId>();
            ICollection<ElementId> _TXD_dimension_ids = new List<ElementId>();
            ICollection<ElementId> _TXA_Core_dimension_ids = new List<ElementId>();//注意，目前核心筒基础模型，不涉及标注元素,此外初始化，是为了满足函数参数的空值调用
            ICollection<ElementId> _TXB_Core_dimension_ids = new List<ElementId>();
            ICollection<ElementId> _TXC_Core_dimension_ids = new List<ElementId>();
            ICollection<ElementId> _TXD_Core_dimension_ids = new List<ElementId>();
            #endregion

            #region ***判断是否需要打开源文档 time_20200312
            //OpenTarDocument(uiapp, _TXA_, _TXB_, _TXC_, _TXD_, _TXA_Core_, _TXB_Core_, _TXC_Core_, _TXD_Core_);
            if (CMD._TXA_rvt != "null_path")
            {
                _TXA_ = app.OpenDocumentFile(CMD._TXA_rvt);
            }
            if (CMD._TXB_rvt != "null_path")
            {
                _TXB_ = app.OpenDocumentFile(CMD._TXB_rvt);
            }
            if (CMD._TXC_rvt != "null_path")
            {
                _TXC_ = app.OpenDocumentFile(CMD._TXC_rvt);
            }
            if (CMD._TXD_rvt != "null_path")
            {
                _TXD_ = app.OpenDocumentFile(CMD._TXD_rvt);
            }
            if (CMD._TXA_HXT_rvt != "null_path")
            {
                _TXA_Core_ = app.OpenDocumentFile(CMD._TXA_HXT_rvt);
            }
            if (CMD._TXB_HXT_rvt != "null_path")
            {
                _TXB_Core_ = app.OpenDocumentFile(CMD._TXB_HXT_rvt);
            }
            if (CMD._TXC_HXT_rvt != "null_path")
            {
                _TXC_Core_ = app.OpenDocumentFile(CMD._TXC_HXT_rvt);
            }
            if (CMD._TXD_HXT_rvt != "null_path")
            {
                _TXD_Core_ = app.OpenDocumentFile(CMD._TXD_HXT_rvt);
            }
            #endregion

            #region 根据户型组合模式 在各个源文件 定位元素 在这里不对核心筒定位做处理 原因：revit后台多次打开同一个文件，属于无效操作 time_20200312
            Plane _mid_plane = null;
            if (CMD._TXA_rvt != "null_path")
            {
                //判断是否需要对基础模型进行定位操作
            }
            if (CMD._TXB_rvt != "null_path")
            {
                ICollection<ElementId> _TXB_3D_EleIds_temp = (new FilteredElementCollector(_TXB_)).WherePasses(orFilter).WhereElementIsNotElementType().ToElementIds();
                FL01_view = new FilteredElementCollector(_TXB_).OfCategory(BuiltInCategory.OST_Views).OfClass(typeof(ViewPlan)).FirstOrDefault(x => x.Name == "FL01") as View;
                ICollection<ElementId> _TXB_TextNotes_temp = (new FilteredElementCollector(_TXB_, FL01_view.Id)).OwnedByView(FL01_view.Id).OfCategory(BuiltInCategory.OST_TextNotes).WhereElementIsNotElementType().ToElementIds();
                ICollection<ElementId> _TXB_Dimensions_temp = (new FilteredElementCollector(_TXB_, FL01_view.Id)).OwnedByView(FL01_view.Id).OfCategory(BuiltInCategory.OST_Dimensions).WhereElementIsNotElementType().ToElementIds();
                //判断是否需要对基础模型进行定位操作
                ICollection<ElementId> mi_TXB_3D_EleIds_temp = _MirrorElements_ChangeHandler(_TXB_, _TXB_3D_EleIds_temp, ori_plane, true);
                ICollection<ElementId> mi_TXB_TextNotes_temp = _MirrorElements_ChangeHandler(_TXB_, _TXB_TextNotes_temp, ori_plane, true);
                ICollection<ElementId> mi_TXB_Dimensions_temp = _MirrorElements_ChangeHandler(_TXB_, _TXB_Dimensions_temp, ori_plane, true);
                _deleteEles(_TXB_, _TXB_3D_EleIds_temp);
                _deleteEles(_TXB_, _TXB_TextNotes_temp);

                double _aBbbbc_max_X;
                double _aBbbbc_min_X = GetLeftRgiht_X_ingrid(_TXB_, mi_TXB_3D_EleIds_temp, out _aBbbbc_max_X);
                mid_plane = Plane.CreateByNormalAndOrigin(new XYZ(_aBbbbc_max_X + 1, 0, 0), new XYZ(_aBbbbc_max_X, 0, 0));
                _mid_plane = Plane.CreateByNormalAndOrigin(new XYZ(_aBbbbc_max_X * 2 + 1, 0, 0), new XYZ(_aBbbbc_max_X * 2, 0, 0));
            }
            if (CMD._TXC_rvt != "null_path")
            {
                ICollection<ElementId> _TXC_3D_EleIds_temp = (new FilteredElementCollector(_TXC_)).WherePasses(orFilter).WhereElementIsNotElementType().ToElementIds();
                FL01_view = new FilteredElementCollector(_TXC_).OfCategory(BuiltInCategory.OST_Views).OfClass(typeof(ViewPlan)).FirstOrDefault(x => x.Name == "FL01") as View;
                ICollection<ElementId> _TXC_TextNotes_temp = (new FilteredElementCollector(_TXC_, FL01_view.Id)).OwnedByView(FL01_view.Id).OfCategory(BuiltInCategory.OST_TextNotes).WhereElementIsNotElementType().ToElementIds();
                ICollection<ElementId> _TXC_Dimensions_temp = (new FilteredElementCollector(_TXC_, FL01_view.Id)).OwnedByView(FL01_view.Id).OfCategory(BuiltInCategory.OST_Dimensions).WhereElementIsNotElementType().ToElementIds();
                //判断是否需要对基础模型进行定位操作
                ICollection<ElementId> mi_TXC_3D_EleIds_temp = _MirrorElements_ChangeHandler(_TXC_, _TXC_3D_EleIds_temp, _mid_plane, true);
                ICollection<ElementId> mi_TXC_TextNotes_temp = _MirrorElements_ChangeHandler(_TXC_, _TXC_TextNotes_temp, _mid_plane, true);
                ICollection<ElementId> mi_TXC_Dimensions_temp = _MirrorElements_ChangeHandler(_TXC_, _TXC_Dimensions_temp, _mid_plane, true);
                _deleteEles(_TXC_, _TXC_3D_EleIds_temp);
                _deleteEles(_TXC_, _TXC_TextNotes_temp);
            }
            if (CMD._TXD_rvt != "null_path")
            {
                //判断是否需要对基础模型进行定位操作
            }
            #endregion

            using (TransactionGroup transGroupCreateDwellingses = new TransactionGroup(doc, "创建户型零件组_group"))//开展事务组
            {
                if (transGroupCreateDwellingses.Start() == TransactionStatus.Started)//开启事务组
                {
                    #region ***将定位好的基础模型数据，复制到当前工作视图 并creat group 标注元素只有list，不可以creat group time_20200312
                    if (CMD._TXA_rvt != "null_path")
                    {
                        group_TXA_ids = Creat_Unit_GroupId_copyFrom_Ori(_TXA_, doc, orFilter, actView, out group_TXA_textnote_ids, out _TXA_dimension_ids, "_TXA_", "_TXA_textnote_");
                    }
                    if (CMD._TXB_rvt != "null_path")
                    {
                        group_TXB_ids = Creat_Unit_GroupId_copyFrom_Ori(_TXB_, doc, orFilter, actView, out group_TXB_textnote_ids, out _TXB_dimension_ids, "_TXB_", "_TXB_textnote_");
                    }
                    if (CMD._TXC_rvt != "null_path")
                    {
                        group_TXC_ids = Creat_Unit_GroupId_copyFrom_Ori(_TXC_, doc, orFilter, actView, out group_TXC_textnote_ids, out _TXC_dimension_ids, "_TXC_", "_TXC_textnote_");
                    }
                    if (CMD._TXD_rvt != "null_path")
                    {
                        group_TXD_ids = Creat_Unit_GroupId_copyFrom_Ori(_TXD_, doc, orFilter, actView, out group_TXD_textnote_ids, out _TXD_dimension_ids, "_TXD_", "_TXD_textnote_");
                    }
                    if (CMD._TXA_HXT_rvt != "null_path")
                    {
                        group_TXA_Core_ids = Creat_Unit_GroupId_copyFrom_Ori(_TXA_Core_, doc, orFilter, actView, out group_TXA_Core_textnote_ids, out _TXA_Core_dimension_ids, "_TXA_Core_", "_TXA_Core_textnote_");
                    }
                    if (CMD._TXB_HXT_rvt != "null_path")
                    {
                        group_TXB_Core_ids = Creat_Unit_GroupId_copyFrom_Ori(_TXB_Core_, doc, orFilter, actView, out group_TXB_Core_textnote_ids, out _TXB_Core_dimension_ids, "_TXB_Core_", "_TXB_Core_textnote_");
                    }
                    if (CMD._TXC_HXT_rvt != "null_path")
                    {
                        group_TXC_Core_ids = Creat_Unit_GroupId_copyFrom_Ori(_TXC_Core_, doc, orFilter, actView, out group_TXC_Core_textnote_ids, out _TXC_Core_dimension_ids, "_TXC_Core_", "_TXC_Core_textnote_");
                    }
                    if (CMD._TXD_HXT_rvt != "null_path")
                    {
                        group_TXD_Core_ids = Creat_Unit_GroupId_copyFrom_Ori(_TXD_Core_, doc, orFilter, actView, out group_TXD_Core_textnote_ids, out _TXD_Core_dimension_ids, "_TXD_Core_", "_TXD_Core_textnote_");
                    }
                    #endregion

                    #region 在当前视图，对户型进行排列组合 注意模型复制到当前文档时，所处的位置 time_20200312
                    //abccbd 根据字母组合进行判断

                    if (CMD._TXA_rvt != "null_path")
                    {
                        //判断是否需要对复制进来的模型数据进行二次操作
                    }
                    if (CMD._TXB_rvt != "null_path")
                    {
                        //判断是否需要对复制进来的模型数据进行二次操作
                        ICollection<ElementId> mi_group_TXB_ids = _MirrorElements_ChangeHandler(doc, group_TXB_ids, mid_plane, true);
                        ICollection<ElementId> mi_group_TXB_textnote_ids = _MirrorElements_ChangeHandler(doc, group_TXB_textnote_ids, mid_plane, true);
                        ICollection<ElementId> mi_TXB_dimension_ids = _MirrorElements_ChangeHandler(doc, _TXB_dimension_ids, mid_plane, true);

                        ICollection<ElementId> _mi_group_TXB_ids = _MirrorElements_ChangeHandler(doc, mi_group_TXB_ids, _mid_plane, true);
                        ICollection<ElementId> _mi_group_TXB_textnote_ids = _MirrorElements_ChangeHandler(doc, mi_group_TXB_textnote_ids, _mid_plane, true);
                        ICollection<ElementId> _mi_TXB_dimension_ids = _MirrorElements_ChangeHandler(doc, mi_TXB_dimension_ids, _mid_plane, true);

                        ICollection<ElementId> __mi_group_TXB_ids = _MirrorElements_ChangeHandler(doc, group_TXB_ids, _mid_plane, true);
                        ICollection<ElementId> __mi_group_TXB_textnote_ids = _MirrorElements_ChangeHandler(doc, group_TXB_textnote_ids, _mid_plane, true);
                        ICollection<ElementId> __mi_TXB_dimension_ids = _MirrorElements_ChangeHandler(doc, _TXB_dimension_ids, _mid_plane, true);
                    }
                    if (CMD._TXC_rvt != "null_path")
                    {
                        //判断是否需要对复制进来的模型数据进行二次操作
                    }
                    if (CMD._TXD_rvt != "null_path")
                    {
                        //判断是否需要对复制进来的模型数据进行二次操作
                    }
                    if (CMD._TXA_HXT_rvt != "null_path")
                    {
                        //判断是否需要对复制进来的模型数据进行二次操作
                        ICollection<ElementId> mi_group_TXA_Core_ids = _MirrorElements_ChangeHandler(doc, group_TXA_Core_ids, mid_plane, true);
                        ICollection<ElementId> mi_group_TXA_Core_textnote_ids = _MirrorElements_ChangeHandler(doc, group_TXA_Core_textnote_ids, mid_plane, true);

                        ICollection<ElementId> _mi_group_TXA_Core_ids = _MirrorElements_ChangeHandler(doc, group_TXA_Core_ids, _mid_plane, true);
                        ICollection<ElementId> _mi_group_TXA_Core_textnote_ids = _MirrorElements_ChangeHandler(doc, group_TXA_Core_textnote_ids, _mid_plane, true);
                    }
                    if (CMD._TXB_HXT_rvt != "null_path")
                    {
                        //判断是否需要对复制进来的模型数据进行二次操作
                    }
                    if (CMD._TXC_HXT_rvt != "null_path")
                    {
                        //判断是否需要对复制进来的模型数据进行二次操作
                    }
                    if (CMD._TXD_HXT_rvt != "null_path")
                    {
                        //判断是否需要对复制进来的模型数据进行二次操作
                    }
                    #endregion
                    transGroupCreateDwellingses.Assimilate();
                }
                else
                {
                    transGroupCreateDwellingses.RollBack();
                }
            }
            #region ***关闭所有源文档，但是不保存  time_20200312
            ResetDocument(_TXA_, _TXB_, _TXC_, _TXD_, _TXA_Core_, _TXB_Core_, _TXC_Core_, _TXD_Core_);
            #endregion
            #endregion
        }
        /// <summary>
        /// 新建户型_1T2H_abccba_3DY
        /// </summary>
        /// <param name="uiapp"></param>
        public void Creat_HXZH_1T2H_abccba_3DY_ABC_(UIApplication uiapp)
        {
            #region all code
            #region ***初始化与revit模型进行交互的辅助变量 time_20200312
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Application app = uiapp.Application;
            Document doc = uidoc.Document;
            View actView = doc.ActiveView;
            View FL01_view = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Views).OfClass(typeof(ViewPlan)).FirstOrDefault(x => x.Name == "FL01") as View;
            Plane ori_plane = Plane.CreateByNormalAndOrigin(new XYZ(1, 0, 0), new XYZ(0, 0, 0));//该处为原点镜像轴
            Plane mid_plane = null;//该处mid定义为整体户型组合模式的中心对称轴位置
            LogicalOrFilter orFilter = CreatorFilter(uiapp);
            #endregion

            #region ***声明 all 变量 time_20200312
            Document _TXA_ = null;
            Document _TXB_ = null;
            Document _TXC_ = null;
            Document _TXD_ = null;
            Document _TXA_Core_ = null;
            Document _TXB_Core_ = null;
            Document _TXC_Core_ = null;
            Document _TXD_Core_ = null;
            ICollection<ElementId> group_TXA_ids = new List<ElementId>();//各个基础模型3D元素group的id_list
            ICollection<ElementId> group_TXB_ids = new List<ElementId>();
            ICollection<ElementId> group_TXC_ids = new List<ElementId>();
            ICollection<ElementId> group_TXD_ids = new List<ElementId>();
            ICollection<ElementId> group_TXA_Core_ids = new List<ElementId>();
            ICollection<ElementId> group_TXB_Core_ids = new List<ElementId>();
            ICollection<ElementId> group_TXC_Core_ids = new List<ElementId>();
            ICollection<ElementId> group_TXD_Core_ids = new List<ElementId>();
            ICollection<ElementId> group_TXA_textnote_ids = new List<ElementId>();//各个基础模型2D文字注释group的id_list
            ICollection<ElementId> group_TXB_textnote_ids = new List<ElementId>();
            ICollection<ElementId> group_TXC_textnote_ids = new List<ElementId>();
            ICollection<ElementId> group_TXD_textnote_ids = new List<ElementId>();
            ICollection<ElementId> group_TXA_Core_textnote_ids = new List<ElementId>();
            ICollection<ElementId> group_TXB_Core_textnote_ids = new List<ElementId>();
            ICollection<ElementId> group_TXC_Core_textnote_ids = new List<ElementId>();
            ICollection<ElementId> group_TXD_Core_textnote_ids = new List<ElementId>();
            ICollection<ElementId> _TXA_dimension_ids = new List<ElementId>();//由于2D标注元素，不可以成group，所以此处为各个标注元素的list
            ICollection<ElementId> _TXB_dimension_ids = new List<ElementId>();
            ICollection<ElementId> _TXC_dimension_ids = new List<ElementId>();
            ICollection<ElementId> _TXD_dimension_ids = new List<ElementId>();
            ICollection<ElementId> _TXA_Core_dimension_ids = new List<ElementId>();//注意，目前核心筒基础模型，不涉及标注元素,此外初始化，是为了满足函数参数的空值调用
            ICollection<ElementId> _TXB_Core_dimension_ids = new List<ElementId>();
            ICollection<ElementId> _TXC_Core_dimension_ids = new List<ElementId>();
            ICollection<ElementId> _TXD_Core_dimension_ids = new List<ElementId>();
            #endregion

            #region ***判断是否需要打开源文档 time_20200312
            //OpenTarDocument(uiapp, _TXA_, _TXB_, _TXC_, _TXD_, _TXA_Core_, _TXB_Core_, _TXC_Core_, _TXD_Core_);
            if (CMD._TXA_rvt != "null_path")
            {
                _TXA_ = app.OpenDocumentFile(CMD._TXA_rvt);
            }
            if (CMD._TXB_rvt != "null_path")
            {
                _TXB_ = app.OpenDocumentFile(CMD._TXB_rvt);
            }
            if (CMD._TXC_rvt != "null_path")
            {
                _TXC_ = app.OpenDocumentFile(CMD._TXC_rvt);
            }
            if (CMD._TXD_rvt != "null_path")
            {
                _TXD_ = app.OpenDocumentFile(CMD._TXD_rvt);
            }
            if (CMD._TXA_HXT_rvt != "null_path")
            {
                _TXA_Core_ = app.OpenDocumentFile(CMD._TXA_HXT_rvt);
            }
            if (CMD._TXB_HXT_rvt != "null_path")
            {
                _TXB_Core_ = app.OpenDocumentFile(CMD._TXB_HXT_rvt);
            }
            if (CMD._TXC_HXT_rvt != "null_path")
            {
                _TXC_Core_ = app.OpenDocumentFile(CMD._TXC_HXT_rvt);
            }
            if (CMD._TXD_HXT_rvt != "null_path")
            {
                _TXD_Core_ = app.OpenDocumentFile(CMD._TXD_HXT_rvt);
            }
            #endregion

            #region 根据户型组合模式 在各个源文件 定位元素 在这里不对核心筒定位做处理 原因：revit后台多次打开同一个文件，属于无效操作 time_20200312

            double distance_TXB_X = 0;
            double distance_TXC_X = 0;
            double distance_move_abCcbd = 0;

            if (CMD._TXA_rvt != "null_path")
            {
                //判断是否需要对基础模型进行定位操作
            }
            if (CMD._TXB_rvt != "null_path")
            {
                ICollection<ElementId> _TXB_3D_EleIds_temp = (new FilteredElementCollector(_TXB_)).WherePasses(orFilter).WhereElementIsNotElementType().ToElementIds();
                FL01_view = new FilteredElementCollector(_TXB_).OfCategory(BuiltInCategory.OST_Views).OfClass(typeof(ViewPlan)).FirstOrDefault(x => x.Name == "FL01") as View;
                ICollection<ElementId> _TXB_TextNotes_temp = (new FilteredElementCollector(_TXB_, FL01_view.Id)).OwnedByView(FL01_view.Id).OfCategory(BuiltInCategory.OST_TextNotes).WhereElementIsNotElementType().ToElementIds();
                ICollection<ElementId> _TXB_Dimensions_temp = (new FilteredElementCollector(_TXB_, FL01_view.Id)).OwnedByView(FL01_view.Id).OfCategory(BuiltInCategory.OST_Dimensions).WhereElementIsNotElementType().ToElementIds();
                //判断是否需要对基础模型进行定位操作

                ICollection<ElementId> mi_TXB_3D_EleIds_temp = _MirrorElements_ChangeHandler(_TXB_, _TXB_3D_EleIds_temp, ori_plane, true);
                ICollection<ElementId> mi_TXB_TextNotes_temp = _MirrorElements_ChangeHandler(_TXB_, _TXB_TextNotes_temp, ori_plane, true);
                ICollection<ElementId> mi_TXB_Dimensions_temp = _MirrorElements_ChangeHandler(_TXB_, _TXB_Dimensions_temp, ori_plane, true);
                _deleteEles(_TXB_, _TXB_3D_EleIds_temp);
                _deleteEles(_TXB_, _TXB_TextNotes_temp);
                double _aBccbd_max_X;
                double _aBccbd_min_X = GetLeftRgiht_X_ingrid(_TXB_, mi_TXB_3D_EleIds_temp, out _aBccbd_max_X);
                distance_TXB_X = _aBccbd_max_X - _aBccbd_min_X;
            }
            if (CMD._TXC_rvt != "null_path")
            {
                ICollection<ElementId> _TXC_3D_EleIds_temp = (new FilteredElementCollector(_TXC_)).WherePasses(orFilter).WhereElementIsNotElementType().ToElementIds();
                FL01_view = new FilteredElementCollector(_TXC_).OfCategory(BuiltInCategory.OST_Views).OfClass(typeof(ViewPlan)).FirstOrDefault(x => x.Name == "FL01") as View;
                ICollection<ElementId> _TXC_TextNotes_temp = (new FilteredElementCollector(_TXC_, FL01_view.Id)).OwnedByView(FL01_view.Id).OfCategory(BuiltInCategory.OST_TextNotes).WhereElementIsNotElementType().ToElementIds();
                ICollection<ElementId> _TXC_Dimensions_temp = (new FilteredElementCollector(_TXC_, FL01_view.Id)).OwnedByView(FL01_view.Id).OfCategory(BuiltInCategory.OST_Dimensions).WhereElementIsNotElementType().ToElementIds();
                //判断是否需要对基础模型进行定位操作

                double _abCcbd_max_X;
                double _abCcbd_min_X = GetLeftRgiht_X_ingrid(_TXC_, _TXC_3D_EleIds_temp, out _abCcbd_max_X);
                distance_TXC_X = _abCcbd_max_X - _abCcbd_min_X;

                distance_move_abCcbd = distance_TXB_X + distance_TXC_X;

                MoveGrouIds(_TXC_, distance_move_abCcbd, _TXC_3D_EleIds_temp);
                MoveGrouIds(_TXC_, distance_move_abCcbd, _TXC_TextNotes_temp);

                _abCcbd_min_X = GetLeftRgiht_X_ingrid(_TXC_, _TXC_3D_EleIds_temp, out _abCcbd_max_X);
                mid_plane = Plane.CreateByNormalAndOrigin(new XYZ(_abCcbd_max_X + 1, 0, 0), new XYZ(_abCcbd_max_X, 0, 0));
            }
            if (CMD._TXD_rvt != "null_path")
            {
                //判断是否需要对基础模型进行定位操作
            }
            #endregion

            using (TransactionGroup transGroupCreateDwellingses = new TransactionGroup(doc, "创建户型零件组_group"))//开展事务组
            {
                if (transGroupCreateDwellingses.Start() == TransactionStatus.Started)//开启事务组
                {
                    #region ***将定位好的基础模型数据，复制到当前工作视图 并creat group 标注元素只有list，不可以creat group time_20200312
                    if (CMD._TXA_rvt != "null_path")
                    {
                        group_TXA_ids = Creat_Unit_GroupId_copyFrom_Ori(_TXA_, doc, orFilter, actView, out group_TXA_textnote_ids, out _TXA_dimension_ids, "_TXA_", "_TXA_textnote_");
                    }
                    if (CMD._TXB_rvt != "null_path")
                    {
                        group_TXB_ids = Creat_Unit_GroupId_copyFrom_Ori(_TXB_, doc, orFilter, actView, out group_TXB_textnote_ids, out _TXB_dimension_ids, "_TXB_", "_TXB_textnote_");
                    }
                    if (CMD._TXC_rvt != "null_path")
                    {
                        group_TXC_ids = Creat_Unit_GroupId_copyFrom_Ori(_TXC_, doc, orFilter, actView, out group_TXC_textnote_ids, out _TXC_dimension_ids, "_TXC_", "_TXC_textnote_");
                    }
                    if (CMD._TXD_rvt != "null_path")
                    {
                        group_TXD_ids = Creat_Unit_GroupId_copyFrom_Ori(_TXD_, doc, orFilter, actView, out group_TXD_textnote_ids, out _TXD_dimension_ids, "_TXD_", "_TXD_textnote_");
                    }
                    if (CMD._TXA_HXT_rvt != "null_path")
                    {
                        group_TXA_Core_ids = Creat_Unit_GroupId_copyFrom_Ori(_TXA_Core_, doc, orFilter, actView, out group_TXA_Core_textnote_ids, out _TXA_Core_dimension_ids, "_TXA_Core_", "_TXA_Core_textnote_");
                    }
                    if (CMD._TXB_HXT_rvt != "null_path")
                    {
                        group_TXB_Core_ids = Creat_Unit_GroupId_copyFrom_Ori(_TXB_Core_, doc, orFilter, actView, out group_TXB_Core_textnote_ids, out _TXB_Core_dimension_ids, "_TXB_Core_", "_TXB_Core_textnote_");
                    }
                    if (CMD._TXC_HXT_rvt != "null_path")
                    {
                        group_TXC_Core_ids = Creat_Unit_GroupId_copyFrom_Ori(_TXC_Core_, doc, orFilter, actView, out group_TXC_Core_textnote_ids, out _TXC_Core_dimension_ids, "_TXC_Core_", "_TXC_Core_textnote_");
                    }
                    if (CMD._TXD_HXT_rvt != "null_path")
                    {
                        group_TXD_Core_ids = Creat_Unit_GroupId_copyFrom_Ori(_TXD_Core_, doc, orFilter, actView, out group_TXD_Core_textnote_ids, out _TXD_Core_dimension_ids, "_TXD_Core_", "_TXD_Core_textnote_");
                    }
                    #endregion

                    #region 在当前视图，对户型进行排列组合 注意模型复制到当前文档时，所处的位置 time_20200312
                    //abccbd 根据字母组合进行判断

                    if (CMD._TXA_rvt != "null_path")
                    {
                        //判断是否需要对复制进来的模型数据进行二次操作
                        ICollection<ElementId> mi_group_TXA_ids = _MirrorElements_ChangeHandler(doc, group_TXA_ids, mid_plane, true);
                        ICollection<ElementId> mi_group_TXA_textnote_ids = _MirrorElements_ChangeHandler(doc, group_TXA_textnote_ids, mid_plane, true);
                        ICollection<ElementId> mi_TXA_dimension_ids = _MirrorElements_ChangeHandler(doc, _TXA_dimension_ids, mid_plane, true);
                    }
                    if (CMD._TXB_rvt != "null_path")
                    {
                        //判断是否需要对复制进来的模型数据进行二次操作
                        ICollection<ElementId> mi_group_TXB_ids = _MirrorElements_ChangeHandler(doc, group_TXB_ids, mid_plane, true);
                        ICollection<ElementId> mi_group_TXB_textnote_ids = _MirrorElements_ChangeHandler(doc, group_TXB_textnote_ids, mid_plane, true);
                        ICollection<ElementId> mi_TXB_dimension_ids = _MirrorElements_ChangeHandler(doc, _TXB_dimension_ids, mid_plane, true);
                    }
                    if (CMD._TXC_rvt != "null_path")
                    {
                        //判断是否需要对复制进来的模型数据进行二次操作
                        ICollection<ElementId> mi_group_TXC_ids = _MirrorElements_ChangeHandler(doc, group_TXC_ids, mid_plane, true);
                        ICollection<ElementId> mi_group_TXC_textnote_ids = _MirrorElements_ChangeHandler(doc, group_TXC_textnote_ids, mid_plane, true);
                        ICollection<ElementId> mi_TXC_dimension_ids = _MirrorElements_ChangeHandler(doc, _TXC_dimension_ids, mid_plane, true);
                    }
                    if (CMD._TXD_rvt != "null_path")
                    {
                        //判断是否需要对复制进来的模型数据进行二次操作
                    }
                    if (CMD._TXA_HXT_rvt != "null_path")
                    {
                        //判断是否需要对复制进来的模型数据进行二次操作
                        ICollection<ElementId> mi_group_TXA_Core_ids = _MirrorElements_ChangeHandler(doc, group_TXA_Core_ids, mid_plane, true);
                        ICollection<ElementId> mi_group_TXA_Core_textnote_ids = _MirrorElements_ChangeHandler(doc, group_TXA_Core_textnote_ids, mid_plane, true);
                    }
                    if (CMD._TXB_HXT_rvt != "null_path")
                    {
                        //判断是否需要对复制进来的模型数据进行二次操作
                    }
                    if (CMD._TXC_HXT_rvt != "null_path")
                    {
                        //判断是否需要对复制进来的模型数据进行二次操作
                        MoveGrouIds(doc, distance_move_abCcbd, group_TXC_Core_ids);
                        MoveGrouIds(doc, distance_move_abCcbd, group_TXC_Core_ids);
                    }
                    if (CMD._TXD_HXT_rvt != "null_path")
                    {
                        //判断是否需要对复制进来的模型数据进行二次操作
                    }
                    #endregion
                    transGroupCreateDwellingses.Assimilate();
                }
                else
                {
                    transGroupCreateDwellingses.RollBack();
                }
            }
            #region ***关闭所有源文档，但是不保存  time_20200312
            ResetDocument(_TXA_, _TXB_, _TXC_, _TXD_, _TXA_Core_, _TXB_Core_, _TXC_Core_, _TXD_Core_);
            #endregion
            #endregion
        }
        /// <summary>
        /// 新建户型_1T2H_abccbd_3DY ***代码块不需要进行操作，属于各个模型调取方法的重复内容
        /// </summary>
        /// <param name="uiapp"></param>
        public void Creat_HXZH_1T2H_abccbd_3DY_ABCD_(UIApplication uiapp)
        {
            #region all code
            #region ***初始化与revit模型进行交互的辅助变量 time_20200312
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Application app = uiapp.Application;
            Document doc = uidoc.Document;
            View actView = doc.ActiveView;
            View FL01_view = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Views).OfClass(typeof(ViewPlan)).FirstOrDefault(x => x.Name == "FL01") as View;
            Plane ori_plane = Plane.CreateByNormalAndOrigin(new XYZ(1, 0, 0), new XYZ(0, 0, 0));//该处为原点镜像轴
            Plane mid_plane = null;//该处mid定义为整体户型组合模式的中心对称轴位置
            LogicalOrFilter orFilter = CreatorFilter(uiapp);
            #endregion

            #region ***声明 all 变量 time_20200312
            Document _TXA_ = null;
            Document _TXB_ = null;
            Document _TXC_ = null;
            Document _TXD_ = null;
            Document _TXA_Core_ = null;
            Document _TXB_Core_ = null;
            Document _TXC_Core_ = null;
            Document _TXD_Core_ = null;
            ICollection<ElementId> group_TXA_ids = new List<ElementId>();//各个基础模型3D元素group的id_list
            ICollection<ElementId> group_TXB_ids = new List<ElementId>();
            ICollection<ElementId> group_TXC_ids = new List<ElementId>();
            ICollection<ElementId> group_TXD_ids = new List<ElementId>();
            ICollection<ElementId> group_TXA_Core_ids = new List<ElementId>();
            ICollection<ElementId> group_TXB_Core_ids = new List<ElementId>();
            ICollection<ElementId> group_TXC_Core_ids = new List<ElementId>();
            ICollection<ElementId> group_TXD_Core_ids = new List<ElementId>();
            ICollection<ElementId> group_TXA_textnote_ids = new List<ElementId>();//各个基础模型2D文字注释group的id_list
            ICollection<ElementId> group_TXB_textnote_ids = new List<ElementId>();
            ICollection<ElementId> group_TXC_textnote_ids = new List<ElementId>();
            ICollection<ElementId> group_TXD_textnote_ids = new List<ElementId>();
            ICollection<ElementId> group_TXA_Core_textnote_ids = new List<ElementId>();
            ICollection<ElementId> group_TXB_Core_textnote_ids = new List<ElementId>();
            ICollection<ElementId> group_TXC_Core_textnote_ids = new List<ElementId>();
            ICollection<ElementId> group_TXD_Core_textnote_ids = new List<ElementId>();
            ICollection<ElementId> _TXA_dimension_ids = new List<ElementId>();//由于2D标注元素，不可以成group，所以此处为各个标注元素的list
            ICollection<ElementId> _TXB_dimension_ids = new List<ElementId>();
            ICollection<ElementId> _TXC_dimension_ids = new List<ElementId>();
            ICollection<ElementId> _TXD_dimension_ids = new List<ElementId>();
            ICollection<ElementId> _TXA_Core_dimension_ids = new List<ElementId>();//注意，目前核心筒基础模型，不涉及标注元素,此外初始化，是为了满足函数参数的空值调用
            ICollection<ElementId> _TXB_Core_dimension_ids = new List<ElementId>();
            ICollection<ElementId> _TXC_Core_dimension_ids = new List<ElementId>();
            ICollection<ElementId> _TXD_Core_dimension_ids = new List<ElementId>();
            #endregion

            #region ***判断是否需要打开源文档 time_20200312
            //OpenTarDocument(uiapp, _TXA_, _TXB_, _TXC_, _TXD_, _TXA_Core_, _TXB_Core_, _TXC_Core_, _TXD_Core_);
            if (CMD._TXA_rvt != "null_path")
            {
                _TXA_ = app.OpenDocumentFile(CMD._TXA_rvt);
            }
            if (CMD._TXB_rvt != "null_path")
            {
                _TXB_ = app.OpenDocumentFile(CMD._TXB_rvt);
            }
            if (CMD._TXC_rvt != "null_path")
            {
                _TXC_ = app.OpenDocumentFile(CMD._TXC_rvt);
            }
            if (CMD._TXD_rvt != "null_path")
            {
                _TXD_ = app.OpenDocumentFile(CMD._TXD_rvt);
            }
            if (CMD._TXA_HXT_rvt != "null_path")
            {
                _TXA_Core_ = app.OpenDocumentFile(CMD._TXA_HXT_rvt);
            }
            if (CMD._TXB_HXT_rvt != "null_path")
            {
                _TXB_Core_ = app.OpenDocumentFile(CMD._TXB_HXT_rvt);
            }
            if (CMD._TXC_HXT_rvt != "null_path")
            {
                _TXC_Core_ = app.OpenDocumentFile(CMD._TXC_HXT_rvt);
            }
            if (CMD._TXD_HXT_rvt != "null_path")
            {
                _TXD_Core_ = app.OpenDocumentFile(CMD._TXD_HXT_rvt);
            }
            #endregion

            #region 根据户型组合模式 在各个源文件 定位元素 在这里不对核心筒定位做处理 原因：revit后台多次打开同一个文件，属于无效操作 time_20200312

            double distance_TXB_X = 0;
            double distance_TXC_X = 0;
            double distance_move_abCcbd = 0;

            if (CMD._TXA_rvt != "null_path")
            {
                //判断是否需要对基础模型进行定位操作
            }
            if (CMD._TXB_rvt != "null_path")
            {
                ICollection<ElementId> _TXB_3D_EleIds_temp = (new FilteredElementCollector(_TXB_)).WherePasses(orFilter).WhereElementIsNotElementType().ToElementIds();
                FL01_view = new FilteredElementCollector(_TXB_).OfCategory(BuiltInCategory.OST_Views).OfClass(typeof(ViewPlan)).FirstOrDefault(x => x.Name == "FL01") as View;
                ICollection<ElementId> _TXB_TextNotes_temp = (new FilteredElementCollector(_TXB_, FL01_view.Id)).OwnedByView(FL01_view.Id).OfCategory(BuiltInCategory.OST_TextNotes).WhereElementIsNotElementType().ToElementIds();
                ICollection<ElementId> _TXB_Dimensions_temp = (new FilteredElementCollector(_TXB_, FL01_view.Id)).OwnedByView(FL01_view.Id).OfCategory(BuiltInCategory.OST_Dimensions).WhereElementIsNotElementType().ToElementIds();
                //判断是否需要对基础模型进行定位操作

                ICollection<ElementId> mi_TXB_3D_EleIds_temp = _MirrorElements_ChangeHandler(_TXB_, _TXB_3D_EleIds_temp, ori_plane, true);
                ICollection<ElementId> mi_TXB_TextNotes_temp = _MirrorElements_ChangeHandler(_TXB_, _TXB_TextNotes_temp, ori_plane, true);
                ICollection<ElementId> mi_TXB_Dimensions_temp = _MirrorElements_ChangeHandler(_TXB_, _TXB_Dimensions_temp, ori_plane, true);
                _deleteEles(_TXB_, _TXB_3D_EleIds_temp);
                _deleteEles(_TXB_, _TXB_TextNotes_temp);
                double _aBccbd_max_X;
                double _aBccbd_min_X = GetLeftRgiht_X_ingrid(_TXB_, mi_TXB_3D_EleIds_temp, out _aBccbd_max_X);
                distance_TXB_X = _aBccbd_max_X - _aBccbd_min_X;
            }
            if (CMD._TXC_rvt != "null_path")
            {
                ICollection<ElementId> _TXC_3D_EleIds_temp = (new FilteredElementCollector(_TXC_)).WherePasses(orFilter).WhereElementIsNotElementType().ToElementIds();
                FL01_view = new FilteredElementCollector(_TXC_).OfCategory(BuiltInCategory.OST_Views).OfClass(typeof(ViewPlan)).FirstOrDefault(x => x.Name == "FL01") as View;
                ICollection<ElementId> _TXC_TextNotes_temp = (new FilteredElementCollector(_TXC_, FL01_view.Id)).OwnedByView(FL01_view.Id).OfCategory(BuiltInCategory.OST_TextNotes).WhereElementIsNotElementType().ToElementIds();
                ICollection<ElementId> _TXC_Dimensions_temp = (new FilteredElementCollector(_TXC_, FL01_view.Id)).OwnedByView(FL01_view.Id).OfCategory(BuiltInCategory.OST_Dimensions).WhereElementIsNotElementType().ToElementIds();
                //判断是否需要对基础模型进行定位操作

                double _abCcbd_max_X;
                double _abCcbd_min_X = GetLeftRgiht_X_ingrid(_TXC_, _TXC_3D_EleIds_temp, out _abCcbd_max_X);
                distance_TXC_X = _abCcbd_max_X - _abCcbd_min_X;

                distance_move_abCcbd = distance_TXB_X + distance_TXC_X;

                MoveGrouIds(_TXC_, distance_move_abCcbd, _TXC_3D_EleIds_temp);
                MoveGrouIds(_TXC_, distance_move_abCcbd, _TXC_TextNotes_temp);

                _abCcbd_min_X = GetLeftRgiht_X_ingrid(_TXC_, _TXC_3D_EleIds_temp, out _abCcbd_max_X);
                mid_plane = Plane.CreateByNormalAndOrigin(new XYZ(_abCcbd_max_X + 1, 0, 0), new XYZ(_abCcbd_max_X, 0, 0));
            }
            if (CMD._TXD_rvt != "null_path")
            {
                ICollection<ElementId> _TXD_3D_EleIds_temp = (new FilteredElementCollector(_TXD_)).WherePasses(orFilter).WhereElementIsNotElementType().ToElementIds();
                FL01_view = new FilteredElementCollector(_TXD_).OfCategory(BuiltInCategory.OST_Views).OfClass(typeof(ViewPlan)).FirstOrDefault(x => x.Name == "FL01") as View;
                ICollection<ElementId> _TXD_TextNotes_temp = (new FilteredElementCollector(_TXD_, FL01_view.Id)).OwnedByView(FL01_view.Id).OfCategory(BuiltInCategory.OST_TextNotes).WhereElementIsNotElementType().ToElementIds();
                ICollection<ElementId> _TXD_Dimensions_temp = (new FilteredElementCollector(_TXD_, FL01_view.Id)).OwnedByView(FL01_view.Id).OfCategory(BuiltInCategory.OST_Dimensions).WhereElementIsNotElementType().ToElementIds();
                //判断是否需要对基础模型进行定位操作

                ICollection<ElementId> mi_TXD_3D_EleIds_temp = _MirrorElements_ChangeHandler(_TXD_, _TXD_3D_EleIds_temp, mid_plane, true);
                ICollection<ElementId> mi_TXD_TextNotes_temp = _MirrorElements_ChangeHandler(_TXD_, _TXD_TextNotes_temp, mid_plane, true);
                ICollection<ElementId> mi_TXD_Dimensions_temp = _MirrorElements_ChangeHandler(_TXD_, _TXD_Dimensions_temp, mid_plane, true);
                _deleteEles(_TXD_, _TXD_3D_EleIds_temp);
                _deleteEles(_TXD_, _TXD_TextNotes_temp);
            }
            #endregion

            using (TransactionGroup transGroupCreateDwellingses = new TransactionGroup(doc, "创建户型零件组_group"))//开展事务组
            {
                if (transGroupCreateDwellingses.Start() == TransactionStatus.Started)//开启事务组
                {
                    #region ***将定位好的基础模型数据，复制到当前工作视图 并creat group 标注元素只有list，不可以creat group time_20200312
                    if (CMD._TXA_rvt != "null_path")
                    {
                        group_TXA_ids = Creat_Unit_GroupId_copyFrom_Ori(_TXA_, doc, orFilter, actView, out group_TXA_textnote_ids, out _TXA_dimension_ids, "_TXA_", "_TXA_textnote_");
                    }
                    if (CMD._TXB_rvt != "null_path")
                    {
                        group_TXB_ids = Creat_Unit_GroupId_copyFrom_Ori(_TXB_, doc, orFilter, actView, out group_TXB_textnote_ids, out _TXB_dimension_ids, "_TXB_", "_TXB_textnote_");
                    }
                    if (CMD._TXC_rvt != "null_path")
                    {
                        group_TXC_ids = Creat_Unit_GroupId_copyFrom_Ori(_TXC_, doc, orFilter, actView, out group_TXC_textnote_ids, out _TXC_dimension_ids, "_TXC_", "_TXC_textnote_");
                    }
                    if (CMD._TXD_rvt != "null_path")
                    {
                        group_TXD_ids = Creat_Unit_GroupId_copyFrom_Ori(_TXD_, doc, orFilter, actView, out group_TXD_textnote_ids, out _TXD_dimension_ids, "_TXD_", "_TXD_textnote_");
                    }
                    if (CMD._TXA_HXT_rvt != "null_path")
                    {
                        group_TXA_Core_ids = Creat_Unit_GroupId_copyFrom_Ori(_TXA_Core_, doc, orFilter, actView, out group_TXA_Core_textnote_ids, out _TXA_Core_dimension_ids, "_TXA_Core_", "_TXA_Core_textnote_");
                    }
                    if (CMD._TXB_HXT_rvt != "null_path")
                    {
                        group_TXB_Core_ids = Creat_Unit_GroupId_copyFrom_Ori(_TXB_Core_, doc, orFilter, actView, out group_TXB_Core_textnote_ids, out _TXB_Core_dimension_ids, "_TXB_Core_", "_TXB_Core_textnote_");
                    }
                    if (CMD._TXC_HXT_rvt != "null_path")
                    {
                        group_TXC_Core_ids = Creat_Unit_GroupId_copyFrom_Ori(_TXC_Core_, doc, orFilter, actView, out group_TXC_Core_textnote_ids, out _TXC_Core_dimension_ids, "_TXC_Core_", "_TXC_Core_textnote_");
                    }
                    if (CMD._TXD_HXT_rvt != "null_path")
                    {
                        group_TXD_Core_ids = Creat_Unit_GroupId_copyFrom_Ori(_TXD_Core_, doc, orFilter, actView, out group_TXD_Core_textnote_ids, out _TXD_Core_dimension_ids, "_TXD_Core_", "_TXD_Core_textnote_");
                    }
                    #endregion

                    #region 在当前视图，对户型进行排列组合 注意模型复制到当前文档时，所处的位置 time_20200312
                    //abccbd 根据字母组合进行判断
                    if (CMD._TXA_rvt != "null_path")
                    {
                        //判断是否需要对复制进来的模型数据进行二次操作
                    }
                    if (CMD._TXB_rvt != "null_path")
                    {
                        //判断是否需要对复制进来的模型数据进行二次操作
                        ICollection<ElementId> mi_group_TXB_ids = _MirrorElements_ChangeHandler(doc, group_TXB_ids, mid_plane, true);
                        ICollection<ElementId> mi_group_TXB_textnote_ids = _MirrorElements_ChangeHandler(doc, group_TXB_textnote_ids, mid_plane, true);
                        ICollection<ElementId> mi_TXB_dimension_ids = _MirrorElements_ChangeHandler(doc, _TXB_dimension_ids, mid_plane, true);
                    }
                    if (CMD._TXC_rvt != "null_path")
                    {
                        //判断是否需要对复制进来的模型数据进行二次操作
                        ICollection<ElementId> mi_group_TXC_ids = _MirrorElements_ChangeHandler(doc, group_TXC_ids, mid_plane, true);
                        ICollection<ElementId> mi_group_TXC_textnote_ids = _MirrorElements_ChangeHandler(doc, group_TXC_textnote_ids, mid_plane, true);
                        ICollection<ElementId> mi_TXC_dimension_ids = _MirrorElements_ChangeHandler(doc, _TXC_dimension_ids, mid_plane, true);
                    }
                    if (CMD._TXD_rvt != "null_path")
                    {
                        //判断是否需要对复制进来的模型数据进行二次操作
                    }
                    if (CMD._TXA_HXT_rvt != "null_path")
                    {
                        //判断是否需要对复制进来的模型数据进行二次操作
                        ICollection<ElementId> mi_group_TXA_Core_ids = _MirrorElements_ChangeHandler(doc, group_TXA_Core_ids, mid_plane, true);
                        ICollection<ElementId> mi_group_TXA_Core_textnote_ids = _MirrorElements_ChangeHandler(doc, group_TXA_Core_textnote_ids, mid_plane, true);
                    }
                    if (CMD._TXB_HXT_rvt != "null_path")
                    {
                        //判断是否需要对复制进来的模型数据进行二次操作
                    }
                    if (CMD._TXC_HXT_rvt != "null_path")
                    {
                        //判断是否需要对复制进来的模型数据进行二次操作
                        MoveGrouIds(doc, distance_move_abCcbd, group_TXC_Core_ids);
                        MoveGrouIds(doc, distance_move_abCcbd, group_TXC_Core_textnote_ids);
                    }
                    if (CMD._TXD_HXT_rvt != "null_path")
                    {
                        //判断是否需要对复制进来的模型数据进行二次操作
                    }
                    #endregion
                    transGroupCreateDwellingses.Assimilate();
                }
                else
                {
                    transGroupCreateDwellingses.RollBack();
                }
            }
            #region ***关闭所有源文档，但是不保存  time_20200312
            ResetDocument(_TXA_, _TXB_, _TXC_, _TXD_, _TXA_Core_, _TXB_Core_, _TXC_Core_, _TXD_Core_);
            #endregion
            #endregion
        }
        //新建户型_1T2H_abaaba_3DY
        public void Creat_HXZH_1T2H_abaaba_3DY_AB_(UIApplication uiapp)
        {
            #region all code
            #region ***初始化与revit模型进行交互的辅助变量 time_20200312
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Application app = uiapp.Application;
            Document doc = uidoc.Document;
            View actView = doc.ActiveView;
            View FL01_view = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Views).OfClass(typeof(ViewPlan)).FirstOrDefault(x => x.Name == "FL01") as View;
            Plane ori_plane = Plane.CreateByNormalAndOrigin(new XYZ(1, 0, 0), new XYZ(0, 0, 0));//该处为原点镜像轴
            Plane mid_plane = null;//该处mid定义为整体户型组合模式的中心对称轴位置
            LogicalOrFilter orFilter = CreatorFilter(uiapp);
            #endregion

            #region ***声明 all 变量 time_20200312
            Document _TXA_ = null;
            Document _TXB_ = null;
            Document _TXC_ = null;
            Document _TXD_ = null;
            Document _TXA_Core_ = null;
            Document _TXB_Core_ = null;
            Document _TXC_Core_ = null;
            Document _TXD_Core_ = null;
            ICollection<ElementId> group_TXA_ids = new List<ElementId>();//各个基础模型3D元素group的id_list
            ICollection<ElementId> group_TXB_ids = new List<ElementId>();
            ICollection<ElementId> group_TXC_ids = new List<ElementId>();
            ICollection<ElementId> group_TXD_ids = new List<ElementId>();
            ICollection<ElementId> group_TXA_Core_ids = new List<ElementId>();
            ICollection<ElementId> group_TXB_Core_ids = new List<ElementId>();
            ICollection<ElementId> group_TXC_Core_ids = new List<ElementId>();
            ICollection<ElementId> group_TXD_Core_ids = new List<ElementId>();
            ICollection<ElementId> group_TXA_textnote_ids = new List<ElementId>();//各个基础模型2D文字注释group的id_list
            ICollection<ElementId> group_TXB_textnote_ids = new List<ElementId>();
            ICollection<ElementId> group_TXC_textnote_ids = new List<ElementId>();
            ICollection<ElementId> group_TXD_textnote_ids = new List<ElementId>();
            ICollection<ElementId> group_TXA_Core_textnote_ids = new List<ElementId>();
            ICollection<ElementId> group_TXB_Core_textnote_ids = new List<ElementId>();
            ICollection<ElementId> group_TXC_Core_textnote_ids = new List<ElementId>();
            ICollection<ElementId> group_TXD_Core_textnote_ids = new List<ElementId>();
            ICollection<ElementId> _TXA_dimension_ids = new List<ElementId>();//由于2D标注元素，不可以成group，所以此处为各个标注元素的list
            ICollection<ElementId> _TXB_dimension_ids = new List<ElementId>();
            ICollection<ElementId> _TXC_dimension_ids = new List<ElementId>();
            ICollection<ElementId> _TXD_dimension_ids = new List<ElementId>();
            ICollection<ElementId> _TXA_Core_dimension_ids = new List<ElementId>();//注意，目前核心筒基础模型，不涉及标注元素,此外初始化，是为了满足函数参数的空值调用
            ICollection<ElementId> _TXB_Core_dimension_ids = new List<ElementId>();
            ICollection<ElementId> _TXC_Core_dimension_ids = new List<ElementId>();
            ICollection<ElementId> _TXD_Core_dimension_ids = new List<ElementId>();
            #endregion

            #region ***判断是否需要打开源文档 time_20200312
            //OpenTarDocument(uiapp, _TXA_, _TXB_, _TXC_, _TXD_, _TXA_Core_, _TXB_Core_, _TXC_Core_, _TXD_Core_);
            if (CMD._TXA_rvt != "null_path")
            {
                _TXA_ = app.OpenDocumentFile(CMD._TXA_rvt);
            }
            if (CMD._TXB_rvt != "null_path")
            {
                _TXB_ = app.OpenDocumentFile(CMD._TXB_rvt);
            }
            if (CMD._TXC_rvt != "null_path")
            {
                _TXC_ = app.OpenDocumentFile(CMD._TXC_rvt);
            }
            if (CMD._TXD_rvt != "null_path")
            {
                _TXD_ = app.OpenDocumentFile(CMD._TXD_rvt);
            }
            if (CMD._TXA_HXT_rvt != "null_path")
            {
                _TXA_Core_ = app.OpenDocumentFile(CMD._TXA_HXT_rvt);
            }
            if (CMD._TXB_HXT_rvt != "null_path")
            {
                _TXB_Core_ = app.OpenDocumentFile(CMD._TXB_HXT_rvt);
            }
            if (CMD._TXC_HXT_rvt != "null_path")
            {
                _TXC_Core_ = app.OpenDocumentFile(CMD._TXC_HXT_rvt);
            }
            if (CMD._TXD_HXT_rvt != "null_path")
            {
                _TXD_Core_ = app.OpenDocumentFile(CMD._TXD_HXT_rvt);
            }
            #endregion

            #region 根据户型组合模式 在各个源文件 定位元素 在这里不对核心筒定位做处理 原因：revit后台多次打开同一个文件，属于无效操作 time_20200312
            double distance_TXA_X = 0;
            double distance_TXB_X = 0;
            double distance_move_abaaba = 0;

            Plane _mid_plane = null;

            if (CMD._TXA_rvt != "null_path")
            {
                ICollection<ElementId> _TXA_3D_EleIds_temp = (new FilteredElementCollector(_TXA_)).WherePasses(orFilter).WhereElementIsNotElementType().ToElementIds();
                FL01_view = new FilteredElementCollector(_TXA_).OfCategory(BuiltInCategory.OST_Views).OfClass(typeof(ViewPlan)).FirstOrDefault(x => x.Name == "FL01") as View;
                ICollection<ElementId> _TXA_TextNotes_temp = (new FilteredElementCollector(_TXA_, FL01_view.Id)).OwnedByView(FL01_view.Id).OfCategory(BuiltInCategory.OST_TextNotes).WhereElementIsNotElementType().ToElementIds();
                ICollection<ElementId> _TXA_Dimensions_temp = (new FilteredElementCollector(_TXA_, FL01_view.Id)).OwnedByView(FL01_view.Id).OfCategory(BuiltInCategory.OST_Dimensions).WhereElementIsNotElementType().ToElementIds();
                //判断是否需要对基础模型进行定位操作

                double _Abaaba_max_X;
                double _Abaaba_min_X = GetLeftRgiht_X_ingrid(_TXA_, _TXA_3D_EleIds_temp, out _Abaaba_max_X);
                distance_TXA_X = _Abaaba_max_X - _Abaaba_min_X;
            }
            if (CMD._TXB_rvt != "null_path")
            {
                ICollection<ElementId> _TXB_3D_EleIds_temp = (new FilteredElementCollector(_TXB_)).WherePasses(orFilter).WhereElementIsNotElementType().ToElementIds();
                FL01_view = new FilteredElementCollector(_TXB_).OfCategory(BuiltInCategory.OST_Views).OfClass(typeof(ViewPlan)).FirstOrDefault(x => x.Name == "FL01") as View;
                ICollection<ElementId> _TXB_TextNotes_temp = (new FilteredElementCollector(_TXB_, FL01_view.Id)).OwnedByView(FL01_view.Id).OfCategory(BuiltInCategory.OST_TextNotes).WhereElementIsNotElementType().ToElementIds();
                ICollection<ElementId> _TXB_Dimensions_temp = (new FilteredElementCollector(_TXB_, FL01_view.Id)).OwnedByView(FL01_view.Id).OfCategory(BuiltInCategory.OST_Dimensions).WhereElementIsNotElementType().ToElementIds();
                //判断是否需要对基础模型进行定位操作
                ICollection<ElementId> mi_TXB_3D_EleIds_temp = _MirrorElements_ChangeHandler(_TXB_, _TXB_3D_EleIds_temp, ori_plane, true);
                ICollection<ElementId> mi_TXB_TextNotes_temp = _MirrorElements_ChangeHandler(_TXB_, _TXB_TextNotes_temp, ori_plane, true);
                ICollection<ElementId> mi_TXB_Dimensions_temp = _MirrorElements_ChangeHandler(_TXB_, _TXB_Dimensions_temp, ori_plane, true);
                _deleteEles(_TXB_, _TXB_3D_EleIds_temp);
                _deleteEles(_TXB_, _TXB_TextNotes_temp);

                double _aBaaba_max_X;
                double _aBaaba_min_X = GetLeftRgiht_X_ingrid(_TXB_, mi_TXB_3D_EleIds_temp, out _aBaaba_max_X);
                distance_TXB_X = _aBaaba_max_X - _aBaaba_min_X;
                distance_move_abaaba = distance_TXA_X + distance_TXB_X;

                mid_plane = Plane.CreateByNormalAndOrigin(new XYZ(distance_move_abaaba / 2 + 1, 0, 0), new XYZ(distance_move_abaaba / 2, 0, 0));
                _mid_plane = Plane.CreateByNormalAndOrigin(new XYZ(distance_move_abaaba + 1, 0, 0), new XYZ(distance_move_abaaba, 0, 0));
            }
            if (CMD._TXC_rvt != "null_path")
            {
                //判断是否需要对基础模型进行定位操作
            }
            if (CMD._TXD_rvt != "null_path")
            {
                //判断是否需要对基础模型进行定位操作
            }
            #endregion

            using (TransactionGroup transGroupCreateDwellingses = new TransactionGroup(doc, "创建户型零件组_group"))//开展事务组
            {
                if (transGroupCreateDwellingses.Start() == TransactionStatus.Started)//开启事务组
                {
                    #region ***将定位好的基础模型数据，复制到当前工作视图 并creat group 标注元素只有list，不可以creat group time_20200312
                    if (CMD._TXA_rvt != "null_path")
                    {
                        group_TXA_ids = Creat_Unit_GroupId_copyFrom_Ori(_TXA_, doc, orFilter, actView, out group_TXA_textnote_ids, out _TXA_dimension_ids, "_TXA_", "_TXA_textnote_");
                    }
                    if (CMD._TXB_rvt != "null_path")
                    {
                        group_TXB_ids = Creat_Unit_GroupId_copyFrom_Ori(_TXB_, doc, orFilter, actView, out group_TXB_textnote_ids, out _TXB_dimension_ids, "_TXB_", "_TXB_textnote_");
                    }
                    if (CMD._TXC_rvt != "null_path")
                    {
                        group_TXC_ids = Creat_Unit_GroupId_copyFrom_Ori(_TXC_, doc, orFilter, actView, out group_TXC_textnote_ids, out _TXC_dimension_ids, "_TXC_", "_TXC_textnote_");
                    }
                    if (CMD._TXD_rvt != "null_path")
                    {
                        group_TXD_ids = Creat_Unit_GroupId_copyFrom_Ori(_TXD_, doc, orFilter, actView, out group_TXD_textnote_ids, out _TXD_dimension_ids, "_TXD_", "_TXD_textnote_");
                    }
                    if (CMD._TXA_HXT_rvt != "null_path")
                    {
                        group_TXA_Core_ids = Creat_Unit_GroupId_copyFrom_Ori(_TXA_Core_, doc, orFilter, actView, out group_TXA_Core_textnote_ids, out _TXA_Core_dimension_ids, "_TXA_Core_", "_TXA_Core_textnote_");
                    }
                    if (CMD._TXB_HXT_rvt != "null_path")
                    {
                        group_TXB_Core_ids = Creat_Unit_GroupId_copyFrom_Ori(_TXB_Core_, doc, orFilter, actView, out group_TXB_Core_textnote_ids, out _TXB_Core_dimension_ids, "_TXB_Core_", "_TXB_Core_textnote_");
                    }
                    if (CMD._TXC_HXT_rvt != "null_path")
                    {
                        group_TXC_Core_ids = Creat_Unit_GroupId_copyFrom_Ori(_TXC_Core_, doc, orFilter, actView, out group_TXC_Core_textnote_ids, out _TXC_Core_dimension_ids, "_TXC_Core_", "_TXC_Core_textnote_");
                    }
                    if (CMD._TXD_HXT_rvt != "null_path")
                    {
                        group_TXD_Core_ids = Creat_Unit_GroupId_copyFrom_Ori(_TXD_Core_, doc, orFilter, actView, out group_TXD_Core_textnote_ids, out _TXD_Core_dimension_ids, "_TXD_Core_", "_TXD_Core_textnote_");
                    }
                    #endregion

                    #region 在当前视图，对户型进行排列组合 注意模型复制到当前文档时，所处的位置 time_20200312
                    //abccbd 根据字母组合进行判断

                    if (CMD._TXA_rvt != "null_path")
                    {
                        //判断是否需要对复制进来的模型数据进行二次操作
                        ICollection<ElementId> mi_group_TXA_ids = _MirrorElements_ChangeHandler(doc, group_TXA_ids, mid_plane, true);
                        ICollection<ElementId> mi_group_TXA_textnote_ids = _MirrorElements_ChangeHandler(doc, group_TXA_textnote_ids, mid_plane, true);
                        ICollection<ElementId> mi_TXA_dimension_ids = _MirrorElements_ChangeHandler(doc, _TXA_dimension_ids, mid_plane, true);

                        ICollection<ElementId> _mi_group_TXA_ids = _MirrorElements_ChangeHandler(doc, mi_group_TXA_ids, _mid_plane, true);
                        ICollection<ElementId> _mi_group_TXA_textnote_ids = _MirrorElements_ChangeHandler(doc, mi_group_TXA_textnote_ids, _mid_plane, true);
                        ICollection<ElementId> _mi_TXA_dimension_ids = _MirrorElements_ChangeHandler(doc, mi_TXA_dimension_ids, _mid_plane, true);

                        ICollection<ElementId> __mi_group_TXA_ids = _MirrorElements_ChangeHandler(doc, group_TXA_ids, _mid_plane, true);
                        ICollection<ElementId> __mi_group_TXA_textnote_ids = _MirrorElements_ChangeHandler(doc, group_TXA_textnote_ids, _mid_plane, true);
                        ICollection<ElementId> __mi_TXA_dimension_ids = _MirrorElements_ChangeHandler(doc, _TXA_dimension_ids, _mid_plane, true);
                    }
                    if (CMD._TXB_rvt != "null_path")
                    {
                        //判断是否需要对复制进来的模型数据进行二次操作
                        ICollection<ElementId> mi_group_TXB_ids = _MirrorElements_ChangeHandler(doc, group_TXB_ids, _mid_plane, true);
                        ICollection<ElementId> mi_group_TXB_textnote_ids = _MirrorElements_ChangeHandler(doc, group_TXB_textnote_ids, _mid_plane, true);
                        ICollection<ElementId> mi_TXB_dimension_ids = _MirrorElements_ChangeHandler(doc, _TXB_dimension_ids, _mid_plane, true);
                    }
                    if (CMD._TXC_rvt != "null_path")
                    {
                        //判断是否需要对复制进来的模型数据进行二次操作
                    }
                    if (CMD._TXD_rvt != "null_path")
                    {
                        //判断是否需要对复制进来的模型数据进行二次操作
                    }
                    if (CMD._TXA_HXT_rvt != "null_path")
                    {
                        //判断是否需要对复制进来的模型数据进行二次操作
                        ICollection<ElementId> mi_group_TXA_Core_ids = _MirrorElements_ChangeHandler(doc, group_TXA_Core_ids, mid_plane, true);
                        ICollection<ElementId> mi_group_TXA_Core_textnote_ids = _MirrorElements_ChangeHandler(doc, group_TXA_Core_textnote_ids, mid_plane, true);

                        ICollection<ElementId> _mi_group_TXA_Core_ids = _MirrorElements_ChangeHandler(doc, group_TXA_Core_ids, _mid_plane, true);
                        ICollection<ElementId> _mi_group_TXA_Core_textnote_ids = _MirrorElements_ChangeHandler(doc, group_TXA_Core_textnote_ids, _mid_plane, true);
                    }
                    if (CMD._TXB_HXT_rvt != "null_path")
                    {
                        //判断是否需要对复制进来的模型数据进行二次操作
                    }
                    if (CMD._TXC_HXT_rvt != "null_path")
                    {
                        //判断是否需要对复制进来的模型数据进行二次操作
                    }
                    if (CMD._TXD_HXT_rvt != "null_path")
                    {
                        //判断是否需要对复制进来的模型数据进行二次操作
                    }
                    #endregion
                    transGroupCreateDwellingses.Assimilate();
                }
                else
                {
                    transGroupCreateDwellingses.RollBack();
                }
            }
            #region ***关闭所有源文档，但是不保存  time_20200312
            ResetDocument(_TXA_, _TXB_, _TXC_, _TXD_, _TXA_Core_, _TXB_Core_, _TXC_Core_, _TXD_Core_);
            #endregion
            #endregion
        }
        //新建户型_1T2H_abaabc_3DY
        public void Creat_HXZH_1T2H_abaabc_3DY_ABC_(UIApplication uiapp)
        {
            #region all code
            #region ***初始化与revit模型进行交互的辅助变量 time_20200312
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Application app = uiapp.Application;
            Document doc = uidoc.Document;
            View actView = doc.ActiveView;
            View FL01_view = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Views).OfClass(typeof(ViewPlan)).FirstOrDefault(x => x.Name == "FL01") as View;
            Plane ori_plane = Plane.CreateByNormalAndOrigin(new XYZ(1, 0, 0), new XYZ(0, 0, 0));//该处为原点镜像轴
            Plane mid_plane = null;//该处mid定义为整体户型组合模式的中心对称轴位置
            LogicalOrFilter orFilter = CreatorFilter(uiapp);
            #endregion

            #region ***声明 all 变量 time_20200312
            Document _TXA_ = null;
            Document _TXB_ = null;
            Document _TXC_ = null;
            Document _TXD_ = null;
            Document _TXA_Core_ = null;
            Document _TXB_Core_ = null;
            Document _TXC_Core_ = null;
            Document _TXD_Core_ = null;
            ICollection<ElementId> group_TXA_ids = new List<ElementId>();//各个基础模型3D元素group的id_list
            ICollection<ElementId> group_TXB_ids = new List<ElementId>();
            ICollection<ElementId> group_TXC_ids = new List<ElementId>();
            ICollection<ElementId> group_TXD_ids = new List<ElementId>();
            ICollection<ElementId> group_TXA_Core_ids = new List<ElementId>();
            ICollection<ElementId> group_TXB_Core_ids = new List<ElementId>();
            ICollection<ElementId> group_TXC_Core_ids = new List<ElementId>();
            ICollection<ElementId> group_TXD_Core_ids = new List<ElementId>();
            ICollection<ElementId> group_TXA_textnote_ids = new List<ElementId>();//各个基础模型2D文字注释group的id_list
            ICollection<ElementId> group_TXB_textnote_ids = new List<ElementId>();
            ICollection<ElementId> group_TXC_textnote_ids = new List<ElementId>();
            ICollection<ElementId> group_TXD_textnote_ids = new List<ElementId>();
            ICollection<ElementId> group_TXA_Core_textnote_ids = new List<ElementId>();
            ICollection<ElementId> group_TXB_Core_textnote_ids = new List<ElementId>();
            ICollection<ElementId> group_TXC_Core_textnote_ids = new List<ElementId>();
            ICollection<ElementId> group_TXD_Core_textnote_ids = new List<ElementId>();
            ICollection<ElementId> _TXA_dimension_ids = new List<ElementId>();//由于2D标注元素，不可以成group，所以此处为各个标注元素的list
            ICollection<ElementId> _TXB_dimension_ids = new List<ElementId>();
            ICollection<ElementId> _TXC_dimension_ids = new List<ElementId>();
            ICollection<ElementId> _TXD_dimension_ids = new List<ElementId>();
            ICollection<ElementId> _TXA_Core_dimension_ids = new List<ElementId>();//注意，目前核心筒基础模型，不涉及标注元素,此外初始化，是为了满足函数参数的空值调用
            ICollection<ElementId> _TXB_Core_dimension_ids = new List<ElementId>();
            ICollection<ElementId> _TXC_Core_dimension_ids = new List<ElementId>();
            ICollection<ElementId> _TXD_Core_dimension_ids = new List<ElementId>();
            #endregion

            #region ***判断是否需要打开源文档 time_20200312
            //OpenTarDocument(uiapp, _TXA_, _TXB_, _TXC_, _TXD_, _TXA_Core_, _TXB_Core_, _TXC_Core_, _TXD_Core_);
            if (CMD._TXA_rvt != "null_path")
            {
                _TXA_ = app.OpenDocumentFile(CMD._TXA_rvt);
            }
            if (CMD._TXB_rvt != "null_path")
            {
                _TXB_ = app.OpenDocumentFile(CMD._TXB_rvt);
            }
            if (CMD._TXC_rvt != "null_path")
            {
                _TXC_ = app.OpenDocumentFile(CMD._TXC_rvt);
            }
            if (CMD._TXD_rvt != "null_path")
            {
                _TXD_ = app.OpenDocumentFile(CMD._TXD_rvt);
            }
            if (CMD._TXA_HXT_rvt != "null_path")
            {
                _TXA_Core_ = app.OpenDocumentFile(CMD._TXA_HXT_rvt);
            }
            if (CMD._TXB_HXT_rvt != "null_path")
            {
                _TXB_Core_ = app.OpenDocumentFile(CMD._TXB_HXT_rvt);
            }
            if (CMD._TXC_HXT_rvt != "null_path")
            {
                _TXC_Core_ = app.OpenDocumentFile(CMD._TXC_HXT_rvt);
            }
            if (CMD._TXD_HXT_rvt != "null_path")
            {
                _TXD_Core_ = app.OpenDocumentFile(CMD._TXD_HXT_rvt);
            }
            #endregion

            #region 根据户型组合模式 在各个源文件 定位元素 在这里不对核心筒定位做处理 原因：revit后台多次打开同一个文件，属于无效操作 time_20200312
            double distance_TXA_X = 0;
            double distance_TXB_X = 0;
            double distance_move_abaaba = 0;

            Plane _mid_plane = null;

            if (CMD._TXA_rvt != "null_path")
            {
                ICollection<ElementId> _TXA_3D_EleIds_temp = (new FilteredElementCollector(_TXA_)).WherePasses(orFilter).WhereElementIsNotElementType().ToElementIds();
                FL01_view = new FilteredElementCollector(_TXA_).OfCategory(BuiltInCategory.OST_Views).OfClass(typeof(ViewPlan)).FirstOrDefault(x => x.Name == "FL01") as View;
                ICollection<ElementId> _TXA_TextNotes_temp = (new FilteredElementCollector(_TXA_, FL01_view.Id)).OwnedByView(FL01_view.Id).OfCategory(BuiltInCategory.OST_TextNotes).WhereElementIsNotElementType().ToElementIds();
                ICollection<ElementId> _TXA_Dimensions_temp = (new FilteredElementCollector(_TXA_, FL01_view.Id)).OwnedByView(FL01_view.Id).OfCategory(BuiltInCategory.OST_Dimensions).WhereElementIsNotElementType().ToElementIds();
                //判断是否需要对基础模型进行定位操作

                double _Abaaba_max_X;
                double _Abaaba_min_X = GetLeftRgiht_X_ingrid(_TXA_, _TXA_3D_EleIds_temp, out _Abaaba_max_X);
                distance_TXA_X = _Abaaba_max_X - _Abaaba_min_X;
            }
            if (CMD._TXB_rvt != "null_path")
            {
                ICollection<ElementId> _TXB_3D_EleIds_temp = (new FilteredElementCollector(_TXB_)).WherePasses(orFilter).WhereElementIsNotElementType().ToElementIds();
                FL01_view = new FilteredElementCollector(_TXB_).OfCategory(BuiltInCategory.OST_Views).OfClass(typeof(ViewPlan)).FirstOrDefault(x => x.Name == "FL01") as View;
                ICollection<ElementId> _TXB_TextNotes_temp = (new FilteredElementCollector(_TXB_, FL01_view.Id)).OwnedByView(FL01_view.Id).OfCategory(BuiltInCategory.OST_TextNotes).WhereElementIsNotElementType().ToElementIds();
                ICollection<ElementId> _TXB_Dimensions_temp = (new FilteredElementCollector(_TXB_, FL01_view.Id)).OwnedByView(FL01_view.Id).OfCategory(BuiltInCategory.OST_Dimensions).WhereElementIsNotElementType().ToElementIds();
                //判断是否需要对基础模型进行定位操作
                ICollection<ElementId> mi_TXB_3D_EleIds_temp = _MirrorElements_ChangeHandler(_TXB_, _TXB_3D_EleIds_temp, ori_plane, true);
                ICollection<ElementId> mi_TXB_TextNotes_temp = _MirrorElements_ChangeHandler(_TXB_, _TXB_TextNotes_temp, ori_plane, true);
                ICollection<ElementId> mi_TXB_Dimensions_temp = _MirrorElements_ChangeHandler(_TXB_, _TXB_Dimensions_temp, ori_plane, true);
                _deleteEles(_TXB_, _TXB_3D_EleIds_temp);
                _deleteEles(_TXB_, _TXB_TextNotes_temp);

                double _aBaaba_max_X;
                double _aBaaba_min_X = GetLeftRgiht_X_ingrid(_TXB_, mi_TXB_3D_EleIds_temp, out _aBaaba_max_X);
                distance_TXB_X = _aBaaba_max_X - _aBaaba_min_X;
                distance_move_abaaba = distance_TXA_X + distance_TXB_X;

                mid_plane = Plane.CreateByNormalAndOrigin(new XYZ(distance_move_abaaba / 2 + 1, 0, 0), new XYZ(distance_move_abaaba / 2, 0, 0));
                _mid_plane = Plane.CreateByNormalAndOrigin(new XYZ(distance_move_abaaba + 1, 0, 0), new XYZ(distance_move_abaaba, 0, 0));
            }
            if (CMD._TXC_rvt != "null_path")
            {
                //判断是否需要对基础模型进行定位操作
            }
            if (CMD._TXD_rvt != "null_path")
            {
                //判断是否需要对基础模型进行定位操作
            }
            #endregion

            using (TransactionGroup transGroupCreateDwellingses = new TransactionGroup(doc, "创建户型零件组_group"))//开展事务组
            {
                if (transGroupCreateDwellingses.Start() == TransactionStatus.Started)//开启事务组
                {
                    #region ***将定位好的基础模型数据，复制到当前工作视图 并creat group 标注元素只有list，不可以creat group time_20200312
                    if (CMD._TXA_rvt != "null_path")
                    {
                        group_TXA_ids = Creat_Unit_GroupId_copyFrom_Ori(_TXA_, doc, orFilter, actView, out group_TXA_textnote_ids, out _TXA_dimension_ids, "_TXA_", "_TXA_textnote_");
                    }
                    if (CMD._TXB_rvt != "null_path")
                    {
                        group_TXB_ids = Creat_Unit_GroupId_copyFrom_Ori(_TXB_, doc, orFilter, actView, out group_TXB_textnote_ids, out _TXB_dimension_ids, "_TXB_", "_TXB_textnote_");
                    }
                    if (CMD._TXC_rvt != "null_path")
                    {
                        group_TXC_ids = Creat_Unit_GroupId_copyFrom_Ori(_TXC_, doc, orFilter, actView, out group_TXC_textnote_ids, out _TXC_dimension_ids, "_TXC_", "_TXC_textnote_");
                    }
                    if (CMD._TXD_rvt != "null_path")
                    {
                        group_TXD_ids = Creat_Unit_GroupId_copyFrom_Ori(_TXD_, doc, orFilter, actView, out group_TXD_textnote_ids, out _TXD_dimension_ids, "_TXD_", "_TXD_textnote_");
                    }
                    if (CMD._TXA_HXT_rvt != "null_path")
                    {
                        group_TXA_Core_ids = Creat_Unit_GroupId_copyFrom_Ori(_TXA_Core_, doc, orFilter, actView, out group_TXA_Core_textnote_ids, out _TXA_Core_dimension_ids, "_TXA_Core_", "_TXA_Core_textnote_");
                    }
                    if (CMD._TXB_HXT_rvt != "null_path")
                    {
                        group_TXB_Core_ids = Creat_Unit_GroupId_copyFrom_Ori(_TXB_Core_, doc, orFilter, actView, out group_TXB_Core_textnote_ids, out _TXB_Core_dimension_ids, "_TXB_Core_", "_TXB_Core_textnote_");
                    }
                    if (CMD._TXC_HXT_rvt != "null_path")
                    {
                        group_TXC_Core_ids = Creat_Unit_GroupId_copyFrom_Ori(_TXC_Core_, doc, orFilter, actView, out group_TXC_Core_textnote_ids, out _TXC_Core_dimension_ids, "_TXC_Core_", "_TXC_Core_textnote_");
                    }
                    if (CMD._TXD_HXT_rvt != "null_path")
                    {
                        group_TXD_Core_ids = Creat_Unit_GroupId_copyFrom_Ori(_TXD_Core_, doc, orFilter, actView, out group_TXD_Core_textnote_ids, out _TXD_Core_dimension_ids, "_TXD_Core_", "_TXD_Core_textnote_");
                    }
                    #endregion

                    #region 在当前视图，对户型进行排列组合 注意模型复制到当前文档时，所处的位置 time_20200312
                    //abccbd 根据字母组合进行判断

                    if (CMD._TXA_rvt != "null_path")
                    {
                        //判断是否需要对复制进来的模型数据进行二次操作
                        ICollection<ElementId> mi_group_TXA_ids = _MirrorElements_ChangeHandler(doc, group_TXA_ids, mid_plane, true);
                        ICollection<ElementId> mi_group_TXA_textnote_ids = _MirrorElements_ChangeHandler(doc, group_TXA_textnote_ids, mid_plane, true);
                        ICollection<ElementId> mi_TXA_dimension_ids = _MirrorElements_ChangeHandler(doc, _TXA_dimension_ids, mid_plane, true);

                        ICollection<ElementId> _mi_group_TXA_ids = _MirrorElements_ChangeHandler(doc, mi_group_TXA_ids, _mid_plane, true);
                        ICollection<ElementId> _mi_group_TXA_textnote_ids = _MirrorElements_ChangeHandler(doc, mi_group_TXA_textnote_ids, _mid_plane, true);
                        ICollection<ElementId> _mi_TXA_dimension_ids = _MirrorElements_ChangeHandler(doc, mi_TXA_dimension_ids, _mid_plane, true);
                    }
                    if (CMD._TXB_rvt != "null_path")
                    {
                        //判断是否需要对复制进来的模型数据进行二次操作
                        ICollection<ElementId> mi_group_TXB_ids = _MirrorElements_ChangeHandler(doc, group_TXB_ids, _mid_plane, true);
                        ICollection<ElementId> mi_group_TXB_textnote_ids = _MirrorElements_ChangeHandler(doc, group_TXB_textnote_ids, _mid_plane, true);
                        ICollection<ElementId> mi_TXB_dimension_ids = _MirrorElements_ChangeHandler(doc, _TXB_dimension_ids, _mid_plane, true);
                    }
                    if (CMD._TXC_rvt != "null_path")
                    {
                        //判断是否需要对复制进来的模型数据进行二次操作
                        ICollection<ElementId> __mi_group_TXC_ids = _MirrorElements_ChangeHandler(doc, group_TXC_ids, _mid_plane, true);
                        ICollection<ElementId> __mi_group_TXC_textnote_ids = _MirrorElements_ChangeHandler(doc, group_TXC_textnote_ids, _mid_plane, true);
                        ICollection<ElementId> __mi_TXC_dimension_ids = _MirrorElements_ChangeHandler(doc, _TXC_dimension_ids, _mid_plane, true);
                    }
                    if (CMD._TXD_rvt != "null_path")
                    {
                        //判断是否需要对复制进来的模型数据进行二次操作
                    }
                    if (CMD._TXA_HXT_rvt != "null_path")
                    {
                        //判断是否需要对复制进来的模型数据进行二次操作
                        ICollection<ElementId> mi_group_TXA_Core_ids = _MirrorElements_ChangeHandler(doc, group_TXA_Core_ids, mid_plane, true);
                        ICollection<ElementId> mi_group_TXA_Core_textnote_ids = _MirrorElements_ChangeHandler(doc, group_TXA_Core_textnote_ids, mid_plane, true);

                        ICollection<ElementId> _mi_group_TXA_Core_ids = _MirrorElements_ChangeHandler(doc, group_TXA_Core_ids, _mid_plane, true);
                        ICollection<ElementId> _mi_group_TXA_Core_textnote_ids = _MirrorElements_ChangeHandler(doc, group_TXA_Core_textnote_ids, _mid_plane, true);
                    }
                    if (CMD._TXB_HXT_rvt != "null_path")
                    {
                        //判断是否需要对复制进来的模型数据进行二次操作
                    }
                    if (CMD._TXC_HXT_rvt != "null_path")
                    {
                        //判断是否需要对复制进来的模型数据进行二次操作
                    }
                    if (CMD._TXD_HXT_rvt != "null_path")
                    {
                        //判断是否需要对复制进来的模型数据进行二次操作
                    }
                    #endregion
                    transGroupCreateDwellingses.Assimilate();
                }
                else
                {
                    transGroupCreateDwellingses.RollBack();
                }
            }
            #region ***关闭所有源文档，但是不保存  time_20200312
            ResetDocument(_TXA_, _TXB_, _TXC_, _TXD_, _TXA_Core_, _TXB_Core_, _TXC_Core_, _TXD_Core_);
            #endregion
            #endregion
        }
        //新建户型_2T4H_abba_1DY_AB_
        public void Creat_HXZU_2T4H_abba_1DY_AB_(UIApplication uiapp)
        {
            #region all code
            #region ***初始化与revit模型进行交互的辅助变量 time_20200312
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Application app = uiapp.Application;
            Document doc = uidoc.Document;
            View actView = doc.ActiveView;
            View FL01_view = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Views).OfClass(typeof(ViewPlan)).FirstOrDefault(x => x.Name == "FL01") as View;
            Plane ori_plane = Plane.CreateByNormalAndOrigin(new XYZ(1, 0, 0), new XYZ(0, 0, 0));//该处为原点镜像轴
            Plane mid_plane = null;//该处mid定义为整体户型组合模式的中心对称轴位置
            LogicalOrFilter orFilter = CreatorFilter(uiapp);
            #endregion

            #region ***声明 all 变量 time_20200312
            Document _TXA_ = null;
            Document _TXB_ = null;
            Document _TXC_ = null;
            Document _TXD_ = null;
            Document _TXA_Core_ = null;
            Document _TXB_Core_ = null;
            Document _TXC_Core_ = null;
            Document _TXD_Core_ = null;
            ICollection<ElementId> group_TXA_ids = new List<ElementId>();//各个基础模型3D元素group的id_list
            ICollection<ElementId> group_TXB_ids = new List<ElementId>();
            ICollection<ElementId> group_TXC_ids = new List<ElementId>();
            ICollection<ElementId> group_TXD_ids = new List<ElementId>();
            ICollection<ElementId> group_TXA_Core_ids = new List<ElementId>();
            ICollection<ElementId> group_TXB_Core_ids = new List<ElementId>();
            ICollection<ElementId> group_TXC_Core_ids = new List<ElementId>();
            ICollection<ElementId> group_TXD_Core_ids = new List<ElementId>();
            ICollection<ElementId> group_TXA_textnote_ids = new List<ElementId>();//各个基础模型2D文字注释group的id_list
            ICollection<ElementId> group_TXB_textnote_ids = new List<ElementId>();
            ICollection<ElementId> group_TXC_textnote_ids = new List<ElementId>();
            ICollection<ElementId> group_TXD_textnote_ids = new List<ElementId>();
            ICollection<ElementId> group_TXA_Core_textnote_ids = new List<ElementId>();
            ICollection<ElementId> group_TXB_Core_textnote_ids = new List<ElementId>();
            ICollection<ElementId> group_TXC_Core_textnote_ids = new List<ElementId>();
            ICollection<ElementId> group_TXD_Core_textnote_ids = new List<ElementId>();
            ICollection<ElementId> _TXA_dimension_ids = new List<ElementId>();//由于2D标注元素，不可以成group，所以此处为各个标注元素的list
            ICollection<ElementId> _TXB_dimension_ids = new List<ElementId>();
            ICollection<ElementId> _TXC_dimension_ids = new List<ElementId>();
            ICollection<ElementId> _TXD_dimension_ids = new List<ElementId>();
            ICollection<ElementId> _TXA_Core_dimension_ids = new List<ElementId>();//注意，目前核心筒基础模型，不涉及标注元素,此外初始化，是为了满足函数参数的空值调用
            ICollection<ElementId> _TXB_Core_dimension_ids = new List<ElementId>();
            ICollection<ElementId> _TXC_Core_dimension_ids = new List<ElementId>();
            ICollection<ElementId> _TXD_Core_dimension_ids = new List<ElementId>();
            #endregion

            #region ***判断是否需要打开源文档 time_20200312
            //OpenTarDocument(uiapp, _TXA_, _TXB_, _TXC_, _TXD_, _TXA_Core_, _TXB_Core_, _TXC_Core_, _TXD_Core_);
            if (CMD._TXA_rvt != "null_path")
            {
                _TXA_ = app.OpenDocumentFile(CMD._TXA_rvt);
            }
            if (CMD._TXB_rvt != "null_path")
            {
                _TXB_ = app.OpenDocumentFile(CMD._TXB_rvt);
            }
            if (CMD._TXC_rvt != "null_path")
            {
                _TXC_ = app.OpenDocumentFile(CMD._TXC_rvt);
            }
            if (CMD._TXD_rvt != "null_path")
            {
                _TXD_ = app.OpenDocumentFile(CMD._TXD_rvt);
            }
            if (CMD._TXA_HXT_rvt != "null_path")
            {
                _TXA_Core_ = app.OpenDocumentFile(CMD._TXA_HXT_rvt);
            }
            if (CMD._TXB_HXT_rvt != "null_path")
            {
                _TXB_Core_ = app.OpenDocumentFile(CMD._TXB_HXT_rvt);
            }
            if (CMD._TXC_HXT_rvt != "null_path")
            {
                _TXC_Core_ = app.OpenDocumentFile(CMD._TXC_HXT_rvt);
            }
            if (CMD._TXD_HXT_rvt != "null_path")
            {
                _TXD_Core_ = app.OpenDocumentFile(CMD._TXD_HXT_rvt);
            }
            #endregion

            #region 根据户型组合模式 在各个源文件 定位元素 在这里不对核心筒定位做处理 原因：revit后台多次打开同一个文件，属于无效操作 time_20200312
            double distance_TXB_X = 0;

            if (CMD._TXA_rvt != "null_path")
            {
                //判断是否需要对基础模型进行定位操作
            }
            if (CMD._TXB_rvt != "null_path")
            {
                ICollection<ElementId> _TXB_3D_EleIds_temp = (new FilteredElementCollector(_TXB_)).WherePasses(orFilter).WhereElementIsNotElementType().ToElementIds();
                FL01_view = new FilteredElementCollector(_TXB_).OfCategory(BuiltInCategory.OST_Views).OfClass(typeof(ViewPlan)).FirstOrDefault(x => x.Name == "FL01") as View;
                ICollection<ElementId> _TXB_TextNotes_temp = (new FilteredElementCollector(_TXB_, FL01_view.Id)).OwnedByView(FL01_view.Id).OfCategory(BuiltInCategory.OST_TextNotes).WhereElementIsNotElementType().ToElementIds();
                ICollection<ElementId> _TXB_Dimensions_temp = (new FilteredElementCollector(_TXB_, FL01_view.Id)).OwnedByView(FL01_view.Id).OfCategory(BuiltInCategory.OST_Dimensions).WhereElementIsNotElementType().ToElementIds();
                //判断是否需要对基础模型进行定位操作

                double _aBba_max_X;
                double _aBba_min_X = GetLeftRgiht_X_ingrid(_TXB_, _TXB_3D_EleIds_temp, out _aBba_max_X);
                distance_TXB_X = _aBba_max_X - _aBba_min_X;
                mid_plane = Plane.CreateByNormalAndOrigin(new XYZ(_aBba_max_X + 1, 0, 0), new XYZ(_aBba_max_X, 0, 0));
            }
            if (CMD._TXC_rvt != "null_path")
            {
                //判断是否需要对基础模型进行定位操作
            }
            if (CMD._TXD_rvt != "null_path")
            {
                //判断是否需要对基础模型进行定位操作
            }
            #endregion

            using (TransactionGroup transGroupCreateDwellingses = new TransactionGroup(doc, "创建户型零件组_group"))//开展事务组
            {
                if (transGroupCreateDwellingses.Start() == TransactionStatus.Started)//开启事务组
                {
                    #region ***将定位好的基础模型数据，复制到当前工作视图 并creat group 标注元素只有list，不可以creat group time_20200312
                    if (CMD._TXA_rvt != "null_path")
                    {
                        group_TXA_ids = Creat_Unit_GroupId_copyFrom_Ori(_TXA_, doc, orFilter, actView, out group_TXA_textnote_ids, out _TXA_dimension_ids, "_TXA_", "_TXA_textnote_");
                    }
                    if (CMD._TXB_rvt != "null_path")
                    {
                        group_TXB_ids = Creat_Unit_GroupId_copyFrom_Ori(_TXB_, doc, orFilter, actView, out group_TXB_textnote_ids, out _TXB_dimension_ids, "_TXB_", "_TXB_textnote_");
                    }
                    if (CMD._TXC_rvt != "null_path")
                    {
                        group_TXC_ids = Creat_Unit_GroupId_copyFrom_Ori(_TXC_, doc, orFilter, actView, out group_TXC_textnote_ids, out _TXC_dimension_ids, "_TXC_", "_TXC_textnote_");
                    }
                    if (CMD._TXD_rvt != "null_path")
                    {
                        group_TXD_ids = Creat_Unit_GroupId_copyFrom_Ori(_TXD_, doc, orFilter, actView, out group_TXD_textnote_ids, out _TXD_dimension_ids, "_TXD_", "_TXD_textnote_");
                    }
                    if (CMD._TXA_HXT_rvt != "null_path")
                    {
                        group_TXA_Core_ids = Creat_Unit_GroupId_copyFrom_Ori(_TXA_Core_, doc, orFilter, actView, out group_TXA_Core_textnote_ids, out _TXA_Core_dimension_ids, "_TXA_Core_", "_TXA_Core_textnote_");
                    }
                    if (CMD._TXB_HXT_rvt != "null_path")
                    {
                        group_TXB_Core_ids = Creat_Unit_GroupId_copyFrom_Ori(_TXB_Core_, doc, orFilter, actView, out group_TXB_Core_textnote_ids, out _TXB_Core_dimension_ids, "_TXB_Core_", "_TXB_Core_textnote_");
                    }
                    if (CMD._TXC_HXT_rvt != "null_path")
                    {
                        group_TXC_Core_ids = Creat_Unit_GroupId_copyFrom_Ori(_TXC_Core_, doc, orFilter, actView, out group_TXC_Core_textnote_ids, out _TXC_Core_dimension_ids, "_TXC_Core_", "_TXC_Core_textnote_");
                    }
                    if (CMD._TXD_HXT_rvt != "null_path")
                    {
                        group_TXD_Core_ids = Creat_Unit_GroupId_copyFrom_Ori(_TXD_Core_, doc, orFilter, actView, out group_TXD_Core_textnote_ids, out _TXD_Core_dimension_ids, "_TXD_Core_", "_TXD_Core_textnote_");
                    }
                    #endregion

                    #region 在当前视图，对户型进行排列组合 注意模型复制到当前文档时，所处的位置 time_20200312
                    //abccbd 根据字母组合进行判断
                    double move_distance_TXA_up = GetGlobalParametersValue_double(_TXA_, "move_distance_up_length_global");//_TXB_移动的为_TXA_的缝隙距离
                    double move_distance_TXA_down = GetGlobalParametersValue_double(_TXA_, "move_distance_down_length_global");//_TXB_移动的为_TXA_的缝隙距离

                    if (CMD._TXA_rvt != "null_path")
                    {
                        //判断是否需要对复制进来的模型数据进行二次操作
                        ICollection<ElementId> mi_group_TXA_ids = _MirrorElements_ChangeHandler(doc, group_TXA_ids, mid_plane, true);
                        ICollection<ElementId> mi_group_TXA_textnote_ids = _MirrorElements_ChangeHandler(doc, group_TXA_textnote_ids, mid_plane, true);
                        ICollection<ElementId> mi_TXA_dimension_ids = _MirrorElements_ChangeHandler(doc, _TXA_dimension_ids, mid_plane, true);

                        MoveGrouIds(doc, -move_distance_TXA_down - move_distance_TXA_down, mi_group_TXA_ids);
                        MoveGrouIds(doc, -move_distance_TXA_down - move_distance_TXA_down, mi_group_TXA_textnote_ids);
                    }
                    if (CMD._TXB_rvt != "null_path")
                    {
                        //判断是否需要对复制进来的模型数据进行二次操作
                        ICollection<ElementId> mi_group_TXB_ids = _MirrorElements_ChangeHandler(doc, group_TXB_ids, mid_plane, true);
                        ICollection<ElementId> mi_group_TXB_textnote_ids = _MirrorElements_ChangeHandler(doc, group_TXB_textnote_ids, mid_plane, true);
                        ICollection<ElementId> mi_TXB_dimension_ids = _MirrorElements_ChangeHandler(doc, _TXB_dimension_ids, mid_plane, true);

                        //TaskDialog.Show("Revit", move_distance_TXA_.ToString());
                        MoveGrouIds(doc, -move_distance_TXA_down, group_TXB_ids);
                        MoveGrouIds(doc, -move_distance_TXA_down, group_TXB_textnote_ids);

                        MoveGrouIds(doc, -move_distance_TXA_down, mi_group_TXB_ids);
                        MoveGrouIds(doc, -move_distance_TXA_down, mi_group_TXB_textnote_ids);
                    }
                    if (CMD._TXC_rvt != "null_path")
                    {
                        //判断是否需要对复制进来的模型数据进行二次操作
                    }
                    if (CMD._TXD_rvt != "null_path")
                    {
                        //判断是否需要对复制进来的模型数据进行二次操作
                    }
                    if (CMD._TXA_HXT_rvt != "null_path")
                    {
                        //判断是否需要对复制进来的模型数据进行二次操作
                        if (!CMD._TXA_HXT_rvt.Contains(@"CORE_5408") && !CMD._TXA_HXT_rvt.Contains(@"CORE_335406"))//core_5408为剪刀楼梯 CORE_335406为1T4H模式
                        {
                            //该处主要原因为剪刀楼梯不需要复制
                            ICollection<ElementId> mi_group_TXA_Core_ids = _MirrorElements_ChangeHandler(doc, group_TXA_Core_ids, mid_plane, true);
                            ICollection<ElementId> mi_group_TXA_Core_textnote_ids = _MirrorElements_ChangeHandler(doc, group_TXA_Core_textnote_ids, mid_plane, true);

                            MoveGrouIds(doc, -move_distance_TXA_up, group_TXA_Core_ids);
                            MoveGrouIds(doc, -move_distance_TXA_up, group_TXA_Core_textnote_ids);
                            MoveGrouIds(doc, -move_distance_TXA_down - move_distance_TXA_down + move_distance_TXA_up, mi_group_TXA_Core_ids);
                            MoveGrouIds(doc, -move_distance_TXA_down - move_distance_TXA_down + move_distance_TXA_up, mi_group_TXA_Core_textnote_ids);

                            //添加核心筒走廊_此处操作为_通过修改全局参数，控制核心筒走廊宽度的一半；
                            AddCorefloor(doc, _TXA_Core_, distance_TXB_X);
                        }
                        else if (CMD._TXA_HXT_rvt.Contains(@"CORE_335406") || CMD._TXA_HXT_rvt.Contains(@"CORE_5408"))
                        {
                            //需要对1T4H的核心筒进行移动 向左移动一个核心筒的凹口
                            MoveGrouIds(doc, -move_distance_TXA_up, group_TXA_Core_ids);
                            MoveGrouIds(doc, -move_distance_TXA_up, group_TXA_Core_textnote_ids);
                        }
                    }
                    if (CMD._TXB_HXT_rvt != "null_path")
                    {
                        //判断是否需要对复制进来的模型数据进行二次操作
                    }
                    if (CMD._TXC_HXT_rvt != "null_path")
                    {
                        //判断是否需要对复制进来的模型数据进行二次操作
                    }
                    if (CMD._TXD_HXT_rvt != "null_path")
                    {
                        //判断是否需要对复制进来的模型数据进行二次操作
                    }
                    #endregion
                    transGroupCreateDwellingses.Assimilate();
                }
                else
                {
                    transGroupCreateDwellingses.RollBack();
                }
            }
            #region ***关闭所有源文档，但是不保存  time_20200312
            ResetDocument(_TXA_, _TXB_, _TXC_, _TXD_, _TXA_Core_, _TXB_Core_, _TXC_Core_, _TXD_Core_);
            #endregion
            #endregion
        }
        ////新建户型_2T4H_abbaabba_2DY_AB_
        public void Creat_HXZU_2T4H_abbaabba_2DY_AB_(UIApplication uiapp)
        {
            #region all code
            #region ***初始化与revit模型进行交互的辅助变量 time_20200312
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Application app = uiapp.Application;
            Document doc = uidoc.Document;
            View actView = doc.ActiveView;
            View FL01_view = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Views).OfClass(typeof(ViewPlan)).FirstOrDefault(x => x.Name == "FL01") as View;
            Plane ori_plane = Plane.CreateByNormalAndOrigin(new XYZ(1, 0, 0), new XYZ(0, 0, 0));//该处为原点镜像轴
            Plane mid_plane = null;//该处mid定义为整体户型组合模式的中心对称轴位置
            LogicalOrFilter orFilter = CreatorFilter(uiapp);
            #endregion

            #region ***声明 all 变量 time_20200312
            Document _TXA_ = null;
            Document _TXB_ = null;
            Document _TXC_ = null;
            Document _TXD_ = null;
            Document _TXA_Core_ = null;
            Document _TXB_Core_ = null;
            Document _TXC_Core_ = null;
            Document _TXD_Core_ = null;
            ICollection<ElementId> group_TXA_ids = new List<ElementId>();//各个基础模型3D元素group的id_list
            ICollection<ElementId> group_TXB_ids = new List<ElementId>();
            ICollection<ElementId> group_TXC_ids = new List<ElementId>();
            ICollection<ElementId> group_TXD_ids = new List<ElementId>();
            ICollection<ElementId> group_TXA_Core_ids = new List<ElementId>();
            ICollection<ElementId> group_TXB_Core_ids = new List<ElementId>();
            ICollection<ElementId> group_TXC_Core_ids = new List<ElementId>();
            ICollection<ElementId> group_TXD_Core_ids = new List<ElementId>();
            ICollection<ElementId> group_TXA_textnote_ids = new List<ElementId>();//各个基础模型2D文字注释group的id_list
            ICollection<ElementId> group_TXB_textnote_ids = new List<ElementId>();
            ICollection<ElementId> group_TXC_textnote_ids = new List<ElementId>();
            ICollection<ElementId> group_TXD_textnote_ids = new List<ElementId>();
            ICollection<ElementId> group_TXA_Core_textnote_ids = new List<ElementId>();
            ICollection<ElementId> group_TXB_Core_textnote_ids = new List<ElementId>();
            ICollection<ElementId> group_TXC_Core_textnote_ids = new List<ElementId>();
            ICollection<ElementId> group_TXD_Core_textnote_ids = new List<ElementId>();
            ICollection<ElementId> _TXA_dimension_ids = new List<ElementId>();//由于2D标注元素，不可以成group，所以此处为各个标注元素的list
            ICollection<ElementId> _TXB_dimension_ids = new List<ElementId>();
            ICollection<ElementId> _TXC_dimension_ids = new List<ElementId>();
            ICollection<ElementId> _TXD_dimension_ids = new List<ElementId>();
            ICollection<ElementId> _TXA_Core_dimension_ids = new List<ElementId>();//注意，目前核心筒基础模型，不涉及标注元素,此外初始化，是为了满足函数参数的空值调用
            ICollection<ElementId> _TXB_Core_dimension_ids = new List<ElementId>();
            ICollection<ElementId> _TXC_Core_dimension_ids = new List<ElementId>();
            ICollection<ElementId> _TXD_Core_dimension_ids = new List<ElementId>();
            #endregion

            #region ***判断是否需要打开源文档 time_20200312
            //OpenTarDocument(uiapp, _TXA_, _TXB_, _TXC_, _TXD_, _TXA_Core_, _TXB_Core_, _TXC_Core_, _TXD_Core_);
            if (CMD._TXA_rvt != "null_path")
            {
                _TXA_ = app.OpenDocumentFile(CMD._TXA_rvt);
            }
            if (CMD._TXB_rvt != "null_path")
            {
                _TXB_ = app.OpenDocumentFile(CMD._TXB_rvt);
            }
            if (CMD._TXC_rvt != "null_path")
            {
                _TXC_ = app.OpenDocumentFile(CMD._TXC_rvt);
            }
            if (CMD._TXD_rvt != "null_path")
            {
                _TXD_ = app.OpenDocumentFile(CMD._TXD_rvt);
            }
            if (CMD._TXA_HXT_rvt != "null_path")
            {
                _TXA_Core_ = app.OpenDocumentFile(CMD._TXA_HXT_rvt);
            }
            if (CMD._TXB_HXT_rvt != "null_path")
            {
                _TXB_Core_ = app.OpenDocumentFile(CMD._TXB_HXT_rvt);
            }
            if (CMD._TXC_HXT_rvt != "null_path")
            {
                _TXC_Core_ = app.OpenDocumentFile(CMD._TXC_HXT_rvt);
            }
            if (CMD._TXD_HXT_rvt != "null_path")
            {
                _TXD_Core_ = app.OpenDocumentFile(CMD._TXD_HXT_rvt);
            }
            #endregion

            #region 根据户型组合模式 在各个源文件 定位元素 在这里不对核心筒定位做处理 原因：revit后台多次打开同一个文件，属于无效操作 time_20200312
            double distance_TXA_X = 0;
            double distance_TXB_X = 0;
            Plane _mid_plane = null;

            if (CMD._TXA_rvt != "null_path")
            {
                ICollection<ElementId> _TXA_3D_EleIds_temp = (new FilteredElementCollector(_TXA_)).WherePasses(orFilter).WhereElementIsNotElementType().ToElementIds();
                FL01_view = new FilteredElementCollector(_TXA_).OfCategory(BuiltInCategory.OST_Views).OfClass(typeof(ViewPlan)).FirstOrDefault(x => x.Name == "FL01") as View;
                ICollection<ElementId> _TXA_TextNotes_temp = (new FilteredElementCollector(_TXA_, FL01_view.Id)).OwnedByView(FL01_view.Id).OfCategory(BuiltInCategory.OST_TextNotes).WhereElementIsNotElementType().ToElementIds();
                ICollection<ElementId> _TXA_Dimensions_temp = (new FilteredElementCollector(_TXA_, FL01_view.Id)).OwnedByView(FL01_view.Id).OfCategory(BuiltInCategory.OST_Dimensions).WhereElementIsNotElementType().ToElementIds();
                //判断是否需要对基础模型进行定位操作

                double _Abba_max_X;
                double _Abba_min_X = GetLeftRgiht_X_ingrid(_TXA_, _TXA_3D_EleIds_temp, out _Abba_max_X);
                distance_TXA_X = _Abba_max_X - _Abba_min_X;
            }
            if (CMD._TXB_rvt != "null_path")
            {
                ICollection<ElementId> _TXB_3D_EleIds_temp = (new FilteredElementCollector(_TXB_)).WherePasses(orFilter).WhereElementIsNotElementType().ToElementIds();
                FL01_view = new FilteredElementCollector(_TXB_).OfCategory(BuiltInCategory.OST_Views).OfClass(typeof(ViewPlan)).FirstOrDefault(x => x.Name == "FL01") as View;
                ICollection<ElementId> _TXB_TextNotes_temp = (new FilteredElementCollector(_TXB_, FL01_view.Id)).OwnedByView(FL01_view.Id).OfCategory(BuiltInCategory.OST_TextNotes).WhereElementIsNotElementType().ToElementIds();
                ICollection<ElementId> _TXB_Dimensions_temp = (new FilteredElementCollector(_TXB_, FL01_view.Id)).OwnedByView(FL01_view.Id).OfCategory(BuiltInCategory.OST_Dimensions).WhereElementIsNotElementType().ToElementIds();
                //判断是否需要对基础模型进行定位操作

                double _aBba_max_X;
                double _aBba_min_X = GetLeftRgiht_X_ingrid(_TXB_, _TXB_3D_EleIds_temp, out _aBba_max_X);
                distance_TXB_X = _aBba_max_X - _aBba_min_X;

                mid_plane = Plane.CreateByNormalAndOrigin(new XYZ(distance_TXB_X + 1, 0, 0), new XYZ(distance_TXB_X, 0, 0));
                _mid_plane = Plane.CreateByNormalAndOrigin(new XYZ(distance_TXB_X + distance_TXB_X + distance_TXA_X + 1, 0, 0), new XYZ(distance_TXB_X + distance_TXB_X + distance_TXA_X, 0, 0));
            }
            if (CMD._TXC_rvt != "null_path")
            {
                //判断是否需要对基础模型进行定位操作
            }
            if (CMD._TXD_rvt != "null_path")
            {
                //判断是否需要对基础模型进行定位操作
            }
            #endregion

            using (TransactionGroup transGroupCreateDwellingses = new TransactionGroup(doc, "创建户型零件组_group"))//开展事务组
            {
                if (transGroupCreateDwellingses.Start() == TransactionStatus.Started)//开启事务组
                {
                    #region ***将定位好的基础模型数据，复制到当前工作视图 并creat group 标注元素只有list，不可以creat group time_20200312
                    if (CMD._TXA_rvt != "null_path")
                    {
                        group_TXA_ids = Creat_Unit_GroupId_copyFrom_Ori(_TXA_, doc, orFilter, actView, out group_TXA_textnote_ids, out _TXA_dimension_ids, "_TXA_", "_TXA_textnote_");
                    }
                    if (CMD._TXB_rvt != "null_path")
                    {
                        group_TXB_ids = Creat_Unit_GroupId_copyFrom_Ori(_TXB_, doc, orFilter, actView, out group_TXB_textnote_ids, out _TXB_dimension_ids, "_TXB_", "_TXB_textnote_");
                    }
                    if (CMD._TXC_rvt != "null_path")
                    {
                        group_TXC_ids = Creat_Unit_GroupId_copyFrom_Ori(_TXC_, doc, orFilter, actView, out group_TXC_textnote_ids, out _TXC_dimension_ids, "_TXC_", "_TXC_textnote_");
                    }
                    if (CMD._TXD_rvt != "null_path")
                    {
                        group_TXD_ids = Creat_Unit_GroupId_copyFrom_Ori(_TXD_, doc, orFilter, actView, out group_TXD_textnote_ids, out _TXD_dimension_ids, "_TXD_", "_TXD_textnote_");
                    }
                    if (CMD._TXA_HXT_rvt != "null_path")
                    {
                        group_TXA_Core_ids = Creat_Unit_GroupId_copyFrom_Ori(_TXA_Core_, doc, orFilter, actView, out group_TXA_Core_textnote_ids, out _TXA_Core_dimension_ids, "_TXA_Core_", "_TXA_Core_textnote_");
                    }
                    if (CMD._TXB_HXT_rvt != "null_path")
                    {
                        group_TXB_Core_ids = Creat_Unit_GroupId_copyFrom_Ori(_TXB_Core_, doc, orFilter, actView, out group_TXB_Core_textnote_ids, out _TXB_Core_dimension_ids, "_TXB_Core_", "_TXB_Core_textnote_");
                    }
                    if (CMD._TXC_HXT_rvt != "null_path")
                    {
                        group_TXC_Core_ids = Creat_Unit_GroupId_copyFrom_Ori(_TXC_Core_, doc, orFilter, actView, out group_TXC_Core_textnote_ids, out _TXC_Core_dimension_ids, "_TXC_Core_", "_TXC_Core_textnote_");
                    }
                    if (CMD._TXD_HXT_rvt != "null_path")
                    {
                        group_TXD_Core_ids = Creat_Unit_GroupId_copyFrom_Ori(_TXD_Core_, doc, orFilter, actView, out group_TXD_Core_textnote_ids, out _TXD_Core_dimension_ids, "_TXD_Core_", "_TXD_Core_textnote_");
                    }
                    #endregion

                    #region 在当前视图，对户型进行排列组合 注意模型复制到当前文档时，所处的位置 time_20200312
                    //abccbd 根据字母组合进行判断
                    double move_distance_TXA_up = GetGlobalParametersValue_double(_TXA_, "move_distance_up_global");//_TXB_移动的为_TXA_的缝隙距离
                    double move_distance_TXA_down = GetGlobalParametersValue_double(_TXA_, "move_distance_down_length_global");//_TXB_移动的为_TXA_的缝隙距离

                    if (CMD._TXA_rvt != "null_path")
                    {
                        //判断是否需要对复制进来的模型数据进行二次操作
                        ICollection<ElementId> mi_group_TXA_ids = _MirrorElements_ChangeHandler(doc, group_TXA_ids, mid_plane, true);
                        ICollection<ElementId> mi_group_TXA_textnote_ids = _MirrorElements_ChangeHandler(doc, group_TXA_textnote_ids, mid_plane, true);
                        ICollection<ElementId> mi_TXA_dimension_ids = _MirrorElements_ChangeHandler(doc, _TXA_dimension_ids, mid_plane, true);

                        ICollection<ElementId> _mi_group_TXA_ids = _MirrorElements_ChangeHandler(doc, group_TXA_ids, _mid_plane, true);
                        ICollection<ElementId> _mi_group_TXA_textnote_ids = _MirrorElements_ChangeHandler(doc, group_TXA_textnote_ids, _mid_plane, true);
                        ICollection<ElementId> _mi_TXA_dimension_ids = _MirrorElements_ChangeHandler(doc, _TXA_dimension_ids, _mid_plane, true);

                        ICollection<ElementId> __mi_group_TXA_ids = _MirrorElements_ChangeHandler(doc, mi_group_TXA_ids, _mid_plane, true);
                        ICollection<ElementId> __mi_group_TXA_textnote_ids = _MirrorElements_ChangeHandler(doc, mi_group_TXA_textnote_ids, _mid_plane, true);
                        ICollection<ElementId> __mi_TXA_dimension_ids = _MirrorElements_ChangeHandler(doc, mi_TXA_dimension_ids, _mid_plane, true);

                        //TaskDialog.Show("Revit", move_distance_TXA_.ToString());
                        MoveGrouIds(doc, -move_distance_TXA_down - move_distance_TXA_down, mi_group_TXA_ids);
                        MoveGrouIds(doc, -move_distance_TXA_down - move_distance_TXA_down, mi_group_TXA_textnote_ids);

                        MoveGrouIds(doc, -move_distance_TXA_down - move_distance_TXA_down, __mi_group_TXA_ids);
                        MoveGrouIds(doc, -move_distance_TXA_down - move_distance_TXA_down, __mi_group_TXA_textnote_ids);

                        MoveGrouIds(doc, -move_distance_TXA_down - move_distance_TXA_down - move_distance_TXA_down - move_distance_TXA_down, _mi_group_TXA_ids);
                        MoveGrouIds(doc, -move_distance_TXA_down - move_distance_TXA_down - move_distance_TXA_down - move_distance_TXA_down, _mi_group_TXA_textnote_ids);
                    }
                    if (CMD._TXB_rvt != "null_path")
                    {
                        //判断是否需要对复制进来的模型数据进行二次操作
                        ICollection<ElementId> mi_group_TXB_ids = _MirrorElements_ChangeHandler(doc, group_TXB_ids, mid_plane, true);
                        ICollection<ElementId> mi_group_TXB_textnote_ids = _MirrorElements_ChangeHandler(doc, group_TXB_textnote_ids, mid_plane, true);
                        ICollection<ElementId> mi_TXB_dimension_ids = _MirrorElements_ChangeHandler(doc, _TXB_dimension_ids, mid_plane, true);

                        ICollection<ElementId> _mi_group_TXB_ids = _MirrorElements_ChangeHandler(doc, group_TXB_ids, _mid_plane, true);
                        ICollection<ElementId> _mi_group_TXB_textnote_ids = _MirrorElements_ChangeHandler(doc, group_TXB_textnote_ids, _mid_plane, true);
                        ICollection<ElementId> _mi_TXB_dimension_ids = _MirrorElements_ChangeHandler(doc, _TXB_dimension_ids, _mid_plane, true);

                        ICollection<ElementId> __mi_group_TXB_ids = _MirrorElements_ChangeHandler(doc, mi_group_TXB_ids, _mid_plane, true);
                        ICollection<ElementId> __mi_group_TXB_textnote_ids = _MirrorElements_ChangeHandler(doc, mi_group_TXB_textnote_ids, _mid_plane, true);
                        ICollection<ElementId> __mi_TXB_dimension_ids = _MirrorElements_ChangeHandler(doc, mi_TXB_dimension_ids, _mid_plane, true);

                        MoveGrouIds(doc, -move_distance_TXA_down, group_TXB_ids);
                        MoveGrouIds(doc, -move_distance_TXA_down, group_TXB_textnote_ids);

                        MoveGrouIds(doc, -move_distance_TXA_down, mi_group_TXB_ids);
                        MoveGrouIds(doc, -move_distance_TXA_down, mi_group_TXB_textnote_ids);

                        MoveGrouIds(doc, -move_distance_TXA_down - move_distance_TXA_down - move_distance_TXA_down, _mi_group_TXB_ids);
                        MoveGrouIds(doc, -move_distance_TXA_down - move_distance_TXA_down - move_distance_TXA_down, _mi_group_TXB_textnote_ids);

                        MoveGrouIds(doc, -move_distance_TXA_down - move_distance_TXA_down - move_distance_TXA_down, __mi_group_TXB_ids);
                        MoveGrouIds(doc, -move_distance_TXA_down - move_distance_TXA_down - move_distance_TXA_down, __mi_group_TXB_textnote_ids);
                    }
                    if (CMD._TXC_rvt != "null_path")
                    {
                        //判断是否需要对复制进来的模型数据进行二次操作
                    }
                    if (CMD._TXD_rvt != "null_path")
                    {
                        //判断是否需要对复制进来的模型数据进行二次操作
                    }
                    if (CMD._TXA_HXT_rvt != "null_path")
                    {
                        //判断是否需要对复制进来的模型数据进行二次操作
                        ICollection<ElementId> _mi_group_TXA_Core_ids = _MirrorElements_ChangeHandler(doc, group_TXA_Core_ids, _mid_plane, true);//aaaA
                        ICollection<ElementId> _mi_group_TXA_Core_textnote_ids = _MirrorElements_ChangeHandler(doc, group_TXA_Core_textnote_ids, _mid_plane, true);//aaaA

                        if (!CMD._TXA_HXT_rvt.Contains(@"CORE_5408") && !CMD._TXA_HXT_rvt.Contains(@"CORE_335406"))//core_5408为剪刀楼梯 CORE_335406为1T4H模式
                        {
                            //该处主要原因为剪刀楼梯不需要复制
                            ICollection<ElementId> mi_group_TXA_Core_ids = _MirrorElements_ChangeHandler(doc, group_TXA_Core_ids, mid_plane, true);//aAaa
                            ICollection<ElementId> mi_group_TXA_Core_textnote_ids = _MirrorElements_ChangeHandler(doc, group_TXA_Core_textnote_ids, mid_plane, true);//aAaa

                            ICollection<ElementId> __mi_group_TXA_Core_ids = _MirrorElements_ChangeHandler(doc, mi_group_TXA_Core_ids, _mid_plane, true);//aaAa
                            ICollection<ElementId> __mi_group_TXA_Core_textnote_ids = _MirrorElements_ChangeHandler(doc, mi_group_TXA_Core_textnote_ids, _mid_plane, true);//aaAa

                            MoveGrouIds(doc, -move_distance_TXA_up, group_TXA_Core_ids);//Aaaa
                            MoveGrouIds(doc, -move_distance_TXA_up, group_TXA_Core_textnote_ids);//Aaaa
                            MoveGrouIds(doc, -move_distance_TXA_down - move_distance_TXA_down + move_distance_TXA_up, mi_group_TXA_Core_ids);//aAaa
                            MoveGrouIds(doc, -move_distance_TXA_down - move_distance_TXA_down + move_distance_TXA_up, mi_group_TXA_Core_textnote_ids);//aAaa

                            MoveGrouIds(doc, -move_distance_TXA_down - move_distance_TXA_down - move_distance_TXA_up, __mi_group_TXA_Core_ids);//aaAa
                            MoveGrouIds(doc, -move_distance_TXA_down - move_distance_TXA_down - move_distance_TXA_up, __mi_group_TXA_Core_textnote_ids);//aaAa      
                            MoveGrouIds(doc, -move_distance_TXA_down - move_distance_TXA_down - move_distance_TXA_down - move_distance_TXA_down + move_distance_TXA_up, _mi_group_TXA_Core_ids);//aaaA
                            MoveGrouIds(doc, -move_distance_TXA_down - move_distance_TXA_down - move_distance_TXA_down - move_distance_TXA_down + move_distance_TXA_up, _mi_group_TXA_Core_textnote_ids);//aaaA

                            //添加核心筒走廊_此处操作为_通过修改全局参数，控制核心筒走廊宽度的一半；
                            AddCorefloor(doc, _TXA_Core_, distance_TXB_X);
                        }
                        else if (CMD._TXA_HXT_rvt.Contains(@"CORE_335406") || CMD._TXA_HXT_rvt.Contains(@"CORE_5408"))
                        {
                            MoveGrouIds(doc, -move_distance_TXA_up, group_TXA_Core_ids);//Aaaa
                            MoveGrouIds(doc, -move_distance_TXA_up, group_TXA_Core_textnote_ids);//Aaaa
                            MoveGrouIds(doc, -move_distance_TXA_down - move_distance_TXA_down - move_distance_TXA_down - move_distance_TXA_down + move_distance_TXA_up, _mi_group_TXA_Core_ids);//aaaA
                            MoveGrouIds(doc, -move_distance_TXA_down - move_distance_TXA_down - move_distance_TXA_down - move_distance_TXA_down + move_distance_TXA_up, _mi_group_TXA_Core_textnote_ids);//aaaA

                        }
                    }
                    if (CMD._TXB_HXT_rvt != "null_path")
                    {
                        //判断是否需要对复制进来的模型数据进行二次操作
                    }
                    if (CMD._TXC_HXT_rvt != "null_path")
                    {
                        //判断是否需要对复制进来的模型数据进行二次操作
                    }
                    if (CMD._TXD_HXT_rvt != "null_path")
                    {
                        //判断是否需要对复制进来的模型数据进行二次操作
                    }
                    #endregion
                    transGroupCreateDwellingses.Assimilate();
                }
                else
                {
                    transGroupCreateDwellingses.RollBack();
                }
            }
            #region ***关闭所有源文档，但是不保存  time_20200312
            ResetDocument(_TXA_, _TXB_, _TXC_, _TXD_, _TXA_Core_, _TXB_Core_, _TXC_Core_, _TXD_Core_);
            #endregion
            #endregion
        }
        //新建户型_2T4H_abbc_1DY_ABC_
        public void Creat_HXZU_2T4H_abbc_1DY_ABC_(UIApplication uiapp)
        {
            #region all code
            #region ***初始化与revit模型进行交互的辅助变量 time_20200312
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Application app = uiapp.Application;
            Document doc = uidoc.Document;
            View actView = doc.ActiveView;
            View FL01_view = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Views).OfClass(typeof(ViewPlan)).FirstOrDefault(x => x.Name == "FL01") as View;
            Plane ori_plane = Plane.CreateByNormalAndOrigin(new XYZ(1, 0, 0), new XYZ(0, 0, 0));//该处为原点镜像轴
            Plane mid_plane = null;//该处mid定义为整体户型组合模式的中心对称轴位置
            LogicalOrFilter orFilter = CreatorFilter(uiapp);
            #endregion

            #region ***声明 all 变量 time_20200312
            Document _TXA_ = null;
            Document _TXB_ = null;
            Document _TXC_ = null;
            Document _TXD_ = null;
            Document _TXA_Core_ = null;
            Document _TXB_Core_ = null;
            Document _TXC_Core_ = null;
            Document _TXD_Core_ = null;
            ICollection<ElementId> group_TXA_ids = new List<ElementId>();//各个基础模型3D元素group的id_list
            ICollection<ElementId> group_TXB_ids = new List<ElementId>();
            ICollection<ElementId> group_TXC_ids = new List<ElementId>();
            ICollection<ElementId> group_TXD_ids = new List<ElementId>();
            ICollection<ElementId> group_TXA_Core_ids = new List<ElementId>();
            ICollection<ElementId> group_TXB_Core_ids = new List<ElementId>();
            ICollection<ElementId> group_TXC_Core_ids = new List<ElementId>();
            ICollection<ElementId> group_TXD_Core_ids = new List<ElementId>();
            ICollection<ElementId> group_TXA_textnote_ids = new List<ElementId>();//各个基础模型2D文字注释group的id_list
            ICollection<ElementId> group_TXB_textnote_ids = new List<ElementId>();
            ICollection<ElementId> group_TXC_textnote_ids = new List<ElementId>();
            ICollection<ElementId> group_TXD_textnote_ids = new List<ElementId>();
            ICollection<ElementId> group_TXA_Core_textnote_ids = new List<ElementId>();
            ICollection<ElementId> group_TXB_Core_textnote_ids = new List<ElementId>();
            ICollection<ElementId> group_TXC_Core_textnote_ids = new List<ElementId>();
            ICollection<ElementId> group_TXD_Core_textnote_ids = new List<ElementId>();
            ICollection<ElementId> _TXA_dimension_ids = new List<ElementId>();//由于2D标注元素，不可以成group，所以此处为各个标注元素的list
            ICollection<ElementId> _TXB_dimension_ids = new List<ElementId>();
            ICollection<ElementId> _TXC_dimension_ids = new List<ElementId>();
            ICollection<ElementId> _TXD_dimension_ids = new List<ElementId>();
            ICollection<ElementId> _TXA_Core_dimension_ids = new List<ElementId>();//注意，目前核心筒基础模型，不涉及标注元素,此外初始化，是为了满足函数参数的空值调用
            ICollection<ElementId> _TXB_Core_dimension_ids = new List<ElementId>();
            ICollection<ElementId> _TXC_Core_dimension_ids = new List<ElementId>();
            ICollection<ElementId> _TXD_Core_dimension_ids = new List<ElementId>();
            #endregion

            #region ***判断是否需要打开源文档 time_20200312
            //OpenTarDocument(uiapp, _TXA_, _TXB_, _TXC_, _TXD_, _TXA_Core_, _TXB_Core_, _TXC_Core_, _TXD_Core_);
            if (CMD._TXA_rvt != "null_path")
            {
                _TXA_ = app.OpenDocumentFile(CMD._TXA_rvt);
            }
            if (CMD._TXB_rvt != "null_path")
            {
                _TXB_ = app.OpenDocumentFile(CMD._TXB_rvt);
            }
            if (CMD._TXC_rvt != "null_path")
            {
                _TXC_ = app.OpenDocumentFile(CMD._TXC_rvt);
            }
            if (CMD._TXD_rvt != "null_path")
            {
                _TXD_ = app.OpenDocumentFile(CMD._TXD_rvt);
            }
            if (CMD._TXA_HXT_rvt != "null_path")
            {
                _TXA_Core_ = app.OpenDocumentFile(CMD._TXA_HXT_rvt);
            }
            if (CMD._TXB_HXT_rvt != "null_path")
            {
                _TXB_Core_ = app.OpenDocumentFile(CMD._TXB_HXT_rvt);
            }
            if (CMD._TXC_HXT_rvt != "null_path")
            {
                _TXC_Core_ = app.OpenDocumentFile(CMD._TXC_HXT_rvt);
            }
            if (CMD._TXD_HXT_rvt != "null_path")
            {
                _TXD_Core_ = app.OpenDocumentFile(CMD._TXD_HXT_rvt);
            }
            #endregion

            #region 根据户型组合模式 在各个源文件 定位元素 在这里不对核心筒定位做处理 原因：revit后台多次打开同一个文件，属于无效操作 time_20200312
            double distance_TXB_X = 0;

            if (CMD._TXA_rvt != "null_path")
            {
                //判断是否需要对基础模型进行定位操作
            }
            if (CMD._TXB_rvt != "null_path")
            {
                ICollection<ElementId> _TXB_3D_EleIds_temp = (new FilteredElementCollector(_TXB_)).WherePasses(orFilter).WhereElementIsNotElementType().ToElementIds();
                FL01_view = new FilteredElementCollector(_TXB_).OfCategory(BuiltInCategory.OST_Views).OfClass(typeof(ViewPlan)).FirstOrDefault(x => x.Name == "FL01") as View;
                ICollection<ElementId> _TXB_TextNotes_temp = (new FilteredElementCollector(_TXB_, FL01_view.Id)).OwnedByView(FL01_view.Id).OfCategory(BuiltInCategory.OST_TextNotes).WhereElementIsNotElementType().ToElementIds();
                ICollection<ElementId> _TXB_Dimensions_temp = (new FilteredElementCollector(_TXB_, FL01_view.Id)).OwnedByView(FL01_view.Id).OfCategory(BuiltInCategory.OST_Dimensions).WhereElementIsNotElementType().ToElementIds();
                //判断是否需要对基础模型进行定位操作

                double _aBbc_max_X;
                double _aBbc_min_X = GetLeftRgiht_X_ingrid(_TXB_, _TXB_3D_EleIds_temp, out _aBbc_max_X);
                distance_TXB_X = _aBbc_max_X - _aBbc_min_X;
                mid_plane = Plane.CreateByNormalAndOrigin(new XYZ(_aBbc_max_X + 1, 0, 0), new XYZ(_aBbc_max_X, 0, 0));
            }
            if (CMD._TXC_rvt != "null_path")
            {
                ICollection<ElementId> _TXC_3D_EleIds_temp = (new FilteredElementCollector(_TXC_)).WherePasses(orFilter).WhereElementIsNotElementType().ToElementIds();
                FL01_view = new FilteredElementCollector(_TXC_).OfCategory(BuiltInCategory.OST_Views).OfClass(typeof(ViewPlan)).FirstOrDefault(x => x.Name == "FL01") as View;
                ICollection<ElementId> _TXC_TextNotes_temp = (new FilteredElementCollector(_TXC_, FL01_view.Id)).OwnedByView(FL01_view.Id).OfCategory(BuiltInCategory.OST_TextNotes).WhereElementIsNotElementType().ToElementIds();
                ICollection<ElementId> _TXC_Dimensions_temp = (new FilteredElementCollector(_TXC_, FL01_view.Id)).OwnedByView(FL01_view.Id).OfCategory(BuiltInCategory.OST_Dimensions).WhereElementIsNotElementType().ToElementIds();
                //判断是否需要对基础模型进行定位操作
                ICollection<ElementId> mi_TXC_3D_EleIds_temp = _MirrorElements_ChangeHandler(_TXC_, _TXC_3D_EleIds_temp, mid_plane, true);
                ICollection<ElementId> mi_TXC_TextNotes_temp = _MirrorElements_ChangeHandler(_TXC_, _TXC_TextNotes_temp, mid_plane, true);
                ICollection<ElementId> mi_TXC_Dimensions_temp = _MirrorElements_ChangeHandler(_TXC_, _TXC_Dimensions_temp, mid_plane, true);
                _deleteEles(_TXC_, _TXC_3D_EleIds_temp);
                _deleteEles(_TXC_, _TXC_TextNotes_temp);
            }
            if (CMD._TXD_rvt != "null_path")
            {
                //判断是否需要对基础模型进行定位操作
            }
            #endregion

            using (TransactionGroup transGroupCreateDwellingses = new TransactionGroup(doc, "创建户型零件组_group"))//开展事务组
            {
                if (transGroupCreateDwellingses.Start() == TransactionStatus.Started)//开启事务组
                {
                    #region ***将定位好的基础模型数据，复制到当前工作视图 并creat group 标注元素只有list，不可以creat group time_20200312
                    if (CMD._TXA_rvt != "null_path")
                    {
                        group_TXA_ids = Creat_Unit_GroupId_copyFrom_Ori(_TXA_, doc, orFilter, actView, out group_TXA_textnote_ids, out _TXA_dimension_ids, "_TXA_", "_TXA_textnote_");
                    }
                    if (CMD._TXB_rvt != "null_path")
                    {
                        group_TXB_ids = Creat_Unit_GroupId_copyFrom_Ori(_TXB_, doc, orFilter, actView, out group_TXB_textnote_ids, out _TXB_dimension_ids, "_TXB_", "_TXB_textnote_");
                    }
                    if (CMD._TXC_rvt != "null_path")
                    {
                        group_TXC_ids = Creat_Unit_GroupId_copyFrom_Ori(_TXC_, doc, orFilter, actView, out group_TXC_textnote_ids, out _TXC_dimension_ids, "_TXC_", "_TXC_textnote_");
                    }
                    if (CMD._TXD_rvt != "null_path")
                    {
                        group_TXD_ids = Creat_Unit_GroupId_copyFrom_Ori(_TXD_, doc, orFilter, actView, out group_TXD_textnote_ids, out _TXD_dimension_ids, "_TXD_", "_TXD_textnote_");
                    }
                    if (CMD._TXA_HXT_rvt != "null_path")
                    {
                        group_TXA_Core_ids = Creat_Unit_GroupId_copyFrom_Ori(_TXA_Core_, doc, orFilter, actView, out group_TXA_Core_textnote_ids, out _TXA_Core_dimension_ids, "_TXA_Core_", "_TXA_Core_textnote_");
                    }
                    if (CMD._TXB_HXT_rvt != "null_path")
                    {
                        group_TXB_Core_ids = Creat_Unit_GroupId_copyFrom_Ori(_TXB_Core_, doc, orFilter, actView, out group_TXB_Core_textnote_ids, out _TXB_Core_dimension_ids, "_TXB_Core_", "_TXB_Core_textnote_");
                    }
                    if (CMD._TXC_HXT_rvt != "null_path")
                    {
                        group_TXC_Core_ids = Creat_Unit_GroupId_copyFrom_Ori(_TXC_Core_, doc, orFilter, actView, out group_TXC_Core_textnote_ids, out _TXC_Core_dimension_ids, "_TXC_Core_", "_TXC_Core_textnote_");
                    }
                    if (CMD._TXD_HXT_rvt != "null_path")
                    {
                        group_TXD_Core_ids = Creat_Unit_GroupId_copyFrom_Ori(_TXD_Core_, doc, orFilter, actView, out group_TXD_Core_textnote_ids, out _TXD_Core_dimension_ids, "_TXD_Core_", "_TXD_Core_textnote_");
                    }
                    #endregion

                    #region 在当前视图，对户型进行排列组合 注意模型复制到当前文档时，所处的位置 time_20200312
                    //abccbd 根据字母组合进行判断
                    double move_distance_TXA_up = GetGlobalParametersValue_double(_TXA_, "move_distance_up_length_global");//_TXB_移动的为_TXA_的缝隙距离
                    double move_distance_TXA_down = GetGlobalParametersValue_double(_TXA_, "move_distance_down_length_global");//_TXB_移动的为_TXA_的缝隙距离
                    double move_distance_TXC_up = GetGlobalParametersValue_double(_TXC_, "move_distance_up_length_global");//_TXB_移动的为_TXA_的缝隙距离
                    double move_distance_TXC_down = GetGlobalParametersValue_double(_TXC_, "move_distance_down_length_global");//_TXB_移动的为_TXA_的缝隙距离

                    if (CMD._TXA_rvt != "null_path")
                    {
                        //判断是否需要对复制进来的模型数据进行二次操作
                    }
                    if (CMD._TXB_rvt != "null_path")
                    {
                        //判断是否需要对复制进来的模型数据进行二次操作
                        ICollection<ElementId> mi_group_TXB_ids = _MirrorElements_ChangeHandler(doc, group_TXB_ids, mid_plane, true);
                        ICollection<ElementId> mi_group_TXB_textnote_ids = _MirrorElements_ChangeHandler(doc, group_TXB_textnote_ids, mid_plane, true);
                        ICollection<ElementId> mi_TXB_dimension_ids = _MirrorElements_ChangeHandler(doc, _TXB_dimension_ids, mid_plane, true);

                        //TaskDialog.Show("Revit", move_distance_TXA_.ToString());
                        MoveGrouIds(doc, -move_distance_TXA_down, group_TXB_ids);
                        MoveGrouIds(doc, -move_distance_TXA_down, group_TXB_textnote_ids);

                        MoveGrouIds(doc, -move_distance_TXA_down, mi_group_TXB_ids);
                        MoveGrouIds(doc, -move_distance_TXA_down, mi_group_TXB_textnote_ids);
                    }
                    if (CMD._TXC_rvt != "null_path")
                    {
                        //判断是否需要对复制进来的模型数据进行二次操作
                        MoveGrouIds(doc, -move_distance_TXA_down - move_distance_TXC_down, group_TXC_ids);
                        MoveGrouIds(doc, -move_distance_TXA_down - move_distance_TXC_down, group_TXC_textnote_ids);
                    }
                    if (CMD._TXD_rvt != "null_path")
                    {
                        //判断是否需要对复制进来的模型数据进行二次操作
                    }
                    if (CMD._TXA_HXT_rvt != "null_path")//核心筒是先镜像后移动
                    {
                        //判断是否需要对复制进来的模型数据进行二次操作
                        if (!CMD._TXA_HXT_rvt.Contains(@"CORE_5408") && !CMD._TXA_HXT_rvt.Contains(@"CORE_335406"))//core_5408为剪刀楼梯 CORE_335406为1T4H模式
                        {
                            //该处主要原因为剪刀楼梯不需要复制
                            ICollection<ElementId> mi_group_TXA_Core_ids = _MirrorElements_ChangeHandler(doc, group_TXA_Core_ids, mid_plane, true);
                            ICollection<ElementId> mi_group_TXA_Core_textnote_ids = _MirrorElements_ChangeHandler(doc, group_TXA_Core_textnote_ids, mid_plane, true);

                            MoveGrouIds(doc, -move_distance_TXA_up, group_TXA_Core_ids);
                            MoveGrouIds(doc, -move_distance_TXA_up, group_TXA_Core_textnote_ids);
                            MoveGrouIds(doc, -move_distance_TXA_down - move_distance_TXC_down + move_distance_TXC_up, mi_group_TXA_Core_ids);
                            MoveGrouIds(doc, -move_distance_TXA_down - move_distance_TXC_down + move_distance_TXC_up, mi_group_TXA_Core_textnote_ids);

                            //添加核心筒走廊_此处操作为_通过修改全局参数，控制核心筒走廊宽度的一半；
                            AddCorefloor(doc, _TXA_Core_, distance_TXB_X);
                        }
                        else if (CMD._TXA_HXT_rvt.Contains(@"CORE_335406") || CMD._TXA_HXT_rvt.Contains(@"CORE_335406"))
                        {
                            //需要对1T4H的核心筒进行移动 由于镜像原因，一个向左移动，一个向右移动
                            MoveGrouIds(doc, -move_distance_TXA_down, group_TXA_Core_ids);
                            MoveGrouIds(doc, -move_distance_TXA_down, group_TXA_Core_textnote_ids);
                        }
                    }
                    if (CMD._TXB_HXT_rvt != "null_path")
                    {
                        //判断是否需要对复制进来的模型数据进行二次操作
                    }
                    if (CMD._TXC_HXT_rvt != "null_path")
                    {
                        //判断是否需要对复制进来的模型数据进行二次操作
                    }
                    if (CMD._TXD_HXT_rvt != "null_path")
                    {
                        //判断是否需要对复制进来的模型数据进行二次操作
                    }
                    #endregion
                    transGroupCreateDwellingses.Assimilate();
                }
                else
                {
                    transGroupCreateDwellingses.RollBack();
                }
            }
            #region ***关闭所有源文档，但是不保存  time_20200312
            ResetDocument(_TXA_, _TXB_, _TXC_, _TXD_, _TXA_Core_, _TXB_Core_, _TXC_Core_, _TXD_Core_);
            #endregion
            #endregion
        }
        //新建户型_2T4H_abbccbba_2DY_ABC_
        public void Creat_HXZU_2T4H_abbccbba_2DY_ABC_(UIApplication uiapp)
        {
            #region all code
            #region ***初始化与revit模型进行交互的辅助变量 time_20200312
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Application app = uiapp.Application;
            Document doc = uidoc.Document;
            View actView = doc.ActiveView;
            View FL01_view = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Views).OfClass(typeof(ViewPlan)).FirstOrDefault(x => x.Name == "FL01") as View;
            Plane ori_plane = Plane.CreateByNormalAndOrigin(new XYZ(1, 0, 0), new XYZ(0, 0, 0));//该处为原点镜像轴
            Plane mid_plane = null;//该处mid定义为整体户型组合模式的中心对称轴位置
            LogicalOrFilter orFilter = CreatorFilter(uiapp);
            #endregion

            #region ***声明 all 变量 time_20200312
            Document _TXA_ = null;
            Document _TXB_ = null;
            Document _TXC_ = null;
            Document _TXD_ = null;
            Document _TXA_Core_ = null;
            Document _TXB_Core_ = null;
            Document _TXC_Core_ = null;
            Document _TXD_Core_ = null;
            ICollection<ElementId> group_TXA_ids = new List<ElementId>();//各个基础模型3D元素group的id_list
            ICollection<ElementId> group_TXB_ids = new List<ElementId>();
            ICollection<ElementId> group_TXC_ids = new List<ElementId>();
            ICollection<ElementId> group_TXD_ids = new List<ElementId>();
            ICollection<ElementId> group_TXA_Core_ids = new List<ElementId>();
            ICollection<ElementId> group_TXB_Core_ids = new List<ElementId>();
            ICollection<ElementId> group_TXC_Core_ids = new List<ElementId>();
            ICollection<ElementId> group_TXD_Core_ids = new List<ElementId>();
            ICollection<ElementId> group_TXA_textnote_ids = new List<ElementId>();//各个基础模型2D文字注释group的id_list
            ICollection<ElementId> group_TXB_textnote_ids = new List<ElementId>();
            ICollection<ElementId> group_TXC_textnote_ids = new List<ElementId>();
            ICollection<ElementId> group_TXD_textnote_ids = new List<ElementId>();
            ICollection<ElementId> group_TXA_Core_textnote_ids = new List<ElementId>();
            ICollection<ElementId> group_TXB_Core_textnote_ids = new List<ElementId>();
            ICollection<ElementId> group_TXC_Core_textnote_ids = new List<ElementId>();
            ICollection<ElementId> group_TXD_Core_textnote_ids = new List<ElementId>();
            ICollection<ElementId> _TXA_dimension_ids = new List<ElementId>();//由于2D标注元素，不可以成group，所以此处为各个标注元素的list
            ICollection<ElementId> _TXB_dimension_ids = new List<ElementId>();
            ICollection<ElementId> _TXC_dimension_ids = new List<ElementId>();
            ICollection<ElementId> _TXD_dimension_ids = new List<ElementId>();
            ICollection<ElementId> _TXA_Core_dimension_ids = new List<ElementId>();//注意，目前核心筒基础模型，不涉及标注元素,此外初始化，是为了满足函数参数的空值调用
            ICollection<ElementId> _TXB_Core_dimension_ids = new List<ElementId>();
            ICollection<ElementId> _TXC_Core_dimension_ids = new List<ElementId>();
            ICollection<ElementId> _TXD_Core_dimension_ids = new List<ElementId>();
            #endregion

            #region ***判断是否需要打开源文档 time_20200312
            //OpenTarDocument(uiapp, _TXA_, _TXB_, _TXC_, _TXD_, _TXA_Core_, _TXB_Core_, _TXC_Core_, _TXD_Core_);
            if (CMD._TXA_rvt != "null_path")
            {
                _TXA_ = app.OpenDocumentFile(CMD._TXA_rvt);
            }
            if (CMD._TXB_rvt != "null_path")
            {
                _TXB_ = app.OpenDocumentFile(CMD._TXB_rvt);
            }
            if (CMD._TXC_rvt != "null_path")
            {
                _TXC_ = app.OpenDocumentFile(CMD._TXC_rvt);
            }
            if (CMD._TXD_rvt != "null_path")
            {
                _TXD_ = app.OpenDocumentFile(CMD._TXD_rvt);
            }
            if (CMD._TXA_HXT_rvt != "null_path")
            {
                _TXA_Core_ = app.OpenDocumentFile(CMD._TXA_HXT_rvt);
            }
            if (CMD._TXB_HXT_rvt != "null_path")
            {
                _TXB_Core_ = app.OpenDocumentFile(CMD._TXB_HXT_rvt);
            }
            if (CMD._TXC_HXT_rvt != "null_path")
            {
                _TXC_Core_ = app.OpenDocumentFile(CMD._TXC_HXT_rvt);
            }
            if (CMD._TXD_HXT_rvt != "null_path")
            {
                _TXD_Core_ = app.OpenDocumentFile(CMD._TXD_HXT_rvt);
            }
            #endregion

            #region 根据户型组合模式 在各个源文件 定位元素 在这里不对核心筒定位做处理 原因：revit后台多次打开同一个文件，属于无效操作 time_20200312

            double distance_TXB_X = 0;
            double distance_TXC_X = 0;
            Plane _mid_plane = null;

            if (CMD._TXA_rvt != "null_path")
            {
                //判断是否需要对基础模型进行定位操作
            }
            if (CMD._TXB_rvt != "null_path")
            {
                ICollection<ElementId> _TXB_3D_EleIds_temp = (new FilteredElementCollector(_TXB_)).WherePasses(orFilter).WhereElementIsNotElementType().ToElementIds();
                FL01_view = new FilteredElementCollector(_TXB_).OfCategory(BuiltInCategory.OST_Views).OfClass(typeof(ViewPlan)).FirstOrDefault(x => x.Name == "FL01") as View;
                ICollection<ElementId> _TXB_TextNotes_temp = (new FilteredElementCollector(_TXB_, FL01_view.Id)).OwnedByView(FL01_view.Id).OfCategory(BuiltInCategory.OST_TextNotes).WhereElementIsNotElementType().ToElementIds();
                ICollection<ElementId> _TXB_Dimensions_temp = (new FilteredElementCollector(_TXB_, FL01_view.Id)).OwnedByView(FL01_view.Id).OfCategory(BuiltInCategory.OST_Dimensions).WhereElementIsNotElementType().ToElementIds();
                //判断是否需要对基础模型进行定位操作

                double _aBbccbba_max_X;
                double _aBbccbba_min_X = GetLeftRgiht_X_ingrid(_TXB_, _TXB_3D_EleIds_temp, out _aBbccbba_max_X);
                distance_TXB_X = _aBbccbba_max_X - _aBbccbba_min_X;

                mid_plane = Plane.CreateByNormalAndOrigin(new XYZ(distance_TXB_X + 1, 0, 0), new XYZ(distance_TXB_X, 0, 0));
            }
            if (CMD._TXC_rvt != "null_path")
            {
                ICollection<ElementId> _TXC_3D_EleIds_temp = (new FilteredElementCollector(_TXC_)).WherePasses(orFilter).WhereElementIsNotElementType().ToElementIds();
                FL01_view = new FilteredElementCollector(_TXC_).OfCategory(BuiltInCategory.OST_Views).OfClass(typeof(ViewPlan)).FirstOrDefault(x => x.Name == "FL01") as View;
                ICollection<ElementId> _TXC_TextNotes_temp = (new FilteredElementCollector(_TXC_, FL01_view.Id)).OwnedByView(FL01_view.Id).OfCategory(BuiltInCategory.OST_TextNotes).WhereElementIsNotElementType().ToElementIds();
                ICollection<ElementId> _TXC_Dimensions_temp = (new FilteredElementCollector(_TXC_, FL01_view.Id)).OwnedByView(FL01_view.Id).OfCategory(BuiltInCategory.OST_Dimensions).WhereElementIsNotElementType().ToElementIds();
                //判断是否需要对基础模型进行定位操作
                ICollection<ElementId> mi_TXC_3D_EleIds_temp = _MirrorElements_ChangeHandler(_TXC_, _TXC_3D_EleIds_temp, mid_plane, true);
                ICollection<ElementId> mi_TXC_TextNotes_temp = _MirrorElements_ChangeHandler(_TXC_, _TXC_TextNotes_temp, mid_plane, true);
                ICollection<ElementId> mi_TXC_Dimensions_temp = _MirrorElements_ChangeHandler(_TXC_, _TXC_Dimensions_temp, mid_plane, true);
                _deleteEles(_TXC_, _TXC_3D_EleIds_temp);
                _deleteEles(_TXC_, _TXC_TextNotes_temp);
                double _abbCcbba_max_X;
                double _abbCcbba_min_X = GetLeftRgiht_X_ingrid(_TXC_, mi_TXC_3D_EleIds_temp, out _abbCcbba_max_X);
                distance_TXC_X = _abbCcbba_max_X - _abbCcbba_min_X;
                _mid_plane = Plane.CreateByNormalAndOrigin(new XYZ(distance_TXB_X + distance_TXB_X + distance_TXC_X + 1, 0, 0), new XYZ(distance_TXB_X + distance_TXB_X + distance_TXC_X, 0, 0));
            }
            if (CMD._TXD_rvt != "null_path")
            {
                //判断是否需要对基础模型进行定位操作
            }
            #endregion

            using (TransactionGroup transGroupCreateDwellingses = new TransactionGroup(doc, "创建户型零件组_group"))//开展事务组
            {
                if (transGroupCreateDwellingses.Start() == TransactionStatus.Started)//开启事务组
                {
                    #region ***将定位好的基础模型数据，复制到当前工作视图 并creat group 标注元素只有list，不可以creat group time_20200312
                    if (CMD._TXA_rvt != "null_path")
                    {
                        group_TXA_ids = Creat_Unit_GroupId_copyFrom_Ori(_TXA_, doc, orFilter, actView, out group_TXA_textnote_ids, out _TXA_dimension_ids, "_TXA_", "_TXA_textnote_");
                    }
                    if (CMD._TXB_rvt != "null_path")
                    {
                        group_TXB_ids = Creat_Unit_GroupId_copyFrom_Ori(_TXB_, doc, orFilter, actView, out group_TXB_textnote_ids, out _TXB_dimension_ids, "_TXB_", "_TXB_textnote_");
                    }
                    if (CMD._TXC_rvt != "null_path")
                    {
                        group_TXC_ids = Creat_Unit_GroupId_copyFrom_Ori(_TXC_, doc, orFilter, actView, out group_TXC_textnote_ids, out _TXC_dimension_ids, "_TXC_", "_TXC_textnote_");
                    }
                    if (CMD._TXD_rvt != "null_path")
                    {
                        group_TXD_ids = Creat_Unit_GroupId_copyFrom_Ori(_TXD_, doc, orFilter, actView, out group_TXD_textnote_ids, out _TXD_dimension_ids, "_TXD_", "_TXD_textnote_");
                    }
                    if (CMD._TXA_HXT_rvt != "null_path")
                    {
                        group_TXA_Core_ids = Creat_Unit_GroupId_copyFrom_Ori(_TXA_Core_, doc, orFilter, actView, out group_TXA_Core_textnote_ids, out _TXA_Core_dimension_ids, "_TXA_Core_", "_TXA_Core_textnote_");
                    }
                    if (CMD._TXB_HXT_rvt != "null_path")
                    {
                        group_TXB_Core_ids = Creat_Unit_GroupId_copyFrom_Ori(_TXB_Core_, doc, orFilter, actView, out group_TXB_Core_textnote_ids, out _TXB_Core_dimension_ids, "_TXB_Core_", "_TXB_Core_textnote_");
                    }
                    if (CMD._TXC_HXT_rvt != "null_path")
                    {
                        group_TXC_Core_ids = Creat_Unit_GroupId_copyFrom_Ori(_TXC_Core_, doc, orFilter, actView, out group_TXC_Core_textnote_ids, out _TXC_Core_dimension_ids, "_TXC_Core_", "_TXC_Core_textnote_");
                    }
                    if (CMD._TXD_HXT_rvt != "null_path")
                    {
                        group_TXD_Core_ids = Creat_Unit_GroupId_copyFrom_Ori(_TXD_Core_, doc, orFilter, actView, out group_TXD_Core_textnote_ids, out _TXD_Core_dimension_ids, "_TXD_Core_", "_TXD_Core_textnote_");
                    }
                    #endregion

                    #region 在当前视图，对户型进行排列组合 注意模型复制到当前文档时，所处的位置 time_20200312
                    //abccbd 根据字母组合进行判断
                    double move_distance_TXA_up = GetGlobalParametersValue_double(_TXA_, "move_distance_up_length_global");//_TXB_移动的为_TXA_的缝隙距离
                    double move_distance_TXA_down = GetGlobalParametersValue_double(_TXA_, "move_distance_down_length_global");//_TXB_移动的为_TXA_的缝隙距离
                    double move_distance_TXC_up = GetGlobalParametersValue_double(_TXC_, "move_distance_up_length_global");//_TXB_移动的为_TXA_的缝隙距离
                    double move_distance_TXC_down = GetGlobalParametersValue_double(_TXC_, "move_distance_down_length_global");//_TXB_移动的为_TXA_的缝隙距离

                    if (CMD._TXA_rvt != "null_path")
                    {
                        //判断是否需要对复制进来的模型数据进行二次操作
                        ICollection<ElementId> mi_group_TXA_ids = _MirrorElements_ChangeHandler(doc, group_TXA_ids, _mid_plane, true);
                        ICollection<ElementId> mi_group_TXA_textnote_ids = _MirrorElements_ChangeHandler(doc, group_TXA_textnote_ids, _mid_plane, true);
                        ICollection<ElementId> mi_TXA_dimension_ids = _MirrorElements_ChangeHandler(doc, _TXA_dimension_ids, _mid_plane, true);

                        MoveGrouIds(doc, -move_distance_TXA_down - move_distance_TXC_down - move_distance_TXC_down - move_distance_TXA_down, mi_group_TXA_ids);
                        MoveGrouIds(doc, -move_distance_TXA_down - move_distance_TXC_down - move_distance_TXC_down - move_distance_TXA_down, mi_group_TXA_textnote_ids);
                    }
                    if (CMD._TXB_rvt != "null_path")
                    {
                        //判断是否需要对复制进来的模型数据进行二次操作
                        ICollection<ElementId> mi_group_TXB_ids = _MirrorElements_ChangeHandler(doc, group_TXB_ids, mid_plane, true);
                        ICollection<ElementId> mi_group_TXB_textnote_ids = _MirrorElements_ChangeHandler(doc, group_TXB_textnote_ids, mid_plane, true);
                        ICollection<ElementId> mi_TXB_dimension_ids = _MirrorElements_ChangeHandler(doc, _TXB_dimension_ids, mid_plane, true);

                        ICollection<ElementId> __mi_group_TXB_ids = _MirrorElements_ChangeHandler(doc, mi_group_TXB_ids, _mid_plane, true);
                        ICollection<ElementId> __mi_group_TXB_textnote_ids = _MirrorElements_ChangeHandler(doc, mi_group_TXB_textnote_ids, _mid_plane, true);
                        ICollection<ElementId> __mi_TXB_dimension_ids = _MirrorElements_ChangeHandler(doc, mi_TXB_dimension_ids, _mid_plane, true);

                        ICollection<ElementId> _mi_group_TXB_ids = _MirrorElements_ChangeHandler(doc, group_TXB_ids, _mid_plane, true);
                        ICollection<ElementId> _mi_group_TXB_textnote_ids = _MirrorElements_ChangeHandler(doc, group_TXB_textnote_ids, _mid_plane, true);
                        ICollection<ElementId> _mi_TXB_dimension_ids = _MirrorElements_ChangeHandler(doc, _TXB_dimension_ids, _mid_plane, true);

                        MoveGrouIds(doc, -move_distance_TXA_down, group_TXB_ids);
                        MoveGrouIds(doc, -move_distance_TXA_down, group_TXB_textnote_ids);
                        MoveGrouIds(doc, -move_distance_TXA_down, mi_group_TXB_ids);
                        MoveGrouIds(doc, -move_distance_TXA_down, mi_group_TXB_textnote_ids);
                        MoveGrouIds(doc, -move_distance_TXA_down - move_distance_TXC_down - move_distance_TXC_down, __mi_group_TXB_ids);
                        MoveGrouIds(doc, -move_distance_TXA_down - move_distance_TXC_down - move_distance_TXC_down, __mi_group_TXB_textnote_ids);
                        MoveGrouIds(doc, -move_distance_TXA_down - move_distance_TXC_down - move_distance_TXC_down, _mi_group_TXB_ids);
                        MoveGrouIds(doc, -move_distance_TXA_down - move_distance_TXC_down - move_distance_TXC_down, _mi_group_TXB_textnote_ids);
                    }
                    if (CMD._TXC_rvt != "null_path")
                    {
                        //判断是否需要对复制进来的模型数据进行二次操作
                        ICollection<ElementId> mi_group_TXC_ids = _MirrorElements_ChangeHandler(doc, group_TXC_ids, _mid_plane, true);
                        ICollection<ElementId> mi_group_TXC_textnote_ids = _MirrorElements_ChangeHandler(doc, group_TXC_textnote_ids, _mid_plane, true);
                        ICollection<ElementId> mi_TXC_dimension_ids = _MirrorElements_ChangeHandler(doc, _TXC_dimension_ids, _mid_plane, true);

                        MoveGrouIds(doc, -move_distance_TXA_down - move_distance_TXC_down, group_TXC_ids);
                        MoveGrouIds(doc, -move_distance_TXA_down - move_distance_TXC_down, group_TXC_textnote_ids);

                        MoveGrouIds(doc, -move_distance_TXA_down - move_distance_TXC_down, mi_group_TXC_ids);
                        MoveGrouIds(doc, -move_distance_TXA_down - move_distance_TXC_down, mi_group_TXC_textnote_ids);
                    }
                    if (CMD._TXD_rvt != "null_path")
                    {
                        //判断是否需要对复制进来的模型数据进行二次操作
                    }
                    if (CMD._TXA_HXT_rvt != "null_path")
                    {
                        //判断是否需要对复制进来的模型数据进行二次操作
                        ICollection<ElementId> _mi_group_TXA_Core_ids = _MirrorElements_ChangeHandler(doc, group_TXA_Core_ids, _mid_plane, true);
                        ICollection<ElementId> _mi_group_TXA_Core_textnote_ids = _MirrorElements_ChangeHandler(doc, group_TXA_Core_textnote_ids, _mid_plane, true);

                        if (!CMD._TXA_HXT_rvt.Contains(@"CORE_5408") && !CMD._TXA_HXT_rvt.Contains(@"CORE_335406"))//core_5408为剪刀楼梯 CORE_335406为1T4H模式
                        {
                            //该处主要原因为剪刀楼梯不需要复制
                            ICollection<ElementId> mi_group_TXA_Core_ids = _MirrorElements_ChangeHandler(doc, group_TXA_Core_ids, mid_plane, true);
                            ICollection<ElementId> mi_group_TXA_Core_textnote_ids = _MirrorElements_ChangeHandler(doc, group_TXA_Core_textnote_ids, mid_plane, true);

                            ICollection<ElementId> __mi_group_TXA_Core_ids = _MirrorElements_ChangeHandler(doc, mi_group_TXA_Core_ids, _mid_plane, true);
                            ICollection<ElementId> __mi_group_TXA_Core_textnote_ids = _MirrorElements_ChangeHandler(doc, mi_group_TXA_Core_textnote_ids, _mid_plane, true);

                            MoveGrouIds(doc, -move_distance_TXA_up, group_TXA_Core_ids);//Aaaa
                            MoveGrouIds(doc, -move_distance_TXA_up, group_TXA_Core_textnote_ids);//Aaaa
                            MoveGrouIds(doc, -move_distance_TXA_down - move_distance_TXC_down + move_distance_TXC_up, mi_group_TXA_Core_ids);//aAaa
                            MoveGrouIds(doc, -move_distance_TXA_down - move_distance_TXC_down + move_distance_TXC_up, mi_group_TXA_Core_textnote_ids);//aAaa

                            MoveGrouIds(doc, -move_distance_TXA_down - move_distance_TXC_down - move_distance_TXC_up, __mi_group_TXA_Core_ids);//aaAa
                            MoveGrouIds(doc, -move_distance_TXA_down - move_distance_TXC_down - move_distance_TXC_up, __mi_group_TXA_Core_textnote_ids);//aaAa      
                            MoveGrouIds(doc, -move_distance_TXA_down - move_distance_TXC_down - move_distance_TXC_down - move_distance_TXA_down + move_distance_TXA_up, _mi_group_TXA_Core_ids);//aaaA
                            MoveGrouIds(doc, -move_distance_TXA_down - move_distance_TXC_down - move_distance_TXC_down - move_distance_TXA_down + move_distance_TXA_up, _mi_group_TXA_Core_textnote_ids);//aaaA

                            //添加核心筒走廊_此处操作为_通过修改全局参数，控制核心筒走廊宽度的一半；
                            AddCorefloor(doc, _TXA_Core_, distance_TXB_X);
                        }
                        else if (CMD._TXA_HXT_rvt.Contains(@"CORE_335406") || CMD._TXA_HXT_rvt.Contains(@"CORE_5408"))
                        {
                            //无操作
                            MoveGrouIds(doc, -move_distance_TXA_up, group_TXA_Core_ids);//Aaaa
                            MoveGrouIds(doc, -move_distance_TXA_up, group_TXA_Core_textnote_ids);//Aaaa
                            MoveGrouIds(doc, -move_distance_TXA_down - move_distance_TXC_down - move_distance_TXC_down - move_distance_TXA_down + move_distance_TXA_up, _mi_group_TXA_Core_ids);//aaaA
                            MoveGrouIds(doc, -move_distance_TXA_down - move_distance_TXC_down - move_distance_TXC_down - move_distance_TXA_down + move_distance_TXA_up, _mi_group_TXA_Core_textnote_ids);//aaaA

                        }

                    }
                    if (CMD._TXB_HXT_rvt != "null_path")
                    {
                        //判断是否需要对复制进来的模型数据进行二次操作
                    }
                    if (CMD._TXC_HXT_rvt != "null_path")
                    {
                        //判断是否需要对复制进来的模型数据进行二次操作
                    }
                    if (CMD._TXD_HXT_rvt != "null_path")
                    {
                        //判断是否需要对复制进来的模型数据进行二次操作
                    }
                    #endregion
                    transGroupCreateDwellingses.Assimilate();
                }
                else
                {
                    transGroupCreateDwellingses.RollBack();
                }
            }
            #region ***关闭所有源文档，但是不保存  time_20200312
            ResetDocument(_TXA_, _TXB_, _TXC_, _TXD_, _TXA_Core_, _TXB_Core_, _TXC_Core_, _TXD_Core_);
            #endregion
            #endregion
        }
        //新建户型_2T4H_abbaabbc_2DY_ABC_
        public void Creat_HXZU_2T4H_abbaabbc_2DY_ABC_(UIApplication uiapp)
        {
            #region all code
            #region ***初始化与revit模型进行交互的辅助变量 time_20200312
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Application app = uiapp.Application;
            Document doc = uidoc.Document;
            View actView = doc.ActiveView;
            View FL01_view = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Views).OfClass(typeof(ViewPlan)).FirstOrDefault(x => x.Name == "FL01") as View;
            Plane ori_plane = Plane.CreateByNormalAndOrigin(new XYZ(1, 0, 0), new XYZ(0, 0, 0));//该处为原点镜像轴
            Plane mid_plane = null;//该处mid定义为整体户型组合模式的中心对称轴位置
            LogicalOrFilter orFilter = CreatorFilter(uiapp);
            #endregion

            #region ***声明 all 变量 time_20200312
            Document _TXA_ = null;
            Document _TXB_ = null;
            Document _TXC_ = null;
            Document _TXD_ = null;
            Document _TXA_Core_ = null;
            Document _TXB_Core_ = null;
            Document _TXC_Core_ = null;
            Document _TXD_Core_ = null;
            ICollection<ElementId> group_TXA_ids = new List<ElementId>();//各个基础模型3D元素group的id_list
            ICollection<ElementId> group_TXB_ids = new List<ElementId>();
            ICollection<ElementId> group_TXC_ids = new List<ElementId>();
            ICollection<ElementId> group_TXD_ids = new List<ElementId>();
            ICollection<ElementId> group_TXA_Core_ids = new List<ElementId>();
            ICollection<ElementId> group_TXB_Core_ids = new List<ElementId>();
            ICollection<ElementId> group_TXC_Core_ids = new List<ElementId>();
            ICollection<ElementId> group_TXD_Core_ids = new List<ElementId>();
            ICollection<ElementId> group_TXA_textnote_ids = new List<ElementId>();//各个基础模型2D文字注释group的id_list
            ICollection<ElementId> group_TXB_textnote_ids = new List<ElementId>();
            ICollection<ElementId> group_TXC_textnote_ids = new List<ElementId>();
            ICollection<ElementId> group_TXD_textnote_ids = new List<ElementId>();
            ICollection<ElementId> group_TXA_Core_textnote_ids = new List<ElementId>();
            ICollection<ElementId> group_TXB_Core_textnote_ids = new List<ElementId>();
            ICollection<ElementId> group_TXC_Core_textnote_ids = new List<ElementId>();
            ICollection<ElementId> group_TXD_Core_textnote_ids = new List<ElementId>();
            ICollection<ElementId> _TXA_dimension_ids = new List<ElementId>();//由于2D标注元素，不可以成group，所以此处为各个标注元素的list
            ICollection<ElementId> _TXB_dimension_ids = new List<ElementId>();
            ICollection<ElementId> _TXC_dimension_ids = new List<ElementId>();
            ICollection<ElementId> _TXD_dimension_ids = new List<ElementId>();
            ICollection<ElementId> _TXA_Core_dimension_ids = new List<ElementId>();//注意，目前核心筒基础模型，不涉及标注元素,此外初始化，是为了满足函数参数的空值调用
            ICollection<ElementId> _TXB_Core_dimension_ids = new List<ElementId>();
            ICollection<ElementId> _TXC_Core_dimension_ids = new List<ElementId>();
            ICollection<ElementId> _TXD_Core_dimension_ids = new List<ElementId>();
            #endregion

            #region ***判断是否需要打开源文档 time_20200312
            //OpenTarDocument(uiapp, _TXA_, _TXB_, _TXC_, _TXD_, _TXA_Core_, _TXB_Core_, _TXC_Core_, _TXD_Core_);
            if (CMD._TXA_rvt != "null_path")
            {
                _TXA_ = app.OpenDocumentFile(CMD._TXA_rvt);
            }
            if (CMD._TXB_rvt != "null_path")
            {
                _TXB_ = app.OpenDocumentFile(CMD._TXB_rvt);
            }
            if (CMD._TXC_rvt != "null_path")
            {
                _TXC_ = app.OpenDocumentFile(CMD._TXC_rvt);
            }
            if (CMD._TXD_rvt != "null_path")
            {
                _TXD_ = app.OpenDocumentFile(CMD._TXD_rvt);
            }
            if (CMD._TXA_HXT_rvt != "null_path")
            {
                _TXA_Core_ = app.OpenDocumentFile(CMD._TXA_HXT_rvt);
            }
            if (CMD._TXB_HXT_rvt != "null_path")
            {
                _TXB_Core_ = app.OpenDocumentFile(CMD._TXB_HXT_rvt);
            }
            if (CMD._TXC_HXT_rvt != "null_path")
            {
                _TXC_Core_ = app.OpenDocumentFile(CMD._TXC_HXT_rvt);
            }
            if (CMD._TXD_HXT_rvt != "null_path")
            {
                _TXD_Core_ = app.OpenDocumentFile(CMD._TXD_HXT_rvt);
            }
            #endregion

            #region 根据户型组合模式 在各个源文件 定位元素 在这里不对核心筒定位做处理 原因：revit后台多次打开同一个文件，属于无效操作 time_20200312
            double distance_TXA_X = 0;
            double distance_TXB_X = 0;
            Plane _mid_plane = null;

            if (CMD._TXA_rvt != "null_path")
            {
                ICollection<ElementId> _TXA_3D_EleIds_temp = (new FilteredElementCollector(_TXA_)).WherePasses(orFilter).WhereElementIsNotElementType().ToElementIds();
                FL01_view = new FilteredElementCollector(_TXA_).OfCategory(BuiltInCategory.OST_Views).OfClass(typeof(ViewPlan)).FirstOrDefault(x => x.Name == "FL01") as View;
                ICollection<ElementId> _TXA_TextNotes_temp = (new FilteredElementCollector(_TXA_, FL01_view.Id)).OwnedByView(FL01_view.Id).OfCategory(BuiltInCategory.OST_TextNotes).WhereElementIsNotElementType().ToElementIds();
                ICollection<ElementId> _TXA_Dimensions_temp = (new FilteredElementCollector(_TXA_, FL01_view.Id)).OwnedByView(FL01_view.Id).OfCategory(BuiltInCategory.OST_Dimensions).WhereElementIsNotElementType().ToElementIds();
                //判断是否需要对基础模型进行定位操作

                double _Abba_max_X;
                double _Abba_min_X = GetLeftRgiht_X_ingrid(_TXA_, _TXA_3D_EleIds_temp, out _Abba_max_X);
                distance_TXA_X = _Abba_max_X - _Abba_min_X;
            }
            if (CMD._TXB_rvt != "null_path")
            {
                ICollection<ElementId> _TXB_3D_EleIds_temp = (new FilteredElementCollector(_TXB_)).WherePasses(orFilter).WhereElementIsNotElementType().ToElementIds();
                FL01_view = new FilteredElementCollector(_TXB_).OfCategory(BuiltInCategory.OST_Views).OfClass(typeof(ViewPlan)).FirstOrDefault(x => x.Name == "FL01") as View;
                ICollection<ElementId> _TXB_TextNotes_temp = (new FilteredElementCollector(_TXB_, FL01_view.Id)).OwnedByView(FL01_view.Id).OfCategory(BuiltInCategory.OST_TextNotes).WhereElementIsNotElementType().ToElementIds();
                ICollection<ElementId> _TXB_Dimensions_temp = (new FilteredElementCollector(_TXB_, FL01_view.Id)).OwnedByView(FL01_view.Id).OfCategory(BuiltInCategory.OST_Dimensions).WhereElementIsNotElementType().ToElementIds();
                //判断是否需要对基础模型进行定位操作

                double _aBba_max_X;
                double _aBba_min_X = GetLeftRgiht_X_ingrid(_TXB_, _TXB_3D_EleIds_temp, out _aBba_max_X);
                distance_TXB_X = _aBba_max_X - _aBba_min_X;

                mid_plane = Plane.CreateByNormalAndOrigin(new XYZ(distance_TXB_X + 1, 0, 0), new XYZ(distance_TXB_X, 0, 0));
                _mid_plane = Plane.CreateByNormalAndOrigin(new XYZ(distance_TXB_X + distance_TXB_X + distance_TXA_X + 1, 0, 0), new XYZ(distance_TXB_X + distance_TXB_X + distance_TXA_X, 0, 0));
            }
            if (CMD._TXC_rvt != "null_path")
            {
                ICollection<ElementId> _TXC_3D_EleIds_temp = (new FilteredElementCollector(_TXC_)).WherePasses(orFilter).WhereElementIsNotElementType().ToElementIds();
                FL01_view = new FilteredElementCollector(_TXC_).OfCategory(BuiltInCategory.OST_Views).OfClass(typeof(ViewPlan)).FirstOrDefault(x => x.Name == "FL01") as View;
                ICollection<ElementId> _TXC_TextNotes_temp = (new FilteredElementCollector(_TXC_, FL01_view.Id)).OwnedByView(FL01_view.Id).OfCategory(BuiltInCategory.OST_TextNotes).WhereElementIsNotElementType().ToElementIds();
                ICollection<ElementId> _TXC_Dimensions_temp = (new FilteredElementCollector(_TXC_, FL01_view.Id)).OwnedByView(FL01_view.Id).OfCategory(BuiltInCategory.OST_Dimensions).WhereElementIsNotElementType().ToElementIds();
                //判断是否需要对基础模型进行定位操作
                ICollection<ElementId> mi_TXC_3D_EleIds_temp = _MirrorElements_ChangeHandler(_TXC_, _TXC_3D_EleIds_temp, _mid_plane, true);
                ICollection<ElementId> mi_TXC_TextNotes_temp = _MirrorElements_ChangeHandler(_TXC_, _TXC_TextNotes_temp, _mid_plane, true);
                ICollection<ElementId> mi_TXC_Dimensions_temp = _MirrorElements_ChangeHandler(_TXC_, _TXC_Dimensions_temp, _mid_plane, true);
                _deleteEles(_TXC_, _TXC_3D_EleIds_temp);
                _deleteEles(_TXC_, _TXC_TextNotes_temp);
            }
            if (CMD._TXD_rvt != "null_path")
            {
                //判断是否需要对基础模型进行定位操作
            }
            #endregion

            using (TransactionGroup transGroupCreateDwellingses = new TransactionGroup(doc, "创建户型零件组_group"))//开展事务组
            {
                if (transGroupCreateDwellingses.Start() == TransactionStatus.Started)//开启事务组
                {
                    #region ***将定位好的基础模型数据，复制到当前工作视图 并creat group 标注元素只有list，不可以creat group time_20200312
                    if (CMD._TXA_rvt != "null_path")
                    {
                        group_TXA_ids = Creat_Unit_GroupId_copyFrom_Ori(_TXA_, doc, orFilter, actView, out group_TXA_textnote_ids, out _TXA_dimension_ids, "_TXA_", "_TXA_textnote_");
                    }
                    if (CMD._TXB_rvt != "null_path")
                    {
                        group_TXB_ids = Creat_Unit_GroupId_copyFrom_Ori(_TXB_, doc, orFilter, actView, out group_TXB_textnote_ids, out _TXB_dimension_ids, "_TXB_", "_TXB_textnote_");
                    }
                    if (CMD._TXC_rvt != "null_path")
                    {
                        group_TXC_ids = Creat_Unit_GroupId_copyFrom_Ori(_TXC_, doc, orFilter, actView, out group_TXC_textnote_ids, out _TXC_dimension_ids, "_TXC_", "_TXC_textnote_");
                    }
                    if (CMD._TXD_rvt != "null_path")
                    {
                        group_TXD_ids = Creat_Unit_GroupId_copyFrom_Ori(_TXD_, doc, orFilter, actView, out group_TXD_textnote_ids, out _TXD_dimension_ids, "_TXD_", "_TXD_textnote_");
                    }
                    if (CMD._TXA_HXT_rvt != "null_path")
                    {
                        group_TXA_Core_ids = Creat_Unit_GroupId_copyFrom_Ori(_TXA_Core_, doc, orFilter, actView, out group_TXA_Core_textnote_ids, out _TXA_Core_dimension_ids, "_TXA_Core_", "_TXA_Core_textnote_");
                    }
                    if (CMD._TXB_HXT_rvt != "null_path")
                    {
                        group_TXB_Core_ids = Creat_Unit_GroupId_copyFrom_Ori(_TXB_Core_, doc, orFilter, actView, out group_TXB_Core_textnote_ids, out _TXB_Core_dimension_ids, "_TXB_Core_", "_TXB_Core_textnote_");
                    }
                    if (CMD._TXC_HXT_rvt != "null_path")
                    {
                        group_TXC_Core_ids = Creat_Unit_GroupId_copyFrom_Ori(_TXC_Core_, doc, orFilter, actView, out group_TXC_Core_textnote_ids, out _TXC_Core_dimension_ids, "_TXC_Core_", "_TXC_Core_textnote_");
                    }
                    if (CMD._TXD_HXT_rvt != "null_path")
                    {
                        group_TXD_Core_ids = Creat_Unit_GroupId_copyFrom_Ori(_TXD_Core_, doc, orFilter, actView, out group_TXD_Core_textnote_ids, out _TXD_Core_dimension_ids, "_TXD_Core_", "_TXD_Core_textnote_");
                    }
                    #endregion

                    #region 在当前视图，对户型进行排列组合 注意模型复制到当前文档时，所处的位置 time_20200312
                    //abccbd 根据字母组合进行判断
                    double move_distance_TXA_up = GetGlobalParametersValue_double(_TXA_, "move_distance_up_length_global");//_TXB_移动的为_TXA_的缝隙距离
                    double move_distance_TXA_down = GetGlobalParametersValue_double(_TXA_, "move_distance_down_length_global");//_TXB_移动的为_TXA_的缝隙距离
                    double move_distance_TXC_up = GetGlobalParametersValue_double(_TXC_, "move_distance_up_length_global");//_TXB_移动的为_TXA_的缝隙距离
                    double move_distance_TXC_down = GetGlobalParametersValue_double(_TXC_, "move_distance_down_length_global");//_TXB_移动的为_TXA_的缝隙距离

                    if (CMD._TXA_rvt != "null_path")
                    {
                        //判断是否需要对复制进来的模型数据进行二次操作
                        ICollection<ElementId> mi_group_TXA_ids = _MirrorElements_ChangeHandler(doc, group_TXA_ids, mid_plane, true);
                        ICollection<ElementId> mi_group_TXA_textnote_ids = _MirrorElements_ChangeHandler(doc, group_TXA_textnote_ids, mid_plane, true);
                        ICollection<ElementId> mi_TXA_dimension_ids = _MirrorElements_ChangeHandler(doc, _TXA_dimension_ids, mid_plane, true);

                        ICollection<ElementId> _mi_group_TXA_ids = _MirrorElements_ChangeHandler(doc, mi_group_TXA_ids, _mid_plane, true);
                        ICollection<ElementId> _mi_group_TXA_textnote_ids = _MirrorElements_ChangeHandler(doc, mi_group_TXA_textnote_ids, _mid_plane, true);
                        ICollection<ElementId> _mi_TXA_dimension_ids = _MirrorElements_ChangeHandler(doc, mi_TXA_dimension_ids, _mid_plane, true);

                        MoveGrouIds(doc, -move_distance_TXA_down - move_distance_TXA_down, mi_group_TXA_ids);
                        MoveGrouIds(doc, -move_distance_TXA_down - move_distance_TXA_down, mi_group_TXA_textnote_ids);

                        MoveGrouIds(doc, -move_distance_TXA_down - move_distance_TXA_down, _mi_group_TXA_ids);
                        MoveGrouIds(doc, -move_distance_TXA_down - move_distance_TXA_down, _mi_group_TXA_textnote_ids);
                    }
                    if (CMD._TXB_rvt != "null_path")
                    {
                        //判断是否需要对复制进来的模型数据进行二次操作
                        ICollection<ElementId> mi_group_TXB_ids = _MirrorElements_ChangeHandler(doc, group_TXB_ids, mid_plane, true);
                        ICollection<ElementId> mi_group_TXB_textnote_ids = _MirrorElements_ChangeHandler(doc, group_TXB_textnote_ids, mid_plane, true);
                        ICollection<ElementId> mi_TXB_dimension_ids = _MirrorElements_ChangeHandler(doc, _TXB_dimension_ids, mid_plane, true);

                        ICollection<ElementId> _mi_group_TXB_ids = _MirrorElements_ChangeHandler(doc, group_TXB_ids, _mid_plane, true);
                        ICollection<ElementId> _mi_group_TXB_textnote_ids = _MirrorElements_ChangeHandler(doc, group_TXB_textnote_ids, _mid_plane, true);
                        ICollection<ElementId> _mi_TXB_dimension_ids = _MirrorElements_ChangeHandler(doc, _TXB_dimension_ids, _mid_plane, true);

                        ICollection<ElementId> __mi_group_TXB_ids = _MirrorElements_ChangeHandler(doc, mi_group_TXB_ids, _mid_plane, true);
                        ICollection<ElementId> __mi_group_TXB_textnote_ids = _MirrorElements_ChangeHandler(doc, mi_group_TXB_textnote_ids, _mid_plane, true);
                        ICollection<ElementId> __mi_TXB_dimension_ids = _MirrorElements_ChangeHandler(doc, mi_TXB_dimension_ids, _mid_plane, true);

                        MoveGrouIds(doc, -move_distance_TXA_down, group_TXB_ids);
                        MoveGrouIds(doc, -move_distance_TXA_down, group_TXB_textnote_ids);

                        MoveGrouIds(doc, -move_distance_TXA_down, mi_group_TXB_ids);
                        MoveGrouIds(doc, -move_distance_TXA_down, mi_group_TXB_textnote_ids);

                        MoveGrouIds(doc, -move_distance_TXA_down - move_distance_TXA_down - move_distance_TXA_down, _mi_group_TXB_ids);
                        MoveGrouIds(doc, -move_distance_TXA_down - move_distance_TXA_down - move_distance_TXA_down, _mi_group_TXB_textnote_ids);

                        MoveGrouIds(doc, -move_distance_TXA_down - move_distance_TXA_down - move_distance_TXA_down, __mi_group_TXB_ids);
                        MoveGrouIds(doc, -move_distance_TXA_down - move_distance_TXA_down - move_distance_TXA_down, __mi_group_TXB_textnote_ids);
                    }
                    if (CMD._TXC_rvt != "null_path")
                    {
                        //判断是否需要对复制进来的模型数据进行二次操作
                        MoveGrouIds(doc, -move_distance_TXA_down - move_distance_TXA_down - move_distance_TXA_down - move_distance_TXC_down, group_TXC_ids);
                        MoveGrouIds(doc, -move_distance_TXA_down - move_distance_TXA_down - move_distance_TXA_down - move_distance_TXC_down, group_TXC_textnote_ids);
                    }
                    if (CMD._TXD_rvt != "null_path")
                    {
                        //判断是否需要对复制进来的模型数据进行二次操作
                    }
                    if (CMD._TXA_HXT_rvt != "null_path")
                    {
                        //判断是否需要对复制进来的模型数据进行二次操作
                        ICollection<ElementId> _mi_group_TXA_Core_ids = _MirrorElements_ChangeHandler(doc, group_TXA_Core_ids, _mid_plane, true);
                        ICollection<ElementId> _mi_group_TXA_Core_textnote_ids = _MirrorElements_ChangeHandler(doc, group_TXA_Core_textnote_ids, _mid_plane, true);

                        if (!CMD._TXA_HXT_rvt.Contains(@"CORE_5408") && !CMD._TXA_HXT_rvt.Contains(@"CORE_335406"))//core_5408为剪刀楼梯 CORE_335406为1T4H模式
                        {
                            //该处主要原因为剪刀楼梯不需要复制
                            ICollection<ElementId> mi_group_TXA_Core_ids = _MirrorElements_ChangeHandler(doc, group_TXA_Core_ids, mid_plane, true);
                            ICollection<ElementId> mi_group_TXA_Core_textnote_ids = _MirrorElements_ChangeHandler(doc, group_TXA_Core_textnote_ids, mid_plane, true);

                            ICollection<ElementId> __mi_group_TXA_Core_ids = _MirrorElements_ChangeHandler(doc, mi_group_TXA_Core_ids, _mid_plane, true);
                            ICollection<ElementId> __mi_group_TXA_Core_textnote_ids = _MirrorElements_ChangeHandler(doc, mi_group_TXA_Core_textnote_ids, _mid_plane, true);

                            MoveGrouIds(doc, -move_distance_TXA_up, group_TXA_Core_ids);//Aaaa
                            MoveGrouIds(doc, -move_distance_TXA_up, group_TXA_Core_textnote_ids);//Aaaa
                            MoveGrouIds(doc, -move_distance_TXA_down - move_distance_TXA_down + move_distance_TXA_up, mi_group_TXA_Core_ids);//aAaa
                            MoveGrouIds(doc, -move_distance_TXA_down - move_distance_TXA_down + move_distance_TXA_up, mi_group_TXA_Core_textnote_ids);//aAaa

                            MoveGrouIds(doc, -move_distance_TXA_down - move_distance_TXA_down - move_distance_TXA_up, __mi_group_TXA_Core_ids);//aaAa
                            MoveGrouIds(doc, -move_distance_TXA_down - move_distance_TXA_down - move_distance_TXA_up, __mi_group_TXA_Core_textnote_ids);//aaAa      
                            MoveGrouIds(doc, -move_distance_TXA_down - move_distance_TXA_down - move_distance_TXA_down - move_distance_TXC_down + move_distance_TXC_up, _mi_group_TXA_Core_ids);//aaaA
                            MoveGrouIds(doc, -move_distance_TXA_down - move_distance_TXA_down - move_distance_TXA_down - move_distance_TXC_down + move_distance_TXC_up, _mi_group_TXA_Core_textnote_ids);//aaaA

                            //添加核心筒走廊_此处操作为_通过修改全局参数，控制核心筒走廊宽度的一半；
                            AddCorefloor(doc, _TXA_Core_, distance_TXB_X);
                        }
                        else if (CMD._TXA_HXT_rvt.Contains(@"CORE_335406") || CMD._TXA_HXT_rvt.Contains(@"CORE_5408"))
                        {
                            //无操作
                            MoveGrouIds(doc, -move_distance_TXA_up, group_TXA_Core_ids);//Aaaa
                            MoveGrouIds(doc, -move_distance_TXA_up, group_TXA_Core_textnote_ids);//Aaaa
                            MoveGrouIds(doc, -move_distance_TXA_down - move_distance_TXA_down - move_distance_TXA_down - move_distance_TXC_down + move_distance_TXC_up, _mi_group_TXA_Core_ids);//aaaA
                            MoveGrouIds(doc, -move_distance_TXA_down - move_distance_TXA_down - move_distance_TXA_down - move_distance_TXC_down + move_distance_TXC_up, _mi_group_TXA_Core_textnote_ids);//aaaA
                        }

                    }
                    if (CMD._TXB_HXT_rvt != "null_path")
                    {
                        //判断是否需要对复制进来的模型数据进行二次操作
                    }
                    if (CMD._TXC_HXT_rvt != "null_path")
                    {
                        //判断是否需要对复制进来的模型数据进行二次操作
                    }
                    if (CMD._TXD_HXT_rvt != "null_path")
                    {
                        //判断是否需要对复制进来的模型数据进行二次操作
                    }
                    #endregion
                    transGroupCreateDwellingses.Assimilate();
                }
                else
                {
                    transGroupCreateDwellingses.RollBack();
                }
            }
            #region ***关闭所有源文档，但是不保存  time_20200312
            ResetDocument(_TXA_, _TXB_, _TXC_, _TXD_, _TXA_Core_, _TXB_Core_, _TXC_Core_, _TXD_Core_);
            #endregion
            #endregion
        }
        //新建户型_2T4H_abbaabbc_2DY_ABC_
        public void Creat_HXZU_2T4H_abbccbbd_2DY_ABCD_(UIApplication uiapp)
        {
            #region all code
            #region ***初始化与revit模型进行交互的辅助变量 time_20200312
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Application app = uiapp.Application;
            Document doc = uidoc.Document;
            View actView = doc.ActiveView;
            View FL01_view = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Views).OfClass(typeof(ViewPlan)).FirstOrDefault(x => x.Name == "FL01") as View;
            Plane ori_plane = Plane.CreateByNormalAndOrigin(new XYZ(1, 0, 0), new XYZ(0, 0, 0));//该处为原点镜像轴
            Plane mid_plane = null;//该处mid定义为整体户型组合模式的中心对称轴位置
            LogicalOrFilter orFilter = CreatorFilter(uiapp);
            #endregion

            #region ***声明 all 变量 time_20200312
            Document _TXA_ = null;
            Document _TXB_ = null;
            Document _TXC_ = null;
            Document _TXD_ = null;
            Document _TXA_Core_ = null;
            Document _TXB_Core_ = null;
            Document _TXC_Core_ = null;
            Document _TXD_Core_ = null;
            ICollection<ElementId> group_TXA_ids = new List<ElementId>();//各个基础模型3D元素group的id_list
            ICollection<ElementId> group_TXB_ids = new List<ElementId>();
            ICollection<ElementId> group_TXC_ids = new List<ElementId>();
            ICollection<ElementId> group_TXD_ids = new List<ElementId>();
            ICollection<ElementId> group_TXA_Core_ids = new List<ElementId>();
            ICollection<ElementId> group_TXB_Core_ids = new List<ElementId>();
            ICollection<ElementId> group_TXC_Core_ids = new List<ElementId>();
            ICollection<ElementId> group_TXD_Core_ids = new List<ElementId>();
            ICollection<ElementId> group_TXA_textnote_ids = new List<ElementId>();//各个基础模型2D文字注释group的id_list
            ICollection<ElementId> group_TXB_textnote_ids = new List<ElementId>();
            ICollection<ElementId> group_TXC_textnote_ids = new List<ElementId>();
            ICollection<ElementId> group_TXD_textnote_ids = new List<ElementId>();
            ICollection<ElementId> group_TXA_Core_textnote_ids = new List<ElementId>();
            ICollection<ElementId> group_TXB_Core_textnote_ids = new List<ElementId>();
            ICollection<ElementId> group_TXC_Core_textnote_ids = new List<ElementId>();
            ICollection<ElementId> group_TXD_Core_textnote_ids = new List<ElementId>();
            ICollection<ElementId> _TXA_dimension_ids = new List<ElementId>();//由于2D标注元素，不可以成group，所以此处为各个标注元素的list
            ICollection<ElementId> _TXB_dimension_ids = new List<ElementId>();
            ICollection<ElementId> _TXC_dimension_ids = new List<ElementId>();
            ICollection<ElementId> _TXD_dimension_ids = new List<ElementId>();
            ICollection<ElementId> _TXA_Core_dimension_ids = new List<ElementId>();//注意，目前核心筒基础模型，不涉及标注元素,此外初始化，是为了满足函数参数的空值调用
            ICollection<ElementId> _TXB_Core_dimension_ids = new List<ElementId>();
            ICollection<ElementId> _TXC_Core_dimension_ids = new List<ElementId>();
            ICollection<ElementId> _TXD_Core_dimension_ids = new List<ElementId>();
            #endregion

            #region ***判断是否需要打开源文档 time_20200312
            //OpenTarDocument(uiapp, _TXA_, _TXB_, _TXC_, _TXD_, _TXA_Core_, _TXB_Core_, _TXC_Core_, _TXD_Core_);
            if (CMD._TXA_rvt != "null_path")
            {
                _TXA_ = app.OpenDocumentFile(CMD._TXA_rvt);
            }
            if (CMD._TXB_rvt != "null_path")
            {
                _TXB_ = app.OpenDocumentFile(CMD._TXB_rvt);
            }
            if (CMD._TXC_rvt != "null_path")
            {
                _TXC_ = app.OpenDocumentFile(CMD._TXC_rvt);
            }
            if (CMD._TXD_rvt != "null_path")
            {
                _TXD_ = app.OpenDocumentFile(CMD._TXD_rvt);
            }
            if (CMD._TXA_HXT_rvt != "null_path")
            {
                _TXA_Core_ = app.OpenDocumentFile(CMD._TXA_HXT_rvt);
            }
            if (CMD._TXB_HXT_rvt != "null_path")
            {
                _TXB_Core_ = app.OpenDocumentFile(CMD._TXB_HXT_rvt);
            }
            if (CMD._TXC_HXT_rvt != "null_path")
            {
                _TXC_Core_ = app.OpenDocumentFile(CMD._TXC_HXT_rvt);
            }
            if (CMD._TXD_HXT_rvt != "null_path")
            {
                _TXD_Core_ = app.OpenDocumentFile(CMD._TXD_HXT_rvt);
            }
            #endregion

            #region 根据户型组合模式 在各个源文件 定位元素 在这里不对核心筒定位做处理 原因：revit后台多次打开同一个文件，属于无效操作 time_20200312

            double distance_TXB_X = 0;
            double distance_TXC_X = 0;
            Plane _mid_plane = null;

            if (CMD._TXA_rvt != "null_path")
            {
                //判断是否需要对基础模型进行定位操作
            }
            if (CMD._TXB_rvt != "null_path")
            {
                ICollection<ElementId> _TXB_3D_EleIds_temp = (new FilteredElementCollector(_TXB_)).WherePasses(orFilter).WhereElementIsNotElementType().ToElementIds();
                FL01_view = new FilteredElementCollector(_TXB_).OfCategory(BuiltInCategory.OST_Views).OfClass(typeof(ViewPlan)).FirstOrDefault(x => x.Name == "FL01") as View;
                ICollection<ElementId> _TXB_TextNotes_temp = (new FilteredElementCollector(_TXB_, FL01_view.Id)).OwnedByView(FL01_view.Id).OfCategory(BuiltInCategory.OST_TextNotes).WhereElementIsNotElementType().ToElementIds();
                ICollection<ElementId> _TXB_Dimensions_temp = (new FilteredElementCollector(_TXB_, FL01_view.Id)).OwnedByView(FL01_view.Id).OfCategory(BuiltInCategory.OST_Dimensions).WhereElementIsNotElementType().ToElementIds();
                //判断是否需要对基础模型进行定位操作

                double _aBbccbba_max_X;
                double _aBbccbba_min_X = GetLeftRgiht_X_ingrid(_TXB_, _TXB_3D_EleIds_temp, out _aBbccbba_max_X);
                distance_TXB_X = _aBbccbba_max_X - _aBbccbba_min_X;

                mid_plane = Plane.CreateByNormalAndOrigin(new XYZ(distance_TXB_X + 1, 0, 0), new XYZ(distance_TXB_X, 0, 0));
            }
            if (CMD._TXC_rvt != "null_path")
            {
                ICollection<ElementId> _TXC_3D_EleIds_temp = (new FilteredElementCollector(_TXC_)).WherePasses(orFilter).WhereElementIsNotElementType().ToElementIds();
                FL01_view = new FilteredElementCollector(_TXC_).OfCategory(BuiltInCategory.OST_Views).OfClass(typeof(ViewPlan)).FirstOrDefault(x => x.Name == "FL01") as View;
                ICollection<ElementId> _TXC_TextNotes_temp = (new FilteredElementCollector(_TXC_, FL01_view.Id)).OwnedByView(FL01_view.Id).OfCategory(BuiltInCategory.OST_TextNotes).WhereElementIsNotElementType().ToElementIds();
                ICollection<ElementId> _TXC_Dimensions_temp = (new FilteredElementCollector(_TXC_, FL01_view.Id)).OwnedByView(FL01_view.Id).OfCategory(BuiltInCategory.OST_Dimensions).WhereElementIsNotElementType().ToElementIds();
                //判断是否需要对基础模型进行定位操作
                ICollection<ElementId> mi_TXC_3D_EleIds_temp = _MirrorElements_ChangeHandler(_TXC_, _TXC_3D_EleIds_temp, mid_plane, true);
                ICollection<ElementId> mi_TXC_TextNotes_temp = _MirrorElements_ChangeHandler(_TXC_, _TXC_TextNotes_temp, mid_plane, true);
                ICollection<ElementId> mi_TXC_Dimensions_temp = _MirrorElements_ChangeHandler(_TXC_, _TXC_Dimensions_temp, mid_plane, true);
                _deleteEles(_TXC_, _TXC_3D_EleIds_temp);
                _deleteEles(_TXC_, _TXC_TextNotes_temp);
                double _abbCcbba_max_X;
                double _abbCcbba_min_X = GetLeftRgiht_X_ingrid(_TXC_, mi_TXC_3D_EleIds_temp, out _abbCcbba_max_X);
                distance_TXC_X = _abbCcbba_max_X - _abbCcbba_min_X;
                _mid_plane = Plane.CreateByNormalAndOrigin(new XYZ(distance_TXB_X + distance_TXB_X + distance_TXC_X + 1, 0, 0), new XYZ(distance_TXB_X + distance_TXB_X + distance_TXC_X, 0, 0));
            }
            if (CMD._TXD_rvt != "null_path")
            {
                ICollection<ElementId> _TXD_3D_EleIds_temp = (new FilteredElementCollector(_TXD_)).WherePasses(orFilter).WhereElementIsNotElementType().ToElementIds();
                FL01_view = new FilteredElementCollector(_TXD_).OfCategory(BuiltInCategory.OST_Views).OfClass(typeof(ViewPlan)).FirstOrDefault(x => x.Name == "FL01") as View;
                ICollection<ElementId> _TXD_TextNotes_temp = (new FilteredElementCollector(_TXD_, FL01_view.Id)).OwnedByView(FL01_view.Id).OfCategory(BuiltInCategory.OST_TextNotes).WhereElementIsNotElementType().ToElementIds();
                ICollection<ElementId> _TXD_Dimensions_temp = (new FilteredElementCollector(_TXD_, FL01_view.Id)).OwnedByView(FL01_view.Id).OfCategory(BuiltInCategory.OST_Dimensions).WhereElementIsNotElementType().ToElementIds();
                //判断是否需要对基础模型进行定位操作
                ICollection<ElementId> mi_TXD_3D_EleIds_temp = _MirrorElements_ChangeHandler(_TXD_, _TXD_3D_EleIds_temp, _mid_plane, true);
                ICollection<ElementId> mi_TXD_TextNotes_temp = _MirrorElements_ChangeHandler(_TXD_, _TXD_TextNotes_temp, _mid_plane, true);
                ICollection<ElementId> mi_TXC_Dimensions_temp = _MirrorElements_ChangeHandler(_TXD_, _TXD_Dimensions_temp, _mid_plane, true);
                _deleteEles(_TXD_, _TXD_3D_EleIds_temp);
                _deleteEles(_TXD_, _TXD_TextNotes_temp);
            }
            #endregion

            using (TransactionGroup transGroupCreateDwellingses = new TransactionGroup(doc, "创建户型零件组_group"))//开展事务组
            {
                if (transGroupCreateDwellingses.Start() == TransactionStatus.Started)//开启事务组
                {
                    #region ***将定位好的基础模型数据，复制到当前工作视图 并creat group 标注元素只有list，不可以creat group time_20200312
                    if (CMD._TXA_rvt != "null_path")
                    {
                        group_TXA_ids = Creat_Unit_GroupId_copyFrom_Ori(_TXA_, doc, orFilter, actView, out group_TXA_textnote_ids, out _TXA_dimension_ids, "_TXA_", "_TXA_textnote_");
                    }
                    if (CMD._TXB_rvt != "null_path")
                    {
                        group_TXB_ids = Creat_Unit_GroupId_copyFrom_Ori(_TXB_, doc, orFilter, actView, out group_TXB_textnote_ids, out _TXB_dimension_ids, "_TXB_", "_TXB_textnote_");
                    }
                    if (CMD._TXC_rvt != "null_path")
                    {
                        group_TXC_ids = Creat_Unit_GroupId_copyFrom_Ori(_TXC_, doc, orFilter, actView, out group_TXC_textnote_ids, out _TXC_dimension_ids, "_TXC_", "_TXC_textnote_");
                    }
                    if (CMD._TXD_rvt != "null_path")
                    {
                        group_TXD_ids = Creat_Unit_GroupId_copyFrom_Ori(_TXD_, doc, orFilter, actView, out group_TXD_textnote_ids, out _TXD_dimension_ids, "_TXD_", "_TXD_textnote_");
                    }
                    if (CMD._TXA_HXT_rvt != "null_path")
                    {
                        group_TXA_Core_ids = Creat_Unit_GroupId_copyFrom_Ori(_TXA_Core_, doc, orFilter, actView, out group_TXA_Core_textnote_ids, out _TXA_Core_dimension_ids, "_TXA_Core_", "_TXA_Core_textnote_");
                    }
                    if (CMD._TXB_HXT_rvt != "null_path")
                    {
                        group_TXB_Core_ids = Creat_Unit_GroupId_copyFrom_Ori(_TXB_Core_, doc, orFilter, actView, out group_TXB_Core_textnote_ids, out _TXB_Core_dimension_ids, "_TXB_Core_", "_TXB_Core_textnote_");
                    }
                    if (CMD._TXC_HXT_rvt != "null_path")
                    {
                        group_TXC_Core_ids = Creat_Unit_GroupId_copyFrom_Ori(_TXC_Core_, doc, orFilter, actView, out group_TXC_Core_textnote_ids, out _TXC_Core_dimension_ids, "_TXC_Core_", "_TXC_Core_textnote_");
                    }
                    if (CMD._TXD_HXT_rvt != "null_path")
                    {
                        group_TXD_Core_ids = Creat_Unit_GroupId_copyFrom_Ori(_TXD_Core_, doc, orFilter, actView, out group_TXD_Core_textnote_ids, out _TXD_Core_dimension_ids, "_TXD_Core_", "_TXD_Core_textnote_");
                    }
                    #endregion

                    #region 在当前视图，对户型进行排列组合 注意模型复制到当前文档时，所处的位置 time_20200312
                    //abccbd 根据字母组合进行判断
                    double move_distance_TXA_up = GetGlobalParametersValue_double(_TXA_, "move_distance_up_length_global");//_TXB_移动的为_TXA_的缝隙距离
                    double move_distance_TXA_down = GetGlobalParametersValue_double(_TXA_, "move_distance_down_length_global");//_TXB_移动的为_TXA_的缝隙距离
                    double move_distance_TXC_up = GetGlobalParametersValue_double(_TXC_, "move_distance_up_length_global");//_TXB_移动的为_TXA_的缝隙距离
                    double move_distance_TXC_down = GetGlobalParametersValue_double(_TXC_, "move_distance_down_length_global");//_TXB_移动的为_TXA_的缝隙距离
                    double move_distance_TXD_up = GetGlobalParametersValue_double(_TXD_, "move_distance_up_length_global");//_TXB_移动的为_TXA_的缝隙距离
                    double move_distance_TXD_down = GetGlobalParametersValue_double(_TXD_, "move_distance_down_length_global");//_TXB_移动的为_TXA_的缝隙距离

                    if (CMD._TXA_rvt != "null_path")
                    {
                        //判断是否需要对复制进来的模型数据进行二次操作
                    }
                    if (CMD._TXB_rvt != "null_path")
                    {
                        //判断是否需要对复制进来的模型数据进行二次操作
                        ICollection<ElementId> mi_group_TXB_ids = _MirrorElements_ChangeHandler(doc, group_TXB_ids, mid_plane, true);
                        ICollection<ElementId> mi_group_TXB_textnote_ids = _MirrorElements_ChangeHandler(doc, group_TXB_textnote_ids, mid_plane, true);
                        ICollection<ElementId> mi_TXB_dimension_ids = _MirrorElements_ChangeHandler(doc, _TXB_dimension_ids, mid_plane, true);

                        ICollection<ElementId> __mi_group_TXB_ids = _MirrorElements_ChangeHandler(doc, mi_group_TXB_ids, _mid_plane, true);
                        ICollection<ElementId> __mi_group_TXB_textnote_ids = _MirrorElements_ChangeHandler(doc, mi_group_TXB_textnote_ids, _mid_plane, true);
                        ICollection<ElementId> __mi_TXB_dimension_ids = _MirrorElements_ChangeHandler(doc, mi_TXB_dimension_ids, _mid_plane, true);

                        ICollection<ElementId> _mi_group_TXB_ids = _MirrorElements_ChangeHandler(doc, group_TXB_ids, _mid_plane, true);
                        ICollection<ElementId> _mi_group_TXB_textnote_ids = _MirrorElements_ChangeHandler(doc, group_TXB_textnote_ids, _mid_plane, true);
                        ICollection<ElementId> _mi_TXB_dimension_ids = _MirrorElements_ChangeHandler(doc, _TXB_dimension_ids, _mid_plane, true);

                        MoveGrouIds(doc, -move_distance_TXA_down, group_TXB_ids);
                        MoveGrouIds(doc, -move_distance_TXA_down, group_TXB_textnote_ids);

                        MoveGrouIds(doc, -move_distance_TXA_down, mi_group_TXB_ids);
                        MoveGrouIds(doc, -move_distance_TXA_down, mi_group_TXB_textnote_ids);

                        MoveGrouIds(doc, -move_distance_TXA_down - move_distance_TXA_down - move_distance_TXA_down, __mi_group_TXB_ids);
                        MoveGrouIds(doc, -move_distance_TXA_down - move_distance_TXA_down - move_distance_TXA_down, __mi_group_TXB_textnote_ids);

                        MoveGrouIds(doc, -move_distance_TXA_down - move_distance_TXA_down - move_distance_TXA_down, _mi_group_TXB_ids);
                        MoveGrouIds(doc, -move_distance_TXA_down - move_distance_TXA_down - move_distance_TXA_down, _mi_group_TXB_textnote_ids);
                    }
                    if (CMD._TXC_rvt != "null_path")
                    {
                        //判断是否需要对复制进来的模型数据进行二次操作
                        ICollection<ElementId> mi_group_TXC_ids = _MirrorElements_ChangeHandler(doc, group_TXC_ids, _mid_plane, true);
                        ICollection<ElementId> mi_group_TXC_textnote_ids = _MirrorElements_ChangeHandler(doc, group_TXC_textnote_ids, _mid_plane, true);
                        ICollection<ElementId> mi_TXC_dimension_ids = _MirrorElements_ChangeHandler(doc, _TXC_dimension_ids, _mid_plane, true);

                        MoveGrouIds(doc, -move_distance_TXA_down - move_distance_TXC_down, group_TXC_ids);
                        MoveGrouIds(doc, -move_distance_TXA_down - move_distance_TXC_down, group_TXC_textnote_ids);

                        MoveGrouIds(doc, -move_distance_TXA_down - move_distance_TXC_down, mi_group_TXC_ids);
                        MoveGrouIds(doc, -move_distance_TXA_down - move_distance_TXC_down, mi_group_TXC_textnote_ids);
                    }
                    if (CMD._TXD_rvt != "null_path")
                    {
                        //判断是否需要对复制进来的模型数据进行二次操作
                        MoveGrouIds(doc, -move_distance_TXA_down - move_distance_TXC_down - move_distance_TXC_down - move_distance_TXD_down, group_TXD_ids);
                        MoveGrouIds(doc, -move_distance_TXA_down - move_distance_TXC_down - move_distance_TXC_down - move_distance_TXD_down, group_TXD_textnote_ids);
                    }
                    if (CMD._TXA_HXT_rvt != "null_path")
                    {
                        //判断是否需要对复制进来的模型数据进行二次操作
                        ICollection<ElementId> _mi_group_TXA_Core_ids = _MirrorElements_ChangeHandler(doc, group_TXA_Core_ids, _mid_plane, true);
                        ICollection<ElementId> _mi_group_TXA_Core_textnote_ids = _MirrorElements_ChangeHandler(doc, group_TXA_Core_textnote_ids, _mid_plane, true);

                        if (!CMD._TXA_HXT_rvt.Contains(@"CORE_5408") && !CMD._TXA_HXT_rvt.Contains(@"CORE_335406"))//core_5408为剪刀楼梯 CORE_335406为1T4H模式
                        {
                            //该处主要原因为剪刀楼梯不需要复制
                            ICollection<ElementId> mi_group_TXA_Core_ids = _MirrorElements_ChangeHandler(doc, group_TXA_Core_ids, mid_plane, true);
                            ICollection<ElementId> mi_group_TXA_Core_textnote_ids = _MirrorElements_ChangeHandler(doc, group_TXA_Core_textnote_ids, mid_plane, true);

                            ICollection<ElementId> __mi_group_TXA_Core_ids = _MirrorElements_ChangeHandler(doc, mi_group_TXA_Core_ids, _mid_plane, true);
                            ICollection<ElementId> __mi_group_TXA_Core_textnote_ids = _MirrorElements_ChangeHandler(doc, mi_group_TXA_Core_textnote_ids, _mid_plane, true);

                            MoveGrouIds(doc, -move_distance_TXA_up, group_TXA_Core_ids);//Aaaa
                            MoveGrouIds(doc, -move_distance_TXA_up, group_TXA_Core_textnote_ids);//Aaaa
                            MoveGrouIds(doc, -move_distance_TXA_down - move_distance_TXC_down + move_distance_TXC_up, mi_group_TXA_Core_ids);//aAaa
                            MoveGrouIds(doc, -move_distance_TXA_down - move_distance_TXC_down + move_distance_TXC_up, mi_group_TXA_Core_textnote_ids);//aAaa

                            MoveGrouIds(doc, -move_distance_TXA_down - move_distance_TXC_down - move_distance_TXC_up, __mi_group_TXA_Core_ids);//aaAa
                            MoveGrouIds(doc, -move_distance_TXA_down - move_distance_TXC_down - move_distance_TXC_up, __mi_group_TXA_Core_textnote_ids);//aaAa      
                            MoveGrouIds(doc, -move_distance_TXA_down - move_distance_TXC_down - move_distance_TXC_down - move_distance_TXD_down + move_distance_TXD_up, _mi_group_TXA_Core_ids);//aaaA
                            MoveGrouIds(doc, -move_distance_TXA_down - move_distance_TXC_down - move_distance_TXC_down - move_distance_TXD_down + move_distance_TXD_up, _mi_group_TXA_Core_textnote_ids);//aaaA

                            //添加核心筒走廊_此处操作为_通过修改全局参数，控制核心筒走廊宽度的一半；
                            AddCorefloor(doc, _TXA_Core_, distance_TXB_X);
                        }
                        else if (CMD._TXA_HXT_rvt.Contains(@"CORE_335406") || CMD._TXA_HXT_rvt.Contains(@"CORE_5408"))
                        {
                            //无操作

                            MoveGrouIds(doc, -move_distance_TXA_up, group_TXA_Core_ids);//Aaaa
                            MoveGrouIds(doc, -move_distance_TXA_up, group_TXA_Core_textnote_ids);//Aaaa
                            MoveGrouIds(doc, -move_distance_TXA_down - move_distance_TXC_down - move_distance_TXC_down - move_distance_TXD_down + move_distance_TXD_up, _mi_group_TXA_Core_ids);//aaaA
                            MoveGrouIds(doc, -move_distance_TXA_down - move_distance_TXC_down - move_distance_TXC_down - move_distance_TXD_down + move_distance_TXD_up, _mi_group_TXA_Core_textnote_ids);//aaaA
                        }
                    }
                    if (CMD._TXB_HXT_rvt != "null_path")
                    {
                        //判断是否需要对复制进来的模型数据进行二次操作
                    }
                    if (CMD._TXC_HXT_rvt != "null_path")
                    {
                        //判断是否需要对复制进来的模型数据进行二次操作
                    }
                    if (CMD._TXD_HXT_rvt != "null_path")
                    {
                        //判断是否需要对复制进来的模型数据进行二次操作
                    }
                    #endregion
                    transGroupCreateDwellingses.Assimilate();
                }
                else
                {
                    transGroupCreateDwellingses.RollBack();
                }
            }
            #region ***关闭所有源文档，但是不保存  time_20200312
            ResetDocument(_TXA_, _TXB_, _TXC_, _TXD_, _TXA_Core_, _TXB_Core_, _TXC_Core_, _TXD_Core_);
            #endregion
            #endregion
        }

        //以下为各种method---------------------------------分割线---------------------------------
        /// <summary>
        /// 添加核心筒走廊_此处操作为_通过修改全局参数，控制核心筒走廊宽度的一半
        /// </summary>
        public void AddCorefloor(Document doc, Document _TXA_Core_, double distance_TXB_X)
        {
            ICollection<ElementId> core_group_AlleleIds = new List<ElementId>();
            using (Transaction modifyGlobalPara = new Transaction(doc))
            {
                modifyGlobalPara.Start("modifyDWparametricfloorPara");
                //SetGlobalParametersValue_double(doc, "楼板宽度为核心筒走廊距离一半", vestibule_length_half);
                //SetGlobalParametersValue_double(doc, "走廊板宽度", 100);
                //FamilyInstance DWparametericfloor = (new FilteredElementCollector(doc)).OfCategory(BuiltInCategory.OST_GenericModel).OfClass(typeof(FamilyInstance)).WhereElementIsNotElementType().FirstOrDefault(x => x.Name == "dw参数化楼板") as FamilyInstance;
                IList<Element> _TXA_Core_groups = (new FilteredElementCollector(doc)).OfCategory(BuiltInCategory.OST_IOSModelGroups).WhereElementIsNotElementType().ToElements();
                foreach (Element ele in _TXA_Core_groups)
                {
                    ICollection<ElementId> _group_eleIds = new List<ElementId>();
                    if (ele.Name == "_TXA_Core_")
                    {
                        Group _group = ele as Group;
                        _group_eleIds = _group.UngroupMembers();

                    }
                    foreach (ElementId eleId in _group_eleIds)
                    {
                        core_group_AlleleIds.Add(eleId);
                    }
                }
                modifyGlobalPara.Commit();
            }
            using (Transaction creatNewGroup = new Transaction(doc))
            {
                creatNewGroup.Start("creatNewGroup");
                double _core_length_left_ = GetGlobalParametersValue_double(_TXA_Core_, "核心筒宽度_左位置块");//求出核心筒左边部位的宽度 核心筒宽度_左位置块
                double vestibule_length_half = (distance_TXB_X * 2 - _core_length_left_ * 2) / 2;
                IList<Element> _TXA_Core_groupElds = (new FilteredElementCollector(doc, core_group_AlleleIds)).OfCategory(BuiltInCategory.OST_GenericModel).OfClass(typeof(FamilyInstance)).WhereElementIsNotElementType().ToElements();
                foreach (Element ele in _TXA_Core_groupElds)
                {
                    if (ele.Name == "dw参数化楼板")
                    {
                        ElementId eletypeId = ele.GetTypeId();//获取族类型；
                        Element eleType = doc.GetElement(eletypeId);
                        Parameter DWparametericfloor_length = eleType.LookupParameter("长度");
                        DWparametericfloor_length.Set(vestibule_length_half);
                    }
                }
                Group __group = doc.Create.NewGroup(core_group_AlleleIds);
                __group.GroupType.Name = "_Core_Group_";
                creatNewGroup.Commit();
            }
        }
        /// <summary>
        /// 获取一个全局参数，该方法读取 double 类型的值
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="globalParaName"></param>
        /// <returns></returns>
        public double GetGlobalParametersValue_double(Document doc, string globalParaName)
        {
            double dvalue_double = 0.0;
            int dvalue_int = 0;
            string dvalue_string = "";

            ElementId gpid = GlobalParametersManager.FindByName(doc, globalParaName);
            if (gpid.IntegerValue != -1)
            {
                if (GlobalParametersManager.IsValidGlobalParameter(doc, gpid))//判断id是否为项目的全局参数id
                {
                    GlobalParameter gp = doc.GetElement(gpid) as GlobalParameter;
                    ParameterValue gpvalue = gp.GetValue();
                    // test whether it is a Double or Integer
                    //TaskDialog.Show("Revit", gp.GetType().ToString());

                    if (gpvalue.GetType() == typeof(DoubleParameterValue))
                    {
                        DoubleParameterValue dvalue = gpvalue as DoubleParameterValue;
                        dvalue_double = dvalue.Value;
                        //gp.SetValue(dvalue);
                    }
                    else if (gpvalue.GetType() == typeof(IntegerParameterValue))
                    {
                        // Integer values may represent Boolean parameters too
                        IntegerParameterValue ivalue = gpvalue as IntegerParameterValue;
                        if (gp.GetDefinition().ParameterType == ParameterType.YesNo)
                        {
                            dvalue_int = ivalue.Value;
                        }
                        else   // common integers
                        {
                            dvalue_int = ivalue.Value;
                        }
                        gp.SetValue(ivalue);
                    }
                    else if (gpvalue.GetType() == typeof(StringParameterValue))
                    {
                        StringParameterValue strvalue = gpvalue as StringParameterValue;
                        dvalue_string = strvalue.Value;
                        //gp.SetValue(dvalue);
                    }
                }
            }
            else
            {
                //string message_error = "该文档不存在名称为“" + globalParaName + "”的共享参数不被允许";
                //TaskDialog.Show("Revit", message_error);
            }
            return dvalue_double;
        }
        public double SetGlobalParametersValue_double(Document doc, string globalParaName, double Set_dvalue_double)
        {
            double dvalue_double = 0.0;
            int dvalue_int = 0;
            string dvalue_string = "";

            ElementId gpid = GlobalParametersManager.FindByName(doc, globalParaName);
            if (gpid.IntegerValue != -1)
            {
                if (GlobalParametersManager.IsValidGlobalParameter(doc, gpid))//判断id是否为项目的全局参数id
                {
                    GlobalParameter gp = doc.GetElement(gpid) as GlobalParameter;
                    ParameterValue gpvalue = gp.GetValue();
                    // test whether it is a Double or Integer
                    //TaskDialog.Show("Revit", gp.GetType().ToString());

                    if (gpvalue.GetType() == typeof(DoubleParameterValue))
                    {
                        DoubleParameterValue dvalue = gpvalue as DoubleParameterValue;
                        dvalue.Value = Set_dvalue_double;
                        gp.SetValue(dvalue);
                    }
                    else if (gpvalue.GetType() == typeof(IntegerParameterValue))
                    {
                        // Integer values may represent Boolean parameters too
                        IntegerParameterValue ivalue = gpvalue as IntegerParameterValue;
                        if (gp.GetDefinition().ParameterType == ParameterType.YesNo)
                        {
                            dvalue_int = ivalue.Value;
                        }
                        else   // common integers
                        {
                            dvalue_int = ivalue.Value;
                        }
                        gp.SetValue(ivalue);
                    }
                    else if (gpvalue.GetType() == typeof(StringParameterValue))
                    {
                        StringParameterValue strvalue = gpvalue as StringParameterValue;
                        dvalue_string = strvalue.Value;
                        //gp.SetValue(dvalue);
                    }
                }
            }
            else
            {
                //string message_error = "该文档不存在名称为“" + globalParaName + "”的共享参数不被允许";
                //TaskDialog.Show("Revit", message_error);
            }
            return dvalue_double;
        }
        /// <summary>
        /// 获取一个全局参数，该方法读取 int 类型的值
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="globalParaName"></param>
        /// <returns></returns>
        public int GetGlobalParametersValue_int(Document doc, string globalParaName)
        {
            double dvalue_double = 0.0;
            int dvalue_int = 0;
            string dvalue_string = "";

            ElementId gpid = GlobalParametersManager.FindByName(doc, globalParaName);
            if (gpid.IntegerValue != -1)
            {
                if (GlobalParametersManager.IsValidGlobalParameter(doc, gpid))//判断id是否为项目的全局参数id
                {
                    GlobalParameter gp = doc.GetElement(gpid) as GlobalParameter;
                    ParameterValue gpvalue = gp.GetValue();
                    // test whether it is a Double or Integer
                    //TaskDialog.Show("Revit", gp.GetType().ToString());

                    if (gpvalue.GetType() == typeof(DoubleParameterValue))
                    {
                        DoubleParameterValue dvalue = gpvalue as DoubleParameterValue;
                        dvalue_double = dvalue.Value;
                        //gp.SetValue(dvalue);
                    }
                    else if (gpvalue.GetType() == typeof(IntegerParameterValue))
                    {
                        // Integer values may represent Boolean parameters too
                        IntegerParameterValue ivalue = gpvalue as IntegerParameterValue;
                        if (gp.GetDefinition().ParameterType == ParameterType.YesNo)
                        {
                            dvalue_int = ivalue.Value;
                        }
                        else   // common integers
                        {
                            dvalue_int = ivalue.Value;
                        }
                        gp.SetValue(ivalue);
                    }
                    else if (gpvalue.GetType() == typeof(StringParameterValue))
                    {
                        StringParameterValue strvalue = gpvalue as StringParameterValue;
                        dvalue_string = strvalue.Value;
                        //gp.SetValue(dvalue);
                    }
                }
            }
            else
            {
                string message_error = "该文档不存在名称为“" + globalParaName + "”的共享参数不被允许";
                TaskDialog.Show("Revit", message_error);
            }
            return dvalue_int;
        }
        public int SetGlobalParametersValue_int(Document doc, string globalParaName)
        {
            double dvalue_double = 0.0;
            int dvalue_int = 0;
            string dvalue_string = "";

            ElementId gpid = GlobalParametersManager.FindByName(doc, globalParaName);
            if (gpid.IntegerValue != -1)
            {
                if (GlobalParametersManager.IsValidGlobalParameter(doc, gpid))//判断id是否为项目的全局参数id
                {
                    GlobalParameter gp = doc.GetElement(gpid) as GlobalParameter;
                    ParameterValue gpvalue = gp.GetValue();
                    // test whether it is a Double or Integer
                    //TaskDialog.Show("Revit", gp.GetType().ToString());

                    if (gpvalue.GetType() == typeof(DoubleParameterValue))
                    {
                        DoubleParameterValue dvalue = gpvalue as DoubleParameterValue;
                        dvalue_double = dvalue.Value;
                        //gp.SetValue(dvalue);
                    }
                    else if (gpvalue.GetType() == typeof(IntegerParameterValue))
                    {
                        // Integer values may represent Boolean parameters too
                        IntegerParameterValue ivalue = gpvalue as IntegerParameterValue;
                        if (gp.GetDefinition().ParameterType == ParameterType.YesNo)
                        {
                            dvalue_int = ivalue.Value;
                        }
                        else   // common integers
                        {
                            dvalue_int = ivalue.Value;
                        }
                        gp.SetValue(ivalue);
                    }
                    else if (gpvalue.GetType() == typeof(StringParameterValue))
                    {
                        StringParameterValue strvalue = gpvalue as StringParameterValue;
                        dvalue_string = strvalue.Value;
                        //gp.SetValue(dvalue);
                    }
                }
            }
            else
            {
                string message_error = "该文档不存在名称为“" + globalParaName + "”的共享参数不被允许";
                TaskDialog.Show("Revit", message_error);
            }
            return dvalue_int;
        }
        /// <summary>
        /// 获取一个全局参数，该方法读取 string 类型的值
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="globalParaName"></param>
        /// <returns></returns>
        public string GetGlobalParametersValue_string(Document doc, string globalParaName)
        {
            double dvalue_double = 0.0;
            int dvalue_int = 0;
            string dvalue_string = "";

            ElementId gpid = GlobalParametersManager.FindByName(doc, globalParaName);
            if (gpid.IntegerValue != -1)
            {
                if (GlobalParametersManager.IsValidGlobalParameter(doc, gpid))//判断id是否为项目的全局参数id
                {
                    GlobalParameter gp = doc.GetElement(gpid) as GlobalParameter;
                    ParameterValue gpvalue = gp.GetValue();
                    // test whether it is a Double or Integer
                    //TaskDialog.Show("Revit", gp.GetType().ToString());

                    if (gpvalue.GetType() == typeof(DoubleParameterValue))
                    {
                        DoubleParameterValue dvalue = gpvalue as DoubleParameterValue;
                        dvalue_double = dvalue.Value;
                        //gp.SetValue(dvalue);
                    }
                    else if (gpvalue.GetType() == typeof(IntegerParameterValue))
                    {
                        // Integer values may represent Boolean parameters too
                        IntegerParameterValue ivalue = gpvalue as IntegerParameterValue;
                        if (gp.GetDefinition().ParameterType == ParameterType.YesNo)
                        {
                            dvalue_int = ivalue.Value;
                        }
                        else   // common integers
                        {
                            dvalue_int = ivalue.Value;
                        }
                        gp.SetValue(ivalue);
                    }
                    else if (gpvalue.GetType() == typeof(StringParameterValue))
                    {
                        StringParameterValue strvalue = gpvalue as StringParameterValue;
                        dvalue_string = strvalue.Value;
                        //gp.SetValue(dvalue);
                    }
                }
            }
            else
            {
                string message_error = "该文档不存在名称为“" + globalParaName + "”的共享参数不被允许";
                TaskDialog.Show("Revit", message_error);
            }
            return dvalue_string;
        }
        public string SetGlobalParametersValue_string(Document doc, string globalParaName)
        {
            double dvalue_double = 0.0;
            int dvalue_int = 0;
            string dvalue_string = "";

            ElementId gpid = GlobalParametersManager.FindByName(doc, globalParaName);
            if (gpid.IntegerValue != -1)
            {
                if (GlobalParametersManager.IsValidGlobalParameter(doc, gpid))//判断id是否为项目的全局参数id
                {
                    GlobalParameter gp = doc.GetElement(gpid) as GlobalParameter;
                    ParameterValue gpvalue = gp.GetValue();
                    // test whether it is a Double or Integer
                    //TaskDialog.Show("Revit", gp.GetType().ToString());

                    if (gpvalue.GetType() == typeof(DoubleParameterValue))
                    {
                        DoubleParameterValue dvalue = gpvalue as DoubleParameterValue;
                        dvalue_double = dvalue.Value;
                        //gp.SetValue(dvalue);
                    }
                    else if (gpvalue.GetType() == typeof(IntegerParameterValue))
                    {
                        // Integer values may represent Boolean parameters too
                        IntegerParameterValue ivalue = gpvalue as IntegerParameterValue;
                        if (gp.GetDefinition().ParameterType == ParameterType.YesNo)
                        {
                            dvalue_int = ivalue.Value;
                        }
                        else   // common integers
                        {
                            dvalue_int = ivalue.Value;
                        }
                        gp.SetValue(ivalue);
                    }
                    else if (gpvalue.GetType() == typeof(StringParameterValue))
                    {
                        StringParameterValue strvalue = gpvalue as StringParameterValue;
                        dvalue_string = strvalue.Value;
                        //gp.SetValue(dvalue);
                    }
                }
            }
            else
            {
                string message_error = "该文档不存在名称为“" + globalParaName + "”的共享参数不被允许";
                TaskDialog.Show("Revit", message_error);
            }
            return dvalue_string;
        }
        /// <summary>
        /// 通过布尔判断，打开目标基础调用文档
        /// </summary>
        public void OpenTarDocument(UIApplication uiapp, Document _TXA_, Document _TXB_, Document _TXC_, Document _TXD_, Document _TXA_Core_, Document _TXB_Core_, Document _TXC_Core_, Document _TXD_Core_)
        {
            Application app = uiapp.Application;
            if (CMD._TXA_rvt != "null_path")
            {
                string path_TXA_rvt = CMD._TXA_rvt;
                _TXA_ = app.OpenDocumentFile(CMD._TXA_rvt);
            }
            if (CMD._TXB_rvt != "null_path")
            {
                _TXB_ = app.OpenDocumentFile(CMD._TXB_rvt);
            }
            if (CMD._TXC_rvt != "null_path")
            {
                _TXC_ = app.OpenDocumentFile(CMD._TXC_rvt);
            }
            if (CMD._TXD_rvt != "null_path")
            {
                _TXD_ = app.OpenDocumentFile(CMD._TXD_rvt);
            }
            if (CMD._TXA_HXT_rvt != "null_path")
            {
                _TXA_Core_ = app.OpenDocumentFile(CMD._TXA_HXT_rvt);
            }
            if (CMD._TXB_HXT_rvt != "null_path")
            {
                _TXB_Core_ = app.OpenDocumentFile(CMD._TXB_HXT_rvt);
            }
            if (CMD._TXC_HXT_rvt != "null_path")
            {
                _TXC_Core_ = app.OpenDocumentFile(CMD._TXC_HXT_rvt);
            }
            if (CMD._TXD_HXT_rvt != "null_path")
            {
                _TXD_Core_ = app.OpenDocumentFile(CMD._TXD_HXT_rvt);
            }
        }
        /// <summary>
        /// 关闭该程序中所有已经打开revit文档，并重置revit文档静态路径
        /// </summary>
        public void ResetDocument(Document _TXA_, Document _TXB_, Document _TXC_, Document _TXD_, Document _TXA_Core_, Document _TXB_Core_, Document _TXC_Core_, Document _TXD_Core_)
        {
            if (CMD._TXA_rvt != "null_path")
            {
                bool _TXA_close = _TXA_.Close(false);
                CMD._TXA_rvt = "null_path";
            }
            if (CMD._TXB_rvt != "null_path")
            {
                bool _TXA_close = _TXB_.Close(false);
                CMD._TXB_rvt = "null_path";
            }
            if (CMD._TXC_rvt != "null_path")
            {
                bool _TXA_close = _TXC_.Close(false);
                CMD._TXC_rvt = "null_path";
            }
            if (CMD._TXD_rvt != "null_path")
            {
                bool _TXA_close = _TXD_.Close(false);
                CMD._TXD_rvt = "null_path";
            }
            if (CMD._TXA_HXT_rvt != "null_path")
            {
                bool _TXA_close = _TXA_Core_.Close(false);
                CMD._TXA_HXT_rvt = "null_path";
            }
            if (CMD._TXB_HXT_rvt != "null_path")
            {
                bool _TXA_close = _TXB_Core_.Close(false);
                CMD._TXB_HXT_rvt = "null_path";
            }
            if (CMD._TXC_HXT_rvt != "null_path")
            {
                bool _TXA_close = _TXC_Core_.Close(false);
                CMD._TXC_HXT_rvt = "null_path";
            }
            if (CMD._TXD_HXT_rvt != "null_path")
            {
                bool _TXA_close = _TXD_Core_.Close(false);
                CMD._TXD_HXT_rvt = "null_path";
            }
        }
        /// <summary>
        /// 创建多重过滤器，可增加或删除BuiltInCategory
        /// </summary>
        /// <param name="uiapp"></param>
        /// <returns></returns>
        public LogicalOrFilter CreatorFilter(UIApplication uiapp)
        {
            //设置多个过滤器，进行目标模型复制
            IList<ElementFilter> filterSet = new List<ElementFilter>();
            filterSet.Add(new ElementCategoryFilter(BuiltInCategory.OST_Stairs));
            filterSet.Add(new ElementCategoryFilter(BuiltInCategory.OST_Walls));
            filterSet.Add(new ElementCategoryFilter(BuiltInCategory.OST_GenericModel));
            filterSet.Add(new ElementCategoryFilter(BuiltInCategory.OST_Grids));
            filterSet.Add(new ElementCategoryFilter(BuiltInCategory.OST_Furniture));
            filterSet.Add(new ElementCategoryFilter(BuiltInCategory.OST_SpecialityEquipment));
            filterSet.Add(new ElementCategoryFilter(BuiltInCategory.OST_Floors));
            //filterSet.Add(new ElementCategoryFilter(BuiltInCategory.OST_Dimensions));
            //filterSet.Add(new ElementCategoryFilter(BuiltInCategory.OST_TextNotes));
            LogicalOrFilter orFilter = new LogicalOrFilter(filterSet);
            return orFilter;
        }
        /// <summary>
        /// 开启移动事物
        /// </summary>
        /// <param name="_TXA_"></param>
        /// <param name="distance"></param>
        /// <param name="mi_group_TXC_ids"></param>
        public void MoveGrouIds(Document _TXA_, double distance, ICollection<ElementId> mi_group_TXC_ids)
        {
            using (Transaction moveGroupId = new Transaction(_TXA_))
            {
                DeleteErrOrWaringTaskDialog(moveGroupId);
                try
                {
                    moveGroupId.Start("moveGroupId");
                    ElementTransformUtils.MoveElements(_TXA_, mi_group_TXC_ids, new XYZ(distance, 0, 0));
                    moveGroupId.Commit();
                }
                catch (Exception ex)
                {
                    TaskDialog.Show("Error", ex.ToString());
                    moveGroupId.RollBack();
                }
            }
        }
        /// <summary>
        /// 通过documentChangeEvent获取mirrorele的元素，该代码中没有采用时间监控机制，源于列表复制，revit api提供镜像返回值
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="elementId"></param>
        /// <param name="plane"></param>
        /// <returns></returns>
        public ICollection<ElementId> _MirrorElement_docChangeHandler(Document doc, ElementId elementId, Plane plane)
        {
            if (doc == null || plane == null || !ElementTransformUtils.CanMirrorElement(doc, elementId))
            {
                throw new ArgumentException("Argument invalid");
            }

            ICollection<ElementId> result = new List<ElementId>();
            //创建documentChangeEvent
            //创建的同时，设定函数内容
            EventHandler<DocumentChangedEventArgs> documentChangeHandler = new EventHandler<DocumentChangedEventArgs>((
               sender, args) => result = args.GetAddedElementIds());
            //注册documentchangeevent
            doc.Application.DocumentChanged += documentChangeHandler;

            using (Transaction transaction = new Transaction(doc))
            {
                //此处使用try，在标注中存在部分标注无法镜像的问题，会被读出错误，因此，该处不在使用 try catch
                try
                {
                    transaction.Start("Mirror");
                    //该处镜像存在问题
                    ElementTransformUtils.MirrorElement(doc, elementId, plane);
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    TaskDialog.Show("Error", ex.ToString());
                    transaction.RollBack();
                }
                finally
                {
                    //注销documentchangeevent
                    doc.Application.DocumentChanged -= documentChangeHandler;
                }
            }
            return result;
        }
        /// <summary>
        /// 通过documentChangeEvent获取mirroreles的元素
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="elementIds"></param>
        /// <param name="plane"></param>
        /// <param name="shifou"></param>
        /// <returns></returns>
        public ICollection<ElementId> _MirrorElements_ChangeHandler(Document doc, ICollection<ElementId> elementIds, Plane plane, bool shifou)
        {
            ICollection<ElementId> result = new List<ElementId>();
            using (Transaction transaction = new Transaction(doc))
            {
                DeleteErrOrWaringTaskDialog(transaction);
                try
                {
                    transaction.Start("Mirror");
                    result = ElementTransformUtils.MirrorElements(doc, elementIds, plane, true);
                    if (!shifou)
                    {
                        doc.Delete(elementIds);
                    }
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    TaskDialog.Show("Error", ex.ToString());
                    transaction.RollBack();
                }
            }
            return result;
        }
        /// <summary>
        ///在事务中删除元素，并获取返回值
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="elementIds"></param>
        /// <returns></returns>
        public ICollection<ElementId> _deleteEles(Document doc, ICollection<ElementId> elementIds)
        {
            ICollection<ElementId> result = new List<ElementId>();
            using (Transaction transaction = new Transaction(doc))
            {
                DeleteErrOrWaringTaskDialog(transaction);
                try
                {
                    transaction.Start("deleteEles");
                    result = doc.Delete(elementIds);
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    TaskDialog.Show("Error", ex.ToString());
                    transaction.RollBack();
                }
            }
            return result;
        }
        /// <summary>
        /// 将group id list转化为element list
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="cb_groupIds"></param>
        /// <returns></returns>
        public ICollection<ElementId> GetElesFromGrou_list(Document doc, ICollection<ElementId> cb_groupIds)
        {
            ICollection<ElementId> _cb_group_eleIds = new List<ElementId>();
            foreach (ElementId eleId in cb_groupIds)
            {
                Group group = doc.GetElement(eleId) as Group;
                if (group != null)
                {
                    IList<ElementId> bb_eles_group = group.GetMemberIds();
                    foreach (ElementId _eleId in bb_eles_group)
                    {
                        _cb_group_eleIds.Add(_eleId);
                    }
                }
            }
            return _cb_group_eleIds;
        }
        /// <summary>
        /// 首先进入group id list 读取各个grou内元素，然后，从收集到元素列表中读取轴网最左侧或者最右侧的 x 数值 x 数值
        /// </summary>
        /// <param name="_TXA_"></param>
        /// <param name="group_TXB_ids"></param>
        /// <param name="_2_maxX"></param>
        /// <returns></returns>
        public double GetLeftRgiht_X_ingrid_fromGroup(Document _TXA_, ICollection<ElementId> group_TXB_ids, out double _2_maxX)
        {
            ICollection<ElementId> _db_group_eleIds = GetElesFromGrou_list(_TXA_, group_TXB_ids);

            ICollection<Element> _db_eles = (new FilteredElementCollector(_TXA_, _db_group_eleIds)).OfCategory(BuiltInCategory.OST_Grids).WhereElementIsNotElementType().ToElements();
            List<double> _2_xLst = new List<double>();
            foreach (Element ele in _db_eles)
            {
                Grid grid = ele as Grid;
                Line line = grid.Curve as Line;
                XYZ direction = line.Direction;
                XYZ origin = line.Origin;
                if (Math.Abs(direction.Y) == 1)//基于direction的 Y 是否为1,判断轴网是否为竖向轴网
                {
                    _2_xLst.Add(origin.X);
                }
            }
            _2_maxX = _2_xLst[0];//求出轴网最右侧 X 值
            foreach (double _double in _2_xLst)
            {
                if (_2_maxX <= _double)
                {
                    _2_maxX = _double;
                }
            }
            double _2_minxX = _2_xLst[0];//求出轴网最左侧 X 值
            foreach (double _double in _2_xLst)
            {
                if (_2_minxX >= _double)
                {
                    _2_minxX = _double;
                }
            }
            return _2_minxX;
        }
        /// <summary>
        /// 直接从元素列表中读取轴网最左侧或者最右侧的 x 数值
        /// </summary>
        /// <param name="_TXA_"></param>
        /// <param name="group_TXB_ids"></param>
        /// <param name="_2_maxX"></param>
        /// <returns></returns>
        public double GetLeftRgiht_X_ingrid(Document _TXA_, ICollection<ElementId> group_TXB_ids, out double _2_maxX)
        {
            ICollection<Element> _db_eles = (new FilteredElementCollector(_TXA_, group_TXB_ids)).OfCategory(BuiltInCategory.OST_Grids).WhereElementIsNotElementType().ToElements();
            List<double> _2_xLst = new List<double>();
            foreach (Element ele in _db_eles)
            {
                Grid grid = ele as Grid;
                Line line = grid.Curve as Line;
                XYZ direction = line.Direction;
                XYZ origin = line.Origin;
                if (Math.Abs(direction.Y) == 1)//基于direction的 Y 是否为1,判断轴网是否为竖向轴网
                {
                    _2_xLst.Add(origin.X);
                }
            }
            _2_maxX = _2_xLst[0];//求出轴网最右侧 X 值
            foreach (double _double in _2_xLst)
            {
                if (_2_maxX <= _double)
                {
                    _2_maxX = _double;
                }
            }
            double _2_minxX = _2_xLst[0];//求出轴网最左侧 X 值
            foreach (double _double in _2_xLst)
            {
                if (_2_minxX >= _double)
                {
                    _2_minxX = _double;
                }
            }
            return _2_minxX;
        }
        /// <summary>
        /// 文档之间元素整体复制，并创建套型的3D/2Ddgroup，以及提取标注list；
        /// </summary>
        /// <param name="ori_doc">源文档</param>
        /// <param name="doc">当前文档</param>
        /// <param name="orFilter">过滤器列表</param>
        /// <param name="actView">当时活动视图</param>
        /// <param name="group_textnote_Ids">输出的成组二维文字图元的group id 列表</param>
        /// <param name="ori_dimension_ids">输出的二维标注list</param>
        /// <param name="group_textnote_name">对3D元素group 命名</param>
        /// <param name="group_3DElements_name">对2D元素group 命名</param>
        /// <returns></returns>
        public ICollection<ElementId> Creat_Unit_GroupId_copyFrom_Ori(Document ori_doc, Document doc, LogicalOrFilter orFilter, View actView, out ICollection<ElementId> group_textnote_Ids, out ICollection<ElementId> ori_dimension_ids, string group_3DElements_name, string group_textnote_name)
        {
            Group group_3DElements = null;//打包3D元素
            Group group_textnote = null;//打包3D元素
            ICollection<ElementId> group_3D_ids = new List<ElementId>();//aBccbd 大写字母位置的图元
            ICollection<ElementId> group_2D_ids = new List<ElementId>();//aBccbd 大写字母位置的图元

            using (Transaction CreateDwellingsNewGroups = new Transaction(doc))//开展creat group事物
            {
                DeleteErrOrWaringTaskDialog(CreateDwellingsNewGroups);
                CreateDwellingsNewGroups.Start("CreateDwellingsNewGroups");

                group_3DElements = doc.Create.NewGroup(Copy3DElementsBetweenDoces(ori_doc, doc, orFilter));//打包3D元素
                group_textnote = doc.Create.NewGroup(Copy2DTextDimensionBetweenDoces(ori_doc, actView, BuiltInCategory.OST_TextNotes));//打包3D元素
                ori_dimension_ids = Copy2DTextDimensionBetweenDoces(ori_doc, actView, BuiltInCategory.OST_Dimensions); //导出2D标注元素

                group_3DElements.GroupType.Name = group_3DElements_name;//group的名字需要rename
                group_textnote.GroupType.Name = group_textnote_name;//group的名字需要rename

                CreateDwellingsNewGroups.Commit();
            }
            group_3D_ids.Add(group_3DElements.Id);
            group_2D_ids.Add(group_textnote.Id);
            group_textnote_Ids = group_2D_ids;

            return group_3D_ids;
        }
        /// <summary>
        /// 获取并复制一个类别文档的二维注释，可指定类别，默认为从 FL01 标高读取数据
        /// </summary>
        /// <param name="_TXA_"></param>
        /// <param name="actView"></param>
        /// <param name="builtInCategory"></param>
        /// <returns></returns>
        public ICollection<ElementId> Copy2DTextDimensionBetweenDoces(Document _TXA_, View actView, BuiltInCategory builtInCategory)
        {
            View FL01_view = (new FilteredElementCollector(_TXA_)).OfCategory(BuiltInCategory.OST_Views).OfClass(typeof(ViewPlan)).WhereElementIsNotElementType().FirstOrDefault(x => x.Name == "FL01") as View;
            ICollection<ElementId> _temp_ = (new FilteredElementCollector(_TXA_, FL01_view.Id)).OwnedByView(FL01_view.Id).OfCategory(builtInCategory).WhereElementIsNotElementType().ToElementIds();
            CopyPasteOptions opts = new CopyPasteOptions();
            opts.SetDuplicateTypeNamesHandler(new CopyEventHandler());
            if (_temp_.Count != 0)
            {
                ICollection<ElementId> planEleIds_TXA_ = ElementTransformUtils.CopyElements(FL01_view, _temp_, actView, null, opts);//复制图元
                return planEleIds_TXA_;
            }
            else
            {
                return new List<ElementId>();
            }
        }
        /// <summary>
        /// 获取并复制一个文档的3D元素，默认为左边套，解决复制的时候，产生重复类型提示的弹窗警告,该处理为设置元素的复制选项
        /// </summary>
        /// <param name="_TXB_"></param>
        /// <param name="doc"></param>
        /// <param name="orFilter"></param>
        /// <returns></returns>
        public ICollection<ElementId> Copy3DElementsBetweenDoces(Document _TXB_, Document doc, LogicalOrFilter orFilter)
        {
            ICollection<ElementId> SetIds_TXB_ = (new FilteredElementCollector(_TXB_)).WherePasses(orFilter).WhereElementIsNotElementType().ToElementIds();
            CopyPasteOptions opts = new CopyPasteOptions();
            opts.SetDuplicateTypeNamesHandler(new CopyEventHandler());
            ICollection<ElementId> group_T2L_ids = ElementTransformUtils.CopyElements(_TXB_, SetIds_TXB_, doc, null, opts);
            return group_T2L_ids;
        }
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
        /// <summary>
        /// 尝试解决——弹窗提示的异常和错误
        /// </summary>
        /// <param name="copyDwellingsEleIds"></param>
        public void DeleteErrOrWaringTaskDialog(Transaction copyDwellingsEleIds)
        {
            FailureHandlingOptions fho = copyDwellingsEleIds.GetFailureHandlingOptions();
            fho.SetFailuresPreprocessor(new FiluresPrecessor());
            copyDwellingsEleIds.SetFailureHandlingOptions(fho);
        }
        /// <summary>
        /// 可用于执行预处理步骤的接口，以过滤出预期的事务失败或将某些失败标记为不可持续的。
        /// </summary>
        public class FiluresPrecessor : IFailuresPreprocessor
        {
            public FailureProcessingResult PreprocessFailures(FailuresAccessor failuresAccessor)
            {
                IList<FailureMessageAccessor> listFma = failuresAccessor.GetFailureMessages();
                if (listFma.Count <= 0)//没有任何 警告 or 弹窗 弹窗提示
                {
                    return FailureProcessingResult.Continue;
                }
                foreach (FailureMessageAccessor fma in listFma)
                {
                    if (fma.GetSeverity() == FailureSeverity.Error)//判断弹窗消息，是否，为 错误 提示
                    {
                        if (fma.HasResolutions())
                        {
                            failuresAccessor.ResolveFailure(fma);
                        }
                    }
                    if (fma.GetSeverity() == FailureSeverity.Warning)//判断弹窗消息，是否，为 警告 提示
                    {
                        failuresAccessor.DeleteWarning(fma);
                    }
                }
                return FailureProcessingResult.ProceedWithCommit;
            }
        }
    }  // public class RequestHandler : IExternalEventHandler
} // namespace

