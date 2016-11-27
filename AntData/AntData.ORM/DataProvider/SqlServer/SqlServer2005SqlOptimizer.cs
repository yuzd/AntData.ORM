using System;
using AntData.ORM.SqlProvider;
using AntData.ORM.SqlQuery;

namespace AntData.ORM.DataProvider.SqlServer
{
	

	class SqlServer2005SqlOptimizer : SqlServerSqlOptimizer
	{
		public SqlServer2005SqlOptimizer(SqlProviderFlags sqlProviderFlags) : base(sqlProviderFlags)
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
