using System;
using System.Diagnostics;
using System.Linq;
using AntData.ORM.Data;
using AntData.ORM.DataProvider.SqlServer;
using DbModels.SqlServer;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AntDataUnitTest
{
    [TestClass]
    public class UnitTest2
    {
        private static DbContext<Entitys> DB;
        [ClassInitialize()]
        public static void MyClassInitialize(TestContext testContext)
        {
            DB = new DbContext<Entitys>("testorm_sqlserver", new SqlServerDataProvider(SqlServerVersion.v2008));
            DB.OnCustomerTraceConnection = OnCustomerTraceConnection;
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
            var users = DB.Tables.QUsers.FirstOrDefault();
            Assert.IsNotNull(users);
        }
    }
}
