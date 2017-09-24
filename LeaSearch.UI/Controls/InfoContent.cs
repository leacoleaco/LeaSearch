using System.Windows;
using System.Windows.Controls;

namespace LeaSearch.UI.Controls
{
    public class InfoContent : ContentControl
    {
        static InfoContent()
        {
            FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(InfoContent), (PropertyMetadata)new FrameworkPropertyMetadata((object)typeof(InfoContent)));
        }

        public static readonly DependencyProperty DisplayContentProperty = DependencyProperty.Register("DisplayContent", typeof(object), typeof(InfoContent), new PropertyMetadata(default(ContentPresenter)));


        /// <summary>
        /// 需要显示的内容
        /// </summary>
        public object DisplayContent
        {
            get { return (ContentPresenter)GetValue(DisplayContentProperty); }
            set { SetValue(DisplayContentProperty, value); }
        }


    }

    public delegate void DisplayContentChanged(object sender, DisplayContentChangedArgs args);

    public class DisplayContentChangedArgs
    {
        public object NewValue { get; set; }
        public object OldValue { get; set; }
    }
}
