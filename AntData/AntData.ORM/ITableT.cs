using System;

namespace AntData.ORM
{
	using Linq;

	public interface ITable<out T> : IExpressionQuery<T>
	{
	


	}

    public interface IAntTable<out T,out C> : IExpressionQuery<T> where C:new()
    {
        string Name { get; }

        C Column { get; }
    }
}
