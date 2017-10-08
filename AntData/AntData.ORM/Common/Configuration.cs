using System;
using System.Collections.Generic;
using System.Linq;
using AntData.ORM.DbEngine.Configuration;
using Microsoft.Extensions.Configuration;

namespace AntData.ORM.Common
{
    public static class Configuration
    {
        public static bool IsStructIsScalarType = true;
        public static bool AvoidSpecificDataProviderAPI;

        /// <summary>
        /// DB配置 如果配置了这个就不从Config文件里面读取了
        /// 注意 只能在程序初始化的时候配置 如果后期重新修改了得重新启动程序才行 不支持reload操作
        /// </summary>
        public static DBSettings DBSettings { get; set; }

       
        /// <summary>
        /// netcore 新的配置读取方式
        /// </summary>
        /// <param name="config"></param>
        public static void UseDBConfig(IConfigurationRoot config)
        {
            if (config == null)
            {
                throw new ArgumentException("config can not be null");
            }
            var dal = config.GetSection("dal");
            if (dal == null)
            {
                throw new ArgumentException("dal section can not be found in config ");
            }

            var dbSettings =  new List<DatabaseSettings>();

            var children = dal.GetChildren();

            foreach (IConfigurationSection child in children)
            {
                var bind = new DatabaseSettings();
                ConfigurationBinder.Bind(child, bind);
                dbSettings.Add(bind);

                if (bind.ConnectionItemList == null || bind.ConnectionItemList.Count < 1)
                {
                    throw new ArgumentException("ConnectionItemList section can not be found in config ");
                }
            }

            DBSettings = new DBSettings{DatabaseSettings = dbSettings};
        }

        public static class Linq
        {
            public static bool PreloadGroups;
            public static bool IgnoreEmptyUpdate;
            public static bool AllowMultipleQuery;
            public static bool GenerateExpressionTest;
            public static bool OptimizeJoins = true;
            /// <summary>
			/// If set to true unllable fields would be checked for IS NULL when comparasion type is NotEqual 
			/// <example>
			/// public class MyEntity
			/// {
			///     public int? Value;
			/// }
			/// 
			/// db.MyEntity.Where(e => e.Value != 10)
			/// 
			/// Would be converted to
			/// 
			/// SELECT Value FROM MyEntity WHERE Value IS NULL OR Value != 10
			/// </example>
			/// </summary>
			public static bool CheckNullForNotEquals = false;
            /// <summary>
            /// 设置全局 插入是否忽略null字段
            /// </summary>
            public static bool IgnoreNullInsert;
            /// <summary>
            /// 设置全局 修改是否忽略null字段
            /// </summary>
            public static bool IgnoreNullUpdate;

            /// <summary>
            /// 设置全局 mapping 序列化的时候 是否忽略字段大小写 默认忽略
            /// </summary>
            public static StringComparison? ColumnComparisonOption = StringComparison.OrdinalIgnoreCase;

            /// <summary>
            /// 如果设置为true 查询条数的为count(1) 否则为count(*) 默认是 count(1)
            /// </summary>
		    public static bool UseAsteriskForCountSql = false;

            /// <summary>
            /// 针对sqlserver使用的开关 是否在table名称后面增加[NOLOCK]
            /// </summary>
		    public static bool UseNoLock = false;


        }

        public static class LinqService
        {
            public static bool SerializeAssemblyQualifiedName;
            public static bool ThrowUnresolvedTypeException;
        }
    }
}
