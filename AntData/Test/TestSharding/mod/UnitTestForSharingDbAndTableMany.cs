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
                    Debug.Write($"Server��{detail.Server},DB���ƣ�{detail.DbName}, ִ��ʱ�䣺{detail.Duration.TotalSeconds}��");
                    Debug.Write(Environment.NewLine);
                }
            }
            catch (Exception)
            {
                //ignore
            }
        }


        /// <summary>
        /// ����mod�ֿ���뵽testorm2���ݿ��order_1��
        /// </summary>
        [TestMethod]
        public void TestMethod6_01()
        {
            var id = 1;
            //��testorm2 �� order_1 ��
            var odIsExist = DB.Tables.Orders.Any(r => r.ID.Equals(1) && r.CityId == 1);
            if (odIsExist)
            {
                return;
            }
            var order = new Order
            {
                ID = 1,//����id�ֱ�
                Name = "�Ϻ���ѧ",
                CityId = 1//����cityid�ֿ�
            };

            var result = DB.Insert(order);
            Assert.AreEqual(result, 1);

        }

        /// <summary>
        /// ����mod�ֿ���뵽testorm1���ݿ�
        /// </summary>
        [TestMethod]
        public void TestMethod6_02()
        {

            var id = 2;
            //��testorm1 �� order_2 ��
            var odIsExist = DB.Tables.Orders.Any(r => r.ID.Equals(2) && r.CityId == 2);
            if (odIsExist)
            {
                return;
            }
            var order = new Order
            {
                ID = 2,
                Name = "������ѧ",
                CityId = 2
            };

            var result = DB.Insert(order);
            Assert.AreEqual(result, 1);
        }

        [TestMethod]
        public void TestMethod6_022()
        {

            var id = 2;
            //3%2=1 ��testorm1 ��3%3=0 order_0 ��
            var odIsExist = DB.Tables.Orders.Any(r => r.ID.Equals(3) && r.CityId == 3);
            if (odIsExist)
            {
                return;
            }
            var order = new Order
            {
                ID = 3,
                Name = "���Ŵ�ѧ",
                CityId =3
            };

            var result = DB.Insert(order);
            Assert.AreEqual(result, 1);
        }

        /// <summary>
        /// ����mod�ֿ� ��ѯtestorm2���ݿ�
        /// </summary>
        [TestMethod]
        public void TestMethod6_03()
        {
            var id = 1;
            // 1%2=1 testorm2�� 1%3=1 order_1
            var tb1 = DB.Tables.Orders.FirstOrDefault(r => r.ID.Equals(1)&&r.CityId ==1);
            Assert.IsNotNull(tb1);
        }

        /// <summary>
        /// ����mod�ֿ� ��ѯtestorm1���ݿ�
        /// </summary>
        [TestMethod]
        public void TestMethod6_04()
        {
            var id = 2;
            // 2%2=0 testorm1�� 2%3=2 order_2
            var tb1 = DB.Tables.Orders.FirstOrDefault(r => r.ID.Equals(2)&&r.CityId ==2);
            Assert.IsNotNull(tb1);
        }

        /// <summary>
        /// ����mod�ֿ� ��ָ��sharing column ��ѯ����
        /// </summary>
        [TestMethod]
        public void TestMethod6_05()
        {
            //û��ָ��CityIDҲû��ָ��ID ���ѯ2��db�ĸ�3�ű� 6�εĵ���
            var tb1 = DB.Tables.Orders.ToList();
            Assert.IsNotNull(tb1);
            Assert.AreEqual(tb1.Count, 3);
            //û��ָ��CityID ��ô���ѯ2��db ������ָ����ID ��ôֻ���ѯ 1%3=1 order_1 2%3=2 order_2
            var odIsExist = DB.Tables.Orders.Where(r => r.ID.Equals(1) || r.ID.Equals(2)).ToList();
            Assert.AreEqual(odIsExist.Count, 2);
        }

        /// <summary>
        /// ����mod�ֿ��޸ĵ�testorm2���ݿ�
        /// </summary>
        [TestMethod]
        public void TestMethod6_06()
        {
            var id = 1;
            //û��ָ��CityID ��ô���ѯ2��db ������ָ����ID ��ôֻ���ѯ 1%3=1 order_1 
            var result = DB.Tables.Orders.Where(r => r.ID.Equals(1)).Set(r => r.Name, y => y.Name + "1").Update();
            Assert.AreEqual(result, 1);
        }

        /// <summary>
        /// ����mod�ֿ��޸ĵ�testorm1���ݿ�
        /// </summary>
        [TestMethod]
        public void TestMethod6_07()
        {
            var id = 2;
            //û��ָ��CityID ��ô���ѯ2��db ������ָ����ID ��ôֻ���ѯ 2%3=2 order_2 
            var result = DB.Tables.Orders.Where(r => r.ID.Equals(2)).Set(r => r.Name, y => y.Name + "1").Update();
            Assert.AreEqual(result, 1);
        }

        /// <summary>
        /// ����mod�ֿ�ɾ����testorm2���ݿ�
        /// </summary>
        [TestMethod]
        public void TestMethod6_08()
        {
            var id = 1;
            //û��ָ��CityID ��ô���ѯ2��db ������ָ����ID ��ôֻ���ѯ 1%3=1 order_1 
            var result = DB.Tables.Orders.Where(r => r.ID.Equals(1)).Delete();
            Assert.AreEqual(result, 1);
        }

        /// <summary>
        /// ����mod�ֿ�ɾ����testorm1���ݿ�
        /// </summary>
        [TestMethod]
        public void TestMethod6_09()
        {
            var id = 2;
            //û��ָ��CityID ��ô���ѯ2��db ������ָ����ID ��ôֻ���ѯ 2%3=2 order_2
            var result = DB.Tables.Orders.Where(r => r.ID.Equals(2)).Delete();
            Assert.AreEqual(result, 1);
        }

        [TestMethod]
        public void TestMethod7_01()
        {
            var id = 2;

            //var odIsExist = DB.Tables.Orders.Any(r => r.ID.Equals(id));
            //û��ָ��CityID ��ô���ѯ2��db ������ָ����ID ��ôֻ���ѯ 2%3=2 order_2
            var odIsExist = DB.Tables.Orders.Any(r => r.ID.Equals(2));
            if (odIsExist)
            {
                return;
            }


        }

        /// <summary>
        /// ����mod�ֿ������ֱ���뵽testorm1 �� testorm2���ݿ�
        /// </summary>
        [TestMethod]
        public void TestMethod7_02()
        {
            var orderList = new List<Order>();
            //��ֵ�3%2=1 testorm2 �� order_0
            orderList.Add(new Order
            {
                ID = 3,
                Name = "�Ϻ���ѧ",
                CityId = 3
            });
            //��ֵ�4%2=0 testorm1 �� order_1
            orderList.Add(new Order
            {
                ID = 4,
                Name = "�Ϻ���ѧ",
                CityId = 4
            });

            //û��ָ�� shading column�Ļ���Ĭ�Ϸֵ���һ����Ƭ
            orderList.Add(new Order
            {
                ID = null,
                Name = "�Ϻ���ѧ",
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
        /// ָ����shadingtable�ķֿ� �Զ����ߵ�1��Ӧ��db
        /// </summary>
        [TestMethod]
        public void TestMethod7_04()
        {
            DB.UseShardingDbAndTable("1","1", con =>
            {
                //1%2 = 1 1%3 = 1 testorm2 �� order_1
                var first = con.Tables.Orders.FirstOrDefault();
                Assert.IsNotNull(first);
                Assert.AreEqual(1, first.ID);
            });

        }

        /// <summary>
        /// ָ����shadingtable�ķֿ� �Զ����ߵ�0��Ӧ��db
        /// </summary>
        [TestMethod]
        public void TestMethod7_05()
        {
            DB.UseShardingDbAndTable("0","0", con =>
            {
                //0%2 = 0 0%3 = 0 testorm1 �� order_0
                var first = con.Tables.Orders.FirstOrDefault();
                Assert.IsNotNull(first);
                Assert.AreEqual(2, first.ID);
            });

        }
    }
}
