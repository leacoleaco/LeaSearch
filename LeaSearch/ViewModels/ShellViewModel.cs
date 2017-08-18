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
        private  ResultMode _resultMode = ResultMode.ListOnly;

        #endregion

        public ShellViewModel(Settings settings) : base(settings)
        {

        }

        //use action is just to convenient and decoupe 
        #region Hotkey Action Event

        public ICommand EscCommand { get; set; } = new ParameterCommand(_ =>
        {
            // Process.Start("http://www.baidu.com");
        });
        public ICommand SelectNextItemCommand { get; set; }
        public ICommand SelectPrevItemCommand { get; set; }
        public ICommand SelectNextPageCommand { get; set; }
        public ICommand SelectPrevPageCommand { get; set; }
        public ICommand StartHelpCommand { get; set; }
        public ICommand LoadContextMenuCommand { get; set; }
        public ICommand LoadHistoryCommand { get; set; }
        public ICommand OpenResultCommand { get; set; }

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
        public SuggestionResultViewModel SuggestionResultViewModel { get; private set; } = new SuggestionResultViewModel();

        /// <summary>
        /// search result
        /// </summary>
        public SearchResultViewModel SearchResultViewModel { get; private set; } = new SearchResultViewModel();

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
                OnPropertyChanged();
            }
        }

        #endregion


        #region QueryMethod


        /// <summary>
        /// do the query
        /// </summary>
        private void Query()
        {
            if (!string.IsNullOrEmpty(QueryText))
            {
                Ioc.Reslove<QueryEngine>().Query(QueryText, result =>
                {

                    if (result.Any())
                    {

                        ResultVisibility = Visibility.Visible;

                        var plugins = result.Keys.ToList();
                        DispatcherHelper.BeginInvoke(new Action(() =>
                        {
                            SuggestionResultViewModel.SetPlugins(plugins);
                            SearchResultViewModel.SetResults(result[plugins[0]].Results);
                        }));
                    }
                    else
                    {
                        ResultVisibility = Visibility.Collapsed;
                    }

                });
            }
            else
            {
                //if no result ,then close search
                ResultVisibility = Visibility.Collapsed;
                SuggestionResultViewModel.Clear();
                SearchResultViewModel.Clear();
            }
        }

        #endregion

    }

    public enum ResultMode
    {
        ListOnly, ListDetail, DetailOnly
    }
}