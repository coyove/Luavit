using System;

namespace Revit.Addon.RevitDBLink.CS
{
	public class TableTag
	{
		private bool m_allowCreate;

		private bool m_customTable;

		private bool m_ROTable;

		private static TableTag s_allowCreate;

		private static TableTag s_notAllowCreate;

		private static TableTag s_customTable;

		private static TableTag s_ROTable;

		public bool IsAllowCreate
		{
			get
			{
				return this.m_allowCreate;
			}
			set
			{
				this.m_allowCreate = value;
			}
		}

		public bool IsCustomTable
		{
			get
			{
				return this.m_customTable;
			}
			set
			{
				this.m_customTable = value;
			}
		}

		public bool isReadOnly
		{
			get
			{
				return this.m_ROTable;
			}
			set
			{
				this.m_ROTable = value;
			}
		}

		public static TableTag AllowCreate
		{
			get
			{
				return TableTag.s_allowCreate;
			}
		}

		public static TableTag NotAllowCreate
		{
			get
			{
				return TableTag.s_notAllowCreate;
			}
		}

		public static TableTag CustomTable
		{
			get
			{
				return TableTag.s_customTable;
			}
		}

		public static TableTag ReadOnlyTable
		{
			get
			{
				return TableTag.s_ROTable;
			}
		}

		public TableTag(bool allowCreate, bool customTable, bool ReadOnly)
		{
			this.m_allowCreate = allowCreate;
			this.m_customTable = customTable;
			this.m_ROTable = ReadOnly;
		}

		static TableTag()
		{
			TableTag.s_allowCreate = new TableTag(true, false, false);
			TableTag.s_notAllowCreate = new TableTag(false, false, false);
			TableTag.s_customTable = new TableTag(false, true, false);
			TableTag.s_ROTable = new TableTag(false, false, true);
		}
	}
}
