//-----------------------------------------------------------------------
// <copyright file="DALExtStatementConstant.cs" company="Company">
// Copyright (C) Company. All Rights Reserved.
// </copyright>
// <author>nainaigu</author>
// <summary></summary>
//-----------------------------------------------------------------------
namespace AntData.ORM.Dao.Common
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// DAL Extended Statement constant definition
    /// </summary>
    public class DALExtStatementConstant
    {
        /// <summary>
        /// Schema
        /// </summary>
        public const String RETRIEVE_SCHEMA = "schema";

        /// <summary>
        /// Disable Constraints
        /// </summary>
        public const String DISABLE_CONSTRAINTS = "disableconstraints";

        /// <summary>
        /// the timeout of SQL command
        /// </summary>
        public const String TIMEOUT = "timeout";

        /// <summary>
        /// the id of specified shard
        /// </summary>
        public const String SHARDID = "shardid";

        /// <summary>
        /// shard by table
        /// </summary>
        public const String TABLEID = "tableid";

        /// <summary>
        /// skip null or not when update
        /// </summary>
        public const String SETNULL = "setnull";

        /// <summary>
        /// specify the LOCK TYPE
        /// </summary>
        public const String LOCK = "lock";

        /// <summary>
        /// mark the SQL statement as sensitive
        /// </summary>
        public const String SENSITIVE = "sensitive";
        /// <summary>
        /// 新增主键的值
        /// </summary>
        public const String INSERTPKVALUE = "insertpkvalue";
        /// <summary>
        /// 是否有自增长id
        /// </summary>
        public const String ISHASID = "ishasid";
        /// <summary>
        /// 读写分离新鲜度
        /// </summary>
        public const String FRESHNESS = "fressness";
        /// <summary>
        /// 当前执行的SQL
        /// </summary>
        public const String SQL = "sql";

        /// <summary>
        /// Operation Type:replaced for parameter in method
        /// </summary>
        public const String OPERATION_TYPE = "OPERATION_TYPE";

        /// <summary>
        /// Map:used for sharding
        /// </summary>
        public const String MAP = "MAP";

        /// <summary>
        /// Shard column value:built in hints to calculate shard id
        /// </summary>
        public const String SHARD_COLUMN_VALUE = "SHARD_COLUMN_VALUE";

        /// <summary>
        /// Specify Shard Ids
        /// </summary>
        public const String SHARD_IDS = "SHARD_IDS";

        /// <summary>
        /// Specify Table Ids
        /// </summary>
        public const String TABLE_IDS = "TABLE_IDS";

        /// <summary>
        /// Specify shard,table mapping dict
        /// </summary>
        public const String SHARD_TABLE_DICT = "SHARD_TABLE_DICT";

        /// <summary>
        /// 数据库的字段参数关键字
        /// </summary>
        public const String PARAMETER_SYMBOL = "ParameterSymbol";
        /// <summary>
        /// 是否是BulkCopy
        /// </summary>
        public const String BULK_COPY = "BulkCopy";
        public const String BULK_COPY_ORACLE = "OracleBulkCopy";

        /// <summary>
        /// 事物执行期间的数据库连接
        /// </summary>
        public const String TRANSACTION_CONNECTION = "ExecuteTransaction_ConnectionWrapper";
        /// <summary>
        /// 事物指定等级
        /// </summary>
        public const String ISOLATION_LEVEL = "IsolationLevel";
    }
}