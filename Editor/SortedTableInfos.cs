using System;
using System.Collections.Generic;

namespace Revit.Addon.RevitDBLink.CS
{
	internal class SortedTableInfos : System.Collections.Generic.List<TableComparer>
	{
		private TableInfoSet m_tableInfoSet;

		public SortedTableInfos(TableInfoSet tableInfoSet)
		{
			this.m_tableInfoSet = tableInfoSet;
			foreach (string current in this.m_tableInfoSet.Keys)
			{
				base.Add(new TableComparer(current, this.GetLevel(current)));
			}
			base.Sort();
		}

		private int GetLevel(string tableId)
		{
			int num = 0;
			if (Command.ConfigFile.DebugSQL && !this.m_tableInfoSet.ContainsKey(tableId))
			{
				Log.WriteLine("Table key is not found in TableSet: [{0}]", new object[]
				{
					tableId
				});
			}
			TableInfo tableInfo = this.m_tableInfoSet[tableId];
			if (tableInfo.ForeignKeys.Count > 0)
			{
				num++;
			}
			foreach (ForeignKey current in tableInfo.ForeignKeys)
			{
				int num2 = this.GetLevel(current.RefTableId) + 1;
				num = ((num2 > num) ? num2 : num);
			}
			return num;
		}
	}
}
