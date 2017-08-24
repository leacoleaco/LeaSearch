﻿using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Documents;
using LeaSearch.UI.Controls.HtmlRichTextBox.HtmlXamlConversion;

namespace LeaSearch.UI.Controls.HtmlRichTextBox.Formatters
{
    /// <summary>
    /// Formats the RichTextBox text as Html
    /// </summary>
    public class HtmlFormatter : ITextFormatter
    {
        public string GetText(System.Windows.Documents.FlowDocument document)
        {
            TextRange tr = new TextRange(document.ContentStart, document.ContentEnd);
            using (MemoryStream ms = new MemoryStream())
            {
                tr.Save(ms, DataFormats.Xaml);
                return HtmlFromXamlConverter.ConvertXamlToHtml(UTF8Encoding.Default.GetString(ms.ToArray()));
            }
        }

        public void SetText(System.Windows.Documents.FlowDocument document, string text)
        {
            text = HtmlToXamlConverter.ConvertHtmlToXaml(text, false);
            try
            {
                TextRange tr = new TextRange(document.ContentStart, document.ContentEnd);
                using (MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(text)))
                {
                    tr.Load(ms, DataFormats.Xaml);
                }
            }
            catch
            {
                throw new InvalidDataException("data provided is not in the correct Html format.");
            }
        }
    }
}
