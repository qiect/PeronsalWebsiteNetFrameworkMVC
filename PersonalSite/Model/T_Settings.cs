using System;
using Dapper;

namespace Model
{
    /// <summary>
    /// T_Settings实体类
    /// </summary>
    [Table("T_Settings")]
    public class T_Settings: DbBaseModel
    {
        /// <summary>
        /// 
        /// </summary>
        public String Name{ get;set;}
        /// <summary>
        /// 
        /// </summary>
        public String Value{ get;set;}
    }
    
}


