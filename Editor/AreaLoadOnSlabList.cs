using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using System;
using System.Data;
using System.Linq;

namespace Revit.Addon.RevitDBLink.CS
{
	public class AreaLoadOnSlabList : NonCreatableElementList
	{
		public override void InitializeList()
		{
			base.AddRange(base.GetElementsByCategory<AreaLoad>(new BuiltInCategory[]
			{
				(BuiltInCategory)(-2005203)
			}).Where(new Func<AreaLoad, bool>(this.Validate)));
		}

		public bool Validate(AreaLoad areaLoad)
		{
			return APIObjectList.GetCategoryId(areaLoad.HostElement) == (BuiltInCategory)(-2000032);
		}

		protected override void PopulateDbRow(Element element, DataRow row)
		{
			AreaLoad areaLoad = element as AreaLoad;
			Utility.Assign(row, APIObjectList.ColumnRes("ColN_CST_AreaLoadId"), APIObjectList.GetIdDbValue(areaLoad));
			Utility.Assign(row, APIObjectList.ColumnRes("ColN_CST_SlabId"), APIObjectList.GetIdDbValue(areaLoad.HostElement));
		}
	}
}
