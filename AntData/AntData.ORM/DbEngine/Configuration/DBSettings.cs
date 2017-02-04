using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AntData.core.DbEngine.Configuration
{
    public class DBSettings
    {
        public IEnumerable<DataProviderSettings> DataProviders { get; set; }
        public IEnumerable<ConnectionStringSettings> ConnectionStrings { get; set; }
    }
}
