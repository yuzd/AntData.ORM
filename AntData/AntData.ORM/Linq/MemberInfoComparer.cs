using System;
using System.Collections.Generic;
using System.Reflection;

namespace AntData.ORM.Linq
{
	using Extensions;

	class MemberInfoComparer : IEqualityComparer<MemberInfo>
	{
		public bool Equals(MemberInfo x, MemberInfo y)
		{
			return x.EqualsTo(y);
		}

		public int GetHashCode(MemberInfo obj)
		{
			return obj == null ? 0 : obj.Name.GetHashCode();
		}
	}
}
