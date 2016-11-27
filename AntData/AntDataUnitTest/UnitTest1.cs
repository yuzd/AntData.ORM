using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
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
            string name = "yuzd";
            var updateResult = DB.Tables.People.Where(r => r.Name.Equals(name)).OrderBy(r => r.Id).ToList();
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

            Assert.AreNotEqual(list.Count,2);
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

        [TestMethod]
        public async Task TestMethod2_06()
        {
            var name = "yuzd";
            var raa = DB.Tables.People.Where(r => r.Name.Equals(name));
            var aa = raa.Where(r => r.Age.Equals(10));
            var a1 = raa.CountAsync();
            var a2 = aa.CountAsync();
            var a11 = await a1;
            var a12 = await a2;

        }

        [TestMethod]
        public async Task TestMethod2_07()
        {
            var name = "yuzd";
            var raa = await  DB.Tables.People.FirstOrDefaultAsync(r => r.Name.Equals(name));
            Assert.IsNotNull(raa);

        }

        [TestMethod]
        public async Task TestMethod2_08()
        {
            var name = "yuzd";
            var raa = await DB.Tables.People.Where(r => r.Name.Equals(name)).ToListAsync();
            Assert.AreEqual(raa.Count>0,true);

        }

        [TestMethod]
        public async Task TestMethod2_09()
        {
            var name = "yuzd";
            var list = await (from p in DB.Tables.People
                        join s in DB.Tables.Schools on p.SchoolId equals s.Id
                        select p).ToListAsync();
            Assert.AreEqual(list.Count > 0, true);

        }

        public class MyClass
        {
            public string Name { get; set; }
            public int Age { get; set; }
        }

        [TestMethod]
        public void TestMethod3_01()
        {
            var name = "yuzd";
            var list = DB.Query<MyClass>("select * from person where name=@name",new DataParameter {Name = "name",Value = name}).ToList();
            Assert.IsNotNull(list);
            Assert.AreEqual(list.Count>0,true);
        }

        [TestMethod]
        public async Task TestMethod3_02()
        {
            var name = "yuzd";
            var list = await DB.Query<MyClass>("select * from person where name=@name", new DataParameter { Name = "name", Value = name }).ToListAsync();
            Assert.IsNotNull(list);
            Assert.AreEqual(list.Count > 0, true);
        }
    }
}
