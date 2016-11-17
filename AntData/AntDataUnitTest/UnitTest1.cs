using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using AntData.ORM;
using AntData.ORM.Data;
using Arch.Data.ORM.Mysql;
using DbModels.Mysql;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AntDataUnitTest
{
    [TestClass]
    public class UnitTest1
    {
        private static MysqlContext<Entitys> DB;
        [ClassInitialize()]
        public static void MyClassInitialize(TestContext testContext)
        {
            DB = new MysqlContext<Entitys>("testorm");
            DB.OnCustomerTraceConnection = OnCustomerTraceConnection;
        }

        private static void OnCustomerTraceConnection(CustomerTraceInfo customerTraceInfo)
        {
            try
            {
                string sql = customerTraceInfo.CustomerParams.Aggregate(customerTraceInfo.SqlText,
                       (current, item) => current.Replace(item.Key, item.Value.Value.ToString()));
                Debug.Write(sql);
            }
            catch (Exception)
            {
                //ignore
            }
        }
        [TestMethod]
        public void TestMethod1_00()
        {
            List<Person> pList = new List<Person>
            {
                new Person
                {
                    Name = "yuzd",
                    Age = 27
                },
                new Person
                {
                    Name = "nainaigu",
                    Age = 18
                }
            };

            var insertResult = DB.BulkCopy(pList);
            Assert.AreEqual(insertResult.RowsCopied, 2);

        }
        [TestMethod]
        public void TestMethod1_01()
        {
            Person p = new Person
            {
                Name = "yuzd",
                Age = 27
            };

            var insertResult = DB.Insert(p) > 0;
            Assert.AreEqual(insertResult, true);

        }

        [TestMethod]
        public void TestMethod1_02()
        {
            var updateResult = DB.Tables.People.Where(r => r.Name.Equals("yuzd")).Set(r => r.Age, 10).Update() > 0;
            Assert.AreEqual(updateResult, true);

        }

        [TestMethod]
        public void TestMethod1_03()
        {
            var updateResult = DB.Tables.People.Where(r => r.Name.Equals("yuzd")).Delete() > 0;
            Assert.AreEqual(updateResult, true);
            TestMethod1_01();
        }

        [TestMethod]
        public void TestMethod1_04()
        {
            var p = DB.Tables.People.FirstOrDefault();
            Assert.IsNotNull(p);
            Assert.IsNotNull(p.Age);
        }

        [TestMethod]
        public void TestMethod1_05()
        {
            Person p = new Person
            {
                Name = "yuzd",
                Age = 27
            };

            var insertResult = DB.InsertWithIdentity(p);
            var id = Convert.ToInt64(insertResult);
            Assert.AreEqual(id > 0, true);
            Assert.AreEqual(p.Id, id);

        }

        [TestMethod]
        public void TestMethod1_06()
        {
            var updateResult = DB.Tables.People.Where(r => r.Name.Equals("yuzd")).Set2<Person, int?>("Age", 20).Update() > 0;
            Assert.AreEqual(updateResult, true);

        }

        [TestMethod]
        public void TestMethod1_07()
        {
            var updateResult = DB.Tables.People.Where(r => r.Name.Equals("yuzd")).OrderBy(r => r.Id).ToList();
            Assert.AreEqual(updateResult.Count > 0, true);

        }

        [TestMethod]
        public void TestMethod1_08()
        {
            var updateResult = DB.Tables.People.Where(r => r.Name.Equals("yuzd")).OrderByDescending(r => r.Id).ToList();
            Assert.AreEqual(updateResult.Count > 0, true);

        }

        [TestMethod]
        public void TestMethod1_09()
        {
            var updateResult = DB.Tables.People.Where(r => r.Name.Equals("yuzd")).OrderByDescending(r => r.Id).Skip(1).Take(1).ToList();
            Assert.AreEqual(updateResult.Count > 0, true);

        }


        [TestMethod]
        public void TestMethod2_01()
        {
            var updateResult = DB.Tables.People.Where(r => r.Name.Equals("yuzd")).DynamicOrderBy("Id", "desc").Skip(1).Take(1).ToList();
            Assert.AreEqual(updateResult.Count > 0, true);

        }

        [TestMethod]
        public void TestMethod2_02()
        {
            var list = (from p in DB.Tables.People
                        join s in DB.Tables.Schools on p.SchoolId equals s.Id
                        select p).ToList();

            Assert.AreEqual(list.Count,2);
        }

        [TestMethod]
        public void TestMethod2_03()
        {
            var list = (from p in DB.Tables.People
                        from s in DB.Tables.Schools.Where(r=>r.Id.Equals(p.SchoolId)).DefaultIfEmpty()
                        select p).ToList();

            Assert.AreEqual(list.Count > 2, true);
        }

        [TestMethod]
        public void TestMethod2_04()
        {
            var p = DB.Tables.People.LoadWith(r=>r.Personsschool).FirstOrDefault(r => r.Name.Equals("nainaigu"));

            Assert.IsNotNull(p);
            Assert.IsNotNull(p.Personsschool);
            Assert.IsNotNull(p.Personsschool.Name);
        }

        [TestMethod]
        public void TestMethod2_05()
        {
            var p = DB.Tables.Schools.LoadWith(r => r.Persons).FirstOrDefault(r => r.Name.Equals("北京大学"));

            Assert.IsNotNull(p);
            Assert.IsNotNull(p.Persons);
            Assert.IsFalse(p.Persons.Any());
        }
    }
}
