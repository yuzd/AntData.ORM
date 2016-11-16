using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace AntData.ORM.Data
{
#if !NOASYNC

	public class DataReaderAsync : IDisposable
	{
		public   CommandInfo       CommandInfo       { get; set; }
		public   DbDataReader      Reader            { get; set; }
		internal int               ReadNumber        { get; set; }
		internal CancellationToken CancellationToken { get; set; }

		public void Dispose()
		{
			if (Reader != null)
				Reader.Dispose();
		}

		#region Query with object reader

		public Task<List<T>> QueryToListAsync<T>(Func<IDataReader,T> objectReader)
		{
			return QueryToListAsync(objectReader, CancellationToken.None);
		}

		public async Task<List<T>> QueryToListAsync<T>(Func<IDataReader,T> objectReader, CancellationToken cancellationToken)
		{
			var list = new List<T>();
			await QueryForEachAsync(objectReader, list.Add, cancellationToken);
			return list;
		}

		public Task<T[]> QueryToArrayAsync<T>(Func<IDataReader,T> objectReader)
		{
			return QueryToArrayAsync(objectReader, CancellationToken.None);
		}

		public async Task<T[]> QueryToArrayAsync<T>(Func<IDataReader,T> objectReader, CancellationToken cancellationToken)
		{
			var list = new List<T>();
			await QueryForEachAsync(objectReader, list.Add, cancellationToken);
			return list.ToArray();
		}

		public Task QueryForEachAsync<T>(Func<IDataReader,T> objectReader, Action<T> action)
		{
			return QueryForEachAsync(objectReader, action, CancellationToken.None);
		}

		public async Task QueryForEachAsync<T>(Func<IDataReader,T> objectReader, Action<T> action, CancellationToken cancellationToken)
		{
			while (await Reader.ReadAsync(cancellationToken))
				action(objectReader(Reader));
		}

		#endregion

	


	}

#endif
}
