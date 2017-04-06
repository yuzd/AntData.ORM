using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using AntData.ORM.Data;
using DbModels.SqlServer;

namespace ConsoleApp1
{
    public class Program
    {
        private static SqlServerlDbContext<Entitys> DB;
        public static void Main(string[] args)
        {
            DB = new SqlServerlDbContext<Entitys>("testorm_sqlserver");
            AntData.ORM.Common.Configuration.Linq.AllowMultipleQuery = true;
            //Insert的时候 忽略Null的字段
            AntData.ORM.Common.Configuration.Linq.IgnoreNullInsert = true;
            DB.IsEnableLogTrace = true;
            DB.OnLogTrace = OnCustomerTraceConnection;
            DB.IsNoLock = true;

            var p = DB.Tables.People.FirstOrDefault();
            Console.WriteLine(p.Name);
            Console.ReadLine();
        }

        private static void OnCustomerTraceConnection(CustomerTraceInfo customerTraceInfo)
        {
            try
            {
                string sql = customerTraceInfo.CustomerParams.Aggregate(customerTraceInfo.SqlText,
                    (current, item) => current.Replace(item.Key, item.Value.Value.ToString()));
                Debug.Write(sql + Environment.NewLine);
            }
            catch (Exception)
            {
                //ignore
            }
        }
    }
}
