//-----------------------------------------------------------------------
// <copyright file="HintsUtil.cs" company="Company">
// Copyright (C) Company. All Rights Reserved.
// </copyright>
// <author>nainaigu</author>
// <summary></summary>
//-----------------------------------------------------------------------

using System.Collections;

namespace AntData.ORM.DbEngine.Dao.Common.Util
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    class HintsUtil
    {
        public static IDictionary CloneHints(IDictionary hints)
        {
            var temp = hints as Dictionary<String, Object>;
            var dict = new Dictionary<String, Object>();

            if (temp == null)
                return dict;

            foreach (var item in temp)
            {
                dict.Add(item.Key, item.Value);
            }

            return dict;
        }
    }
}