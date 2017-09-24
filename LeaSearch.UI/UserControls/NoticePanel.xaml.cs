using System.Windows;
using System.Windows.Controls;

namespace LeaSearch.UI.UserControls
{
    /// <summary>
    /// NoticePanel.xaml 的交互逻辑
    /// </summary>
    public partial class NoticePanel : UserControl
    {
        public static readonly DependencyProperty ErrorTextProperty = DependencyProperty.Register("ErrorText", typeof(string), typeof(NoticePanel), new PropertyMetadata(default(string), OnErrorTextChanged));
        private static void OnErrorTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var panel = d as NoticePanel;
            panel?.SetVisibilty(panel, e.NewValue, panel.InfoText);
        }
        public static readonly DependencyProperty InfoTextProperty = DependencyProperty.Register("InfoText", typeof(string), typeof(NoticePanel), new PropertyMetadata(default(string), OnInfoTextChanged));
        private static void OnInfoTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var panel = d as NoticePanel;
            panel?.SetVisibilty(panel, panel.ErrorText, e.NewValue);
        }

        public NoticePanel()
        {
            InitializeComponent();
        }


        public string ErrorText
        {
            get { return (string)GetValue(ErrorTextProperty); }
            set
            {
                SetValue(ErrorTextProperty, value);
            }
        }

        public string InfoText
        {
            get { return (string)GetValue(InfoTextProperty); }
            set
            {
                SetValue(InfoTextProperty, value);
            }
        }

        

        private void SetVisibilty(NoticePanel panel, object errorText, object infoText)
        {
            var needInfoTextHide = string.IsNullOrWhiteSpace(infoText?.ToString());
            var needErrorTextHide = string.IsNullOrWhiteSpace(errorText?.ToString());
            if (needInfoTextHide && needErrorTextHide)
            {
                this.Visibility = Visibility.Collapsed;
            }
            else
            {
                panel.InfoPanel.Visibility = needInfoTextHide ? Visibility.Collapsed : Visibility.Visible;
                panel.ErrorPanel.Visibility = needErrorTextHide ? Visibility.Collapsed : Visibility.Visible;
                this.Visibility = Visibility.Visible;
            }
        }


    }
}
