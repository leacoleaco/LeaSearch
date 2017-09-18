using System;
using System.Windows;
using System.Windows.Input;
using GalaSoft.MvvmLight.Messaging;
using LeaSearch.Common.Env;
using LeaSearch.Common.Messages;
using LeaSearch.Common.View;
using LeaSearch.Core.HotKey;
using LeaSearch.Core.Ioc;
using LeaSearch.Infrastructure.Helper;
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


            Messenger.Default.Register<QueryState>(this, ShellViewModel_QueryStateChanged);
            Messenger.Default.Register<FocusMessage>(this, o =>
            {
                switch (o.FocusTarget)
                {
                    case FocusTarget.QueryTextBox:
                        this.QueryTextBox.Focus();
                        break;
                    case FocusTarget.ResultList:
                        this.SearchResultListView.SetFocus();
                        break;
                }
            });
            Messenger.Default.Register<ShellDisplayMessage>(this, (m) =>
            {
                switch (m.Display)
                {
                    case Display.WakeUp:
                        WakeUpProgram();
                        break;
                    case Display.Hide:
                        HideProgram();
                        break;
                }
            });

            Messenger.Default.Register<NoticeMessage>(this, m =>
            {
                if (m == null)
                {
                    NoticePanel.ErrorText = null;
                    NoticePanel.InfoText = null;
                    return;
                }
                switch (m.NoticeType)
                {
                    case NoticeType.Info:
                        NoticePanel.InfoText = m.Message;
                        break;
                    case NoticeType.Error:
                        NoticePanel.ErrorText = m.Message;
                        break;
                }
            });

            this.Unloaded += (sender, e) => Messenger.Default.Unregister(this);
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





        private void ShellViewModel_QueryStateChanged(QueryState queryState)
        {
            this.Dispatcher.BeginInvoke(new Action(() =>
                       {

                           switch (queryState)
                           {
                               case QueryState.QueryStart:
                                   ProgressBar.Visibility = Visibility.Visible;
                                   break;
                               case QueryState.QueryEnd:
                                   ProgressBar.Visibility = Visibility.Hidden;
                                   break;
                               case QueryState.QueryError:
                                   ProgressBar.Visibility = Visibility.Hidden;
                                   break;
                               case QueryState.QueryStop:
                                   ProgressBar.Visibility = Visibility.Hidden;
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