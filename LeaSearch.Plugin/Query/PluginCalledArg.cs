using LeaSearch.Plugin.DetailInfos;

namespace LeaSearch.Plugin
{
    public class PluginCalledArg
    {
        /// <summary>
        /// 在激活插件后，展示的提示信息
        /// </summary>
        public string InfoMessage { get; set; }

        /// <summary>
        /// 在激活插件后，默认展示的更多信息
        /// </summary>
        public IInfo MoreInfo { get; set; }
    }

}
