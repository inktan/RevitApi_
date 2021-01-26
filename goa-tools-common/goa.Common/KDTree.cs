using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.Revit.DB;

namespace goa.Common
{
    public class KDTreeXYZ
    {
        private alglib.kdtree tree;

        public KDTreeXYZ(List<XYZ> _coords)
        {
            //construct KNN tree, 
            var tree_nodes = convertFromXYZ(_coords);
            int[] tree_tags = Enumerable.Range(0, _coords.Count).ToArray();
            this.tree = new alglib.kdtree();
            alglib.kdtreebuildtagged(tree_nodes, tree_tags, 3, 0, 2, out tree);
        }
        public int[] SearchIndicesByCoord(XYZ p, int numNN)
        {
            var pKey = new double[3] { p.X, p.Y, p.Z };
            alglib.kdtreequeryknn(this.tree, pKey, numNN, true);
            int[] tags_nn = new int[numNN];
            alglib.kdtreequeryresultstags(this.tree, ref tags_nn);
            return tags_nn;
        }
        private static double[,] convertFromXYZ(List<XYZ> _list)
        {
            var array = new double[_list.Count, 3];
            for (int i = 0; i < _list.Count; i++)
            {
                var p = _list[i];
                array[i, 0] = p.X;
                array[i, 1] = p.Y;
                array[i, 2] = p.Z;
            }
            return array;
        }
        private static List<XYZ> convertToXYZ(double[,] _data, int _count)
        {
            var list = new List<XYZ>();
            for (int i = 0; i < _count; i++)
            {
                var x = _data[i, 0];
                var y = _data[i, 1];
                var z = _data[i, 2];
                list.Add(new XYZ(x, y, z));
            }
            return list;
        }
    }
}
