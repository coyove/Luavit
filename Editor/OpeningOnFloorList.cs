using Autodesk.Revit.DB;
using System;
using System.Data;

namespace Revit.Addon.RevitDBLink.CS
{
	public class OpeningOnFloorList : NonCreatableElementList
	{
		public override void InitializeList()
		{
			base.AddRange(new FilteredElementCollector(APIObjectList.ActiveDocument).OfCategory((BuiltInCategory)(-2000898)));
		}

		protected override void PopulateDbRow(Element element, DataRow row)
		{
			Opening opening = element as Opening;
			Floor host = opening.Host as Floor;
			base.SetDbRowIDAndHostId(opening, host, row);
		}
	}
}
