using System;
using System.Data;

namespace Revit.Addon.RevitDBLink.CS
{
	public class EnumeratorList : APIObjectList
	{
		public System.Type EnumType
		{
			get;
			set;
		}

		public override bool Readonly
		{
			get
			{
				return true;
			}
		}

		public override int Count
		{
			get
			{
				return System.Enum.GetNames(this.EnumType).Length;
			}
		}

		public EnumeratorList(System.Type enumType)
		{
			this.EnumType = enumType;
		}

		public override void ExportToDataTable(DataTable dataTable)
		{
			if (dataTable == null)
			{
				return;
			}
			base.DataTable = dataTable;
			EnumeratorTableInfo arg_16_0 = (EnumeratorTableInfo)base.TableInfo;
			System.Type enumType = this.EnumType;
			foreach (object current in System.Enum.GetValues(enumType))
			{
				DataRow dataRow = this.GetDataRow(current);
				if (dataRow == null)
				{
					dataRow = dataTable.NewRow();
					dataTable.Rows.Add(dataRow);
					Utility.Assign(dataRow, APIObjectList.ColumnRes("ColN_CST_EnumIndex"), (int)current);
				}
				string text = current.ToString();
				string a = dataRow[APIObjectList.ColumnRes("ColN_CST_EnumName")] as string;
				if (a != text)
				{
					Utility.Assign(dataRow, APIObjectList.ColumnRes("ColN_CST_EnumName"), text);
				}
			}
		}

		public override void ClearUpdatedOrAddedRecords()
		{
			if (base.DataTable == null)
			{
				return;
			}
			base.DataTable.Clear();
		}

		public override object GetRevitObject(DataRow dataRow)
		{
			return null;
		}

		protected override object[] GetPrimaryKeyValues(object apiObject)
		{
			return new object[]
			{
				(int)apiObject
			};
		}

		public override void InitializeList()
		{
		}
	}
}
