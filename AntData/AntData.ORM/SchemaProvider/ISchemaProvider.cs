using System;

namespace AntData.ORM.SchemaProvider
{
	using Data;

	public interface ISchemaProvider
	{
		DatabaseSchema GetSchema(DataConnection dataConnection, GetSchemaOptions options = null);
	}
}
