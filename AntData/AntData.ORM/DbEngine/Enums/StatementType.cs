using System.ComponentModel;

namespace AntData.ORM.Enums
{
    /// <summary>
    /// Sql指令类型
    /// </summary>
    public enum StatementType
    {
        /// <summary>
        /// Sql语句
        /// </summary>
        [Description("Sql语句")]
        Sql = 0,

        /// <summary>
        /// 存贮过程
        /// </summary>
        [Description("存储过程")]
        StoredProcedure = 1
    }
}