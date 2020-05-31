using System;
using Dapper;

namespace Model
{
    /// <summary>
    /// T_Channels实体类
    /// </summary>
    [Table("T_Channels")]
    public class T_Channels: DbBaseModel
    {
        /// <summary>
        /// 
        /// </summary>
        public Int32 ParentId{ get;set;}
        /// <summary>
        /// 
        /// </summary>
        public String Name{ get;set;}
    }
    
}


