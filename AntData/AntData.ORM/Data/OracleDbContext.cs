//-----------------------------------------------------------------------
// <copyright file="SqlServerDataConnection.cs" company="Company">
// Copyright (C) Company. All Rights Reserved.
// </copyright>
// <author>nainaigu</author>
// <summary></summary>
//-----------------------------------------------------------------------
using AntData.ORM.DataProvider;
using AntData.ORM.DataProvider.Oracle;

namespace AntData.ORM.Data
{
    /// <summary>
    /// SqlServerlDbContext的数据驱动
    /// </summary>
    public class OracleDbContext<T>:DbContext<T> where T : class
    {
        private static readonly IDataProvider _provider = new OracleDataProvider();

        public OracleDbContext(string dbMappingName) : base(dbMappingName)
        {
        }

        protected override IDataProvider provider {
            get { return _provider; } 
        }
    }
}