using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Reflection;

namespace Revit.Addon.RevitDBLink.CS
{
	public class OmniClassNumbersList : ElementList
	{
		private System.Collections.Generic.List<string[]> m_omniClassTaxonomyList = new System.Collections.Generic.List<string[]>();

		public static string OmniClassNumberFilePath;

		public static bool OmniClassNumberFileExist;

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
				return this.m_omniClassTaxonomyList.Count;
			}
		}

		static OmniClassNumbersList()
		{
			OmniClassNumbersList.OmniClassNumberFileExist = OmniClassNumbersList.GetOmniClassTaxonomyFile(out OmniClassNumbersList.OmniClassNumberFilePath);
		}

		public override void InitializeList()
		{
			if (OmniClassNumbersList.OmniClassNumberFileExist)
			{
				using (System.IO.StreamReader streamReader = new System.IO.StreamReader(OmniClassNumbersList.OmniClassNumberFilePath))
				{
					while (!streamReader.EndOfStream)
					{
						string text = streamReader.ReadLine();
						string[] array = text.Split(new char[]
						{
							'\t'
						});
						string text2 = array[0];
						string text3 = (array.Length > 1) ? array[1] : string.Empty;
						this.m_omniClassTaxonomyList.Add(new string[]
						{
							text2,
							text3
						});
					}
					streamReader.Close();
				}
			}
		}

		private static bool GetOmniClassTaxonomyFile(out string fullpath)
		{
			string path = "OmniClassTaxonomy.txt";
			string path2 = System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData);
			path2 = System.IO.Path.Combine(path2, "Autodesk\\Revit\\" + Command.RevitVersionName);
			fullpath = System.IO.Path.Combine(path2, path);
			if (System.IO.File.Exists(fullpath))
			{
				Log.WriteLine("OmniClassTaxonomyFile: " + fullpath);
				return true;
			}
			Log.WriteLine("File not found: " + fullpath);
			string directoryName = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetAssembly(typeof(Element)).Location);
			string text = System.IO.Path.Combine(directoryName, path);
			if (System.IO.File.Exists(text))
			{
				fullpath = text;
				Log.WriteLine("OmniClassTaxonomyFile: " + fullpath);
				return true;
			}
			Log.WriteLine("File not found: " + fullpath);
			Log.WriteLine("File not found: " + text);
			return false;
		}

		public override void ClearUpdatedOrAddedRecords()
		{
			if (base.DataTable == null)
			{
				return;
			}
			base.DataTable.Clear();
		}

		public override void ExportToDataTable(DataTable dataTable)
		{
			if (dataTable == null)
			{
				return;
			}
			base.DataTable = dataTable;
			foreach (string[] current in this.m_omniClassTaxonomyList)
			{
				this.InsertDbRow(current[0], current[1]);
			}
			foreach (Element element in base.Elements)
			{
				base.UpdateOrAddDbRow(element, false);
			}
		}

		public override object GetRevitObject(DataRow dataRow)
		{
			return null;
		}

		private void InsertDbRow(string code, string description)
		{
			DataRow dataRow = this.GetDataRow(code);
			if (dataRow == null)
			{
				dataRow = base.DataTable.NewRow();
				Utility.Assign(dataRow, APIObjectList.ColumnRes((BuiltInParameter)( -1002502)), code);
				base.DataTable.Rows.Add(dataRow);
			}
			Utility.Assign(dataRow, APIObjectList.ColumnRes((BuiltInParameter)( - 1002503)), description);
		}

		protected override object[] GetPrimaryKeyValues(object apiObject)
		{
			Element element = apiObject as Element;
			if (element != null)
			{
				Parameter parameter = element.get_Parameter((BuiltInParameter)( - 1002502));
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
				apiObject
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
