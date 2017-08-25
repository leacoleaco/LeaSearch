﻿using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Threading;
using LeaSearch.Common.Messages;
using LeaSearch.UI.Controls;
using LeaSearch.UI.UserControls;

namespace LeaSearch.Views
{
    /// <summary>
    /// SearchResultListView.xaml 的交互逻辑
    /// </summary>
    public partial class SearchResultView : UserControl
    {

        public SearchResultView()
        {
            InitializeComponent();

            this.ListBox.SizeChanged += ListBox_SizeChanged
                ;

            //如果指示显示loading，则显示，否则取消
            Messenger.Default.Register<DetailLoaddingDisplayMessage>(this, (m) =>
            {

                if (m.IsShow)
                {
                    this.MoreInfoLoadding.Visibility = Visibility.Visible;

                    if (this.MoreInfoPanel != null)
                    {
                        this.MoreInfoPanel.Visibility = Visibility.Visible;
                        UpdateInfoPanelPosition();
                    }
                }
                else
                {

                    this.MoreInfoLoadding.Visibility = Visibility.Collapsed;
                }
            });
        }

        private void ListBox_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (e.HeightChanged)
            {
                UpdateInfoPanelPosition();

            }

        }

        private void UpdateInfoPanelPosition()
        {

            var infoPanel = this.MoreInfoPanel;

            if (infoPanel == null) return;
            if (this.ListBox.ActualHeight <= 300)
            {
                //上下模式
                infoPanel.SetValue(Grid.RowProperty, 1);
                infoPanel.SetValue(Grid.ColumnProperty, 0);
                this.GridSplitter.SetValue(Grid.RowSpanProperty, 1);
                this.GridSplitter.SetValue(Grid.ColumnSpanProperty, 2);
                //this.GridSplitter.SetValue(Grid.RowProperty, 1);
                //this.GridSplitter.SetValue(Grid.ColumnProperty, 0);
                this.GridSplitter.Width = double.NaN;
                this.GridSplitter.Height = 2;

                FirstRow.Height = new GridLength(2, GridUnitType.Star);
                if (infoPanel.IsVisible)
                {
                    SecondRow.Height = new GridLength(1, GridUnitType.Star);
                }
                else
                {
                    SecondRow.Height = new GridLength(0);
                }
                FirstCol.Width = new GridLength(1, GridUnitType.Star);
                SecondCol.Width = new GridLength(0);
            }
            else
            {
                //左右模式
                infoPanel.SetValue(Grid.RowProperty, 0);
                infoPanel.SetValue(Grid.ColumnProperty, 1);
                this.GridSplitter.SetValue(Grid.RowSpanProperty, 2);
                this.GridSplitter.SetValue(Grid.ColumnSpanProperty, 1);
                //this.GridSplitter.SetValue(Grid.RowProperty, 0);
                //this.GridSplitter.SetValue(Grid.ColumnProperty, 1);
                this.GridSplitter.Width = 2;
                this.GridSplitter.Height = double.NaN;

                FirstCol.Width = new GridLength(2, GridUnitType.Star);
                if (infoPanel.IsVisible)
                {
                    SecondCol.Width = new GridLength(1, GridUnitType.Star);
                }
                else
                {
                    SecondCol.Width = new GridLength(0);
                }
                FirstRow.Height = new GridLength(1, GridUnitType.Star);
                SecondRow.Height = new GridLength(0);
            }
        }

        private void ListBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0 && e.AddedItems[0] != null)
            {
                ListBox.ScrollIntoView(e.AddedItems[0]);
            }
        }

        public void SetFocus()
        {
            this.ListBox.Focus();
        }

        private void MoreInfoContent_OnDisplayContentChanged(object sender, DisplayContentChangedArgs args)
        {
            var infoPanel = this.MoreInfoPanel;

            if (infoPanel == null) return;

            if (args.NewValue == null)
            {
                infoPanel.Visibility = Visibility.Collapsed;
            }
            else
            {
                infoPanel.Visibility = Visibility.Visible;
            }
            UpdateInfoPanelPosition();
        }
    }
}
