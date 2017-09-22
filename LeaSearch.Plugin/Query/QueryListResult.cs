using System;
using System.Collections.Generic;
using System.Windows.Documents;

namespace LeaSearch.Plugin.Query
{
    public class QueryListResult : QueryResult
    {


        /// <summary>
        /// 查询列表结果
        /// </summary>
        public List<ResultItem> Results { get; } = new List<ResultItem>();


        /// <summary>
        /// 公共的选中后执行的方法
        /// </summary>
        public Func<SharedContext, ResultItem, StateAfterCommandInvoke> SelectedAction { get; set; }


        /// <summary>
        ///查询模式 
        /// </summary>
        public QueryMode QueryMode { get; set; }

        /// <summary>
        /// 主界面的错误提示
        /// </summary>
        public string ErrorMessage { get; set; }

        public FlowDocument HelpInfo { get; set; }


        public void AddResultItem(ResultItem item)
        {
            Results.Add(item);
        }
    }
}