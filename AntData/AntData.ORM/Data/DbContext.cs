//-----------------------------------------------------------------------
// <copyright file="SqlServerDataConnection.cs" company="Company">
// Copyright (C) Company. All Rights Reserved.
// </copyright>
// <author>nainaigu</author>
// <summary></summary>
//-----------------------------------------------------------------------

using AntData.ORM.DataProvider;
using AntData.ORM.DataProvider.MySql;

namespace AntData.ORM.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;


    /// <summary>
    /// 
    /// </summary>
    public class DbContext<T> : AntData.ORM.Data.DataConnection, IDataContext where T : class
    {
        //private static readonly IDataProvider provider = new MySqlDataProvider();
        //private static readonly IDataProvider provider = new SqlServerDataProvider(System.String.Empty, LinqToDB.DataProvider.SqlServer.SqlServerVersion.v2008);
        private readonly Lazy<T> _lazy = null;
        public T Tables
        {
            get
            {
                return _lazy.Value;
            }
        }

        public DbContext(string dbMappingName,IDataProvider provider)
            : base(provider, dbMappingName)
        {
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


        #region New Transaction

        public void UseTransaction(System.Action<DbContext<T>> func)
        {
            using (var scope = new System.Transactions.TransactionScope())
            {
                func(this);
                scope.Complete();
            }
        }

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
        #endregion
    }
}