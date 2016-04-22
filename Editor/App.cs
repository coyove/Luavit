using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using System;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Windows.Media.Imaging;

namespace Luavit
{
	public class App : IExternalApplication
	{
		public Result OnShutdown(UIControlledApplication application)
		{
			return 0;
		}

		public Result OnStartup(UIControlledApplication application)
		{
			string location = typeof(Command).Assembly.Location;
			try
			{
				System.Threading.Thread.CurrentThread.CurrentUICulture = System.Globalization.CultureInfo.CurrentCulture;

				RibbonPanel ribbonPanel = application.CreateRibbonPanel("Luavit");
				ribbonPanel.Enabled = (true);
				ribbonPanel.Visible = (true);
				PushButtonData pushButtonData = new PushButtonData("Luavit", "Luavit", location, typeof(Command).FullName);
				// pushButtonData.AvailabilityClassName = (typeof(AddonAvailability).FullName);
				PushButton pushButton = ribbonPanel.AddItem(pushButtonData) as PushButton;
				pushButton.Enabled = (true);
				pushButton.Visible = (true);
                string directoryName = System.IO.Path.GetDirectoryName(location);
				string text = System.IO.Path.Combine(directoryName, "RevitDBLink.ico");
				string text2 = System.IO.Path.Combine(directoryName, "RevitDBLink_32.ico");
				if (System.IO.File.Exists(text))
				{
					pushButton.Image = (new BitmapImage(new Uri(text)));
				}
				if (System.IO.File.Exists(text2))
				{
					pushButton.LargeImage = (new BitmapImage(new Uri(text2)));
				}
				//ContextualHelp contextualHelp = new ContextualHelp(ContextualHelpType.ContextId, RDBResource.GetString("Form_RevitDBLinkForm_ContextId_Overview"));
				//pushButton.SetContextualHelp(contextualHelp);
			}
			catch
			{
			//	TaskDialog.Show("IExternalApplication", RDBResource.GetString("RibbonButtonCreationProblems"));
				return Result.Failed;
			}
			return 0;
		}
	}
}
