using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using System;
using System.Data;
using System.Linq;

namespace Revit.Addon.RevitDBLink.CS
{
	public class RebarOnFramingList : NonCreatableElementList
	{
		public override void InitializeList()
		{
			base.AddRange(from rebar in base.GetElementsByTypeAndCast<Rebar>()
			where APIObjectList.GetCategoryId(rebar.GetHostId(), rebar.Document) == (BuiltInCategory) (- 2001320)
			select rebar);
		}

		protected override void PopulateDbRow(Element element, DataRow row)
		{
			Rebar rebar = element as Rebar;
			base.SetDbRowIDAndHostId(rebar, rebar.Document.GetElement(rebar.GetHostId()), row);
		}
	}
}
