using System.Windows;
using System.Windows.Controls;

namespace LeaSearch.UI.Controls
{
    public class TextBoxEx : TextBox
    {
        public static readonly DependencyProperty WatermarkProperty = DependencyProperty.Register("Watermark", typeof(object), typeof(TextBoxEx), (PropertyMetadata)new UIPropertyMetadata((PropertyChangedCallback)null));
        public static readonly DependencyProperty WatermarkTemplateProperty = DependencyProperty.Register("WatermarkTemplate", typeof(DataTemplate), typeof(TextBoxEx), (PropertyMetadata)new UIPropertyMetadata((PropertyChangedCallback)null));

        static TextBoxEx()
        {
            FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(TextBoxEx), (PropertyMetadata)new FrameworkPropertyMetadata((object)typeof(TextBoxEx)));
        }

        public object Watermark
        {
            get
            {
                return this.GetValue(TextBoxEx.WatermarkProperty);
            }
            set
            {
                this.SetValue(TextBoxEx.WatermarkProperty, value);
            }
        }

        public DataTemplate WatermarkTemplate
        {
            get
            {
                return (DataTemplate)this.GetValue(TextBoxEx.WatermarkTemplateProperty);
            }
            set
            {
                this.SetValue(TextBoxEx.WatermarkTemplateProperty, (object)value);
            }
        }
    }
}
