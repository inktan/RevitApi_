using g3;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace g3
{
    /// <summary>
    /// 射线
    /// </summary>
    class Ray2d
    {
        public Vector2d Origin;
        public Vector2d Direction;

        public Ray2d(Vector2d origin, Vector2d direction)
        {
            this.Origin = origin;
            this.Direction = direction;
            this.Direction.Normalize();     // float cast may not be normalized in double, is trouble in algorithms!
        }

    }
}
