using System;
using AntData.ORM.SqlProvider;
using AntData.ORM.SqlQuery;

namespace AntData.ORM.DataProvider.SqlServer
{


	class SqlServer2000SqlOptimizer : SqlServerSqlOptimizer
	{
		public SqlServer2000SqlOptimizer(SqlProviderFlags sqlProviderFlags) : base(sqlProviderFlags)
		{
		}

		public override ISqlExpression ConvertExpression(ISqlExpression expr)
		{
			expr = base.ConvertExpression(expr);

			if (expr is SqlFunction)
				return ConvertConvertFunction((SqlFunction)expr);

			return expr;
		}
	}
}
