using System.Windows;
using System.Windows.Controls;
using HTMLConverter;

namespace LeaSearch.Core.Views
{
    /// <summary>
    /// HtmlEditorView.xaml 的交互逻辑
    /// </summary>
    public partial class HtmlEditorView : Window
    {
        public HtmlEditorView()
        {
            InitializeComponent();
        }

        private void TextEditor_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (TextEditor1 == null || TextEditor == null) return;

            var xaml = HtmlToXamlConverter.ConvertHtmlToXaml(TextEditor.Text, true);

            TextEditor1.Text = xaml;
        }
    }
}
