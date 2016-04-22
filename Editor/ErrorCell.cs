using System;

namespace Revit.Addon.RevitDBLink.CS
{
	public class ErrorCell
	{
		private string m_columnName;

		private UpdateResult m_state;

		public string ColumnName
		{
			get
			{
				return this.m_columnName;
			}
		}

		public UpdateResult State
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

		public ErrorCell(string columnName, UpdateResult state)
		{
			this.m_columnName = columnName;
			this.m_state = state;
		}
	}
}
