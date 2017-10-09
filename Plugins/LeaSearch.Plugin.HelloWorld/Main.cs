using System.Reflection;
using System.Threading;
using System.Windows.Documents;
using System.Windows.Markup;
using LeaSearch.Plugin.DetailInfos;
using LeaSearch.Plugin.Query;

namespace LeaSearch.Plugin.HelloWorld
{
    public class Main : Plugin
    {
        private FlowDocument _HelpDocument;

        public override void InitPlugin(SharedContext sharedContext, IPluginApi pluginApi)
        {
            base.InitPlugin(sharedContext, pluginApi);

            pluginApi.GetPluginEmbedResouceStream("app.png");

            //读取 dll 内文件，可以吧文件设置为 “嵌入的资源”，然后通过以下代码方式读取
            var helpInfoStream = pluginApi.GetPluginEmbedResouceStream("Resources.HelpInfo.xaml");
            if (helpInfoStream != null) _HelpDocument = XamlReader.Load(helpInfoStream) as FlowDocument;

        }

        public override bool SuitableForSuggectionQuery(QueryParam queryParam)
        {
            var queryParamPrefixKeyword = queryParam.PrefixKeyword.ToLower();
            if (queryParamPrefixKeyword == "lea" || queryParamPrefixKeyword == "leasearch")
            {
                return true;
            }
            return false;
        }

        public override PluginCalledArg PluginCallActive(QueryParam queryParam)
        {
            return new PluginCalledArg()
            {
                //插件模式激活后，在插件下发的提示信息
                InfoMessage = SharedContext.SharedMethod.GetTranslation(@"leasearch_plugin_helloWorld_pluginCallActive"),
                //在程序进入插件模式后展示的信息
                //可以可以返回的信息： FlowDocumentInfo （流文档 形式）
            };
        }

        public override QueryListResult QueryList(QueryParam queryParam)
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
                        },
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
                SelectAction = (shareContext, resultItem) =>
                 {
                     shareContext.SharedMethod.ShowMessage(resultItem.Title);

                     //执行命令后保持程序不隐藏
                     return new StateAfterCommandInvoke() { ShowProgram = true };
                 },

                //在程序进入插件模式后取得查询结果后展示的信息
                MoreInfo = new TextInfo() { Text = "this is a test info when got result" }


            };

            //这句话是为了模拟搜索耗费时间的情况
            Thread.Sleep(1500);
            return result;
        }

        public override QueryItemDetailResult QueryItemDetail(ResultItem currentItem)
        {
            //模拟查询详情的耗时情况
            Thread.Sleep(3000);
            //返回查询的详情显示
            return new QueryItemDetailResult() { DetailResult = new TextInfo() { Text = $"this is a test info when preview {currentItem.Title}" } };
        }


    }
}
