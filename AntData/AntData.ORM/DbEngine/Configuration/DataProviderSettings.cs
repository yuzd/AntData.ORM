using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;

namespace AntData.ORM.DbEngine.Configuration
{
    public class DataProviderSettings
    {
        /// <summary>
        /// Gets or sets an assembly qualified type name of this data provider.
        /// </summary>
        public Type Type
        {
            get
            {
                if (string.IsNullOrEmpty(TypeName))
                {
                    return null;
                }
                return Type.GetType(TypeName);
            }
        }

        /// <summary>
        /// Gets or sets a name of this data provider.
        /// </summary>
        public string Name { get; set; }

        public string TypeName { get; set; }

    }
}
