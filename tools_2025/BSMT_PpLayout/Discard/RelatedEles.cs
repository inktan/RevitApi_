//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//using Autodesk.Revit.DB;
//using Autodesk.Revit.UI;

//namespace BSMT_PpLayout
//{
//    /// <summary>
//    /// 对二维元素构建数据结构管理
//    /// </summary>
//    class RelatedEles : RevitElementCtrl
//    {

//        #region 提取目标类型的Revit元素
//        internal View view { get; set; }
//        internal List<Element> Eles { get; set; }
//        //internal DictionaryEleInfo dictionaryEleInfo { get; set; }
//        //internal List<BsmtBoundary> BasementBoundarys { get; set; }// 地库外墙线
//        #endregion

//        #region 构造函数
//        /// <summary>
//        /// 构造函数
//        /// </summary>
//        internal RelatedEles(Element view) :base(view)
//        {
//            this.view = view as View;
//            this.Eles = GetEles(this.view);
//            if (this._basementBoundarys.Count < 1) throw new NotImplementedException("面积平面：" + view.Name + "，不存在地库_地库外墙线填充区域");

//            #region 字典处理
//            this.dictionaryEleInfo = new DictionaryEleInfo(Eles, this.view);
//            #endregion
//        }
//        #endregion

//        /// <summary>
//        /// 获取相关的所有Revit元素 / 获取当前视图层级上的，所有地库外墙线线圈，线圈改写为 g3 表达式
//        /// </summary>
//        public List<Element> GetEles(View view)
//        {
//            Document doc = this.Doc;

//            // 过滤相关类型元素
//            ElementCategoryFilter detailComponentsFilter = new ElementCategoryFilter(BuiltInCategory.OST_DetailComponents);// 详图项目
//            ElementCategoryFilter linesFilter = new ElementCategoryFilter(BuiltInCategory.OST_Lines);// 线
//            LogicalOrFilter logicalOrFilter = new LogicalOrFilter(new List<ElementFilter>() { detailComponentsFilter, linesFilter });// 逻辑过滤器 - 或
//            List<Element> allTarEles = (new FilteredElementCollector(doc, view.Id)) // 使用视图构建过滤器-
//                .WherePasses(logicalOrFilter)
//                .WhereElementIsNotElementType()
//                .Where(p=>p.IsValidObject)
//                .ToList();

//            #region 寻找视图中需要的元素

//            List<BsmtBoundary> basementBoundarys = new List<BsmtBoundary>();// 地库外墙线
//            List<Element> _allTarEles = new List<Element>();
//            foreach (Element _ele in allTarEles)
//            {

//                if (_ele is FilledRegion)
//                {
//                    string filledRegionTypeName = doc.GetElement(_ele.GetTypeId()).Name;
                                       
//                    if (filledRegionTypeName.Contains("地库外墙范围"))
//                    {
//                        //if(InputParameter.baseMentWallLoopIds.Contains(_ele.Id))// 这里决定了，从记录的地库外墙线填充区域所在视图，返回来去寻找被记录的地库外墙线填充区域元素id
//                        //{
//                        //    FilledRegion filledRegion = _ele as FilledRegion;
//                        //    basementBoundarys.Add(new BasementBoundary(filledRegion));
//                        //}
//                        FilledRegion filledRegion = _ele as FilledRegion;
//                        basementBoundarys.Add(new BsmtBoundary(filledRegion));
//                    }
//                }

//                //if (_ele.Pinned) continue;//【】判断图元是否锁定，锁定图元不进行处理
//                _allTarEles.Add(_ele);
//            }

//            this._basementBoundarys = basementBoundarys;
//            #endregion

//            return _allTarEles;
//        }
//    }
//}
