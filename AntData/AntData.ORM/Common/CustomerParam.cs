using System;
using System.Collections.Generic;
using System.Data;
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
        public string TableName { get; set; }
        public string ColumnName { get; set; }
        public ParameterDirection ParameterDirection { get; set; }
    }
}
