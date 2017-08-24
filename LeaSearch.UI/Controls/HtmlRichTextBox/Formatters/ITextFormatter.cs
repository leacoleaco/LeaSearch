using System.Windows.Documents;

namespace LeaSearch.UI.Controls.HtmlRichTextBox.Formatters
{
    public interface ITextFormatter
    {
        string GetText(FlowDocument document);
        void SetText(FlowDocument document, string text);
    }
}
