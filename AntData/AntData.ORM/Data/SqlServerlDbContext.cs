//-----------------------------------------------------------------------
// <copyright file="SqlServerDataConnection.cs" company="Company">
// Copyright (C) Company. All Rights Reserved.
// </copyright>
// <author>nainaigu</author>
// <summary></summary>
//-----------------------------------------------------------------------
using AntData.ORM.DataProvider;
using AntData.ORM.DataProvider.SqlServer;

namespace AntData.ORM.Data
{

    /// <summary>
    /// SqlServerlDbContext的数据驱动
    /// </summary>
    public class SqlServerlDbContext<T>:DbContext<T> where T : class
    {
        private static readonly IDataProvider _provider =  new SqlServerDataProvider(SqlServerVersion.v2008);

        public SqlServerlDbContext(string dbMappingName) : base(dbMappingName)
        {
        }

        protected override IDataProvider provider {
            get { return _provider; } 
        }
    }
}