using System;
using System.Collections.Generic;

namespace Revit.Addon.RevitDBLink.CS
{
	public class ErrorTable
	{
		private string m_tableName;

		private bool m_hasChange;

		private bool m_notSupportImporting;

		private string m_tableId;

		private System.Collections.Generic.List<ErrorRow> m_rows;

		public System.Collections.Generic.List<ErrorRow> Rows
		{
			get
			{
				return this.m_rows;
			}
		}

		public bool HasChange
		{
			get
			{
				return this.m_hasChange;
			}
			set
			{
				this.m_hasChange = value;
			}
		}

		public bool NotSupportImporting
		{
			get
			{
				return this.m_notSupportImporting;
			}
			set
			{
				this.m_notSupportImporting = value;
			}
		}

		public string TableName
		{
			get
			{
				return this.m_tableName;
			}
		}

		public string TableId
		{
			get
			{
				return this.m_tableId;
			}
		}

		public ErrorTable(string tableId, string tableName)
		{
			this.m_rows = new System.Collections.Generic.List<ErrorRow>();
			this.m_tableName = tableName;
			this.m_tableId = tableId;
		}
	}
}
