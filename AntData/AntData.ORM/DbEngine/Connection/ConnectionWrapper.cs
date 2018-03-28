using System;
using System.Data;
using System.Data.Common;

namespace AntData.ORM.DbEngine.Connection
{
    /// <summary>
    /// 数据库链接包装器
    /// 控制数据库链接
    /// </summary>
    public class ConnectionWrapper : IDisposable
    {
        #region private field

        /// <summary>
        /// 真正的数据库链接
        /// </summary>
        private readonly DbConnection m_Connection;

        /// <summary>
        /// DbTransaction
        /// </summary>
        private  IDbTransaction m_dbTransaction;

        /// <summary>
        /// 是否释放链接
        /// </summary>
        private readonly Boolean m_DisposeConnection;

        /// <summary>
        /// 是否已释放
        /// </summary>
        private Boolean m_Disposed;

        #endregion

        /// <summary>
        /// 真正物理的DB名称
        /// </summary>
        public string DBName { get; set; }
        public string DataSource { get; set; }
        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="connection">数据库链接</param>
        /// <param name="disposeConnection">是否释放链接</param>
        public ConnectionWrapper(DbConnection connection, Boolean disposeConnection)
        {
            m_Connection = connection;
            m_DisposeConnection = disposeConnection;
            m_Disposed = false;
            DBName = connection.Database;
            DataSource = connection.DataSource;
        }
        public ConnectionWrapper(DbConnection connection, IDbTransaction dbTransaction)
        {
            m_Connection = connection;
            m_DisposeConnection = false;
            m_Disposed = false;
            DBName = connection.Database;
            DataSource = connection.DataSource;
            m_dbTransaction = dbTransaction;
        }
        /// <summary>
        /// 数据库链接
        /// </summary>
        public DbConnection Connection
        {
            get { return m_Connection; }
        }


        public void CommitTransaction()
        {
            if (m_dbTransaction != null)
            {
                m_dbTransaction.Commit();

            }
        }

        public void RollbackTransaction()
        {
            if (m_dbTransaction != null)
            {
                m_dbTransaction.Rollback();

            }
        }

        public IDbTransaction GetTransaction()
        {
            if (m_dbTransaction != null)
            {
                return m_dbTransaction;
            }

            return null;
        }
        #region IDisposable Members


        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~ConnectionWrapper()
        {
            Dispose(false);
        }

        /// <summary>
        /// 释放链接
        /// </summary>
        public void Dispose(Boolean isDisposing,Boolean endTranse = false)
        {
            if (m_Disposed && !endTranse) return;
            m_Disposed = true;

            if (isDisposing && (m_DisposeConnection || endTranse))
            {
                m_Connection.Dispose();
                if (m_dbTransaction != null)
                {
                    m_dbTransaction.Dispose();
                    m_dbTransaction = null;
                }
            }

            
        }

       
        #endregion
    }
}
