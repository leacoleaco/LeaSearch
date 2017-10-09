using LeaSearch.Common.Env;
using Microsoft.Win32;

namespace LeaSearch.Infrastructure.Helper
{
    public class AutoStartupHelper
    {
        private const string StartupPath = "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run";

        public static void SetStartup()
        {
            using (var key = Registry.CurrentUser.OpenSubKey(StartupPath, true))
            {
                key?.SetValue(Constant.LeaSearch, Constant.ExecutablePath);
            }
        }

        public static void RemoveStartup()
        {
            using (var key = Registry.CurrentUser.OpenSubKey(StartupPath, true))
            {
                key?.DeleteValue(Constant.LeaSearch, false);
            }
        }

        public static bool IsStartupSet()
        {
            using (var key = Registry.CurrentUser.OpenSubKey(StartupPath, true))
            {
                var path = key?.GetValue(Constant.LeaSearch) as string;
                if (path != null)
                {
                    return path == Constant.ExecutablePath;
                }
                else
                {
                    return false;
                }
            }
        }
    }
}
