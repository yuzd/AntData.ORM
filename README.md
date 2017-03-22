# AntData.ORM

# NUGET FOR .NET

SqlServer：Install-Package AntData.ORM

Mysql：Install-Package AntData.ORM.Mysql

Oracle: Install-Package AntData.ORM.Oracle


# NUGET FOR Dotnetcore

SqlServer : Install-Package AntData.Core

QQ Group ：609142508

# How to use CodeGen to auto Create Db models 

Please see:

http://note.youdao.com/noteshare?id=f4958dbc7b42971f64f44675fd116413&sub=8EF852DD85064E02AB187ACA9823D576

# DEMO
AntData.ORM For Oracle Demo：

http://note.youdao.com/share/?id=998f6da78d9d7dfcc5293ca41a6d7c3a&type=note#/

Dotnetcore Demo

http://note.youdao.com/noteshare?id=5e85736b7f5de49e4f4fecda4e3c1e8b&sub=8A0F25C519F4413193B5D6F20DC22ED1

# Instance  DbContext

```csharp

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
		string sql = customerTraceInfo.CustomerParams.Aggregate(customerTraceInfo.SqlText,
				(current, item) => current.Replace(item.Key, item.Value.Value.ToString()));
		Debug.Write(sql + Environment.NewLine);
	}
	catch (Exception)
	{
		//ignore
	}
}

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
DB.Tables.People.Where(r => r.Name.Equals("yuzd")).Set2<Person, int?>("Age", 20).Update() ;


//update  by  entity
var entity = DB.Tables.People.FirstOrDefault();
entity.DataChangeLastTime = DateTime.Now;
DB.Update(entity);

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
    con.Tables.Schools.Where(r=>r.Name.Equals("上海大学")).Set(r=>r.Address,"no update").Update();
    con.Insert(p);
    return true;
});

```

# 6.Read-write separation DEMO

http://note.youdao.com/noteshare?id=6249b5fc2d17569bef12a24d54fc1b30&sub=2A83E464B28D4F41B8A93C87179AE20D

# 6.Sharding By DB And Sharding By Table
Please see unit test
