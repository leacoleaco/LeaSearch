using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace LeaSearch.UI.Controls
{
    /// <summary>
    /// 使用 <em>xxx</em> 标注高亮
    /// </summary>
    public class HighlightControl : Control
    {
        private TextBlock _textBlock;

        public string Html
        {
            get { return (string)GetValue(HtmlProperty); }
            set { SetValue(HtmlProperty, value); }
        }
        public static readonly DependencyProperty HtmlProperty = DependencyProperty.Register(
            "Html", typeof(string), typeof(HighlightControl), new PropertyMetadata(default(string), OnHtmlChanged));

        private static void OnHtmlChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var h = d as HighlightControl;
            var newText = e.NewValue as string;
            h?.SetText(newText);
        }

        public static readonly DependencyProperty HighLightForegroundProperty = DependencyProperty.Register(
            "HighLightForeground", typeof(Brush), typeof(HighlightControl), new PropertyMetadata(Brushes.Red));

        public Brush HighLightForeground
        {
            get { return (Brush)GetValue(HighLightForegroundProperty); }
            set { SetValue(HighLightForegroundProperty, value); }
        }

        private string startWord = "<em>";
        private string endWord = "</em>";

        private void SetText(string newText)
        {
            if (_textBlock == null) return;

            _textBlock.Inlines.Clear();
            if (string.IsNullOrEmpty(newText)) return;


            var textBuffer = newText;
            var segs = new List<WordSeg>();
            var startLen = startWord.Length;
            var endLen = endWord.Length;
            while (!string.IsNullOrEmpty(textBuffer))
            {
                var startIndex = textBuffer.IndexOf(startWord, StringComparison.Ordinal);
                var endIndex = textBuffer.IndexOf(endWord, StringComparison.Ordinal);
                if (startIndex < 0 || endIndex < 0)
                {
                    segs.Add(new WordSeg() { Word = textBuffer });
                    textBuffer = null;
                }
                else
                {
                    var preStr = textBuffer.Substring(0, startIndex);
                    if (!string.IsNullOrEmpty(preStr))
                    {
                        segs.Add(new WordSeg() { Word = preStr });
                    }
                    var wordIndex = startIndex + startLen;
                    var wordLen = endIndex - wordIndex;
                    var highStr = textBuffer.Substring(wordIndex, wordLen);
                    if (!string.IsNullOrEmpty(highStr))
                    {
                        segs.Add(new WordSeg() { Word = highStr, IsHighlight = true });
                    }
                    textBuffer = textBuffer.Substring(endIndex + endLen);
                }

            }

            foreach (var seg in segs)
            {
                _textBlock.Inlines.Add(CreateRun(seg, HighLightForeground));
            }

        }



        protected Run CreateRun(WordSeg seg, Brush highLightForeground)
        {
            var run = new Run(seg.Word);

            if (seg.IsHighlight)
            {
                run.Foreground = highLightForeground;
            }

            return run;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();


            _textBlock = this.GetTemplateChild("PART_Text") as TextBlock;

            //模版设置后，设置文本
            SetText(Html);

        }
    }

    public class WordSeg
    {
        public string Word;
        public bool IsHighlight;
    }
}
