//-----------------------------------------------------------------------
// <copyright file="LinqToDBEntity .cs" company="Company">
// Copyright (C) Company. All Rights Reserved.
// </copyright>
// <author>nainaigu</author>
// <create>2016-05-24</create>
// <summary></summary>
//-----------------------------------------------------------------------

using System;
using AntData.ORM.Mapping;

namespace AntData.ORM.Mysql.Base
{
    /// <summary>
    /// LinqToDBEntity
    /// </summary>
    public class LinqToDBEntity:LinqToDBEntityBase
    {
        //[PrimaryKey, Column(Name = "Tid"), NotNull]
        //public override long Tid { get; set; } 

        ////private DateTime _createTime = DateTime.Now;

        ////[Column(Name = "CreateTime")]
        ////public override DateTime CreateTime {
        ////    get
        ////    {
        ////        return _createTime;
        ////    }
        ////    set
        ////    {
        ////        _createTime = value;
                
        ////    }
        ////}

        //private DateTime _updateTime = DateTime.Now;

        //[Column(Name = "DataChange_LastTime")]
        //public override DateTime DataChange_LastTime
        //{
        //    get
        //    {
        //        return _updateTime;
        //    }
        //    set
        //    {
        //        _updateTime = value;

        //    }
        //} 
    }
}