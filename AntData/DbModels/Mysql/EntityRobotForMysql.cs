using System;
using System.Linq;

using AntData.ORM;
using AntData.ORM.Mapping;
using AntData.ORM.Mysql.Base;

namespace DbModels.Mysql
{
	/// <summary>
	/// Database       : iworkcloudbagdb
	/// Data Source    : 127.0.0.1
	/// Server Version : 5.6.26-log
	/// </summary>
	public partial class Entitys : IEntity
	{
		/// <summary>
		/// 购买过服务的用户信息记录表
		/// </summary>
		public IQueryable<Account>                  Accounts                 { get { return this.Get<Account>(); } }
		public IQueryable<Config>                   Configs                  { get { return this.Get<Config>(); } }
		/// <summary>
		/// 这个表应该显示的是你获取到的订单号(有且只会有一条记录)
		/// </summary>
		public IQueryable<CurrentOrderId>           CurrentOrderId           { get { return this.Get<CurrentOrderId>(); } }
		/// <summary>
		/// 省市地区表
		/// </summary>
		public IQueryable<District>                 Districts                { get { return this.Get<District>(); } }
		/// <summary>
		/// 地区行李尺寸价格配置表
		/// </summary>
		public IQueryable<DistrictGoods>            DistrictGoods            { get { return this.Get<DistrictGoods>(); } }
		/// <summary>
		/// 地区行李尺寸价格配置中间表
		/// </summary>
		public IQueryable<DistrictGoodsPackageType> DistrictGoodsPackageType { get { return this.Get<DistrictGoodsPackageType>(); } }
		/// <summary>
		/// 城市服务类型对应表
		/// </summary>
		public IQueryable<DistrictService>          DistrictService          { get { return this.Get<DistrictService>(); } }
		/// <summary>
		/// 保价配置表
		/// </summary>
		public IQueryable<InsuranceType>            InsuranceType            { get { return this.Get<InsuranceType>(); } }
		/// <summary>
		/// 埋点数据
		/// </summary>
		public IQueryable<OneUbt>                   OneUbt                   { get { return this.Get<OneUbt>(); } }
		/// <summary>
		/// 订单表
		/// </summary>
		public IQueryable<Order>                    Orders                   { get { return this.Get<Order>(); } }
		/// <summary>
		/// 这个是订单号分配过来的订单池数据
		/// </summary>
		public IQueryable<OrderIds>                 OrderIds                 { get { return this.Get<OrderIds>(); } }
		/// <summary>
		/// 订单额外信息表
		/// </summary>
		public IQueryable<OrderMessage>             OrderMessage             { get { return this.Get<OrderMessage>(); } }
		/// <summary>
		/// 订单下的行李尺寸价格明细表
		/// </summary>
		public IQueryable<OrderPackageDetail>       OrderPackageDetail       { get { return this.Get<OrderPackageDetail>(); } }
		/// <summary>
		/// 订单支付信息表
		/// </summary>
		public IQueryable<OrderPayment>             OrderPayment             { get { return this.Get<OrderPayment>(); } }
		/// <summary>
		/// 订单规则
		/// </summary>
		public IQueryable<OrderRule>                OrderRule                { get { return this.Get<OrderRule>(); } }
		/// <summary>
		/// 订单状态更改记录表
		/// </summary>
		public IQueryable<OrderStateChange>         OrderStateChange         { get { return this.Get<OrderStateChange>(); } }
		/// <summary>
		/// 订单系统用户表
		/// </summary>
		public IQueryable<OrderUser>                OrderUser                { get { return this.Get<OrderUser>(); } }
		/// <summary>
		/// 行李尺寸价格明细配置
		/// </summary>
		public IQueryable<PackageType>              PackageType              { get { return this.Get<PackageType>(); } }
		/// <summary>
		/// 提供的服务产品表
		/// </summary>
		public IQueryable<Product>                  Products                 { get { return this.Get<Product>(); } }
		/// <summary>
		/// 产品对应行李尺寸价格配置表
		/// </summary>
		public IQueryable<ProductPackageType>       ProductPackageType       { get { return this.Get<ProductPackageType>(); } }
		/// <summary>
		/// 供应商表
		/// </summary>
		public IQueryable<Provider>                 Providers                { get { return this.Get<Provider>(); } }
		/// <summary>
		/// 供应商订单表
		/// </summary>
		public IQueryable<ProviderOrder>            ProviderOrder            { get { return this.Get<ProviderOrder>(); } }
		/// <summary>
		/// 供应商提供的服务产品表
		/// </summary>
		public IQueryable<ProviderProduct>          ProviderProduct          { get { return this.Get<ProviderProduct>(); } }
		/// <summary>
		/// 供应商用户表
		/// </summary>
		public IQueryable<ProviderUser>             ProviderUser             { get { return this.Get<ProviderUser>(); } }
		public IQueryable<SystemMenu>               SystemMenu               { get { return this.Get<SystemMenu>(); } }
		public IQueryable<SystemRole>               SystemRole               { get { return this.Get<SystemRole>(); } }
		public IQueryable<SystemUser>               SystemUser               { get { return this.Get<SystemUser>(); } }
		/// <summary>
		/// 交接方式配置表
		/// </summary>
		public IQueryable<TransferType>             TransferType             { get { return this.Get<TransferType>(); } }

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
	/// 购买过服务的用户信息记录表
	/// </summary>
	[Table("accounts")]
	public partial class Account : LinqToDBEntity
	{
		/// <summary>
		/// 用户Uid
		/// </summary>
		[Column("Uid",                 DataType=DataType.VarChar,  Length=36),    Nullable]
		public string Uid { get; set; } // varchar(36)

		/// <summary>
		/// 用户名称
		/// </summary>
		[Column("UserName",            DataType=DataType.VarChar,  Length=50),    Nullable]
		public string UserName { get; set; } // varchar(50)

		/// <summary>
		/// 用户手机号
		/// </summary>
		[Column("UserPhone",           DataType=DataType.VarChar,  Length=24),    Nullable]
		public string UserPhone { get; set; } // varchar(24)

		/// <summary>
		/// 用户邮箱
		/// </summary>
		[Column("Email",               DataType=DataType.VarChar,  Length=50),    Nullable]
		public string Email { get; set; } // varchar(50)

		/// <summary>
		/// 用户身份证号或者护照号
		/// </summary>
		[Column("CardNo",              DataType=DataType.VarChar,  Length=50),    Nullable]
		public string CardNo { get; set; } // varchar(50)
	}

	[Table("config")]
	public partial class Config : LinqToDBEntity
	{
		/// <summary>
		/// 类型1:首页轮播
		/// </summary>
		[Column("Type",                DataType=DataType.Int32)   ,    Nullable]
		public int? Type { get; set; } // int(11)

		/// <summary>
		/// 具体内容
		/// </summary>
		[Column("Value",               DataType=DataType.VarChar,  Length=10480),    Nullable]
		public string Value { get; set; } // varchar(10480)
	}

	/// <summary>
	/// 这个表应该显示的是你获取到的订单号(有且只会有一条记录)
	/// </summary>
	[Table("current_order_id")]
	public partial class CurrentOrderId : LinqToDBEntity
	{
		/// <summary>
		/// 唯一订单号
		/// </summary>
		[Column("CurrentId",           DataType=DataType.Int64)   ,    Nullable]
		public long? CurrentId { get; set; } // bigint(20)
	}

	/// <summary>
	/// 省市地区表
	/// </summary>
	[Table("district")]
	public partial class District : LinqToDBEntity
	{
		/// <summary>
		/// 名称
		/// </summary>
		[Column("Name",                DataType=DataType.VarChar,  Length=90),    Nullable]
		public string Name { get; set; } // varchar(90)

		/// <summary>
		/// 父级
		/// </summary>
		[Column("Parentid",            DataType=DataType.Int64)   ,    Nullable]
		public long? Parentid { get; set; } // bigint(20)

		/// <summary>
		/// 补充说明
		/// </summary>
		[Column("Extra",               DataType=DataType.VarChar,  Length=135),    Nullable]
		public string Extra { get; set; } // varchar(135)

		/// <summary>
		/// 后缀
		/// </summary>
		[Column("Suffix",              DataType=DataType.VarChar,  Length=45),    Nullable]
		public string Suffix { get; set; } // varchar(45)

		/// <summary>
		/// 区域码
		/// </summary>
		[Column("AreaId",              DataType=DataType.VarChar,  Length=30),    Nullable]
		public string AreaId { get; set; } // varchar(30)

		/// <summary>
		/// 排序
		/// </summary>
		[Column("OrderRule",           DataType=DataType.Int32)   ,    Nullable]
		public int? OrderRule { get; set; } // int(11)

		/// <summary>
		/// 是否有效
		/// </summary>
		[Column("IsActive",            DataType=DataType.Boolean) , NotNull]
		public bool IsActive { get; set; } // tinyint(1)

		/// <summary>
		/// 是否是热点
		/// </summary>
		[Column("IsHot",               DataType=DataType.Boolean) , NotNull]
		public bool IsHot { get; set; } // tinyint(1)

		/// <summary>
		/// 类型 0境内 1境外 2国外
		/// </summary>
		[Column("Type",                DataType=DataType.Int32)   , NotNull]
		public int Type { get; set; } // int(11)
	}

	/// <summary>
	/// 地区行李尺寸价格配置表
	/// </summary>
	[Table("district_goods")]
	public partial class DistrictGoods : LinqToDBEntity
	{
		/// <summary>
		/// 1到火车站 2到机场 3从火车站 4从机场
		/// </summary>
		[Column("Type",                DataType=DataType.Int32)   ,    Nullable]
		public int? Type { get; set; } // int(11)

		/// <summary>
		/// 区域Tid
		/// </summary>
		[Column("DistrictTid",         DataType=DataType.Int64)   ,    Nullable]
		public long? DistrictTid { get; set; } // bigint(20)

		/// <summary>
		/// 是否有效
		/// </summary>
		[Column("IsActive",            DataType=DataType.Boolean) , NotNull]
		public bool IsActive { get; set; } // tinyint(1)

		/// <summary>
		/// 描述
		/// </summary>
		[Column("Title",               DataType=DataType.VarChar,  Length=150),    Nullable]
		public string Title { get; set; } // varchar(150)

		/// <summary>
		/// 图片
		/// </summary>
		[Column("Photo",               DataType=DataType.VarChar,  Length=200),    Nullable]
		public string Photo { get; set; } // varchar(200)

		/// <summary>
		/// 产品特点
		/// </summary>
		[Column("Remark",              DataType=DataType.VarChar,  Length=10280),    Nullable]
		public string Remark { get; set; } // varchar(10280)
	}

	/// <summary>
	/// 地区行李尺寸价格配置中间表
	/// </summary>
	[Table("district_goods_package_type")]
	public partial class DistrictGoodsPackageType : LinqToDBEntity
	{
		/// <summary>
		/// 地区商品Tid
		/// </summary>
		[Column("DistrictGoodsTid",    DataType=DataType.Int64)   ,    Nullable]
		public long? DistrictGoodsTid { get; set; } // bigint(20)

		/// <summary>
		/// 行李配置Tid
		/// </summary>
		[Column("PackageTypeTid",      DataType=DataType.Int64)   ,    Nullable]
		public long? PackageTypeTid { get; set; } // bigint(20)

		/// <summary>
		/// 是否有效
		/// </summary>
		[Column("IsActive",            DataType=DataType.Boolean) , NotNull]
		public bool IsActive { get; set; } // tinyint(1)
	}

	/// <summary>
	/// 城市服务类型对应表
	/// </summary>
	[Table("district_service")]
	public partial class DistrictService : LinqToDBEntity
	{
		/// <summary>
		/// 区域Tid
		/// </summary>
		[Column("DistrictTid",         DataType=DataType.Int64)   ,    Nullable]
		public long? DistrictTid { get; set; } // bigint(20)

		/// <summary>
		/// 服务名称
		/// </summary>
		[Column("ServiceName",         DataType=DataType.VarChar,  Length=100),    Nullable]
		public string ServiceName { get; set; } // varchar(100)

		/// <summary>
		/// 服务类型 1到火车站 2到机场 3从火车站 4从机场
		/// </summary>
		[Column("ServiceType",         DataType=DataType.Int32)   ,    Nullable]
		public int? ServiceType { get; set; } // int(11)

		/// <summary>
		/// 是否有效
		/// </summary>
		[Column("IsActive",            DataType=DataType.Boolean) , NotNull]
		public bool IsActive { get; set; } // tinyint(1)
	}

	/// <summary>
	/// 保价配置表
	/// </summary>
	[Table("insurance_type")]
	public partial class InsuranceType : LinqToDBEntity
	{
		/// <summary>
		/// 区域Tid
		/// </summary>
		[Column("DistrictTid",         DataType=DataType.Int64)   ,    Nullable]
		public long? DistrictTid { get; set; } // bigint(20)

		/// <summary>
		/// X(元/件)
		/// </summary>
		[Column("Fee",                 DataType=DataType.Decimal,  Precision=10, Scale=0),    Nullable]
		public decimal? Fee { get; set; } // decimal(10,0)

		/// <summary>
		/// 保价信息
		/// </summary>
		[Column("Description",         DataType=DataType.VarChar,  Length=100),    Nullable]
		public string Description { get; set; } // varchar(100)

		/// <summary>
		/// 是否有效
		/// </summary>
		[Column("IsActive",            DataType=DataType.Boolean) , NotNull]
		public bool IsActive { get; set; } // tinyint(1)
	}

	/// <summary>
	/// 埋点数据
	/// </summary>
	[Table("one_ubt")]
	public partial class OneUbt : LinqToDBEntity
	{
		/// <summary>
		/// 按钮ID
		/// </summary>
		[Column("ControlID",           DataType=DataType.VarChar,  Length=45),    Nullable]
		public string ControlID { get; set; } // varchar(45)

		/// <summary>
		/// 按钮名称
		/// </summary>
		[Column("ControlName",         DataType=DataType.VarChar,  Length=45),    Nullable]
		public string ControlName { get; set; } // varchar(45)

		/// <summary>
		/// 按钮描述
		/// </summary>
		[Column("ControlDescription",  DataType=DataType.VarChar,  Length=100),    Nullable]
		public string ControlDescription { get; set; } // varchar(100)

		/// <summary>
		/// 用户ID
		/// </summary>
		[Column("UID",                 DataType=DataType.VarChar,  Length=45),    Nullable]
		public string UID { get; set; } // varchar(45)

		/// <summary>
		/// 用户设备ID
		/// </summary>
		[Column("CID",                 DataType=DataType.VarChar,  Length=45),    Nullable]
		public string CID { get; set; } // varchar(45)

		/// <summary>
		/// 类别（首页大分类）
		/// </summary>
		[Column("Category",            DataType=DataType.Int32)   ,    Nullable]
		public int? Category { get; set; } // int(11)

		/// <summary>
		/// 分类名称（首页大分类名称）
		/// </summary>
		[Column("CategoryName",        DataType=DataType.VarChar,  Length=45),    Nullable]
		public string CategoryName { get; set; } // varchar(45)

		/// <summary>
		/// 产品ID
		/// </summary>
		[Column("ProductID",           DataType=DataType.VarChar,  Length=45),    Nullable]
		public string ProductID { get; set; } // varchar(45)

		/// <summary>
		/// 产品名称
		/// </summary>
		[Column("ProductName",         DataType=DataType.VarChar,  Length=45),    Nullable]
		public string ProductName { get; set; } // varchar(45)

		/// <summary>
		/// 产品金额
		/// </summary>
		[Column("Price",               DataType=DataType.Decimal,  Precision=18, Scale=2), NotNull]
		public decimal Price { get; set; } // decimal(18,2)

		/// <summary>
		/// 产品链接
		/// </summary>
		[Column("Url",                 DataType=DataType.VarChar,  Length=2000),    Nullable]
		public string Url { get; set; } // varchar(2000)

		/// <summary>
		/// 其他类型
		/// </summary>
		[Column("ObjectType",          DataType=DataType.Int32)   ,    Nullable]
		public int? ObjectType { get; set; } // int(11)

		/// <summary>
		/// 其他类型ID
		/// </summary>
		[Column("ObjectID",            DataType=DataType.VarChar,  Length=45),    Nullable]
		public string ObjectID { get; set; } // varchar(45)

		/// <summary>
		/// 其他项目名称
		/// </summary>
		[Column("ObejctName",          DataType=DataType.VarChar,  Length=45),    Nullable]
		public string ObejctName { get; set; } // varchar(45)

		/// <summary>
		/// 额外参数
		/// </summary>
		[Column("Exinfo",              DataType=DataType.VarChar,  Length=1000),    Nullable]
		public string Exinfo { get; set; } // varchar(1000)

		/// <summary>
		/// 设备本地时间
		/// </summary>
		[Column("LocalTime",           DataType=DataType.DateTime),    Nullable]
		public DateTime? LocalTime { get; set; } // datetime

		/// <summary>
		/// 服务器时间
		/// </summary>
		[Column("ServerTime",          DataType=DataType.DateTime),    Nullable]
		public DateTime? ServerTime { get; set; } // datetime

		/// <summary>
		/// 操作类型
		/// </summary>
		[Column("ActionType",          DataType=DataType.Int32)   ,    Nullable]
		public int? ActionType { get; set; } // int(11)

		/// <summary>
		/// 操作名称
		/// </summary>
		[Column("ActionName",          DataType=DataType.VarChar,  Length=45),    Nullable]
		public string ActionName { get; set; } // varchar(45)

		/// <summary>
		/// 客户会话ID
		/// </summary>
		[Column("SessionID",           DataType=DataType.VarChar,  Length=45),    Nullable]
		public string SessionID { get; set; } // varchar(45)

		/// <summary>
		/// 客户端IP
		/// </summary>
		[Column("ClientIP",            DataType=DataType.VarChar,  Length=20),    Nullable]
		public string ClientIP { get; set; } // varchar(20)

		/// <summary>
		/// 版本号
		/// </summary>
		[Column("Version",             DataType=DataType.VarChar,  Length=20),    Nullable]
		public string Version { get; set; } // varchar(20)
	}

	/// <summary>
	/// 订单表
	/// </summary>
	[Table("orders")]
	public partial class Order : LinqToDBEntity
	{
		/// <summary>
		/// 供应商Tid
		/// </summary>
		[Column("ProviderTid",         DataType=DataType.Int64)   ,    Nullable]
		public long? ProviderTid { get; set; } // bigint(20)

		/// <summary>
		/// 用户Uid
		/// </summary>
		[Column("Uid",                 DataType=DataType.VarChar,  Length=36),    Nullable]
		public string Uid { get; set; } // varchar(36)

		/// <summary>
		/// 用户名称
		/// </summary>
		[Column("UserName",            DataType=DataType.VarChar,  Length=50),    Nullable]
		public string UserName { get; set; } // varchar(50)

		/// <summary>
		/// 用户手机号
		/// </summary>
		[Column("UserPhone",           DataType=DataType.Char,     Length=24),    Nullable]
		public string UserPhone { get; set; } // char(24)

		/// <summary>
		/// 寄件区域Tid
		/// </summary>
		[Column("LocationTid",         DataType=DataType.Int64)   ,    Nullable]
		public long? LocationTid { get; set; } // bigint(20)

		/// <summary>
		/// 寄件详细地址
		/// </summary>
		[Column("LocationDetail",      DataType=DataType.VarChar,  Length=250),    Nullable]
		public string LocationDetail { get; set; } // varchar(250)

		/// <summary>
		/// 寄件时间
		/// </summary>
		[Column("GetTime",             DataType=DataType.DateTime),    Nullable]
		public DateTime? GetTime { get; set; } // datetime

		/// <summary>
		/// 寄件交接方式
		/// </summary>
		[Column("GetTransferTid",      DataType=DataType.VarChar,  Length=250),    Nullable]
		public string GetTransferTid { get; set; } // varchar(250)

		/// <summary>
		/// 寄件类型( 0其他 1火车 2机场)
		/// </summary>
		[Column("GetType",             DataType=DataType.Int32)   ,    Nullable]
		public int? GetType { get; set; } // int(11)

		/// <summary>
		/// 收件区域Tid
		/// </summary>
		[Column("DestinationTid",      DataType=DataType.Int64)   ,    Nullable]
		public long? DestinationTid { get; set; } // bigint(20)

		/// <summary>
		/// 收件详细地址
		/// </summary>
		[Column("DestinationDetail",   DataType=DataType.VarChar,  Length=250),    Nullable]
		public string DestinationDetail { get; set; } // varchar(250)

		/// <summary>
		/// 收件时间
		/// </summary>
		[Column("TakeTime",            DataType=DataType.DateTime),    Nullable]
		public DateTime? TakeTime { get; set; } // datetime

		/// <summary>
		/// 收件交接方式
		/// </summary>
		[Column("TakeTransferTid",     DataType=DataType.VarChar,  Length=250),    Nullable]
		public string TakeTransferTid { get; set; } // varchar(250)

		/// <summary>
		/// 收件类型( 0其他 1火车 2机场)
		/// </summary>
		[Column("TakeType",            DataType=DataType.Int32)   ,    Nullable]
		public int? TakeType { get; set; } // int(11)

		/// <summary>
		/// 留言
		/// </summary>
		[Column("Remark",              DataType=DataType.VarChar,  Length=500),    Nullable]
		public string Remark { get; set; } // varchar(500)

		/// <summary>
		/// 产品Tid
		/// </summary>
		[Column("ProductTid",          DataType=DataType.Int64)   ,    Nullable]
		public long? ProductTid { get; set; } // bigint(20)

		/// <summary>
		/// 订单号
		/// </summary>
		[Column("OrderId",             DataType=DataType.Int64)   ,    Nullable]
		public long? OrderId { get; set; } // bigint(20)

		/// <summary>
		/// 订单价格
		/// </summary>
		[Column("OrderPrice",          DataType=DataType.Decimal,  Precision=10, Scale=3),    Nullable]
		public decimal? OrderPrice { get; set; } // decimal(10,3)

		/// <summary>
		/// 订单状态：1：待付款、2：待提取；3、配送中、4：已送达、5：已成交、9：已取消
		/// </summary>
		[Column("OrderState",          DataType=DataType.Int32)   ,    Nullable]
		public int? OrderState { get; set; } // int(11)

		/// <summary>
		/// 订单创建时间
		/// </summary>
		[Column("CreateTime",          DataType=DataType.DateTime),    Nullable]
		public DateTime? CreateTime { get; set; } // datetime

		/// <summary>
		/// 是否跨省
		/// </summary>
		[Column("isOutside",           DataType=DataType.Boolean) , NotNull]
		public bool IsOutside { get; set; } // tinyint(1)

		/// <summary>
		/// 保价金额
		/// </summary>
		[Column("InsuranceFee",        DataType=DataType.Decimal,  Precision=10, Scale=3),    Nullable]
		public decimal? InsuranceFee { get; set; } // decimal(10,3)

		/// <summary>
		/// 保价Tid
		/// </summary>
		[Column("InsuranceTid",        DataType=DataType.Int64)   ,    Nullable]
		public long? InsuranceTid { get; set; } // bigint(20)

		/// <summary>
		/// 支付状态：1:待付款、2：已付款、3：待退款；4：待补款；5：支付失败；6：已退款；7：支付处理
		/// </summary>
		[Column("PayState",            DataType=DataType.Int32)   ,    Nullable]
		public int? PayState { get; set; } // int(11)

		/// <summary>
		/// app版本号
		/// </summary>
		[Column("AppVersion",          DataType=DataType.VarChar,  Length=50),    Nullable]
		public string AppVersion { get; set; } // varchar(50)

		/// <summary>
		/// 渠道
		/// </summary>
		[Column("Channel",             DataType=DataType.Int32)   ,    Nullable]
		public int? Channel { get; set; } // int(11)

		/// <summary>
		/// 最迟支付时间
		/// </summary>
		[Column("LastPayTime",         DataType=DataType.DateTime),    Nullable]
		public DateTime? LastPayTime { get; set; } // datetime

		/// <summary>
		/// 终端(0:PC,1:APP，2:微信,3:H5)
		/// </summary>
		[Column("Terminal",            DataType=DataType.Int32)   ,    Nullable]
		public int? Terminal { get; set; } // int(11)

		/// <summary>
		/// 取货码
		/// </summary>
		[Column("TakeCode",            DataType=DataType.Int64)   ,    Nullable]
		public long? TakeCode { get; set; } // bigint(20)

		/// <summary>
		/// 用户是否删除
		/// </summary>
		[Column("IsActive",            DataType=DataType.Boolean) , NotNull]
		public bool IsActive { get; set; } // tinyint(1)

		/// <summary>
		/// 用户邮箱
		/// </summary>
		[Column("Email",               DataType=DataType.VarChar,  Length=50),    Nullable]
		public string Email { get; set; } // varchar(50)

		/// <summary>
		/// 用户身份证号或者护照号
		/// </summary>
		[Column("CardNo",              DataType=DataType.VarChar,  Length=50),    Nullable]
		public string CardNo { get; set; } // varchar(50)

		/// <summary>
		/// 服务类型 1到火车站 2到机场 3从火车站 4从机场
		/// </summary>
		[Column("ServiceType",         DataType=DataType.Int32)   ,    Nullable]
		public int? ServiceType { get; set; } // int(11)
	}

	/// <summary>
	/// 这个是订单号分配过来的订单池数据
	/// </summary>
	[Table("order_ids")]
	public partial class OrderIds : LinqToDBEntity
	{
		/// <summary>
		/// 唯一订单号
		/// </summary>
		[Column("OrderId",             DataType=DataType.Int64)   ,    Nullable]
		public long? OrderId { get; set; } // bigint(20)
	}

	/// <summary>
	/// 订单额外信息表
	/// </summary>
	[Table("order_message")]
	public partial class OrderMessage : LinqToDBEntity
	{
		/// <summary>
		/// 订单号
		/// </summary>
		[Column("OrderId",             DataType=DataType.Int64)   ,    Nullable]
		public long? OrderId { get; set; } // bigint(20)

		/// <summary>
		/// 留言备注等提醒信息
		/// </summary>
		[Column("Message",             DataType=DataType.VarChar,  Length=500),    Nullable]
		public string Message { get; set; } // varchar(500)

		/// <summary>
		/// 0火车支付平台 1系统 2供应商系统 3订单系统
		/// </summary>
		[Column("UserType",            DataType=DataType.Int32)   ,    Nullable]
		public int? UserType { get; set; } // int(11)

		/// <summary>
		/// 对应订单系统或者供应商系统用户的Tid
		/// </summary>
		[Column("UserTid",             DataType=DataType.Int64)   ,    Nullable]
		public long? UserTid { get; set; } // bigint(20)
	}

	/// <summary>
	/// 订单下的行李尺寸价格明细表
	/// </summary>
	[Table("order_package_detail")]
	public partial class OrderPackageDetail : LinqToDBEntity
	{
		/// <summary>
		/// 唯一订单号
		/// </summary>
		[Column("OrderId",               DataType=DataType.Int64)   ,    Nullable]
		public long? OrderId { get; set; } // bigint(20)

		/// <summary>
		/// 产品对应行李尺寸价格配置
		/// </summary>
		[Column("ProductPackageTypeTid", DataType=DataType.Int64)   ,    Nullable]
		public long? ProductPackageTypeTid { get; set; } // bigint(20)

		/// <summary>
		/// 数量
		/// </summary>
		[Column("Amount",                DataType=DataType.Int32)   ,    Nullable]
		public int? Amount { get; set; } // int(11)

		/// <summary>
		/// 总金额
		/// </summary>
		[Column("TotalPackagePrice",     DataType=DataType.Decimal,  Precision=10, Scale=3),    Nullable]
		public decimal? TotalPackagePrice { get; set; } // decimal(10,3)

		/// <summary>
		/// 行李规则尺寸名称等信息
		/// </summary>
		[Column("PackageName",           DataType=DataType.VarChar,  Length=50),    Nullable]
		public string PackageName { get; set; } // varchar(50)

		/// <summary>
		/// 价格
		/// </summary>
		[Column("Price",                 DataType=DataType.Decimal,  Precision=10, Scale=0),    Nullable]
		public decimal? Price { get; set; } // decimal(10,0)
	}

	/// <summary>
	/// 订单支付信息表
	/// </summary>
	[Table("order_payment")]
	public partial class OrderPayment : LinqToDBEntity
	{
		/// <summary>
		/// 订单号
		/// </summary>
		[Column("OrderId",             DataType=DataType.Int64)   ,    Nullable]
		public long? OrderId { get; set; } // bigint(20)

		/// <summary>
		/// 应收金额
		/// </summary>
		[Column("OrderPrice",          DataType=DataType.Decimal,  Precision=10, Scale=3),    Nullable]
		public decimal? OrderPrice { get; set; } // decimal(10,3)

		/// <summary>
		/// 实收金额
		/// </summary>
		[Column("UserPayFee",          DataType=DataType.Decimal,  Precision=10, Scale=3),    Nullable]
		public decimal? UserPayFee { get; set; } // decimal(10,3)

		/// <summary>
		/// 交易流水号
		/// </summary>
		[Column("TradeNumber",         DataType=DataType.VarChar,  Length=32),    Nullable]
		public string TradeNumber { get; set; } // varchar(32)

		/// <summary>
		/// 支付发生的时间
		/// </summary>
		[Column("PayTime",             DataType=DataType.DateTime),    Nullable]
		public DateTime? PayTime { get; set; } // datetime

		/// <summary>
		/// 支付的其它信息
		/// </summary>
		[Column("ExtInfo",             DataType=DataType.VarChar,  Length=10240),    Nullable]
		public string ExtInfo { get; set; } // varchar(10240)

		/// <summary>
		/// 请求号
		/// </summary>
		[Column("RequestId",           DataType=DataType.VarChar,  Length=32),    Nullable]
		public string RequestId { get; set; } // varchar(32)
	}

	/// <summary>
	/// 订单规则
	/// </summary>
	[Table("order_rule")]
	public partial class OrderRule : LinqToDBEntity
	{
		/// <summary>
		/// 取件开始时间段 格式为 HH:mm
		/// </summary>
		[Column("GetRangeStart",       DataType=DataType.VarChar,  Length=50),    Nullable]
		public string GetRangeStart { get; set; } // varchar(50)

		/// <summary>
		/// 取件结束时间段 格式为 HH:mm
		/// </summary>
		[Column("GetRangeEnd",         DataType=DataType.VarChar,  Length=50),    Nullable]
		public string GetRangeEnd { get; set; } // varchar(50)

		/// <summary>
		/// 提取开始时间段 格式为 HH:mm
		/// </summary>
		[Column("TakeRangeStart",      DataType=DataType.VarChar,  Length=50),    Nullable]
		public string TakeRangeStart { get; set; } // varchar(50)

		/// <summary>
		/// 提取结束时间段 格式为 HH:mm
		/// </summary>
		[Column("TakeRangeEnd",        DataType=DataType.VarChar,  Length=50),    Nullable]
		public string TakeRangeEnd { get; set; } // varchar(50)

		/// <summary>
		/// 区域主键
		/// </summary>
		[Column("DistrictTid",         DataType=DataType.Int64)   ,    Nullable]
		public long? DistrictTid { get; set; } // bigint(20)

		/// <summary>
		/// 产品主键
		/// </summary>
		[Column("ProductTid",          DataType=DataType.Int64)   ,    Nullable]
		public long? ProductTid { get; set; } // bigint(20)

		/// <summary>
		/// 提取必须是第二天
		/// </summary>
		[Column("TakeMustNextDay",     DataType=DataType.Boolean) , NotNull]
		public bool TakeMustNextDay { get; set; } // tinyint(1)

		/// <summary>
		/// 是否有效
		/// </summary>
		[Column("IsActive",            DataType=DataType.Boolean) , NotNull]
		public bool IsActive { get; set; } // tinyint(1)
	}

	/// <summary>
	/// 订单状态更改记录表
	/// </summary>
	[Table("order_state_change")]
	public partial class OrderStateChange : LinqToDBEntity
	{
		/// <summary>
		/// 描述信息
		/// </summary>
		[Column("Info",                DataType=DataType.VarChar,  Length=2000),    Nullable]
		public string Info { get; set; } // varchar(2000)

		/// <summary>
		/// 订单号
		/// </summary>
		[Column("OrderId",             DataType=DataType.Int64)   ,    Nullable]
		public long? OrderId { get; set; } // bigint(20)

		/// <summary>
		/// 改变前状态
		/// </summary>
		[Column("Befor",               DataType=DataType.Int32)   ,    Nullable]
		public int? Befor { get; set; } // int(11)

		/// <summary>
		/// 改变后状态
		/// </summary>
		[Column("After",               DataType=DataType.Int32)   ,    Nullable]
		public int? After { get; set; } // int(11)

		/// <summary>
		/// 0火车支付平台 1系统 2供应商系统 3订单系统
		/// </summary>
		[Column("UserType",            DataType=DataType.Int32)   ,    Nullable]
		public int? UserType { get; set; } // int(11)

		/// <summary>
		/// 对应订单系统或者供应商系统用户的Tid
		/// </summary>
		[Column("UserTid",             DataType=DataType.Int64)   ,    Nullable]
		public long? UserTid { get; set; } // bigint(20)
	}

	/// <summary>
	/// 订单系统用户表
	/// </summary>
	[Table("order_user")]
	public partial class OrderUser : LinqToDBEntity
	{
		/// <summary>
		/// 昵称
		/// </summary>
		[Column("NickName",            DataType=DataType.VarChar,  Length=30),    Nullable]
		public string NickName { get; set; } // varchar(30)

		/// <summary>
		/// 是否可用
		/// </summary>
		[Column("IsActive",            DataType=DataType.Boolean) , NotNull]
		public bool IsActive { get; set; } // tinyint(1)

		/// <summary>
		/// 登陆账号
		/// </summary>
		[Column("UserName",            DataType=DataType.VarChar,  Length=20),    Nullable]
		public string UserName { get; set; } // varchar(20)

		/// <summary>
		/// 登陆密码
		/// </summary>
		[Column("Pwd",                 DataType=DataType.VarChar,  Length=20),    Nullable]
		public string Pwd { get; set; } // varchar(20)

		/// <summary>
		/// 区域Tid
		/// </summary>
		[Column("DistrictTid",         DataType=DataType.Int64)   ,    Nullable]
		public long? DistrictTid { get; set; } // bigint(20)
	}

	/// <summary>
	/// 行李尺寸价格明细配置
	/// </summary>
	[Table("package_type")]
	public partial class PackageType : LinqToDBEntity
	{
		/// <summary>
		/// 行李规则尺寸名称等信息
		/// </summary>
		[Column("PackageName",         DataType=DataType.VarChar,  Length=50),    Nullable]
		public string PackageName { get; set; } // varchar(50)

		/// <summary>
		/// 价格
		/// </summary>
		[Column("Price",               DataType=DataType.Decimal,  Precision=10, Scale=0),    Nullable]
		public decimal? Price { get; set; } // decimal(10,0)

		/// <summary>
		/// 市场数据
		/// </summary>
		[Column("MarketPrice",         DataType=DataType.Decimal,  Precision=10, Scale=0),    Nullable]
		public decimal? MarketPrice { get; set; } // decimal(10,0)

		/// <summary>
		/// 排序
		/// </summary>
		[Column("OrderRule",           DataType=DataType.Int32)   , NotNull]
		public int OrderRule { get; set; } // int(11)

		/// <summary>
		/// 备注说明
		/// </summary>
		[Column("Description",         DataType=DataType.VarChar,  Length=500),    Nullable]
		public string Description { get; set; } // varchar(500)
	}

	/// <summary>
	/// 提供的服务产品表
	/// </summary>
	[Table("product")]
	public partial class Product : LinqToDBEntity
	{
		/// <summary>
		/// 1到火车站 2到机场 3从火车站 4从机场
		/// </summary>
		[Column("Type",                DataType=DataType.Int32)   ,    Nullable]
		public int? Type { get; set; } // int(11)

		/// <summary>
		/// 地区Tid
		/// </summary>
		[Column("DistrictTid",         DataType=DataType.Int64)   ,    Nullable]
		public long? DistrictTid { get; set; } // bigint(20)

		/// <summary>
		/// 名称(例如T1航站楼)
		/// </summary>
		[Column("ProductName",         DataType=DataType.VarChar,  Length=100),    Nullable]
		public string ProductName { get; set; } // varchar(100)

		/// <summary>
		/// 其他信息
		/// </summary>
		[Column("Description",         DataType=DataType.VarChar,  Length=1000),    Nullable]
		public string Description { get; set; } // varchar(1000)

		/// <summary>
		/// 是否有效
		/// </summary>
		[Column("IsActive",            DataType=DataType.Boolean) , NotNull]
		public bool IsActive { get; set; } // tinyint(1)

		/// <summary>
		/// 当天最迟下单时间
		/// </summary>
		[Column("LastAcceptTime",      DataType=DataType.DateTime),    Nullable]
		public DateTime? LastAcceptTime { get; set; } // datetime

		/// <summary>
		/// 取件交接方式JSON数组
		/// </summary>
		[Column("GetTransfer",         DataType=DataType.VarChar,  Length=5000),    Nullable]
		public string GetTransfer { get; set; } // varchar(5000)

		/// <summary>
		/// 收件交接方式JSON数组
		/// </summary>
		[Column("TakeTransfer",        DataType=DataType.VarChar,  Length=5000),    Nullable]
		public string TakeTransfer { get; set; } // varchar(5000)
	}

	/// <summary>
	/// 产品对应行李尺寸价格配置表
	/// </summary>
	[Table("product_package_type")]
	public partial class ProductPackageType : LinqToDBEntity
	{
		/// <summary>
		/// 产品Tid
		/// </summary>
		[Column("ProductTid",          DataType=DataType.Int64)   ,    Nullable]
		public long? ProductTid { get; set; } // bigint(20)

		/// <summary>
		/// 行李配置Tid
		/// </summary>
		[Column("PackageTypeTid",      DataType=DataType.Int64)   ,    Nullable]
		public long? PackageTypeTid { get; set; } // bigint(20)

		/// <summary>
		/// 是否有效
		/// </summary>
		[Column("IsActive",            DataType=DataType.Boolean) , NotNull]
		public bool IsActive { get; set; } // tinyint(1)
	}

	/// <summary>
	/// 供应商表
	/// </summary>
	[Table("provider")]
	public partial class Provider : LinqToDBEntity
	{
		/// <summary>
		/// 供应商名称
		/// </summary>
		[Column("Name",                DataType=DataType.VarChar,  Length=200),    Nullable]
		public string Name { get; set; } // varchar(200)

		/// <summary>
		/// 详细地址
		/// </summary>
		[Column("Address",             DataType=DataType.VarChar,  Length=200),    Nullable]
		public string Address { get; set; } // varchar(200)

		/// <summary>
		/// 联系人姓名
		/// </summary>
		[Column("ContactName",         DataType=DataType.VarChar,  Length=20),    Nullable]
		public string ContactName { get; set; } // varchar(20)

		/// <summary>
		/// 联系方式
		/// </summary>
		[Column("ContactPhone",        DataType=DataType.VarChar,  Length=20),    Nullable]
		public string ContactPhone { get; set; } // varchar(20)

		/// <summary>
		/// 说明
		/// </summary>
		[Column("Description",         DataType=DataType.VarChar,  Length=500),    Nullable]
		public string Description { get; set; } // varchar(500)

		/// <summary>
		/// 是否可用
		/// </summary>
		[Column("IsActive",            DataType=DataType.Boolean) , NotNull]
		public bool IsActive { get; set; } // tinyint(1)

		/// <summary>
		/// 访问的Json Web Token
		/// </summary>
		[Column("AccessJWT",           DataType=DataType.VarChar,  Length=10480),    Nullable]
		public string AccessJWT { get; set; } // varchar(10480)

		/// <summary>
		/// 邀请码
		/// </summary>
		[Column("Code",                DataType=DataType.Int64)   ,    Nullable]
		public long? Code { get; set; } // bigint(20)
	}

	/// <summary>
	/// 供应商订单表
	/// </summary>
	[Table("provider_order")]
	public partial class ProviderOrder : LinqToDBEntity
	{
		/// <summary>
		/// 供应商Tid
		/// </summary>
		[Column("ProviderTid",         DataType=DataType.Int64)   ,    Nullable]
		public long? ProviderTid { get; set; } // bigint(20)

		/// <summary>
		/// 用户Uid
		/// </summary>
		[Column("Uid",                 DataType=DataType.VarChar,  Length=36),    Nullable]
		public string Uid { get; set; } // varchar(36)

		/// <summary>
		/// 用户名称
		/// </summary>
		[Column("UserName",            DataType=DataType.VarChar,  Length=50),    Nullable]
		public string UserName { get; set; } // varchar(50)

		/// <summary>
		/// 用户手机号
		/// </summary>
		[Column("UserPhone",           DataType=DataType.Char,     Length=24),    Nullable]
		public string UserPhone { get; set; } // char(24)

		/// <summary>
		/// 取件区域Tid
		/// </summary>
		[Column("LocationTid",         DataType=DataType.Int64)   ,    Nullable]
		public long? LocationTid { get; set; } // bigint(20)

		/// <summary>
		/// 取件详细地址
		/// </summary>
		[Column("LocationDetail",      DataType=DataType.VarChar,  Length=250),    Nullable]
		public string LocationDetail { get; set; } // varchar(250)

		/// <summary>
		/// 取件时间
		/// </summary>
		[Column("GetTime",             DataType=DataType.DateTime),    Nullable]
		public DateTime? GetTime { get; set; } // datetime

		/// <summary>
		/// 取件交接方式
		/// </summary>
		[Column("GetTransferTid",      DataType=DataType.VarChar,  Length=250),    Nullable]
		public string GetTransferTid { get; set; } // varchar(250)

		/// <summary>
		/// 取件类型( 0其他 1火车 2机场)
		/// </summary>
		[Column("GetType",             DataType=DataType.Int32)   ,    Nullable]
		public int? GetType { get; set; } // int(11)

		/// <summary>
		/// 收件区域Tid
		/// </summary>
		[Column("DestinationTid",      DataType=DataType.Int64)   ,    Nullable]
		public long? DestinationTid { get; set; } // bigint(20)

		/// <summary>
		/// 收件详细地址
		/// </summary>
		[Column("DestinationDetail",   DataType=DataType.VarChar,  Length=250),    Nullable]
		public string DestinationDetail { get; set; } // varchar(250)

		/// <summary>
		/// 收件时间
		/// </summary>
		[Column("TakeTime",            DataType=DataType.DateTime),    Nullable]
		public DateTime? TakeTime { get; set; } // datetime

		/// <summary>
		/// 收件交接方式
		/// </summary>
		[Column("TakeTransferTid",     DataType=DataType.VarChar,  Length=250),    Nullable]
		public string TakeTransferTid { get; set; } // varchar(250)

		/// <summary>
		/// 收件类型( 0其他 1火车 2机场)
		/// </summary>
		[Column("TakeType",            DataType=DataType.Int32)   ,    Nullable]
		public int? TakeType { get; set; } // int(11)

		/// <summary>
		/// 留言
		/// </summary>
		[Column("Remark",              DataType=DataType.VarChar,  Length=500),    Nullable]
		public string Remark { get; set; } // varchar(500)

		/// <summary>
		/// 产品Tid
		/// </summary>
		[Column("ProductTid",          DataType=DataType.Int64)   ,    Nullable]
		public long? ProductTid { get; set; } // bigint(20)

		/// <summary>
		/// 订单号
		/// </summary>
		[Column("OrderId",             DataType=DataType.Int64)   ,    Nullable]
		public long? OrderId { get; set; } // bigint(20)

		/// <summary>
		/// 订单价格
		/// </summary>
		[Column("OrderPrice",          DataType=DataType.Decimal,  Precision=10, Scale=3),    Nullable]
		public decimal? OrderPrice { get; set; } // decimal(10,3)

		/// <summary>
		/// 订单状态：1：待付款、2：待提取；3、配送中、4：已送达、5：已成交、9：已取消
		/// </summary>
		[Column("OrderState",          DataType=DataType.Int32)   ,    Nullable]
		public int? OrderState { get; set; } // int(11)

		/// <summary>
		/// 订单创建时间
		/// </summary>
		[Column("CreateTime",          DataType=DataType.DateTime),    Nullable]
		public DateTime? CreateTime { get; set; } // datetime

		/// <summary>
		/// 保价金额
		/// </summary>
		[Column("InsuranceFee",        DataType=DataType.Decimal,  Precision=10, Scale=3),    Nullable]
		public decimal? InsuranceFee { get; set; } // decimal(10,3)

		/// <summary>
		/// 保价Tid
		/// </summary>
		[Column("InsuranceTid",        DataType=DataType.Int64)   ,    Nullable]
		public long? InsuranceTid { get; set; } // bigint(20)

		/// <summary>
		/// 是否跨省
		/// </summary>
		[Column("isOutside",           DataType=DataType.Boolean) , NotNull]
		public bool IsOutside { get; set; } // tinyint(1)

		/// <summary>
		/// 支付状态：1:待付款、2：已付款、3：待退款；4：待补款；5：支付失败；6：已退款；7：支付处理
		/// </summary>
		[Column("PayState",            DataType=DataType.Int32)   ,    Nullable]
		public int? PayState { get; set; } // int(11)

		/// <summary>
		/// app版本号
		/// </summary>
		[Column("AppVersion",          DataType=DataType.VarChar,  Length=50),    Nullable]
		public string AppVersion { get; set; } // varchar(50)

		/// <summary>
		/// 渠道
		/// </summary>
		[Column("Channel",             DataType=DataType.Int32)   ,    Nullable]
		public int? Channel { get; set; } // int(11)

		/// <summary>
		/// 最迟支付时间
		/// </summary>
		[Column("LastPayTime",         DataType=DataType.DateTime),    Nullable]
		public DateTime? LastPayTime { get; set; } // datetime

		/// <summary>
		/// 终端类型
		/// </summary>
		[Column("Terminal",            DataType=DataType.Int32)   ,    Nullable]
		public int? Terminal { get; set; } // int(11)

		/// <summary>
		/// 取货码
		/// </summary>
		[Column("TakeCode",            DataType=DataType.Int64)   ,    Nullable]
		public long? TakeCode { get; set; } // bigint(20)

		/// <summary>
		/// 用户是否删除
		/// </summary>
		[Column("IsActive",            DataType=DataType.Boolean) , NotNull]
		public bool IsActive { get; set; } // tinyint(1)

		/// <summary>
		/// 用户邮箱
		/// </summary>
		[Column("Email",               DataType=DataType.VarChar,  Length=50),    Nullable]
		public string Email { get; set; } // varchar(50)

		/// <summary>
		/// 用户身份证号或者护照号
		/// </summary>
		[Column("CardNo",              DataType=DataType.VarChar,  Length=50),    Nullable]
		public string CardNo { get; set; } // varchar(50)

		/// <summary>
		/// 服务类型 1到火车站 2到机场 3从火车站 4从机场
		/// </summary>
		[Column("ServiceType",         DataType=DataType.Int32)   ,    Nullable]
		public int? ServiceType { get; set; } // int(11)
	}

	/// <summary>
	/// 供应商提供的服务产品表
	/// </summary>
	[Table("provider_product")]
	public partial class ProviderProduct : LinqToDBEntity
	{
		/// <summary>
		/// 服务Tid
		/// </summary>
		[Column("ProductTid",          DataType=DataType.Int64)   ,    Nullable]
		public long? ProductTid { get; set; } // bigint(20)

		/// <summary>
		/// 供应商Tid
		/// </summary>
		[Column("ProviderTid",         DataType=DataType.Int64)   ,    Nullable]
		public long? ProviderTid { get; set; } // bigint(20)

		/// <summary>
		/// 是否有效
		/// </summary>
		[Column("IsActive",            DataType=DataType.Boolean) , NotNull]
		public bool IsActive { get; set; } // tinyint(1)

		/// <summary>
		/// 联系人名称
		/// </summary>
		[Column("ContactName",         DataType=DataType.VarChar,  Length=20),    Nullable]
		public string ContactName { get; set; } // varchar(20)

		/// <summary>
		/// 联系方式
		/// </summary>
		[Column("ContactPhone",        DataType=DataType.VarChar,  Length=20),    Nullable]
		public string ContactPhone { get; set; } // varchar(20)

		/// <summary>
		/// 当天最迟下单时间
		/// </summary>
		[Column("LastAcceptTime",      DataType=DataType.DateTime),    Nullable]
		public DateTime? LastAcceptTime { get; set; } // datetime
	}

	/// <summary>
	/// 供应商用户表
	/// </summary>
	[Table("provider_user")]
	public partial class ProviderUser : LinqToDBEntity
	{
		/// <summary>
		/// 昵称
		/// </summary>
		[Column("NickName",            DataType=DataType.VarChar,  Length=30),    Nullable]
		public string NickName { get; set; } // varchar(30)

		/// <summary>
		/// 是否可用
		/// </summary>
		[Column("IsActive",            DataType=DataType.Boolean) , NotNull]
		public bool IsActive { get; set; } // tinyint(1)

		/// <summary>
		/// 登陆账号
		/// </summary>
		[Column("UserName",            DataType=DataType.VarChar,  Length=20),    Nullable]
		public string UserName { get; set; } // varchar(20)

		/// <summary>
		/// 登陆密码
		/// </summary>
		[Column("Pwd",                 DataType=DataType.VarChar,  Length=20),    Nullable]
		public string Pwd { get; set; } // varchar(20)

		/// <summary>
		/// 供应商
		/// </summary>
		[Column("Providers",           DataType=DataType.VarChar,  Length=200),    Nullable]
		public string Providers { get; set; } // varchar(200)
	}

	[Table("system_menu")]
	public partial class SystemMenu : LinqToDBEntity
	{
		/// <summary>
		/// 是否可用
		/// </summary>
		[Column("IsActive",            DataType=DataType.Boolean) , NotNull]
		public bool IsActive { get; set; } // tinyint(1)

		/// <summary>
		/// 父节点Id
		/// </summary>
		[Column("ParentTid",           DataType=DataType.Int64)   , NotNull]
		public long ParentTid { get; set; } // bigint(20)

		/// <summary>
		/// 名称
		/// </summary>
		[Column("Name",                DataType=DataType.VarChar,  Length=50),    Nullable]
		public string Name { get; set; } // varchar(50)

		/// <summary>
		/// 展示的图标
		/// </summary>
		[Column("Ico",                 DataType=DataType.VarChar,  Length=100),    Nullable]
		public string Ico { get; set; } // varchar(100)

		/// <summary>
		/// 连接地址
		/// </summary>
		[Column("Url",                 DataType=DataType.VarChar,  Length=200),    Nullable]
		public string Url { get; set; } // varchar(200)

		/// <summary>
		/// 排序
		/// </summary>
		[Column("OrderRule",           DataType=DataType.Int32)   ,    Nullable]
		public int? OrderRule { get; set; } // int(11)

		/// <summary>
		/// 类型
		/// </summary>
		[Column("MenuType",            DataType=DataType.Int32)   ,    Nullable]
		public int? MenuType { get; set; } // int(11)

		/// <summary>
		/// 样式
		/// </summary>
		[Column("Class",               DataType=DataType.VarChar,  Length=100),    Nullable]
		public string Class { get; set; } // varchar(100)
	}

	[Table("system_role")]
	public partial class SystemRole : LinqToDBEntity
	{
		/// <summary>
		/// 角色名称
		/// </summary>
		[Column("RoleName",            DataType=DataType.VarChar,  Length=100),    Nullable]
		public string RoleName { get; set; } // varchar(100)

		/// <summary>
		/// 描述
		/// </summary>
		[Column("Description",         DataType=DataType.VarChar,  Length=200),    Nullable]
		public string Description { get; set; } // varchar(200)

		/// <summary>
		/// 是否可用
		/// </summary>
		[Column("IsActive",            DataType=DataType.Boolean) , NotNull]
		public bool IsActive { get; set; } // tinyint(1)

		/// <summary>
		/// 菜单权限
		/// </summary>
		[Column("MenuRights",          DataType=DataType.VarChar,  Length=50),    Nullable]
		public string MenuRights { get; set; } // varchar(50)
	}

	[Table("system_user")]
	public partial class SystemUser : LinqToDBEntity
	{
		/// <summary>
		/// 是否可用
		/// </summary>
		[Column("IsActive",            DataType=DataType.Boolean) , NotNull]
		public bool IsActive { get; set; } // tinyint(1)

		/// <summary>
		/// 登陆名
		/// </summary>
		[Column("Eid",                 DataType=DataType.VarChar,  Length=36),    Nullable]
		public string Eid { get; set; } // varchar(36)

		/// <summary>
		/// 用户名
		/// </summary>
		[Column("UserName",            DataType=DataType.VarChar,  Length=50),    Nullable]
		public string UserName { get; set; } // varchar(50)

		/// <summary>
		/// 密码
		/// </summary>
		[Column("Pwd",                 DataType=DataType.VarChar,  Length=20),    Nullable]
		public string Pwd { get; set; } // varchar(20)

		/// <summary>
		/// 手机号
		/// </summary>
		[Column("Phone",               DataType=DataType.VarChar,  Length=20),    Nullable]
		public string Phone { get; set; } // varchar(20)

		/// <summary>
		/// 登陆IP
		/// </summary>
		[Column("LoginIp",             DataType=DataType.VarChar,  Length=30),    Nullable]
		public string LoginIp { get; set; } // varchar(30)

		/// <summary>
		/// 菜单权限
		/// </summary>
		[Column("MenuRights",          DataType=DataType.VarChar,  Length=50),    Nullable]
		public string MenuRights { get; set; } // varchar(50)

		/// <summary>
		/// 角色Tid(一个人只有一个角色)
		/// </summary>
		[Column("RoleTid",             DataType=DataType.Int64)   ,    Nullable]
		public long? RoleTid { get; set; } // bigint(20)

		/// <summary>
		/// 最后登录系统时间
		/// </summary>
		[Column("LastLoginTime",       DataType=DataType.DateTime),    Nullable]
		public DateTime? LastLoginTime { get; set; } // datetime

		/// <summary>
		/// 登录的浏览器信息
		/// </summary>
		[Column("UserAgent",           DataType=DataType.VarChar,  Length=500),    Nullable]
		public string UserAgent { get; set; } // varchar(500)
	}

	/// <summary>
	/// 交接方式配置表
	/// </summary>
	[Table("transfer_type")]
	public partial class TransferType : LinqToDBEntity
	{
		/// <summary>
		/// 1收取行李 2提取行李
		/// </summary>
		[Column("Type",                DataType=DataType.Int32)   ,    Nullable]
		public int? Type { get; set; } // int(11)

		/// <summary>
		/// 交接方式
		/// </summary>
		[Column("Transfer",            DataType=DataType.VarChar,  Length=50),    Nullable]
		public string Transfer { get; set; } // varchar(50)

		/// <summary>
		/// 备注信息
		/// </summary>
		[Column("Description",         DataType=DataType.VarChar,  Length=200),    Nullable]
		public string Description { get; set; } // varchar(200)

		/// <summary>
		/// 产品Tid
		/// </summary>
		[Column("ProductTid",          DataType=DataType.Int64)   ,    Nullable]
		public long? ProductTid { get; set; } // bigint(20)

		/// <summary>
		/// 是否有效
		/// </summary>
		[Column("IsActive",            DataType=DataType.Boolean) , NotNull]
		public bool IsActive { get; set; } // tinyint(1)

		/// <summary>
		/// 区域主键
		/// </summary>
		[Column("DistinctTid",         DataType=DataType.Int64)   ,    Nullable]
		public long? DistinctTid { get; set; } // bigint(20)

		/// <summary>
		/// 取件交接方式JSON数组
		/// </summary>
		[Column("GetTransfer",         DataType=DataType.VarChar,  Length=5000),    Nullable]
		public string GetTransfer { get; set; } // varchar(5000)

		/// <summary>
		/// 收件交接方式JSON数组
		/// </summary>
		[Column("TakeTransfer",        DataType=DataType.VarChar,  Length=5000),    Nullable]
		public string TakeTransfer { get; set; } // varchar(5000)
	}
}
