using System.Collections.Generic;
using LeaSearch.Common.Env;
using LeaSearch.Plugin;
using LeaSearch.SearchEngine;

namespace LeaSearch.Core.Plugin
{

    /// <summary>
    /// manage the plugin
    /// install,uninstall,update,load etc.
    /// </summary>
    public class PluginManager
    {
        private readonly PluginsLoader _pluginsLoader;

        private List<Plugin> _plugins;

        public PluginManager(SharedContext sharedContext, LuceneManager luceneManager)
        {
            _pluginsLoader = new PluginsLoader(sharedContext, luceneManager);
        }

        /// <summary>
        /// load plugins
        /// </summary>
        public void LoadPlugins()
        {
            _plugins = _pluginsLoader.LoadPlugins(Constant.PluginsDirectory);
        }


        public List<Plugin> GetPlugins()
        {
            return _plugins;
        }


    }
}
