using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using AntData.ORM.Common;
using AntData.ORM.Reflection;
using JetBrains.Annotations;

namespace AntData.ORM
{
	using Expressions;
	using Linq;
	using Linq.Builder;

	[PublicAPI]
	public static class LinqExtensions
	{
		#region Table Helpers

		static readonly MethodInfo _tableNameMethodInfo = MemberHelper.MethodOf(() => TableName<int>(null, null)).GetGenericMethodDefinition();

		[LinqTunnel]
		public static ITable<T> TableName<T>([NotNull] this ITable<T> table, [NotNull] string name)
		{
			if (table == null) throw new ArgumentNullException("table");
			if (name  == null) throw new ArgumentNullException("name");

			table.Expression = Expression.Call(
				null,
				_tableNameMethodInfo.MakeGenericMethod(new[] { typeof(T) }),
				new[] { table.Expression, Expression.Constant(name) });

			var tbl = table as Table<T>;
			if (tbl != null)
				tbl.TableName = name;

			return table;
		}

		static readonly MethodInfo _databaseNameMethodInfo = MemberHelper.MethodOf(() => DatabaseName<int>(null, null)).GetGenericMethodDefinition();

		[LinqTunnel]
		public static ITable<T> DatabaseName<T>([NotNull] this ITable<T> table, [NotNull] string name)
		{
			if (table == null) throw new ArgumentNullException("table");
			if (name  == null) throw new ArgumentNullException("name");

			table.Expression = Expression.Call(
				null,
				_databaseNameMethodInfo.MakeGenericMethod(new[] { typeof(T) }),
				new[] { table.Expression, Expression.Constant(name) });

			var tbl = table as Table<T>;
			if (tbl != null)
				tbl.DatabaseName = name;

			return table;
		}

		static readonly MethodInfo _ownerNameMethodInfo = MemberHelper.MethodOf(() => OwnerName<int>(null, null)).GetGenericMethodDefinition();

		[LinqTunnel]
		public static ITable<T> OwnerName<T>([NotNull] this ITable<T> table, [NotNull] string name)
		{
			if (table == null) throw new ArgumentNullException("table");
			if (name  == null) throw new ArgumentNullException("name");

			table.Expression = Expression.Call(
				null,
				_ownerNameMethodInfo.MakeGenericMethod(new[] { typeof(T) }),
				new[] { table.Expression, Expression.Constant(name) });

			var tbl = table as Table<T>;
			if (tbl != null)
				tbl.SchemaName = name;

			return table;
		}

		static readonly MethodInfo _schemaNameMethodInfo = MemberHelper.MethodOf(() => SchemaName<int>(null, null)).GetGenericMethodDefinition();

		[LinqTunnel]
		public static ITable<T> SchemaName<T>([NotNull] this ITable<T> table, [NotNull] string name)
		{
			if (table == null) throw new ArgumentNullException("table");
			if (name  == null) throw new ArgumentNullException("name");

			table.Expression = Expression.Call(
				null,
				_schemaNameMethodInfo.MakeGenericMethod(new[] { typeof(T) }),
				new[] { table.Expression, Expression.Constant(name) });

			var tbl = table as Table<T>;
			if (tbl != null)
				tbl.SchemaName = name;

			return table;
		}

		static readonly MethodInfo _withTableExpressionMethodInfo = MemberHelper.MethodOf(() => WithTableExpression<int>(null, null)).GetGenericMethodDefinition();

		[LinqTunnel]
		public static ITable<T> WithTableExpression<T>([NotNull] this ITable<T> table, [NotNull] string expression)
		{
			if (expression == null) throw new ArgumentNullException("expression");

			table.Expression = Expression.Call(
				null,
				_withTableExpressionMethodInfo.MakeGenericMethod(new[] { typeof(T) }),
				new[] { table.Expression, Expression.Constant(expression) });

			return table;
		}

		static readonly MethodInfo _with = MemberHelper.MethodOf(() => With<int>(null, null)).GetGenericMethodDefinition();

		[LinqTunnel]
		public static ITable<T> With<T>([NotNull] this ITable<T> table, [NotNull] string args)
		{
			if (args == null) throw new ArgumentNullException("args");

			table.Expression = Expression.Call(
				null,
				_with.MakeGenericMethod(new[] { typeof(T) }),
				new[] { table.Expression, Expression.Constant(args) });

			return table;
		}

		#endregion

		#region LoadWith

		static readonly MethodInfo _loadWithMethodInfo = MemberHelper.MethodOf(() => LoadWith<int>(null, null)).GetGenericMethodDefinition();

        /// <summary>
        /// 加载外键
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="table"></param>
        /// <param name="selector">外键选择器</param>
        /// <returns></returns>
		[LinqTunnel]
		public static ITable<T> LoadWith<T>(
			[NotNull]                this ITable<T> table,
			[NotNull, InstantHandle] Expression<Func<T,object>> selector)
		{
			if (table == null) throw new ArgumentNullException("table");

			table.Expression = Expression.Call(
				null,
				_loadWithMethodInfo.MakeGenericMethod(new[] { typeof(T) }),
				new[] { table.Expression, Expression.Quote(selector) });

			return table;
		}
        [LinqTunnel]
        public static IQueryable<T> LoadWith<T>(
            [NotNull]                this IQueryable<T> query,
            [NotNull, InstantHandle] Expression<Func<T, object>> selector)
        {
            if (query == null) throw new ArgumentNullException("query");

            ITable<T> table = query as ITable<T>;

            if (table == null) throw new ArgumentNullException("IQueryable cast to ITable error");

            table.Expression = Expression.Call(
                null,
                _loadWithMethodInfo.MakeGenericMethod(new[] { typeof(T) }),
                new[] { table.Expression, Expression.Quote(selector) });

            return table;
        }
        #endregion

        #region Scalar Select
        /// <summary>
        /// 选择
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dataContext"></param>
        /// <param name="selector"></param>
        /// <returns></returns>
        public static T Select<T>(
			[NotNull]                this IDataContext   dataContext,
			[NotNull, InstantHandle] Expression<Func<T>> selector)
		{
			if (dataContext == null) throw new ArgumentNullException("dataContext");
			if (selector    == null) throw new ArgumentNullException("selector");

			var q = new Table<T>(dataContext, selector);

			foreach (var item in q)
				return item;

			throw new InvalidOperationException();
		}

		#endregion

		#region Delete

		static readonly MethodInfo _deleteMethodInfo = MemberHelper.MethodOf(() => Delete<int>(null)).GetGenericMethodDefinition();

        /// <summary>
        /// 执行删除 返回影响条数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
		public static int Delete<T>([NotNull] this IQueryable<T> source)
		{
			if (source == null) throw new ArgumentNullException("source");

			return source.Provider.Execute<int>(
				Expression.Call(
					null,
					_deleteMethodInfo.MakeGenericMethod(new[] { typeof(T) }),
					new[] { source.Expression }));
		}

		static readonly MethodInfo _deleteMethodInfo2 = MemberHelper.MethodOf(() => Delete<int>(null, null)).GetGenericMethodDefinition();

        /// <summary>
        /// 按条件删除 返回影响条数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="predicate">条件</param>
        /// <returns></returns>
		public static int Delete<T>(
			[NotNull]                this IQueryable<T>       source,
			[NotNull, InstantHandle] Expression<Func<T,bool>> predicate)
		{
			if (source    == null) throw new ArgumentNullException("source");
			if (predicate == null) throw new ArgumentNullException("predicate");

			return source.Provider.Execute<int>(
				Expression.Call(
					null,
					_deleteMethodInfo2.MakeGenericMethod(new[] { typeof(T) }),
					new[] { source.Expression, Expression.Quote(predicate) }));
		}

		#endregion

		#region Update

		static readonly MethodInfo _updateMethodInfo = MemberHelper.MethodOf(() => Update<int,int>(null, (ITable<int>)null, null)).GetGenericMethodDefinition();

        /// <summary>
        /// 指定更新的源 根据自定义的表达式去执行更新
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TTarget"></typeparam>
        /// <param name="source"></param>
        /// <param name="target">更新对象</param>
        /// <param name="setter">自定义的表达式从源获取要更新的目标</param>
        /// <returns></returns>
		public static int Update<TSource,TTarget>(
			[NotNull]                this IQueryable<TSource>          source,
			[NotNull]                ITable<TTarget>                   target,
			[NotNull, InstantHandle] Expression<Func<TSource,TTarget>> setter)
		{
			if (source == null) throw new ArgumentNullException("source");
			if (target == null) throw new ArgumentNullException("target");
			if (setter == null) throw new ArgumentNullException("setter");

			return source.Provider.Execute<int>(
				Expression.Call(
					null,
					_updateMethodInfo.MakeGenericMethod(new[] { typeof(TSource), typeof(TTarget) }),
					new[] { source.Expression, ((IQueryable<TTarget>)target).Expression, Expression.Quote(setter) }));
		}

		static readonly MethodInfo _updateMethodInfo2 = MemberHelper.MethodOf(() => Update<int>(null, null)).GetGenericMethodDefinition();

        /// <summary>
        /// 更新的指定字段
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="setter">设置更新的字段和更新的值</param>
        /// <returns></returns>
		public static int Update<T>(
			[NotNull]                this IQueryable<T>    source,
			[NotNull, InstantHandle] Expression<Func<T,T>> setter)
		{
			if (source == null) throw new ArgumentNullException("source");
			if (setter == null) throw new ArgumentNullException("setter");

			return source.Provider.Execute<int>(
				Expression.Call(
					null,
					_updateMethodInfo2.MakeGenericMethod(new[] { typeof(T) }),
					new[] { source.Expression, Expression.Quote(setter) }));
		}

		static readonly MethodInfo _updateMethodInfo3 = MemberHelper.MethodOf(() => Update<int>(null, null, null)).GetGenericMethodDefinition();

        /// <summary>
        /// 根据条件去更新的指定字段
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="predicate">指定条件</param>
        /// <param name="setter">设置更新的字段和更新的值</param>
        /// <returns></returns>
		public static int Update<T>(
			[NotNull]                this IQueryable<T>       source,
			[NotNull, InstantHandle] Expression<Func<T,bool>> predicate,
			[NotNull, InstantHandle] Expression<Func<T,T>>    setter)
		{
			if (source    == null) throw new ArgumentNullException("source");
			if (predicate == null) throw new ArgumentNullException("predicate");
			if (setter    == null) throw new ArgumentNullException("setter");

			return source.Provider.Execute<int>(
				Expression.Call(
					null,
					_updateMethodInfo3.MakeGenericMethod(new[] { typeof(T) }),
					new[] { source.Expression, Expression.Quote(predicate), Expression.Quote(setter) }));
		}

		static readonly MethodInfo _updateMethodInfo4 = MemberHelper.MethodOf(() => Update<int>(null)).GetGenericMethodDefinition();

        /// <summary>
        /// 执行更新操作 返回影响条数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
		public static int Update<T>([NotNull] this IUpdatable<T> source)
		{
			if (source == null) throw new ArgumentNullException("source");

			var query = ((Updatable<T>)source).Query;

			return query.Provider.Execute<int>(
				Expression.Call(
					null,
					_updateMethodInfo4.MakeGenericMethod(new[] { typeof(T) }),
					new[] { query.Expression }));
		}

		static readonly MethodInfo _updateMethodInfo5 = MemberHelper.MethodOf(() => Update<int,int>(null, (Expression<Func<int,int>>)null, null)).GetGenericMethodDefinition();

		public static int Update<TSource,TTarget>(
			[NotNull]                this IQueryable<TSource>          source,
			[NotNull, InstantHandle] Expression<Func<TSource,TTarget>> target,
			[NotNull, InstantHandle] Expression<Func<TSource,TTarget>> setter)
		{
			if (source == null) throw new ArgumentNullException("source");
			if (target == null) throw new ArgumentNullException("target");
			if (setter == null) throw new ArgumentNullException("setter");

			return source.Provider.Execute<int>(
				Expression.Call(
					null,
					_updateMethodInfo5.MakeGenericMethod(new[] { typeof(TSource), typeof(TTarget) }),
					new[] { source.Expression, Expression.Quote(target), Expression.Quote(setter) }));
		}

		class Updatable<T> : IUpdatable<T>
		{
			public IQueryable<T> Query;
		}

		static readonly MethodInfo _asUpdatableMethodInfo = MemberHelper.MethodOf(() => AsUpdatable<int>(null)).GetGenericMethodDefinition();

        /// <summary>
        /// 转换为IUpdatable类型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
		[LinqTunnel]
		public static IUpdatable<T> AsUpdatable<T>([NotNull] this IQueryable<T> source)
		{
			if (source  == null) throw new ArgumentNullException("source");

			var query = source.Provider.CreateQuery<T>(
				Expression.Call(
					null,
					_asUpdatableMethodInfo.MakeGenericMethod(new[] { typeof(T) }),
					new[] { source.Expression }));

			return new Updatable<T> { Query = query };
		}

		static readonly MethodInfo _setMethodInfo = MemberHelper.MethodOf(() =>
			Set<int,int>((IQueryable<int>)null,null,(Expression<Func<int,int>>)null)).GetGenericMethodDefinition();

        /// <summary>
        /// 根据表达式设置新的值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TV"></typeparam>
        /// <param name="source"></param>
        /// <param name="extract"></param>
        /// <param name="update"></param>
        /// <returns></returns>
		[LinqTunnel]
		public static IUpdatable<T> Set<T,TV>(
			[NotNull]                this IQueryable<T>     source,
			[NotNull, InstantHandle] Expression<Func<T,TV>> extract,
			[NotNull, InstantHandle] Expression<Func<T,TV>> update)
		{
			if (source  == null) throw new ArgumentNullException("source");
			if (extract == null) throw new ArgumentNullException("extract");
			if (update  == null) throw new ArgumentNullException("update");

			var query = source.Provider.CreateQuery<T>(
				Expression.Call(
					null,
					_setMethodInfo.MakeGenericMethod(new[] { typeof(T), typeof(TV) }),
					new[] { source.Expression, Expression.Quote(extract), Expression.Quote(update) }));

			return new Updatable<T> { Query = query };
		}

		static readonly MethodInfo _setMethodInfo2 = MemberHelper.MethodOf(() =>
			Set<int,int>((IUpdatable<int>)null,null,(Expression<Func<int,int>>)null)).GetGenericMethodDefinition();

        /// <summary>
        /// 设置新的值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TV"></typeparam>
        /// <param name="source"></param>
        /// <param name="extract">取出要设置的字段</param>
        /// <param name="update">根据取出的字段设置新的值</param>
        /// <returns></returns>
		[LinqTunnel]
		public static IUpdatable<T> Set<T,TV>(
			[NotNull]                this IUpdatable<T>    source,
			[NotNull, InstantHandle] Expression<Func<T,TV>> extract,
			[NotNull, InstantHandle] Expression<Func<T,TV>> update)
		{
			if (source  == null) throw new ArgumentNullException("source");
			if (extract == null) throw new ArgumentNullException("extract");
			if (update  == null) throw new ArgumentNullException("update");

			var query = ((Updatable<T>)source).Query;

			query = query.Provider.CreateQuery<T>(
				Expression.Call(
					null,
					_setMethodInfo2.MakeGenericMethod(new[] { typeof(T), typeof(TV) }),
					new[] { query.Expression, Expression.Quote(extract), Expression.Quote(update) }));

			return new Updatable<T> { Query = query };
		}

		static readonly MethodInfo _setMethodInfo3 = MemberHelper.MethodOf(() =>
			Set<int,int>((IQueryable<int>)null,null,(Expression<Func<int>>)null)).GetGenericMethodDefinition();

        /// <summary>
        /// 根据表达式设置新的值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TV"></typeparam>
        /// <param name="source"></param>
        /// <param name="extract">取出要更新值的字段表达式</param>
        /// <param name="update">设置新值的表达式</param>
        /// <returns></returns>
		[LinqTunnel]
		public static IUpdatable<T> Set<T,TV>(
			[NotNull]                this IQueryable<T>     source,
			[NotNull, InstantHandle] Expression<Func<T,TV>> extract,
			[NotNull, InstantHandle] Expression<Func<TV>>   update)
		{
			if (source  == null) throw new ArgumentNullException("source");
			if (extract == null) throw new ArgumentNullException("extract");
			if (update  == null) throw new ArgumentNullException("update");

			var query = source.Provider.CreateQuery<T>(
				Expression.Call(
					null,
					_setMethodInfo3.MakeGenericMethod(new[] { typeof(T), typeof(TV) }),
					new[] { source.Expression, Expression.Quote(extract), Expression.Quote(update) }));

			return new Updatable<T> { Query = query };
		}

		static readonly MethodInfo _setMethodInfo4 = MemberHelper.MethodOf(() =>
			Set<int,int>((IUpdatable<int>)null,null,(Expression<Func<int>>)null)).GetGenericMethodDefinition();

        /// <summary>
        /// 根据表达式设置新的值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TV"></typeparam>
        /// <param name="source"></param>
        /// <param name="extract">取出要设置的字段</param>
        /// <param name="update">根据取出的字段设置新的值</param>
        /// <returns></returns>
		[LinqTunnel]
		public static IUpdatable<T> Set<T,TV>(
			[NotNull]                this IUpdatable<T>    source,
			[NotNull, InstantHandle] Expression<Func<T,TV>> extract,
			[NotNull, InstantHandle] Expression<Func<TV>>   update)
		{
			if (source  == null) throw new ArgumentNullException("source");
			if (extract == null) throw new ArgumentNullException("extract");
			if (update  == null) throw new ArgumentNullException("update");

			var query = ((Updatable<T>)source).Query;

			query = query.Provider.CreateQuery<T>(
				Expression.Call(
					null,
					_setMethodInfo4.MakeGenericMethod(new[] { typeof(T), typeof(TV) }),
					new[] { query.Expression, Expression.Quote(extract), Expression.Quote(update) }));

			return new Updatable<T> { Query = query };
		}

		static readonly MethodInfo _setMethodInfo5 = MemberHelper.MethodOf(() => Set((IQueryable<int>)null,null,0)).GetGenericMethodDefinition();

        /// <summary>
        /// 根据表达式设置新的值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TV"></typeparam>
        /// <param name="source"></param>
        /// <param name="extract"></param>
        /// <param name="value"></param>
        /// <returns></returns>
		[LinqTunnel]
		public static IUpdatable<T> Set<T,TV>(
			[NotNull]                this IQueryable<T>     source,
			[NotNull, InstantHandle] Expression<Func<T,TV>> extract,
			TV                                              value)
		{
			if (source  == null) throw new ArgumentNullException("source");
			if (extract == null) throw new ArgumentNullException("extract");

			var query = source.Provider.CreateQuery<T>(
				Expression.Call(
					null,
					_setMethodInfo5.MakeGenericMethod(new[] { typeof(T), typeof(TV) }),
					new[] { source.Expression, Expression.Quote(extract), Expression.Constant(value, typeof(TV)) }));

			return new Updatable<T> { Query = query };
		}

        /// <summary>
        /// 根据字段名设置
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TV">如果是string 可以不传 其他类型不传报错</typeparam>
        /// <param name="source"></param>
        /// <param name="fieldName">字段名称</param>
        /// <param name="value">对应的值</param>
        /// <returns></returns>
        [LinqTunnel]
        public static IUpdatable<T> Set2<T, TV>(
            [NotNull]                this IQueryable<T> source,
            [NotNull, InstantHandle] string fieldName,
            TV value)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (string.IsNullOrEmpty(fieldName))
            {
                throw new ArgumentNullException("fieldName");
            }
            var expression = AntData.ORM.Expressions.Extensions.GenerateMemberExpression<T, TV>(fieldName);
            var query = source.Provider.CreateQuery<T>(
                Expression.Call(
                    null,
                    _setMethodInfo5.MakeGenericMethod(new[] { typeof(T), typeof(TV) }),
                    new[] { source.Expression, Expression.Quote(expression), Expression.Constant(value, typeof(TV)) }));

            return new Updatable<T> { Query = query };
        }

		static readonly MethodInfo _setMethodInfo6 = MemberHelper.MethodOf(() => Set((IUpdatable<int>)null,null,0)).GetGenericMethodDefinition();

        /// <summary>
        /// 设置某个字段为新的值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TV"></typeparam>
        /// <param name="source"></param>
        /// <param name="extract">获取要设值的字段</param>
        /// <param name="value">新的值</param>
        /// <returns></returns>
		[LinqTunnel]
		public static IUpdatable<T> Set<T,TV>(
			[NotNull]                this IUpdatable<T>    source,
			[NotNull, InstantHandle] Expression<Func<T,TV>> extract,
			TV                                              value)
		{
			if (source  == null) throw new ArgumentNullException("source");
			if (extract == null) throw new ArgumentNullException("extract");

			var query = ((Updatable<T>)source).Query;

			query = query.Provider.CreateQuery<T>(
				Expression.Call(
					null,
					_setMethodInfo6.MakeGenericMethod(new[] { typeof(T), typeof(TV) }),
					new[] { query.Expression, Expression.Quote(extract), Expression.Constant(value, typeof(TV)) }));

			return new Updatable<T> { Query = query };
		}

        /// <summary>
        /// 根据字段的名称来设置新的值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TV"></typeparam>
        /// <param name="source"></param>
        /// <param name="fieldName">字段名称</param>
        /// <param name="value">新的值</param>
        /// <returns></returns>

        [LinqTunnel]
        public static IUpdatable<T> Set2<T, TV>(
            [NotNull]                this IUpdatable<T> source,
            [NotNull, InstantHandle] string fieldName,
            TV value)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (string.IsNullOrEmpty(fieldName))
            {
                throw new ArgumentNullException("fieldName");
            }

            var query = ((Updatable<T>)source).Query;
            var expression = AntData.ORM.Expressions.Extensions.GenerateMemberExpression<T, TV>(fieldName);
            query = query.Provider.CreateQuery<T>(
                Expression.Call(
                    null,
                    _setMethodInfo6.MakeGenericMethod(new[] { typeof(T), typeof(TV) }),
                    new[] { query.Expression, Expression.Quote(expression), Expression.Constant(value, typeof(TV)) }));

            return new Updatable<T> { Query = query };
        }

		#endregion

		#region Insert

		static readonly MethodInfo _insertMethodInfo = MemberHelper.MethodOf(() => Insert<int>(null,null)).GetGenericMethodDefinition();

        /// <summary>
        /// 插入数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="target"></param>
        /// <param name="setter"></param>
        /// <returns></returns>
		public static int Insert<T>(
			[NotNull]                this ITable<T>      target,
			[NotNull, InstantHandle] Expression<Func<T>> setter)
		{
			if (target == null) throw new ArgumentNullException("target");
			if (setter == null) throw new ArgumentNullException("setter");

			IQueryable<T> query = target;

			return query.Provider.Execute<int>(
				Expression.Call(
					null,
					_insertMethodInfo.MakeGenericMethod(new[] { typeof(T) }),
					new[] { query.Expression, Expression.Quote(setter) }));
		}

		static readonly MethodInfo _insertWithIdentityMethodInfo = MemberHelper.MethodOf(() => InsertWithIdentity<int>(null,null)).GetGenericMethodDefinition();

        /// <summary>
        /// 插入并返回自增值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="target"></param>
        /// <param name="setter"></param>
        /// <returns></returns>
		public static object InsertWithIdentity<T>(
			[NotNull]                this ITable<T>      target,
			[NotNull, InstantHandle] Expression<Func<T>> setter)
		{
			if (target == null) throw new ArgumentNullException("target");
			if (setter == null) throw new ArgumentNullException("setter");

			IQueryable<T> query = target;

			return query.Provider.Execute<object>(
				Expression.Call(
					null,
					_insertWithIdentityMethodInfo.MakeGenericMethod(new[] { typeof(T) }),
					new[] { query.Expression, Expression.Quote(setter) }));
		}

		#region ValueInsertable

		class ValueInsertable<T> : IValueInsertable<T>
		{
			public IQueryable<T> Query;
		}

		static readonly MethodInfo _intoMethodInfo = MemberHelper.MethodOf(() => Into<int>(null,null)).GetGenericMethodDefinition();

		[LinqTunnel]
		public static IValueInsertable<T> Into<T>(this IDataContext dataContext, [NotNull] ITable<T> target)
		{
			if (target == null) throw new ArgumentNullException("target");

			IQueryable<T> query = target;

			var q = query.Provider.CreateQuery<T>(
				Expression.Call(
					null,
					_intoMethodInfo.MakeGenericMethod(new[] { typeof(T) }),
					new[] { Expression.Constant(null, typeof(IDataContext)), query.Expression }));

			return new ValueInsertable<T> { Query = q };
		}

		static readonly MethodInfo _valueMethodInfo =
			MemberHelper.MethodOf(() => Value<int,int>((ITable<int>)null,null,(Expression<Func<int>>)null)).GetGenericMethodDefinition();

        /// <summary>
        /// 按字段设值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TV"></typeparam>
        /// <param name="source"></param>
        /// <param name="field">获取字段</param>
        /// <param name="value">值</param>
        /// <returns></returns>
		[LinqTunnel]
		public static IValueInsertable<T> Value<T,TV>(
			[NotNull]                this ITable<T>         source,
			[NotNull, InstantHandle] Expression<Func<T,TV>> field,
			[NotNull, InstantHandle] Expression<Func<TV>>   value)
		{
			if (source == null) throw new ArgumentNullException("source");
			if (field  == null) throw new ArgumentNullException("field");
			if (value  == null) throw new ArgumentNullException("value");

			var query = (IQueryable<T>)source;

			var q = query.Provider.CreateQuery<T>(
				Expression.Call(
					null,
					_valueMethodInfo.MakeGenericMethod(new[] { typeof(T), typeof(TV) }),
					new[] { query.Expression, Expression.Quote(field), Expression.Quote(value) }));

			return new ValueInsertable<T> { Query = q };
		}

		static readonly MethodInfo _valueMethodInfo2 =
			MemberHelper.MethodOf(() => Value((ITable<int>)null,null,0)).GetGenericMethodDefinition();

        /// <summary>
        /// 按字段设值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TV"></typeparam>
        /// <param name="source"></param>
        /// <param name="field">获取字段</param>
        /// <param name="value">设值</param>
        /// <returns></returns>
		[LinqTunnel]
		public static IValueInsertable<T> Value<T,TV>(
			[NotNull]                this ITable<T>         source,
			[NotNull, InstantHandle] Expression<Func<T,TV>> field,
			TV                                              value)
		{
			if (source == null) throw new ArgumentNullException("source");
			if (field  == null) throw new ArgumentNullException("field");

			var query = (IQueryable<T>)source;

			var q = query.Provider.CreateQuery<T>(
				Expression.Call(
					null,
					_valueMethodInfo2.MakeGenericMethod(new[] { typeof(T), typeof(TV) }),
					new[] { query.Expression, Expression.Quote(field), Expression.Constant(value, typeof(TV)) }));

			return new ValueInsertable<T> { Query = q };
		}

		static readonly MethodInfo _valueMethodInfo3 =
			MemberHelper.MethodOf(() => Value<int,int>((IValueInsertable<int>)null,null,(Expression<Func<int>>)null)).GetGenericMethodDefinition();

        /// <summary>
        /// 按字段设值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TV"></typeparam>
        /// <param name="source"></param>
        /// <param name="field"></param>
        /// <param name="value"></param>
        /// <returns></returns>
		[LinqTunnel]
		public static IValueInsertable<T> Value<T,TV>(
			[NotNull]                this IValueInsertable<T> source,
			[NotNull, InstantHandle] Expression<Func<T,TV>>   field,
			[NotNull, InstantHandle] Expression<Func<TV>>     value)
		{
			if (source == null) throw new ArgumentNullException("source");
			if (field  == null) throw new ArgumentNullException("field");
			if (value  == null) throw new ArgumentNullException("value");

			var query = ((ValueInsertable<T>)source).Query;

			var q = query.Provider.CreateQuery<T>(
				Expression.Call(
					null,
					_valueMethodInfo3.MakeGenericMethod(new[] { typeof(T), typeof(TV) }),
					new[] { query.Expression, Expression.Quote(field), Expression.Quote(value) }));

			return new ValueInsertable<T> { Query = q };
		}

		static readonly MethodInfo _valueMethodInfo4 =
			MemberHelper.MethodOf(() => Value((IValueInsertable<int>)null,null,0)).GetGenericMethodDefinition();

        /// <summary>
        /// 按字段设置
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TV"></typeparam>
        /// <param name="source"></param>
        /// <param name="field"></param>
        /// <param name="value"></param>
        /// <returns></returns>
		[LinqTunnel]
		public static IValueInsertable<T> Value<T,TV>(
			[NotNull]                this IValueInsertable<T> source,
			[NotNull, InstantHandle] Expression<Func<T,TV>>   field,
			TV                                                value)
		{
			if (source == null) throw new ArgumentNullException("source");
			if (field  == null) throw new ArgumentNullException("field");

			var query = ((ValueInsertable<T>)source).Query;

			var q = query.Provider.CreateQuery<T>(
				Expression.Call(
					null,
					_valueMethodInfo4.MakeGenericMethod(new[] { typeof(T), typeof(TV) }),
					new[] { query.Expression, Expression.Quote(field), Expression.Constant(value, typeof(TV)) }));

			return new ValueInsertable<T> { Query = q };
		}

		static readonly MethodInfo _insertMethodInfo2 = MemberHelper.MethodOf(() => Insert<int>(null)).GetGenericMethodDefinition();

        /// <summary>
        /// 插入
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
		public static int Insert<T>([NotNull] this IValueInsertable<T> source)
		{
			if (source == null) throw new ArgumentNullException("source");

			var query = ((ValueInsertable<T>)source).Query;

			return query.Provider.Execute<int>(
				Expression.Call(
					null,
					_insertMethodInfo2.MakeGenericMethod(new[] { typeof(T) }),
					new[] { query.Expression }));
		}

		static readonly MethodInfo _insertWithIdentityMethodInfo2 = MemberHelper.MethodOf(() => InsertWithIdentity<int>(null)).GetGenericMethodDefinition();

        /// <summary>
        /// 插入并返回主键
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
		public static object InsertWithIdentity<T>([NotNull] this IValueInsertable<T> source)
		{
			if (source == null) throw new ArgumentNullException("source");

			var query = ((ValueInsertable<T>)source).Query;

			return query.Provider.Execute<object>(
				Expression.Call(
					null,
					_insertWithIdentityMethodInfo2.MakeGenericMethod(new[] { typeof(T) }),
					new[] { query.Expression }));
		}

		#endregion

		#region SelectInsertable

		static readonly MethodInfo _insertMethodInfo3 =
			MemberHelper.MethodOf(() => Insert<int,int>(null,null,null)).GetGenericMethodDefinition();

		public static int Insert<TSource,TTarget>(
			[NotNull]                this IQueryable<TSource>          source,
			[NotNull]                ITable<TTarget>                   target,
			[NotNull, InstantHandle] Expression<Func<TSource,TTarget>> setter)
		{
			if (source == null) throw new ArgumentNullException("source");
			if (target == null) throw new ArgumentNullException("target");
			if (setter == null) throw new ArgumentNullException("setter");

			return source.Provider.Execute<int>(
				Expression.Call(
					null,
					_insertMethodInfo3.MakeGenericMethod(new[] { typeof(TSource), typeof(TTarget) }),
					new[] { source.Expression, ((IQueryable<TTarget>)target).Expression, Expression.Quote(setter) }));
		}

		static readonly MethodInfo _insertWithIdentityMethodInfo3 =
			MemberHelper.MethodOf(() => InsertWithIdentity<int,int>(null,null,null)).GetGenericMethodDefinition();

		public static object InsertWithIdentity<TSource,TTarget>(
			[NotNull]                this IQueryable<TSource>          source,
			[NotNull]                ITable<TTarget>                   target,
			[NotNull, InstantHandle] Expression<Func<TSource,TTarget>> setter)
		{
			if (source == null) throw new ArgumentNullException("source");
			if (target == null) throw new ArgumentNullException("target");
			if (setter == null) throw new ArgumentNullException("setter");

			return source.Provider.Execute<object>(
				Expression.Call(
					null,
					_insertWithIdentityMethodInfo3.MakeGenericMethod(new[] { typeof(TSource), typeof(TTarget) }),
					new[] { source.Expression, ((IQueryable<TTarget>)target).Expression, Expression.Quote(setter) }));
		}

		class SelectInsertable<T,TT> : ISelectInsertable<T,TT>
		{
			public IQueryable<T> Query;
		}

		static readonly MethodInfo _intoMethodInfo2 =
			MemberHelper.MethodOf(() => Into<int,int>(null,null)).GetGenericMethodDefinition();

		[LinqTunnel]
		public static ISelectInsertable<TSource,TTarget> Into<TSource,TTarget>(
			[NotNull] this IQueryable<TSource> source,
			[NotNull] ITable<TTarget>          target)
		{
			if (target == null) throw new ArgumentNullException("target");

			var q = source.Provider.CreateQuery<TSource>(
				Expression.Call(
					null,
					_intoMethodInfo2.MakeGenericMethod(new[] { typeof(TSource), typeof(TTarget) }),
					new[] { source.Expression, ((IQueryable<TTarget>)target).Expression }));

			return new SelectInsertable<TSource,TTarget> { Query = q };
		}

		static readonly MethodInfo _valueMethodInfo5 =
			MemberHelper.MethodOf(() => Value<int,int,int>(null,null,(Expression<Func<int,int>>)null)).GetGenericMethodDefinition();

		[LinqTunnel]
		public static ISelectInsertable<TSource,TTarget> Value<TSource,TTarget,TValue>(
			[NotNull]                this ISelectInsertable<TSource,TTarget> source,
			[NotNull, InstantHandle] Expression<Func<TTarget,TValue>>        field,
			[NotNull, InstantHandle] Expression<Func<TSource,TValue>>        value)
		{
			if (source == null) throw new ArgumentNullException("source");
			if (field  == null) throw new ArgumentNullException("field");
			if (value  == null) throw new ArgumentNullException("value");

			var query = ((SelectInsertable<TSource,TTarget>)source).Query;

			var q = query.Provider.CreateQuery<TSource>(
				Expression.Call(
					null,
					_valueMethodInfo5.MakeGenericMethod(new[] { typeof(TSource), typeof(TTarget), typeof(TValue) }),
					new[] { query.Expression, Expression.Quote(field), Expression.Quote(value) }));

			return new SelectInsertable<TSource,TTarget> { Query = q };
		}

		static readonly MethodInfo _valueMethodInfo6 =
			MemberHelper.MethodOf(() => Value<int,int,int>(null,null,(Expression<Func<int>>)null)).GetGenericMethodDefinition();

		[LinqTunnel]
		public static ISelectInsertable<TSource,TTarget> Value<TSource,TTarget,TValue>(
			[NotNull]                this ISelectInsertable<TSource,TTarget> source,
			[NotNull, InstantHandle] Expression<Func<TTarget,TValue>>        field,
			[NotNull, InstantHandle] Expression<Func<TValue>>                value)
		{
			if (source == null) throw new ArgumentNullException("source");
			if (field  == null) throw new ArgumentNullException("field");
			if (value  == null) throw new ArgumentNullException("value");

			var query = ((SelectInsertable<TSource,TTarget>)source).Query;

			var q = query.Provider.CreateQuery<TSource>(
				Expression.Call(
					null,
					_valueMethodInfo6.MakeGenericMethod(new[] { typeof(TSource), typeof(TTarget), typeof(TValue) }),
					new[] { query.Expression, Expression.Quote(field), Expression.Quote(value) }));

			return new SelectInsertable<TSource,TTarget> { Query = q };
		}

		static readonly MethodInfo _valueMethodInfo7 =
			MemberHelper.MethodOf(() => Value<int,int,int>(null,null,0)).GetGenericMethodDefinition();

		[LinqTunnel]
		public static ISelectInsertable<TSource,TTarget> Value<TSource,TTarget,TValue>(
			[NotNull]                this ISelectInsertable<TSource,TTarget> source,
			[NotNull, InstantHandle] Expression<Func<TTarget,TValue>>        field,
			TValue                                                           value)
		{
			if (source == null) throw new ArgumentNullException("source");
			if (field  == null) throw new ArgumentNullException("field");

			var query = ((SelectInsertable<TSource,TTarget>)source).Query;

			var q = query.Provider.CreateQuery<TSource>(
				Expression.Call(
					null,
					_valueMethodInfo7.MakeGenericMethod(new[] { typeof(TSource), typeof(TTarget), typeof(TValue) }),
					new[] { query.Expression, Expression.Quote(field), Expression.Constant(value, typeof(TValue)) }));

			return new SelectInsertable<TSource,TTarget> { Query = q };
		}

		static readonly MethodInfo _insertMethodInfo4 =
			MemberHelper.MethodOf(() => Insert<int,int>(null)).GetGenericMethodDefinition();

		public static int Insert<TSource,TTarget>([NotNull] this ISelectInsertable<TSource,TTarget> source)
		{
			if (source == null) throw new ArgumentNullException("source");

			var query = ((SelectInsertable<TSource,TTarget>)source).Query;

			return query.Provider.Execute<int>(
				Expression.Call(
					null,
					_insertMethodInfo4.MakeGenericMethod(new[] { typeof(TSource), typeof(TTarget) }),
					new[] { query.Expression }));
		}

		static readonly MethodInfo _insertWithIdentityMethodInfo4 =
			MemberHelper.MethodOf(() => InsertWithIdentity<int,int>(null)).GetGenericMethodDefinition();

		public static object InsertWithIdentity<TSource,TTarget>([NotNull] this ISelectInsertable<TSource,TTarget> source)
		{
			if (source == null) throw new ArgumentNullException("source");

			var query = ((SelectInsertable<TSource,TTarget>)source).Query;

			return query.Provider.Execute<object>(
				Expression.Call(
					null,
					_insertWithIdentityMethodInfo4.MakeGenericMethod(new[] { typeof(TSource), typeof(TTarget) }),
					new[] { query.Expression }));
		}

		#endregion

		#endregion

		#region InsertOrUpdate

		//static readonly MethodInfo _insertOrUpdateMethodInfo =
		//	MemberHelper.MethodOf(() => InsertOrUpdate<int>(null,null,null)).GetGenericMethodDefinition();

		//public static int InsertOrUpdate<T>(
		//	[NotNull]                this ITable<T>        target,
		//	[NotNull, InstantHandle] Expression<Func<T>>   insertSetter,
		//	[NotNull, InstantHandle] Expression<Func<T,T>> onDuplicateKeyUpdateSetter)
		//{
		//	if (target                     == null) throw new ArgumentNullException("target");
		//	if (insertSetter               == null) throw new ArgumentNullException("insertSetter");
		//	if (onDuplicateKeyUpdateSetter == null) throw new ArgumentNullException("onDuplicateKeyUpdateSetter");

		//	IQueryable<T> query = target;

		//	return query.Provider.Execute<int>(
		//		Expression.Call(
		//			null,
		//			_insertOrUpdateMethodInfo.MakeGenericMethod(new[] { typeof(T) }),
		//			new[] { query.Expression, Expression.Quote(insertSetter), Expression.Quote(onDuplicateKeyUpdateSetter) }));
		//}

		//static readonly MethodInfo _insertOrUpdateMethodInfo2 =
		//	MemberHelper.MethodOf(() => InsertOrUpdate<int>(null,null,null,null)).GetGenericMethodDefinition();

		//public static int InsertOrUpdate<T>(
		//	[NotNull]                this ITable<T>        target,
		//	[NotNull, InstantHandle] Expression<Func<T>>   insertSetter,
		//	[NotNull, InstantHandle] Expression<Func<T,T>> onDuplicateKeyUpdateSetter,
		//	[NotNull, InstantHandle] Expression<Func<T>>   keySelector)
		//{
		//	if (target                     == null) throw new ArgumentNullException("target");
		//	if (insertSetter               == null) throw new ArgumentNullException("insertSetter");
		//	if (onDuplicateKeyUpdateSetter == null) throw new ArgumentNullException("onDuplicateKeyUpdateSetter");
		//	if (keySelector                == null) throw new ArgumentNullException("keySelector");

		//	IQueryable<T> query = target;

		//	return query.Provider.Execute<int>(
		//		Expression.Call(
		//			null,
		//			_insertOrUpdateMethodInfo2.MakeGenericMethod(new[] { typeof(T) }),
		//			new[]
		//			{
		//				query.Expression,
		//				Expression.Quote(insertSetter),
		//				Expression.Quote(onDuplicateKeyUpdateSetter),
		//				Expression.Quote(keySelector)
		//			}));
		//}

		#endregion

		#region DDL Operations

		static readonly MethodInfo _dropMethodInfo2 = MemberHelper.MethodOf(() => Drop<int>(null)).GetGenericMethodDefinition();

        /// <summary>
        /// 删除表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="target"></param>
        /// <returns></returns>
		public static int Drop<T>([NotNull] this ITable<T> target)
		{
			if (target == null) throw new ArgumentNullException("target");

			IQueryable<T> query = target;

			return query.Provider.Execute<int>(
				Expression.Call(
					null,
					_dropMethodInfo2.MakeGenericMethod(new[] { typeof(T) }),
					new[] { query.Expression }));
		}

        #endregion

        static readonly MethodInfo _whereMethodInfo = MemberHelper.MethodOf(() => Where<string>(null, null,null)).GetGenericMethodDefinition();
        /// <summary>
        /// Where扩展查询
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <param name="whereString">查询字符串 注意一定要以半角隔开</param>
        /// <param name="whereObj">外部指定查询的值</param>
        /// <returns></returns>
        [LinqTunnel]
        public static IQueryable<TSource> Where<TSource>(
            [NotNull]                this IQueryable<TSource> source,
            [NotNull] string whereString, object whereObj = null)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (string.IsNullOrEmpty(whereString)) throw new ArgumentNullException("whereString");

            return source.Provider.CreateQuery<TSource>(
                Expression.Call(
                    null,
                    _whereMethodInfo.MakeGenericMethod(new[] { typeof(TSource) }),
                    new[] { source.Expression, Expression.Constant(whereString, typeof(string)), Expression.Constant(whereObj, typeof(object))  }));
        }

        
        #region Take / Skip / ElementAt

        static readonly MethodInfo _takeMethodInfo = MemberHelper.MethodOf(() => Take<int>(null,null)).GetGenericMethodDefinition();

        /// <summary>
        /// 取数据
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <param name="count"></param>
        /// <returns></returns>
		[LinqTunnel]
		public static IQueryable<TSource> Take<TSource>(
			[NotNull]                this IQueryable<TSource> source,
			[NotNull, InstantHandle] Expression<Func<int>>    count)
		{
			if (source == null) throw new ArgumentNullException("source");
			if (count  == null) throw new ArgumentNullException("count");

			return source.Provider.CreateQuery<TSource>(
				Expression.Call(
					null,
					_takeMethodInfo.MakeGenericMethod(new[] { typeof(TSource) }),
					new[] { source.Expression, Expression.Quote(count) }));
		}

		static readonly MethodInfo _skipMethodInfo = MemberHelper.MethodOf(() => Skip<int>(null,null)).GetGenericMethodDefinition();

        /// <summary>
        /// 跳过
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <param name="count"></param>
        /// <returns></returns>
		[LinqTunnel]
		public static IQueryable<TSource> Skip<TSource>(
			[NotNull]                this IQueryable<TSource> source,
			[NotNull, InstantHandle] Expression<Func<int>>    count)
		{
			if (source == null) throw new ArgumentNullException("source");
			if (count  == null) throw new ArgumentNullException("count");

			return source.Provider.CreateQuery<TSource>(
				Expression.Call(
					null,
					_skipMethodInfo.MakeGenericMethod(new[] { typeof(TSource) }),
					new[] { source.Expression, Expression.Quote(count) }));
		}

		static readonly MethodInfo _elementAtMethodInfo = MemberHelper.MethodOf(() => ElementAt<int>(null,null)).GetGenericMethodDefinition();

        /// <summary>
        /// 按照index获取
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <param name="index"></param>
        /// <returns></returns>
		public static TSource ElementAt<TSource>(
			[NotNull]                this IQueryable<TSource> source,
			[NotNull, InstantHandle] Expression<Func<int>>    index)
		{
			if (source == null) throw new ArgumentNullException("source");
			if (index  == null) throw new ArgumentNullException("index");

			return source.Provider.Execute<TSource>(
				Expression.Call(
					null,
					_elementAtMethodInfo.MakeGenericMethod(new[] { typeof(TSource) }),
					new[] { source.Expression, Expression.Quote(index) }));
		}

		static readonly MethodInfo _elementAtOrDefaultMethodInfo = MemberHelper.MethodOf(() => ElementAtOrDefault<int>(null,null)).GetGenericMethodDefinition();

        /// <summary>
        /// 按照index获取 没有就返回null
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <param name="index"></param>
        /// <returns></returns>
		public static TSource ElementAtOrDefault<TSource>(
			[NotNull]                this IQueryable<TSource> source,
			[NotNull, InstantHandle] Expression<Func<int>>    index)
		{
			if (source == null) throw new ArgumentNullException("source");
			if (index  == null) throw new ArgumentNullException("index");

			return source.Provider.Execute<TSource>(
				Expression.Call(
					null,
					_elementAtOrDefaultMethodInfo.MakeGenericMethod(new[] { typeof(TSource) }),
					new[] { source.Expression, Expression.Quote(index) }));
		}

		#endregion

		#region Having

		static readonly MethodInfo _setMethodInfo7 = MemberHelper.MethodOf(() => Having((IQueryable<int>)null,null)).GetGenericMethodDefinition();

        /// <summary>
        /// having 查询
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
		[LinqTunnel]
		public static IQueryable<TSource> Having<TSource>(
			[NotNull]                this IQueryable<TSource>       source,
			[NotNull, InstantHandle] Expression<Func<TSource,bool>> predicate)
		{
			if (source    == null) throw new ArgumentNullException("source");
			if (predicate == null) throw new ArgumentNullException("predicate");

			return source.Provider.CreateQuery<TSource>(
				Expression.Call(
					null,
					_setMethodInfo7.MakeGenericMethod(typeof(TSource)),
					new[] { source.Expression, Expression.Quote(predicate) }));
		}

		#endregion

		static readonly MethodInfo _setMethodInfo8 = MemberHelper.MethodOf(() => GetContext((IQueryable<int>)null)).GetGenericMethodDefinition();

		internal static ContextParser.Context GetContext<TSource>(this IQueryable<TSource> source)
		{
			if (source == null) throw new ArgumentNullException("source");

			return source.Provider.Execute<ContextParser.Context>(
				Expression.Call(
					null,
					_setMethodInfo8.MakeGenericMethod(typeof(TSource)),
					new[] { source.Expression }));
		}

		#region Stub helpers

		internal static TOutput Where<TOutput,TSource,TInput>(this TInput source, Func<TSource,bool> predicate)
		{
			throw new InvalidOperationException();
		}

        #endregion

        #region dynamicOrderby
        #region Fields
        private static readonly MethodInfo s_orderBy = typeof(Queryable).GetMethods().First(m => m.Name == "OrderBy");
        private static readonly MethodInfo s_orderByDesc = typeof(Queryable).GetMethods().First(m => m.Name == "OrderByDescending");
        private static readonly string[] s_orderSequence = new string[] { "asc", "desc" };
        #endregion
        #region Public Methods
        /// <summary>
        /// 动态排序
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="property">如果没有指定 默认查询的第一个作为排序 且为asc</param>
        /// <param name="asc">如果没有指定 默认为asc</param>
        /// <returns></returns>
        //public static IOrderedQueryable<T> OrderBy<T>(this IQueryable<T> source, string property, bool asc)
        //{
        //    try
        //    {
        //        string orderSequence = "asc";
        //        if (string.IsNullOrEmpty(property))
        //        {
        //            return source as IOrderedQueryable<T>;
        //        }
        //        if (!asc)
        //        {
        //            orderSequence = "desc";
        //        }
        //        var expr = source.Expression;
        //        var p = Expression.Parameter(typeof(T), "x");
        //        var propInfo = typeof(T).GetProperty(property, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
        //        var sortExpr = Expression.Lambda(Expression.Property(p, propInfo), p);
        //        var method = orderSequence.ToLower().Equals("asc") ? s_orderBy.MakeGenericMethod(typeof(T), propInfo.PropertyType) : s_orderByDesc.MakeGenericMethod(typeof(T), propInfo.PropertyType);
        //        var call = Expression.Call(method, expr, sortExpr);
        //        var newQuery = source.Provider.CreateQuery<T>(call);
        //        return newQuery as IOrderedQueryable<T>;
        //    }
        //    catch (Exception)
        //    {

        //        throw new LinqException("OrderBy传参不正确,必须是返回模型中的字段!");
        //    }
        //}

        #region GroupBy
        /// <summary>
        /// 动态分组
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <param name="column">分组字段 目前只能支持一个</param>
        /// <returns></returns>
        public static IQueryable<IGrouping<object, TSource>> GroupBy<TSource>(this IQueryable<TSource> source, string column)
        {
            if (string.IsNullOrEmpty(column))
            {
                throw new LinqException("传参不正确");
            }
            IQueryable<IGrouping<object, TSource>> groupedCollection = source.GroupBy(Construct<TSource>(column));
            return groupedCollection;
        }

        #endregion

	    /// <summary>
	    /// 动态指定单个字段的 desc or asc的排序
	    /// </summary>
	    /// <typeparam name="T"></typeparam>
	    /// <param name="source"></param>
	    /// <param name="column">排序字段</param>
	    /// <param name="squence">排序方式</param>
	    /// <returns></returns>
	    public static IOrderedQueryable<T> DynamicOrderBy<T>(this IQueryable<T> source, string column,string squence=null)
        {
            if (string.IsNullOrEmpty(column) )
            {
                throw new LinqException("OrderBy传参不正确");
            }
            if (string.IsNullOrEmpty(squence))
            {
                squence = "asc";
            }
            squence = squence.ToLower();
            if (!s_orderSequence.Contains(squence)) squence = "asc";

            IOrderedQueryable<T> sortedCollection = squence.Equals("asc")? source.OrderBy(Construct<T>(column)) : source.OrderByDescending(Construct<T>(column));
            return sortedCollection;
        }

        /// <summary>
        /// 动态排序 asc
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="columns">多个排序字段</param>
        /// <returns></returns>
        public static IOrderedQueryable<T> OrderBy<T>(this IQueryable<T> source, params string[] columns)
        {
            if (columns == null || columns.Length < 1)
            {
                throw new LinqException("OrderBy传参不正确");
            }

            IOrderedQueryable<T> sortedCollection = source.OrderBy(Construct<T>(columns[0]));

            if (columns.Length>1)
            {
                for (int i = 1; i < columns.Length; i++)
                {
                    sortedCollection = sortedCollection
                                        .ThenBy(Construct<T>(columns[i]));
                }
            }
            
            return sortedCollection;
        }

        /// <summary>
        /// 动态排序 desc
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="columns">多个排序字段</param>
        /// <returns></returns>
        public static IOrderedQueryable<T> OrderByDescending<T>(this IQueryable<T> source, params string[] columns)
        {
            if (columns == null || columns.Length < 1)
            {
                throw new LinqException("OrderBy传参不正确");
            }

            IOrderedQueryable<T> sortedCollection = source.OrderByDescending(Construct<T>(columns[0]));

            if (columns.Length > 1)
            {
                for (int i = 1; i < columns.Length; i++)
                {
                    sortedCollection = sortedCollection
                                        .ThenByDescending(Construct<T>(columns[i]));
                }
            }

            return sortedCollection;
        }

        /// <summary>
        /// 动态排序
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="columns">排序字段拼接 例如.OrderBy("id desc,name asc")</param>
        /// <returns></returns>
        public static IOrderedQueryable<T> OrderByMultiple<T>(this IQueryable<T> source, string columns)
        {
            if (string.IsNullOrEmpty(columns))
            {
                throw new LinqException("排序字段不能为空");
            }
            IOrderedQueryable<T> sortedCollection = null;
            var allColumns = columns.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var cc in allColumns)
            {
                var fieldAndseq = cc.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                var squence = "asc";
                if (fieldAndseq.Length > 1)
                {
                    squence = fieldAndseq[1].ToLower();
                    if (!s_orderSequence.Contains(squence)) squence = "asc";
                    
                }
                if (squence.ToLower().Equals("asc"))
                {
                    if (sortedCollection == null)
                    {
                        sortedCollection = source.OrderBy(Construct<T>(fieldAndseq[0]));
                    }
                    else
                    {
                        sortedCollection = sortedCollection.ThenBy(Construct<T>(fieldAndseq[0]));
                    }
                }
                else
                {
                    if (sortedCollection == null)
                    {
                        sortedCollection = source.OrderByDescending(Construct<T>(fieldAndseq[0]));
                    }
                    else
                    {
                        sortedCollection = sortedCollection.ThenByDescending(Construct<T>(fieldAndseq[0]));
                    }
                }
            }
            return sortedCollection;
        }

        #endregion
        private static Expression<Func<T, dynamic>> Construct<T>(string propertyName)
        {
            PropertyInfo property = typeof(T).GetCanReadPropertyInfo().FirstOrDefault(r=>r.Name.ToLower().Equals(propertyName.ToLower()));

            if (property == null)
            {
                throw new LinqException("传参不正确,字段:{0}不存在!".Args(propertyName));
            }
            
            ParameterExpression typeExpression = Expression.Parameter(typeof(T), "type");

            MemberExpression propExpression = Expression.Property(typeExpression, property);

            UnaryExpression objectpropExpression = Expression.Convert(propExpression, typeof(object));

            return Expression.Lambda<Func<T, dynamic>>(objectpropExpression, new ParameterExpression[] { typeExpression });
        }
        #endregion

      
    }
   
}
