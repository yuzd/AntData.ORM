using System;
using System.Linq;

using AntData.ORM;
using AntData.ORM.Linq;
using AntData.ORM.Mapping;

namespace DbModels.Mysql2
{
	/// <summary>
	/// Database       : testorm1
	/// Data Source    : localhost
	/// Server Version : 5.6.26-log
	/// </summary>
	public partial class Entitys : IEntity
	{
		/// <summary>
		/// 订单表
		/// </summary>
		public IQueryable<Order> Orders { get { return this.Get<Order>(); } }

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
	/// 订单表
	/// </summary>
	[Table(Comment="订单表", Name="orders")]
	public partial class Order : BaseEntity
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
