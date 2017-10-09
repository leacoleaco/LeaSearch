using System.IO;
using System.Windows.Media;
using LeaSearch.Plugin.Index;

namespace LeaSearch.Plugin
{

    /// <summary>
    /// 提供给插件可以调用的API
    /// 本类与 <see cref="SharedContext"/> 的区别是，本类会为每个插件单独生成一个Api对象， 而SharedContext 是全局共享的
    /// 不同插件的api调用结果都可能会不同
    /// 所以，以插件特异的方法都会放置到这个类中
    /// </summary>
    public interface IPluginApi
    {

        /// <summary>
        /// 插件的根目录
        /// </summary>
        string PluginRootPath { get; }

        /// <summary>
        /// 取得嵌入插件的资源流
        /// 调用本方法获取资源文件的时候，记得把文件生成操作设置为“嵌入的资源”
        /// </summary>
        /// <param name="name">资源名称，相对插件跟路径,例如如在" Resource/Icon.png" 目录中，则填写 " Resource.Icon.png "
        /// 注意要把 “/” 变为 “.”
        /// </param>
        /// <returns></returns>
        Stream GetPluginEmbedResouceStream(string name);

        /// <summary>
        /// 取得嵌入的图片
        /// </summary>
        /// <param name="name">资源名称，相对插件跟路径,例如如在" Resource/Icon.png" 目录中，则填写 " Resource.Icon.png "
        /// 注意要把 “/” 变为 “.”
        /// </param>
        /// <returns></returns>
        ImageSource GetPluginEmbedImage(string name);


        /// <summary>
        /// 从嵌入资源中读取Icon并设置为icon
        /// </summary>
        /// <param name="imageName">资源名称，相对插件跟路径,例如如在" Resource/Icon.png" 目录中，则填写 " Resource.Icon.png "
        /// 注意要把 “/” 变为 “.”
        /// </param>
        /// <returns></returns>
        void SetIconFromEmbedResource(string imageName);

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