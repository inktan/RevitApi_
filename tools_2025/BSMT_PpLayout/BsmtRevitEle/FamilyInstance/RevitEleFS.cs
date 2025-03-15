using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.Revit.DB;
using g3;
using goa.Common.g3InterOp;
using goa.Common;
using PubFuncWt;

namespace BSMT_PpLayout
{
    /// <summary>
    /// 族实例
    /// 不同的族实例，线圈不同
    /// </summary>
    class RevitEleFS : RevitEleCtrl
    {
        internal FamilyInstance FamilyInstance  { get; }
        internal Transform Trans { get { return GetLCS_frame(this.FamilyInstance); } }
        internal double RotateAngle { get; }
        // internal Vector2d LocVector2d_Self { get; }//
        internal Vector2d LocVector2d { get; }//

        internal XYZ LocXyz { get; }
        internal RevitEleFS(Element ele, EleProperty eleProperty) : base(ele, eleProperty)
        {
            this.FamilyInstance = ele as FamilyInstance;
            this.LocXyz = (this.FamilyInstance.Location as LocationPoint).Point;
            this.RotateAngle = (this.FamilyInstance.Location as LocationPoint).Rotation;

            //this.LocVector2d_Self = LocXyz.ToVector2d();
            //this.LocVector2d = this.LocVector2d_Self + (this.FamilyInstance.FacingOrientation.ToVector2d()) * 2;//  基准点需要修改，应对情况，该点刚好与边界重合，无法判断是否位于线圈内 应对策略，将线圈放大，或者缩小

            this.LocVector2d = LocXyz.ToVector2d();
        }

        /// <summary>
        /// 实例参数锁定，或Rvt工具锁定
        /// </summary>
        internal bool PinnedByTool=>this.FamilyInstance.LookupParameter("锁定_").AsValueString() == "是" || this.FamilyInstance.Pinned;

        /// <summary>
        /// 填充区域类型
        /// </summary>
        internal string TypeName => this.FamilyInstance.Name;


        Transform GetLCS_frame(FamilyInstance _fi)
        {
            var origin = _fi.GetPos();
            var x = _fi.HandOrientation;
            var y = _fi.FacingOrientation;
            var z = x.CrossProduct(y);
            if (_fi.HandFlipped)
                z *= -1.0;
            if (_fi.FacingFlipped)
                z *= -1.0;

            Transform transform = Transform.Identity;

            transform.Origin = origin;
            transform.BasisX = x;
            transform.BasisY = y;
            transform.BasisZ = z;

            return transform;
        }
    }
}
