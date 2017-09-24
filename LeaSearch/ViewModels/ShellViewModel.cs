using System.Linq;
using System.Windows;
using System.Windows.Input;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;
using LeaSearch.Common.Env;
using LeaSearch.Common.Messages;
using LeaSearch.Common.ViewModel;
using LeaSearch.Core.I18N;
using LeaSearch.Core.MessageModels;
using LeaSearch.Core.Notice;
using LeaSearch.Core.QueryEngine;
using LeaSearch.Plugin;
using LeaSearch.Plugin.Query;

namespace LeaSearch.ViewModels
{
    public class ShellViewModel : BaseViewModel
    {
        #region Private Fields

        private string _queryText;
        private Visibility _resultVisibility = Visibility.Collapsed;
        private Core.Plugin.Plugin _currentSearchPlugin;
        private QueryState _queryState = QueryState.QueryStart;
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
            queryEngine.QueryEnd += EngineQueryEnd;
            queryEngine.QueryError += QueryEngine_QueryError;

            queryEngine.ShowHelpInfo += (helpinfo) =>
            {
                Messenger.Default.Send(new SetHelpInfoMessage() { HelpInfo = helpinfo });
            };
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
                    var resultsCount = SearchResultViewModel?.Results?.Count;
                    if (resultsCount == 1)
                    {
                        SearchResultViewModel?.OpenResult();
                    }
                    else if (resultsCount > 1)
                    {
                        ShowNotice(@"notice_SelectMode".GetTranslation());
                        Messenger.Default.Send<FocusMessage>(new FocusMessage() { FocusTarget = FocusTarget.ResultList });
                        SearchResultViewModel?.SelectFirst();

                    }

                });
            }
        }

        public ICommand OpenResultCommand { get; }

        public ICommand SelectNextItemCommand { get; }

        public ICommand SelectPrefItemCommand { get; set; }

        public ICommand TabCommand
        {
            get
            {
                return new RelayCommand((() =>
           {
               var resultsCount = SearchResultViewModel?.Results?.Count;
               if (resultsCount == 1)
               {
                   Messenger.Default.Send<FocusMessage>(new FocusMessage() { FocusTarget = FocusTarget.ResultList });
                   SearchResultViewModel?.SelectFirst();
               }
               else if (resultsCount > 1)
               {
                   ShowNotice(@"notice_SelectMode".GetTranslation());
                   Messenger.Default.Send<FocusMessage>(new FocusMessage() { FocusTarget = FocusTarget.ResultList });
                   SearchResultViewModel?.SelectFirst();

               }

           }));
            }
        }

        #endregion

        #region Method



        #endregion

        #region Event




        /// <summary>
        /// 通知前端查询状态发生了改变
        /// </summary>
        /// <param name="queryState"></param>
        protected void NotifyQueryStateChanged(QueryState queryState)
        {
            _queryState = queryState;
            Messenger.Default.Send<QueryState>(queryState);
            //QueryStateChanged?.Invoke(queryState);
        }

        /// <summary>
        /// 清理提示消息，并通知前端唤醒界面
        /// </summary>
        protected void NotifyWakeUpProgram()
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

            NotifyQueryStateChanged(QueryState.QueryStart);

            if (!string.IsNullOrEmpty(QueryText))
            {
                _queryEngine.Query(QueryText);
            }
            else
            {
                //if no result ,then close search
                NotifyQueryStateChanged(QueryState.QueryEnd);
                SuggestionResultViewModel.Clear();
                SearchResultViewModel.Clear();
                SearchResultViewModel.ClearMoreInfo();
                CurrentSearchPlugin = null;
            }


        }


        private void QueryEngine_PluginCallActive(Core.Plugin.Plugin currentPlugin, PluginCalledArg pluginCalledArg)
        {
            //插件模式激活，也是查询完毕的一个方式
            NotifyQueryStateChanged(QueryState.QueryEnd);

            CurrentSearchPlugin = currentPlugin;

            //插件模式如果激活了，则产生指示
            ShowNotice(pluginCalledArg?.InfoMessage);
        }

        private void QueryEngine_PluginCallQuery(Core.Plugin.Plugin currentPlugin)
        {
            CurrentSearchPlugin = currentPlugin;
            //清理指示
            ShowNotice(null);
        }

        private void QueryEngine_SuggectionQuery(Core.Plugin.Plugin currentPlugin)
        {
            //建议模式激活了，清理指示
            ShowNotice(null);
            CurrentSearchPlugin = null;
        }

        private void EngineQueryEnd()
        {
            //如果查询终止，无法继续查询，则清理指示
            ShowNotice(null);
            CurrentSearchPlugin = null;
            SearchResultViewModel.ClearMoreInfo();
            NotifyQueryStateChanged(QueryState.QueryEnd);

        }

        private void QueryEngine_QueryError()
        {
            //插件查询出错，
            NotifyQueryStateChanged(QueryState.QueryError);
            SearchResultViewModel.ClearMoreInfo();
        }


        private void QueryEngine_GetResult(QueryListResult result)
        {
            if (result?.Results == null || !result.Results.Any())
            {
                //如果没有返回结果
                ShowNotice(@"notice_NoResult".GetTranslation());
                SearchResultViewModel.ClearMoreInfo();
            }
            else
            {
                //如果返回了结果
                if (result.Results.Count > 1)
                {
                    //大于1条记录，则需要进行选择模式
                    ShowNotice(@"notice_EnterChooseMode".GetTranslation());
                }
            }
            NotifyQueryStateChanged(QueryState.QueryEnd);

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
        /// <summary>
        /// 开始查询
        /// </summary>
        QueryStart,
        /// <summary>
        /// 查询结束
        /// </summary>
        QueryEnd,
        /// <summary>
        /// 查询出错
        /// </summary>
        QueryError,
        /// <summary>
        /// 查询被终止
        /// </summary>
        QueryStop
    }


}