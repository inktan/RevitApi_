using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.Revit.DB;

using NetTopologySuite.Geometries;
using NetTopologySuite.Operation.Polygonize;
using NetTopologySuite.Operation.Overlay;
using NetTopologySuite.Operation.Union;
using NetTopologySuite.Operation.Buffer;
using NetTopologySuite.GeometriesGraph;
using NetTopologySuite.Simplify;
using NetTopologySuite.Mathematics;
using NetTopologySuite.Precision;

namespace goa.Common.NtsExtension
{
    public static class Method_NTS_extension
    {
        public static Polygon FullySimplify(this Polygon _input)
        {
            var shell = FullySimplify(_input.Shell);
            if (shell == null)
                return null;
            var holes = _input.Holes
                .Select(x => FullySimplify(x))
                .Where(x => x != null);
            return new Polygon(shell, holes.ToArray());
        }
        public static LinearRing FullySimplify(this LinearRing _ring, double _threshold = 0.02)
        {
            //first round of simplify
            var roughSimplifyGeom = DouglasPeuckerSimplifier.Simplify(_ring, _threshold);
            if (roughSimplifyGeom is LinearRing == false)
                return null;
            var roughSimplify = roughSimplifyGeom as LinearRing;
            //check the starting point of each polygon's contours
            //shift to a corner if it lies on en edge
            //otherwise cannot be full simplified
            var shifted = ShiftStartToCorner(roughSimplify);
            var fullySimplified = DouglasPeuckerSimplifier.Simplify(shifted, _threshold) as LinearRing;
            return fullySimplified;
        }
        public static LinearRing ShiftStartToCorner(LinearRing _ring)
        {
            var coords = _ring.Coordinates.ToList();
            if (coords.Count == 0)
                return _ring;
            coords.RemoveAt(coords.Count - 1);
            //find the first index of a corner
            int index = 0;
            for (int i = 0; i < coords.Count; i++)
            {
                Coordinate pre, next;
                if (i == 0)
                {
                    pre = coords.Last();
                    next = coords[i + 1];
                }
                else if (i == coords.Count - 1)
                {
                    pre = coords[i - 1];
                    next = coords[0];
                }
                else
                {
                    pre = coords[i - 1];
                    next = coords[i + 1];
                }

                var v = new Vector2D(coords[i]);
                var vPre = new Vector2D(pre);
                var vNext = new Vector2D(next);
                var dir0 = v.Subtract(vPre).Normalize();
                var dir1 = vNext.Subtract(v).Normalize();
                bool shareLine = dir0.Subtract(dir1).LengthSquared().IsAlmostEqualByDifference(0);
                if (!shareLine)
                {
                    index = i;
                    break;
                }
            }

            //shift list by index
            var newCoords = coords.ShiftLeft(index).ToList();
            //add first item to the back
            newCoords.Add(newCoords.First());
            return new LinearRing(newCoords.ToArray());
        }
    }
}
