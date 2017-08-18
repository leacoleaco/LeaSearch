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
                Results =
                {
                    new ResultItem() { IconPath = "app.png", Title = "Query Sample For C#  row1", SubTitle = $"Query:{queryParam.Keyword}" },
                    new ResultItem() { IconPath = "app.png", Title = "Query Sample For C#  row2", SubTitle = $"Query:{queryParam.Keyword}" },
                    new ResultItem() { IconPath = "app.png", Title = "Query Sample For C#  row3", SubTitle = $"Query:{queryParam.Keyword}" },
                    new ResultItem() { IconPath = "app.png", Title = "Query Sample For C#  row4", SubTitle = $"Query:{queryParam.Keyword}" },
                    new ResultItem() { IconPath = "app.png", Title = "Query Sample For C#  row5", SubTitle = $"Query:{queryParam.Keyword}" },
                    new ResultItem() { IconPath = "app.png", Title = "Query Sample For C#  row6", SubTitle = $"Query:{queryParam.Keyword}" },
                    new ResultItem() { IconPath = "app.png", Title = "Query Sample For C#  row7", SubTitle = $"Query:{queryParam.Keyword}" },
                    new ResultItem() { IconPath = "app.png", Title = "Query Sample For C#  row8", SubTitle = $"Query:{queryParam.Keyword}" },
                    new ResultItem() { IconPath = "app.png", Title = "Query Sample For C#  row9", SubTitle = $"Query:{queryParam.Keyword}" },
                    new ResultItem() { IconPath = "app.png", Title = "Query Sample For C#  row10", SubTitle = $"Query:{queryParam.Keyword}" },
                    new ResultItem() { IconPath = "app.png", Title = "Query Sample For C#  row11", SubTitle = $"Query:{queryParam.Keyword}" },
                    new ResultItem() { IconPath = "app.png", Title = "Query Sample For C#  row12", SubTitle = $"Query:{queryParam.Keyword}" },
                    new ResultItem() { IconPath = "app.png", Title = "Query Sample For C#  row13", SubTitle = $"Query:{queryParam.Keyword}" },
                    new ResultItem() { IconPath = "app.png", Title = "Query Sample For C#  row14", SubTitle = $"Query:{queryParam.Keyword}" },
                    new ResultItem() { IconPath = "app.png", Title = "Query Sample For C#  row15", SubTitle = $"Query:{queryParam.Keyword}" },
                    new ResultItem() { IconPath = "app.png", Title = "Query Sample For C#  row16", SubTitle = $"Query:{queryParam.Keyword}" },
                    new ResultItem() { IconPath = "app.png", Title = "Query Sample For C#  row17", SubTitle = $"Query:{queryParam.Keyword}" },
                    new ResultItem() { IconPath = "app.png", Title = "Query Sample For C#  row18", SubTitle = $"Query:{queryParam.Keyword}" },
                    new ResultItem() { IconPath = "app.png", Title = "Query Sample For C#  row19", SubTitle = $"Query:{queryParam.Keyword}" },
                }
            };
            return result;
        }

        public QueryDetailResult QueryDetail(QueryParam queryParam)
        {
            throw new NotImplementedException();
        }


    }
}
