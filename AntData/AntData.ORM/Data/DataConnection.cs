using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using AntData.ORM.DbEngine.Configuration;
using AntData.ORM.Dao;
using AntData.ORM.Dao.Common;
using AntData.ORM.DbEngine.Connection;
using JetBrains.Annotations;

namespace AntData.ORM.Data
{
	using System.Text;

	using Common;
	using DataProvider;

	using Mapping;

	public partial class DataConnection
	{
        static DataConnection()
        {
            _configurationIDs = new ConcurrentDictionary<string, int>();
            AntData.ORM.DataProvider.SqlServer.SqlServerTools.GetDataProvider();
            AntData.ORM.DataProvider.MySql.MySqlTools.GetDataProvider();
            AntData.ORM.DataProvider.Oracle.OracleTools.GetDataProvider();
        }


        #region  构造方法
        public DataConnection(string dbMappingName)
        {
            ConnectionString = dbMappingName;

            #region 默认实现
            this.CustomerExecuteNonQuery = DalBridge.CustomerExecuteNonQuery;
            this.CustomerExecuteScalar = DalBridge.CustomerExecuteScalar;
            this.CustomerExecuteQuery = DalBridge.CustomerExecuteQuery;
            this.CustomerExecuteQueryTable = DalBridge.CustomerExecuteQueryTable;
            this.CustomerBeginTransaction = DalBridge.CustomerBeginTransaction;

            #endregion
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dataProvider">设置数据库的信息</param>
        /// <param name="dbMappingName">逻辑数据库名称</param>
        public DataConnection([JetBrains.Annotations.NotNull] IDataProvider dataProvider, string dbMappingName)
	    {
            if (dataProvider == null) throw new ArgumentNullException("dataProvider");
            AddDataProvider(dataProvider);
            DataProvider = dataProvider;
            _mappingSchema = DataProvider.MappingSchema;
            ConnectionString = dbMappingName;

            #region 默认实现
            this.CustomerExecuteNonQuery = DalBridge.CustomerExecuteNonQuery;
            this.CustomerExecuteScalar = DalBridge.CustomerExecuteScalar;
            this.CustomerExecuteQuery = DalBridge.CustomerExecuteQuery;
            this.CustomerExecuteQueryTable = DalBridge.CustomerExecuteQueryTable; 
            this.CustomerBeginTransaction = DalBridge.CustomerBeginTransaction; 
            #endregion
        }


	    /// <summary>
	    /// 可扩展的构造函数 可以自己实现DalBridge的四个方法 然后注入进来
	    /// </summary>
	    /// <param name="dataProvider">设置数据库的信息</param>
	    /// <param name="dbMappingName">逻辑数据库名称</param>
	    /// <param name="CustomerExecuteNonQuery">执行insert update delete 语句(不包括insertWithIdentity)</param>
	    /// <param name="CustomerExecuteScalar">执行查询单个信息(包括insertWithIdentity)</param>
	    /// <param name="CustomerExecuteQuery">执行select 序列化成对象 </param>
	    /// <param name="CustomerExecuteQueryTable">执行select 不走序列化 生成DataTable</param>
	    /// <param name="CustomerBeginTransaction">执行事物</param>
	    public DataConnection([JetBrains.Annotations.NotNull] IDataProvider dataProvider, string dbMappingName, Func<string, string, Dictionary<string, CustomerParam>, IDictionary,bool, int> CustomerExecuteNonQuery, Func<string, string, Dictionary<string, CustomerParam>, IDictionary, bool, object> CustomerExecuteScalar, Func<string, string, Dictionary<string, CustomerParam>, IDictionary, bool, List<IDataReader>> CustomerExecuteQuery, Func<string, string, Dictionary<string, CustomerParam>, IDictionary, bool, DataTable> CustomerExecuteQueryTable, Func<string, IDictionary, DataConnectionTransaction> CustomerBeginTransaction)
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
            this.CustomerBeginTransaction = CustomerBeginTransaction;
        }
        #endregion

        #region Public Properties

        public static DBSettings DefaultSettings { get; set; }
        /// <summary>
        /// 逻辑数据库名称
        /// </summary>
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


        /// <summary>
        /// 针对sqlserver
        /// </summary>
	    public bool IsNoLock { get; set; }
        #endregion


        #region 执行sql后显示Trace

        //是否开启打sql日志
	    public bool IsEnableLogTrace { get; set; }

        /// <summary>
        /// 打日志方法
        /// </summary>
        public virtual Action<CustomerTraceInfo> OnLogTrace { get; set; }


        #endregion

	   


	    #region 执行sql操作
        /// <summary>
        /// 执行insert update delete 语句(不包括insertWithIdentity)
        /// </summary>
        private Func<string, string, Dictionary<string, CustomerParam>, IDictionary, bool, int> CustomerExecuteNonQuery { get; set; }

        /// <summary>
        /// 执行查询单个信息(包括insertWithIdentity)
        /// </summary>
        private Func<string, string, Dictionary<string, CustomerParam>, IDictionary, bool, object> CustomerExecuteScalar { get; set; }

        /// <summary>
        /// 执行select 序列化成对象
        /// </summary>
        private Func<string, string, Dictionary<string, CustomerParam>, IDictionary, bool, IList<IDataReader>> CustomerExecuteQuery { get; set; }

        /// <summary>
        /// 执行select 不走序列化 生成DataTable
        /// </summary>
        private Func<string, string, Dictionary<string, CustomerParam>, IDictionary, bool, DataTable> CustomerExecuteQueryTable { get; set; }

        /// <summary>
        /// 事物
        /// </summary>
        private Func<string, IDictionary, DataConnectionTransaction> CustomerBeginTransaction { get; set; }


        #endregion







        #region 数据库引擎

        /// <summary>
        /// 数据库引擎集合
        /// </summary>
        static readonly ConcurrentDictionary<string, IDataProvider> _dataProviders = new ConcurrentDictionary<string, IDataProvider>();

	    /// <summary>
	    /// 设置数据库的信息
	    /// 在生成表达式树和SQL语句之前，我们有必要知道数据库的相关信息。比如当前数据库是用什么——Sql Server还是MySql
	    /// </summary>
	    private IDataProvider _dataProvider;
        internal IDataProvider DataProvider {
            get
            {
                return _dataProvider;
            }
            set
            {
                _dataProvider = value;
                _mappingSchema = _dataProvider.MappingSchema;
                AddDataProvider(_dataProvider);
            } 
        }

        /// <summary>
        /// 获取默认的数据库的信息
        /// </summary>
        /// <param name="providerName">名称</param>
        /// <returns></returns>
        public static IDataProvider GetDataProvider([JetBrains.Annotations.NotNull] string providerName)
        {
            return _dataProviders[providerName];
        }

        /// <summary>
        /// 添加数据库信息
        /// </summary>
        /// <param name="providerName">名称</param>
        /// <param name="dataProvider">类型</param>
        public static void AddDataProvider([JetBrains.Annotations.NotNull] string providerName, [JetBrains.Annotations.NotNull] IDataProvider dataProvider)
        {
            if (providerName == null) throw new ArgumentNullException("providerName");
            if (dataProvider == null) throw new ArgumentNullException("dataProvider");

            if (string.IsNullOrEmpty(dataProvider.Name))
                throw new ArgumentException("dataProvider.Name cannot be empty.", "dataProvider");

            _dataProviders[providerName] = dataProvider;
        }

        /// <summary>
        /// 添加数据库信息
        /// </summary>
        /// <param name="dataProvider">类型</param>
        public static void AddDataProvider([JetBrains.Annotations.NotNull] IDataProvider dataProvider)
        {
            if (dataProvider == null) throw new ArgumentNullException("dataProvider");

            AddDataProvider(dataProvider.Name, dataProvider);
        } 
        #endregion


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




        /// <summary>
        /// 执行ExecuteNonQuery
        /// </summary>
        /// <param name="sqlString">执行sql</param>
        /// <param name="Params">执行参数</param>
        /// <param name="isWrite">默认写</param>
        /// <returns></returns>
        internal int ExecuteNonQuery(string sqlString, Dictionary<string, CustomerParam> Params,bool isWrite = true)
		{

            var dic = new Dictionary<string, object>();
		    if (this.CommandTimeout > 0)
		    {
                dic.Add("TIMEOUT", this.CommandTimeout);
		    }
		    if (sqlString.StartsWith(DALExtStatementConstant.BULK_COPY))
		    {
                sqlString = sqlString.Substring(DALExtStatementConstant.BULK_COPY.Length);
                dic.Add(DALExtStatementConstant.BULK_COPY, true);
            }else if (sqlString.StartsWith(DALExtStatementConstant.BULK_COPY_ORACLE))
		    {
                sqlString = sqlString.Substring(DALExtStatementConstant.BULK_COPY_ORACLE.Length);
                dic.Add(DALExtStatementConstant.BULK_COPY_ORACLE, true);

            }
            if (OnLogTrace != null && IsEnableLogTrace)
            {
                OnLogTrace(new CustomerTraceInfo
                {
                    CustomerParams = Params,
                    SqlText = sqlString
                });
            }
            dic.Add(DALExtStatementConstant.PARAMETER_SYMBOL, DataProvider.ParameterSymbol);
            dic.Add(DALExtStatementConstant.TRANSACTION_CONNECTION, ConnectionWrapper);
            var result = CustomerExecuteNonQuery(ConnectionString, sqlString, Params, dic,  isWrite);
            this.Dispose();
            return result;
		}

        /// <summary>
        /// 执行ExecuteScalar
        /// </summary>
        /// <param name="sqlString">执行sql</param>
        /// <param name="Params">执行参数</param>
        /// <param name="isWrite">默认读</param>
        /// <returns></returns>
		object ExecuteScalar(string sqlString, Dictionary<string, CustomerParam> Params, bool isWrite = false)
		{
            var dic = new Dictionary<string, object>();
            if (this.CommandTimeout > 0)
            {
                dic.Add("TIMEOUT", this.CommandTimeout);
            }
            if (OnLogTrace != null && IsEnableLogTrace)
            {
                OnLogTrace(new CustomerTraceInfo
                {
                    CustomerParams = Params,
                    SqlText = sqlString
                });
            }
            dic.Add(DALExtStatementConstant.PARAMETER_SYMBOL, DataProvider.ParameterSymbol);
            dic.Add(DALExtStatementConstant.TRANSACTION_CONNECTION, ConnectionWrapper);
            var result = CustomerExecuteScalar(ConnectionString, sqlString, Params, dic,  isWrite);
            this.Dispose();
            return result;
		}

        /// <summary>
        /// 执行ExecuteReader
        /// </summary>
        /// <param name="sqlString">执行sql</param>
        /// <param name="Params">执行参数</param>
        /// <param name="isWrite">默认读</param>
        /// <returns></returns>
		internal IList<IDataReader> ExecuteReader(string sqlString, Dictionary<string, CustomerParam> Params, bool isWrite = false)
		{
            var dic = new Dictionary<string, object>();
            if (this.CommandTimeout > 0)
            {
                dic.Add("TIMEOUT", this.CommandTimeout);
            }

            if (OnLogTrace != null && IsEnableLogTrace)
            {
                OnLogTrace(new CustomerTraceInfo
                {
                    CustomerParams = Params,
                    SqlText = sqlString
                });
            }
            dic.Add(DALExtStatementConstant.PARAMETER_SYMBOL, DataProvider.ParameterSymbol);
            dic.Add(DALExtStatementConstant.TRANSACTION_CONNECTION, ConnectionWrapper);
            var result =  CustomerExecuteQuery(ConnectionString,sqlString, Params,dic,  isWrite);
            this.Dispose();
		    return result;
		}

	   
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sqlString">执行sql</param>
        /// <param name="Params">执行参数</param>
        /// <param name="isWrite">默认读</param>
        /// <returns></returns>
        internal DataTable ExecuteDataTable(string sqlString, Dictionary<string, CustomerParam> Params, bool isWrite = false)
        {
            var dic = new Dictionary<string, object>();
            if (this.CommandTimeout > 0)
            {
                dic.Add("TIMEOUT", this.CommandTimeout);
            }

            if (OnLogTrace != null && IsEnableLogTrace)
            {
                OnLogTrace(new CustomerTraceInfo
                {
                    CustomerParams = Params,
                    SqlText = sqlString
                });
                
            }
            dic.Add(DALExtStatementConstant.PARAMETER_SYMBOL, DataProvider.ParameterSymbol);
            dic.Add(DALExtStatementConstant.TRANSACTION_CONNECTION, ConnectionWrapper);
            var result = CustomerExecuteQueryTable(ConnectionString, sqlString, Params, dic,  isWrite);
            
            this.Dispose();
            return result;
        }
        internal ConnectionWrapper ConnectionWrapper { get; private set; }
        internal DataConnectionTransaction ExecuteTransaction()
        {
            var dic = new Dictionary<string, object>();
            dic.Add(DALExtStatementConstant.PARAMETER_SYMBOL, DataProvider.ParameterSymbol);
            var result = CustomerBeginTransaction(ConnectionString, dic);
            ConnectionWrapper = result.DataConnection;
            return result;
        }
        internal DataConnectionTransaction ExecuteTransaction(IsolationLevel isolationLevel)
        {
            var dic = new Dictionary<string, object>();
            dic.Add(DALExtStatementConstant.PARAMETER_SYMBOL, DataProvider.ParameterSymbol);
            dic.Add(DALExtStatementConstant.ISOLATION_LEVEL, isolationLevel);
            var result = CustomerBeginTransaction(ConnectionString, dic);
            ConnectionWrapper = result.DataConnection;
            return result;
        }
        #endregion


        #region MappingSchema 转sql

        private MappingSchema _mappingSchema;

		public  MappingSchema  MappingSchema
		{
		    get
		    {
		        return _mappingSchema;
		    }
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

	


		#endregion

		

		#region System.IDisposable Members

		public void Dispose()
		{
            //注释下面的代码 因为会导致query方法的序列化缓存bug
		    //this.LastQuery = string.Empty;
		}
        //public void Close()
        //{
        //    if (ConnectionWrapper != null)
        //    {
        //        ConnectionWrapper = null;
        //    }
        //}
        #endregion



    }
}
