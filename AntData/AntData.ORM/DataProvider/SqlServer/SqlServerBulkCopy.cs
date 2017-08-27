using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using AntData.ORM.Data;
using AntData.ORM.DataProvider;
using AntData.ORM.SqlProvider;

namespace AntData.ORM.DataProvider.SqlServer
{


	class SqlServerBulkCopy : BasicBulkCopy
	{
		
		
		protected override BulkCopyRowsCopied MultipleRowsCopy<T>(
			DataConnection dataConnection, BulkCopyOptions options, IEnumerable<T> source)
		{
			BulkCopyRowsCopied ret;

			var helper = new MultipleRowsHelper<T>(dataConnection, options, options.KeepIdentity == true);

			if (options.KeepIdentity == true)
				dataConnection.Execute("SET IDENTITY_INSERT " + helper.TableName + " ON");

			switch (((SqlServerDataProvider)dataConnection.DataProvider).Version)
			{
				case SqlServerVersion.v2000 :
				case SqlServerVersion.v2005 : ret = MultipleRowsCopy2(helper, dataConnection, options, source, ""); break;
				default                     : ret = MultipleRowsCopy1(helper, dataConnection, options, source);     break;
			}

			if (options.KeepIdentity == true)
				dataConnection.Execute("SET IDENTITY_INSERT " + helper.TableName + " OFF");

			return ret;
		}
	}
}
