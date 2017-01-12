using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using AntData.ORM;
using AntData.ORM.Data;
using AntData.ORM.DataProvider.SqlServer;
using Dapper;
using Dapper.Contrib;
using Dapper.Contrib.Extensions;
using DbModels.SqlServer;
using SqlSugar;
using SyntacticSugar;
using Entitys = DbModels.SqlServer.Entitys;

namespace PkDapper.Demos
{
    public class InsertList : IDemos
    {
        private static DbContext<Entitys> DB = new DbContext<Entitys>("testorm_sqlserver", new SqlServerDataProvider(SqlServerVersion.v2008));
        public void Init()
        {
            Console.WriteLine("测试插入1000条记录的集合");

            var eachCount = 10;

            /*******************车轮战是性能评估最准确的一种方式***********************/
            for (int i = 0; i < 10; i++)
            {
                //清除
                DeleteAddDatas();

                //dapper
                Dapper(eachCount);


                //清除
                DeleteAddDatas();

                //sqlSugar
                AntData(eachCount);
                //清除
                DeleteAddDatas();

                //sqlSugar
                //SqlSugar(eachCount);
            }
            Console.WriteLine("测试结束");
        }

        private static void DeleteAddDatas()
        {
            DB.Tables.Tests.Where(r => r.FString == "Test").Delete();
            
        }
        private static List<Test> GetList
        {
            get
            {
                List<Test> list = new List<Test>();
                for (int i = 0; i < 1000; i++)
                {
                    Test t = new Test()
                    {
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

        private static List<Models.PkDapper.Test> GetList1
        {
            get
            {
                List<Models.PkDapper.Test> list = new List<Models.PkDapper.Test>();
                for (int i = 0; i < 1000; i++)
                {
                    Models.PkDapper.Test t = new Models.PkDapper.Test()
                    {
                        F_Int32 = 1,
                        F_String = "Test",
                        F_Float = 1,
                        F_DateTime = DateTime.Now,
                        F_Byte = 1,
                        F_Bool = true
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
                    var list = conn.SqlBulkCopy(GetList1);
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
                    var list = conn.Insert(GetList1);
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
