using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PersonSite.Search
{
    public class SearchResult
    {
        public string Number { get; set; }
        public string Title { get; set; }
        public string StaticPath { get; set; }
        public string BodyPreview { get; set; }
    }
}