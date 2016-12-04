using System;

namespace AntData.ORM.Linq
{
	using Mapping;
	using SqlProvider;

	class DataContextInfo : IDataContextInfo
	{
        #region Ctor
        public DataContextInfo(IDataContext dataContext)
        {
            DataContext = dataContext;
            DisposeContext = false;
        }

        public DataContextInfo(IDataContext dataContext, bool disposeContext)
        {
            DataContext = dataContext;
            DisposeContext = disposeContext;
        }

        #endregion
        #region Impl

        public IDataContext DataContext { get; private set; }
        public string ContextID
        {
            get
            {
                return DataContext.ContextID;

            }
        }

        public MappingSchema MappingSchema
        {
            get
            {
                return DataContext.MappingSchema;
            }
        }

        public bool DisposeContext { get; private set; }

        public SqlProviderFlags SqlProviderFlags
        {
            get
            {
                return DataContext.SqlProviderFlags;
            }
        }

        public ISqlBuilder CreateSqlBuilder()
        {
            return DataContext.CreateSqlProvider();
        }

        public ISqlOptimizer GetSqlOptimizer()
        {
            return DataContext.GetSqlOptimizer();
        }

        #endregion
        public static IDataContextInfo Create(IDataContext dataContext)
        {
            return dataContext == null ? (IDataContextInfo)new DefaultDataContextInfo() : new DataContextInfo(dataContext);
        }
    }
}
