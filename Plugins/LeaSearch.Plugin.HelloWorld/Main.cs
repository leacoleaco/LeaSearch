using System;
using System.Threading;

namespace LeaSearch.Plugin.HelloWorld
{
    public class Main : IPlugin
    {
        private SharedContext _sharedContext;

        public void InitPlugin(SharedContext sharedContext)
        {
            _sharedContext = sharedContext;
        }

        public bool SuitableForSuggectionQuery(QueryParam queryParam)
        {
            var queryParamPrefixKeyword = queryParam.PrefixKeyword.ToLower();
            if (queryParamPrefixKeyword == "lea" || queryParamPrefixKeyword == "leasearch")
            {
                return true;
            }
            return false;
        }

        public PluginCalledArg PluginCallActive(QueryParam queryParam)
        {
            return new PluginCalledArg() { InfoMessage = _sharedContext.SharedMethod.GetTranslation(@"leasearch_plugin_helloWorld_pluginCallActive") };
        }

        public QueryListResult Query(QueryParam queryParam)
        {
            var result = new QueryListResult()
            {
                Results =
                {
                    new ResultItem() {
                        IconPath = "app.png",
                        Title = "Query Sample For C#  row1",
                        SubTitle = $"Query:{queryParam.Keyword}" ,
                        //特定的命令执行，只对当前item生效
                        SelectedAction =shareContext =>
                        {
                            shareContext.SharedMethod.ShowMessage("test");
                            return new StateAfterCommandInvoke();
                        }
                    },
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
                },

                //全局的执行命令
                //这里是为了让每个子项都执行相同的内容
                SelectedAction = (shareContext, resultItem) =>
                 {
                     shareContext.SharedMethod.ShowMessage(resultItem.Title);

                     //执行命令后保持程序不隐藏
                     return new StateAfterCommandInvoke() { ShowProgram = true };
                 },

                //在程序进入插件模式后展示的信息
                DefaultInfo = new TextInfo() { Text = "this is a test info"}


            };
            
            //这句话是为了模拟搜索耗费时间的情况
            Thread.Sleep(1500);
            return result;
        }

        public QueryDetailResult QueryDetail(QueryParam queryParam)
        {
            throw new NotImplementedException();
        }


    }
}
