using System;

namespace AntData.ORM.DbEngine.Executor
{
    public class DefaultExecutor : IExecutor
    {
        public void Daemon() { }

        public void Dispose() { }

        public Boolean EnqueueLogEntry()
        {
            return false;
        }

        public Boolean EnqueueCallback(Action callback)
        {
            return false;
        }

        public void StartRWCallback(Action callback, Int32 delay) { }

    }
}
