using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AntData.ORM.Dao.Common;
using AntData.ORM.DbEngine;
using AntData.ORM.DbEngine.Configuration;
using AntData.ORM.DbEngine.Dao.Common.Util;
using AntData.ORM.DbEngine.Sharding;

namespace AntData.DbEngine.Sharding
{
    class SequenceShardingStrategy : IShardingStrategy
    {
        //通常一个字段就够用了，但是如果不够，比如A表用CityId，B表用OrderId，可以在配置中设置,但是 CityId和OrderId不能同时出现在同一表中，否则无法自动识别
        private readonly IList<String> shardColumns = new List<String>();
        private readonly List<SequenceInnerClass> shards = new List<SequenceInnerClass>();
        private readonly IDictionary<String, String> shardColumnAndTable = new Dictionary<String, String>();
        private readonly ISet<String> allShards = new HashSet<String>();
        private Boolean shardByDB = true;
        private Boolean shardByTable;

        /// <summary>
        /// 测试时需要注意computeByColumn的Delegate返回Null的情况
        /// </summary>
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
                String strColumnValue = columnValue is DateTime ? Convert.ToDateTime(columnValue).ToString("yyyy-MM-dd HH:mm:ss") : columnValue.ToString();

                foreach (SequenceInnerClass s in shards)
                {
                    if (String.Compare(strColumnValue, s.SequenceStart.ToString(), StringComparison.Ordinal) >= 0 && String.Compare(strColumnValue, s.SequenceEnd.ToString(), StringComparison.Ordinal) <= 0)
                    {
                        shardid = s.Sharding;
                        break;
                    }
                }
            }

            return shardid;
        }


        public void SetShardConfig(IDictionary<String, String> config, List<ShardingConfig> ShardingConfig)
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

            foreach (var db in ShardingConfig)
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

        public IList<String> AllShards => allShards.ToList();

        public IList<String> ShardColumns => shardColumns;

        public IDictionary<String, String> ShardColumnAndTable => shardColumnAndTable;

        public Boolean ShardByDB => shardByDB;

        public Boolean ShardByTable => shardByTable;

  
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



        class SequenceInnerClass
        {
            public Object SequenceStart { get; set; }

            public Object SequenceEnd { get; set; }

            public String Sharding { get; set; }
        }
    }
}
