using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using AntData.ORM.Data;
using AntData.ORM.DataProvider;
using AntData.ORM.Extensions;
using LinqToDB.DataProvider;
using JetBrains.Annotations;

namespace AntData.ORM.DataProvider.PostgreSQL
{

	using Data;

	[PublicAPI]
	public static class PostgreSQLTools
	{
		static readonly PostgreSQLDataProvider _postgreSQLDataProvider   = new PostgreSQLDataProvider();
		static readonly PostgreSQLDataProvider _postgreSQLDataProvider92 = new PostgreSQLDataProvider(ProviderName.PostgreSQL92, PostgreSQLVersion.v92);
		static readonly PostgreSQLDataProvider _postgreSQLDataProvider93 = new PostgreSQLDataProvider(ProviderName.PostgreSQL93, PostgreSQLVersion.v93);

		public static bool AutoDetectProvider { get; set; }

		static PostgreSQLTools()
		{
			AutoDetectProvider = true;

			DataConnection.AddDataProvider(_postgreSQLDataProvider);
			DataConnection.AddDataProvider(_postgreSQLDataProvider92);
			DataConnection.AddDataProvider(_postgreSQLDataProvider93);
		}

		public static IDataProvider GetDataProvider(PostgreSQLVersion version = PostgreSQLVersion.v92)
		{
			return version == PostgreSQLVersion.v92 ? _postgreSQLDataProvider : _postgreSQLDataProvider93;
		}

		public static void ResolvePostgreSQL(string path)
		{
			new AssemblyResolver(path, "Npgsql");
		}

		public static void ResolvePostgreSQL(Assembly assembly)
		{
			new AssemblyResolver(assembly, "Npgsql");
		}

		public static Type GetBitStringType       () { return _postgreSQLDataProvider92.BitStringType;        }
		public static Type GetNpgsqlIntervalType  () { return _postgreSQLDataProvider92.NpgsqlIntervalType;   }
		public static Type GetNpgsqlInetType      () { return _postgreSQLDataProvider92.NpgsqlInetType;       }
		public static Type GetNpgsqlTimeTZType    () { return _postgreSQLDataProvider92.NpgsqlTimeTZType;     }
		public static Type GetNpgsqlTimeType      () { return _postgreSQLDataProvider92.NpgsqlTimeType;       }
		public static Type GetNpgsqlPointType     () { return _postgreSQLDataProvider92.NpgsqlPointType;      }
		public static Type GetNpgsqlLSegType      () { return _postgreSQLDataProvider92.NpgsqlLSegType;       }
		public static Type GetNpgsqlBoxType       () { return _postgreSQLDataProvider92.NpgsqlBoxType;        }
		public static Type GetNpgsqlPathType      () { return _postgreSQLDataProvider92.NpgsqlPathType;       }
		public static Type GetNpgsqlPolygonType   () { return _postgreSQLDataProvider92.NpgsqlPolygonType;    }
		public static Type GetNpgsqlCircleType    () { return _postgreSQLDataProvider92.NpgsqlCircleType;     }
		public static Type GetNpgsqlMacAddressType() { return _postgreSQLDataProvider92.NpgsqlMacAddressType; }


		#region BulkCopy

		private static BulkCopyType _defaultBulkCopyType = BulkCopyType.MultipleRows;
		public  static BulkCopyType  DefaultBulkCopyType
		{
			get { return _defaultBulkCopyType;  }
			set { _defaultBulkCopyType = value; }
		}

		public static BulkCopyRowsCopied MultipleRowsCopy<T>(
			DataConnection             dataConnection,
			IEnumerable<T>             source,
			int                        maxBatchSize       = 1000,
			Action<BulkCopyRowsCopied> rowsCopiedCallback = null)
		{
			return dataConnection.BulkCopy(
				new BulkCopyOptions
				{
					BulkCopyType       = BulkCopyType.MultipleRows,
					MaxBatchSize       = maxBatchSize,
					RowsCopiedCallback = rowsCopiedCallback,
				}, source);
		}

		#endregion
	}
}
