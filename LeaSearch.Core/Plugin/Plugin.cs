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
    public class Plugin : PluginBase
    {

        public Plugin(PluginBase pluginBase, IPlugin pluginInstance) : base(pluginBase.PluginRootPath, pluginBase.PluginMetadata, pluginBase.PluginSettings)
        {
            this.PluginInstance = pluginInstance;
        }

        /// <summary>
        /// plugin 's instance
        /// </summary>
        public IPlugin PluginInstance { get; }

        /// <summary>
        /// if plugin is not correct load, or set to disabled, then it will disabled
        /// </summary>
        public bool IsDisabled => PluginMetadata == null || PluginSettings == null || PluginInstance == null || PluginSettings.Disabled;


    }

    public class PluginBase
    {

        public PluginBase(string pluginRootPath, PluginMetadata pluginMetadata, PluginSettings pluginSettings)
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


        public string PrefixKeyword
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(PluginSettings?.PrefixKeyword))
                {
                    return PluginSettings.PrefixKeyword;
                }

                if (!string.IsNullOrWhiteSpace(PluginMetadata?.DefalutPrefixKeyword))
                {
                    return PluginMetadata.DefalutPrefixKeyword;
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

        /// <summary>
        /// Icon Img
        /// </summary>
        public ImageSource PluginIconImage => Ioc.Ioc.Reslove<ImageManager>().GetImageSource(PluginIconPath);
    }
}
