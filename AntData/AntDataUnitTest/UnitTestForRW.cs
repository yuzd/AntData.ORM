using System;
using System.Diagnostics;
using System.Linq;
using AntData.ORM;
using AntData.ORM.Data;
using DbModels.Mysql;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AntDataUnitTest
{
    [TestClass]
    public class UnitTestForRW
    {
        private static MysqlDbContext<Entitys> DB
        {
            get
            {
                var db = new MysqlDbContext<Entitys>("testormrw");
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

        /// <summary>
        /// 测试写操作从Master
        /// </summary>
        [TestMethod]
        public void TestMethod1_01()
        {
            var person = new Person
            {
                Name = "test1",
                SchoolId = 3,
                Age = 11,
            };
            var result = DB.Insert(person);
            Assert.AreEqual(result,1);
        }

        /// <summary>
        /// 测试读操作从Slaver
        /// </summary>
        [TestMethod]
        public void TestMethod1_02()
        {
            var result = DB.Tables.People.FirstOrDefault(r => r.Name.Equals("test1"));
            Assert.IsNotNull(result);
        }

        /// <summary>
        /// 测试读写
        /// </summary>
        [TestMethod]
        public void TestMethod1_03()
        {
            var person = new Person
            {
                Name = "test2",
                SchoolId = 3,
                Age = 11,
            };
            var result1 = DB.Insert(person);
            Assert.AreEqual(result1, 1);
            var result = DB.Tables.People.FirstOrDefault(r => r.Name.Equals("test2"));
            Assert.IsNotNull(result);
        }


        /// <summary>
        /// 测试事物不管是读写都是走Master
        /// </summary>
        [TestMethod]
        public void TestMethod1_04()
        {
           DB.UseTransaction(con =>
           {
               var person = new Person
               {
                   Name = "test3",
                   SchoolId = 3,
                   Age = 11,
               };
               var result1 = con.Insert(person);
               Assert.AreEqual(result1, 1);

               var result = con.Tables.People.FirstOrDefault(r => r.Name.Equals("test3"));
               Assert.IsNotNull(result);
           });
        }
    }
}
