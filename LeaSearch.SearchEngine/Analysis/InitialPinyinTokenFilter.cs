using System;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Tokenattributes;

namespace LeaSearch.SearchEngine.Analysis
{
    /// <summary>
    /// 拼音首字母过滤器，负责将汉字转为拼音
    /// </summary>
    public sealed class InitialPinyinTokenFilter : TokenFilter
    {
        private ITermAttribute _termAtt;


        /**Term最小长度[小于这个最小长度的不进行拼音转换]*/
        private readonly int _minTermLength = 1;
        private char[] _curTermBuffer;
        private int _curTermLength;
        private bool _outChinese;



        public InitialPinyinTokenFilter(TokenStream input,
                int minTermLenght, bool outChinese = false) : base(input)
        {

            _termAtt = AddAttribute<ITermAttribute>();

            this._outChinese = outChinese;


            this._minTermLength = minTermLenght;
            if (this._minTermLength < 1)
            {
                this._minTermLength = 1;
            }

        }


        public static bool ContainsChinese(String s)
        {
            if ((s == null) || (string.Empty == s.Trim()))
                return false;

            foreach (char t in s)
            {
                if (IsChinese(t))
                    return true;
            }
            return false;
        }

        public static bool IsChinese(char a)
        {
            int v = a;
            return (v >= 19968) && (v <= 171941);
        }

        public override bool IncrementToken()
        {
            while (true)
            {
                if (this._curTermBuffer == null)
                {
                    if (!this.input.IncrementToken())
                    {
                        return false;
                    }
                    this._curTermBuffer = (char[])this._termAtt.TermBuffer().Clone();
                    this._curTermLength = this._termAtt.TermLength();
                }

                if (this._outChinese)
                {
                    this._outChinese = false;
                    this._termAtt.SetTermBuffer(this._curTermBuffer, 0,
                            this._curTermLength);
                    return true;
                }
                this._outChinese = true;
                String chinese = this._termAtt.Term;

                if (ContainsChinese(chinese))
                {
                    this._outChinese = true;
                    if (chinese.Length >= this._minTermLength)
                    {
                        try
                        {
                            String chineseTerm = GetPinyinString(chinese);
                            this._termAtt.SetTermBuffer(chineseTerm.ToCharArray(), 0,
                                    chineseTerm.Length);
                        }
                        catch (Exception e)
                        {
                            throw e;
                            //TODO
                        }
                        this._curTermBuffer = null;
                        return true;
                    }

                }

                this._curTermBuffer = null;
            }
        }



        private String GetPinyinString(String chinese)
        {
            String initialPinyin = null;

            initialPinyin = NPinyin.Pinyin.GetInitials(chinese);

            return initialPinyin;
        }
    }
}
