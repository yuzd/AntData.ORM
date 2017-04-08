using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using AntData.ORM;
using AntData.ORM.Data;
using AntData.ORM.DataProvider.MySql;
using AntData.ORM.Linq;
using AntData.ORM.Mapping;
using DbModels.Mysql;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MySql.Data.MySqlClient;

namespace AntDataUnitTest
{
    [TestClass]
    public class UnitTestForMysql
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

        [TestMethod]
        public void TestMethod1_00()
        {
            if (!DB.Tables.Schools.Any())
            {
                List<School> sList = new List<School>
                {
                    new School
                    {
                        Name = "上海大学",
                        Address = "上海"
                    },
                    new School
                    {
                        Name = "北京大学",
                        Address = "北京"
                    }
                };

                DB.BulkCopy(sList);
            }

            List<Person> pList = new List<Person>
            {
                new Person
                {
                    Name = "yuzd",
                    Age = 27,
                    SchoolId = 1
                },
                new Person
                {
                    Name = "nainaigu",
                    Age = 18,
                    SchoolId = 2
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
            Debug.WriteLine(p.Name);
        }

        [TestMethod]
        public void TestMethod1_05()
        {
            Person p = new Person
            {
                Name = "yuzd",
                Age = 27,
                SchoolId = 1
            };

            var insertResult = DB.InsertWithIdentity(p);
            var id = Convert.ToInt64(insertResult);
            Assert.AreEqual(id > 0, true);
            Assert.AreEqual(p.Id, id);

        }

        [TestMethod]
        public void TestMethod1_06()
        {
            var updateResult =
                DB.Tables.People.Where(r => r.Name.Equals("yuzd")).Set2<Person, int?>("Age", 20).Update() > 0;
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
            var updateResult =
                DB.Tables.People.Where(r => r.Name.Equals("yuzd")).OrderByDescending(r => r.Id).Skip(1).Take(1).ToList();
            Assert.AreEqual(updateResult.Count > 0, true);

        }


        [TestMethod]
        public void TestMethod2_01()
        {
            var updateResult = DB.Tables.People.Where(r => r.Name.Equals("yuzd")).OrderBy("Id").Skip(1).Take(1).ToList();
            Assert.AreEqual(updateResult.Count > 0, true);

        }

        [TestMethod]
        public void TestMethod2_02()
        {
            var list = (from p in DB.Tables.People
                join s in DB.Tables.Schools on p.SchoolId equals s.Id
                select p).ToList();

            Assert.IsTrue(list.Count > 0);
        }

        [TestMethod]
        public void TestMethod2_03()
        {
            var list = (from p in DB.Tables.People
                from s in DB.Tables.Schools.Where(r => r.Id.Equals(p.SchoolId)).DefaultIfEmpty()
                select p).ToList();

            Assert.IsTrue(list.Count > 0);
        }

        [TestMethod]
        public void TestMethod2_04()
        {
            var p = DB.Tables.People.LoadWith(r => r.Personsschool).FirstOrDefault(r => r.Name.Equals("nainaigu"));
            
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
            Assert.IsTrue(p.Persons.Any());
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
            var raa = await DB.Tables.People.FirstOrDefaultAsync(r => r.Name.Equals(name));
            Assert.IsNotNull(raa);

        }

        [TestMethod]
        public async Task TestMethod2_08()
        {
            var name = "yuzd";
            var raa = await DB.Tables.People.Where(r => r.Name.Equals(name)).ToListAsync();
            Assert.AreEqual(raa.Count > 0, true);

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
            var list =
                DB.Query<MyClass>("select * from person where name=@name",
                    new DataParameter {Name = "name", Value = name}).ToList();
            Assert.IsNotNull(list);
            Assert.AreEqual(list.Count > 0, true);
        }

        [TestMethod]
        public async Task TestMethod3_02()
        {
            var name = "yuzd";
            var age = 20;
            var name2 = "nainaigu";
            var age2 = 18;
            var list =
                DB.Query<MyClass>("select * from person where name=@name and age=@age",
                        new DataParameter {Name = "name", Value = name}, new DataParameter {Name = "age", Value = age})
                    .ToListAsync();
            var list2 =
                DB.Query<MyClass>("select * from person where name=@name and age=@age",
                        new DataParameter {Name = "name", Value = name2}, new DataParameter {Name = "age", Value = age2})
                    .ToListAsync();
            var list3 =
                DB.Query<MyClass>("select * from person where name=@name",
                    new DataParameter {Name = "name", Value = name}).ToListAsync();
            Assert.AreEqual((await list).Count > 0, true);
            Assert.AreEqual((await list2).Count > 0, true);
            Assert.AreEqual((await list3).Count > 0, true);
        }

        [TestMethod]
        public void TestMethod3_04()
        {
            var name = "yuzd";
            var list = DB.Query(new MyClass(), "select * from person where name=@name",
                new DataParameter {Name = "name", Value = name}).ToList();
            Assert.IsNotNull(list);
            Assert.AreEqual(list.Count > 0, true);
        }

        [TestMethod]
        public void TestMethod3_05()
        {
            var name = "yuzd";
            var list = DB.Query(new {Id = 0, Name = "", Age = 0}, "select * from person where name=@name",
                new DataParameter {Name = "name", Value = name}).ToList();
            Assert.IsNotNull(list);
            Assert.AreEqual(list.Count > 0, true);
        }

        [TestMethod]
        public void TestMethod3_06()
        {
            var name = "yuzd";
            SQL sql = "select * from person where name=@name";
            sql = sql["name", name];
            var list = DB.Query<MyClass>(sql).ToList();
            Assert.IsNotNull(list);
            Assert.AreEqual(list.Count > 0, true);
        }

        [TestMethod]
        public async Task TestMethod3_07()
        {
            var name = "yuzd";
            SQL sql = "select * from person where name=@name";
            sql = sql["name", name];
            var list = await DB.Query<MyClass>(sql).ToListAsync();
            Assert.IsNotNull(list);
            Assert.AreEqual(list.Count > 0, true);

        }


        [TestMethod]
        public void TestMethod3_09()
        {
            var name = "yuzd";
            var age = 20;
            SQL sql = "select count(*) from person where 1=1";
            if (!string.IsNullOrEmpty(name))
            {
                sql += " and name = @name";
                sql = sql["name", name];
            }
            if (age > 0)
            {
                sql += " and age = @age";
                sql = sql["age", age];
            }

            var list = DB.Execute<long>(sql);
            Assert.IsNotNull(list);
            Assert.AreEqual(list > 0, true);

        }

        [TestMethod]
        [ExpectedException(typeof(MySqlException))]
        public void TestMethod4_01()
        {
            AntData.ORM.Common.Configuration.Linq.IgnoreNullInsert = false;
            Person p = new Person
            {
                Age = 27
            };

            DB.UseTransaction(con =>
            {
                con.Tables.Schools.Where(r => r.Name.Equals("上海大学")).Set(r => r.Address, "no update").Update();
                con.Insert(p);
                return true;
            });

        }


        [TestMethod]
        public void TestMethod4_02()
        {
            var p = DB.Tables.People.FindByBk(69);
            Assert.IsNotNull(p);
            var s = DB.Tables.Schools.FindByBk(1);
            Assert.IsNotNull(s);
        }

        [TestMethod]
        public async Task TestMethod4_03()
        {

            // var aa = DB.Tables.People.Where(r => r.Age > 10).Where(r=>r.Age < 20).ToList();
            var bb = DB.Tables.People.Where("age > 10").Where(r => r.Age < 20).ToListAsync();
            var bbb = DB.Tables.People.Where(r => r.Age < 20).Where("age > 10").ToListAsync();
            Assert.IsTrue((await bb).Count == (await bbb).Count);
        }

        [TestMethod]
        public void TestMethod4_04()
        {
            var age = 10;
            //var bb = DB.Tables.People.Where(r=>r.Age> age).ToList();
            var bb = DB.Tables.People.Where("age > @age", new {age = age}).ToList();
            var bbb = DB.Tables.People.Where("age > @age and name = @name", new {age = age, name = "nainaigu"}).ToList();
            ;
            var bbbd =
                DB.Tables.People.Where("age > @age", new {age = age})
                    .Where("name = @name", new {name = "nainaigu"})
                    .ToList();
            var bbbc =
                DB.Tables.People.Where("age > @age and name = @name", new {age = age, name = "nainaigu"})
                    .Where(r => r.Name.Equals("aaa"))
                    .ToList();
            var bbbcc =
                DB.Tables.People.Where(r => r.SchoolId.Equals(2))
                    .Where("age > @age", new {age = age})
                    .Where(r => r.Name.Equals("nainaigu"))
                    .ToList();
        }

        [TestMethod]
        public void TestMethod4_05()
        {
            var age = 10;
            //、var bb = DB.Tables.People.Where(r=>r.Age> age).ToList();
            var bb =
                DB.Tables.People.Where(r => r.Age > age)
                    .Where("age > @age", new {age = age})
                    .Select(r => r.Name)
                    .ToList();
        }

        [TestMethod]
        public void TestMethod4_06()
        {
            var age = 10;

            var list = (from p in DB.Tables.People
                    join s in DB.Tables.Schools on p.SchoolId equals s.Id
                    select new {Name = p.Name, SchoolName = s.Name, Age = p.Age}).Where(r => r.Age > age)
                .Where("school.name = @name", new {name = "nainaigu"})
                .Where("person.name = @name", new {name = "nainaigu"})
                .ToList();
        }

        [TestMethod]
        [ExpectedException(typeof(LinqException))]
        public void TestMethod4_07()
        {
            var age = 10;
            //、var bb = DB.Tables.People.Where(r=>r.Age> age).ToList();
            var bb =
                DB.Tables.People.Where(r => r.Age > age)
                    .Where("person.age2 > @age", new {age = age})
                    .Select(r => r.Name)
                    .ToList();
        }

        [TestMethod]
        [ExpectedException(typeof(LinqException))]
        public void TestMethod4_08()
        {
            var age = 10;
            //、var bb = DB.Tables.People.Where(r=>r.Age> age).ToList();
            var bb =
                DB.Tables.People.Where(r => r.Age > age).Where("age > @age", new {age = age}).OrderBy("aa").ToList();
        }

        [TestMethod]
        public void TestMethod4_09()
        {
            var age = 10;
            //、var bb = DB.Tables.People.Where(r=>r.Age> age).ToList();
            var bb =
                DB.Tables.People.Where(r => r.Age > age)
                    .Where("age > @age", new {age = age})
                    .OrderBy("age", "name")
                    .ToList();
        }

        [TestMethod]
        public void TestMethod5_01()
        {
            var age = 10;
            //、var bb = DB.Tables.People.Where(r=>r.Age> age).ToList();
            var bb =
                DB.Tables.People.Where(r => r.Age > age)
                    .Where("age > @age", new {age = age})
                    .OrderByMultiple("age desc, name asc")
                    .ToList();
        }

        [TestMethod]
        public void TestMethod5_02()
        {
            Person p = new Person
            {
                Name = "yuzd",
                Age = 27,
                SchoolId = 1
            };

            var insertResult = DB.InsertWithIdentity<Person, long>(p);
            Assert.AreEqual(insertResult > 0, true);
            Assert.AreEqual(p.Id, insertResult);

        }

        [TestMethod]
        public void TestMethod5_03()
        {
            var bb = DB.Tables.People.OrderByDescending("name", "age").ToList();
        }

        [TestMethod]
        public void TestMethod5_04()
        {
            var bb = DB.Tables.People.GroupBy("name").Select(r => r.Key).ToList();
            var bbb = DB.Tables.People.GroupBy(r => new {r.Name, r.Age}).Select(r => r.ToList()).ToList();
            var bbbbb = DB.Tables.People.GroupBy(r => r.Name).Select(r => r.Key).ToList();
        }

        [TestMethod]
        public void TestMethod5_05()
        {
            //var expression = AntData.ORM.Expressions.Extensions.GenerateMemberExpression<Person, string>("name");
            var bbb = DB.Tables.People.GroupBy(r => r.Name).Select(r => new {Value = r.ToList()}).ToList();

            var bb = DB.Tables.People.GroupBy(r => r.Age).Select(r => new {Key = r.Key, Value = r.ToList()}).ToList();

        }

        [TestMethod]
        public void TestMethod5_06()
        {
            var name = "yuzd";
            var list = DB.Query<Person>("select * from person where name=@name", new {name = name}).ToList();
            Assert.IsNotNull(list);
            Assert.AreEqual(list.Count > 0, true);
        }



        [TestMethod]
        public void TestMethod5_07()
        {
            AntData.ORM.Common.Configuration.Linq.IgnoreNullInsert = true;
            Person p = new Person
            {
                Name = "yuzd",
                Age = 27,
                SchoolId = 1
            };

            var insertResult = DB.InsertWithIdentity<Person, long>(p);
            Assert.AreEqual(insertResult > 0, true);
            Assert.AreEqual(p.Id, insertResult);

            p = new Person
            {
                Name = null,
                Age = 11
            };
            DB.Insert(p);
            DB.InsertWithIdentity(p);

        }

        [TestMethod]
        public async Task TestMethod5_08()
        {
            Person p = new Person
            {
                Name = "yuzd",
                Age = 27
            };

            var insertResult = await DB.InsertAsync(p) > 0;
            Assert.AreEqual(insertResult, true);

        }

        [TestMethod]
        public async Task TestMethod5_09()
        {
            Person p = new Person
            {
                Name = "yuzd",
                Age = 27
            };

            var insertResult = await DB.InsertWithIdentityAsync(p);
            Debug.WriteLine(p.Id);

        }


        [TestMethod]
        public void TestMethod6_01()
        {

            Person p = new Person
            {
                Name = null,
                Age = 27
            };

            var insertResult = DB.Insert(p);

            p.Name = "yuzd";
            var insertResult2 = DB.Insert(p);

            p.Name = null;
            var insertResult3 = DB.Insert(p);

            Debug.WriteLine(p.Id);

        }


        [TestMethod]
        public async Task TestMethod6_02()
        {
            var p =
                await DB.Tables.People.LoadWith(r => r.Personsschool)
                    .FirstOrDefaultAsync(r => r.Name.Equals("nainaigu"));

            Assert.IsNotNull(p);
            Assert.IsNotNull(p.Personsschool);
            Assert.IsNotNull(p.Personsschool.Name);
        }

        [TestMethod]
        public async Task TestMethod6_03()
        {
            var p = await DB.Tables.Schools.LoadWith(r => r.Persons).FirstOrDefaultAsync(r => r.Name.Equals("北京大学"));

            Assert.IsNotNull(p);
            Assert.IsNotNull(p.Persons);
            Assert.IsTrue(p.Persons.Any());
        }

        [TestMethod]
        public void TestMethod6_04()
        {
            var name = "yuzd";
            var age = 20;
            var paramList = new List<DataParameter>();
            paramList.Add(new DataParameter {Name = "name", Value = name});
            paramList.Add(new DataParameter {Name = "age", Value = age});
            var list = DB.Query<MyClass>("select * from person where name=@name and age=@age", paramList).ToList();

        }

        [TestMethod]
        public void TestMethod6_05()
        {
            var p = from p1 in DB.Tables.People
                from p2 in DB.Tables.Schools.Where(r => r.Id.Equals(p1.SchoolId)).DefaultIfEmpty()
                select StaticMethod.BuildPerson(p1, p2);

            var re = p.ToList();


        }


        [TestMethod]
        public void TestMethod6_06()
        {
            AntData.ORM.Common.Configuration.Linq.CheckNullForNotEquals = false;
            var p1 = DB.Tables.People.Where(r => r.SchoolId == null || r.SchoolId.Value != 1).ToList();
            AntData.ORM.Common.Configuration.Linq.CheckNullForNotEquals = true;
            var p2 = DB.Tables.People.Where(r => r.SchoolId != 1).ToList();
            Assert.AreEqual(p1.Count, p2.Count);
        }

        [TestMethod]
        public void TestMethod6_07()
        {
            var updateResult = DB.Tables.People.Where(r => r.Name.Equals("nainaigu")).DynamicOrderBy("name", "desc").ToList();

        }

        [TestMethod]
        public void TestMethod6_08()
        {
            List<School> sList = new List<School>
                {
                    new School
                    {
                        Name = "上海大学",
                        Address = "上海"
                    },
                    new School
                    {
                        Name = "北京大学",
                        Address = "北京"
                    }
                };

            var rows =  DB.BulkCopy(sList);
            Assert.AreEqual(rows.RowsCopied,2);
        }

        //[TestMethod]
        //public void TestMethod6_09()
        //{
        //    Person p = new Person
        //    {
        //        Age = 27,
        //    };

        //    DB.UseTransaction1(con =>
        //    {
        //        con.Tables.Schools.Where(r => r.Id == 1).Set(r => r.Address, "no update1").Update();
        //        con.Insert(p);
        //    });

         
        //}
    }
}
