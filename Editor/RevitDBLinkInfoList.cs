using System;
using System.Data;

namespace Revit.Addon.RevitDBLink.CS
{
	public class RevitDBLinkInfoList : APIObjectList
	{
		public override int Count
		{
			get
			{
				return 1;
			}
		}

		public override void InitializeList()
		{
		}

		public override void ExportToDataTable(DataTable dataTable)
		{
			if (dataTable == null)
			{
				return;
			}
			base.DataTable = dataTable;
			string @string = RDBResource.GetString("ExportedDrawing");
			DataRow dataRow = this.GetDataRow(@string);
			if (dataRow == null)
			{
				dataRow = dataTable.NewRow();
				dataTable.Rows.Add(dataRow);
				Utility.Assign(dataRow, APIObjectList.ColumnRes("ColN_CST_Name"), @string);
			}
			string a = dataRow[APIObjectList.ColumnRes("ColN_CST_Value")] as string;
			if (a != APIObjectList.ActiveDocument.PathName)
			{
				Utility.Assign(dataRow, APIObjectList.ColumnRes("ColN_CST_Value"), APIObjectList.ActiveDocument.PathName);
			}
		}

		public override object GetRevitObject(DataRow dataRow)
		{
			return null;
		}

		public override void ClearUpdatedOrAddedRecords()
		{
			if (base.DataTable == null)
			{
				return;
			}
			base.DataTable.Clear();
		}

		protected override object[] GetPrimaryKeyValues(object apiObject)
		{
			return new object[]
			{
				apiObject
			};
		}
	}
}
