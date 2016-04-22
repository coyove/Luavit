using Autodesk.Revit.DB;
using Revit.Addon.RevitDBLink.CS.Properties;
using System;
using System.Globalization;
using System.Reflection;
using System.Resources;
using System.Threading;

namespace Revit.Addon.RevitDBLink.CS
{
	public static class RDBResource
	{
		public const string TableKeyPrefix = "TabN_";

		public const string TableKeyInstanceSuffix = "_INSTANCE";

		public const string TableKeySymbolSuffix = "_SYMBOL";

		private static System.Resources.ResourceManager resource;

		public static System.Globalization.CultureInfo Culture;

		private static System.Globalization.CultureInfo DefaultCulture;

		static RDBResource()
		{
			RDBResource.Culture = System.Threading.Thread.CurrentThread.CurrentCulture;
			RDBResource.DefaultCulture = System.Threading.Thread.CurrentThread.CurrentCulture;
			RDBResource.resource = new System.Resources.ResourceManager("Revit.Addon.RevitDBLink.CS.Properties.Resources", System.Reflection.Assembly.GetExecutingAssembly());
		}

		public static void UpdateCulture()
		{
			try
			{
				string cultureString = new ConfigFile().GetCultureString();
				if (!string.IsNullOrEmpty(cultureString))
				{
					RDBResource.Culture = new System.Globalization.CultureInfo(cultureString);
				}
				else
				{
					RDBResource.Culture = RDBResource.DefaultCulture;
				}
			}
			catch
			{
				RDBResource.Culture = RDBResource.DefaultCulture;
			}
			System.Threading.Thread.CurrentThread.CurrentCulture = RDBResource.Culture;
			System.Threading.Thread.CurrentThread.CurrentUICulture = RDBResource.Culture;
			Resources.Culture = RDBResource.Culture;
		}

		public static string GetString(string key)
		{
			return RDBResource.resource.GetString(key);
		}

		public static string GetColumnName(BuiltInParameter key)
		{
			string text = LabelUtils.GetLabelFor(key);
			if (text == null)
			{
				Log.WriteError("Value is null for column: {0}", new object[]
				{
					key
				});
				return null;
			}
			text = text.Replace(" ", "");
			return DatabaseManager.GetValidCoumnName(text);
		}

		public static string GetColumnId(BuiltInParameter parameterId)
		{
			return "ColN_BP_" + System.Enum.GetName(typeof(BuiltInParameter), parameterId);
		}

		public static string GetColumnName(string key)
		{
			string @string = RDBResource.GetString(key);
			if (@string == null)
			{
				Log.WriteLine("Value is null for key: {0}", new object[]
				{
					key
				});
			}
			return DatabaseManager.GetValidCoumnName(@string);
		}

		public static string GetTableName(string key)
		{
			if (key == null)
			{
				return null;
			}
			string text = RDBResource.GetString(key);
			if (text == null)
			{
				return null;
			}
			string text2 = null;
			DatabaseManager.KeywordsMap.TryGetValue(text, out text2);
			if (text2 != null)
			{
				text = text2;
			}
			if (DatabaseManager.isExcelExport)
			{
				int num = 30;
				if (text.Length > num)
				{
					text = text.Substring(0, num);
				}
			}
			return text;
		}

		public static string GetTableKey(Category category, bool isSymbol)
		{
			if (category == null)
			{
				return null;
			}
			BuiltInCategory integerValue = (BuiltInCategory)category.Id.IntegerValue;
			string str = isSymbol ? "_SYMBOL" : "_INSTANCE";
			return "TabN_" + integerValue.ToString() + str;
		}

		public static string GetTableKey(int categoryId, bool isSymbol)
		{
			string str = isSymbol ? "_SYMBOL" : "_INSTANCE";
			return "TabN_" + categoryId.ToString() + str;
		}
	}
}
