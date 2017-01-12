using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using AntData.ORM.Data;
using AntData.ORM.DataProvider.SqlServer;
using SqlSugar;
using Dapper;
using Dapper.Contrib;
using Dapper.Contrib.Extensions;
using DbModels.SqlServer;
using Test = Models.PkDapper.Test;

namespace PkDapper.Demos
{
    public class WarmUp
    {
        public WarmUp()
        {
            Console.WriteLine("开启预热");
            //预热处理
            for (int i = 0; i < 2; i++)
            {
                var DB = new DbContext<Entitys>("testorm_sqlserver", new SqlServerDataProvider(SqlServerVersion.v2008));
                var aa = DB.Tables.Tests.FindByBk(1000);
            }
            Console.WriteLine("预热完毕");
            Console.WriteLine("----------------比赛开始-------------------");
        }
    }
}
