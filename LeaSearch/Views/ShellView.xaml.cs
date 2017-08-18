﻿using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using LeaSearch.Common.Env;
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
        }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);

            Ioc.Reslove<HotKeyManager>().WakeUpCommand += Instance_WakeUpCommand;

            Ioc.Reslove<ShellViewModel>().ResultModeChanged += ShellView_ResultModeChanged;

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
                    PluginCol.Width =GridLength.Auto; 
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
            //TODO: help website
            //Process.Start("http://doc.getwox.com");
        }

        private void EscCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }

        private void SelectPrevPageCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            throw new NotImplementedException();
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

        }
    }


}
