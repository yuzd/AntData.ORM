using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AntData.ORM.Common;
using AntData.ORM.DataProvider;


namespace Arch.Data.ORM.Mysql
{
    public abstract class LinqToDBConnection : AntData.ORM.Data.DataConnection
    {
        protected LinqToDBConnection(IDataProvider provider, string dbMappingName, Func<string, string, Dictionary<string, CustomerParam>, IDictionary, int> CustomerExecuteNonQuery, Func<string, string, Dictionary<string, CustomerParam>, IDictionary, object> CustomerExecuteScalar, Func<string, string, Dictionary<string, CustomerParam>, IDictionary, IDataReader> CustomerExecuteQuery,  Func<string, string, Dictionary<string, CustomerParam>, IDictionary, DataTable> CustomerExecuteQueryTable)
            : base(provider, dbMappingName, CustomerExecuteNonQuery, CustomerExecuteScalar, CustomerExecuteQuery,CustomerExecuteQueryTable) { }




    }
}
