using System;

namespace Revit.Addon.RevitDBLink.CS
{
	public class CellTag
	{
		private bool m_readOnly;

		private static CellTag s_readOnly;

		private static CellTag s_notReadOnly;

		public bool IsReadOnly
		{
			get
			{
				return this.m_readOnly;
			}
			set
			{
				this.m_readOnly = value;
			}
		}

		public static CellTag ReadOnly
		{
			get
			{
				return CellTag.s_readOnly;
			}
		}

		public static CellTag NotReadOnly
		{
			get
			{
				return CellTag.s_notReadOnly;
			}
		}

		public CellTag(bool readOnly)
		{
			this.m_readOnly = readOnly;
		}

		static CellTag()
		{
			CellTag.s_readOnly = new CellTag(true);
			CellTag.s_notReadOnly = new CellTag(false);
		}
	}
}
