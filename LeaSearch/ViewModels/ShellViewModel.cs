using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using LeaSearch.Common.Env;
using LeaSearch.Common.ViewModel;
using LeaSearch.Common.Windows.Input;
using LeaSearch.Core.Ioc;
using LeaSearch.Core.QueryEngine;
using LeaSearch.Plugin;

namespace LeaSearch.ViewModels
{
    public class ShellViewModel : BaseViewModel
    {
        private string _queryText;
        private Visibility _resultVisibility = Visibility.Collapsed;

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

        public ICommand OpenResultCommand1 { get; set; }



        #endregion

        #region Private Fields

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
        public SuggestionResults SuggestionResults { get; private set; } = new SuggestionResults();

        /// <summary>
        /// search result
        /// </summary>
        public SearchResults SearchResults { get; private set; } = new SearchResults();

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
                        SuggestionResults.SetPlugins(result.Keys.ToList());
                        ResultVisibility = Visibility.Visible;
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
                SuggestionResults.Clear();
                SearchResults.Clear();
            }
        }

        #endregion

    }

    public class SearchResults
    {
        public void Clear()
        {

        }
    }

    public class SuggestionResults
    {
        public ObservableCollection<Core.Plugin.Plugin> Plugins { get; set; } = new ObservableCollection<Core.Plugin.Plugin>();


        public void SetPlugins(List<Core.Plugin.Plugin> plugins)
        {
            Plugins.Clear();
            if (plugins.Any())
            {
                plugins.ForEach(p =>
                {
                    Plugins.Add(p);
                });
            }

        }

        public void Clear()
        {

        }
    }
}