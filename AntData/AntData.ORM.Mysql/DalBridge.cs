using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AntData.ORM.Common;
using AntData.ORM.Dao;
using AntData.ORM.DbEngine;


namespace Arch.Data.ORM.Mysql
{
    public class DalBridge
    {
        protected static readonly ConcurrentDictionary<string, BaseDao> DaoCache = new ConcurrentDictionary<string, BaseDao>();

        public static IDataReader CustomerExecuteQuery(string dbName, string sql, Dictionary<string, CustomerParam> paras,IDictionary hints=null)
        {
            var baseDao = DaoCache.GetOrAdd(dbName, BaseDaoFactory.CreateBaseDao(dbName));
            var dic = ConvertStatement(paras);
            if (dic != null && dic.Count > 0)
            {
                if (hints!=null && hints.Count > 0)
                {
                    return baseDao.SelectDataReader(sql, dic,hints);
                }
                return baseDao.SelectDataReader(sql, dic);
            }
            if (hints != null && hints.Count > 0)
            {
                return baseDao.SelectDataReader(sql, null, hints);
            }
            return baseDao.SelectDataReader(sql);
        }

        public static DataTable CustomerExecuteQueryTable(string dbName, string sql, Dictionary<string, CustomerParam> paras, IDictionary hints = null)
        {
            var baseDao = DaoCache.GetOrAdd(dbName, BaseDaoFactory.CreateBaseDao(dbName));
            var dic = ConvertStatement(paras);
            if (dic != null && dic.Count > 0)
            {
                if (hints != null && hints.Count > 0)
                {
                    return baseDao.SelectDataTable(sql, dic, hints);
                }
                return baseDao.SelectDataTable(sql, dic);
            }
            if (hints != null && hints.Count > 0)
            {
                return baseDao.SelectDataTable(sql, null, hints);
            }
            return baseDao.SelectDataTable(sql);
        }

        public static object CustomerExecuteScalar(string dbName, string sql, Dictionary<string, CustomerParam> paras, IDictionary hints = null)
        {
            var baseDao = DaoCache.GetOrAdd(dbName, BaseDaoFactory.CreateBaseDao(dbName));
            var dic = ConvertStatement(paras);
            if (dic != null && dic.Count > 0)
            {
                if (hints != null && hints.Count > 0)
                {
                    return baseDao.ExecScalar(sql, dic, hints);
                }
                return baseDao.ExecScalar(sql, dic);
            }
            if (hints != null && hints.Count > 0)
            {
                return baseDao.ExecScalar(sql, null, hints);
            }
            return baseDao.ExecScalar(sql);
        }

        public static int CustomerExecuteNonQuery(string dbName, string sql, Dictionary<string, CustomerParam> paras, IDictionary hints = null)
        {
            var baseDao = DaoCache.GetOrAdd(dbName, BaseDaoFactory.CreateBaseDao(dbName));
            var dic = ConvertStatement(paras);
            if (dic != null && dic.Count > 0)
            {
                if (hints != null && hints.Count > 0)
                {
                    return baseDao.ExecNonQuery(sql, dic, hints);
                }
                return baseDao.ExecNonQuery(sql, dic);
            }
            if (hints != null && hints.Count > 0)
            {
                return baseDao.ExecNonQuery(sql, null, hints);
            }
            return baseDao.ExecNonQuery(sql);
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
