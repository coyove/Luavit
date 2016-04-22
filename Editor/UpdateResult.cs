using System;

namespace Revit.Addon.RevitDBLink.CS
{
	public enum UpdateResult
	{
		Unknown,
		AssemblyCode,
		Success,
		Failed,
		ReadOnlyFailed,
		ParameterNull,
		Exception,
		Equals
	}
}
