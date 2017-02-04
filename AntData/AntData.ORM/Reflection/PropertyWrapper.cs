using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Reflection;
using System.Collections;

namespace AntData.ORM.Reflection
{
    /// <summary>
    /// 定义读属性操作的接口
    /// </summary>
    internal interface IGetValue
    {
        object Get(object target);
    }
    /// <summary>
    /// 定义写属性操作的接口
    /// </summary>
    internal interface ISetValue
    {
        void Set(object target, object val);
    }


    /// <summary>
    /// 创建IGetValue或者ISetValue实例的工厂方法类
    /// </summary>
    internal static class GetterSetterFactory
    {
        private static readonly Hashtable s_getterDict = Hashtable.Synchronized(new Hashtable(10240));
        private static readonly Hashtable s_setterDict = Hashtable.Synchronized(new Hashtable(10240));

        internal static IGetValue GetPropertyGetterWrapper(PropertyInfo propertyInfo)
        {
            IGetValue property = (IGetValue)s_getterDict[propertyInfo];
            if (property == null)
            {
                property = CreatePropertyGetterWrapper(propertyInfo);
                s_getterDict[propertyInfo] = property;
            }
            return property;
        }

        internal static ISetValue GetPropertySetterWrapper(PropertyInfo propertyInfo)
        {
            ISetValue property = (ISetValue)s_setterDict[propertyInfo];
            if (property == null)
            {
                property = CreatePropertySetterWrapper(propertyInfo);
                s_setterDict[propertyInfo] = property;
            }
            return property;
        }

        /// <summary>
        /// 根据指定的PropertyInfo对象，返回对应的IGetValue实例
        /// </summary>
        /// <param name="propertyInfo"></param>
        /// <returns></returns>
        public static IGetValue CreatePropertyGetterWrapper(PropertyInfo propertyInfo)
        {
            if (propertyInfo == null)
                throw new ArgumentNullException("propertyInfo");
            if (propertyInfo.CanRead == false)
                throw new InvalidOperationException("属性不支持读操作。");

            MethodInfo mi = propertyInfo.GetGetMethod(true);

            if (mi.GetParameters().Length > 0)
                throw new NotSupportedException("不支持构造索引器属性的委托。");

            if (mi.IsStatic)
            {
                Type instanceType = typeof(StaticGetterWrapper<>).MakeGenericType(propertyInfo.PropertyType);
                return (IGetValue)Activator.CreateInstance(instanceType, propertyInfo);
            }
            else
            {
                Type instanceType = typeof(GetterWrapper<,>).MakeGenericType(propertyInfo.DeclaringType, propertyInfo.PropertyType);
                return (IGetValue)Activator.CreateInstance(instanceType, propertyInfo);
            }
        }

        /// <summary>
        /// 根据指定的PropertyInfo对象，返回对应的ISetValue实例
        /// </summary>
        /// <param name="propertyInfo"></param>
        /// <returns></returns>
        public static ISetValue CreatePropertySetterWrapper(PropertyInfo propertyInfo)
        {
            if (propertyInfo == null)
                throw new ArgumentNullException("propertyInfo");
            if (propertyInfo.CanWrite == false)
                throw new NotSupportedException("属性不支持写操作。");

            MethodInfo mi = propertyInfo.GetSetMethod(true);

            if (mi.GetParameters().Length > 1)
                throw new NotSupportedException("不支持构造索引器属性的委托。");

            if (mi.IsStatic)
            {
                Type instanceType = typeof(StaticSetterWrapper<>).MakeGenericType(propertyInfo.PropertyType);
                return (ISetValue)Activator.CreateInstance(instanceType, propertyInfo);
            }
            else
            {
                Type instanceType = typeof(SetterWrapper<,>).MakeGenericType(propertyInfo.DeclaringType, propertyInfo.PropertyType);
                return (ISetValue)Activator.CreateInstance(instanceType, propertyInfo);
            }
        }
    }


    internal class GetterWrapper<TTarget, TValue> : IGetValue
    {
        private Func<TTarget, TValue> _getter;

        public GetterWrapper(PropertyInfo propertyInfo)
        {
            if (propertyInfo == null)
                throw new ArgumentNullException("propertyInfo");

            if (propertyInfo.CanRead == false)
                throw new InvalidOperationException("属性不支持读操作。");
#if !NETSTANDARD
            MethodInfo m = propertyInfo.GetGetMethod(true);
            _getter = (Func<TTarget, TValue>)Delegate.CreateDelegate(typeof(Func<TTarget, TValue>), null, m);
#else
            _getter = (Func<TTarget, TValue>)DynamicMethodFactory.CreatePropertyGetter(typeof(Func<TTarget, TValue>), propertyInfo);
#endif

        }

        public TValue GetValue(TTarget target)
        {
            return _getter(target);
        }
        object IGetValue.Get(object target)
        {
            return _getter((TTarget)target);
        }
    }

    internal class SetterWrapper<TTarget, TValue> : ISetValue
    {
        private Action<TTarget, TValue> _setter;

        public SetterWrapper(PropertyInfo propertyInfo)
        {
            if (propertyInfo == null)
                throw new ArgumentNullException("propertyInfo");

            if (propertyInfo.CanWrite == false)
                throw new NotSupportedException("属性不支持写操作。");
#if !NETSTANDARD
            MethodInfo m = propertyInfo.GetSetMethod(true);
            _setter = (Action<TTarget, TValue>)Delegate.CreateDelegate(typeof(Action<TTarget, TValue>), null, m);
#else
            _setter = (Action<TTarget, TValue>)DynamicMethodFactory.CreatePropertySetter(typeof(Action<TTarget, TValue>), propertyInfo);
#endif

        }

        public void SetValue(TTarget target, TValue val)
        {
            _setter(target, val);
        }
        void ISetValue.Set(object target, object val)
        {
            _setter((TTarget)target, (TValue)val);
        }
    }


    internal class StaticGetterWrapper<TValue> : IGetValue
    {
        private Func<TValue> _getter;

        public StaticGetterWrapper(PropertyInfo propertyInfo)
        {
            if (propertyInfo == null)
                throw new ArgumentNullException("propertyInfo");

            if (propertyInfo.CanRead == false)
                throw new InvalidOperationException("属性不支持读操作。");
#if !NETSTANDARD
			MethodInfo m = propertyInfo.GetGetMethod(true);

		    _getter = (Func<TValue>)Delegate.CreateDelegate(typeof(Func<TValue>), m);
#else
            _getter = (Func<TValue>)DynamicMethodFactory.CreatePropertyGetter(typeof(Func<TValue>), propertyInfo,true);
#endif

        }

        public TValue GetValue()
        {
            return _getter();
        }
        object IGetValue.Get(object target)
        {
            return _getter();
        }
    }

    internal class StaticSetterWrapper<TValue> : ISetValue
    {
        private Action<TValue> _setter;

        public StaticSetterWrapper(PropertyInfo propertyInfo)
        {
            if (propertyInfo == null)
                throw new ArgumentNullException("propertyInfo");

            if (propertyInfo.CanWrite == false)
                throw new NotSupportedException("属性不支持写操作。");
#if !NETSTANDARD
            MethodInfo m = propertyInfo.GetSetMethod(true);
            _setter = (Action<TValue>)Delegate.CreateDelegate(typeof(Action<TValue>), m);
#else
            _setter = (Action<TValue>)DynamicMethodFactory.CreatePropertySetter(typeof(Func<TValue>), propertyInfo,true);
#endif

        }

        public void SetValue(TValue val)
        {
            _setter(val);
        }
        void ISetValue.Set(object target, object val)
        {
            _setter((TValue)val);
        }
    }
}
