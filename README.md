![](https://img.shields.io/badge/platform-dotnet-red.svg) ![](https://img.shields.io/badge/language-CSharp-orange.svg) 
![](https://img.shields.io/badge/license-MIT%20License-brightgreen.svg) 
[![Support](https://img.shields.io/badge/support-NetCore-blue.svg?style=flat)](https://www.microsoft.com/net/core) 
[![Weibo](https://img.shields.io/badge/博客园-@鱼东东-yellow.svg?style=flat)](http://www.cnblogs.com/yudongdong) 
[![GitHub stars](https://img.shields.io/github/stars/yuzd/AntData.ORM.svg)](https://github.com/yuzd/AntData.ORM/stargazers)
[![NuGet Status](https://img.shields.io/nuget/v/GoogleMapsApi.svg)](https://www.nuget.org/packages/AntData.Core/)

# AntData.ORM Is Fork from [Linq2db](https://github.com/linq2db/linq2db) And CtripDal
.Read-write separation
.Sharding By DB And Sharding By Table

# NUGET FOR .NET

SqlServer：Install-Package AntData.ORM

Mysql：Install-Package AntData.ORM.Mysql

Oracle: Install-Package AntData.ORM.Oracle


# NUGET FOR Dotnetcore2.0

SqlServer : Install-Package AntData.Core

Mysql：Install-Package AntData.Core.Mysql

Postgre：Install-Package AntData.Core.Postgre

QQ Group ：774590645

[如何配置tt模板自动生成entity以及常见的问题(文章末尾会有最新的tt文件包)](http://www.cnblogs.com/yudongdong/p/6421312.html)


[VS插件生成Db model Class](https://marketplace.visualstudio.com/items?itemName=nainaigu.AntDataVS)


# DEMO

[Dotnetcore Demo](http://www.cnblogs.com/yudongdong/p/6427432.html)

# Instance  DbContext

```csharp
//使用DB这个对象一定要确保每次都是一个新的实例 例如像下面的这种写法是安全的
private static DbContext<Entitys> DB
 {
        get
        {
            var db = new MysqlDbContext<Entitys>("testorm");
			//Trance Sql Log
            db.IsEnableLogTrace = true;
            db.OnLogTrace = OnCustomerTraceConnection;
			//如果是sqlserver的话 可以设置下面
			//db.IsNoLock = true;
            return db;
        }
  }


private static void OnCustomerTraceConnection(CustomerTraceInfo customerTraceInfo)
{
	try
	{
		//因为是防止sql注入的 所以SqlText指的orm给你生成的sql。CustomerParams是执行值传给数据库驱动的参数 
		string sql = customerTraceInfo.CustomerParams.Aggregate(customerTraceInfo.SqlText,
				(current, item) => current.Replace(item.Key, item.Value.Value.ToString()));
		Debug.Write(sql + Environment.NewLine);
	}
	catch (Exception)
	{
		//ignore 有可能会失败
		Debug.Write(sql + Environment.NewLine);
	}
}

//netcore2的建议大家去netcore2分支下载对应的demo
```


# 1.Select 

##### 1.1 queryable
```csharp

//FirstOrDefault
var p = DB.Tables.People.FirstOrDefault();
var p = await DB.Tables.People.FirstOrDefaultAsync();
var p = DB.Tables.People.FirstOrDefault(r=>r.Name.Equals("nainaigu"));
var p = await DB.Tables.People.FirstOrDefaultAsync(r=>r.Name.Equals("nainaigu"));

//select single
var p = DB.Tables.People.Single(r=>r.Name.Equals("nainaigu"));
var p = await DB.Tables.People.SingleAsync(r=>r.Name.Equals("nainaigu"));
var p = DB.Tables.People.SingleOrDefault(r=>r.Name.Equals("nainaigu"));
var p = await DB.Tables.People.SingleOrDefaultAsync(r=>r.Name.Equals("nainaigu"));

//select single by primarykey
var p = DB.Tables.People.FindByBk(1);

//select first
var p = DB.Tables.People.Where(r=>r.Age>10).First();
var p = await DB.Tables.People.Where(r=>r.Age>10).FirstOrDefault();

//between 11 and 20
var page1 = DB.Tables.People.Where(r => r.Name.Equals("yuzd")).Skip(11).Take(20).ToList();
var page1 = await DB.Tables.People.Where(r => r.Name.Equals("yuzd")).Skip(11).Take(20).ToListAsync();

//get count
var count = DB.Tables.People.Where(r => r.Name.Equals("yuzd")).Count();
var count = await DB.Tables.People.Where(r => r.Name.Equals("yuzd")).CountAsync();
var p = DB.Tables.People.Where(r=>r.Age>10).LongCount();
var p = await DB.Tables.People.Where(r=>r.Age>10).LongCountAsync();

//skip 2
var skip = DB.Tables.People.Where(c => c.Age > 10).OrderBy(it => it.Age).Skip(2).ToList();

//take 2
var take = DB.Tables.People.Where(c => c.Age > 10).OrderBy(it => it.Age).Take(2).ToList();

//Not like 
string conval = "a";
var notLike = DB.Tables.People.Where(c => !c.Name.Contains(conval)).ToList();

//Like
var conval = "三";
var like = DB.Tables.People.Where(c => c.Name.Contains(conval)).ToList();

//where sql string
var age = 10;
var b = DB.Tables.People.Where("age > 10").ToList();
var bb = DB.Tables.People.Where("age > @age",new {age= age }).ToList();
var bbb = DB.Tables.People.Where("age > @age and name = @name", new { age = age ,name= "nainaigu" }).ToList();;
var bbbb = DB.Tables.People.Where("age > @age", new { age = age }).Where("name = @name",new {name="nainaigu"}).ToList();
var bbbbb = DB.Tables.People.Where("age > @age and name = @name", new { age = age ,name= "nainaigu" }).Where(r=>r.Name.Equals("aaa")).ToList();
var bbbbbb = DB.Tables.People.Where(r=>r.SchoolId.Equals(2)).Where("age > @age", new { age = age }).Where(r => r.Name.Equals("nainaigu")).ToList();
var list = (from p in DB.Tables.People
            join s in DB.Tables.Schools on p.SchoolId equals s.Id
            select new {Name = p.Name,SchoolName = s.Name,Age = p.Age}
			).Where(r=>r.Age > age)
			 .Where("school.name = @name", new { name = "nainaigu" })
			 .Where("person.name = @name", new { name = "nainaigu" })
			 .ToList();

//is any
bool isAny100 = DB.Tables.People.Any(c => c.Id == 100);


//get max Age
var p = DB.Tables.People.Max(r=>r.Age);
var p = await DB.Tables.People.MaxAsync(r=>r.Age);

//get min Age
var p = DB.Tables.People.Min(r=>r.Age);
var p = await  DB.Tables.People.MinAsync(r=>r.Age);


//order By 
var age = 10;
var bb = DB.Tables.People.Where(r => r.Age > age).Where("age > @age", new { age = age }).OrderBy("age","name").ToList();
var bb = DB.Tables.People.Where(r => r.Age > age).Where("age > @age", new { age = age }).OrderByDescending("age","name").ToList();
var bb = DB.Tables.People.Where(r => r.Age > age).Where("age > @age", new { age = age }).OrderByMultiple("age desc, name asc").ToList();

//Page
var bb = DB.Tables.People.Where(r => r.Name.Equals("yuzd")).OrderBy("Id").Skip(1).Take(10).ToList();
var bb = DB.Tables.People.Where(r => r.Name.Equals("yuzd")).OrderBy(r => r.Id).Skip(1).Take(10).ToList();
var bb = DB.Tables.People.Where(r => r.Name.Equals("yuzd")).OrderByDescending("Id").Skip(1).Take(10).ToList();
var bb = DB.Tables.People.Where(r => r.Name.Equals("yuzd")).OrderByDescending(r => r.Id).Skip(1).Take(10).ToList();

//In
var arr = new []{ "nainaigu","yuzd" };
var listnew = DB.Tables.People.Where(r => arr.Contains(r.Name)).ToList();

//group by
var b = DB.Tables.People.GroupBy("name").Select(r=>r.Key).ToList();
var bb = DB.Tables.People.GroupBy(r => r.Name).Select(r => r.Key).ToList();
var bbb = DB.Tables.People.GroupBy(r=> new {r.Name,r.Age}).Select(r=>r.ToList()).ToList();
var bbbb = DB.Tables.People.GroupBy(r=> r.Age ).Select(r =>new {Key =r.Key,Value = r.ToList()}).ToList();
var bbbbb = DB.Tables.People.GroupBy(r => r.Name).Select(r => new {  Value = r.ToList() }).ToList();


//inner join
var list = (from p in DB.Tables.People
            join s in DB.Tables.Schools on p.SchoolId equals s.Id
            select p).ToList();

//left join
var list = (from p in DB.Tables.People
            from s in DB.Tables.Schools.Where(r=>r.Id.Equals(p.SchoolId)).DefaultIfEmpty()
            select p).ToList();


//express functions



```

1.2 SqlQuery
```csharp
//to list
var list1 = DB.Query<Person>("select * from person").ToList();

//to list with par
var name = "yuzd";
var list = DB.Query<Person>("select * from person where name=@name",new { name = name }).ToList();
var list = await DB.Query<Person>("select * from person where name=@name",new { name = name }).ToListAsync();

//to DataTable
DataTable tb = DB.QueryTable("select * from person");
var tb = await DB.QueryTableAsync("select * from person");
var name = "yuzd";
SQL sql = "select * from person where name=@name";
sql = sql["name", name];
var list =  DB.QueryTable(sql);

//get int or long or other
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

//customer pt

public class MyClass
{
    public string Name { get; set; }
    public int Age { get; set; }
}

var name = "yuzd";
var list = DB.Query<MyClass>("select * from person where name=@name",new {name = name}).ToList();

// to anonymous object
var name = "yuzd";
var list = DB.Query(new {Id= 0 ,Name = "",Age = 0}, "select * from person where name=@name", new {name = name}).ToList();

```

# 2.Insert
````csharp
//insert item
Person p = new Person
{
    Name = "yuzd",
    Age = 27
};
DB.Insert(p);

//insert list
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

//insert without same columns
DB.Tables.People.Value(r=>r.Name,"yuzd").Value(r=>r.DataChangeLastTime,DateTime.Now).Insert() // insert  with no 【Age】

//insert with  Identity
var id = DB.Tables.People.Value(r => r.Name, "yuzd").Value(r => r.DataChangeLastTime, DateTime.Now).InsertWithIdentity();
Person p = new Person
{
    Name = "yuzd",
    Age = 27,
    SchoolId = 1
};

DB.InsertWithIdentity(p);【 now p.Id is the Identity result 】

//if identity is guid
var guidIdentity = DB.InsertWithIdentity<Person,string>(p);

//insert ignore null field
Person p = new Person
{
    Name = null,
    Age = 11,
    SchoolId = null // will be ignored
};
DB.Insert(p);

````

# 3.Update 
```csharp
//update specified column
DB.Tables.People.Where(r => r.Name.Equals("yuzd")).Set(r => r.Age, 10).Update() ;
//下面这种例如你是传一个字段名称和值就完成更新的话能派上用场
DB.Tables.People.Where(r => r.Name.Equals("yuzd")).Set2<Person, int?>("Age", 20).Update() ;
//分开多次
var updateQuery = DB.Tables.People.Where(r => r.Name.Equals("yuzd")).Set(r => r.Age, 10);
if(XXXX){
  updateQuery=updateQuery.Set(r => r.Name, 'xxx'); //切记多次的情况要覆盖updateQuery
}
updateQuery.Update();

//update  by  entity 
var entity = DB.Tables.People.FirstOrDefault();
entity.DataChangeLastTime = DateTime.Now;
DB.Update(entity);//注意这样的话是会根据entity的主键来更新所有的字段的

//第一个参数代表条件 第二个参数是要组织你想要更新的字段和对应的值
//如果People有100个字段 只想要更新其中的2个字段
DB.Tables.People.Update(o => o.Name == "yuzd", o => new People { Age = o.Age + 1,Name = "yuzd2"});
对应的sql--->update person set age = age+1,name = 'yuzd2' where name ='yuzd'
如果是DB.Tables.People.Update(o => o.Name == "yuzd", o => new People { Age = o.Age + 1});
那么对应的sql --->update person set age = age+1 where name ='yuzd'

DB.Tables.People.Where(r => r.Name.Equals("yuzd")).Set(r => r.Age, y=>y.Age+1).Update();
对应的sql--->update person set age = age+1 where name ='yuzd'


//bulkupdate 【sqlserver only】
var allPerson = DB.Tables.People.ToList();
allPerson.ForEach(r =>
{
    r.DataChangeLastTime = DateTime.Now;
});

DB.Tables.People.Merge(allPerson);

```

# 4.Delete
```csharp

//delete by exp
DB.Tables.People.Where(r => r.Name.Equals("yuzd")).Delete();

//delete by entity
var entity = DB.Tables.People.FirstOrDefault();
DB.Delete(entity);


```


# 5.Tran
```csharp
Person p = new Person
{
    Age = 27
};


DB.UseTransaction(con =>
{
     //注意在Transaction使用的时候 一定要用 con 不要用外层的DB对象 
    con.Tables.Schools.Where(r=>r.Name.Equals("上海大学")).Set(r=>r.Address,"no update").Update();
    con.Insert(p);
    return true;
});

```

# 6.[Read-write separation DEMO](http://www.cnblogs.com/yudongdong/p/6432049.html)

# 7.Sharding By DB And Sharding By Table
Please see unit test

# 8.我就是喜欢纯写sql,怎么整
```csharp
//提供了Query方法执行查询sql(select)   
//例如悲观锁的select也是可以通过写sql来实现的  ====更多例子可以参考上面的 [1.2 SqlQuery章节]
var currentOrderId = DB.Query<currentOrderId>("select * from current_order_id where Tid = 1 for update;").FirstOrDefault();

//Execute方法是为了执行sql(insert update delete)
var count= DB.Execute("update person where name='yuzd'");//count是执行sql受影响的条数

```

# 9.常用的配置数据源方法
1 代码配置(netfx和netcore都适用)
```csharp
//配置1个mysql数据源 在使用的时候直接用 "testorm_mysql" 这个逻辑名称
 AntData.ORM.Common.Configuration.DBSettings = new DBSettings
            {
                DatabaseSettings = new List<DatabaseSettings>
                {
                    new DatabaseSettings
                    {
                        Name = "testorm_mysql",
                        Provider = "mysql",
                        ConnectionItemList = new List<ConnectionStringItem>
                        {
                            new ConnectionStringItem
                            {
                                Name = "testorm_mysql",
                                ConnectionString = connectionString
                            }
                        }
                    }
                }
            };
	    
//配置2个不同的mysql数据源 在使用的时候分别使用不同的 "testorm_mysql1" or "testorm_mysql2" 逻辑名称
 AntData.ORM.Common.Configuration.DBSettings = new DBSettings
            {
                DatabaseSettings = new List<DatabaseSettings>
                {
                    new DatabaseSettings
                    {
                        Name = "testorm_mysql1",
                        Provider = "mysql",
                        ConnectionItemList = new List<ConnectionStringItem>
                        {
                            new ConnectionStringItem
                            {
                                Name = "testorm_mysql1",
                                ConnectionString = connectionString1
                            }
                        }
                    },
		    new DatabaseSettings
                    {
                        Name = "testorm_mysql2",
                        Provider = "mysql",
                        ConnectionItemList = new List<ConnectionStringItem>
                        {
                            new ConnectionStringItem
                            {
                                Name = "testorm_mysql2",
                                ConnectionString = connectionString2
                            }
                        }
                    }
                }
            };	    
	    
//配置Master-Slave类型mysql数据源 
 AntData.ORM.Common.Configuration.DBSettings = new DBSettings
            {
                DatabaseSettings = new List<DatabaseSettings>
                {
                    new DatabaseSettings
                    {
                        Name = "testorm_mysql",
                        Provider = "mysql",
                        ConnectionItemList = new List<ConnectionStringItem>
                        {
                            new ConnectionStringItem
                            {
                                Name = "testorm_mysql1",
                                ConnectionString = connectionString1,
				DatabaseType = DatabaseType.Master
                            },
			    new ConnectionStringItem
                            {
                                Name = "testorm_mysql2",
                                ConnectionString = connectionString2,
				DatabaseType = DatabaseType.Slave
                            }
                        }
                    }
                }
            };	 
```
2 netfx下用config文件配置
```csharp
以下是App.config配置内容
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <configSections>
    <section name="dal" type="AntData.ORM.DbEngine.Configuration.DbEngineConfigurationSection, AntData.ORM"/>
  </configSections>


  <location path="." allowOverride="true" inheritInChildApplications="false">
    <dal configSource="Config\Dal.config"/>
  </location>

</configuration>
以下是Config\Dal.config文件的配置内容
<dal name="DBDal">
  <databaseSets>
    <databaseSet name="testorm" provider="mySqlProvider">
      <add name="testorm1" databaseType="Master" connectionString="Server=127.0.0.1;Port=28747;Database=testorm;Uid=root;Pwd=123456;charset=utf8;"/>
    </databaseSet>
  <databaseProviders>
    <add name="mySqlProvider" type="AntData.ORM.Mysql.MySqlDatabaseProvider,AntData.ORM.Mysql"/>
  </databaseProviders>
</dal>
```	
3 netcore下用appsettings.json文件配置
```csharp
{
  "Logging": {
    "IncludeScopes": false,
    "LogLevel": {
      "Default": "Warning"
    }
  },
  "dal": [
    {
      "Provider": "mysql",
      "Name": "testorm_mysql",
      "ConnectionItemList": [
        {
          "Name": "testorm_mysql",
          "ConnectionString": ConnectionString,
          "DatabaseType": "Master"
        }
      ]

    },
    {
      "Provider": "mysql",
      "Name": "testorm_mysql2",
      "ConnectionItemList": [
        {
          "Name": "testorm_mysql2",
          "ConnectionString": "Server=127.0.0.1;Port=28747;Database=testorm;Uid=root;Pwd=123456;charset=utf8;SslMode=none",
          "DatabaseType": "Master"
        }
      ]

    }
  ]
}

然后在Startup.cs里面的 Configure方法里面
AntData.ORM.Common.Configuration.UseDBConfig(Configuration);
```
