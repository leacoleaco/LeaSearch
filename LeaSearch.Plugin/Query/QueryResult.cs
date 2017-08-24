using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;

namespace LeaSearch.Plugin
{
    /// <summary>
    /// return the query result
    /// </summary>
    public class QueryResult
    {

        public QueryParam OriginQueryParam { get; set; }

        /// <summary>
        /// info 区域需要展示的信息类型
        /// </summary>
        public IInfo MoreInfo;

    }
}
