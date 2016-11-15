using System;

namespace AntData.ORM.Data
{
	public enum BulkCopyType
	{
		Default = 0,
		RowByRow,
		MultipleRows,
		ProviderSpecific
	}
}
