using System;
#if !NETSTANDARD
using System.Configuration;
#endif
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Globalization;
using System.Threading;
#if !NETSTANDARD
using System.Transactions;
#endif
using AntData.ORM.Common.Util;
using AntData.ORM.Dao;
using AntData.ORM.Dao.Common;
using AntData.ORM.DbEngine.Connection;
using AntData.ORM.DbEngine.Providers;
using AntData.ORM.Enums;
using StatementType = AntData.ORM.Enums.StatementType;

namespace AntData.ORM.DbEngine.DB
{
    /// <summary>
    /// 物理数据库对象
    /// </summary>
    public sealed class Database
    {
        /// <summary>
        /// 数据库链接
        /// </summary>
        private String m_ConnectionString;


        /// <summary>
        /// 数据库提供者对象
        /// </summary>
        private readonly IDatabaseProvider m_DatabaseProvider;

        #region properties

        /// <summary>
        /// 数据库链接
        /// </summary>
        public String ConnectionString
        {
            get
            {
                // 原来是 重新读取All In One中的连接串
                // connectionStringLock.EnterReadLock();
                //String result = m_ConnectionString;
                //connectionStringLock.ExitReadLock();
                return m_ConnectionString;
            }
        }

        /// <summary>
        /// 真正的数据库名
        /// </summary>
        public volatile String ActualDatabaseName;

        /// <summary>
        /// 对应的AllInOne中的Key
        /// </summary>
        public String AllInOneKey { get; private set; }

        /// <summary>
        /// DalConfig中配置的DatabaseName
        /// </summary>
        public String DatabaseName { get; private set; }

        /// <summary>
        /// 数据库的类型，读库/写库
        /// </summary>
        public DatabaseType DatabaseRWType { get; set; }

        public String DatabaseSetName { get; private set; }

        /// <summary>
        /// 当前数据库是否处于可用状态
        /// </summary>
        public Boolean Available
        {
            get { return true; }
        }

        #endregion

        #region construction

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="databaseSetName"></param>
        /// <param name="databaseName"></param>
        /// <param name="connectionStringName">数据库链接名称</param>
        /// <param name="databaseProvider">数据库提供者</param>
        public Database(String databaseSetName, String databaseName, String connectionStringName, IDatabaseProvider databaseProvider)
        {
            if (String.IsNullOrEmpty(connectionStringName))
                throw new ArgumentNullException("connectionStringName");
            if (String.IsNullOrEmpty(databaseSetName))
                throw new ArgumentNullException("databaseSet");
            if (String.IsNullOrEmpty(databaseName))
                throw new ArgumentNullException("databaseName");
            if (databaseProvider == null)
                throw new ArgumentNullException("databaseProvider");

            DatabaseSetName = databaseSetName;
            DatabaseName = databaseName;
            AllInOneKey = connectionStringName;
            m_DatabaseProvider = databaseProvider;
            LoadActualConnectionString();
            _closeTransaction = false;
        }

        /// <summary>
        /// 重新读取All In One中的连接串 这里为了方便直接用了配置文件里面的
        /// </summary>
        private void LoadActualConnectionString()
        {
            //var connectionStringSetting = ConnectionLocatorManager.Instance.GetConnectionString(AllInOneKey);
            //connectionStringLock.EnterWriteLock();
            //m_ConnectionString = connectionStringSetting == null ? String.Empty : connectionStringSetting.ConnectionString;
            //connectionStringLock.ExitWriteLock();

            m_ConnectionString = AllInOneKey;
        }

        #endregion

        #region helper methods

        /// <summary>
        /// 获取打开的数据库链接
        /// </summary>
        /// <param name="disposeInnerConnection">是否释放数据库链接对象</param>
        /// <returns></returns>
        private ConnectionWrapper GetOpenConnection(Boolean disposeInnerConnection)
        {
            var connection = TransactionConnectionManager.GetConnection(this);
            if (connection != null)
            {
#if DEBUG
                Debug.WriteLine(connection.ConnectionString);
#endif
                return new ConnectionWrapper(connection, false);
            }


            try
            {
                connection = CreateConnection();
                Interlocked.CompareExchange(ref ActualDatabaseName, connection.Database, null);
                connection.Open();
            }
            catch
            {
                if (connection != null)
                    connection.Close();
                throw;
            }

            return new ConnectionWrapper(connection, disposeInnerConnection);
        }

        #region Transactions
        bool _closeTransaction;
        public IDbTransaction Transactions { get; internal set; }
        public DataConnectionTransaction BeginTransaction(Statement statement)
        {
            // If transaction is open, we dispose it, it will rollback all changes.
            //
            if (Transactions != null)
                Transactions.Dispose();

            // Create new transaction object.
            DbConnection connection = null;
            try
            {
                connection = CreateConnection();
                connection.Open();
            }
            catch
            {
                if (connection != null)
                    connection.Close();
                throw;
            }

            if (statement.Hints != null && statement.Hints.Contains(DALExtStatementConstant.ISOLATION_LEVEL))
            {
                var level = (System.Data.IsolationLevel)statement.Hints[DALExtStatementConstant.ISOLATION_LEVEL];
                Transactions = connection.BeginTransaction(level);
            }
            else
            {
                Transactions = connection.BeginTransaction();
            }


            _closeTransaction = true;

            return new DataConnectionTransaction(new ConnectionWrapper(connection, this));
        }

        public void CommitTransaction()
        {
            if (Transactions != null)
            {
                Transactions.Commit();

                if (_closeTransaction)
                {
                    Transactions.Dispose();
                    Transactions = null;

                }
            }
        }

        public void RollbackTransaction()
        {
            if (Transactions != null)
            {
                Transactions.Rollback();

                if (_closeTransaction)
                {
                    Transactions.Dispose();
                    Transactions = null;

                }
            }
        }

        #endregion

        /// <summary>
        /// 创建数据库链接
        /// </summary>
        /// <returns></returns>
        public DbConnection CreateConnection()
        {
            if (String.IsNullOrEmpty(ConnectionString))
                throw new DalException(String.Format("ConnectionString:{0} can't be found!", AllInOneKey));
#if DEBUG
            Debug.WriteLine(ConnectionString);
#endif
            var connection = m_DatabaseProvider.CreateConnection();
            connection.ConnectionString = ConnectionString;
            return connection;
        }

        /// <summary>
        /// 准备数据库指令
        /// </summary>
        /// <param name="statement">上层指令</param>
        /// <returns>数据库指令</returns>
        private DbCommand PrepareCommand(Statement statement)
        {
            DbCommand command = m_DatabaseProvider.CreateCommand();
            command.CommandText = statement.StatementText;
            command.CommandType = statement.StatementType == StatementType.Sql ? CommandType.Text : CommandType.StoredProcedure;
            command.CommandTimeout = statement.Timeout;
            String providerName = m_DatabaseProvider.GetType().Name;

            foreach (var p in statement.Parameters)
            {
                if (p.ExtendType == 1)
                {
                    var parameter = (SqlParameter)command.CreateParameter();
                    parameter.ParameterName = m_DatabaseProvider.CreateParameterName(p.Name);
                    parameter.SqlDbType = (SqlDbType)p.ExtendTypeValue;

                    if (!String.IsNullOrEmpty(p.TypeName))
                        parameter.TypeName = p.TypeName;
                    parameter.Size = p.Size;
                    parameter.Value = p.Value ?? DBNull.Value;
                    parameter.Direction = p.Direction;
                    parameter.IsNullable = p.IsNullable;
                    command.Parameters.Add(parameter);
                }
                else
                {
                    if (p.DbType == DbType.Time && providerName == "SqlDatabaseProvider")
                    {
                        var parameter = (SqlParameter)command.CreateParameter();
                        parameter.ParameterName = m_DatabaseProvider.CreateParameterName(p.Name);
                        parameter.SqlDbType = SqlDbType.Time;
                        parameter.Size = p.Size;
                        parameter.Value = p.Value ?? DBNull.Value;
                        parameter.Direction = p.Direction;
                        parameter.IsNullable = p.IsNullable;
                        command.Parameters.Add(parameter);
                    }
                    else
                    {
                        var parameter = command.CreateParameter();
                        parameter.ParameterName = m_DatabaseProvider.CreateParameterName(p.Name);
                        parameter.DbType = p.DbType;
                        parameter.Size = p.Size;
                        parameter.Value = p.Value ?? DBNull.Value;
                        parameter.Direction = p.Direction;
                        parameter.IsNullable = p.IsNullable;

                        if (providerName.Equals("MySqlDatabaseProvider"))
                        {
                            command.Parameters.Insert(-1, parameter);   //work around for legacy mysql driver versions
                        }
                        else
                        {
                            command.Parameters.Add(parameter);
                        }
                    }
                }
            }

            return command;
        }

        /// <summary>
        /// 更新执行后的参数
        /// </summary>
        /// <param name="statement">指令</param>
        /// <param name="command">数据库指令</param>
        private void UpdateStatementParamenters(Statement statement, DbCommand command)
        {
            foreach (var p in statement.Parameters)
            {
                if (p.Direction != ParameterDirection.Input)
                    p.Value = command.Parameters[m_DatabaseProvider.CreateParameterName(p.Name)].Value;
            }
        }
#if !NETSTANDARD

        /// <summary>
        /// 加载程序集
        /// </summary>
        /// <param name="statement">statement</param>
        /// <param name="command">指令</param>
        /// <param name="dataSet">程序集</param>
        /// <param name="tableNames">表名称</param>
        private void LoadDataSet(Statement statement, DbCommand command, DataSet dataSet, params String[] tableNames)
        {
            Boolean schemaRequired = false;
            Boolean disableConstraints = false;
            if (statement.Hints != null && statement.Hints.Contains(DALExtStatementConstant.RETRIEVE_SCHEMA))
            {
                schemaRequired = true;
                if (statement.Hints.Contains(DALExtStatementConstant.DISABLE_CONSTRAINTS))
                    disableConstraints = true;
            }

            if (tableNames == null || tableNames.Length == 0)
                tableNames = new[] { "Table" };

            for (Int32 i = 0; i < tableNames.Length; i++)
            {
                if (String.IsNullOrEmpty(tableNames[i]))
                    throw new ArgumentException(String.Concat("tableNames[", i, "]"));
            }

            using (var adapter = m_DatabaseProvider.CreateDataAdapter())
            {
                adapter.SelectCommand = command;

                for (Int32 i = 0; i < tableNames.Length; i++)
                {
                    String tableName = (i == 0) ? "Table" : "Table" + i;
                    adapter.TableMappings.Add(tableName, tableNames[i]);
                }

                if (schemaRequired)
                {
                    adapter.FillSchema(dataSet, SchemaType.Mapped);
                    if (disableConstraints)
                        dataSet.EnforceConstraints = false;
                }

                adapter.Fill(dataSet);
            }
        }
#endif
        #endregion

#if !NETSTANDARD


        /// <summary>
        /// 执行返回数据集指令
        /// </summary>
        /// <param name="statement">指令</param>
        /// <returns>数据集</returns>
        public DataSet ExecuteDataSet(Statement statement)
        {
            var watch = new Stopwatch();

            try
            {
                DataSet dataSet = new DataSet { Locale = CultureInfo.InvariantCulture };
                statement.PreProcess(AllInOneKey, ActualDatabaseName, DatabaseRWType, m_DatabaseProvider, ConnectionString);
                var trans_wrapper = GetConnectionInTransaction(statement);
                using (IDbCommand command = PrepareCommand(statement))
                {
                    using (var wrapper = trans_wrapper ?? GetOpenConnection(true))
                    {
                        command.Connection = wrapper.Connection;
                        if (trans_wrapper != null)
                        {
                            command.Transaction = trans_wrapper.Database.Transactions;
                        }
                        LoadDataSet(statement, (DbCommand)command, dataSet);
                        UpdateStatementParamenters(statement, (DbCommand)command);
                    }
                }

                statement.ExecStatus = DALState.Success;
                return dataSet;
            }
            catch (Exception ex)
            {
                statement.ExecStatus = DALState.Fail;
                throw ex;
            }
            finally
            {
                watch.Stop();
                statement.Duration = TimeSpan.FromMilliseconds(watch.ElapsedMilliseconds);
            }
        }
#endif


        /// <summary>
        /// 执行非查询指令
        /// </summary>
        /// <param name="statement">指令</param>
        /// <returns>影响行数</returns>
        public Int32 ExecuteNonQuery(Statement statement)
        {
            var watch = new Stopwatch();

            try
            {
                Int32 result;
                statement.PreProcess(AllInOneKey, ActualDatabaseName, DatabaseRWType, m_DatabaseProvider, ConnectionString);
                var trans_wrapper = GetConnectionInTransaction(statement);
                using (IDbCommand command = PrepareCommand(statement))
                {
                    using (var wrapper = trans_wrapper ??  GetOpenConnection(true))
                    {
                        command.Connection = wrapper.Connection;
                        if (trans_wrapper!=null)
                        {
                            command.Transaction = trans_wrapper.Database.Transactions;
                        }
                        result = command.ExecuteNonQuery();
                        UpdateStatementParamenters(statement, (DbCommand)command);
                    }
                }
                statement.ExecStatus = DALState.Success;
                return result;
            }
            catch (Exception ex)
            {
                statement.ExecStatus = DALState.Fail;
                throw ex;
            }
            finally
            {
                watch.Stop();
                statement.Duration = TimeSpan.FromMilliseconds(watch.ElapsedMilliseconds);
            }
        }

        /// <summary>
        /// 执行返回单向只读数据集的指令
        /// </summary>
        /// <param name="statement">指令</param>
        /// <returns>单向只读DataReader对象</returns>
        public IDataReader ExecuteReader(Statement statement)
        {
            var watch = new Stopwatch();

            try
            {
                IDataReader reader;
                statement.PreProcess(AllInOneKey, ActualDatabaseName, DatabaseRWType, m_DatabaseProvider, ConnectionString);
                var trans_wrapper = GetConnectionInTransaction(statement);
                using (IDbCommand command = PrepareCommand(statement))
                {
                    using (var wrapper = trans_wrapper ?? GetOpenConnection(false))
                    {
                        command.Connection = wrapper.Connection;
                        if (trans_wrapper != null)
                        {
                            command.Transaction = trans_wrapper.Database.Transactions;
                        }
#if !NETSTANDARD
                        reader = command.ExecuteReader(Transaction.Current != null ? CommandBehavior.Default : CommandBehavior.CloseConnection);
#else
                        reader = command.ExecuteReader(trans_wrapper!=null ?CommandBehavior.Default :CommandBehavior.CloseConnection);
#endif
                        UpdateStatementParamenters(statement, (DbCommand)command);
                    }
                }
                statement.ExecStatus = DALState.Success;
                return reader;
            }
            catch (Exception ex)
            {
                statement.ExecStatus = DALState.Fail;
                throw ex;
            }
            finally
            {
                watch.Stop();
                statement.Duration = TimeSpan.FromMilliseconds(watch.ElapsedMilliseconds);
            }
        }

        /// <summary>
        /// 判断 hints里面是否已有连接
        /// </summary>
        /// <param name="statement"></param>
        /// <returns></returns>
        public ConnectionWrapper GetConnectionInTransaction(Statement statement)
        {
            ConnectionWrapper trans_wrapper = null;
            if (statement.Hints != null && statement.Hints.Contains(DALExtStatementConstant.TRANSACTION_CONNECTION))
            {
                trans_wrapper = statement.Hints[DALExtStatementConstant.TRANSACTION_CONNECTION] as ConnectionWrapper;
            }
            return trans_wrapper;
        }

        /// <summary>
        /// 执行单返回值聚集查询指令
        /// </summary>
        /// <param name="statement">指令</param>
        /// <returns>聚集结果</returns>
        public Object ExecuteScalar(Statement statement)
        {
            var watch = new Stopwatch();

            try
            {
                Object result;
                statement.PreProcess(AllInOneKey, ActualDatabaseName, DatabaseRWType, m_DatabaseProvider, ConnectionString);
                var trans_wrapper = GetConnectionInTransaction(statement);
                using (IDbCommand command = PrepareCommand(statement))
                {
                    using (var wrapper = trans_wrapper ?? GetOpenConnection(true))
                    {
                        command.Connection = wrapper.Connection;
                        if (trans_wrapper != null)
                        {
                            command.Transaction = trans_wrapper.Database.Transactions;
                        }
                        result = command.ExecuteScalar();
                        UpdateStatementParamenters(statement, (DbCommand)command);
                    }
                }
                statement.ExecStatus = DALState.Success;
                return result;
            }
            catch (Exception ex)
            {
                statement.ExecStatus = DALState.Fail;
                throw ex;
            }
            finally
            {
                watch.Stop();
                statement.Duration = TimeSpan.FromMilliseconds(watch.ElapsedMilliseconds);
            }
        }

        /// <summary>
        /// 执行返回单向只读数据集的指令
        /// </summary>
        /// <param name="statement">指令</param>
        /// <returns>单向只读DataReader对象</returns>
        public IDataReader InnnerExecuteReader(Statement statement)
        {
            var watch = new Stopwatch();

            try
            {
                IDataReader reader;
                statement.SQLHash = CommonUtil.GetHashCodeOfSQL(statement.StatementText);
                if (statement.StatementType == StatementType.Sql)
                    statement.StatementText = CommonUtil.GetTaggedAppIDSql(statement.StatementText);
                watch.Start();
                var trans_wrapper = GetConnectionInTransaction(statement);
                using (IDbCommand command = PrepareCommand(statement))
                {
                    using (var wrapper = trans_wrapper ?? GetOpenConnection(false))
                    {
                        command.Connection = wrapper.Connection;
                        if (trans_wrapper != null)
                        {
                            command.Transaction = trans_wrapper.Database.Transactions;
                        }
#if !NETSTANDARD
                        reader =
                               command.ExecuteReader(Transaction.Current != null
                                   ? CommandBehavior.Default
                                   : CommandBehavior.CloseConnection);
#else
                        reader =command.ExecuteReader(trans_wrapper !=null ?CommandBehavior.Default : CommandBehavior.CloseConnection);
#endif

                        UpdateStatementParamenters(statement, (DbCommand)command);
                    }
                }

                statement.ExecStatus = DALState.Success;
                return reader;
            }
            catch
            {
                statement.ExecStatus = DALState.Fail;
                return null;
            }
            finally
            {
                watch.Stop();
                statement.Duration = TimeSpan.FromMilliseconds(watch.ElapsedMilliseconds);
            }
        }

        /// <summary>
        /// 执行单返回值聚集查询指令
        /// </summary>
        /// <param name="statement">指令</param>
        /// <returns>聚集结果</returns>
        public Object InnnerExecuteScalar(Statement statement)
        {
            var watch = new Stopwatch();

            try
            {
                statement.SQLHash = CommonUtil.GetHashCodeOfSQL(statement.StatementText);
                if (statement.StatementType == StatementType.Sql)
                    statement.StatementText = CommonUtil.GetTaggedAppIDSql(statement.StatementText);

                watch.Start();
                Object result;
                var trans_wrapper = GetConnectionInTransaction(statement);
                using (IDbCommand command = PrepareCommand(statement))
                {
                    using (var wrapper = trans_wrapper ?? GetOpenConnection(true))
                    {
                        if (trans_wrapper != null)
                        {
                            command.Transaction = trans_wrapper.Database.Transactions;
                        }
                        command.Connection = wrapper.Connection;
                        result = command.ExecuteScalar();
                        UpdateStatementParamenters(statement, (DbCommand)command);
                    }
                }

                statement.ExecStatus = DALState.Success;
                statement.RecordCount = result == null ? 0 : 1;
                return result;
            }
            catch
            {
                statement.ExecStatus = DALState.Fail;
                return null;
            }
            finally
            {
                watch.Stop();
                statement.Duration = TimeSpan.FromMilliseconds(watch.ElapsedMilliseconds);
            }
        }

    }
}
