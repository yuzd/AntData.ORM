using System;
using System.Collections.Generic;

namespace AntData.ORM.SqlQuery
{
	public interface ISqlTableSource : ISqlExpression
	{
		SqlField              All          { get; }
		int                   SourceID     { get; }
		SqlTableType          SqlTableType { get; }
		IList<ISqlExpression> GetKeys(bool allIfEmpty);
	}
}
