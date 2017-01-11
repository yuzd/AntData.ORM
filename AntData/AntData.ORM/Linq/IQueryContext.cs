using System;
using System.Collections.Generic;
using AntData.ORM.Common;

namespace AntData.ORM.Linq
{
	using SqlQuery;

	public interface IQueryContext
	{
		SelectQuery    SelectQuery { get; }
		object         Context     { get; set; }
		List<string>   QueryHints  { get; }
        Dictionary<string, CustomerParam> Params { get; }
		SqlParameter[] GetParameters();

        List<ParameterAccessor> Parameters { get; set; }
    }
}
