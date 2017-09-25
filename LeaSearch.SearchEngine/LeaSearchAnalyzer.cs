using System;
using System.IO;
using Lucene.Net.Analysis;
using Version = Lucene.Net.Util.Version;

namespace LeaSearch.SearchEngine
{
    internal class LeaSearchAnalyzer : Analyzer
    {
        public LeaSearchAnalyzer(Version lucene30)
        {
            throw new NotImplementedException();
        }

        public override TokenStream TokenStream(string fieldName, TextReader reader)
        {
            throw new NotImplementedException();
        }

    }
}