using AntData.ORM.DbEngine.DB;

namespace AntData.ORM.DbEngine.RW
{
    public interface IReadWriteSplitting
    {
        OperationalDatabases GetOperationalDatabases(Statement statement);
    }
}