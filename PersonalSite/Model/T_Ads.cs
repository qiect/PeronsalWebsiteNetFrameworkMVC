using System;
using Dapper;

namespace Model
{
    /// <summary>
    /// T_Ads实体类
    /// </summary>
    [Table("T_Ads")]
    public class T_Ads: DbBaseModel
    {
        /// <summary>
        /// 
        /// </summary>
        public String Name{ get;set;}
        /// <summary>
        /// 
        /// </summary>
        public Int32 PositionId{ get;set;}
        /// <summary>
        /// 
        /// </summary>
        public Int32 AdType{ get;set;}
        /// <summary>
        /// 
        /// </summary>
        public String TextAdText{ get;set;}
        /// <summary>
        /// 
        /// </summary>
        public String TextAdUrl{ get;set;}
        /// <summary>
        /// 
        /// </summary>
        public String PicAdImgUrl{ get;set;}
        /// <summary>
        /// 
        /// </summary>
        public String PicAdUrl{ get;set;}
        /// <summary>
        /// 
        /// </summary>
        public String CodeAdHTML{ get;set;}
    }
    
}


