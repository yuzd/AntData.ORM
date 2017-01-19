using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using AntData.ORM;
using AntData.ORM.Linq;
using AntData.ORM.Mapping;

namespace DbModels.Oracle
{
	/// <summary>
	/// Database       : orcl
	/// Data Source    : dbtest
	/// Server Version : 11.2.0.1.0
	/// </summary>
	public partial class Entitys : IEntity
	{
		/// <summary>
		/// 用户表
		/// </summary>
		public ITable<Person> People  { get { return this.Get<Person>(); } }
		/// <summary>
		/// 学校表
		/// </summary>
		public ITable<School> Schools { get { return this.Get<School>(); } }

		private readonly IDataContext con;

		public ITable<T> Get<T>()
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
	/// 用户表
	/// </summary>
	[Table(Schema="TEST", Comment="用户表", Name="person")]
	public partial class Person : BaseEntity
	{
		#region Column

		/// <summary>
		/// 主键
		/// </summary>
		[Column("Id",                  DataType=DataType.Decimal,  Length=22, Precision=15, Scale=0, Comment="主键"), PrimaryKey, NotNull]
		public long Id { get; set; } // NUMBER (15,0)

		/// <summary>
		/// 姓名
		/// </summary>
		[Column("Name",                DataType=DataType.VarChar,  Length=50, Comment="姓名"),    Nullable]
		public string Name { get; set; } // VARCHAR2(50)

		/// <summary>
		/// 年纪
		/// </summary>
		[Column("Age",                 DataType=DataType.Decimal,  Length=22, Precision=5, Scale=0, Comment="年纪"),    Nullable]
		public int? Age { get; set; } // NUMBER (5,0)

		/// <summary>
		/// School外键
		/// </summary>
		[Column("SchoolId",            DataType=DataType.Decimal,  Length=22, Precision=15, Scale=0, Comment="School外键"),    Nullable]
		public long? SchoolId { get; set; } // NUMBER (15,0)

		/// <summary>
		/// 最后更新时间
		/// </summary>
		[Column("DataChange_LastTime", DataType=DataType.DateTime, Length=7, Comment="最后更新时间"),    Nullable]
		public DateTime? DataChangeLastTime { get; set; } // DATE

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
	/// 学校表
	/// </summary>
	[Table(Schema="TEST", Comment="学校表", Name="school")]
	public partial class School : BaseEntity
	{
		#region Column

		[Column("Id",                  DataType=DataType.Decimal,  Length=22, Precision=15, Scale=0), PrimaryKey, NotNull]
		public long Id { get; set; } // NUMBER (15,0)

		/// <summary>
		/// 学校名称
		/// </summary>
		[Column("Name",                DataType=DataType.VarChar,  Length=50, Comment="学校名称"),    Nullable]
		public string Name { get; set; } // VARCHAR2(50)

		/// <summary>
		/// 学校地址
		/// </summary>
		[Column("Address",             DataType=DataType.VarChar,  Length=100, Comment="学校地址"),    Nullable]
		public string Address { get; set; } // VARCHAR2(100)

		/// <summary>
		/// 最后更新时间
		/// </summary>
		[Column("DataChange_LastTime", DataType=DataType.DateTime, Length=7, Comment="最后更新时间"),    Nullable]
		public DateTime? DataChangeLastTime { get; set; } // DATE

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
		public static Person FindByBk(this ITable<Person> table, long Id)
		{
			return table.FirstOrDefault(t =>
				t.Id == Id);
		}

		public static async Task<Person> FindByBkAsync(this ITable<Person> table, long Id)
		{
			return await table.FirstOrDefaultAsync(t =>
				t.Id == Id);
		}

		public static School FindByBk(this ITable<School> table, long Id)
		{
			return table.FirstOrDefault(t =>
				t.Id == Id);
		}

		public static async Task<School> FindByBkAsync(this ITable<School> table, long Id)
		{
			return await table.FirstOrDefaultAsync(t =>
				t.Id == Id);
		}
	}
}
