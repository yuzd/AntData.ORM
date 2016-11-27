//-----------------------------------------------------------------------
// <copyright file="SQL.cs" company="Company">
// Copyright (C) Company. All Rights Reserved.
// </copyright>
// <author>nainaigu</author>
// <summary></summary>
//-----------------------------------------------------------------------

using AntData.ORM.Data;

namespace AntData.ORM.Mapping
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;


    /// <summary>
    /// 
    /// </summary>
    public class SQL
    {

        /// <summary>
        /// 构建SQL对象并指定相应的sql语句
        /// </summary>
        /// <param name="sql"></param>
        public SQL(string sql)
        {
            mCommand.AddSqlText(sql);
        }

        public static SQL operator +(string subsql, SQL sql)
        {
            sql.AddSql(subsql);
            return sql;
        }

        public static SQL operator +(SQL sql, string subsql)
        {
            sql.AddSql(subsql);
            return sql;
        }

        private Command mCommand = new Command("");

        private Command Command
        {
            get
            {
                return mCommand;
            }
        }
        /// <summary>
        /// 设置SQL参数值
        /// </summary>
        /// <param name="name">参数名称</param>
        /// <param name="value">参数值</param>
        /// <returns>SQL对象</returns>
        public SQL this[string name, object value]
        {
            get
            {
                return Parameter(name, value);
            }
        }

        public static implicit operator SQL(string sql)
        {
            return new SQL(sql);
        }

        public SQL AddSql(string sql)
        {
            mCommand.AddSqlText(sql);
            return this;
        }
        public SQL Parameter(string name, object value)
        {
            mCommand.AddParameter(name, value);
            return this;
        }

        public IList<DataParameter> Parameters
        {
            get { return Command.Parameters; }
        }

        public override string ToString()
        {
            return Command.Text.ToString();
        }
    }

    /// <summary>
    /// 命令处理对象
    /// </summary>
    public class Command
    {
        /// <summary>
        /// 构建命令对象,并指定相应的SQL
        /// </summary>
        /// <param name="text">SQL语句</param>
        public Command(string text)
        {
            Text.Append(text);

        }

        private System.Text.StringBuilder mText = new System.Text.StringBuilder(256);
        /// <summary>
        /// 获取相应的SQL内容
        /// </summary>
        public System.Text.StringBuilder Text
        {
            get
            {
                return mText;
            }

        }
    

        private IList<DataParameter> mParameters = new List<DataParameter>();
        /// <summary>
        /// 获取对应的参数集合
        /// </summary>
        public IList<DataParameter> Parameters
        {
            get
            {
                return mParameters;
            }
        }
        /// <summary>
        /// 添加指数名称和值的命令参数
        /// </summary>
        /// <param name="name">参数名</param>
        /// <param name="value">参数值</param>
        /// <returns>Command</returns>
        public Command AddParameter(string name, object value)
        {
            DataParameter p = new DataParameter();
            p.Name = name;
            p.Value = value;
            return AddParameter(p);
        }
      
        /// <summary>
        /// 添加命令参数
        /// </summary>
        /// <param name="parameter">参数对象</param>
        /// <returns>Command</returns>
        public Command AddParameter(DataParameter parameter)
        {
            Parameters.Add(parameter);
            return this;
        }
  
        /// <summary>
        /// 添加SQL内容
        /// </summary>
        /// <param name="text">SQL内容</param>DataParameter
        /// <returns>Command</returns>
        public Command AddSqlText(string text)
        {
            Text.Append(text);
            return this;
        }
        /// <summary>
        /// 清除所有参数
        /// </summary>
        public void ClearParameter()
        {
            Parameters.Clear();
        }
        /// <summary>
        /// 清除命令内部内容
        /// </summary>
        public void Clean()
        {
            ClearParameter();
#if NET_4
            Text.Clear();
#else
            Text.Remove(0, Text.Length);
#endif

        }

     
    }
}