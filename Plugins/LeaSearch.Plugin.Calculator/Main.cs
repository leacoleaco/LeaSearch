﻿using System.Text.RegularExpressions;
using System.Windows.Documents;
using System.Windows.Markup;
using LeaSearch.Plugin.DetailInfos;
using LeaSearch.Plugin.Query;
using org.mariuszgromada.math.mxparser;

namespace LeaSearch.Plugin.Calculator
{
    public class Main : Plugin
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


        private FlowDocument _helpDocument;

        private static readonly Regex RegBrackets = new Regex(@"[\(\)\[\]]", RegexOptions.Compiled);


        public override void InitPlugin(SharedContext sharedContext, IPluginApi pluginApi)
        {
            base.InitPlugin(sharedContext, pluginApi);

            pluginApi.SetIconFromEmbedResource("calculator.png");

            //var executingAssembly = Assembly.GetExecutingAssembly();
            //var helpInfoStream = executingAssembly?.GetManifestResourceStream("LeaSearch.Plugin.Calculator.Resources.HelpInfo.xaml");            //var executingAssembly = Assembly.GetExecutingAssembly();
            var helpInfoStream = pluginApi.GetPluginEmbedResouceStream("Resources.HelpInfo.xaml");
            if (helpInfoStream != null) _helpDocument = XamlReader.Load(helpInfoStream) as FlowDocument;

        }

        public override bool SuitableForSuggectionQuery(QueryParam queryParam)
        {
            if (queryParam.Keyword.Length <= 2 // don't affect when user only input "e" or "i" keyword
                || !regValidExpressChar.IsMatch(queryParam.Keyword)
                || !IsBracketComplete(queryParam.Keyword))
            {
                return false;
            }
            return true;

        }

        public override PluginCalledArg PluginCallActive(QueryParam queryParam)
        {
            return new PluginCalledArg()
            {
                ResultMode = ResultMode.Detail,
                InfoMessage = PluginApi.GetTranslation("PluginCallActive"),
            };
        }


        public override QueryListResult QueryList(QueryParam queryParam)
        {
            var expression = new Expression(queryParam.Keyword);

            if (!expression.checkSyntax())
            {
                //如果表达式有误                
                var translation = PluginApi.GetTranslation(@"ExpressErr");
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
                    IconSource = PluginApi.GetPluginEmbedImage("calculator.png"),
                    Title = result,
                    SubTitle = PluginApi.GetTranslation(@"Copy"),
                    SelectedAction =(SharedContext) =>
                    {
                        SharedContext.SharedMethod.CopyToClipboard(result);
                        return new StateAfterCommandInvoke(){ShowProgram = false};
                    }
                }},
            };
        }

        public override QueryDetailResult QueryDetail(QueryParam queryParam)
        {
            var expression = new Expression(queryParam.Keyword);

            if (!expression.checkSyntax())
            {
                //如果表达式有误                
                var translation = PluginApi.GetTranslation(@"ExpressErr");
                return new QueryDetailResult()
                {
                    ErrorMessage = translation
                };
            }

            var result = expression.calculate().ToString();
            return new QueryDetailResult()
            {
                Result = new TextInfo()
                {
                    Text = $"{result}",
                },
                EnterAction = (ctx, item) =>
                {
                    SharedContext.SharedMethod.CopyToClipboard(result);
                    return new StateAfterCommandInvoke() { ShowProgram = false };
                },

            };
        }


        public override HelpInfo GetHelpInfo(QueryParam queryParam)
        {
            return new HelpInfo()
            {
                Info = new FlowDocumentInfo() { Document = _helpDocument }
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
