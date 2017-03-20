using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace AntData.ORM.Data
{
	public class DataReader : IDisposable
	{
		public   CommandInfo CommandInfo { get; set; }
		public   IList<IDataReader> Reader      { get; set; }
		internal int         ReadNumber  { get; set; }

		public void Dispose()
		{
		    if (Reader != null)
		    {
		        foreach (var reader in Reader)
		        {
                    reader.Dispose();
                }
		    }
		}

		#region Query with object reader

		public IEnumerable<T> Query<T>(Func<IDataReader,T> objectReader)
		{
		    foreach (var reader in Reader)
		    {
                while (reader.Read())
                    yield return objectReader(reader);
            }
			
		}

		#endregion

		#region Query

		public IEnumerable<T> Query<T>()
		{
			//if (ReadNumber != 0)
			//	if (!Reader.NextResult())
			//		return Enumerable.Empty<T>();

			ReadNumber++;

			return CommandInfo.ExecuteQuery<T>(Reader, CommandInfo.DataConnection.LastQuery + "$$$" + ReadNumber);
		}

		#endregion

		#region Query with template

		public IEnumerable<T> Query<T>(T template)
		{
			return Query<T>();
		}

		#endregion

		#region Execute scalar

		public T Execute<T>()
		{
			//if (ReadNumber != 0)
			//	if (!Reader.NextResult())
			//		return default(T);

			ReadNumber++;

			var sql = CommandInfo.DataConnection.LastQuery + "$$$" + ReadNumber;

			return CommandInfo.ExecuteScalar<T>(Reader, sql);
		}

		#endregion
	}
}
