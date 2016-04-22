using System;

namespace Revit.Addon.RevitDBLink.CS
{
	internal class TableComparer : System.IComparable
	{
		private string m_tableId;

		private int m_level;

		public string TableId
		{
			get
			{
				return this.m_tableId;
			}
		}

		public int Level
		{
			get
			{
				return this.m_level;
			}
		}

		public TableComparer(string tableId, int level)
		{
			this.m_tableId = tableId;
			this.m_level = level;
		}

		public int CompareTo(object obj)
		{
			TableComparer tableComparer = obj as TableComparer;
			if (tableComparer != null)
			{
				return this.Level.CompareTo(tableComparer.Level);
			}
			return -1;
		}
	}
}
