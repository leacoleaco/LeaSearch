using Newtonsoft.Json;

namespace LeaSearch.Core.Plugin
{
    public class PluginSettings
    {
        public bool Disabled { get; set; }
        [JsonIgnore]
        public long InitTime { get; set; }
        [JsonIgnore]
        public long AvgQueryTime { get; set; }
        [JsonIgnore]
        public int QueryCount { get; set; }

        public string PrefixKeyword { get; set; }
    }
}