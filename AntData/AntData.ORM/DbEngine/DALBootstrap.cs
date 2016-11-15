using AntData.ORM.DbEngine.Configuration;
using AntData.ORM.DbEngine.DB;
using AntData.ORM.DbEngine.Providers;
using AntData.ORM.DbEngine.Sharding;
using AntData.ORM.Properties;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using AntData.ORM.Dao;
using AntData.ORM.Enums;

namespace AntData.ORM.DbEngine
{
    public class DALBootstrap
    {
        static DALBootstrap()
        {
            LoadConfig();
            LoadDatabaseProviders();
            LoadAllInOneKeys();
            LoadDatabaseSets();
        }

        private static DbEngineConfigurationSection ConfigurationSection { get; set; }

        private static Dictionary<String, IDatabaseProvider> DatabaseProviders { get; set; }

        public static Dictionary<String, DatabaseSetWrapper> DatabaseSets { get; set; }

        public static NameValueCollection ConnectionStringKeys { get; set; }

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
        }

        private static void LoadDatabaseProviders()
        {
            var databaseProviders = ConfigurationSection.DatabaseProviders;
            if (databaseProviders == null)
                throw new DalException("Missing DatabaseProviders.");
            DatabaseProviders = new Dictionary<String, IDatabaseProvider>();

            foreach (DatabaseProviderElement provider in databaseProviders)
            {
                if (provider.Type == null)
                    throw new ConfigurationErrorsException(String.Format(Resources.InvalidDatabaseProviderException, provider.TypeName));
                var databaseProvider = Activator.CreateInstance(provider.Type) as IDatabaseProvider;
                String[] names = provider.Name.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var name in names)
                {
                    DatabaseProviders.Add(name, databaseProvider);
                }
            }
        }

        private static void LoadAllInOneKeys()
        {
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
        }

        private static void LoadDatabaseSets()
        {
            var databaseSets = ConfigurationSection.DatabaseSets;
            if (databaseSets == null)
                throw new DalException("Missing DatabaseSets.");
            DatabaseSets = new Dictionary<String, DatabaseSetWrapper>();

            foreach (DatabaseSetElement databaseSet in databaseSets)
            {
                if (!DatabaseProviders.ContainsKey(databaseSet.Provider))
                    throw new DalException("DatabaseProvider doesn't match.");
                IDatabaseProvider provider = DatabaseProviders[databaseSet.Provider];

                //build set wrapper
                var databaseSetWrapper = new DatabaseSetWrapper
                {
                    Name = databaseSet.Name,
                    EnableReadWriteSpliding = false,
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

       
    }
}
