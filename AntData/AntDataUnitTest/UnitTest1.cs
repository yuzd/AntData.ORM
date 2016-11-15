using System;
using System.Diagnostics;
using System.Linq;
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
        public void TestMethod1()
        {
            var db = new MysqlContext<Entitys>("iworkcloudbagdb");
            db.OnCustomerTraceConnection = OnCustomerTraceConnection;
            var aa = db.Tables.Accounts.ToList();
        }

        private void OnCustomerTraceConnection(CustomerTraceInfo customerTraceInfo)
        {
            Debug.WriteLine("");
        }
    }
}
