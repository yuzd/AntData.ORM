using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AntData.ORM.Dao;
using AntData.ORM.Dao.Common;
using AntData.ORM.DbEngine;
using AntData.ORM.DbEngine.Configuration;
using AntData.ORM.DbEngine.Dao.Common.Util;
using AntData.ORM.DbEngine.Sharding;

namespace AntData.DbEngine.Sharding
{
    class ModShardingStrategy : IShardingStrategy
    {
        private Int32 MOD = -1;
        //通常一个字段就够用了，但是如果不够，比如A表用CityId，B表用OrderId，可以在配置中设置
        private readonly IList<String> shardColumns = new List<String>();
        private readonly IList<String> shardDbColumns = new List<String>();
        private readonly ISet<String> tableShards = new HashSet<String>();
        private readonly ISet<String> dbShards = new HashSet<String>();
        /// <summary>
        /// key 为table 名称 value 为 字段名称
        /// </summary>
        private readonly IDictionary<String, String> shardColumnAndTable = new Dictionary<String, String>();
        private readonly IDictionary<String, String> dbshardColumnAndTable = new Dictionary<String, String>();
        private Boolean shardByDB = true;
        private Boolean shardByTable;

      
        /// <summary>
        /// 通过取模, 获取字段的值应该位于哪个Shard
        /// </summary>
        /// <typeparam name="TColumnType"></typeparam>
        /// <param name="columnValue"></param>
        /// <returns></returns>
        public String ComputeShardId<TColumnType>(TColumnType columnValue, bool isDb) where TColumnType : IComparable
        {
            Int32 modValue = -1;
            if (columnValue == null)
            {
                return isDb?dbShards.FirstOrDefault():tableShards.FirstOrDefault();
            }
            if (!TypeUtils.IsNumericType(columnValue.GetType()))
            {
                modValue = Math.Abs(columnValue.GetHashCode()) % (isDb? dbShards.Count:  MOD);
            }
            else
            {
                //to be refactored
                switch (TypeUtils.GetNumericTypeEnum(columnValue.GetType()))
                {
                    case NumericType.Int:
                        modValue = Convert.ToInt32(columnValue) % (isDb ? dbShards.Count : MOD);
                        break;
                    case NumericType.Long:
                        modValue = (Int32)(Convert.ToInt64(columnValue) % (isDb ? dbShards.Count : MOD));
                        break;
                    case NumericType.Double:
                        modValue = (Int32)(Convert.ToDouble(columnValue) % (isDb ? dbShards.Count : MOD));
                        break;
                }
            }

            return modValue > -1 ? modValue.ToString() : null;
        }
        

        public void SetShardConfig(IDictionary<String, String> config, List<ShardingConfig> allShardList)
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
                        shardColumnAndTable[tableColumnPair[0]] = tableColumnPair[1];
                        shardColumns.Add(tableColumnPair[1]);
                    }
                    else
                    {
                        //兼容最初版本的DAL.config配置
                        shardColumns.Add(column);
                    }
                }
            }

            string dbColumn;
            if (config.TryGetValue("dbcolumn", out dbColumn))
            {
                var tempColumns = dbColumn.Split(',');
                foreach (var column in tempColumns)
                {
                    if (column.Contains(':'))
                    {
                        var tableColumnPair = column.Split(':');
                        dbshardColumnAndTable[tableColumnPair[0]] = tableColumnPair[1];
                        shardDbColumns.Add(tableColumnPair[1]);
                    }
                    else
                    {
                        shardDbColumns.Add(column);
                    }
                   
                }
            }

            //如果db没有专门配置就共用table的配置
            if (!shardDbColumns.Any())
            {
                foreach (var shardColumn in shardColumns)
                {
                    shardDbColumns.Add(shardColumn);
                }
            }
            if (!dbshardColumnAndTable.Any())
            {
                foreach (var shardColumn in shardColumnAndTable)
                {
                    dbshardColumnAndTable.Add(shardColumn);
                }
            }

            string tableSharding;
            if (config.TryGetValue("tableSharding", out tableSharding))
            {
                var tempTableSharding = tableSharding.Split(',');
                foreach (var s in tempTableSharding)
                {
                    tableShards.Add(s);
                }
            }

            if (!Int32.TryParse(tempMod, out MOD))
                throw new ArgumentException("Mod settings invalid.");

            if (!tableShards.Any())
            {
                for (int i = 0; i < MOD; i++)
                {
                    tableShards.Add(i + "");
                }
            }
            if (allShardList != null && allShardList.Count > 0)
            {
                foreach (var s in allShardList)
                {
                   if(!string.IsNullOrWhiteSpace(s.Sharding)) dbShards.Add(s.Sharding);
                }
            }
           

            String shardByDb;
            String tmpShardByTable;

            if (config.TryGetValue("shardByDB", out shardByDb))
                Boolean.TryParse(shardByDb, out shardByDB);
            if (config.TryGetValue("shardByTable", out tmpShardByTable))
                Boolean.TryParse(tmpShardByTable, out shardByTable);

            if (shardByTable && tableShards.Count < 1)
            {
                throw new ArgumentException("Mod settings invalid. shardByTable must have key:[tableSharding] in ShardingStrategy");
            }

            if (shardByDB && dbShards.Count < 1)
            {
                throw new ArgumentException("Mod settings invalid. shardByDb must have key:[Sharding] in ConnectionItemList");
            }
        }

        public IList<String> AllTableShards => tableShards.ToList();
        public IList<string> AllDbShards => dbShards.ToList();

        public IList<String> TableShardColumns => shardColumns;

        public IDictionary<String, String> ShardColumnAndTable => shardColumnAndTable;
        public IList<string> DbShardColumns => shardDbColumns;
        public IDictionary<string, string> DbShardColumnAndTable => dbshardColumnAndTable;

        public Boolean ShardByDB => shardByDB;

        public Boolean ShardByTable => shardByTable;


        public List<IComparable> GetShardColumnValueList(string logicDbName, StatementParameterCollection parameters, IDictionary hints,bool isDb)
        {
            List<IComparable> shardColumnValueList = new List<IComparable>();
            var tempshardColumns = isDb? shardDbColumns: shardColumns;

            if ((tempshardColumns == null || tempshardColumns.Count == 0))
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

                dict.Add(Tuple.Create<string, StatementParameter>(parameterName, item));
            }

            var quote = string.Empty;
            if (hints != null && hints.Contains(DALExtStatementConstant.PARAMETER_SYMBOL))
            {
                quote = hints[DALExtStatementConstant.PARAMETER_SYMBOL] as string;
            }
            foreach (var item in tempshardColumns)
            {
               
                String name = quote + item.ToLower();
                foreach (var itemKv in dict)
                {
                    IComparable shardColumnValue = null;
                    if (itemKv.Item1.Equals(name))
                    {
                        shardColumnValue = (itemKv.Item2.Value ?? itemKv.Item2.DbDataParameter?.Value) as IComparable;
                     
                        if (!isDb)
                        {
                            itemKv.Item2.IsShardingColumn = true;
                            itemKv.Item2.ShardingValue = ComputeShardId(shardColumnValue, isDb);
                        }
                        else
                        {
                            itemKv.Item2.IsShardingDb = true;
                            itemKv.Item2.ShardingDbValue = ComputeShardId(shardColumnValue, isDb);
                        }
                    }
                    else if (itemKv.Item1.Equals(item))
                    {
                        shardColumnValue = (itemKv.Item2.Value??itemKv.Item2.DbDataParameter?.Value) as IComparable;
                       
                        if (!isDb)
                        {
                            itemKv.Item2.IsShardingColumn = true;
                            itemKv.Item2.ShardingValue = ComputeShardId(shardColumnValue, isDb);
                        }
                        else
                        {
                            itemKv.Item2.IsShardingDb = true;
                            itemKv.Item2.ShardingDbValue = ComputeShardId(shardColumnValue, isDb);
                        }
                        
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

