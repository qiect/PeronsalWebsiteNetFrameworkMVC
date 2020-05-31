using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PersonalSite.Models
{
    public class ResultModel<T>
    {
        public int status { get; set; }
        public string msg { get; set; }
        public int code { get; set; }
        public string action { get; set; }
        public string url { get; set; }
        public int count { get; set; }
        public IEnumerable<T> data { get; set; }
    }
}