using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;

namespace AntData.ORM
{
	using Linq;
	using Mapping;
	using SqlProvider;

	public interface IDataContext : IDisposable
	{
		string              ContextID         { get; }
		Func<ISqlBuilder>   CreateSqlProvider { get; }
		Func<ISqlOptimizer> GetSqlOptimizer   { get; }
		SqlProviderFlags    SqlProviderFlags  { get; }
		Type                DataReaderType    { get; }
		MappingSchema       MappingSchema     { get; }
		bool                InlineParameters  { get; set; }
		List<string>        QueryHints        { get; }
		List<string>        NextQueryHints    { get; }

		Expression          GetReaderExpression(MappingSchema mappingSchema, IDataReader reader, int idx, Expression readerExpression, Type toType);
		bool?               IsDBNullAllowed    (IDataReader reader, int idx);

        #region 执行sql

        object SetQuery(IQueryContext queryContext, ArrayList arrList = null);
        int ExecuteNonQuery(object query);
        object ExecuteScalar(object query);
        object ExecuteScalar(object query, bool Identity);
        IList<IDataReader> ExecuteReader(object query);
        void ReleaseQuery(object query);

        #endregion
        /// <summary>
        /// 获取Sql文
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        string              GetSqlText         (object query);

	}
}
