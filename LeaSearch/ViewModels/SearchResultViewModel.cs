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
using LeaSearch.Core.I18N;
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
            _queryEngine.GetListResult += queryEngine_GetListResult;
            _queryEngine.GetDetailResult += _queryEngine_GetDetailResult;
            _queryEngine.GetItemDetailResult += QueryEngineGetItemDetailResult;

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

            ClearListResult();
            ClearDetailResult();
        }

        private void queryEngine_GetListResult(QueryListResult result)
        {

            ClearDetailResult();
            ClearListResult();
            _listResult = result;
            if (result == null)
            {
                return;
            }

            //如果有更多信息，则替换或者弹出，但是不清理，因为在激活的插件模式的时候可能有信息已经返回
            if (result.MoreInfo != null)
            {
                SetDetailInfo(result.MoreInfo);
            }

            foreach (var resultItem in result.Results)
            {
                Results.Add(resultItem);
            }

            SetErrorNotice(result?.ErrorMessage);
        }



        private void _queryEngine_GetDetailResult(QueryDetailResult result)
        {
            ClearListResult();
            ClearDetailResult();
            _detailResult = result;
            if (result == null)
            {
                return;
            }
            SetDetailInfo(result.Result);
            SetErrorNotice(result?.ErrorMessage);

        }

        private void QueryEngineGetItemDetailResult(QueryItemDetailResult result)
        {
            var resultMoreInfo = result.DetailResult;
            if (resultMoreInfo != null)
            {
                SetDetailInfo(resultMoreInfo);
            }


            Messenger.Default.Send(new DetailLoaddingDisplayMessage() { IsShow = false });
        }



        #region ListResultMode
        private QueryListResult _listResult;

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

        private void OpenListResult()
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
                var r1 = _listResult?.SelectAction?.Invoke(_sharedContext, CurrentItem);
                if (r1 != null)
                {
                    OnAfterOpenResultCommand(r1);
                }
            }

        }

        #endregion

        #region DetailResultMode

        private QueryDetailResult _detailResult;
        private void OpenDetailResult()
        {
            if (_detailResult?.Result == null) return;

            var r1 = _detailResult.EnterAction?.Invoke(_sharedContext, _detailResult.Result);
            if (r1 != null)
            {
                OnAfterOpenResultCommand(r1);
            }
        }

        #endregion

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
                    EnterResult();
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
                        ClearDetailResult();
                    }
                    else
                    {
                        QueryDetail();
                    }
                });
            }
        }

        #endregion

        public void EnterResult()
        {
            if (_queryEngine.CurrentResultMode == ResultMode.List)
            {
                if (Results == null) return;
                var resultsCount = Results?.Count;
                if (resultsCount == 1)
                {
                    OpenListResult();
                }
                else if (resultsCount > 1)
                {
                    UiNoticeHelper.ShowInfoNotice(@"notice_SelectMode".GetTranslation());
                    Messenger.Default.Send<FocusMessage>(new FocusMessage() { FocusTarget = FocusTarget.ResultList });
                    SelectFirst();
                }
            }
            else if (_queryEngine.CurrentResultMode == ResultMode.Detail)
            {
                OpenDetailResult();
            }
        }


        /// <summary>
        /// 清理列表
        /// </summary>
        public void ClearListResult()
        {
            Results.Clear();
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

        public void ClearDetailResult()
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
        private void SetDetailInfo(IInfo contentInfo)
        {
            if (contentInfo == null)
            {
                ClearDetailResult();
                return;
            }

            DispatcherHelper.CheckBeginInvokeOnUI(new Action(() =>
            {
                Messenger.Default.Send(new SetMoreInfoContentMessage() { MoreInfoContent = contentInfo });
                _isPreviewing = true;
            }));

        }


        /// <summary>
        /// 设置错误信息，如果为null，则清除错误信息
        /// </summary>
        /// <param name="errorMsg"></param>
        private void SetErrorNotice(string errorMsg)
        {
            //如果有错误信息，则弹出
            if (!string.IsNullOrWhiteSpace(errorMsg))
            {
                UiNoticeHelper.ShowErrorNotice(errorMsg);
            }
            else
            {
                UiNoticeHelper.ClearErrorNotice();
            }
        }

    }


}