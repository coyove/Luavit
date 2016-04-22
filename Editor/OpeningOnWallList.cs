using Autodesk.Revit.DB;
using System;
using System.Data;
using System.Linq;

namespace Revit.Addon.RevitDBLink.CS
{
	public class OpeningOnWallList : NonCreatableElementList
	{
		public override void InitializeList()
		{
			base.AddRange(base.GetElementsByCategory(new BuiltInCategory[]
			{
				(BuiltInCategory)(-2000999),
                (BuiltInCategory)(-2000997)
			}).Where(delegate(Element e)
			{
				Opening opening = e as Opening;
				return opening != null && APIObjectList.GetCategoryId(opening.Host) == (BuiltInCategory)(- 2000011);
			}));
		}

		protected override void PopulateDbRow(Element element, DataRow row)
		{
			Opening opening = element as Opening;
			base.SetDbRowIDAndHostId(opening, opening.Host, row);
		}
	}
}
