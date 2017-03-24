using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace AntData.ORM.Data
{
	using Common;
	using Expressions;
	using Extensions;

	public class CommandInfo
	{
		public DataConnection  DataConnection;
		public string          CommandText;
		public DataParameter[] Parameters;
		public CommandType     CommandType = CommandType.Text;

		#region Init

		public CommandInfo(DataConnection dataConnection, string commandText)
		{
			DataConnection = dataConnection;
			CommandText    = commandText;
		}

		public CommandInfo(DataConnection dataConnection, string commandText, params DataParameter[] parameters)
		{
			DataConnection = dataConnection;
			CommandText    = commandText;
			Parameters     = parameters;
		}

		public CommandInfo(DataConnection dataConnection, string commandText, DataParameter parameter)
		{
			DataConnection = dataConnection;
			CommandText    = commandText;
			Parameters     = new[] { parameter };
		}

		public CommandInfo(DataConnection dataConnection, string commandText, object parameters)
		{
			DataConnection = dataConnection;
			CommandText    = commandText;
			Parameters     = GetDataParameters(dataConnection, parameters);
		}

		#endregion

		#region Query with object reader

		public IEnumerable<T> QueryProc<T>(Func<IDataReader,T> objectReader)
		{
			CommandType = CommandType.StoredProcedure;
			return Query(objectReader);
		}

		public IEnumerable<T> Query<T>(Func<IDataReader,T> objectReader)
		{
			DataConnection.InitCommand(CommandType, CommandText, Parameters, null);
            Dictionary<string, CustomerParam> param = new Dictionary<string, CustomerParam>();
            if (Parameters != null && Parameters.Length > 0)
                param = SetParameters(DataConnection, Parameters);

		    foreach (var rd in DataConnection.ExecuteReader(CommandText, param))
		    {
                using (rd)
                    while (rd.Read())
                        yield return objectReader(rd);
            }
			
		}

        #endregion

        #region Query

        public IEnumerable<T> QueryProc<T>()
        {
            CommandType = CommandType.StoredProcedure;
            return Query<T>();
        }

        public IEnumerable<T> Query<T>()
        {
            DataConnection.InitCommand(CommandType, CommandText, Parameters, null);
            Dictionary<string, CustomerParam> param = new Dictionary<string, CustomerParam>();
            if (Parameters != null && Parameters.Length > 0)
                param = SetParameters(DataConnection, Parameters);

            foreach (var rd in DataConnection.ExecuteReader(CommandText, param))
            {
                using (rd)
                {
                    if (rd.Read())
                    {
                        var objectReader = GetObjectReader<T>(DataConnection, rd, CommandText);
                        var isFaulted = false;

                        do
                        {
                            T result;

                            try
                            {
                                result = objectReader(rd);
                            }
                            catch (InvalidCastException)
                            {
                                if (isFaulted)
                                    throw;

                                isFaulted = true;
                                objectReader = GetObjectReader2<T>(DataConnection, rd, CommandText);
                                result = objectReader(rd);
                            }

                            yield return result;

                        } while (rd.Read());
                    }
                }
            }
           
        }

        public DataTable QueryTable()
        {
            DataConnection.InitCommand(CommandType, CommandText, Parameters, null);
            Dictionary<string, CustomerParam> param = new Dictionary<string, CustomerParam>();
            if (Parameters != null && Parameters.Length > 0)
                param = SetParameters(DataConnection, Parameters);

            return DataConnection.ExecuteDataTable(CommandText, param);
        }

        public async Task<DataTable> QueryTableAsync()
        {
            DataConnection.InitCommand(CommandType, CommandText, Parameters, null);
            Dictionary<string, CustomerParam> param = new Dictionary<string, CustomerParam>();
            if (Parameters != null && Parameters.Length > 0)
                param = SetParameters(DataConnection, Parameters);

            return await AsyncExtensions.GetTask(() => DataConnection.ExecuteDataTable(CommandText, param),CancellationToken.None);
        }
        #endregion
        #region Query with template

        public IEnumerable<T> Query<T>(T template)
		{
			return Query<T>();
		}

		public IEnumerable<T> QueryProc<T>(T template)
		{
			return QueryProc<T>();
		}

		#endregion

		#region Execute

		public int ExecuteProc()
		{
			CommandType = CommandType.StoredProcedure;
			return Execute();
		}

		public int Execute()
		{
			DataConnection.InitCommand(CommandType, CommandText, Parameters, null);

			var hasParameters = Parameters != null && Parameters.Length > 0;
            Dictionary < string, CustomerParam > param = new Dictionary<string, CustomerParam>();

            if (hasParameters)
                param = SetParameters(DataConnection, Parameters);

			var commandResult = DataConnection.ExecuteNonQuery(CommandText, param);

            //if (hasParameters)
            //    RebindParameters(DataConnection, Parameters);

			return commandResult;
		}

		#endregion

		

		#region Execute scalar

		public T ExecuteProc<T>()
		{
			CommandType = CommandType.StoredProcedure;
			return Execute<T>();
		}

		public T Execute<T>()
		{
			DataConnection.InitCommand(CommandType, CommandText, Parameters, null);
            Dictionary<string, CustomerParam> param = new Dictionary<string, CustomerParam>();
            if (Parameters != null && Parameters.Length > 0)
                param = SetParameters(DataConnection, Parameters);
            
          
            foreach (var rd in DataConnection.ExecuteReader(CommandText, param))
		    {
                using (rd)
                {
                    if (rd.Read())
                    {
                        var objectReader = GetObjectReader<T>(DataConnection, rd, CommandText);
                        try
                        {
                            return objectReader(rd);
                        }
                        catch (InvalidCastException)
                        {
                            return GetObjectReader2<T>(DataConnection, rd, CommandText)(rd);
                        }
                        catch (FormatException)
                        {
                            return GetObjectReader2<T>(DataConnection, rd, CommandText)(rd);
                        }
                    }
                }
            }
			

			return default(T);
		}

	    public long ExcuteSharding()
	    {
            DataConnection.InitCommand(CommandType, CommandText, Parameters, null);
            Dictionary<string, CustomerParam> param = new Dictionary<string, CustomerParam>();
            if (Parameters != null && Parameters.Length > 0)
                param = SetParameters(DataConnection, Parameters);

	        var result = 0L;
            foreach (var rd in DataConnection.ExecuteReader(CommandText, param))
            {
                using (rd)
                {
                    if (rd.Read())
                    {
                        var objectReader = GetObjectReader<long>(DataConnection, rd, CommandText);
                        try
                        {
                            result += objectReader(rd);
                        }
                        catch (InvalidCastException)
                        {
                            result += GetObjectReader2<long>(DataConnection, rd, CommandText)(rd);
                        }
                        catch (FormatException)
                        {
                            result += GetObjectReader2<long>(DataConnection, rd, CommandText)(rd);
                        }
                    }
                }
            }
	        return result;
	    }
		#endregion



		#region ExecuteReader

		public DataReader ExecuteReaderProc()
		{
			CommandType = CommandType.StoredProcedure;
			return ExecuteReader();
		}

		public DataReader ExecuteReader()
		{
			DataConnection.InitCommand(CommandType, CommandText, Parameters, null);
            Dictionary<string, CustomerParam> param = new Dictionary<string, CustomerParam>();
            if (Parameters != null && Parameters.Length > 0)
                param = SetParameters(DataConnection, Parameters);

			return new DataReader { CommandInfo = this, Reader = DataConnection.ExecuteReader( CommandText, param) };
		}

		internal IEnumerable<T> ExecuteQuery<T>(IList<IDataReader> readers, string sql)
		{
		    foreach (var rd in readers)
		    {
                if (rd.Read())
                {
                    var objectReader = GetObjectReader<T>(DataConnection, rd, sql);
                    var isFaulted = false;

                    do
                    {
                        T result;

                        try
                        {
                            result = objectReader(rd);
                        }
                        catch (InvalidCastException)
                        {
                            if (isFaulted)
                                throw;

                            isFaulted = true;
                            objectReader = GetObjectReader2<T>(DataConnection, rd, sql);
                            result = objectReader(rd);
                        }

                        yield return result;

                    } while (rd.Read());
                }
            }
			
		}

		internal T ExecuteScalar<T>(IList<IDataReader> readers, string sql)
		{
		    foreach (var rd in readers)
		    {
                if (rd.Read())
                {
                    try
                    {
                        return GetObjectReader<T>(DataConnection, rd, sql)(rd);
                    }
                    catch (InvalidCastException)
                    {
                        return GetObjectReader2<T>(DataConnection, rd, sql)(rd);
                    }
                }
            }

			return default(T);
		}

		#endregion

		

		#region SetParameters

		static Dictionary<string, CustomerParam> SetParameters(DataConnection dataConnection, DataParameter[] parameters)
		{
		    var result = new Dictionary<string, CustomerParam>();
            if (parameters == null)
				return result;

			foreach (var parameter in parameters)
			{
                var p = new CustomerParam();  //p = dataConnection.Command.CreateParameter();
				var dataType = parameter.DataType;
				var value    = parameter.Value;
			    p.TableName = parameter.TableName;
			    p.ColumnName = parameter.ColumnName;

                if (dataType == DataType.Undefined && value != null)
					dataType = dataConnection.MappingSchema.GetDataType(value.GetType()).DataType;

                //if (parameter.Direction != null) p.Direction = parameter.Direction.Value;
                //if (parameter.Size      != null) p.Size      = parameter.Size.     Value;

				dataConnection.DataProvider.SetParameter(p, parameter.Name, dataType, value);

			    p.ParameterName = parameter.Name;
                p.DbType = DataTypeConvert.Convert(dataType);
                result.Add(parameter.Name.StartsWith(dataConnection.DataProvider.ParameterSymbol) ? parameter.Name : dataConnection.DataProvider.ParameterSymbol + parameter.Name, p);
				//dataConnection.Command.Parameters.Add(p);
			}
		    return result;
		}


		struct ParamKey : IEquatable<ParamKey>
		{
			public ParamKey(Type type, int configID)
			{
				_type     = type;
				_configID = configID;

				unchecked
				{
					_hashCode = -1521134295 * (-1521134295 * 639348056 + _type.GetHashCode()) + _configID.GetHashCode();
				}
			}

			public override bool Equals(object obj)
			{
				return Equals((ParamKey)obj);
			}

			readonly int    _hashCode;
			readonly Type   _type;
			readonly int    _configID;

			public override int GetHashCode()
			{
				return _hashCode;
			}

			public bool Equals(ParamKey other)
			{
				return
					_type     == other._type &&
					_configID == other._configID
					;
			}
		}

		static readonly ConcurrentDictionary<ParamKey,Func<object,DataParameter[]>> _parameterReaders =
			new ConcurrentDictionary<ParamKey,Func<object,DataParameter[]>>();

		static readonly PropertyInfo _dataParameterName     = MemberHelper.PropertyOf<DataParameter>(p => p.Name);
		static readonly PropertyInfo _dataParameterDataType = MemberHelper.PropertyOf<DataParameter>(p => p.DataType);
		static readonly PropertyInfo _dataParameterValue    = MemberHelper.PropertyOf<DataParameter>(p => p.Value);

		static DataParameter[] GetDataParameters(DataConnection dataConnection, object parameters)
		{
			if (parameters == null)
				return null;

			if (parameters is DataParameter[])
				return (DataParameter[])parameters;

			if (parameters is DataParameter)
				return new[] { (DataParameter)parameters };

            if (parameters is IEnumerable<DataParameter>)
                return ((IEnumerable<DataParameter>)parameters).ToArray();

            Func<object,DataParameter[]> func;
			var type = parameters.GetType();
			var key  = new ParamKey(type, dataConnection.ID);

			if (!_parameterReaders.TryGetValue(key, out func))
			{
				var td  = dataConnection.MappingSchema.GetEntityDescriptor(type);
				var p   = Expression.Parameter(typeof(object), "p");
				var obj = Expression.Parameter(parameters.GetType(), "obj");

				var expr = Expression.Lambda<Func<object,DataParameter[]>>(
					Expression.Block(
						new[] { obj },
						new Expression[]
						{
							Expression.Assign(obj, Expression.Convert(p, type)),
							Expression.NewArrayInit(
								typeof(DataParameter),
								td.Columns.Select(m =>
								{
									if (m.MemberType == typeof(DataParameter))
									{
										var pobj = Expression.Parameter(typeof(DataParameter));

										return Expression.Block(
											new[] { pobj },
											new Expression[]
											{
												Expression.Assign(pobj, Expression.PropertyOrField(obj, m.MemberName)),
												Expression.MemberInit(
													Expression.New(typeof(DataParameter)),
													Expression.Bind(
														_dataParameterName,
														Expression.Coalesce(
															Expression.MakeMemberAccess(pobj, _dataParameterName),
															Expression.Constant(m.ColumnName))),
													Expression.Bind(
														_dataParameterDataType,
														Expression.MakeMemberAccess(pobj, _dataParameterDataType)),
													Expression.Bind(
														_dataParameterValue,
														Expression.Convert(
															Expression.MakeMemberAccess(pobj, _dataParameterValue),
															typeof(object))))
											});
									}

									var memberType  = m.MemberType.ToNullableUnderlying();
									var valueGetter = Expression.PropertyOrField(obj, m.MemberName) as Expression;
									var mapper      = dataConnection.MappingSchema.GetConvertExpression(memberType, typeof(DataParameter), createDefault : false);

									if (mapper != null)
									{
										return Expression.Call(
											MemberHelper.MethodOf(() => PrepareDataParameter(null, null)),
											mapper.GetBody(valueGetter),
											Expression.Constant(m.ColumnName));
									}

									if (memberType.IsEnumEx())
									{
										var mapType  = ConvertBuilder.GetDefaultMappingFromEnumType(dataConnection.MappingSchema, memberType);
										var convExpr = dataConnection.MappingSchema.GetConvertExpression(m.MemberType, mapType);

										memberType  = mapType;
										valueGetter = convExpr.GetBody(valueGetter);
									}

									return (Expression)Expression.MemberInit(
										Expression.New(typeof(DataParameter)),
										Expression.Bind(
											_dataParameterName,
											Expression.Constant(m.ColumnName)),
										Expression.Bind(
											_dataParameterDataType,
											Expression.Constant(dataConnection.MappingSchema.GetDataType(memberType).DataType)),
										Expression.Bind(
											_dataParameterValue,
											Expression.Convert(valueGetter, typeof(object))));
								}))
						}
					),
					p);

				_parameterReaders[key] = func = expr.Compile();
			}

			return func(parameters);
		}

		static DataParameter PrepareDataParameter(DataParameter dataParameter, string name)
		{
			if (dataParameter == null)
				return new DataParameter { Name = name };

			dataParameter.Name = name;

			return dataParameter;
		}

		#endregion

		#region GetObjectReader

		struct QueryKey : IEquatable<QueryKey>
		{
			public QueryKey(Type type, int configID, string sql)
			{
				_type     = type;
				_configID = configID;
				_sql      = sql;

				unchecked
				{
					_hashCode = -1521134295 * (-1521134295 * (-1521134295 * 639348056 + _type.GetHashCode()) + _configID.GetHashCode()) + _sql.GetHashCode();
				}
			}

			public override bool Equals(object obj)
			{
				return Equals((QueryKey)obj);
			}

			readonly int    _hashCode;
			readonly Type   _type;
			readonly int    _configID;
			readonly string _sql;

			public override int GetHashCode()
			{
				return _hashCode;
			}

			public bool Equals(QueryKey other)
			{
				return
					_type     == other._type &&
					_sql      == other._sql  &&
					_configID == other._configID
					;
			}
		}

		static readonly ConcurrentDictionary<QueryKey,Delegate> _objectReaders = new ConcurrentDictionary<QueryKey,Delegate>();

		public static void ClearObjectReaderCache()
		{
			_objectReaders.   Clear();
			_parameterReaders.Clear();
		}

		static Func<IDataReader,T> GetObjectReader<T>(DataConnection dataConnection, IDataReader dataReader, string sql)
		{
			var key = new QueryKey(typeof(T), dataConnection.ID, sql);

			Delegate func;

			if (!_objectReaders.TryGetValue(key, out func))
			{
				//return GetObjectReader2<T>(dataConnection, dataReader);
				_objectReaders[key] = func = CreateObjectReader<T>(dataConnection, dataReader, (type,idx,dataReaderExpr) =>
					new ConvertFromDataReaderExpression(type, idx, dataReaderExpr, dataConnection).Reduce(dataReader));
			}

			return (Func<IDataReader,T>)func;
		}

		static Func<IDataReader,T> GetObjectReader2<T>(DataConnection dataConnection, IDataReader dataReader, string sql)
		{
			var key = new QueryKey(typeof(T), dataConnection.ID, sql);

			var func = CreateObjectReader<T>(dataConnection, dataReader, (type,idx,dataReaderExpr) =>
				new ConvertFromDataReaderExpression(type, idx, dataReaderExpr, dataConnection).Reduce());

			_objectReaders[key] = func;

			return func;
		}

		static Func<IDataReader,T> CreateObjectReader<T>(
			DataConnection dataConnection,
			IDataReader    dataReader,
			Func<Type,int,Expression,Expression> getMemberExpression)
		{
			var parameter      = Expression.Parameter(typeof(IDataReader));
			var dataReaderExpr = Expression.Convert(parameter, dataReader.GetType());

			Expression expr;

			if (dataConnection.MappingSchema.IsScalarType(typeof(T)))
			{
				expr = getMemberExpression(typeof(T), 0, dataReaderExpr);
			}
			else
			{
				var td    = dataConnection.MappingSchema.GetEntityDescriptor(typeof(T));
				var names = new List<string>(dataReader.FieldCount);

				for (var i = 0; i < dataReader.FieldCount; i++)
					names.Add(dataReader.GetName(i));

				expr = null;

				var ctors = typeof(T).GetConstructors().Select(c => new { c, ps = c.GetParameters() }).ToList();

				if (ctors.Count > 0 && ctors.All(c => c.ps.Length > 0))
				{
					var q =
						from c in ctors
						let count = c.ps.Count(p => names.Contains(p.Name))
						orderby count descending
						select c;

					var ctor = q.FirstOrDefault();

					if (ctor != null)
					{
						expr = Expression.New(
							ctor.c,
							ctor.ps.Select(p => names.Contains(p.Name) ?
								getMemberExpression(p.ParameterType, names.IndexOf(p.Name), dataReaderExpr) :
								Expression.Constant(dataConnection.MappingSchema.GetDefaultValue(p.ParameterType), p.ParameterType)));
					}
				}

				if (expr == null)
				{
					var members =
					(
						from n in names.Select((name,idx) => new { name, idx })
						let   member = td.Columns.FirstOrDefault(m => 
							string.Compare(m.ColumnName, n.name, dataConnection.MappingSchema.ColumnComparisonOption) == 0)
						where member != null
						select new
						{
							Member = member,
							Expr   = getMemberExpression(member.MemberType, n.idx, dataReaderExpr),
						}
					).ToList();

					expr = Expression.MemberInit(
						Expression.New(typeof(T)),
						members.Select(m => Expression.Bind(m.Member.MemberInfo, m.Expr)));
				}
			}

			if (expr.GetCount(e => e == dataReaderExpr) > 1)
			{
				var dataReaderVar = Expression.Variable(dataReaderExpr.Type, "dr");
				var assignment    = Expression.Assign(dataReaderVar, dataReaderExpr);

				expr = expr.Transform(e => e == dataReaderExpr ? dataReaderVar : e);
				expr = Expression.Block(new[] { dataReaderVar }, new[] { assignment, expr });
			}

			var lex = Expression.Lambda<Func<IDataReader,T>>(expr, parameter);

			return lex.Compile();
		}

		#endregion
	}
}
