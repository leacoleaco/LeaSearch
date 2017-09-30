using System;
using System.Collections.Generic;
using System.Text;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Tokenattributes;
using PanGu;
using PanGu.Match;

namespace LeaSearch.SearchEngine.Analysis
{
    public class PanGuTokenizer : Tokenizer
    {
        private static object _lockObj = new object();
        private static bool _inited = false;

        private WordInfo[] _wordList;
        private int _position = -1; //词汇在缓冲中的位置.
        private bool _originalResult = false;
        string _inputText;

        // this tokenizer generates three attributes:
        // offset, positionIncrement and type
        private ITermAttribute _termAtt;
        private IOffsetAttribute _offsetAtt;
        private IPositionIncrementAttribute _posIncrAtt;
        private ITypeAttribute _typeAtt;

        static private void InitPanGuSegment()
        {
            //Init PanGu Segment.
            if (!_inited)
            {
                global::PanGu.Segment.Init();
                _inited = true;
            }
        }

        /// <summary>
        /// Init PanGu Segment
        /// </summary>
        /// <param name="fileName">PanGu.xml file path</param>
        static public void InitPanGuSegment(string fileName)
        {
            lock (_lockObj)
            {
                //Init PanGu Segment.
                if (!_inited)
                {
                    global::PanGu.Segment.Init(fileName);
                    _inited = true;
                }
            }
        }

        void Init()
        {
            InitPanGuSegment();
            _termAtt = AddAttribute<ITermAttribute>();
            _offsetAtt = AddAttribute<IOffsetAttribute>();
            _posIncrAtt = AddAttribute<IPositionIncrementAttribute>();
            _typeAtt = AddAttribute<ITypeAttribute>();
        }

        public PanGuTokenizer(System.IO.TextReader input, bool originalResult)
            : this(input, originalResult, null, null)
        {

        }

        public PanGuTokenizer(System.IO.TextReader input, bool originalResult, MatchOptions options, MatchParameter parameters)
            : this(input, options, parameters)
        {
            _originalResult = originalResult;
        }

        public PanGuTokenizer()
        {
            lock (_lockObj)
            {
                Init();
            }
        }

        public PanGuTokenizer(System.IO.TextReader input, MatchOptions options, MatchParameter parameters)
            : base(input) 
        {
            lock (_lockObj)
            {
                Init();
            }

            _inputText = base.input.ReadToEnd();

            if (string.IsNullOrEmpty(_inputText))
            {
                char[] readBuf = new char[1024];

                int relCount = base.input.Read(readBuf, 0, readBuf.Length);

                StringBuilder inputStr = new StringBuilder(readBuf.Length);


                while (relCount > 0)
                {
                    inputStr.Append(readBuf, 0, relCount);

                    relCount = input.Read(readBuf, 0, readBuf.Length);
                }

                if (inputStr.Length > 0)
                {
                    _inputText = inputStr.ToString();
                }
            }

            if (string.IsNullOrEmpty(_inputText))
            {
                _wordList = new WordInfo[0];
            }
            else
            {
                global::PanGu.Segment segment = new Segment();
                ICollection<WordInfo> wordInfos = segment.DoSegment(_inputText, options, parameters);
                _wordList = new WordInfo[wordInfos.Count];
                wordInfos.CopyTo(_wordList, 0);
            }
        }

        public override bool IncrementToken()
        {
            ClearAttributes();
            Token word = Next();
            if (word != null)
            {
                _termAtt.SetTermBuffer(word.Term);
                _offsetAtt.SetOffset(word.StartOffset, word.EndOffset);
                _typeAtt.Type = word.Type;
                return true;
            }
            End();
            return false;
        }

        //DotLucene的分词器简单来说，就是实现Tokenizer的Next方法，把分解出来的每一个词构造为一个Token，因为Token是DotLucene分词的基本单位。
        public Token Next()
        {
            if (_originalResult)
            {
                string retStr = _inputText;
                
                _inputText = null;

                if (retStr == null)
                {
                    return null;
                }

                return new Token(retStr, 0, retStr.Length);
            }

            int length = 0;    //词汇的长度.
            int start = 0;     //开始偏移量.

            while (true)
            {
                _position++;
                if (_position < _wordList.Length)
                {
                    if (_wordList[_position] != null)
                    {
                        length = _wordList[_position].Word.Length;
                        start = _wordList[_position].Position;
                        return new Token(_wordList[_position].Word, start, start + length);
                    }
                }
                else
                {
                    break;
                }
            }

            _inputText = null;
            return null;
        }

        public ICollection<WordInfo> SegmentToWordInfos(String str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return new LinkedList<WordInfo>();
            }

            global::PanGu.Segment segment = new Segment();
            return segment.DoSegment(str);
        }
    }

}
