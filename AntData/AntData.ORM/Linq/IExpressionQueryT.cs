using System;
using System.Linq;
using System.Linq.Expressions;

namespace AntData.ORM.Linq
{
	public interface IExpressionQuery<out T> : IOrderedQueryable<T>, IQueryProvider, IExpressionQuery
	{
		new Expression Expression { get; set; }
	}
}
