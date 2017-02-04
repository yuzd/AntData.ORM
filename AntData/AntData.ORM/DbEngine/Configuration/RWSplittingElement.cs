#if !NETSTANDARD
using System;
using System.Configuration;

namespace AntData.ORM.DbEngine.Configuration
{
    /// <summary>
    /// 读写分离配置
    /// </summary>
    public sealed class RWSplittingElement : ConfigurationElement
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
#endif
