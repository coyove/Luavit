using Autodesk.Revit.DB;
using System;
using System.Data;
using System.Linq;

namespace Revit.Addon.RevitDBLink.CS
{
	public class DoorWallList : NonCreatableElementList
	{
		public override void InitializeList()
		{
			base.AddRange(from fi in base.GetFamilyInstance(new BuiltInCategory[]
			{
                (BuiltInCategory)(-2000023)
			})
			where APIObjectList.GetCategoryId(fi.Host) == (BuiltInCategory) (- 2000011)
			select fi);
		}

		protected override void PopulateDbRow(Element element, DataRow row)
		{
			FamilyInstance familyInstance = element as FamilyInstance;
			Utility.Assign(row, APIObjectList.ColumnRes("ColN_CST_DoorId"), APIObjectList.GetIdDbValue(element));
			Utility.Assign(row, APIObjectList.ColumnRes("ColN_CST_DoorName"), APIObjectList.GetNameDbValue(element));
			Utility.Assign(row, APIObjectList.ColumnRes("ColN_CST_WallId"), APIObjectList.GetIdDbValue(familyInstance.Host));
			Utility.Assign(row, APIObjectList.ColumnRes("ColN_CST_WallName"), APIObjectList.GetNameDbValue(familyInstance.Host));
		}
	}
}
