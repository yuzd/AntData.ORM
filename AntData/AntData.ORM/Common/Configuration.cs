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
