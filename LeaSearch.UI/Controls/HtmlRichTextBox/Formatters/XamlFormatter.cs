﻿using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Documents;

namespace LeaSearch.UI.Controls.HtmlRichTextBox.Formatters
{
    /// <summary>
    /// Formats the RichTextBox text as Xaml
    /// </summary>
    public class XamlFormatter : ITextFormatter
    {
        public string GetText(System.Windows.Documents.FlowDocument document)
        {
            TextRange tr = new TextRange(document.ContentStart, document.ContentEnd);
            using (MemoryStream ms = new MemoryStream())
            {
                tr.Save(ms, DataFormats.Xaml);
                return ASCIIEncoding.Default.GetString(ms.ToArray());
            }
        }

        public void SetText(System.Windows.Documents.FlowDocument document, string text)
        {
            try
            {
                TextRange tr = new TextRange(document.ContentStart, document.ContentEnd);
                using (MemoryStream ms = new MemoryStream(Encoding.ASCII.GetBytes(text)))
                {
                    tr.Load(ms, DataFormats.Xaml);
                }
            }
            catch
            {
                throw new InvalidDataException("data provided is not in the correct Xaml format.");
            }
        }
    }
}
