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
                    break;
                case DataType.Binary:
                case DataType.VarBinary:
                case DataType.Blob:
                case DataType.Image:
                    return 1;
                    break;
                case DataType.Boolean:
                    return 3;
                    break;
                case DataType.Guid:
                     return 9;
                    break;
                case DataType.SByte:
                    return 14;
                    break;
                case DataType.Int16:
                    return 10;
                    break;
                case DataType.Int32:
                    return 11;
                    break;
                case DataType.Int64:
                    return 12;
                    break;
                case DataType.Byte:
                    return 2;
                    break;
                case DataType.UInt16:
                    return 18;
                    break;
                case DataType.UInt32:
                    return 19;
                    break;
                case DataType.UInt64:
                    return 20;
                    break;
                case DataType.Single:
                    return 15;
                    break;
                case DataType.Double:
                    return 8;
                    break;
                case DataType.Decimal:
                    return 7;
                    break;
                case DataType.Money:
                case DataType.SmallMoney:
                    return 4;
                    break;
                case DataType.Date:
                    return 5;
                    break;
                case DataType.Time:
                    return 17;
                    break;
                case DataType.DateTime:
                    return 6;
                    break;
                case DataType.DateTime2:
                case DataType.SmallDateTime:
                    return 26;
                    break;
                case DataType.DateTimeOffset:
                    return 27;
                    break;
                case DataType.Xml:
                    return 25;
                    break;
                case DataType.Variant:
                    return 21;
                    break;
                 
            }
            return -1;
        }
    }
}
