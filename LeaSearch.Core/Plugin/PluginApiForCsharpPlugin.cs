using System.IO;
using System.Windows.Media;
using LeaSearch.Core.Notice;
using LeaSearch.Plugin;
using LeaSearch.Plugin.Index;
using LeaSearch.SearchEngine;

namespace LeaSearch.Core.Plugin
{
    internal class PluginApiForCsharpPlugin : IPluginApi
    {
        private readonly Plugin _plugin;
        private readonly LuceneManager _luceneManager;

        public PluginApiForCsharpPlugin(Plugin plugin, LuceneManager luceneManager)
        {
            _plugin = plugin;
            _luceneManager = luceneManager;

            if (plugin != null)
            {
                PluginRootPath = plugin.PluginRootPath;
            }
        }

        public string PluginRootPath { get; }


        public Stream GetPluginEmbedResouceStream(string name)
        {
            return _plugin.GetPluginEmbedResouceStream(name);
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

        public DataItem[] SearchDataItems(string keyword, int top = 10)
        {
            return _luceneManager.Search(keyword, _plugin.PluginId, top);
        }

        public DataItem[] GetAllDataItems()
        {
            return _luceneManager.SearchByPluginId(_plugin.PluginId);
        }

        public void RemoveIndex()
        {
            _luceneManager.DeleteIndexByPluginId(_plugin.PluginId);
        }

        public string GetTranslation(string key, params object[] paramObjects)
        {
            var translation = _plugin.GetTranslation(key);
            return string.Format(translation, paramObjects);
        }

        public void ShowMessageWithTranslation(string key, params object[] paramObjects)
        {
            var trans = GetTranslation(key, paramObjects);
            UiNoticeHelper.ShowMessage(trans);
        }

    }
}