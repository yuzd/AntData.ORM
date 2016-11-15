using System;
using System.Collections.Specialized;

namespace AntData.ORM.DataProvider
{
	public interface IDataProviderFactory
	{
		IDataProvider GetDataProvider (NameValueCollection attributes);
	}
}
