using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using AntData.ORM;
using AntData.ORM.Data;
using AntData.ORM.DataProvider.MySql;
using AntData.ORM.DbEngine.Configuration;
using AntData.ORM.Linq;
using AntData.ORM.Mapping;
using DbModels.Mysql;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MySql.Data.MySqlClient;

namespace AntDataUnitTest
{
    [TestClass]
    public class UnitTestForDBSetting
    {
        private static MysqlDbContext<Entitys> DB
        {
            get
            {
                var db = new MysqlDbContext<Entitys>("testorm");
                db.IsEnableLogTrace = true;
                db.OnLogTrace = OnCustomerTraceConnection;
                return db;
            }
        }

        [ClassInitialize()]
        public static void MyClassInitialize(TestContext testContext)
        {
            AntData.ORM.Common.Configuration.Linq.AllowMultipleQuery = true;
            AntData.ORM.Common.Configuration.Linq.IgnoreNullInsert = true;
            AntData.ORM.Common.Configuration.DBSettings = new DBSettings
            {
                DatabaseSettings = new List<DatabaseSettings>
                {
                    new DatabaseSettings
                    {
                        Name = "testorm",
                        Provider = "mysql",
                        ConnectionItemList = new List<ConnectionStringItem>
                        {
                            new ConnectionStringItem
                            {
                                Name = "testorm1",
                                ConnectionString = "Server=127.0.0.1;Port=28747;Database=testorm;Uid=root;Pwd=123456;charset=utf8;"
                            }
                        }
                    }
                }
            };
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

        [TestMethod]
        public void TestMethod1_01()
        {
            string name = "yuzd";
            var updateResult = DB.Tables.People.Where(r => r.Name.Equals(name)).OrderBy(r => r.Id).ToList();
            Assert.AreEqual(updateResult.Count > 0, true);

        }

    }
}
