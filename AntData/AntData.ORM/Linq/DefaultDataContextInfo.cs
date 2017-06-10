using System;

namespace AntData.ORM.Linq
{
	using Data;
	using DataProvider;
	using SqlProvider;
	using Mapping;

	class DefaultDataContextInfo : IDataContextInfo
	{
		//private IDataContext    _dataContext;
		public  IDataContext     DataContext { get; set; }

		public MappingSchema     MappingSchema    { get { return MappingSchema.Default; } }
		public bool              DisposeContext   { get { return true;                  } }
		public SqlProviderFlags  SqlProviderFlags { get { return _dataProvider.SqlProviderFlags; } }
		public string            ContextID        { get { return _dataProvider.Name;    } }

		public ISqlBuilder CreateSqlBuilder()
		{
			return _dataProvider.CreateSqlBuilder();
		}

		public ISqlOptimizer GetSqlOptimizer()
		{
			return _dataProvider.GetSqlOptimizer();
		}

		static readonly IDataProvider _dataProvider = DataConnection.GetDataProvider("");
	}
}
