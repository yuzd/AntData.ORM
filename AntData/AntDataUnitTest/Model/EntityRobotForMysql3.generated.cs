using System;
using System.Linq;

using AntData.ORM;
using AntData.ORM.Linq;
using AntData.ORM.Mapping;

namespace DbModels.Mysql3
{
	/// <summary>
	/// Database       : testorm3
	/// Data Source    : localhost
	/// Server Version : 5.6.26-log
	/// </summary>
	public partial class Entitys : IEntity
	{
		public IQueryable<Orders> Orders { get { return this.Get<Orders>(); } }

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

	[Table("orders_{0}")]
	public partial class Orders : BaseEntity
	{
		#region Column

		/// <summary>
		/// 订单号
		/// </summary>
		[Column("ID",   DataType=DataType.Int64,   Comment="订单号"), Nullable]
		public long? ID { get; set; } // bigint(20)

		/// <summary>
		/// 名称
		/// </summary>
		[Column("Name", DataType=DataType.VarChar, Length=50, Comment="名称"), Nullable]
		public string Name { get; set; } // varchar(50)

		#endregion
	}
}
