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
        /// <summary>
        /// 启用建议功能
        /// </summary>
        public bool EnableSuggection { get; set; }
        /// <summary>
        /// 启用系统索引功能
        /// </summary>
        public bool EnableIndex { get; set; }
        /// <summary>
        /// 每次启动的时候都执行索引
        /// </summary>
        public bool DoIndexWhenEveryStart { get; set; }
        public string Author { get; set; }
        public string Version { get; set; }
        public string Language { get; set; }
        public string Description { get; set; }
        public string Website { get; set; }
        /// <summary>
        /// 插件入口文件
        /// </summary>
        public string EntryFileName { get; set; }
        public string IcoPath { get; set; }

        public override string ToString()
        {
            return Name;
        }

    }


}