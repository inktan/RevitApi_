using Autodesk.Revit.DB;
using g3;
using goa.Common.g3InterOp;
using PubFuncWt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSMT_PpLayout
{
    class ColGeneration
    {
        internal double Wd { get { return GlobalData.Instance.Wd_sec_num; } }
        internal double ColumnWidth { get { return GlobalData.Instance.ColumnWidth_num; } }// 柱宽
        internal double ColBufDis { get { return GlobalData.Instance.ColumnBurfferDistance_num; } }// 柱子空隙距离
        internal double ColBackDis { get { return GlobalData.Instance.ColumnBackwardDistance_num; } }// 柱子后退距离

        internal RevitElePS PS { get; }
        internal ColGeneration(RevitElePS _pS)
        {
            this.PS = _pS;
        }

        internal Transform Transform { get { return this.PS.Trans; } }
        internal double Width { get { return this.PS.Width; } }
        internal double Height { get { return this.PS.Height; } }

        internal List<ColUnitSpace> ColumnUnitSpaces
        {
            get
            {
                if (GlobalData.Instance.BigOrsmallColumn == BigOrSmallColumn.Big)
                    return GetColumnUnitSpace_Big();
                else if (GlobalData.Instance.BigOrsmallColumn == BigOrSmallColumn.Small)
                    return GetColumnUnitSpace_Small();
                else
                    return GetColumnUnitSpace_Big();
            }
        }
        /// <summary>
        /// transmform的停车位族示例的四个角点位置-小柱网
        /// </summary>
        internal List<ColUnitSpace> GetColumnUnitSpace_Small()
        {
            Vector2d lD = this.PS.LocVector2d;
            Vector2d lU = this.PS.LocVector2d;
            Vector2d rU = this.PS.LocVector2d;
            Vector2d rD = this.PS.LocVector2d;

            Vector2d baceDis = (this.ColBackDis+ this.ColumnWidth / 2) * this.PS.FamilyInstance.FacingOrientation.ToVector2d();// 柱头后退距离 当前柱子位置位于停车位最底边线中点，使用FacingOrientation的方向，向上移动
            // 右下角
            Vector2d moveLR = (this.Width / 2 + this.ColBufDis / 2 + this.ColumnWidth / 2) * this.PS.FamilyInstance.HandOrientation.ToVector2d();// 左右移动
            rD = rD + moveLR + baceDis;// 左右+向上运动
            // 左下角
            lD = lD - moveLR + baceDis;// 左右+向上运动

            // 右上角
            Vector2d moveHeight = this.Height * this.PS.FamilyInstance.FacingOrientation.ToVector2d();
            rU = rU + moveLR + moveHeight;
            // 左上角
            lU = lU - moveLR + moveHeight;

            // 数据转换
            ColUnitSpace lDColUnitSpace = ToColumnUnitSpace(lD);
            ColUnitSpace leftUpColumnUnitSpace = ToColumnUnitSpace(lU);
            ColUnitSpace rightUpColumnUnitSpace = ToColumnUnitSpace(rU);
            ColUnitSpace rightDownColumnUnitSpace = ToColumnUnitSpace(rD);

            return new List<ColUnitSpace>() { lDColUnitSpace, rightDownColumnUnitSpace, rightUpColumnUnitSpace, leftUpColumnUnitSpace };
        }
        /// <summary>
        /// transmform的停车位族示例的四个角点位置-大柱网
        /// </summary>
        internal List<ColUnitSpace> GetColumnUnitSpace_Big()
        {
            Vector2d lD = this.PS.LocVector2d;
            Vector2d rD = this.PS.LocVector2d;

            // 距离来自于公式
            double backDistance = ((2 * this.Height + this.Wd) / 2 - this.Wd) / 2;
            Vector2d baceDis = backDistance * this.PS.FamilyInstance.FacingOrientation.ToVector2d();

            // 右下角
            Vector2d moveLR = (this.Width/2 + this.ColBufDis / 2 + this.ColumnWidth / 2) * this.PS.FamilyInstance.HandOrientation.ToVector2d();// 左右移动
            rD = rD + moveLR + baceDis;
            // 左下角
            lD = lD - moveLR + baceDis;

            // 数据转换
            ColUnitSpace lUUnitSpace = ToColumnUnitSpace(lD);
            ColUnitSpace rUUnitSpace = ToColumnUnitSpace(rD);

            return new List<ColUnitSpace>() { rUUnitSpace, lUUnitSpace };
        }

        internal double RotateAngle
        {
            get
            {
                return this.PS.RotateAngle;
            }
        }

        /// <summary>
        /// 基于柱子中心，创建柱子线圈
        /// </summary>
        internal ColUnitSpace ToColumnUnitSpace(Vector2d p)
        {
            double distance = this.ColumnWidth / 2;
            // 上下 左右 
            Vector2d lD = p - distance * this.PS.FamilyInstance.FacingOrientation.ToVector2d() - distance * this.PS.FamilyInstance.HandOrientation.ToVector2d();
            Vector2d lU = p + distance * this.PS.FamilyInstance.FacingOrientation.ToVector2d() - distance * this.PS.FamilyInstance.HandOrientation.ToVector2d();
            Vector2d rU = p + distance * this.PS.FamilyInstance.FacingOrientation.ToVector2d() + distance * this.PS.FamilyInstance.HandOrientation.ToVector2d();
            Vector2d rD = p - distance * this.PS.FamilyInstance.FacingOrientation.ToVector2d() + distance * this.PS.FamilyInstance.HandOrientation.ToVector2d();
            Polygon2d polygon2d = new Polygon2d(new List<Vector2d>() { lU, lD, rD, rU });
            ColUnitSpace leftDownColumnUnitSpace = new ColUnitSpace(polygon2d, new ColLocPoint(p, this.RotateAngle));
            return leftDownColumnUnitSpace;
        }
    }
}
