using System;

namespace AntData.ORM
{
	using Linq;

	public interface ITable<out T> : IExpressionQuery<T>
	{
	}
}
