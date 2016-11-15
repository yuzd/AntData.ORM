using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace AntData.ORM.Common
{
	public static class Tools
	{
		[StringFormatMethod("format")]
		public static string Args(this string format, params object[] args)
		{
			return string.Format(format, args);
		}

		public static bool IsNullOrEmpty(this ICollection array)
		{
			return array == null || array.Count == 0;
		}

		public static bool IsNullOrEmpty(this string str)
		{
			return string.IsNullOrEmpty(str);
		}
	}
    /// <summary>
    /// 枚举帮助类
    /// </summary>
    public static class LinqEnumHelper
    {
       

        /// <summary>
        /// 将整型值转换成相应的枚举
        /// </summary>
        /// <typeparam name="T">枚举类型</typeparam>
        /// <param name="value">整形值</param>
        /// <returns>枚举</returns>
        public static T IntToEnum<T>(int value) where T : struct, IConvertible
        {
            Type enumType = typeof(T);
            if (!Enum.IsDefined(enumType, value))
            {
                throw new ArgumentException("整形值在相应的枚举里面未定义！");
            }

            return (T)Enum.ToObject(enumType, value);
        }

    }
}
