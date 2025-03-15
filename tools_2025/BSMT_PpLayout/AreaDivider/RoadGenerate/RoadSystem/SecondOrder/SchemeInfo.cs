using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using g3;
using PubFuncWt;

namespace BSMT_PpLayout
{
    class SchemeInfo
    {
        internal List<List<Route>> Designs;
        internal Polygon2d Region;

        internal SchemeInfo()
        {
            this.Designs = new List<List<Route>>();
        }

        internal void FromFrame2d(Frame3d frame3d)
        {
            List<List<Route>> result = new List<List<Route>>();
            foreach (List<Route> routes in this.Designs)
            {
                List<Route> subResult = new List<Route>();
                foreach (Route route in routes)
                {
                    Segment2d segment2d = frame3d.FromFrame2dSeg(route.Segment2d) ;
                    subResult.Add(new Route(segment2d));
                }
                result.Add(subResult);
            }
            this.Designs = result;

            this.Region = frame3d.FromFrame2dPoly(this.Region);

        }
    }
}
