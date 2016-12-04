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
#pragma warning disable 618


namespace AntData.ORM.Dao
{
    public class DalBridge
    {
        protected static readonly ConcurrentDictionary<string, BaseDao> DaoCache = new ConcurrentDictionary<string, BaseDao>();

        [Obsolete("IDataReader一定要在外部关闭可能造成连接泄漏 一定要关闭Idatareader")]
        public static IDataReader CustomerExecuteQuery(string dbName, string sql, Dictionary<string, CustomerParam> paras,IDictionary hints=null, bool isWrite = false)
        {
            var baseDao = DaoCache.GetOrAdd(dbName, BaseDaoFactory.CreateBaseDao(dbName));
            var dic = ConvertStatement(paras);
            if (dic != null && dic.Count > 0)
            {
                if (hints!=null && hints.Count > 0)
                {
                    return baseDao.SelectDataReader(sql, dic,hints,isWrite);
                }
                return baseDao.SelectDataReader(sql, dic,isWrite);
            }
            if (hints != null && hints.Count > 0)
            {
                return baseDao.SelectDataReader(sql, null, hints,isWrite);
            }
            return baseDao.SelectDataReader(sql,isWrite);
        }

        public static DataTable CustomerExecuteQueryTable(string dbName, string sql, Dictionary<string, CustomerParam> paras, IDictionary hints = null, bool isWrite = false)
        {
            var baseDao = DaoCache.GetOrAdd(dbName, BaseDaoFactory.CreateBaseDao(dbName));
            var dic = ConvertStatement(paras);
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
        }

        public static object CustomerExecuteScalar(string dbName, string sql, Dictionary<string, CustomerParam> paras, IDictionary hints = null, bool isWrite = false)
        {
            var baseDao = DaoCache.GetOrAdd(dbName, BaseDaoFactory.CreateBaseDao(dbName));
            var dic = ConvertStatement(paras);
            if (dic != null && dic.Count > 0)
            {
                if (hints != null && hints.Count > 0)
                {
                    return baseDao.ExecScalar(sql, dic, hints, isWrite);
                }
                return baseDao.ExecScalar(sql, dic, isWrite);
            }
            if (hints != null && hints.Count > 0)
            {
                return baseDao.ExecScalar(sql, null, hints, isWrite);
            }
            return baseDao.ExecScalar(sql, isWrite);
        }

        public static int CustomerExecuteNonQuery(string dbName, string sql, Dictionary<string, CustomerParam> paras, IDictionary hints = null, bool isWrite = true)
        {
            var baseDao = DaoCache.GetOrAdd(dbName, BaseDaoFactory.CreateBaseDao(dbName));
            var dic = ConvertStatement(paras);
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

        public static StatementParameterCollection ConvertStatement(Dictionary<string, CustomerParam> paras)
        {
            if (paras != null && paras.Count > 0)
            {
                StatementParameterCollection dic = new StatementParameterCollection();
                foreach (KeyValuePair<string, CustomerParam> item in paras)
                {
                    dic.AddInParameter(item.Key, LinqEnumHelper.IntToEnum<DbType>(item.Value.DbType), item.Value.Value);
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
