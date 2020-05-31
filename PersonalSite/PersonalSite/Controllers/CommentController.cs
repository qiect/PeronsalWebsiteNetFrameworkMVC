using BLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PersonalSite.Controllers
{
    public class CommentController : Controller
    {
        // GET: Comment
        public ActionResult Index()
        {
            T_CommentsBLL commentsBll = new T_CommentsBLL();
            var comments = commentsBll.GetList(new { ArticleId = 1436 });
            return View(comments);
        }

        public ActionResult GetComments(int id)
        {
            T_CommentsBLL commentsBll = new T_CommentsBLL();
            var comments = commentsBll.GetList(new { ArticleId = id });
            return PartialView(comments);
        }
    }
}