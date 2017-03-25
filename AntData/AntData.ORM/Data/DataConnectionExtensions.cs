using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AntData.ORM.Mapping;
using JetBrains.Annotations;

namespace AntData.ORM.Data
{
	using Linq;

	[PublicAPI]
	public static class DataConnectionExtensions
	{



        #region Query with object reader

        /// <summary>
        /// 执行sql返回数据使用自定义序列化Func
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connection"></param>
        /// <param name="objectReader">自定义序列化Func</param>
        /// <param name="sql">sql语句</param>
        /// <returns></returns>
        public static IEnumerable<T> Query<T>(this DataConnection connection, Func<IDataReader,T> objectReader, string sql)
		{
			return new CommandInfo(connection, sql).Query(objectReader);
		}

        /// <summary>
        /// 带有参数的执行存储过程返回数据使用自定义序列化Func
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connection"></param>
        /// <param name="objectReader">自定义序列化Func</param>
        /// <param name="sql">存储过程</param>
        /// <param name="parameters">参数集合 Array</param>
        /// <returns></returns>
		public static IEnumerable<T> QueryProc<T>(this DataConnection connection, Func<IDataReader,T> objectReader, string sql, params DataParameter[] parameters)
		{
			return new CommandInfo(connection, sql, parameters).QueryProc(objectReader);
		}

        /// <summary>
        /// 带有参数的执行sql返回数据使用自定义序列化Func
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connection"></param>
        /// <param name="objectReader">自定义序列化Func</param>
        /// <param name="sql">sql语句</param>
        /// <param name="parameters">参数集合 Array</param>
        /// <returns></returns>
		public static IEnumerable<T> Query<T>(this DataConnection connection, Func<IDataReader,T> objectReader, string sql, params DataParameter[] parameters)
		{
			return new CommandInfo(connection, sql, parameters).Query(objectReader);
		}

        /// <summary>
        /// 带有匿名参数的执行sql返回数据使用自定义序列化Func
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connection"></param>
        /// <param name="objectReader">自定义序列化Func</param>
        /// <param name="sql">sql语句</param>
        /// <param name="parameters">参数集合 Array</param>
        /// <returns></returns>
		public static IEnumerable<T> Query<T>(this DataConnection connection, Func<IDataReader,T> objectReader, string sql, object parameters)
		{
			return new CommandInfo(connection, sql, parameters).Query(objectReader);
		}

        #endregion

        #region Query

        /// <summary>
        /// 根据sql语句查询 返回结果集
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connection"></param>
        /// <param name="sql">sql语句</param>
        /// <returns>返回IEnumerable</returns>
        public static IEnumerable<T> Query<T>(this DataConnection connection, string sql)
		{
			return new CommandInfo(connection, sql).Query<T>();
		}

        /// <summary>
        /// 带有参数的sql语句查询 返回结果集
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connection"></param>
        /// <param name="sql">sql语句</param>
        /// <param name="parameters">参数集合 Array</param>
        /// <returns></returns>
		public static IEnumerable<T> Query<T>(this DataConnection connection, string sql, params DataParameter[] parameters)
		{
			return new CommandInfo(connection, sql, parameters).Query<T>();
		}

        /// <summary>
        /// 根据sql语句查询 返回DataTable
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="sql">sql语句</param>
        /// <returns>返回一个DataTable</returns>
        public static DataTable QueryTable(this DataConnection connection, string sql)
        {
            return new CommandInfo(connection, sql).QueryTable();
        }

        /// <summary>
        /// 带有参数的sql语句查询 返回DataTable
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="sql">sql语句</param>
        /// <param name="parameters">参数集合 Array</param>
        /// <returns>返回一个DataTable</returns>
        public static DataTable QueryTable(this DataConnection connection, string sql, params DataParameter[] parameters)
        {
            return new CommandInfo(connection, sql, parameters).QueryTable();
        }

        /// <summary>
        /// 带有参数的sql语句查询 异步 返回DataTable
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="sql">sql语句</param>
        /// <returns>Task<DataTable></returns>
        public static async Task<DataTable> QueryTableAsync(this DataConnection connection, string sql)
        {
            return await new CommandInfo(connection, sql).QueryTableAsync();
        }

        /// <summary>
        /// 带有参数的sql语句查询 异步 返回DataTable
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="sql">sql语句</param>
        /// <param name="parameters">参数集合 Array</param>
        /// <returns>Task<DataTable></returns>
        public static async Task<DataTable> QueryTableAsync(this DataConnection connection, string sql, params DataParameter[] parameters)
        {
            return await new CommandInfo(connection, sql, parameters).QueryTableAsync();
        }

        /// <summary>
        /// 执行一个存储过程 返回结果集
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connection"></param>
        /// <param name="sql"></param>
        /// <param name="parameters">参数集合 Array</param>
        /// <returns>IEnumerable</returns>
        public static IEnumerable<T> QueryProc<T>(this DataConnection connection, string sql, params DataParameter[] parameters)
		{
			return new CommandInfo(connection, sql, parameters).QueryProc<T>();
		}

        /// <summary>
        /// 执行带有参数的存储过程  返回结果集
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connection"></param>
        /// <param name="sql"></param>
        /// <param name="parameter">单个DataParameter对象</param>
        /// <returns>IEnumerable</returns>
		public static IEnumerable<T> Query<T>(this DataConnection connection, string sql, DataParameter parameter)
		{
			return new CommandInfo(connection, sql, parameter).Query<T>();
		}

        /// <summary>
        /// 执行带有匿名对象参数的sql语句  返回结果集
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connection"></param>
        /// <param name="sql"></param>
        /// <param name="parameters">匿名对象</param>
        /// <returns>IEnumerable</returns>
		public static IEnumerable<T> Query<T>(this DataConnection connection, string sql, object parameters)
		{
			return new CommandInfo(connection, sql, parameters).Query<T>();
		}

        #endregion

        #region Query with template

        /// <summary>
        /// 带有模板的执行sql语句返回结果集
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connection"></param>
        /// <param name="template">模板 可以是一个匿名对象</param>
        /// <param name="sql">sql语句</param>
        /// <param name="parameters">参数集合 Array</param>
        /// <returns>IEnumerable</returns>
        public static IEnumerable<T> Query<T>(this DataConnection connection, T template, string sql, params DataParameter[] parameters)
		{
			return new CommandInfo(connection, sql, parameters).Query(template);
		}

        /// <summary>
        /// 带有模板的执行sql语句返回结果集
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connection"></param>
        /// <param name="template">模板 可以是一个匿名对象 或者一个 class</param>
        /// <param name="sql">sql语句</param>
        /// <param name="parameters">匿名对象</param>
        /// <returns>IEnumerable</returns>
		public static IEnumerable<T> Query<T>(this DataConnection connection, T template, string sql, object parameters)
		{
			return new CommandInfo(connection, sql, parameters).Query(template);
		}

        #endregion

        #region Query SQL
        /// <summary>
        /// 执行SQL对象返回DataTable
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="sql">SQL对象</param>
        /// <returns>DataTable</returns>
        public static DataTable QueryTable(this DataConnection connection, SQL sql)
        {
            return new CommandInfo(connection, sql.ToString(), sql.Parameters.ToArray()).QueryTable();
        }

        /// <summary>
        /// 异步 执行SQL对象返回DataTable
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="sql">SQL对象</param>
        /// <returns>Task<DataTable></returns>
        public static async Task<DataTable> QueryTableAsync(this DataConnection connection, SQL sql)
        {
            return await new CommandInfo(connection, sql.ToString(), sql.Parameters.ToArray()).QueryTableAsync();
        }

        /// <summary>
        /// 执行SQL对象返回结果集
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connection"></param>
        /// <param name="sql">SQL对象</param>
        /// <returns>IEnumerable</returns>
        public static IEnumerable<T> Query<T>(this DataConnection connection, SQL sql)
        {
            return new CommandInfo(connection, sql.ToString(), sql.Parameters.ToArray()).Query<T>();
        }

        /// <summary>
        /// 执行带有模板的SQL对象返回结果集
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connection"></param>
        /// <param name="template">模板 可以是一个匿名对象或者是一个class</param>
        /// <param name="sql">SQL对象</param>
        /// <returns>IEnumerable</returns>
        public static IEnumerable<T> Query<T>(this DataConnection connection, T template, SQL sql)
        {
            return new CommandInfo(connection, sql.ToString(), sql.Parameters.ToArray()).Query(template);
        }

        /// <summary>
        /// 执行SQL对象返回单个结果 例如查询select count(*)等返回单个结果是int类型的查询
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="sql">SQL对象</param>
        /// <returns>int</returns>
        public static int Execute(this DataConnection connection, SQL sql)
        {
            return new CommandInfo(connection, sql.ToString(), sql.Parameters.ToArray()).Execute();
        }

        /// <summary>
        /// 执行SQL对象返回单个数据 例如查询select count(*)等返回单个结果是类型的查询
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connection"></param>
        /// <param name="sql">SQL对象</param>
        /// <returns></returns>
        public static T Execute<T>(this DataConnection connection, SQL sql)
        {
            return new CommandInfo(connection, sql.ToString(), sql.Parameters.ToArray()).Execute<T>();
        }

        /// <summary>
        /// 查询分库分表的总数 要求sql是返回数字类型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connection"></param>
        /// <param name="sql">SQL对象</param>
        /// <returns></returns>
        public static long ExcuteSharding(this DataConnection connection, SQL sql)
        {
            return new CommandInfo(connection, sql.ToString(), sql.Parameters.ToArray()).ExcuteSharding();
        }
        #endregion

        #region Execute
        /// <summary>
        /// 执行sql语句返回单个结果 例如查询select count(*)等返回单个结果是int类型的查询
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="sql">sql语句</param>
        /// <returns>int</returns>
        public static int Execute(this DataConnection connection, string sql)
		{
			return new CommandInfo(connection, sql).Execute();
		}

        public static long ExcuteSharding(this DataConnection connection, string sql)
        {
            return new CommandInfo(connection, sql).ExcuteSharding();
        }

        /// <summary>
        /// 带参数执行sql语句返回单个结果 例如查询select count(*)等返回单个结果是int类型的查询
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="sql">sql语句</param>
        /// <param name="parameters">参数集合 Array</param>
        /// <returns>int</returns>
		public static int Execute(this DataConnection connection, string sql, params DataParameter[] parameters)
		{
			return new CommandInfo(connection, sql, parameters).Execute();
		}
        public static long ExcuteSharding(this DataConnection connection, string sql, params DataParameter[] parameters)
        {
            return new CommandInfo(connection, sql, parameters).ExcuteSharding();
        }
        /// <summary>
        /// 带参数执行存储过程返回单个结果 例如查询select count(*)等返回单个结果是int类型的查询
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="sql">存储过程</param>
        /// <param name="parameters">参数集合 Array</param>
        /// <returns>int</returns>
		public static int ExecuteProc(this DataConnection connection, string sql, params DataParameter[] parameters)
		{
			return new CommandInfo(connection, sql, parameters).ExecuteProc();
		}

        /// <summary>
        /// 带匿名参数执行sql语句返回单个结果 例如查询select count(*)等返回单个结果是int类型的查询
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="sql">sql语句</param>
        /// <param name="parameters">参数集合 Array</param>
        /// <returns></returns>
		public static int Execute(this DataConnection connection, string sql, object parameters)
		{
			return new CommandInfo(connection, sql, parameters).Execute();
		}
        public static long ExcuteSharding(this DataConnection connection, string sql, object parameters)
        {
            return new CommandInfo(connection, sql, parameters).ExcuteSharding();
        }
        #endregion

        #region Execute scalar
        /// <summary>
        /// 执行sql语句返回单个结果
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connection"></param>
        /// <param name="sql">sql语句</param>
        /// <returns></returns>

        public static T Execute<T>(this DataConnection connection, string sql)
		{
			return new CommandInfo(connection, sql).Execute<T>();
		}
        
        /// <summary>
        /// 带参数执行sql语句返回单个结果
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connection"></param>
        /// <param name="sql">sql语句</param>
        /// <param name="parameters">参数集合 Array</param>
        /// <returns></returns>
		public static T Execute<T>(this DataConnection connection, string sql, params DataParameter[] parameters)
		{
			return new CommandInfo(connection, sql, parameters).Execute<T>();
		}

        /// <summary>
        /// 带一个参数执行sql语句返回单个结果
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connection"></param>
        /// <param name="sql">sql语句</param>
        /// <param name="parameter">单个参数</param>
        /// <returns></returns>
		public static T Execute<T>(this DataConnection connection, string sql, DataParameter parameter)
		{
			return new CommandInfo(connection, sql, parameter).Execute<T>();
		}

        /// <summary>
        /// 带匿名参数执行sql语句返回单个结果
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connection"></param>
        /// <param name="sql">sql语句</param>
        /// <param name="parameters">匿名对象</param>
        /// <returns></returns>
		public static T Execute<T>(this DataConnection connection, string sql, object parameters)
		{
			return new CommandInfo(connection, sql, parameters).Execute<T>();
		}

		#endregion

		#region ExecuteReader
        /// <summary>
        /// 执行sql语句返回DataReader
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="sql">sql语句</param>
        /// <returns></returns>
		public static DataReader ExecuteReader(this DataConnection connection, string sql)
		{
			return new CommandInfo(connection, sql).ExecuteReader();
		}

        /// <summary>
        /// 带参数执行sql语句返回DataReader
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="sql">sql语句</param>
        /// <param name="parameters">参数集合 Array</param>
        /// <returns>DataReader</returns>
		public static DataReader ExecuteReader(this DataConnection connection, string sql, params DataParameter[] parameters)
		{
			return new CommandInfo(connection, sql, parameters).ExecuteReader();
		}

        /// <summary>
        ///  带一个参数执行sql语句返回DataReader
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="sql">sql语句</param>
        /// <param name="parameter">单个参数</param>
        /// <returns></returns>
        public static DataReader ExecuteReader(this DataConnection connection, string sql, DataParameter parameter)
		{
			return new CommandInfo(connection, sql, parameter).ExecuteReader();
		}

        /// <summary>
        /// 带匿名参数执行sql语句返回DataReader
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connection"></param>
        /// <param name="sql">sql语句</param>
        /// <param name="parameters">匿名对象</param>
        /// <returns>DataReader</returns>
		public static DataReader ExecuteReader(this DataConnection connection, string sql, object parameters)
		{
			return new CommandInfo(connection, sql, parameters).ExecuteReader();
		}


		//public static DataReader ExecuteReader(
		//	this DataConnection    connection,
		//	string                 sql,
		//	CommandType            commandType,
		//	CommandBehavior        commandBehavior,
		//	params DataParameter[] parameters)
		//{
		//	return new CommandInfo(connection, sql, parameters)
		//	{
		//		CommandType     = commandType,
		//	}.ExecuteReader();
		//}

		#endregion

		#region BulkCopy

       /// <summary>
       /// 带设置条件的去批量插入
       /// </summary>
       /// <typeparam name="T"></typeparam>
       /// <param name="dataConnection"></param>
       /// <param name="options">设置条件</param>
       /// <param name="source">插入对象的集合</param>
       /// <returns></returns>
		public static BulkCopyRowsCopied BulkCopy<T>([JetBrains.Annotations.NotNull] this DataConnection dataConnection, BulkCopyOptions options, IEnumerable<T> source)
		{
			if (dataConnection == null) throw new ArgumentNullException("dataConnection");
			return dataConnection.DataProvider.BulkCopy(dataConnection, options, source);
		}

        /// <summary>
        /// 可设置每批次批量数量，进行批量插入
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dataConnection"></param>
        /// <param name="maxBatchSize">每批次插入数量</param>
        /// <param name="source">插入对象的集合</param>
        /// <returns></returns>
		public static BulkCopyRowsCopied BulkCopy<T>([JetBrains.Annotations.NotNull] this DataConnection dataConnection, int maxBatchSize, IEnumerable<T> source)
		{
			if (dataConnection == null) throw new ArgumentNullException("dataConnection");

			return dataConnection.DataProvider.BulkCopy(
				dataConnection,
				new BulkCopyOptions { MaxBatchSize = maxBatchSize },
				source);
		}

        /// <summary>
        /// 批量插入
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dataConnection"></param>
        /// <param name="source">插入对象的集合</param>
        /// <returns></returns>
		public static BulkCopyRowsCopied BulkCopy<T>([JetBrains.Annotations.NotNull] this DataConnection dataConnection, IEnumerable<T> source)
		{
			if (dataConnection == null) throw new ArgumentNullException("dataConnection");

			return dataConnection.DataProvider.BulkCopy(
				dataConnection,
				new BulkCopyOptions(),
				source);
		}

        /// <summary>
        /// 批量插入
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="table"></param>
        /// <param name="options"></param>
        /// <param name="source">插入对象的集合</param>
        /// <returns></returns>
		public static BulkCopyRowsCopied BulkCopy<T>([JetBrains.Annotations.NotNull] this ITable<T> table, BulkCopyOptions options, IEnumerable<T> source)
		{
			if (table == null) throw new ArgumentNullException("table");

			var tbl            = (Table<T>)table;
			var dataConnection = tbl.DataContextInfo.DataContext as DataConnection;

			if (dataConnection == null)
				throw new ArgumentException("DataContext must be of DataConnection type.");

			if (options.TableName    == null) options.TableName    = tbl.TableName;
			if (options.DatabaseName == null) options.DatabaseName = tbl.DatabaseName;
			if (options.SchemaName   == null) options.SchemaName   = tbl.SchemaName;

			return dataConnection.DataProvider.BulkCopy(dataConnection, options, source);
		}

        /// <summary>
        /// 可设置每批次批量数量，进行批量插入
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="table"></param>
        /// <param name="maxBatchSize">每批次插入数量</param>
        /// <param name="source">插入对象的集合</param>
        /// <returns></returns>
        public static BulkCopyRowsCopied BulkCopy<T>(this ITable<T> table, int maxBatchSize, IEnumerable<T> source)
		{
			if (table == null) throw new ArgumentNullException("table");

			var tbl            = (Table<T>)table;
			var dataConnection = tbl.DataContextInfo.DataContext as DataConnection;

			if (dataConnection == null)
				throw new ArgumentException("DataContext must be of DataConnection type.");

			return dataConnection.DataProvider.BulkCopy(
				dataConnection,
				new BulkCopyOptions
				{
					MaxBatchSize = maxBatchSize,
					TableName    = tbl.TableName,
					DatabaseName = tbl.DatabaseName,
					SchemaName   = tbl.SchemaName,
				},
				source);
		}


        /// <summary>
        /// 批量插入
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="table"></param>
        /// <param name="source">插入对象的集合</param>
        /// <returns></returns>
		public static BulkCopyRowsCopied BulkCopy<T>(this ITable<T> table, IEnumerable<T> source)
		{
			if (table == null) throw new ArgumentNullException("table");

			var tbl            = (Table<T>)table;
			var dataConnection = tbl.DataContextInfo.DataContext as DataConnection;

			if (dataConnection == null)
				throw new ArgumentException("DataContext must be of DataConnection type.");

			return dataConnection.DataProvider.BulkCopy(
				dataConnection,
				new BulkCopyOptions
				{
					TableName    = tbl.TableName,
					DatabaseName = tbl.DatabaseName,
					SchemaName   = tbl.SchemaName,
				},
				source);
		}

        #endregion

        #region Merge http://www.cnblogs.com/CareySon/archive/2012/03/07/2383690.html

        /// <summary>
        /// sqlserver下可以使用
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dataConnection"></param>
        /// <param name="source"></param>
        /// <param name="predicate"></param>
        /// <param name="tableName"></param>
        /// <param name="databaseName"></param>
        /// <param name="schemaName"></param>
        /// <returns></returns>
        public static int Merge<T>(this DataConnection dataConnection, IQueryable<T> source, Expression<Func<T,bool>> predicate,
			string tableName = null, string databaseName = null, string schemaName = null)
			where T : class 
		{
			return dataConnection.DataProvider.Merge(dataConnection, predicate, true, source.Where(predicate), tableName, databaseName, schemaName);
		}

        /// <summary>
        /// sqlserver下可以使用
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dataConnection"></param>
        /// <param name="predicate"></param>
        /// <param name="source"></param>
        /// <param name="tableName"></param>
        /// <param name="databaseName"></param>
        /// <param name="schemaName"></param>
        /// <returns></returns>
		public static int Merge<T>(this DataConnection dataConnection, Expression<Func<T,bool>> predicate, IEnumerable<T> source,
			string tableName = null, string databaseName = null, string schemaName = null)
			where T : class 
		{
			return dataConnection.DataProvider.Merge(dataConnection, predicate, true, source, tableName, databaseName, schemaName);
		}

        /// <summary>
        /// sqlserver下可以使用
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dataConnection"></param>
        /// <param name="delete"></param>
        /// <param name="source"></param>
        /// <param name="tableName"></param>
        /// <param name="databaseName"></param>
        /// <param name="schemaName"></param>
        /// <returns></returns>
		public static int Merge<T>(this DataConnection dataConnection, bool delete, IEnumerable<T> source,
			string tableName = null, string databaseName = null, string schemaName = null)
			where T : class 
		{
			return dataConnection.DataProvider.Merge(dataConnection, null, delete, source, tableName, databaseName, schemaName);
		}

        /// <summary>
        /// sqlserver下可以使用
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dataConnection"></param>
        /// <param name="source"></param>
        /// <param name="tableName"></param>
        /// <param name="databaseName"></param>
        /// <param name="schemaName"></param>
        /// <returns></returns>
		public static int Merge<T>(this DataConnection dataConnection, IEnumerable<T> source,
			string tableName = null, string databaseName = null, string schemaName = null)
			where T : class 
		{
			return dataConnection.DataProvider.Merge(dataConnection, null, false, source, tableName, databaseName, schemaName);
		}

        /// <summary>
        /// sqlserver下可以使用
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="table"></param>
        /// <param name="source"></param>
        /// <param name="predicate"></param>
        /// <param name="tableName"></param>
        /// <param name="databaseName"></param>
        /// <param name="schemaName"></param>
        /// <returns></returns>
		public static int Merge<T>(this IQueryable<T> table, IQueryable<T> source, Expression<Func<T,bool>> predicate,
			string tableName = null, string databaseName = null, string schemaName = null)
			where T : class 
		{
			if (table == null) throw new ArgumentNullException("table");

			var tbl            = (Table<T>)table;
			var dataConnection = tbl.DataContextInfo.DataContext as DataConnection;

			if (dataConnection == null)
				throw new ArgumentException("DataContext must be of DataConnection type.");

			return dataConnection.DataProvider.Merge(dataConnection, predicate, true, source.Where(predicate),
				tableName    ?? tbl.TableName,
				databaseName ?? tbl.DatabaseName,
				schemaName   ?? tbl.SchemaName);
		}

        /// <summary>
        /// sqlserver下可以使用
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="table"></param>
        /// <param name="predicate"></param>
        /// <param name="source"></param>
        /// <param name="tableName"></param>
        /// <param name="databaseName"></param>
        /// <param name="schemaName"></param>
        /// <returns></returns>
		public static int Merge<T>(this ITable<T> table, Expression<Func<T,bool>> predicate, IEnumerable<T> source,
			string tableName = null, string databaseName = null, string schemaName = null)
			where T : class 
		{
			if (table == null) throw new ArgumentNullException("table");

			var tbl            = (Table<T>)table;
			var dataConnection = tbl.DataContextInfo.DataContext as DataConnection;

			if (dataConnection == null)
				throw new ArgumentException("DataContext must be of DataConnection type.");

			return dataConnection.DataProvider.Merge(dataConnection, predicate, true, source,
				tableName    ?? tbl.TableName,
				databaseName ?? tbl.DatabaseName,
				schemaName   ?? tbl.SchemaName);
		}

        /// <summary>
        /// sqlserver下可以使用
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="table"></param>
        /// <param name="delete"></param>
        /// <param name="source"></param>
        /// <param name="tableName"></param>
        /// <param name="databaseName"></param>
        /// <param name="schemaName"></param>
        /// <returns></returns>
		public static int Merge<T>(this IQueryable<T> table, bool delete, IEnumerable<T> source,
			string tableName = null, string databaseName = null, string schemaName = null)
			where T : class 
		{
			if (table == null) throw new ArgumentNullException("table");

			var tbl            = (Table<T>)table;
			var dataConnection = tbl.DataContextInfo.DataContext as DataConnection;

			if (dataConnection == null)
				throw new ArgumentException("DataContext must be of DataConnection type.");

			return dataConnection.DataProvider.Merge(dataConnection, null, delete, source,
				tableName    ?? tbl.TableName,
				databaseName ?? tbl.DatabaseName,
				schemaName   ?? tbl.SchemaName);
		}

        /// <summary>
        /// sqlserver下可以使用
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="table"></param>
        /// <param name="source"></param>
        /// <param name="tableName"></param>
        /// <param name="databaseName"></param>
        /// <param name="schemaName"></param>
        /// <returns></returns>
		public static int Merge<T>(this IQueryable<T> table, IEnumerable<T> source,
			string tableName = null, string databaseName = null, string schemaName = null)
			where T : class 
		{
			if (table == null) throw new ArgumentNullException("table");

			var tbl            = (Table<T>)table;
			var dataConnection = tbl.DataContextInfo.DataContext as DataConnection;

			if (dataConnection == null)
				throw new ArgumentException("DataContext must be of DataConnection type.");

			return dataConnection.DataProvider.Merge(dataConnection, null, false, source,
				tableName    ?? tbl.TableName,
				databaseName ?? tbl.DatabaseName,
				schemaName   ?? tbl.SchemaName);
		}

		#endregion
	}
}
