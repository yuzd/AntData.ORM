using System;
using System.Configuration;

namespace AntData.ORM.DbEngine.Configuration
{
    /// <summary>
    /// 数据库引擎配置节
    /// </summary>
    public sealed class DbEngineConfigurationSection : ConfigurationSection
    {
        #region private fields

        private readonly ConfigurationProperty databaseSets;

        private readonly ConfigurationProperty databaseProviders;

        private readonly ConfigurationProperty connectionLocator;

        #endregion

        #region construction

        /// <summary>
        /// 构造方法
        /// </summary>
        public DbEngineConfigurationSection()
        {
            databaseSets = new ConfigurationProperty("databaseSets", typeof(DatabaseSetElementCollection), null, ConfigurationPropertyOptions.None);
            databaseProviders = new ConfigurationProperty("databaseProviders", typeof(DatabaseProviderElementCollection), null, ConfigurationPropertyOptions.None);

            connectionLocator = new ConfigurationProperty("connectionLocator", typeof(ConnectionLocatorElement), null, ConfigurationPropertyOptions.None);
        }

        #endregion

        #region get configuration

        /// <summary>
        /// Db引擎配置节名称
        /// </summary>
        private const String SectionName = "dal";

        /// <summary>
        /// 获取Db引擎配置节配置节
        /// </summary>
        /// <returns></returns>
        public static DbEngineConfigurationSection GetConfig()
        {
            return ConfigurationManager.GetSection(SectionName) as DbEngineConfigurationSection;
        }

        /// <summary>
        /// 名称,关键字
        /// </summary>
        [ConfigurationProperty("name", DefaultValue = "DALFx")]
        public String Name
        {
            get { return (String)base["name"]; }
#if NETSTANDARD
            set { Name = value; }
#endif
        }

        #endregion

        #region public properties

        /// <summary>
        /// 数据库集配置数组
        /// </summary>
        [ConfigurationProperty("databaseSets")]
        public DatabaseSetElementCollection DatabaseSets
        {
            get { return (DatabaseSetElementCollection)base[databaseSets]; }
#if NETSTANDARD
            set { DatabaseSets = value; }
#endif
        }

        /// <summary>
        /// 数据库提供者配置数组
        /// </summary>
        [ConfigurationProperty("databaseProviders")]
        public DatabaseProviderElementCollection DatabaseProviders
        {
            get { return (DatabaseProviderElementCollection)base[databaseProviders]; }
#if NETSTANDARD
            set { DatabaseProviders = value; }
#endif
        }

        /// <summary>
        /// Connection String
        /// </summary>
        [ConfigurationProperty("connectionLocator")]
        public ConnectionLocatorElement ConnectionLocator
        {
            get { return (ConnectionLocatorElement)base[connectionLocator]; }
#if NETSTANDARD
            set { ConnectionLocator = value; }
#endif
        }

        #endregion
    }
}
