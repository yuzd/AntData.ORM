using System;
using System.Collections.Generic;
using AntData.ORM.Data;
using AntData.ORM.DataProvider;

namespace LinqToDB.DataProvider.Oracle
{

	class OracleMerge : BasicMerge
	{
		protected override bool BuildUsing<T>(DataConnection dataConnection, IEnumerable<T> source)
		{
			return BuildUsing2(dataConnection, source, null, "FROM SYS.DUAL");
		}
	}
}
