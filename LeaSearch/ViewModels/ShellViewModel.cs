using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using LeaSearch.Common.Env;
using LeaSearch.Common.ViewModel;
using LeaSearch.Common.Windows.Input;
using LeaSearch.Core.Ioc;
using LeaSearch.Core.QueryEngine;
using LeaSearch.Infrastructure.Dispatcher;
using LeaSearch.Plugin;

namespace LeaSearch.ViewModels
{
    public class ShellViewModel : BaseViewModel
    {
        #region Private Fields

        private string _queryText;
        private Visibility _resultVisibility = Visibility.Collapsed;
        private ResultMode _resultMode = ResultMode.ListOnly;

        #endregion

        public ShellViewModel(Settings settings, SuggestionResultViewModel suggestionResultViewModel, SearchResultViewModel searchResultViewModel, DetailResultViewModel detailResultViewModel) : base(settings)
        {
            SuggestionResultViewModel = suggestionResultViewModel;
            SearchResultViewModel = searchResultViewModel;
            DetailResultViewModel = detailResultViewModel;
        }

        //use action is just to convenient and decoupe 
        #region Hotkey Action Event


        #endregion



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
            QueryStateChanged?.Invoke(queryState);
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
                Ioc.Reslove<QueryEngine>().Query(QueryText, result =>
                {

                    if (result.Any())
                    {
                        OnQueryStateChanged(QueryState.QueryGotResult);

                        var plugins = result.Keys.ToList();
                        DispatcherHelper.BeginInvoke(new Action(() =>
                        {
                            SuggestionResultViewModel.SetPlugins(plugins);
                            SearchResultViewModel.SetResults(result[plugins[0]].Results);
                        }));
                    }
                    else
                    {
                        OnQueryStateChanged(QueryState.QueryGotNoResult);
                    }

                });
            }
            else
            {
                //if no result ,then close search
                OnQueryStateChanged(QueryState.QueryGotNoResult);
                SuggestionResultViewModel.Clear();
                SearchResultViewModel.Clear();
            }
        }

        #endregion

      
    }

    public enum QueryState
    {
        StartQuery, QueryGotResult, QueryGotNoResult

    }

    public enum ResultMode
    {
        ListOnly, ListDetail, DetailOnly
    }
}