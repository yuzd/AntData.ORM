//-----------------------------------------------------------------------
// <copyright file="DalException.cs" company="Company">
// Copyright (C) Company. All Rights Reserved.
// </copyright>
// <author>nainaigu</author>
// <summary></summary>
//-----------------------------------------------------------------------

using System.Runtime.Serialization;

namespace AntData.ORM.Dao
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;



    [Serializable]
    public class DalException : Exception
    {
        public DalException() : base("Database Error.")
        {
        }

        public DalException(String errorMessage) : base(errorMessage)
        {
        }

        public DalException(String msgFormat, params Object[] os) : base(String.Format(msgFormat, os))
        {
        }

#if !NETSTANDARD
        protected DalException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
#endif
        public DalException(String message, Exception innerException) : base(message, innerException)
        {
        }
    }
}