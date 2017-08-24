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
                    infoContent.SetValue(Grid.RowProperty, 1);
                    infoContent.SetValue(Grid.ColumnProperty, 0);
                    //SecondRow.Height=new GridLength(1,GridUnitType.Star);
                    //SecondCol.Width=new GridLength(1,GridUnitType.Auto);
                }
                else
                {
                    infoContent.SetValue(Grid.RowProperty, 0);
                    infoContent.SetValue(Grid.ColumnProperty, 1);
                    //SecondRow.Height=new GridLength(1,GridUnitType.Auto);
                    //SecondCol.Width=new GridLength(1,GridUnitType.Star);
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
