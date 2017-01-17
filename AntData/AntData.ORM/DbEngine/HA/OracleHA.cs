using System;
using System.Collections.Generic;
using AntData.ORM.Common.Util;

namespace AntData.ORM.DbEngine.HA
{
    class OracleHA : AbstractHA
    {
        //1043：无效连接
        //1205：加锁超时
        private static readonly HashSet<Int32> OracleErrorCodes = new HashSet<Int32> { 1043, 1205 };

        public override HashSet<Int32> RetryFailOverErrorCodes
        {
            get
            {
                //String errorCodes = null;
                //var result = String.IsNullOrEmpty(errorCodes) ? MySqlErrorCodes : SplitUtil.SplitAsInt32(errorCodes);
                return OracleErrorCodes;
            }
        }
    }
}
