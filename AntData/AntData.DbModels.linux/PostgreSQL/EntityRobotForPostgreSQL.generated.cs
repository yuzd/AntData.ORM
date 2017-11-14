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
		public IQueryable<Person> People  { get { return this.Get<Person>(); } }
		/// <summary>
		/// 学校表
		/// </summary>
		public IQueryable<School> Schools { get { return this.Get<School>(); } }

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
		[Column("id",                  DataType=AntData.ORM.DataType.Int64,     Precision=64, Scale=0, Comment="自增主键"), PrimaryKey, Identity]
		public long Id { get; set; } // bigint

		/// <summary>
		/// 名字
		/// </summary>
		[Column("name",                DataType=AntData.ORM.DataType.NVarChar,  Length=20, Comment="名字"), NotNull]
		public string Name { get; set; } // character varying(20)

		/// <summary>
		/// 年龄
		/// </summary>
		[Column("age",                 DataType=AntData.ORM.DataType.Int32,     Precision=32, Scale=0, Comment="年龄"),    Nullable]
		public int? Age { get; set; } // integer

		/// <summary>
		/// 最后更新时间
		/// </summary>
		[Column("DataChange_LastTime", DataType=AntData.ORM.DataType.DateTime2, Precision=6, Comment="最后更新时间"), NotNull]
		public DateTime DataChangeLastTime // timestamp (6) without time zone
		{
			get { return _DataChangeLastTime; }
			set { _DataChangeLastTime = value; }
		}

		/// <summary>
		/// 学校id
		/// </summary>
		[Column("SchoolId",            DataType=AntData.ORM.DataType.Int64,     Precision=64, Scale=0, Comment="学校id"),    Nullable]
		public long? SchoolId { get; set; } // bigint

		#endregion

		#region Field

		private DateTime _DataChangeLastTime = System.Data.SqlTypes.SqlDateTime.MinValue.Value;

		#endregion

		#region Associations

		/// <summary>
		/// pgTest.public.person_fk_person_school_id
		/// </summary>
		[Association(ThisKey="SchoolId", OtherKey="Id", CanBeNull=true, KeyName="pgTest.public.person_fk_person_school_id", BackReferenceName="pgTest.public.personfkpersonid")]
		public School School { get; set; }

		#endregion
	}

	/// <summary>
	/// 学校表
	/// </summary>
	[Table(Schema="public", Comment="学校表", Name="school")]
	public partial class School : BaseEntity
	{
		#region Column

		/// <summary>
		/// 自增主键
		/// </summary>
		[Column("id",                  DataType=AntData.ORM.DataType.Int64,     Precision=64, Scale=0, Comment="自增主键"), PrimaryKey, Identity]
		public long Id { get; set; } // bigint

		/// <summary>
		/// 名称
		/// </summary>
		[Column("name",                DataType=AntData.ORM.DataType.NVarChar,  Length=50, Comment="名称"),    Nullable]
		public string Name { get; set; } // character varying(50)

		/// <summary>
		/// 地址
		/// </summary>
		[Column("address",             DataType=AntData.ORM.DataType.NVarChar,  Length=100, Comment="地址"),    Nullable]
		public string Address { get; set; } // character varying(100)

		/// <summary>
		/// 最后更新时间
		/// </summary>
		[Column("DataChange_LastTime", DataType=AntData.ORM.DataType.DateTime2, Precision=6, Comment="最后更新时间"), NotNull]
		public DateTime DataChangeLastTime // timestamp (6) without time zone
		{
			get { return _DataChangeLastTime; }
			set { _DataChangeLastTime = value; }
		}

		/// <summary>
		/// json测试
		/// </summary>
		[Column("attr",                DataType=AntData.ORM.DataType.Undefined, Comment="json测试"),    Nullable]
		public object Attr { get; set; } // json

		#endregion

		#region Field

		private DateTime _DataChangeLastTime = System.Data.SqlTypes.SqlDateTime.MinValue.Value;

		#endregion

		#region Associations

		/// <summary>
		/// pgTest.public.person_fk_person_school_id_BackReference
		/// </summary>
		[Association(ThisKey="Id", OtherKey="SchoolId", CanBeNull=true, IsBackReference=true)]
		public IEnumerable<Person> PersonList { get; set; }

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

		public static School FindByBk(this IQueryable<School> table, long Id)
		{
			return table.FirstOrDefault(t =>
				t.Id == Id);
		}

		public static async Task<School> FindByBkAsync(this IQueryable<School> table, long Id)
		{
			return await table.FirstOrDefaultAsync(t =>
				t.Id == Id);
		}
	}
}
