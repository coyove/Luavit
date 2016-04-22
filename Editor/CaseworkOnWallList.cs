using Autodesk.Revit.DB;
using System;
using System.Data;
using System.Linq;

namespace Revit.Addon.RevitDBLink.CS
{
	public class CaseworkOnWallList : NonCreatableElementList
	{
		public override void InitializeList()
		{
			base.AddRange(from fi in base.GetFamilyInstance(new BuiltInCategory[]
			{
				 (BuiltInCategory)(-2001000)
			})
			where fi.Host is Wall
			select fi);
		}

		protected override void PopulateDbRow(Element element, DataRow row)
		{
			FamilyInstance familyInstance = element as FamilyInstance;
			base.SetDbRowIDAndHostId(familyInstance, familyInstance.Host, row);
		}
	}
}
