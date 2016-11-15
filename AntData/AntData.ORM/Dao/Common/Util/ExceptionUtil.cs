using System;
using System.Collections.Concurrent;
using System.Data.Common;
using System.Diagnostics;
using AntData.ORM.Dao.sql;

namespace AntData.ORM.Common.Util
{
    public class ExceptionUtil
    {
        

        /// <summary>
        /// 判断一个异常的堆栈中是否有超时异常
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        public static Boolean IsTimeoutException(Exception ex)
        {
            if (null == ex)
                return false;
            if (ex is TimeoutException)
                return true;
            var currentEx = ex.InnerException;

            while (currentEx != null)
            {
                if (currentEx is TimeoutException)
                    return true;
                currentEx = currentEx.InnerException;
            }
            return false;
        }

    }
}
