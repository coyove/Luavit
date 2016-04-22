using System;

namespace Revit.Addon.RevitDBLink.CS
{
	public class ForeignKey
	{
		private string m_columnId;

		private string m_refTableId;

		private string m_refColumnId;

		public TableInfo Table
		{
			get;
			set;
		}

		public TableInfo RefTable
		{
			get;
			set;
		}

		public ColumnInfo RefColumn
		{
			get;
			set;
		}

		public ColumnInfo Column
		{
			get;
			set;
		}

		public string ColumnId
		{
			get
			{
				return this.m_columnId;
			}
			set
			{
				this.m_columnId = value;
			}
		}

		public string RefTableId
		{
			get
			{
				return this.m_refTableId;
			}
			set
			{
				this.m_refTableId = value;
			}
		}

		public string RefColumnId
		{
			get
			{
				return this.m_refColumnId;
			}
			set
			{
				this.m_refColumnId = value;
			}
		}

		public string ColumnName
		{
			get
			{
				return this.Column.Name;
			}
		}

		public string RefTableName
		{
			get
			{
				return this.RefTable.Name;
			}
		}

		public string RefColumnName
		{
			get
			{
				return this.RefColumn.Name;
			}
		}

		public ForeignKey(TableInfo table, string columnId, string refTableId, string refColumnId)
		{
			if (!Command.TableInfoSet.ContainsKey(refTableId))
			{
				Log.WriteLine("TableInfo key not found: {0}", new object[]
				{
					refTableId
				});
				Log.WriteLine(string.Format("[ForeignKey1] {0}[{1}] - {2}[{3}]", new object[]
				{
					table.TableId,
					columnId,
					refTableId,
					refColumnId
				}));
				return;
			}
			this.RefTable = Command.TableInfoSet[refTableId];
			if (!this.RefTable.ContainsKey(refColumnId))
			{
				Log.WriteLine(string.Format("[ForeignKey1] Table [{0}] does not contain column[{1}]", refTableId, refColumnId));
				return;
			}
			this.Table = table;
			this.RefColumn = this.RefTable[refColumnId];
			this.Column = this.Table[columnId];
			this.m_columnId = columnId;
			this.m_refTableId = refTableId;
			this.m_refColumnId = refColumnId;
		}
	}
}
