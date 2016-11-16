using System;
using System.Collections.Generic;
using AntData.ORM.Enums;

namespace AntData.ORM.DbEngine.DB
{
    public class DatabaseSetWrapper
    {
        #region private fields

        /// <summary>
        /// 数据集名称
        /// </summary>
        public String Name { get; set; }

        /// <summary>
        /// 数据集提供者类型
        /// </summary>
        //public string ProviderType { get; set; }
        public DatabaseProviderType ProviderType { get; set; }

        /// <summary>
        /// 是否支持读写分离
        /// </summary>
        public Boolean EnableReadWriteSpliding { get; set; }


        /// <summary>
        /// 数据库列表
        /// </summary>
        private List<DatabaseWrapper> m_DatabaseWrappers;

        
  

        #endregion


        /// <summary>
        /// 数据库列表
        /// </summary>
        public List<DatabaseWrapper> DatabaseWrappers
        {
            get
            {
                if (m_DatabaseWrappers == null)
                    m_DatabaseWrappers = new List<DatabaseWrapper>();
                return m_DatabaseWrappers;
            }
        }

    }
}
