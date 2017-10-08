using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AntData.ORM.Data;
using DbModels.SqlServer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ConsoleApp1
{
    public class Program
    {
        private static SqlServerlDbContext<Entitys> DB;
        public static void Main(string[] args)
        {
            AntData.ORM.Common.Configuration.Linq.AllowMultipleQuery = true;
            //Insert的时候 忽略Null的字段
            AntData.ORM.Common.Configuration.Linq.IgnoreNullInsert = true;
       


            var builder = new ConfigurationBuilder()
                .AddJsonFile(Directory.GetCurrentDirectory() + "\\Config\\Dal.json").Build();
            AntData.ORM.Common.Configuration.UseDBConfig(builder);


            IServiceCollection serviceCollection = new ServiceCollection();
            serviceCollection.AddSqlServerEntitys<Entitys>("testorm_sqlserver", ops =>
            {
                ops.IsEnableLogTrace = true;
                ops.OnLogTrace = OnCustomerTraceConnection;
            });

            var Services = serviceCollection.BuildServiceProvider();
            var entitys = Services.GetService<Entitys>();

            var p1 = entitys.People.FirstOrDefault();
           
            var p2 = entitys.People.FirstOrDefault(r => r.Name.Equals("yuzd"));
            Console.WriteLine(p1.Name);

            DB = new SqlServerlDbContext<Entitys>("testorm_sqlserver");
            DB.IsEnableLogTrace = true;
            DB.OnLogTrace = OnCustomerTraceConnection;

            var p = DB.Tables.People.FirstOrDefault();
            var p22 = DB.Tables.People.FirstOrDefault(r => r.Name.Equals("yuzd"));
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
                foreach (var detail in customerTraceInfo.RunTimeList)
                {
                    Debug.Write($"Server：{detail.Server},DB名称：{detail.DbName}, 执行时间：{detail.Duration.TotalSeconds}秒");
                    Debug.Write(Environment.NewLine);
                }
            }
            catch (Exception)
            {
                //ignore
            }
        }
    }
}
