using System;
using System.Collections.Generic;
using AntData.ORM.Dao;
using AntData.ORM.DbEngine.Configuration;
using AntData.ORM.DbEngine.Sharding;

namespace AntData.DbEngine.Sharding
{
    class ShardingStrategyFactory
    {
        public static readonly ShardingStrategyFactory Instance = new ShardingStrategyFactory();

        private ShardingStrategyFactory() { }

        /// <summary>
        /// get shard strategy object via shard config
        /// </summary>
        /// <param name="element"></param>
        /// <param name="settings">兼容非config文件</param>
        /// <returns></returns>
        public IShardingStrategy GetShardingStrategy(DatabaseSetElement element,DatabaseSettings settings = null)
        {
            String shardingStrategy = settings == null ? element.ShardingStrategy : settings.ShardingStrategy;
            if (String.IsNullOrEmpty(shardingStrategy))
                return null;

            var shardconfs = shardingStrategy.Split(';');
            var config = new Dictionary<String, String>();

            foreach (var shardcnfg in shardconfs)
            {
                var param = shardcnfg.Split('=');
                if (param.Length != 2)
                    throw new ArgumentException("Sharding parameters invalid.");
                //will fix key issue(ignore case in the future)
                config.Add(param[0].Trim(), param[1].Trim());
            }

            if (!config.TryGetValue("class", out var classname))
                throw new ArgumentException("Strategy invalid.");

            Type type = Type.GetType(classname);
            if (type == null)
                throw new ArgumentException("Strategy invalid.");

            try
            {
                var resultStrategy = Activator.CreateInstance(type) as IShardingStrategy;
                if (resultStrategy == null)
                {
                    throw new DalException("Strategy {0} didn't implement IShardingStrategy", classname);
                }
                else
                {
                    var allShardingConfig = new List<ShardingConfig>();
                    if (element != null)
                    {
                        foreach (DatabaseElement e in element.Databases)
                        {
                            allShardingConfig.Add(new ShardingConfig
                            {
                                Sharding = e.Sharding,
                                Start = e.Start,
                                End = e.End
                            });
                        }
                    }
                    if (settings != null)
                    {
                        foreach (var e in settings.ConnectionItemList)
                        {
                            allShardingConfig.Add(new ShardingConfig
                            {
                                Sharding = e.Sharding,
                                Start = e.Start,
                                End = e.End
                            });
                        }
                    }
                    resultStrategy.SetShardConfig(config, allShardingConfig);
                    return resultStrategy;
                }

            }
            catch (Exception ex)
            {
                throw new DalException("Strategy invalid.", ex);
            }
        }

    }
}
