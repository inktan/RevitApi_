//using NLog;
//using NLog.Config;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Reflection;
//using System.Text;
//using System.Threading.Tasks;

//namespace PubFuncWt
//{
//    /// <summary>
//    /// nLog使用帮助类
//    /// </summary>
//    public class NLogger
//    {
//        /// <summary>
//        /// 实例化nLog，即为获取配置文件相关信息
//        /// </summary>
//        private  Logger _logger 
//        {
//            get
//            {
//                // 读取配置文件的路径
//                string nlogConfigPath = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "NLog.config");
//                nlogConfigPath = @"W:\BIM_ARCH\03.插件\goa tools 当前版本\Content\goa_tools\NLog.config";
//                //InputData.Instance.backStr += "\n" + nlogConfigPath;
//                LogManager.Configuration = new XmlLoggingConfiguration(nlogConfigPath);

//                return LogManager.GetCurrentClassLogger();
//            }
//        }

//        private static NLogger _instance;

//        public static NLogger Instance
//        {
//            get
//            {
//                if (ReferenceEquals(_instance, null))
//                {
//                    _instance = new NLogger();
//                }
//                return _instance;
//            }
//        }

//        #region Debug，调试
//        public void Debug(string msg)
//        {
//            _logger.Debug(msg);
//        }

//        public void Debug(string msg, Exception err)
//        {
//            _logger.Debug(err, msg);
//        }
//        #endregion

//        #region Info，信息
//        public void Info(string msg)
//        {
//            _logger.Info(msg);
//        }

//        public void Info(string msg, Exception err)
//        {
//            _logger.Info(err, msg);
//        }
//        #endregion

//        #region Warn，警告
//        public void Warn(string msg)
//        {
//            _logger.Warn(msg);
//        }

//        public void Warn(string msg, Exception err)
//        {
//            _logger.Warn(err, msg);
//        }
//        #endregion

//        #region Trace，追踪
//        public void Trace(string msg)
//        {
//            _logger.Trace(msg);
//        }

//        public void Trace(string msg, Exception err)
//        {
//            _logger.Trace(err, msg);
//        }
//        #endregion

//        #region Error，错误
//        public void Error(string msg)
//        {
//            _logger.Error(msg);
//        }

//        public void Error(string msg, Exception err)
//        {
//            _logger.Error(err, msg);
//        }
//        #endregion

//        #region Fatal,致命错误
//        public void Fatal(string msg)
//        {
//            _logger.Fatal(msg);
//        }

//        public void Fatal(string msg, Exception err)
//        {
//            _logger.Fatal(err, msg);
//        }
//        #endregion
//    }
//}
