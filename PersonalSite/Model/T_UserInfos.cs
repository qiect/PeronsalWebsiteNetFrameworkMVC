using System;
using Dapper;

namespace Model
{
    /// <summary>
    /// T_UserInfos实体类
    /// </summary>
    [Table("T_UserInfos")]
    public class T_UserInfos : DbBaseModel
    {
        /// <summary>
        /// 用户Id
        /// </summary>
        public string UserId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public String Email { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public String Status { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public String VCode { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Int32 Credit { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public String Avatar { get; set; }
    }

}


