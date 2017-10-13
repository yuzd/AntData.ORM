using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
		/// <summary>
		/// 用户表
		/// </summary>
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

	/// <summary>
	/// 用户表
	/// </summary>
	[Table(Schema="public", Comment="用户表", Name="person")]
	public partial class Person : BaseEntity
	{
		#region Column

		/// <summary>
		/// 自增主键
		/// </summary>
		[Column("id",   DataType=AntData.ORM.DataType.Int64,    Precision=64, Scale=0, Comment="自增主键"), PrimaryKey, Identity]
		public long Id { get; set; } // bigint

		/// <summary>
		/// 名字
		/// </summary>
		[Column("name", DataType=AntData.ORM.DataType.NVarChar, Length=20, Comment="名字"), Nullable]
		public string Name { get; set; } // character varying(20)

		#endregion
	}

	public static partial class TableExtensions
	{
		public static Person FindByBk(this IQueryable<Person> table, long Id)
		{
			return table.FirstOrDefault(t =>
				t.Id == Id);
		}

		public static async Task<Person> FindByBkAsync(this IQueryable<Person> table, long Id)
		{
			return await table.FirstOrDefaultAsync(t =>
				t.Id == Id);
		}
	}
}
