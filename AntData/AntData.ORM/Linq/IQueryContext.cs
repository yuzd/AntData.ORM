using System;
using System.Collections.Generic;

namespace AntData.ORM.Linq
{
	using SqlQuery;

	public interface IQueryContext
	{
		SelectQuery    SelectQuery { get; }
		object         Context     { get; set; }
		List<string>   QueryHints  { get; }
		SqlParameter[] GetParameters();
	}
}
