﻿using LeaSearch.Plugin.Index;
using LeaSearch.Plugin.Query;

namespace LeaSearch.Plugin
{
    public interface IPlugin
    {
        /// <summary>
        /// initilze the plugin
        /// 初始化插件
        /// </summary>
        /// <param name="sharedContext">全局唯一的上下文</param>
        /// <param name="pluginApi">插件与系统共享的接口</param>
        void InitPlugin(SharedContext sharedContext, IPluginApi pluginApi);

        /// <summary>
        /// 初始化插件的索引
        /// 只有在主程序单独调用插件的 ReloadIndex 方法时才会激活本方法
        /// 为了提高程序加载速度，本方法只会在触发了某个条件的情况下才会自动执行，例如第一次加载插件等
        /// </summary>
        /// <param name="indexInfo"></param>
        IndexInfo InitIndex(IndexInfo indexInfo);


        /// <summary>
        /// if plugin is in suggection mode, and plugin setup "ParticipateSuggection=true",
        ///  then this method will be called when prepare query
        /// 
        /// 这个方法会在“建议模式”唤醒，
        /// 如果插件设置了“ParticipateSuggection=true”,那么当系统准备查询插件过程的时候就会调用这个方法
        /// </summary>
        /// <param name="queryParam"></param>
        /// <returns>返回true，为插件数据可以支持本次查询</returns>
        bool SuitableForSuggectionQuery(QueryParam queryParam);


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
        /// <returns>插件激活后，返回的支持调用等的参数</returns>
        PluginCalledArg PluginCallActive(QueryParam queryParam);


        /// <summary>
        /// query result for list 
        /// this method only call if query is not null
        ///
        /// 当开始查询数据列表的时候会唤醒本方法。
        /// 本方法仅当程序认为有查询字符时候才会唤醒
        /// </summary>
        /// <param name="queryParam"></param>
        /// <returns>查询的结果</returns>
        QueryListResult QueryList(QueryParam queryParam);


        ///  <summary>
        /// 在执行详情查询的时候会调用本方法
        /// 如果在插件激活后，返回调用参数中为详情查询方式，才会执行本方法
        ///  插件激活后，使用详情查询模式
        ///  </summary>
        /// <param name="queryParam"></param>
        /// <returns></returns>
        QueryDetailResult QueryDetail(QueryParam queryParam);


        /// <summary>
        /// query a detail info when input space in select item
        /// 当在某个列表项目上，点击空格后展示的更多信息
        /// </summary>
        /// <param name="currentItem">需要预览的节点，可以用里面的 queryparam 属性获取之前的查询讯息</param>
        /// <returns>查询详情的结果</returns>
        QueryItemDetailResult QueryItemDetail(ResultItem currentItem);



        /// <summary>
        /// 发起帮助请求信息时激活
        /// </summary>
        /// <returns>帮助信息</returns>
        HelpInfo GetHelpInfo(QueryParam queryParam);

    }
}