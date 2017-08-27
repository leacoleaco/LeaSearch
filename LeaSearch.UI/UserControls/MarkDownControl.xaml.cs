using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Xaml;
using Markdig;
using XamlReader = System.Windows.Markup.XamlReader;

namespace LeaSearch.UI.UserControls
{
    /// <summary>
    /// MarkDownControl.xaml 的交互逻辑
    /// </summary>
    public partial class MarkDownControl : UserControl
    {

        public MarkDownControl()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
            "Text", typeof(string), typeof(MarkDownControl), new PropertyMetadata(default(string)));


        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }



    }
}
