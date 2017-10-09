using System;
using LeaSearch.Plugin.DetailInfos;

namespace LeaSearch.Plugin.Query
{
    public class QueryDetailResult : QueryResult
    {
        /// <summary>
        /// 详情查询结果
        /// </summary>
        public IInfo Result { get; set; }

        /// <summary>
        /// 默认的点击选中结果或者点击回车的事件
        /// </summary>
        public Func<SharedContext, IInfo, StateAfterCommandInvoke> EnterAction { get; set; }
    }
}