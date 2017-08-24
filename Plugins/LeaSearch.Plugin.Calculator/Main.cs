using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows.Documents;

namespace LeaSearch.Plugin.Calculator
{
    public class Main : IPlugin
    {

        private static Regex regValidExpressChar = new Regex(
            @"^(" +
            @"ceil|floor|exp|pi|e|max|min|det|abs|log|ln|sqrt|" +
            @"sin|cos|tan|arcsin|arccos|arctan|" +
            @"eigval|eigvec|eig|sum|polar|plot|round|sort|real|zeta|" +
            @"bin2dec|hex2dec|oct2dec|" +
            @"==|~=|&&|\|\||" +
            @"[ei]|[0-9]|[\+\-\*\/\^\., ""]|[\(\)\|\!\[\]]" +
            @")+$", RegexOptions.Compiled);

        private static readonly Regex RegBrackets = new Regex(@"[\(\)\[\]]", RegexOptions.Compiled);

        private SharedContext _sharedContext;

        public void InitPlugin(SharedContext sharedContext)
        {
            _sharedContext = sharedContext;
        }

        public bool SuitableForSuggectionQuery(QueryParam queryParam)
        {
            if (queryParam.Keyword.Length <= 2 // don't affect when user only input "e" or "i" keyword
                || !regValidExpressChar.IsMatch(queryParam.Keyword)
                || !IsBracketComplete(queryParam.Keyword))
            {
                return false;
            }
            return true;

        }

        public PluginCalledArg PluginCallActive(QueryParam queryParam)
        {
            return new PluginCalledArg()
            {
                InfoMessage = _sharedContext.SharedMethod.GetTranslation("leasearch_plugin_calculator_pluginCallActive")
            };
        }


        public QueryListResult Query(QueryParam queryParam)
        {
            return new QueryListResult()
            {
                Results = { new ResultItem()
            {
                IconPath = "calculator.png",
                Title = $"{queryParam.Keyword}"
            }},
                DefaultInfo = new TextInfo() { Text = "test" }
            };
        }

        public QueryDetailResult QueryDetail(QueryParam queryParam)
        {
            throw new System.NotImplementedException();
        }



        private bool IsBracketComplete(string query)
        {
            var matchs = RegBrackets.Matches(query);
            var leftBracketCount = 0;
            foreach (Match match in matchs)
            {
                if (match.Value == "(" || match.Value == "[")
                {
                    leftBracketCount++;
                }
                else
                {
                    leftBracketCount--;
                }
            }

            return leftBracketCount == 0;

        }
    }
}
