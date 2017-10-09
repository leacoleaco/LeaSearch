using System;
using System.Collections.Generic;
using System.Windows;
using LeaSearch.Common.Env;
using LeaSearch.Infrastructure.Logger;
using LeaSearch.Plugin;
using LeaSearch.Plugin.Index;
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

        private LuceneManager _luceneManager;


        public PluginManager(SharedContext sharedContext, LuceneManager luceneManager)
        {
            _luceneManager = luceneManager;
            _pluginsLoader = new PluginsLoader(sharedContext, luceneManager);
        }

        /// <summary>
        /// load plugins
        /// </summary>
        public void LoadPlugins()
        {
            _plugins = _pluginsLoader.LoadPlugins(Constant.PluginsDirectory);
        }

        public void BuildIndexForeachPlugin()
        {
            List<IndexInfo> prepareIndexInfos = new List<IndexInfo>();
            foreach (var plugin in _plugins)
            {
                var initIndex = plugin.InvokeInitIndex(new IndexInfo(plugin.PluginId));
                prepareIndexInfos.Add(initIndex);
            }
            _luceneManager.CreateIndex(prepareIndexInfos.ToArray());
        }

        public List<Plugin> GetPlugins()
        {
            return _plugins;
        }


    }
}
