//-----------------------------------------------------------------------
// <copyright file="BaseDao.cs" company="Company">
// Copyright (C) Company. All Rights Reserved.
// </copyright>
// <author>nainaigu</author>
// <summary></summary>
//-----------------------------------------------------------------------

using System.Collections;
using System.Data;
using AntData.DbEngine.Sharding;
using AntData.ORM.Common.Util;
using AntData.ORM.Dao.Common;
using AntData.ORM.Dao.sql;
using AntData.ORM.DbEngine;
using AntData.ORM.DbEngine.Connection;
using AntData.ORM.DbEngine.Dao.Common.Util;
using AntData.ORM.DbEngine.DB;
using AntData.ORM.Enums;
using AntData.ORM.Extensions;


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

        /// <summary>
        /// 开始事物
        /// </summary>
        /// <param name="hints"></param>
        /// <returns></returns>
        public DataConnectionTransaction BeginTransaction(IDictionary hints = null)
        {
            if (IsShardEnabled)
            {
          
                throw new DalException("Transaction can nou used for sharding ");
            }
            Statement statement = SqlBuilder.GetSqlStatement(LogicDbName, ShardingStrategy, LogicDbName + "=>BeginTransaction", null, null, OperationType.Write).Single();
            return DatabaseBridge.Instance.BeginTransaction(statement) ;
        }

        #region SelectDataReader VisitDataReader

        /// <summary>
        /// 执行查询语句
        /// </summary>
        /// <param name="sql">SQL语句</param>
        /// <param name="isWrite">默认读</param>
        /// <returns>IDataReader</returns>
        /// <exception cref="DalException">数据访问框架异常</exception>
        public IList<IDataReader> SelectDataReader(String sql, bool isWrite = false)
        {
            return SelectDataReader(sql, null, isWrite);
        }

        /// <summary>
        /// 执行查询语句
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <param name="parameters">查询参数</param>
        /// <param name="isWrite">默认读</param>
        /// <returns>IDataReader</returns>
        /// <exception cref="DalException">数据访问框架异常</exception>
        public IList<IDataReader> SelectDataReader(String sql, StatementParameterCollection parameters, bool isWrite = false)
        {
            return SelectDataReader(sql, parameters, null, isWrite);
        }

        /// <summary>
        /// 执行查询语句
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <param name="parameters">查询参数</param>
        /// <param name="hints">指令扩展属性</param>
        /// <param name="isWrite">默认读</param>
        /// <returns>IDataReader</returns>
        /// <exception cref="DalException">数据访问框架异常</exception>
        public IList<IDataReader> SelectDataReader(String sql, StatementParameterCollection parameters, IDictionary hints, bool isWrite = false)
        {

            return SelectDataReader(sql, parameters, hints, isWrite ? OperationType.Write : OperationType.Read);
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
        public IList<IDataReader> SelectDataReader(String sql, StatementParameterCollection parameters, IDictionary hints, OperationType operationType)
        {
            try
            {
                if (!IsShardEnabled)
                {
                    Statement statement = SqlBuilder.GetSqlStatement(LogicDbName, ShardingStrategy, sql, parameters, hints, operationType).Single();
                    AddSqlToExtendParams(statement, hints);
                    return new List<IDataReader> { DatabaseBridge.Instance.ExecuteReader(statement) };

                }
                else
                {
                    var statements = ShardingUtil.GetShardStatement(LogicDbName, ShardingStrategy, parameters, hints, newHints => ShardingUtil.GetQueryStatement(LogicDbName, sql, ShardingStrategy, parameters, newHints, operationType), SqlStatementType.SELECT);
                    return ShardingExecutor.GetShardingDataReaderList(statements);
                }
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
#if !NETSTANDARD
        #region SelectDataTable

        /// <summary>
        /// 执行查询语句
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <param name="isWrite">读写配置</param>
        /// <returns>DataTable</returns>
        /// <exception cref="DalException">数据访问框架异常</exception>
        public DataTable SelectDataTable(String sql, bool isWrite = false)
        {
            return SelectDataTable(sql, null, isWrite);
        }

        /// <summary>
        /// 执行查询语句
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <param name="parameters">查询参数</param>
        /// <param name="isWrite">读写配置</param>
        /// <returns>DataTable</returns>
        /// <exception cref="DalException">数据访问框架异常</exception>
        public DataTable SelectDataTable(String sql, StatementParameterCollection parameters, bool isWrite = false)
        {
            return SelectDataTable(sql, parameters, null, isWrite ? OperationType.Write : OperationType.Read);
        }

        /// <summary>
        ///  执行查询语句
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <param name="parameters">查询参数</param>
        /// <param name="hints">指令的扩展属性</param>
        /// <returns>DataTable</returns>
        /// <exception cref="DalException">数据访问框架异常</exception>
        public DataTable SelectDataTable(String sql, StatementParameterCollection parameters, IDictionary hints, bool isWrite = false)
        {
            return SelectDataTable(sql, parameters, hints, isWrite ? OperationType.Write : OperationType.Read);
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
        /// <param name="isWrite">读写配置</param>
        /// <returns>DataSet</returns>
        /// <exception cref="DalException">数据访问框架异常</exception>
        public DataSet SelectDataSet(String sql, bool isWrite = false)
        {
            return SelectDataSet(sql, null, isWrite);
        }

        /// <summary>
        /// 执行查询
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <param name="parameters">查询参数</param>
        /// <param name="isWrite">读写配置</param>
        /// <returns>DataSet</returns>
        /// <exception cref="DalException">数据访问框架异常</exception>
        public DataSet SelectDataSet(String sql, StatementParameterCollection parameters, bool isWrite = false)
        {
            return SelectDataSet(sql, parameters, null, isWrite);
        }

        /// <summary>
        /// 执行查询
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <param name="parameters">查询参数</param>
        /// <param name="hints">指令的扩展属性</param>
        /// <param name="isWrite">读写配置</param>
        /// <returns>DataSet</returns>
        /// <exception cref="DalException">数据访问框架异常</exception>
        public DataSet SelectDataSet(String sql, StatementParameterCollection parameters, IDictionary hints, bool isWrite = false)
        {
            return SelectDataSet(sql, parameters, hints, isWrite ? OperationType.Write : OperationType.Read);
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
                    Statement statement = SqlBuilder.GetSqlStatement(LogicDbName, ShardingStrategy, sql, parameters,
                        hints, operationType).Single();
                    AddSqlToExtendParams(statement, hints);
                    dataSet = DatabaseBridge.Instance.ExecuteDataSet(statement);

                }
                else
                {
                    var statements = ShardingUtil.GetShardStatement(LogicDbName, ShardingStrategy, null, hints, newHints => ShardingUtil.GetQueryStatement(LogicDbName, sql, ShardingStrategy, parameters, newHints, operationType),SqlStatementType.SELECT);
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
#endif
        #region ExecScalar

        /// <summary>
        /// 执行单返回值聚集查询指令
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <param name="isWrite">读写配置</param>
        /// <returns>object</returns>
        /// <exception cref="DalException">数据访问框架异常</exception>
        public Object ExecScalar(String sql, bool isWrite = false)
        {
            return ExecScalar(sql, null, isWrite);
        }

        /// <summary>
        /// 执行单返回值聚集查询指令
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <param name="parameters">查询参数</param>
        /// <param name="isWrite">读写配置</param>
        /// <returns>object</returns>
        /// <exception cref="DalException">数据访问框架异常</exception>
        public Object ExecScalar(String sql, StatementParameterCollection parameters, bool isWrite = false)
        {
            return ExecScalar(sql, parameters, null, isWrite);
        }

        /// <summary>
        /// 执行单返回值聚集查询指令
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <param name="parameters">查询参数</param>
        /// <param name="hints">指令参数：如timeout</param>
        /// <param name="isWrite">读写配置</param>
        /// <returns>object</returns>
        /// <exception cref="DalException">数据访问框架异常</exception>
        public Object ExecScalar(String sql, StatementParameterCollection parameters, IDictionary hints, bool isWrite = false)
        {
            return ExecScalar(sql, parameters, hints, isWrite ? OperationType.Write : OperationType.Read);
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
                    Statement statement = SqlBuilder.GetScalarStatement(LogicDbName, ShardingStrategy, sql, parameters, hints, operationType).Single();
                    AddSqlToExtendParams(statement, hints);
                    result = DatabaseBridge.Instance.ExecuteScalar(statement);

                }
                else
                {
                    var statements = ShardingUtil.GetShardStatement(LogicDbName, ShardingStrategy, parameters, hints,
                        newHints => SqlBuilder.GetScalarStatement(LogicDbName, ShardingStrategy, sql, parameters, newHints, operationType),SqlStatementType.SELECT);

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
        /// <param name="isWrite">读写配置</param>
        /// <returns>影响行数</returns>
        /// <exception cref="DalException">数据访问框架异常</exception>
        public Int32 ExecNonQuery(String sql, bool isWrite = true)
        {
            return ExecNonQuery(sql, null, isWrite);
        }

        /// <summary>
        /// 执行非查询指令
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <param name="parameters">查询参数</param>
        /// <param name="isWrite">读写配置</param>
        /// <returns>影响行数</returns>
        /// <exception cref="DalException">数据访问框架异常</exception>
        public Int32 ExecNonQuery(String sql, StatementParameterCollection parameters, bool isWrite = true)
        {
            return ExecNonQuery(sql, parameters, null, isWrite);
        }

        /// <summary>
        /// 执行非查询指令
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <param name="parameters">查询参数</param>
        /// <param name="hints">指令参数：如timeout</param>
        /// <param name="isWrite">读写配置</param>
        /// <returns>影响行数</returns>
        /// <exception cref="DalException">数据访问框架异常</exception>
        public Int32 ExecNonQuery(String sql, StatementParameterCollection parameters, IDictionary hints, bool isWrite = true)
        {
            return ExecNonQuery(sql, parameters, hints, isWrite ? OperationType.Write : OperationType.Read);
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
                    Statement statement = SqlBuilder.GetNonQueryStatement(LogicDbName, ShardingStrategy, sql, parameters, hints, operationType).Single();
                    AddSqlToExtendParams(statement, hints);
                    result = DatabaseBridge.Instance.ExecuteNonQuery(statement);
                }
                else
                {
                   
                    var statements = ShardingUtil.GetShardStatement(LogicDbName, ShardingStrategy, parameters, hints,
                        newHints => SqlBuilder.GetNonQueryStatement(LogicDbName, ShardingStrategy, sql, parameters, newHints, operationType, SqlStatementType.UNKNOWN), SqlStatementType.UNKNOWN);

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

        public void AddSqlToExtendParams(Statement statement, IDictionary extendParams)
        {
            if (extendParams == null) return;
            var types = extendParams.GetType().GetGenericArgumentsEx();

            //仅当extendParams是 Dictionary<string, object> 或者 Dictionary<string, string>时，
            //才将SQL填回
            if (types.Length != 2 || types[0] != typeof(String) || (types[1] != typeof(String) && types[1] != typeof(Object))) return;
            extendParams[DALExtStatementConstant.SQL] = statement.StatementText;
        }
    }
}