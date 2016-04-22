using Autodesk.Revit.DB;
using System;
using System.Data;
using System.Linq;

namespace Revit.Addon.RevitDBLink.CS
{
	public class ElementPhaseList : NonCreatableElementList
	{
		public override void InitializeList()
		{
			base.AddRange(from element in base.GetElements()
			where element.Id.IntegerValue != -1 && element.CreatedPhaseId != ElementId.InvalidElementId
			select element);
		}

		protected override void PopulateDbRow(Element element, DataRow row)
		{
			Utility.Assign(row, APIObjectList.ColumnRes("ColN_CST_ElementId"), APIObjectList.GetIdDbValue(element));
			Utility.Assign(row, APIObjectList.ColumnRes("ColN_CST_ElementName"), APIObjectList.GetNameDbValue(element));
			Utility.Assign(row, APIObjectList.ColumnRes("ColN_CST_PhaseCreatedId"), APIObjectList.GetIdDbValue(element.CreatedPhaseId));
			Utility.Assign(row, APIObjectList.ColumnRes("ColN_CST_PhaseName"), APIObjectList.GetNameDbValue(element.Document.GetElement(element.CreatedPhaseId)));
		}
	}
}
