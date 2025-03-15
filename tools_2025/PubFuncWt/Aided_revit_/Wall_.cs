using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PubFuncWt
{
    public static class Wall_
    {
        /// <summary>
        /// 创建只有厚度的结构墙体
        /// </summary>
        /// <param name="wallType"></param>
        /// <param name="dThickness"></param>
        public static WallType NewWallType(this Document document, string newWallType, double dThickness)
        {
            WallType wallType = ((new FilteredElementCollector(document))
                .OfCategory(BuiltInCategory.OST_Walls)
                .Where(p => p is WallType)
                .Where(p => (p as WallType)
                .FamilyName == "基本墙")
                .FirstOrDefault() as WallType)
                .Duplicate(newWallType) as WallType;

            CompoundStructure cs = wallType.GetCompoundStructure();

            // 设置结构层
            CompoundStructureLayer compoundStructureLayer = new CompoundStructureLayer();
            compoundStructureLayer.Width = dThickness;

            //墙体构造层收集器
            IList<CompoundStructureLayer> lstLayers = new List<CompoundStructureLayer>() { compoundStructureLayer };

            //修改后要设置一遍
            cs.SetLayers(lstLayers);
            wallType.SetCompoundStructure(cs);

            return wallType;
        }
    }
}
