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
        private static String CalculateShardId(IShardingStrategy shardingStrategy, IComparable shardColumnValue,bool isDb)
        {
            if (shardingStrategy == null || shardColumnValue == null)
                return null;
            return shardingStrategy.ComputeShardId(shardColumnValue, isDb);
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
                    List<IComparable> shardColumnValue = shardingStrategy.GetShardColumnValueList(logicDbName, parameters, hints,true);
                    foreach (var comparable in shardColumnValue)
                    {
                        shardId = CalculateShardId(shardingStrategy, comparable,true);
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
                    List<IComparable> shardColumnValue = shardingStrategy.GetShardColumnValueList(logicDbName, parameters, hints,false);

                    foreach (var comparable in shardColumnValue)
                    {
                        tableId = CalculateShardId(shardingStrategy, comparable,false);
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


        public static IList<Statement> GetShardStatement(string sql,String logicDbName, IShardingStrategy shardingStrategy,
            StatementParameterCollection parameters, IDictionary hints, SqlStatementType sqlStatementType)
        {
            if (shardingStrategy == null)
                return null;

            IList<String> shardsdb = null;
            IList<String> shardstable = null;

            //Get shards from hints
            if (hints != null)
            {

                if (hints.Contains(DALExtStatementConstant.SHARD_IDS))//看下hints里面有没有指定分配id(一组)
                {
                    shardsdb = hints[DALExtStatementConstant.SHARD_IDS] as List<String>;
                }
                else if (hints.Contains(DALExtStatementConstant.SHARDID))//单个的分配id
                {
                    shardsdb = new List<String> { hints[DALExtStatementConstant.SHARDID] as String };
                }

                if (hints.Contains(DALExtStatementConstant.TABLE_IDS))
                {
                    shardstable = hints[DALExtStatementConstant.TABLE_IDS] as List<String>;
                }
                else if (hints.Contains(DALExtStatementConstant.TABLEID))
                {
                    shardstable = new List<String> { hints[DALExtStatementConstant.TABLEID] as String };
                }
            }

            //Get shards from parameters 这里会根据 查询参数得出分配的信息
            if (shardsdb == null || !shardsdb.Any())
            {
                if (shardingStrategy.ShardByDB)
                {
                    shardsdb = GetShardId(logicDbName, shardingStrategy, parameters, hints);
                }
            }

            if (shardstable == null || !shardstable.Any())
            {
                if (shardingStrategy.ShardByTable)
                {
                    shardstable = GetTableId(logicDbName, shardingStrategy, parameters, hints);
                }
            }
                
            //对于不带条件的查询或者删除 都默认查询所有的
            if (((shardsdb == null || shardsdb.Count == 0) && (shardstable == null || shardstable.Count == 0)) )
            {
                if (shardingStrategy.ShardByDB)
                {
                    shardsdb = shardingStrategy.AllDbShards;
                }
                if (shardingStrategy.ShardByTable)
                {
                    shardstable = shardingStrategy.AllTableShards;
                }
            }
            else if (shardingStrategy.ShardByDB && (shardsdb == null || shardsdb.Count == 0))
            {
                shardsdb = shardingStrategy.AllDbShards;
            }
            else if (shardingStrategy.ShardByTable && (shardstable == null || shardstable.Count == 0))
            {
                shardstable = shardingStrategy.AllTableShards;
            }

            //Build statements
            IList<Statement> statements = new List<Statement>();
            if (shardsdb == null) shardsdb = new List<string>();
            if (shardstable == null) shardstable = new List<string>();


            if (hints != null && hints.Contains(DALExtStatementConstant.BULK_COPY))
            {
                //批量插入 需要
                return BulkCopyCase(logicDbName, shardingStrategy, sql, parameters, hints, shardsdb, shardstable);

            }

            if (shardsdb.Any() && shardstable.Any())
            {
                foreach (var item in shardsdb)
                {
                    foreach (var table in shardstable)
                    {
                        var newHints = HintsUtil.CloneHints(hints);
                        newHints[DALExtStatementConstant.SHARDID] = item;
                        newHints[DALExtStatementConstant.TABLEID] = table;
                        Statement statement = new Statement
                        {
                            DatabaseSet = logicDbName,
                            StatementType = StatementType.Sql,
                            Hints = hints,
                            ShardID = item,//分库的情况
                            IsSharding = true,
                            TableName = "",
                            SqlOperationType = sqlStatementType,
                            Parameters = parameters
                        };
                        statement.StatementText =  string.Format(sql, table);
                        statements.Add(statement);
                    }
                }
            }
            else if (shardsdb.Any())
            {
                foreach (var item in shardsdb)
                {
                    var newHints = HintsUtil.CloneHints(hints);
                    newHints[DALExtStatementConstant.SHARDID] = item;

                    Statement statement = new Statement
                    {
                        DatabaseSet = logicDbName,
                        StatementType = StatementType.Sql,
                        Hints = hints,
                        ShardID = item,//分库的情况
                        IsSharding = true,
                        TableName = "",
                        SqlOperationType = sqlStatementType,
                        Parameters = parameters
                    };
                    statement.StatementText = sql;
                    statements.Add(statement);

                }
            }
            else if(shardstable.Any())
            {
                foreach (var table in shardstable)
                {
                    var newHints = HintsUtil.CloneHints(hints);
                    newHints[DALExtStatementConstant.TABLEID] = table;

                    Statement statement = new Statement
                    {
                        DatabaseSet = logicDbName,
                        StatementType = StatementType.Sql,
                        Hints = hints,
                        ShardID = "",//分库的情况
                        IsSharding = true,
                        TableName = "",
                        SqlOperationType = sqlStatementType,
                        Parameters = parameters
                    };
                    statement.StatementText = string.Format(sql, table);
                    statements.Add(statement);
                }
            }
           

            return statements;
        }


        /// <summary>
        /// 批量插入分片规则： 解析sqlstring 按照分片进行分组 在重新组合 如果没有找到分片的默认第一个分片
        /// </summary>
        /// <param name="logicDbName"></param>
        /// <param name="shardingStrategy"></param>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <param name="hints"></param>
        /// <param name="sqlType"></param>
        /// <returns></returns>
        private static List<Statement> BulkCopyCase(String logicDbName, IShardingStrategy shardingStrategy,
            String sql, StatementParameterCollection parameters, IDictionary hints,IList<string> dbList,IList<string> tableList)
        {

            //根据sqlstring 进行分组
            var dbDic = dbList.GroupBy(r=>r).ToDictionary(r => r.Key, y => y.First());
            var tableDic = tableList.GroupBy(r=>r).ToDictionary(r => r.Key, y => y.First());
            var result = new List<Statement>();

            var dicValues = new Dictionary<string, Dictionary<string, Tuple<List<string>, StatementParameterCollection>>>();
           
            string defaultDBSharding = !dbDic.Any()? "": dbDic.Keys.FirstOrDefault();
            string defaultTableSharding = !tableDic.Any() ?"": tableDic.Keys.FirstOrDefault();
            //分库的情况
            var arr = sql.Split(new string[] { "VALUES" }, StringSplitOptions.None);
            if (arr.Length != 2)
            {
                throw new DalException("sharding db for bulkInsert sql string err.");
            }
            var title = arr[0] + " VALUES ";
            var body = arr[1].Replace("\r\n", "");
            var values = body.Split(')').Where(r => !string.IsNullOrEmpty(r)).Select(r => r.StartsWith(",") ? r.Substring(1) : r).ToArray();//总共有多少行
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
                var shardingValue = string.Empty;
                var shardingDB = string.Empty;
                List<StatementParameter> newC = new List<StatementParameter>();
                for (int j = 0; j < columns.Length; j++)
                {
                    var p = parameters.ElementAt(ii);
                    if (p.IsShardingColumn)
                    {
                        shardingValue = p.ShardingValue;
                    }

                    if (p.IsShardingDb)
                    {
                        shardingDB = p.ShardingDbValue;
                    }
                    newC.Add(p);
                    ii++;
                }

                if (!string.IsNullOrEmpty(shardingDB) && !string.IsNullOrEmpty(shardingValue))
                {
                    //分表又分库的情况
                    if (!dicValues.TryGetValue(shardingDB, out var dic2))
                    {
                        dic2 = new Dictionary<string, Tuple<List<string>, StatementParameterCollection>>();
                        dicValues.Add(shardingDB, dic2);
                    }

                    if (!dic2.TryGetValue(shardingValue, out var dic3))
                    {
                        dic3 = Tuple.Create(new List<string>(), new StatementParameterCollection());
                        dic2.Add(shardingValue, dic3);
                    }

                    dic3.Item1.Add(values[i] + ")");
                    foreach (var pp in newC)
                    {
                        dic3.Item2.Add(pp);
                    }
                }

                else if (!string.IsNullOrEmpty(shardingDB))
                {
                    //只分库的情况
                    if (!dicValues.TryGetValue(shardingDB, out var dic2))
                    {
                        dic2 = new Dictionary<string, Tuple<List<string>, StatementParameterCollection>>();
                        dicValues.Add(shardingDB, dic2);
                    }

                    if (!dic2.TryGetValue("", out var dic3))
                    {
                        dic3 = Tuple.Create(new List<string>(), new StatementParameterCollection());
                        dic2.Add("", dic3);
                    }

                    dic3.Item1.Add(values[i] + ")");
                    foreach (var pp in newC)
                    {
                        dic3.Item2.Add(pp);
                    }
                }
                else if (!string.IsNullOrEmpty(shardingValue))
                {
                    //只分表的情况
                    if (!dicValues.TryGetValue("", out var dic2))
                    {
                        dic2 = new Dictionary<string, Tuple<List<string>, StatementParameterCollection>>();
                        dicValues.Add("", dic2);
                    }

                    if (!dic2.TryGetValue(shardingValue, out var dic3))
                    {
                        dic3 = Tuple.Create(new List<string>(), new StatementParameterCollection());
                        dic2.Add(shardingValue, dic3);
                    }

                    dic3.Item1.Add(values[i] + ")");
                    foreach (var pp in newC)
                    {
                        dic3.Item2.Add(pp);
                    }

                }
                else
                {

                    //添加到第一个分片
                    if (!dicValues.TryGetValue(defaultDBSharding, out var dic2))
                    {
                        dic2 = new Dictionary<string, Tuple<List<string>, StatementParameterCollection>>();
                        dicValues.Add(defaultDBSharding, dic2);
                    }

                    if (!dic2.TryGetValue(defaultTableSharding, out var dic3))
                    {
                        dic3 = Tuple.Create(new List<string>(), new StatementParameterCollection());
                        dic2.Add(defaultTableSharding, dic3);
                    }
                    dic3.Item1.Add(values[i] + ")");
                    foreach (var pp in newC)
                    {
                        dic3.Item2.Add(pp);
                    }
                }
            }

            foreach (var dic in dicValues)
            {
                var newHints = HintsUtil.CloneHints(hints);
                if (!dic.Key.Equals(""))
                {
                    newHints[DALExtStatementConstant.SHARDID] = dic.Key;
                }
                foreach (var dic2 in dic.Value)
                {
                    if (!dic2.Key.Equals(""))
                    {
                        newHints[DALExtStatementConstant.TABLEID] = dic2.Key;
                    }

                    Statement statement = new Statement
                    {
                        DatabaseSet = logicDbName,
                        StatementType = StatementType.Sql,
                        Hints = hints,
                        ShardID = dic.Key,//分库的情况
                        IsSharding = true,
                        TableName = "",
                        SqlOperationType = SqlStatementType.INSERT,
                        Parameters = parameters
                    };
                    statement.StatementText = GetSql(title + string.Join(",", dic2.Value.Item1), dic2.Key);
                    statement.Parameters = dic2.Value.Item2;
                    result.Add(statement);
                }
            }

            return result;
        }
        private static String GetSql(String sql, String tableId)
        {
            return String.IsNullOrEmpty(tableId) ? sql : String.Format(sql, tableId);
        }
    }
}