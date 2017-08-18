using System;

namespace LeaSearch.Plugin.HelloWorld
{
    public class Main : IPlugin
    {
        public void InitPlugin(SharedContext sharedContext)
        {
        }

        public QueryListResult Query(QueryParam queryParam)
        {
            var result = new QueryListResult()
            {
                Results = { new ResultItem() { IconPath = "app.png", Title = "Query Sample For C#", SubTitle = $"Query:{queryParam.Keyword}" } }
            };
            return result;
        }

        public QueryDetailResult QueryDetail(QueryParam queryParam)
        {
            throw new NotImplementedException();
        }


    }
}
