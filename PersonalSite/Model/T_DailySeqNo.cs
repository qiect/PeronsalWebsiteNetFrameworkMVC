using System;
using Dapper;

namespace Model
{
    /// <summary>
    /// T_DailySeqNo实体类
    /// </summary>
    [Table("T_DailySeqNo")]
    public class T_DailySeqNo: DbBaseModel
    {
        /// <summary>
        /// 
        /// </summary>
        public String SeqName{ get;set;}
        /// <summary>
        /// 
        /// </summary>
        public DateTime SeqDate{ get;set;}
        /// <summary>
        /// 
        /// </summary>
        public Int32 CurValue{ get;set;}
    }
    
}


