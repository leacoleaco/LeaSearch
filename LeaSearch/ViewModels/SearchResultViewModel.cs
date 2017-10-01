using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Threading;
using LeaSearch.Common.Env;
using LeaSearch.Common.Messages;
using LeaSearch.Common.ViewModel;
using LeaSearch.Core.Ioc;
using LeaSearch.Core.MessageModels;
using LeaSearch.Core.Notice;
using LeaSearch.Core.QueryEngine;
using LeaSearch.Plugin;
using LeaSearch.Plugin.DetailInfos;
using LeaSearch.Plugin.Query;

namespace LeaSearch.ViewModels
{
    public class SearchResultViewModel : BaseViewModel
    {
        private ResultItem _currentItem;
        private int _currentIndex;
        private readonly SharedContext _sharedContext;
        private QueryListResult _queryListResult;
        private object _moreInfoContent;
        private QueryEngine _queryEngine;
        /// <summary>
        /// 指示是否正在预览中
        /// </summary>
        private bool _isPreviewing;

        public SearchResultViewModel(Settings settings, SharedContext sharedContext, QueryEngine queryEngine) : base(settings)
        {
            _sharedContext = sharedContext;
            _queryEngine = queryEngine;


            _queryEngine.PluginCallActive += queryEngine_PluginCallActive;
            _queryEngine.GetResult += _queryEngine_GetResult;
            _queryEngine.GetDetailResult += queryEngine_GetDetailResult;
            _queryEngine.QueryEnd += EngineQueryEnd;
        }

        private void EngineQueryEnd()
        {
        }

        private void queryEngine_PluginCallActive(Core.Plugin.Plugin plugin, PluginCalledArg arg)
        {
            //if (arg.MoreInfo != null)
            //{
            //    ShowMoreInfo(arg.MoreInfo);
            //}
            //else
            //{
            //    ClearMoreInfo();
            //}

            Clear();
        }

        private void _queryEngine_GetResult(QueryListResult result)
        {
            if (result == null)
            {
                //如果没有返回结果,则清除列表
                Clear();
                return;
            }

            //如果有更多信息，则替换或者弹出，但是不清理，因为在激活的插件模式的时候可能有信息已经返回
            if (result.MoreInfo != null)
            {
                ShowMoreInfo(result.MoreInfo);
            }


            //如果有错误信息，则弹出
            if (!string.IsNullOrWhiteSpace(result?.ErrorMessage))
            {
                UiNoticeHelper.ShowErrorNotice(result.ErrorMessage);
            }
            else
            {
                UiNoticeHelper.ClearErrorNotice();
            }


            SetResults(result);



        }

        private void queryEngine_GetDetailResult(QueryDetailResult result)
        {
            var resultMoreInfo = result.MoreInfo;
            if (resultMoreInfo != null)
            {
                ShowMoreInfo(resultMoreInfo);
            }


            Messenger.Default.Send(new DetailLoaddingDisplayMessage() { IsShow = false });
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

        #region Command

        public ICommand SelectNextItemCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    SelectNext();
                    if (_isPreviewing)
                    {
                        QueryDetail();
                    }
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
                    if (_isPreviewing)
                    {
                        QueryDetail();
                    }
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


        public ICommand PreviewCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    if (_isPreviewing)
                    {
                        ClearMoreInfo();
                    }
                    else
                    {
                        QueryDetail();
                    }
                });
            }
        }

        #endregion



        /// <summary>
        /// 设置查询结果
        /// </summary>
        /// <param name="queryListResult"></param>
        public void SetResults(QueryListResult queryListResult)
        {
            _queryListResult = queryListResult;

            if (queryListResult == null) return;

            //如果有默认信息，则显示信息,否则清理信息
            if (queryListResult.MoreInfo != null)
            {
                ShowMoreInfo(queryListResult.MoreInfo);
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


        /// <summary>
        /// 清理列表
        /// </summary>
        public void Clear()
        {
            Results.Clear();
        }



        /// <summary>
        /// 选中第一个
        /// </summary>
        public void SelectFirst()
        {
            CurrentIndex = 0;
        }

        /// <summary>
        /// 选择下一个
        /// </summary>
        /// <param name="stepSize"></param>
        public void SelectNext(int stepSize = 1)
        {
            CurrentIndex = NewIndex(CurrentIndex + stepSize);
        }

        /// <summary>
        /// 选择上一个
        /// </summary>
        /// <param name="stepSize"></param>
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

        /// <summary>
        ///执行了打开结果后的处理
        /// </summary>
        /// <param name="state"></param>
        protected void OnAfterOpenResultCommand(StateAfterCommandInvoke state)
        {
            if (state == null) return;

            if (!state.ShowProgram)
            {
                //如果指示隐藏主界面，则发送隐藏消息
                Messenger.Default.Send(new ShellDisplayMessage() { Display = Display.Hide });
            }


        }


        /// <summary>
        /// 调用对应插件查询详情
        /// </summary>
        private void QueryDetail()
        {
            DispatcherHelper.CheckBeginInvokeOnUI(new Action(() =>
            {
                Messenger.Default.Send(new DetailLoaddingDisplayMessage() { IsShow = true });
            }));
            _queryEngine.QueryDetail(CurrentItem);

        }

        public void ClearMoreInfo()
        {
            DispatcherHelper.CheckBeginInvokeOnUI(new Action(() =>
            {
                Messenger.Default.Send(new SetMoreInfoContentMessage() { MoreInfoContent = null });

                _isPreviewing = false;
            }));
        }

        /// <summary>
        /// 自动根据不同的 内容设置不同信息
        /// </summary>
        /// <param name="contentInfo"></param>
        private void ShowMoreInfo(IInfo contentInfo)
        {
            if (contentInfo == null)
            {
                ClearMoreInfo();
                return;
            }

            DispatcherHelper.CheckBeginInvokeOnUI(new Action(() =>
            {
                Messenger.Default.Send(new SetMoreInfoContentMessage() { MoreInfoContent = contentInfo });
                _isPreviewing = true;
            }));

        }



    }


}