using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using g3;

namespace BSMT_PpLayout
{
    class KdTree
    {
        #region 构建 alglib.kdtree

        internal static alglib.kdtree ConstructTree<T>(List<T> nodes, List<Vector2d> nodeCoords)
        {
            //construct KNN tree, 
            var tree_nodes = convertFromG3(nodeCoords);
            int[] tree_tags = Enumerable.Range(0, nodes.Count).ToArray();
            var tree = new alglib.kdtree();
            alglib.kdtreebuildtagged(tree_nodes, tree_tags, 3, 0, 2, out tree);
            return tree;
        }
        public static List<T> SearchByCoord<T>(Vector2d p, List<T> nodes, alglib.kdtree _searchTree, int numNN, bool searchSelf = true)
        {
            var pKey = new double[3] { p.x, p.y, 0 };
            alglib.kdtreequeryknn(_searchTree, pKey, numNN, searchSelf);
            int[] tags_nn = new int[numNN];
            alglib.kdtreequeryresultstags(_searchTree, ref tags_nn);
            var knn = tags_nn.Select(x => nodes[x]).ToList();

            return knn;
        }
        internal static double[,] convertFromG3(List<Vector2d> _list)
        {
            var array = new double[_list.Count, 3];
            for (int i = 0; i < _list.Count; i++)
            {
                var p = _list[i];
                array[i, 0] = p.x;
                array[i, 1] = p.y;
                array[i, 2] = 0;
            }
            return array;
        }
        #endregion

    }
}
