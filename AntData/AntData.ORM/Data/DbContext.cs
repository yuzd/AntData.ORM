//-----------------------------------------------------------------------
// <copyright file="SqlServerDataConnection.cs" company="Company">
// Copyright (C) Company. All Rights Reserved.
// </copyright>
// <author>nainaigu</author>
// <summary></summary>
//-----------------------------------------------------------------------
#if !NETSTANDARD
using System.Transactions;
#endif
using AntData.ORM.DataProvider;
using System;
using AntData.ORM.Dao.Common;
using AntData.ORM.DbEngine.Enums;
using AntData.ORM.Mapping;

namespace AntData.ORM.Data
{

    public abstract class DbContext : AntData.ORM.Data.DataConnection, IDataContext
    {
        protected abstract IDataProvider provider { get; }
        public DbContext(string dbMappingName) : base(dbMappingName)
        {
            
        }

    }


    /// <summary>
    /// 
    /// </summary>
    public abstract class DbContext<T> : DbContext
    {
      
        private readonly Lazy<T> _lazy = null;
        public T Tables
        {
            get
            {
                return _lazy.Value;
            }
        }

        public DbContext(string dbMappingName):base(dbMappingName)
        {
            base.DataProvider = provider;
            _lazy = new Lazy<T>(() =>
            {
                try
                {
                    Type type = typeof(T);
                    var ctor = type.GetConstructors();
                    if (ctor.Length > 0)
                    {
                        return (T)ctor[0].Invoke(new object[] { this });
                    }
                }
                catch (Exception)
                {

                    throw;
                }
                return default(T);
            });
        }




        #region Transaction

        public void UseTransaction(System.Action<DbContext<T>> func)
        {

            using (var scope = ExecuteTransaction())
            {
                try
                {
                    func(this);
                    scope.Commit();
                }
                catch (Exception)
                {
                    scope.Rollback();
                    throw;
                }
                finally
                {
                    Close();
                }
            }
        }

        public void UseTransaction(System.Action<DbContext<T>> func, System.Data.IsolationLevel isolationLevel)
        {
            using (var scope = ExecuteTransaction(isolationLevel))
            {
                try
                {
                    func(this);
                    scope.Commit();
                }
                catch (Exception)
                {
                    scope.Rollback();
                    throw;
                }
                finally
                {
                    Close();
                }
            }
        }

        /// <summary>
        /// Transaction
        /// </summary>
        /// <param name="func">委托</param>
        /// <returns>True代表Commit成功 false代表没有Commit</returns>
        public bool UseTransaction(System.Func<DbContext<T>,bool> func)
        {
            using (var scope = ExecuteTransaction())
            {
                try
                {
                    if (func(this))
                    {
                        scope.Commit();
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                catch (Exception)
                {
                    scope.Rollback();
                    throw;
                }
                finally
                {
                    Close();
                }
            }
        }

        /// <summary>
        /// Transaction
        /// </summary>
        /// <param name="func"></param>
        /// <param name="isolationLevel"></param>
        /// <returns>True代表Commit成功 false代表没有Commit</returns>
        public bool UseTransaction(System.Func<DbContext<T>,bool> func, System.Data.IsolationLevel isolationLevel)
        {
            using (var scope = ExecuteTransaction(isolationLevel))
            {
                try
                {
                    if (func(this))
                    {
                        scope.Commit();
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                 
                }
                catch (Exception)
                {
                    scope.Rollback();
                    throw;
                }
                finally
                {
                    Close();
                }
            }
        }
        #endregion



        #region Sharding

        public void UseShardingDb(string shadingId, System.Action<DbContext<T>> func)
        {
            if(!this.HintWrapper.TryGetValue(DALExtStatementConstant.SHARDID,out _))
            {
                this.HintWrapper.Add(DALExtStatementConstant.SHARDID ,shadingId);
            }
            else
            {
                this.HintWrapper[DALExtStatementConstant.SHARDID] = shadingId;
            }
            
            func(this);
        }

        public void UseShardingTable(string tableId, System.Action<DbContext<T>> func)
        {
            if (!this.HintWrapper.TryGetValue(DALExtStatementConstant.TABLEID, out _))
            {
                this.HintWrapper.Add(DALExtStatementConstant.TABLEID, tableId);
            }
            else
            {
                this.HintWrapper[DALExtStatementConstant.TABLEID] = tableId;
            }

            func(this);
        }

        public void UseShardingDbAndTable(string shadingId,string tableId,System.Action<DbContext<T>> func)
        {
            if (!this.HintWrapper.TryGetValue(DALExtStatementConstant.SHARDID, out _))
            {
                this.HintWrapper.Add(DALExtStatementConstant.SHARDID, shadingId);
            }
            else
            {
                this.HintWrapper[DALExtStatementConstant.SHARDID] = shadingId;
            }

            if (!this.HintWrapper.TryGetValue(DALExtStatementConstant.TABLEID, out _))
            {
                this.HintWrapper.Add(DALExtStatementConstant.TABLEID, tableId);
            }
            else
            {
                this.HintWrapper[DALExtStatementConstant.TABLEID] = tableId;
            }

            func(this);
        }
        #endregion
    }
}