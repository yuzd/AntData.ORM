using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;

namespace AntData.ORM.DataProvider.MySql
{
//	using Common;
//	using Data;
//	using SchemaProvider;

//	class MySqlSchemaProvider : SchemaProviderBase
//	{
//		protected override List<DataTypeInfo> GetDataTypes(DataConnection dataConnection)
//		{
//			return base.GetDataTypes(dataConnection)
//				.Select(dt =>
//				{
//					if (dt.CreateFormat != null && dt.CreateFormat.EndsWith(" UNSIGNED", StringComparison.OrdinalIgnoreCase))
//					{
//						return new DataTypeInfo
//						{
//							TypeName         = dt.CreateFormat,
//							DataType         = dt.DataType,
//							CreateFormat     = dt.CreateFormat,
//							CreateParameters = dt.CreateParameters,
//							ProviderDbType   = dt.ProviderDbType,
//						};
//					}

//					return dt;
//				})
//				.ToList();
//		}

//        public static DataTable FillDataTable(DbConnection conn, string strSQL)
//        {
//            using (var cmd = conn.CreateCommand())
//            {
//                cmd.CommandText = strSQL;
//                using (var reader = cmd.ExecuteReader())
//                {
//                    DataSet ds = new DataSet();
//                    ds.EnforceConstraints = false;
//                    DataTable table = new DataTable("_tb_other");
//                    ds.Tables.Add(table);
//                    table.Load(reader);
//                    return table;
//                }
//            }
//        }
//		protected override List<TableInfo> GetTables(DataConnection dataConnection)
//		{
//			var tables = ((DbConnection)dataConnection.Connection).GetSchema("Tables");
//			var views  = ((DbConnection)dataConnection.Connection).GetSchema("Views");
//		    string sql_get_table_comment = @"SELECT  table_name,table_comment FROM INFORMATION_SCHEMA.TABLES WHERE LENGTH(table_comment)>0";
//            DataTable comments = FillDataTable(((DbConnection)dataConnection.Connection), sql_get_table_comment);
//            var commentsDic = (from myRow in comments.AsEnumerable()
//                          select new { TableName = myRow.Field<string>("table_name"), TableComment = myRow.Field<string>("table_comment") })
//                          .ToDictionary(r=>r.TableName,v=>v.TableComment);

//			return
//			(
//				from t in tables.AsEnumerable()
//				let catalog = t.Field<string>("TABLE_SCHEMA")
//				let name    = t.Field<string>("TABLE_NAME")
//				select new TableInfo
//				{
//					TableID         = catalog + ".." + name,
//					CatalogName     = catalog,
//					SchemaName      = "",
//					TableName       = name,
//					IsDefaultSchema = true,
//					IsView          = false,
//                    Description = commentsDic.ContainsKey(name)?commentsDic[name]:String.Empty
//				}
//			).Concat(
//				from t in views.AsEnumerable()
//				let catalog = t.Field<string>("TABLE_SCHEMA")
//				let name    = t.Field<string>("TABLE_NAME")
//				select new TableInfo
//				{
//					TableID         = catalog + ".." + name,
//					CatalogName     = catalog,
//					SchemaName      = "",
//					TableName       = name,
//					IsDefaultSchema = true,
//					IsView          = true,
//                    Description = commentsDic.ContainsKey(name) ? commentsDic[name] : String.Empty
//				}
//			).ToList();
//		}

//		protected override List<PrimaryKeyInfo> GetPrimaryKeys(DataConnection dataConnection)
//		{
//			var dbConnection = (DbConnection)dataConnection.Connection;
//			var pks          = dbConnection.GetSchema("IndexColumns");
//			var idxs         = dbConnection.GetSchema("Indexes");

//			return
//			(
//				from pk  in pks. AsEnumerable()
//				join idx in idxs.AsEnumerable()
//					on
//						pk. Field<string>("INDEX_CATALOG") + "." +
//						pk. Field<string>("INDEX_SCHEMA")  + "." +
//						pk. Field<string>("INDEX_NAME")    + "." +
//						pk. Field<string>("TABLE_NAME")
//					equals
//						idx.Field<string>("INDEX_CATALOG") + "." +
//						idx.Field<string>("INDEX_SCHEMA")  + "." +
//						idx.Field<string>("INDEX_NAME")    + "." +
//						idx.Field<string>("TABLE_NAME")
//				where idx.Field<bool>("PRIMARY")
//				select new PrimaryKeyInfo
//				{
//					TableID        = pk.Field<string>("INDEX_SCHEMA") + ".." + pk.Field<string>("TABLE_NAME"),
//					PrimaryKeyName = pk.Field<string>("INDEX_NAME"),
//					ColumnName     = pk.Field<string>("COLUMN_NAME"),
//					Ordinal        = pk.Field<int>   ("ORDINAL_POSITION"),
//				}
//			).ToList();
//		}

//		protected override List<ColumnInfo> GetColumns(DataConnection dataConnection)
//		{
//			var tcs = ((DbConnection)dataConnection.Connection).GetSchema("Columns");
////			var vcs = ((DbConnection)dataConnection.Connection).GetSchema("ViewColumns");
//		    var tabelName = tcs.AsEnumerable().Select(r => r.Field<string>("TABLE_NAME")).ToArray();
//		    var name = string.Join("','", tabelName);
//            string sql_get_table_comment = string.Format(" SELECT a.TABLE_NAME tableName , a.COLUMN_NAME columnNanme, a.COLUMN_COMMENT commentName FROM  information_schema.COLUMNS a WHERE a.TABLE_NAME in ({0}) ", "'" + name + "'");
//            DataTable comments = FillDataTable(((DbConnection)dataConnection.Connection), sql_get_table_comment);
//            Dictionary<string, string>  commentsDic = (from myRow in comments.AsEnumerable()
//                                                       select new { ColumnName = myRow.Field<string>("tableName") + "."+myRow.Field<string>("columnNanme"), TableComment = myRow.Field<string>("commentName") })
//                          .ToDictionary(r =>  r.ColumnName, v => v.TableComment);
//			var ret =
//			(
//				from c in tcs.AsEnumerable()
//				let dataType = c.Field<string>("DATA_TYPE")
//                let key = c.Field<string>("TABLE_NAME") + "." + c.Field<string>("COLUMN_NAME")
//				select new ColumnInfo
//				{
//					TableID      = c.Field<string>("TABLE_SCHEMA") + ".." + c.Field<string>("TABLE_NAME"),
//					Name         = c.Field<string>("COLUMN_NAME"),
//					IsNullable   = c.Field<string>("IS_NULLABLE") == "YES",
//					Ordinal      = Converter.ChangeTypeTo<int> (c["ORDINAL_POSITION"]),
//					DataType     = dataType,
//					Length       = Converter.ChangeTypeTo<long?>(c["CHARACTER_MAXIMUM_LENGTH"]),
//					Precision    = Converter.ChangeTypeTo<int?> (c["NUMERIC_PRECISION"]),
//					Scale        = Converter.ChangeTypeTo<int?> (c["NUMERIC_SCALE"]),
//					ColumnType   = c.Field<string>("COLUMN_TYPE"),
//					IsIdentity   = c.Field<string>("EXTRA") == "auto_increment",
//                    Description = commentsDic.ContainsKey(key) ? commentsDic[key] : String.Empty
//				}
//			)
////			.Concat(
////				from c in vcs.AsEnumerable()
////				let dataType = c.Field<string>("DATA_TYPE")
////				select new ColumnInfo
////				{
////					TableID      = c.Field<string>("VIEW_SCHEMA") + ".." + c.Field<string>("VIEW_NAME"),
////					Name         = c.Field<string>("COLUMN_NAME"),
////					IsNullable   = c.Field<string>("IS_NULLABLE") == "YES",
////					Ordinal      = Converter.ChangeTypeTo<int> (c["ORDINAL_POSITION"]),
////					DataType     = dataType,
////					Length       = Converter.ChangeTypeTo<long?>(c["CHARACTER_MAXIMUM_LENGTH"]),
////					Precision    = Converter.ChangeTypeTo<int?> (c["NUMERIC_PRECISION"]),
////					Scale        = Converter.ChangeTypeTo<int?> (c["NUMERIC_SCALE"]),
////					ColumnType   = c.Field<string>("COLUMN_TYPE"),
////					IsIdentity   = c.Field<string>("EXTRA") == "auto_increment",
////				}
////			)
//			.Select(ci =>
//			{
//				switch (ci.DataType)
//				{
//					case "bit"        :
//					case "date"       :
//					case "datetime"   :
//					case "timestamp"  :
//					case "time"       :
//					case "tinyint"    :
//					case "smallint"   :
//					case "int"        :
//					case "year"       :
//					case "mediumint"  :
//					case "bigint"     :
//					case "tiny int"   :
//						ci.Precision = null;
//						ci.Scale     = null;
//						break;
//				}

//				return ci;
//			})
//			.ToList();

//			return ret;
//		}

//		protected override List<ForeingKeyInfo> GetForeignKeys(DataConnection dataConnection)
//		{
//			var fks = ((DbConnection)dataConnection.Connection).GetSchema("Foreign Key Columns");

//			return
//			(
//				from fk in fks.AsEnumerable()
//				select new ForeingKeyInfo
//				{
//					Name         = fk.Field<string>("CONSTRAINT_NAME"),
//					ThisTableID  = fk.Field<string>("TABLE_SCHEMA")   + ".." + fk.Field<string>("TABLE_NAME"),
//					ThisColumn   = fk.Field<string>("COLUMN_NAME"),
//					OtherTableID = fk.Field<string>("REFERENCED_TABLE_SCHEMA") + ".." + fk.Field<string>("REFERENCED_TABLE_NAME"),
//					OtherColumn  = fk.Field<string>("REFERENCED_COLUMN_NAME"),
//					Ordinal      = Converter.ChangeTypeTo<int>(fk["ORDINAL_POSITION"]),
//				}
//			).ToList();
//		}

//		protected override DataType GetDataType(string dataType, string columnType, long? length, int? prec, int? scale)
//		{
//			switch (dataType.ToLower())
//			{
//				case "bit"        : return DataType.UInt64;
//				case "blob"       : return DataType.Blob;
//				case "tinyblob"   : return DataType.Binary;
//				case "mediumblob" : return DataType.Binary;
//				case "longblob"   : return DataType.Binary;
//				case "binary"     : return DataType.Binary;
//				case "varbinary"  : return DataType.VarBinary;
//				case "date"       : return DataType.Date;
//				case "datetime"   : return DataType.DateTime;
//				case "timestamp"  : return DataType.Timestamp;
//				case "time"       : return DataType.Time;
//				case "char"       : return DataType.Char;
//				case "nchar"      : return DataType.NChar;
//				case "varchar"    : return DataType.VarChar;
//				case "nvarchar"   : return DataType.NVarChar;
//				case "set"        : return DataType.NVarChar;
//				case "enum"       : return DataType.NVarChar;
//				case "tinytext"   : return DataType.Text;
//				case "text"       : return DataType.Text;
//				case "mediumtext" : return DataType.Text;
//				case "longtext"   : return DataType.Text;
//				case "double"     : return DataType.Double;
//				case "float"      : return DataType.Single;
//				case "tinyint"    : return columnType == "tinyint(1)" ? DataType.Boolean : DataType.SByte;
//				case "smallint"   : return columnType != null && columnType.Contains("unsigned") ? DataType.UInt16 : DataType.Int16;
//				case "int"        : return columnType != null && columnType.Contains("unsigned") ? DataType.UInt32 : DataType.Int32;
//				case "year"       : return DataType.Int32;
//				case "mediumint"  : return columnType != null && columnType.Contains("unsigned") ? DataType.UInt32 : DataType.Int32;
//				case "bigint"     : return columnType != null && columnType.Contains("unsigned") ? DataType.UInt64 : DataType.Int64;
//				case "decimal"    : return DataType.Decimal;
//				case "tiny int"   : return DataType.Byte;
//			}

//			return DataType.Undefined;
//		}

//		protected override string GetProviderSpecificTypeNamespace()
//		{
//			return "MySql.Data.Types";
//		}

//		protected override string GetProviderSpecificType(string dataType)
//		{
//			switch (dataType.ToLower())
//			{
//				case "geometry"  : return "MySqlGeometry";
//				case "decimal"   : return "MySqlDecimal";
//				case "date"      :
//				case "newdate"   :
//				case "datetime"  :
//				case "timestamp" : return "MySqlDateTime";
//			}

//			return base.GetProviderSpecificType(dataType);
//		}

//		protected override Type GetSystemType(string dataType, string columnType, DataTypeInfo dataTypeInfo, long? length, int? precision, int? scale)
//		{
//			if (columnType != null && columnType.Contains("unsigned"))
//			{
//				switch (dataType.ToLower())
//				{
//					case "smallint"   : return typeof(UInt16);
//					case "int"        : return typeof(UInt32);
//					case "mediumint"  : return typeof(UInt32);
//					case "bigint"     : return typeof(UInt64);
//					case "tiny int"   : return typeof(Byte);
//				}
//			}

//			switch (dataType)
//			{
//				case "tinyint"   :
//					if (columnType == "tinyint(1)")
//						return typeof(Boolean);
//					break;
//				case "datetime2" : return typeof(DateTime);
//			}

//			return base.GetSystemType(dataType, columnType, dataTypeInfo, length, precision, scale);
//		}
//	}
}
