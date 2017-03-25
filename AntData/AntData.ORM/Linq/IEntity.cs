//-----------------------------------------------------------------------
// <copyright file="IEntity.cs" company="Company">
// Copyright (C) Company. All Rights Reserved.
// </copyright>
// <author>nainaigu</author>
// <summary></summary>
//-----------------------------------------------------------------------

using System.Linq;

namespace AntData.ORM.Linq
{
    /// <summary>
    /// 
    /// </summary>
    public interface IEntity
    {
       IQueryable<T> Get<T>() where T : class;
    }

    public class BaseEntity
    {
        
    }
}