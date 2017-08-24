using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Threading;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Threading;
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
        private object _moreInfoContent;

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

            //如果有默认信息，则显示信息,否则清理信息
            if (queryListResult.DefaultInfo != null)
            {
                SetMoreInfo(queryListResult.DefaultInfo);
            }
            else
            {
                ClearMoreInfo();
            }

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

        /// <summary>
        /// 用于给出一些提示和建议
        /// </summary>
        public object MoreInfoContent
        {
            get { return _moreInfoContent; }
            set
            {
                _moreInfoContent = value;
                RaisePropertyChanged();
            }
        }

        private void ClearMoreInfo()
        {
            MoreInfoContent = null;

        }

        /// <summary>
        /// 自动根据不同的 内容设置不同信息
        /// </summary>
        /// <param name="contentInfo"></param>
        private void SetMoreInfo(IInfo contentInfo)
        {
            if (contentInfo == null) return;


            DispatcherHelper.CheckBeginInvokeOnUI(new Action(() =>
            ////TODO: check if is useful
            //Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Render, new Action(() =>
            {
                if (contentInfo is TextInfo)
                {
                    var ct = contentInfo as TextInfo;
                    MoreInfoContent = new TextBlock() { Text = ct.Text };
                }
                else if (contentInfo is FlowDocumentInfo)
                {
                    //MoreInfoContent=new FlowDocument(){Blocks = { blo}};
                    MoreInfoContent = new FlowDocument();
                }
            }));

        }


    }


}