using System;
using System.Collections.Generic;
using System.Linq;

using AntData.ORM;
using AntData.ORM.Data;
using AntData.ORM.Linq;
using AntData.ORM.Mapping;

namespace DbModels.PostgreSQL
{
	/// <summary>
	/// Database       : pgTest
	/// Data Source    : tcp://127.0.0.1:5432
	/// Server Version : 9.4.4
	/// </summary>
	public partial class PgTestEntitys : IEntity
	{
		public IQueryable<Person> People { get { return this.Get<Person>(); } }

		private readonly DataConnection con;

		public DataConnection DbContext
		{
			get { return this.con; }
		}

		public IQueryable<T> Get<T>()
			 where T : class
		{
			return this.con.GetTable<T>();
		}

		public PgTestEntitys(DataConnection con)
		{
			this.con = con;
		}
	}

	[Table(Schema="public", Name="person")]
	public partial class Person : BaseEntity
	{
		#region Column

		[Column("id",   DataType=AntData.ORM.DataType.Int32,     Precision=32, Scale=0), Identity]
		public int Id { get; set; } // integer

		[Column("name", DataType=AntData.ORM.DataType.Undefined), Nullable]
		public object Name { get; set; } // ARRAY

		#endregion
	}
}
