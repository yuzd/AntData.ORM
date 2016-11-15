using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntData.ORM.Common
{
    public class CustomerParam
    {
        public string ParameterName { get; set; }

        public Type ParameterType { get; set; }

        public object Value { get; set; }

        public int DbType { get; set; }
    }
}
