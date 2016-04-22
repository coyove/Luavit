using Autodesk.Revit.DB;
using System;
using System.Data;

namespace Revit.Addon.RevitDBLink.CS
{
	public abstract class NonCreatableElementList : ElementList
	{
		protected void SetDbRowIDAndHostId(Element element, Element host, DataRow dataRow)
		{
			Utility.Assign(dataRow, APIObjectList.ColumnRes("ColN_CST_Id"), APIObjectList.GetIdDbValue(element));
			Utility.Assign(dataRow, APIObjectList.ColumnRes("ColN_CST_Name"), APIObjectList.GetNameDbValue(element));
			Utility.Assign(dataRow, APIObjectList.ColumnRes("ColN_CST_HostId"), APIObjectList.GetIdDbValue(host));
			Utility.Assign(dataRow, APIObjectList.ColumnRes("ColN_CST_HostName"), APIObjectList.GetNameDbValue(host));
		}
	}
}
