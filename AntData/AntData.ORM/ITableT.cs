using System;

namespace AntData.ORM
{
	using Linq;

	public interface ITable<
#if !SL4
		out
#endif
		T> : IExpressionQuery<T>
	{
	}
}
