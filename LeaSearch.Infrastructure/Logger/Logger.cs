using System.IO;
using System.Runtime.CompilerServices;
using LeaSearch.Common.Env;
using NLog;
using NLog.Config;
using NLog.Targets;

namespace LeaSearch.Infrastructure.Logger
{
    public static class Logger
    {
        /// <summary>
        /// 存放日志的文件夹
        /// </summary>
        public const string DirectoryName = "Logs";

        static Logger()
        {
            var path = Path.Combine(Constant.DataDirectory, DirectoryName, Constant.Version);
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            var configuration = new LoggingConfiguration();
            var target = new FileTarget();
            configuration.AddTarget("file", target);
            target.FileName = DirectoryName + "/" +
                              Constant.Version + "/${shortdate}.txt";
#if DEBUG
            var rule = new LoggingRule("*", LogLevel.Debug, target);
#else
            var rule = new LoggingRule("*", LogLevel.Info, target);
#endif
            configuration.LoggingRules.Add(rule);

            LogManager.Configuration = configuration;
        }


        public static void Error(string message)
        {
            var logger = LogManager.GetCurrentClassLogger();

            System.Diagnostics.Debug.WriteLine($"ERROR|{message}");
            logger.Error(message);
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public static void Exception(string message, System.Exception e)
        {
            var logger = LogManager.GetCurrentClassLogger();

            System.Diagnostics.Debug.WriteLine($"ERROR|{message}");

            logger.Error("-------------------------- Begin exception --------------------------");

            do
            {
                logger.Error($"Exception fulle name:\n <{e.GetType().FullName}>");
                logger.Error($"Exception message:\n <{e.Message}>");
                logger.Error($"Exception stack trace:\n <{e.StackTrace}>");
                logger.Error($"Exception source:\n <{e.Source}>");
                logger.Error($"Exception target site:\n <{e.TargetSite}>");
                logger.Error($"Exception HResult:\n <{e.HResult}>");
                e = e.InnerException;
            } while (e != null);

            logger.Error("-------------------------- End exception --------------------------");
        }

        public static void Debug(string message)
        {
            var logger = LogManager.GetCurrentClassLogger();

            System.Diagnostics.Debug.WriteLine($"DEBUG|{message}");
            logger.Debug(message);
        }

        public static void Info(string message)
        {
            var logger = LogManager.GetCurrentClassLogger();

            System.Diagnostics.Debug.WriteLine($"INFO|{message}");
            logger.Info(message);
        }

        public static void Warn(string message)
        {
            var logger = LogManager.GetCurrentClassLogger();

            System.Diagnostics.Debug.WriteLine($"WARN|{message}");
            logger.Warn(message);
        }
    }
}