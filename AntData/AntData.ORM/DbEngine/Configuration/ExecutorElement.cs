//-----------------------------------------------------------------------
// <copyright file="ExecutorElement.cs" company="Company">
// Copyright (C) Company. All Rights Reserved.
// </copyright>
// <author>nainaigu</author>
// <summary></summary>
//-----------------------------------------------------------------------

using System.Configuration;

namespace AntData.ORM.DbEngine.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;


    public sealed class ExecutorElement : ConfigurationElement
    {
        /// <summary>
        /// 类型
        /// </summary>
        private const String type = "type";

        /// <summary>
        /// 类型
        /// </summary>
        [ConfigurationProperty(type)]
        public String TypeName
        {
            get { return (String)this[type]; }
            set { this[type] = value; }
        }

        /// <summary>
        /// 类型
        /// </summary>
        public Type Type
        {
            get
            {
                try
                {
                    return Type.GetType(TypeName);
                }
                catch
                {
                    return null;
                }

            }
            set { TypeName = value.AssemblyQualifiedName; }
        }

    }
}