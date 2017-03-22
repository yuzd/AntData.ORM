using System;

namespace AntData.ORM.Common
{
	public static class Configuration
	{
		public static bool IsStructIsScalarType = true;
		public static bool AvoidSpecificDataProviderAPI;
		

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
        }

		public static class LinqService
		{
			public static bool SerializeAssemblyQualifiedName;
			public static bool ThrowUnresolvedTypeException;
		}
	}
}
