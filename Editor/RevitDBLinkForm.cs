using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;

namespace Revit.Addon.RevitDBLink.CS
{
	public class RevitDBLinkForm : Form
	{
		private bool m_isExport;

		private bool m_UsePreviousConnection;

		private string m_PreviousConnection;

		private ContextualHelp m_contextualHelp;

		private System.Collections.Generic.List<string> m_previousHistory;

		private IContainer components;

		private Button exportOdbcButton;

		private Button importOdbcButton;

		private Button importAccess2003Button;

		private Button exportAccess2003Button;

		private TabControl tabControl;

		private TabPage msAccess2003Tabpage;

		private TabPage odbcTabpage;

		private ListBox msAccess2003PreviousListBox;

		private Label odbcLabel;

		private ListBox odbcPreviousListBox;

		private ContextMenuStrip contextMenuStripHisotry;

		private ToolStripMenuItem removeToolStripMenuItem;

		private TabPage msAccess2007Tabpage;

		private Label access2007Label;

		private ListBox msAccess2007PreviousListBox;

		private Button exportAccess2007Button;

		private Button importAccess2007Button;

		private Label msAccess2003Label;

		public bool IsExport
		{
			get
			{
				return this.m_isExport;
			}
		}

		public bool UsePreviousConnection
		{
			get
			{
				return this.m_UsePreviousConnection;
			}
		}

		public string PreviousConnection
		{
			get
			{
				return this.m_PreviousConnection;
			}
		}

		public RevitDBLinkForm()
		{
			this.InitializeComponent();
			this.m_contextualHelp = new ContextualHelp((ContextualHelpType)1, RDBResource.GetString("Form_RevitDBLinkForm_ContextId_Overview"));
			base.HelpButton = true;
		}

		private void RevitDBLinkForm_Load(object sender, System.EventArgs e)
		{
			if (DatabaseManager.CanExport(DatabaseManager.DatabaseType.MSAccess))
			{
				Log.WriteLine("Load MSAccess2003 connection history to UI");
				this.m_previousHistory = DatabaseManager.GetPreviousConnectionsFor(DatabaseManager.DatabaseType.MSAccess);
				this.LogHistory();
				this.msAccess2003PreviousListBox.Items.Add(RDBResource.GetString("IDS_SELECT_NEW_CONNECTION"));
				foreach (string current in this.m_previousHistory)
				{
					this.msAccess2003PreviousListBox.Items.Add(current);
				}
				this.msAccess2003PreviousListBox.Tag = this.m_previousHistory;
			}
			else
			{
				this.exportAccess2003Button.Enabled = false;
				this.importAccess2003Button.Enabled = false;
			}
			if (DatabaseManager.CanExport(DatabaseManager.DatabaseType.MSAccess2007))
			{
				Log.WriteLine("Load MSAccess2007 connection history to UI");
				this.m_previousHistory = DatabaseManager.GetPreviousConnectionsFor(DatabaseManager.DatabaseType.MSAccess2007);
				this.LogHistory();
				this.msAccess2007PreviousListBox.Items.Add(RDBResource.GetString("IDS_SELECT_NEW_CONNECTION"));
				foreach (string current2 in this.m_previousHistory)
				{
					this.msAccess2007PreviousListBox.Items.Add(current2);
				}
				this.msAccess2007PreviousListBox.Tag = this.m_previousHistory;
			}
			else
			{
				this.exportAccess2007Button.Enabled = false;
				this.importAccess2007Button.Enabled = false;
			}
			if (DatabaseManager.CanExport(DatabaseManager.DatabaseType.ODBC))
			{
				Log.WriteLine("Load ODBC connection history to UI");
				this.m_previousHistory = DatabaseManager.GetPreviousConnectionsFor(DatabaseManager.DatabaseType.ODBC);
				this.LogHistory();
				this.odbcPreviousListBox.Items.Add(RDBResource.GetString("IDS_SELECT_NEW_CONNECTION"));
				foreach (string current3 in this.m_previousHistory)
				{
					this.odbcPreviousListBox.Items.Add(current3);
				}
				this.odbcPreviousListBox.Tag = this.m_previousHistory;
			}
			else
			{
				this.exportOdbcButton.Enabled = false;
				this.importOdbcButton.Enabled = false;
			}
			Log.WriteLine("DatabaseManager.LastDatabaseType = " + DatabaseManager.LastDatabaseType);
			if (DatabaseManager.LastDatabaseType == DatabaseManager.DatabaseType.Invalid)
			{
				if (this.exportAccess2003Button.Enabled)
				{
					this.tabControl.SelectedTab = this.msAccess2003Tabpage;
					Log.WriteLine("Access2003 tab is selected");
					return;
				}
				if (this.exportAccess2007Button.Enabled)
				{
					this.tabControl.SelectedTab = this.msAccess2007Tabpage;
					Log.WriteLine("Access2007 tab is selected");
					return;
				}
				if (this.exportOdbcButton.Enabled)
				{
					this.tabControl.SelectedTab = this.odbcTabpage;
					Log.WriteLine("odbc tab set is selected");
					return;
				}
			}
			else
			{
				switch (DatabaseManager.LastDatabaseType)
				{
				case DatabaseManager.DatabaseType.ODBC:
					this.tabControl.SelectedTab = this.odbcTabpage;
					Log.WriteLine("odbc tab set is selected");
					break;
				case DatabaseManager.DatabaseType.MSAccess:
					this.tabControl.SelectedTab = this.msAccess2003Tabpage;
					Log.WriteLine("Access2003 tab is selected");
					return;
				case DatabaseManager.DatabaseType.MSAccess2007:
					this.tabControl.SelectedTab = this.msAccess2007Tabpage;
					Log.WriteLine("Access2007 tab is selected");
					return;
				default:
					return;
				}
			}
		}

		private void exportOdbc_Click(object sender, System.EventArgs e)
		{
			if (this.odbcPreviousListBox.SelectedIndex > 0)
			{
				this.m_UsePreviousConnection = true;
				this.m_PreviousConnection = (this.odbcPreviousListBox.SelectedItem as string);
			}
			DatabaseManager.CurrentDatabaseType = DatabaseManager.DatabaseType.ODBC;
			this.m_isExport = true;
			base.Close();
		}

		private void importOdbc_Click(object sender, System.EventArgs e)
		{
			if (this.odbcPreviousListBox.SelectedIndex > 0)
			{
				this.m_UsePreviousConnection = true;
				this.m_PreviousConnection = (this.odbcPreviousListBox.SelectedItem as string);
			}
			DatabaseManager.CurrentDatabaseType = DatabaseManager.DatabaseType.ODBC;
			this.m_isExport = false;
			base.Close();
		}

		private void exportAccess2003_Click(object sender, System.EventArgs e)
		{
			if (this.msAccess2003PreviousListBox.SelectedIndex > 0)
			{
				this.m_UsePreviousConnection = true;
				this.m_PreviousConnection = (this.msAccess2003PreviousListBox.SelectedItem as string);
			}
			DatabaseManager.CurrentDatabaseType = DatabaseManager.DatabaseType.MSAccess;
			this.m_isExport = true;
			base.Close();
		}

		private void importAccess2003_Click(object sender, System.EventArgs e)
		{
			if (this.msAccess2003PreviousListBox.SelectedIndex > 0)
			{
				this.m_UsePreviousConnection = true;
				this.m_PreviousConnection = (this.msAccess2003PreviousListBox.SelectedItem as string);
			}
			DatabaseManager.CurrentDatabaseType = DatabaseManager.DatabaseType.MSAccess;
			this.m_isExport = false;
			base.Close();
		}

		private void RevitDBLinkForm_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Escape)
			{
				base.DialogResult = DialogResult.Cancel;
				base.Close();
			}
		}

		private void removeToolStripMenuItem_Click(object sender, System.EventArgs e)
		{
			ListBox visibleListBox = this.GetVisibleListBox();
			if (visibleListBox != null && visibleListBox.SelectedIndex > 0)
			{
				this.m_previousHistory = (visibleListBox.Tag as System.Collections.Generic.List<string>);
				if (this.m_previousHistory == null)
				{
					Log.WriteNotice("m_previousHistory == null");
				}
				else if (!this.m_previousHistory.Remove(visibleListBox.SelectedItem.ToString()))
				{
					Log.WriteNotice("Remove history failed: " + visibleListBox.SelectedItem);
					this.LogHistory();
					Log.Unindent();
				}
				else
				{
					Log.WriteLine("Removed history: " + visibleListBox.SelectedItem);
				}
				visibleListBox.Items.RemoveAt(visibleListBox.SelectedIndex);
			}
		}

		private void LogHistory()
		{
			Log.WriteLine("m_previousHistory Count: " + this.m_previousHistory.Count);
			Log.Indent();
			foreach (string current in this.m_previousHistory)
			{
				Log.WriteLine(current);
			}
			Log.Unindent();
		}

		private ListBox GetVisibleListBox()
		{
			if (this.msAccess2003PreviousListBox.Visible)
			{
				return this.msAccess2003PreviousListBox;
			}
			if (this.msAccess2007PreviousListBox.Visible)
			{
				return this.msAccess2007PreviousListBox;
			}
			if (this.odbcPreviousListBox.Visible)
			{
				return this.odbcPreviousListBox;
			}
			return null;
		}

		private void exportAccess2007_Click(object sender, System.EventArgs e)
		{
			if (this.msAccess2007PreviousListBox.SelectedIndex > 0)
			{
				this.m_UsePreviousConnection = true;
				this.m_PreviousConnection = (this.msAccess2007PreviousListBox.SelectedItem as string);
			}
			DatabaseManager.CurrentDatabaseType = DatabaseManager.DatabaseType.MSAccess2007;
			this.m_isExport = true;
			base.Close();
		}

		private void importAccess2007_Click(object sender, System.EventArgs e)
		{
			if (this.msAccess2007PreviousListBox.SelectedIndex > 0)
			{
				this.m_UsePreviousConnection = true;
				this.m_PreviousConnection = (this.msAccess2007PreviousListBox.SelectedItem as string);
			}
			DatabaseManager.CurrentDatabaseType = DatabaseManager.DatabaseType.MSAccess2007;
			this.m_isExport = false;
			base.Close();
		}

		private void RevitDBLinkForm_HelpRequested(object sender, HelpEventArgs hlpevent)
		{
			this.m_contextualHelp.Launch();
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
			this.components = new Container();
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(RevitDBLinkForm));
			this.exportOdbcButton = new Button();
			this.importOdbcButton = new Button();
			this.importAccess2003Button = new Button();
			this.exportAccess2003Button = new Button();
			this.tabControl = new TabControl();
			this.msAccess2003Tabpage = new TabPage();
			this.msAccess2003Label = new Label();
			this.msAccess2003PreviousListBox = new ListBox();
			this.contextMenuStripHisotry = new ContextMenuStrip(this.components);
			this.removeToolStripMenuItem = new ToolStripMenuItem();
			this.msAccess2007Tabpage = new TabPage();
			this.access2007Label = new Label();
			this.msAccess2007PreviousListBox = new ListBox();
			this.exportAccess2007Button = new Button();
			this.importAccess2007Button = new Button();
			this.odbcTabpage = new TabPage();
			this.odbcLabel = new Label();
			this.odbcPreviousListBox = new ListBox();
			this.tabControl.SuspendLayout();
			this.msAccess2003Tabpage.SuspendLayout();
			this.contextMenuStripHisotry.SuspendLayout();
			this.msAccess2007Tabpage.SuspendLayout();
			this.odbcTabpage.SuspendLayout();
			base.SuspendLayout();
			componentResourceManager.ApplyResources(this.exportOdbcButton, "exportOdbcButton");
			this.exportOdbcButton.DialogResult = DialogResult.OK;
			this.exportOdbcButton.Name = "exportOdbcButton";
			this.exportOdbcButton.UseVisualStyleBackColor = true;
			this.exportOdbcButton.Click += new System.EventHandler(this.exportOdbc_Click);
			componentResourceManager.ApplyResources(this.importOdbcButton, "importOdbcButton");
			this.importOdbcButton.DialogResult = DialogResult.OK;
			this.importOdbcButton.Name = "importOdbcButton";
			this.importOdbcButton.UseVisualStyleBackColor = true;
			this.importOdbcButton.Click += new System.EventHandler(this.importOdbc_Click);
			componentResourceManager.ApplyResources(this.importAccess2003Button, "importAccess2003Button");
			this.importAccess2003Button.DialogResult = DialogResult.OK;
			this.importAccess2003Button.Name = "importAccess2003Button";
			this.importAccess2003Button.UseVisualStyleBackColor = true;
			this.importAccess2003Button.Click += new System.EventHandler(this.importAccess2003_Click);
			componentResourceManager.ApplyResources(this.exportAccess2003Button, "exportAccess2003Button");
			this.exportAccess2003Button.DialogResult = DialogResult.OK;
			this.exportAccess2003Button.Name = "exportAccess2003Button";
			this.exportAccess2003Button.UseVisualStyleBackColor = true;
			this.exportAccess2003Button.Click += new System.EventHandler(this.exportAccess2003_Click);
			this.tabControl.Controls.Add(this.msAccess2003Tabpage);
			this.tabControl.Controls.Add(this.msAccess2007Tabpage);
			this.tabControl.Controls.Add(this.odbcTabpage);
			componentResourceManager.ApplyResources(this.tabControl, "tabControl");
			this.tabControl.Name = "tabControl";
			this.tabControl.SelectedIndex = 0;
			this.msAccess2003Tabpage.Controls.Add(this.msAccess2003Label);
			this.msAccess2003Tabpage.Controls.Add(this.msAccess2003PreviousListBox);
			this.msAccess2003Tabpage.Controls.Add(this.exportAccess2003Button);
			this.msAccess2003Tabpage.Controls.Add(this.importAccess2003Button);
			componentResourceManager.ApplyResources(this.msAccess2003Tabpage, "msAccess2003Tabpage");
			this.msAccess2003Tabpage.Name = "msAccess2003Tabpage";
			this.msAccess2003Tabpage.UseVisualStyleBackColor = true;
			componentResourceManager.ApplyResources(this.msAccess2003Label, "msAccess2003Label");
			this.msAccess2003Label.Name = "msAccess2003Label";
			this.msAccess2003PreviousListBox.ContextMenuStrip = this.contextMenuStripHisotry;
			this.msAccess2003PreviousListBox.FormattingEnabled = true;
			componentResourceManager.ApplyResources(this.msAccess2003PreviousListBox, "msAccess2003PreviousListBox");
			this.msAccess2003PreviousListBox.Name = "msAccess2003PreviousListBox";
			this.contextMenuStripHisotry.Items.AddRange(new ToolStripItem[]
			{
				this.removeToolStripMenuItem
			});
			this.contextMenuStripHisotry.Name = "contextMenuStripHisotry";
			componentResourceManager.ApplyResources(this.contextMenuStripHisotry, "contextMenuStripHisotry");
			this.removeToolStripMenuItem.Name = "removeToolStripMenuItem";
			componentResourceManager.ApplyResources(this.removeToolStripMenuItem, "removeToolStripMenuItem");
			this.removeToolStripMenuItem.Click += new System.EventHandler(this.removeToolStripMenuItem_Click);
			this.msAccess2007Tabpage.Controls.Add(this.access2007Label);
			this.msAccess2007Tabpage.Controls.Add(this.msAccess2007PreviousListBox);
			this.msAccess2007Tabpage.Controls.Add(this.exportAccess2007Button);
			this.msAccess2007Tabpage.Controls.Add(this.importAccess2007Button);
			componentResourceManager.ApplyResources(this.msAccess2007Tabpage, "msAccess2007Tabpage");
			this.msAccess2007Tabpage.Name = "msAccess2007Tabpage";
			this.msAccess2007Tabpage.UseVisualStyleBackColor = true;
			componentResourceManager.ApplyResources(this.access2007Label, "access2007Label");
			this.access2007Label.Name = "access2007Label";
			this.msAccess2007PreviousListBox.ContextMenuStrip = this.contextMenuStripHisotry;
			this.msAccess2007PreviousListBox.FormattingEnabled = true;
			componentResourceManager.ApplyResources(this.msAccess2007PreviousListBox, "msAccess2007PreviousListBox");
			this.msAccess2007PreviousListBox.Name = "msAccess2007PreviousListBox";
			componentResourceManager.ApplyResources(this.exportAccess2007Button, "exportAccess2007Button");
			this.exportAccess2007Button.DialogResult = DialogResult.OK;
			this.exportAccess2007Button.Name = "exportAccess2007Button";
			this.exportAccess2007Button.UseVisualStyleBackColor = true;
			this.exportAccess2007Button.Click += new System.EventHandler(this.exportAccess2007_Click);
			componentResourceManager.ApplyResources(this.importAccess2007Button, "importAccess2007Button");
			this.importAccess2007Button.DialogResult = DialogResult.OK;
			this.importAccess2007Button.Name = "importAccess2007Button";
			this.importAccess2007Button.UseVisualStyleBackColor = true;
			this.importAccess2007Button.Click += new System.EventHandler(this.importAccess2007_Click);
			this.odbcTabpage.Controls.Add(this.odbcLabel);
			this.odbcTabpage.Controls.Add(this.odbcPreviousListBox);
			this.odbcTabpage.Controls.Add(this.importOdbcButton);
			this.odbcTabpage.Controls.Add(this.exportOdbcButton);
			componentResourceManager.ApplyResources(this.odbcTabpage, "odbcTabpage");
			this.odbcTabpage.Name = "odbcTabpage";
			this.odbcTabpage.UseVisualStyleBackColor = true;
			componentResourceManager.ApplyResources(this.odbcLabel, "odbcLabel");
			this.odbcLabel.Name = "odbcLabel";
			this.odbcPreviousListBox.ContextMenuStrip = this.contextMenuStripHisotry;
			this.odbcPreviousListBox.FormattingEnabled = true;
			componentResourceManager.ApplyResources(this.odbcPreviousListBox, "odbcPreviousListBox");
			this.odbcPreviousListBox.Name = "odbcPreviousListBox";
			componentResourceManager.ApplyResources(this, "$this");
			base.AutoScaleMode = AutoScaleMode.Dpi;
			base.Controls.Add(this.tabControl);
			base.FormBorderStyle = FormBorderStyle.FixedDialog;
			base.KeyPreview = true;
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "RevitDBLinkForm";
			base.ShowInTaskbar = false;
			base.Load += new System.EventHandler(this.RevitDBLinkForm_Load);
			base.HelpRequested += new HelpEventHandler(this.RevitDBLinkForm_HelpRequested);
			base.KeyDown += new KeyEventHandler(this.RevitDBLinkForm_KeyDown);
			this.tabControl.ResumeLayout(false);
			this.msAccess2003Tabpage.ResumeLayout(false);
			this.msAccess2003Tabpage.PerformLayout();
			this.contextMenuStripHisotry.ResumeLayout(false);
			this.msAccess2007Tabpage.ResumeLayout(false);
			this.msAccess2007Tabpage.PerformLayout();
			this.odbcTabpage.ResumeLayout(false);
			this.odbcTabpage.PerformLayout();
			base.ResumeLayout(false);
		}
	}
}
