//-----------------------------------------------------------------------
// <copyright file="BaseDao.cs" company="Company">
// Copyright (C) Company. All Rights Reserved.
// </copyright>
// <author>nainaigu</author>
// <summary></summary>
//-----------------------------------------------------------------------

using System.Collections;
using System.Data;
using AntData.ORM.Common.Util;
using AntData.ORM.Dao.sql;
using AntData.ORM.Dao.sql.imp;
using AntData.ORM.DbEngine;
using AntData.ORM.DbEngine.DB;
using AntData.ORM.DbEngine.Sharding;
using AntData.ORM.Enums;


namespace AntData.ORM.Dao
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// 数据库访问类，一般通过BaseDaoFactory的静态方法CreateBaseDao创建
    /// </summary>
    public class BaseDao
    {
        /// <summary>
        /// DatabaseSet name
        /// </summary>
        private readonly String LogicDbName;

        private readonly SqlTable Table = SqlTableFactory.Instance.Build();

        private readonly Lazy<IShardingStrategy> shardingStrategyLazy;

        /// <summary>
        ///当前的分片策略， 使用ShardingStrategy时，需要确保已经指定了逻辑数据库名，即 new BaseDao("逻辑数据库名");
        /// </summary>
        public IShardingStrategy ShardingStrategy { get { return shardingStrategyLazy.Value; } }

        private Boolean IsShardEnabled
        {
            get { return ShardingStrategy != null; }
        }

        /// <summary>
        /// 构造初始化
        /// </summary>
        /// <param name="logicDbName">逻辑数据库名</param>
        public BaseDao(String logicDbName)
        {
            if (String.IsNullOrEmpty(logicDbName))
                throw new DalException("Please specify databaseSet.");

            LogicDbName = logicDbName;
            shardingStrategyLazy = new Lazy<IShardingStrategy>(() => DALBootstrap.GetShardingStrategy(logicDbName), true);
        }


     


        #region SelectDataReader VisitDataReader

        /// <summary>
        /// 执行查询语句
        /// </summary>
        /// <param name="sql">SQL语句</param>
        /// <returns>IDataReader</returns>
        /// <exception cref="DalException">数据访问框架异常</exception>
        [Obsolete("此方法有可能造成连接泄漏，不推荐使用，建议使用VisitDataReader！")]
        public IDataReader SelectDataReader(String sql)
        {
            return SelectDataReader(sql, null);
        }

        /// <summary>
        /// 执行查询语句
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <param name="parameters">查询参数</param>
        /// <returns>IDataReader</returns>
        /// <exception cref="DalException">数据访问框架异常</exception>
        [Obsolete("此方法有可能造成连接泄漏，不推荐使用，建议使用VisitDataReader！")]
        public IDataReader SelectDataReader(String sql, StatementParameterCollection parameters)
        {
            return SelectDataReader(sql, parameters, null);
        }

        /// <summary>
        /// 执行查询语句
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <param name="parameters">查询参数</param>
        /// <param name="hints">指令扩展属性</param>
        /// <returns>IDataReader</returns>
        /// <exception cref="DalException">数据访问框架异常</exception>
        [Obsolete("此方法有可能造成连接泄漏，不推荐使用，建议使用VisitDataReader！")]
        public IDataReader SelectDataReader(String sql, StatementParameterCollection parameters, IDictionary hints)
        {
            return SelectDataReader(sql, parameters, hints, OperationType.Default);
        }

        /// <summary>
        /// 执行查询语句
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <param name="parameters">查询参数</param>
        /// <param name="hints">指令扩展属性</param>
        /// <param name="operationType">操作类型，读写分离，默认从master库读取</param>
        /// <returns>IDataReader</returns>
        /// <exception cref="DalException">数据访问框架异常</exception>
        [Obsolete("此方法有可能造成连接泄漏，不推荐使用，建议使用VisitDataReader！")]
        public IDataReader SelectDataReader(String sql, StatementParameterCollection parameters, IDictionary hints, OperationType operationType)
        {
            try
            {
                Statement statement = SqlBuilder.GetSqlStatement(Table, LogicDbName, ShardingStrategy, sql, parameters, hints, operationType);
                SqlTable.AddSqlToExtendParams(statement, hints);
                return DatabaseBridge.Instance.ExecuteReader(statement);
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
            }
        }

        /// <summary>
        /// 执行查询语句，并返回指定的结果（连接会确认被释放，安全）
        /// </summary>
        /// <typeparam name="T">返回值的类型</typeparam>
        /// <param name="sql">SQL语句</param>
        /// <param name="callback">回调，接受IDataReader作为参数，返回T类型的结果</param>
        /// <returns>T</returns>
        public T VisitDataReader<T>(String sql, Func<IDataReader, T> callback)
        {
            return VisitDataReader(sql, null, callback);
        }

        /// <summary>
        /// 执行查询语句，并返回指定的结果（连接会确认被释放，安全）
        /// </summary>
        /// <typeparam name="T">返回值的类型</typeparam>
        /// <param name="sql">SQL语句</param>
        /// <param name="parameters">查询参数</param>
        /// <param name="callback">回调，接受IDataReader作为参数，返回T类型的结果</param>
        /// <returns>T</returns>
        public T VisitDataReader<T>(String sql, StatementParameterCollection parameters, Func<IDataReader, T> callback)
        {
            return VisitDataReader(sql, parameters, null, callback);
        }

        /// <summary>
        /// 执行查询语句，并返回指定的结果（连接会确认被释放，安全）
        /// </summary>
        /// <typeparam name="T">返回值的类型</typeparam>
        /// <param name="sql">SQL语句</param>
        /// <param name="parameters">查询参数</param>
        /// <param name="hints">指令扩展属性</param>
        /// <param name="callback">回调，接受IDataReader作为参数，返回T类型的结果</param>
        /// <returns>T</returns>
        public T VisitDataReader<T>(String sql, StatementParameterCollection parameters, IDictionary hints, Func<IDataReader, T> callback)
        {
            return VisitDataReader(sql, parameters, hints, OperationType.Default, callback);
        }

        /// <summary>
        /// 执行查询语句，并返回指定的结果（连接会确认被释放，安全）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql">sql语句</param>
        /// <param name="parameters">查询参数</param>
        /// <param name="hints">指令扩展属性</param>
        /// <param name="operationType">操作类型，读写分离，默认从master库读取</param>
        /// <param name="callback">回调，接受IDataReader作为参数，返回T类型的结果</param>
        /// <returns>T</returns>
        public T VisitDataReader<T>(String sql, StatementParameterCollection parameters, IDictionary hints, OperationType operationType, Func<IDataReader, T> callback)
        {
            using (var reader = SelectDataReader(sql, parameters, hints, operationType))
            {
                return callback(reader);
            }
        }

        #endregion

        #region SelectDataTable

        /// <summary>
        /// 执行查询语句
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <returns>DataTable</returns>
        /// <exception cref="DalException">数据访问框架异常</exception>
        public DataTable SelectDataTable(String sql)
        {
            return SelectDataTable(sql, null);
        }

        /// <summary>
        /// 执行查询语句
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <param name="parameters">查询参数</param>
        /// <returns>DataTable</returns>
        /// <exception cref="DalException">数据访问框架异常</exception>
        public DataTable SelectDataTable(String sql, StatementParameterCollection parameters)
        {
            return SelectDataTable(sql, parameters, null, OperationType.Default);
        }

        /// <summary>
        ///  执行查询语句
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <param name="parameters">查询参数</param>
        /// <param name="hints">指令的扩展属性</param>
        /// <returns>DataTable</returns>
        /// <exception cref="DalException">数据访问框架异常</exception>
        public DataTable SelectDataTable(String sql, StatementParameterCollection parameters, IDictionary hints)
        {
            return SelectDataTable(sql, parameters, hints, OperationType.Default);
        }

        /// <summary>
        /// 执行查询语句
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <param name="parameters">查询参数</param>
        /// <param name="hints">指令的扩展属性 </param>
        /// <param name="operationType">操作类型，读写分离，默认从master库读取</param>
        /// <returns>DataTable</returns>
        /// <exception cref="DalException">数据访问框架异常</exception>
        public DataTable SelectDataTable(String sql, StatementParameterCollection parameters, IDictionary hints, OperationType operationType)
        {
            DataSet ds = SelectDataSet(sql, parameters, hints, operationType);
            if (ds == null)
                return null;
            return ds.Tables.Count > 0 ? ds.Tables[0] : null;
        }

        #endregion

        #region SelectDataSet

        /// <summary>
        /// 执行查询
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <returns>DataSet</returns>
        /// <exception cref="DalException">数据访问框架异常</exception>
        public DataSet SelectDataSet(String sql)
        {
            return SelectDataSet(sql, null);
        }

        /// <summary>
        /// 执行查询
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <param name="parameters">查询参数</param>
        /// <returns>DataSet</returns>
        /// <exception cref="DalException">数据访问框架异常</exception>
        public DataSet SelectDataSet(String sql, StatementParameterCollection parameters)
        {
            return SelectDataSet(sql, parameters, null);
        }

        /// <summary>
        /// 执行查询
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <param name="parameters">查询参数</param>
        /// <param name="hints">指令的扩展属性</param>
        /// <returns>DataSet</returns>
        /// <exception cref="DalException">数据访问框架异常</exception>
        public DataSet SelectDataSet(String sql, StatementParameterCollection parameters, IDictionary hints)
        {
            return SelectDataSet(sql, parameters, hints, OperationType.Default);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sql">SQL语句</param>
        /// <param name="parameters">查询参数</param>
        /// <param name="hints">指令的扩展属性</param>
        /// <param name="operationType">操作类型，读写分离，默认从master库读取</param>
        /// <returns>DataSet</returns>
        /// <exception cref="DalException">数据访问框架异常</exception>
        public DataSet SelectDataSet(String sql, StatementParameterCollection parameters, IDictionary hints, OperationType operationType)
        {
            try
            {
                DataSet dataSet;

                if (!IsShardEnabled)
                {
                    Statement statement = SqlBuilder.GetSqlStatement(Table, LogicDbName, ShardingStrategy, sql, parameters, hints, operationType);
                    SqlTable.AddSqlToExtendParams(statement, hints);
                    dataSet = DatabaseBridge.Instance.ExecuteDataSet(statement, null);
                }
                else
                {
                    var statements = ShardingUtil.GetShardStatement(LogicDbName, ShardingStrategy, parameters, hints,
                        newHints => SqlBuilder.GetSqlStatement(Table, LogicDbName, ShardingStrategy, sql, parameters, newHints, operationType));
                    dataSet = ShardingExecutor.ExecuteShardingDataSet(statements);
                }

                return dataSet;
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
            }
        }

     


        #endregion

    
  

        #region ExecScalar

        /// <summary>
        /// 执行单返回值聚集查询指令
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <returns>object</returns>
        /// <exception cref="DalException">数据访问框架异常</exception>
        public Object ExecScalar(String sql)
        {
            return ExecScalar(sql, null);
        }

        /// <summary>
        /// 执行单返回值聚集查询指令
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <param name="parameters">查询参数</param>
        /// <returns>object</returns>
        /// <exception cref="DalException">数据访问框架异常</exception>
        public Object ExecScalar(String sql, StatementParameterCollection parameters)
        {
            return ExecScalar(sql, parameters, null);
        }

        /// <summary>
        /// 执行单返回值聚集查询指令
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <param name="parameters">查询参数</param>
        /// <param name="hints">指令参数：如timeout</param>
        /// <returns>object</returns>
        /// <exception cref="DalException">数据访问框架异常</exception>
        public Object ExecScalar(String sql, StatementParameterCollection parameters, IDictionary hints)
        {
            return ExecScalar(sql, parameters, hints, OperationType.Default);
        }

        /// <summary>
        /// 执行单返回值聚集查询指令
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <param name="parameters">查询参数</param>
        /// <param name="hints">指令参数：如timeout</param>
        /// <param name="operationType">操作类型，读写分离，默认从master库读取</param>
        /// <returns>object</returns>
        /// <exception cref="DalException">数据访问框架异常</exception>
        public Object ExecScalar(String sql, StatementParameterCollection parameters, IDictionary hints, OperationType operationType)
        {
            try
            {
                Object result = null;

                if (!IsShardEnabled)
                {
                    Statement statement = SqlBuilder.GetScalarStatement(Table, LogicDbName, ShardingStrategy, sql, parameters, hints, operationType);
                    SqlTable.AddSqlToExtendParams(statement, hints);
                    result = DatabaseBridge.Instance.ExecuteScalar(statement);
                }
                else
                {
                    var statements = ShardingUtil.GetShardStatement(LogicDbName, ShardingStrategy, parameters, hints,
                        newHints => SqlBuilder.GetScalarStatement(Table, LogicDbName, ShardingStrategy, sql, parameters, newHints, operationType));

                    var temp = ShardingExecutor.ExecuteShardingScalar(statements);

                    if (temp.Count > 0)
                    {
                        if (temp.Count == 1)
                        {
                            result = temp[0];
                        }
                        else
                        {
                            throw new DalException("ExecScalar exception:more than one shard.");
                        }
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
            }
        }

        #endregion

        #region ExecNonQuery

        /// <summary>
        /// 执行非查询指令
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <returns>影响行数</returns>
        /// <exception cref="DalException">数据访问框架异常</exception>
        public Int32 ExecNonQuery(String sql)
        {
            return ExecNonQuery(sql, null);
        }

        /// <summary>
        /// 执行非查询指令
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <param name="parameters">查询参数</param>
        /// <returns>影响行数</returns>
        /// <exception cref="DalException">数据访问框架异常</exception>
        public Int32 ExecNonQuery(String sql, StatementParameterCollection parameters)
        {
            return ExecNonQuery(sql, parameters, null);
        }

        /// <summary>
        /// 执行非查询指令
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <param name="parameters">查询参数</param>
        /// <param name="hints">指令参数：如timeout</param>
        /// <returns>影响行数</returns>
        /// <exception cref="DalException">数据访问框架异常</exception>
        public Int32 ExecNonQuery(String sql, StatementParameterCollection parameters, IDictionary hints)
        {
            return ExecNonQuery(sql, parameters, hints, OperationType.Write);
        }

        /// <summary>
        /// 执行非查询指令
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <param name="parameters">查询参数</param>
        /// <param name="hints">指令参数：如timeout</param>
        /// <param name="operationType">操作类型，读写分离，默认从master库读取</param>
        /// <returns>影响行数</returns>
        /// <exception cref="DalException">数据访问框架异常</exception>
        public Int32 ExecNonQuery(String sql, StatementParameterCollection parameters, IDictionary hints, OperationType operationType)
        {
            try
            {
                Int32 result;

                if (!IsShardEnabled)
                {
                    Statement statement = SqlBuilder.GetNonQueryStatement(Table, LogicDbName, ShardingStrategy, sql, parameters, hints, operationType);
                    SqlTable.AddSqlToExtendParams(statement, hints);
                    result = DatabaseBridge.Instance.ExecuteNonQuery(statement);
                }
                else
                {
                    var statements = ShardingUtil.GetShardStatement(LogicDbName, ShardingStrategy, parameters, hints,
                        newHints => SqlBuilder.GetNonQueryStatement(Table, LogicDbName, ShardingStrategy, sql, parameters, newHints));

                    result = ShardingExecutor.ExecuteShardingNonQuery(statements).Sum();
                }

                return result;
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
            }
        }

        #endregion

    }
}