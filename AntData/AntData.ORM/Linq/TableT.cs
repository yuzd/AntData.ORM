using System;
using System.Linq.Expressions;
using System.Reflection;
using AntData.ORM.Mapping;

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

    class AntDataTable<T, B> : ExpressionQuery<T>, IAntTable<T,B>, ITable where  B:new()
    {
        public AntDataTable(IDataContextInfo dataContextInfo)
        {
            Init(dataContextInfo, null);
        }

        public AntDataTable(IDataContext dataContext)
        {
            Init(dataContext == null ? null : new DataContextInfo(dataContext), null);
        }

        public AntDataTable(IDataContext dataContext, Expression expression)
        {
            Init(dataContext == null ? null : new DataContextInfo(dataContext), expression);
        }

        public AntDataTable()
        {
            Init(null, null);
        }

        internal string DatabaseName;
        internal string SchemaName;
        internal string TableName;

        #region Overrides

        public override string ToString()
        {

            return "Table(" + typeof(T).Name + ")";

        }

        #endregion

  
        public string Table { get; } = TableNames<T>.Name;

        public B Column { get; } = ColumnNames<B>.Instance;
    }


    class TableNames<T>
    {
        private TableNames()
        {
        }
        private static readonly Lazy<TableName<T>> lazy = new Lazy<TableName<T>>(() =>
        {
            return new TableName<T>();
        });

        public static string Name => lazy.Value.Name;
    }

    class TableName<T>
    {
        public TableName()
        {
            var type = typeof(T);
            var tableAttr = type.GetCustomAttribute<TableAttribute>();
            if (tableAttr != null && !string.IsNullOrEmpty(tableAttr.Name))
            {
                this.Name = tableAttr.Name;
            }
        }
        public string Name { get; set; }
    }

    class ColumnNames<T> where T:new()
    {
        private ColumnNames()
        {
        }
        private static readonly Lazy<T> lazy = new Lazy<T>(() =>
        {
            return new T();
        });
        public static T Instance => lazy.Value;
    }
}
