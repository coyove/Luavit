using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.DB.Structure;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Revit.Addon.RevitDBLink.CS
{
	public abstract class APIObjectList
	{
		protected const double paraDoubleNull = 0.0;

		protected const int paraIntegerNull = 0;

		protected const int paraElementIdValueNull = -1;

		protected const string paraStringNull = null;

		private DataTable m_dataTable;

		private TableInfo m_tableInfo;

		public virtual bool Readonly
		{
			get
			{
				return false;
			}
		}

		public TableInfo TableInfo
		{
			get
			{
				return this.m_tableInfo;
			}
			set
			{
				this.m_tableInfo = value;
			}
		}

		public DataTable DataTable
		{
			get
			{
				return this.m_dataTable;
			}
			set
			{
				this.m_dataTable = value;
			}
		}

		public virtual bool SupportCreate
		{
			get
			{
				return false;
			}
		}

		public virtual bool SupportImport
		{
			get
			{
				return false;
			}
		}

		public abstract int Count
		{
			get;
		}

		public static Document ActiveDocument
		{
			get;
			set;
		}

		public abstract void InitializeList();

		public abstract void ExportToDataTable(DataTable dataTable);

		public virtual ErrorTable ImportFromDataTable(DataTable dataTable)
		{
			return new ErrorTable(this.TableInfo.TableId, this.TableInfo.Name)
			{
				NotSupportImporting = true
			};
		}

		public abstract void ClearUpdatedOrAddedRecords();

		public abstract object GetRevitObject(DataRow dataRow);

		public virtual DataRow GetDataRow(object apiObject)
		{
            // *** Coyove Patched ***

			System.Text.StringBuilder stringBuilder = new System.Text.StringBuilder();
			object[] primaryKeyValues = this.GetPrimaryKeyValues(apiObject);
			int count = this.TableInfo.PrimaryKeys.Count;
			if (count != primaryKeyValues.Length)
			{
				return null;
			}
			for (int i = 0; i < count; i++)
			{
				ColumnInfo columnInfo = this.TableInfo[this.TableInfo.PrimaryKeys[i]];
				string name = columnInfo.Name;
				object obj = primaryKeyValues[i];
				if (obj.GetType() == typeof(string))
				{
                    // stringBuilder.AppendFormat("[{0}]='{1}'", name, obj);
                    stringBuilder.AppendFormat("{0}='{1}'", name, obj);
                }
				else
				{
                    // stringBuilder.AppendFormat("[{0}]={1}", name, obj);
                    stringBuilder.AppendFormat("{0}={1}", name, obj);
                }
				if (i < count - 1)
				{
					stringBuilder.Append(" and ");
				}
			}
            // Log.WriteError(stringBuilder.ToString());

			DataRow[] array = this.DataTable.Select(stringBuilder.ToString());
			if (array.Length > 1)
			{
				Log.WriteWarning("Got more then one records from \t" + stringBuilder);
			}
			if (array.Length > 0)
			{
				return array[0];
			}
			return null;
		}

		protected abstract object[] GetPrimaryKeyValues(object obj);

		public System.Collections.Generic.IEnumerable<T> GetElementsByTypeAndCast<T>() where T : Element
		{
			return this.GetElementsByType<T>().Cast<T>();
		}

		public FilteredElementCollector GetElementsByType<T>() where T : Element
		{
			return new FilteredElementCollector(APIObjectList.ActiveDocument).OfClass(typeof(T));
		}

		public System.Collections.Generic.IEnumerable<T> GetElementsByTypeExcluded<T>() where T : Element
		{
			System.Collections.Generic.List<T> list = new System.Collections.Generic.List<T>();
			foreach (Element current in this.GetElements())
			{
				if (typeof(T).IsAssignableFrom(current.GetType()))
				{
					list.Add(current as T);
				}
			}
			return list;
		}

		public FilteredElementCollector GetElements()
		{
			return new FilteredElementCollector(APIObjectList.ActiveDocument).WherePasses(new LogicalOrFilter(new ElementIsElementTypeFilter(), new ElementIsElementTypeFilter(true)));
		}

		public System.Collections.Generic.IEnumerable<T> GetElementsByCategory<T>(params BuiltInCategory[] categorys) where T : Element
		{
			return this.GetElementsByCategory(categorys).OfType<T>();
		}

		public FilteredElementCollector GetElementsByCategory(params BuiltInCategory[] categorys)
		{
			if (categorys.Length > 0)
			{
				ElementFilter elementFilter = new ElementCategoryFilter(categorys[0]);
				for (int i = 1; i < categorys.Length; i++)
				{
					BuiltInCategory builtInCategory = categorys[i];
					elementFilter = new LogicalOrFilter(elementFilter, new ElementCategoryFilter(builtInCategory));
				}
				return new FilteredElementCollector(APIObjectList.ActiveDocument).WherePasses(elementFilter);
			}
			throw new System.ArgumentException("please specify at least one BuiltInCategory", "categorys");
		}

		public System.Collections.Generic.IEnumerable<FamilyInstance> GetFamilyInstance(params BuiltInCategory[] categorys)
		{
			return this.GetElementsByCategory<FamilyInstance>(categorys);
		}

		public static Element GetElementById(int id)
		{
			return APIObjectList.ActiveDocument.GetElement(new ElementId(id));
		}

		public static Element GetElementById(ElementId eid)
		{
			return APIObjectList.ActiveDocument.GetElement(eid);
		}

		protected static string ColumnRes(string key)
		{
			return RDBResource.GetColumnName(key);
		}

		protected static string ColumnRes(BuiltInParameter key)
		{
			return RDBResource.GetColumnName(key);
		}

		public static System.Collections.Generic.List<Element> GetElements(bool isElementType, params BuiltInCategory[] categorys)
		{
			if (categorys.Length > 0)
			{
				ElementFilter elementFilter = new ElementCategoryFilter(categorys[0]);
				for (int i = 1; i < categorys.Length; i++)
				{
					elementFilter = new LogicalOrFilter(elementFilter, new ElementCategoryFilter(categorys[i]));
				}
				elementFilter = new LogicalAndFilter(elementFilter, new ElementIsElementTypeFilter(!isElementType));
				FilteredElementCollector source = new FilteredElementCollector(APIObjectList.ActiveDocument).WherePasses(elementFilter);
				return source.ToList<Element>();
			}
			throw new System.ArgumentException("please specify at least one BuiltInCategory", "categorys");
		}

		public static System.Collections.Generic.List<Element> GetElements(bool isElementType, params Category[] categorys)
		{
			if (categorys.Length > 0)
			{
				ElementFilter elementFilter = new ElementCategoryFilter(categorys[0].Id);
				for (int i = 1; i < categorys.Length; i++)
				{
					elementFilter = new LogicalOrFilter(elementFilter, new ElementCategoryFilter(categorys[i].Id));
				}
				elementFilter = new LogicalAndFilter(elementFilter, new ElementIsElementTypeFilter(!isElementType));
				FilteredElementCollector source = new FilteredElementCollector(APIObjectList.ActiveDocument).WherePasses(elementFilter);
				return source.ToList<Element>();
			}
			throw new System.ArgumentException("please specify at least one BuiltInCategory", "categorys");
		}

		public static double ConvertImperial(string valueString)
		{
			Regex regex = new Regex("((-?\\d+(?:\\.\\d+)?)')?\\s*(-)?\\s*(([^\"]+)\\\")?");
			Match match = regex.Match(valueString);
			double num = string.IsNullOrEmpty(match.Groups[2].Value) ? 0.0 : double.Parse(match.Groups[2].Value);
			string value = match.Groups[5].Value;
			Regex regex2 = new Regex("^(\\d+)$|^(\\d+) (\\d+)/(\\d+)$|^(\\d+)/(\\d+)$");
			match = regex2.Match(value);
			double num2 = 0.0;
			double num3;
			if (string.IsNullOrEmpty(match.Groups[1].Value))
			{
				if (string.IsNullOrEmpty(match.Groups[2].Value))
				{
					num3 = 0.0;
					num2 = (string.IsNullOrEmpty(match.Groups[5].Value) ? 0.0 : ((double)int.Parse(match.Groups[5].Value) / (double)int.Parse(match.Groups[6].Value)));
				}
				else
				{
					num3 = (double)(string.IsNullOrEmpty(match.Groups[2].Value) ? 0 : int.Parse(match.Groups[2].Value));
					num2 = (string.IsNullOrEmpty(match.Groups[3].Value) ? 0.0 : ((double)int.Parse(match.Groups[3].Value) / (double)int.Parse(match.Groups[4].Value)));
				}
			}
			else
			{
				num3 = (double)(string.IsNullOrEmpty(match.Groups[1].Value) ? 0 : int.Parse(match.Groups[1].Value));
			}
			double num4 = num3 + num2;
			return num + num4 / 12.0;
		}

		protected static object GetIdDbValue(Element element)
		{
			int elementId = APIObjectList.GetElementId(element);
			if (elementId == -1)
			{
				return System.DBNull.Value;
			}
			return elementId;
		}

		protected static object GetIdDbValue(ElementId elementId)
		{
			if (elementId.IntegerValue == -1)
			{
				return System.DBNull.Value;
			}
			return elementId.IntegerValue;
		}

		protected static object GetIdDbValue(int elementId)
		{
			if (elementId == -1)
			{
				return System.DBNull.Value;
			}
			return elementId;
		}

		protected static object GetNameDbValue(Element element)
		{
			if (element == null)
			{
				return System.DBNull.Value;
			}
			try
			{
				if (element.Name != null)
				{
					return element.Name;
				}
			}
			catch
			{
			}
			return System.DBNull.Value;
		}

		public static Parameter GetParameterByDefinitionName(ParameterMap parameterMap, string name)
		{
			if (parameterMap == null)
			{
				Log.WriteLine("Element.ParametersMap is null");
				return null;
			}
			Parameter result = null;
			if (parameterMap.Contains(name))
			{
				try
				{
					result = parameterMap.get_Item(name);
				}
				catch (System.Exception value)
				{
					Log.WriteLine("ParmeterMap contains the key, but can't get it");
					Log.WriteLine(value);
				}
			}
			return result;
		}

		public static BuiltInCategory GetCategoryId(Element element)
		{
			if (element != null)
			{
                Parameter parameter = element.get_Parameter(BuiltInParameter.ELEM_CATEGORY_PARAM); //-1140362);
				if (parameter != null)
				{
					return (BuiltInCategory)parameter.AsElementId().IntegerValue;
				}
			}
			return (BuiltInCategory)(-1);
		}

		public static BuiltInCategory GetCategoryId(ElementId elementId, Document doc)
		{
			if (elementId != ElementId.InvalidElementId && doc != null)
			{
				Element element = doc.GetElement(elementId);
				return APIObjectList.GetCategoryId(element);
			}
			return (BuiltInCategory)(-1);
		}

		public static System.Collections.Generic.List<string> GenerateResourceIdsForTableName(Element element, BuiltInCategory categoryId)
		{
			System.Collections.Generic.List<string> list = new System.Collections.Generic.List<string>();
			ElementTypeEnum elementTypeEnum = (element is ElementType) ? ElementTypeEnum.SYMBOL : ElementTypeEnum.INSTANCE;
			list.Add(string.Format("TabN_{0}_{1}", categoryId, elementTypeEnum));
			BuiltInCategory builtInCategory = (BuiltInCategory)(-1);
			bool flag = false;
			FamilyInstance familyInstance = element as FamilyInstance;
			if (familyInstance != null && familyInstance.Host != null)
			{
				builtInCategory = APIObjectList.GetCategoryId(familyInstance.Host);
				flag = true;
			}
			else
			{
				LoadBase loadBase = element as LoadBase;
				if (loadBase != null && loadBase.HostElement != null)
				{
					builtInCategory = APIObjectList.GetCategoryId(loadBase.HostElement);
					flag = true;
				}
				else
				{
					Opening opening = element as Opening;
					if (opening != null && opening.Host != null)
					{
						builtInCategory = APIObjectList.GetCategoryId(opening.Host);
						list.Add(string.Format("TabN_{0}_{1}", "OST_Type_Opening", builtInCategory));
						list.Add("TabN_Type_" + opening.GetType().FullName.Replace('.', '_'));
					}
					else
					{
						Rebar rebar = element as Rebar;
						if (rebar != null && rebar.GetHostId() != ElementId.InvalidElementId)
						{
							builtInCategory = APIObjectList.GetCategoryId(rebar.GetHostId(), rebar.Document);
							flag = true;
						}
					}
				}
			}
			if (flag)
			{
				list.Add(string.Format("TabN_{0}_{1}", categoryId, builtInCategory));
			}
			if (element is RoomTag)
			{
				list.Add("TabN_Type_" + element.GetType().FullName.Replace('.', '_'));
			}
			if (element != null && element.Id.IntegerValue != -1)
			{
				if (element.LevelId != ElementId.InvalidElementId)
				{
					list.Add("TabN_CST_ElementLevel");
				}
				if (element.CreatedPhaseId != ElementId.InvalidElementId)
				{
					list.Add("TabN_CST_ElementPhase");
				}
			}
			return list;
		}

		public static string GenerateResourceIdsForTableName(ElementTypeEnum symbolOrInstance, BuiltInCategory categoryId)
		{
			return string.Format("TabN_{0}_{1}", categoryId, symbolOrInstance);
		}

		public static int GetElementId(Element element)
		{
			if (element != null)
			{
				Parameter parameter = element.get_Parameter((BuiltInParameter)(-1002100));
				if (parameter != null)
				{
					return parameter.AsElementId().IntegerValue;
				}
			}
			return -1;
		}
	}
}
