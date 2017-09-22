using System;
using System.Diagnostics;
using HtmlAgilityPack;
using LeaSearch.Plugin.Query;

namespace LeaSearch.Plugin.Baidu
{
    public class Main : IPlugin
    {
        private SharedContext _sharedContext;

        public void InitPlugin(SharedContext sharedContext, PluginMetaData pluginMetaData)
        {
            _sharedContext = sharedContext;
        }

        public bool SuitableForSuggectionQuery(QueryParam queryParam)
        {
            return true;
        }

        public PluginCalledArg PluginCallActive(QueryParam queryParam)
        {
            return new PluginCalledArg()
            {
                InfoMessage = "使用百度搜索信息。"
            };
        }

        public QueryListResult Query(QueryParam queryParam)
        {
            var res = new QueryListResult();

            //查询手机百度
            HtmlWeb webClient = new HtmlWeb();
            HtmlDocument doc = webClient.Load($"https://m.baidu.com/s?wd={queryParam.Keyword}");
            HtmlNodeCollection resNodes = doc.DocumentNode.SelectNodes("//*[@id=\"results\"]/div/div");

            if (resNodes != null)
            {
                foreach (HtmlNode resNode in resNodes)
                {
                    var resultItem = new ResultItem()
                    {
                        Title = resNode?.ChildNodes[0]?.ChildNodes[0]?.InnerHtml,
                        //Title = resNode?.SelectSingleNode("//a/h3")?.InnerHtml,
                        SubTitle = resNode?.SelectSingleNode("//div/div/a/p")?.InnerHtml,
                        IconPath = "baidu.png",
                        SelectedAction = x =>
                        {
                            string url = resNode?.SelectSingleNode("//a")?.Attributes["href"]?.Value;
                            if (!string.IsNullOrWhiteSpace(url))
                            {
                                Process.Start(url);
                                return new StateAfterCommandInvoke();
                            }
                            else
                            {
                                return new StateAfterCommandInvoke();
                            }
                        }

                    };
                    res.AddResultItem(resultItem);

                    //HtmlAttribute att = href.Attributes["href"];
                    //_sharedContext.SharedMethod.LogInfo(att.Value);
                }

            }

            return res;
        }

        public QueryDetailResult QueryDetail(ResultItem currentItem)
        {
            throw new NotImplementedException();
        }

        public HelpInfo GetHelpInfo(QueryParam queryParam)
        {
            throw new NotImplementedException();
        }

        private void DoSearch(string queryStr)
        {
            //var client = new RestClient($"https://m.baidu.com/s?wd={queryStr}");
            //// client.Authenticator = new HttpBasicAuthenticator(username, password);

            //var request = new RestRequest("resource/{id}", Method.GET);
            //// execute the request
            //IRestResponse response = client.Execute(request);
            //var content = response.Content; // raw content as string
            //_sharedContext.SharedMethod.LogInfo(content);








        }
    }
}
