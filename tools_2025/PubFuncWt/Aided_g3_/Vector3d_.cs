using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using g3;

namespace PubFuncWt
{
    public static class Vector3d_
    {
        /// <summary>
        /// 旋转轴方向指向眼睛，进行逆时针旋转
        /// </summary>
        /// <returns></returns>
        public static Vector3d Rotate(this Vector3d vector3d, Vector3d axis, double AngleDeg)
        {
            Quaterniond quaterniond = new Quaterniond(axis.Normalized, AngleDeg);
            return MeshTransforms.Rotate(vector3d, new Vector3d(0, 0, 0), quaterniond);
        }

        public static Vector3d Move(this Vector3d vector3d, Vector3d dis)
        {
            return new Vector3d(vector3d.x + dis.x, vector3d.y + dis.y, vector3d.z + dis.z);
        }

        public static Vector3f To3f(this Vector3d vector3d)
        {
            return new Vector3f(vector3d.x, vector3d.y, vector3d.z);
        }


    }
}
