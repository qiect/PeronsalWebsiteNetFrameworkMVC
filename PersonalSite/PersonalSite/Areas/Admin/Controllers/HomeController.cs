using BLL;
using Model;
using Newtonsoft.Json;
using PersonalSite.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PersonalSite.Areas.Admin.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Admin/Home/
        #region 页面代码
        public ActionResult Index()
        {
            return View();
        }

        //引用LayUI后，将一下部分页修改为视图页，不在使用无刷新页面
        //update:PartialView更改为View
        [HttpGet]
        public ActionResult AdMgr()
        {
            return View();
        }
        [HttpPost]
        public ActionResult AdMgr(int page, int limit)
        {
            Result<T_Ads> result = new Result<T_Ads>();
            T_AdsBLL bll = new T_AdsBLL();
            result.Status = 0;
            result.Data = bll.GetListPages<T_Ads>(page, limit, null, "Id desc").ToList();
            result.Total = bll.GetList(null).Count();
            result.Message = "";
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ModComments()
        {
            return View();
        }
        [HttpPost]
        public ActionResult ModComments(int page, int limit)
        {
            Result<T_Comments> result = new Result<T_Comments>();
            T_CommentsBLL bll = new T_CommentsBLL();
            result.Status = 0;
            result.Data = bll.GetListPages<T_Comments>(page, limit, "where IsVisible=@IsVisible", "PostDate desc", parameters: new { IsVisible = false }).ToList();
            result.Total = bll.GetList(new { IsVisible = false }).Count();
            return Json(result, JsonRequestBehavior.AllowGet);
        }


        public ActionResult FilterWordsMgr()
        {
            return View();
        }
        [HttpPost]
        public ActionResult FilterWordsMgr(int page, int limit)
        {
            Result<T_FilterWords> result = new Result<T_FilterWords>();
            T_FilterWordsBLL bll = new T_FilterWordsBLL();
            result.Status = 0;
            result.Data = bll.GetListPages<T_FilterWords>(page, limit, null, "Id desc").ToList();
            result.Total = bll.GetList(null).Count();
            result.Message = "";
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ChannelMgr()
        {
            return View();
        }
        [HttpPost]
        public ActionResult ChannelMgr(int page, int limit)
        {
            Result<T_Channels> result = new Result<T_Channels>();
            T_ChannelsBLL bll = new T_ChannelsBLL();
            result.Status = 0;
            result.Data = bll.GetListPages<T_Channels>(page, limit, null, "Id desc").ToList();
            result.Total = bll.GetList(null).Count();
            result.Message = "";
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region ArticleMgr
        [HttpGet]
        public ActionResult ArticleMgr()
        {
            return View();
        }
        [HttpPost]
        public ActionResult ArticleMgr(int page, int limit)
        {
            var bll = new T_ArticlesBLL();
            var list = bll.GetListPagesSimple<T_ArticlesSimple>(page, limit, null, "PostDate desc").ToList();
            Result<T_ArticlesSimple> result = new Result<T_ArticlesSimple>();
            result.Status = 0;
            result.Message = "";
            result.Total = bll.GetListSimple(null).Count();
            result.Data = list.ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult PostArticle()
        {
            //绑定频道
            T_ChannelsBLL chaBll = new T_ChannelsBLL();
            ViewBag.Channel = chaBll.GetList(null);
            string action = Request["action"];
            if (action == "edit")
            {
                int id = Convert.ToInt32(Request["id"]);
                T_ArticlesBLL bll = new T_ArticlesBLL();
                var art = bll.GetModel(id);
                return View(art);

            }
            else if (action == "addnew")
            {

            }
            else
            {
                //Response.Write("参数错误");
                return Content("参数错误");
            }
            return View();
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult PostArticle(T_Articles model)
        {
            //提交文章
            string act = Request.UrlReferrer.Query;
            if (act.Contains("edit"))
            {
                //如果action=edit则是编辑一条数据
                string strId = act.Split(new string[] { "id=" }, System.StringSplitOptions.RemoveEmptyEntries)[1];
                int id = Convert.ToInt32(strId);
                T_ArticlesBLL bll = new T_ArticlesBLL();
                //先从数据库取出旧数据
                var art = bll.GetModel(id);
                //从界面读取值更新model
                art.ChannelId = model.ChannelId;
                art.Title = model.Title;
                art.Msg = model.Msg;
                art.DingCount = model.DingCount;
                art.ComCount = model.ComCount;
                //把对model的修改保存到数据库中
                bll.ModifyDb(art);

                bll.StaticArticle(art.Id);//保存修改的时候重新生成静态页

                //IndexManager.GetInstance().AddArticle(art.Id);

                //返回列表界面
                //return Redirect("/Admin/Home/ArticleMgr");
                return View("Index");
            }
            else if (act.Contains("addnew"))
            {
                T_Articles article = new T_Articles();
                article.ChannelId = model.ChannelId;
                article.Msg = model.Msg;
                article.PostDate = DateTime.Now;
                article.Title = model.Title;
                article.DingCount = model.DingCount;
                article.ComCount = model.ComCount;
                T_ArticlesBLL bll = new T_ArticlesBLL();
                int id = bll.InsertDb(article);

                bll.StaticArticle(id);//保存新增的时候生成静态页

                //把任务加入索引库中
                //IndexManager.GetInstance().AddArticle(article.Id);

                //return Redirect("/Admin/Home/ArticleMgr");
                return View("Index");
            }
            else
            {
                return View("error");
            }
        }

        public ActionResult RequestArticles()
        {
            ResultModel<T_Articles> result = new ResultModel<T_Articles>();
            result.status = 1;
            string strJson = Request["data"], action = Request["action"];
            if (string.IsNullOrEmpty(action))
            {
                result.msg = "请求数据错误";
            }
            T_ArticlesBLL bll = new T_ArticlesBLL();
            switch (action)
            {
                case "check":
                    {
                        var artData = JsonConvert.DeserializeObject<IEnumerable<T_Articles>>(strJson);
                        foreach (var item in artData)
                        {
                            bll.CheckById(item);
                        }
                        result.status = 0;
                        result.msg = "已审核";
                        break;
                    }
                case "delete":
                    {
                        var artData = JsonConvert.DeserializeObject<T_Articles>(strJson);
                        bll.DeleteDb(artData.Id);
                        result.status = 0;
                        result.msg = "删除成功";
                        break;
                    }
                case "batchStatic":
                    {
                        bll.StaticAllArticle();
                        result.status = 0;
                        result.msg = "静态化成功";
                        break;
                    }
                case "batchdel":
                    {
                        var artData = JsonConvert.DeserializeObject<IEnumerable<T_Articles>>(strJson);
                        foreach (var item in artData)
                        {
                            bll.DeleteDb(item);
                        }
                        result.status = 0;
                        result.msg = "批量删除成功";
                        break;
                    }
                case "addpagedata":
                    {
                        bll.AddPageDataByRule(12, null, null, null);
                        break;
                    }
                case "createindexall":
                    {
                        if (bll.CreateIndexAll())
                        {
                            result.status = 0;
                            result.msg = "全部创建索引成功";
                        }
                        else
                        {
                            result.msg = "全部创建索引失败";
                        }
                        break;
                    }
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region RequestFilterWords
        public ActionResult RequestFilterWords()
        {
            T_FilterWordsBLL bll = new T_FilterWordsBLL();
            string action = Request["action"],
                id = Request["id"],
                word = Request["word"],
                replace = Request["replace"],
                msg = "";
            if (string.IsNullOrEmpty(action))
            {
                Response.Write("请求非法！");
            }
            switch (action)
            {
                case "add":
                    {
                        msg = Add(word, replace);
                        bll.ClearCache();//对数据做了增删改，则需要清理过滤词缓存
                        break;
                    }
                case "edit":
                    {
                        msg = "";
                        bll.ClearCache();
                        break;
                    }
                case "delete":
                    {
                        msg = bll.DeleteDb(Convert.ToInt32(id)).ToString();
                        bll.ClearCache();
                        break;
                    }
                case "import":
                    {
                        msg = "";
                        break;
                    }
                case "export":
                    {
                        Export();
                        msg = "ok";
                        break;
                    }
            }
            return Content(msg);
        }

        private string Add(string word, string replace)
        {
            T_FilterWords fw = new T_FilterWords();
            fw.WordPattern = word;
            fw.ReplaceWord = replace;
            T_FilterWordsBLL bll = new T_FilterWordsBLL();
            var result = bll.InsertDb(fw);
            return JsonConvert.SerializeObject(result);
        }


        /// <summary>
        /// 导出
        /// </summary>
        /// <returns></returns>
        public ActionResult Export()
        {
            T_FilterWordsBLL bll = new T_FilterWordsBLL();
            //context.Response.AddHeader("Content-Disposition", "attachment");
            //context.Response.AddHeader("Content-Disposition", "attachment;filename=word.txt");
            string encodeFileName = HttpUtility.UrlEncode("过滤词.txt");
            //在浏览器弹出下载对话框保存Response，而不是直接显示到浏览器
            //filename设置默认文件名
            HttpContext.Response.AddHeader("Content-Disposition", string.Format("attachment;filename=\"{0}\"", encodeFileName));
            var filterwords = bll.GetList(null);//因为数据量不大，所以也没用SqlDataReader
            foreach (var filterword in filterwords)
            {
                return Content(filterword.WordPattern + "=" + filterword.ReplaceWord + "\r\n");
            }
            return new EmptyResult();
            //不要这样写：
            //File.WriteAllText(HttpContext.Current.Server.MapPath("~/1.txt"),"asfasfdasfasdfsa");
            //HttpContext.Current.Response.Redirect("/1.txt");
        }
        /// <summary>
        /// 导入
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult UploadFile(HttpPostedFileBase file)
        {
            T_FilterWordsBLL bll = new T_FilterWordsBLL();
            bll.Import(file.InputStream);
            //bll.ClearCache();//注意只有通过ListView控件新增的时候，Inserted事件才会触发。所以这种通过代码导入的方式，还是要手动清理缓存
            return Content("导入成功");
        }
        #endregion

    }


    public class Result<T>
    {
        public int Status { get; set; }

        public string Message { get; set; }

        public int Total { get; set; }

        public List<T> Data { get; set; }
    }
}