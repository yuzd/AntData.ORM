using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using AntData.ORM.Data;
using AntData.ORM.DataProvider;
using AntData.ORM.Mapping;
using AntData.ORM.SqlProvider;

namespace AntData.ORM.DataProvider.Oracle
{
	

	class OracleBulkCopy : BasicBulkCopy
	{

		protected override BulkCopyRowsCopied MultipleRowsCopy<T>(
			DataConnection dataConnection, BulkCopyOptions options, IEnumerable<T> source)
		{
		    options.KeepIdentity = true;

            var helper = new OracleMultipleRowsHelper<T>(dataConnection, options, true);


            helper.StringBuilder.AppendLine("OracleBulkCopy INSERT ALL");
			helper.SetHeader();
            foreach (var item in source)
			{
				helper.StringBuilder.AppendFormat("\tINTO {0} (", helper.TableName);

				foreach (var column in helper.Columns)
				{
                  

                    helper.StringBuilder
				        .Append(helper.SqlBuilder.Convert(column.ColumnName, ConvertType.NameToQueryField))
				        .Append(", ");
				}

				helper.StringBuilder.Length -= 2;

				helper.StringBuilder.Append(") VALUES (");
				helper.BuildColumns(item, helper.TableName);
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

        //private void AppenCreateFunction<T>(OracleMultipleRowsHelper<T> sb)
        //{
        //    if (string.IsNullOrEmpty(sb.WithSeqName)) return;
        //    sb.StringBuilder.AppendLine("create or replace");
        //    sb.StringBuilder.AppendLine("FUNCTION GET_IDENTITY_ID RETURN NUMBER AS ");
        //    sb.StringBuilder.AppendLine("num NUMBER; ");
        //    sb.StringBuilder.AppendLine("BEGIN ");
        //    sb.StringBuilder.AppendLine(string.Format(" SELECT {0}  ",sb.WithSeqName));
        //    sb.StringBuilder.AppendLine("  INTO num  ");
        //    sb.StringBuilder.AppendLine("    FROM dual;    ");
        //    sb.StringBuilder.AppendLine("   return num;   ");
        //    sb.StringBuilder.AppendLine("   END GET_IDENTITY_ID;   ");
        //}
        //private void AppenDropFunction<T>(OracleMultipleRowsHelper<T> sb)
        //{
        //    if (string.IsNullOrEmpty(sb.WithSeqName)) return;
        //    sb.StringBuilder.AppendLine(";drop function GET_IDENTITY_ID;");

        //}
	   
	}
}
