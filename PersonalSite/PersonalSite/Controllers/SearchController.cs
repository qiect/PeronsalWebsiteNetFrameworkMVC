using PersonalSite.Helper;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace PersonalSite.Controllers
{
    public class SearchController : Controller
    {
        //
        // GET: /Search/
        public ActionResult Index(string kw, string pageIndex)
        {
            int count;
            int index = Convert.ToInt32(pageIndex);
            int pageSize = 10;
            if (string.IsNullOrEmpty(kw))
            {
                return View("error");
            }
            var pager = new PagerHelper();
            List<MySearchUnit> searchlist = PanGuLuceneHelper.instance.Search("", PanGuLuceneHelper.instance.Token(kw), index, pageSize, out count);
            ViewBag.Hint = count;
            /*可选参数*/
            pager.SetIsEnglish = false;// 是否是英文       (默认：false)
            pager.SetIsShowText = false;//是否显示分页文字 (默认：true)
            pager.SetTextFormat = "<span class=\"pagetext\"><strong>总共</strong>:{0} 条 <strong>当前</strong>:{1}/{2}</span><br/>";
            pager.SetClassName = "layui-box layui-laypage layui-laypage-default";

            /*函数参数*/
            var page = pager.ToString(count, pageSize, index, "Index?kw=" + kw + "&");

            ViewBag.PagerHtml = page;
            return View(searchlist);
        }
    }
}