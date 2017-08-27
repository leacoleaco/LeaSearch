using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;
using LeaSearch.Common.Env;
using LeaSearch.Common.Messages;
using LeaSearch.Common.ViewModel;
using LeaSearch.Core.I18N;
using LeaSearch.Core.Ioc;
using LeaSearch.Core.Notice;
using LeaSearch.Core.QueryEngine;
using LeaSearch.Plugin;
using LeaSearch.Plugin.Query;
using Microsoft.Expression.Interactivity.Core;

namespace LeaSearch.ViewModels
{
    public class ShellViewModel : BaseViewModel
    {
        #region Private Fields

        private string _queryText;
        private Visibility _resultVisibility = Visibility.Collapsed;
        private Core.Plugin.Plugin _currentSearchPlugin;
        private QueryState _queryState = QueryState.StartQuery;
        private QueryEngine _queryEngine;

        #endregion

        public ShellViewModel(Settings settings, SuggestionResultViewModel suggestionResultViewModel, SearchResultViewModel searchResultViewModel, DetailResultViewModel detailResultViewModel, QueryEngine queryEngine) : base(settings)
        {
            SuggestionResultViewModel = suggestionResultViewModel;
            SearchResultViewModel = searchResultViewModel;
            DetailResultViewModel = detailResultViewModel;
            this._queryEngine = queryEngine;



            queryEngine.PluginCallActive += QueryEngine_PluginCallActive;
            queryEngine.PluginCallQuery += QueryEngine_PluginCallQuery;
            queryEngine.SuggectionQuery += QueryEngine_SuggectionQuery;
            queryEngine.GetResult += QueryEngine_GetResult;
            queryEngine.EndQuery += QueryEngine_EndQuery;
        }

        #region Property

        /// <summary>
        /// text to search 
        /// </summary>
        public string QueryText
        {
            get { return _queryText; }
            set
            {
                _queryText = value;
                Query();
            }
        }

        /// <summary>
        /// which plugin is current using
        /// </summary>
        public Core.Plugin.Plugin CurrentSearchPlugin
        {
            get { return _currentSearchPlugin; }
            set
            {
                _currentSearchPlugin = value;

                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// search result's suggestion such as :history、bookmark、plugin etc..
        /// </summary>
        public SuggestionResultViewModel SuggestionResultViewModel { get; }

        /// <summary>
        /// search result
        /// </summary>
        public SearchResultViewModel SearchResultViewModel { get; }

        /// <summary>
        /// detail result
        /// </summary>
        public DetailResultViewModel DetailResultViewModel { get; }




        /// <summary>
        /// should the result show
        /// </summary>
        public Visibility ResultVisibility
        {
            get { return _resultVisibility; }
            set
            {
                _resultVisibility = value;
                RaisePropertyChanged();
            }
        }



        #endregion

        #region Command


        public ICommand EscCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    ShowNotice(null);
                    NotifyHideProgram();
                });
            }
        }

        public ICommand EnterCommand
        {
            get
            {
                return new RelayCommand(() =>
                {

                    switch (_queryState)
                    {
                        case QueryState.StartQuery:
                            break;
                        case QueryState.BeginPluginSearch:
                            break;
                        case QueryState.QuerySuitNoPlugin:
                            break;
                        case QueryState.QuerySuitOnePlugin:
                            break;
                        case QueryState.QuerySuitManyPlugin:
                            break;
                        case QueryState.QueryGotOneResult:
                            SearchResultViewModel?.OpenResult();
                            break;
                        case QueryState.QueryGotManyResult:
                            SearchResultViewModel?.SelectFirst();
                            ShowNotice(@"notice_SelectMode".GetTranslation());
                            Messenger.Default.Send<FocusMessage>(new FocusMessage() { FocusTarget = FocusTarget.ResultList });
                            break;
                        case QueryState.QueryGotNoResult:
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }


                });
            }
        }

        public ICommand OpenResultCommand { get; }

        public ICommand SelectNextItemCommand { get; }

        public ICommand SelectPrefItemCommand { get; set; }

        #endregion

        #region Method



        #endregion

        #region Event




        /// <summary>
        /// 通知前端查询状态发生了改变
        /// </summary>
        /// <param name="queryState"></param>
        protected  void NotifyQueryStateChanged(QueryState queryState)
        {
            _queryState = queryState;
            Messenger.Default.Send<QueryState>(queryState);
            //QueryStateChanged?.Invoke(queryState);
        }

        /// <summary>
        /// 通知前端唤醒界面
        /// </summary>
        protected  void NotifyWakeUpProgram()
        {
            ShowNotice(null);
            Messenger.Default.Send(new ShellDisplayMessage() { Display = Display.WakeUp });
        }

        /// <summary>
        /// 通知前端隐藏界面
        /// </summary>
        protected virtual void NotifyHideProgram()
        {
            Messenger.Default.Send(new ShellDisplayMessage() { Display = Display.Hide });
        }
        #endregion

        #region QueryMethod



        /// <summary>
        /// 执行查询
        /// </summary>
        private void Query()
        {

            NotifyQueryStateChanged(QueryState.StartQuery);

            if (!string.IsNullOrEmpty(QueryText))
            {
                _queryEngine.Query(QueryText);
            }
            else
            {
                //if no result ,then close search
                NotifyQueryStateChanged(QueryState.QueryGotNoResult);
                SuggestionResultViewModel.Clear();
                SearchResultViewModel.Clear();
                CurrentSearchPlugin = null;
            }


        }


        private void QueryEngine_PluginCallActive(Core.Plugin.Plugin currentPlugin, PluginCalledArg pluginCalledArg)
        {
            //插件模式如果激活了，则产生指示
            ShowNotice(pluginCalledArg.InfoMessage);

            CurrentSearchPlugin = currentPlugin;


        }

        private void QueryEngine_PluginCallQuery(Core.Plugin.Plugin currentPlugin)
        {
            CurrentSearchPlugin = currentPlugin;
            //清理指示
            ShowNotice(null);
            NotifyQueryStateChanged(QueryState.BeginPluginSearch);
        }

        private void QueryEngine_SuggectionQuery(Core.Plugin.Plugin currentPlugin)
        {
            //建议模式激活了，清理指示
            ShowNotice(null);
            CurrentSearchPlugin = null;
        }

        private void QueryEngine_EndQuery()
        {
            //如果查询终止，无法继续查询，则清理指示
            ShowNotice(null);
            CurrentSearchPlugin = null;
        }


        private void QueryEngine_GetResult(QueryListResult result)
        {
            if (result == null || !result.Results.Any())
            {
                //如果没有返回结果
                NotifyQueryStateChanged(QueryState.QueryGotNoResult);
                ShowNotice(@"notice_NoResult".GetTranslation());
            }
            else
            {
                //如果返回了结果
                if (result.Results.Count > 1)
                {
                    //大于1条记录，则需要进行选择模式
                    ShowNotice(@"notice_EnterChooseMode".GetTranslation());
                    NotifyQueryStateChanged(QueryState.QueryGotManyResult);
                }
                else
                {
                    //只返回了一条记录
                    NotifyQueryStateChanged(QueryState.QueryGotOneResult);
                }
            }

        }


        /// <summary>
        /// 显示提示
        /// </summary>
        /// <param name="message"></param>
        public void ShowNotice(string message)
        {
            UiNoticeHelper.ShowInfoNotice(message);
        }

        #endregion

    }


    public enum QueryState
    {
        StartQuery,
        BeginPluginSearch,
        QuerySuitNoPlugin,
        QuerySuitOnePlugin,
        QuerySuitManyPlugin,
        QueryGotOneResult,
        QueryGotManyResult,
        QueryGotNoResult,
    }


}