using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using Dapper;

namespace Helper.DbHelper
{
    #region 支持数据库枚举类型
    /// <summary>
    /// 支持数据库枚举类型
    /// </summary>
    public enum DataBaseType
    {
        /// <summary>
        /// 默认值 无类型
        /// </summary>
        Empty,
        /// <summary>
        /// Sql任意版本
        /// </summary>
        SqlServer,
        Mysql,
        /// <summary>
        /// 使用Access时程序要生成32位的
        /// </summary>
        Access,
        Sqlite

    }
    #endregion

    #region 多数据库工厂类
    /// <summary>
    /// 多数据库工厂类
    /// </summary>
    public class DataBaseFactory
    {
        /// <summary>
        /// 读取AppSettings中的连接字符串
        /// </summary>
        public static string Connstr { get; set; }
        /// <summary>
        /// 读取AppSettings中的连接库类型
        /// </summary>
        public static string DataBaseTypeByConfig { get; set; }
        /// <summary>
        /// 实例后的数据类型枚举
        /// </summary>
        public static DataBaseType InstantiateDbType { get; set; }

        #region 读取配置文件生成Connection或者实例DataBaseFactory时设置InstantiateDbType值生成对应数据库类型
        /// <summary>
        /// 默认读取配置文件 生成对应数据库的Connection 
        /// </summary>
        /// <returns></returns>
        public static IDbConnection CreateDbConnection()
        {
            if (string.IsNullOrEmpty(Connstr))
            {
                //Connstr = ConfigurationManager.AppSettings["ConnStr"];
                //Connstr = StringHelper.DesJie(StringHelper.GetAppSetting("ConnStr"));
                Connstr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            }
            //如果该值为空 就读取配置 否则直接使用该值  即：实际使用中可以同时连接两个数据库
            if (InstantiateDbType == DataBaseType.Empty)
            {
                if (string.IsNullOrEmpty(DataBaseTypeByConfig))
                {
                    // DataBaseTypeByConfig = ConfigurationManager.AppSettings["DataBaseType"];
                    DataBaseTypeByConfig = CommonHelper.GetAppSetting("DataBaseType");
                    //根据配置文件生成枚举
                    DataBaseType dbType;
                    Enum.TryParse(DataBaseTypeByConfig, out dbType);
                    //赋值给全局变量 后面生成使用
                    InstantiateDbType = dbType;
                }
            }
            IDbConnection conn = null;
            switch (InstantiateDbType)
            {
                case DataBaseType.SqlServer:
                    conn = new SqlConnection(Connstr);
                    break;
                case DataBaseType.Mysql:
                    {
                        SimpleCRUD.SetDialect(SimpleCRUD.Dialect.MySQL);
                        //conn = new MySqlConnection(Connstr);
                    }

                    break;
                case DataBaseType.Access:
                    conn = new OleDbConnection(Connstr);
                    break;
                case DataBaseType.Sqlite:
                    {
                        SimpleCRUD.SetDialect(SimpleCRUD.Dialect.SQLite);
                        //conn = new SQLiteConnection(Connstr);
                    }

                    break;
                default:
                    {
                        throw new Exception("不支持该数据库类型");
                    }
            }
            return conn;
        }
        #endregion
    }
    #endregion

    /// <summary>
    /// 多数据库工具类 
    /// 需Dapper、DapperSimpleCRUD（用于单类操作）
    /// </summary>
    public class DbHelperByDapper
    {
        /// <summary>
        /// 如果要再一个项目使用多个数据库且不跟配置一致时 需设置该参数
        /// </summary>
        public static DataBaseType EDataBaseType { get; set; }
        public static IDbConnection OpenConnection()
        {
            if (EDataBaseType == DataBaseType.Empty)
                return DataBaseFactory.CreateDbConnection();
            DataBaseFactory.InstantiateDbType = EDataBaseType;
            return DataBaseFactory.CreateDbConnection();
        }

        #region 查询

        /// <summary>
        /// 条件查询 不带分页
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="whereConditions">匿名条件 eg：new {Category = 1, SubCategory=2}</param>
        /// <returns></returns>
        public static IEnumerable<T> GetList<T>(object whereConditions)
        {
            using (var connection = OpenConnection())
            {
                return connection.GetList<T>(whereConditions);
            }
        }

        /// <summary>
        /// 条件查询 不带分页
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="whereConditions">匿名条件 eg：new {Category = 1, SubCategory=2}</param>
        /// <returns></returns>
        public static IEnumerable<T> GetList<T>(string whereConditions, object parameters)
        {
            using (var connection = OpenConnection())
            {
                return connection.GetList<T>(whereConditions, parameters);
            }
        }

        /// <summary>
        /// 主键获取实体类
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id">主键</param>
        /// <returns></returns>
        public static T GetModel<T>(object id)
        {
            using (var connection = OpenConnection())
            {
                return connection.Get<T>(id);
            }
        }
        /// <summary>
        /// 分页查询
        /// </summary>
        /// <typeparam name="T">要查询的实体</typeparam>
        /// <param name="pageNumber">第几页</param>
        /// <param name="rowsPerPage">分页大小</param>
        /// <param name="conditions">带@的条件</param>
        /// <param name="orderby">排序 默认按主键</param>
        /// <param name="parameters">上面设置条件后对应匿名 不能单独写 必需跟conditions一起</param>
        /// <returns></returns>
        public static IEnumerable<T> GetListPages<T>(int pageNumber, int rowsPerPage, string conditions = null, string orderby = null, object parameters = null)
        {
            using (var connection = OpenConnection())
            {
                return connection.GetListPaged<T>(pageNumber, rowsPerPage, conditions, orderby, parameters);
            }
        }
        #endregion

        #region 新增
        /// <summary>
        /// 单实体插入返回主键
        /// </summary>
        /// <typeparam name="T">需要插入的实体</typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static int InsertDb<T>(T entity)
        {
            using (var connection = OpenConnection())
            {
                int? key = connection.Insert(entity);
                return key ?? 0;
            }
        }

        /// <summary>
        /// 事务批量插入
        /// </summary>
        /// <typeparam name="T">实体类</typeparam>
        /// <param name="entityList">实体集合</param>
        /// <returns>成功返回True 否则返回False</returns>
        public static bool InsertDb<T>(List<T> entityList)
        {
            using (var connection = OpenConnection())
            {
                connection.Open();
                IDbTransaction transaction = connection.BeginTransaction();
                try
                {
                    int i = 0;
                    foreach (T item in entityList)
                    {
                        if (connection.Insert(item, transaction) > 0)
                            i++;
                    }
                    if (i > 0)
                        transaction.Commit();
                    return true;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return false;
                }
            }
        }
        #endregion

        #region 修改
        /// <summary>
        /// 单实体修改  注意 如有要修改 一定要需要所有属性要不然会丢失
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity">实体</param>
        /// <returns></returns>
        public static int ModifyDb<T>(T entity)
        {
            using (var connection = OpenConnection())
            {
                return connection.Update(entity);
            }
        }
        /// <summary>
        /// 批量实体修改 带事务   注意 如有要修改 一定要需要所有属性要不然会丢失
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entityList">实体集合</param>
        /// <returns></returns>
        public static bool ModifyDb<T>(List<T> entityList)
        {
            using (var connection = OpenConnection())
            {
                connection.Open();
                IDbTransaction transaction = connection.BeginTransaction();
                try
                {
                    int i = 0;
                    foreach (T item in entityList)
                    {
                        if (connection.Update(item, transaction) > 0)
                            i++;
                    }
                    if (i > 0)
                        transaction.Commit();
                    return true;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return false;
                }
            }
        }
        #endregion

        #region 删除
        /// <summary>
        /// 实体删除 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity">实体删除</param>
        /// <returns>返回影响行数</returns>
        public static int DeleteDb<T>(T entity)
        {
            using (var connection = OpenConnection())
            {
                return connection.Delete(entity);
            }
        }

        /// <summary>
        /// 根据主键删除
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns>返回影响行数</returns>
        public static int DeleteDb(object id)
        {
            using (var connection = OpenConnection())
            {
                return connection.Delete(id);
            }
        }
        /// <summary>
        /// 条件删除
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity">实体</param>
        /// <param name="whereConditions">匿名实体条件  eg：new {Category = 1, SubCategory=2}</param>
        /// <returns>返回影响行数</returns>
        public static int DeleteDbList<T>(T entity, object whereConditions)
        {
            using (var connection = OpenConnection())
            {
                return connection.DeleteList<T>(whereConditions);
            }
        }
        #endregion
    }
}
