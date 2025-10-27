using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using goa.Common;

namespace XYY_lib
{
    

    public static class Xyy_curves
    {
        public static bool IfcurveIsLine(this Curve c)
        {
            XYZ startp = c.GetEndPoint(0);
            XYZ endp = c.GetEndPoint(1);
            var length = c.Length;
            var distance = startp.DistanceTo(endp);
            if (length == distance)
            {
                return true;
            }
            else
            {
                return false;
            }

        }
    }

    public static class Xyy_points
    {
        public static bool Definehavekey(Dictionary<XYZ,List<XYZ>>dic,XYZ key)
        {
            bool ifequal = false;
            foreach(XYZ xyz in dic.Keys)
            {
                if(key.X==xyz.X&&key.Y==xyz.Y&&key.Z==xyz.Z)
                {
                    ifequal = true;
                }
            }
            return ifequal;
        }




        public static List<XYZ> DistinctLowPt(this List<XYZ> originpts)
        {

            List<XYZ> pts = new List<XYZ>();
            foreach(XYZ p in originpts)
            {
                XYZ newpt = new XYZ(Math.Round(p.X,8),Math.Round( p.Y,8),Math.Round(  p.Z,8));
               
                pts.Add(newpt);
            }



            List<XYZ> uniqpts = new List<XYZ>();
            List<XYZ> samepts = new List<XYZ>();
            foreach (XYZ pt in pts)
            {
                var x = pt.X;
                var y = pt.Y;
                var z = pt.Z;
                int i = 0;
           
                foreach (XYZ ppt in pts)
                {
                    if (x.IsAlmostEqualByDifference(ppt.X,0.001)  && y.IsAlmostEqualByDifference(ppt.Y, 0.001))
                    {
                        i = i + 1;
                    }
                }
                if(i==1)
                {
                    uniqpts.Add(pt);
                }
                else
                {
                    samepts.Add(pt);
                }
            }
            

            Dictionary<XYZ, List<XYZ>> dic = new Dictionary<XYZ, List<XYZ>>();
           
            foreach(XYZ p in samepts)
            {
                XYZ key = new XYZ(p.X, p.Y, 0);
                if (Definehavekey(dic,key)==false)
               
                {
                    List<XYZ> list = new List<XYZ>();
                    list.Add(p);
                    dic.Add(key, list);
                    foreach (XYZ pp in samepts)
                    {
                        if (p.X == pp.X && p.Y == pp.Y)
                        {
                            dic[key].Add(pp);
                        }
                    }
                }

                   
            }
           foreach(XYZ kk in dic.Keys)
            {
                double xx = kk.X;
                double yy = kk.Y;
                double maxz = kk.Z;

                foreach(XYZ kkk in dic[kk])
                {
                    if(kkk.Z>=maxz)
                    {
                        maxz = kkk.Z;
                    }
                }
                XYZ finalpt = new XYZ(xx, yy, maxz);
                uniqpts.Add(finalpt);
            }

            


            return uniqpts;
        }

    }

    
}
