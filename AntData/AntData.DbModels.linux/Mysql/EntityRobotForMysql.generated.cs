using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using AntData.ORM;
using AntData.ORM.Data;
using AntData.ORM.Linq;
using AntData.ORM.Mapping;

namespace DbModels.Mysql
{
	/// <summary>
	/// Database       : testorm
	/// Data Source    : localhost
	/// Server Version : 5.6.26-log
	/// </summary>
	public partial class TestormEntitys : IEntity
	{
		public IQueryable<ChildDistrict> ChildDistrict { get { return this.Get<ChildDistrict>(); } }
		public IQueryable<District>      Districts     { get { return this.Get<District>(); } }
		/// <summary>
		/// import
		/// </summary>
		public IQueryable<Import>        Imports       { get { return this.Get<Import>(); } }
		/// <summary>
		/// ORM
		/// </summary>
		public IQueryable<Orm>           Orms          { get { return this.Get<Orm>(); } }
		/// <summary>
		/// 人员
		/// </summary>
		public IQueryable<Person>        People        { get { return this.Get<Person>(); } }
		/// <summary>
		/// 学校
		/// </summary>
		public IQueryable<School>        Schools       { get { return this.Get<School>(); } }
		public IQueryable<SchoolName>    SchoolName    { get { return this.Get<SchoolName>(); } }
		public IQueryable<school_name>   school_name   { get { return this.Get<school_name>(); } }

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

		public TestormEntitys(DataConnection con)
		{
			this.con = con;
		}
	}

	[Table("child_district")]
	public partial class ChildDistrict : BaseEntity
	{
		#region Column

		/// <summary>
		/// 主键
		/// </summary>
		[Column("Tid",                 DataType=AntData.ORM.DataType.Int64,    Comment="主键"), PrimaryKey, Identity]
		public long Tid { get; set; } // bigint(20)

		/// <summary>
		/// 名称
		/// </summary>
		[Column("Name",                DataType=AntData.ORM.DataType.VarChar,  Length=30, Comment="名称"), Nullable]
		public string Name { get; set; } // varchar(30)

		/// <summary>
		/// Code
		/// </summary>
		[Column("Code",                DataType=AntData.ORM.DataType.VarChar,  Length=30, Comment="Code"), Nullable]
		public string Code { get; set; } // varchar(30)

		/// <summary>
		/// 更新时间
		/// </summary>
		[Column("DataChange_LastTime", DataType=AntData.ORM.DataType.DateTime, Comment="更新时间"), Nullable]
		public DateTime? DataChangeLastTime { get; set; } // datetime

		#endregion
	}

	[Table("district")]
	public partial class District : BaseEntity
	{
		#region Column

		/// <summary>
		/// 主键
		/// </summary>
		[Column("Tid",                 DataType=AntData.ORM.DataType.Int64,    Comment="主键"), PrimaryKey, Identity]
		public long Tid { get; set; } // bigint(20)

		/// <summary>
		/// 区域名称
		/// </summary>
		[Column("Name",                DataType=AntData.ORM.DataType.VarChar,  Length=30, Comment="区域名称"), Nullable]
		public string Name { get; set; } // varchar(30)

		/// <summary>
		/// 区域Code
		/// </summary>
		[Column("Code",                DataType=AntData.ORM.DataType.VarChar,  Length=30, Comment="区域Code"), Nullable]
		public string Code { get; set; } // varchar(30)

		/// <summary>
		/// 更新时间
		/// </summary>
		[Column("DataChange_LastTime", DataType=AntData.ORM.DataType.DateTime, Comment="更新时间"), Nullable]
		public DateTime? DataChangeLastTime { get; set; } // datetime

		#endregion
	}

	/// <summary>
	/// import
	/// </summary>
	[Table(Comment="import", Name="import")]
	public partial class Import : BaseEntity
	{
		#region Column

		/// <summary>
		/// 主键
		/// </summary>
		[Column("Id",    DataType=AntData.ORM.DataType.Int64,   Comment="主键"), PrimaryKey, Identity]
		public long Id { get; set; } // bigint(20)

		/// <summary>
		/// 标签
		/// </summary>
		[Column("Label", DataType=AntData.ORM.DataType.VarChar, Length=20, Comment="标签"), Nullable]
		public string Label { get; set; } // varchar(20)

		#endregion
	}

	/// <summary>
	/// ORM
	/// </summary>
	[Table(Comment="ORM", Name="orm")]
	public partial class Orm : BaseEntity
	{
		#region Column

		/// <summary>
		/// 主键
		/// </summary>
		[Column("Id",    DataType=AntData.ORM.DataType.Int64,   Comment="主键"), PrimaryKey, Identity]
		public long Id { get; set; } // bigint(20)

		/// <summary>
		/// 标签
		/// </summary>
		[Column("Label", DataType=AntData.ORM.DataType.VarChar, Length=20, Comment="标签"), Nullable]
		public string Label { get; set; } // varchar(20)

		#endregion
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
		[Column("Id",                  DataType=AntData.ORM.DataType.Int64,    Comment="主键"), PrimaryKey, Identity]
		public long Id { get; set; } // bigint(20)

		/// <summary>
		/// 最后更新时间
		/// </summary>
		[Column("DataChange_LastTime", DataType=AntData.ORM.DataType.DateTime, Comment="最后更新时间"), NotNull]
		public DateTime DataChangeLastTime // datetime
		{
			get { return _DataChangeLastTime; }
			set { _DataChangeLastTime = value; }
		}

		/// <summary>
		/// 姓名
		/// </summary>
		[Column("Name",                DataType=AntData.ORM.DataType.VarChar,  Length=50, Comment="姓名"), NotNull]
		public string Name { get; set; } // varchar(50)

		/// <summary>
		/// 年纪
		/// </summary>
		[Column("Age",                 DataType=AntData.ORM.DataType.Int32,    Comment="年纪"),    Nullable]
		public int? Age { get; set; } // int(11)

		/// <summary>
		/// 学校主键
		/// </summary>
		[Column("SchoolId",            DataType=AntData.ORM.DataType.Int64,    Comment="学校主键"),    Nullable]
		public long? SchoolId { get; set; } // bigint(20)

		#endregion

		#region Field

		private DateTime _DataChangeLastTime = System.Data.SqlTypes.SqlDateTime.MinValue.Value;

		#endregion

		#region Associations

		/// <summary>
		/// person_SchoolId_school_Id
		/// </summary>
		[Association(ThisKey="SchoolId", OtherKey="Id", CanBeNull=true, IsBackReference=true)]
		public School School { get; set; }

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
		[Column("Id",                  DataType=AntData.ORM.DataType.Int64,    Comment="主键"), PrimaryKey, Identity]
		public long Id { get; set; } // bigint(20)

		/// <summary>
		/// 学校名称
		/// </summary>
		[Column("Name",                DataType=AntData.ORM.DataType.VarChar,  Length=50, Comment="学校名称"),    Nullable]
		public string Name { get; set; } // varchar(50)

		/// <summary>
		/// 学校地址
		/// </summary>
		[Column("Address",             DataType=AntData.ORM.DataType.VarChar,  Length=100, Comment="学校地址"),    Nullable]
		public string Address { get; set; } // varchar(100)

		/// <summary>
		/// 最后更新时间
		/// </summary>
		[Column("DataChange_LastTime", DataType=AntData.ORM.DataType.DateTime, Comment="最后更新时间"), NotNull]
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
		/// school_Id_person_SchoolId
		/// </summary>
		[Association(ThisKey="Id", OtherKey="SchoolId", CanBeNull=true, IsBackReference=true)]
		public IEnumerable<Person> PersonList { get; set; }

		#endregion
	}

	[Table("school__name")]
	public partial class SchoolName : BaseEntity
	{
		#region Column

		/// <summary>
		/// 主键
		/// </summary>
		[Column("Id", DataType=AntData.ORM.DataType.Int64, Comment="主键"), PrimaryKey, Identity]
		public long Id { get; set; } // bigint(20)

		#endregion
	}

	[Table("school_name")]
	public partial class school_name : BaseEntity
	{
		#region Column

		/// <summary>
		/// 主键
		/// </summary>
		[Column("Id",      DataType=AntData.ORM.DataType.Int64,   Comment="主键"), PrimaryKey, NotNull]
		public long Id { get; set; } // bigint(20)

		/// <summary>
		/// 测试
		/// </summary>
		[Column("Name",    DataType=AntData.ORM.DataType.Boolean, Comment="测试"), NotNull]
		public bool Name { get; set; } // bit(1)

		/// <summary>
		/// 订单号
		/// </summary>
		[Column("OrderID", DataType=AntData.ORM.DataType.Int64,   Comment="订单号"), NotNull]
		public long OrderID { get; set; } // bigint(20)

		#endregion
	}

	public static partial class TableExtensions
	{
		public static ChildDistrict FindByBk(this IQueryable<ChildDistrict> table, long Tid)
		{
			return table.FirstOrDefault(t =>
				t.Tid == Tid);
		}

		public static async Task<ChildDistrict> FindByBkAsync(this IQueryable<ChildDistrict> table, long Tid)
		{
			return await table.FirstOrDefaultAsync(t =>
				t.Tid == Tid);
		}

		public static District FindByBk(this IQueryable<District> table, long Tid)
		{
			return table.FirstOrDefault(t =>
				t.Tid == Tid);
		}

		public static async Task<District> FindByBkAsync(this IQueryable<District> table, long Tid)
		{
			return await table.FirstOrDefaultAsync(t =>
				t.Tid == Tid);
		}

		public static Import FindByBk(this IQueryable<Import> table, long Id)
		{
			return table.FirstOrDefault(t =>
				t.Id == Id);
		}

		public static async Task<Import> FindByBkAsync(this IQueryable<Import> table, long Id)
		{
			return await table.FirstOrDefaultAsync(t =>
				t.Id == Id);
		}

		public static Orm FindByBk(this IQueryable<Orm> table, long Id)
		{
			return table.FirstOrDefault(t =>
				t.Id == Id);
		}

		public static async Task<Orm> FindByBkAsync(this IQueryable<Orm> table, long Id)
		{
			return await table.FirstOrDefaultAsync(t =>
				t.Id == Id);
		}

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

		public static SchoolName FindByBk(this IQueryable<SchoolName> table, long Id)
		{
			return table.FirstOrDefault(t =>
				t.Id == Id);
		}

		public static async Task<SchoolName> FindByBkAsync(this IQueryable<SchoolName> table, long Id)
		{
			return await table.FirstOrDefaultAsync(t =>
				t.Id == Id);
		}

		public static school_name FindByBk(this IQueryable<school_name> table, long Id)
		{
			return table.FirstOrDefault(t =>
				t.Id == Id);
		}

		public static async Task<school_name> FindByBkAsync(this IQueryable<school_name> table, long Id)
		{
			return await table.FirstOrDefaultAsync(t =>
				t.Id == Id);
		}
	}
}
