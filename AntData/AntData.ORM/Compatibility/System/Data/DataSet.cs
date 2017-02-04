using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace AntData.core.Compatibility.System.Data
{
    public class DataSet
    {
        internal DataSet()
        {
            
        }

        public List<DataTable> Tables { get; set; }
    }
}
