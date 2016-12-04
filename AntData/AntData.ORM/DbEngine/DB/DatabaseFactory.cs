using System;
using System.Collections.Generic;
using System.Linq;
using AntData.ORM.Common.Util;
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
            OperationalDatabases result = null;
            String databaseSet = statement.DatabaseSet;

            if (!DALBootstrap.DatabaseSets.ContainsKey(databaseSet))
                throw new ArgumentOutOfRangeException(String.Format(Properties.Resources.DatabaseSetDoesNotExistException, databaseSet));

            var databaseSetWrapper = DALBootstrap.DatabaseSets[databaseSet];
            if (databaseSetWrapper.DatabaseWrappers.Count == 0)
                throw new System.Configuration.ConfigurationErrorsException(String.Format("DatabaseSet '{0}' doesn't contain any database.", databaseSetWrapper.Name));

           
            if (databaseSetWrapper.EnableReadWriteSpliding)
            {
                if (readWriteSplit == null)
                {
                    lock (Locker)
                    {
                        if (readWriteSplit == null)
                            readWriteSplit = RWSplittingManager.Instance;
                    }

                    result = readWriteSplit.GetOperationalDatabases(statement);
                }
            }

            //如果没有合适的数据库
            if (result == null || (result.FirstCandidate == null && result.OtherCandidates.Count == 0))
            {
                result = new OperationalDatabases();
                Database master = databaseSetWrapper.DatabaseWrappers.Single(item => item.DatabaseType == DatabaseType.Master).Database;
                result.FirstCandidate = master;
            }

            if (result.FirstCandidate == null && (result.OtherCandidates == null || result.OtherCandidates.Count == 0))
                throw new ArgumentException("Specified database not found.");
            return result;
        }

    }
}
