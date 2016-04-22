using Autodesk.Revit.DB;
using System;

namespace Revit.Addon.RevitDBLink.CS
{
	public class InstanceTableInfo : TableInfo
	{
		public InstanceTableInfo(BuiltInCategory category) : this(category, new InstanceList(category))
		{
		}

		public InstanceTableInfo(BuiltInCategory category, APIObjectList objectList) : base(APIObjectList.GenerateResourceIdsForTableName(ElementTypeEnum.INSTANCE, category), objectList)
		{
			base.WithColumn( (BuiltInParameter)(-1002100), DataType.INTEGER).WithPrimaryKey((BuiltInParameter) (- 1002100));
		}
	}
}
