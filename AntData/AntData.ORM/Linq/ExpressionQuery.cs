using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

using JetBrains.Annotations;

namespace AntData.ORM.Linq
{
	using Extensions;

	abstract class ExpressionQuery<T> : IExpressionQuery<T>
	{
		#region Init
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="dataContextInfo">如果为空获取默认的</param>
        /// <param name="expression"></param>
		protected void Init(IDataContextInfo dataContextInfo, Expression expression)
		{
            DataContextInfo = dataContextInfo ?? new DefaultDataContextInfo();
            Expression      = expression      ?? Expression.Constant(this);
		}

		[NotNull]
        public Expression       Expression      { get; set; }

	    public Type ElementType { get; }
	    public IQueryProvider Provider { get; }

	    [NotNull]
        public IDataContextInfo DataContextInfo { get; set; }

		internal  Query<T> Info;
		internal  object[] Parameters;

		#endregion

		#region Public Members

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private string _sqlTextHolder;

// ReSharper disable InconsistentNaming
		[UsedImplicitly]
		private string _sqlText { get { return SqlText; }}
// ReSharper restore InconsistentNaming

		public  string  SqlText
		{
			get
			{
				var hasQueryHints = DataContextInfo.DataContext.QueryHints.Count > 0 || DataContextInfo.DataContext.NextQueryHints.Count > 0;

				if (_sqlTextHolder == null || hasQueryHints)
				{
					var info    = GetQuery(Expression, true);
					var sqlText = info.GetSqlText(DataContextInfo.DataContext, Expression, Parameters, 0);

					if (hasQueryHints)
						return sqlText;

					_sqlTextHolder = sqlText;
				}

				return _sqlTextHolder;
			}
		}

        #endregion

        #region Impl

        public IEnumerator<T> GetEnumerator()
        {
            return Execute(DataContextInfo, Expression).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Execute(DataContextInfo, Expression).GetEnumerator();
        }
        object IQueryProvider.Execute(Expression expression)
        {
            return GetQuery(expression, false).GetElement(null, DataContextInfo, expression, Parameters);
        }

        public IQueryable CreateQuery(Expression expression)
        {
            if (expression == null)
                throw new ArgumentNullException("expression");

            var elementType = expression.Type.GetItemType() ?? expression.Type;

            try
            {
                return (IQueryable)Activator.CreateInstance(typeof(ExpressionQueryImpl<>).MakeGenericType(elementType), new object[] { DataContextInfo, expression });
            }
            catch (TargetInvocationException ex)
            {
                throw ex.InnerException;
            }
        }

        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            if (expression == null)
                throw new ArgumentNullException("expression");

            return new ExpressionQueryImpl<TElement>(DataContextInfo, expression);
        }

        public object Execute(Expression expression)
        {
            return GetQuery(expression, false).GetElement(null, DataContextInfo, expression, Parameters);
        }

        public TResult Execute<TResult>(Expression expression)
        {
            return (TResult)GetQuery(expression, false).GetElement(null, DataContextInfo, expression, Parameters);
        }
        #endregion

        #region IQueryable Members

        Type IQueryable.ElementType
        {
            get { return typeof(T); }
        }

        Expression IQueryable.Expression
        {
            get { return Expression; }
        }

        IQueryProvider IQueryable.Provider
        {
            get { return this; }
        }

        #endregion
        #region Private

        IEnumerable<T> Execute(IDataContextInfo dataContextInfo, Expression expression)
        {
            return GetQuery(expression, true).GetIEnumerable(null, dataContextInfo, expression, Parameters);
        }
        Query<T> GetQuery(Expression expression, bool cache)
        {
            if (cache && Info != null)
                return Info;

            var info = Query<T>.GetQuery(DataContextInfo, expression);

            if (cache)
                Info = info;

            return info;
        } 
        #endregion
    }
}
