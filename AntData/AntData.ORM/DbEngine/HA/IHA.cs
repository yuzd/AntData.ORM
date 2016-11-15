using System;
using System.Collections.Generic;
using AntData.ORM.DbEngine.DB;

namespace AntData.ORM.DbEngine.HA
{
    interface IHA
    {
        HashSet<Int32> RetryFailOverErrorCodes { get; }

        T ExecuteWithHa<T>(Func<Database, T> func, OperationalDatabases databases);
    }
}
