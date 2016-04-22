using Autodesk.Revit.DB;
using System;

namespace Revit.Addon.RevitDBLink.CS
{
	public class RoomAssociationsList : SpatialElementAssociationsList
	{
		protected override string GetSpatialElementColumnKey()
		{
			return "ColN_CST_RoomId";
		}

		protected override SpatialElement GetSpatialElement(FamilyInstance familyInstance, Phase phase)
		{
			if (familyInstance == null)
			{
				return null;
			}
			try
			{
				return familyInstance.get_Room(phase);
			}
			catch (System.Exception value)
			{
				Log.WriteLine("FamilyInstance.get_Room Id:{0}", new object[]
				{
					APIObjectList.GetElementId(familyInstance)
				});
				Log.WriteLine(value);
			}
			return null;
		}
	}
}
