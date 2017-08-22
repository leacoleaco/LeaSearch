using Newtonsoft.Json;

namespace LeaSearch.Core.Plugin
{
    /// <summary>
    /// container the plugin's info
    /// </summary>
    [JsonObject(MemberSerialization.OptOut)]
    public class PluginMetadata
    {
        public string PluginId { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string DefalutPrefixKeyword { get; set; }
        public bool ParticipateSuggection { get; set; }
        public string Author { get; set; }
        public string Version { get; set; }
        public string Language { get; set; }
        public string Description { get; set; }
        public string Website { get; set; }
        public string EntryFileName { get; set; }
        public string IcoPath { get; set; }

        public override string ToString()
        {
            return Name;
        }

    }
}