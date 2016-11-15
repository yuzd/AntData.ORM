using System;
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
        [TestMethod]
        public void Method_01()
        {

            using (var DbContent = new MysqlContext<Entitys>("hangfire"))
            {
                DbContent.OnCustomerTraceConnection = info =>
                {
                    var sqlText = info.SqlText;
                    var parms = info.CustomerParams;
                };
                string aaa = "3";
                var aa = DbContent.Tables.Jobs.Where(r => r.StateName.Equals(aaa)).Skip(1).Take(2).ToList();
                Assert.AreEqual(aa.Count > 0, true);
            }
        }


        [TestMethod]
        public void Method_02()
        {

            using (var DbContent = new MysqlContext<Entitys>("hangfire"))
            {
                DbContent.OnCustomerTraceConnection = info =>
                {
                    var sqlText = info.SqlText;
                    var parms = info.CustomerParams;
                };
                string staffNumber = "yuzd1";
                var aa = (from u in DbContent.Tables.Jobs
                          join bb in DbContent.Tables.Jobstates on u.Id equals  bb.JobId
                          where bb.Name.Equals(staffNumber)
                          select u)
                          .ToList();
                Assert.AreEqual(aa.Count > 0, true);
            }
        }


        [TestMethod]
        public void Method_03()
        {

            using (var DbContent = new MysqlContext<Entitys>("hangfire"))
            {
                DbContent.CommandTimeout = 500;
                DbContent.OnCustomerTraceConnection = info =>
                {
                    var sqlText = info.SqlText;
                    var parms = info.CustomerParams;
                };
                string staffNumber = "yuzd1";
                var aa = (from u in DbContent.Tables.Jobs
                          from bb in DbContent.Tables.Jobstates.Where(r => u.Id.Equals(r.JobId)).DefaultIfEmpty()
                          where bb.Name.Equals(staffNumber)
                          select u)
                          .ToList();
                Assert.AreEqual(aa.Count > 0, true);
            }
        }

        [TestMethod]
        public void Method_04()
        {
            using (var DbContent = new MysqlContext<Entitys>("hangfire"))
            {
                DbContent.OnCustomerTraceConnection = info =>
                {
                    var sqlText = info.SqlText;
                    var parms = info.CustomerParams;
                };
                string staffNumber = "yuzd1";
                var updateResult = DbContent.Tables.Jobstates.Where(r => r.Name.Equals(staffNumber))
                                                                 .Set(r => r.CreatedAt, DateTime.Now)
                                                                 .Update() > 0;
                Assert.AreEqual(updateResult, true);

            }
        }

        [TestMethod]
        public void Method_05()
        {

            using (var DbContent = new MysqlContext<Entitys>("hangfire"))
            {
                DbContent.OnCustomerTraceConnection = info1 =>
                {
                    var sqlText = info1.SqlText;
                    var parms = info1.CustomerParams;
                };
                Job info = new Job()
                {
                 
                };
                var insetResult = DbContent.Insert(info);
                var result = DbContent.Tables.Jobs.FirstOrDefault(r => r.StateName.Equals("aaaaaaaaa"));
                Assert.IsNotNull(result);
                Assert.AreEqual(result.StateName, "ddada");
            }
        }

        [TestMethod]
        public void Method_06()
        {

            using (var DbContent = new MysqlContext<Entitys>("hangfire"))
            {

                var result = DbContent.Tables.Jobs.Where(r => r.StateName.Equals("aaaaaaaaa")).Delete() > 0;
                Assert.AreEqual(result, true);
            }

        }

        public class MyClass
        {
            public string StaffCode { get; set; }

            public string StaffName { get; set; }
        }

        [TestMethod]
        public void Method_07()
        {

            using (var DbContent = new MysqlContext<Entitys>("hangfire"))
            {

                var result =
                    DbContent.Tables.Jobstates.Select(r => new MyClass
                    {
                        StaffCode = r.Name,
                        StaffName = r.Reason
                    }).ToList();

                Assert.AreEqual(result.Count > 0, true);
            }

        }

        [TestMethod]
        public void Method_08()
        {

            using (var DbContent = new MysqlContext<Entitys>("hangfire"))
            {

                var result =
                    DbContent.Tables.Jobstates.Select(r => new
                    {
                        StaffCode = r.Name,
                        StaffName = r.Reason
                    }).ToList();
                Assert.AreEqual(result.Count > 0, true);
            }

        }

        [TestMethod]
        public void Method_09()
        {

            using (var DbContent = new MysqlContext<Entitys>("hangfire"))
            {

                var result = DbContent.Query<Job>("select * from job").ToList();

                Assert.AreEqual(result.Count > 0, true);
            }
        }

        [TestMethod]
        public void Method2_01()
        {
            using (var DbContent = new MysqlContext<Entitys>("hangfire"))
            {

                var result = DbContent.Query<MyClass>("select staff_number StaffName,staff_code StaffCode from job").ToList();

                Assert.AreEqual(result.Count > 0, true);
            }
        }


        [TestMethod]
        public void Method2_02()
        {
            using (var DbContent = new MysqlContext<Entitys>("hangfire"))
            {

                var updateResult = DbContent.Tables.Jobs.Where(r => r.StateName.Equals("yuzd1"))
                                                                 .Set2("Productline", "9")
                                                                 .Update() > 0;

                Assert.AreEqual(updateResult, true);
            }
        }

        [TestMethod]
        public void Method2_03()
        {
            using (var DbContent = new MysqlContext<Entitys>("hangfire"))
            {
                DbContent.OnCustomerTraceConnection = info1 =>
                {
                    var sqlText = info1.SqlText;
                    var parms = info1.CustomerParams;
                };
                var list = DbContent.Tables.Jobs.Where(r => r.StateName.Equals("yuzd1"))
                                                                 .DynamicOrderBy("ProductLine", "desc").ToList();

                Assert.AreEqual(list.Count > 0, true);
            }
        }


        [TestMethod]
        public void Method2_04()
        {
            using (var DbContent = new MysqlContext<Entitys>("hangfire"))
            {
                DbContent.OnCustomerTraceConnection = info1 =>
                {
                    var sqlText = info1.SqlText;
                    var parms = info1.CustomerParams;
                };
                string sql =
                    "select staff_number StaffName,staff_code StaffCode from chat_staff_info where staff_number = @staff_number";
                var result = DbContent.Query<MyClass>(sql, new DataParameter("staff_number", "yuzd1")).ToList();

                Assert.AreEqual(result.Count > 0, true);
            }
        }


        [TestMethod]
        public void Method2_05()
        {
            using (var DbContent = new MysqlContext<Entitys>("hangfire"))
            {
                try
                {
                    DbContent.UseTransaction(con =>
                    {
                        Jobstate info = new Jobstate()
                        {
                         
                        };

                        var insetResult = con.Insert(info);

                        var delete = con.Tables.Jobstates.Where(r => r.Name.Equals("ddada")).Delete();
                        // error in update
                        var update = con.Tables.Jobstates.Where(r => r.Name.Equals("yuzd1")).Set2("productline", "13").Update() > 0;
                    });

                }
                catch (Exception)
                {

                    throw;
                }
            }
        }

        [TestMethod]
        public void Method2_06()
        {
            using (var DbContent = new MysqlContext<Entitys>("hangfire"))
            {
                try
                {
                    DbContent.OnCustomerTraceConnection = info1 =>
                    {
                        var sqlText = info1.SqlText;
                        var parms = info1.CustomerParams;
                    };
                    Jobstate info = new Jobstate
                    {
                       
                    };

                    var index = DbContent.InsertWithIdentity(info);

                    DbContent.Tables.Jobstates.Where(r => r.Id > 33).Delete();

                }
                catch (Exception)
                {

                    throw;
                }
            }
        }

        [TestMethod]
        public void Method2_07()
        {
            using (var DbContent = new MysqlContext<Entitys>("hangfire"))
            {
                DbContent.OnCustomerTraceConnection = info1 =>
                {
                    var sqlText = info1.SqlText;
                    var parms = info1.CustomerParams;
                };
                string sql =
                    "select staff_number StaffName,staff_code StaffCode from chat_staff_info where staff_number = @staff_number";
                var result = DbContent.Query(new { StaffName = "", StaffCode = "" }, sql, new DataParameter("staff_number", "yuzd1")).ToList();

                Assert.AreEqual(result.Count > 0, true);
            }
        }

        [TestMethod]
        public void Method2_08()
        {
            using (var DbContent = new MysqlContext<Entitys>("hangfire"))
            {
                DbContent.OnCustomerTraceConnection = info1 =>
                {
                    var sqlText = info1.SqlText;
                    var parms = info1.CustomerParams;
                };
                string sql =
                    "select staff_number StaffName,staff_code StaffCode from chat_staff_info where staff_number = @staff_number";
                var result = DbContent.QueryTable(sql, new DataParameter("staff_number", "yuzd1"));

                Assert.AreEqual(result.Rows.Count > 0, true);
            }
        }
    }
}
