using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BLL;

namespace PersonalSite.Controllers
{
    public class ChannelController : Controller
    {

        //
        // GET: /Channel/
        public ActionResult Index()
        {
            T_ChannelsBLL bll = new T_ChannelsBLL();
            ViewBag.Channel = bll.GetList(new { ParentId = 0 }).ToList();
            return PartialView();
        }

        public ActionResult Filter()
        {
            return PartialView();
        }

        public ActionResult Channel()
        {
            T_ChannelsBLL bll = new T_ChannelsBLL();
            return PartialView(bll.GetList(null).ToList());
        }


    }
}