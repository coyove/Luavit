using Autodesk.Revit.DB;
using System;
using System.Data;

namespace Revit.Addon.RevitDBLink.CS
{
	public class OpeningsList : NonCreatableElementList
	{
		public override void InitializeList()
		{
			base.AddRange(new FilteredElementCollector(APIObjectList.ActiveDocument).OfClass(typeof(Opening)));
		}

		protected override void PopulateDbRow(Element element, DataRow row)
		{
			Utility.Assign(row, APIObjectList.ColumnRes("ColN_CST_Id"), APIObjectList.GetIdDbValue(element));
			Utility.Assign(row, APIObjectList.ColumnRes("ColN_CST_Name"), APIObjectList.GetNameDbValue(element));
			Utility.Assign(row, APIObjectList.ColumnRes("ColN_CST_LevelId"), APIObjectList.GetIdDbValue(element.LevelId));
			Utility.Assign(row, APIObjectList.ColumnRes("ColN_CST_HostId"), APIObjectList.GetIdDbValue((element as Opening).Host));
		}
	}
}
