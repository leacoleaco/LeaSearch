using System;
using System.IO;

namespace LeaSearch.Core.Plugin
{
    /// <summary>
    /// this is the all things for a plugin
    /// </summary>
    public class Plugin : PluginBaseInfo
    {

        public Plugin(PluginBaseInfo pluginBaseInfo, LeaSearch.Plugin.Plugin pluginInstance, string pluginId, PluginType pluginType) : base(pluginBaseInfo.PluginRootPath, pluginBaseInfo.PluginMetadata, pluginBaseInfo.PluginSettings)
        {
            PluginInstance = pluginInstance;
            PluginId = pluginId;
            PluginType = pluginType;
        }

        /// <summary>
        /// plugin 's instance
        /// </summary>
        public LeaSearch.Plugin.Plugin PluginInstance { get; }

        /// <summary>
        /// plugin's id
        /// </summary>
        public String PluginId { get; }

        public PluginType PluginType { get; }

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
            if (PluginMetadata != null && string.IsNullOrEmpty(PluginMetadata.Name))
            {
                return PluginMetadata.Name;
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
