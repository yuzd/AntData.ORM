//-----------------------------------------------------------------------
// <copyright file="DataConnectionTransaction.cs" company="Company">
// Copyright (C) Company. All Rights Reserved.
// </copyright>
// <author>nainaigu</author>
// <summary></summary>
//-----------------------------------------------------------------------

using System.Data;
using System.Data.Common;
using AntData.ORM.DbEngine.DB;

namespace AntData.ORM.DbEngine.Connection
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;


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

        public void Commit()
        {
            DataConnection.Database.CommitTransaction();
            _disposeTransaction = false;
        }

        public void Rollback()
        {
            DataConnection.Database.RollbackTransaction();
            _disposeTransaction = false;
        }

        public void Dispose()
        {
            if (_disposeTransaction)
                DataConnection.Database.RollbackTransaction();

            DataConnection.Dispose(true,true);
        }
    }
}