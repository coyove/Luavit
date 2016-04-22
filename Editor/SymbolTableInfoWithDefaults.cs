using Autodesk.Revit.DB;
using System;

namespace Revit.Addon.RevitDBLink.CS
{
	public class SymbolTableInfoWithDefaults : TableInfo
	{
		public SymbolTableInfoWithDefaults(BuiltInCategory category) : this(category, new SymbolList(category))
		{
		}

		public SymbolTableInfoWithDefaults(BuiltInCategory category, APIObjectList objectList) : this(APIObjectList.GenerateResourceIdsForTableName(ElementTypeEnum.SYMBOL, category), objectList)
		{
		}

		public SymbolTableInfoWithDefaults(string tableId, APIObjectList objectList) : base(tableId, objectList)
		{
			base.WithColumn((BuiltInParameter)(-1002100), DataType.INTEGER).WithColumn((BuiltInParameter)(-1140422), DataType.TEXT).WithColumn((BuiltInParameter)(-1010109), DataType.TEXT).WithColumn((BuiltInParameter)(-1010108), DataType.TEXT).WithColumn((BuiltInParameter)(-1010105), DataType.TEXT).WithColumn((BuiltInParameter)(-1010104), DataType.TEXT).WithColumn((BuiltInParameter)(-1010103), DataType.TEXT).WithColumn((BuiltInParameter)(-1002502), DataType.TEXT).WithColumn((BuiltInParameter)(-1002500), DataType.TEXT).WithColumn((BuiltInParameter)(-1002002), DataType.TEXT).WithDisplayColumn((BuiltInParameter)(-1002001), DataType.TEXT).WithColumn((BuiltInParameter)(-1001405), DataType.TEXT).WithColumn((BuiltInParameter)(-1001205), DataType.DOUBLE).WithPrimaryKey((BuiltInParameter)(-1002100)).WithForeignKey((BuiltInParameter)(-1002500), "TabN_CST_AssemblyCodes", (BuiltInParameter)(-1002500));
			if (OmniClassNumbersList.OmniClassNumberFileExist)
			{
				base.WithForeignKey((BuiltInParameter)(-1002502), "TabN_CST_OmniClassNumbers", (BuiltInParameter)(-1002502));
			}
		}
	}
	public class SymbolTableInfoWithDefaults<T> : SymbolTableInfoWithDefaults where T : ElementType
	{
		public SymbolTableInfoWithDefaults(BuiltInCategory category) : this(APIObjectList.GenerateResourceIdsForTableName(ElementTypeEnum.SYMBOL, category) + "_" + SymbolTableInfoWithDefaults<T>.GetSuffix(), new SymbolList<T>(category))
		{
		}

		private static string GetSuffix()
		{
			return typeof(T).Name;
		}

		public SymbolTableInfoWithDefaults(string tableId, APIObjectList objectList) : base(tableId, objectList)
		{
		}
	}
}
