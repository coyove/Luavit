using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using System;
using System.Data;
using System.Linq;

namespace Revit.Addon.RevitDBLink.CS
{
	public class LineLoadOnBeamList : NonCreatableElementList
	{
		public override void InitializeList()
		{
			base.AddRange(from lineLoad in base.GetElementsByCategory<LineLoad>(new BuiltInCategory[]
			{
				 (BuiltInCategory)(-2005202)
			})
			where APIObjectList.GetCategoryId(lineLoad.HostElement) == (BuiltInCategory)(-2001320)
			select lineLoad);
		}

		protected override void PopulateDbRow(Element element, DataRow row)
		{
			LineLoad lineLoad = element as LineLoad;
			Utility.Assign(row, APIObjectList.ColumnRes("ColN_CST_LineLoadId"), APIObjectList.GetIdDbValue(lineLoad));
			Utility.Assign(row, APIObjectList.ColumnRes("ColN_CST_BeamId"), APIObjectList.GetIdDbValue(lineLoad.HostElement));
		}
	}
}
