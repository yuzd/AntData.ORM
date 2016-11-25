using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using AntData.ORM;
using AntData.ORM.DataProvider;
using AntData.ORM.DataProvider.MySql;


namespace Arch.Data.ORM.Mysql
{
    public class MysqlContext<T> : AntData.ORM.Data.DataConnection, IDataContext where T : class
    {
        private static readonly IDataProvider provider = new MySqlDataProvider();

        private readonly Lazy<T> _lazy = null;
        public T Tables
        {
            get
            {
                return _lazy.Value;
            }
        }

        public MysqlContext(string dbMappingName)
            : base(provider, dbMappingName)
        {
#if DEBUG
            //AntData.ORM.Common.Configuration.Linq.GenerateExpressionTest = true;
            AntData.ORM.Common.Configuration.Linq.AllowMultipleQuery = true;
#endif
            _lazy = new Lazy<T>(() =>
            {
                try
                {
                    Type type = typeof(T);
                    var ctor = type.GetConstructors();
                    if (ctor.Length > 0)
                    {
                        return (T)ctor[0].Invoke(new object[] { this });
                    }
                }
                catch (Exception ex)
                {

                    throw;
                }
                return null;
            });
        }


        #region New Transaction

        public void UseTransaction(System.Action<MysqlContext<T>> func)
        {
            using (var scope = new System.Transactions.TransactionScope())
            {
                func(this);
                scope.Complete();
            }
        }

        public void UseTransaction(System.Func<MysqlContext<T>, bool> func)
        {
            using (var scope = new System.Transactions.TransactionScope())
            {
                if (func(this))
                {
                    scope.Complete();
                }
            }
        }
        #endregion


    }
}
