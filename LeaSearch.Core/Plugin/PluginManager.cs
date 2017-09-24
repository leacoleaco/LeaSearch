using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        private SharedContext _sharedContext;

        private List<Plugin> _plugins;

        private LuceneManager _luceneManager;

        public PluginManager(SharedContext sharedContext, LuceneManager luceneManager)
        {
            this._sharedContext = sharedContext;
            _luceneManager = luceneManager;
            this._pluginsLoader = new PluginsLoader(sharedContext);
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
