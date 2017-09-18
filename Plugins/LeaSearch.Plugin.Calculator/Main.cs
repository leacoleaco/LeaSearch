using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using LeaSearch.Plugin.DetailInfos;
using LeaSearch.Plugin.Query;
using org.mariuszgromada.math.mxparser;

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


        private FlowDocument _HelpDocument;

        private static readonly Regex RegBrackets = new Regex(@"[\(\)\[\]]", RegexOptions.Compiled);

        private SharedContext _sharedContext;

        public void InitPlugin(SharedContext sharedContext)
        {
            _sharedContext = sharedContext;

            var executingAssembly = Assembly.GetExecutingAssembly();
            var helpInfoStream = executingAssembly?.GetManifestResourceStream("LeaSearch.Plugin.Calculator.Resources.HelpInfo.xaml");
            if (helpInfoStream != null) _HelpDocument = XamlReader.Load(helpInfoStream) as FlowDocument;

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
                InfoMessage = _sharedContext.SharedMethod.GetTranslation("leasearch_plugin_calculator_pluginCallActive"),
            };
        }


        public QueryListResult Query(QueryParam queryParam)
        {
            var expression = new Expression(queryParam.Keyword);

            if (!expression.checkSyntax())
            {
                //如果表达式有误                
                var translation = _sharedContext.SharedMethod.GetTranslation(@"leasearch_plugin_calculator_express_err");
                return new QueryListResult()
                {
                    ErrorMessage = translation
                };
            }

            var result = expression.calculate().ToString();
            return new QueryListResult()
            {
                Results = { new ResultItem()
            {
                IconPath = "calculator.png",
                Title = result,
                SubTitle = _sharedContext.SharedMethod.GetTranslation(@"leasearch_plugin_calculator_copy"),
                SelectedAction =(SharedContext) =>
                {
                    SharedContext.SharedMethod.CopyToClipboard(result);
                    return new StateAfterCommandInvoke(){ShowProgram = false};
                }
            }},
                MoreInfo = new TextInfo() { Text = "test" }
            };
        }

        public QueryDetailResult QueryDetail(ResultItem currentItem)
        {
            throw new System.NotImplementedException();
        }

        public HelpInfo GetHelpInfo(QueryParam queryParam)
        {
            return new HelpInfo()
            {
                Info = new FlowDocumentInfo() { Document = _HelpDocument }
            };
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
