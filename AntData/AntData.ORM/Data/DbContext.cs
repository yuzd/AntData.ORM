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
                return null;
            });
        }




    }
}