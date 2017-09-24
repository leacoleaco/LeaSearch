using LeaSearch.Plugin;

namespace LeaSearch.Core.Plugin
{
    internal class PluginApi : IPluginApi
    {
        private PluginBaseInfo pluginBaseInfo;

        public PluginApi()
        {
        }

        public PluginApi(PluginBaseInfo pluginBaseInfo)
        {
            this.pluginBaseInfo = pluginBaseInfo;


            PluginRootPath = pluginBaseInfo.PluginRootPath;
        }

        public string PluginRootPath { get; }
    }
}