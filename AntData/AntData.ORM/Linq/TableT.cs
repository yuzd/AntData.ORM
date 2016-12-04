using System;
using System.Linq.Expressions;

namespace AntData.ORM.Linq
{
	class Table<T> : ExpressionQuery<T>, ITable<T>, ITable
	{
		public Table(IDataContextInfo dataContextInfo)
		{
			Init(dataContextInfo, null);
		}

		public Table(IDataContext dataContext)
		{
			Init(dataContext == null ? null : new DataContextInfo(dataContext), null);
		}

		public Table(IDataContext dataContext, Expression expression)
		{
			Init(dataContext == null ? null : new DataContextInfo(dataContext), expression);
		}

        public Table()
        {
            Init(null, null);
        }

        public string DatabaseName;
		public string SchemaName;
		public string TableName;

        #region Overrides

        public override string ToString()
        {
            return "Table(" + typeof(T).Name + ")";
        }

        #endregion
    }
}
