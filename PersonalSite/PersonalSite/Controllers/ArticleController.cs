using BLL;
using Microsoft.AspNet.Identity;
using Model;
using Newtonsoft.Json;
using PersonalSite.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;

namespace PersonalSite.Controllers
{
    public class ArticleController : Controller
    {
        int pageRow = 10;
        T_ArticlesBLL bll = new T_ArticlesBLL();
        //
        // GET: /Article/

        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 数据列表(频道id)
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [ChildActionOnly]
        public ActionResult List(int id)
        {
            IEnumerable<T_ArticlesSimple> result;
            switch (id)
            {
                case 0:
                    result = bll.GetListPagesSimple<T_ArticlesSimple>(1, pageRow, "where IsVisible=1", "PostDate desc").ToList();
                    break;
                case 1:
                    result = bll.GetListPagesSimple<T_ArticlesSimple>(1, pageRow, "where ( ChannelId='1' or ChannelId='2' or ChannelId='3' or ChannelId='4' ) and IsVisible=1", "PostDate desc").ToList();
                    break;
                case 12:
                    result = bll.GetListPagesSimple<T_ArticlesSimple>(1, pageRow, "where ChannelId=@ChannelId and IsVisible=1", "PostDate asc", parameters: new { ChannelId = id }).ToList();
                    break;
                default:
                    result = bll.GetListPagesSimple<T_ArticlesSimple>(1, pageRow, "where ChannelId=@ChannelId and IsVisible=1", "PostDate desc", parameters: new { ChannelId = id }).ToList();
                    break;

            }
            TempData["ChannelId"] = id;
            return PartialView(result);
        }

        /// <summary>
        /// 加载数据
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        public ActionResult AddData(int id = 0, int pageIndex = 2)
        {
            IEnumerable<T_ArticlesSimple> result;
            switch (id)
            {
                case 0:
                    result = bll.GetListPagesSimple<T_ArticlesSimple>(pageIndex, pageRow, "where IsVisible=1", "PostDate desc").ToList();
                    break;
                case 1:
                    result = bll.GetListPagesSimple<T_ArticlesSimple>(pageIndex, pageRow, "where ( ChannelId='1' or ChannelId='2' or ChannelId='3' or ChannelId='4' ) and IsVisible=1", "PostDate desc").ToList();
                    break;
                case 12:
                    result = bll.GetListPagesSimple<T_ArticlesSimple>(pageIndex, pageRow, "where ChannelId=@ChannelId and IsVisible=1", "PostDate asc", parameters: new { ChannelId = id }).ToList();
                    break;
                default:
                    result = bll.GetListPagesSimple<T_ArticlesSimple>(pageIndex, pageRow, "where ChannelId=@ChannelId and IsVisible=1", "PostDate desc", parameters: new { ChannelId = id }).ToList();
                    break;
            }
            return PartialView(result);
        }

        /// <summary>
        /// 本周热议
        /// </summary>
        /// <returns></returns>
        public ActionResult ListHot()
        {
            var result = bll.GetListSimple(null).AsQueryable().Where(p => p.IsVisible == true).OrderByDescending(p => p.ComCount).Take(15).ToList();
            return PartialView(result);

        }
        /// <summary>
        /// 置顶
        /// </summary>
        /// <returns></returns>
        public ActionResult ListTop()
        {
            return PartialView(bll.GetTop());
        }
        /// <summary>
        /// 文章详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Detail(int id)
        {
            var article = bll.GetModel(id);
            if (article == null)
            {
                return View("Error");
            }
            return View(article);
        }
        /// <summary>
        /// 发表文章
        /// </summary>
        /// <returns></returns>
        public ActionResult Add()
        {
            //绑定频道
            T_ChannelsBLL chaBll = new T_ChannelsBLL();
            ViewBag.Channel = chaBll.GetList(null);
            return View();
        }
        /// <summary>
        /// 发表文章Post
        /// </summary>
        /// <returns></returns>
        [ValidateInput(false)]
        public ActionResult AddArticle()
        {
            ResultModel<T_Articles> result = new ResultModel<T_Articles>();
            result.status = 1;
            var form = Request.Form;
            if (Session["VCode"].ToString() != form["vercode"])
            {
                result.msg = "验证码不正确";
                result.action = "/Article/Add";
                return Json(result, JsonRequestBehavior.AllowGet);
            }

            string filterResult = FilterArticle(Convert.ToInt32(form["channel"]), form["title"], form["content"]);
            switch (filterResult)
            {
                case "ok":
                    result.status = 0;
                    result.msg = "发表成功，待审核";
                    result.action = "/Article/Add"; break;
                case "mod":
                    result.msg = "内容中包含违禁词，待审核"; break;
                case "banned":
                    result.msg = "内容含有禁用词汇,请注意文明用语！"; break;
                default:
                    result.msg = "服务器返回错误：" + filterResult; break;
            }
            return Json(result, behavior: JsonRequestBehavior.AllowGet);

        }
        /// <summary>
        /// 动态
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Trends(int id)
        {
            var result = bll.GetListPages<T_Articles>(id, 5, "where ChannelId='11'", "PostDate desc").ToList();
            return View(result);
        }
        /// <summary>
        /// 回复
        /// </summary>
        /// <returns></returns>
        public ActionResult Comments()
        {
            ResultModel<T_Comments> result = new ResultModel<T_Comments>();
            result.status = 0;
            if (Session["VCode"].ToString() != Request["vercode"])
            {
                result.status = 1;
                result.msg = "验证码不正确";
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            int articleId = Convert.ToInt32(Request["artId"]);
            string msg = Request["content"];
            //int articleId = Convert.ToInt32(RouteData.Values["id"]);
            //int articleId = Convert.ToInt32(Request.UrlReferrer.Segments[3]);
            //防止用户提交有潜在危险的html代码，防止XSS（跨站脚本漏洞）
            if (msg.Contains("<") || msg.Contains(">"))
            {
                result.status = 1;
                result.msg = "服务器返回错误";
            }

            T_FilterWordsBLL filterBll = new T_FilterWordsBLL();
            string replaceMsg;

            //对用户输入的评论进行过滤处理
            FilterResult filterResult = filterBll.FilterMsg(msg, out replaceMsg);
            if (filterResult == FilterResult.OK)
            {
                T_Comments comment = new T_Comments();
                comment.ArticleId = articleId;
                comment.Msg = replaceMsg;
                comment.PostDate = DateTime.Now;
                comment.IsVisible = true;

                T_CommentsBLL commentBll = new T_CommentsBLL();
                commentBll.InsertDb(comment);
                //插入指定文章的回复条数+1
                bll.ComCount(articleId);

            }
            else if (filterResult == FilterResult.Mod)
            {
                //含有审核词则向数据库中插入，但是IsVisible=false，需要审核设置为true才能显示
                T_Comments comment = new T_Comments();
                comment.ArticleId = Convert.ToInt32(articleId);
                comment.Msg = replaceMsg;
                comment.PostDate = DateTime.Now;
                comment.IsVisible = false;//需要审核

                T_CommentsBLL commentBll = new T_CommentsBLL();
                commentBll.InsertDb(comment);
                //插入指定文章的回复条数+1
                bll.ComCount(articleId);
                result.status = 1;
                result.msg = "等待审核";
            }
            else if (filterResult == FilterResult.Banned)
            {
                //如果含有禁用词，则不向数据库中插入
                result.status = 1;
                result.msg = "您的评论内容含有禁用词汇，请注意文明用语！";
            }
            return Json(result, JsonRequestBehavior.AllowGet);

        }
        /// <summary>
        /// 获取回复列表
        /// </summary>
        /// <param name="articleId"></param>
        /// <returns></returns>
        public ActionResult GetComments(int articleId)
        {
            T_CommentsBLL commentsBll = new T_CommentsBLL();
            var comments = commentsBll.GetList(new { ArticleId = articleId });
            return Content(JsonConvert.SerializeObject(comments));
        }
        /// <summary>
        /// 点赞
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult RateArticle(int id)
        {
            string ip = Request.UserHostAddress;

            //判断这个ip的用户在24小时之内是否针对这个文章做出了打分
            //select count(*) from T_VideoRates where IP=@IP and datediff(hour,CreateDate,now())<=24 and id=@id
            T_ArticleRatesBLL rateBll = new T_ArticleRatesBLL();
            ResultModel<T_ArticleRates> result = new ResultModel<T_ArticleRates>();
            if (rateBll.Get24HRateCount(ip, id) > 0)
            {
                result.status = 1;
                result.msg = "一天之内不能重复喜欢哦";
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            //评价记录插入Rate表
            T_ArticleRates rate = new T_ArticleRates();
            rate.Action = 1;
            rate.CreateDate = DateTime.Now;
            rate.IP = ip;
            rate.ArticleId = id;
            rateBll.InsertDb(rate);

            //更新文章的dingcount。通过冗余字段提高效率
            bll.Ding(id);
            result.msg = "您喜欢了它";
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 对用户发表的文章内容审核过滤
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="replaceMsg"></param>
        /// <returns></returns>
        public string FilterArticle(int channelId, string title, string msg)
        {
            T_FilterWordsBLL filterBll = new T_FilterWordsBLL();
            string replaceMsg;
            //对用户输入的评论进行过滤处理
            FilterResult result = filterBll.FilterMsg(msg, out replaceMsg);
            if (result == FilterResult.OK)
            {
                T_Articles article = new T_Articles();
                article.ChannelId = channelId;
                article.Title = title;
                article.Msg = replaceMsg;
                article.PostDate = DateTime.Now;
                article.IsVisible = false;
                //判断用户是否登录
                if (Request.IsAuthenticated)
                {
                    article.UserId = User.Identity.GetUserId();
                }
                else
                {
                    //未登录，默认guest用户
                    article.UserId = "9e03a459-17b6-4a04-aeee-c26e16ae0230";
                }
                bll.InsertDb(article);
                return "ok";

            }
            else if (result == FilterResult.Mod)
            {
                //含有审核词则向数据库中插入，但是IsVisible=false，需要审核设置为true才能显示
                T_Articles article = new T_Articles();
                article.ChannelId = channelId;
                article.Title = title;
                article.Msg = replaceMsg;
                article.PostDate = DateTime.Now;
                article.IsVisible = false;
                //判断用户是否登录
                if (Request.IsAuthenticated)
                {
                    article.UserId = User.Identity.GetUserId();
                }
                else
                {
                    //未登录，默认guest用户
                    article.UserId = "9e03a459-17b6-4a04-aeee-c26e16ae0230";
                }
                bll.InsertDb(article);
                return "mod";
            }
            else if (result == FilterResult.Banned)
            {
                //如果含有禁用词，则不向数据库中插入
                return "banned";

            }
            else
            {
                return "error";
            }
        }
        
    }
}