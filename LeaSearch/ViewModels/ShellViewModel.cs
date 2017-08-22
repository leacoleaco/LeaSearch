using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using LeaSearch.Common.Env;
using LeaSearch.Common.ViewModel;
using LeaSearch.Core.I18N;
using LeaSearch.Core.Ioc;
using LeaSearch.Core.QueryEngine;
using LeaSearch.Plugin;
using Microsoft.Expression.Interactivity.Core;

namespace LeaSearch.ViewModels
{
    public class ShellViewModel : BaseViewModel
    {
        #region Private Fields

        private string _queryText;
        private Visibility _resultVisibility = Visibility.Collapsed;
        private ResultMode _resultMode = ResultMode.ListOnly;
        private Core.Plugin.Plugin _currentSearchPlugin;
        private string _errorTextBlock;
        private string _infoTextBlock;
        private QueryState _queryState = QueryState.StartQuery;

        #endregion

        public ShellViewModel(Settings settings, SuggestionResultViewModel suggestionResultViewModel, SearchResultViewModel searchResultViewModel, DetailResultViewModel detailResultViewModel) : base(settings)
        {
            SuggestionResultViewModel = suggestionResultViewModel;
            SearchResultViewModel = searchResultViewModel;
            DetailResultViewModel = detailResultViewModel;




            var queryEngine = Ioc.Reslove<QueryEngine>();

            queryEngine.PluginCallActive += QueryEngine_PluginCallActive;
            queryEngine.PluginCallQuery += QueryEngine_PluginCallQuery;
            queryEngine.SuggectionQuery += QueryEngine_SuggectionQuery;
            queryEngine.GetResult += QueryEngine_GetResult;
            queryEngine.EndQuery += QueryEngine_EndQuery;


            EnterCommand = new ActionCommand(o =>
             {

                 switch (KeyInputMode)
                 {
                     case KeyInputMode.Input:
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
                             case QueryState.QueryGotNoResult:
                                 break;
                             case QueryState.QueryGotOneResult:
                                 SearchResultViewModel.OpenResult();
                                 break;
                             case QueryState.QueryGotManyResult:
                                 KeyInputMode = KeyInputMode.Select;
                                 searchResultViewModel.SelectFirst();
                                 break;
                             default:
                                 throw new ArgumentOutOfRangeException();
                         }
                         break;
                     case KeyInputMode.Select:
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
                                 break;
                             case QueryState.QueryGotManyResult:
                                 SearchResultViewModel.OpenResult();
                                 break;
                             case QueryState.QueryGotNoResult:
                                 break;
                             default:
                                 throw new ArgumentOutOfRangeException();
                         }
                         break;
                     default:
                         throw new ArgumentOutOfRangeException();
                 }
             });

            EscCommand = new ActionCommand(o =>
           {
               switch (KeyInputMode)
               {
                   case KeyInputMode.Input:
                       OnNotifyHideProgram();
                       break;
                   case KeyInputMode.Select:
                       KeyInputMode = KeyInputMode.Input;
                       break;
                   default:
                       throw new ArgumentOutOfRangeException();
               }

           });

            SelectNextItemCommand = new ActionCommand(o =>
              {
                  if (KeyInputMode == KeyInputMode.Select)
                  {
                      SearchResultViewModel.SelectNext();
                  }
              });

            SelectPrefItemCommand = new ActionCommand(o =>
              {
                  if (KeyInputMode == KeyInputMode.Select)
                  {
                      SearchResultViewModel.SelectPrev();
                  }
              });
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

                OnPropertyChanged();
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
        /// 目前按键模式
        /// </summary>
        public KeyInputMode KeyInputMode { get; private set; } = KeyInputMode.Input;

        /// <summary>
        /// should the result show
        /// </summary>
        public Visibility ResultVisibility
        {
            get { return _resultVisibility; }
            set
            {
                _resultVisibility = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// which way result shows
        /// </summary>
        public ResultMode ResultMode
        {
            get { return _resultMode; }
            set
            {
                _resultMode = value;
                OnResultModeChanged(value);
            }
        }

        /// <summary>
        /// Info we show
        /// </summary>
        public string InfoTextBlock
        {
            get { return _infoTextBlock; }
            set
            {
                _infoTextBlock = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// error we show
        /// </summary>
        public string ErrorTextBlock
        {
            get { return _errorTextBlock; }
            set
            {
                _errorTextBlock = value;
                OnPropertyChanged();
            }
        }



        #endregion

        #region Command

        public ICommand OpenResultCommand { get; }

        private ICommand EscCommand { get; }

        public ICommand EnterCommand { get; }

        public ICommand SelectNextItemCommand { get; }

        public ICommand SelectPrefItemCommand { get; set; }

        #endregion

        #region Method



        #endregion

        #region Event


        /// <summary>
        /// notify the result mode has changed
        /// </summary>
        public event Action<ResultMode> ResultModeChanged;
        protected virtual void OnResultModeChanged(ResultMode resultMod)
        {
            ResultModeChanged?.Invoke(resultMod);
        }

        public event Action<QueryState> QueryStateChanged;
        protected virtual void OnQueryStateChanged(QueryState queryState)
        {
            _queryState = queryState;
            QueryStateChanged?.Invoke(queryState);
        }

        /// <summary>
        /// 通知唤醒界面
        /// </summary>
        public event Action NotifyWakeUpProgram;
        protected virtual void OnNotifyWakeUpProgram()
        {
            NotifyWakeUpProgram?.Invoke();
        }

        /// <summary>
        /// 通知隐藏界面
        /// </summary>
        public event Action NotifyHideProgram;
        protected virtual void OnNotifyHideProgram()
        {
            NotifyHideProgram?.Invoke();
        }
        #endregion

        #region QueryMethod



        /// <summary>
        /// do the query
        /// </summary>
        private void Query()
        {

            OnQueryStateChanged(QueryState.StartQuery);

            if (!string.IsNullOrEmpty(QueryText))
            {
                Ioc.Reslove<QueryEngine>().Query(QueryText);
            }
            else
            {
                //if no result ,then close search
                OnQueryStateChanged(QueryState.QueryGotNoResult);
                SuggestionResultViewModel.Clear();
                SearchResultViewModel.Clear();
            }


        }


        private void QueryEngine_PluginCallActive(Core.Plugin.Plugin currentPlugin, PluginCalledArg pluginCalledArg)
        {
            //插件模式如果激活了，则产生指示
            InfoTextBlock = pluginCalledArg.InfoMessage;

            CurrentSearchPlugin = currentPlugin;

            SearchResultViewModel.Clear();

        }

        private void QueryEngine_PluginCallQuery(Core.Plugin.Plugin currentPlugin)
        {
            CurrentSearchPlugin = currentPlugin;
            //清理指示
            ShowNotice(null);
            OnQueryStateChanged(QueryState.BeginPluginSearch);
        }

        private void QueryEngine_SuggectionQuery(Core.Plugin.Plugin currentPlugin)
        {
            //建议模式激活了，清理指示
            ShowNotice(null);
        }

        private void QueryEngine_EndQuery()
        {
            //如果查询终止，无法继续查询，则清理指示
            ShowNotice(null);
        }


        private void QueryEngine_GetResult(Plugin.QueryListResult result)
        {
            if (result == null || !result.Results.Any())
            {
                //如果没有返回结果
                OnQueryStateChanged(QueryState.QueryGotNoResult);

                ShowNotice(@"notice_NoResult".GetTranslation());

                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                {
                    SearchResultViewModel.Clear();
                }));
            }
            else
            {
                //如果返回了结果


                if (result.Results.Count > 1)
                {
                    //大于1条记录，则需要进行选择模式
                    ShowNotice(@"notice_EnterChooseMode".GetTranslation());
                    OnQueryStateChanged(QueryState.QueryGotManyResult);
                }
                else
                {
                    //只返回了一条记录
                    OnQueryStateChanged(QueryState.QueryGotOneResult);
                }

                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                {
                    SearchResultViewModel.SetResults(result);
                }));
            }

        }


        /// <summary>
        /// 显示提示
        /// </summary>
        /// <param name="noticeInfo"></param>
        private void ShowNotice(string noticeInfo)
        {
            InfoTextBlock = noticeInfo;
        }

        #endregion

    }

    /// <summary>
    /// 目前点击按键用作什么模式
    /// </summary>
    public enum KeyInputMode
    {
        /// <summary>
        /// 输入模式
        /// </summary>
        Input,
        /// <summary>
        /// 选择模式
        /// </summary>
        Select

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

    public enum ResultMode
    {
        ListOnly, ListDetail, DetailOnly
    }
}