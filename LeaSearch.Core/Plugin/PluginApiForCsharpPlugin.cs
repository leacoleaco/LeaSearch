using System.IO;
using System.Reflection;
using System.Windows.Media;
using LeaSearch.Plugin;
using LeaSearch.Plugin.Index;
using LeaSearch.SearchEngine;

namespace LeaSearch.Core.Plugin
{
    internal class PluginApiForCsharpPlugin : IPluginApi
    {
        private readonly Plugin _plugin;
        private readonly Assembly _pluginAssembly;
        private readonly LuceneManager _luceneManager;

        public PluginApiForCsharpPlugin(Plugin plugin, LuceneManager luceneManager, Assembly pluginAssembly)
        {
            _plugin = plugin;
            _luceneManager = luceneManager;
            _pluginAssembly = pluginAssembly;

            if (plugin != null)
            {
                PluginRootPath = plugin.PluginRootPath;
            }
        }

        public string PluginRootPath { get; }

        public Stream GetPluginEmbedResouceStream(string name)
        {
            if (_pluginAssembly == null) return null;
            var pluginName = _pluginAssembly.GetName()?.Name;

            return _pluginAssembly.GetManifestResourceStream($"{pluginName}.{name}");
        }

        public ImageSource GetPluginEmbedImage(string name)
        {
            var stream = GetPluginEmbedResouceStream(name);
            if (stream == null)
            {
                return null;
            }
            ImageSourceConverter converter = new ImageSourceConverter();
            return (ImageSource)converter.ConvertFrom(stream);
        }

        public void SetIconFromEmbedResource(string imageName)
        {
            _plugin.PluginInitInfo.Icon = GetPluginEmbedImage(imageName);
        }

        public void AddDataItemToIndex(DataItem[] dataItems)
        {
            _luceneManager.AddToIndex(dataItems, _plugin.PluginId);
        }

        public void UpdateDataItemToIndex(DataItem[] dataItems)
        {
            _luceneManager.UpdateToIndex(dataItems, _plugin.PluginId);
        }

        public DataItem[] SearchDataItemInIndex(string keyword, int page = 1, int pageSize = 10)
        {
            throw new System.NotImplementedException();
        }


        public DataItem[] SearchDataItems(string keyword, int top = 10)
        {
            return _luceneManager.Search(keyword, top);
        }

        public DataItem[] GetAllDataItems()
        {
            return _luceneManager.SearchByPluginId(_plugin.PluginId);
        }

        public void RemoveIndex()
        {
            _luceneManager.DeleteIndexByPluginId(_plugin.PluginId);
        }
    }
}