using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeaSearch.Plugin
{
    public class QueryParam
    {
        public string PrefixKeyword { get; set; }

        public string Keyword { get; set; }

        public string OriginQueryStr { get; set; }

        public QueryType QueryType { get; set; }
    }
}
