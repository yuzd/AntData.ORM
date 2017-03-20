using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using AntData.DbEngine.Sharding;
using AntData.ORM.Common.Util;
using AntData.ORM.Dao;
using AntData.ORM.Dao.Common;
using AntData.ORM.DbEngine;
using AntData.ORM.DbEngine.Dao.Common.Util;
using AntData.ORM.Enums;
using StatementType = AntData.ORM.Enums.StatementType;

namespace AntData.ORM.Dao.sql
{
    public class SqlBuilder
    {
      

       

        #region Statement

        private static Statement GetStatement(String logicDbName, StatementType statementType, OperationType operationType, SqlStatementType sqlType, IDictionary hints, String shardId = null)
        {
            return new Statement
            {
                DatabaseSet = logicDbName,
                StatementType = statementType,
                OperationType = operationType,
                Hints = hints,
                ShardID = shardId,
                SqlOperationType = sqlType,
            };
        }

        #endregion

        /// <summary>
        /// GetSqlStatement
        /// </summary>
        /// <param name="logicDbName"></param>
        /// <param name="shardingStrategy"></param>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <param name="extendedParameters"></param>
        /// <param name="operationType"></param>
        /// <returns></returns>
        public static Statement GetSqlStatement(String logicDbName, IShardingStrategy shardingStrategy,
            String sql, StatementParameterCollection parameters, IDictionary extendedParameters, OperationType? operationType = null)
        {
            return GetDefaultSqlStatement(logicDbName, shardingStrategy, sql, parameters, extendedParameters, SqlStatementType.SELECT, operationType ?? OperationType.Default);
        }

        /// <summary>
        /// GetScalarStatement
        /// </summary>
        /// <param name="logicDbName"></param>
        /// <param name="shardingStrategy"></param>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <param name="extendedParameters"></param>
        /// <param name="operationType"></param>
        /// <returns></returns>
        public static Statement GetScalarStatement(String logicDbName, IShardingStrategy shardingStrategy,
            String sql, StatementParameterCollection parameters, IDictionary extendedParameters, OperationType? operationType = null)
        {
            return GetDefaultSqlStatement(logicDbName, shardingStrategy, sql, parameters, extendedParameters, SqlStatementType.SELECT, operationType ?? OperationType.Default);
        }

        /// <summary>
        /// </summary>
        /// <param name="logicDbName"></param>
        /// <param name="shardingStrategy"></param>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <param name="extendedParameters"></param>
        /// <param name="operationType"></param>
        /// <returns></returns>
        public static Statement GetNonQueryStatement(String logicDbName, IShardingStrategy shardingStrategy,
            String sql, StatementParameterCollection parameters, IDictionary extendedParameters, OperationType? operationType = null)
        {
            return GetDefaultSqlStatement(logicDbName, shardingStrategy, sql, parameters, extendedParameters, SqlStatementType.UNKNOWN, operationType ?? OperationType.Write);
        }

        private static Statement GetDefaultSqlStatement(String logicDbName, IShardingStrategy shardingStrategy,
            String sql, StatementParameterCollection parameters, IDictionary hints, SqlStatementType sqlType, OperationType? operationType = null)
        {
            if (String.IsNullOrEmpty(logicDbName))
                throw new DalException("Please specify databaseSet.");
            if (String.IsNullOrEmpty(sql))
                throw new DalException("Please specify sql.");
            var tuple = ShardingUtil.GetShardInfo(logicDbName, shardingStrategy, hints);
            Statement statement = GetStatement(logicDbName, StatementType.Sql, operationType ?? OperationType.Default, sqlType, hints, tuple.Item1);
            statement.StatementText = GetSql(sql, tuple.Item2);
            statement.Parameters = parameters;
#if !NETSTANDARD
            CurrentStackCustomizedLog(statement);
#endif
            return statement;
        }
        private static String GetSql(String sql, String tableId)
        {
            return String.IsNullOrEmpty(tableId) ? sql : String.Format(sql, tableId);
        }
#if !NETSTANDARD

        private static void CurrentStackCustomizedLog(Statement statement)
        {
            StackTrace stackTrace = new StackTrace(false);
            StringBuilder sb = new StringBuilder();

            for (Int32 x = 2; x < stackTrace.FrameCount; ++x)
            {
                var stackFrame = stackTrace.GetFrame(x);

                if (IsMethodToBeIncluded(stackFrame))
                {
                    var method = stackFrame.GetMethod();
                    if (method.ReflectedType == null) continue;
                    statement.Invoker = method.ReflectedType.FullName;
                    sb.Append(method.Name).Append("(");
                    var parameters = method.GetParameters();

                    if (parameters.Length > 0)
                    {
                        foreach (var parameter in parameters)
                        {
                            sb.Append(parameter.ParameterType.Name).Append(",");
                        }

                        sb.Remove(sb.Length - 1, 1);
                    }

                    sb.Append(")");
                    statement.InvokeMethod = sb.ToString();
                    break;
                }
            }
        }

        private static Boolean IsMethodToBeIncluded(StackFrame stackFrame)
        {
            var method = stackFrame.GetMethod();
            return method.DeclaringType == null || method.DeclaringType.FullName == null || !method.DeclaringType.FullName.Contains(BaseDaoName);
        }
#endif

        private const String BaseDaoName = "Ant.Data";

    }
}
