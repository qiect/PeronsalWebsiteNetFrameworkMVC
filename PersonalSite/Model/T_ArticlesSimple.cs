using System;
using Dapper;

namespace Model
{
    /// <summary>
    /// T_Articles实体类
    /// </summary>
    [Table("T_Articles")]
    public class T_ArticlesSimple : DbBaseModel
    {
        /// <summary>
        /// 
        /// </summary>
        public Int32 ChannelId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public String Title { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DateTime PostDate { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public String StaticPath { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Int32 DingCount { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Int32 ComCount { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public String UserId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Boolean IsVisible { get; set; }
        /// <summary>
        /// 置顶
        /// </summary>
        public Boolean IsFirst { get; set; }
    }

}


