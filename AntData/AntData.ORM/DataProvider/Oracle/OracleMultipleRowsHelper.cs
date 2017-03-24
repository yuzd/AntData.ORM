//-----------------------------------------------------------------------
// <copyright file="OracleMultipleRowsHelper.cs" company="Company">
// Copyright (C) Company. All Rights Reserved.
// </copyright>
// <author>nainaigu</author>
// <summary></summary>
//-----------------------------------------------------------------------

using AntData.ORM.Data;
using AntData.ORM.Extensions;
using AntData.ORM.Mapping;

namespace AntData.ORM.DataProvider.Oracle
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;


    /// <summary>
    /// 
    /// </summary>
    public class OracleMultipleRowsHelper<T>: MultipleRowsHelper<T>
    {
        public string WithSeqName;
        public OracleMultipleRowsHelper(DataConnection dataConnection, BulkCopyOptions options, bool enforceKeepIdentity) : base(dataConnection, options, enforceKeepIdentity)
        {
            var id = base.Columns.FirstOrDefault(r => r.IsIdentity);
            if (id != null)
            {
                var seq = id.MemberInfo.GetCustomAttributesEx(typeof(SequenceNameAttribute), true).FirstOrDefault();
                var seqName = seq as SequenceNameAttribute;
                if (seqName != null)
                {
                    WithSeqName = seqName.SequenceFunction + "()";
                }
            }
        }

        public override void BuildColumns(object item, string tableName, Func<ColumnDescriptor, bool> skipConvert = null)
        {
            skipConvert = skipConvert ?? (_ => false);
            for (var i = 0; i < Columns.Length; i++)
            {
                var column = Columns[i];
                if (column.IsIdentity && !string.IsNullOrEmpty(WithSeqName))
                {
                    StringBuilder.Append(WithSeqName);
                    StringBuilder.Append(" ,");
                    continue;
                }
                var value = column.GetValue(item);

                if (!skipConvert(column) /*|| !ValueConverter.TryConvert(StringBuilder, ColumnTypes[i], value)*/)
                {
                    var name = ParameterName == "?" ? ParameterName : ParameterName + ++ParameterIndex;

                    StringBuilder.Append(name);

                    if (value is DataParameter)
                    {
                        value = ((DataParameter)value).Value;
                    }
                    var p = new DataParameter(ParameterName == "?" ? ParameterName : "p" + ParameterIndex, value,
                                           column.DataType);
                    p.TableName = tableName;
                    p.ColumnName = column.ColumnName;
                    Parameters.Add(p);
                }

                StringBuilder.Append(",");
            }

            StringBuilder.Length--;
        }
    }
}