using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Lucene.Net.Store;
using System.IO;
using Lucene.Net.Index;
using Lucene.Net.Analysis.PanGu;
using System.Net;
using Lucene.Net.Documents;
using log4net;
using Lucene.Net.Search;
using System.Text;
using PanGu;
using System.Xml.Linq;
using System.Text.RegularExpressions;
using System.Collections;

namespace PersonSite.Search
{
    public class SearchBLL
    {
        public IEnumerable<SearchResult> Search(string kw, int startIndex, int pageSize, out int totalCount)
        {
            string indexPath = AppDomain.CurrentDomain.BaseDirectory.ToString() + "Search\\index";
            FSDirectory directory = FSDirectory.Open(new DirectoryInfo(indexPath), new NoLockFactory());
            IndexReader reader = IndexReader.Open(directory, true);
            IndexSearcher searcher = new IndexSearcher(reader);
            PhraseQuery query = new PhraseQuery();

            //todo:把用户输入的关键词进行拆词

            foreach (string word in CommonHelper.SplitWord(kw))//先用空格，让用户去分词，空格分隔的就是词“计算机 专业”
            {
                query.Add(new Term("body", word));
            }

            query.SetSlop(100);
            TopScoreDocCollector collector = TopScoreDocCollector.create(1000, true);
            searcher.Search(query, null, collector);
            totalCount = collector.GetTotalHits();//返回总条数
            ScoreDoc[] docs = collector.TopDocs(startIndex, pageSize).scoreDocs;
            List<SearchResult> listResult = new List<SearchResult>();
            for (int i = 0; i < docs.Length; i++)
            {
                int docId = docs[i].doc;//取到文档的编号（主键，这个是Lucene .net分配的）
                //检索结果中只有文档的id，如果要取Document，则需要Doc再去取
                //降低内容占用
                Document doc = searcher.Doc(docId);//根据id找Document
                string number = doc.Get("number");
                string title = doc.Get("title");
                string staticPath = doc.Get("staticPath");
                string body = doc.Get("body");

                SearchResult result = new SearchResult();
                result.Number = number;
                result.Title = title;
                result.StaticPath = staticPath;

                result.BodyPreview = Preview(body, kw);

                listResult.Add(result);
            }
            return listResult;
        }
        private static string Preview(string body, string keyword)
        {
            //创建HTMLFormatter,参数为高亮单词的前后缀 
            PanGu.HighLight.SimpleHTMLFormatter simpleHTMLFormatter =
                   new PanGu.HighLight.SimpleHTMLFormatter("<font color=\"red\">", "</font>");
            //创建 Highlighter ，输入HTMLFormatter 和 盘古分词对象Semgent 
            PanGu.HighLight.Highlighter highlighter =
                            new PanGu.HighLight.Highlighter(simpleHTMLFormatter,
                            new Segment());
            //设置每个摘要段的字符数 
            highlighter.FragmentSize = 100;
            //获取最匹配的摘要段 
            String bodyPreview = highlighter.GetBestFragment(keyword, body);
            return bodyPreview;
        }
    }
}