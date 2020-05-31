using System;
using Dapper;

namespace Model
{
    /// <summary>
    /// T_AdPositions实体类
    /// </summary>
    [Table("T_AdPositions")]
    public class T_AdPositions: DbBaseModel
    {
        /// <summary>
        /// 
        /// </summary>
        public String Name{ get;set;}
    }
    
}


