using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using AntData.ORM.Common;

namespace AntData.ORM.Data
{
	using System.Data;

	public class TraceInfo
	{
		public bool           BeforeExecute   { get; set; }
		public TraceLevel     TraceLevel      { get; set; }
		public DataConnection DataConnection  { get; set; }
		public IDbCommand     Command         { get; set; }
		public TimeSpan?      ExecutionTime   { get; set; }
		public int?           RecordsAffected { get; set; }
		public Exception      Exception       { get; set; }
		public string         CommandText     { get; set; }

		private string _sqlText;
		public  string  SqlText
		{
			get
			{
				if (CommandText != null)
					return CommandText;

				if (Command != null)
				{
					if (_sqlText != null)
						return _sqlText;

					var sqlProvider = DataConnection.DataProvider.CreateSqlBuilder();
					var sb          = new StringBuilder();

					sb.Append("-- ").Append(DataConnection.ConnectionString);

					if (DataConnection.DataProvider.Name != sqlProvider.Name)
						sb.Append(' ').Append(sqlProvider.Name);

					sb.AppendLine();

					sqlProvider.PrintParameters(sb, Command.Parameters.Cast<IDbDataParameter>().ToArray());

					sb.AppendLine(Command.CommandText);

					while (sb[sb.Length - 1] == '\n' || sb[sb.Length - 1] == '\r')
						sb.Length--;

					sb.AppendLine();

					return _sqlText = sb.ToString();
				}

				return "";
			}
		}
	}

    public class CustomerTraceInfo
    {
        public CustomerTraceInfo()
        {
            RunTimeList = new List<RunTimeDetail>();
        }

        /// <summary>
        /// 执行的sql文本
        /// </summary>
        public string SqlText { get; set; }

        /// <summary>
        /// 执行sql的具体信息列表 如果是分表分库的情况会有多个
        /// </summary>
        public List<RunTimeDetail> RunTimeList { get; set; }

        /// <summary>
        /// 执行参数
        /// </summary>
        public Dictionary<string, CustomerParam> CustomerParams { get; set; }
    }

    /// <summary>
    /// 执行sql的具体信息
    /// </summary>
    public class RunTimeDetail
    {
        /// <summary>
        /// 执行sql耗费的时间
        /// </summary>
        public TimeSpan Duration { get; set; }

        /// <summary>
        /// DB所在的IP
        /// </summary>
        public string Server { get; set; }

        /// <summary>
        /// DB名称
        /// </summary>
        public string DbName { get; set; }
    }
}
