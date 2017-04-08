using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AntData.ORM.Common;
using AntData.ORM.DbEngine;
using AntData.ORM.DbEngine.Connection;

#pragma warning disable 618


namespace AntData.ORM.Dao
{
    /// <summary>
    /// linq 和 底层执行 的桥梁
    /// </summary>
    public class DalBridge
    {
        protected static readonly ConcurrentDictionary<string, BaseDao> DaoCache = new ConcurrentDictionary<string, BaseDao>();

        /// <summary>
        /// 开启事物
        /// </summary>
        /// <param name="dbName"></param>
        /// <param name="hints"></param>
        /// <returns></returns>
        public static DataConnectionTransaction CustomerBeginTransaction(string dbName, IDictionary hints = null)
        {
            var baseDao = DaoCache.GetOrAdd(dbName, BaseDaoFactory.CreateBaseDao(dbName));
            return baseDao.BeginTransaction(hints);
        }

        /// <summary>
        /// 执行 insert update delete 查询
        /// </summary>
        /// <param name="dbName"></param>
        /// <param name="sql"></param>
        /// <param name="paras"></param>
        /// <param name="hints"></param>
        /// <param name="isWrite"></param>
        /// <returns></returns>
        public static IList<IDataReader> CustomerExecuteQuery(string dbName, string sql, Dictionary<string, CustomerParam> paras, IDictionary hints = null, bool isWrite = false)
        {
            var baseDao = DaoCache.GetOrAdd(dbName, BaseDaoFactory.CreateBaseDao(dbName));
            var isNonQuery = false;
            var dic = ConvertStatement(paras, out isNonQuery);
            if (dic != null && dic.Count > 0)
            {
                if (hints != null && hints.Count > 0)
                {
                    return baseDao.SelectDataReader(sql, dic, hints, isWrite);
                }
                return baseDao.SelectDataReader(sql, dic, isWrite);
            }
            if (hints != null && hints.Count > 0)
            {
                return baseDao.SelectDataReader(sql, null, hints, isWrite);
            }
            return baseDao.SelectDataReader(sql, isWrite);
        }

        /// <summary>
        /// 执行查询 返回 DataTable
        /// </summary>
        /// <param name="dbName"></param>
        /// <param name="sql"></param>
        /// <param name="paras"></param>
        /// <param name="hints"></param>
        /// <param name="isWrite"></param>
        /// <returns></returns>
        public static DataTable CustomerExecuteQueryTable(string dbName, string sql, Dictionary<string, CustomerParam> paras, IDictionary hints = null, bool isWrite = false)
        {
#if !NETSTANDARD

            var baseDao = DaoCache.GetOrAdd(dbName, BaseDaoFactory.CreateBaseDao(dbName));
            var isNonQuery = false;
            var dic = ConvertStatement(paras, out isNonQuery);
            if (dic != null && dic.Count > 0)
            {
                if (hints != null && hints.Count > 0)
                {
                    return baseDao.SelectDataTable(sql, dic, hints, isWrite);
                }
                return baseDao.SelectDataTable(sql, dic, isWrite);
            }
            if (hints != null && hints.Count > 0)
            {
                return baseDao.SelectDataTable(sql, null, hints, isWrite);
            }
            return baseDao.SelectDataTable(sql, isWrite);
#else
            throw new NotSupportedException("not support in dotnet core");
#endif
        }

        /// <summary>
        /// 执行查询返回单个结果 另外 sqlserver和mysql的insertWithIdentity也是走这个
        /// </summary>
        /// <param name="dbName"></param>
        /// <param name="sql"></param>
        /// <param name="paras"></param>
        /// <param name="hints"></param>
        /// <param name="isWrite"></param>
        /// <returns></returns>
        public static object CustomerExecuteScalar(string dbName, string sql, Dictionary<string, CustomerParam> paras, IDictionary hints = null, bool isWrite = false)
        {
            var baseDao = DaoCache.GetOrAdd(dbName, BaseDaoFactory.CreateBaseDao(dbName));
            var isNonQuery = false;
            var dic = ConvertStatement(paras, out isNonQuery);
            if (dic != null && dic.Count > 0)
            {
                if (hints != null && hints.Count > 0)
                {
                    if (isNonQuery)
                    {
                        baseDao.ExecNonQuery(sql, dic, hints, isWrite);
                        var rq = dic.First(r => r.Direction.Equals(ParameterDirection.Output));
                        return rq.Value;
                    }
                    else
                    {
                        return  baseDao.ExecScalar(sql, dic, hints, isWrite);
                    }
                }
                if (isNonQuery)
                {
                    baseDao.ExecNonQuery(sql, dic, isWrite);
                    var rq = dic.First(r => r.Direction.Equals(ParameterDirection.Output));
                    return rq.Value;
                }
                else
                {
                    return baseDao.ExecScalar(sql, dic, isWrite);
                }
            }
            if (hints != null && hints.Count > 0)
            {
                return isNonQuery ? baseDao.ExecNonQuery(sql, null, hints, isWrite) : baseDao.ExecScalar(sql, null, hints, isWrite);
            }
            return isNonQuery ? baseDao.ExecNonQuery(sql, isWrite) : baseDao.ExecScalar(sql, isWrite);
        }

        /// <summary>
        /// 执行 select 查询
        /// </summary>
        /// <param name="dbName"></param>
        /// <param name="sql"></param>
        /// <param name="paras"></param>
        /// <param name="hints"></param>
        /// <param name="isWrite"></param>
        /// <returns></returns>
        public static int CustomerExecuteNonQuery(string dbName, string sql, Dictionary<string, CustomerParam> paras, IDictionary hints = null, bool isWrite = true)
        {
            var baseDao = DaoCache.GetOrAdd(dbName, BaseDaoFactory.CreateBaseDao(dbName));
            var isNonQuery = false;
            var dic = ConvertStatement(paras, out isNonQuery);
            if (dic != null && dic.Count > 0)
            {
                if (hints != null && hints.Count > 0)
                {
                    return baseDao.ExecNonQuery(sql, dic, hints, isWrite);
                }
                return baseDao.ExecNonQuery(sql, dic, isWrite);
            }
            if (hints != null && hints.Count > 0)
            {
                return baseDao.ExecNonQuery(sql, null, hints, isWrite);
            }
            return baseDao.ExecNonQuery(sql, isWrite);
        }

        /// <summary>
        /// 参数转换
        /// </summary>
        /// <param name="paras"></param>
        /// <param name="isNonQuery"></param>
        /// <returns></returns>
        public static StatementParameterCollection ConvertStatement(Dictionary<string, CustomerParam> paras, out bool
            isNonQuery)
        {
            isNonQuery = false;
            if (paras != null && paras.Count > 0)
            {
                StatementParameterCollection dic = new StatementParameterCollection();
                foreach (KeyValuePair<string, CustomerParam> item in paras)
                {
                    if (item.Value.ParameterDirection.Equals(ParameterDirection.ReturnValue))
                    {
                        dic.AddReturnParameter(item.Key, LinqEnumHelper.IntToEnum<DbType>(item.Value.DbType));
                    }
                    else if (item.Value.ParameterDirection.Equals(ParameterDirection.Output))
                    {
                        if (!isNonQuery) isNonQuery = true;
                        dic.AddOutParameter(item.Key, LinqEnumHelper.IntToEnum<DbType>(item.Value.DbType));
                    }
                    else
                    {
                        dic.AddInParameter(item.Key, LinqEnumHelper.IntToEnum<DbType>(item.Value.DbType), item.Value.Value);
                    }
                    dic[item.Key].TableName = item.Value.TableName;
                    dic[item.Key].ColumnName = item.Value.ColumnName;
                }
                return dic;
            }
            else
            {
                return new StatementParameterCollection();
            }
        }
    }
}
