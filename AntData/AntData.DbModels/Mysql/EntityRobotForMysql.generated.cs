using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using AntData.ORM;
using AntData.ORM.Linq;
using AntData.ORM.Mapping;

namespace DbModels.Mysql
{
	/// <summary>
	/// Database       : testorm
	/// Data Source    : localhost
	/// Server Version : 5.6.26-log
	/// </summary>
	public partial class Entitys : IEntity
	{
		/// <summary>
		/// 人员
		/// </summary>
		public IQueryable<Person> People  { get { return this.Get<Person>(); } }
		/// <summary>
		/// 学校
		/// </summary>
		public IQueryable<School> Schools { get { return this.Get<School>(); } }

		private readonly IDataContext con;

		public IQueryable<T> Get<T>()
			 where T : class
		{
			return this.con.GetTable<T>();
		}

		public Entitys(IDataContext con)
		{
			this.con = con;
		}
	}

	/// <summary>
	/// 人员
	/// </summary>
	[Table(Comment="人员", Name="person")]
	public partial class Person : BaseEntity
	{
		#region Column

		/// <summary>
		/// 主键
		/// </summary>
		[Column("Id",                  DataType=DataType.Int64,    Comment="主键"), PrimaryKey, Identity]
		public long Id { get; set; } // bigint(20)

		/// <summary>
		/// 最后更新时间
		/// </summary>
		[Column("DataChange_LastTime", DataType=DataType.DateTime, Comment="最后更新时间"), NotNull]
		public DateTime DataChangeLastTime // datetime
		{
			get { return _DataChangeLastTime; }
			set { _DataChangeLastTime = value; }
		}

		/// <summary>
		/// 姓名
		/// </summary>
		[Column("Name",                DataType=DataType.VarChar,  Length=50, Comment="姓名"), NotNull]
		public string Name { get; set; } // varchar(50)

		/// <summary>
		/// 年纪
		/// </summary>
		[Column("Age",                 DataType=DataType.Int32,    Comment="年纪"),    Nullable]
		public int? Age { get; set; } // int(11)

		/// <summary>
		/// 学校主键
		/// </summary>
		[Column("SchoolId",            DataType=DataType.Int64,    Comment="学校主键"),    Nullable]
		public long? SchoolId { get; set; } // bigint(20)

		#endregion

		#region Field

		private DateTime _DataChangeLastTime = System.Data.SqlTypes.SqlDateTime.MinValue.Value;

		#endregion

		#region Associations

		/// <summary>
		/// persons_school
		/// </summary>
		[Association(ThisKey="SchoolId", OtherKey="Id", CanBeNull=true, KeyName="persons_school", BackReferenceName="persons")]
		public School Personsschool { get; set; }

		#endregion
	}

	/// <summary>
	/// 学校
	/// </summary>
	[Table(Comment="学校", Name="school")]
	public partial class School : BaseEntity
	{
		#region Column

		/// <summary>
		/// 主键
		/// </summary>
		[Column("Id",                  DataType=DataType.Int64,    Comment="主键"), PrimaryKey, Identity]
		public long Id { get; set; } // bigint(20)

		/// <summary>
		/// 学校名称
		/// </summary>
		[Column("Name",                DataType=DataType.VarChar,  Length=50, Comment="学校名称"),    Nullable]
		public string Name { get; set; } // varchar(50)

		/// <summary>
		/// 学校地址
		/// </summary>
		[Column("Address",             DataType=DataType.VarChar,  Length=100, Comment="学校地址"),    Nullable]
		public string Address { get; set; } // varchar(100)

		/// <summary>
		/// 最后更新时间
		/// </summary>
		[Column("DataChange_LastTime", DataType=DataType.DateTime, Comment="最后更新时间"), NotNull]
		public DateTime DataChangeLastTime // datetime
		{
			get { return _DataChangeLastTime; }
			set { _DataChangeLastTime = value; }
		}

		#endregion

		#region Field

		private DateTime _DataChangeLastTime = System.Data.SqlTypes.SqlDateTime.MinValue.Value;

		#endregion

		#region Associations

		/// <summary>
		/// persons_school_BackReference
		/// </summary>
		[Association(ThisKey="Id", OtherKey="SchoolId", CanBeNull=true, IsBackReference=true)]
		public IEnumerable<Person> Persons { get; set; }

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
