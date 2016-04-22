using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;

namespace Revit.Addon.RevitDBLink.CS
{
	public abstract class MultiPrimaryKeyObjectList : APIObjectList
	{
		protected System.Collections.ArrayList m_list = new System.Collections.ArrayList();

		public override int Count
		{
			get
			{
				return this.m_list.Count;
			}
		}

		public override void ExportToDataTable(DataTable dataTable)
		{
			base.DataTable = dataTable;
			foreach (object current in this.m_list)
			{
				this.UpdateOrAddDbRow(current, false);
			}
		}

		public DataRow UpdateOrAddDbRow(object item, bool isElementCreated)
		{
			DataRow dataRow = this.GetDataRow(item);
			if (dataRow == null)
			{
				if (base.DataTable == null)
				{
					return null;
				}
				dataRow = base.DataTable.NewRow();
				if (!isElementCreated)
				{
					base.DataTable.Rows.Add(dataRow);
				}
			}
			this.PopulateDbRow(item, dataRow);
			return dataRow;
		}

		protected abstract void PopulateDbRow(object element, DataRow row);

		public override void ClearUpdatedOrAddedRecords()
		{
			if (this.m_list == null || base.DataTable == null)
			{
				return;
			}
			if (this.m_list.Count != 0)
			{
				System.Collections.Generic.List<DataRow> list = new System.Collections.Generic.List<DataRow>();
				DataRowCollection rows = base.DataTable.Rows;
				for (int i = rows.Count - 1; i >= 0; i--)
				{
					if (this.GetRevitObject(rows[i]) != null)
					{
						list.Add(rows[i]);
					}
				}
				foreach (DataRow current in list)
				{
					base.DataTable.Rows.Remove(current);
				}
			}
		}
	}
}
