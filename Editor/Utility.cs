using System;
using System.Data;
using System.Text;

namespace Revit.Addon.RevitDBLink.CS
{
	public class Utility
	{
		public static void Assign(DataRow row, string columnName, object value)
		{
			try
			{
				row[columnName] = value;
			}
			catch (System.Exception ex)
			{
				Log.WriteError("Failed to assign value '{0}' to column '{1}', DataRow: {2}, Exception: {3}", new object[]
				{
					value,
					columnName,
					Utility.GetItemArrayString(row),
					ex
				});
			}
		}

		public static string GetItemArrayString(DataRow dataRow)
		{
			System.Text.StringBuilder stringBuilder = new System.Text.StringBuilder();
			foreach (DataColumn dataColumn in dataRow.Table.Columns)
			{
				stringBuilder.AppendFormat("[{0}={1}] ", dataColumn.ColumnName, dataRow[dataColumn]);
			}
			return stringBuilder.ToString();
		}
	}
}
