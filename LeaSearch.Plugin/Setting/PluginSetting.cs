using System.Collections.Generic;

namespace LeaSearch.Plugin.Setting
{
    public class PluginSettings
    {

        /// <summary>
        /// s是否禁用
        /// </summary>
        public bool Disabled { get; set; }

        /// <summary>
        /// 是否需要更新索引
        /// 1 需要更新
        /// 0 无需更新
        /// -1 永不更新
        /// </summary>
        public int NeedUpdateIndex { get; set; }

        /// <summary>
        /// 重新设置的 前缀激活词
        /// </summary>
        public string PrefixKeyword { get; set; }

        /// <summary>
        /// 自定义的设置项目
        /// </summary>
        public Dictionary<string, string> Option { get; set; } = new Dictionary<string, string>();
    }
}
