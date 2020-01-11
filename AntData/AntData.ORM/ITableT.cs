using System;

namespace AntData.ORM
{
	using Linq;

	public interface ITable<out T> : IExpressionQuery<T>
	{
	


	}

    public interface IAntTable<out T,out C> : ITable<T> where C:new()
    {
        string Table { get; }

        C Column { get; }
    }
}
