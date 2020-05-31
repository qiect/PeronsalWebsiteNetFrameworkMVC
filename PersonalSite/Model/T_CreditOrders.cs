using System;
using Dapper;

namespace Model
{
    /// <summary>
    /// T_CreditOrders实体类
    /// </summary>
    [Table("T_CreditOrders")]
    public class T_CreditOrders: DbBaseModel
    {
        /// <summary>
        /// 
        /// </summary>
        public Guid UserId{ get;set;}
        /// <summary>
        /// 
        /// </summary>
        public Int32 CreditCount{ get;set;}
        /// <summary>
        /// 
        /// </summary>
        public String Status{ get;set;}
    }
    
}


