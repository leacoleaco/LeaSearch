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
            Ioc.Reslove<HotKeyManager>().WakeUpCommand += () =>
            {
                if (this.IsVisible)
                {
                    HideProgram();
                }
                else
                {
                    WakeUpProgram();
                }
            };

            var shellViewModel = Ioc.Reslove<ShellViewModel>();
            shellViewModel.ResultModeChanged += ShellView_ResultModeChanged;
            shellViewModel.QueryStateChanged += ShellViewModel_QueryStateChanged;
            shellViewModel.NotifyWakeUpProgram += WakeUpProgram;
            shellViewModel.NotifyHideProgram += HideProgram;


            var searchResultViewModel = Ioc.Reslove<SearchResultViewModel>();
            searchResultViewModel.AfterOpenResultCommand += (o) =>
            {
                //打开查询结果后，如果插件建议关闭主窗口，则隐藏窗口
                if (!o.ShowProgram)
                {
                    HideProgram();
                }
            };


            if (_settings.HideOnStartup)
            {
                HideProgram();
            }
            else
            {
                WakeUpProgram();
            }

        }


        private void HideProgram()
        {
            this.Hide();
            QueryTextBox.Text = string.Empty;
        }

        private void WakeUpProgram()
        {
            //if ignore hotkey on fullscreen, 
            //double if to omit calling win32 function
            if (_settings.IgnoreHotkeysOnFullscreen && WindowsInteropHelper.IsWindowFullscreen())
                return;

            this.Show();
            //we need to focus textbox when wake up
            //why use dispatcher:
            //http://blog.csdn.net/xuewen880926/article/details/6910561
            Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Render, new Action(() => QueryTextBox.Focus()));
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
                               case QueryState.QueryGotOneResult:
                               case QueryState.QueryGotManyResult:
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


    }
}