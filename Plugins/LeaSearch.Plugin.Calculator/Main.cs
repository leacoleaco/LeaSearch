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

        private const string _helpStr = "### home page\r\n" +
                                        "http://mathparser.org/\r\n" +
                                        "### Math Expressions Parser - features list with examples\r\n" +
                                        "| FUNCTIONALITY | EXAMPLE | SUPPORT LEVEL |\r\n" +
"| Simple calculator | i.e.: 2+3, n! | Full support |\r\n" +
"| Binary relations | i.e.: a* b | Full support |\r\n" +
"Boolean operators   i.e.: a&b Full support\r\n" +
"Built-in constants i.e.: 2+pi Full support\r\n" +
"User defined constants  i.e.: 3tau, where tau = 2pi Full support\r\n" +
"Built-in unary functions    i.e.: sin(2)    Extensive collection\r\n" +
"Built-in binary functions   i.e.: log(a, b)  Main functions\r\n" +
"Built-in n-arguments functions  i.e.: gcd(a, b, c, d,…)    Special functions\r\n" +
"Evaluating conditions   i.e.: if(a=true, then b, else c)	Full support\r\n" +
"Cases functions i.e.: iff(case1, then a1, case2, then a2, ...)  Full support\r\n" +
"User defined arguments i.e.: x = 5, cos(x) Full support\r\n" +
"User defined dependent arguments i.e.: x= 2, y= x ^ 2    Full support\r\n" +
"Iterated operators - SIGMA summation    i.e.: sum( 1, n, f(…, i)\r\n" +
"Iterated operators - PI product i.e.: prod( 1, n, f(…, i) { step} )	Full support\r\n" +
"Derivatives i.e.: der(sin(x), x) )	Full support\r\n" +
"Integrals i.e.: 2*int(sqrt(1-x^2), x, -1, 1)	Full support\r\n" +
"User defined functions i.e.: f(x, y) = sin(x + y) Full support\r\n" +
"Fast (limited) recursion    i.e.: fib(n) = fib(n - 1) + fib(n - 2), addBaseCase(0, 0), addBaseCase(1, 1)    Full support\r\n" +
"Recursion, any kind i.e.: Cnk(n, k) = if(k>0, if(k<n, Cnk(n-1,k-1)+Cnk(n-1, k), 1), 1)	Full supp\r\n" +
                                        "## Number format\r\n" +
"| Key word|Category|Description|Example|Since|\r\n" +
"|---|---|---|---|---|\r\n" +
"|Number|Decimal Number|Decimal number|1, 1.5, -2.3|1.0|\r\n" +
"|Number|Decimal Number|Decimal number - scientific notation|1.2e10, -2.4e-10, 2.3E+10|4.0|\r\n" +
"|Number|Binary Number|Binary number - number literal| b.10101, B.10101, b2.10010|4.1|\r\n" +
"|Number|Octal Number|Octal number - number literal| o.1027, O.1027, b8.1027|4.1|\r\n" +
"|Number|Hexadecimal Number|Hexadecimal number - number literal| h.12fE, H.12fE, b16.12fE|4.1|\r\n" +
"|Number|Unary Number|Unary number - number literal| b1.111 , B1.111|4.1|\r\n" +
"|Number|Base 1-36|Base 1-36 number - number literal| bN.xxxx , BN.xxxx|4.1|";

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
                MoreInfo = new FlowDocumentInfo() { Document = _HelpDocument }
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
