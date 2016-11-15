using System;

namespace AntData.ORM.Reflection
{
	public interface IObjectFactory
	{
		object CreateInstance(TypeAccessor typeAccessor);
	}
}
