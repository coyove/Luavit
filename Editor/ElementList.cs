using Autodesk.Revit.DB;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;

namespace Revit.Addon.RevitDBLink.CS
{
	public abstract class ElementList : APIObjectList
	{
		private ElementSet m_elements = new ElementSet();

		public ElementSet Elements
		{
			get
			{
				return this.m_elements;
			}
		}

		public override int Count
		{
			get
			{
				return this.m_elements.Size;
			}
		}

		public abstract override void InitializeList();

		public void Add(Element element)
		{
			this.m_elements.Insert(element);
		}

		public void AddRange(System.Collections.Generic.IEnumerable<Element> elements)
		{
			foreach (Element current in elements)
			{
				this.Add(current);
			}
		}

		public void AddRange(System.Collections.IEnumerable elements)
		{
			this.AddRange(elements.Cast<Element>());
		}

		public override void ExportToDataTable(DataTable dataTable)
		{
			base.DataTable = dataTable;
			foreach (Element element in this.m_elements)
			{
				this.UpdateOrAddDbRow(element, false);
			}
		}

		protected abstract void PopulateDbRow(Element element, DataRow row);

		public DataRow UpdateOrAddDbRow(Element element, bool isElementCreated)
		{
			DataRow dataRow = this.GetDataRow(element);
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
			this.PopulateDbRow(element, dataRow);
			return dataRow;
		}

		protected object GetParameterDdValue(Parameter parameter)
		{
			if (parameter == null)
			{
				return System.DBNull.Value;
			}
			if (!parameter.HasValue)
			{
				return System.DBNull.Value;
			}
			object obj = null;
			switch (parameter.StorageType)
			{
			case (StorageType)0:
				obj = parameter.AsValueString();
				break;
			case (StorageType)1:
				obj = parameter.AsInteger();
				break;
			case (StorageType)2:
			{
				double num = parameter.AsDouble();
				try
				{
					DisplayUnitType displayUnitType = parameter.DisplayUnitType;
					obj = Unit.CovertFromAPI(displayUnitType, num);
				}
				catch
				{
					obj = num;
				}
				if (Command.IsInRegressionMode)
				{
					ElementList.TestUnitConversion(parameter, (double)obj);
				}
				break;
			}
			case (StorageType)3:
				obj = parameter.AsString();
				break;
			case (StorageType)4:
			{
				int integerValue = parameter.AsElementId().IntegerValue;
				obj = ((integerValue == -1) ? (obj = System.DBNull.Value) : integerValue);
				break;
			}
			}
			return obj ?? System.DBNull.Value;
		}

		private static void TestUnitConversion(Parameter parameter, double retval)
		{
			double num = 0.01;
			double num2 = double.NaN;
			string text = parameter.AsValueString();
			if (text.Contains("\"") || text.Contains("'"))
			{
				string text2 = parameter.DisplayUnitType.ToString();
				if (text2.Contains("FEET") || text2.Contains("INCHE"))
				{
					num2 = APIObjectList.ConvertImperial(text);
				}
			}
			else
			{
				Regex regex = new Regex("-?\\d+(\\.\\d+)?");
				string value = regex.Match(parameter.AsValueString()).Groups[0].Value;
				try
				{
					num2 = double.Parse(value);
				}
				catch (System.Exception ex)
				{
					Log.WriteWarning("1. RetVal={1} ValueString={0}, RegexInput={2}, DUT={3}, Exception={4}", new object[]
					{
						parameter.AsValueString(),
						retval,
						text,
						parameter.DisplayUnitType,
						ex.Message
					});
				}
			}
			if (System.Math.Abs(num2 - retval) > num)
			{
				Log.WriteWarning("Unit conversion failure: RetVal={0} ApiValue={1} ValueString={2}, calculated={3}, DUT={4}, Parameter={5}", new object[]
				{
					retval,
					parameter.AsDouble(),
					parameter.AsValueString(),
					num2,
					parameter.DisplayUnitType,
					parameter.Definition.Name
				});
			}
		}

		public override void ClearUpdatedOrAddedRecords()
		{
			if (this.Elements == null || base.DataTable == null)
			{
				return;
			}
			if (this.Elements.Size != 0)
			{
				DataRowCollection rows = base.DataTable.Rows;
				for (int i = rows.Count - 1; i >= 0; i--)
				{
					Element element = this.GetRevitObject(rows[i]) as Element;
					if (this.Elements.Contains(element))
					{
						base.DataTable.Rows.RemoveAt(i);
					}
				}
			}
		}

		public override object GetRevitObject(DataRow dataRow)
		{
			ColumnInfo columnInfo = base.TableInfo[base.TableInfo.PrimaryKeys[0]];
			int id = System.Convert.ToInt32(dataRow[columnInfo.Name]);
			return APIObjectList.GetElementById(id);
		}

		protected override object[] GetPrimaryKeyValues(object apiObject)
		{
			Element element = apiObject as Element;
			if (element != null)
			{
				return new object[]
				{
					APIObjectList.GetIdDbValue(element)
				};
			}
			return new object[]
			{
				System.DBNull.Value
			};
		}
	}
}
