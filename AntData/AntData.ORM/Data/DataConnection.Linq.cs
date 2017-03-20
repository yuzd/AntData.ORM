using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using AntData.ORM.Common;

namespace AntData.ORM.Data
{
	using DataProvider;
	using Linq;
	using Mapping;
	using SqlQuery;
	using SqlProvider;

	public partial class DataConnection : IDataContext
	{
		public ITable<T> GetTable<T>()
			where T : class
		{
			return new Table<T>(this);
		}

		public ITable<T> GetTable<T>(bool dispose)
			where T : class
		{
			return new Table<T>(new DataContextInfo(this, dispose));
		}

		public ITable<T> GetTable<T>(object instance, MethodInfo methodInfo, params object[] parameters)
			where T : class
		{
			return DataExtensions.GetTable<T>(this, instance, methodInfo, parameters);
		}

		internal class PreparedQuery
		{
			public string[]           Commands;
			public List<SqlParameter> SqlParameters;
			public IDbDataParameter[] Parameters;
			public SelectQuery        SelectQuery;
			public ISqlBuilder        SqlProvider;
			public List<string>       QueryHints;
		    public Dictionary<string, CustomerParam> Params;
		}

		#region SetQuery

		internal PreparedQuery GetCommand(IQueryContext query, ArrayList arrList = null)
		{
		    var needRefresh = false;
            if (arrList != null && arrList.Count > 0 && (bool)arrList[0])
            {
                if (query.SelectQuery.Insert.WithIdentity)
                {
                    needRefresh = true;
                }

                if (query.Parameters != null && query.Parameters.Count > 0)
                {
                    var ignoreList = new List<string>();
                    //如果指定了忽略NULL字段的插入
                    foreach (var v in query.Parameters)
                    {
                        if (v.SqlParameter != null && v.SqlParameter.Value == null)
                        {
                            ignoreList.Add(v.SqlParameter.Name.ReplaceNumbers());//如果是关键字会在结尾里面加数字
                        }
                    }
                    if (ignoreList.Count > 0)
                    {
                        needRefresh = true;
                        query.Parameters.RemoveAll(r => r.SqlParameter != null && r.SqlParameter.Value == null);
                        if (arrList.Count == 2 && (bool)arrList[1])
                        {
                            //指定了update的更新忽略null
                            query.SelectQuery.Update.Items.RemoveAll(r => ignoreList.Contains(((SqlQuery.SqlField)r.Column).Name));

                        }
                        else
                        {
                            query.SelectQuery.Insert.Items.RemoveAll(r => ignoreList.Contains(((SqlQuery.SqlField)r.Column).Name));
                        }
                    }
                }
            }
            if (query.Context != null && !needRefresh )
			{
                return new PreparedQuery
				{
					Commands      = (string[])query.Context,
					SqlParameters = query.SelectQuery.Parameters,
					SelectQuery   = query.SelectQuery,
					QueryHints    = query.QueryHints,
                    Params = query.Params
                };
			}

            

            var sql    = query.SelectQuery.ProcessParameters();
			var newSql = ProcessQuery(sql);

			if (!object.ReferenceEquals(sql, newSql))
			{
				sql = newSql;
				sql.IsParameterDependent = true;
			}

			var sqlProvider = DataProvider.CreateSqlBuilder();
		    sqlProvider.IsNoLock = this.IsNoLock;
            var cc = sqlProvider.CommandCount(sql);//获取自增的时候 才会是2
			var sb = new StringBuilder();
            var commands = new string[cc];
		    var extendHit = new List<CustomerParam>();
			for (var i = 0; i < cc; i++)
			{
				sb.Length = 0;

				sqlProvider.BuildSql(i, sql, sb, extendHit);
				commands[i] = sb.ToString();
			}
           
            if (!query.SelectQuery.IsParameterDependent)
				query.Context = commands;

		    return  new PreparedQuery
			{
				Commands      = commands,
				SqlParameters = sql.Parameters,
				SelectQuery   = sql,
				SqlProvider   = sqlProvider,
				QueryHints    = query.QueryHints,
                Params = extendHit.Count > 0? extendHit.ToDictionary(r=>r.ParameterName,r=>r) :query.Params
            };
		}

		protected virtual SelectQuery ProcessQuery(SelectQuery selectQuery)
		{
			return selectQuery;
		}

		void GetParameters(IQueryContext query, PreparedQuery pq)
		{
			var parameters = query.GetParameters();

			if (parameters.Length == 0 && pq.SqlParameters.Count == 0)
				return;

			var ordered = DataProvider.SqlProviderFlags.IsParameterOrderDependent;
			var c       = ordered ? pq.SqlParameters.Count : parameters.Length;
			var parms   = new List<IDbDataParameter>(c);
           

            if (ordered)
			{
				for (var i = 0; i < pq.SqlParameters.Count; i++)
				{
					var sqlp = pq.SqlParameters[i];

					if (sqlp.IsQueryParameter)
					{
						var parm = parameters.Length > i && object.ReferenceEquals(parameters[i], sqlp) ? parameters[i] : parameters.First(p => object.ReferenceEquals(p, sqlp));
                        pq.Params.Add(DataProvider.ParameterSymbol + parm.Name,AddParameter(parms, parm.Name, parm));
					}
				}
			}
			else
			{
				foreach (var parm in parameters)
				{
				    if (parm.IsQueryParameter) // && pq.SqlParameters.Contains(parm)
                    {
                        pq.Params.Add(DataProvider.ParameterSymbol + parm.Name, AddParameter(parms, parm.Name, parm));
                    }
						
				}
			}

			pq.Parameters = parms.ToArray();
		}

        CustomerParam AddParameter(ICollection<IDbDataParameter> parms, string name, SqlParameter parm)
		{
		   
            var p = new CustomerParam();
			var dataType = parm.DataType;

			if (dataType == DataType.Undefined)
			{
				dataType = MappingSchema.GetDataType(
					parm.SystemType == typeof(object) && parm.Value != null ?
						parm.Value.GetType() :
						parm.SystemType).DataType;
			}

			DataProvider.SetParameter(p, name, dataType, parm.Value);
		    p.DbType = DataTypeConvert.Convert(dataType);
		    p.TableName = parm.TableName;
		    p.ColumnName = parm.CoumnName;
		    return p;
		;}

		#endregion

		#region ExecuteXXX

		int IDataContext.ExecuteNonQuery(object query)
		{
			var pq = (PreparedQuery)query;

			if (pq.Commands.Length == 1)
			{
				InitCommand(CommandType.Text, pq.Commands[0], null, pq.QueryHints);
				return ExecuteNonQuery(pq.Commands[0],pq.Params);
			}
			else
			{
				for (var i = 0; i < pq.Commands.Length; i++)
				{
					InitCommand(CommandType.Text, pq.Commands[i], null, i == 0 ? pq.QueryHints : null);

					if (i < pq.Commands.Length - 1 && pq.Commands[i].StartsWith("DROP"))
					{
						try
						{
							ExecuteNonQuery(pq.Commands[i],pq.Params);
						}
						catch (Exception)
						{
						}
					}
					else
					{
						ExecuteNonQuery(pq.Commands[i], pq.Params);
					}
				}

				return -1;
			}
		}

        object IDataContext.ExecuteScalar(object query, bool Identity)
        {
            var pq = (PreparedQuery)query;
            
            if (Identity)
            {
                InitCommand(CommandType.Text,pq.Commands[0] + pq.Commands[1], null, null);
                return ExecuteScalar(pq.Commands[0]  + pq.Commands[1],pq.Params,true);//执行Insert
            }

            InitCommand(CommandType.Text, pq.Commands[0], null, pq.QueryHints);

            if (pq.Commands.Length == 1)
            {
                return ExecuteScalar(pq.Commands[0], pq.Params);
            }

            ExecuteNonQuery(pq.Commands[0], pq.Params);

            InitCommand(CommandType.Text, pq.Commands[1], null, null);

            return ExecuteScalar(pq.Commands[1], pq.Params);
        }
		object IDataContext.ExecuteScalar(object query)
		{
			var pq = (PreparedQuery)query;

            InitCommand(CommandType.Text, pq.Commands[0], null, pq.QueryHints);

            if (pq.Commands.Length == 1)
			{
				return ExecuteScalar(pq.Commands[0],pq.Params);
			}

			ExecuteNonQuery(pq.Commands[0], pq.Params);

			InitCommand(CommandType.Text, pq.Commands[1], null, null);

			return ExecuteScalar(pq.Commands[1], pq.Params);
		}

		IList<IDataReader> IDataContext.ExecuteReader(object query)
		{
			var pq = (PreparedQuery)query;

			InitCommand(CommandType.Text, pq.Commands[0], null, pq.QueryHints);

			return ExecuteReader(pq.Commands[0], pq.Params);
		}

		void IDataContext.ReleaseQuery(object query)
		{
		}

		#endregion

		#region GetSqlText

		string IDataContext.GetSqlText(object query)
		{
			var pq = (PreparedQuery)query;

			var sqlProvider = pq.SqlProvider ?? DataProvider.CreateSqlBuilder();

			var sb = new StringBuilder();

			sb.Append("-- ").Append(ConnectionString);


			if (DataProvider.Name != sqlProvider.Name)
				sb.Append(' ').Append(sqlProvider.Name);

			sb.AppendLine();

			sqlProvider.PrintParameters(sb, pq.Parameters);

			var isFirst = true;

			foreach (var command in pq.Commands)
			{
				sb.AppendLine(command);

				if (isFirst && pq.QueryHints != null && pq.QueryHints.Count > 0)
				{
					isFirst = false;

					while (sb[sb.Length - 1] == '\n' || sb[sb.Length - 1] == '\r')
						sb.Length--;

					sb.AppendLine();

					var sql = sb.ToString();

					var sqlBuilder = DataProvider.CreateSqlBuilder();
					sql = sqlBuilder.ApplyQueryHints(sql, pq.QueryHints);

					sb = new StringBuilder(sql);
				}
			}

			while (sb[sb.Length - 1] == '\n' || sb[sb.Length - 1] == '\r')
				sb.Length--;

			sb.AppendLine();

			return sb.ToString();
		}

		#endregion

		#region IDataContext Members

		SqlProviderFlags IDataContext.SqlProviderFlags { get { return DataProvider.SqlProviderFlags; } }
		Type             IDataContext.DataReaderType   { get { return DataProvider.DataReaderType;   } }

		Expression IDataContext.GetReaderExpression(MappingSchema mappingSchema, IDataReader reader, int idx, Expression readerExpression, Type toType)
		{
			return DataProvider.GetReaderExpression(mappingSchema, reader, idx, readerExpression, toType);
		}

		bool? IDataContext.IsDBNullAllowed(IDataReader reader, int idx)
		{
			return DataProvider.IsDBNullAllowed(reader, idx);
		}

		object IDataContext.SetQuery(IQueryContext queryContext, ArrayList arrList = null)
		{
            var reverseCount = 0;
			var query = GetCommand(queryContext, arrList);
		    if (query.Params == null)
		    {
		        query.Params = new Dictionary<string, CustomerParam>();
		    }
		    else
		    {
                reverseCount = query.Params.Count;

		    }
            GetParameters(queryContext, query);
#if DEBUG
           // Debug.WriteLine(((IDataContext)this).GetSqlText(query).Replace("\r", ""));
#endif
		    if (reverseCount > 0)
		    {
		        var q = query.Params.Skip(reverseCount).Take(query.Params.Count - reverseCount).ToList();
                q.AddRange(query.Params.Take(reverseCount));
                query.Params = q.ToDictionary(r=>r.Key,r=>r.Value);
		    }
            return query;
		}



		string IDataContext.ContextID
		{
			get { return DataProvider.Name; }
		}

		static Func<ISqlBuilder> GetCreateSqlProvider(IDataProvider dp)
		{
			return dp.CreateSqlBuilder;
		}

		Func<ISqlBuilder> IDataContext.CreateSqlProvider
		{
			get { return GetCreateSqlProvider(DataProvider); }
		}

		static Func<ISqlOptimizer> GetGetSqlOptimizer(IDataProvider dp)
		{
			return dp.GetSqlOptimizer;
		}

		Func<ISqlOptimizer> IDataContext.GetSqlOptimizer
		{
			get { return GetGetSqlOptimizer(DataProvider); }
		}

		#endregion
	}
}
