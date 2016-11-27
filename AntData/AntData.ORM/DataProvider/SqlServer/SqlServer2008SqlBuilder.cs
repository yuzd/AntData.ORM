using System;
using AntData.ORM;
using AntData.ORM.SqlProvider;
using AntData.ORM.SqlQuery;

namespace AntData.ORM.DataProvider.SqlServer
{
	

	class SqlServer2008SqlBuilder : SqlServerSqlBuilder
	{
		public SqlServer2008SqlBuilder(ISqlOptimizer sqlOptimizer, SqlProviderFlags sqlProviderFlags, ValueToSqlConverter valueToSqlConverter)
			: base(sqlOptimizer, sqlProviderFlags, valueToSqlConverter)
		{
		}

		protected override ISqlBuilder CreateSqlBuilder()
		{
			return new SqlServer2008SqlBuilder(SqlOptimizer, SqlProviderFlags, ValueToSqlConverter);
		}

		protected override void BuildInsertOrUpdateQuery()
		{
			BuildInsertOrUpdateQueryAsMerge(null);
			StringBuilder.AppendLine(";");
		}

		protected override void BuildFunction(SqlFunction func)
		{
			func = ConvertFunctionParameters(func);
			base.BuildFunction(func);
		}

		public override string  Name
		{
			get { return ProviderName.SqlServer2008; }
		}
	}
}
