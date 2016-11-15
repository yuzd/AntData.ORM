//-----------------------------------------------------------------------
// <copyright file="IEntity.cs" company="Company">
// Copyright (C) Company. All Rights Reserved.
// </copyright>
// <author>nainaigu</author>
// <summary></summary>
//-----------------------------------------------------------------------

namespace AntData.ORM.Mysql.Base
{
    /// <summary>
    /// 
    /// </summary>
    public interface IEntity
    {
        System.Linq.IQueryable<T> Get<T>() where T : class;
    }
}