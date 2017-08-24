using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using GalaSoft.MvvmLight.Messaging;
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
        }

        private void ListBox_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (e.HeightChanged)
            {
                var infoContent = this.MoreInfo_InfoContent as InfoContent;
                if (infoContent == null) return;
                if (this.ListBox.ActualHeight <= 300)
                {
                    //上下模式
                    infoContent.SetValue(Grid.RowProperty, 1);
                    infoContent.SetValue(Grid.ColumnProperty, 0);
                    this.GridSplitter.SetValue(Grid.RowSpanProperty,1);
                    this.GridSplitter.SetValue(Grid.ColumnSpanProperty,2);
                    //this.GridSplitter.SetValue(Grid.RowProperty, 1);
                    //this.GridSplitter.SetValue(Grid.ColumnProperty, 0);
                    this.GridSplitter.Width = double.NaN;
                    this.GridSplitter.Height = 2;

                    InfoRow.Height = new GridLength(1, GridUnitType.Star);
                    InfoCol.Width = new GridLength(1, GridUnitType.Auto);
                }
                else
                {
                    //左右模式
                    infoContent.SetValue(Grid.RowProperty, 0);
                    infoContent.SetValue(Grid.ColumnProperty, 1);
                    this.GridSplitter.SetValue(Grid.RowSpanProperty, 2);
                    this.GridSplitter.SetValue(Grid.ColumnSpanProperty, 1);
                    //this.GridSplitter.SetValue(Grid.RowProperty, 0);
                    //this.GridSplitter.SetValue(Grid.ColumnProperty, 1);
                    this.GridSplitter.Width = 2;
                    this.GridSplitter.Height = double.NaN;

                    InfoRow.Height = new GridLength(1, GridUnitType.Auto);
                    InfoCol.Width = new GridLength(1, GridUnitType.Star);
                }
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
            var infoContent = sender as InfoContent;

            if (infoContent == null) return;

            if (args.NewValue == null)
            {
                infoContent.Visibility = Visibility.Collapsed;
            }
            else
            {


                infoContent.Visibility = Visibility.Visible;
            }
        }
    }
}
