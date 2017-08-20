using System;
using RestSharp;

namespace LeaSearch.Plugin.Baidu
{
    public class Main:IPlugin
    {
        private SharedContext _sharedContext;

        public void InitPlugin(SharedContext sharedContext)
        {
            _sharedContext = sharedContext;
        }

        public bool SuitableForThisQuery(QueryParam queryParam)
        {
            return true;
        }

        public QueryListResult Query(QueryParam queryParam)
        {
            var res=new QueryListResult();
            //DoSearch(queryParam.Keyword);

            return res;
        }

        public QueryDetailResult QueryDetail(QueryParam queryParam)
        {
            throw new NotImplementedException();
        }

        private void DoSearch(string queryStr)
        {
            var client = new RestClient($"https://m.baidu.com/s?wd={queryStr}");
            // client.Authenticator = new HttpBasicAuthenticator(username, password);

            var request = new RestRequest("resource/{id}", Method.GET);
            // execute the request
            IRestResponse response = client.Execute(request);
            var content = response.Content; // raw content as string


            _sharedContext.SharedMethod.LogInfo(content);

        }
    }
}
