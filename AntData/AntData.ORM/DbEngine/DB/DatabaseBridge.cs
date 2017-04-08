using System;
using System.Data;
using AntData.ORM.DbEngine.Connection;
using AntData.ORM.DbEngine.HA;
using AntData.ORM.Properties;

namespace AntData.ORM.DbEngine.DB
{
    class DatabaseBridge
    {
        private DatabaseBridge() { }

        private static readonly DatabaseBridge instance = new DatabaseBridge();

        public static DatabaseBridge Instance { get { return instance; } }
#if !NETSTANDARD
        /// <summary>
        /// 根据Statement，返回DataSet结果集
        /// </summary>
        /// <param name="statement"></param>
        /// <returns></returns>
        public DataSet ExecuteDataSet(Statement statement)
        {
            try
            {
              
                var databases = DatabaseFactory.GetDatabasesByStatement(statement);
                return HAFactory.GetInstance(statement.DatabaseSet).ExecuteWithHa(db => db.ExecuteDataSet(statement), databases);
            }
            finally
            {
            }
        }
#endif

        public DataConnectionTransaction BeginTransaction(Statement statement)
        {
            try
            {

                var databases = DatabaseFactory.GetDatabasesByStatement(statement);
                return HAFactory.GetInstance(statement.DatabaseSet).ExecuteWithHa(db => db.BeginTransaction(statement), databases);
            }
            finally
            {
            }
        }

        /// <summary>
        /// 根据Statement,执行增删改操作
        /// </summary>
        /// <param name="statement"></param>
        /// <returns></returns>
        public Int32 ExecuteNonQuery(Statement statement)
        {
            try
            {
                var databases = DatabaseFactory.GetDatabasesByStatement(statement);
                return HAFactory.GetInstance(statement.DatabaseSet).ExecuteWithHa(db => db.ExecuteNonQuery(statement), databases);
            }
            finally
            {
            }
        }

        /// <summary>
        /// 根据Statement，返回IDataReader形式的结果集
        /// </summary>
        /// <param name="statement"></param>
        /// <returns></returns>
        public IDataReader ExecuteReader(Statement statement)
        {
            try
            {
                var databases = DatabaseFactory.GetDatabasesByStatement(statement);
                return HAFactory.GetInstance(statement.DatabaseSet).ExecuteWithHa(db => db.ExecuteReader(statement), databases);
            }
            finally
            {
            }
        }

        /// <summary>
        /// 根据Statement，返回第一行第一列
        /// </summary>
        /// <param name="statement"></param>
        /// <returns></returns>
        public Object ExecuteScalar(Statement statement)
        {
            try
            {
              
                var databases = DatabaseFactory.GetDatabasesByStatement(statement);
                return HAFactory.GetInstance(statement.DatabaseSet).ExecuteWithHa(db => db.ExecuteScalar(statement), databases);
            }
            finally
            {
            }
        }

    }
}
