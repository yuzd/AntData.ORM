using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AntData.ORM.DbEngine.Configuration
{
    public class DBSettings
    {
        internal IEnumerable<DataProviderSettings> DataProviders
        {
            get
            {
                var result = new List<DataProviderSettings>();
                try
                {
                    if (DatabaseSettings == null) return result;
                    foreach (var database in DatabaseSettings)
                    {
                        if (!string.IsNullOrEmpty(database.Provider))
                        {
                            result.Add(new DataProviderSettings
                            {
                                Name = database.Provider,
                                TypeName = database.Provider
                            });
                        }
                    }
                }
                catch
                {

                    //ignore
                }
                return result;
            }
        }

        public IEnumerable<DatabaseSettings> DatabaseSettings { get; set; }
    }
}
