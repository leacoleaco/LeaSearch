using System.Collections.Generic;
using LeaSearch.Common.Env;
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

        private TaskManager.TaskManager _taskManager;

        public PluginManager(SharedContext sharedContext, LuceneManager luceneManager, TaskManager.TaskManager taskManager)
        {
            _luceneManager = luceneManager;
            _taskManager = taskManager;
            _pluginsLoader = new PluginsLoader(sharedContext, luceneManager);


            //定时更新插件索引
            _taskManager.UpdateIndexTaskActive += BuildIndexForeachPlugin;
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
                if (initIndex != null)
                {
                    prepareIndexInfos.Add(initIndex);
                }
            }
            _luceneManager.CreateIndex(prepareIndexInfos.ToArray());
        }

        public List<Plugin> GetPlugins()
        {
            return _plugins;
        }


    }
}
