using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace LeaSearch.Plugin
{
    public class QueryListResult : QueryResult
    {
       

        public List<ResultItem> Results { get; } = new List<ResultItem>();


        /// <summary>
        /// 公共的选中后执行的方法
        /// </summary>
        public Func<SharedContext, ResultItem, StateAfterCommandInvoke> SelectedAction { get; set; }

       

        public void AddResultItem(ResultItem item)
        {
            Results.Add(item);
        }
    }
}