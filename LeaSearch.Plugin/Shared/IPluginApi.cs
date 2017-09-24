using System.Collections;
using LeaSearch.Plugin.Query;

namespace LeaSearch.Plugin
{
    public interface IPluginApi
    {
        /// <summary>
        /// 插件的根目录
        /// </summary>
        string PluginRootPath { get; }

        /// <summary>
        /// 添加数据到全局索引文件中
        /// </summary>
        /// <param name="dataItems"></param>
        void AddDataItemToIndex(DataItem[] dataItems);

        /// <summary>
        /// 更新数据到全局索引文件中
        /// 更新的时候，会自动以 pluginId-name 作为目标的键值
        /// </summary>
        /// <param name="dataItems"></param>
        void UpdateDataItemToIndex(DataItem[] dataItems);

        /// <summary>
        /// 检索name关键词
        /// </summary>
        /// <param name="keyword"></param>
        /// <param name="top"></param>
        /// <returns></returns>
        DataItem[] SearchDataItems(string keyword, int top = 10);

        /// <summary>
        /// 取出所有的插件索引数据
        /// </summary>
        /// <returns></returns>
        DataItem[] GetAllDataItems();

        /// <summary>
        ///移除插件的索引 
        /// </summary>
        void RemoveIndex();

    }
}