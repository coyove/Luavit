using Autodesk.Revit.DB;
using System;
using System.Data;

namespace Revit.Addon.RevitDBLink.CS
{
	public class InstanceList : SymbolList
	{
		public override bool SupportCreate
		{
			get
			{
				return false;
			}
		}

		public InstanceList(BuiltInCategory builtInCategory) : base(builtInCategory)
		{
			this.m_instanceOrSymbol = ElementTypeEnum.INSTANCE;
		}

		public override void InitializeList()
		{
			base.AddRange(APIObjectList.GetElements(false, new BuiltInCategory[]
			{
				this.m_category
			}));
		}

		protected override Element CreateNewElement(DataRow row)
		{
			return null;
		}
	}
}
