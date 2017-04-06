using System;
using System.Collections.Generic;
using System.Linq;
#if !NETSTANDARD
using System.Transactions;
#endif
using AntData.ORM.Common.Util;
using AntData.ORM.Dao;
using AntData.ORM.DbEngine.RW;
using AntData.ORM.Enums;

namespace AntData.ORM.DbEngine.DB
{
    /// <summary>
    /// 数据库管理器，也是DAL的初始化管理器
    /// </summary>
    class DatabaseFactory
    {
#region private fields

        /// <summary>
        /// 随机发生器,用来动态选择slave数据库
        /// </summary>
        private static readonly Random Random = new Random();
        private static IReadWriteSplitting readWriteSplit;
        private static readonly Object Locker = new Object();

#endregion

        /// <summary>
        /// 自动读写分离时，获取本次操作涉及到的Database
        /// </summary>
        /// <param name="statement"></param>
        /// <returns></returns>
        public static OperationalDatabases GetDatabasesByStatement(Statement statement)
        {
#if !NETSTANDARD
            if (Transaction.Current != null)
            {
                statement.OperationType = OperationType.Write;//如果是在事物中强制改成读写都走master
            }
#endif
            OperationalDatabases result = null;
            String databaseSet = statement.DatabaseSet;

            if (!DALBootstrap.DatabaseSets.ContainsKey(databaseSet))
                throw new ArgumentOutOfRangeException(String.Format(Properties.Resources.DatabaseSetDoesNotExistException, databaseSet));

            var databaseSetWrapper = DALBootstrap.DatabaseSets[databaseSet];
            if (databaseSetWrapper.DatabaseWrappers.Count == 0)
                throw new DalException(String.Format("DatabaseSet '{0}' doesn't contain any database.", databaseSetWrapper.Name));

            String shard = statement.ShardID ?? String.Empty;
            if (shard.Length > 0 && !databaseSetWrapper.TotalRatios.ContainsKey(shard))
                throw new ArgumentOutOfRangeException(String.Format("Shard '{0}' doesn't exist.", shard));

            if (databaseSetWrapper.EnableReadWriteSpliding)
            {
                if (readWriteSplit == null)
                {
                    lock (Locker)
                    {
                        if (readWriteSplit == null)
                            readWriteSplit = RWSplittingManager.Instance;
                    }
                }
                result = readWriteSplit.GetOperationalDatabases(statement);
            }

            //如果没有合适的数据库
            if (result == null || (result.FirstCandidate == null && result.OtherCandidates.Count == 0))
            {
                result = new OperationalDatabases();
                Database master = databaseSetWrapper.DatabaseWrappers.Single(item => item.DatabaseType == DatabaseType.Master && item.Sharding == shard).Database;
                if (databaseSetWrapper.EnableReadWriteSpliding && statement.OperationType == OperationType.Read)
                {
                    //首先选出所有Slave
                    var slaves = databaseSetWrapper.DatabaseWrappers.Where(item => item.DatabaseType == DatabaseType.Slave && item.Sharding == shard);
                    Int32 count = slaves.Count();

                    //如果多于1个Slave，随机选择一个
                    if (count > 0)
                    {
                        Int32 index = Random.Next(0, count);
                        result.OtherCandidates = new List<Database>();

                        for (Int32 i = 0; i < count; i++)
                        {
                            if (i == index)
                            {
                                result.FirstCandidate = slaves.ElementAt(index).Database;
                            }
                            else
                            {
                                result.OtherCandidates.Add(slaves.ElementAt(i).Database);
                            }
                        }

                        //将主库加入作为最后的备选
                        result.OtherCandidates.Add(master);
                    }
                }
                else
                {
                    //如果强制到写库，仅作Retry，不做Fail Over，因此不提供Candidate Slaves
                    result.FirstCandidate = master;
                }
            }

            if (result.FirstCandidate == null && (result.OtherCandidates == null || result.OtherCandidates.Count == 0))
                throw new ArgumentException("Specified database not found.");
            return result;
        }

    }
}
