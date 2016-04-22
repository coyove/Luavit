using System;

namespace Revit.Addon.RevitDBLink.CS
{
	public class RowTag
	{
		private bool m_notExist;

		private bool m_pastable;

		private bool m_deletable;

		private static RowTag s_aNewRow;

		private static RowTag s_anOldRowExist;

		public bool NotExist
		{
			get
			{
				return this.m_notExist;
			}
			set
			{
				this.m_notExist = value;
			}
		}

		public bool Pastable
		{
			get
			{
				return this.m_pastable;
			}
			set
			{
				this.m_pastable = value;
			}
		}

		public bool Deletable
		{
			get
			{
				return this.m_deletable;
			}
			set
			{
				this.m_deletable = value;
			}
		}

		public static RowTag ANewRow
		{
			get
			{
				return RowTag.s_aNewRow;
			}
		}

		public static RowTag AnOldRowExist
		{
			get
			{
				return RowTag.s_anOldRowExist;
			}
		}

		static RowTag()
		{
			RowTag.s_aNewRow = new RowTag(true, true, true);
			RowTag.s_anOldRowExist = new RowTag(false, false, false);
		}

		public RowTag(bool notExist, bool pastable, bool deletable)
		{
			this.m_notExist = notExist;
			this.m_pastable = pastable;
			this.m_deletable = deletable;
		}
	}
}
