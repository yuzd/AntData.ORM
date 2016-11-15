using System;

namespace AntData.ORM.Linq
{
	using Mapping;
	using SqlProvider;

	public interface IDataContextInfo
	{
		IDataContext     DataContext      { get; }
		string           ContextID        { get; }
		MappingSchema    MappingSchema    { get; }
		bool             DisposeContext   { get; }
		SqlProviderFlags SqlProviderFlags { get; }

		ISqlBuilder      CreateSqlBuilder ();
		ISqlOptimizer    GetSqlOptimizer  ();
		IDataContextInfo Clone(bool forNestedQuery);
	}
}
