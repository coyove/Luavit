using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Revit.Addon.RevitDBLink.CS
{
	public class ParameterPropertyForm : System.Windows.Forms.Form
    {
		private Categories m_categories;

		private Autodesk.Revit.ApplicationServices.Application m_revitApp;

		private Document m_revitDoc;

		private ParameterInfo m_parameterInfo;

		private IContainer components;

		private Button okButton;

		private Button cancelButton;

		private RadioButton sharedParameterRadioButton;

		private RadioButton projectParameterRadioButton;

		private GroupBox groupBox1;

		private GroupBox groupBox2;

		private System.Windows.Forms.TextBox nameTextBox;

		private Label label4;

		private Label label2;

		private Label label1;

		private System.Windows.Forms.ComboBox typeComboBox;

		private System.Windows.Forms.ComboBox groupComboBox;

		private RadioButton typeRadioButton;

		private RadioButton instanceRadioButton;

		private GroupBox groupBox3;

		private Button checkNoneButton;

		private Button checkAllButton;

		private CheckedListBox categoryCheckedListBox;

		public ParameterInfo ParameterInfo
		{
			get
			{
				return this.m_parameterInfo;
			}
		}

		private ParameterPropertyForm()
		{
			this.InitializeComponent();
		}

		public ParameterPropertyForm(UIApplication revitUiApp) : this()
		{
			this.m_revitApp = revitUiApp.Application;
			this.m_revitDoc = revitUiApp.ActiveUIDocument.Document;
			this.m_categories = this.m_revitDoc.Settings.Categories;
			this.m_parameterInfo = new ParameterInfo();
			this.InitializeDataSourcesAndControls();
		}

		private void InitializeDataSourcesAndControls()
		{
			this.groupComboBox.Sorted = true;
			this.groupComboBox.DataSource = System.Enum.GetValues(typeof(BuiltInParameterGroup));
			this.groupComboBox.Format += new ListControlConvertEventHandler(this.groupComboBox_Format);
			this.typeComboBox.Sorted = true;
			this.typeComboBox.DataSource = System.Enum.GetValues(typeof(ParameterType));
			foreach (Category item in this.m_categories)
			{
				this.categoryCheckedListBox.Items.Add(item);
			}
			this.categoryCheckedListBox.DisplayMember = "Name";
			this.projectParameterRadioButton.Checked = this.m_parameterInfo.ParameterIsProject;
			this.sharedParameterRadioButton.Checked = !this.m_parameterInfo.ParameterIsProject;
			this.typeRadioButton.Checked = this.m_parameterInfo.ParameterForType;
			this.instanceRadioButton.Checked = !this.m_parameterInfo.ParameterForType;
			this.nameTextBox.Text = this.m_parameterInfo.ParameterName;
			this.groupComboBox.SelectedItem = this.m_parameterInfo.ParameterGroup;
			this.typeComboBox.SelectedItem = this.m_parameterInfo.ParameterType;
			if (this.m_parameterInfo.Categories != null)
			{
				foreach (Category value in this.m_parameterInfo.Categories)
				{
					int index = this.categoryCheckedListBox.Items.IndexOf(value);
					this.categoryCheckedListBox.SetItemChecked(index, true);
				}
			}
			ParameterPropertyForm.ComboBoxDropDownWidthFitToContent(this.groupComboBox);
			ParameterPropertyForm.ComboBoxDropDownWidthFitToContent(this.typeComboBox);
		}

		private void groupComboBox_Format(object sender, ListControlConvertEventArgs e)
		{
			e.Value = RDBResource.GetString("IDS_" + e.ListItem.ToString());
		}

		private void okButton_Click(object sender, System.EventArgs e)
		{
			CategorySet categorySet = new CategorySet();
			foreach (Category category in this.categoryCheckedListBox.CheckedItems)
			{
				categorySet.Insert(category);
			}
			this.m_parameterInfo.Categories = categorySet;
			string text = this.nameTextBox.Text.Trim();
			if (text == string.Empty)
			{
				MessageBox.Show(RDBResource.GetString("MessageBox_Name_Shoud_Not_Be_Empty"));
				return;
			}
			if (char.IsNumber(text[0]) || text[0] == ' ')
			{
				MessageBox.Show(RDBResource.GetString("MessageBox_Name_Should_Start_With_Letter"));
				return;
			}
			string text2 = text;
			for (int i = 0; i < text2.Length; i++)
			{
				char c = text2[i];
				if (!char.IsLetterOrDigit(c) && c != '_' && c != ' ')
				{
					MessageBox.Show(string.Format(RDBResource.GetString("MessageBox_Name_Contains_Illegal_Characters"), c));
					return;
				}
			}
			if (this.categoryCheckedListBox.CheckedItems.Count == 0)
			{
				MessageBox.Show(RDBResource.GetString("MessageBox_Select_One_Category"));
				return;
			}
			this.m_parameterInfo.ParameterIsProject = this.projectParameterRadioButton.Checked;
			this.m_parameterInfo.ParameterForType = this.typeRadioButton.Checked;
			this.m_parameterInfo.ParameterName = text;
			this.m_parameterInfo.ParameterGroup = (BuiltInParameterGroup)this.groupComboBox.SelectedItem;
			this.m_parameterInfo.ParameterType = (ParameterType)this.typeComboBox.SelectedItem;
			base.Close();
			base.DialogResult = DialogResult.OK;
		}

		private void cancelButton_Click(object sender, System.EventArgs e)
		{
			base.Close();
		}

		private void checkAllButton_Click(object sender, System.EventArgs e)
		{
			for (int i = 0; i < this.categoryCheckedListBox.Items.Count; i++)
			{
				this.categoryCheckedListBox.SetItemChecked(i, true);
			}
		}

		private void checkNoneButton_Click(object sender, System.EventArgs e)
		{
			foreach (int index in this.categoryCheckedListBox.CheckedIndices)
			{
				this.categoryCheckedListBox.SetItemChecked(index, false);
			}
		}

		private static void ComboBoxDropDownWidthFitToContent(System.Windows.Forms.ComboBox cbo)
		{
			cbo.DropDownWidth = ParameterPropertyForm.GetLargestTextExtent(cbo);
		}

		private static int GetLargestTextExtent(System.Windows.Forms.ComboBox cbo)
		{
			int num = -1;
			if (cbo.Items.Count >= 1)
			{
				using (Graphics graphics = cbo.CreateGraphics())
				{
					int num2 = 0;
					if (cbo.Items.Count > cbo.MaxDropDownItems)
					{
						num2 = SystemInformation.VerticalScrollBarWidth;
					}
					for (int i = 0; i < cbo.Items.Count; i++)
					{
						int num3 = (int)graphics.MeasureString(cbo.Items[i].ToString(), cbo.Font).Width + num2;
						if (num3 > num)
						{
							num = num3;
						}
					}
				}
			}
			return num;
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
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(ParameterPropertyForm));
			this.okButton = new Button();
			this.cancelButton = new Button();
			this.sharedParameterRadioButton = new RadioButton();
			this.projectParameterRadioButton = new RadioButton();
			this.groupBox1 = new GroupBox();
			this.groupBox2 = new GroupBox();
			this.typeRadioButton = new RadioButton();
			this.instanceRadioButton = new RadioButton();
			this.typeComboBox = new System.Windows.Forms.ComboBox();
			this.groupComboBox = new System.Windows.Forms.ComboBox();
			this.nameTextBox = new System.Windows.Forms.TextBox();
			this.label4 = new Label();
			this.label2 = new Label();
			this.label1 = new Label();
			this.groupBox3 = new GroupBox();
			this.categoryCheckedListBox = new CheckedListBox();
			this.checkNoneButton = new Button();
			this.checkAllButton = new Button();
			this.groupBox1.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.groupBox3.SuspendLayout();
			base.SuspendLayout();
			componentResourceManager.ApplyResources(this.okButton, "okButton");
			this.okButton.Name = "okButton";
			this.okButton.UseVisualStyleBackColor = true;
			this.okButton.Click += new System.EventHandler(this.okButton_Click);
			componentResourceManager.ApplyResources(this.cancelButton, "cancelButton");
			this.cancelButton.DialogResult = DialogResult.Cancel;
			this.cancelButton.Name = "cancelButton";
			this.cancelButton.UseVisualStyleBackColor = true;
			this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
			componentResourceManager.ApplyResources(this.sharedParameterRadioButton, "sharedParameterRadioButton");
			this.sharedParameterRadioButton.Checked = true;
			this.sharedParameterRadioButton.Name = "sharedParameterRadioButton";
			this.sharedParameterRadioButton.TabStop = true;
			this.sharedParameterRadioButton.UseVisualStyleBackColor = true;
			componentResourceManager.ApplyResources(this.projectParameterRadioButton, "projectParameterRadioButton");
			this.projectParameterRadioButton.Name = "projectParameterRadioButton";
			this.projectParameterRadioButton.UseVisualStyleBackColor = true;
			this.groupBox1.Controls.Add(this.sharedParameterRadioButton);
			this.groupBox1.Controls.Add(this.projectParameterRadioButton);
			componentResourceManager.ApplyResources(this.groupBox1, "groupBox1");
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.TabStop = false;
			this.groupBox2.Controls.Add(this.typeRadioButton);
			this.groupBox2.Controls.Add(this.instanceRadioButton);
			this.groupBox2.Controls.Add(this.typeComboBox);
			this.groupBox2.Controls.Add(this.groupComboBox);
			this.groupBox2.Controls.Add(this.nameTextBox);
			this.groupBox2.Controls.Add(this.label4);
			this.groupBox2.Controls.Add(this.label2);
			this.groupBox2.Controls.Add(this.label1);
			componentResourceManager.ApplyResources(this.groupBox2, "groupBox2");
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.TabStop = false;
			componentResourceManager.ApplyResources(this.typeRadioButton, "typeRadioButton");
			this.typeRadioButton.Name = "typeRadioButton";
			this.typeRadioButton.UseVisualStyleBackColor = true;
			componentResourceManager.ApplyResources(this.instanceRadioButton, "instanceRadioButton");
			this.instanceRadioButton.Checked = true;
			this.instanceRadioButton.Name = "instanceRadioButton";
			this.instanceRadioButton.TabStop = true;
			this.instanceRadioButton.UseVisualStyleBackColor = true;
			this.typeComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
			this.typeComboBox.FormattingEnabled = true;
			componentResourceManager.ApplyResources(this.typeComboBox, "typeComboBox");
			this.typeComboBox.Name = "typeComboBox";
			this.groupComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
			this.groupComboBox.FormattingEnabled = true;
			componentResourceManager.ApplyResources(this.groupComboBox, "groupComboBox");
			this.groupComboBox.Name = "groupComboBox";
			componentResourceManager.ApplyResources(this.nameTextBox, "nameTextBox");
			this.nameTextBox.Name = "nameTextBox";
			componentResourceManager.ApplyResources(this.label4, "label4");
			this.label4.Name = "label4";
			componentResourceManager.ApplyResources(this.label2, "label2");
			this.label2.Name = "label2";
			componentResourceManager.ApplyResources(this.label1, "label1");
			this.label1.Name = "label1";
			this.groupBox3.Controls.Add(this.categoryCheckedListBox);
			this.groupBox3.Controls.Add(this.checkNoneButton);
			this.groupBox3.Controls.Add(this.checkAllButton);
			componentResourceManager.ApplyResources(this.groupBox3, "groupBox3");
			this.groupBox3.Name = "groupBox3";
			this.groupBox3.TabStop = false;
			this.categoryCheckedListBox.CheckOnClick = true;
			this.categoryCheckedListBox.FormattingEnabled = true;
			componentResourceManager.ApplyResources(this.categoryCheckedListBox, "categoryCheckedListBox");
			this.categoryCheckedListBox.Name = "categoryCheckedListBox";
			this.categoryCheckedListBox.Sorted = true;
			componentResourceManager.ApplyResources(this.checkNoneButton, "checkNoneButton");
			this.checkNoneButton.Name = "checkNoneButton";
			this.checkNoneButton.UseVisualStyleBackColor = true;
			this.checkNoneButton.Click += new System.EventHandler(this.checkNoneButton_Click);
			componentResourceManager.ApplyResources(this.checkAllButton, "checkAllButton");
			this.checkAllButton.Name = "checkAllButton";
			this.checkAllButton.UseVisualStyleBackColor = true;
			this.checkAllButton.Click += new System.EventHandler(this.checkAllButton_Click);
			base.AcceptButton = this.okButton;
			componentResourceManager.ApplyResources(this, "$this");
			base.AutoScaleMode = AutoScaleMode.Dpi;
			base.CancelButton = this.cancelButton;
			base.Controls.Add(this.groupBox3);
			base.Controls.Add(this.groupBox2);
			base.Controls.Add(this.groupBox1);
			base.Controls.Add(this.cancelButton);
			base.Controls.Add(this.okButton);
			base.FormBorderStyle = FormBorderStyle.FixedDialog;
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "ParameterPropertyForm";
			base.ShowInTaskbar = false;
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.groupBox2.ResumeLayout(false);
			this.groupBox2.PerformLayout();
			this.groupBox3.ResumeLayout(false);
			base.ResumeLayout(false);
		}
	}
}
