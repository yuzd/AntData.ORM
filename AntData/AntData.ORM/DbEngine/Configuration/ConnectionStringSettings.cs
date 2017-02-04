using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AntData.ORM.Enums;

namespace AntData.ORM.DbEngine.Configuration
{
    public class ConnectionStringSettings
    {
        public List<ConnectionItem> ConnectionItemList { get; set; }
        public string ProviderName { get; set; }
        public string Name { get; set; }
    }

    public class ConnectionItem
    {
        public string ConnectionString { get; set; }
        public string Name { get; set; }

        public DatabaseType DatabaseType { get; set; }
    }
}
