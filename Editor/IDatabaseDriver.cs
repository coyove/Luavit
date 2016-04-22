using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;

namespace Revit.Addon.RevitDBLink.CS
{
	public interface IDatabaseDriver : System.IDisposable
	{
		event ProgressHandler PerformProgressStep;

		DatabaseManager.DatabaseType Type
		{
			get;
		}

		System.Collections.Generic.Dictionary<string, string> KeywordsMap
		{
			get;
		}

		System.Collections.Generic.Dictionary<DataType, string> DataTypeMap
		{
			get;
		}

		string GetValidCoumnName(string columnName);

		bool SetDatabase(string connectionstring);

		bool SelectDatabase();

		bool CreateDatabase(TableInfoSet tableinfoset, out string errorMessage);

		bool IsDatabaseValid(TableInfoSet tableinfoset);

		string GetDatabase();

		bool RetrieveValue(string SQLStatement, ref string value);

		bool Open();

		DataTable GetSchema(string collectionname, string[] restrictionvalues);

		DbCommand CreateCommand();

		DbDataAdapter CreateAdapter(DbCommand cmd);

		DbCommandBuilder CreateCommandBuilder(DbDataAdapter adapter);

		bool CompareToExpected(string connectionString);
	}
}
