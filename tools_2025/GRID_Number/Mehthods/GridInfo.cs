using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PubFuncWt;

namespace GRID_Number
{
    public class GridInfo
    {
        internal Grid Grid { get; set; }
        internal Line Line { get; set; }
        internal XYZ InterSectPoint { get; set; }
        /// <summary>
        /// 交点在基准线上的投影位置
        /// </summary>
        internal double intersectPara => Line.Project(InterSectPoint).Parameter;

        public GridInfo()
        {
        }

    }
    public class GridInfoHandle
    {
        internal List<GridInfo> GridInfos { get; set; }
        public GridInfoHandle(List<GridInfo> gridInfos)
        {
            this.GridInfos = gridInfos;
        }

        internal List<GridInfo> DupGridInfos = new List<GridInfo>();
        internal List<GridInfo> TarGridInfos = new List<GridInfo>();

        List<List<GridInfo>> gridInfosLst = new List<List<GridInfo>>();

        internal void RemoveDuplicates()
        {
            double temp = this.GridInfos.First().intersectPara;

            List<GridInfo> reuslt = new List<GridInfo>();
            for (int i = 0; i < GridInfos.Count; i++)
            {
                if (GridInfos[i].intersectPara.EqualPrecision(temp))
                {
                    reuslt.Add(GridInfos[i]);
                }
                else
                {
                    gridInfosLst.Add(reuslt);
                    temp = this.GridInfos[i].intersectPara;
                    reuslt = new List<GridInfo>() { GridInfos[i] };
                }

                if (i == GridInfos.Count - 1)
                {
                    gridInfosLst.Add(reuslt);
                }
            }

            foreach (var item in gridInfosLst)
            {
                if (item.Count > 1)
                {
                    List<GridInfo> gridInfos = item.OrderBy(p => p.Grid.Curve.Length).ToList();

                    for (int i = 0; i < gridInfos.Count - 1; i++)
                    {
                        DupGridInfos.Add(gridInfos[i]);

                    }
                    TarGridInfos.Add(gridInfos.Last());
                }
                else
                {
                    TarGridInfos.AddRange(item);
                }

            }

        }
    }
}
