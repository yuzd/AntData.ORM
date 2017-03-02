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

    /// <summary>
    /// Mysql的数据驱动
    /// </summary>
    public class MysqlDbContext<T>:DbContext<T> where T : class
    {
        private static readonly IDataProvider _provider = new MySqlDataProvider();

        public MysqlDbContext(string dbMappingName) : base(dbMappingName)
        {
        }

        protected override IDataProvider provider {
            get { return _provider; } 
        }
    }
}