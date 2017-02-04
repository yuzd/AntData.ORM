using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Linq;
using System.Linq.Expressions;
using AntData.ORM;
using AntData.ORM.Common;
using AntData.ORM.Data;
using AntData.ORM.DataProvider;
using AntData.ORM.Extensions;
using AntData.ORM.Mapping;
using AntData.ORM.SchemaProvider;
using AntData.ORM.SqlProvider;

namespace AntData.ORM.DataProvider.SqlServer
{

	public class SqlServerDataProvider : DataProviderBase
	{
        #region Init

        public SqlServerDataProvider(SqlServerVersion version)
           : this(ProviderName.SqlServer, version)
        {
            
        }

        public SqlServerDataProvider(string name, SqlServerVersion version)
			: base(name, null)
		{
			Version = version;

			if (version == SqlServerVersion.v2000)
			{
				SqlProviderFlags.AcceptsTakeAsParameter   = false;
				SqlProviderFlags.IsSkipSupported          = false;
				SqlProviderFlags.IsCountSubQuerySupported = false;
			}
            else 
            {
                SqlProviderFlags.IsApplyJoinSupported = true;
            }

            SetCharField("char",  (r,i) => r.GetString(i).TrimEnd());
			SetCharField("nchar", (r,i) => r.GetString(i).TrimEnd());

			if (!Configuration.AvoidSpecificDataProviderAPI)
			{
				SetProviderField<SqlDataReader,SqlBinary  ,SqlBinary  >((r,i) => r.GetSqlBinary  (i));
				SetProviderField<SqlDataReader,SqlBoolean ,SqlBoolean >((r,i) => r.GetSqlBoolean (i));
				SetProviderField<SqlDataReader,SqlByte    ,SqlByte    >((r,i) => r.GetSqlByte    (i));
				SetProviderField<SqlDataReader,SqlDateTime,SqlDateTime>((r,i) => r.GetSqlDateTime(i));
				SetProviderField<SqlDataReader,SqlDecimal ,SqlDecimal >((r,i) => r.GetSqlDecimal (i));
				SetProviderField<SqlDataReader,SqlDouble  ,SqlDouble  >((r,i) => r.GetSqlDouble  (i));
				SetProviderField<SqlDataReader,SqlGuid    ,SqlGuid    >((r,i) => r.GetSqlGuid    (i));
				SetProviderField<SqlDataReader,SqlInt16   ,SqlInt16   >((r,i) => r.GetSqlInt16   (i));
				SetProviderField<SqlDataReader,SqlInt32   ,SqlInt32   >((r,i) => r.GetSqlInt32   (i));
				SetProviderField<SqlDataReader,SqlInt64   ,SqlInt64   >((r,i) => r.GetSqlInt64   (i));
				SetProviderField<SqlDataReader,SqlMoney   ,SqlMoney   >((r,i) => r.GetSqlMoney   (i));
				SetProviderField<SqlDataReader,SqlSingle  ,SqlSingle  >((r,i) => r.GetSqlSingle  (i));
				SetProviderField<SqlDataReader,SqlString  ,SqlString  >((r,i) => r.GetSqlString  (i));
				SetProviderField<SqlDataReader,SqlXml     ,SqlXml     >((r,i) => r.GetSqlXml     (i));

				SetProviderField<SqlDataReader,DateTimeOffset>((r,i) => r.GetDateTimeOffset(i));
				SetProviderField<SqlDataReader,TimeSpan>      ((r,i) => r.GetTimeSpan      (i));
			}
			else
			{
				SetProviderField<IDataReader,SqlString  ,SqlString  >((r,i) => r.GetString  (i));
			}

			_sqlOptimizer              = new SqlServerSqlOptimizer    (SqlProviderFlags);
			_sqlServer2000SqlOptimizer = new SqlServer2000SqlOptimizer(SqlProviderFlags);
			_sqlServer2005SqlOptimizer = new SqlServer2005SqlOptimizer(SqlProviderFlags);
		}

		#endregion

		#region Public Properties

	    public override string ParameterSymbol
	    {
	        get { return "@"; }
	    }

	    public override bool InsertWinthIdentityWithNoCache {
	        get { return false; }
	    }

	    public override string ConnectionNamespace { get { return typeof(SqlConnection).Namespace; } }
		public override Type   DataReaderType      { get { return typeof(SqlDataReader);           } }

		public SqlServerVersion Version { get; private set; }

		#endregion

		#region Overrides

		static class MappingSchemaInstance
		{
			public static readonly SqlServer2000MappingSchema SqlServer2000MappingSchema = new SqlServer2000MappingSchema();
			public static readonly SqlServer2005MappingSchema SqlServer2005MappingSchema = new SqlServer2005MappingSchema();
			public static readonly SqlServer2008MappingSchema SqlServer2008MappingSchema = new SqlServer2008MappingSchema();
			public static readonly SqlServer2012MappingSchema SqlServer2012MappingSchema = new SqlServer2012MappingSchema();
		}

		public override MappingSchema MappingSchema
		{
			get
			{
				switch (Version)
				{
					case SqlServerVersion.v2000 : return MappingSchemaInstance.SqlServer2000MappingSchema;
					case SqlServerVersion.v2005 : return MappingSchemaInstance.SqlServer2005MappingSchema;
					case SqlServerVersion.v2008 : return MappingSchemaInstance.SqlServer2008MappingSchema;
					case SqlServerVersion.v2012 : return MappingSchemaInstance.SqlServer2012MappingSchema;
				}

				return base.MappingSchema;
			}
		}


		public override ISqlBuilder CreateSqlBuilder()
		{
			switch (Version)
			{
				case SqlServerVersion.v2000 : return new SqlServer2000SqlBuilder(GetSqlOptimizer(), SqlProviderFlags, MappingSchema.ValueToSqlConverter);
				case SqlServerVersion.v2005 : return new SqlServer2005SqlBuilder(GetSqlOptimizer(), SqlProviderFlags, MappingSchema.ValueToSqlConverter);
				case SqlServerVersion.v2008 : return new SqlServer2008SqlBuilder(GetSqlOptimizer(), SqlProviderFlags, MappingSchema.ValueToSqlConverter);
				case SqlServerVersion.v2012 : return new SqlServer2012SqlBuilder(GetSqlOptimizer(), SqlProviderFlags, MappingSchema.ValueToSqlConverter);
			}

			throw new InvalidOperationException();
		}

		readonly ISqlOptimizer _sqlOptimizer;
		readonly ISqlOptimizer _sqlServer2000SqlOptimizer;
		readonly ISqlOptimizer _sqlServer2005SqlOptimizer;

		public override ISqlOptimizer GetSqlOptimizer()
		{
			switch (Version)
			{
				case SqlServerVersion.v2000 : return _sqlServer2000SqlOptimizer;
				case SqlServerVersion.v2005 : return _sqlServer2005SqlOptimizer;
			}

			return _sqlOptimizer;
		}

		public override bool IsCompatibleConnection(IDbConnection connection)
		{
			return typeof(SqlConnection).IsSameOrParentOf(connection.GetType());
		}

	

		static readonly ConcurrentDictionary<string,bool> _marsFlags = new ConcurrentDictionary<string,bool>();



        public override void SetParameter(IDbDataParameter parameter, string name, DataType dataType, object value)
        {
            switch (dataType)
            {
                case DataType.Udt:
                    {
                        string s;
                        if (value != null && _udtTypes.TryGetValue(value.GetType(), out s))
                            if (parameter is SqlParameter)
#if NETSTANDARD
                                ((SqlParameter)parameter).TypeName = s;
#else
								((SqlParameter)parameter).UdtTypeName = s;
#endif

                    }

                    break;
            }

            base.SetParameter(parameter, name, dataType, value);
        }

        protected override void SetParameterType(IDbDataParameter parameter, DataType dataType)
		{
			if (parameter is BulkCopyReader.Parameter)
				return;

			switch (dataType)
			{
				case DataType.SByte         : parameter.DbType = DbType.Int16;   break;
				case DataType.UInt16        : parameter.DbType = DbType.Int32;   break;
				case DataType.UInt32        : parameter.DbType = DbType.Int64;   break;
				case DataType.UInt64        : parameter.DbType = DbType.Decimal; break;
				case DataType.VarNumeric    : parameter.DbType = DbType.Decimal; break;
				case DataType.DateTime2     :
					parameter.DbType =
						Version == SqlServerVersion.v2000 || Version == SqlServerVersion.v2005 ?
							DbType.DateTime :
							DbType.DateTime2;
					break;
				case DataType.Text          : ((SqlParameter)parameter).SqlDbType = SqlDbType.Text;          break;
				case DataType.NText         : ((SqlParameter)parameter).SqlDbType = SqlDbType.NText;         break;
				case DataType.Binary        : ((SqlParameter)parameter).SqlDbType = SqlDbType.Binary;        break;
				case DataType.Blob          :
				case DataType.VarBinary     : ((SqlParameter)parameter).SqlDbType = SqlDbType.VarBinary;     break;
				case DataType.Image         : ((SqlParameter)parameter).SqlDbType = SqlDbType.Image;         break;
				case DataType.Money         : ((SqlParameter)parameter).SqlDbType = SqlDbType.Money;         break;
				case DataType.SmallMoney    : ((SqlParameter)parameter).SqlDbType = SqlDbType.SmallMoney;    break;
				case DataType.Date          : ((SqlParameter)parameter).SqlDbType = SqlDbType.Date;          break;
				case DataType.Time          : ((SqlParameter)parameter).SqlDbType = SqlDbType.Time;          break;
				case DataType.SmallDateTime : ((SqlParameter)parameter).SqlDbType = SqlDbType.SmallDateTime; break;
				case DataType.Timestamp     : ((SqlParameter)parameter).SqlDbType = SqlDbType.Timestamp;     break;
				default                     : base.SetParameterType(parameter, dataType);                    break;
			}
		}

		#endregion

		#region Udt support

		static readonly ConcurrentDictionary<Type,string> _udtTypes = new ConcurrentDictionary<Type,string>();

		internal static void SetUdtType(Type type, string udtName)
		{
			_udtTypes[type] = udtName;
		}

		internal static Type GetUdtType(string udtName)
		{
			foreach (var udtType in _udtTypes)
				if (udtType.Value == udtName)
					return udtType.Key;

			return null;
		}

		public void AddUdtType(Type type, string udtName)
		{
			MappingSchema.SetScalarType(type);

			_udtTypes[type] = udtName;
		}

		public void AddUdtType<T>(string udtName, T defaultValue, DataType dataType = DataType.Undefined)
		{
			MappingSchema.AddScalarType(typeof(T), defaultValue, dataType);

			_udtTypes[typeof(T)] = udtName;
		}

		#endregion

		#region BulkCopy

		SqlServerBulkCopy _bulkCopy;

		public override BulkCopyRowsCopied BulkCopy<T>(DataConnection dataConnection, BulkCopyOptions options, IEnumerable<T> source)
		{
			if (_bulkCopy == null)
				_bulkCopy = new SqlServerBulkCopy();

			return _bulkCopy.BulkCopy(
				options.BulkCopyType == BulkCopyType.Default ? SqlServerTools.DefaultBulkCopyType : options.BulkCopyType,
				dataConnection,
				options,
				source);
		}

		#endregion

		#region Merge

		public override int Merge<T>(DataConnection dataConnection, Expression<Func<T,bool>> deletePredicate, bool delete, IEnumerable<T> source,
			string tableName, string databaseName, string schemaName)
		{
			return new SqlServerMerge().Merge(dataConnection, deletePredicate, delete, source, tableName, databaseName, schemaName);
		}

		#endregion
	}
}
