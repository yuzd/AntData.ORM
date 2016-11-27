using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

using AntData.ORM;
using AntData.ORM.Linq;
using AntData.ORM.Mapping;

namespace DbModels.SqlServer
{
	/// <summary>
	/// Database       : Test
	/// Data Source    : YUZD\SERVERQ
	/// Server Version : 11.00.3156
	/// </summary>
	public partial class Entitys : IEntity
	{
		/// <summary>
		/// 订单表
		/// </summary>
		public ITable<QOrder> QOrders { get { return this.Get<QOrder>(); } }
		/// <summary>
		/// 用户表
		/// </summary>
		public ITable<QUser>  QUsers  { get { return this.Get<QUser>(); } }

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
	/// 订单表
	/// </summary>
	[Table(Schema="dbo", Name="QOrder")]
	public partial class QOrder : BaseEntity
	{
		/// <summary>
		/// 主键
		/// </summary>
		[Column("Guid",      DataType=DataType.VarChar, Length=50), NotNull]
		public string Guid { get; set; } // varchar(50)

		/// <summary>
		/// 用户Id
		/// </summary>
		[Column("QUserId",   DataType=DataType.VarChar, Length=50),    Nullable]
		public string QUserId { get; set; } // varchar(50)

		/// <summary>
		/// 订单名称
		/// </summary>
		[Column("OrderName", DataType=DataType.VarChar, Length=50),    Nullable]
		public string OrderName { get; set; } // varchar(50)
	}

	/// <summary>
	/// 用户表
	/// </summary>
	[Table(Schema="dbo", Name="QUser")]
	public partial class QUser : BaseEntity
	{
		/// <summary>
		/// 主键
		/// </summary>
		[Column("Guid",       DataType=DataType.VarChar,  Length=36), PrimaryKey, NotNull]
		public string Guid { get; set; } // varchar(36)

		/// <summary>
		/// 用户名称
		/// </summary>
		[Column("UserName",   DataType=DataType.VarChar,  Length=50),    Nullable]
		public string UserName { get; set; } // varchar(50)

		/// <summary>
		/// 地址
		/// </summary>
		[Column("Address",    DataType=DataType.VarChar,  Length=50),    Nullable]
		public string Address { get; set; } // varchar(50)

		/// <summary>
		/// 更新时间
		/// </summary>
		[Column("UpdateTime", DataType=DataType.DateTime),    Nullable]
		public DateTime? UpdateTime { get; set; } // datetime

		/// <summary>
		/// 创建时间
		/// </summary>
		[Column("CreateTime", DataType=DataType.DateTime),    Nullable]
		public DateTime? CreateTime { get; set; } // datetime
	}
}
