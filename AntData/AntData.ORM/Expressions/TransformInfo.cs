using System;
using System.Linq.Expressions;

namespace AntData.ORM.Expressions
{
	public struct TransformInfo
	{
		public TransformInfo(Expression expression, bool stop)
		{
			Expression = expression;
			Stop       = stop;
		}

		public TransformInfo(Expression expression)
		{
			Expression = expression;
			Stop       = false;
		}

		public Expression Expression;
		public bool       Stop;
	}
}
