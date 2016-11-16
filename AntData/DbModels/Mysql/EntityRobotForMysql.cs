using System;
using System.Linq;

using AntData.ORM;
using AntData.ORM.Linq;
using AntData.ORM.Mapping;

namespace DbModels.Mysql
{
	/// <summary>
	/// Database       : testorm
	/// Data Source    : 127.0.0.1
	/// Server Version : 5.6.26-log
	/// </summary>
	public partial class Entitys : IEntity
	{
		public IQueryable<Person> People  { get { return this.Get<Person>(); } }
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

	[Table("person")]
	public partial class Person : BaseEntity
	{
		/// <summary>
		/// 主键
		/// </summary>
		[Column("Id",                  DataType=DataType.Int64)   , PrimaryKey, Identity]
		public long Id { get; set; } // bigint(20)

		/// <summary>
		/// 姓名
		/// </summary>
		[Column("Name",                DataType=DataType.VarChar,  Length=50),    Nullable]
		public string Name { get; set; } // varchar(50)

		/// <summary>
		/// 年纪
		/// </summary>
		[Column("Age",                 DataType=DataType.Int32)   ,    Nullable]
		public int? Age { get; set; } // int(11)
	}

	[Table("school")]
	public partial class School : BaseEntity
	{
		/// <summary>
		/// 主键
		/// </summary>
		[Column("Id",                  DataType=DataType.Int64)   , PrimaryKey, Identity]
		public long Id { get; set; } // bigint(20)

		/// <summary>
		/// 学校名称
		/// </summary>
		[Column("Name",                DataType=DataType.VarChar,  Length=50),    Nullable]
		public string Name { get; set; } // varchar(50)

		/// <summary>
		/// 学校地址
		/// </summary>
		[Column("Address",             DataType=DataType.VarChar,  Length=100),    Nullable]
		public string Address { get; set; } // varchar(100)
	}
}
