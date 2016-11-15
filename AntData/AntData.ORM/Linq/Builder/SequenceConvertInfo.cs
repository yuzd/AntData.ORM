using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace AntData.ORM.Linq.Builder
{
	public class SequenceConvertInfo
	{
		public ParameterExpression       Parameter;
		public Expression                Expression;
		public List<SequenceConvertPath> ExpressionsToReplace;
	}
}
