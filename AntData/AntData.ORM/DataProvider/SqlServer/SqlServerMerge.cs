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

    /// <summary>
    /// sqlserver 的 merge 功能 从sqlserver2008开始引进
    /// </summary>
	class SqlServerMerge : BasicMerge
	{
		public SqlServerMerge()
		{
			ByTargetText = "BY Target ";
		}

		protected override bool IsIdentitySupported { get { return true; } }
	    protected string specialSql { get; set; }   
        public override int Merge<T>(DataConnection dataConnection, Expression<Func<T, bool>> predicate, bool delete, IEnumerable<T> source,
			string tableName, string databaseName, string schemaName)
		{
			var table       = dataConnection.MappingSchema.GetEntityDescriptor(typeof(T));
			var HasIdentity = table.Columns.Any(c => c.IsIdentity);

			string tblName = null;

			if (HasIdentity)
			{
				var sqlBuilder = dataConnection.DataProvider.CreateSqlBuilder();

				tblName = sqlBuilder.BuildTableName(new StringBuilder(),
					(string)sqlBuilder.Convert(databaseName ?? table.DatabaseName, ConvertType.NameToDatabase),
					(string)sqlBuilder.Convert(schemaName   ?? table.SchemaName,   ConvertType.NameToOwner),
					(string)sqlBuilder.Convert(tableName    ?? table.TableName,    ConvertType.NameToQueryTable)).ToString();
                specialSql = ("SET IDENTITY_INSERT {0} ON;".Args(tblName)) + Environment.NewLine + "@_@" + Environment.NewLine + ("SET IDENTITY_INSERT {0} OFF;".Args(tblName));
            }

			return base.Merge(dataConnection, predicate, delete, source, tableName, databaseName, schemaName);
			
		}

		protected override bool BuildCommand<T>(DataConnection dataConnection, Expression<Func<T,bool>> deletePredicate, bool delete, IEnumerable<T> source,
			string tableName, string databaseName, string schemaName)
		{
			if (!base.BuildCommand(dataConnection, deletePredicate, delete, source, tableName, databaseName, schemaName))
				return false;

			StringBuilder.AppendLine(";");

			return true;
		}

	    protected override int Execute(DataConnection dataConnection, List<DataParameter> Parameters1)
	    {
            var cmd = StringBuilder.AppendLine().ToString();
	        if (!string.IsNullOrEmpty(specialSql))
	        {
	            cmd = specialSql.Replace("@_@", cmd);
	        }
            return dataConnection.Execute(cmd, Parameters1.ToArray());
        }

    }
}
