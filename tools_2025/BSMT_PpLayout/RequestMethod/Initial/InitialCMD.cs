using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using PubFuncWt;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSMT_PpLayout
{
    /// <summary>
    /// CMD启动时，需要进行的启动设置
    /// </summary>
    class InitialCMD : RequestMethod
    {
        internal InitialCMD(UIApplication uiapp) : base(uiapp)
        {
        }
        /// <summary>
        /// 初始化操作
        /// </summary>
        internal override void Execute()
        {
            // 读取插件面板数据
            RecordPanelData panelData = new RecordPanelData(this.uiApp);
            panelData.Write();

            //清除无效详图组类型
            this.doc.DeleteInvalidDetailGroupTypes();

            //throw new NotImplementedException();
            #region 启动命令，区域当前文档的已存在的地库方案
            GlobalData.basementWallOutlineNames.Clear();

            //【】找到所有的地库外墙范围
            IEnumerable<FilledRegion> eles = new FilteredElementCollector(this.doc)
                .OfCategory(BuiltInCategory.OST_DetailComponents)
                .OfClass(typeof(FilledRegion))
                .WhereElementIsNotElementType()
                .Cast<FilledRegion>()
                //.Where(p=>p.OwnerViewId==this.View.Id)
                ;

            List<FilledRegion> filledRegions = new List<FilledRegion>();
            foreach (FilledRegion ele in eles)
            {
                FilledRegionType tempFilledRegionType = doc.GetElement(ele.GetTypeId()) as FilledRegionType;
                if (tempFilledRegionType.Name == "地库外墙范围" || tempFilledRegionType.Name == "地库_外墙范围")
                {
                    filledRegions.Add(ele);
                }
            }

            #endregion
            UpdateCommandText(filledRegions);
        }
        /// <summary>
        /// 地库方案标记值更新
        /// </summary>
        internal void UpdateCommandText(IEnumerable<FilledRegion> filledRegions)
        {
            string A_TAG_Curtainwallpanels_NUMBER_siName = "Arial Narrow-2.0-1.00-HDDN";// 组类型Name
            string A_TAG_Curtainwallpanels_NUMBER_FiName = "A-TAG-BasementWallName-NUMBER";// 族Name
            string A_TAG_Curtainwallpanels_NUMBER_FiPath = App.FamilyFilePath + @"标记族\A-TAG-BasementWallName-NUMBER.rfa";// 族Path
            FamilySymbol fs_A_TAG_BasementWallName_NUMBER = this.doc.FamilySymbolByPath(A_TAG_Curtainwallpanels_NUMBER_FiPath, A_TAG_Curtainwallpanels_NUMBER_siName, A_TAG_Curtainwallpanels_NUMBER_FiName);

            //【】判断地库外墙范围是否已经存在标记值
            // 获取所有的标记族实例
            List<IndependentTag> tagElements = new FilteredElementCollector(doc).OfClass(typeof(IndependentTag)).WhereElementIsNotElementType().Cast<IndependentTag>().ToList();

            List<ElementId> elementIds = new List<ElementId>();//标记值的主体图元Id
            foreach (IndependentTag independentTag in tagElements)
            {
                FamilySymbol familySymbol = doc.GetElement(independentTag.GetTypeId()) as FamilySymbol;
                string faName = familySymbol.FamilyName;
                if (faName.Contains(A_TAG_Curtainwallpanels_NUMBER_FiName))
                {
                    elementIds.Add(independentTag.TaggedLocalElementId);
                }
            }
            //【】输入界面
            foreach (FilledRegion filledRegion in filledRegions)
            {
                string doorNumber = filledRegion.get_Parameter(BuiltInParameter.DOOR_NUMBER).AsString();
                string id = filledRegion.Id.ToString();
                if (doorNumber == "" || doorNumber == null)
                    continue;

                string basementName = $"#-{doorNumber}- (Id:{id})";
                //【】界面显示
                GlobalData.basementWallOutlineNames.Add(basementName);

                //【】标记注释 需要判断是否需要开启注释事务
                if (elementIds.Contains(filledRegion.Id)) continue;

                SetTag(this.doc, this.doc.ActiveView, filledRegion, fs_A_TAG_BasementWallName_NUMBER.Id);
            }
        }
        /// <summary>
        /// 载入需要的族类型
        /// </summary>
        internal void ReLoadFStype()
        {

            string nowPathName = doc.PathName;
            if (App.pathNames.Contains(nowPathName))
                App.Time += 1;
            else
            {
                App.pathNames.Add(nowPathName);
                App.Time = 0;
            }

            //GlobalData.Instance.Instance.strBackgroundMonitorDta += "\n" + "004 " + "当前工作文档路径：" + nowPathName;
            //GlobalData.Instance.Instance.strBackgroundMonitorDta += "\n" + "005 " + "该插件图标点击第-" + App.Time.ToString() + "-次";

            if (App.Time > 0) return;

            List<string> vs = new List<string>();
            vs.Add(App.FamilyFilePath + @"机动车\停车位_.rfa");
            vs.Add(App.FamilyFilePath + @"机动车\机械车位_.rfa");
            vs.Add(App.FamilyFilePath + @"机动车\无障碍车位_.rfa");
            vs.Add(App.FamilyFilePath + @"机动车\子母车位_.rfa");
            vs.Add(App.FamilyFilePath + @"机动车\公共泊车位_.rfa");
            vs.Add(App.FamilyFilePath + @"机动车\慢充电位_.rfa");
            vs.Add(App.FamilyFilePath + @"机动车\快充电位_.rfa");
            vs.Add(App.FamilyFilePath + @"柱子\柱子_.rfa");
            vs.Add(App.FamilyFilePath + @"坡道\上下坡道.rfa");
            vs.Add(App.FamilyFilePath + @"坡道\坡道-直线.rfa");
            vs.Add(App.FamilyFilePath + @"坡道\坡道-直线-下坡.rfa");
            //vs.Add(App.FamilyFilePath + @"坡道\坡道A-弧-直\坡道-弧线A.rfa");
            vs.Add(App.FamilyFilePath + @"坡道\坡道B-直-弧-直\坡道-弧线B.rfa");

            foreach (var item in vs)
            {
                doc.ReLoadFamily(item);
            }
        }
        /// <summary>
        /// 将地库外墙范围的标记值，以标记族的形式，注释出来
        /// </summary>
        internal void SetTag(Document doc, View acvtiView, Element ele, ElementId elementId)
        {
            using (Transaction setTag = new Transaction(doc))
            {
                setTag.Start("给图元打上标记族");
                acvtiView.SetTag(doc, ele, elementId, 1);
                // 设置图元ogs透明度改为100%
                //acvtiView.SetFilledRegionColor(ele.Id, new Color(0, 0, 0), new Color(0, 166, 0), 100);
                setTag.Commit();
            }
        }

    }
}
