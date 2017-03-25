//-----------------------------------------------------------------------
// <copyright file="ShardingUtil.cs" company="Company">
// Copyright (C) Company. All Rights Reserved.
// </copyright>
// <author>nainaigu</author>
// <summary></summary>
//-----------------------------------------------------------------------

using System.Collections;
using AntData.DbEngine.Sharding;
using AntData.ORM.Dao;
using AntData.ORM.Dao.Common;
using AntData.ORM.DbEngine.Enums;
using AntData.ORM.Enums;

namespace AntData.ORM.DbEngine.Dao.Common.Util
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;


    /// 是否做了Sharding
    /// 
    /// 判断流程（需要考虑性能）：
    /// 1. IsNeedShard : 是否在Dal.config中配置了ShardingStrategy，如果是False，则IsShardByDb始终是False
    /// 2. IsShardByDb： 是否分库，即是否在Dal.config为每个DatabaseSet中配置了多个数据库
    /// 3. 如果传入TABLEID，则表名不相同
    /// 4. 如果传入SHARDID，则不需要自动计算
    ///     4.1 如果没有传入ShardID，且IsShardByDb为True，则自动计算ShardID
    ///     4.2 如果IsShardByDb为True，但是计算出来ShardID是NULL，则抛出异常
    class ShardingUtil
    {

        /// <summary>
        /// 获取分表还是分库 或者分表分库都有
        /// </summary>
        /// <param name="shardingStrategy"></param>
        /// <returns></returns>
        public static ShardingType GetShardingType(IShardingStrategy shardingStrategy)
        {
            Boolean shardByDb = IsShardByDb(shardingStrategy);
            Boolean shardByTable = IsShardByTable(shardingStrategy);

            if (shardByDb && shardByTable)
                return ShardingType.ShardByDBAndTable;
            if (shardByDb)
                return ShardingType.ShardByDB;
            if (shardByTable)
                return ShardingType.ShardByTable;

            return ShardingType.ShardByDB;
        }

        /// <summary>
        /// 是否配置了ShardingStrategy
        /// </summary>
        /// <param name="shardingStrategy"></param>
        /// <returns></returns>
        public static Boolean IsShardEnabled(IShardingStrategy shardingStrategy)
        {
            return shardingStrategy != null;
        }

        /// <summary>
        /// 是否是分库
        /// </summary>
        /// <param name="shardingStrategy"></param>
        /// <returns></returns>
        private static Boolean IsShardByDb(IShardingStrategy shardingStrategy)
        {
            return shardingStrategy != null && shardingStrategy.ShardByDB;
        }

        /// <summary>
        /// 是否是分表
        /// </summary>
        /// <param name="shardingStrategy"></param>
        /// <returns></returns>
        private static Boolean IsShardByTable(IShardingStrategy shardingStrategy)
        {
            return shardingStrategy != null && shardingStrategy.ShardByTable;
        }



        /// <summary>
        /// 从hints中提取ShardID 这种情况是手动指定 读哪个库
        /// </summary>
        /// <param name="hints"></param>
        /// <returns></returns>
        private static String GetShardIdByHints(IDictionary hints)
        {
            if (hints != null && hints.Contains(DALExtStatementConstant.SHARDID) && hints[DALExtStatementConstant.SHARDID] != null)
                return hints[DALExtStatementConstant.SHARDID].ToString();
            return null;
        }

        /// <summary>
        /// 从hints中提取TABLEID 这种情况是手动指定 读哪个表
        /// </summary>
        /// <param name="hints"></param>
        /// <returns></returns>
        private static String GetTableIdByHints(IDictionary hints)
        {
            if (hints != null && hints.Contains(DALExtStatementConstant.TABLEID) && hints[DALExtStatementConstant.TABLEID] != null)
                return hints[DALExtStatementConstant.TABLEID].ToString();
            return null;
        }

        /// <summary>
        /// 计算在哪个Shard执行操作
        /// </summary>
        /// <param name="shardingStrategy"></param>
        /// <param name="shardColumnValue"></param>
        /// <returns></returns>
        private static String CalculateShardId(IShardingStrategy shardingStrategy, IComparable shardColumnValue)
        {
            if (shardingStrategy == null || shardColumnValue == null)
                return null;
            return shardingStrategy.ComputeShardId(shardColumnValue);
        }

        /// <summary>
        /// 获取分片信息
        /// </summary>
        /// <param name="logicDbName"></param>
        /// <param name="shardingStrategy"></param>
        /// <param name="hints"></param>
        /// <returns></returns>
        public static List<Tuple<String, String>> GetShardInfo(String logicDbName, IShardingStrategy shardingStrategy, StatementParameterCollection parameters, IDictionary hints)
        {
            List<String> shardId = null;
            List<String> tableId = null;
            var result = new List<Tuple<string, string>>();
            Boolean shardEnabled = IsShardEnabled(shardingStrategy);
            if (!shardEnabled)
                return result;

            shardId = GetShardId(logicDbName, shardingStrategy, parameters, hints);//是否有分库
            tableId = GetTableId(logicDbName, shardingStrategy, parameters, hints);//分表

            if (shardId.Count < 1 && tableId.Count < 1)
                throw new DalException("Please provide shard information.");

            if (shardId.Count > 0 && tableId.Count == 0)
            {
                foreach (var d in shardId)
                {
                    result.Add(Tuple.Create<String, String>(d, null));

                }
            }
            else if (shardId.Count == 0 && tableId.Count > 0)
            {
                foreach (var d in tableId)
                {
                    result.Add(Tuple.Create<String, String>(null, d));
                }

            }
            else
            {
                foreach (var d in shardId)
                {
                    foreach (var t in tableId)
                    {
                        result.Add(Tuple.Create<String, String>(d, t));
                    }

                }
            }

            return result;
        }

        private static List<String> GetShardId(String logicDbName, IShardingStrategy shardingStrategy, StatementParameterCollection parameters, IDictionary hints)
        {
            List<String> shardIdList = new List<string>();
            Boolean shardByDb = IsShardByDb(shardingStrategy);

            if (shardByDb)
            {
                var shardId = GetShardIdByHints(hints);
                if (String.IsNullOrEmpty(shardId))
                {
                    List<IComparable> shardColumnValue = shardingStrategy.GetShardColumnValueList(logicDbName, parameters, hints);
                    foreach (var comparable in shardColumnValue)
                    {
                        shardId = CalculateShardId(shardingStrategy, comparable);
                        shardIdList.Add(shardId);
                    }
                }
                else
                {
                    shardIdList.Add(shardId);
                }
            }
            return shardIdList;
        }


        private static List<String> GetTableId(String logicDbName, IShardingStrategy shardingStrategy, StatementParameterCollection parameters, IDictionary hints)
        {
            List<String> tableIdList = new List<string>();
            Boolean shardByTable = IsShardByTable(shardingStrategy);

            if (shardByTable)
            {
                var tableId = GetTableIdByHints(hints);
                if (String.IsNullOrEmpty(tableId))
                {
                    List<IComparable> shardColumnValue = shardingStrategy.GetShardColumnValueList(logicDbName, parameters, hints);

                    foreach (var comparable in shardColumnValue)
                    {
                        tableId = CalculateShardId(shardingStrategy, comparable);
                        tableIdList.Add(tableId);
                    }
                }
                else
                {
                    tableIdList.Add(tableId);
                }
            }

            return tableIdList;
        }

        #region Shuffled Items

        //private static IDictionary<String, IList<T>> ShuffledByDb<T>(String logicDbName, IShardingStrategy shardingStrategy, StatementParameterCollection parameters,
        //    IList<T> list, IDictionary hints)
        //{
        //    if (String.IsNullOrEmpty(logicDbName))
        //        return null;
        //    if (list == null || list.Count == 0)
        //        return null;
        //    var dict = new Dictionary<String, IList<T>>();

        //    foreach (var item in list)
        //    {
        //        String shardId = GetShardId(logicDbName, shardingStrategy, parameters, hints);
        //        if (String.IsNullOrEmpty(shardId))
        //            continue;

        //        if (!dict.ContainsKey(shardId))
        //            dict.Add(shardId, new List<T>());
        //        dict[shardId].Add(item);
        //    }

        //    return dict;
        //}

        //private static IDictionary<String, IList<T>> ShuffledByTable<T>(String logicDbName, IShardingStrategy shardingStrategy, StatementParameterCollection parameters,
        //    IList<T> list,IDictionary hints)
        //{
        //    if (String.IsNullOrEmpty(logicDbName))
        //        return null;
        //    if (list == null || list.Count == 0)
        //        return null;
        //    var dict = new Dictionary<String, IList<T>>();

        //    foreach (var item in list)
        //    {
        //        String tableId = GetTableId(logicDbName, shardingStrategy, parameters, hints);
        //        if (String.IsNullOrEmpty(tableId))
        //            continue;

        //        if (!dict.ContainsKey(tableId))
        //            dict.Add(tableId, new List<T>());
        //        dict[tableId].Add(item);
        //    }

        //    return dict;
        //}

        //private static IDictionary<String, IDictionary<String, IList<T>>> ShuffledByDbTable<T>(String logicDbName, IShardingStrategy shardingStrategy, StatementParameterCollection parameters,
        //    IList<T> list, IDictionary hints)
        //{
        //    if (String.IsNullOrEmpty(logicDbName))
        //        return null;
        //    if (list == null || list.Count == 0)
        //        return null;

        //    var dict = new Dictionary<String, IDictionary<String, IList<T>>>();

        //    foreach (var item in list)
        //    {
        //        String shardId = GetShardId(logicDbName, shardingStrategy, parameters, hints);
        //        String tableId = GetTableId(logicDbName, shardingStrategy , parameters,hints);

        //        if (!dict.ContainsKey(shardId))
        //            dict.Add(shardId, new Dictionary<String, IList<T>>());
        //        if (!dict[shardId].ContainsKey(tableId))
        //            dict[shardId].Add(tableId, new List<T>());
        //        dict[shardId][tableId].Add(item);
        //    }

        //    return dict;
        //}

        #endregion Shuffled Items

        public static IList<Statement> GetShardStatement(String logicDbName, IShardingStrategy shardingStrategy,
            StatementParameterCollection parameters, IDictionary hints, Func<IDictionary, List<Statement>> func, SqlStatementType sqlStatementType)
        {
            IList<Statement> statements;
            if (shardingStrategy == null)
                return null;
            var shardingType = GetShardingType(shardingStrategy);

            if (shardingType != ShardingType.ShardByDBAndTable)
            {
                IList<String> shards = null;

                //Get shards from hints
                if (hints != null)
                {
                    IList<String> temp = null;

                    if (shardingType == ShardingType.ShardByDB)//是分库的情况
                    {
                        if (hints.Contains(DALExtStatementConstant.SHARD_IDS))//看下hints里面有没有指定分配id(一组)
                        {
                            temp = hints[DALExtStatementConstant.SHARD_IDS] as List<String>;
                        }
                        else if (hints.Contains(DALExtStatementConstant.SHARDID))//单个的分配id
                        {
                            temp = new List<String> { hints[DALExtStatementConstant.SHARDID] as String };
                        }
                    }
                    else if (shardingType == ShardingType.ShardByTable)//是分表的情况
                    {
                        if (hints.Contains(DALExtStatementConstant.TABLE_IDS))
                        {
                            temp = hints[DALExtStatementConstant.TABLE_IDS] as List<String>;
                        }
                        else if (hints.Contains(DALExtStatementConstant.TABLEID))
                        {
                            temp = new List<String> { hints[DALExtStatementConstant.TABLEID] as String };
                        }
                    }

                    if (temp != null)
                        shards = temp;
                }


                var bulkCopy = false;
                if (hints != null && hints.Contains(DALExtStatementConstant.BULK_COPY))//查看是否是批量插入的case
                {
                    bulkCopy = Convert.ToBoolean(hints[DALExtStatementConstant.BULK_COPY]);
                }

                if (bulkCopy)
                {
                    return func.Invoke(hints);
                }
                //Get shards from parameters 这里会根据 查询参数得出分配的信息
                if (shards == null)
                {
                    if (shardingType == ShardingType.ShardByDB)
                    {
                        shards = GetShardId(logicDbName, shardingStrategy, parameters, hints);
                    }
                    else if (shardingType == ShardingType.ShardByTable)
                    {
                        shards = GetTableId(logicDbName, shardingStrategy, parameters, hints);
                    }
                }

                //对于不带条件的查询 都默认查询所有的
                if ((shards == null || shards.Count == 0) && sqlStatementType.Equals(SqlStatementType.SELECT))
                {
                    shards = shardingStrategy.AllShards;
                }

                if (shards == null || shards.Count == 0)
                    throw new DalException("Please provide shard information.");

                //Build statements
                statements = new List<Statement>();

                foreach (var item in shards.Distinct())
                {
                    var newHints = HintsUtil.CloneHints(hints);

                    switch (shardingType)
                    {
                        case ShardingType.ShardByDB:
                            newHints[DALExtStatementConstant.SHARDID] = item;
                            break;
                        case ShardingType.ShardByTable:
                            newHints[DALExtStatementConstant.TABLEID] = item;
                            break;
                    }

                    var statement = func.Invoke(newHints);
                    foreach (var ss in statement)
                    {
                        statements.Add(ss);
                    }
                }
            }
            else
            {
                statements = new List<Statement>();
                IDictionary<String, IList<String>> shardDict = null;
                if (hints != null && hints.Contains(DALExtStatementConstant.SHARD_TABLE_DICT))
                    shardDict = hints[DALExtStatementConstant.SHARD_TABLE_DICT] as IDictionary<String, IList<String>>;

                if (shardDict == null)
                {
                    var newHints = HintsUtil.CloneHints(hints);
                    newHints[DALExtStatementConstant.SHARDID] = GetShardIdByHints(hints);
                    newHints[DALExtStatementConstant.TABLEID] = GetTableIdByHints(hints);
                    var statement = func.Invoke(newHints);
                    foreach (var ss in statement)
                    {
                        statements.Add(ss);
                    }
                }
                else
                {
                    foreach (var shard in shardDict)
                    {
                        foreach (var table in shard.Value)
                        {
                            var newHints = HintsUtil.CloneHints(hints);
                            newHints[DALExtStatementConstant.SHARDID] = shard.Key;
                            newHints[DALExtStatementConstant.TABLEID] = table;
                            var statement = func.Invoke(newHints);
                            foreach (var ss in statement)
                            {
                                statements.Add(ss);
                            }
                        }
                    }
                }
            }

            return statements;
        }


        public static List<Statement> GetQueryStatement(String logicDbName, string sql, IShardingStrategy shardingStrategy, StatementParameterCollection parameters, IDictionary hints, OperationType? operationType = null)
        {
            var result = new List<Statement>();
            var tupleList = ShardingUtil.GetShardInfo(logicDbName, shardingStrategy, parameters, hints);
            foreach (var tuple in tupleList)
            {
                Statement statement = new Statement
                {
                    DatabaseSet = logicDbName,
                    StatementType = StatementType.Sql,
                    OperationType = operationType ?? OperationType.Default,
                    Hints = hints,
                    ShardID = tuple.Item1,
                    TableName = "",
                    SqlOperationType = SqlStatementType.SELECT,
                    Parameters = parameters
                };

                statement.StatementText = string.IsNullOrEmpty(tuple.Item2) ? sql : string.Format(sql, tuple.Item2);
                result.Add(statement);
            }


            return result;
        }
    }
}