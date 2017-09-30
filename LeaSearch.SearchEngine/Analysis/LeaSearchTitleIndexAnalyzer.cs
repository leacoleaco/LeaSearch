using System.IO;
using Lucene.Net.Analysis;
using PanGu.Match;

namespace LeaSearch.SearchEngine.Analysis
{
    /// <summary>
    /// 
    /// 自定义标题分词器
    ///支持 盘古分词、拼音、拼音首字母
    /// author：leaco
    /// </summary>
    public class LeaSearchTitleIndexAnalyzer : Analyzer
    {
        private bool _supportFirstChar = true;
        private bool _originalResult = false;
        private MatchOptions _options;
        private MatchParameter _parameters;

        public override TokenStream TokenStream(string fieldName, TextReader reader)
        {
            //Lucene.Net.Analysis.TokenStream result = new MMSegTokenizer(NewSeg, reader);
            //result.Reset();

            TokenStream result = new PanGuTokenizer(reader, _originalResult, _options, _parameters);

            result = new PinyinTokenFilter(result, 1);
            result = new PinyinNGramTokenFilter(result, 3, 10, true);
            result = new InitialPinyinTokenFilter(result, 2);
            result = new LowerCaseFilter(result);
            return result;
        }

    }
}