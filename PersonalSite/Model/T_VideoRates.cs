using System;
using Dapper;

namespace Model
{
    /// <summary>
    /// T_VideoRates实体类
    /// </summary>
    [Table("T_VideoRates")]
    public class T_VideoRates: DbBaseModel
    {
        /// <summary>
        /// 
        /// </summary>
        public Int32 VideoId{ get;set;}
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


