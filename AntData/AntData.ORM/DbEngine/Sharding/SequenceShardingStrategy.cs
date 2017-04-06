using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AntData.ORM.Dao.Common;
using AntData.ORM.DbEngine;
using AntData.ORM.DbEngine.Configuration;
using AntData.ORM.DbEngine.Dao.Common.Util;

namespace AntData.DbEngine.Sharding
{
    class SequenceShardingStrategy : IShardingStrategy
    {
        //通常一个字段就够用了，但是如果不够，比如A表用CityId，B表用OrderId，可以在配置中设置,但是 CityId和OrderId不能同时出现在同一表中，否则无法自动识别
        private IList<String> shardColumns = new List<String>();
        private List<SequenceInnerClass> shards = new List<SequenceInnerClass>();
        private IDictionary<String, String> shardColumnAndTable = new Dictionary<String, String>();
        private ISet<String> allShards = new HashSet<String>();
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
            IDictionary<String, IList<T>> results = new Dictionary<String, IList<T>>();

            if (dataList == null || shuffleByColumn == null)
                return results;

            foreach (T t in dataList)
            {
                TColumnType column = shuffleByColumn(t);
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
        /// 测试时需要注意computeByColumn的Delegate返回Null的情况
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <param name="computeByColumn"></param>
        /// <returns></returns>
        public String ComputeShardId<TColumnType>(TColumnType columnValue) where TColumnType : IComparable
        {
            String shardid = null;
            if (columnValue == null)
            {
                return null;
            }

            Type type = columnValue.GetType();
            
            if (TypeUtils.IsNumericType(type))
            {
                foreach (SequenceInnerClass s in shards)
                {
                    s.SequenceStart = TypeUtils.GetNumericValue<TColumnType>(type, s.SequenceStart);
                    s.SequenceEnd = TypeUtils.GetNumericValue<TColumnType>(type, s.SequenceEnd);

                    if (columnValue.CompareTo(s.SequenceStart) >= 0 && columnValue.CompareTo(s.SequenceEnd) <= 0)
                    {
                        shardid = s.Sharding;
                        break;
                    }
                }
            }
            else
            {
                String strColumnValue = columnValue.GetType() == typeof(DateTime) ? Convert.ToDateTime(columnValue).ToString("yyyy-MM-dd HH:mm:ss") : columnValue.ToString();

                foreach (SequenceInnerClass s in shards)
                {
                    if (strColumnValue.CompareTo(s.SequenceStart.ToString()) >= 0 && strColumnValue.CompareTo(s.SequenceEnd.ToString()) <= 0)
                    {
                        shardid = s.Sharding;
                        break;
                    }
                }
            }

            return shardid;
        }

        public IList<String> ComputeShardIdsBetween<TColumnType>(TColumnType start, TColumnType end) where TColumnType : IComparable
        {
            if (start.CompareTo(end) >= 0)
                throw new ArgumentException("end must greater than start!");

            ISet<String> resultShards = new HashSet<String>();
            //之后考虑优化查找算法
            if (TypeUtils.IsNumericType(start.GetType()))
            {
                foreach (SequenceInnerClass s in shards)
                {
                    s.SequenceStart = TypeUtils.GetNumericValue<TColumnType>(typeof(TColumnType), s.SequenceStart);
                    s.SequenceEnd = TypeUtils.GetNumericValue<TColumnType>(typeof(TColumnType), s.SequenceEnd);

                    //range overlap checking
                    if (start.CompareTo(s.SequenceEnd) == 0 || end.CompareTo(s.SequenceStart) == 0 || (start.CompareTo(s.SequenceEnd) < 0 && end.CompareTo(s.SequenceStart) > 0))
                    {
                        resultShards.Add(s.Sharding);
                    }
                }
            }
            else
            {
                String strStart = start.ToString();
                String strEnd = end.ToString();

                foreach (SequenceInnerClass s in shards)
                {
                    if (strStart.CompareTo(s.SequenceEnd.ToString()) == 0 || strEnd.CompareTo(s.SequenceStart.ToString()) == 0 || (strStart.CompareTo(s.SequenceEnd.ToString()) < 0 && strEnd.CompareTo(s.SequenceStart.ToString()) > 0))
                    {
                        resultShards.Add(s.Sharding);
                    }
                }
            }

            return resultShards.Count > 0 ? resultShards.ToList() : allShards.ToList();
        }

        public IList<String> ComputeShardIdsIn<TColumnType>(IList<TColumnType> columnValues) where TColumnType : IComparable
        {
            return ComputeShardIdsIn<TColumnType>(columnValues.ToArray<TColumnType>());
        }

        public IList<String> ComputeShardIdsIn<TColumnType>(params TColumnType[] columnValues) where TColumnType : IComparable
        {
            ISet<String> resultShards = new HashSet<String>();

            foreach (TColumnType col in columnValues)
            {
                resultShards.Add(ComputeShardId<TColumnType>(col));
            }

            return resultShards.Count > 0 ? resultShards.ToList() : allShards.ToList();
        }

        class SequenceInnerClass
        {
            public Object SequenceStart { get; set; }

            public Object SequenceEnd { get; set; }

            public String Sharding { get; set; }
        }

        public void SetShardConfig(IDictionary<String, String> config, DatabaseSetElement databaseSet)
        {
            String tempColumn = null;

            if (config.TryGetValue("column", out tempColumn))
            {
                //shardColumns = tempColumn.Split(',').ToList();
                String[] tempColumns = tempColumn.Split(',');

                foreach (String column in tempColumns)
                {
                    if (column.Contains(':'))
                    {
                        String[] tableColumnPair = column.Split(':');
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

            foreach (DatabaseElement db in databaseSet.Databases)
            {
                shards.Add(new SequenceInnerClass() { Sharding = db.Sharding, SequenceStart = db.Start, SequenceEnd = db.End });
                allShards.Add(db.Sharding);
            }

            String _shardByDb;
            String _shardByTable;

            if (config.TryGetValue("shardByDB", out _shardByDb))
                Boolean.TryParse(_shardByDb, out shardByDB);

            if (config.TryGetValue("shardByTable", out _shardByTable))
                Boolean.TryParse(_shardByTable, out shardByTable);
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

       


        private static IComparable getShardColumnValueByMap(IList<String> shardColumns, IDictionary hints)
        {
            IComparable shardColumnValue = null;

            if (shardColumns == null || shardColumns.Count == 0)
                return shardColumnValue;

            if (hints == null)
                return shardColumnValue;

            if (!hints.Contains(DALExtStatementConstant.MAP))
                return shardColumnValue;

            IDictionary<String, Object> dict = hints[DALExtStatementConstant.MAP] as Dictionary<String, Object>;

            if (dict == null)
                return shardColumnValue;

            foreach (var item in shardColumns)
            {
                if (dict.ContainsKey(item))
                    shardColumnValue = dict[item] as IComparable;
            }

            return shardColumnValue;
        }

        private static IComparable getShardColumnValueByValue(IDictionary hints)
        {
            IComparable shardColumnValue = null;

            if (hints == null)
                return shardColumnValue;

            if (!hints.Contains(DALExtStatementConstant.SHARD_COLUMN_VALUE))
                return shardColumnValue;

            shardColumnValue = hints[DALExtStatementConstant.SHARD_COLUMN_VALUE] as IComparable;
            return shardColumnValue;
        }
        #region 要删除

        public IComparable GetShardColumnValue(String logicDbName, StatementParameterCollection parameters, IDictionary hints)
        {
            IComparable shardColumnValue = null;

            if (String.IsNullOrEmpty(logicDbName))
                return shardColumnValue;

            if (shardColumns == null || shardColumns.Count == 0)
                return shardColumnValue;

            //Verify by shard column value in hints
            shardColumnValue = getShardColumnValueByValue(hints);

            //Verify by map in hints
            if (shardColumnValue == null)
                shardColumnValue = getShardColumnValueByMap(shardColumns, hints);

            //Verify by parameters
            if (shardColumnValue == null)
                shardColumnValue = getShardColumnValueByParameters(logicDbName, shardColumns, parameters, hints);
            return shardColumnValue;
        }

        private static IComparable getShardColumnValueByParameters(String logicDbName, IList<String> shardColumns, StatementParameterCollection parameters, IDictionary hints)
        {
            IComparable shardColumnValue = null;

            if (shardColumns == null || shardColumns.Count == 0)
                return null;

            if (parameters == null || parameters.Count == 0)
                return null;


            IDictionary<String, StatementParameter> dict = new Dictionary<String, StatementParameter>();

            foreach (var item in parameters)
            {
                String parameterName = item.Name;

                if (String.IsNullOrEmpty(parameterName))
                    continue;

                parameterName = parameterName.ToLower();

                if (!dict.ContainsKey(parameterName))
                    dict.Add(parameterName, item);

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
                String name = item.ToLower();

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
