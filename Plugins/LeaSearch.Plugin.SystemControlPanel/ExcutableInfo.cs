using System.Diagnostics;

namespace LeaSearch.Plugin.SystemControlPanel
{
    public class ExcutableInfo
    {
        public string FileName { get; set; }
        public string Arguments { get; set; }
        public bool HideWindow { get; set; }


        public ProcessStartInfo GetProcessStartInfo()
        {
            var ret = new ProcessStartInfo();
            ret.FileName = FileName;
            ret.Arguments = Arguments;
            if (HideWindow) ret.WindowStyle = ProcessWindowStyle.Hidden;
            return ret;
        }
    }
}