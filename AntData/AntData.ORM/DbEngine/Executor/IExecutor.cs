using System;

namespace AntData.ORM.DbEngine.Executor
{
    public interface IExecutor
    {
        void Daemon();

        void Dispose();

        Boolean EnqueueLogEntry();

        Boolean EnqueueCallback(Action callback);

        void StartRWCallback(Action callback, Int32 delay);

    }
}