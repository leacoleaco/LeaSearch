using System;
using System.Windows;
using System.Windows.Threading;
using LeaSearch.Common.Env;
using NLog;

namespace LeaSearch.Infrastructure.ErrorReport
{
    public static class ErrorReporting
    {
        private static void Report(Exception e, string name)
        {
            var logger = LogManager.GetLogger(name);
            logger.Error($"program crash \r\n {name}\r\nreasom:\r\n{e.Message}");
            logger.Error(ExceptionFormatter.FormatExcpetion(e));
            //TODO: modify error report window

            MessageBox.Show("程序遇到一些问题而崩溃。\r\n原因：" + e.Message);

            Application.Current.Shutdown();
        }


        public static void UnhandledExceptionHandle(object sender, UnhandledExceptionEventArgs e)
        {
            //handle non-ui thread exceptions
            Report((Exception)e.ExceptionObject, "UnHandledException");
        }

        public static void DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            //handle ui thread exceptions
            Report(e.Exception, "DispatcherUnhandledException");
            //prevent application exist, so the user can copy prompted error info
            e.Handled = true;
        }

        public static string RuntimeInfo()
        {
            var info = $"\nWox version: {Constant.Version}" +
                       $"\nOS Version: {Environment.OSVersion.VersionString}" +
                       $"\nIntPtr Length: {IntPtr.Size}" +
                       $"\nx64: {Environment.Is64BitOperatingSystem}";
            return info;
        }

        public static string DependenciesInfo()
        {
            var info = $"\nPython Path: {Constant.PythonPath}" +
                       $"\nEverything SDK Path: {Constant.EverythingSDKPath}";
            return info;
        }
    }
}
