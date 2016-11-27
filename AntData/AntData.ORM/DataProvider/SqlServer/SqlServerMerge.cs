using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using AntData.ORM.Common;
using AntData.ORM.Data;
using AntData.ORM.DataProvider;
using AntData.ORM.SqlProvider;

namespace AntData.ORM.DataProvider.SqlServer
{


	class SqlServerMerge : BasicMerge
	{
		public SqlServerMerge()
		{
			ByTargetText = "BY Target ";
		}

		protected override bool IsIdentitySupported { get { return true; } }

		public override int Merge<T>(DataConnection dataConnection, Expression<Func<T, bool>> predicate, bool delete, IEnumerable<T> source,
			string tableName, string databaseName, string schemaName)
		{
			var table       = dataConnection.MappingSchema.GetEntityDescriptor(typeof(T));
			var hasIdentity = table.Columns.Any(c => c.IsIdentity);

			string tblName = null;

			if (hasIdentity)
			{
				var sqlBuilder = dataConnection.DataProvider.CreateSqlBuilder();

				tblName = sqlBuilder.BuildTableName(new StringBuilder(),
					(string)sqlBuilder.Convert(databaseName ?? table.DatabaseName, ConvertType.NameToDatabase),
					(string)sqlBuilder.Convert(schemaName   ?? table.SchemaName,   ConvertType.NameToOwner),
					(string)sqlBuilder.Convert(tableName    ?? table.TableName,    ConvertType.NameToQueryTable)).ToString();

				dataConnection.Execute("SET IDENTITY_INSERT {0} ON".Args(tblName));
			}

			try
			{
				return base.Merge(dataConnection, predicate, delete, source, tableName, databaseName, schemaName);
			}
			finally
			{
				if (hasIdentity)
					dataConnection.Execute("SET IDENTITY_INSERT {0} OFF".Args(tblName));
			}
		}

		protected override bool BuildCommand<T>(DataConnection dataConnection, Expression<Func<T,bool>> deletePredicate, bool delete, IEnumerable<T> source,
			string tableName, string databaseName, string schemaName)
		{
			if (!base.BuildCommand(dataConnection, deletePredicate, delete, source, tableName, databaseName, schemaName))
				return false;

			StringBuilder.AppendLine(";");

			return true;
		}
	}
}
