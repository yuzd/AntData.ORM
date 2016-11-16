//-----------------------------------------------------------------------
// <copyright file="BaseDaoFactory.cs" company="Company">
// Copyright (C) Company. All Rights Reserved.
// </copyright>
// <author>nainaigu</author>
// <summary></summary>
//-----------------------------------------------------------------------

using System.Collections.Concurrent;

namespace AntData.ORM.Dao
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;


    /// <summary>
    /// 创建basedao工厂
    /// </summary>
    public static class BaseDaoFactory
    {
        static readonly ConcurrentDictionary<String, BaseDao> BaseDaos = new ConcurrentDictionary<String, BaseDao>();

        /// <summary>
        /// 创建basedao工厂
        /// </summary>
        /// <param name="logicDbName">数据库逻辑名称</param>
        /// <returns>BaseDao</returns>
        public static BaseDao CreateBaseDao(String logicDbName)
        {
            return BaseDaos.GetOrAdd(logicDbName, key => new BaseDao(key));
        }

       
    }
}