using System;
using System.Collections.Specialized;

using JetBrains.Annotations;

namespace AntData.ORM.DataProvider.MySql
{
	[UsedImplicitly]
	class MySqlFactory : IDataProviderFactory
	{
		IDataProvider IDataProviderFactory.GetDataProvider(NameValueCollection attributes)
		{
			return new MySqlDataProvider();
		}
	}
}
