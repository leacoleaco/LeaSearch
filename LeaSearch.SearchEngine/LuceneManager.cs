using System.Collections.Generic;
using System.IO;
using LeaSearch.Plugin.Query;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using Lucene.Net.Store;
using Version = Lucene.Net.Util.Version;

namespace LeaSearch.SearchEngine
{
    public class LuceneManager
    {
        private static readonly DirectoryInfo IndexDir = new DirectoryInfo("Data\\Index");
        //static Analyzer analyzer = new PanGuAnalyzer(); //MMSegAnalyzer //StandardAnalyzer
        private static readonly Analyzer Analyzer = new StandardAnalyzer(Version.LUCENE_30);
        private readonly IndexSearcher _indexSearcher = new IndexSearcher(FSDirectory.Open(IndexDir), true);

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
            //拼装查询语句
            var query = new QueryParser(Version.LUCENE_30, "Name", Analyzer).Parse(keyword);
            //new MultiFieldQueryParser(Version.LUCENE_30,new string[] {"PluginId"},Analyzer).Parse()


            //返回命中数量
            TopDocs tds = _indexSearcher.Search(query, top);

            var res = new List<DataItem>();
            foreach (ScoreDoc sd in tds.ScoreDocs)
            {
                //Console.WriteLine(sd.Score);
                Document doc = _indexSearcher.Doc(sd.Doc);
                //Console.WriteLine(doc.Get("body"));
                res.Add(new DataItem()
                {
                    Name = doc.Get("Name"),
                    IconPath = doc.Get("IconPath"),
                    PluginId = doc.Get("PluginId"),
                    Body = doc.Get("Body"),
                });

            }
            return res.ToArray();
        }

        public DataItem[] SearchByPluginId(string pluginId, int top = 100)
        {
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
                res.Add(new DataItem()
                {
                    Name = doc.Get("Name"),
                    IconPath = doc.Get("IconPath"),
                    PluginId = doc.Get("PluginId"),
                    Body = doc.Get("Body"),
                });

            }
            return res.ToArray();
        }



        public void DeleteAllIndex()
        {
            IndexWriter _indexWriter = new IndexWriter(FSDirectory.Open(IndexDir), Analyzer, true, IndexWriter.MaxFieldLength.LIMITED);
            _indexWriter.DeleteAll();//删除所有的索引
            //writer.DeleteDocuments(new Term("key", "val"));//删除该词条
            //                                               //注意 在执行这个删除操作的时候，其实lucene本身并没有将数据从硬盘删除，而是保存到了一个单独的后缀名为.del的文件中。
            _indexWriter.Commit();//进行提交操作 标记为删除的索引会被从硬盘删除
            _indexWriter.Dispose();

        }


        public void Add(DataItem[] items)
        {
            IndexWriter _indexWriter = new IndexWriter(FSDirectory.Open(IndexDir), Analyzer, true, IndexWriter.MaxFieldLength.LIMITED);
            foreach (var item in items)
            {
                //Index         Store	TermVector	            用法
                //NOT_ANSLYZED	YES	    NO	                    文件名、主键
                //ANSLYZED	    YES	    WITH_POSITUION_OFFSETS	标题、摘要
                //ANSLYZED	    NO	    WITH_POSITUION_OFFSETS	很长的全文
                //NO	        YES	    NO	                    文档类型
                //NOT_ANSLYZED	NO	    NO	                    隐藏的关键词
                Document doc = new Document();
                doc.Add(new Field("Id", $"{item.PluginId}-{item.Name}", Field.Store.YES, Field.Index.NOT_ANALYZED, Field.TermVector.NO));
                doc.Add(new Field("PluginId", item.PluginId, Field.Store.YES, Field.Index.NOT_ANALYZED, Field.TermVector.NO));
                doc.Add(new Field("Name", item.Name, Field.Store.YES, Field.Index.ANALYZED));
                doc.Add(new Field("Tip", item.Tip, Field.Store.YES, Field.Index.ANALYZED));
                doc.Add(new Field("IconPath", item.IconPath, Field.Store.YES, Field.Index.NOT_ANALYZED));
                doc.Add(new Field("Body", item.Body, Field.Store.YES, Field.Index.ANALYZED));
                _indexWriter.AddDocument(doc);
            }
            _indexWriter.Commit();
            _indexWriter.Optimize();
            _indexWriter.Dispose();
        }

        public void Update(DataItem[] items)
        {
            IndexWriter _indexWriter = new IndexWriter(FSDirectory.Open(IndexDir), Analyzer, true, IndexWriter.MaxFieldLength.LIMITED);
            foreach (var item in items)
            {
                var id = $"{item.PluginId}-{item.Name}";
                Term term = new Term("Id", id);
                Document doc = new Document();
                doc.Add(new Field("Id", id, Field.Store.YES, Field.Index.NO));
                doc.Add(new Field("PluginId", item.PluginId, Field.Store.YES, Field.Index.NOT_ANALYZED));
                doc.Add(new Field("Name", item.Name, Field.Store.YES, Field.Index.ANALYZED));
                doc.Add(new Field("Tip", item.Tip, Field.Store.YES, Field.Index.ANALYZED));
                doc.Add(new Field("IconPath", item.IconPath, Field.Store.YES, Field.Index.NOT_ANALYZED));
                doc.Add(new Field("Body", item.Body, Field.Store.YES, Field.Index.ANALYZED));
                _indexWriter.UpdateDocument(term, doc);

            }
            _indexWriter.Commit();
            _indexWriter.Optimize();
            _indexWriter.Dispose();
        }

        public void DeleteIndexByPluginId(string pluginId)
        {
            IndexWriter _indexWriter = new IndexWriter(FSDirectory.Open(IndexDir), Analyzer, true, IndexWriter.MaxFieldLength.LIMITED);
            Term term = new Term("PluginId", pluginId);
            _indexWriter.DeleteDocuments(term);
            _indexWriter.Commit();
            _indexWriter.Optimize();
            _indexWriter.Dispose();
        }


    }
}
