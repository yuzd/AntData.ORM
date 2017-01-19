using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AntData.ORM.DbEngine.Providers;
using Oracle.ManagedDataAccess.Client;

namespace AntData.ORM.Oracle
{
    public class OracleDatabaseProvider : IDatabaseProvider
    {
        public string ProviderType { get { return "Oracle"; } }

        /// <summary>
        /// 创建数据库链接
        /// </summary>
        /// <returns>数据库链接</returns>
        public DbConnection CreateConnection()
        {
            return new OracleConnection();
        }

        /// <summary>
        /// 创建数据库指令
        /// </summary>
        /// <returns>数据库指令</returns>
        public DbCommand CreateCommand()
        {
            return new OracleCommand();
        }

        /// <summary>
        /// 创建数据库适配器
        /// </summary>
        /// <returns></returns>
        public DbDataAdapter CreateDataAdapter()
        {
            return new OracleDataAdapter();
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
        /// 是否支持参数导出
        /// </summary>
        public bool DeriveParametersSupported {
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
            if (command.GetType() != typeof(OracleCommand))
                throw new ArgumentException("command must be OracleCommand!");
            OracleCommandBuilder.DeriveParameters((OracleCommand)command);
            foreach (DbParameter para in command.Parameters)
            {
                if (para.Direction == ParameterDirection.InputOutput || para.Direction == ParameterDirection.Output)
                {
                    para.Value = DBNull.Value;
                }
            }
        }
    }
}
