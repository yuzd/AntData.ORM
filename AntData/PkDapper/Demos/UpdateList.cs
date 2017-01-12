using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using AntData.ORM.Data;
using AntData.ORM.DataProvider.SqlServer;
using Dapper;
using Dapper.Contrib;
using Dapper.Contrib.Extensions;
using DbModels.SqlServer;
using SqlSugar;
using SyntacticSugar;
namespace PkDapper.Demos
{
    public class UpdateList : IDemos
    {
        private static DbContext<Entitys> DB = new DbContext<Entitys>("testorm_sqlserver", new SqlServerDataProvider(SqlServerVersion.v2008));

        public void Init()
        {
            Console.WriteLine("测试更新1000条的集合");

            var eachCount = 10;

            /*******************车轮战是性能评估最准确的一种方式***********************/
            for (int i = 0; i < 10; i++)
            {

                //dapper
                Dapper(eachCount);

                //sqlSugar
                //SqlSugar(eachCount);
                AntData(eachCount);
            }

        }
        private static List<Test> GetList
        {
            get
            {
                List<Test> list = new List<Test>();
                for (int i = 1000; i < 2000; i++)
                {
                    Test t = new Test()
                    {
                        Id=i,
                        FInt32 = 1,
                        FString = "Test",
                        FFloat = 1,
                        FDateTime = DateTime.Now,
                        FByte = 1,
                        FBool = true
                    };
                    list.Add(t);
                }
                return list;
            }
        }
        private static void SqlSugar(int eachCount)
        {
            GC.Collect();//回收资源
            System.Threading.Thread.Sleep(2000);//休息2秒

            PerHelper.Execute(eachCount, "SqlSugar", () =>
            {
                using (SqlSugarClient conn = new SqlSugarClient(PubConst.connectionString))
                {
                    var list = conn.SqlBulkReplace(GetList);
                }
            });
        }

        private static void Dapper(int eachCount)
        {
            GC.Collect();//回收资源
            System.Threading.Thread.Sleep(2000);//休息2秒

            //正试比拼
            PerHelper.Execute(eachCount, "Dapper", () =>
            {
                using (SqlConnection conn = new SqlConnection(PubConst.connectionString))
                {
                    var list = conn.Update(GetList);
                }
            });
        }

        private static void AntData(int eachCount)
        {
            GC.Collect();//回收资源
            System.Threading.Thread.Sleep(2000);//休息2秒

            //正试比拼
            PerHelper.Execute(eachCount, "AntData", () =>
            {
                DB.BulkCopy(GetList);
            });
        }
    }
}
