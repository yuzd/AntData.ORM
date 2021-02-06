using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using AntData.ORM;
using AntData.ORM.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestSharding.MysqlDBAndtableMany;

namespace TestSharding
{
    [TestClass]
    public class UnitTestForSharingDbAndTableMany
    {
        private static MysqlDbContext<Entitys> DB
        {
            get
            {
                var db = new MysqlDbContext<Entitys>("testshardingdbandtableMany");
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
                .AddJsonFile("appsettings.json", optional: true);
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
        /// 测试mod分库插入到testorm2数据库的order_1表
        /// </summary>
        [TestMethod]
        public void TestMethod6_01()
        {
            var id = 1;
            //查testorm2 的 order_1 表
            var odIsExist = DB.Tables.Orders.Any(r => r.ID.Equals(1) && r.CityId == 1);
            if (odIsExist)
            {
                return;
            }
            var order = new Order
            {
                ID = 1,//按照id分表
                Name = "上海大学",
                CityId = 1//按照cityid分库
            };

            var result = DB.Insert(order);
            Assert.AreEqual(result, 1);

        }

        /// <summary>
        /// 测试mod分库插入到testorm1数据库
        /// </summary>
        [TestMethod]
        public void TestMethod6_02()
        {

            var id = 2;
            //查testorm1 的 order_2 表
            var odIsExist = DB.Tables.Orders.Any(r => r.ID.Equals(2) && r.CityId == 2);
            if (odIsExist)
            {
                return;
            }
            var order = new Order
            {
                ID = 2,
                Name = "北京大学",
                CityId = 2
            };

            var result = DB.Insert(order);
            Assert.AreEqual(result, 1);
        }

        [TestMethod]
        public void TestMethod6_022()
        {

            var id = 2;
            //3%2=1 查testorm1 的3%3=0 order_0 表
            var odIsExist = DB.Tables.Orders.Any(r => r.ID.Equals(3) && r.CityId == 3);
            if (odIsExist)
            {
                return;
            }
            var order = new Order
            {
                ID = 3,
                Name = "厦门大学",
                CityId =3
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
            // 1%2=1 testorm2的 1%3=1 order_1
            var tb1 = DB.Tables.Orders.FirstOrDefault(r => r.ID.Equals(1)&&r.CityId ==1);
            Assert.IsNotNull(tb1);
        }

        /// <summary>
        /// 测试mod分库 查询testorm1数据库
        /// </summary>
        [TestMethod]
        public void TestMethod6_04()
        {
            var id = 2;
            // 2%2=0 testorm1的 2%3=2 order_2
            var tb1 = DB.Tables.Orders.FirstOrDefault(r => r.ID.Equals(2)&&r.CityId ==2);
            Assert.IsNotNull(tb1);
        }

        /// <summary>
        /// 测试mod分库 不指定sharing column 查询叠加
        /// </summary>
        [TestMethod]
        public void TestMethod6_05()
        {
            //没有指定CityID也没有指定ID 会查询2个db的各3张表 6次的叠加
            var tb1 = DB.Tables.Orders.ToList();
            Assert.IsNotNull(tb1);
            Assert.AreEqual(tb1.Count, 3);
            //没有指定CityID 那么会查询2个db 。由于指定了ID 那么只会查询 1%3=1 order_1 2%3=2 order_2
            var odIsExist = DB.Tables.Orders.Where(r => r.ID.Equals(1) || r.ID.Equals(2)).ToList();
            Assert.AreEqual(odIsExist.Count, 2);
        }

        /// <summary>
        /// 测试mod分库修改到testorm2数据库
        /// </summary>
        [TestMethod]
        public void TestMethod6_06()
        {
            var id = 1;
            //没有指定CityID 那么会查询2个db 。由于指定了ID 那么只会查询 1%3=1 order_1 
            var result = DB.Tables.Orders.Where(r => r.ID.Equals(1)).Set(r => r.Name, y => y.Name + "1").Update();
            Assert.AreEqual(result, 1);
        }

        /// <summary>
        /// 测试mod分库修改到testorm1数据库
        /// </summary>
        [TestMethod]
        public void TestMethod6_07()
        {
            var id = 2;
            //没有指定CityID 那么会查询2个db 。由于指定了ID 那么只会查询 2%3=2 order_2 
            var result = DB.Tables.Orders.Where(r => r.ID.Equals(2)).Set(r => r.Name, y => y.Name + "1").Update();
            Assert.AreEqual(result, 1);
        }

        /// <summary>
        /// 测试mod分库删除到testorm2数据库
        /// </summary>
        [TestMethod]
        public void TestMethod6_08()
        {
            var id = 1;
            //没有指定CityID 那么会查询2个db 。由于指定了ID 那么只会查询 1%3=1 order_1 
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
            //没有指定CityID 那么会查询2个db 。由于指定了ID 那么只会查询 2%3=2 order_2
            var result = DB.Tables.Orders.Where(r => r.ID.Equals(2)).Delete();
            Assert.AreEqual(result, 1);
        }

        [TestMethod]
        public void TestMethod7_01()
        {
            var id = 2;

            //var odIsExist = DB.Tables.Orders.Any(r => r.ID.Equals(id));
            //没有指定CityID 那么会查询2个db 。由于指定了ID 那么只会查询 2%3=2 order_2
            var odIsExist = DB.Tables.Orders.Any(r => r.ID.Equals(2));
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
            //会分到3%2=1 testorm2 的 order_0
            orderList.Add(new Order
            {
                ID = 3,
                Name = "上海大学",
                CityId = 3
            });
            //会分到4%2=0 testorm1 的 order_1
            orderList.Add(new Order
            {
                ID = 4,
                Name = "上海大学",
                CityId = 4
            });

            //没有指定 shading column的话是默认分到第一个分片
            orderList.Add(new Order
            {
                ID = null,
                Name = "上海大学",
                //CityId = 0
            });
             var rows = DB.BulkCopy(orderList);
            Assert.AreEqual(rows.RowsCopied, 3);
        }

        [TestMethod]
        public void TestMethod7_03()
        {
            var odIsExist = DB.Tables.Orders.Delete();

        }

        /// <summary>
        /// 指定了shadingtable的分库 自动会走到1对应的db
        /// </summary>
        [TestMethod]
        public void TestMethod7_04()
        {
            DB.UseShardingDbAndTable("1","1", con =>
            {
                //1%2 = 1 1%3 = 1 testorm2 的 order_1
                var first = con.Tables.Orders.FirstOrDefault();
                Assert.IsNotNull(first);
                Assert.AreEqual(1, first.ID);
            });

        }

        /// <summary>
        /// 指定了shadingtable的分库 自动会走到0对应的db
        /// </summary>
        [TestMethod]
        public void TestMethod7_05()
        {
            DB.UseShardingDbAndTable("0","0", con =>
            {
                //0%2 = 0 0%3 = 0 testorm1 的 order_0
                var first = con.Tables.Orders.FirstOrDefault();
                Assert.IsNotNull(first);
                Assert.AreEqual(2, first.ID);
            });

        }
    }
}
