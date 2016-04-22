using Autodesk.Revit.DB;
using System;
using System.Data;
using System.Linq;

namespace Revit.Addon.RevitDBLink.CS
{
	public class ElementLevelList : NonCreatableElementList
	{
		public override void InitializeList()
		{
			base.AddRange(from element in base.GetElements()
			where element.Id.IntegerValue != -1 && element.LevelId != ElementId.InvalidElementId
			select element);
		}

		protected override void PopulateDbRow(Element element, DataRow row)
		{
			Utility.Assign(row, APIObjectList.ColumnRes("ColN_CST_ElementId"), APIObjectList.GetIdDbValue(element));
			Utility.Assign(row, APIObjectList.ColumnRes("ColN_CST_ElementName"), APIObjectList.GetNameDbValue(element));
			Utility.Assign(row, APIObjectList.ColumnRes("ColN_CST_LevelId"), APIObjectList.GetIdDbValue(element.LevelId));
			Utility.Assign(row, APIObjectList.ColumnRes("ColN_CST_LevelName"), APIObjectList.GetNameDbValue(element.Document.GetElement(element.LevelId)));
		}
	}
}
