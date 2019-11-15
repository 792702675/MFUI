using Abp.Dependency;
using jieba.NET;
using JiebaNet.Segmenter;
using Lucene.Net.Analysis;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers.Classic;
using Lucene.Net.Search;
using Lucene.Net.Search.Highlight;
using Lucene.Net.Store;
using Lucene.Net.Util;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MF.LuceneNet
{
    public class LuceneNetManager : ISingletonDependency
    {
        private readonly IWebHostEnvironment _hostingEnvironment;
        public LuceneNetManager(IWebHostEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }
        /// <summary>
        /// 初始化索引
        /// </summary>
        public void InitIndex()
        {
            Analyzer analyze = new JieBaAnalyzer(TokenizerMode.Default);
            IndexWriterConfig _indexWriterConfig = new IndexWriterConfig(LuceneVersion.LUCENE_48, analyze);
            FSDirectory directory = FSDirectory.Open(new DirectoryInfo(_hostingEnvironment.ContentRootPath + "\\Lucene\\Index"));
            using (IndexWriter _indexWriter = new IndexWriter(directory, _indexWriterConfig))
            {
                _indexWriter.DeleteAll();
                foreach (var item in BookList())
                {
                    Document doc = new Document();
                    doc.Add(new StringField("Id", item.Id, Field.Store.YES));
                    doc.Add(new TextField("Author", item.Author, Field.Store.YES));
                    //doc.Add(new StringField("Author_String", item.Author, Field.Store.YES));
                    doc.Add(new TextField("Name", item.Name, Field.Store.YES));
                    //doc.Add(new StringField("Name_String", item.Name, Field.Store.YES));
                    doc.Add(new TextField("FileName", item.FileName, Field.Store.YES));
                    //doc.Add(new StringField("FileName_String", item.FileName, Field.Store.YES));
                    doc.Add(new TextField("Content", item.Content, Field.Store.YES));
                    _indexWriter.AddDocument(doc);
                }
                //_indexWriter.ForceMerge(1);
            }

            //https://www.cnblogs.com/dacc123/p/8431369.html
            //https://www.cnblogs.com/jesen1315/p/11065331.html
        }

        private object GetResultData(IndexSearcher searcher, TopDocs docs, Query query)
        {
            SimpleHTMLFormatter simpleHtmlFormatter = new SimpleHTMLFormatter("<span style='color:red;'>", "</span>");
            Highlighter highlighter = new Highlighter(simpleHtmlFormatter, new QueryScorer(query));

            highlighter.TextFragmenter = new SimpleFragmenter(150);
            Analyzer analyzer = new JieBaAnalyzer(TokenizerMode.Search);



            var result = new List<Book>();
            foreach (ScoreDoc sd in docs.ScoreDocs)
            {
                Document doc = searcher.Doc(sd.Doc);
                var author = highlighter.GetBestFragment(analyzer, "Author", doc.Get("Author"));
                if (string.IsNullOrWhiteSpace(author))
                {
                    author = doc.Get("Author");
                }
                var name = highlighter.GetBestFragment(analyzer, "Name", doc.Get("Name"));
                if (string.IsNullOrWhiteSpace(name))
                {
                    name = doc.Get("Name");
                }
                var fileName = highlighter.GetBestFragment(analyzer, "FileName", doc.Get("FileName"));
                if (string.IsNullOrWhiteSpace(fileName))
                {
                    fileName = doc.Get("FileName");
                }
                var content = highlighter.GetBestFragment(analyzer, "Content", doc.Get("Content"));
                if (string.IsNullOrWhiteSpace(content))
                {
                    content = doc.Get("Content");
                }
                result.Add(new Book()
                {
                    Id = doc.Get("Id"),
                    Author = author,
                    Name = name,
                    FileName = fileName,
                    Content = content
                });
            }

            return new { Items = result, TotalCount = docs.TotalHits };
        }

        public object ShowFields(string[] field, string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
            {
                return new { Items = new List<Book>(), TotalCount = 0 };
            }
            FSDirectory directory = FSDirectory.Open(new DirectoryInfo(_hostingEnvironment.ContentRootPath + "\\Lucene\\Index"));
            using (IndexReader reader = DirectoryReader.Open(directory))
            {
                IndexSearcher searcher = new IndexSearcher(reader);
                Analyzer analyzer = new JieBaAnalyzer(TokenizerMode.Default);
                QueryParser parser = new MultiFieldQueryParser(LuceneVersion.LUCENE_48, field, analyzer);
                Query query = parser.Parse(keyword);
                TopDocs docs = searcher.Search(query, null, 1000);

                return GetResultData(searcher, docs, query);
            }
        }

        public object ShowAdvanced(IEnumerable<MultiFieldInput> input)
        {
            FSDirectory directory = FSDirectory.Open(new DirectoryInfo(_hostingEnvironment.ContentRootPath + "\\Lucene\\Index"));
            using (IndexReader reader = DirectoryReader.Open(directory))
            {
                IndexSearcher searcher = new IndexSearcher(reader);
                Analyzer analyzer = new JieBaAnalyzer(TokenizerMode.Default);
                BooleanQuery bq = new BooleanQuery();
                foreach (var item in input)
                {
                    if (string.IsNullOrWhiteSpace(item.Keyword))
                    {
                        continue;
                    }
                    QueryParser parser = new QueryParser(LuceneVersion.LUCENE_48, item.Field, analyzer);
                    Query query = parser.Parse(item.Keyword);
                    bq.Add(query, item.Occur);
                }
                TopDocs docs = searcher.Search(bq, null, 1000);
                return GetResultData(searcher, docs, bq);
            }
        }

        public IEnumerable<Book> BookList()
        {
            var paths = System.IO.Directory.GetFiles(_hostingEnvironment.ContentRootPath + "\\Lucene\\File");
            var i = 0;
            foreach (var path in paths)
            {
                i++;
                Console.WriteLine(i);
                var fileName = new FileInfo(path).Name;
                var name = fileName.Split('-')[0];
                var author = fileName.Split('-')[1].Split('_')[0];
                var content = File.ReadAllText(path);
                if (content.Length > 10000)
                {
                    content = content.Substring(0, 10000);
                }
                yield return new Book()
                {
                    Id = i.ToString(),
                    Author = author,
                    Name = name,
                    FileName = fileName,
                    Content = content
                };
            }
        }
    }

    public class Book
    {
        public string Id { get; set; }
        public string Author { get; set; }
        public string Name { get; set; }
        public string FileName { get; set; }
        public string Content { get; set; }
    }
}
