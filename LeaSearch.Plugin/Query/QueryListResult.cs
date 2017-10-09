using System;
using System.Collections.Generic;
using LeaSearch.Plugin.DetailInfos;

namespace LeaSearch.Plugin.Query
{
    public class QueryListResult : QueryResult
    {

        /// <summary>
        /// 查询列表结果
        /// </summary>
        public List<ResultItem> Results { get; } = new List<ResultItem>();

        /// <summary>
        /// 列表子项的更多信息
        /// </summary>
        public IInfo MoreInfo;

        /// <summary>
        /// 默认的点击选中结果或者点击回车的事件
        /// </summary>
        public Func<ResultItem, StateAfterCommandInvoke> SelectAction { get; set; }

        public void AddResultItem(ResultItem item)
        {
            Results.Add(item);
        }
    }
}