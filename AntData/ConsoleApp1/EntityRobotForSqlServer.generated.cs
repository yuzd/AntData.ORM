using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

using AntData.ORM;
using AntData.ORM.Linq;
using AntData.ORM.Mapping;

namespace DbModels.SqlServer
{
	/// <summary>
	/// Database       : Test
	/// Data Source    : .\SERVERQ
	/// Server Version : 11.00.3156
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
		public IQueryable<Test>   Tests   { get { return this.Get<Test>(); } }

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
	[Table(Schema="dbo", Comment="人员", Name="person")]
	public partial class Person : BaseEntity
	{
		#region Column

		[Column("Id",                  DataType=DataType.Int64)   , PrimaryKey, Identity]
		public long Id { get; set; } // bigint

		[Column("DataChange_LastTime", DataType=DataType.DateTime), NotNull]
		public DateTime DataChangeLastTime // datetime
		{
			get { return _DataChangeLastTime; }
			set { _DataChangeLastTime = value; }
		}

		[Column("Name",                DataType=DataType.VarChar,  Length=50), NotNull]
		public string Name { get; set; } // varchar(50)

		[Column("Age",                 DataType=DataType.Int32)   ,    Nullable]
		public int? Age { get; set; } // int

		[Column("SchoolId",            DataType=DataType.Int64)   ,    Nullable]
		public long? SchoolId { get; set; } // bigint

		#endregion

		#region Field

		private DateTime _DataChangeLastTime = System.Data.SqlTypes.SqlDateTime.MinValue.Value;

		#endregion

		#region Associations

		/// <summary>
		/// FK_persons_school
		/// </summary>
		[Association(ThisKey="SchoolId", OtherKey="Id", CanBeNull=true, KeyName="FK_persons_school", BackReferenceName="persons")]
		public School Personsschool { get; set; }

		#endregion
	}

	/// <summary>
	/// 学校
	/// </summary>
	[Table(Schema="dbo", Comment="学校", Name="school")]
	public partial class School : BaseEntity
	{
		#region Column

		/// <summary>
		/// 主键
		/// </summary>
		[Column("Id",                  DataType=DataType.Int64,    Comment="主键"), PrimaryKey, Identity]
		public long Id { get; set; } // bigint

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
		/// FK_persons_school_BackReference
		/// </summary>
		[Association(ThisKey="Id", OtherKey="SchoolId", CanBeNull=true, IsBackReference=true)]
		public IEnumerable<Person> Persons { get; set; }

		#endregion
	}

	[Table(Schema="dbo", Name="Test")]
	public partial class Test : BaseEntity
	{
		#region Column

		[Column("Id",         DataType=DataType.Int32)   , PrimaryKey, Identity]
		public int Id { get; set; } // int

		[Column("F_Byte",     DataType=DataType.Byte)    , Nullable]
		public byte? FByte { get; set; } // tinyint

		[Column("F_Int16",    DataType=DataType.Int16)   , Nullable]
		public short? FInt16 { get; set; } // smallint

		[Column("F_Int32",    DataType=DataType.Int32)   , Nullable]
		public int? FInt32 { get; set; } // int

		[Column("F_Int64",    DataType=DataType.Int64)   , Nullable]
		public long? FInt64 { get; set; } // bigint

		[Column("F_Double",   DataType=DataType.Double)  , Nullable]
		public double? FDouble { get; set; } // float

		[Column("F_Float",    DataType=DataType.Single)  , Nullable]
		public float? FFloat { get; set; } // real

		[Column("F_Decimal",  DataType=DataType.Decimal,  Precision=18, Scale=0), Nullable]
		public decimal? FDecimal { get; set; } // decimal(18, 0)

		[Column("F_Bool",     DataType=DataType.Boolean) , Nullable]
		public bool? FBool { get; set; } // bit

		[Column("F_DateTime", DataType=DataType.DateTime), Nullable]
		public DateTime? FDateTime { get; set; } // datetime

		[Column("F_Guid",     DataType=DataType.Guid)    , Nullable]
		public Guid? FGuid { get; set; } // uniqueidentifier

		[Column("F_String",   DataType=DataType.NVarChar, Length=100), Nullable]
		public string FString { get; set; } // nvarchar(100)

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

		public static Test FindByBk(this IQueryable<Test> table, int Id)
		{
			return table.FirstOrDefault(t =>
				t.Id == Id);
		}

		public static async Task<Test> FindByBkAsync(this IQueryable<Test> table, int Id)
		{
			return await table.FirstOrDefaultAsync(t =>
				t.Id == Id);
		}
	}
}
