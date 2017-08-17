using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using LeaSearch.Common.Env;
using LeaSearch.Common.View;
using LeaSearch.Core.HotKey;
using LeaSearch.Core.Ioc;
using LeaSearch.Infrastructure.Helper;

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

            Ioc.Reslove<HotKeyManager>().WakeUpCommand += Instance_WakeUpCommand;

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

        private void Instance_WakeUpCommand()
        {
            if (this.IsVisible)
            {
                this.Hide();
            }
            else
            {
                this.Show();
                //we need to focus textbox when wake up
                QueryTextBox.Focus();
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
            Process.Start("http://doc.getwox.com");
        }
        private void StartHelpCommand1_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Process.Start("http://www.baidu.com");
        }
    }


}
