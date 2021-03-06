﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using FuzzyString;
using LeaSearch.Common.Env;
using LeaSearch.Core.Command;
using LeaSearch.Core.Plugin;
using LeaSearch.Plugin;
using LeaSearch.Plugin.Query;
using QueryMode = LeaSearch.Plugin.Query.QueryMode;

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
        private readonly string[] _queryCommandChars = new string[] { ":", "：" };

        /// <summary>
        /// this is the split word that use to indicate action word   
        /// </summary>
        private const char PluginActiveKeyword = ' ';

        private const char NoneQueryPrefix = '*';

        private CancellationTokenSource _updateSource;

        private readonly PluginManager _pluginManager;

        private readonly LeaSearchCommandManager _commandManager;

        private readonly SharedContext _sharedContext;

        private QueryMode _currentQueryMode = QueryMode.None;

        /// <summary>
        /// 当前的返回结果模式
        /// </summary>
        private ResultMode _currentResultMode = ResultMode.List;


        /// <summary>
        /// 当前的查询模式
        /// </summary>
        public QueryMode CurrentQueryMode
        {
            get { return _currentQueryMode; }
            private set
            {
                if (_currentQueryMode != value)
                {
                    //每切换一个模式，就要清除之前的帮助信息
                    OnShowHelpInfo(null);
                    _currentResultMode = ResultMode.List;
                }

                _currentQueryMode = value;

                if (_sharedContext != null)
                {
                    _sharedContext.CurrentQueryMode = value;
                }
            }

        }

        /// <summary>
        /// 当前的结果模式
        /// </summary>
        public ResultMode CurrentResultMode => _currentResultMode;

        public void Init()
        {

        }

        /// <summary>
        /// 模糊查询配置
        /// </summary>
        private readonly List<FuzzyStringComparisonOptions> _options = new List<FuzzyStringComparisonOptions>()
        {
            FuzzyStringComparisonOptions.UseJaccardDistance,
            FuzzyStringComparisonOptions.UseNormalizedLevenshteinDistance,
            FuzzyStringComparisonOptions.UseJaccardDistance,
            FuzzyStringComparisonOptions.UseOverlapCoefficient,
            FuzzyStringComparisonOptions.UseLongestCommonSubsequence,
            FuzzyStringComparisonOptions.CaseSensitive,
        };


        public QueryEngine(PluginManager pluginManager, LeaSearchCommandManager commandManager, SharedContext sharedContext)
        {
            this._pluginManager = pluginManager;
            _commandManager = commandManager;
            _sharedContext = sharedContext;
        }



        /// <summary>
        /// 启动查询
        /// </summary>
        /// <param name="queryText"></param>
        public void Query(string queryText)
        {
            //如果什么字符都没传入，则不执行查询
            if (string.IsNullOrWhiteSpace(queryText))
            {
                CurrentQueryMode = QueryMode.None;
                return;
            }

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


            //启用新线程查询
            Task.Factory.StartNew(() =>
            {

                //如果输入的是系统命令
                var startWithCommand = _queryCommandChars.Any(c => c != null && queryText.StartsWith(c));

                if (startWithCommand)
                {
                    queryParam.Keyword = queryText;
                    queryParam.PrefixKeyword = "*";
                    //进入 comand 模式
                    DoCommandCallQuery(queryParam);
                    return;
                }

                if (queryText.Contains(PluginActiveKeyword))
                {
                    //do the plugin call mode search
                    //如果包含有插件关键字分隔符 判断是否有符合要求的插件
                    var actionCharIndex = queryText.IndexOf(PluginActiveKeyword);
                    string part1 = queryText.Substring(0, actionCharIndex);
                    string part2 = queryText.Substring(actionCharIndex + 1);
                    queryParam.PrefixKeyword = part1;
                    queryParam.Keyword = part2;
                    //查找插件并执行
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
                //查找合适的插件批量执行
                var suitableQueryPlugins = SearchSuitablePlugins(queryParam, QueryMode.Suggestion);
                if (suitableQueryPlugins == null || !suitableQueryPlugins.Any())
                {
                    //没有合适的可以执行的查询
                    CurrentQueryMode = QueryMode.None;
                    OnQueryEnd();
                    return;
                }
                //进入 suggect 模式
                DoSuggestionQuery(queryParam, suitableQueryPlugins);

            }, _updateSource.Token);

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

                var queryDetailResult = plugin?.InvokeQueryItemDetail(currentItem);
                OnGetItemDetailResult(queryDetailResult);

            }, _updateSource.Token);
        }

        /// <summary>
        /// 执行程序内置命令模式
        /// </summary>
        /// <param name="queryParam"></param>
        private void DoCommandCallQuery(QueryParam queryParam)
        {

            CurrentQueryMode = QueryMode.Command;

            //查询是否有合适的系统插件
            if (_commandManager == null) return;
            var keywords = _commandManager.GetKeywords();


            //模糊检索程序内置命令
            var queryResults = keywords?.Where(s => s.ApproximatelyEquals(queryParam.Keyword, _options, FuzzyStringComparisonTolerance.Weak)).ToArray();

            //var queryListResult = _commandManager.
            var queryListResult = new QueryListResult();

            //返回查找的信息
            _commandManager.GetCommandInfo(queryResults).ForEach(x =>
            {
                queryListResult.AddResultItem(new ResultItem()
                {
                    QueryParam = queryParam,
                    Title = x.Name,
                    SubTitle = x.Introduce,
                    IconPath = Constant.DefaultIcon,
                    SelectedAction = (s) =>
                    {
                        x.Action.Invoke(queryParam);

                        return new StateAfterCommandInvoke()
                        {
                            ShowProgram = false
                        };
                    }
                });
            });

            queryListResult.QueryMode = QueryMode.Command;


            //return the result
            OnGetListResult(queryListResult);

            _commandManager.InvokeCommand(queryParam);
        }

        /// <summary>
        /// 执行插件查询，每次只能有一个插件被激活
        /// </summary>
        /// <param name="queryParam"></param>
        /// <param name="queryPlugin"></param>
        private void DoPluginCallQuery(QueryParam queryParam, Plugin.Plugin queryPlugin)
        {
            CurrentQueryMode = QueryMode.PluginCall;

            //先显示帮助信息
            var helpInfo = queryPlugin.InvokeGetHelpInfo(queryParam);
            if (helpInfo != null)
            {
                OnShowHelpInfo(helpInfo);
            }

            if (string.IsNullOrWhiteSpace(queryParam.Keyword))
            {
                //如果只是刚刚激活plugin call 查询模式
                var pluginCalledArg = queryPlugin.InvokePluginCallActive(queryParam, OnQueryError);

                if (pluginCalledArg != null)
                {
                    _currentResultMode = pluginCalledArg.ResultMode;
                }

                OnPluginCallActive(queryPlugin, pluginCalledArg);
            }
            else
            {
                //如果激活了 plugin call 查询模式并已经传入数据

                var currentSearchPlugin = queryPlugin;

                OnPluginCallQuery(currentSearchPlugin);

                if (_currentResultMode == ResultMode.List)
                {
                    var queryListResult = currentSearchPlugin.InvokeQueryList(queryParam, () =>
                     {
                         OnGetListResult(null);
                         OnQueryError();
                     });

                    //预处理一些需要展示的数据
                    //prepare some data for result display
                    if (queryListResult != null)
                    {
                        queryListResult.Results?.ForEach(x =>
                        {
                            x.PluginId = currentSearchPlugin.PluginId;
                            x.QueryParam = queryParam;
                        });


                        queryListResult.QueryMode = QueryMode.PluginCall;
                    }

                    //return the result
                    OnGetListResult(queryListResult);

                }
                else if (_currentResultMode == ResultMode.Detail)
                {

                    var detailResult = currentSearchPlugin.InvokeQueryDetail(queryParam, () =>
                    {
                        OnGetDetailResult(null);
                        OnQueryError();
                    });

                    //预处理一些需要展示的数据
                    //prepare some data for result display
                    if (detailResult != null)
                    {
                        detailResult.PluginId = currentSearchPlugin.PluginId;
                        detailResult.QueryParam = queryParam;
                        detailResult.QueryMode = QueryMode.PluginCall;
                    }

                    //return the result
                    OnGetDetailResult(detailResult);

                }

            }
        }

        private void DoSuggestionQuery(QueryParam queryParam, Plugin.Plugin[] queryPlugins)
        {

            CurrentQueryMode = QueryMode.Suggestion;

            //使用 提供的所有 plugin ，按照关联度进行查询
            //TODO : 按照关联度执行多个查询


            var queryPlugin = queryPlugins[0];


            OnSuggectionQuery(queryPlugin);

            var queryListResult = queryPlugin.InvokeQueryList(queryParam, () =>
            {
                OnGetListResult(null);
                OnQueryError();
            });

            //prepare some data for result display
            if (queryListResult != null)
            {
                queryListResult?.Results?.ForEach(x =>
                {
                    x.PluginId = queryPlugin.PluginId;
                    x.QueryParam = queryParam;
                    if (!string.IsNullOrWhiteSpace(x.IconPath))
                    {
                        x.IconPath = Path.Combine(queryPlugin.PluginRootPath, x.IconPath);
                    }
                });
                queryListResult.QueryMode = QueryMode.Suggestion;
            }
            //return the result
            OnGetListResult(queryListResult);

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
                else if (queryMode == QueryMode.Suggestion)
                {

                    //suggection mode

                    //do not give suggection
                    if (!plugin.PluginMetadata.EnableSuggection) return;


                    //if plugin will give suggection ,then it must have some rule
                    if (plugin.InvokeSuitableForSuggectionQuery(queryParam, () =>
                    {
                        OnQueryError();
                    }))
                    {
                        //other plugin is add to second choice if suitable
                        result.Add(plugin);
                    }

                }

            });
            return result.ToArray();
        }


        #region Event

        /// <summary>
        /// 当得到了返回列表数据的时候，发生事件
        /// </summary>
        public event Action<QueryListResult> GetListResult;

        public event Action<QueryDetailResult> GetDetailResult;

        public event Action<QueryItemDetailResult> GetItemDetailResult;


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
        public event Action QueryEnd;

        /// <summary>
        /// 查询过程中发生异常
        /// </summary>
        public event Action QueryError;



        public event Action<HelpInfo> ShowHelpInfo;
        #endregion

        protected virtual void OnGetListResult(QueryListResult result)
        {
            //如果返回了结果，则添加列表
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
            {
                GetListResult?.Invoke(result);
            }));
        }

        protected virtual void OnGetDetailResult(QueryDetailResult detail)
        {
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
            {
                GetDetailResult?.Invoke(detail);
            }));
        }

        protected virtual void OnGetItemDetailResult(QueryItemDetailResult result)
        {
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
            {
                GetItemDetailResult?.Invoke(result);
            }));
        }

        protected virtual void OnPluginCallActive(Plugin.Plugin calledPlugin, PluginCalledArg arg)
        {
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
            {
                PluginCallActive?.Invoke(calledPlugin, arg);
            }));
        }

        protected virtual void OnPluginCallQuery(Plugin.Plugin plugin)
        {
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
            {
                PluginCallQuery?.Invoke(plugin);
            }));
        }

        protected virtual void OnSuggectionQuery(Plugin.Plugin plugin)
        {
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
           {
               SuggectionQuery?.Invoke(plugin);
           }));
        }

        protected virtual void OnQueryEnd()
        {
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
            {
                QueryEnd?.Invoke();
            }));
        }


        protected virtual void OnQueryError()
        {
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
            {
                QueryError?.Invoke();
            }));
        }

        protected virtual void OnShowHelpInfo(HelpInfo info)
        {
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
            {
                ShowHelpInfo?.Invoke(info);
            }));
        }


    }
}
