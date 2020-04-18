using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AntData.ORM.Enums;

namespace AntData.ORM.DbEngine.Configuration
{
    /// <summary>
    /// 自定义的db配置
    /// </summary>
    public class DatabaseSettings
    {
        /// <summary>
        /// 连接字符串配置列表
        /// </summary>
        public List<ConnectionStringItem> ConnectionItemList { get; set; }

       
        private string _provder = string.Empty;

        /// <summary>
        /// 同一个DataBaseSet下的配置的db对应的Provider只有一个
        /// </summary>
        public string Provider
        {
            get
            {
                switch (_provder.ToLower())
                {
                    case "mysql":
                    case "mysqlprovider":
                        return "AntData.ORM.Mysql.MySqlDatabaseProvider,AntData.ORM.Mysql";
                    case "sqlserver":
                    case "sql":
                    case "mssql":
                    case "mssqlrovider":
                    case "sqlprovider":
                        return "AntData.ORM.DbEngine.Providers.SqlDatabaseProvider,AntData.ORM";
                    case "oracle":
                    case "oracleprovider":
                        return "AntData.ORM.Oracle.OracleDatabaseProvider,AntData.ORM.Oracle";
                    case "postgre":
                    case "postgresql":
                    case "postgreprovider":
                    case "postgresqlprovider":
                        return "AntData.ORM.Postgre.PostgreDatabaseProvider,AntData.ORM.Postgre";
                }
                return _provder;
            }
            set
            {
                _provder = value;
            }
        }

        /// <summary>
        /// DataBaseSet名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 分片的类
        /// </summary>
        public string ShardingStrategy { get; set; }
    }

    /// <summary>
    /// 连接字符串
    /// </summary>
    public class ConnectionStringItem
    {
        public ConnectionStringItem()
        {
            DatabaseType = DatabaseType.Master;
        }
        /// <summary>
        /// db连接字符串
        /// </summary>
        public string ConnectionString { get; set; }

        /// <summary>
        /// 逻辑名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 对应的db类型(主or从)
        /// </summary>

        public DatabaseType DatabaseType { get; set; }

        /// <summary>
        /// 分片值
        /// </summary>
        public string Sharding { get; set; }
        public string Start { get; set; }
        public string End { get; set; }

        /// <summary>
        /// 分片权重
        /// </summary>
        public int Ratio { get; set; }
    }
}
