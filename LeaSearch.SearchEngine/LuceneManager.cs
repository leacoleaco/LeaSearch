using System;
using System.Collections.Generic;
using System.IO;
using LeaSearch.Plugin.Index;
using LeaSearch.SearchEngine.Analysis;
using LeaSearch.SearchEngine.PanGuEx;
using Lucene.Net.Analysis;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using Lucene.Net.Store;
using PanGu;
using Version = Lucene.Net.Util.Version;

namespace LeaSearch.SearchEngine
{
    public class LuceneManager
    {
        private const string IndexDir = "Data\\Index";
        private readonly DirectoryInfo _indexDir;
        private readonly Analyzer _indexAnalyzer = new LeaSearchTitleIndexAnalyzer();
        private readonly Analyzer _searchAnalyzer = new PanGuAnalyzer();
        private IndexSearcher _indexSearcher;

        private Highlighter _highlighter;


        public LuceneManager()
        {
            if (!System.IO.Directory.Exists(IndexDir))
            {
                System.IO.Directory.CreateDirectory(IndexDir);
            }
            _indexDir = new DirectoryInfo(IndexDir);

            _highlighter = new Highlighter(new SimpleHTMLFormatter("<em>", "</em>"), new Segment())
            {
                //最多显示的字符数
                FragmentSize = 50
            };
        }

        /// <summary>
        /// 初始化 查询引擎
        /// </summary>
        public void InitSearcher()
        {
            _indexSearcher = new IndexSearcher(FSDirectory.Open(_indexDir), true);
        }


        //~LuceneManager()
        //{
        //    _indexWriter.Dispose();
        //    _indexSearcher.Dispose();
        //}


        /// <summary>
        /// 搜索
        /// </summary>
        /// <param name="keyword"></param>
        /// <param name="top"></param>
        /// <returns></returns>
        public DataItem[] Search(string keyword, int top)
        {

            if (_indexSearcher == null) throw new Exception("Lucene index is not ready!");

            //拼装查询语句
            var query = new QueryParser(Version.LUCENE_30, "Name", _searchAnalyzer).Parse(keyword);
            //new MultiFieldQueryParser(Version.LUCENE_30,new string[] {"PluginId"},Analyzer).Parse()


            //返回命中数量
            TopDocs tds = _indexSearcher.Search(query, top);

            var res = new List<DataItem>();
            foreach (ScoreDoc sd in tds.ScoreDocs)
            {
                //Console.WriteLine(sd.Score);
                Document doc = _indexSearcher.Doc(sd.Doc);
                //Console.WriteLine(doc.Get("body"));
                res.Add(Document2DataItem(doc, keyword, "Name"));

            }
            return res.ToArray();
        }


        /// <summary>
        /// 搜索
        /// </summary>
        /// <param name="keyword"></param>
        /// <param name="top"></param>
        /// <returns></returns>
        public DataItem[] Search(string keyword, string pluginId, int top)
        {

            if (_indexSearcher == null) throw new Exception("Lucene index is not ready!");

            //拼装查询语句
            var query = new BooleanQuery();

            var pluginQuery = new TermQuery(new Term("PluginId", pluginId));
            query.Add(pluginQuery, Occur.MUST);

            var nameQuery = new QueryParser(Version.LUCENE_30, "Name", _searchAnalyzer).Parse(QueryParser.Escape(keyword));
            query.Add(nameQuery, Occur.MUST);


            //返回命中数量
            TopDocs tds = _indexSearcher.Search(query, top);

            var res = new List<DataItem>();
            foreach (ScoreDoc sd in tds.ScoreDocs)
            {
                //Console.WriteLine(sd.Score);
                Document doc = _indexSearcher.Doc(sd.Doc);
                //Console.WriteLine(doc.Get("body"));
                res.Add(Document2DataItem(doc, keyword, "Name"));

            }
            return res.ToArray();
        }

        public DataItem[] SearchByPluginId(string pluginId, int top = 100)
        {

            if (_indexSearcher == null) throw new Exception("Lucene index is not ready!");

            Term t = new Term("PluginId", pluginId);
            Query query = new TermQuery(t);

            //返回命中数量
            TopDocs tds = _indexSearcher.Search(query, top);

            var res = new List<DataItem>();
            foreach (ScoreDoc sd in tds.ScoreDocs)
            {
                //Console.WriteLine(sd.Score);
                Document doc = _indexSearcher.Doc(sd.Doc);
                //Console.WriteLine(doc.Get("body"));
                res.Add(Document2DataItem(doc));

            }
            return res.ToArray();
        }

        private IndexWriter GetIndexWriter()
        {
            return new IndexWriter(FSDirectory.Open(_indexDir), _indexAnalyzer, true, IndexWriter.MaxFieldLength.LIMITED);
        }

        public void DeleteAllIndex()
        {
            IndexWriter indexWriter = GetIndexWriter();
            indexWriter.DeleteAll();//删除所有的索引
            //writer.DeleteDocuments(new Term("key", "val"));//删除该词条
            //                                               //注意 在执行这个删除操作的时候，其实lucene本身并没有将数据从硬盘删除，而是保存到了一个单独的后缀名为.del的文件中。
            indexWriter.Commit();//进行提交操作 标记为删除的索引会被从硬盘删除
            indexWriter.Dispose();

        }

        public void CreateIndex()
        {
            IndexWriter indexWriter = GetIndexWriter();
            indexWriter.Commit();
            indexWriter.Dispose();
        }

        ///  <summary>
        /// 重新收集插件提供的索引
        ///  </summary>
        /// <param name="indexInfos"></param>
        public void CreateIndex(IndexInfo[] indexInfos)
        {
            IndexWriter indexWriter = GetIndexWriter();

            foreach (var indexInfo in indexInfos)
            {
                if (indexInfo == null) continue;

                Term term = new Term("PluginId", indexInfo.PluginId);
                indexWriter.DeleteDocuments(term);
                foreach (var item in indexInfo.Items)
                {
                    indexWriter.AddDocument(DataItem2Document(item, indexInfo.PluginId));
                }
                indexWriter.Commit();
            }

            indexWriter.Optimize();
            indexWriter.Dispose();
        }

        public void AddToIndex(DataItem[] items, string pluginId)
        {
            IndexWriter indexWriter = GetIndexWriter();
            foreach (var item in items)
            {
                indexWriter.AddDocument(DataItem2Document(item, pluginId));
            }
            indexWriter.Commit();
            indexWriter.Optimize();
            indexWriter.Dispose();
        }

        public void UpdateToIndex(DataItem[] items, string pluginId)
        {
            IndexWriter _indexWriter = GetIndexWriter();
            foreach (var item in items)
            {
                var id = $"{pluginId}-{item.Name}";
                Term term = new Term("Id", id);
                _indexWriter.UpdateDocument(term, DataItem2Document(item, pluginId));

            }
            _indexWriter.Commit();
            _indexWriter.Optimize();
            _indexWriter.Dispose();
        }

        public void DeleteIndexByPluginId(string pluginId)
        {
            IndexWriter indexWriter = GetIndexWriter();
            Term term = new Term("PluginId", pluginId);
            indexWriter.DeleteDocuments(term);
            indexWriter.Commit();
            indexWriter.Optimize();
            indexWriter.Dispose();
        }

        private Document DataItem2Document(DataItem item, string pluginId)
        {
            //Field.Store.YES:存储字段值（未分词前的字段值） 
            //Field.Store.NO:不存储,存储与索引没有关系
            //Field.Store.COMPRESS:压缩存储,用于长文本或二进制，但性能受损

            //Field.Index.ANALYZED:分词建索引
            //Field.Index.ANALYZED_NO_NORMS:分词建索引，但是Field的值不像通常那样被保存，而是只取一个byte，这样节约存储空间
            //Field.Index.NOT_ANALYZED:不分词且索引
            //Field.Index.NOT_ANALYZED_NO_NORMS:不分词建索引，Field的值去一个byte保存

            //TermVector表示文档的条目（由一个Document和Field定位）和它们在当前文档中所出现的次数
            //Field.TermVector.YES:为每个文档（Document）存储该字段的TermVector
            //Field.TermVector.NO:不存储TermVector
            //Field.TermVector.WITH_POSITIONS:存储位置
            //Field.TermVector.WITH_OFFSETS:存储偏移量
            //Field.TermVector.WITH_POSITIONS_OFFSETS:存储位置和偏移量

            //Index         Store	TermVector	            用法
            //NOT_ANSLYZED	YES	    NO	                    文件名、主键
            //ANSLYZED	    YES	    WITH_POSITUION_OFFSETS	标题、摘要
            //ANSLYZED	    NO	    WITH_POSITUION_OFFSETS	很长的全文
            //NO	        YES	    NO	                    文档类型
            //NOT_ANSLYZED	NO	    NO	                    隐藏的关键词
            Document doc = new Document();
            doc.Add(new Field("Id", $"{pluginId}-{item.Name}", Field.Store.YES, Field.Index.NOT_ANALYZED, Field.TermVector.NO));
            doc.Add(new Field("PluginId", pluginId, Field.Store.YES, Field.Index.NOT_ANALYZED, Field.TermVector.NO));
            doc.Add(new Field("Name", item.Name, Field.Store.YES, Field.Index.ANALYZED));
            if (item.Tip != null)
            {
                doc.Add(new Field("Tip", item.Tip, Field.Store.YES, Field.Index.ANALYZED));
            }

            if (item.IconPath != null)
            {
                doc.Add(new Field("IconPath", item.IconPath, Field.Store.YES, Field.Index.NO));
            }

            if (item.IconBytes != null)
            {
                doc.Add(new Field("IconBytes", item.IconBytes, Field.Store.YES));
            }

            if (item.Body != null)
            {
                doc.Add(new Field("Body", item.Body, Field.Store.YES, Field.Index.ANALYZED));
            }

            if (item.Extra != null)
            {
                doc.Add(new Field("Extra", item.Extra, Field.Store.YES, Field.Index.NO));
            }

            return doc;
        }

        private DataItem Document2DataItem(Document doc, string keyword = null, string searchField = null)
        {
            var ret = new DataItem(doc.Get("PluginId"))
            {
                Name = GetHighlightStr(doc, "Name", keyword, searchField),
                Tip = GetHighlightStr(doc, "Tip", keyword, searchField),
                IconPath = doc.Get("IconPath"),
                IconBytes = doc.GetBinaryValue("IconBytes"),
                Body = GetHighlightStr(doc, "Body", keyword, searchField),
                Extra = doc.Get("Extra"),
            };
            return ret;
        }

        private string GetHighlightStr(Document doc, string valueField, string keyword = null, string searchField = null)
        {
            var content = doc.Get(valueField);

            if (string.IsNullOrEmpty(searchField) || string.IsNullOrWhiteSpace(keyword) || searchField != valueField) return content;

            return _highlighter.GetBestFragment(keyword, content);
        }
    }
}
