using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.ComponentModel;
using System.Data;
using System.Windows.Forms;

namespace Revit.Addon.RevitDBLink.CS
{
	public class ParameterListForm : System.Windows.Forms.Form
    {
		private Autodesk.Revit.ApplicationServices.Application m_revitApp;

		private UIApplication m_revitUiApp;

		private ParameterCreation m_parameterCreation;

		private TableInfoSet m_tableInfoSet;

		private DataSet m_dataSet;

		private IContainer components;

		private Button okButton;

		private Button cancelButton;

		private ListBox parameterListBox;

		private Label label1;

		private Button addButton;

		private ParameterListForm()
		{
			this.InitializeComponent();
		}

		public ParameterListForm(UIApplication revitUiApp, DataSet dataSet) : this()
		{
			this.m_revitUiApp = revitUiApp;
			this.m_revitApp = revitUiApp.Application;
			this.m_tableInfoSet = Command.TableInfoSet;
			this.m_parameterCreation = new ParameterCreation(revitUiApp);
			this.m_dataSet = dataSet;
			this.parameterListBox.DisplayMember = "ParameterName";
			this.parameterListBox.Sorted = true;
			DefinitionBindingMapIterator definitionBindingMapIterator = this.m_revitUiApp.ActiveUIDocument.Document.ParameterBindings.ForwardIterator();
			definitionBindingMapIterator.Reset();
			while (definitionBindingMapIterator.MoveNext())
			{
				Definition key = definitionBindingMapIterator.Key;
				string name = key.Name;
				if (!this.ParameterExists(name))
				{
					this.parameterListBox.Items.Add(name);
				}
			}
		}

		private void addButton_Click(object sender, System.EventArgs e)
		{
			using (ParameterPropertyForm parameterPropertyForm = new ParameterPropertyForm(this.m_revitUiApp))
			{
				if (parameterPropertyForm.ShowDialog() == DialogResult.OK)
				{
					try
					{
						if (!this.m_parameterCreation.CreateUserDefinedParameter(parameterPropertyForm.ParameterInfo))
						{
							MessageBox.Show(RDBResource.GetString("MessageBox_Import_CreateParameterFailed"));
						}
						else
						{
							ParameterInfo parameterInfo = parameterPropertyForm.ParameterInfo;
							this.parameterListBox.Items.Add(parameterInfo.ParameterName);
							this.parameterListBox.SelectedItem = parameterInfo.ParameterName;
							foreach (Category category in parameterInfo.Categories)
							{
								if (category != null)
								{
									string tableKey = RDBResource.GetTableKey(category.Id.IntegerValue, parameterInfo.ParameterForType);
									string tableName = RDBResource.GetTableName(tableKey);
									if (tableName != null && !Command.ColumnExists(tableName, parameterInfo.ParameterName))
									{
										bool flag = Command.AddColumn(parameterInfo.ParameterName, DatabaseManager.GetDataType(parameterInfo.ParameterType), tableName);
										if (flag)
										{
											bool flag2 = true;
											TableInfo tableInfo = this.m_tableInfoSet[tableKey];
											foreach (ColumnInfo current in tableInfo.Values)
											{
												if (current.Name == parameterInfo.ParameterName)
												{
													flag2 = false;
													break;
												}
											}
											if (flag2)
											{
												tableInfo.AddColumn(new CustomColumnInfo(parameterInfo.ParameterName, DatabaseManager.GetDataType(parameterInfo.ParameterType)));
											}
											if (this.m_dataSet.Tables.Contains(tableName))
											{
												Log.WriteLine("Refresh data table " + tableName + " after addding column");
												Command.RefreshDataTable(this.m_dataSet.Tables[tableName]);
											}
										}
									}
								}
							}
						}
					}
					catch (System.Exception ex)
					{
						MessageBox.Show(ex.Message);
						Log.WriteLine(ex.ToString());
					}
				}
			}
		}

		private void cancelButton_Click(object sender, System.EventArgs e)
		{
			base.Close();
		}

		private void okButton_Click(object sender, System.EventArgs e)
		{
		}

		private bool ParameterExists(string definitionName)
		{
			foreach (string a in this.parameterListBox.Items)
			{
				if (a == definitionName)
				{
					return true;
				}
			}
			return false;
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && this.components != null)
			{
				this.components.Dispose();
			}
			base.Dispose(disposing);
		}

		private void InitializeComponent()
		{
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(ParameterListForm));
			this.okButton = new Button();
			this.cancelButton = new Button();
			this.parameterListBox = new ListBox();
			this.label1 = new Label();
			this.addButton = new Button();
			base.SuspendLayout();
			componentResourceManager.ApplyResources(this.okButton, "okButton");
			this.okButton.DialogResult = DialogResult.OK;
			this.okButton.Name = "okButton";
			this.okButton.UseVisualStyleBackColor = true;
			this.okButton.Click += new System.EventHandler(this.okButton_Click);
			componentResourceManager.ApplyResources(this.cancelButton, "cancelButton");
			this.cancelButton.DialogResult = DialogResult.Cancel;
			this.cancelButton.Name = "cancelButton";
			this.cancelButton.UseVisualStyleBackColor = true;
			this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
			componentResourceManager.ApplyResources(this.parameterListBox, "parameterListBox");
			this.parameterListBox.FormattingEnabled = true;
			this.parameterListBox.Name = "parameterListBox";
			componentResourceManager.ApplyResources(this.label1, "label1");
			this.label1.Name = "label1";
			componentResourceManager.ApplyResources(this.addButton, "addButton");
			this.addButton.Name = "addButton";
			this.addButton.UseVisualStyleBackColor = true;
			this.addButton.Click += new System.EventHandler(this.addButton_Click);
			base.AcceptButton = this.okButton;
			componentResourceManager.ApplyResources(this, "$this");
			base.AutoScaleMode = AutoScaleMode.Dpi;
			base.CancelButton = this.cancelButton;
			base.Controls.Add(this.addButton);
			base.Controls.Add(this.label1);
			base.Controls.Add(this.parameterListBox);
			base.Controls.Add(this.cancelButton);
			base.Controls.Add(this.okButton);
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "ParameterListForm";
			base.ShowIcon = false;
			base.ShowInTaskbar = false;
			base.ResumeLayout(false);
			base.PerformLayout();
		}
	}
}
