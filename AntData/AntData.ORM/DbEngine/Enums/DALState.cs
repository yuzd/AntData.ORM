using System.ComponentModel;

namespace AntData.ORM.Enums
{
    public enum DALState
    {
        [Description("executing")]
        Executing,

        [Description("fail")]
        Fail,

        [Description("success")]
        Success
    }
}
