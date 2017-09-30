using System.IO;
using Lucene.Net.Analysis;
using PanGu.Match;

namespace LeaSearch.SearchEngine.Analysis
{
    public class PanGuAnalyzer : Analyzer
    {
        private bool _originalResult = false;
        private MatchOptions _options;
        private MatchParameter _parameters;
        
        public PanGuAnalyzer()
        {
        }

        public PanGuAnalyzer(MatchOptions options, MatchParameter parameters)
            : base()
        {
            _options = options;
            _parameters = parameters;
        }
        
        /// <summary>
        /// Return original string.
        /// Does not use only segment
        /// </summary>
        /// <param name="originalResult"></param>
        public PanGuAnalyzer(bool originalResult)
        {
            _originalResult = originalResult;
        }

        public override TokenStream TokenStream(string fieldName, TextReader reader)
        {
            TokenStream result = new PanGuTokenizer(reader, _originalResult, _options, _parameters);
            result = new LowerCaseFilter(result);
            return result;
        }
    }


}
