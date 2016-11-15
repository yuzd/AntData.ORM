using System;
using System.Configuration;

namespace AntData.ORM.DbEngine.ConnectionString
{
    public interface IConnectionString
    {
        ConnectionStringSettings GetConnectionString(String connectionStringName);
    }
}