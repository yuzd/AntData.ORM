using System;

namespace AntData.ORM.Expressions
{
	using Mapping;

	public interface IGenericInfoProvider
	{
		void SetInfo(MappingSchema mappingSchema);
	}
}
