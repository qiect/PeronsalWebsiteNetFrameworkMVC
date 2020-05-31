using System;
using Dapper;

namespace Model
{
    /// <summary>
    /// T_Videos实体类
    /// </summary>
    [Table("T_Videos")]
    public class T_Videos: DbBaseModel
    {
        /// <summary>
        /// 
        /// </summary>
        public String Title{ get;set;}
        /// <summary>
        /// 
        /// </summary>
        public String Url{ get;set;}
        /// <summary>
        /// 
        /// </summary>
        public Int32 DingCount{ get;set;}
        /// <summary>
        /// 
        /// </summary>
        public Int32 CaiCount{ get;set;}
    }
    
}


