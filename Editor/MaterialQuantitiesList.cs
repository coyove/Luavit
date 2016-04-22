using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Revit.Addon.RevitDBLink.CS
{
	public class MaterialQuantitiesList : ElementList
	{
		public override void InitializeList()
		{
			Category[] categorys = (from c in APIObjectList.ActiveDocument.Settings.Categories.OfType<Category>()
			where c != null && c.HasMaterialQuantities
			select c).ToArray<Category>();
			base.AddRange(APIObjectList.GetElements(false, categorys));
		}

		public override void ExportToDataTable(DataTable dataTable)
		{
			if (dataTable == null)
			{
				return;
			}
			base.DataTable = dataTable;
			ElementList elementList = base.TableInfo.ObjectList as ElementList;
			foreach (Element element in elementList.Elements)
			{
				Document document = element.Document;
				bool[] array = new bool[]
				{
					default(bool),
					true
				};
				for (int i = 0; i < array.Length; i++)
				{
					bool flag = array[i];
					if (!flag || element is HostObject)
					{
						System.Collections.Generic.ICollection<ElementId> materials = MaterialQuantitiesList.GetMaterials(element, flag);
						if (materials != null && materials.Count > 0)
						{
							foreach (ElementId current in materials)
							{
								if (current.IntegerValue != ElementId.InvalidElementId.IntegerValue && document.GetElement(current) is Material)
								{
									try
									{
										double materialArea = element.GetMaterialArea(current, flag);
										double num = flag ? 0.0 : element.GetMaterialVolume(current);
										DataRow dataRow = this.GetDataRow(new object[]
										{
											element.Id.IntegerValue,
											current.IntegerValue,
											flag
										});
										if (dataRow == null)
										{
											dataRow = dataTable.NewRow();
											Utility.Assign(dataRow, APIObjectList.ColumnRes("ColN_CST_ElementId"), element.Id.IntegerValue);
											Utility.Assign(dataRow, APIObjectList.ColumnRes("ColN_CST_MaterialId"), current.IntegerValue);
											Utility.Assign(dataRow, APIObjectList.ColumnRes("ColN_CST_AsPaint"), flag);
											dataTable.Rows.Add(dataRow);
										}
										Utility.Assign(dataRow, APIObjectList.ColumnRes("ColN_CST_Area"), materialArea);
										Utility.Assign(dataRow, APIObjectList.ColumnRes("ColN_CST_Volume"), num);
									}
									catch (System.Exception arg)
									{
										Log.WriteWarning("Cannot get material id {0} of element {1}. " + arg, new object[]
										{
											current,
											element.Id
										});
									}
								}
							}
						}
					}
				}
			}
		}

		public static System.Collections.Generic.ICollection<ElementId> GetMaterials(Element element, bool asPaint)
		{
			System.Collections.Generic.ICollection<ElementId> result;
			try
			{
				result = element.GetMaterialIds(asPaint);
			}
			catch (System.Exception ex)
			{
				Log.WriteWarning("Get Materials (AsPaint = {0}) from element throws exception, ElementId: {1}", new object[]
				{
					asPaint,
					element.Id.IntegerValue
				});
				Log.WriteWarning(ex.ToString());
				result = null;
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
				object obj = rows[i][APIObjectList.ColumnRes("ColN_CST_ElementId")];
				if (obj != null && this.GetRevitObject(rows[i]) != null)
				{
					base.DataTable.Rows.RemoveAt(i);
				}
			}
		}

		public override object GetRevitObject(DataRow dataRow)
		{
			int id = System.Convert.ToInt32(dataRow[base.TableInfo[base.TableInfo.PrimaryKeys[0]].Name]);
			return APIObjectList.GetElementById(id);
		}

		protected override object[] GetPrimaryKeyValues(object apiObject)
		{
			return apiObject as object[];
		}

		protected override void PopulateDbRow(Element element, DataRow row)
		{
		}
	}
}
