using System;

// ReSharper disable CheckNamespace

namespace AntData.ORM
{
	partial class Sql
	{
		[AttributeUsage(AttributeTargets.Enum, AllowMultiple = false, Inherited = false)]
		public class EnumAttribute : Attribute
		{
		}
	}
}
