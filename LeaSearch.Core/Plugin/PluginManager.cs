using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeaSearch.Common.Env;
using LeaSearch.Plugin;

namespace LeaSearch.Core.Plugin
{

    /// <summary>
    /// manage the plugin
    /// install,uninstall,update,load etc.
    /// </summary>
    public class PluginManager
    {
        private PluginsLoader _pluginsLoader;

        private SharedContext _sharedContext;

        private List<Plugin> _plugins;

        public PluginManager(SharedContext sharedContext)
        {
            this._sharedContext = sharedContext;
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
