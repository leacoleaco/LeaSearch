using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace LeaSearch.UI.UserControls
{
    /// <summary>
    /// HelpInfoPanel.xaml 的交互逻辑
    /// </summary>
    public partial class HelpInfoPanel : UserControl
    {

        public HelpInfoPanel()
        {
            //InitializeComponent();
        }


        public static readonly DependencyProperty InfoProperty = DependencyProperty.Register(
            "Info", typeof(FlowDocument), typeof(HelpInfoPanel), new PropertyMetadata(default(FlowDocument),OnInfoChanged));

        private static void OnInfoChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            
            var panel = d as HelpInfoPanel;
            panel?.SetVisibilty(panel, panel.Info, e.NewValue);

        }

        public FlowDocument Info
        {
            get { return (FlowDocument) GetValue(InfoProperty); }
            set { SetValue(InfoProperty, value); }
        }

        private void SetVisibilty(HelpInfoPanel panel, object errorText, object infoText)
        {
            var needInfoTextHide = string.IsNullOrWhiteSpace(infoText?.ToString());
            var needErrorTextHide = string.IsNullOrWhiteSpace(errorText?.ToString());
            if (needInfoTextHide && needErrorTextHide)
            {
                this.Visibility = Visibility.Collapsed;
            }
            else
            {
                //panel.ErrorPanel.Visibility = needErrorTextHide ? Visibility.Collapsed : Visibility.Visible;
                this.Visibility = Visibility.Visible;
            }
        }


    }
}
