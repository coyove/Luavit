using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Revit.Addon.RevitDBLink.CS
{
	public class WindowWallList : NonCreatableElementList
	{
		public override void InitializeList()
		{
			System.Collections.Generic.IEnumerable<Element> elements = from element in base.GetFamilyInstance(new BuiltInCategory[]
			{
                (BuiltInCategory)(-2000014)
			})
			where element.Host is Wall
			select element;
			base.AddRange(elements);
		}

		protected override void PopulateDbRow(Element element, DataRow row)
		{
			FamilyInstance familyInstance = element as FamilyInstance;
			Utility.Assign(row, APIObjectList.ColumnRes("ColN_CST_WindowId"), APIObjectList.GetIdDbValue(element));
			Utility.Assign(row, APIObjectList.ColumnRes("ColN_CST_WindowName"), APIObjectList.GetNameDbValue(element));
			Utility.Assign(row, APIObjectList.ColumnRes("ColN_CST_WallId"), APIObjectList.GetIdDbValue(familyInstance.Host));
			Utility.Assign(row, APIObjectList.ColumnRes("ColN_CST_WallName"), APIObjectList.GetNameDbValue(familyInstance.Host));
		}
	}
}
