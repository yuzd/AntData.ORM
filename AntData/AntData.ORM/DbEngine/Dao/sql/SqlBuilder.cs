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
        public static List<Statement> GetSqlStatement(String logicDbName, IShardingStrategy shardingStrategy,
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
        public static List<Statement> GetScalarStatement(String logicDbName, IShardingStrategy shardingStrategy,
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
        /// <param name="sqlStatementType"></param>
        /// <returns></returns>
        public static List<Statement> GetNonQueryStatement(String logicDbName, IShardingStrategy shardingStrategy,
            String sql, StatementParameterCollection parameters, IDictionary extendedParameters, OperationType? operationType = null, SqlStatementType? sqlStatementType = null)
        {
            return GetDefaultSqlStatement(logicDbName, shardingStrategy, sql, parameters, extendedParameters, sqlStatementType ?? SqlStatementType.UNKNOWN, operationType ?? OperationType.Write);
        }

        private static List<Statement> GetDefaultSqlStatement(String logicDbName, IShardingStrategy shardingStrategy,
            String sql, StatementParameterCollection parameters, IDictionary hints, SqlStatementType sqlType, OperationType? operationType = null)
        {
            if (String.IsNullOrEmpty(logicDbName))
                throw new DalException("Please specify databaseSet.");
            if (String.IsNullOrEmpty(sql))
                throw new DalException("Please specify sql.");

            var result = new List<Statement>();
            var tupleList = ShardingUtil.GetShardInfo(logicDbName, shardingStrategy, parameters, hints);
            if (tupleList.Count < 1)
            {
                //非sharding的场合
                Statement statement = GetStatement(logicDbName, StatementType.Sql,
                    operationType ?? OperationType.Default, sqlType, hints, null);
                statement.StatementText = GetSql(sql, null);
                statement.Parameters = parameters;
#if !NETSTANDARD
                CurrentStackCustomizedLog(statement);
#endif
                result.Add(statement);
            }
            else
            {
                var bulkCopy = false;
                if (hints != null && hints.Contains(DALExtStatementConstant.BULK_COPY))//查看是否是批量插入的case
                {
                    bulkCopy = Convert.ToBoolean(hints[DALExtStatementConstant.BULK_COPY]);
                }

                if (bulkCopy)
                {
                    
                    result.AddRange(BulkCopyCase(logicDbName, shardingStrategy, sql, parameters, hints,  tupleList, sqlType, operationType));
                }
                else
                {
                    foreach (var tuple in tupleList)
                    {
                        Statement statement = GetStatement(logicDbName, StatementType.Sql, operationType ?? OperationType.Default, sqlType, hints, tuple.Item1);
                        statement.StatementText = GetSql(sql, tuple.Item2);
                        statement.Parameters = parameters;
#if !NETSTANDARD
                        CurrentStackCustomizedLog(statement);
#endif
                        result.Add(statement);
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// 批量插入分片规则： 解析sqlstring 按照分片进行分组 在重新组合 如果没有找到分片的默认第一个分片
        /// </summary>
        /// <param name="logicDbName"></param>
        /// <param name="shardingStrategy"></param>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <param name="hints"></param>
        /// <param name="tupleList"></param>
        /// <param name="sqlType"></param>
        /// <param name="operationType"></param>
        /// <returns></returns>
        private static List<Statement> BulkCopyCase(String logicDbName, IShardingStrategy shardingStrategy,
            String sql, StatementParameterCollection parameters, IDictionary hints, List<Tuple<String, String>> tupleList, SqlStatementType sqlType, OperationType? operationType = null)
        {
            var result = new List<Statement>();
           

            //var shardingDB = tuple.Item1;
            //var sharingTable = tuple.Item2;
            if (shardingStrategy.ShardByDB && shardingStrategy.ShardByTable)
            {
                //又分表又分库的情况

            }
            else if (shardingStrategy.ShardByDB || shardingStrategy.ShardByTable)
            {
               
                var dicValues = new Dictionary<string,Tuple<List<string>, StatementParameterCollection>>();
                foreach (var tuple in tupleList)
                {
                    dicValues.Add(shardingStrategy.ShardByDB?tuple.Item1: tuple.Item2, Tuple.Create(new List<string>(),new StatementParameterCollection()));
                }
                var defaultSharding = dicValues.Keys.First();
                //分库的情况
                var arr = sql.Split(new string[] { "VALUES" }, StringSplitOptions.None);
                if (arr.Length != 2)
                {
                    throw new DalException("sharding db for bulkInsert sql string err.");
                }
                var title = arr[0] + " VALUES ";
                var body = arr[1].Replace("\r\n","");
                var values = body.Split(')').Where(r => !string.IsNullOrEmpty(r)).Select(r=>r.StartsWith(",")?r.Substring(1):r).ToArray();//总共有多少行
                var first = values.First();
                var cloumnCount = first.Split(',').Length;//每行总共有多少列

                if (parameters.Count != (cloumnCount * values.Length))
                {
                    throw new DalException("sharding db for bulkInsert sql parameters counts err.");
                }
                var ii = 0;
                //将parameters 按 分库进行分组
                for (int i = 0; i < values.Length; i++)
                {
                    var columns = values[i].Split(',');
                    var haveShardingC = false;
                    var shardingValue = string.Empty;
                    List<StatementParameter> newC = new List<StatementParameter>();
                    for (int j = 0; j < columns.Length; j++)
                    {
                        var p = parameters.ElementAt(ii);
                        if (p.IsShardingColumn)
                        {
                            haveShardingC = true;
                            shardingValue = p.ShardingValue;
                        }
                        newC.Add(p);
                        ii++;
                    }

                    if (haveShardingC)
                    {
                        if (!string.IsNullOrEmpty(shardingValue))
                        {
                            if (dicValues.ContainsKey(shardingValue))
                            {
                                dicValues[shardingValue].Item1.Add(values[i] + ")");
                                foreach (var pp in newC)
                                {
                                    dicValues[shardingValue].Item2.Add(pp);
                                }
                            }
                        }
                        else
                        {
                            //添加到第一个分片
                            dicValues[defaultSharding].Item1.Add(values[i] + ")");
                            foreach (var pp in newC)
                            {
                                dicValues[defaultSharding].Item2.Add(pp);
                            }
                        }
                        
                    }
                }
                
                foreach (var dic in dicValues)
                {
                  
                    if (dic.Value.Item1.Count == 0)
                    {
                        continue;
                    }
                    var newHints = HintsUtil.CloneHints(hints);
                    if (shardingStrategy.ShardByDB)
                    {
                        newHints[DALExtStatementConstant.SHARDID] = dic.Key;
                    }
                    else
                    {
                        newHints[DALExtStatementConstant.TABLEID] = dic.Key;
                    }
                   
                    var statement = GetStatement(logicDbName, StatementType.Sql, operationType ?? OperationType.Default, sqlType, newHints,shardingStrategy.ShardByDB? dic.Key:null);
                    statement.StatementText = shardingStrategy.ShardByDB ? (title + string.Join(",", dic.Value.Item1)): GetSql(title + string.Join(",", dic.Value.Item1), dic.Key);
                    statement.Parameters = dic.Value.Item2;
#if !NETSTANDARD
                    CurrentStackCustomizedLog(statement);
#endif
                    result.Add(statement);
                }
            }

            return result;
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
