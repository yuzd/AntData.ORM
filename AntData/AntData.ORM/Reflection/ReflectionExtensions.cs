using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Collections;
using AntData.ORM.Extensions;

namespace AntData.ORM.Reflection
{
	/// <summary>
	/// 反射优化相关扩展方法
	/// </summary>
	public static class ReflectionExtensions
	{
		/// <summary>
		/// 返回MemberInfo对象指定类型的Attribute
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="m"></param>
		/// <param name="inherit"></param>
		/// <returns></returns>
		public static T GetMyAttribute<T>(this MemberInfo m, bool inherit) where T : Attribute
		{
			T[] array = m.GetCustomAttributes(typeof(T), inherit) as T[];

			if( array.Length == 1 )
				return array[0];

			if( array.Length > 1 )
				throw new InvalidProgramException(string.Format("方法 {0} 不能同时指定多次 [{1}]。", m.Name, typeof(T)));

			return default(T);
		}

		/// <summary>
		/// 返回MemberInfo对象指定类型的Attribute实例
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="m"></param>
		/// <returns></returns>
		public static T GetMyAttribute<T>(this MemberInfo m) where T : Attribute
		{
			return GetMyAttribute<T>(m, false);
		}

		/// <summary>
		/// 返回MemberInfo对象指定类型的Attribute实例
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="m"></param>
		/// <param name="inherit"></param>
		/// <returns></returns>
		public static T[] GetMyAttributes<T>(this MemberInfo m, bool inherit) where T : Attribute
		{
			return m.GetCustomAttributes(typeof(T), inherit) as T[];
		}

		/// <summary>
		/// 返回MemberInfo对象指定类型的Attribute实例集合
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="m"></param>
		/// <returns></returns>
		public static T[] GetMyAttributes<T>(this MemberInfo m) where T : Attribute
		{
			return m.GetCustomAttributes(typeof(T), false) as T[];
		}

		/// <summary>
		/// 返回指定类型是否为可控类型
		/// </summary>
		/// <param name="nullableType"></param>
		/// <returns></returns>
		public static bool IsNullableType(this Type nullableType)
		{
			return ((nullableType.IsGenericTypeEx() && !nullableType.IsGenericTypeDefinitionEx())
				&& (nullableType.GetGenericTypeDefinition() == typeof(Nullable<>)));
		}

		/// <summary>
		/// 返回指定类型是否支持二进制序列化
		/// </summary>
		/// <param name="testType"></param>
		/// <returns></returns>
		public static bool IsSupportBinSerializable(this Type testType)
		{
			return testType.GetAttributes<SerializableAttribute>() != null;
		}


        #region 缓存集合

        private static readonly Hashtable s_getterDict = Hashtable.Synchronized(new Hashtable(10240));
        private static readonly Hashtable s_setterDict = Hashtable.Synchronized(new Hashtable(10240));
        private static readonly Hashtable s_methodDict = Hashtable.Synchronized(new Hashtable(10240));
        private static readonly Hashtable s_propertyDict = Hashtable.Synchronized(new Hashtable(10240)); 

        #endregion

        /// <summary>
        /// 返回FieldInfo实例反射优化后的GetValue调用结果
        /// </summary>
        /// <param name="fieldInfo">FieldInfo对象实例</param>
        /// <param name="obj">调用参数,用于数组索引器等成员</param>
        /// <returns>调用结果</returns>
        public static object FastGetValue(this FieldInfo fieldInfo, object obj)
		{
			if( fieldInfo == null )
				throw new ArgumentNullException("fieldInfo");

			GetValueDelegate getter = (GetValueDelegate)s_getterDict[fieldInfo];
			if( getter == null ) {
				getter = DynamicMethodFactory.CreateFieldGetter(fieldInfo);
				s_getterDict[fieldInfo] = getter;
			}

			return getter(obj);
		}

		/// <summary>
		/// 使用反射优化的方式对FieldInfo实例赋值
		/// </summary>
		/// <param name="fieldInfo">FieldInfo对象实例</param>
		/// <param name="obj">调用参数,用于数组索引器等成员</param>
		/// <param name="value">对象值</param>
		public static void FastSetField(this FieldInfo fieldInfo, object obj, object value)
		{
			if( fieldInfo == null )
				throw new ArgumentNullException("fieldInfo");

			SetValueDelegate setter = (SetValueDelegate)s_setterDict[fieldInfo];
			if( setter == null ) {
				setter = DynamicMethodFactory.CreateFieldSetter(fieldInfo);
				s_setterDict[fieldInfo] = setter;
			}

			setter(obj, value);
		}

		/// <summary>
		/// 返回使用反射优化后动态创建对应类型实例的结果
		/// </summary>
		/// <param name="instanceType">类型</param>
		/// <returns>类型实例</returns>
		public static object FastNew(this Type instanceType)
		{
			if( instanceType == null )
				throw new ArgumentNullException("instanceType");

			CtorDelegate ctor = (CtorDelegate)s_methodDict[instanceType];
			if( ctor == null ) {
				ConstructorInfo ctorInfo = instanceType.GetConstructor(Type.EmptyTypes);
				ctor = DynamicMethodFactory.CreateConstructor(ctorInfo);
				s_methodDict[instanceType] = ctor;
			}

			return ctor();
		}


        /// <summary>
        /// 返回类型可写属性
        /// </summary>
        /// <param name="type">类型</param>
        /// <returns>属性</returns>
        public static PropertyInfo[] GetCanWritePropertyInfo(this Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }
            PropertyInfo[] properties = (PropertyInfo[])s_propertyDict[type];
            if (properties == null)
            {
                properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.Static).Where(e => e.CanWrite).ToArray();
                s_propertyDict[type] = properties;
            }
            return properties;    
        }

        /// <summary>
        /// 返回类型的属性
        /// </summary>
        /// <param name="type">类型</param>
        /// <returns></returns>
        public static PropertyInfo[] GetCanReadPropertyInfo(this Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }
            PropertyInfo[] properties = (PropertyInfo[])s_propertyDict[type];
            if (properties == null)
            {
                properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.Static).ToArray();
                s_propertyDict[type] = properties;
            }
            return properties;
        }
        /// <summary>
        /// 根据指定的MethodInfo以及参数数组，快速调用相关的方法。
        /// </summary>
        /// <param name="methodInfo">MethodInfo实例成员</param>
        /// <param name="obj">目标实例成员</param>
        /// <param name="parameters">函数参数</param>
        /// <returns>调用结果</returns>
        public static object FastInvoke(this MethodInfo methodInfo, object obj, params object[] parameters)
		{
			if( methodInfo == null )
				throw new ArgumentNullException("methodInfo");

			IInvokeMethod method = MethodInvokerFactory.GetMethodInvokerWrapper(methodInfo);
			return method.Invoke(obj, parameters);
		}


		/// <summary>
		/// 快速调用PropertyInfo的GetValue方法
		/// </summary>
		/// <param name="propertyInfo"></param>
		/// <param name="obj"></param>
		/// <returns></returns>
		public static object FastGetValue(this PropertyInfo propertyInfo, object obj)
		{
			if( propertyInfo == null )
				throw new ArgumentNullException("propertyInfo");

			return GetterSetterFactory.GetPropertyGetterWrapper(propertyInfo).Get(obj);
		}

		/// <summary>
		/// 快速调用PropertyInfo的SetValue方法
		/// </summary>
		/// <param name="propertyInfo"></param>
		/// <param name="obj"></param>
		/// <param name="value"></param>
		public static void FastSetValue(this PropertyInfo propertyInfo, object obj, object value)
		{
			if( propertyInfo == null )
				throw new ArgumentNullException("propertyInfo");

			GetterSetterFactory.GetPropertySetterWrapper(propertyInfo).Set(obj, value);
		}

        /// <summary>
        /// 获取程序集CustomAttribute属性
        /// </summary>
        /// <typeparam name="T">CustomAttribute</typeparam>
        /// <param name="provider">Type、Assembly、Module、MethodInfo</param>
        /// <returns>CustomAttribute</returns>
        public static T GetCustomAttribute<T>(this ICustomAttributeProvider provider) where T : Attribute
        {
            var attributes = provider.GetCustomAttributes(typeof(T), false);
            return attributes.Length > 0 ? attributes[0] as T : default(T);
        }

        /// <summary>
        /// 根据对象的属性名称获取指
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public static object EmitGet(this object instance, string propertyName)
        {
            return instance.GetType().GetCanWritePropertyInfo().First(r=>r.Name.Equals(propertyName)).FastGetValue(instance);
        }

        /// <summary>
        /// 根据对象的属性名称设值
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="propertyName"></param>
        /// <param name="value"></param>
        public static void EmitSet(this object instance, string propertyName, object value)
        {
            instance.GetType().GetCanWritePropertyInfo().First(r => r.Name.Equals(propertyName)).FastSetValue(instance,value);
        }
    }
}
