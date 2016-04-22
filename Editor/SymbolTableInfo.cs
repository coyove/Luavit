using Autodesk.Revit.DB;
using System;

namespace Revit.Addon.RevitDBLink.CS
{
	public class SymbolTableInfo : TableInfo
	{
		public SymbolTableInfo(BuiltInCategory category) : base(APIObjectList.GenerateResourceIdsForTableName(ElementTypeEnum.SYMBOL, category), new SymbolList(category))
		{
		}
	}
}
