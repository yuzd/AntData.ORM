using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntData.ORM.Common
{
    public class DataTypeConvert
    {
        public static int Convert(DataType type)
        {
            switch (type)
            {
                case DataType.Char:
                case DataType.VarChar:
                case DataType.NChar:
                case DataType.NVarChar:
                case DataType.Text:
                case DataType.NText:
                    return 16;
                case DataType.Binary:
                case DataType.VarBinary:
                case DataType.Blob:
                case DataType.Image:
                    return 1;
                case DataType.Boolean:
                    return 3;
                case DataType.Guid:
                     return 9;
                case DataType.SByte:
                    return 14;
                case DataType.Int16:
                    return 10;
                case DataType.Int32:
                    return 11;
                case DataType.Int64:
                    return 12;
                case DataType.Byte:
                    return 2;
                case DataType.UInt16:
                    return 18;
                case DataType.UInt32:
                    return 19;
                case DataType.UInt64:
                    return 20;
                case DataType.Single:
                    return 15;
                case DataType.Double:
                    return 8;
                case DataType.Decimal:
                    return 7;
                case DataType.Money:
                case DataType.SmallMoney:
                    return 4;
                case DataType.Date:
                    return 5;
                case DataType.Time:
                    return 17;
                case DataType.DateTime:
                    return 6;
                case DataType.DateTime2:
                case DataType.SmallDateTime:
                    return 26;
                case DataType.DateTimeOffset:
                    return 27;
                case DataType.Xml:
                    return 25;
                case DataType.Variant:
                    return 21;
                 
            }
            return -1;
        }
    }
}
