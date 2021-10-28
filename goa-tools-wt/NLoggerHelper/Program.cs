using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NLoggerHelper
{
    class Program
    {
        static void Main(string[] args)
        {
            Logger _logger = LogManager.GetCurrentClassLogger();

            LoggerHelper.Instance.Debug("debug测试一下");
            LoggerHelper.Instance.Info("info测试一下");
            LoggerHelper.Instance.Warn("info测试一下");
            LoggerHelper.Instance.Trace("info测试一下");
            LoggerHelper.Instance.Error("info测试一下");
            LoggerHelper.Instance.Fatal("info测试一下");
        }
    }
}
