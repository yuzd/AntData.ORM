using System;

namespace AntData.ORM.SqlQuery
{
	public interface ISqlPredicate : IQueryElement, ISqlExpressionWalkable, ICloneableElement
	{
		bool CanBeNull  { get; }
		int  Precedence { get; }
	}
}
