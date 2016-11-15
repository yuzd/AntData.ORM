using System;
using System.Collections.Generic;
using System.Linq;

using AntData.ORM;
using AntData.ORM.Mapping;
using AntData.ORM.Mysql.Base;

namespace DbModels.Mysql
{
	/// <summary>
	/// Database       : hangfire
	/// Data Source    : 127.0.0.1
	/// Server Version : 5.6.26-log
	/// </summary>
	public partial class Entitys : IEntity
	{
		public IQueryable<Aggregatedcounter> Aggregatedcounters { get { return this.Get<Aggregatedcounter>(); } }
		public IQueryable<Counter>           Counters           { get { return this.Get<Counter>(); } }
		public IQueryable<Distributedlock>   Distributedlocks   { get { return this.Get<Distributedlock>(); } }
		public IQueryable<Hash>              Hashes             { get { return this.Get<Hash>(); } }
		public IQueryable<Job>               Jobs               { get { return this.Get<Job>(); } }
		public IQueryable<Jobparameter>      Jobparameters      { get { return this.Get<Jobparameter>(); } }
		public IQueryable<Jobqueue>          Jobqueues          { get { return this.Get<Jobqueue>(); } }
		public IQueryable<Jobstate>          Jobstates          { get { return this.Get<Jobstate>(); } }
		public IQueryable<List>              Lists              { get { return this.Get<List>(); } }
		public IQueryable<Server>            Servers            { get { return this.Get<Server>(); } }
		public IQueryable<Set>               Sets               { get { return this.Get<Set>(); } }
		public IQueryable<State>             States             { get { return this.Get<State>(); } }

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

	[Table("aggregatedcounter")]
	public partial class Aggregatedcounter : LinqToDBEntity
	{
		[Column("Id",       DataType=DataType.Int32)   , PrimaryKey, Identity]
		public int Id { get; set; } // int(11)

		[Column("Key",      DataType=DataType.VarChar,  Length=100), NotNull]
		public string Key { get; set; } // varchar(100)

		[Column("Value",    DataType=DataType.Int32)   , NotNull]
		public int Value { get; set; } // int(11)

		[Column("ExpireAt", DataType=DataType.DateTime),    Nullable]
		public DateTime? ExpireAt { get; set; } // datetime
	}

	[Table("counter")]
	public partial class Counter : LinqToDBEntity
	{
		[Column("Id",       DataType=DataType.Int32)   , PrimaryKey, Identity]
		public int Id { get; set; } // int(11)

		[Column("Key",      DataType=DataType.VarChar,  Length=100), NotNull]
		public string Key { get; set; } // varchar(100)

		[Column("Value",    DataType=DataType.Int32)   , NotNull]
		public int Value { get; set; } // int(11)

		[Column("ExpireAt", DataType=DataType.DateTime),    Nullable]
		public DateTime? ExpireAt { get; set; } // datetime
	}

	[Table("distributedlock")]
	public partial class Distributedlock : LinqToDBEntity
	{
		[Column("Resource",  DataType=DataType.VarChar,  Length=100), NotNull]
		public string Resource { get; set; } // varchar(100)

		[Column("CreatedAt", DataType=DataType.DateTime), NotNull]
		public DateTime CreatedAt { get; set; } // datetime(6)
	}

	[Table("hash")]
	public partial class Hash : LinqToDBEntity
	{
		[Column("Id",       DataType=DataType.Int32)   , PrimaryKey, Identity]
		public int Id { get; set; } // int(11)

		[Column("Key",      DataType=DataType.VarChar,  Length=100), NotNull]
		public string Key { get; set; } // varchar(100)

		[Column("Field",    DataType=DataType.VarChar,  Length=40), NotNull]
		public string Field { get; set; } // varchar(40)

		[Column("Value",    DataType=DataType.Text,     Length=4294967295),    Nullable]
		public string Value { get; set; } // longtext

		[Column("ExpireAt", DataType=DataType.DateTime),    Nullable]
		public DateTime? ExpireAt { get; set; } // datetime(6)
	}

	[Table("job")]
	public partial class Job : LinqToDBEntity
	{
		[Column("Id",             DataType=DataType.Int32)   , PrimaryKey, Identity]
		public int Id { get; set; } // int(11)

		[Column("StateId",        DataType=DataType.Int32)   ,    Nullable]
		public int? StateId { get; set; } // int(11)

		[Column("StateName",      DataType=DataType.VarChar,  Length=20),    Nullable]
		public string StateName { get; set; } // varchar(20)

		[Column("InvocationData", DataType=DataType.Text,     Length=4294967295), NotNull]
		public string InvocationData { get; set; } // longtext

		[Column("Arguments",      DataType=DataType.Text,     Length=4294967295), NotNull]
		public string Arguments { get; set; } // longtext

		[Column("CreatedAt",      DataType=DataType.DateTime), NotNull]
		public DateTime CreatedAt { get; set; } // datetime(6)

		[Column("ExpireAt",       DataType=DataType.DateTime),    Nullable]
		public DateTime? ExpireAt { get; set; } // datetime(6)

		#region Associations

		/// <summary>
		/// FK_JobParameter_Job_BackReference
		/// </summary>
		[Association(ThisKey="Id", OtherKey="JobId", CanBeNull=true, IsBackReference=true)]
		public IEnumerable<Jobparameter> JobParameterJobs { get; set; }

		/// <summary>
		/// FK_JobState_Job_BackReference
		/// </summary>
		[Association(ThisKey="Id", OtherKey="JobId", CanBeNull=true, IsBackReference=true)]
		public IEnumerable<Jobstate> JobStateJobs { get; set; }

		/// <summary>
		/// FK_HangFire_State_Job_BackReference
		/// </summary>
		[Association(ThisKey="Id", OtherKey="JobId", CanBeNull=true, IsBackReference=true)]
		public IEnumerable<State> HangFireStateJobs { get; set; }

		#endregion
	}

	[Table("jobparameter")]
	public partial class Jobparameter : LinqToDBEntity
	{
		[Column("Id",    DataType=DataType.Int32)  , PrimaryKey, Identity]
		public int Id { get; set; } // int(11)

		[Column("JobId", DataType=DataType.Int32)  , NotNull]
		public int JobId { get; set; } // int(11)

		[Column("Name",  DataType=DataType.VarChar, Length=40), NotNull]
		public string Name { get; set; } // varchar(40)

		[Column("Value", DataType=DataType.Text,    Length=4294967295),    Nullable]
		public string Value { get; set; } // longtext

		#region Associations

		/// <summary>
		/// FK_JobParameter_Job
		/// </summary>
		[Association(ThisKey="JobId", OtherKey="Id", CanBeNull=false, KeyName="FK_JobParameter_Job", BackReferenceName="JobParameterJobs")]
		public Job JobParameterJob { get; set; }

		#endregion
	}

	[Table("jobqueue")]
	public partial class Jobqueue : LinqToDBEntity
	{
		[Column("Id",         DataType=DataType.Int32)   , PrimaryKey, Identity]
		public int Id { get; set; } // int(11)

		[Column("JobId",      DataType=DataType.Int32)   , NotNull]
		public int JobId { get; set; } // int(11)

		[Column("Queue",      DataType=DataType.VarChar,  Length=50), NotNull]
		public string Queue { get; set; } // varchar(50)

		[Column("FetchedAt",  DataType=DataType.DateTime),    Nullable]
		public DateTime? FetchedAt { get; set; } // datetime(6)

		[Column("FetchToken", DataType=DataType.VarChar,  Length=36),    Nullable]
		public string FetchToken { get; set; } // varchar(36)
	}

	[Table("jobstate")]
	public partial class Jobstate : LinqToDBEntity
	{
		[Column("Id",        DataType=DataType.Int32)   , PrimaryKey, Identity]
		public int Id { get; set; } // int(11)

		[Column("JobId",     DataType=DataType.Int32)   , NotNull]
		public int JobId { get; set; } // int(11)

		[Column("Name",      DataType=DataType.VarChar,  Length=20), NotNull]
		public string Name { get; set; } // varchar(20)

		[Column("Reason",    DataType=DataType.VarChar,  Length=100),    Nullable]
		public string Reason { get; set; } // varchar(100)

		[Column("CreatedAt", DataType=DataType.DateTime), NotNull]
		public DateTime CreatedAt { get; set; } // datetime(6)

		[Column("Data",      DataType=DataType.Text,     Length=4294967295),    Nullable]
		public string Data { get; set; } // longtext

		#region Associations

		/// <summary>
		/// FK_JobState_Job
		/// </summary>
		[Association(ThisKey="JobId", OtherKey="Id", CanBeNull=false, KeyName="FK_JobState_Job", BackReferenceName="JobStateJobs")]
		public Job JobStateJob { get; set; }

		#endregion
	}

	[Table("list")]
	public partial class List : LinqToDBEntity
	{
		[Column("Id",       DataType=DataType.Int32)   , PrimaryKey, Identity]
		public int Id { get; set; } // int(11)

		[Column("Key",      DataType=DataType.VarChar,  Length=100), NotNull]
		public string Key { get; set; } // varchar(100)

		[Column("Value",    DataType=DataType.Text,     Length=4294967295),    Nullable]
		public string Value { get; set; } // longtext

		[Column("ExpireAt", DataType=DataType.DateTime),    Nullable]
		public DateTime? ExpireAt { get; set; } // datetime(6)
	}

	[Table("server")]
	public partial class Server : LinqToDBEntity
	{
		[Column("Id",            DataType=DataType.VarChar,  Length=100), PrimaryKey, NotNull]
		public string Id { get; set; } // varchar(100)

		[Column("Data",          DataType=DataType.Text,     Length=4294967295), NotNull]
		public string Data { get; set; } // longtext

		[Column("LastHeartbeat", DataType=DataType.DateTime),    Nullable]
		public DateTime? LastHeartbeat { get; set; } // datetime(6)
	}

	[Table("set")]
	public partial class Set : LinqToDBEntity
	{
		[Column("Id",       DataType=DataType.Int32)   , PrimaryKey, Identity]
		public int Id { get; set; } // int(11)

		[Column("Key",      DataType=DataType.VarChar,  Length=100), NotNull]
		public string Key { get; set; } // varchar(100)

		[Column("Value",    DataType=DataType.VarChar,  Length=256), NotNull]
		public string Value { get; set; } // varchar(256)

		[Column("Score",    DataType=DataType.Single,   Precision=12), NotNull]
		public float Score { get; set; } // float

		[Column("ExpireAt", DataType=DataType.DateTime),    Nullable]
		public DateTime? ExpireAt { get; set; } // datetime
	}

	[Table("state")]
	public partial class State : LinqToDBEntity
	{
		[Column("Id",        DataType=DataType.Int32)   , PrimaryKey, Identity]
		public int Id { get; set; } // int(11)

		[Column("JobId",     DataType=DataType.Int32)   , NotNull]
		public int JobId { get; set; } // int(11)

		[Column("Name",      DataType=DataType.VarChar,  Length=20), NotNull]
		public string Name { get; set; } // varchar(20)

		[Column("Reason",    DataType=DataType.VarChar,  Length=100),    Nullable]
		public string Reason { get; set; } // varchar(100)

		[Column("CreatedAt", DataType=DataType.DateTime), NotNull]
		public DateTime CreatedAt { get; set; } // datetime(6)

		[Column("Data",      DataType=DataType.Text,     Length=4294967295),    Nullable]
		public string Data { get; set; } // longtext

		#region Associations

		/// <summary>
		/// FK_HangFire_State_Job
		/// </summary>
		[Association(ThisKey="JobId", OtherKey="Id", CanBeNull=false, KeyName="FK_HangFire_State_Job", BackReferenceName="HangFireStateJobs")]
		public Job HangFireStateJob { get; set; }

		#endregion
	}
}
