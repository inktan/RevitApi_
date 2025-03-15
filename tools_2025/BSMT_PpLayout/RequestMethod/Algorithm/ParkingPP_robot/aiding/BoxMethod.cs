using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using g3;

namespace BSMT_PpLayout
{
    /// <summary>
    /// 先粗略判断是否与矩形碰撞，再进行详细判断
    /// </summary>
    class BoxMethod
    {
        internal Polygon2d O;
        internal AxisAlignedBox2d Box2d;

        internal BoxMethod(Polygon2d o)
        {
            this.O = o;
            this.Box2d = o.GetBounds();
        }
    }
}
