using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

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





        public static readonly DependencyProperty IconSourceProperty = DependencyProperty.Register("IconSource", typeof(ImageSource), typeof(NoticePanel), new PropertyMetadata(default(ImageSource)));

        public NoticePanel()
        {
            InitializeComponent();
        }


        //public ImageSource IconSource
        //{
        //    get { return PART_Icon.Source; }
        //    set { PART_Icon.Source = value; }
        //}

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

        public ImageSource IconSource
        {
            get { return (ImageSource)GetValue(IconSourceProperty); }
            set { SetValue(IconSourceProperty, value); }
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
                panel.NoticeInfoTextBlock.Visibility = needInfoTextHide ? Visibility.Collapsed : Visibility.Visible;
                panel.NoticeErrorTextBlock.Visibility = needErrorTextHide ? Visibility.Collapsed : Visibility.Visible;
                this.Visibility = Visibility.Visible;
            }
        }


    }
}
