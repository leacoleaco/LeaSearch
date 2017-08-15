using LeaSearch.Infrastructure.Helper;
using System;
using System.Collections.Generic;
using System.Windows;
using LeaSearch.ViewModel;
using LeaSearch.Common.Env;
using LeaSearch.Core.HotKey;
using LeaSearch.Core.I18N;
using LeaSearch.Core.Notice;

namespace LeaSearch
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private MainViewModel _mainViewModel;
        private Settings _settings;


        public MainWindow(Settings settings, MainViewModel mainViewModel)
        {
            _settings = settings;
            _mainViewModel = mainViewModel;
            InitializeComponent();
        }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);

            HotKeyManager.Instance.OnHotKeyWakeUp += Instance_OnHotKeyWakeUp;

            //we need to focus textbox when startup
            QueryTextBox.Focus();
        }

        private void Instance_OnHotKeyWakeUp()
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



    }
}
