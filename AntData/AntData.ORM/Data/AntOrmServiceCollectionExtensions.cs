//-----------------------------------------------------------------------
// <copyright file="AntOrmServiceCollectionExtensions.cs" company="Company">
// Copyright (C) Company. All Rights Reserved.
// </copyright>
// <author>nainaigu</author>
// <summary></summary>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AntData.ORM;
using AntData.ORM.Data;
using AntData.ORM.DataProvider;
using AntData.ORM.DataProvider.MySql;
using AntData.ORM.DataProvider.Oracle;
using AntData.ORM.DataProvider.SqlServer;
using AntData.ORM.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddSingletonFactory<T, TFactory>(this IServiceCollection collection)
            where T : class where TFactory : class, IServiceFactory<T>
        {
            collection.AddTransient<TFactory>();
            return AddInternal<T, TFactory>(collection, p => p.GetRequiredService<TFactory>(), ServiceLifetime.Singleton);
        }

        public static IServiceCollection AddSingletonFactory<T, TFactory>(this IServiceCollection collection, TFactory factory)
            where T : class where TFactory : class, IServiceFactory<T>
        {
            return AddInternal<T, TFactory>(collection, p => factory, ServiceLifetime.Singleton);
        }

        public static IServiceCollection AddTransientFactory<T, TFactory>(this IServiceCollection collection)
            where T : class where TFactory : class, IServiceFactory<T>
        {
            collection.AddTransient<TFactory>();
            return AddInternal<T, TFactory>(collection, p => p.GetRequiredService<TFactory>(), ServiceLifetime.Transient);
        }

        public static IServiceCollection AddTransientFactory<T, TFactory>(this IServiceCollection collection, TFactory factory)
            where T : class where TFactory : class, IServiceFactory<T>
        {
            return AddInternal<T, TFactory>(collection, p => factory, ServiceLifetime.Transient);
        }

        public static IServiceCollection AddScopedFactory<T, TFactory>(this IServiceCollection collection)
            where T : class where TFactory : class, IServiceFactory<T>
        {
            collection.AddTransient<TFactory>();
            return AddInternal<T, TFactory>(collection, p => p.GetRequiredService<TFactory>(), ServiceLifetime.Scoped);
        }

        public static IServiceCollection AddScopedFactory<T, TFactory>(this IServiceCollection collection, TFactory factory)
            where T : class where TFactory : class, IServiceFactory<T>
        {
            return AddInternal<T, TFactory>(collection, p => factory, ServiceLifetime.Scoped);
        }

        private static IServiceCollection AddInternal<T, TFactory>(
            this IServiceCollection collection,
            Func<IServiceProvider, TFactory> factoryProvider,
            ServiceLifetime lifetime) where T : class where TFactory : class, IServiceFactory<T>
        {
            Func<IServiceProvider, object> factoryFunc = provider =>
            {
                var factory = factoryProvider(provider);
                return factory.Build();
            };
            var descriptor = new ServiceDescriptor(
                typeof(T), factoryFunc, lifetime);
            collection.Add(descriptor);
            return collection;
        }
    }
    public interface IServiceFactory<T> where T : class
    {
        T Build();
    }

    public class DbContextFactory : IServiceFactory<DataConnection>
    {
        private readonly IDataProvider _dataProvider;
        private readonly DbContextOptions _dbContextOptions;

        public DbContextFactory(IDataProvider dataProvider, DbContextOptions dbContextOptions)
        {
            _dataProvider = dataProvider;
            _dbContextOptions = dbContextOptions;
        }

        public DataConnection Build()
        {
            return new DataConnection(_dataProvider, _dbContextOptions.Name)
            {
                IsEnableLogTrace = _dbContextOptions.IsEnableLogTrace,
                OnLogTrace = _dbContextOptions.OnLogTrace
            };
        }
    }

    public class DbContextOptions
    {
        
        public DbContextOptions(string name)
        {
            Name = name;
        }

        public string Name { get; private set; }
        public bool IsEnableLogTrace { get; set; }
        public Action<CustomerTraceInfo> OnLogTrace { get; set; }
    }
    /// <summary>
    /// 依赖注入
    /// </summary>
    public static class AntOrmServiceCollectionExtensions
    {
        /// <summary>
        /// 添加Msyql entity到DI
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="serviceCollection"></param>
        /// <param name="mappingName">逻辑数据库名称</param>
        /// <param name="opsAction"></param>
        /// <param name="contextLifetime">默认每次获取都是新的实例</param>
        /// <param name="optionsLifetime">默认每次获取都是新的实例</param>
        /// <returns></returns>
        public static IServiceCollection AddMysqlEntitys<T>(this IServiceCollection serviceCollection, string mappingName,Action<DbContextOptions> opsAction = null, ServiceLifetime contextLifetime = ServiceLifetime.Transient, ServiceLifetime optionsLifetime = ServiceLifetime.Transient) where T : IEntity
        {
            if (serviceCollection == null)
            {
                throw new ArgumentException("serviceCollection is null");
            }
            serviceCollection.AddSingleton<IDataProvider>(new MySqlDataProvider());
            var dbOptions = new DbContextOptions(mappingName);
            if (opsAction!=null)
            {
                opsAction(dbOptions);
            }
            serviceCollection.AddSingleton<DbContextOptions>(dbOptions);
            serviceCollection.AddTransientFactory<DataConnection, DbContextFactory>();
            serviceCollection.TryAdd(new ServiceDescriptor(typeof(T), typeof(T), contextLifetime));
            return serviceCollection;
        }

        /// <summary>
        /// 添加sqlserver entity 到DI
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="serviceCollection"></param>
        /// <param name="mappingName">逻辑数据库名称</param>
        /// <param name="opsAction"></param>
        /// <param name="contextLifetime">默认每次获取都是新的实例</param>
        /// <param name="optionsLifetime">默认每次获取都是新的实例</param>
        /// <returns></returns>
        public static IServiceCollection AddSqlServerEntitys<T>(this IServiceCollection serviceCollection, string mappingName, Action<DbContextOptions> opsAction = null, ServiceLifetime contextLifetime = ServiceLifetime.Transient, ServiceLifetime optionsLifetime = ServiceLifetime.Transient) where T : IEntity
        {
            if (serviceCollection == null)
            {
                throw new ArgumentException("serviceCollection is null");
            }
            serviceCollection.AddSingleton<IDataProvider>(new SqlServerDataProvider(SqlServerVersion.v2008));
            serviceCollection.TryAdd(new ServiceDescriptor(typeof(T), typeof(T), contextLifetime));
            var dbOptions = new DbContextOptions(mappingName);
            if (opsAction != null)
            {
                opsAction(dbOptions);
            }
            serviceCollection.AddSingleton<DbContextOptions>(dbOptions);
            serviceCollection.AddTransientFactory<DataConnection, DbContextFactory>();
            serviceCollection.TryAdd(new ServiceDescriptor(typeof(T), typeof(T), contextLifetime));
            return serviceCollection;
        }

        /// <summary>
        /// 添加oracle entity 到DI
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="serviceCollection"></param>
        /// <param name="mappingName">逻辑数据库名称</param>
        /// <param name="opsAction"></param>
        /// <param name="contextLifetime">默认每次获取都是新的实例</param>
        /// <param name="optionsLifetime">默认每次获取都是新的实例</param>
        /// <returns></returns>
        public static IServiceCollection AddOracleEntitys<T>(this IServiceCollection serviceCollection, string mappingName, Action<DbContextOptions> opsAction = null, ServiceLifetime contextLifetime = ServiceLifetime.Transient, ServiceLifetime optionsLifetime = ServiceLifetime.Transient) where T : IEntity
        {
            if (serviceCollection == null)
            {
                throw new ArgumentException("serviceCollection is null");
            }
            serviceCollection.AddSingleton<IDataProvider>(new OracleDataProvider());
            serviceCollection.TryAdd(new ServiceDescriptor(typeof(T), typeof(T), contextLifetime));
            var dbOptions = new DbContextOptions(mappingName);
            if (opsAction != null)
            {
                opsAction(dbOptions);
            }
            serviceCollection.AddSingleton<DbContextOptions>(dbOptions);
            serviceCollection.AddTransientFactory<DataConnection, DbContextFactory>();
            serviceCollection.TryAdd(new ServiceDescriptor(typeof(T), typeof(T), contextLifetime));
            return serviceCollection;
        }
        private static void CheckContextConstructors(Type type)
        {
            List<ConstructorInfo> list = type.GetTypeInfo().DeclaredConstructors.ToList<ConstructorInfo>();
            if (list.Count != 1)
                throw new ArgumentException("DbContext with wrong Constructor");
        }
    }
}