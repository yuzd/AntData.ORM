using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
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

        public static string GetPath(this Assembly assembly)
        {
            return Path.GetDirectoryName(assembly.GetFileName());
        }

        public static string GetFileName(this Assembly assembly)
        {
            return assembly.CodeBase.GetPathFromUri();
        }

        public static string GetPathFromUri(this string uriString)
        {
            try
            {
                var uri = new Uri(Uri.EscapeUriString(uriString));
                var path =
                      Uri.UnescapeDataString(uri.AbsolutePath)
                    + Uri.UnescapeDataString(uri.Query)
                    + Uri.UnescapeDataString(uri.Fragment);

                return Path.GetFullPath(path);
            }
            catch (Exception ex)
            {
                throw new LinqToDBException("Error while trying to extract path from " + uriString + " " + ex.Message, ex);
            }
        }
        public static string ReplaceNumbers(this string st)
        {
            return Regex.Replace(st, @"\d", "");
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
