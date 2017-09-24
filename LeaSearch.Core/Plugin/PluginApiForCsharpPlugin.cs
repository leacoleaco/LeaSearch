﻿using LeaSearch.Plugin;
using LeaSearch.Plugin.Query;
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

        public void AddDataItemToIndex(DataItem[] dataItems)
        {
            foreach (var dataItem in dataItems)
            {
                dataItem.PluginId = _plugin.PluginId;
            }
            _luceneManager.Add(dataItems);
        }

        public void UpdateDataItemToIndex(DataItem[] dataItems)
        {
            foreach (var dataItem in dataItems)
            {
                dataItem.PluginId = _plugin.PluginId;
            }
            _luceneManager.Update(dataItems);
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