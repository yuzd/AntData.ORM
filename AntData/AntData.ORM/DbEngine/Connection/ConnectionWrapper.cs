using System;
using System.Data.Common;
using AntData.ORM.DbEngine.DB;

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
        private readonly Database m_dataBase;

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
        /// 构造方法
        /// </summary>
        /// <param name="connection">数据库链接</param>
        /// <param name="disposeConnection">是否释放链接</param>
        public ConnectionWrapper(DbConnection connection, Boolean disposeConnection)
        {
            m_Connection = connection;
            m_DisposeConnection = disposeConnection;
            m_Disposed = false;
        }
        public ConnectionWrapper(DbConnection connection, Database database)
        {
            m_Connection = connection;
            m_dataBase = database;
            m_DisposeConnection = false;
            m_Disposed = false;
        }
        /// <summary>
        /// 数据库链接
        /// </summary>
        public DbConnection Connection
        {
            get { return m_Connection; }
        }

        public Database Database
        {
            get { return m_dataBase; }
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
                if (Database != null && Database.Transactions != null)
                {
                    Database.Transactions.Dispose();
                    Database.Transactions = null;
                }
            }

            
        }

       
        #endregion
    }
}
