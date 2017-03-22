//-----------------------------------------------------------------------
// <copyright file="StaticMethod.cs" company="Company">
// Copyright (C) Company. All Rights Reserved.
// </copyright>
// <author>nainaigu</author>
// <summary></summary>
//-----------------------------------------------------------------------

using DbModels.Mysql;

namespace AntDataUnitTest
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;


    public class BPerson
    {
        public string PersonName { get; set; }
        public string SchoolName { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class StaticMethod
    {
        public static BPerson BuildPerson(Person p, School s)
        {
            BPerson pp = new BPerson();
            if (s!=null)
            {
                pp.SchoolName = s.Name;
            }
            pp.PersonName = p.Name;
            return pp;
        }
    }
}