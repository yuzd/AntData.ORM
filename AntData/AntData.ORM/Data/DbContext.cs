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

namespace AntData.ORM.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Threading.Tasks;


    /// <summary>
    /// 
    /// </summary>
    public abstract class DbContext<T> : AntData.ORM.Data.DataConnection, IDataContext where T : class
    {
        protected abstract IDataProvider provider { get; }
        //private static readonly IDataProvider provider = new SqlServerDataProvider(System.String.Empty, LinqToDB.DataProvider.SqlServer.SqlServerVersion.v2008);
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
#if DEBUG
            //AntData.ORM.Common.Configuration.Linq.GenerateExpressionTest = true;
            AntData.ORM.Common.Configuration.Linq.AllowMultipleQuery = true;
#endif
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
                catch (Exception ex)
                {

                    throw;
                }
                return null;
            });
        }


#if !NETSTANDARD

        #region New Transaction

        //Required=》该范围需要一个事务。 如果已经存在事务，则使用该事务。否则，在进入范围之前创建新的事务。 这是默认值。
        //RequiresNew=》总是为该范围创建新事务
        //Suppress =》事务上下文在创建范围时被取消。 范围中的所有操作都在无事务上下文的情况下完成。

        //Serializable 可以在事务期间读取可变数据，但是不可以修改，也不可以添加任何新数据。 
        //RepeatableRead 可以在事务期间读取可变数据，但是不可以修改。 可以在事务期间添加新数据。  
        //ReadCommitted 不可以在事务期间读取可变数据，但是可以修改它。 
        //ReadUncommitted 可以在事务期间读取和修改可变数据。 
        //Snapshot 可以读取可变数据。在事务修改数据之前，它验证在它最初读取数据之后另一个事务是否更改过这些数据。如果数据已被更新，则会引发错误。这样使事务可获取先前提交的数据值。在尝试提升以此隔离级别创建的事务时，将引发一个InvalidOperationException，并产生错误信息“Transactions with IsolationLevel Snapshot cannot be promoted”（无法提升具有 IsolationLevel 快照的事务）。
        //Chaos 无法覆盖隔离级别更高的事务中的挂起的更改。 
        //Unspecified 正在使用与指定隔离级别不同的隔离级别，但是无法确定该级别。如果设置了此值，则会引发异常。   

        //4.5.1 支持异步的TransactionScopeAsync
        //https://github.com/danielmarbach/async-dolls/blob/master/async-dolls/2-AsyncTransactions/Script.cs
        /// <summary>
        /// 使用事物 在事物里面的代码都是走master
        /// </summary>
        /// <param name="func"></param>
        public void UseTransaction(System.Action<DbContext<T>> func)
        {
            using (var scope = new System.Transactions.TransactionScope())
            {
                func(this);
                scope.Complete();
            }
        }
        /// <summary>
        /// 使用事物 在事物里面的代码都是走master
        /// </summary>
        /// <param name="func"></param>
        /// <param name="scopeOption">TransactionScopeOption</param>
        public void UseTransaction(System.Action<DbContext<T>> func, TransactionScopeOption scopeOption)
        {
            using (var scope = new System.Transactions.TransactionScope(scopeOption))
            {
                func(this);
                scope.Complete();
            }
        }

        /// <summary>
        /// 使用事物 在事物里面的代码都是走master
        /// </summary>
        /// <param name="func"></param>
        /// <param name="scopeOption">TransactionScopeOption</param>
        /// <param name="options">TransactionOptions</param>
        public void UseTransaction(System.Action<DbContext<T>> func, TransactionScopeOption scopeOption, TransactionOptions options)
        {
            using (var scope = new System.Transactions.TransactionScope(scopeOption, options))
            {
                func(this);
                scope.Complete();
            }
        }

        /// <summary>
        /// 使用事物 在事物里面的代码都是走master
        /// </summary>
        /// <param name="func"></param>
        public void UseTransaction(System.Func<DbContext<T>, bool> func)
        {
            using (var scope = new System.Transactions.TransactionScope())
            {
                if (func(this))
                {
                    scope.Complete();
                }
            }
        }

        /// <summary>
        /// 使用事物 在事物里面的代码都是走master
        /// </summary>
        /// <param name="func"></param>
        /// <param name="scopeOption">TransactionScopeOption</param>
        public void UseTransaction(System.Func<DbContext<T>, bool> func, TransactionScopeOption scopeOption)
        {
            using (var scope = new System.Transactions.TransactionScope(scopeOption))
            {
                if (func(this))
                {
                    scope.Complete();
                }
            }
        }

        /// <summary>
        /// 使用事物 在事物里面的代码都是走master
        /// </summary>
        /// <param name="func"></param>
        /// <param name="scopeOption">TransactionScopeOption</param>
        /// <param name="options">TransactionOptions</param>
        public void UseTransaction(System.Func<DbContext<T>, bool> func, TransactionScopeOption scopeOption, TransactionOptions options)
        {
            using (var scope = new System.Transactions.TransactionScope(scopeOption, options))
            {
                if (func(this))
                {
                    scope.Complete();
                }
            }
        }
        #endregion
#else
         public void UseTransaction(System.Action<DbContext<T>> func)
        {
            using (var scope = base.ExecuteTransaction())
            {
                func(this);
                scope.Commit();
            }
        }
        public void UseTransaction(System.Action<DbContext<T>> func, System.Data.IsolationLevel isolationLevel)
        {
            using (var scope = base.ExecuteTransaction(isolationLevel))
            {
                func(this);
                scope.Commit();
            }
        }

        public void UseTransaction(System.Func<DbContext<T>, bool> func)
        {
            using (var scope = base.ExecuteTransaction())
            {
                if (func(this))
                {
                    scope.Commit();
                }
            }
        }

        public void UseTransaction(System.Func<DbContext<T>, bool> func, System.Data.IsolationLevel isolationLevel)
        {
            using (var scope = base.ExecuteTransaction(isolationLevel))
            {
                if (func(this))
                {
                    scope.Commit();
                }
            }
        }
#endif


    }
}