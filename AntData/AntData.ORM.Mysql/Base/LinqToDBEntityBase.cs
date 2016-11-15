//-----------------------------------------------------------------------
// <copyright file="LinqToDBEntityBase .cs" company="Company">
// Copyright (C) Company. All Rights Reserved.
// </copyright>
// <author>nainaigu</author>
// <create>2016-05-24</create>
// <summary></summary>
//-----------------------------------------------------------------------

using System;

namespace AntData.ORM.Mysql.Base
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class LinqToDBEntityBase
    {

        //public static readonly Dictionary<string, Member> MetaData;

        //static LinqToDBEntityBase()
        //{
        //    MetaData = new Dictionary<string, Member>();
        //    var ass = System.AppDomain.CurrentDomain.GetAssemblies();

        //    foreach (var item in ass)
        //    {
        //        var types = item.GetTypes();

        //        foreach (var type in types)
        //        {
        //            if (type.IsSubclassOf(typeof(LinqToDBEntity)))
        //            {
        //                MetaData.Add(type.FullName, GetMetaData(type));
        //            }
        //        }
        //    }
        //}

        //public static List<object> GetArray<Entity>(Entity entity, params string[] members) where Entity : LinqToDBEntityBase
        //{
        //    if (null == entity) { return null; }

        //    var name = entity.GetType().FullName;

        //    if (MetaData.ContainsKey(name))
        //    {
        //        var list = new List<object>();
        //        var meta = MetaData[name];

        //        //var member = meta.MemberAccessor.Where(c => 0 < members.Length ? members.Contains(c.Key) : true);

        //        var member = new List<System.Tuple<System.Type, System.Func<object, object>, System.Action<object, object>>>();

        //        if (0 < members.Length)
        //        {
        //            foreach (var item in members)
        //            {
        //                if (meta.MemberAccessor.ContainsKey(item)) { member.Add(meta.MemberAccessor[item]); }
        //            }
        //        }
        //        else
        //        {
        //            member.AddRange(meta.MemberAccessor.Values);
        //        }

        //        if (0 < member.Count)
        //        {
        //            foreach (var item in member)
        //            {
        //                list.Add(item.Item2(entity));
        //            }

        //            return list;
        //        }
        //    }

        //    return null;
        //}

        //public static List<List<object>> GetArrayList<Entity>(IEnumerable<Entity> entitys, params string[] members)
        //    where Entity : LinqToDBEntityBase
        //{
        //    if (null == entitys || 0 == entitys.Count()) { return null; }

        //    var name = entitys.ElementAt(0).GetType().FullName;

        //    if (MetaData.ContainsKey(name))
        //    {
        //        var list = new List<List<object>>();
        //        var meta = MetaData[name];

        //        var member = new List<System.Tuple<System.Type, System.Func<object, object>, System.Action<object, object>>>();

        //        if (0 < members.Length)
        //        {
        //            foreach (var item in members)
        //            {
        //                if (meta.MemberAccessor.ContainsKey(item)) { member.Add(meta.MemberAccessor[item]); }
        //            }
        //        }
        //        else
        //        {
        //            member.AddRange(meta.MemberAccessor.Values);
        //        }

        //        //var member = meta.MemberAccessor.Where(c => 0 < members.Length ? members.Contains(c.Key) : true);

        //        if (0 < member.Count)
        //        {
        //            foreach (var item in entitys)
        //            {
        //                var list1 = new List<object>();

        //                foreach (var item1 in member)
        //                {
        //                    list1.Add(item1.Item2(item));
        //                }
        //                list.Add(list1);
        //            }

        //            return list;
        //        }
        //    }

        //    return null;
        //}

        //static Member GetMetaData(System.Type type)
        //{
        //    var member = new Member(new Dictionary<string, System.Tuple<System.Type, System.Func<object, object>, System.Action<object, object>>>());

        //    var fields = type.GetFields();
        //    foreach (var field in fields)
        //    {
        //        member.MemberAccessor.Add(field.Name, System.Tuple.Create(field.FieldType, cmstar.RapidReflection.Emit.FieldAccessorGenerator.CreateGetter(field), cmstar.RapidReflection.Emit.FieldAccessorGenerator.CreateSetter(field)));
        //    }

        //    var propertys = type.GetProperties();
        //    foreach (var property in propertys)
        //    {
        //        member.MemberAccessor.Add(property.Name, System.Tuple.Create(property.PropertyType, cmstar.RapidReflection.Emit.PropertyAccessorGenerator.CreateGetter(property), cmstar.RapidReflection.Emit.PropertyAccessorGenerator.CreateSetter(property)));
        //    }

        //    return member;
        //}

        //public struct Member
        //{
        //    public Member(Dictionary<string, System.Tuple<System.Type, System.Func<object, object>, System.Action<object, object>>> memberAccessor)
        //    {
        //        this.memberAccessor = memberAccessor;
        //    }

        //    readonly Dictionary<string, System.Tuple<System.Type, System.Func<object, object>, System.Action<object, object>>> memberAccessor;
        //    public Dictionary<string, System.Tuple<System.Type, System.Func<object, object>, System.Action<object, object>>> MemberAccessor { get { return memberAccessor; } }
        //}

        //public abstract long Tid { get; set; }
        //public abstract DateTime CreateTime { get; set; }
        //public abstract DateTime DataChange_LastTime { get; set; }
    }
}