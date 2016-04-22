using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;

namespace Revit.Addon.RevitDBLink.CS
{
	public class TableInfo : System.Collections.Generic.Dictionary<string, ColumnInfo>
	{
		public delegate void workerFunction(TableInfo tableinfo);

		public delegate void workerFunction1Param(TableInfo tableinfo, string par1);

		public delegate void workerFunction2Param(TableInfo tableinfo, string par1, string par2);

		private System.Collections.Generic.List<ForeignKey> m_foreignKeys = new System.Collections.Generic.List<ForeignKey>();

		private PrimaryKeys m_primaryKey = new PrimaryKeys();

		private APIObjectList m_objectList;

		private string m_tableId;

		private string m_name;

		private string m_displayColumnId;

		private System.Collections.Generic.Dictionary<string, ColumnInfo> m_map = new System.Collections.Generic.Dictionary<string, ColumnInfo>();

		public APIObjectList ObjectList
		{
			get
			{
				return this.m_objectList;
			}
		}

		public string TableId
		{
			get
			{
				return this.m_tableId;
			}
		}

		public string Name
		{
			get
			{
				return this.m_name;
			}
		}

		public string DisplayColumnId
		{
			get
			{
				return this.m_displayColumnId;
			}
			set
			{
				this.m_displayColumnId = value;
			}
		}

		public string DisplayColumnName
		{
			get
			{
				return this.DisplayColumn.Name;
			}
		}

		public ColumnInfo DisplayColumn
		{
			get;
			set;
		}

		public PrimaryKeys PrimaryKeys
		{
			get
			{
				return this.m_primaryKey;
			}
		}

		public System.Collections.Generic.List<ForeignKey> ForeignKeys
		{
			get
			{
				return this.m_foreignKeys;
			}
			set
			{
				this.m_foreignKeys = value;
			}
		}

		public int RecordCount
		{
			get
			{
				return this.ObjectList.Count;
			}
		}

		public bool Readonly
		{
			get
			{
				return this.m_objectList.Readonly;
			}
		}

		public TableInfo(string tableId)
		{
			this.m_tableId = tableId;
			if (Command.ConfigFile.DebugSQL)
			{
				Log.WriteLine("TableInfo ctor: tableId [{0}]", new object[]
				{
					this.m_tableId
				});
			}
			this.m_name = RDBResource.GetTableName(this.m_tableId);
			if (Command.ConfigFile.DebugSQL)
			{
				Log.WriteLine("\ttableName [{0}]", new object[]
				{
					this.m_name
				});
			}
		}

		public virtual void Initialize()
		{
		}

		public TableInfo(string tableId, APIObjectList list) : this(tableId)
		{
			this.m_objectList = list;
			list.TableInfo = this;
		}

		public TableInfo(BuiltInCategory builtInCategory, ElementTypeEnum instanceOrSymbol, APIObjectList list) : this(string.Concat(new object[]
		{
			"TabN_",
			builtInCategory,
			"_",
			instanceOrSymbol
		}))
		{
			this.m_objectList = list;
			list.TableInfo = this;
		}

		public bool AddColumn(ColumnInfo columnInfo)
		{
			string lowerNameOfColumn = this.GetLowerNameOfColumn(columnInfo);
			if (this.m_map.ContainsKey(lowerNameOfColumn))
			{
				Log.WriteWarning("Parameter '{0}' conflicts with an existing column '{1}' in table '{2}', it will be ignored to export/import", new object[]
				{
					columnInfo.Name,
					this.m_map[lowerNameOfColumn].Name,
					this.Name
				});
				return false;
			}
			this.m_map.Add(lowerNameOfColumn, columnInfo);
			base.Add(columnInfo.ColumnId, columnInfo);
			return true;
		}

		private string GetLowerNameOfColumn(ColumnInfo columnInfo)
		{
			return columnInfo.Name.ToLower();
		}

		public TableInfo WithColumn(BuiltInParameter parameterId, DataType dataType)
		{
			this.AddColumn(new ColumnInfo(parameterId, dataType));
			return this;
		}

		public TableInfo WithColumn(string columnId, DataType dataType)
		{
			this.AddColumn(new ColumnInfo(columnId, dataType));
			return this;
		}

		public TableInfo WithPrimaryKey(BuiltInParameter parameterId)
		{
			return this.WithPrimaryKey(RDBResource.GetColumnId(parameterId));
		}

		public TableInfo WithPrimaryKey(string name)
		{
			this.PrimaryKeys.Add(name);
			if (!base.ContainsKey(name))
			{
				throw new System.ArgumentException("The column [" + name + "]couldn't be found");
			}
			base[name].IsPrimaryKey = true;
			return this;
		}

		public TableInfo WithForeignKey(BuiltInParameter columnId, string refTableId, BuiltInParameter refColumnId)
		{
			return this.WithForeignKey(RDBResource.GetColumnId(columnId), refTableId, RDBResource.GetColumnId(refColumnId));
		}

		public TableInfo WithForeignKey(BuiltInParameter columnId, string refTableId, string refColumnId)
		{
			return this.WithForeignKey(RDBResource.GetColumnId(columnId), refTableId, refColumnId);
		}

		public TableInfo WithForeignKey(string columnId, string refTableId, BuiltInParameter refColumnId)
		{
			return this.WithForeignKey(columnId, refTableId, RDBResource.GetColumnId(refColumnId));
		}

		public TableInfo WithForeignKey(string columnId, string refTableId, string refColumnId)
		{
			ForeignKey foreignKey = new ForeignKey(this, columnId, refTableId, refColumnId);
			if (Command.ConfigFile.DebugSQL && foreignKey.Table == null)
			{
				Log.WriteLine(string.Format("[ForeignKey1] {0}[{1}] - {2}[{3}]", new object[]
				{
					this.TableId,
					columnId,
					refTableId,
					refColumnId
				}));
				return this;
			}
			this.ForeignKeys.Add(foreignKey);
			return this;
		}

		public TableInfo WithDisplayColumn(BuiltInParameter parameterId, DataType dataType)
		{
			ColumnInfo columnInfo = new ColumnInfo(parameterId, dataType);
			this.AddColumn(columnInfo);
			this.DisplayColumn = columnInfo;
			return this;
		}

		public TableInfo WithDisplayColumn(string columnId, DataType dataType)
		{
			ColumnInfo columnInfo = new ColumnInfo(columnId, dataType);
			this.AddColumn(columnInfo);
			this.DisplayColumn = columnInfo;
			return this;
		}

		public TableInfo With(TableInfo.workerFunction wfunction)
		{
			wfunction(this);
			return this;
		}

		public TableInfo With(TableInfo.workerFunction1Param wfunction, string par1)
		{
			wfunction(this, par1);
			return this;
		}

		public TableInfo With(TableInfo.workerFunction2Param wfunction, string par1, string par2)
		{
			wfunction(this, par1, par2);
			return this;
		}

		public TableInfo With(Action<TableInfo, string, string, string, string> wfunction, string refTableId1, string refColumnId1, string refTableId2, string refColumnId2)
		{
			wfunction(this, refTableId1, refColumnId1, refTableId2, refColumnId2);
			return this;
		}

		public TableInfo With(Action<TableInfo, string, string, string, string> wfunction, string refTableId1, string refColumnId1, string refTableId2, BuiltInParameter refColumnId2)
		{
			wfunction(this, refTableId1, refColumnId1, refTableId2, RDBResource.GetColumnId(refColumnId2));
			return this;
		}

		public override string ToString()
		{
			return string.Concat(new string[]
			{
				"[",
				this.Name,
				"] (",
				this.TableId,
				")"
			});
		}
	}
}
