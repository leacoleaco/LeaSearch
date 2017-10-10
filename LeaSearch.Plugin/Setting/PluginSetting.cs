using System;
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
        /// 2 每次启动都需要更新
        /// 1 本次启动需要更新,更新后变为0，并按照时间计划更新
        /// 0 本次启动不更新，按照时间计划更新
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

        /// <summary>
        /// 上一次更新的时间
        /// </summary>
        public DateTime LastIndexTime { get; set; }
    }
}
