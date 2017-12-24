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
using System.Collections;
using System.Data;
using AntData.ORM.Common;
using AntData.ORM.DataProvider;
using System;
namespace AntData.ORM.Data
{

    public abstract class DbContext : AntData.ORM.Data.DataConnection, IDataContext
    {
        protected abstract IDataProvider provider { get; }
        public DbContext(string dbMappingName) : base(dbMappingName)
        {
            
        }


        #region Transaction

        public void UseTransaction(System.Action<DbContext> func)
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

        public void UseTransaction(System.Action<DbContext> func, System.Data.IsolationLevel isolationLevel)
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

        public void UseTransaction(System.Func<DbContext, bool> func)
        {
            using (var scope = ExecuteTransaction())
            {
                try
                {
                    if (func(this))
                    {
                        scope.Commit();
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

        public void UseTransaction(System.Func<DbContext, bool> func, System.Data.IsolationLevel isolationLevel)
        {
            using (var scope = ExecuteTransaction(isolationLevel))
            {
                try
                {
                    if (func(this))
                    {
                        scope.Commit();
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

        public void UseTransaction(System.Func<DbContext<T>,bool> func)
        {
            using (var scope = ExecuteTransaction())
            {
                try
                {
                    if (func(this))
                    {
                        scope.Commit();
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

        public void UseTransaction(System.Func<DbContext<T>,bool> func, System.Data.IsolationLevel isolationLevel)
        {
            using (var scope = ExecuteTransaction(isolationLevel))
            {
                try
                {
                    if (func(this))
                    {
                        scope.Commit();
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

    }
}