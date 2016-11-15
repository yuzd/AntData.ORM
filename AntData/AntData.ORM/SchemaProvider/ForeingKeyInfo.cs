using System;

namespace AntData.ORM.SchemaProvider
{
	public class ForeingKeyInfo
	{
		public string Name;
		public string ThisTableID;
		public string ThisColumn;
		public string OtherTableID;
		public string OtherColumn;
		public int    Ordinal;
	}
}
