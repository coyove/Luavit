using System;

namespace Revit.Addon.RevitDBLink.CS
{
	public class RevitDBLinkInfoTableInfo : TableInfo
	{
		public RevitDBLinkInfoTableInfo(string resid) : base(resid, new RevitDBLinkInfoList())
		{
			base.WithColumn("ColN_CST_Name", DataType.TEXT).WithColumn("ColN_CST_Value", DataType.TEXT).WithPrimaryKey("ColN_CST_Name");
		}
	}
}
