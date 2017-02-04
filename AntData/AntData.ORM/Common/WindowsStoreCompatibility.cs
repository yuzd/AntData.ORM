using System;
using System.Runtime.InteropServices;

#if NETSTANDARD
namespace System.Threading
{
	public static class Extensions
	{
		public static void Close(this ManualResetEvent ev)
		{
			ev.Dispose();
		}
	}
}
#endif