using Autodesk.Revit.DB;
using System;

namespace Revit.Addon.RevitDBLink.CS
{
	public class CustomColumnInfo : ColumnInfo
	{
		public CustomColumnInfo(string columnId, DataType dataType)
		{
			base.ColumnId = columnId;
			base.DataType = dataType;
			base.BuiltInParameter = (BuiltInParameter)(-1);
			base.Name = columnId;
		}
	}
}
