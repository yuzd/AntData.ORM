using System;
using AntData.ORM.Enums;

namespace AntData.ORM.DbEngine.HA
{
    class HAFactory
    {
        /// <summary>
        /// 获取HA对象
        /// </summary>
        /// <param name="logicDbName"></param>
        /// <returns></returns>
        public static IHA GetInstance(String logicDbName)
        {
            var providerType = DALBootstrap.GetProviderType(logicDbName);

            switch (providerType)
            {
                case DatabaseProviderType.SqlServer:
                    return new SqlServerHA();
                case DatabaseProviderType.MySql:
                    return new MySqlHA();
                case DatabaseProviderType.Oracle:
                    return new OracleHA();
                default:
                    throw new NotImplementedException("Not supported.");
            }
        }

    }
}
