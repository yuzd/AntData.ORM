using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using AntData.ORM;
using AntData.ORM.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestSharding.Mysql1;

namespace TestSharding
{
    [TestClass]
    public class RangeTestForSharingDb
    {
        private static MysqlDbContext<Entitys> DB
        {
            get
            {
                var db = new MysqlDbContext<Entitys>("testshardingdb");
                db.IsEnableLogTrace = true;
                db.OnLogTrace = OnCustomerTraceConnection;
                return db;
            }
        }

        [ClassInitialize()]
        public static void MyClassInitialize(TestContext testContext)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings_range.json", optional: true);
            var configuration = builder.Build();
            AntData.ORM.Common.Configuration.UseDBConfig(configuration);
        }

        private static void OnCustomerTraceConnection(CustomerTraceInfo customerTraceInfo)
        {
            try
            {
                string sql = customerTraceInfo.SqlText;
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


        /// <summary>
        /// 测试range分库插入到testorm1数据库
        /// </summary>
        [TestMethod]
        public void TestMethod6_01()
        {
            var id = 1;
            var odIsExist = DB.Tables.Orders.Any(r => r.ID.Equals(1));
            if (odIsExist)
            {
                return;
            }
            var order = new Order
            {
                ID = 1,
                Name = "上海大学"
            };

            var result = DB.Insert(order);
            Assert.AreEqual(result, 1);

        }

        /// <summary>
        /// 测试range分库插入到testorm2数据库
        /// </summary>
        [TestMethod]
        public void TestMethod6_02()
        {

            var id = 2;
            var odIsExist = DB.Tables.Orders.Any(r => r.ID.Equals(11));
            if (odIsExist)
            {
                return;
            }
            var order = new Order
            {
                ID = 11,
                Name = "北京大学"
            };

            var result = DB.Insert(order);
            Assert.AreEqual(result, 1);
        }

        /// <summary>
        /// 测试mod分库 查询testorm2数据库
        /// </summary>
        [TestMethod]
        public void TestMethod6_03()
        {
            var id = 1;
            var tb1 = DB.Tables.Orders.FirstOrDefault(r => r.ID.Equals(1));
            Assert.IsNotNull(tb1);
        }

        /// <summary>
        /// 测试mod分库 查询testorm1数据库
        /// </summary>
        [TestMethod]
        public void TestMethod6_04()
        {
            var id = 2;
            var tb1 = DB.Tables.Orders.FirstOrDefault(r => r.ID.Equals(11));
            Assert.IsNotNull(tb1);
        }

        /// <summary>
        /// 测试mod分库 不指定sharing column 查询叠加
        /// </summary>
        [TestMethod]
        public void TestMethod6_05()
        {
            var tb1 = DB.Tables.Orders.ToList();
            Assert.IsNotNull(tb1);
            Assert.AreEqual(tb1.Count, 2);

            var odIsExist = DB.Tables.Orders.Where(r => r.ID.Equals(1) || r.ID.Equals(11)).ToList();
            Assert.AreEqual(odIsExist.Count, 2);
        }

        /// <summary>
        /// 测试mod分库修改到testorm2数据库
        /// </summary>
        [TestMethod]
        public void TestMethod6_06()
        {
            var id = 1;
            var result = DB.Tables.Orders.Where(r => r.ID.Equals(11)).Set(r => r.Name, y => y.Name + "1").Update();
            Assert.AreEqual(result, 1);
        }

        /// <summary>
        /// 测试mod分库修改到testorm1数据库
        /// </summary>
        [TestMethod]
        public void TestMethod6_07()
        {
            var id = 2;
            var result = DB.Tables.Orders.Where(r => r.ID.Equals(11)).Set(r => r.Name, y => y.Name + "1").Update();
            Assert.AreEqual(result, 1);
        }

        /// <summary>
        /// 测试mod分库删除到testorm2数据库
        /// </summary>
        [TestMethod]
        public void TestMethod6_08()
        {
            var id = 1;
            var result = DB.Tables.Orders.Where(r => r.ID.Equals(1)).Delete();
            Assert.AreEqual(result, 1);
        }

        /// <summary>
        /// 测试mod分库删除到testorm1数据库
        /// </summary>
        [TestMethod]
        public void TestMethod6_09()
        {
            var id = 2;
            var result = DB.Tables.Orders.Where(r => r.ID.Equals(11)).Delete();
            Assert.AreEqual(result, 1);
        }

        [TestMethod]
        public void TestMethod7_01()
        {
            var id = 2;

            //var odIsExist = DB.Tables.Orders.Any(r => r.ID.Equals(id));

            var odIsExist = DB.Tables.Orders.Any(r => r.ID.Equals(11));
            if (odIsExist)
            {
                return;
            }


        }

        /// <summary>
        /// 测试mod分库批量分别插入到testorm1 和 testorm2数据库
        /// </summary>
        [TestMethod]
        public void TestMethod7_02()
        {
            var orderList = new List<Order>();
            orderList.Add(new Order
            {
                ID = 2,
                Name = "上海大学"
            });
            orderList.Add(new Order
            {
                ID = 12,
                Name = "上海大学"
            });
            //没有指定 shading column的话是默认分到第一个分片
            orderList.Add(new Order
            {
                ID = null,
                Name = "上海大学"
            });
            var rows = DB.BulkCopy(orderList);
            Assert.AreEqual(rows.RowsCopied, 3);
        }

        [TestMethod]
        public void TestMethod7_03()
        {
            var odIsExist = DB.Tables.Orders.Delete();
            Assert.AreEqual(odIsExist, 3);

        }
    }
}
