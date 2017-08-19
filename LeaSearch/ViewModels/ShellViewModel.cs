using System;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using LeaSearch.Common.Env;
using LeaSearch.Common.ViewModel;
using LeaSearch.Core.Image;
using LeaSearch.Core.Ioc;
using LeaSearch.Core.QueryEngine;
using LeaSearch.Infrastructure.Dispatcher;

namespace LeaSearch.ViewModels
{
    public class ShellViewModel : BaseViewModel
    {
        #region Private Fields

        private string _queryText;
        private Visibility _resultVisibility = Visibility.Collapsed;
        private ResultMode _resultMode = ResultMode.ListOnly;
        private Core.Plugin.Plugin _currentSearchPlugin;

        #endregion

        public ShellViewModel(Settings settings, SuggestionResultViewModel suggestionResultViewModel, SearchResultViewModel searchResultViewModel, DetailResultViewModel detailResultViewModel) : base(settings)
        {
            SuggestionResultViewModel = suggestionResultViewModel;
            SearchResultViewModel = searchResultViewModel;
            DetailResultViewModel = detailResultViewModel;


            Ioc.Reslove<QueryEngine>().GetSuitablePlugins += QueryEngine_GetSuitablePlugins;
            Ioc.Reslove<QueryEngine>().GetResult += QueryEngine_GetResult;
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
        /// to show which plugin is frist to search
        /// </summary>
        public ImageSource CurrentSearchPluginImage { get; private set; } = Ioc.Reslove<ImageManager>().GetDefaultIcon();

        public Core.Plugin.Plugin CurrentSearchPlugin
        {
            get { return _currentSearchPlugin; }
            set
            {
                _currentSearchPlugin = value;

                if (value != null)
                {
                    CurrentSearchPluginImage = Ioc.Reslove<ImageManager>().GetImageSource(value.PluginIconPath);
                }
                else
                {
                    CurrentSearchPluginImage = Ioc.Reslove<ImageManager>().GetDefaultIcon();
                }
                OnPropertyChanged("CurrentSearchPluginImage");
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


        private void QueryEngine_GetSuitablePlugins(Core.Plugin.Plugin[] plugins)
        {
            DispatcherHelper.BeginInvoke(new Action(() =>
            {
                if (plugins != null && plugins.Any())
                {
                    CurrentSearchPlugin = plugins[0];
                    SuggestionResultViewModel.SetPlugins(plugins);

                }
                else
                {
                    CurrentSearchPlugin = null;
                }
            }));
        }

        private void QueryEngine_GetResult(Plugin.QueryListResult result)
        {
            if (result.Results.Any())
            {
                OnQueryStateChanged(QueryState.QueryGotResult);

                DispatcherHelper.BeginInvoke(new Action(() =>
                {
                    SearchResultViewModel.SetResults(result.Results);
                }));
            }
            else
            {
                OnQueryStateChanged(QueryState.QueryGotNoResult);
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