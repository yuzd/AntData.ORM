using System;
using System.Linq.Expressions;

namespace AntData.ORM.Linq
{
	class ExpressionQueryImpl<T> : ExpressionQuery<T>
	{
		public ExpressionQueryImpl(IDataContextInfo dataContext, Expression expression)
		{
			Init(dataContext, expression);
		}

		public override string ToString()
		{
			return SqlText;
		}
	}
}
