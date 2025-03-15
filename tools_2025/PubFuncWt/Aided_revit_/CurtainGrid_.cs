using Autodesk.Revit.DB;
using g3;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PubFuncWt
{
    public static class CurtainGrid_
    {
        /// <summary>
        /// 判断两个幕墙网格是否一模一样
        /// </summary>
        /// <param name="wall01"></param>
        /// <param name="wall02"></param>
        public static bool IsSameCurtainWall(this Wall curtainWall01, Wall curtainWall02, double precision)
        {
            // 01 判断基础数量是否一致
            // 判断两个幕墙网格的元素组成数量是否一致=包含、嵌板数量、UV网格数量
            if (curtainWall01.CurtainGrid.NumPanels != curtainWall02.CurtainGrid.NumPanels)// 嵌板数量
            {
                return false;
            }
            if (curtainWall01.CurtainGrid.NumULines != curtainWall02.CurtainGrid.NumULines)// U方向分割数量
            {
                return false;
            }
            if (curtainWall01.CurtainGrid.NumVLines != curtainWall02.CurtainGrid.NumVLines)// V方向分割数量
            {
                return false;
            }

            // 02 判断 网格线分段数量 网格线每分段上是否放置竖梃 网格线位置 == 是否一致

            CurtainWallCtrl_ curtainWallCtrl_01 = new CurtainWallCtrl_(curtainWall01);
            CurtainWallCtrl_ curtainWallCtrl_02 = new CurtainWallCtrl_(curtainWall02);

            Frame3d frame3d01 = curtainWallCtrl_01.Frame3d;
            Frame3d frame3d02 = curtainWallCtrl_02.Frame3d;

            curtainWallCtrl_01.GetGrids();
            curtainWallCtrl_02.GetGrids();

            curtainWallCtrl_01.GetGridSegments();// 获取所有的幕墙网格线的详细信息 key为fullcurve的首尾点信息 value包含网格线的分段模式
            curtainWallCtrl_02.GetGridSegments();

            // 03 竖梃类型
            curtainWallCtrl_01.GetMullions();
            curtainWallCtrl_02.GetMullions();

            // 判断每根网格线的竖梃放置模式是否一致
            List<CurtainGridLineCtrl_> gridLineCtrl_s01 = curtainWallCtrl_01.GridLineCtrl_s.Values.ToList();
            List<CurtainGridLineCtrl_> gridLineCtrl_s02 = curtainWallCtrl_02.GridLineCtrl_s.Values.ToList();
            for (int i = 0; i < gridLineCtrl_s01.Count; i++)
            {
                CurtainGridLineCtrl_ gridLineCtrl_01 = gridLineCtrl_s01[i];
                CurtainGridLineCtrl_ gridLineCtrl_02 = gridLineCtrl_s02[i];
                // 每根网格线上各分段上，是否存在竖梃
                if (gridLineCtrl_01.SegmentPattern.Count != gridLineCtrl_02.SegmentPattern.Count)
                {
                    return false;
                }

                for (int j = 0; j < gridLineCtrl_01.SegmentPattern.Count; j++)
                {
                    var yes0 = gridLineCtrl_01.SegmentPattern[j];
                    var yes1 = gridLineCtrl_02.SegmentPattern[j];
                    if (yes0 != yes1)
                    {
                        return false;
                    }
                }
           
                // 03 竖梃类型
                List<Mullion> mullions01 = gridLineCtrl_01.Mullions.Values.ToList();
                List<Mullion> mullions02 = gridLineCtrl_02.Mullions.Values.ToList();

                if (mullions01.Count != mullions02.Count)
                {
                    return false;
                }

                if (mullions01.Count > 0)
                {
                    for (int j = 0; j < mullions01.Count; j++)
                    {
                        var mullion01 = mullions01[j];
                        var mullion02 = mullions02[j];
                        if (mullion01.GetTypeId() != mullion02.GetTypeId())
                        {
                            return false;
                        }
                    }
                }

                // 每根网格线的位置是否一致
                // 需要进行frame3d转换
                Curve curve01 = gridLineCtrl_01.GridLine.FullCurve;
                Curve curve02 = gridLineCtrl_02.GridLine.FullCurve;

                XYZ xYZ01_1 = curve01.GetEndPoint(0);
                XYZ xYZ01_2 = curve01.GetEndPoint(1);
                Vector3d vector3d01_1 = frame3d01.ToFrameP(xYZ01_1.ToVector3d());
                Vector3d vector3d01_2 = frame3d01.ToFrameP(xYZ01_2.ToVector3d());

                XYZ xYZ02_1 = curve02.GetEndPoint(0);
                XYZ xYZ02_2 = curve02.GetEndPoint(1);
                Vector3d vector3d02_1 = frame3d02.ToFrameP(xYZ02_1.ToVector3d());
                Vector3d vector3d02_2 = frame3d02.ToFrameP(xYZ02_2.ToVector3d());

                if (vector3d01_1.Distance(vector3d02_1) > precision)
                {
                    return false;
                }
                if (vector3d01_2.Distance(vector3d02_2) > precision)
                {
                    return false;
                }
            }

            // 04 嵌板类型 基于嵌板判断镜像关系
            curtainWallCtrl_01.GetPanels();
            curtainWallCtrl_02.GetPanels();

            List<Element> elements01 = curtainWallCtrl_01.Panels.Values.ToList();
            List<Element> elements02 = curtainWallCtrl_02.Panels.Values.ToList();

            for (int i = 0; i < elements01.Count(); i++)
            {
                Element ele01 = elements01[i];
                Element ele02 = elements02[i];
                if (ele01.GetTypeId() != ele02.GetTypeId())
                {
                    return false;
                }
                // 判断是否有镜像关系，镜像也需要区分
                // 嵌板镜像不足以证明幕墙被镜像，此处修改为手动添加注释模式

                //if (ele01 is Panel)
                //{
                //    bool isMirrored01 = (ele01 as Panel).Mirrored;
                //    bool isMirrored02 = (ele02 as Panel).Mirrored;
                //    if (isMirrored01!= isMirrored02)
                //    {
                //        return false;
                //    }
                //}
                //else if (ele01 is FamilyInstance)
                //{
                //    bool isMirrored01 = (ele01 as FamilyInstance).Mirrored;
                //    bool isMirrored02 = (ele02 as FamilyInstance).Mirrored;
                //    if (isMirrored01 != isMirrored02)
                //    {
                //        return false;
                //    }
                //}
            }

            return true;
        }

        public static IEnumerable<CurtainGridLineCtrl_> GetUGridLines(this Wall CurtainWall)
        {
            var doc = CurtainWall.Document;
            foreach (var item in CurtainWall.CurtainGrid.GetUGridLineIds())
            {
                yield return new CurtainGridLineCtrl_(doc.GetElement(item) as CurtainGridLine);
            }

        }
        public static IEnumerable<CurtainGridLineCtrl_> GetVGridLines(this Wall CurtainWall)
        {
            var doc = CurtainWall.Document;
            foreach (var item in CurtainWall.CurtainGrid.GetVGridLineIds())
            {
                yield return new CurtainGridLineCtrl_(doc.GetElement(item) as CurtainGridLine);
            }

        }

    }
}
