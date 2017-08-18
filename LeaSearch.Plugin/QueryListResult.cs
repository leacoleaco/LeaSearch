using System.Collections.Generic;

namespace LeaSearch.Plugin
{
    public class QueryListResult : QueryResult
    {
        public List<ResultItem> Results { get; } = new List<ResultItem>();
        public void AddResultItem(ResultItem item)
        {
            Results.Add(item);
        }
    }
}