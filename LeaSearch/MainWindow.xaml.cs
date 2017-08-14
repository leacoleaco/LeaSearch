using LeaSearch.Infrastructure.Helper;
using System;
using System.Collections.Generic;
using System.Windows;
using LeaSearch.ViewModel;
using LeaSearch.Common.Env;

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

            //we need to focus textbox when started
            QueryTextBox.Focus();
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
