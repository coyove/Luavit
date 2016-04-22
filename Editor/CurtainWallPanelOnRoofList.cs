using Autodesk.Revit.DB;
using System;
using System.Data;
using System.Linq;

namespace Revit.Addon.RevitDBLink.CS
{
	public class CurtainWallPanelOnRoofList : NonCreatableElementList
	{
		public override void InitializeList()
		{
			base.AddRange(from panel in base.GetElementsByCategory<Panel>(new BuiltInCategory[]
			{
			    (BuiltInCategory)(-2000170)
			})
			let panelInstance = panel
			where APIObjectList.GetCategoryId(panelInstance.Host) == (BuiltInCategory)(-2000035)
			select panelInstance);
		}

		protected override void PopulateDbRow(Element element, DataRow row)
		{
			FamilyInstance familyInstance = element as FamilyInstance;
			base.SetDbRowIDAndHostId(familyInstance, familyInstance.Host, row);
		}
	}
}
