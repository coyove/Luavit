using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Data;

namespace Revit.Addon.RevitDBLink.CS
{
	public class AssemblyCodesList : ElementList
	{
		private System.Collections.Generic.List<AssemblyCode> m_assemblyCodes = new System.Collections.Generic.List<AssemblyCode>();

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
				return base.Elements.Size;
			}
		}

		public AssemblyCodesList(BuiltInCategory builtInCategory)
		{
		}

		public AssemblyCodesList() : this((BuiltInCategory)(-1))
		{
		}

		public override void InitializeList()
		{
			FilteredElementCollector elements = base.GetElements();
			System.Collections.Generic.IEnumerator<Element> enumerator = elements.GetEnumerator();
			while (enumerator.MoveNext())
			{
				Element element = null;
				try
				{
					element = enumerator.Current;
					Parameter parameter = element.get_Parameter((BuiltInParameter)(-1002500));
					if (parameter != null && !string.IsNullOrEmpty(parameter.AsString()))
					{
						base.Add(element);
					}
				}
				catch (System.Exception)
				{
					if (element != null)
					{
						Log.WriteLine("Get BuiltInParameter.UNIFORMAT_CODE failed: " + element.Id);
					}
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
			Element element = apiObject as Element;
			if (element != null)
			{
				Parameter parameter = element.get_Parameter((BuiltInParameter)(-1002500));
				if (parameter != null)
				{
					return new object[]
					{
						parameter.AsString()
					};
				}
			}
			return new object[]
			{
				System.DBNull.Value
			};
		}

		protected override void PopulateDbRow(Element element, DataRow row)
		{
			foreach (ColumnInfo current in base.TableInfo.Values)
			{
				if (current.BuiltInParameter != (BuiltInParameter)(-1))
				{
					BuiltInParameter builtInParameter = current.BuiltInParameter;
					Parameter parameter = element.get_Parameter(builtInParameter);
					if (parameter != null)
					{
						string value = base.GetParameterDdValue(parameter) as string;
						if (!string.IsNullOrEmpty(value))
						{
							Utility.Assign(row, current.Name, value);
						}
					}
				}
			}
		}
	}
}
