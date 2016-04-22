using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.IO;

namespace Revit.Addon.RevitDBLink.CS
{
	public class ParameterCreation
	{
		private Application m_revitApp;

		private Document m_revitDoc;

		private Transaction m_updateUserDefinedParameterTransaction;

		public ParameterCreation(UIApplication uiApp)
		{
			this.m_revitApp = uiApp.Application;
			this.m_revitDoc = uiApp.ActiveUIDocument.Document;
			this.m_updateUserDefinedParameterTransaction = new Transaction(this.m_revitDoc, "ParameterCreation.CreateUserDefinedParameter");
		}

		public bool CreateUserDefinedParameter(ParameterInfo parameterInfo)
		{
			if (parameterInfo.ParameterIsProject)
			{
				return false;
			}
			this.StartTransaction();
			try
			{
				DefinitionGroup sharedGroup = this.GetSharedGroup();
				Definition definition = sharedGroup.Definitions.get_Item(parameterInfo.ParameterName);
				if (definition == null)
				{
					ExternalDefinitionCreationOptions externalDefinitionCreationOptions = new ExternalDefinitionCreationOptions(parameterInfo.ParameterName, parameterInfo.ParameterType);
					definition = sharedGroup.Definitions.Create(externalDefinitionCreationOptions);
					Log.WriteLine("Create shared parameter: " + parameterInfo.ParameterName);
				}
				ElementBinding elementBinding;
				if (parameterInfo.ParameterForType)
				{
					elementBinding = this.m_revitApp.Create.NewTypeBinding(parameterInfo.Categories);
					Log.WriteLine("Binding with types");
				}
				else
				{
					elementBinding = this.m_revitApp.Create.NewInstanceBinding(parameterInfo.Categories);
					Log.WriteLine("Binding with instances");
				}
				bool flag = this.m_revitDoc.ParameterBindings.Insert(definition, elementBinding, parameterInfo.ParameterGroup);
				Log.WriteLine("Submit binding");
				bool result;
				if (!flag)
				{
					this.RollbackTransaction();
					result = false;
					return result;
				}
				this.CommitTransaction();
				result = true;
				return result;
			}
			catch (System.Exception ex)
			{
				Log.WriteLine("Create shared parameter failed");
				Log.WriteLine(ex.ToString());
				this.RollbackTransaction();
			}
			return false;
		}

		private void RollbackTransaction()
		{
			this.m_updateUserDefinedParameterTransaction.RollBack();
		}

		private void StartTransaction()
		{
			this.m_updateUserDefinedParameterTransaction.Start();
		}

		private void CommitTransaction()
		{
			this.m_updateUserDefinedParameterTransaction.Commit();
		}

		private DefinitionGroup GetSharedGroup()
		{
			string sharedParametersFile = this.GetSharedParametersFile();
			this.m_revitApp.SharedParametersFilename = sharedParametersFile;
			DefinitionFile definitionFile = this.m_revitApp.OpenSharedParameterFile();
			if (definitionFile == null)
			{
				return null;
			}
			DefinitionGroups groups = definitionFile.Groups;
			DefinitionGroup definitionGroup = groups.get_Item("RDBParameters");
			if (definitionGroup == null)
			{
				groups.Create("RDBParameters");
				definitionGroup = groups.get_Item("RDBParameters");
			}
			return definitionGroup;
		}

		private string GetSharedParametersFile()
		{
			string localRevitDBLinkApplicationDataFolder = Command.GetLocalRevitDBLinkApplicationDataFolder();
			string text = System.IO.Path.Combine(localRevitDBLinkApplicationDataFolder, "Revit DB Link_SharedParameters.txt");
			if (!System.IO.File.Exists(text))
			{
				System.IO.FileStream fileStream = System.IO.File.Create(text);
				fileStream.Close();
			}
			return text;
		}
	}
}
