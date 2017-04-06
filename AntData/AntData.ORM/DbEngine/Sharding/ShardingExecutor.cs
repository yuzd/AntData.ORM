using System;
using System.Collections.Generic;
using System.Data;
using AntData.ORM.DbEngine;
using AntData.ORM.DbEngine.DB;

namespace AntData.DbEngine.Sharding
{
    class ShardingExecutor
    {
        #region ExecuteShardTable
#if !NETSTANDARD
        /// <summary>
        /// 表结构均相同, 只有一个数据库，将一个表分为多个名称不同的表
        /// </summary>
        /// <param name="statements"></param>
        /// <returns></returns>
        public static DataTable ExecuteShardTable(IList<Statement> statements)
        {
            if (statements == null || statements.Count == 0)
                return null;

            var excuter = new List<Func<IDataReader>>();
            for (Int32 i = 0; i < statements.Count; i++)
            {
                Statement statement = statements[i];
                excuter.Add(() => DatabaseBridge.Instance.ExecuteReader(statement));
            }

            var result = ExecuteParallelHelper.ParallelExcuter(excuter, CheckSameShard(statements));
            return MergeDataReader(result);
        }

#endif
        public static List<int> ExecuteShardingNonQuery(IList<Statement> statements)
        {
            if (statements == null || statements.Count == 0)
                return null;

            var funcs = new List<Func<Int32>>();
            for (Int32 i = 0; i < statements.Count; i++)
            {
                Statement statement = statements[i];
                funcs.Add(() => DatabaseBridge.Instance.ExecuteNonQuery(statement));
            }

            return ExecuteParallelHelper.ParallelExcuter(funcs, CheckSameShard(statements));
        }

        public static IList<object> ExecuteShardingScalar(IList<Statement> statements)
        {
            if (statements == null || statements.Count == 0)
                return null;

            var funcs = new List<Func<object>>();
            for (Int32 i = 0; i < statements.Count; i++)
            {
                Statement statement = statements[i];
                funcs.Add(() => DatabaseBridge.Instance.ExecuteScalar(statement));
            }

            return ExecuteParallelHelper.ParallelExcuter(funcs, CheckSameShard(statements));
        }
#if !NETSTANDARD
        public static DataSet ExecuteShardingDataSet(IList<Statement> statements)
        {
            if (statements == null || statements.Count == 0)
                return null;

            var dataSets = GetShardingDataSetList(statements);
            return MergeDataSet(dataSets);
        }

        private static IList<DataSet> GetShardingDataSetList(IList<Statement> statements)
        {
            var dataSets = new List<Func<DataSet>>();

            for (Int32 i = 0; i < statements.Count; i++)
            {
                Statement statement = statements[i];
                dataSets.Add(() => DatabaseBridge.Instance.ExecuteDataSet(statement));
            }

            return ExecuteParallelHelper.ParallelExcuter(dataSets, CheckSameShard(statements));
        }
#endif
        public static IList<IDataReader> GetShardingDataReaderList(IList<Statement> statements)
        {
            var dataReaders = new List<Func<IDataReader>>();

            for (Int32 i = 0; i < statements.Count; i++)
            {
                Statement statement = statements[i];
                dataReaders.Add(() => DatabaseBridge.Instance.ExecuteReader(statement));
            }

            return ExecuteParallelHelper.ParallelExcuter(dataReaders, CheckSameShard(statements));
        }

#endregion
#if !NETSTANDARD
        private static DataTable MergeDataReader(IList<IDataReader> dataReaders)
        {
            if (dataReaders == null || dataReaders.Count == 0)
                return null;

            var dataTable = new DataTable();

            foreach (var dataReader in dataReaders)
            {
                var dt = new DataTable();
                dt.Load(dataReader);
                dataTable.Merge(dt);
            }

            return dataTable;
        }


        private static DataSet MergeDataSet(IList<DataSet> dataSets)
        {
            if (dataSets == null || dataSets.Count == 0)
                return null;

            DataSet result = null;
            foreach (var dataSet in dataSets)
            {
                if (dataSet != null)
                {
                    if (result == null)
                    {
                        result = dataSet;
                    }
                    else
                    {
                        result.Merge(dataSet);
                    }
                }
            }

            return result;
        }
#endif
        private static bool CheckSameShard(IList<Statement> statements)
        {
            var shardId = (string)null;
            var first = true;
            foreach (var item in statements)
            {
                if (first)
                {
                    first = false;
                    shardId = item.ShardID;
                    continue;
                }
                if (item.ShardID != shardId)
                {
                    return false;
                }
            }
            return true;
        }

    }
}
