using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using LeaSearch.Common.Env;
using LeaSearch.Core.Image;
using LeaSearch.Plugin;

namespace LeaSearch.Core.Plugin
{
    /// <summary>
    /// this is the all things for a plugin
    /// </summary>
    public class Plugin : PluginBaseInfo
    {

        public Plugin(PluginBaseInfo pluginBaseInfo, LeaSearch.Plugin.Plugin pluginInstance, string pluginId) : base(pluginBaseInfo.PluginRootPath, pluginBaseInfo.PluginMetadata, pluginBaseInfo.PluginSettings)
        {
            this.PluginInstance = pluginInstance;
            this.PluginId = pluginId;
        }

        /// <summary>
        /// plugin 's instance
        /// </summary>
        public LeaSearch.Plugin.Plugin PluginInstance { get; }

        public String PluginId { get; }

        /// <summary>
        /// if plugin is not correct load, or set to disabled, then it will disabled
        /// </summary>
        public bool IsDisabled => PluginMetadata == null || PluginSettings == null || PluginInstance == null || PluginSettings.Disabled;

        /// <summary>
        /// the mode that we call for this plugin 
        /// </summary>
        public QueryMode QueryMode { get; set; }

        public override string ToString()
        {
            if (this.PluginMetadata != null && string.IsNullOrEmpty(this.PluginMetadata.Name))
            {
                return this.PluginMetadata.Name;
            }

            return base.ToString();
        }
    }

    public enum QueryMode
    {
        Suggection, PluginCall
    }

    public class PluginBaseInfo
    {

        public PluginBaseInfo(string pluginRootPath, PluginMetadata pluginMetadata, PluginSettings pluginSettings)
        {
            PluginRootPath = pluginRootPath;
            PluginMetadata = pluginMetadata;
            PluginSettings = pluginSettings;

        }

        /// <summary>
        ///  plugin's dir
        /// </summary>
        internal string PluginRootPath;


        /// <summary>
        /// Plugin ‘s info
        /// </summary>
        public PluginMetadata PluginMetadata { get; set; }



        /// <summary>
        /// plugin's custom setting
        /// </summary>
        public PluginSettings PluginSettings { get; set; }


        public string[] PrefixKeywords
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(PluginSettings?.PrefixKeyword))
                {
                    return PluginSettings.PrefixKeyword.Split('|');
                }

                if (!string.IsNullOrWhiteSpace(PluginMetadata?.DefalutPrefixKeyword))
                {
                    return PluginMetadata.DefalutPrefixKeyword.Split('|');
                }

                return null;
            }
        }


        /// <summary>
        /// plugin 's icon
        /// </summary>
        public string PluginIconPath => Path.Combine(PluginRootPath, PluginMetadata.IcoPath);

        /// <summary>
        /// plugin 's entry file
        /// </summary>
        public string PluginEntryPath => Path.Combine(PluginRootPath, PluginMetadata.EntryFileName);

    }
}
