using Autodesk.Revit.DB;
using System;
using System.Data;

namespace Revit.Addon.RevitDBLink.CS
{
	public class CategoriesList : APIObjectList
	{
		private Categories m_categories;

		public override int Count
		{
			get
			{
				return this.m_categories.Size;
			}
		}

		public override void InitializeList()
		{
			this.m_categories = APIObjectList.ActiveDocument.Settings.Categories;
		}

		public override void ExportToDataTable(DataTable dataTable)
		{
			if (dataTable == null)
			{
				return;
			}
			base.DataTable = dataTable;
			foreach (Category category in this.m_categories)
			{
				if (category != null)
				{
					string text;
					CategoriesList.TryGetName(category, out text);
					DataRow dataRow = this.GetDataRow(category);
					if (dataRow == null)
					{
						dataRow = dataTable.NewRow();
						Utility.Assign(dataRow, APIObjectList.ColumnRes("ColN_CST_Id"), category.Id.IntegerValue);
						dataTable.Rows.Add(dataRow);
					}
					if (text == null)
					{
						Utility.Assign(dataRow, APIObjectList.ColumnRes("ColN_CST_Name"), System.DBNull.Value);
					}
					else
					{
						Utility.Assign(dataRow, APIObjectList.ColumnRes("ColN_CST_Name"), text);
					}
					if (category.Material != null)
					{
						Utility.Assign(dataRow, APIObjectList.ColumnRes("ColN_CST_MaterialId"), category.Material.Id.IntegerValue);
					}
					else
					{
						Utility.Assign(dataRow, APIObjectList.ColumnRes("ColN_CST_MaterialId"), System.DBNull.Value);
					}
				}
			}
		}

		public static bool TryGetName(Category category, out string categoryName)
		{
			categoryName = null;
			bool result;
			try
			{
				categoryName = category.Name;
				result = true;
			}
			catch (System.Exception)
			{
				result = false;
			}
			return result;
		}

		public override void ClearUpdatedOrAddedRecords()
		{
			if (base.DataTable == null)
			{
				return;
			}
			DataRowCollection rows = base.DataTable.Rows;
			for (int i = rows.Count - 1; i >= 0; i--)
			{
				object obj = rows[i][APIObjectList.ColumnRes("ColN_CST_Id")];
				if (obj != null && this.GetRevitObject(rows[i]) != null)
				{
					base.DataTable.Rows.RemoveAt(i);
				}
			}
		}

		public override object GetRevitObject(DataRow dataRow)
		{
			string text = (string)dataRow[APIObjectList.ColumnRes("ColN_CST_Name")];
			object result;
			try
			{
				result = this.m_categories.get_Item(text);
			}
			catch (System.Exception)
			{
				result = null;
			}
			return result;
		}

		protected override object[] GetPrimaryKeyValues(object apiObject)
		{
			Category category = apiObject as Category;
			if (category != null)
			{
				return new object[]
				{
					category.Id.IntegerValue
				};
			}
			return new object[]
			{
				System.DBNull.Value
			};
		}
	}
}
