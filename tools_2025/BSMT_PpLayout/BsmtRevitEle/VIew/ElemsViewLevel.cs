using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.Revit.DB;
using goa.Common.g3InterOp;
using g3;
using PubFuncWt;
using QuadTrees;
using System.Drawing;

namespace BSMT_PpLayout
{
    /// <summary>
    /// 在视图层级从全局进行元素控制
    /// </summary>
    class ElemsViewLevel
    {
        internal Document Doc => this.View.Document;
        internal View View { get; }

        internal ElemsViewLevel(Element ele)
        {
            this.View = ele as View;
            Computer();
        }
        internal void Computer()
        {
            this._eles = eles();
            this._QTreeRevitEles = qTreeRevitEles();
            this._bsmts = bsmts();
            this._unUsefulEles = unUsefulEles();
        }
        private List<Element> _eles { get; set; } // 对视图元素进行第一次清洗
        internal List<Element> Eles { get { return this._eles; } }// 对视图元素进行第一次清洗
        private List<Element> eles() // 对视图元素进行第一次清洗
        {
            List<ElementFilter> elementFilters = new List<ElementFilter>();

            elementFilters.Add(new ElementCategoryFilter(BuiltInCategory.OST_DetailComponents));// 详图项目
            elementFilters.Add(new ElementCategoryFilter(BuiltInCategory.OST_Lines));// 线
            //elementFilters.Add(new ElementCategoryFilter(BuiltInCategory.OST_IOSDetailGroups));
            //elementFilters.Add(new ElementCategoryFilter(BuiltInCategory.OST_Walls));

            LogicalOrFilter logicalOrFilter = new LogicalOrFilter(elementFilters);// 逻辑过滤器 - 或

            return (new FilteredElementCollector(this.Doc, this.View.Id)) // 使用视图构建过滤器-
                .WherePasses(logicalOrFilter)
                .WhereElementIsNotElementType()
                .Where(p => p.IsValidObject)
                .ToList();
        }

        private List<QTreeRevitEleCtrl> _QTreeRevitEles { get; set; }
        internal List<QTreeRevitEleCtrl> QTreeRevitEles { get { return _QTreeRevitEles; } }
        /// <summary>
        /// 元素提取
        /// </summary>
        /// <returns></returns>
        private List<QTreeRevitEleCtrl> qTreeRevitEles()
        {
            EleCtrlFactory eleCtrlFactory = new EleCtrlFactory(this.Eles, this.Doc, this.View);
            eleCtrlFactory.Computer();
            return eleCtrlFactory.QTreeRevitEleCtrls;
        }

        internal List<BsmtBound> BsmtBounds => this._QTreeRevitEles.Where(p => p.RevitEleCtrl is BsmtBound).Select(p => p.RevitEleCtrl as BsmtBound).ToList();

        private IEnumerable<Bsmt> _bsmts { get; set; }
        internal IEnumerable<Bsmt> Bsmts => this._bsmts;
        private IEnumerable<Bsmt> bsmts() // 基于地库边界线，对图元进行第二次清洗
        {
            QuadTreeRectF<QTreeRevitEleCtrl> qtree = new QuadTreeRectF<QTreeRevitEleCtrl>();
            qtree.AddRange(this.QTreeRevitEles);

            InitialUIinter initialUIinter = new InitialUIinter(new Autodesk.Revit.UI.UIApplication(this.View.Document.Application));
            foreach (var item in this.BsmtBounds)
            {

                RectangleF rectangleF = item.Ele.get_BoundingBox(this.View).ToRectangleF();

                // 首次使用四叉树，定位与地库边界相关的元素
                List<QTreeRevitEleCtrl> tarQTrRevitEles = qtree.GetObjects(rectangleF);
                yield return new Bsmt(tarQTrRevitEles, item);
            }
        }

        private IEnumerable<QTreeRevitEleCtrl> _unUsefulEles { get; set; }// 抓取所有在地库边界范围外的未锁定的图元 关键字 位_
        private IEnumerable<QTreeRevitEleCtrl> UnUsefulEles => this._unUsefulEles;
        private IEnumerable<QTreeRevitEleCtrl> unUsefulEles()
        {
            List<QTreeRevitEleCtrl> qTreeRevitEleCtrls = new List<QTreeRevitEleCtrl>();
            foreach (var item in this.Bsmts)
            {
                qTreeRevitEleCtrls.AddRange(item.QTreeRevitEleCtrls);
            }

            // 剔除位于地库边界范围内的停车位
            return this.QTreeRevitEles.Except(qTreeRevitEleCtrls).Where(p => p.RevitEleCtrl is RevitElePS || p.RevitEleCtrl is RevitEleCol);
        }

        /// <summary>
        /// 清除与地库边界暧昧不清的图元
        /// Boundingbox是不精确的
        /// </summary>
        internal void DelUnUsefulEles()
        {
            using (Transaction transDelete = new Transaction(this.Doc, "DeleteUnUsefulEleIds"))
            {
                transDelete.Start();
                this.Doc.Delete(this.UnUsefulEles.Select(p => p.RevitEleCtrl.Id).ToList());
                transDelete.Commit();
            }
        }

    }
}
