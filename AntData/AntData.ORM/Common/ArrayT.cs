using System;

using JetBrains.Annotations;

namespace AntData.ORM.Common
{
	public static class Array<T>
	{
		[NotNull]
		public static readonly T[] Empty = new T[0];
	}
}
