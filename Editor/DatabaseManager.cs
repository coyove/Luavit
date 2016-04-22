using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.IO;
using System.IO.IsolatedStorage;
using System.Reflection;

namespace Revit.Addon.RevitDBLink.CS
{
	public class DatabaseManager : System.IDisposable
	{
		public enum DatabaseType
		{
			ODBC,
			MSAccess,
			MSAccess2007,
			Invalid
		}

		private const string LastDatabaseTypePrefix = "#Last Database type: ";

		private static IDatabaseDriver m_CurrentDriver = null;

		private static System.Collections.Generic.Dictionary<DatabaseManager.DatabaseType, System.Collections.Generic.List<string>> m_LastConnection = new System.Collections.Generic.Dictionary<DatabaseManager.DatabaseType, System.Collections.Generic.List<string>>();

		private static bool isExcelConnection = false;

		private static DatabaseManager.DatabaseType lastDatabaseType = DatabaseManager.DatabaseType.Invalid;

		public static DatabaseManager.DatabaseType CurrentDatabaseType
		{
			get
			{
				if (DatabaseManager.m_CurrentDriver == null)
				{
					return DatabaseManager.DatabaseType.Invalid;
				}
				return DatabaseManager.m_CurrentDriver.Type;
			}
			set
			{
				if (DatabaseManager.m_CurrentDriver != null)
				{
					DatabaseManager.m_CurrentDriver.Dispose();
					DatabaseManager.m_CurrentDriver = null;
				}
				DatabaseManager.m_CurrentDriver = DatabaseManager.CreateDatabaseDriver(value);
			}
		}

		public static DatabaseManager.DatabaseType LastDatabaseType
		{
			get
			{
				return DatabaseManager.lastDatabaseType;
			}
			set
			{
				DatabaseManager.lastDatabaseType = value;
			}
		}

		public static System.Collections.Generic.Dictionary<string, string> KeywordsMap
		{
			get
			{
				return DatabaseManager.m_CurrentDriver.KeywordsMap;
			}
		}

		public static bool isExcelExport
		{
			get
			{
				return DatabaseManager.isExcelConnection;
			}
			set
			{
				DatabaseManager.isExcelConnection = value;
			}
		}

		public DatabaseManager()
		{
			if (!Command.IsInRegressionMode)
			{
				this.LoadSettings();
			}
		}

		~DatabaseManager()
		{
			this.Dispose();
		}

		public void Dispose()
		{
			Log.WriteLine("DatabaseManager dispose");
			if (!Command.IsInRegressionMode)
			{
				this.SaveSettings();
			}
			if (DatabaseManager.m_CurrentDriver != null)
			{
				DatabaseManager.m_CurrentDriver.Dispose();
				DatabaseManager.m_CurrentDriver = null;
			}
			System.GC.SuppressFinalize(this);
		}

		private void LoadSettings()
		{
			DatabaseManager.m_LastConnection.Clear();
			string connectionHistoryFile = this.GetConnectionHistoryFile();
			System.IO.StreamReader streamReader = null;
			try
			{
				if (Command.ConfigFile.Debug)
				{
					if (!System.IO.File.Exists(connectionHistoryFile))
					{
						return;
					}
					streamReader = new System.IO.StreamReader(connectionHistoryFile);
				}
				else
				{
					System.IO.IsolatedStorage.IsolatedStorageFile store = System.IO.IsolatedStorage.IsolatedStorageFile.GetStore(System.IO.IsolatedStorage.IsolatedStorageScope.User | System.IO.IsolatedStorage.IsolatedStorageScope.Assembly, null, null);
					streamReader = new System.IO.StreamReader(new System.IO.IsolatedStorage.IsolatedStorageFileStream(System.IO.Path.GetFileName(connectionHistoryFile), System.IO.FileMode.Open, store));
				}
				if (streamReader != null)
				{
					while (!streamReader.EndOfStream)
					{
						string text = streamReader.ReadLine();
						if (string.IsNullOrEmpty(text))
						{
							break;
						}
						if (text.StartsWith("#"))
						{
							if (!text.StartsWith("#Last Database type: "))
							{
								continue;
							}
							string value = text.Replace("#Last Database type: ", "").Trim();
							try
							{
								DatabaseManager.lastDatabaseType = (DatabaseManager.DatabaseType)System.Enum.Parse(typeof(DatabaseManager.DatabaseType), value, true);
								Log.WriteLine("Get last DatabaseType: " + DatabaseManager.lastDatabaseType);
								continue;
							}
							catch (System.Exception)
							{
								continue;
							}
						}
						int num = text.IndexOf(',');
						string value2 = text.Substring(0, num);
						string item = text.Substring(num + 1);
						DatabaseManager.DatabaseType key = (DatabaseManager.DatabaseType)System.Enum.Parse(typeof(DatabaseManager.DatabaseType), value2);
						System.Collections.Generic.List<string> list;
						if (!DatabaseManager.m_LastConnection.TryGetValue(key, out list))
						{
							list = new System.Collections.Generic.List<string>();
							DatabaseManager.m_LastConnection.Add(key, list);
						}
						list.Add(item);
					}
				}
			}
			catch (System.IO.FileNotFoundException)
			{
			}
			catch (System.Exception ex)
			{
				Log.WriteLine(ex.ToString());
			}
			finally
			{
				if (streamReader != null)
				{
					streamReader.Close();
				}
			}
		}

		private void SaveSettings()
		{
			string connectionHistoryFile = this.GetConnectionHistoryFile();
			Log.WriteLine("SaveSettings");
			Log.Indent();
			DatabaseManager.DatabaseType databaseType = (DatabaseManager.CurrentDatabaseType == DatabaseManager.DatabaseType.Invalid) ? DatabaseManager.lastDatabaseType : DatabaseManager.CurrentDatabaseType;
			Log.WriteLine("#Last Database type: " + databaseType);
			foreach (System.Collections.Generic.KeyValuePair<DatabaseManager.DatabaseType, System.Collections.Generic.List<string>> current in DatabaseManager.m_LastConnection)
			{
				foreach (string current2 in current.Value)
				{
					Log.WriteLine("{0},{1}", new object[]
					{
						current.Key,
						current2
					});
				}
			}
			Log.Unindent();
			System.IO.StreamWriter streamWriter = null;
			try
			{
				if (Command.ConfigFile.Debug)
				{
					streamWriter = new System.IO.StreamWriter(connectionHistoryFile, false);
				}
				else
				{
					System.IO.IsolatedStorage.IsolatedStorageFile store = System.IO.IsolatedStorage.IsolatedStorageFile.GetStore(System.IO.IsolatedStorage.IsolatedStorageScope.User | System.IO.IsolatedStorage.IsolatedStorageScope.Assembly, null, null);
					streamWriter = new System.IO.StreamWriter(new System.IO.IsolatedStorage.IsolatedStorageFileStream(System.IO.Path.GetFileName(connectionHistoryFile), System.IO.FileMode.Create, store));
				}
				streamWriter.WriteLine("#Last Database type: " + databaseType);
				foreach (System.Collections.Generic.KeyValuePair<DatabaseManager.DatabaseType, System.Collections.Generic.List<string>> current3 in DatabaseManager.m_LastConnection)
				{
					foreach (string current4 in current3.Value)
					{
						streamWriter.WriteLine("{0},{1}", current3.Key, current4);
					}
				}
			}
			catch
			{
			}
			finally
			{
				if (streamWriter != null)
				{
					streamWriter.Close();
				}
			}
		}

		private string GetConnectionHistoryFile()
		{
			string location = System.Reflection.Assembly.GetExecutingAssembly().Location;
			string path = System.IO.Path.ChangeExtension(location, "connections.txt");
			return System.IO.Path.Combine(Command.GetLocalRevitDBLinkApplicationDataFolder(), System.IO.Path.GetFileName(path));
		}

		private static IDatabaseDriver CreateDatabaseDriver(DatabaseManager.DatabaseType type)
		{
			switch (type)
			{
			case DatabaseManager.DatabaseType.ODBC:
				return new ODBCDriver();
			//case DatabaseManager.DatabaseType.MSAccess:
			//	return new MSAccessDriver2003();
			//case DatabaseManager.DatabaseType.MSAccess2007:
			//	return new MSAccessDriver2007();
			default:
				return null;
			}
		}

		public static string GetValidCoumnName(string columnName)
		{
			return DatabaseManager.m_CurrentDriver.GetValidCoumnName(columnName);
		}

		public static bool CanExport(DatabaseManager.DatabaseType type)
		{
			bool result;
			try
			{
				switch (type)
				{
				case DatabaseManager.DatabaseType.ODBC:
					result = ODBCDriver.CanExport;
					break;
				//case DatabaseManager.DatabaseType.MSAccess:
				//	result = MSAccessDriver2003.CanExport;
				//	break;
				//case DatabaseManager.DatabaseType.MSAccess2007:
				//	result = MSAccessDriver2007.CanExport;
				//	break;
				default:
					result = false;
					break;
				}
			}
			catch (System.Exception ex)
			{
				Log.WriteLine("DatabaseType {0} is not supported because creating database driver failed", new object[]
				{
					type
				});
				Log.WriteLine(ex.ToString());
				result = false;
			}
			return result;
		}

		public static System.Collections.Generic.List<string> GetPreviousConnectionsFor(DatabaseManager.DatabaseType type)
		{
			System.Collections.Generic.List<string> list;
			if (!DatabaseManager.m_LastConnection.TryGetValue(type, out list))
			{
				list = new System.Collections.Generic.List<string>();
				DatabaseManager.m_LastConnection.Add(type, list);
			}
			return list;
		}

		public static bool SetDatabase(string connectionstring)
		{
			return DatabaseManager.m_CurrentDriver.SetDatabase(connectionstring);
		}

		public static bool SelectDatabase()
		{
			return DatabaseManager.m_CurrentDriver.SelectDatabase();
		}

		public static bool RetrieveValue(string sqlstatement, ref string value)
		{
			return DatabaseManager.m_CurrentDriver.RetrieveValue(sqlstatement, ref value);
		}

		public static bool CreateDatabase(TableInfoSet tableinfoset, out string errorMessage)
		{
			return DatabaseManager.m_CurrentDriver.CreateDatabase(tableinfoset, out errorMessage);
		}

		public static bool IsDatabaseValid(TableInfoSet tableinfoset)
		{
			Log.WriteLine("Check whether database is valid");
			return DatabaseManager.m_CurrentDriver.IsDatabaseValid(tableinfoset);
		}

		public static DataTable GetSchema(string collectionname, string[] restrictionvalues)
		{
			return DatabaseManager.m_CurrentDriver.GetSchema(collectionname, restrictionvalues);
		}

		public static void StoreLastConnection()
		{
			string database = DatabaseManager.m_CurrentDriver.GetDatabase();
			System.Collections.Generic.List<string> list;
			if (DatabaseManager.m_LastConnection.ContainsKey(DatabaseManager.CurrentDatabaseType))
			{
				list = DatabaseManager.m_LastConnection[DatabaseManager.CurrentDatabaseType];
			}
			else
			{
				list = new System.Collections.Generic.List<string>();
				DatabaseManager.m_LastConnection.Add(DatabaseManager.CurrentDatabaseType, list);
			}
			if (list.Contains(database))
			{
				list.Remove(database);
			}
			list.Insert(0, database);
			if (list.Count > 7)
			{
				list.RemoveRange(7, list.Count - 7);
			}
		}

		public static bool Open()
		{
			return DatabaseManager.m_CurrentDriver.Open();
		}

		public static bool Close()
		{
			if (DatabaseManager.m_CurrentDriver != null)
			{
				Log.WriteLine("CurrentDriver is not null");
				DatabaseManager.m_CurrentDriver.Dispose();
				DatabaseManager.m_CurrentDriver = null;
			}
			Log.WriteLine("DatabaseManager Closed");
			return true;
		}

		public static DbCommand CreateCommand()
		{
			return DatabaseManager.m_CurrentDriver.CreateCommand();
		}

		public static DbDataAdapter CreateAdapter(DbCommand cmd)
		{
			return DatabaseManager.m_CurrentDriver.CreateAdapter(cmd);
		}

		public static DbCommandBuilder CreateCommandBuilder(DbDataAdapter adapter)
		{
			return DatabaseManager.m_CurrentDriver.CreateCommandBuilder(adapter);
		}

		public static string GetDataType(DataType datatype)
		{
			return DatabaseManager.m_CurrentDriver.DataTypeMap[datatype];
		}

		public static DataType GetDataType(ParameterType paramtype)
		{
			DataType result = DataType.TEXT;
			switch (paramtype)
			{
			case (ParameterType)0:
			case (ParameterType)2:
			case (ParameterType)9:
			case (ParameterType)10:
				result = DataType.INTEGER;
				break;
			case (ParameterType)1:
			case (ParameterType)8:
				result = DataType.TEXT;
				break;
			case (ParameterType)3:
			case (ParameterType)4:
			case (ParameterType)5:
			case (ParameterType)6:
			case (ParameterType)7:
			case (ParameterType)11:
			case (ParameterType)12:
			case (ParameterType)13:
			case (ParameterType)14:
				result = DataType.DOUBLE;
				break;
			}
			return result;
		}

		public static IDatabaseDriver GetDriver()
		{
			return DatabaseManager.m_CurrentDriver;
		}
	}
}
