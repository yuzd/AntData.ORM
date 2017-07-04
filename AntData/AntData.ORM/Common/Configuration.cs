using System;
using AntData.ORM.DbEngine.Configuration;

namespace AntData.ORM.Common
{
	public static class Configuration
	{
		public static bool IsStructIsScalarType = true;
		public static bool AvoidSpecificDataProviderAPI;

	    /// <summary>
	    /// DB配置 如果配置了这个就不从Config文件里面读取了
	    /// </summary>
	    public static DBSettings DBSettings { get; set; }   

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
