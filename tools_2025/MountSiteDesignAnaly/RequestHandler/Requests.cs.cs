using System.Threading;

namespace MountSiteDesignAnaly
{
    internal enum RequestId : int
    {
        None,
        EaCalByGenericModel,// 土方计算
        CoverHeightDetection,// 净高检查
        BsmtSpaceHeightDetection,// 净高检查
        ClearCalculationResults, // 清除分析结果
        CalculateGreenSpaceRate, // 计算绿地率
        GenerateFloor,// 生成楼板
        RetainWallAnakysis_External,// 挡土墙分析 外部
        RetainWallAnakysis_Internal,// 挡土墙分析 内部
        RetainWallAnakysis_Site,// 挡土墙分析 场地
        Show,
        Hide,
        Test,
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
