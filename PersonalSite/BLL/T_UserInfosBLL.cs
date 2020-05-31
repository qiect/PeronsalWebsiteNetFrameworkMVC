using DAL;
using Model;
using System.Collections.Generic;
using System.Linq;

namespace BLL
{
    /// <summary>
    /// T_UserInfos业务逻辑类
    /// </summary>
    public partial class T_UserInfosBLL
    {
        #region 查询
        /// <summary>
        /// 获取实体根据主键
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        public T_UserInfos GetModel(object id)
        {
            return new T_UserInfosDAL().GetModel(id);
        }
        /// <summary>
        /// 获取列表根据条件
        /// </summary>
        /// <param name="whereConditions">匿名条件 eg：new {Category = 1, SubCategory=2}</param></param>
        /// <returns></returns>
        public IEnumerable<T_UserInfos> GetList(object whereConditions)
        {
            return new T_UserInfosDAL().GetList(whereConditions);
        }
        /// <summary>
        /// 获取列表根据条件字符串
        /// </summary>
        /// <param name="conditions">where条件</param>
        /// <param name="parameters">对应条件参数</param>
        /// <returns></returns>
        public IEnumerable<T_UserInfos> GetList(string conditions, object parameters)
        {
            return new T_UserInfosDAL().GetList(conditions, parameters);
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
        public IEnumerable<T_UserInfos> GetListPages<T_UserInfos>(int pageNumber, int rowsPerPage, string conditions = null, string orderby = null, object parameters = null)
        {
            return new T_UserInfosDAL().GetListPages<T_UserInfos>(pageNumber, rowsPerPage, conditions, orderby, parameters);
        }
        #endregion

        #region 新增
        /// <summary>
        /// 单实体插入返回主键
        /// </summary>
        /// <typeparam name="T">需要插入的实体</typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        public int InsertDb<T_UserInfos>(T_UserInfos entity)
        {
            return new T_UserInfosDAL().InsertDb(entity);
        }
        /// <summary>
        /// 事务批量插入
        /// </summary>
        /// <param name="entityList">实体集合</param>
        /// <returns>成功返回True 否则返回False</returns>
        public bool InsertDb<T_UserInfos>(List<T_UserInfos> entityList)
        {
            return new T_UserInfosDAL().InsertDb(entityList);
        }
        #endregion

        #region 修改
        /// <summary>
        /// 单实体修改  注意 如有要修改 一定要需要所有属性要不然会丢失
        /// </summary>
        /// <param name="entity">实体</param>
        /// <returns></returns>
        public int ModifyDb<T_UserInfos>(T_UserInfos entity)
        {
            return new T_UserInfosDAL().ModifyDb(entity);
        }
        /// <summary>
        /// 批量实体修改 带事务   注意 如有要修改 一定要需要所有属性要不然会丢失
        /// </summary>
        /// <param name="entityList">实体集合</param>
        /// <returns></returns>
        public bool ModifyDb<T_UserInfos>(List<T_UserInfos> entityList)
        {
            return new T_UserInfosDAL().ModifyDb(entityList);
        }
        #endregion

        #region 删除
        /// <summary>
        /// 实体删除 
        /// </summary>
        /// <param name="entity">实体删除</param>
        /// <returns>返回影响行数</returns>
        public int DeleteDb<T_UserInfos>(T_UserInfos entity)
        {
            return new T_UserInfosDAL().DeleteDb(entity);
        }

        /// <summary>
        /// 根据主键删除
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns>返回影响行数</returns>
        public int DeleteDb<T_UserInfos>(object id)
        {
            return new T_UserInfosDAL().DeleteDb(id);
        }
        /// <summary>
        /// 条件删除
        /// </summary>
        /// <param name="entity">实体</param>
        /// <param name="whereConditions">匿名实体条件  eg：new {Category = 1, SubCategory=2}</param>
        /// <returns>返回影响行数</returns>
        public int DeleteDbList<T_UserInfos>(T_UserInfos entity, object whereConditions)
        {
            return new T_UserInfosDAL().DeleteDbList(entity, whereConditions);
        }
        #endregion

    }
}







