using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
#if !NETSTANDARD
using System.Transactions;
#endif
using AntData.ORM.Common.Util;
using AntData.ORM.Dao;
using AntData.ORM.DbEngine.DB;
using AntData.ORM.Properties;

namespace AntData.ORM.DbEngine.HA
{
    abstract class AbstractHA : IHA
    {
      
        public abstract HashSet<Int32> RetryFailOverErrorCodes { get; }

        /// <summary>
        /// 默认重试一次
        /// </summary>
        private Int32 RetryFailOverTimes
        {
            get
            {
                Int32 retryTimes = 0;
                if (retryTimes < 1)
                    retryTimes = 1;
                return retryTimes;
            }
        }

        private Boolean SatisfyRetryFailOverCondition(DbException ex)
        {
#if !NETSTANDARD
            //如果使用了事务，不进行重试或者Fail Over
            if (Transaction.Current != null || ex == null)
                return false;
#endif
            Int32 errorCode = 0;
            if (errorCode != 0 && RetryFailOverErrorCodes != null && RetryFailOverErrorCodes.Count > 0)
                return RetryFailOverErrorCodes.Contains(errorCode);
            return ExceptionUtil.IsTimeoutException(ex);
        }

        private Database FallToNextDatabase(Database current, IList<Database> candidates, BitArray bitArray)
        {
            var result = current;
            if (candidates != null && candidates.Count >= 0)
            {
                Int32 length = bitArray.Length;

                for (Int32 i = 0; i < length; i++)
                {
                    if (!bitArray[i] && candidates[i].Available)
                    {
                        bitArray[i] = true;
                        result = candidates[i];
                        break;
                    }
                }
            }

            return result;
        }

        public T ExecuteWithHa<T>(Func<Database, T> func, OperationalDatabases databases)
        {
            T result = default(T);
            BitArray bitArray = (databases.OtherCandidates != null && databases.OtherCandidates.Count > 0) ? new BitArray(databases.OtherCandidates.Count) : null;
            var currentOperateDatabase = databases.FirstCandidate;
            Int32 retryTimes = RetryFailOverTimes;

            try
            {
                // ExecutorManager.Executor.Daemon();
                //被Mark Down了，且当前Request没有放行
                //while (haBean.EnableHA && retryTimes > 0 && !currentOperateDatabase.Available)
                //{
                //    var fallbackDatabase = FallToNextDatabase(currentOperateDatabase, databases.OtherCandidates, bitArray);
                //    if (fallbackDatabase == currentOperateDatabase)
                //        throw new DalException(String.Format(Resources.DBMarkDownException, currentOperateDatabase.AllInOneKey));
                //    currentOperateDatabase = fallbackDatabase;
                //    retryTimes--;
                //}
                //if (!currentOperateDatabase.Available)
                //    throw new DalException(String.Format(Resources.DBMarkDownException, currentOperateDatabase.AllInOneKey));
                result = func(currentOperateDatabase);
            }
            catch (DalException)
            {
                throw;
            }
            catch (DbException ex)
            {

                var exception = ex;
                Boolean failoverSucceed = false;
                String databaseSet = currentOperateDatabase.DatabaseSetName;
                String allInOneKey = currentOperateDatabase.AllInOneKey;

                while (retryTimes > 0)
                {
                    retryTimes--;

                    try
                    {
                        Boolean failoverNecessary = SatisfyRetryFailOverCondition(exception);
                        if (!failoverNecessary)
                            throw;
                        //有备用的就去备用的执行试试
                        var failoverDatabase = FallToNextDatabase(currentOperateDatabase, databases.OtherCandidates,
                            bitArray);
                        if (failoverDatabase == null)
                            throw;

                        allInOneKey = failoverDatabase.AllInOneKey;
                        result = func(failoverDatabase);
                        failoverSucceed = true;
                    }
                    catch (DbException exp)
                    {
                        exception = exp;
                        failoverSucceed = false;
                    }

                    if (failoverSucceed)
                    {
                        break;
                    }
                }

                if (!failoverSucceed)
                    throw;
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }
    }
}
