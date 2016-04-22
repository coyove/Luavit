using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;

namespace Revit.Addon.RevitDBLink.CS
{
	public class AddonAvailability : IExternalCommandAvailability
	{
		public bool IsCommandAvailable(UIApplication applicationData, CategorySet selectedCategories)
		{
			return applicationData != null && applicationData.ActiveUIDocument != null && applicationData.ActiveUIDocument.Document != null;
		}
	}
}
