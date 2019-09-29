//-----------------------------------------------------------------------
// <copyright file="AntOrmServiceCollectionExtensions.cs" company="Company">
// Copyright (C) Company. All Rights Reserved.
// </copyright>
// <author>nainaigu</author>
// <summary></summary>
//-----------------------------------------------------------------------

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using AntData.ORM;
using AntData.ORM.Data;
using AntData.ORM.DataProvider;
using AntData.ORM.DataProvider.MySql;
using AntData.ORM.DataProvider.Oracle;
using AntData.ORM.DataProvider.PostgreSQL;
using AntData.ORM.DataProvider.SqlServer;
using AntData.ORM.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection
{
    internal static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddSingletonFactory<T, TFactory>(this IServiceCollection collection)
            where T : class where TFactory : class, IServiceFactory
        {
            collection.AddTransient<TFactory>();
            return AddInternal<T, TFactory>(collection, p => p.GetRequiredService<TFactory>(), ServiceLifetime.Singleton);
        }

        public static IServiceCollection AddSingletonFactory<T, TFactory>(this IServiceCollection collection, TFactory factory)
            where T : class where TFactory : class, IServiceFactory
        {
            return AddInternal<T, TFactory>(collection, p => factory, ServiceLifetime.Singleton);
        }

        public static IServiceCollection AddTransientFactory<T, TFactory>(this IServiceCollection collection)
            where T : class where TFactory : class, IServiceFactory
        {
            collection.AddTransient<TFactory>();
            return AddInternal<T, TFactory>(collection, p => p.GetRequiredService<TFactory>(), ServiceLifetime.Transient);
        }

        public static IServiceCollection AddTransientFactory<T, TFactory>(this IServiceCollection collection, TFactory factory)
            where T : class where TFactory : class, IServiceFactory
        {
            return AddInternal<T, TFactory>(collection, p => factory, ServiceLifetime.Transient);
        }

        public static IServiceCollection AddScopedFactory<T, TFactory>(this IServiceCollection collection)
            where T : class where TFactory : class, IServiceFactory
        {
            collection.AddTransient<TFactory>();
            return AddInternal<T, TFactory>(collection, p => p.GetRequiredService<TFactory>(), ServiceLifetime.Scoped);
        }

        public static IServiceCollection AddScopedFactory<T, TFactory>(this IServiceCollection collection, TFactory factory)
            where T : class where TFactory : class, IServiceFactory
        {
            return AddInternal<T, TFactory>(collection, p => factory, ServiceLifetime.Scoped);
        }

        private static IServiceCollection AddInternal<T, TFactory>(
            this IServiceCollection collection,
            Func<IServiceProvider, TFactory> factoryProvider,
            ServiceLifetime lifetime) where T : class where TFactory : class, IServiceFactory
        {
            Func<IServiceProvider, object> factoryFunc = provider =>
            {
                var factory = factoryProvider(provider);
                return factory.Build<T>();
            };
            var descriptor = new ServiceDescriptor(
                typeof(T), factoryFunc, lifetime);
            collection.Add(descriptor);
            return collection;
        }
    }
    public interface IServiceFactory
    {
        object Build<T>() where T : class;
    }

    public class DbContextFactory<T> : IServiceFactory
    {
        private readonly IDataProvider _dataProvider;
        private readonly DbContextOptions _dbContextOptions;
        private static readonly ConcurrentDictionary<string, ConstructorInfo> _dbConstructDic = new ConcurrentDictionary<string, ConstructorInfo>();

        public DbContextFactory(IDataProvider dataProvider, DbContextOptions dbContextOptions)
        {
            _dataProvider = dataProvider;
            _dbContextOptions = dbContextOptions;
        }

#pragma warning disable 693
        public object Build<T>() where T : class
#pragma warning restore 693
        {
            var type = typeof(T);
            if (!type.IsGenericType)
            {
                return null;
            }
            var genericType = type.GetGenericArguments()[0];
            Type contextType;
            ConstructorInfo contextTypeConstr = null;
            DbContext dbcontext = null;
            switch (_dataProvider.Name)
            {
                case ProviderName.MySql:
                    if (!_dbConstructDic.TryGetValue(ProviderName.MySql, out contextTypeConstr))
                    {
                        contextType = typeof(MysqlDbContext<>).MakeGenericType(genericType);
                        contextTypeConstr = contextType.GetConstructor(new Type[] { typeof(String) });
                        _dbConstructDic.TryAdd(ProviderName.MySql, contextTypeConstr);
                    }
                    break;
                case ProviderName.SqlServer:
                case ProviderName.SqlServer2008:
                case ProviderName.SqlServer2000:
                case ProviderName.SqlServer2012:
                case ProviderName.SqlServer2014:
                    if (!_dbConstructDic.TryGetValue(ProviderName.SqlServer, out contextTypeConstr))
                    {
                        contextType = typeof(SqlServerlDbContext<>).MakeGenericType(genericType);
                        contextTypeConstr = contextType.GetConstructor(new Type[] { typeof(String) });
                        _dbConstructDic.TryAdd(ProviderName.SqlServer, contextTypeConstr);
                    }
                    break;
                case ProviderName.Oracle:
                case ProviderName.OracleManaged:
                case ProviderName.OracleNative:
                    if (!_dbConstructDic.TryGetValue(ProviderName.Oracle, out contextTypeConstr))
                    {
                        contextType = typeof(OracleDbContext<>).MakeGenericType(genericType);
                        contextTypeConstr = contextType.GetConstructor(new Type[] { typeof(String) });
                        _dbConstructDic.TryAdd(ProviderName.Oracle, contextTypeConstr);
                    }
                    break;
                case ProviderName.PostgreSQL:
                case ProviderName.PostgreSQL92:
                case ProviderName.PostgreSQL93:
                    if (!_dbConstructDic.TryGetValue(ProviderName.PostgreSQL, out contextTypeConstr))
                    {
                        contextType = typeof(PostgreDbContext<>).MakeGenericType(genericType);
                        contextTypeConstr = contextType.GetConstructor(new Type[] { typeof(String) });
                        _dbConstructDic.TryAdd(ProviderName.PostgreSQL, contextTypeConstr);
                    }
                    break;
                default:
                    break;
            }
            if (contextTypeConstr == null) return null;
            dbcontext = contextTypeConstr.Invoke(new object[] { _dbContextOptions.Name }) as DbContext;
            if (dbcontext == null) return null;
            dbcontext.IsEnableLogTrace = _dbContextOptions.IsEnableLogTrace;
            dbcontext.OnLogTrace = _dbContextOptions.OnLogTrace;
            return dbcontext;
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
        /// 使用配置
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseAntData(this IApplicationBuilder app)
        {
            var Configuration = app.ApplicationServices.GetService<IConfiguration>();
            AntData.ORM.Common.Configuration.UseDBConfig(Configuration);
            return app;
        }

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
        public static IServiceCollection AddMysqlEntitys<T>(this IServiceCollection serviceCollection, string mappingName,Action<DbContextOptions> opsAction = null, ServiceLifetime contextLifetime = ServiceLifetime.Transient, ServiceLifetime optionsLifetime = ServiceLifetime.Transient) where T : class 
        {
            if (serviceCollection == null)
            {
                throw new ArgumentException("serviceCollection is null");
            }
            serviceCollection.AddSingleton<IDataProvider>(new MySqlDataProvider());
            var dbOptions = new DbContextOptions(mappingName);
            opsAction?.Invoke(dbOptions);
            serviceCollection.AddSingleton<DbContextOptions>(dbOptions);
            serviceCollection.AddTransientFactory<DbContext<T>, DbContextFactory<T>>();
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
        public static IServiceCollection AddSqlServerEntitys<T>(this IServiceCollection serviceCollection, string mappingName, Action<DbContextOptions> opsAction = null, ServiceLifetime contextLifetime = ServiceLifetime.Transient, ServiceLifetime optionsLifetime = ServiceLifetime.Transient) where T : class
        {
            if (serviceCollection == null)
            {
                throw new ArgumentException("serviceCollection is null");
            }
            serviceCollection.AddSingleton<IDataProvider>(new SqlServerDataProvider(SqlServerVersion.v2008));
            var dbOptions = new DbContextOptions(mappingName);
            opsAction?.Invoke(dbOptions);
            serviceCollection.AddSingleton<DbContextOptions>(dbOptions);
            serviceCollection.AddTransientFactory<DbContext<T>, DbContextFactory<T>>();
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
        public static IServiceCollection AddOracleEntitys<T>(this IServiceCollection serviceCollection, string mappingName, Action<DbContextOptions> opsAction = null, ServiceLifetime contextLifetime = ServiceLifetime.Transient, ServiceLifetime optionsLifetime = ServiceLifetime.Transient) where T : class
        {
            if (serviceCollection == null)
            {
                throw new ArgumentException("serviceCollection is null");
            }
            serviceCollection.AddSingleton<IDataProvider>(new OracleDataProvider());
            var dbOptions = new DbContextOptions(mappingName);
            opsAction?.Invoke(dbOptions);
            serviceCollection.AddSingleton<DbContextOptions>(dbOptions);
            serviceCollection.AddTransientFactory<DbContext<T>, DbContextFactory<T>>();
            return serviceCollection;
        }

        /// <summary>
        /// 添加Postgre entity 到DI
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="serviceCollection"></param>
        /// <param name="mappingName"></param>
        /// <param name="opsAction"></param>
        /// <param name="contextLifetime"></param>
        /// <param name="optionsLifetime"></param>
        /// <returns></returns>
        public static IServiceCollection AddPostgreSQLEntitys<T>(this IServiceCollection serviceCollection, string mappingName, Action<DbContextOptions> opsAction = null, ServiceLifetime contextLifetime = ServiceLifetime.Transient, ServiceLifetime optionsLifetime = ServiceLifetime.Transient) where T : class
        {
            if (serviceCollection == null)
            {
                throw new ArgumentException("serviceCollection is null");
            }
            serviceCollection.AddSingleton<IDataProvider>(new PostgreSQLDataProvider());
            serviceCollection.TryAdd(new ServiceDescriptor(typeof(T), typeof(T), contextLifetime));
            var dbOptions = new DbContextOptions(mappingName);
            opsAction?.Invoke(dbOptions);
            serviceCollection.AddSingleton<DbContextOptions>(dbOptions);
            serviceCollection.AddTransientFactory<DbContext<T>, DbContextFactory<T>>();
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