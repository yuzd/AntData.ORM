//-----------------------------------------------------------------------
// <copyright file="DataConnectionTransaction.cs" company="Company">
// Copyright (C) Company. All Rights Reserved.
// </copyright>
// <author>nainaigu</author>
// <summary></summary>
//-----------------------------------------------------------------------

using System.Data;

namespace AntData.ORM.DbEngine.Connection
{
    using System;


    /// <summary>
    /// 
    /// </summary>
    public class DataConnectionTransaction : IDisposable
    {

        public DataConnectionTransaction(ConnectionWrapper dataConnection)
        {
            if (dataConnection == null) throw new ArgumentNullException("dataConnection");

            DataConnection = dataConnection;
        }

        public ConnectionWrapper DataConnection { get; private set; }

        bool _disposeTransaction = true;

        public IDbTransaction Transactions { get; internal set; }
        

        public void Commit()
        {
            DataConnection.CommitTransaction();
            _disposeTransaction = false;
        }

        public void Rollback()
        {
            DataConnection.RollbackTransaction();
            _disposeTransaction = false;
        }

        public void Dispose()
        {
            if (_disposeTransaction)
                DataConnection.RollbackTransaction();

            DataConnection.Dispose(true,true);
        }
    }
}