﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Threading;
using LeaSearch.Common.Env;
using LeaSearch.Common.ViewModel;
using LeaSearch.Plugin;

namespace LeaSearch.ViewModels
{
    public class SearchResultViewModel : BaseViewModel
    {
        private ResultItem _currentItem;
        private int _currentIndex;

        public SearchResultViewModel(Settings settings) : base(settings)
        {
        }

        public int CurrentIndex
        {
            get { return _currentIndex; }
            set
            {
                _currentIndex = value;
                OnPropertyChanged();
            }
        }

        public ResultItem CurrentItem
        {
            get { return _currentItem; }
            set
            {
                _currentItem = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<ResultItem> Results { get; } = new ObservableCollection<ResultItem>();



        public void SetResults(ResultItem[] list)
        {

            Results.Clear();
            foreach (var resultItem in list)
            {
                Results.Add(resultItem);
            }

        }

        public void Clear()
        {
            Results.Clear();
        }

        private int NewIndex(int i)
        {
            var n = Results.Count;
            if (n > 0)
            {
                i = (n + i) % n;
                return i;
            }
            else
            {
                // SelectedIndex returns -1 if selection is empty.
                return -1;
            }
        }

        public void MoveNext(int pageSize = 1)
        {
            CurrentIndex = NewIndex(CurrentIndex + pageSize);
        }

        public void MovePrev(int pageSize = 1)
        {
            CurrentIndex = NewIndex(CurrentIndex - pageSize);
        }

    }
}