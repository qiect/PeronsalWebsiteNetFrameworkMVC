using System;
using Helper.DbHelper;
using Model;
using System.Collections.Generic;
using System.Linq;


namespace DAL
{
    public partial class T_ArticlesDAL
    {

        /// <summary>
        /// 获取列表根据条件
        /// </summary>
        /// <param name="whereConditions">匿名条件 eg：new {Category = 1, SubCategory=2}</param></param>
        /// <returns></returns>
        public List<T_ArticlesSimple> GetListSimple(object whereConditions)
        {
            return DbHelperByDapper.GetList<T_ArticlesSimple>(whereConditions).ToList();
        }

        /// <summary>
        /// 分页查询
        /// </summary>
        /// <param name="pageNumber">第几页</param>
        /// <param name="rowsPerPage">分页大小</param>
        /// <param name="conditions">带@的条件</param>
        /// <param name="orderby">排序 默认按主键</param>
        /// <param name="parameters">上面设置条件后对应匿名 不能单独写 必需跟conditions一起</param>
        /// <returns></returns>
        public IEnumerable<T_ArticlesSimple> GetListPagesSimple<T_ArticlesSimple>(int pageNumber, int rowsPerPage, string conditions = null, string orderby = null, object parameters = null)
        {
            return DbHelperByDapper.GetListPages<T_ArticlesSimple>(pageNumber, rowsPerPage, conditions, orderby, parameters);
        }
    }
}
