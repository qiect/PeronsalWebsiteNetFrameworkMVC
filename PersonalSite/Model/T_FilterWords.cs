using System;
using Dapper;

namespace Model
{
    /// <summary>
    /// T_FilterWords实体类
    /// </summary>
    [Table("T_FilterWords")]
    public class T_FilterWords: DbBaseModel
    {
        /// <summary>
        /// 
        /// </summary>
        public String WordPattern{ get;set;}
        /// <summary>
        /// 
        /// </summary>
        public String ReplaceWord{ get;set;}
    }
    
}


