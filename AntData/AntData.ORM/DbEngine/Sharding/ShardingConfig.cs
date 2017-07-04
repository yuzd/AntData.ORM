//-----------------------------------------------------------------------
// <copyright file="ShardingConfig.cs" company="Company">
// Copyright (C) Company. All Rights Reserved.
// </copyright>
// <author>nainaigu</author>
// <summary></summary>
//-----------------------------------------------------------------------
namespace AntData.ORM.DbEngine.Sharding
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;


    /// <summary>
    /// Sharing 需要的一些配置参数
    /// </summary>
    public class ShardingConfig
    {
        public Object Start { get; set; }

        public Object End { get; set; }

        public String Sharding { get; set; }
    }
}