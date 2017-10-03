using System.Diagnostics;
using Wox.Plugin.Program.Programs;

namespace LeaSearch.Plugin.Programs
{
    public class ExtItem
    {
        public UWP.Application Application { get; set; }
        public Win32 Win32 { get; set; }
        public void Run()
        {
            if (Win32 != null)
            {
                var info = new ProcessStartInfo
                {
                    FileName = Win32.FullPath,
                    WorkingDirectory = Win32.ParentDirectory,
                };
                Process.Start(info);
                return;
            }
            if (Application != null)
            {
                Application.Launch();
                return;
            }
        }
    }
}