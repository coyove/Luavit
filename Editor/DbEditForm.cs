using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Revit.Addon.RevitDBLink.CS
{
	public class DbEditForm : System.Windows.Forms.Form
    {
		private Document m_revitDoc;

		private Autodesk.Revit.ApplicationServices.Application m_revitApp;

		private UIApplication m_revitUiApp;

		private DataSet m_dataSet;

		private IDbCommand m_command;

		private DataGridViewRow m_newDataGridViewRow;

		private TableInfoSet m_tableInfoSet;

		private BindingSource m_bindingSource;

		private DataGridViewCellStyle m_readOnlyStyle = new DataGridViewCellStyle();

		private DataGridViewCellStyle m_toBeDeleteStyle = new DataGridViewCellStyle();

		private DataGridViewCellStyle m_headerCellStyle = new DataGridViewCellStyle();

		private DataGridViewCellStyle m_dataErrorCellStyle = new DataGridViewCellStyle();

		private int m_autoNegativeNewId;

		private static string IdColumnResId = RDBResource.GetColumnId((BuiltInParameter)(-1002100));

		private static string IdColumnResName = RDBResource.GetColumnName((BuiltInParameter)(-1002100));

		private DataFormats.Format m_rdbDataGridViewDataFormat = DataFormats.GetFormat("rdbDataGridViewDataFormat");

		private bool m_initializeFinished;

		private IContainer components;

		private Button okButton;

		private Button cancelButton;

		private ListBox listBox;

		private DataGridView tableDataGridView;

		private ToolStrip toolStrip;

		private ToolStripButton cutToolStripButton;

		private ToolStripButton copyToolStripButton;

		private ToolStripButton pasteToolStripButton;

		private ToolStripButton newRecordToolStripButton;

		private ToolStripButton deleteToolStripButton;

		private SplitContainer splitContainer;

		private MenuStrip menuStrip1;

		private ToolStripMenuItem parametersToolStripMenuItem;

		private Label label1;

		private DbEditForm()
		{
			this.InitializeComponent();
		}

		public DbEditForm(DataSet dataSet, TableInfoSet tableInfoSet, UIApplication revitUiApp) : this()
		{
			this.m_dataSet = dataSet;
			this.m_command = DatabaseManager.CreateCommand();
			this.m_tableInfoSet = tableInfoSet;
			this.m_revitUiApp = revitUiApp;
			this.m_revitApp = revitUiApp.Application;
			this.m_revitDoc = revitUiApp.ActiveUIDocument.Document;
			this.m_bindingSource = new BindingSource();
			this.m_bindingSource.DataSource = this.m_dataSet;
			this.tableDataGridView.AutoGenerateColumns = false;
			this.tableDataGridView.VirtualMode = true;
			this.tableDataGridView.ClipboardCopyMode = DataGridViewClipboardCopyMode.EnableAlwaysIncludeHeaderText;
			this.m_readOnlyStyle.ForeColor = SystemColors.GrayText;
			this.m_headerCellStyle.BackColor =  System.Drawing.Color.Pink;
			this.m_dataErrorCellStyle.BackColor = System.Drawing.Color.Red;
		}

		private void okButton_Click(object sender, System.EventArgs e)
		{
			this.EndEdit();
			base.Close();
		}

		private void EndEdit()
		{
			try
			{
				if (this.tableDataGridView.DataSource != null)
				{
					string text = "End edit DataGridView";
					if (this.m_bindingSource != null && this.m_bindingSource.DataMember != null)
					{
						text = text + ": " + this.m_bindingSource.DataMember;
					}
					Log.WriteLine(text);
					this.tableDataGridView.EndEdit();
					this.BindingContext[this.tableDataGridView.DataSource].EndCurrentEdit();
				}
			}
			catch (NoNullAllowedException ex)
			{
				MessageBox.Show(ex.Message);
			}
		}

		private void DbEditForm_Load(object sender, System.EventArgs e)
		{
			this.InitializeListBox();
		}

		private void listBox_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			this.EndEdit();
			this.InitializeToolstripStatus();
			this.UpdateDataGridView();
		}

		private void cancelButton_Click(object sender, System.EventArgs e)
		{
			base.Close();
		}

		private void tableDataGridView_DataError(object sender, DataGridViewDataErrorEventArgs e)
		{
			string text = string.Format(RDBResource.GetString("tableDataGridView_DataError"), new object[]
			{
				e.RowIndex,
				e.ColumnIndex,
				e.Context,
				e.Exception
			});
			Log.WriteLine(text);
			MessageBox.Show(text);
		}

		private void tableDataGridView_UserAddedRow(object sender, DataGridViewRowEventArgs e)
		{
			DataGridViewCell dataGridViewCell;
			try
			{
				int index = e.Row.Index;
				dataGridViewCell = this.tableDataGridView.Rows[index - 1].Cells[DbEditForm.IdColumnResId];
			}
			catch (System.Exception ex)
			{
				Log.WriteLine(ex.ToString());
				return;
			}
			dataGridViewCell.Value = --this.m_autoNegativeNewId;
		}

		private void tableDataGridView_NewRowNeeded(object sender, DataGridViewRowEventArgs e)
		{
			DataGridViewCell dataGridViewCell;
			try
			{
				dataGridViewCell = e.Row.Cells[DbEditForm.IdColumnResId];
			}
			catch (System.Exception ex)
			{
				Log.WriteLine(ex.ToString());
				return;
			}
			dataGridViewCell.ReadOnly = true;
			dataGridViewCell.Style = this.m_readOnlyStyle;
		}

		private void tableDataGridView_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
		{
			this.m_newDataGridViewRow = this.tableDataGridView.Rows[e.RowIndex];
			this.m_newDataGridViewRow.Tag = RowTag.ANewRow;
		}

		private void copyToolStripButton_Click(object sender, System.EventArgs e)
		{
			this.CopyToClipBoard();
		}

		private void cutToolStripButton_Click(object sender, System.EventArgs e)
		{
			this.CopyToClipBoard();
			foreach (DataGridViewRow dataGridViewRow in this.tableDataGridView.SelectedRows)
			{
				if (!dataGridViewRow.IsNewRow)
				{
					this.tableDataGridView.Rows.Remove(dataGridViewRow);
				}
			}
		}

		private void pasteToolStripButton_Click(object sender, System.EventArgs e)
		{
			if (this.tableDataGridView.SelectedRows.Count == 0)
			{
				return;
			}
			IDataObject dataObject = Clipboard.GetDataObject();
			if (Clipboard.ContainsData(this.m_rdbDataGridViewDataFormat.Name))
			{
				this.PasteFromClipBoard(dataObject, this.tableDataGridView.SelectedRows[0].Index);
			}
			this.tableDataGridView.Update();
		}

		private void newRecordToolStripButton_Click(object sender, System.EventArgs e)
		{
			if (this.tableDataGridView.CurrentRow.IsNewRow)
			{
				this.tableDataGridView.CancelEdit();
			}
			DataTable dataTable = this.m_dataSet.Tables[this.m_bindingSource.DataMember.ToString()];
			DataRow dataRow = dataTable.NewRow();
			string idColumnResName = DbEditForm.IdColumnResName;
			if (dataTable.Columns.Contains(idColumnResName))
			{
				dataRow[idColumnResName] = --this.m_autoNegativeNewId;
				dataTable.Rows.Add(dataRow);
				this.UpdateNewRowStatus();
				this.tableDataGridView.Update();
			}
		}

		private void deleteToolStripButton_Click(object sender, System.EventArgs e)
		{
			foreach (DataGridViewRow dataGridViewRow in this.tableDataGridView.SelectedRows)
			{
				if (!dataGridViewRow.IsNewRow)
				{
					this.tableDataGridView.Rows.Remove(dataGridViewRow);
				}
			}
		}

		private void tableDataGridView_SelectionChanged(object sender, System.EventArgs e)
		{
			bool flag = this.tableDataGridView.SelectedRows.Count > 0;
			foreach (DataGridViewRow dataGridViewRow in this.tableDataGridView.SelectedRows)
			{
				RowTag rowTag = dataGridViewRow.Tag as RowTag;
				if (rowTag != null && rowTag.Equals(RowTag.AnOldRowExist))
				{
					flag = false;
				}
			}
			this.copyToolStripButton.Enabled = (this.tableDataGridView.SelectedRows.Count > 0);
			if (flag)
			{
				this.pasteToolStripButton.Enabled = Clipboard.ContainsText();
				this.cutToolStripButton.Enabled = true;
				this.deleteToolStripButton.Enabled = true;
				return;
			}
			this.pasteToolStripButton.Enabled = false;
			this.cutToolStripButton.Enabled = false;
			this.deleteToolStripButton.Enabled = false;
		}

		private void DbEditForm_FormClosed(object sender, FormClosedEventArgs e)
		{
			this.EndEdit();
		}

		private void parametersToolStripMenuItem_Click(object sender, System.EventArgs e)
		{
			using (ParameterListForm parameterListForm = new ParameterListForm(this.m_revitUiApp, this.m_dataSet))
			{
				if (parameterListForm.ShowDialog() == DialogResult.OK)
				{
					this.UpdateDataGridView();
				}
			}
		}

		private void InitializeListBox()
		{
			this.m_initializeFinished = false;
			this.listBox.Sorted = true;
			this.listBox.DataSource = this.m_tableInfoSet.Values.ToList<TableInfo>();
			this.listBox.DisplayMember = "Name";
			this.m_initializeFinished = true;
		}

		private void UpdateDataGridView()
		{
			if (!this.m_initializeFinished)
			{
				return;
			}
			this.m_autoNegativeNewId = 0;
			TableInfo tableInfo = this.listBox.SelectedItem as TableInfo;
			this.tableDataGridView.Columns.Clear();
			this.LoadDataTable(tableInfo.Name);
			this.m_bindingSource.DataMember = tableInfo.Name;
			this.tableDataGridView.DataSource = this.m_bindingSource;
			if (tableInfo != null)
			{
				System.Collections.Generic.Dictionary<string, ForeignKey> dictionary = new System.Collections.Generic.Dictionary<string, ForeignKey>();
				foreach (ForeignKey current in tableInfo.ForeignKeys)
				{
					dictionary.Add(current.ColumnId, current);
				}
				foreach (ColumnInfo current2 in tableInfo.Values)
				{
					string name = current2.Name;
					DataGridViewColumn dataGridViewColumn;
					if (dictionary.ContainsKey(current2.ColumnId))
					{
						DataGridViewComboBoxColumn dataGridViewComboBoxColumn = new DataGridViewComboBoxColumn();
						TableInfo tableInfo2 = this.m_tableInfoSet[dictionary[current2.ColumnId].RefTableId];
						dataGridViewComboBoxColumn.DataSource = this.LoadDataTable(dictionary[current2.ColumnId].RefTableName);
						if (tableInfo2.DisplayColumn == null)
						{
							dataGridViewComboBoxColumn.DisplayMember = dictionary[current2.ColumnId].RefColumnName;
						}
						else
						{
							dataGridViewComboBoxColumn.ValueMember = dictionary[current2.ColumnId].RefColumnName;
							dataGridViewComboBoxColumn.DisplayMember = tableInfo2.DisplayColumnName;
						}
						dataGridViewColumn = dataGridViewComboBoxColumn;
					}
					else
					{
						DataGridViewTextBoxColumn dataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
						dataGridViewColumn = dataGridViewTextBoxColumn;
					}
					dataGridViewColumn.Name = current2.ColumnId;
					dataGridViewColumn.SortMode = DataGridViewColumnSortMode.NotSortable;
					dataGridViewColumn.DataPropertyName = name;
					dataGridViewColumn.HeaderText = name;
					this.tableDataGridView.Columns.Add(dataGridViewColumn);
				}
			}
			this.SetDataGridViewTags(tableInfo);
			this.SetDataGridViewAppearence();
			this.InitializeAutoNegativeNewId();
		}

		private DataTable LoadDataTable(string tableName)
		{
			if (this.m_dataSet.Tables.Contains(tableName))
			{
				return this.m_dataSet.Tables[tableName];
			}
			Log.WriteLine("Load DataTable = " + tableName);
			DataTable dataTable = this.m_dataSet.Tables.Add(tableName);
			Command.RefreshDataTable(dataTable);
			return dataTable;
		}

		private void InitializeAutoNegativeNewId()
		{
			if (this.tableDataGridView.Columns.Contains(DbEditForm.IdColumnResId) && this.tableDataGridView.Rows.Count != 0)
			{
				foreach (DataGridViewRow dataGridViewRow in ((System.Collections.IEnumerable)this.tableDataGridView.Rows))
				{
					if (!dataGridViewRow.IsNewRow && this.m_autoNegativeNewId > int.Parse(dataGridViewRow.Cells[DbEditForm.IdColumnResId].Value.ToString()))
					{
						this.m_autoNegativeNewId = System.Convert.ToInt32(dataGridViewRow.Cells[DbEditForm.IdColumnResId].Value);
					}
				}
			}
		}

		private void InitializeToolstripStatus()
		{
			Clipboard.Clear();
			this.cutToolStripButton.Enabled = false;
			this.copyToolStripButton.Enabled = false;
			this.pasteToolStripButton.Enabled = false;
			this.newRecordToolStripButton.Enabled = false;
			this.deleteToolStripButton.Enabled = false;
		}

		private void SetDataGridViewAppearence()
		{
			TableTag tableTag = this.tableDataGridView.Tag as TableTag;
			if (tableTag != null)
			{
				foreach (DataGridViewRow dataGridViewRow in ((System.Collections.IEnumerable)this.tableDataGridView.Rows))
				{
					RowTag rowTag = dataGridViewRow.Tag as RowTag;
					if (rowTag != null && rowTag.NotExist)
					{
						if (!tableTag.IsAllowCreate)
						{
							dataGridViewRow.DefaultCellStyle = this.m_toBeDeleteStyle;
						}
						else
						{
							this.SetAllCellsState(dataGridViewRow);
						}
					}
					else if (tableTag.IsCustomTable || tableTag.isReadOnly)
					{
						dataGridViewRow.ReadOnly = true;
						dataGridViewRow.DefaultCellStyle = this.m_readOnlyStyle;
					}
					else
					{
						this.SetAllCellsState(dataGridViewRow);
					}
				}
			}
		}

		private void SetAllCellsState(DataGridViewRow row)
		{
			foreach (DataGridViewCell dataGridViewCell in row.Cells)
			{
				CellTag cellTag = dataGridViewCell.Tag as CellTag;
				if (cellTag != null && cellTag.IsReadOnly)
				{
					dataGridViewCell.ReadOnly = true;
					dataGridViewCell.Style = this.m_readOnlyStyle;
				}
			}
		}

		private void SetDataGridViewTags(TableInfo tableInfo)
		{
			bool @readonly = tableInfo.Readonly;
			if (tableInfo.ObjectList.SupportCreate)
			{
				this.tableDataGridView.AllowUserToAddRows = true;
				this.newRecordToolStripButton.Enabled = true;
			}
			else
			{
				this.tableDataGridView.AllowUserToAddRows = false;
				this.newRecordToolStripButton.Enabled = false;
			}
			if (@readonly)
			{
				this.tableDataGridView.Tag = TableTag.ReadOnlyTable;
			}
			else
			{
				this.tableDataGridView.Tag = (tableInfo.ObjectList.SupportCreate ? TableTag.AllowCreate : TableTag.NotAllowCreate);
				if (this.m_tableInfoSet.CustomTableIds.Contains(tableInfo.TableId))
				{
					this.tableDataGridView.Tag = TableTag.CustomTable;
				}
			}
			DataTable dataTable = this.m_dataSet.Tables[tableInfo.Name];
			for (int i = 0; i < dataTable.Rows.Count; i++)
			{
				DataGridViewRow dataGridViewRow = this.tableDataGridView.Rows[i];
				if (@readonly)
				{
					dataGridViewRow.Tag = RowTag.AnOldRowExist;
				}
				else
				{
					DataRow dataRow = dataTable.Rows[i];
					object revitObject = tableInfo.ObjectList.GetRevitObject(dataRow);
					Element element = revitObject as Element;
					if (revitObject != null)
					{
						dataGridViewRow.Tag = RowTag.AnOldRowExist;
						if (element == null)
						{
							goto IL_249;
						}
						ParameterMap parametersMap = element.ParametersMap;
						using (System.Collections.Generic.Dictionary<string, ColumnInfo>.KeyCollection.Enumerator enumerator = tableInfo.Keys.GetEnumerator())
						{
							while (enumerator.MoveNext())
							{
								string current = enumerator.Current;
								ColumnInfo columnInfo = tableInfo[current];
								DataGridViewCell dataGridViewCell = dataGridViewRow.Cells[current];
								Parameter parameter;
								if (columnInfo.BuiltInParameter == (BuiltInParameter)(-1))
								{
									parameter = APIObjectList.GetParameterByDefinitionName(parametersMap, columnInfo.Name);
								}
								else
								{
									parameter = element.get_Parameter(columnInfo.BuiltInParameter);
								}
								if (parameter == null || parameter.IsReadOnly)
								{
									dataGridViewCell.Tag = CellTag.ReadOnly;
								}
							}
							goto IL_249;
						}
					}
					dataGridViewRow.Tag = RowTag.ANewRow;
					if (tableInfo.ObjectList.SupportCreate)
					{
						foreach (string current2 in tableInfo.Keys)
						{
							ColumnInfo columnInfo2 = tableInfo[current2];
							DataGridViewCell dataGridViewCell2 = dataGridViewRow.Cells[current2];
							if (tableInfo.PrimaryKeys.Contains(columnInfo2.ColumnId))
							{
								dataGridViewCell2.Tag = CellTag.ReadOnly;
							}
						}
					}
				}
				IL_249:;
			}
		}

		private void UpdateNewRowStatus()
		{
			if (this.tableDataGridView.Columns.Contains(DbEditForm.IdColumnResId))
			{
				DataGridViewCell dataGridViewCell = this.m_newDataGridViewRow.Cells[DbEditForm.IdColumnResId];
				dataGridViewCell.Tag = CellTag.ReadOnly;
				dataGridViewCell.ReadOnly = true;
				dataGridViewCell.Style = this.m_readOnlyStyle;
			}
		}

		private void CopyToClipBoard()
		{
			Clipboard.Clear();
			DataObject clipboardContent = this.tableDataGridView.GetClipboardContent();
			object[] array = new object[this.tableDataGridView.SelectedRows.Count];
			for (int i = 0; i < this.tableDataGridView.SelectedRows.Count; i++)
			{
				object[] array2 = new object[this.tableDataGridView.SelectedRows[i].Cells.Count];
				for (int j = 0; j < array2.Length; j++)
				{
					array2[j] = this.tableDataGridView.SelectedRows[i].Cells[j].Value;
				}
				array[i] = array2;
			}
			clipboardContent.SetData(this.m_rdbDataGridViewDataFormat.Name, array);
			Clipboard.SetDataObject(clipboardContent);
		}

		private void PasteFromClipBoard(IDataObject dataObject, int startRowIndex)
		{
			object data = dataObject.GetData(this.m_rdbDataGridViewDataFormat.Name);
			if (data != null)
			{
				this.tableDataGridView.CancelEdit();
				int num = startRowIndex;
				object[] array = (object[])data;
				DataTable dataTable = this.m_dataSet.Tables[this.m_bindingSource.DataMember];
				for (int i = 0; i < array.Length; i++)
				{
					DataGridViewRow dataGridViewRow = this.tableDataGridView.Rows[num];
					object[] array2 = array[i] as object[];
					DataRow dataRow = dataGridViewRow.IsNewRow ? dataTable.NewRow() : dataTable.Rows[num];
					for (int j = 0; j < array2.Length; j++)
					{
						if (this.tableDataGridView.Columns[j].Name == DbEditForm.IdColumnResId)
						{
							dataRow[j] = --this.m_autoNegativeNewId;
						}
						else
						{
							dataRow[j] = array2[j];
						}
					}
					if (dataGridViewRow.IsNewRow)
					{
						dataTable.Rows.Add(dataRow);
					}
					this.UpdateNewRowStatus();
					num++;
				}
			}
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
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(DbEditForm));
			this.okButton = new Button();
			this.cancelButton = new Button();
			this.listBox = new ListBox();
			this.tableDataGridView = new DataGridView();
			this.toolStrip = new ToolStrip();
			this.cutToolStripButton = new ToolStripButton();
			this.copyToolStripButton = new ToolStripButton();
			this.pasteToolStripButton = new ToolStripButton();
			this.newRecordToolStripButton = new ToolStripButton();
			this.deleteToolStripButton = new ToolStripButton();
			this.splitContainer = new SplitContainer();
			this.label1 = new Label();
			this.menuStrip1 = new MenuStrip();
			this.parametersToolStripMenuItem = new ToolStripMenuItem();
			((ISupportInitialize)this.tableDataGridView).BeginInit();
			this.toolStrip.SuspendLayout();
			this.splitContainer.Panel1.SuspendLayout();
			this.splitContainer.Panel2.SuspendLayout();
			this.splitContainer.SuspendLayout();
			this.menuStrip1.SuspendLayout();
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
			componentResourceManager.ApplyResources(this.listBox, "listBox");
			this.listBox.Name = "listBox";
			this.listBox.SelectedIndexChanged += new System.EventHandler(this.listBox_SelectedIndexChanged);
			this.tableDataGridView.AllowUserToDeleteRows = false;
			componentResourceManager.ApplyResources(this.tableDataGridView, "tableDataGridView");
			this.tableDataGridView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.tableDataGridView.Name = "tableDataGridView";
			this.tableDataGridView.UserAddedRow += new DataGridViewRowEventHandler(this.tableDataGridView_UserAddedRow);
			this.tableDataGridView.RowsAdded += new DataGridViewRowsAddedEventHandler(this.tableDataGridView_RowsAdded);
			this.tableDataGridView.DataError += new DataGridViewDataErrorEventHandler(this.tableDataGridView_DataError);
			this.tableDataGridView.NewRowNeeded += new DataGridViewRowEventHandler(this.tableDataGridView_NewRowNeeded);
			this.tableDataGridView.SelectionChanged += new System.EventHandler(this.tableDataGridView_SelectionChanged);
			this.toolStrip.Items.AddRange(new ToolStripItem[]
			{
				this.cutToolStripButton,
				this.copyToolStripButton,
				this.pasteToolStripButton,
				this.newRecordToolStripButton,
				this.deleteToolStripButton
			});
			componentResourceManager.ApplyResources(this.toolStrip, "toolStrip");
			this.toolStrip.Name = "toolStrip";
			componentResourceManager.ApplyResources(this.cutToolStripButton, "cutToolStripButton");
			this.cutToolStripButton.Name = "cutToolStripButton";
			this.cutToolStripButton.Click += new System.EventHandler(this.cutToolStripButton_Click);
			componentResourceManager.ApplyResources(this.copyToolStripButton, "copyToolStripButton");
			this.copyToolStripButton.Name = "copyToolStripButton";
			this.copyToolStripButton.Click += new System.EventHandler(this.copyToolStripButton_Click);
			componentResourceManager.ApplyResources(this.pasteToolStripButton, "pasteToolStripButton");
			this.pasteToolStripButton.Name = "pasteToolStripButton";
			this.pasteToolStripButton.Click += new System.EventHandler(this.pasteToolStripButton_Click);
			componentResourceManager.ApplyResources(this.newRecordToolStripButton, "newRecordToolStripButton");
			this.newRecordToolStripButton.Name = "newRecordToolStripButton";
			this.newRecordToolStripButton.Click += new System.EventHandler(this.newRecordToolStripButton_Click);
			componentResourceManager.ApplyResources(this.deleteToolStripButton, "deleteToolStripButton");
			this.deleteToolStripButton.Name = "deleteToolStripButton";
			this.deleteToolStripButton.Click += new System.EventHandler(this.deleteToolStripButton_Click);
			componentResourceManager.ApplyResources(this.splitContainer, "splitContainer");
			this.splitContainer.Name = "splitContainer";
			this.splitContainer.Panel1.Controls.Add(this.label1);
			this.splitContainer.Panel1.Controls.Add(this.listBox);
			this.splitContainer.Panel2.Controls.Add(this.toolStrip);
			this.splitContainer.Panel2.Controls.Add(this.tableDataGridView);
			componentResourceManager.ApplyResources(this.label1, "label1");
			this.label1.Name = "label1";
			this.menuStrip1.Items.AddRange(new ToolStripItem[]
			{
				this.parametersToolStripMenuItem
			});
			componentResourceManager.ApplyResources(this.menuStrip1, "menuStrip1");
			this.menuStrip1.Name = "menuStrip1";
			this.parametersToolStripMenuItem.Name = "parametersToolStripMenuItem";
			componentResourceManager.ApplyResources(this.parametersToolStripMenuItem, "parametersToolStripMenuItem");
			this.parametersToolStripMenuItem.Click += new System.EventHandler(this.parametersToolStripMenuItem_Click);
			componentResourceManager.ApplyResources(this, "$this");
			base.AutoScaleMode = AutoScaleMode.Dpi;
			base.CancelButton = this.cancelButton;
			base.Controls.Add(this.splitContainer);
			base.Controls.Add(this.cancelButton);
			base.Controls.Add(this.okButton);
			base.Controls.Add(this.menuStrip1);
			this.DoubleBuffered = true;
			base.KeyPreview = true;
			base.MainMenuStrip = this.menuStrip1;
			base.MinimizeBox = false;
			base.Name = "DbEditForm";
			base.ShowIcon = false;
			base.ShowInTaskbar = false;
			base.Load += new System.EventHandler(this.DbEditForm_Load);
			base.FormClosed += new FormClosedEventHandler(this.DbEditForm_FormClosed);
			((ISupportInitialize)this.tableDataGridView).EndInit();
			this.toolStrip.ResumeLayout(false);
			this.toolStrip.PerformLayout();
			this.splitContainer.Panel1.ResumeLayout(false);
			this.splitContainer.Panel1.PerformLayout();
			this.splitContainer.Panel2.ResumeLayout(false);
			this.splitContainer.Panel2.PerformLayout();
			this.splitContainer.ResumeLayout(false);
			this.menuStrip1.ResumeLayout(false);
			this.menuStrip1.PerformLayout();
			base.ResumeLayout(false);
			base.PerformLayout();
		}
	}
}
