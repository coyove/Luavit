using System;

namespace Revit.Addon.RevitDBLink.CS
{
	public class EnumeratorTableInfo : TableInfo
	{
		public EnumeratorTableInfo(string resId, APIObjectList list) : base(resId, list)
		{
			base.WithColumn("ColN_CST_EnumIndex", DataType.INTEGER).WithDisplayColumn("ColN_CST_EnumName", DataType.TEXT).WithPrimaryKey("ColN_CST_EnumIndex");
		}
	}
}
