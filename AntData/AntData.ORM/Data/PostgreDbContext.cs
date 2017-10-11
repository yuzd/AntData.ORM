//-----------------------------------------------------------------------
// <copyright file="SqlServerDataConnection.cs" company="Company">
// Copyright (C) Company. All Rights Reserved.
// </copyright>
// <author>nainaigu</author>
// <summary></summary>
//-----------------------------------------------------------------------
using AntData.ORM.DataProvider;
using AntData.ORM.DataProvider.MySql;
using AntData.ORM.DataProvider.PostgreSQL;

namespace AntData.ORM.Data
{

    /// <summary>
    /// Postgre的数据驱动
    /// </summary>
    public class PostgreDbContext<T>:DbContext<T> where T : class
    {
        private static readonly IDataProvider _provider = new PostgreSQLDataProvider();

        public PostgreDbContext(string dbMappingName) : base(dbMappingName)
        {
        }

        protected override IDataProvider provider {
            get { return _provider; } 
        }
    }
}