using LeaSearch.SearchEngine.Pinyin;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Tokenattributes;

namespace LeaSearch.SearchEngine.Analysis
{
    /** 
   * 对转换后的拼音进行NGram处理的TokenFilter 
   * @author Lanxiaowei 
   * 
   */
    public sealed class PinyinNGramTokenFilter : TokenFilter
    {
        public static bool DefaultNgramChinese = false;
        private int _minGram;
        private int _maxGram;
        /**是否需要对中文进行NGram[默认为false]*/
        private bool _nGramChinese;
        private ITermAttribute _termAtt;
        private IOffsetAttribute _offsetAtt;
        private char[] _curTermBuffer;
        private int _curTermLength;
        private int _curGramSize;
        private int _tokStart;

        public PinyinNGramTokenFilter(TokenStream input, int minGram, int maxGram,
              bool nGramChinese = false) : base(input)
        {

            this._termAtt = AddAttribute<ITermAttribute>();
            this._offsetAtt = AddAttribute<IOffsetAttribute>();

            //        if (minGram< 1) {  
            //            throw new ArgumentOutOfRangeException("");
            //}  
            //        if (minGram > maxGram) {  
            //            throw new IllegalArgumentException(  
            //                    "minGram must not be greater than maxGram");  
            //        }  
            this._minGram = minGram;
            this._maxGram = maxGram;
            this._nGramChinese = nGramChinese;
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
                    if ((!this._nGramChinese)
                            && (PinyinUtil.ContainsChinese(this._termAtt.Term)))
                    {
                        return true;
                    }
                    this._curTermBuffer = (char[])this._termAtt.TermBuffer().Clone();

                    this._curTermLength = this._termAtt.TermLength();
                    this._curGramSize = this._minGram;
                    this._tokStart = this._offsetAtt.StartOffset;
                }

                if (this._curGramSize <= this._maxGram)
                {
                    if (this._curGramSize >= this._curTermLength)
                    {
                        ClearAttributes();
                        this._offsetAtt.SetOffset(this._tokStart + 0, this._tokStart
                                + this._curTermLength);
                        this._termAtt.SetTermBuffer(this._curTermBuffer, 0,
                                this._curTermLength);
                        this._curTermBuffer = null;
                        return true;
                    }
                    int start = 0;
                    int end = start + this._curGramSize;
                    ClearAttributes();
                    this._offsetAtt.SetOffset(this._tokStart + start, this._tokStart
                            + end);
                    this._termAtt.SetTermBuffer(this._curTermBuffer, start,
                            this._curGramSize);
                    this._curGramSize += 1;
                    return true;
                }

                this._curTermBuffer = null;
            }
        }

        public override void Reset()
        {
            base.Reset();
            this._curTermBuffer = null;
        }

    }
}
