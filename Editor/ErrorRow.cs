using System;
using System.Collections.Generic;
using System.Data;

namespace Revit.Addon.RevitDBLink.CS
{
	public class ErrorRow
	{
		private DataRow m_dataRow;

		private System.Collections.Generic.Dictionary<string, ErrorCell> m_cells;

		private bool m_hasChange;

		private DataRowState m_state;

		public DataRowState State
		{
			get
			{
				return this.m_state;
			}
			set
			{
				this.m_state = value;
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

		public DataRow DataRow
		{
			get
			{
				return this.m_dataRow;
			}
		}

		public System.Collections.Generic.Dictionary<string, ErrorCell> Cells
		{
			get
			{
				return this.m_cells;
			}
		}

		public ErrorRow(DataRow dataRow)
		{
			this.m_dataRow = dataRow;
			this.m_cells = new System.Collections.Generic.Dictionary<string, ErrorCell>();
		}
	}
}
