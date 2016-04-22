using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.Odbc;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Revit.Addon.RevitDBLink.CS
{
	internal class ODBCDriver : IDatabaseDriver, System.IDisposable
	{
		private const ushort SQL_DRIVER_NOPROMPT = 0;

		private const ushort SQL_DRIVER_COMPLETE = 1;

		private const ushort SQL_DRIVER_PROMPT = 2;

		private const ushort SQL_DRIVER_COMPLETE_REQUIRED = 3;

		private const short SQL_NTS = -3;

		private const short SQL_CLOSE = 0;

		private const short SQL_DROP = 1;

		private const short SQL_UNBIND = 2;

		private const short SQL_RESET_PARAMS = 3;

		private const short SQL_SUCCESS = 0;

		private const short SQL_SUCCESS_WITH_INFO = 1;

		private const short SQL_STILL_EXECUTING = 2;

		private const short SQL_NEED_DATA = 99;

		private const short SQL_NO_DATA = 100;

		private const short SQL_ERROR = -1;

		private const short SQL_INVALID_HANDLE = -2;

		private const short SQL_CHAR_TYPE = 1;

		private OdbcConnection m_connection;

		private System.IntPtr m_EnvironmentHandle = System.IntPtr.Zero;

		private System.IntPtr m_ConnectionHandle = System.IntPtr.Zero;

		private System.IntPtr m_StatementHandle = System.IntPtr.Zero;

		private bool m_IsAllocated;

		private string m_ConnectionString = string.Empty;

		private char[] m_InvalidCharacters;

		private System.Collections.Generic.Dictionary<string, string> m_KeywordsMap = new System.Collections.Generic.Dictionary<string, string>();

		private System.Collections.Generic.Dictionary<DataType, string> m_DataTypeMap = new System.Collections.Generic.Dictionary<DataType, string>();

		public event ProgressHandler PerformProgressStep;

		public DatabaseManager.DatabaseType Type
		{
			get
			{
				return DatabaseManager.DatabaseType.ODBC;
			}
		}

		public System.Collections.Generic.Dictionary<string, string> KeywordsMap
		{
			get
			{
				return this.m_KeywordsMap;
			}
		}

		public System.Collections.Generic.Dictionary<DataType, string> DataTypeMap
		{
			get
			{
				return this.m_DataTypeMap;
			}
		}

		public static bool CanExport
		{
			get
			{
				return true;
			}
		}

		[System.Runtime.InteropServices.DllImport("user32.dll")]
		internal static extern System.IntPtr GetActiveWindow();

		[System.Runtime.InteropServices.DllImport("odbc32.dll", CharSet = CharSet.Unicode)]
		private static extern short SQLDriverConnect(System.IntPtr hdbc, System.IntPtr hwnd, string szConnStrIn, short cbConnStrIn, System.Text.StringBuilder szConnStrOut, short cbConnStrOutMax, out short pcbConnStrOut, ushort fDriverCompletion);

		[System.Runtime.InteropServices.DllImport("odbc32.dll", CharSet = CharSet.Unicode)]
		private static extern short SQLFreeConnect(System.IntPtr ConnectionHandle);

		[System.Runtime.InteropServices.DllImport("odbc32.dll", CharSet = CharSet.Unicode)]
		private static extern short SQLDisconnect(System.IntPtr ConnectionHandle);

		[System.Runtime.InteropServices.DllImport("odbc32.dll", CharSet = CharSet.Unicode)]
		private static extern short SQLFreeEnv(System.IntPtr EnvironmentHandle);

		[System.Runtime.InteropServices.DllImport("odbc32.dll")]
		private static extern short SQLAllocEnv(out System.IntPtr EnvironmentHandle);

		[System.Runtime.InteropServices.DllImport("odbc32.dll")]
		private static extern short SQLAllocConnect(System.IntPtr EnvironmentHandle, out System.IntPtr ConnectionHandle);

		[System.Runtime.InteropServices.DllImport("odbc32.dll")]
		private static extern short SQLAllocStmt(System.IntPtr ConnectionHandle, out System.IntPtr StmtHandle);

		[System.Runtime.InteropServices.DllImport("odbc32.dll")]
		private static extern short SQLFreeStmt(System.IntPtr StmtHandle, short Option);

		[System.Runtime.InteropServices.DllImport("odbc32.dll", CharSet = CharSet.Unicode)]
		private static extern short SQLExecDirect(System.IntPtr StatementHandle, string StatementText, int TextLength);

		[System.Runtime.InteropServices.DllImport("odbc32.dll", CharSet = CharSet.Unicode)]
		private static extern short SQLTables(System.IntPtr StatementHandle, string CatalogName, short NameLength1, string SchemaName, short NameLength2, string TableName, short NameLength3, string TableType, short NameLength4);

		[System.Runtime.InteropServices.DllImport("odbc32.dll", CharSet = CharSet.Unicode)]
		private static extern short SQLColumns(System.IntPtr StatementHandle, string CatalogName, short NameLength1, string SchemaName, short NameLength2, string TableName, short NameLength3, string ColumnName, short NameLength4);

		[System.Runtime.InteropServices.DllImport("odbc32.dll")]
		private static extern short SQLFetch(System.IntPtr StatementHandle);

		[System.Runtime.InteropServices.DllImport("odbc32.dll")]
		private static extern short SQLCloseCursor(System.IntPtr StatementHandle);

		[System.Runtime.InteropServices.DllImport("odbc32.dll")]
		private static extern short SQLGetInfo(System.IntPtr sqlHdbc, short fInfoType, [System.Runtime.InteropServices.Out] System.Text.StringBuilder rgbInfoValue, short cbInfoValueMax, out short pcbInfoValue);

		[System.Runtime.InteropServices.DllImport("odbc32.dll")]
		private static extern short SQLGetData(System.IntPtr StatementHandle, short ColumnNumber, short TargetType, [System.Runtime.InteropServices.Out] System.Text.StringBuilder ColumnValue, short BufferLength, out short ValueLength);

		private bool IsOK(int value)
		{
			return value == 0 || value == 1;
		}

		private bool Initialize()
		{
			if (!this.m_IsAllocated)
			{
				if (!this.IsOK((int)ODBCDriver.SQLAllocEnv(out this.m_EnvironmentHandle)))
				{
					return false;
				}
				if (!this.IsOK((int)ODBCDriver.SQLAllocConnect(this.m_EnvironmentHandle, out this.m_ConnectionHandle)))
				{
					return false;
				}
				this.m_IsAllocated = true;
			}
			return true;
		}

		private int Connect(string in_connect, ushort driverCompletion, ref string retConnStr)
		{
			retConnStr = string.Empty;
			System.Text.StringBuilder stringBuilder = new System.Text.StringBuilder(1024);
			short num = 0;
			int result = (int)ODBCDriver.SQLDriverConnect(this.m_ConnectionHandle, ODBCDriver.GetActiveWindow(), in_connect, (short)in_connect.Length, stringBuilder, 1024, out num, driverCompletion);
			retConnStr = stringBuilder.ToString();
			short value = ODBCDriver.SQLAllocStmt(this.m_ConnectionHandle, out this.m_StatementHandle);
			if (!this.IsOK((int)value))
			{
				Log.WriteError("Connection database failed!");
			}
			return result;
		}

		private bool ExecuteSQL(string sqlStmt)
		{
			short value = ODBCDriver.SQLExecDirect(this.m_StatementHandle, sqlStmt, sqlStmt.Length);
			return this.IsOK((int)value);
		}

		private bool TableExist(string tableName)
		{
			short value = ODBCDriver.SQLTables(this.m_StatementHandle, null, 0, null, 0, tableName, -3, "TABLE", -3);
			if (this.IsOK((int)value))
			{
				value = ODBCDriver.SQLFetch(this.m_StatementHandle);
				ODBCDriver.SQLCloseCursor(this.m_StatementHandle);
				return this.IsOK((int)value);
			}
			ODBCDriver.SQLCloseCursor(this.m_StatementHandle);
			return false;
		}

		public bool ColumnExist(string tableName, string columnName)
		{
			short value = ODBCDriver.SQLColumns(this.m_StatementHandle, null, 0, null, 0, tableName, -3, columnName, -3);
			if (this.IsOK((int)value))
			{
				value = ODBCDriver.SQLFetch(this.m_StatementHandle);
				ODBCDriver.SQLCloseCursor(this.m_StatementHandle);
				return this.IsOK((int)value);
			}
			ODBCDriver.SQLCloseCursor(this.m_StatementHandle);
			return false;
		}

		public bool RetrieveValue(string sqlstatement, ref string result)
		{
			short value = ODBCDriver.SQLExecDirect(this.m_StatementHandle, sqlstatement, -3);
			if (!this.IsOK((int)value))
			{
				return false;
			}
			value = ODBCDriver.SQLFetch(this.m_StatementHandle);
			if (!this.IsOK((int)value))
			{
				ODBCDriver.SQLCloseCursor(this.m_StatementHandle);
				return false;
			}
			System.Text.StringBuilder stringBuilder = new System.Text.StringBuilder(1024);
			short num;
			value = ODBCDriver.SQLGetData(this.m_StatementHandle, 1, 1, stringBuilder, 1024, out num);
			if (!this.IsOK((int)value))
			{
				ODBCDriver.SQLCloseCursor(this.m_StatementHandle);
				return false;
			}
			result = stringBuilder.ToString();
			ODBCDriver.SQLCloseCursor(this.m_StatementHandle);
			return true;
		}

		public void Terminate()
		{
			if (this.m_IsAllocated)
			{
				if (this.m_StatementHandle.ToInt64() != 0L)
				{
					ODBCDriver.SQLFreeStmt(this.m_StatementHandle, 0);
					this.m_StatementHandle = System.IntPtr.Zero;
				}
				if (this.m_ConnectionHandle.ToInt64() != 0L)
				{
					ODBCDriver.SQLDisconnect(this.m_ConnectionHandle);
					ODBCDriver.SQLFreeConnect(this.m_ConnectionHandle);
					this.m_ConnectionHandle = System.IntPtr.Zero;
				}
				if (this.m_EnvironmentHandle.ToInt64() != 0L)
				{
					ODBCDriver.SQLFreeEnv(this.m_EnvironmentHandle);
					this.m_EnvironmentHandle = System.IntPtr.Zero;
				}
				this.m_IsAllocated = false;
			}
		}

		internal ODBCDriver()
		{
		}

		public void Dispose()
		{
			Log.WriteLine("ODBCDriver dispose");
			this.Terminate();
		}

		public string GetValidCoumnName(string columnName)
		{
            // *** Coyove Patched ***

			if (string.IsNullOrEmpty(columnName))
			{
				throw new System.ArgumentException(RDBResource.GetString("Exception_ColumnNameIsEmpty"), "columnName");
			}
			string databaseName;
			if ((databaseName = this.GetDatabaseName()) != null)
			{
				string text;
				if (!(databaseName == "Microsoft SQL Server"))
				{
					if (!(databaseName == "ACCESS"))
					{
						if (!(databaseName == "EXCEL"))
						{
							return columnName;
						}
						text = Regex.Replace(columnName, "['#\\.!`\\[\\]]", (Match m) => string.Empty);
					}
					else
					{
						text = Regex.Replace(columnName, "[\\.!`\\[\\]']", (Match m) => string.Empty);
					}
				}
				else
				{
					text = Regex.Replace(columnName, "^[#'`\\t ]", (Match m) => string.Empty);
				}
				text = text.Trim();
				if (text.Length > 64)
				{
					text = text.Substring(0, 64);
				}
				return text;
			}
			return columnName;
		}

		private bool CheckConnectionString()
		{
			short num = 1000;
			System.Text.StringBuilder stringBuilder = new System.Text.StringBuilder((int)num);
			short fInfoType = 2;
			short num2 = 0;
			ODBCDriver.SQLGetInfo(this.m_ConnectionHandle, fInfoType, stringBuilder, num, out num2);
			string text = stringBuilder.ToString();
			if (!string.IsNullOrEmpty(text))
			{
				this.m_ConnectionString = string.Format("DSN={0};", text);
			}
			string databaseName = this.GetDatabaseName();
			Log.WriteLine("DatabaseName: " + databaseName);
			string a;
			if ((a = databaseName) != null)
			{
				//if ((a != "Microsoft SQL Server"))
				//{
    //                // *** Coyove Patched ***
				//	// if ((a != "ACCESS"))
    //                if (a != "MariaDB")
				//	{
				//		if ((a != "EXCEL"))
				//		{
    //                        Log.WriteError("Return 1");
    //                        return false;
				//		}
				//		DatabaseManager.isExcelExport = true;
				//		this.m_InvalidCharacters = " #".ToCharArray();
				//		this.m_DataTypeMap.Add(DataType.DOUBLE, "double");
				//		this.m_DataTypeMap.Add(DataType.TEXT, "TEXT");
				//		this.m_DataTypeMap.Add(DataType.INTEGER, "int");
				//	}
				//	else
				//	{
						DatabaseManager.isExcelExport = false;
						this.m_InvalidCharacters = ".!`[]".ToCharArray();
						this.m_DataTypeMap.Add(DataType.DOUBLE, "DOUBLE PRECISION");
						this.m_DataTypeMap.Add(DataType.TEXT, "TEXT");
						this.m_DataTypeMap.Add(DataType.INTEGER, "int");
				//	}
				//}
				//else
				//{
				//	DatabaseManager.isExcelExport = false;
				//	this.m_InvalidCharacters = "-/ ".ToCharArray();
				//	this.m_KeywordsMap.Add("Columns", "Columns1");
				//	this.m_DataTypeMap.Add(DataType.DOUBLE, "float");
				//	this.m_DataTypeMap.Add(DataType.TEXT, "nvarchar(255)");
				//	this.m_DataTypeMap.Add(DataType.INTEGER, "int");
				//}
				return true;
			}
            // Log.WriteError("Return 2");
			return false;
		}

		private string GetDatabaseName()
		{
			short num = 1000;
			System.Text.StringBuilder stringBuilder = new System.Text.StringBuilder((int)num);
			short fInfoType = 17;
			short num2 = 0;
			ODBCDriver.SQLGetInfo(this.m_ConnectionHandle, fInfoType, stringBuilder, num, out num2);
			return stringBuilder.ToString();
		}

		public bool SetDatabase(string connectionstring)
		{
			if (!this.Initialize())
			{
				return false;
			}
			int value = this.Connect(connectionstring, 1, ref this.m_ConnectionString);
			return this.IsOK(value) && this.CheckConnectionString();
		}

		public bool SelectDatabase()
		{
            // TODO
            if (!this.Initialize())
			{
				return false;
			}

            int value = this.Connect("", 2, ref this.m_ConnectionString);

            bool flag1 = this.IsOK(value);
            bool flag2 = this.CheckConnectionString();

            Log.WriteLine(flag1.ToString() + flag2.ToString());
            return this.IsOK(value) && flag2;
		}

		private string GetTableCreationSqlStmt(TableInfo tableInfo)
		{
            // *** Coyove Patched ***

			System.Text.StringBuilder stringBuilder = new System.Text.StringBuilder(100);
            stringBuilder.AppendFormat("CREATE TABLE \"{0}\" (", tableInfo.Name);
            foreach (ColumnInfo current in tableInfo.Values)
			{
                stringBuilder.AppendFormat("\"{0}\" {1},", current.Name, this.m_DataTypeMap[current.DataType]);
            }
			stringBuilder.Remove(stringBuilder.Length - 1, 1);

            // MySQL does not support setting a text column as the primary key
            // Switched to Postgresql

            if (!DatabaseManager.isExcelExport && tableInfo.PrimaryKeys.Count > 0)
			{
				stringBuilder.AppendFormat(", CONSTRAINT \"PK_{0}\" PRIMARY KEY (", tableInfo.Name);
				for (int i = 0; i < tableInfo.PrimaryKeys.Count; i++)
				{
                    stringBuilder.AppendFormat("\"{0}\",", tableInfo[tableInfo.PrimaryKeys[i]].Name);
              }
				stringBuilder.Remove(stringBuilder.Length - 1, 1);
				stringBuilder.Append(")");
			}

			stringBuilder.Append(")");
			return stringBuilder.ToString();
		}

		private string[] GetTableForeignKeysSqlStmt(TableInfo tableInfo)
		{
            // *** Coyove Patched ***

			string[] array = new string[tableInfo.ForeignKeys.Count];
			if (!DatabaseManager.isExcelExport)
			{
				for (int i = 0; i < tableInfo.ForeignKeys.Count; i++)
				{
					ForeignKey foreignKey = tableInfo.ForeignKeys[i];
                    array[i] = string.Format("ALTER TABLE \"{0}\" ADD CONSTRAINT \"FK_{0}_{1}\" FOREIGN KEY(\"{1}\") REFERENCES \"{2}\"(\"{3}\")", new object[]
                    {
						tableInfo.Name,
						foreignKey.ColumnName,
						foreignKey.RefTableName,
						foreignKey.RefColumnName
					});
				}
			}
			return array;
		}

		private string GetAddColumnSqlStmt(string tableName, System.Collections.Generic.List<ColumnInfo> columns)
		{
            // *** Coyove Patched ***

			System.Text.StringBuilder stringBuilder = new System.Text.StringBuilder(100);
            // stringBuilder.AppendFormat("ALTER TABLE [{0}] ADD ", tableName);
            stringBuilder.AppendFormat("ALTER TABLE \"{0}\" ADD ", tableName);
            for (int i = 0; i < columns.Count; i++)
			{
                // stringBuilder.AppendFormat("[{0}] {1},", columns[i].Name, this.m_DataTypeMap[columns[i].DataType]);
                stringBuilder.AppendFormat("\"{0}\" {1},", columns[i].Name, this.m_DataTypeMap[columns[i].DataType]);
            }
			stringBuilder.Remove(stringBuilder.Length - 1, 1);
			return stringBuilder.ToString();
		}

		public bool CreateDatabase(TableInfoSet tableinfoset, out string errorMessage)
		{
			errorMessage = string.Empty;
			Log.WriteLine("Create tables ...");
			Log.Indent();
			foreach (TableInfo current in tableinfoset.Values)
			{
				if (current.PrimaryKeys.Count > 1)
				{
					Log.WriteLine("{0},{1}", new object[]
					{
						current.TableId,
						current.Name
					});
					Log.Indent();
					foreach (string current2 in current.PrimaryKeys)
					{
						Log.WriteLine("primaryKey:" + current2);
					}
					Log.Unindent();
				}
				if (this.TableExist(current.Name))
				{
					if (DatabaseManager.isExcelExport)
					{
						MessageBox.Show(RDBResource.GetString("MessageBox_CanNotUpdateExcelTable"), RDBResource.GetString("MessageBox_CanNotUpdateExcelTable_Title"), MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
						bool result = false;
						return result;
					}
					System.Collections.Generic.List<ColumnInfo> list = new System.Collections.Generic.List<ColumnInfo>();
					foreach (ColumnInfo current3 in current.Values)
					{
						if (!this.ColumnExist(current.Name, current3.Name))
						{
							list.Add(current3);
						}
					}
					if (list.Count > 0)
					{
						string addColumnSqlStmt = this.GetAddColumnSqlStmt(current.Name, list);
						bool flag = this.ExecuteSQL(addColumnSqlStmt);
						if (Command.ConfigFile.DebugSQL && !flag)
						{
							Log.WriteWarning("Update table '" + current.Name + "' failed, statement: " + addColumnSqlStmt);
						}
					}
				}
				else
				{
					string tableCreationSqlStmt = this.GetTableCreationSqlStmt(current);

                    if (Command.ConfigFile.DebugSQL)
					{
						Log.WriteLine("Create table: (" + current.Name + ") " + tableCreationSqlStmt);
					}
					if (!this.ExecuteSQL(tableCreationSqlStmt))
					{
						Log.WriteError("Failed: " + tableCreationSqlStmt);
						errorMessage = tableCreationSqlStmt;
						bool result = false;
						return result;
					}
				}
				this.ProgressStep();
			}
			Log.Unindent();
			Log.WriteLine("Create tables Finished");
			Log.WriteLine("Add foreign keys ...");
			Log.Indent();
			if (!DatabaseManager.isExcelExport)
			{
				foreach (TableInfo current4 in tableinfoset.Values)
				{
					string[] tableForeignKeysSqlStmt = this.GetTableForeignKeysSqlStmt(current4);
					string[] array = tableForeignKeysSqlStmt;
					for (int i = 0; i < array.Length; i++)
					{
						string sqlStmt = array[i];
						this.ExecuteSQL(sqlStmt);
					}
					this.ProgressStep();
				}
			}
			Log.Unindent();
			Log.WriteLine("Add foreign keys Finished");
			return true;
		}

		public void ProgressStep()
		{
			if (this.PerformProgressStep != null)
			{
				this.PerformProgressStep();
			}
		}

		public bool IsDatabaseValid(TableInfoSet tableinfoset)
		{
            // *** Coyove Patched ***
            int num = 0;
			foreach (TableInfo current in tableinfoset.Values)
			{
				string name = current.Name;
				if (this.TableExist(name))
				{
					num++;
					bool flag = current is InstanceTableInfo || current is SymbolTableInfo;
					foreach (ColumnInfo current2 in current.Values)
					{
						string name2 = current2.Name;
						if (!this.ColumnExist(name, name2))
						{
							if (!flag || !(current2 is CustomColumnInfo))
							{
								bool result = false;
								return result;
							}
                            // string text = string.Format("ALTER TABLE [{0}] ADD [{1}] {2}", name, current2.Name, this.m_DataTypeMap[current2.DataType]);
                            string text = string.Format("ALTER TABLE \"{0}\" ADD \"{1}\" {2}", name, current2.Name, this.m_DataTypeMap[current2.DataType]);
                            if (!this.ExecuteSQL(text))
							{
								Log.WriteWarning("SQL Statement '{0}' executed failed", new object[]
								{
									text
								});
								bool result = false;
								return result;
							}
						}
					}
				}
			}
			return num > 0;
		}

		public string GetDatabase()
		{
			return this.m_ConnectionString;
		}

		public bool Open()
		{
			this.Terminate();
			this.m_connection = new OdbcConnection(this.m_ConnectionString);
			Log.WriteLine("ODBCDriver Opened");
			Log.WriteLine("Conn String = " + this.m_ConnectionString);
			try
			{
				this.m_connection.Open();
			}
			catch (System.Exception ex)
			{
				TaskDialog.Show(RDBResource.GetString("Connect_Database_Failed"), ex.ToString());
				return false;
			}
			return true;
		}

		public DataTable GetSchema(string collectionname, string[] restrictionvalues)
		{
			return this.m_connection.GetSchema(collectionname, restrictionvalues);
		}

		public DbCommand CreateCommand()
		{
			return this.m_connection.CreateCommand();
		}

		public DbDataAdapter CreateAdapter(DbCommand cmd)
		{
			return new OdbcDataAdapter((OdbcCommand)cmd);
		}

		public DbCommandBuilder CreateCommandBuilder(DbDataAdapter adapter)
		{
			return new OdbcCommandBuilder((OdbcDataAdapter)adapter);
		}

		public bool CompareToExpected(string connectionString)
		{
			return true;
		}
	}
}
