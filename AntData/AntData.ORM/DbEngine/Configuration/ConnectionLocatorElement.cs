#if !NETSTANDARD
using System;
using System.Configuration;

namespace AntData.ORM.DbEngine.Configuration
{
    /// <summary>
    /// 这是提供的一个接口，接受connection的来源，比如可以从database.config中读取连接串，从titan中获取连接串，这是一个接口
    /// </summary>
    public sealed class ConnectionLocatorElement : ConfigurationElement
    {
        /// <summary>
        /// 类型
        /// </summary>
        private const String type = "type";

        private const String path = "path";

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
            get { return Type.GetType(TypeName); }
            set { TypeName = value.AssemblyQualifiedName; }
        }

        [ConfigurationProperty(path)]
        public String Path
        {
            get { return (String)this[path]; }
            set { this[path] = value; }
        }

    }
}
#endif