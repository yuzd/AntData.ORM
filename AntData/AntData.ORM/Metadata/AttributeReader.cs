using JetBrains.Annotations;
using System;
using System.Reflection;

namespace AntData.ORM.Metadata
{
    using Extensions;

    public class AttributeReader : IMetadataReader
    {
        [NotNull]
        public T[] GetAttributes<T>(Type type, bool inherit = true)
            where T : Attribute
        {
            var attrs = type.GetCustomAttributesEx(typeof(T), inherit);
            var arr = new T[attrs.Length];

            for (var i = 0; i < attrs.Length; i++)
                arr[i] = (T)attrs[i];

            return arr;
        }

        [NotNull]
        public T[] GetAttributes<T>(Type type, MemberInfo memberInfo, bool inherit = true)
            where T : Attribute
        {
            var attrs = memberInfo.GetCustomAttributesEx(typeof(T), inherit);
            if (attrs == null || attrs.Length < 1)
            {
                if (memberInfo.DeclaringType != null && memberInfo.DeclaringType.BaseType != null)
                {
                    if (memberInfo.IsFieldEx())
                    {
                        var newMenberInfo = memberInfo.DeclaringType.BaseType.GetField(memberInfo.Name);
                        if (newMenberInfo != null) return GetAttributes<T>(type, newMenberInfo, inherit);

                    }
                    else if (memberInfo.IsPropertyEx())
                    {
                        var newMenberInfo = memberInfo.DeclaringType.BaseType.GetProperty(memberInfo.Name);
                        if (newMenberInfo != null) return GetAttributes<T>(type, newMenberInfo, inherit);
                    }
                }
            }
            var arr = new T[attrs.Length];

            for (var i = 0; i < attrs.Length; i++)
                arr[i] = (T)attrs[i];

            return arr;
        }
    }
}
