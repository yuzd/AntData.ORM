using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using AntData.ORM.Data;
using AntData.ORM.DataProvider;
using AntData.ORM.SqlProvider;

namespace LinqToDB.DataProvider.Oracle
{
	

	class OracleBulkCopy : BasicBulkCopy
	{

		protected override BulkCopyRowsCopied MultipleRowsCopy<T>(
			DataConnection dataConnection, BulkCopyOptions options, IEnumerable<T> source)
		{
			var helper = new MultipleRowsHelper<T>(dataConnection, options, false);

			helper.StringBuilder.AppendLine("INSERT ALL");
			helper.SetHeader();

			foreach (var item in source)
			{
				helper.StringBuilder.AppendFormat("\tINTO {0} (", helper.TableName);

				foreach (var column in helper.Columns)
					helper.StringBuilder
						.Append(helper.SqlBuilder.Convert(column.ColumnName, ConvertType.NameToQueryField))
						.Append(", ");

				helper.StringBuilder.Length -= 2;

				helper.StringBuilder.Append(") VALUES (");
				helper.BuildColumns(item);
				helper.StringBuilder.AppendLine(")");

				helper.RowsCopied.RowsCopied++;
				helper.CurrentCount++;

				if (helper.CurrentCount >= helper.BatchSize || helper.Parameters.Count > 10000 || helper.StringBuilder.Length > 100000)
				{
					helper.StringBuilder.AppendLine("SELECT * FROM dual");
					if (!helper.Execute())
						return helper.RowsCopied;
				}
			}

			if (helper.CurrentCount > 0)
			{
				helper.StringBuilder.AppendLine("SELECT * FROM dual");
				helper.Execute();
			}

			return helper.RowsCopied;
		}
	}
}
