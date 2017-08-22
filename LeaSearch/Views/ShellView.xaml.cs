using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using LeaSearch.Common.Env;
using LeaSearch.Common.View;
using LeaSearch.Core.HotKey;
using LeaSearch.Core.Ioc;
using LeaSearch.Core.QueryEngine;
using LeaSearch.Infrastructure.Helper;
using LeaSearch.Plugin;
using LeaSearch.ViewModels;

namespace LeaSearch.Views
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class ShellView : Window, IView
    {
        private Settings _settings;


        public ShellView(Settings settings)
        {
            _settings = settings;
            InitializeComponent();
        }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);

#if !DEBUG
            this.ShowInTaskbar = false;
            this.Topmost = true;
#endif

            //Global wake up hotkey
            Ioc.Reslove<HotKeyManager>().WakeUpCommand += Instance_WakeUpCommand;

            var shellViewModel = Ioc.Reslove<ShellViewModel>();
            shellViewModel.ResultModeChanged += ShellView_ResultModeChanged;
            shellViewModel.QueryStateChanged += ShellViewModel_QueryStateChanged;


            if (_settings.HideOnStartup)
            {
                this.Hide();
            }
            else
            {
                this.Show();
                //we need to focus textbox when startup
                QueryTextBox.Focus();
            }

        }




        /// <summary>
        /// change the result mode
        /// </summary>
        /// <param name="resultMode"></param>
        private void ShellView_ResultModeChanged(ResultMode resultMode)
        {
            switch (resultMode)
            {
                case ResultMode.ListOnly:
                    PluginCol.Width = GridLength.Auto;
                    ResultCol.Width = new GridLength(1, GridUnitType.Star);
                    DetailCol.Width = new GridLength(0, GridUnitType.Pixel); ;
                    break;
                case ResultMode.ListDetail:
                    PluginCol.Width = new GridLength(0, GridUnitType.Pixel);
                    ResultCol.Width = new GridLength(4, GridUnitType.Star);
                    DetailCol.Width = new GridLength(6, GridUnitType.Star);
                    break;
                case ResultMode.DetailOnly:
                    PluginCol.Width = new GridLength(0, GridUnitType.Pixel);
                    ResultCol.Width = new GridLength(0, GridUnitType.Pixel);
                    DetailCol.Width = new GridLength(1, GridUnitType.Star);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(resultMode), resultMode, null);
            }
        }


        private void ShellViewModel_QueryStateChanged(QueryState queryState)
        {
            this.Dispatcher.BeginInvoke(new Action(() =>
                       {

                           switch (queryState)
                           {
                               case QueryState.StartQuery:

                                   ProgressBar.Visibility = Visibility.Hidden;
                                   break;
                               case QueryState.QuerySuitNoPlugin:
                                   //PluginCol.Width = new GridLength(0, GridUnitType.Pixel);
                                   ResultGrid.Visibility = Visibility.Collapsed;
                                   break;
                               case QueryState.QuerySuitOnePlugin:
                                   PluginCol.Width = new GridLength(0, GridUnitType.Pixel);
                                   ProgressBar.Visibility = Visibility.Hidden;
                                   ResultGrid.Visibility = Visibility.Collapsed;
                                   break;
                               case QueryState.QuerySuitManyPlugin:
                                   //PluginCol.Width = new GridLength(0, GridUnitType.Pixel);
                                   ProgressBar.Visibility = Visibility.Hidden;
                                   ResultGrid.Visibility = Visibility.Collapsed;
                                   break;
                               case QueryState.BeginPluginSearch:
                                   ProgressBar.Visibility = Visibility.Visible;
                                   break;
                               case QueryState.QueryGotResult:
                                   ProgressBar.Visibility = Visibility.Hidden;
                                   ResultGrid.Visibility = Visibility.Visible;
                                   break;
                               case QueryState.QueryGotNoResult:
                                   ProgressBar.Visibility = Visibility.Hidden;
                                   ResultGrid.Visibility = Visibility.Collapsed;
                                   break;

                               default:
                                   throw new ArgumentOutOfRangeException(nameof(queryState), queryState, null);
                           }
                       }));
        }

        private void Instance_WakeUpCommand()
        {

            //if ignore hotkey on fullscreen, 
            //double if to omit calling win32 function
            if (_settings.IgnoreHotkeysOnFullscreen && WindowsInteropHelper.IsWindowFullscreen())
                return;

            if (this.IsVisible)
            {
                this.Hide();
            }
            else
            {
                this.Show();
                //we need to focus textbox when wake up
                //why use dispatcher:
                //http://blog.csdn.net/xuewen880926/article/details/6910561
                Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Render,new Action(() => QueryTextBox.Focus()));
            }
        }


        /// <summary>
        /// make windows just in perfect position
        /// </summary>
        private void InitializeWindowPosition()
        {
            Top = this.GetCenterScreenTop(QueryTextBox.ActualHeight);
            Left = this.GetCenterScreenLeft(this.ActualWidth);
        }

        private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            InitializeWindowPosition();
        }


        private void StartHelpCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            //TODO: help website
            //Process.Start("http://doc.getwox.com");
        }

        private void EscCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            this.Hide();
            QueryTextBox.Text = string.Empty;
        }

        private void SelectNextItemCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Ioc.Reslove<SearchResultViewModel>().MoveNext();
        }

        private void SelectPrevItemCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Ioc.Reslove<SearchResultViewModel>().MovePrev();
        }

        private void SelectNextPageCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {

            Ioc.Reslove<SearchResultViewModel>().MoveNext(8);
        }

        private void SelectPrevPageCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Ioc.Reslove<SearchResultViewModel>().MovePrev(8);
        }

        private void LoadContextMenuCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void LoadHistoryCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void OpenResultCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {

            //当点击了获取结果的按钮后
            //根据是否执行命令后回调结果是 true 还是 false，来显示、隐藏输入框
            SharedContext sharedContext = Ioc.Reslove<SharedContext>();
            var r = Ioc.Reslove<SearchResultViewModel>().CurrentItem?.SelectedAction?.Invoke(sharedContext);
            if (r != null && r.Value)
            {
                this.Hide();
                this.QueryTextBox.Clear();
            }
        }
    }


}
