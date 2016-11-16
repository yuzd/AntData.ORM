using System;
using System.Collections.Generic;
using System.Linq;
using AntData.ORM.DbEngine.DB;
using AntData.ORM.Enums;

namespace AntData.ORM.DbEngine.RW
{
    class DefaultReadWriteSplitting : IReadWriteSplitting
    {
        /// <summary>
        /// 目前只支持一主一丛
        /// </summary>
        /// <param name="statement"></param>
        /// <returns></returns>
        public OperationalDatabases GetOperationalDatabases(Statement statement)
        {
            OperationalDatabases databases = new OperationalDatabases
            {
                OtherCandidates = new List<Database>()
            };

            try
            {
                String databaseSet = statement.DatabaseSet;
                var master = DALBootstrap.DatabaseSets[databaseSet].DatabaseWrappers.Where(item => item.DatabaseType == DatabaseType.Master).Single();
                var slaves = DALBootstrap.DatabaseSets[databaseSet].DatabaseWrappers.Where(item => item.DatabaseType == DatabaseType.Slave)
                   .Single();
                databases.FirstCandidate = slaves.Database;
                databases.OtherCandidates.Add(master.Database);
               
            }
            catch
            {
                throw;
            }

            return databases;
        }
    }
}
