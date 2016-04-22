using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Revit.Addon.RevitDBLink.CS
{
	public class PlumbingFixtureOnWallList : NonCreatableElementList
	{
		public override void InitializeList()
		{
			System.Collections.Generic.IEnumerable<Element> elements = from element in base.GetFamilyInstance(new BuiltInCategory[]
			{
                (BuiltInCategory)(-2001160)
			})
			where element.Host is Wall
			select element;
			base.AddRange(elements);
		}

		protected override void PopulateDbRow(Element element, DataRow row)
		{
			FamilyInstance familyInstance = element as FamilyInstance;
			base.SetDbRowIDAndHostId(familyInstance, familyInstance.Host, row);
		}
	}
}
