using System;
using Dapper;

namespace Model
{
    /// <summary>
    /// T_Comments实体类
    /// </summary>
    [Table("T_Comments")]
    public class T_Comments: DbBaseModel
    {
        /// <summary>
        /// 
        /// </summary>
        public Int32 ArticleId{ get;set;}
        /// <summary>
        /// 
        /// </summary>
        public DateTime PostDate{ get;set;}
        /// <summary>
        /// 
        /// </summary>
        public String Msg{ get;set;}
        /// <summary>
        /// 
        /// </summary>
        public Boolean IsVisible{ get;set;}
    }
    
}


