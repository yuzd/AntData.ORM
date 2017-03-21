using System;
using System.Diagnostics;
using System.Linq;
using AntData.ORM;
using AntData.ORM.Data;
using DbModels.Mysql3;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AntDataUnitTest
{
    [TestClass]
    public class UnitTestForSharingTable
    {
        private static MysqlDbContext<Entitys> DB
        {
            get
            {
                var db = new MysqlDbContext<Entitys>("testshardingtable");
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
        /// 测试mod分表插入到testorm3数据库的orders_1表里面
        /// </summary>
        [TestMethod]
        public void TestMethod1_01()
        {
            var id = 1;
            var odIsExist = DB.Tables.Orders.Any(r => r.ID.Equals(id));
            if (odIsExist)
            {
                return;
            }

            var order = new Orders
            {
                ID = id,
                Name = "订单1"
            };

            var result = DB.Insert(order);
            Assert.AreEqual(result, 1);
        }

        /// <summary>
        /// 测试mod分表插入到testorm3数据库的orders_0表里面
        /// </summary>
        [TestMethod]
        public void TestMethod1_02()
        {
            var id = 2;
            var odIsExist = DB.Tables.Orders.Any(r => r.ID.Equals(id));
            if (odIsExist)
            {
                return;
            }
            var order = new Orders
            {
                ID = id,
                Name = "订单2"
            };

            var result = DB.Insert(order);
            Assert.AreEqual(result, 1);
        }

        /// <summary>
        /// 测试mod分表 查询testorm3数据库orders_1表
        /// </summary>
        [TestMethod]
        public void TestMethod1_03()
        {
            var id = 1;
            var tb1 = DB.Tables.Orders.FirstOrDefault(r => r.ID.Equals(id));
            Assert.IsNotNull(tb1);
        }

        /// <summary>
        /// 测试mod分表 查询testorm3数据库orders_0表
        /// </summary>
        [TestMethod]
        public void TestMethod1_04()
        {
            var id = 2;
            var tb1 = DB.Tables.Orders.FirstOrDefault(r => r.ID.Equals(id));
            Assert.IsNotNull(tb1);
        }


        /// <summary>
        /// 测试mod分表 不指定sharing column 查询叠加
        /// </summary>
        [TestMethod]
        public void TestMethod1_05()
        {
            var tb1 = DB.Tables.Orders.ToList();
            Assert.IsNotNull(tb1);
            Assert.AreEqual(tb1.Count, 2);
        }

        /// <summary>
        /// 测试mod分表修改到testorm3数据库orders_1表
        /// </summary>
        [TestMethod]
        public void TestMethod1_06()
        {
            var id = 1;
            var result = DB.Tables.Orders.Where(r => r.ID.Equals(id)).Set(r => r.Name, y => y.Name + "1").Update();
            Assert.AreEqual(result, 1);
        }

        /// <summary>
        /// 测试mod分表修改到testorm3数据库orders_0表
        /// </summary>
        [TestMethod]
        public void TestMethod1_07()
        {
            var id = 2;
            var result = DB.Tables.Orders.Where(r => r.ID.Equals(id)).Set(r => r.Name, y => y.Name + "1").Update();
            Assert.AreEqual(result, 1);
        }

        /// <summary>
        /// 测试mod分表删除到testorm3数据库orders_1表
        /// </summary>
        [TestMethod]
        public void TestMethod6_08()
        {
            var id = 1;
            var result = DB.Tables.Orders.Where(r => r.ID.Equals(id)).Delete();
            Assert.AreEqual(result, 1);
        }

        /// <summary>
        /// 测试mod分表删除到testorm3数据库orders_0表
        /// </summary>
        [TestMethod]
        public void TestMethod6_09()
        {
            var id = 2;
            var result = DB.Tables.Orders.Where(r => r.ID.Equals(id)).Delete();
            Assert.AreEqual(result, 1);
        }
    }
}
