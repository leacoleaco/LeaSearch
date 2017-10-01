using System;
using System.Collections.Generic;
using System.Text;
using LeaSearch.SearchEngine.Pinyin;
using PanGu;

namespace LeaSearch.SearchEngine.PanGuEx
{
    public class Highlighter
    {
        #region Private fields

        private int _FragmentSize = 50;
        private Formatter _Formatter;
        private global::PanGu.Segment _PanGuSegment;
        List<WordInfo> _Selection;
        string _Content;

        #endregion

        #region Public properties
        /// <summary>
        /// set or get fragment size.
        /// </summary>
        public int FragmentSize
        {
            get
            {
                return _FragmentSize;
            }

            set
            {
                _FragmentSize = value;
            }
        }
        #endregion

        #region Private methods
        private List<WordInfo> PickupKeywords(ICollection<WordInfo> keywordsWordInfos, ICollection<WordInfo> contentWordInfos)
        {
            var ret = new List<WordInfo>();

            var dict = new Dictionary<string, bool>();
            var pinyinDict = new Dictionary<string, bool>();
            var initPinyinDict = new Dictionary<string, bool>();


            //先生成高亮关键词
            foreach (var wordInfoEx in keywordsWordInfos)
            {
                if (wordInfoEx == null)
                {
                    continue;
                }

                if (!dict.ContainsKey(wordInfoEx.Word))
                {
                    dict.Add(wordInfoEx.Word, true);


                    //添加拼音支持
                    pinyinDict.Add(PinyinUtil.GetPinyinString(wordInfoEx.Word), true);
                    initPinyinDict.Add(PinyinUtil.GetInitPinyinString(wordInfoEx.Word), true);
                }

            }

            foreach (var wordInfoEx in contentWordInfos)
            {
                if (wordInfoEx == null)
                {
                    continue;
                }

                //检查拼音
                if (dict.ContainsKey(wordInfoEx.Word) || CheckPinyin(wordInfoEx.Word, pinyinDict, initPinyinDict))
                {
                    ret.Add(wordInfoEx);
                }
            }

            ret.Sort();
            return ret;
        }

        /// <summary>
        /// 检查词语是否包含在列表中（查询拼音方式）
        /// </summary>
        /// <param name="word"></param>
        /// <param name="minGram">多少个字符起算符合</param>
        /// <returns></returns>
        private static bool CheckPinyin(string word, Dictionary<string, bool> keywordPinyinDict, Dictionary<string, bool> keywordInitPinyinDict, int minGram = 2)
        {
            if (string.IsNullOrWhiteSpace(word) || !PinyinUtil.ContainsChinese(word)) return false;

            var pinyin = PinyinUtil.GetPinyinString(word);
            if (pinyin.Length < minGram)
            {
                return false;
            }

            foreach (var piny in keywordPinyinDict)
            {
                if (pinyin.StartsWith(piny.Key)) return true;
            }

            var initPinyin = PinyinUtil.GetInitPinyinString(word);
            foreach (var piny in keywordInitPinyinDict)
            {
                if (initPinyin.StartsWith(piny.Key)) return true;
            }

            return false;
        }


        //检索关键词
        private IEnumerable<WordInfo> Optimize(List<WordInfo> selection, int start, int end)
        {
            var maxRank = 0;
            WordInfo lst = null;

            for (var i = start; i < end; i++)
            {
                if (selection[i].Rank > maxRank)
                {
                    maxRank = selection[i].Rank;
                }
            }

            var rankSum = 0;
            var endPosition = selection[start].GetEndPositon();

            for (var i = start; i < end; i++)
            {
                if (endPosition < selection[i].GetEndPositon())
                {
                    endPosition = selection[i].GetEndPositon();
                }

                if (selection[i].Rank == maxRank)
                {
                    if (lst == null)
                    {
                        rankSum += (int)Math.Pow(3, selection[i].Rank);
                        //yield return selection[i];
                    }
                    else if (selection[i].Position >= lst.Position + lst.Word.Length)
                    {
                        rankSum += (int)Math.Pow(3, selection[i].Rank);
                        //yield return selection[i];
                    }

                    lst = selection[i];
                }
            }

            var word = new WordInfo();

            word.Position = selection[start].Position;
            word.Word = _Content.Substring(word.Position, endPosition - word.Position);
            word.Rank = rankSum == 0 ? (int)Math.Pow(3, maxRank) : rankSum;
            yield return word;
        }

        private List<WordInfo> Optimize(List<WordInfo> selection)
        {
            var start = 0;
            var end = 0;
            var ret = new List<WordInfo>();

            while (start < selection.Count)
            {
                var endCharPos = selection[start].Position + selection[start].Word.Length;

                while (end < selection.Count)
                {
                    if (selection[end].Position >= endCharPos)
                    {
                        foreach (var wordInfo in Optimize(selection, start, end))
                        {
                            ret.Add(wordInfo);
                        }

                        start = end;
                        break;
                    }

                    endCharPos = Math.Max(endCharPos, selection[end].Position + selection[end].Word.Length);
                    end++;
                }

                if (start != end)
                {
                    //end point to the last word in the list
                    foreach (var wordInfo in Optimize(selection, start, end))
                    {
                        ret.Add(wordInfo);
                    }

                    break;
                }
            }

            return ret;
        }

        private List<Fragment> GetFragments(List<WordInfo> selection)
        {
            var fragments = new List<Fragment>();

            if (selection.Count == 0)
            {
                return fragments;
            }

            var start = 0;
            var end = 0;

            while (start < selection.Count)
            {
                while (end < selection.Count)
                {
                    if (selection[end].GetEndPositon() - selection[start].Position > FragmentSize)
                    {
                        fragments.Add(new Fragment(start, end, selection));
                        start = end;
                    }

                    end++;
                }

                if (start != end)
                {
                    //end point to the last word in the list
                    foreach (var wordInfo in Optimize(selection, start, end))
                    {
                        fragments.Add(new Fragment(start, end, selection));
                    }

                    break;
                }

            }

            fragments.Sort();
            return fragments;
        }

        private List<Fragment> GetFragments(string keywords, string content)
        {
            var keywordsWordInfos = _PanGuSegment.DoSegment(keywords);

            //Make lower
            foreach (var wordInfo in keywordsWordInfos)
            {
                if (wordInfo?.Word == null)
                {
                    continue;
                }

                wordInfo.Word = wordInfo.Word.ToLower();
            }

            var contentWordInfos = _PanGuSegment.DoSegment(content);

            //Make lower
            foreach (var wordInfo in contentWordInfos)
            {
                if (wordInfo?.Word == null)
                {
                    continue;
                }

                wordInfo.Word = wordInfo.Word.ToLower();
            }

            _Content = content;

            _Selection = PickupKeywords(keywordsWordInfos, contentWordInfos);

            _Selection = Optimize(_Selection);

            return GetFragments(_Selection);
        }

        private string FragmentToString(Fragment fragment)
        {
            var str = new StringBuilder();

            var width = _Selection[fragment.End - 1].GetEndPositon() - _Selection[fragment.Start].Position;

            var remain = (_FragmentSize - width) / 2;

            if (remain < 0)
            {
                remain = 0;
            }

            var fst = Math.Max(0, _Selection[fragment.Start].Position - remain);

            var lst = Math.Min(_Content.Length, _Selection[fragment.End - 1].GetEndPositon() + remain);

            if (fst == 0)
            {
                lst += remain - _Selection[fragment.Start].Position;
                lst = Math.Min(_Content.Length, lst);
            }
            else if (lst == _Content.Length)
            {
                fst -= _Selection[fragment.End - 1].GetEndPositon() + remain - lst;
                fst = Math.Max(0, fst);
            }


            for (var i = fragment.Start; i < fragment.End; i++)
            {
                str.AppendFormat("{0}{1}",
                    _Content.Substring(fst, _Selection[i].Position - fst),
                    _Formatter.HighlightTerm(_Content.Substring(_Selection[i].Position, _Selection[i].Word.Length)));
                fst = _Selection[i].GetEndPositon();
            }

            str.Append(_Content.Substring(_Selection[fragment.End - 1].GetEndPositon(),
                lst - _Selection[fragment.End - 1].GetEndPositon()));

            return str.ToString();
        }

        #endregion

        public Highlighter(Formatter formatter, global::PanGu.Segment segment)
        {
            _Formatter = formatter;
            _PanGuSegment = segment;
        }

        /// <summary>
        /// Get best fragment
        /// </summary>
        /// <param name="keywords">keywords</param>
        /// <param name="content">content</param>
        /// <returns></returns>
        public string GetBestFragment(string keywords, string content)
        {
            var fragments = GetFragments(keywords, content);

            if (fragments.Count > 0)
            {
                return FragmentToString(fragments[0]);
            }

            return "";
        }

        /// <summary>
        /// Get the fragments 
        /// </summary>
        /// <param name="keywords">keywords</param>
        /// <param name="content">content</param>
        /// <param name="maxFragments">max fragments will be outputed</param>
        /// <returns></returns>
        public IEnumerable<string> GetFragments(string keywords, string content, int maxFragments)
        {
            var fragments = GetFragments(keywords, content);
            var index = 0;

            foreach (var fragment in fragments)
            {
                if (index >= maxFragments)
                {
                    break;
                }

                index++;
                yield return FragmentToString(fragment);
            }
        }
    }
}
