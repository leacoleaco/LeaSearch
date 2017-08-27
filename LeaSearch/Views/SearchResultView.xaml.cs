using System.Windows;
using System.Windows.Controls;
using GalaSoft.MvvmLight.Messaging;
using LeaSearch.Common.Messages;
using LeaSearch.Core.MessageModels;

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


            //如果指示显示loading，则显示，否则取消
            Messenger.Default.Register<DetailLoaddingDisplayMessage>(this, (m) =>
            {

                if (m.IsShow)
                {
                    this.MoreInfoLoadding.Visibility = Visibility.Visible;

                    if (this.MoreInfoPanel != null)
                    {
                        UpdateInfoPanelPosition(true);
                    }
                }
                else
                {

                    this.MoreInfoLoadding.Visibility = Visibility.Collapsed;
                }
            });


            //设置更多信息
            Messenger.Default.Register<SetMoreInfoContentMessage>(this, m =>
            {
                this.MoreInfoInfoContent.Content = m.MoreInfoContent;

                //如果更多信息不为空，则显示，否则隐藏
                UpdateInfoPanelPosition(m.MoreInfoContent != null);
            });
        }

    

        /// <summary>
        /// 调整 moreinfo 的显示位置，已经显示状态
        /// </summary>
        /// <param name="isShowMoreInfoPanel">true显示   false 不显示 </param>
        private void UpdateInfoPanelPosition(bool isShowMoreInfoPanel)
        {

            var infoPanel = this.MoreInfoPanel;

            if (infoPanel == null) return;



            if (this.ListBox.Items.Count <= 4)
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


                FirstRow.Height = new GridLength(1, GridUnitType.Star);


                if (isShowMoreInfoPanel)
                {
                    SecondRow.Height = new GridLength(1, GridUnitType.Auto);
                    infoPanel.Visibility = Visibility.Visible;
                }
                else
                {
                    SecondRow.Height = new GridLength(0);
                    infoPanel.Visibility = Visibility.Collapsed;
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

                FirstCol.Width = new GridLength(1, GridUnitType.Star);

                if (isShowMoreInfoPanel)
                {
                    SecondCol.Width = new GridLength(1, GridUnitType.Auto);
                    infoPanel.Visibility = Visibility.Visible;
                }
                else
                {
                    SecondCol.Width = new GridLength(0);
                    infoPanel.Visibility = Visibility.Collapsed;
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


    }
}
