using LeaSearch.Plugin.Index;
using LeaSearch.Plugin.Query;

namespace LeaSearch.Plugin
{

    /// <summary>
    /// 插件基类，
    /// 本类只是用于在编写插件的时候继承，方便重写方法
    /// </summary>
    public abstract class Plugin : IPlugin
    {
        protected SharedContext SharedContext;
        protected IPluginApi PluginApi;


        /// <summary>
        /// initilze the plugin
        /// 初始化插件
        /// </summary>
        /// <param name="sharedContext">全局唯一的上下文</param>
        /// <param name="pluginApi">每个plugin单独共享的操作用的API</param>
        public virtual void InitPlugin(SharedContext sharedContext, IPluginApi pluginApi)
        {
            SharedContext = sharedContext;
            PluginApi = pluginApi;
        }

        /// <summary>
        /// 初始化插件的索引
        /// 只有在主程序单独调用插件的 ReloadIndex 方法时才会激活本方法
        /// 为了提高程序加载速度，本方法只会在触发了某个条件的情况下才会自动执行，例如第一次加载插件等
        /// </summary>
        /// <param name="indexInfo"></param>
        public virtual IndexInfo InitIndex(IndexInfo indexInfo)
        {
            return null;
        }

        /// <summary>
        /// if plugin is in suggection mode, and plugin setup "ParticipateSuggection=true",
        ///  then this method will be called when prepare query
        /// 
        /// 如果插件设置了“EnableSuggection=True”,那么当系统准备查询插件过程的时候就会调用这个方法
        /// 这个方法会在“建议模式”唤醒，
        /// 返回true，说明插件在本次查询中可以提供建议
        /// </summary>
        /// <param name="queryParam">本次查询的请求</param>
        /// <returns> 返回true，说明插件在本次查询中可以提供建议</returns>
        public virtual bool SuitableForSuggectionQuery(QueryParam queryParam)
        {
            return false;
        }

        ///  <summary>
        ///  this method will call when in "direct call plugin mode",
        ///  such as "calculator" plugin, if we tap "space" key after input "calculator",this method will be call first time
        ///  and we continue input word will call “query” method
        /// 
        ///  这个方法会在“呼唤插件模式”唤醒
        ///  举个例子，当我们使用“计算器”插件的时候，如果我们输入了“calculator”,之后再输入了一个“空格”，那么这个方法会立刻唤醒
        ///  然后输入数据后才会调用 query 方法
        ///  </summary>
        /// <param name="queryParam"></param>
        /// <returns></returns>
        public virtual PluginCalledArg PluginCallActive(QueryParam queryParam)
        {
            return null;
        }

        /// <summary>
        /// query result for list 
        /// this method only call if query is not null
        ///
        /// 当开始查询数据列表的时候会唤醒本方法。
        /// 本方法仅当程序认为有查询字符时候才会唤醒
        /// </summary>
        /// <param name="queryParam"></param>
        /// <returns></returns>
        public virtual QueryListResult Query(QueryParam queryParam)
        {
            return null;
        }

        /// <summary>
        /// query a detail info when input space in select item
        /// 当在某个列表项目上，点击空格后展示的更多信息
        /// </summary>
        /// <param name="currentItem">需要预览的节点，可以用里面的 queryparam 属性获取之前的查询讯息</param>
        /// <returns></returns>
        public virtual QueryDetailResult QueryDetail(ResultItem currentItem)
        {
            return null;
        }


        /// <summary>
        /// 发起帮助请求信息时激活
        /// </summary>
        /// <returns></returns>
        public virtual HelpInfo GetHelpInfo(QueryParam queryParam)
        {
            return null;
        }

    }
}