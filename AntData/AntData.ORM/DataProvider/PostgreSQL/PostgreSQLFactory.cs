using System;
using System.Collections.Specialized;

using JetBrains.Annotations;

namespace AntData.ORM.DataProvider.PostgreSQL
{
	using System.Collections.Generic;

	[UsedImplicitly]
	class PostgreSQLFactory: IDataProviderFactory
	{
	    IDataProvider IDataProviderFactory.GetDataProvider(NameValueCollection attributes)
	    {
	        return new PostgreSQLDataProvider();
        }
	}
}
