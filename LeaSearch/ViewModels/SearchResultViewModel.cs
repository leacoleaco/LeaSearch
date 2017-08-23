using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;
using LeaSearch.Common.Env;
using LeaSearch.Common.Messages;
using LeaSearch.Common.ViewModel;
using LeaSearch.Core.Ioc;
using LeaSearch.Plugin;
using Microsoft.Expression.Interactivity.Core;

namespace LeaSearch.ViewModels
{
    public class SearchResultViewModel : BaseViewModel
    {
        private ResultItem _currentItem;
        private int _currentIndex;
        private readonly SharedContext _sharedContext;
        private QueryListResult _queryListResult;

        public SearchResultViewModel(Settings settings, SharedContext sharedContext) : base(settings)
        {
            _sharedContext = sharedContext;



        }

        public int CurrentIndex
        {
            get { return _currentIndex; }
            set
            {
                _currentIndex = value;
                RaisePropertyChanged();
            }
        }

        public ResultItem CurrentItem
        {
            get { return _currentItem; }
            set
            {
                _currentItem = value;
                RaisePropertyChanged();
            }
        }

        public ObservableCollection<ResultItem> Results { get; } = new ObservableCollection<ResultItem>();


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

        /// <summary>
        /// 设置查询结果
        /// </summary>
        /// <param name="queryListResult"></param>
        public void SetResults(QueryListResult queryListResult)
        {
            _queryListResult = queryListResult;

            if (queryListResult == null) return;

            Results.Clear();
            foreach (var resultItem in queryListResult.Results)
            {
                Results.Add(resultItem);
            }
        }

        public void Clear()
        {
            Results.Clear();
        }

        public void SelectFirst()
        {
            CurrentIndex = 0;
        }

        public void SelectNext(int stepSize = 1)
        {
            CurrentIndex = NewIndex(CurrentIndex + stepSize);
        }

        public void SelectPrev(int stepSize = 1)
        {
            CurrentIndex = NewIndex(CurrentIndex - stepSize);
        }


        /// <summary>
        ///打开查询结果
        /// </summary>
        public void OpenResult()
        {
            if (!Results.Any()) return;

            if (Results.Count == 1) CurrentItem = Results[0];

            if (CurrentItem == null) return;

            //当点击了获取结果的按钮后
            //按照不同的结果执行不同的操作方案
            var r = CurrentItem.SelectedAction?.Invoke(_sharedContext);
            if (r != null)
            {
                OnAfterOpenResultCommand(r);
            }

            //执行全局的 selectedAction，仅适用于 plugin call 模式
            if (CurrentItem != null)
            {
                var r1 = _queryListResult?.SelectedAction?.Invoke(_sharedContext, CurrentItem);
                if (r1 != null)
                {
                    OnAfterOpenResultCommand(r1);
                }
            }
        }

        protected void OnAfterOpenResultCommand(StateAfterCommandInvoke state)
        {
            if (state == null) return;

            if (!state.ShowProgram)
                //如果指示隐藏主界面，则发送隐藏消息
                Messenger.Default.Send(new ShellDisplayMessage() { Display = Display.Hide });

        }


        #region Command

        public ICommand SelectNextItemCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    SelectNext();
                });
            }
        }

        public ICommand SelectPrevItemCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    SelectPrev();
                });
            }
        }

        public ICommand OpenResultCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    OpenResult();
                });
            }
        }

        public ICommand EscCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    CurrentIndex = -1;
                    Messenger.Default.Send(new FocusMessage() { FocusTarget = FocusTarget.QueryTextBox });
                    Ioc.Reslove<ShellViewModel>().ShowNotice(null);
                });
            }
        }

        #endregion

    }


}