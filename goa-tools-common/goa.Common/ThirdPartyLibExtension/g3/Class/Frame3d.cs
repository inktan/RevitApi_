using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using goa.Common.g3InterOp;

namespace g3
{
    public struct Frame3d
    {
        public static Frame3d Identity = new Frame3d(Vector3d.Zero, Vector3d.AxisX, Vector3d.AxisY, Vector3d.AxisZ);
        Quaterniond rotation;
        Vector3d origin;

        /// <summary>
        /// base vectors must be right-handed.
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        public Frame3d(Vector3d origin, Vector3d x, Vector3d y, Vector3d z)
        {
            this.origin = origin;
            Matrix3d m = new Matrix3d(x, y, z, false);
            this.rotation = m.ToQuaternion();
        }

        public Vector3d Origin
        {
            get { return origin; }
            set { origin = value; }
        }

        public Vector3d X
        {
            get { return rotation.AxisX; }
        }
        public Vector3d Y
        {
            get { return rotation.AxisY; }
        }
        public Vector3d Z
        {
            get { return rotation.AxisZ; }
        }

        ///<summary> Map point *into* local coordinates of Frame </summary>
        public Vector3d ToFrameP(Vector3d v)
        {
            v.x -= origin.x; v.y -= origin.y; v.z -= origin.z;
            return rotation.InverseMultiply(ref v);
        }
        /// <summary> Map point *from* local frame coordinates into "world" coordinates </summary>
        public Vector3d FromFrameP(Vector3d v)
        {
            return this.rotation * v + this.origin;
        }
        /// <summary> Map point *from* local frame coordinates into "world" coordinates </summary>
        public Vector3d FromFrameP(Vector2d v)
        {
            return this.rotation * v.ToVector3d() + this.origin;
        }

        ///<summary> Map vector *into* local coordinates of Frame </summary>
        public Vector3d ToFrameV(Vector3d v)
        {
            return rotation.InverseMultiply(ref v);
        }
        /// <summary> Map vector *from* local frame coordinates into "world" coordinates </summary>
        public Vector3d FromFrameV(Vector3d v)
        {
            return this.rotation * v;
        }
    }

}
