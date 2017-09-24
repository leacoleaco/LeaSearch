using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        private readonly IndexWriter _indexWriter = new IndexWriter(FSDirectory.Open(IndexDir), Analyzer, true, IndexWriter.MaxFieldLength.LIMITED);
        private readonly IndexSearcher _indexSearcher = new IndexSearcher(FSDirectory.Open(IndexDir), true);

        ~LuceneManager()
        {
            _indexWriter.Commit();
            _indexWriter.Optimize();
            _indexWriter.Dispose();
        }


        public void Search(string keyword, int num)//搜索
        {
            QueryParser qp = new QueryParser(Lucene.Net.Util.Version.LUCENE_30, "Name", Analyzer);

            //拼装查询语句
            Query query = qp.Parse(keyword);
            //Console.WriteLine("query> {0}", query);

            //返回命中数量
            TopDocs tds = _indexSearcher.Search(query, 10);
            //Console.WriteLine("TotalHits: " + tds.TotalHits);
            foreach (ScoreDoc sd in tds.ScoreDocs)
            {
                //Console.WriteLine(sd.Score);
                Document doc = _indexSearcher.Doc(sd.Doc);
                //Console.WriteLine(doc.Get("body"));
            }

            _indexSearcher.Dispose();
        }



        public void DeleteAllIndex()
        {
            _indexWriter.DeleteAll();//删除所有的索引
            //writer.DeleteDocuments(new Term("key", "val"));//删除该词条
            //                                               //注意 在执行这个删除操作的时候，其实lucene本身并没有将数据从硬盘删除，而是保存到了一个单独的后缀名为.del的文件中。
            _indexWriter.Commit();//进行提交操作 标记为删除的索引会被从硬盘删除

        }


        //添加文档到索引中
        private void AddTextToIndex(DataItem item)
        {
            Document doc = new Document();
            doc.Add(new Field("PluginId", item.PluginId, Field.Store.YES, Field.Index.NOT_ANALYZED));
            doc.Add(new Field("Name", item.Name, Field.Store.YES, Field.Index.ANALYZED));
            doc.Add(new Field("IconPath", item.IconPath, Field.Store.YES, Field.Index.NOT_ANALYZED));
            doc.Add(new Field("Body", item.Body, Field.Store.YES, Field.Index.ANALYZED));
            _indexWriter.AddDocument(doc);
        }

       
    }
}
