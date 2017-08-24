using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using LeaSearch.Core.Plugin;
using LeaSearch.Infrastructure.Logger;
using LeaSearch.Plugin;

namespace LeaSearch.Core.QueryEngine
{

    /// <summary>
    /// 查询引擎的策略思路：
    /// 
    /// 1、为每个查询的plugin都设置查询类型
    /// 2、用户输入查询字符串后，首先进行字符串的判断，先预判是匹配那几个查询类型。
    /// 3、吧符合要求的查询 plugin 都读到待执行 列表中
    /// 4、吧每个插件的查询结果都进行一个排序优化，  对本次调用插件排名靠前的插件的数据也优先展示
    /// </summary>
    public class QueryEngine
    {
        /// <summary>
        /// this is to judge if this query is a command
        /// </summary>
        private const char QueryCommandChar = ':';

        /// <summary>
        /// this is the split word that use to indicate action word   
        /// </summary>
        private const char PluginActiveKeyword = ' ';

        private const char NoneQueryPrefix = '*';

        private CancellationTokenSource _updateSource;

        private readonly PluginManager _pluginManager;

        public QueryEngine(PluginManager pluginManager)
        {
            this._pluginManager = pluginManager;
        }

        public void Init()
        {

        }


        /// <summary>
        /// 启动查询
        /// </summary>
        /// <param name="queryText"></param>
        public void Query(string queryText)
        {
            //如果什么字符都没传入，则不执行查询
            if (string.IsNullOrWhiteSpace(queryText)) return;

            //we need to trim start space
            queryText = queryText.TrimStart();

            //if a new search start, cancle the last one
            _updateSource?.Cancel();
            _updateSource = new CancellationTokenSource();

            var queryParam = new QueryParam
            {
                OriginQueryStr = queryText,
                QueryType = QueryType.List
            };


            if (queryText.Contains(PluginActiveKeyword))
            {

                //do the plugin call mode search
                //如果包含有插件关键字分隔符 判断是否有符合要求的插件
                var actionCharIndex = queryText.IndexOf(PluginActiveKeyword);
                string part1 = queryText.Substring(0, actionCharIndex);
                string part2 = queryText.Substring(actionCharIndex + 1);
                queryParam.PrefixKeyword = part1;
                queryParam.Keyword = part2;

                var pluginCallSuitablePlugins = SearchSuitablePlugins(queryParam, QueryMode.PluginCall);
                if (pluginCallSuitablePlugins.Any())
                {
                    //进入 plugin call模式 
                    DoPluginCallQuery(queryParam, pluginCallSuitablePlugins.First());
                    return;
                }

                //否则继续建议模式
            }

            //如果不包含插件关键词分割符，则进入  建议模式
            // do the normal search
            queryParam.Keyword = queryText;
            queryParam.PrefixKeyword = "*";

            var suitableQueryPlugins = SearchSuitablePlugins(queryParam, QueryMode.Suggection);

            if (suitableQueryPlugins == null || !suitableQueryPlugins.Any())
            {
                OnEndQuery();
                return;
            }

            //进入 suggect 模式
            DoSuggestionQuery(queryParam, suitableQueryPlugins);


        }

        /// <summary>
        /// 调用插件查询当前项目的详情
        /// </summary>
        /// <param name="currentItem"></param>
        public void QueryDetail(ResultItem currentItem)
        {
            if (string.IsNullOrWhiteSpace(currentItem?.PluginId)) return;

            //if a new search start, cancle the last one
            _updateSource?.Cancel();
            _updateSource = new CancellationTokenSource();

            Task.Factory.StartNew(() =>
            {


                var plugin = _pluginManager.GetPlugins().Find(p =>
                {
                    if (p.PluginId == null) return false;
                    return p.PluginId == currentItem.PluginId;
                });

                var queryDetailResult = plugin?.PluginInstance.QueryDetail(currentItem);
                OnGetDetailResult(queryDetailResult);

            }, _updateSource.Token);
        }

        /// <summary>
        /// 执行插件查询，每次只能有一个插件被激活
        /// </summary>
        /// <param name="queryParam"></param>
        /// <param name="queryPlugin"></param>
        private void DoPluginCallQuery(QueryParam queryParam, Plugin.Plugin queryPlugin)
        {
            if (string.IsNullOrWhiteSpace(queryParam.Keyword))
            {
                //如果只是刚刚激活plugin call 查询模式
                try
                {
                    var pluginCalledArg = queryPlugin.PluginInstance.PluginCallActive(queryParam);
                    OnPluginCallActive(queryPlugin, pluginCalledArg);
                }
                catch (Exception e)
                {
                    Logger.Exception($"plugin <{queryPlugin}> call PluginCallActive error: {e.Message}", e);
#if DEBUG
                    MessageBox.Show($"plugin <{queryPlugin}> call PluginCallActive error: {e.Message}");
#endif
                }
            }
            else
            {
                //如果激活了 plugin call 查询模式并已经传入数据


                //使用 提供的 plugin ，进行查询
                Task.Factory.StartNew(() =>
                {

                    var currentSearchPlugin = queryPlugin;

                    OnPluginCallQuery(currentSearchPlugin);

                    try
                    {
                        var queryListResult = currentSearchPlugin.PluginInstance.Query(queryParam);

                        //预处理一些需要展示的数据
                        //prepare some data for result display
                        queryListResult?.Results?.ForEach(x =>
                        {
                            x.PluginId = currentSearchPlugin.PluginId;
                            x.QueryParam = queryParam;
                            if (!string.IsNullOrWhiteSpace(x.IconPath))
                            {
                                x.IconPath = Path.Combine(currentSearchPlugin.PluginRootPath, x.IconPath);
                            }
                        });

                        //return the result
                        OnGetResult(queryListResult);
                    }
                    catch (Exception e)
                    {
                        Logger.Exception($"plugin <{queryPlugin}> call query error: {e.Message}", e);
#if DEBUG
                        MessageBox.Show($"plugin <{queryPlugin}> call query error: {e.Message}");
#endif
                        OnGetResult(null);
                    }



                }, _updateSource.Token);



            }
        }

        private void DoSuggestionQuery(QueryParam queryParam, Plugin.Plugin[] queryPlugins)
        {

            //使用 提供的所有 plugin ，按照关联度进行查询
            //TODO : 按照关联度执行多个查询
            Task.Factory.StartNew(() =>
            {

                var queryPlugin = queryPlugins[0];


                try
                {
                    OnSuggectionQuery(queryPlugin);

                    var queryListResult = queryPlugin.PluginInstance.Query(queryParam);

                    //prepare some data for result display
                    queryListResult?.Results?.ForEach(x =>
                    {
                        x.PluginId = queryPlugin.PluginId;
                        x.QueryParam = queryParam;
                        if (!string.IsNullOrWhiteSpace(x.IconPath))
                        {
                            x.IconPath = Path.Combine(queryPlugin.PluginRootPath, x.IconPath);
                        }
                    });

                    //return the result
                    OnGetResult(queryListResult);
                }
                catch (Exception e)
                {
                    Logger.Exception($"plugin <{queryPlugin}> call query error: {e.Message}", e);
#if DEBUG
                    MessageBox.Show($"plugin <{queryPlugin}> call query error: {e.Message}");
#endif
                    OnGetResult(null);
                }




            }, _updateSource.Token);

        }


        /// <summary>
        /// list the plugin that suit for this query
        /// 查找合适的查询插件
        /// or suggection
        /// </summary>
        /// <param name="queryParam">
        /// 如果为 插件调取 模式， 则本参数的 prefixKeyword 应该不能为空
        /// 如果为 建议模式， 则 prefixKeyword必须为 “*”
        /// </param>
        /// <param name="queryMode"></param>
        /// <returns></returns>
        private Plugin.Plugin[] SearchSuitablePlugins(QueryParam queryParam, QueryMode queryMode)
        {
            var result = new List<Plugin.Plugin>();
            _pluginManager.GetPlugins()?.ForEach(plugin =>
            {

                if (plugin.IsDisabled) return;

                var prefixKeywords = plugin.PrefixKeywords;
                if (prefixKeywords == null || prefixKeywords.Length <= 0) return;

                if (queryMode == QueryMode.PluginCall)
                {
                    if (prefixKeywords.Contains(queryParam.PrefixKeyword))
                    {
                        //pluin call mode, display in first choice
                        result.Add(plugin);
                    }
                }
                else if (queryMode == QueryMode.Suggection)
                {

                    //suggection mode

                    //do not give suggection
                    if (!plugin.PluginMetadata.ParticipateSuggection) return;

                    try
                    {
                        //if plugin will give suggection ,then it must have some rule
                        if (plugin.PluginInstance.SuitableForSuggectionQuery(queryParam))
                        {
                            //other plugin is add to second choice if suitable
                            result.Add(plugin);
                        }

                    }
                    catch (Exception e)
                    {
                        Logger.Exception($"plugin <{plugin}>  call SuitableForThisQuery method error: {e.Message}", e);
#if DEBUG
                        MessageBox.Show($"plugin  <{plugin}> call SuitableForThisQuery method error: {e.Message}");
#endif
                    }
                }

            });
            return result.ToArray();
        }


        #region Event

        /// <summary>
        /// 当得到了返回列表数据的时候，发生事件
        /// </summary>
        public event Action<QueryListResult> GetResult;

        public event Action<QueryDetailResult> GetDetailResult;


        /// <summary>
        /// 当激活插件查询，但是并未传入数据的时候 发生事件
        /// </summary>
        public event Action<Plugin.Plugin, PluginCalledArg> PluginCallActive;

        /// <summary>
        /// 当开始插件查询模式，并传入查询数据的时候 发生事件
        /// </summary>
        public event Action<Plugin.Plugin> PluginCallQuery;


        /// <summary>
        /// 开始建议模式 ，并传入查询数据的时候发生事件
        /// </summary>
        public event Action<Plugin.Plugin> SuggectionQuery;

        /// <summary>
        /// 指示结束查询，没有合适的查询插件或者查询词为空等，导致无法继续查询
        /// </summary>
        public event Action EndQuery;


        #endregion

        protected virtual void OnGetResult(QueryListResult result)
        {
            GetResult?.Invoke(result);
        }

        protected virtual void OnGetDetailResult(QueryDetailResult result)
        {
            GetDetailResult?.Invoke(result);
        }

        protected virtual void OnPluginCallActive(Plugin.Plugin calledPlugin, PluginCalledArg arg)
        {
            PluginCallActive?.Invoke(calledPlugin, arg);
        }

        protected virtual void OnPluginCallQuery(Plugin.Plugin plugin)
        {
            PluginCallQuery?.Invoke(plugin);
        }

        protected virtual void OnSuggectionQuery(Plugin.Plugin plugin)
        {
            SuggectionQuery?.Invoke(plugin);
        }

        protected virtual void OnEndQuery()
        {
            EndQuery?.Invoke();
        }



    }
}
