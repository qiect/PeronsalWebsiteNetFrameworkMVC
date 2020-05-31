using System;
using Dapper;

namespace Model
{
    /// <summary>
    /// Dapper实体基类 所有都继承此类
    /// </summary>
    public class DbBaseModel
    {
        /// <summary>
        /// 主键 可重写
        /// </summary>
        [Key, Column("Id")]
        public virtual Int32 Id { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        //[System.ComponentModel.ReadOnly(true), IgnoreUpdate]
        //public string CreateTime
        //{
        //    get { return DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"); }
        //}

        //private string _modifyTime;
        /// <summary>
        /// 修改时间
        /// </summary>
        //public string ModifyTime
        //{
        //    get
        //    {
        //        return string.IsNullOrWhiteSpace(_modifyTime) ? DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") : _modifyTime;
        //    }
        //    set
        //    {
        //        _modifyTime = string.IsNullOrWhiteSpace(value) ? DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") : value;
        //    }
        //}
    }
}
