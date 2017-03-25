using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AntData.ORM.Dao;
using AntData.ORM.Dao.Common;
using AntData.ORM.DbEngine;
using AntData.ORM.DbEngine.Configuration;
using AntData.ORM.DbEngine.Dao.Common.Util;

namespace AntData.DbEngine.Sharding
{
    class ModShardingStrategy : IShardingStrategy
    {
        private Int32 MOD = -1;
        //通常一个字段就够用了，但是如果不够，比如A表用CityId，B表用OrderId，可以在配置中设置
        private IList<String> shardColumns = new List<String>();
        private ISet<String> allShards = new HashSet<String>();
        /// <summary>
        /// key 为table 名称 value 为 字段名称
        /// </summary>
        private IDictionary<String, String> shardColumnAndTable = new Dictionary<String, String>();
        private Boolean shardByDB = true;
        private Boolean shardByTable;

        /// <summary>
        /// 将数据按照当前的策略进行分组，返回ShardID：T的键值对，主要用于增删改
        /// </summary>
        /// <typeparam name="T">需要Shuffle的类型</typeparam>
        /// <typeparam name="TColumnType">Sharding的字段</typeparam>
        /// <param name="dataList">需要Shuffle的数据</param>
        /// <param name="shuffleByColumn"></param>
        /// <returns>ShardID：T的键值对</returns>
        public IDictionary<String, IList<T>> ShuffleData<T, TColumnType>(IList<T> dataList, Func<T, TColumnType> shuffleByColumn) where TColumnType : IComparable
        {
            var results = new Dictionary<String, IList<T>>();
            if (dataList == null || shuffleByColumn == null)
                return results;

            foreach (var t in dataList)
            {
                var column = shuffleByColumn(t);
                String shard = ComputeShardId(column);

                if (results.ContainsKey(shard))
                {
                    results[shard].Add(t);
                }
                else
                {
                    IList<T> currentGroup = new List<T>();
                    currentGroup.Add(t);
                    results[shard] = currentGroup;
                }
            }

            return results;
        }

        /// <summary>
        /// 通过取模, 获取字段的值应该位于哪个Shard
        /// </summary>
        /// <typeparam name="TColumnType"></typeparam>
        /// <param name="columnValue"></param>
        /// <returns></returns>
        public String ComputeShardId<TColumnType>(TColumnType columnValue) where TColumnType : IComparable
        {
            Int32 modValue = -1;
            if (columnValue == null)
            {
                return null;
            }
            if (!TypeUtils.IsNumericType(columnValue.GetType()))
            {
                modValue = Math.Abs(columnValue.GetHashCode()) % MOD;
            }
            else
            {
                //to be refactored
                switch (TypeUtils.GetNumericTypeEnum(columnValue.GetType()))
                {
                    case NumericType.Int:
                        modValue = Convert.ToInt32(columnValue) % MOD;
                        break;
                    case NumericType.Long:
                        modValue = (Int32)(Convert.ToInt64(columnValue) % MOD);
                        break;
                    case NumericType.Double:
                        modValue = (Int32)(Convert.ToDouble(columnValue) % MOD);
                        break;
                }
            }

            return modValue > -1 ? modValue.ToString() : null;
        }

        public IList<String> ComputeShardIdsBetween<TColumnType>(TColumnType start, TColumnType end) where TColumnType : IComparable
        {
            if (!TypeUtils.IsNumericType(typeof(TColumnType)))
                throw new DalException("Between does not support hashcode/mod,mod strategy require numeric type.");

            var resultShards = new HashSet<String>();

            switch (TypeUtils.GetNumericTypeEnum(typeof(TColumnType)))
            {
                case NumericType.Int:
                    Int32 intStart = Convert.ToInt32(start);
                    Int32 intEnd = Convert.ToInt32(end);

                    if (intEnd - intStart + 1 < MOD)
                    {
                        for (Int32 i = intStart; i <= intEnd; i++)
                        {
                            resultShards.Add((i % MOD).ToString());
                        }

                        return resultShards.ToList();
                    }
                    break;
                case NumericType.Long:
                    Int64 longStart = Convert.ToInt64(start);
                    Int64 longEnd = Convert.ToInt64(end);

                    if (longEnd - longStart + 1 < MOD)
                    {
                        for (Int64 i = longStart; i <= longEnd; i++)
                        {
                            resultShards.Add((i % MOD).ToString());
                        }

                        return resultShards.ToList();
                    }
                    break;
                case NumericType.Double:
                    Double doubleStart = Convert.ToDouble(start);
                    Double doubleEnd = Convert.ToDouble(end);

                    if (doubleEnd - doubleStart + 1 < MOD)
                    {
                        for (Double i = doubleStart; i <= doubleEnd; i++)
                        {
                            resultShards.Add(((Int32)(i % MOD)).ToString());
                        }

                        return resultShards.ToList();
                    }
                    break;
            }

            return allShards.ToList();
        }

        public IList<String> ComputeShardIdsIn<TColumnType>(IList<TColumnType> columnValues) where TColumnType : IComparable
        {
            return ComputeShardIdsIn(columnValues.ToArray());
        }

        public IList<String> ComputeShardIdsIn<TColumnType>(params TColumnType[] columnValues) where TColumnType : IComparable
        {
            var resultShards = new HashSet<String>();

            foreach (var col in columnValues)
            {
                resultShards.Add(ComputeShardId(col));
            }

            return resultShards.Count > 0 ? resultShards.ToList() : allShards.ToList();
        }

        public void SetShardConfig(IDictionary<String, String> config, DatabaseSetElement databaseSet)
        {
            String tempMod;
            if (!config.TryGetValue("mod", out tempMod))
                throw new ArgumentException("sharding config does not contains mod setting!");

            String tempColumn;
            if (config.TryGetValue("column", out tempColumn))
            {
                var tempColumns = tempColumn.Split(',');
                foreach (var column in tempColumns)
                {
                    if (column.Contains(':'))
                    {
                        var tableColumnPair = column.Split(':');
                        shardColumnAndTable[tableColumnPair[0].ToLower()] = tableColumnPair[1].ToLower();
                        shardColumns.Add(tableColumnPair[1].ToLower());
                    }
                    else
                    {
                        //兼容最初版本的DAL.config配置
                        shardColumns.Add(column.ToLower());
                    }
                }
            }

            string tableSharding;
            if (config.TryGetValue("tableSharding", out tableSharding))
            {
                var tempTableSharding = tableSharding.Split(',');
                foreach (var s in tempTableSharding)
                {
                    allShards.Add(s);
                }
            }

            if (!Int32.TryParse(tempMod, out MOD))
                throw new ArgumentException("Mod settings invalid.");

            foreach (DatabaseElement db in databaseSet.Databases)
            {
                if (!string.IsNullOrWhiteSpace(db.Sharding))
                {
                    allShards.Add(db.Sharding);
                }
            }

            String shardByDb;
            String tmpShardByTable;

            if (config.TryGetValue("shardByDB", out shardByDb))
                Boolean.TryParse(shardByDb, out shardByDB);
            if (config.TryGetValue("shardByTable", out tmpShardByTable))
                Boolean.TryParse(tmpShardByTable, out shardByTable);

            if (shardByTable && allShards.Count < 1)
            {
                throw new ArgumentException("Mod settings invalid. shardByTable must have key:[tableSharding]");
            }
        }

        public IList<String> AllShards
        {
            get { return allShards.ToList(); }
        }

        public IList<String> ShardColumns
        {
            get { return shardColumns; }
        }

        public IDictionary<String, String> ShardColumnAndTable
        {
            get { return shardColumnAndTable; }
        }

        public Boolean ShardByDB
        {
            get { return shardByDB; }
        }

        public Boolean ShardByTable
        {
            get { return shardByTable; }
        }

     
      


        private static IComparable GetShardColumnValueByMap(IList<String> shardColumns, IDictionary hints)
        {
            if (shardColumns == null || shardColumns.Count == 0)
                return null;
            if (hints == null)
                return null;

            if (!hints.Contains(DALExtStatementConstant.MAP))
                return null;
            var dict = hints[DALExtStatementConstant.MAP] as Dictionary<String, Object>;
            if (dict == null)
                return null;

            foreach (var item in shardColumns)
            {
                if (dict.ContainsKey(item))
                    return dict[item] as IComparable;
            }

            return null;
        }

        private static IComparable GetShardColumnValueByValue(IDictionary hints)
        {
            if (hints == null)
                return null;

            if (!hints.Contains(DALExtStatementConstant.SHARD_COLUMN_VALUE))
                return null;
            var shardColumnValue = hints[DALExtStatementConstant.SHARD_COLUMN_VALUE] as IComparable;
            return shardColumnValue;
        }


        #region 要删除

        public IComparable GetShardColumnValue(String logicDbName, StatementParameterCollection parameters, IDictionary hints)
        {
            if (String.IsNullOrEmpty(logicDbName))
                return null;
            if (shardColumns == null || shardColumns.Count == 0)
                return null;

            //Verify by shard column value in hints
            var shardColumnValue = GetShardColumnValueByValue(hints);

            //Verify by map in hints
            if (shardColumnValue == null)
                shardColumnValue = GetShardColumnValueByMap(shardColumns, hints);

            //Verify by parameters
            if (shardColumnValue == null)
                shardColumnValue = GetShardColumnValueByParameters(logicDbName, shardColumns, parameters, hints);

            return shardColumnValue;
        }


        private static IComparable GetShardColumnValueByParameters(String logicDbName, IList<String> shardColumns, StatementParameterCollection parameters, IDictionary hints)
        {
            IComparable shardColumnValue = null;

            if (shardColumns == null || shardColumns.Count == 0)
                return null;
            if (parameters == null || parameters.Count == 0)
                return null;

            var dict = new Dictionary<String, StatementParameter>();

            foreach (var item in parameters)
            {
                String parameterName = item.Name;
                if (String.IsNullOrEmpty(parameterName)) continue;

                parameterName = parameterName.ToLower();
                if (!dict.ContainsKey(parameterName)) dict.Add(parameterName, item);

                parameterName = item.ColumnName;
                if (String.IsNullOrEmpty(parameterName)) continue;

                parameterName = parameterName.ToLower();
                if (!dict.ContainsKey(parameterName)) dict.Add(parameterName, item);
            }

            var quote = string.Empty;
            if (hints != null && hints.Contains(DALExtStatementConstant.PARAMETER_SYMBOL))
            {
                quote = hints[DALExtStatementConstant.PARAMETER_SYMBOL] as string;
            }
            foreach (var item in shardColumns)
            {
                String name = quote + item.ToLower();

                if (dict.ContainsKey(name))
                {
                    shardColumnValue = dict[name].Value as IComparable;
                    break;
                }

                if (dict.ContainsKey(item.ToLower()))
                {
                    shardColumnValue = dict[item.ToLower()].Value as IComparable;
                    break;
                }
            }

            return shardColumnValue;
        }

        #endregion

        public List<IComparable> GetShardColumnValueList(string logicDbName, StatementParameterCollection parameters, IDictionary hints)
        {
            List<IComparable> shardColumnValueList = new List<IComparable>();

            if (shardColumns == null || shardColumns.Count == 0)
                return shardColumnValueList;
            if (parameters == null || parameters.Count == 0)
                return shardColumnValueList;

            var dict = new List<Tuple<String, StatementParameter>>();

            foreach (var item in parameters)
            {
                String parameterName = item.Name;
                if (String.IsNullOrEmpty(parameterName)) continue;

                parameterName = parameterName.ToLower();
                dict.Add(Tuple.Create<string, StatementParameter>(parameterName, item));

                parameterName = item.ColumnName;
                if (String.IsNullOrEmpty(parameterName)) continue;

                parameterName = parameterName.ToLower();
                dict.Add(Tuple.Create<string, StatementParameter>(parameterName, item));
            }

            var quote = string.Empty;
            if (hints != null && hints.Contains(DALExtStatementConstant.PARAMETER_SYMBOL))
            {
                quote = hints[DALExtStatementConstant.PARAMETER_SYMBOL] as string;
            }
            foreach (var item in shardColumns)
            {
               
                String name = quote + item.ToLower();

                foreach (var itemKv in dict)
                {
                    IComparable shardColumnValue = null;
                    if (itemKv.Item1.Equals(name))
                    {
                        shardColumnValue = itemKv.Item2.Value as IComparable;
                        itemKv.Item2.IsShardingColumn = true;
                        itemKv.Item2.ShardingValue = ComputeShardId(shardColumnValue);
                    }
                    else if (itemKv.Item1.Equals(item.ToLower()))
                    {
                        shardColumnValue = itemKv.Item2.Value as IComparable;
                        itemKv.Item2.IsShardingColumn = true;
                        itemKv.Item2.ShardingValue = ComputeShardId(shardColumnValue);
                    }

                    if (shardColumnValue != null)
                    {
                        shardColumnValueList.Add(shardColumnValue);
                    }
                }
               
            }

            return shardColumnValueList;
        }
    }
}

