using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using System;
using System.Data;

namespace Revit.Addon.RevitDBLink.CS
{
	public class RoomTagsList : NonCreatableElementList
	{
		public override void InitializeList()
		{
			base.AddRange(base.GetElementsByTypeExcluded<RoomTag>());
		}

		protected override void PopulateDbRow(Element element, DataRow row)
		{
			RoomTag roomTag = element as RoomTag;
			Utility.Assign(row, APIObjectList.ColumnRes("ColN_CST_Id"), APIObjectList.GetIdDbValue(roomTag));
			Utility.Assign(row, APIObjectList.ColumnRes("ColN_CST_RoomTagType"), APIObjectList.GetNameDbValue(APIObjectList.GetElementById(roomTag.GetTypeId())));
			Utility.Assign(row, APIObjectList.ColumnRes("ColN_CST_RoomId"), APIObjectList.GetIdDbValue(roomTag.Room));
			Utility.Assign(row, APIObjectList.ColumnRes("ColN_CST_ViewId"), APIObjectList.GetIdDbValue(roomTag.View));
		}
	}
}
