using System;
using Dapper;

namespace Model
{
    /// <summary>
    /// T_ArticleRates实体类
    /// </summary>
    [Table("T_ArticleRates")]
    public class T_ArticleRates: DbBaseModel
    {
        /// <summary>
        /// 
        /// </summary>
        public Int32 ArticleId{ get;set;}
        /// <summary>
        /// 1代表顶，-1代表踩
        /// </summary>
        public Int32 Action{ get;set;}
        /// <summary>
        /// 
        /// </summary>
        public DateTime CreateDate{ get;set;}
        /// <summary>
        /// 
        /// </summary>
        public String IP{ get;set;}
    }
    
}


