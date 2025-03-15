using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.Revit.DB;

using g3;
using ClipperLib;
using goa.Common;
using goa.Common.g3InterOp;
using PubFuncWt;

namespace BSMT_PpLayout
{
    using cInt = Int64;

    using Path = List<IntPoint>;
    using Paths = List<List<IntPoint>>;

    /// <summary>
    /// 基于南北关系，设计4种平行车道规则
    /// </summary>
    class FourRoadDistriNnorSou
    {

        CellArea CellArea;

        // 数据抓取
        double Wd = GlobalData.Instance.Wd_pri_num;// 路宽
        double pSHeight = GlobalData.Instance.pSHeight_num;// 车长vector

        double regionWidth;
        double reginHeight;

        // 几何数据处理
        Polygon2d Region;

        Vector2d RightUp;
        Vector2d RightDown;

        internal FourRoadDistriNnorSou(CellArea cellArea)
        {
            this.CellArea = cellArea;

            this.Region = cellArea.Polygon2d;

            this.regionWidth = this.Region.Width();
            this.reginHeight = this.Region.Height();

            this.RightUp = this.Region.RUpOfBox2d();
            this.RightDown = this.Region.RDpOfBox2d();
        }

        internal SchemeInfo Computer()
        {
            // 解决排列组合问题 ==>
            SchemeInfo schemeInfo = new SchemeInfo();

            schemeInfo.Designs.Add(RightDown01().ToList());
            schemeInfo.Designs.Add(RightDown02().ToList());

            schemeInfo.Designs.Add(RightUp01().ToList());
            schemeInfo.Designs.Add(RightUp02().ToList());

            if (this.CellArea.UpSeg.EleProperty == EleProperty.Lane )
            {
                schemeInfo.Designs.Add(RightUp03().ToList());
            }
            if (this.CellArea.DownSeg.EleProperty != EleProperty.Lane)
            {
                schemeInfo.Designs.Add(RightDown03().ToList());
            }

            return schemeInfo;
        }
        #region 不同的定位起点

        /*
         * 
         * 基于下边界第一排为车道 - RightDown01() - 自下而上
         * 
         * 基于下边界第二排为车道 - RightDown02() - 自下而上
         * 
         * 基于下边界第一排与第二排均为车道 - RightDown03() - 自下而上
         * 
         * 基于上边界第一排为车道 - RightUp01() - 自上而下
         * 
         * 基于上边界第二排为车道 - RightUp02() - 自上而下
         * 
         * 基于上边界第一排与第二排为车道 - RightUp03() - 自上而下
         * 
         */

        internal IEnumerable<Route> RightDown01()
        {
            double spacingDistance = this.pSHeight * 2 + this.Wd;// 车道上移距离
            double distance = this.Wd / 2;// 当前车道以及车道间空间在y方向上的距离

            // 提取基准点
            Vector2d basicVect = this.RightDown;
            int i = 0;
            while (true)
            {
                if (this.reginHeight - distance < this.Wd / 2 - Precision_.Precison)
                {
                    break;
                }
                if (i == 0)
                {
                    basicVect += new Vector2d(0, this.Wd / 2);
                }
                else
                {
                    basicVect += new Vector2d(0, spacingDistance);
                }
                // 构建车道中心线
                Segment2d seg2d = new Segment2d(basicVect, basicVect - new Vector2d(this.regionWidth, 0));

                // 循环中断条件
                distance += spacingDistance;

                i++;

                yield return new Route(seg2d);
            }
        }
        internal IEnumerable<Route> RightDown02()
        {
            double spacingDistance = this.pSHeight * 2 + this.Wd;// 车道上移距离
            double distance = this.pSHeight + this.Wd / 2;// 当前车道以及车道间空间在y方向上的距离

            // 提取基准点
            Vector2d basicVect = this.RightDown;
            int i = 0;
            while (true)
            {
                if (this.reginHeight - distance < this.Wd / 2 - Precision_.Precison)
                {
                    break;
                }
                if (i == 0)
                {
                    basicVect += new Vector2d(0, this.Wd / 2 + this.pSHeight);
                }
                else
                {
                    basicVect += new Vector2d(0, spacingDistance);
                }
                // 构建车道中心线
                Segment2d seg2d = new Segment2d(basicVect, basicVect - new Vector2d(this.regionWidth, 0));

                // 循环中断条件
                distance += spacingDistance;// 距离判断出现了问题

                i++;

                yield return new Route(seg2d);
            }
        }
        internal IEnumerable<Route> RightDown03()
        {
            double spacingDistance = this.pSHeight * 2 + this.Wd;// 车道上移距离
            double distance = this.pSHeight*2 + this.Wd / 2;// 当前车道以及车道间空间在y方向上的距离

            // 提取基准点
            Vector2d basicVect = this.RightDown;
            int i = 0;
            while (true)
            {
                if (this.reginHeight - distance < this.Wd / 2 - Precision_.Precison)
                {
                    break;
                }
                if (i == 0)
                {
                    basicVect += new Vector2d(0, this.Wd / 2 + this.pSHeight*2);
                }
                else
                {
                    basicVect += new Vector2d(0, spacingDistance);
                }
                // 构建车道中心线
                Segment2d seg2d = new Segment2d(basicVect, basicVect - new Vector2d(this.regionWidth, 0));

                // 循环中断条件
                distance += spacingDistance;// 距离判断出现了问题

                i++;

                yield return new Route(seg2d);
            }
        }


        internal IEnumerable<Route> RightUp01()
        {
            double spacingDistance = this.pSHeight * 2 + this.Wd;// 车道上移距离
            double distance = this.Wd / 2;// 当前车道以及车道间空间在y方向上的距离

            // 提取基准点
            Vector2d basicVect = this.RightUp;
            int i = 0;
            while (true)
            {
                if (this.reginHeight - distance < this.Wd / 2 - Precision_.Precison)
                {
                    break;
                }
                if (i == 0)
                {
                    basicVect -= new Vector2d(0, this.Wd / 2);
                }
                else
                {
                    basicVect -= new Vector2d(0, spacingDistance);
                }
                // 构建车道中心线
                Segment2d seg2d = new Segment2d(basicVect, basicVect - new Vector2d(this.regionWidth, 0));

                // 循环中断条件
                distance += spacingDistance;

                i++;

                yield return new Route(seg2d);
            }
        }
        internal IEnumerable<Route> RightUp02()
        {
            double spacingDistance = this.pSHeight * 2 + this.Wd;// 车道上移距离
            double distance = this.Wd / 2 + this.pSHeight;// 当前车道以及车道间空间在y方向上的距离

            // 提取基准点
            Vector2d basicVect = this.RightUp;
            int i = 0;
            while (true)
            {
                if (this.reginHeight - distance < this.Wd / 2 - Precision_.Precison)
                {
                    break;
                }

                if (i == 0)
                {
                    basicVect -= new Vector2d(0, this.Wd / 2 + this.pSHeight);
                }
                else
                {
                    basicVect -= new Vector2d(0, spacingDistance);
                }
                // 构建车道中心线
                Segment2d seg2d = new Segment2d(basicVect, basicVect - new Vector2d(this.regionWidth, 0));

                // 循环中断条件
                distance += spacingDistance;

                i++;

                yield return new Route(seg2d);
            }
        }
        internal IEnumerable<Route> RightUp03()
        {
            double spacingDistance = this.pSHeight * 2 + this.Wd;// 车道上移距离
            double distance = this.Wd / 2 + this.pSHeight*2;// 当前车道以及车道间空间在y方向上的距离

            // 提取基准点
            Vector2d basicVect = this.RightUp;
            int i = 0;
            while (true)
            {
                if (this.reginHeight - distance < this.Wd / 2 - Precision_.Precison)
                {
                    break;
                }

                if (i == 0)
                {
                    basicVect -= new Vector2d(0, this.Wd / 2 + this.pSHeight*2);
                }
                else
                {
                    basicVect -= new Vector2d(0, spacingDistance);
                }
                // 构建车道中心线
                Segment2d seg2d = new Segment2d(basicVect, basicVect - new Vector2d(this.regionWidth, 0));

                // 循环中断条件
                distance += spacingDistance;

                i++;

                yield return new Route(seg2d);
            }
        }

        #endregion


    }
}
