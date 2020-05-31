using DAL;
using Helper;
using Model;
using PersonalSite.Helper;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using static PersonalSite.DAL.SqlHelper;

namespace BLL
{
    public partial class T_ArticlesBLL
    {


        /// <summary>
        /// 审核文章
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool CheckById(T_Articles model)
        {
            model = GetModel(model.Id);
            model.IsVisible = true;
            if (ModifyDb(model) > 0)
            {
                StaticArticle(model.Id);
                return true;
            }
            else
            {
                return false;
            }


        }

        /// <summary>
        /// 给文章点赞
        /// </summary>
        /// <param name="id"></param>
        public bool Ding(int id)
        {
            var model = GetModel(id);
            model.DingCount += 1;
            return ModifyDb(model) > 0 ? true : false;
        }
        /// <summary>
        /// 评论条数
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool ComCount(int id)
        {
            var model = GetModel(id);
            model.ComCount += 1;
            return ModifyDb(model) > 0 ? true : false;
        }

        /// <summary>
        /// 获取人气最高的文章
        /// </summary>
        /// <returns></returns>
        public IEnumerable<T_ArticlesSimple> GetTop()
        {
            return new T_ArticlesDAL().GetListSimple(new { IsFirst = true });
        }

        /// <summary>
        /// 页面全部创建索引
        /// </summary>
        /// <returns></returns>
        public bool CreateIndexAll()
        {
            PanGuLuceneHelper.instance.DeleteAll();
            List<MySearchUnit> list = new List<MySearchUnit>();
            var articleList = GetList(null).ToList();
            foreach (var article in articleList)
            {
                list.Add(new MySearchUnit(article.Id.ToString(), article.Title, article.Msg, article.StaticPath, "", article.PostDate.ToString()));
            }
            return PanGuLuceneHelper.instance.CreateIndex(list);
        }

        /// <summary>
        /// 页面全部静态化
        /// </summary>
        public void StaticAllArticle()
        {
            List<T_ArticlesSimple> arts = GetListSimple(null);
            foreach (T_ArticlesSimple art in arts)
            {
                StaticArticle(art.Id);
                //IndexManager.GetInstance().AddArticle(art.Id);
            }
        }

        /// <summary>
        /// 为某篇文章静态化
        /// </summary>
        /// <param name="artId"></param>
        public void StaticArticle(int artId)
        {
            var art = GetModel(artId);
            string staticPath;
            //如果StaticPath为空，则是第一次生成静态页
            if (string.IsNullOrEmpty(art.StaticPath))
            {
                SqlParameter pName = new SqlParameter();
                pName.SqlDbType = System.Data.SqlDbType.NVarChar;
                pName.ParameterName = "@SeqName";
                pName.Value = "静态页面流水号";

                SqlParameter pResult = new SqlParameter();
                pResult.SqlDbType = System.Data.SqlDbType.Int;
                pResult.ParameterName = "@result";
                pResult.Direction = System.Data.ParameterDirection.Output;

                //调用存储过程usp_getSeqNo生成当日顺序流水号
                ExecuteStoredProcedure("usp_getSeqNo", pName, pResult);
                int seqNo = (int)pResult.Value;
                //要生成的静态页面的保存路径
                //注意是发布日期，不是当前日期
                staticPath = art.PostDate.ToString(@"yyyy\/MM\/dd\/") + seqNo + ".htm";

                art.StaticPath = staticPath;
                ModifyDb(art);//把生成的静态路径保存在数据库中
            }
            else//如果不为空，则表示是重新生成静态页，重新生成不应该改变路径，复用旧的staticPath
            {
                staticPath = art.StaticPath;
            }


            string localPath = HttpContext.Current.Server.MapPath("~/Art/" + staticPath); //HostingEnvironment.MapPath();
            string downStaticPage = "";
            //开发时加载开发环境的静态页下载地址
            if (!CommonHelper.GetAppSetting("IsDebug").Equals("true"))
            {
                downStaticPage = "http://hlj3.cn/Article/Detail/";
            }
            else
            {
                downStaticPage = "http://localhost:2148/Article/Detail/";
            }
            //string downStaticPage = ConfigurationManager.AppSettings["downStaticPage"];
            //创建文件夹
            Directory.CreateDirectory(Path.GetDirectoryName(localPath));

            WebClient wc = new WebClient();
            wc.Encoding = Encoding.UTF8;
            //通过WebClient向服务器发Get请求，把服务器返回的html内容保存到磁盘上。以后用户直接请html文件请求。
            //todo:静态化下载文件地址做成可配置的
            wc.DownloadFile(downStaticPage + art.Id, localPath);

            //保存修改的时候先删除这篇文章的索引
            PanGuLuceneHelper.instance.Delete(art.Id.ToString());
            //再创建索引，避免数据重复
            PanGuLuceneHelper.instance.CreateIndex(new MySearchUnit(art.Id.ToString(), art.Title, art.Msg, art.StaticPath, "", art.PostDate.ToString()));

        }

        /// <summary>
        /// 获取列表根据条件
        /// </summary>
        /// <param name="whereConditions">匿名条件 eg：new {Category = 1, SubCategory=2}</param></param>
        /// <returns></returns>
        public List<T_ArticlesSimple> GetListSimple(object whereConditions)
        {
            return new T_ArticlesDAL().GetListSimple(whereConditions);
        }
        /// <summary>
        /// 分页查询
        /// </summary>
        /// <param name="pageNumber">第几页</param>
        /// <param name="rowsPerPage">分页大小</param>
        /// <param name="conditions">带@的条件</param>
        /// <param name="orderby">排序 默认按主键</param>
        /// <param name="parameters">上面设置条件后对应匿名 不能单独写 必需跟conditions一起</param>
        /// <returns></returns>
        public IEnumerable<T_ArticlesSimple> GetListPagesSimple<T_ArticlesSimple>(int pageNumber, int rowsPerPage, string conditions = null, string orderby = null, object parameters = null)
        {
            return new T_ArticlesDAL().GetListPagesSimple<T_ArticlesSimple>(pageNumber, rowsPerPage, conditions, orderby, parameters);
        }


        /// <summary>
        /// 录入页面数据通过规则
        /// </summary>
        /// <param name="channelId">频道ID</param>
        /// <param name="url">录入的页面URL</param>
        /// <param name="matchRule">页面规则</param>
        /// <param name="matchContentRule">内容规则</param>
        /// <param name="maxCount">最大条数</param>
        //todo:规则模块，可以配置规则添加页面数据
        public void AddPageDataByRule(int channelId, string url, string matchRule, string matchContentRule, int maxCount = 250)
        {
            //从配置文件获取文章规则Url
            string artRuleUrl = CommonHelper.GetAppSetting("artRuleUrl");
            string hrefRegex = CommonHelper.GetAppSetting("hrefRegex");
            string contentRegex = CommonHelper.GetAppSetting("contentRegex");

            var data = GetHtmlStr(artRuleUrl, "gb2312");
            string temp = data.Substring(3300).Replace("\r", " ").Replace("\n", " ");
            var matches = Regex.Matches(temp, hrefRegex, RegexOptions.IgnoreCase | RegexOptions.Multiline);
            T_Articles article = new T_Articles();
            if (matches.Count < maxCount)
            {
                for (int i = 0; i < matches.Count; i++)
                {
                    Match match = matches[i];
                    string href = match.Groups[2].Value;
                    var content = GetHtmlStr(href, "gb2312");
                    var contentMatches = Regex.Match(content.Replace("\r", " ").Replace("\n", " "), contentRegex);
                    var contentTemp = contentMatches.Value.Remove(0, 109).Replace("</strong>", "").Replace("<font color=\"#ff0000\">", "").Replace("<font color=\"#0000ff\">", "").Replace("<font face=\"黑体\">", "").Replace("<font size=\"5\">", "").Replace("<font >", "").Replace("<strong>", "").Replace("</strong>", "").Replace("</font>", "").Replace("<br /> 　　", "\n").Replace("<br />", "\n");
                    contentTemp = contentTemp.Remove(contentTemp.Length - 105, 105);

                    article.Title = match.Groups[4].Value;
                    article.PostDate = DateTime.Now;
                    article.ChannelId = channelId;
                    string tempContent = contentTemp;
                    article.Msg = tempContent;
                    article.UserId = "e804a846-ae14-4721-bc9a-3e9ad1245abd";
                    InsertDb(article);
                }
            }
        }

        /// <summary>  
        /// 获取网页的HTML码  
        /// </summary>  
        /// <param name="url">链接地址</param>  
        /// <param name="encoding">编码类型</param>  
        /// <returns></returns>  
        public string GetHtmlStr(string url, string encoding)
        {
            string htmlStr = "";
            try
            {
                if (!String.IsNullOrEmpty(url))
                {
                    WebRequest request = WebRequest.Create(url);            //实例化WebRequest对象  
                    WebResponse response = request.GetResponse();           //创建WebResponse对象  
                    Stream datastream = response.GetResponseStream();       //创建流对象  
                    Encoding ec = Encoding.Default;
                    if (encoding == "UTF8")
                    {
                        ec = Encoding.UTF8;
                    }
                    else if (encoding == "Default")
                    {
                        ec = Encoding.Default;
                    }
                    StreamReader reader = new StreamReader(datastream, ec);
                    htmlStr = reader.ReadToEnd();                  //读取网页内容  
                    reader.Close();
                    datastream.Close();
                    response.Close();
                }
            }
            catch { }
            return htmlStr;
        }

    }
}
