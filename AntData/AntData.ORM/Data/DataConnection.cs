using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using AntData.ORM.Dao;
using JetBrains.Annotations;

namespace AntData.ORM.Data
{
	using System.Text;

	using Common;
	using DataProvider;

	using Mapping;

	public partial class DataConnection
	{
		#region .ctor

	    public DataConnection([JetBrains.Annotations.NotNull] IDataProvider dataProvider, string dbMappingName)
	    {
            if (dataProvider == null) throw new ArgumentNullException("dataProvider");
            AddDataProvider(dataProvider);
            DataProvider = dataProvider;
            _mappingSchema = DataProvider.MappingSchema;
            ConnectionString = dbMappingName;
	        this.CustomerExecuteNonQuery = DalBridge.CustomerExecuteNonQuery;
            this.CustomerExecuteScalar = DalBridge.CustomerExecuteScalar;
            this.CustomerExecuteQuery = DalBridge.CustomerExecuteQuery;
            this.CustomerExecuteQueryTable = DalBridge.CustomerExecuteQueryTable;
        }

        public DataConnection([JetBrains.Annotations.NotNull] IDataProvider dataProvider, string dbMappingName, Func<string, string, Dictionary<string, CustomerParam>, IDictionary, int> CustomerExecuteNonQuery, Func<string, string, Dictionary<string, CustomerParam>, IDictionary, object> CustomerExecuteScalar, Func<string, string, Dictionary<string, CustomerParam>, IDictionary, IDataReader> CustomerExecuteQuery, Func<string, string, Dictionary<string, CustomerParam>, IDictionary, DataTable> CustomerExecuteQueryTable)
        {
            if (dataProvider == null) throw new ArgumentNullException("dataProvider");

            AddDataProvider(dataProvider);
            DataProvider = dataProvider;
            _mappingSchema = DataProvider.MappingSchema;
            ConnectionString = dbMappingName;
            this.CustomerExecuteNonQuery = CustomerExecuteNonQuery;
            this.CustomerExecuteScalar = CustomerExecuteScalar;
            this.CustomerExecuteQuery = CustomerExecuteQuery;
            this.CustomerExecuteQueryTable = CustomerExecuteQueryTable;
        }
		#endregion

		#region Public Properties

		public IDataProvider DataProvider        { get; private set; }
		public string        ConnectionString    { get; private set; }

		static readonly ConcurrentDictionary<string,int> _configurationIDs;
		static int _maxID;

		private int? _id;
		public  int   ID
		{
			get
			{
				if (!_id.HasValue)
				{
					var key = MappingSchema.ConfigurationID + "." + ConnectionString;
					int id;

					if (!_configurationIDs.TryGetValue(key, out id))
						_configurationIDs[key] = id = Interlocked.Increment(ref _maxID);

					_id = id;
				}

				return _id.Value;
			}
		}

		private bool? _isMarsEnabled;
		internal  bool   IsMarsEnabled
		{
			get
			{
				if (_isMarsEnabled == null)
					_isMarsEnabled = (bool)(DataProvider.GetConnectionInfo(this, "IsMarsEnabled") ?? false);

				return _isMarsEnabled.Value;
			}
			set { _isMarsEnabled = value; }
		}


		

        private Action<CustomerTraceInfo> _onCustomerTraceConnection;
		[JetBrains.Annotations.CanBeNull]
        public Action<CustomerTraceInfo> OnCustomerTraceConnection
		{
            get { return _onCustomerTraceConnection; }
            set { _onCustomerTraceConnection = value; }
		}

        public static IDataProvider GetDataProvider([JetBrains.Annotations.NotNull] string providerName)
        {
            return _dataProviders[providerName];
        }

        private Func<string, string, Dictionary<string, CustomerParam>, IDictionary,int > CustomerExecuteNonQuery { get; set; }

        private Func<string, string, Dictionary<string, CustomerParam>, IDictionary, object> CustomerExecuteScalar { get; set; }

        private Func<string, string, Dictionary<string, CustomerParam>, IDictionary, IDataReader> CustomerExecuteQuery { get; set; }

        private Func<string, string, Dictionary<string, CustomerParam>, IDictionary, DataTable> CustomerExecuteQueryTable { get; set; }
	  


		public static Action<string,string> WriteTraceLine = (message, displayName) => Debug.WriteLine(message, displayName);

		#endregion



		static DataConnection()
		{
			_configurationIDs = new ConcurrentDictionary<string,int>();

            //LinqToDB.DataProvider.SqlServer. SqlServerTools. GetDataProvider();
            //LinqToDB.DataProvider.Access.    AccessTools.    GetDataProvider();
            //LinqToDB.DataProvider.SqlCe.     SqlCeTools.     GetDataProvider();
            //LinqToDB.DataProvider.Firebird.  FirebirdTools.  GetDataProvider();
            AntData.ORM.DataProvider.MySql.MySqlTools.GetDataProvider();
            //LinqToDB.DataProvider.SQLite.    SQLiteTools.    GetDataProvider();
            //LinqToDB.DataProvider.Sybase.    SybaseTools.    GetDataProvider();
            //LinqToDB.DataProvider.Oracle.    OracleTools.    GetDataProvider();
            //LinqToDB.DataProvider.PostgreSQL.PostgreSQLTools.GetDataProvider();
            //LinqToDB.DataProvider.DB2.       DB2Tools.       GetDataProvider();
            //LinqToDB.DataProvider.Informix.  InformixTools.  GetDataProvider();
            //LinqToDB.DataProvider.SapHana.   SapHanaTools.   GetDataProvider(); 

			
		}



		static readonly ConcurrentDictionary<string,IDataProvider> _dataProviders =
			new ConcurrentDictionary<string,IDataProvider>();

		public static void AddDataProvider([JetBrains.Annotations.NotNull] string providerName, [JetBrains.Annotations.NotNull] IDataProvider dataProvider)
		{
			if (providerName == null) throw new ArgumentNullException("providerName");
			if (dataProvider == null) throw new ArgumentNullException("dataProvider");

			if (string.IsNullOrEmpty(dataProvider.Name))
				throw new ArgumentException("dataProvider.Name cannot be empty.", "dataProvider");

			_dataProviders[providerName] = dataProvider;
		}

		public static void AddDataProvider([JetBrains.Annotations.NotNull] IDataProvider dataProvider)
		{
			if (dataProvider == null) throw new ArgumentNullException("dataProvider");

			AddDataProvider(dataProvider.Name, dataProvider);
		}


		#region Command

		public string LastQuery;

		internal void InitCommand(CommandType commandType, string sql, DataParameter[] parameters, List<string> queryHints)
		{
            
            if (queryHints != null && queryHints.Count > 0)
            {
                var sqlProvider = DataProvider.CreateSqlBuilder();
                sql = sqlProvider.ApplyQueryHints(sql, queryHints);
                queryHints.Clear();
            }
		    LastQuery = sql;
		    //DataProvider.InitCommand(this, commandType, sql, parameters);
		    //LastQuery = Command.CommandText;


		}

		private int? _commandTimeout;
        /// <summary>
        /// 设置0代表采用默认的
        /// </summary>
		public  int   CommandTimeout
		{
			get { return _commandTimeout ?? 0; }
			set { _commandTimeout = value;     }
		}

		


       

		internal int ExecuteNonQuery(string sqlString, Dictionary<string, CustomerParam> Params)
		{

            var dic = new Dictionary<string, object>();
		    if (this.CommandTimeout > 0)
		    {
                dic.Add("TIMEOUT", this.CommandTimeout);
		    }
            var result = CustomerExecuteNonQuery(ConnectionString, sqlString, Params, dic);
            if (OnCustomerTraceConnection!=null)
		    {
		        OnCustomerTraceConnection(new CustomerTraceInfo
		        {
                    CustomerParams = Params,
                    SqlText = sqlString
		        });
		    }
            this.Dispose();
            return result;
		}

		object ExecuteScalar(string sqlString, Dictionary<string, CustomerParam> Params)
		{
            var dic = new Dictionary<string, object>();
            if (this.CommandTimeout > 0)
            {
                dic.Add("TIMEOUT", this.CommandTimeout);
            }
            var result = CustomerExecuteScalar(ConnectionString, sqlString, Params, dic);
            if (OnCustomerTraceConnection != null)
            {
                OnCustomerTraceConnection(new CustomerTraceInfo
                {
                    CustomerParams = Params,
                    SqlText = sqlString
                });
            }
            this.Dispose();
            return result;
		}

		internal IDataReader ExecuteReader(string sqlString, Dictionary<string, CustomerParam> Params)
		{
            var dic = new Dictionary<string, object>();
            if (this.CommandTimeout > 0)
            {
                dic.Add("TIMEOUT", this.CommandTimeout);
            }
            var result =  CustomerExecuteQuery(ConnectionString,sqlString, Params,dic);
            if (OnCustomerTraceConnection != null)
            {
                OnCustomerTraceConnection(new CustomerTraceInfo
                {
                    CustomerParams = Params,
                    SqlText = sqlString
                });
            }
            this.Dispose();
		    return result;
		}

		internal IDataReader ExecuteReader(CommandBehavior commandBehavior, string sqlString, Dictionary<string, CustomerParam> Params)
		{
            var dic = new Dictionary<string, object>();
            if (this.CommandTimeout > 0)
            {
                dic.Add("TIMEOUT", this.CommandTimeout);
            }
            var result = CustomerExecuteQuery(ConnectionString, sqlString, Params,dic);
            if (OnCustomerTraceConnection != null)
            {
                OnCustomerTraceConnection(new CustomerTraceInfo
                {
                    CustomerParams = Params,
                    SqlText = sqlString
                });
            }
            this.Dispose();
            return result;
		}

        internal DataTable ExecuteDataTable(CommandBehavior commandBehavior, string sqlString, Dictionary<string, CustomerParam> Params)
        {
            var dic = new Dictionary<string, object>();
            if (this.CommandTimeout > 0)
            {
                dic.Add("TIMEOUT", this.CommandTimeout);
            }
            var result = CustomerExecuteQueryTable(ConnectionString, sqlString, Params, dic);
            if (OnCustomerTraceConnection != null)
            {
                OnCustomerTraceConnection(new CustomerTraceInfo
                {
                    CustomerParams = Params,
                    SqlText = sqlString
                });
            }
            this.Dispose();
            return result;
        }
		

		#endregion


		#region MappingSchema

		private MappingSchema _mappingSchema;

		public  MappingSchema  MappingSchema
		{
			get { return _mappingSchema; }
		}

		public bool InlineParameters { get; set; }

		private List<string> _queryHints;
		public  List<string>  QueryHints
		{
			get { return _queryHints ?? (_queryHints = new List<string>()); }
		}

		private List<string> _nextQueryHints;
		public  List<string>  NextQueryHints
		{
			get { return _nextQueryHints ?? (_nextQueryHints = new List<string>()); }
		}

	    public event EventHandler OnClosing;

	    public DataConnection AddMappingSchema(MappingSchema mappingSchema)
		{
			_mappingSchema = new MappingSchema(mappingSchema, _mappingSchema);
			_id            = null;

			return this;
		}

		#endregion

		

		#region System.IDisposable Members

		public void Dispose()
		{
		    this.LastQuery = string.Empty;
		}

		#endregion
	}
}
