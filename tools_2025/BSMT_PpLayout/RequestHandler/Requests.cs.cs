using System.Threading;

namespace BSMT_PpLayout
{
    internal enum RequestId : int
    {
        None,
        SelBsmtExWall,// 加选地库方案
        GlobalAroundBasedExistRoadSystem,// 全局强制刷新
        RefreshDataStatistics,// 统计数据刷新
        //ChangeDirectionByRectangle,
        //ChangeDirectionByBoundary,
        PointPillar,
        SelColumsFs,
        SelPSByLine,
        CutAlgorithm,
        RestBsmtWallEles,
        LookForEndPS,
        //TowerDistanceCheck,
        //BatchBreakLine,批量断线
        //Backtrack,
        LineArray,
        DimensionMarking,
        GridMarking,
        //MechanicalParkingReplace,// 机械车位替换
        //ReMechanicalParkingReplace,// 机械车位替换
        ShowDesignLocation,
        AddSequenceNum,
        RoadIntersecIconChamfer,
        //ModifySurfaceTransparency,
        UpdatedPsTypes,
        CheckCollisions,
        //FourRulesNorSou,// 道路生成体系
        Road2Growth,
        RampGenerated,// 坡道生成
        DivideStorageRoom,// 一键划分储藏间
        FRoffset,// 详图区域偏移
        SubAreaLaneGeneration,// 指定子区域，在水平方向进行车道优化排布
        DrawBsmtWallLine,
        AdjustStartPoint,
        ReadPanelData,
        UpdateDesignList,
        ExtractCad,
        AutoPathfinding,// 自动寻路
        BrushLayer,// 刷图层
        LineCheck,// 两点阵列
        ArcArray,// 弧线阵列
        Temp01,
        Temp02,
        Temp03,
        Temp04,
        Temp05,
        Temp06,
    }

    public class Request
    {
        private int m_request = (int)RequestId.None;
        internal RequestId Take()
        {
            return (RequestId)Interlocked.Exchange(ref m_request, (int)RequestId.None);
        }

        internal void Make(RequestId request)
        {
            Interlocked.Exchange(ref m_request, (int)request);
        }
    }
}
