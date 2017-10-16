//-----------------------------------------------------------------------
// <copyright file="MySqlDatabaseProvider.cs" company="Company">
// Copyright (C) Company. All Rights Reserved.
// </copyright>
// <author>nainaigu</author>
// <summary></summary>
//-----------------------------------------------------------------------
using System;
using System.Data;
using System.Data.Common;
using AntData.ORM.DbEngine.Providers;
using Npgsql;

namespace AntData.ORM.Postgre
{
    
    /// <summary>
    /// Postgre Server 数据库提供者实现
    /// </summary>
    public class PostgreDatabaseProvider : IDatabaseProvider
    {
        #region Implementation of IDatabaseProvider

        /// <summary>
        /// 创建数据库链接
        /// </summary>
        /// <returns>数据库链接</returns>
        public DbConnection CreateConnection()
        {

            return new NpgsqlConnection();
        }

        /// <summary>
        /// 创建数据库指令
        /// </summary>
        /// <returns>数据库指令</returns>
        public DbCommand CreateCommand()
        {
            return new NpgsqlCommand();;
        }

        /// <summary>
        /// 将名称变为数据库相关的参数名称
        /// </summary>
        /// <param name="name">参数名称</param>
        /// <returns>数据库相关的参数名称</returns>
        public string CreateParameterName(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");

            if (name[0] != ':')
            {
                return ":" + name;
            }
            return name;
        }

        /// <summary>
        /// 创建数据库适配器
        /// </summary>
        /// <returns></returns>
        public DbDataAdapter CreateDataAdapter()
        {
            return new NpgsqlDataAdapter();
            
        }

#if !NETSTANDARD
        /// <summary>
        /// 是否支持参数导出
        /// </summary>
        public bool DeriveParametersSupported
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// 导出数据库指令参数
        /// </summary>
        /// <param name="command">数据库指令</param>
        public void DeriveParameters(DbCommand command)
        {
            if (command == null)
                throw new ArgumentNullException("command");
            if (command.GetType() != typeof(NpgsqlCommand))
                throw new ArgumentException("command must be NpgsqlCommand!");


            NpgsqlCommandBuilder.DeriveParameters((NpgsqlCommand)command);

            foreach (DbParameter para in command.Parameters)
            {
                if (para.Direction == ParameterDirection.InputOutput || para.Direction == ParameterDirection.Output)
                {
                    para.Value = DBNull.Value;
                }
            }
        }
#endif
        public string ProviderType
        {
            get { return "Postgre"; }
        }
#endregion
    }
}