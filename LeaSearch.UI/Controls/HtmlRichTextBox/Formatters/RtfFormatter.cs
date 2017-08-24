﻿using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Documents;

namespace LeaSearch.UI.Controls.HtmlRichTextBox.Formatters
{
    /// <summary>
    /// Formats the RichTextBox text as RTF
    /// </summary>
    public class RtfFormatter : ITextFormatter
    {
        public string GetText(FlowDocument document)
        {
            TextRange tr = new TextRange(document.ContentStart, document.ContentEnd);
            using (MemoryStream ms = new MemoryStream())
            {
                tr.Save(ms, DataFormats.Rtf);
                return ASCIIEncoding.Default.GetString(ms.ToArray());
            }
        }

        public void SetText(FlowDocument document, string text)
        {
            TextRange tr = new TextRange(document.ContentStart, document.ContentEnd);
            using (MemoryStream ms = new MemoryStream(Encoding.ASCII.GetBytes(text)))
            {
                tr.Load(ms, DataFormats.Rtf);
            }
        }
    }
}
