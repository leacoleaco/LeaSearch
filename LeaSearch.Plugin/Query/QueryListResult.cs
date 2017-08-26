using System;
using System.Collections.Generic;

namespace LeaSearch.Plugin.Query
{
    public class QueryListResult : QueryResult
    {
       

        public List<ResultItem> Results { get; } = new List<ResultItem>();


        /// <summary>
        /// 公共的选中后执行的方法
        /// </summary>
        public Func<SharedContext, ResultItem, StateAfterCommandInvoke> SelectedAction { get; set; }

        /// <summary>
        /// 主界面的错误提示
        /// </summary>
        public string ErrorMessage { get; set; }


        public void AddResultItem(ResultItem item)
        {
            Results.Add(item);
        }
    }
}