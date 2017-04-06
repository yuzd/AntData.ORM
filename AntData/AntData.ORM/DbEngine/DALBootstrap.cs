using AntData.ORM.DbEngine.Configuration;
using AntData.ORM.DbEngine.DB;
using AntData.ORM.DbEngine.Providers;
using AntData.ORM.Properties;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using AntData.DbEngine.Sharding;
using AntData.ORM.Dao;
using AntData.ORM.Data;
using AntData.ORM.Enums;

namespace AntData.ORM.DbEngine
{
    public class DALBootstrap
    {
        static DALBootstrap()
        {
            LoadConfig();//读取配置文件
            LoadDatabaseProviders(); //配置文件中解析providers
            LoadAllInOneKeys(); //配置文件中解析 DatabaseSet 里面的节点 DatabaseElement 的name 和 connectionstring
            LoadDatabaseSets();
        }

        /// <summary>
        /// 配置对象
        /// </summary>
        private static DbEngineConfigurationSection ConfigurationSection { get; set; }

        /// <summary>
        /// 配置的所有的Provider
        /// </summary>
        private static Dictionary<String, IDatabaseProvider> DatabaseProviders { get; set; }

        public static Dictionary<String, DatabaseSetWrapper> DatabaseSets { get; set; }

        public static NameValueCollection ConnectionStringKeys { get; set; }

        /// <summary>
        /// 获取配置
        /// </summary>
        private static void LoadConfig()
        {


            try
            {

                ConfigurationSection = DbEngineConfigurationSection.GetConfig();
                if (ConfigurationSection == null)
                    throw new DalException(Resources.DalConfigNotFoundException);
            }
            catch (NullReferenceException ex)
            {
                throw new DalException(Resources.DalConfigNotFoundException, ex);
            }
            catch (Exception ex)
            {
                throw new DalException(ex.Message);
            }

        }

        /// <summary>
        /// 从配置中获取Providers
        /// </summary>
        private static void LoadDatabaseProviders()
        {
//#if !NETSTANDARD
            var databaseProviders = ConfigurationSection.DatabaseProviders;
            if (databaseProviders == null)
                throw new DalException("Missing DatabaseProviders.");
            DatabaseProviders = new Dictionary<String, IDatabaseProvider>();

            foreach (DatabaseProviderElement provider in databaseProviders)
            {
                if (provider.Type == null)
                    throw new ConfigurationErrorsException(String.Format(Resources.InvalidDatabaseProviderException, provider.TypeName));
                //创建provider实例
                var databaseProvider = Activator.CreateInstance(provider.Type) as IDatabaseProvider;
                String[] names = provider.Name.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var name in names)
                {
                    DatabaseProviders.Add(name, databaseProvider);
                }
            }
//#else
//            if (DataConnection.DefaultSettings == null || DataConnection.DefaultSettings.DataProviders == null || !DataConnection.DefaultSettings.DataProviders.Any())
//            {
//                throw new DalException("Missing DataConnection.DefaultSettings.");
//            }
//            var databaseProviders = DataConnection.DefaultSettings.DataProviders;
//            DatabaseProviders = new Dictionary<String, IDatabaseProvider>();
//            foreach (var provider in databaseProviders)
//            {
//                if (provider.Type == null)
//                    throw new DalException(String.Format(Resources.InvalidDatabaseProviderException, provider.TypeName));
//                //创建provider实例
//                var databaseProvider = Activator.CreateInstance(provider.Type) as IDatabaseProvider;
//                DatabaseProviders.Add(provider.Name, databaseProvider);
//            }
//#endif
        }

        /// <summary>
        /// 解析DataBaseSet
        /// </summary>
        private static void LoadAllInOneKeys()
        {
//#if !NETSTANDARD
            var databaseSets = ConfigurationSection.DatabaseSets;
            if (databaseSets == null)
                throw new DalException("Missing DatabaseSets.");
            ConnectionStringKeys = new NameValueCollection();

            foreach (DatabaseSetElement databaseSet in databaseSets)
            {
                foreach (DatabaseElement database in databaseSet.Databases)
                {
                    if (database != null)
                        ConnectionStringKeys.Add(database.ConnectionString, database.Name);
                }
            }
//#else
//            if (DataConnection.DefaultSettings == null || DataConnection.DefaultSettings.ConnectionStrings == null || !DataConnection.DefaultSettings.ConnectionStrings.Any())
//            {
//                throw new DalException("Missing DataConnection.ConnectionStrings.");
//            }
//            ConnectionStringKeys = new NameValueCollection();
//            foreach (var database in DataConnection.DefaultSettings.ConnectionStrings)
//            {
//                foreach (var item in database.ConnectionItemList)
//                {
//                    if (string.IsNullOrEmpty(item.ConnectionString) || string.IsNullOrEmpty(item.Name))
//                    {
//                        throw new DalException("Missing DataConnection.ConnectionStrings.ConnectionString or Name .");
//                    }
//                    ConnectionStringKeys.Add(item.ConnectionString, item.Name);
//                }
               
//            }
//#endif
        }

        private static void LoadDatabaseSets()
        {
//#if !NETSTANDARD
            var databaseSets = ConfigurationSection.DatabaseSets;
            if (databaseSets == null)
                throw new DalException("Missing DatabaseSets.");
            //一个DataBaseSet可以配置多个 例如主从 或者 分片
            DatabaseSets = new Dictionary<String, DatabaseSetWrapper>();

            foreach (DatabaseSetElement databaseSet in databaseSets)
            {
                if (!DatabaseProviders.ContainsKey(databaseSet.Provider))
                    throw new DalException("DatabaseProvider doesn't match.");
                IDatabaseProvider provider = DatabaseProviders[databaseSet.Provider];

                //build set wrapper 同一个DataBaseSet的Provider必须一致
                var databaseSetWrapper = new DatabaseSetWrapper
                {
                    Name = databaseSet.Name,
                    EnableReadWriteSpliding = false,//默认关闭读写分离
                    ProviderType = DatabaseProviderTypeFactory.GetProviderType(provider.ProviderType),
                    ShardingStrategy = ShardingStrategyFactory.Instance.GetShardingStrategy(databaseSet)
                };

                foreach (DatabaseElement database in databaseSet.Databases)
                {
                    String shard = database.Sharding ?? String.Empty;
                    Int32 ratio = 0;
                    Int32 ratioStart = 0;
                    Int32 ratioEnd = 0;
                    if (shard.Length > 0)
                    {
                        if (database.DatabaseType == DatabaseType.Slave)
                        {
                            ratioStart = ratio;
                            ratio += database.Ratio;
                            ratioEnd = ratio;
                        }

                        databaseSetWrapper.AllShards.Add(shard);
                        if (!databaseSetWrapper.TotalRatios.ContainsKey(shard))
                            databaseSetWrapper.TotalRatios.Add(shard, ratio);
                    }

                    databaseSetWrapper.DatabaseWrappers.Add(new DatabaseWrapper
                    {
                        Name = database.Name,
                        ConnectionString = database.ConnectionString,
                        DatabaseType = database.DatabaseType,
                        DatabaseProvider = provider,
                        Database = new Database(databaseSet.Name, database.Name, database.ConnectionString, provider) { DatabaseRWType = database.DatabaseType },
                        Sharding = shard,
                        RatioStart = ratioStart,
                        RatioEnd = ratioEnd
                    });

                    if (database.DatabaseType == DatabaseType.Slave && !databaseSetWrapper.EnableReadWriteSpliding)
                        databaseSetWrapper.EnableReadWriteSpliding = true;
                }

                DatabaseSets.Add(databaseSet.Name, databaseSetWrapper);
            }
//#else
//            if (DataConnection.DefaultSettings == null || DataConnection.DefaultSettings.ConnectionStrings == null || !DataConnection.DefaultSettings.ConnectionStrings.Any())
//            {
//                throw new DalException("Missing DataConnection.ConnectionStrings.");
//            }
//            //一个DataBaseSet可以配置多个 例如主从 或者 分片
//            DatabaseSets = new Dictionary<String, DatabaseSetWrapper>();
//            foreach (var databaseSet in DataConnection.DefaultSettings.ConnectionStrings)
//            {
//                if (!DatabaseProviders.ContainsKey(databaseSet.ProviderName))
//                    throw new DalException("DatabaseProvider doesn't match.");
//                IDatabaseProvider provider = DatabaseProviders[databaseSet.ProviderName];

//                //build set wrapper 同一个DataBaseSet的Provider必须一致
//                var databaseSetWrapper = new DatabaseSetWrapper
//                {
//                    Name = databaseSet.Name,
//                    EnableReadWriteSpliding = false,//默认关闭读写分离
//                    ProviderType = DatabaseProviderTypeFactory.GetProviderType(provider.ProviderType),
//                };

//                foreach (var database in databaseSet.ConnectionItemList)
//                {
//                    databaseSetWrapper.DatabaseWrappers.Add(new DatabaseWrapper
//                    {
//                        Name = database.Name,
//                        ConnectionString = database.ConnectionString,
//                        DatabaseType = database.DatabaseType,
//                        DatabaseProvider = provider,
//                        Database = new Database(databaseSet.Name, database.Name, database.ConnectionString, provider) { DatabaseRWType = database.DatabaseType },
//                    });

//                    if (database.DatabaseType == DatabaseType.Slave && !databaseSetWrapper.EnableReadWriteSpliding)
//                        databaseSetWrapper.EnableReadWriteSpliding = true;
//                }

//                DatabaseSets.Add(databaseSet.Name, databaseSetWrapper);
//            }
//#endif
        }

        public static IShardingStrategy GetShardingStrategy(String logicDbName)
        {
            if (!DatabaseSets.ContainsKey(logicDbName))
                throw new ArgumentOutOfRangeException(String.Format(Resources.DatabaseSetDoesNotExistException, logicDbName));
            return DatabaseSets[logicDbName].ShardingStrategy;
        }

        public static DatabaseProviderType GetProviderType(String logicDbName)
        {
            if (!DatabaseSets.ContainsKey(logicDbName))
                throw new ArgumentOutOfRangeException(String.Format(Resources.DatabaseSetDoesNotExistException, logicDbName));
            return DatabaseSets[logicDbName].ProviderType;
        }

        /// <summary>
        /// 获取读取真实数据库字符串的文本地址
        /// </summary>
        /// <returns></returns>
        public static String GetConnectionLocatorPath()
        {
            if (ConfigurationSection == null || ConfigurationSection.ConnectionLocator == null)
                return null;
            return ConfigurationSection.ConnectionLocator.Path;
        }
    }
}
