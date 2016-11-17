using System;
using System.Collections.Generic;
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
		public ITable<Person> People  { get { return this.Get<Person>(); } }
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

		/// <summary>
		/// 学校主键
		/// </summary>
		[Column("SchoolId",            DataType=DataType.Int64)   ,    Nullable]
		public long? SchoolId { get; set; } // bigint(20)

		#region Associations

		/// <summary>
		/// persons_school
		/// </summary>
		[Association(ThisKey="SchoolId", OtherKey="Id", CanBeNull=true, KeyName="persons_school", BackReferenceName="persons")]
		public School Personsschool { get; set; }

		#endregion
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

		#region Associations

		/// <summary>
		/// persons_school_BackReference
		/// </summary>
		[Association(ThisKey="Id", OtherKey="SchoolId", CanBeNull=true, IsBackReference=true)]
		public IEnumerable<Person> Persons { get; set; }

		#endregion
	}
}
