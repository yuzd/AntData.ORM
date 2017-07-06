using System;
using System.Collections.Generic;
using System.Linq;
using AntData.ORM.Dao;
using AntData.ORM.DbEngine.Dao.Common.Util;
using AntData.ORM.DbEngine.DB;
using AntData.ORM.Enums;

namespace AntData.ORM.DbEngine.RW
{
    class DefaultReadWriteSplitting : IReadWriteSplitting
    {
        /// <summary>
        ///  读写分离规则
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

                var master = DALBootstrap.Instance().DatabaseSets[databaseSet].DatabaseWrappers.Single(item => item.DatabaseType == DatabaseType.Master);

                var slaves = DALBootstrap.Instance().DatabaseSets[databaseSet].DatabaseWrappers.Where(item => item.DatabaseType == DatabaseType.Slave)
                   .ToList();
                Int32 count = slaves.Count();
                if (statement.OperationType == OperationType.Read && count > 0)
                {
                    
                    //如果多于1个Slave，随机选择一个
                    int index = ThreadLocalRandom.Current.Next(0, count);
                    for (Int32 i = 0; i < count; i++)
                    {
                        if (i == index)
                        {
                            databases.FirstCandidate = slaves.ElementAt(index).Database;
                        }
                        else
                        {
                            databases.OtherCandidates.Add(slaves.ElementAt(i).Database);
                        }
                    }
                    //将主库加入作为最后的备选
                    databases.OtherCandidates.Add(master.Database);
                }
                else
                {
                    databases.FirstCandidate = master.Database;
                    databases.OtherCandidates.Add(count>0 ? slaves.ElementAt(0).Database:null);
                }
               
            }
            catch(Exception ex)
            {
                throw new DalException(ex.Message + Environment.NewLine + ex.StackTrace);
            }

            return databases;
        }
    }
}
