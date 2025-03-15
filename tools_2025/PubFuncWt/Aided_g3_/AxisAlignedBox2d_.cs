using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using g3;

namespace PubFuncWt
{
    public static class AxisAlignedBox2d_
    {
        public static AxisAlignedBox2d ScaleLeftRight(this AxisAlignedBox2d box2d, double left, double right)
        {
            return new AxisAlignedBox2d(box2d.Min - new Vector2d(left, 0), box2d.Max + new Vector2d(right, 0));
        }
        public static double Length(this AxisAlignedBox2d box2d)
        {
            return box2d.Max.x - box2d.Min.x;
        }
        public static double Width(this AxisAlignedBox2d box2d)
        {
            return box2d.Max.y - box2d.Min.y;
        }
        public static Vector2d LDp(this AxisAlignedBox2d box2d)
        {
            return box2d.Min;
        }
        public static Vector2d RDp(this AxisAlignedBox2d box2d)
        {
            Vector2d min = box2d.Min;
            Vector2d max = box2d.Max;
            return new Vector2d(max.x, min.y);
        }
        public static Vector2d RUp(this AxisAlignedBox2d box2d)
        {

            return box2d.Max;
        }
        public static Vector2d LUp(this AxisAlignedBox2d box2d)
        {
            Vector2d min = box2d.Min;
            Vector2d max = box2d.Max;
            return new Vector2d(min.x, max.y);
        }
      

    }
}
