using System;
using System.Diagnostics;
using System.Text.RegularExpressions;
using LeaSearch.Plugin.Query;

namespace LeaSearch.Plugin.OpenUrl
{
    public class Main : Plugin
    {
        //based on https://gist.github.com/dperini/729294
        private const string urlPattern = "^" +
            // protocol identifier
            "(?:(?:https?|ftp)://|)" +
            // user:pass authentication
            "(?:\\S+(?::\\S*)?@)?" +
            "(?:" +
            // IP address exclusion
            // private & local networks
            "(?!(?:10|127)(?:\\.\\d{1,3}){3})" +
            "(?!(?:169\\.254|192\\.168)(?:\\.\\d{1,3}){2})" +
            "(?!172\\.(?:1[6-9]|2\\d|3[0-1])(?:\\.\\d{1,3}){2})" +
            // IP address dotted notation octets
            // excludes loopback network 0.0.0.0
            // excludes reserved space >= 224.0.0.0
            // excludes network & broacast addresses
            // (first & last IP address of each class)
            "(?:[1-9]\\d?|1\\d\\d|2[01]\\d|22[0-3])" +
            "(?:\\.(?:1?\\d{1,2}|2[0-4]\\d|25[0-5])){2}" +
            "(?:\\.(?:[1-9]\\d?|1\\d\\d|2[0-4]\\d|25[0-4]))" +
            "|" +
            // host name
            "(?:(?:[a-z\\u00a1-\\uffff0-9]-*)*[a-z\\u00a1-\\uffff0-9]+)" +
            // domain name
            "(?:\\.(?:[a-z\\u00a1-\\uffff0-9]-*)*[a-z\\u00a1-\\uffff0-9]+)*" +
            // TLD identifier
            "(?:\\.(?:[a-z\\u00a1-\\uffff]{2,}))" +
            ")" +
            // port number
            "(?::\\d{2,5})?" +
            // resource path
            "(?:/\\S*)?" +
            "$";
        private readonly Regex _reg = new Regex(urlPattern, RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public override void InitPlugin(SharedContext sharedContext, IPluginApi pluginApi)
        {
            base.InitPlugin(sharedContext, pluginApi);

            pluginApi.SetIconFromEmbedResource("url.png");
        }

        public override bool SuitableForSuggectionQuery(QueryParam queryParam)
        {
            return IsUrl(queryParam.Keyword);
        }

        public override PluginCalledArg PluginCallActive(QueryParam queryParam)
        {
            return new PluginCalledArg()
            {
                InfoMessage = "请输入一个网址以用默认浏览器打开"
            };
        }

        public override QueryListResult QueryList(QueryParam queryParam)
        {
            var res = new QueryListResult();
            var keyword = queryParam.Keyword;
            res.AddResultItem(
                new ResultItem
                {
                    Title = keyword,
                    SubTitle = SharedContext.SharedMethod.GetTranslation("leasearch_plugin_openurl_open_url", keyword),
                    IconPath = "url.png",
                    SelectedAction = x =>
                    {
                        if (!keyword.ToLower().StartsWith("http"))
                        {
                            keyword = "http://" + keyword;
                        }
                        try
                        {
                            Process.Start(keyword);
                            return new StateAfterCommandInvoke();
                        }
                        catch (Exception)
                        {
                            x.SharedMethod.ShowMessageWithTranslation("leasearch_plugin_openurl_canot_open_url", keyword);
                            return new StateAfterCommandInvoke();
                        }
                    }
                }
            );
            return res;
        }

        public bool IsUrl(string raw)
        {
            raw = raw.ToLower();

            if (_reg.Match(raw).Value == raw) return true;

            if (raw == "localhost" || raw.StartsWith("localhost:") ||
                raw == "http://localhost" || raw.StartsWith("http://localhost:") ||
                raw == "https://localhost" || raw.StartsWith("https://localhost:")
                )
            {
                return true;
            }

            return false;
        }

    }
}
