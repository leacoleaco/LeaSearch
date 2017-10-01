using System.Collections.Generic;

namespace LeaSearch.Plugin.Index
{
    /// <summary>
    /// 插件返回的索引信息
    /// </summary>
    public class IndexInfo
    {
        private IList<DataItem> _items;

        public IndexInfo(string pluginId)
        {
            PluginId = pluginId;
        }

        public string PluginId { get; }

        /// <summary>
        /// 实时从内存中收集到索引中
        /// </summary>
        public IEnumerable<DataItem> Items => _items;

        private void CheckItems()
        {
            if (Items == null)
            {
                _items = new List<DataItem>();
            }
        }

        public void AddItem(DataItem dataItem)
        {
            CheckItems();
            _items.Add(dataItem);
        }

        public void RemoveItem(DataItem dataItem)
        {
            CheckItems();
            _items.Remove(dataItem);
        }

        public void InsertItem(int index, DataItem dataItem)
        {
            CheckItems();
            _items.Insert(index, dataItem);
        }
    }
}