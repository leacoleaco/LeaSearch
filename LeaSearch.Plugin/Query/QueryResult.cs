using System.Windows.Documents;

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
        public QueryParam QueryParam { get; set; }

        /// <summary>
        /// 得到此结果的插件
        /// </summary>
        public string PluginId { get; set; }


        /// <summary>
        ///查询模式 
        /// </summary>
        public QueryMode QueryMode { get; set; }

        /// <summary>
        /// 主界面的错误提示
        /// </summary>
        public string ErrorMessage { get; set; }

        /// <summary>
        /// 当前的帮助信息
        /// </summary>
        public FlowDocument HelpInfo { get; set; }



    }
}
