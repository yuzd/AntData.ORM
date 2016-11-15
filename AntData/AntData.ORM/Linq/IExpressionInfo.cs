using System;
using System.Linq.Expressions;

namespace AntData.ORM.Linq
{
	using Mapping;

	public interface IExpressionInfo
	{
		LambdaExpression GetExpression(MappingSchema mappingSchema);
	}
}
