using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using g3;

namespace goa.Common
{
    public static class Method_g3Extension
    {
        public static Vector3f Rotate(this Vector3f _v, Vector3f _axis, float _deg)
        {
            Quaternionf ro = new Quaternionf(_axis, _deg);
            var trans = new TransformSequence();
            trans.AppendRotation(ro);
            return trans.TransformP(_v);
        }
    }
}
