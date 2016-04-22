using Autodesk.Revit.DB;
using System;

namespace Revit.Addon.RevitDBLink.CS
{
	public class ColumnInfo
	{
		public DataType DataType
		{
			get;
			set;
		}

		public string ColumnId
		{
			get;
			set;
		}

		public string Name
		{
			get;
			set;
		}

		public bool IsPrimaryKey
		{
			get;
			set;
		}

		public BuiltInParameter BuiltInParameter
		{
			get;
			set;
		}

		public ColumnInfo()
		{
		}

		public ColumnInfo(BuiltInParameter parameterId, DataType dataType) : this()
		{
			this.BuiltInParameter = parameterId;
			this.DataType = dataType;
			this.ColumnId = RDBResource.GetColumnId(parameterId);
			this.Name = RDBResource.GetColumnName(parameterId);
			if (Command.ConfigFile.DebugSQL)
			{
				Log.WriteLine("\t\tName [{0}]", new object[]
				{
					this.Name
				});
			}
		}

		public ColumnInfo(string columnId, DataType dataType) : this()
		{
			this.ColumnId = columnId;
			this.DataType = dataType;
			this.BuiltInParameter = (BuiltInParameter)(-1);
			if (Command.ConfigFile.DebugSQL)
			{
				Log.WriteLine("\tColumnInfo ctor [{0}] {1}", new object[]
				{
					this.ColumnId,
					dataType
				});
			}
			this.Name = RDBResource.GetColumnName(this.ColumnId);
			if (Command.ConfigFile.DebugSQL)
			{
				Log.WriteLine("\t\tName [{0}]", new object[]
				{
					this.Name
				});
			}
		}
	}
}
