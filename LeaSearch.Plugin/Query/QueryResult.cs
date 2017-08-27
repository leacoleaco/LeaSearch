using LeaSearch.Plugin.DetailInfos;

namespace LeaSearch.Plugin.Query
{
    /// <summary>
    /// return the query result
    /// </summary>
    public class QueryResult
    {
        /// <summary>
        /// 传入的查询信息
        /// </summary>
        public QueryParam OriginQueryParam { get; set; }

        /// <summary>
        /// info 区域需要展示的信息类型
        /// </summary>
        public IInfo MoreInfo;

    }
}
