namespace Luavit
{
    partial class TypeForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TypeForm));
            this.MainContainer = new System.Windows.Forms.SplitContainer();
            this.mainTab = new System.Windows.Forms.TabControl();
            this.listLog = new System.Windows.Forms.ListView();
            this.logTime = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.logProvider = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.logContent = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.textLua = new FastColoredTextBoxNS.FastColoredTextBox();
            this.topMenu = new System.Windows.Forms.MenuStrip();
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newScriptToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
            this.openScriptToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem5 = new System.Windows.Forms.ToolStripSeparator();
            this.saveScriptToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveAsSnippetToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.closeTabToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.kitchenMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.runToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.runScriptToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.debugToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem4 = new System.Windows.Forms.ToolStripSeparator();
            this.fulldescrOutputToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.importToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.autodeskRevitDBToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.autodeskRevitDBArchitectureToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.autodeskRevitUIToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.autodeskRevitUISelectionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.autodeskRevitCreationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.logOutputToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem6 = new System.Windows.Forms.ToolStripSeparator();
            this.clearLogOutputToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewLogToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.zToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.currentToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem7 = new System.Windows.Forms.ToolStripSeparator();
            this.configurationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.MainContainer)).BeginInit();
            this.MainContainer.Panel1.SuspendLayout();
            this.MainContainer.Panel2.SuspendLayout();
            this.MainContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.textLua)).BeginInit();
            this.topMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // MainContainer
            // 
            this.MainContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MainContainer.Location = new System.Drawing.Point(0, 28);
            this.MainContainer.Name = "MainContainer";
            this.MainContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // MainContainer.Panel1
            // 
            this.MainContainer.Panel1.Controls.Add(this.mainTab);
            // 
            // MainContainer.Panel2
            // 
            this.MainContainer.Panel2.Controls.Add(this.listLog);
            this.MainContainer.Panel2.Controls.Add(this.textLua);
            this.MainContainer.Size = new System.Drawing.Size(805, 494);
            this.MainContainer.SplitterDistance = 306;
            this.MainContainer.TabIndex = 9;
            // 
            // mainTab
            // 
            this.mainTab.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainTab.Location = new System.Drawing.Point(0, 0);
            this.mainTab.Name = "mainTab";
            this.mainTab.SelectedIndex = 0;
            this.mainTab.Size = new System.Drawing.Size(805, 306);
            this.mainTab.TabIndex = 0;
            this.mainTab.SelectedIndexChanged += new System.EventHandler(this.mainTab_SelectedIndexChanged);
            this.mainTab.MouseClick += new System.Windows.Forms.MouseEventHandler(this.mainTab_MouseClick);
            // 
            // listLog
            // 
            this.listLog.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.logTime,
            this.logProvider,
            this.logContent});
            this.listLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listLog.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.listLog.FullRowSelect = true;
            this.listLog.Location = new System.Drawing.Point(0, 0);
            this.listLog.Name = "listLog";
            this.listLog.Size = new System.Drawing.Size(805, 184);
            this.listLog.TabIndex = 13;
            this.listLog.UseCompatibleStateImageBehavior = false;
            this.listLog.View = System.Windows.Forms.View.Details;
            this.listLog.VirtualMode = true;
            this.listLog.RetrieveVirtualItem += new System.Windows.Forms.RetrieveVirtualItemEventHandler(this.listLog_RetrieveVirtualItem);
            this.listLog.SelectedIndexChanged += new System.EventHandler(this.listLog_SelectedIndexChanged);
            // 
            // logTime
            // 
            this.logTime.Text = "Time";
            // 
            // logProvider
            // 
            this.logProvider.Text = "Provider";
            // 
            // logContent
            // 
            this.logContent.Text = "Content";
            this.logContent.Width = 189;
            // 
            // textLua
            // 
            this.textLua.AutoCompleteBracketsList = new char[] {
        '(',
        ')',
        '{',
        '}',
        '[',
        ']',
        '\"',
        '\"',
        '\'',
        '\''};
            this.textLua.AutoScrollMinSize = new System.Drawing.Size(2, 21);
            this.textLua.BackBrush = null;
            this.textLua.CharHeight = 21;
            this.textLua.CharWidth = 10;
            this.textLua.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.textLua.DisabledColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))));
            this.textLua.Font = new System.Drawing.Font("Consolas", 10.8F);
            this.textLua.IsReplaceMode = false;
            this.textLua.Location = new System.Drawing.Point(678, 25);
            this.textLua.Name = "textLua";
            this.textLua.Paddings = new System.Windows.Forms.Padding(0);
            this.textLua.SelectionColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(255)))));
            this.textLua.ServiceColors = ((FastColoredTextBoxNS.ServiceColors)(resources.GetObject("textLua.ServiceColors")));
            this.textLua.Size = new System.Drawing.Size(87, 73);
            this.textLua.TabIndex = 12;
            this.textLua.Visible = false;
            this.textLua.Zoom = 100;
            // 
            // topMenu
            // 
            this.topMenu.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.topMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.editToolStripMenuItem,
            this.kitchenMenuItem,
            this.runToolStripMenuItem,
            this.importToolStripMenuItem,
            this.viewToolStripMenuItem});
            this.topMenu.Location = new System.Drawing.Point(0, 0);
            this.topMenu.Name = "topMenu";
            this.topMenu.Size = new System.Drawing.Size(805, 28);
            this.topMenu.TabIndex = 10;
            this.topMenu.Text = "menuStrip1";
            // 
            // editToolStripMenuItem
            // 
            this.editToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newScriptToolStripMenuItem,
            this.toolStripMenuItem2,
            this.openScriptToolStripMenuItem,
            this.toolStripMenuItem5,
            this.saveScriptToolStripMenuItem,
            this.saveAsToolStripMenuItem,
            this.saveAsSnippetToolStripMenuItem,
            this.toolStripMenuItem1,
            this.closeTabToolStripMenuItem,
            this.toolStripMenuItem3,
            this.exitToolStripMenuItem});
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            this.editToolStripMenuItem.Size = new System.Drawing.Size(71, 24);
            this.editToolStripMenuItem.Text = "&Recipe";
            this.editToolStripMenuItem.Click += new System.EventHandler(this.editToolStripMenuItem_Click);
            // 
            // newScriptToolStripMenuItem
            // 
            this.newScriptToolStripMenuItem.Name = "newScriptToolStripMenuItem";
            this.newScriptToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.N)));
            this.newScriptToolStripMenuItem.Size = new System.Drawing.Size(244, 26);
            this.newScriptToolStripMenuItem.Text = "&New recipe";
            this.newScriptToolStripMenuItem.Click += new System.EventHandler(this.newScriptToolStripMenuItem_Click);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(241, 6);
            // 
            // openScriptToolStripMenuItem
            // 
            this.openScriptToolStripMenuItem.Name = "openScriptToolStripMenuItem";
            this.openScriptToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.openScriptToolStripMenuItem.Size = new System.Drawing.Size(244, 26);
            this.openScriptToolStripMenuItem.Text = "Open recipe...";
            this.openScriptToolStripMenuItem.Click += new System.EventHandler(this.openScriptToolStripMenuItem_Click);
            // 
            // toolStripMenuItem5
            // 
            this.toolStripMenuItem5.Name = "toolStripMenuItem5";
            this.toolStripMenuItem5.Size = new System.Drawing.Size(241, 6);
            // 
            // saveScriptToolStripMenuItem
            // 
            this.saveScriptToolStripMenuItem.Name = "saveScriptToolStripMenuItem";
            this.saveScriptToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.saveScriptToolStripMenuItem.Size = new System.Drawing.Size(244, 26);
            this.saveScriptToolStripMenuItem.Text = "&Save recipe...";
            this.saveScriptToolStripMenuItem.Click += new System.EventHandler(this.saveScriptToolStripMenuItem_Click);
            // 
            // saveAsToolStripMenuItem
            // 
            this.saveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem";
            this.saveAsToolStripMenuItem.Size = new System.Drawing.Size(244, 26);
            this.saveAsToolStripMenuItem.Text = "Save as...";
            this.saveAsToolStripMenuItem.Click += new System.EventHandler(this.saveAsToolStripMenuItem_Click);
            // 
            // saveAsSnippetToolStripMenuItem
            // 
            this.saveAsSnippetToolStripMenuItem.Name = "saveAsSnippetToolStripMenuItem";
            this.saveAsSnippetToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F2;
            this.saveAsSnippetToolStripMenuItem.Size = new System.Drawing.Size(244, 26);
            this.saveAsSnippetToolStripMenuItem.Text = "&Save to kitchen...";
            this.saveAsSnippetToolStripMenuItem.Click += new System.EventHandler(this.saveAsSnippetToolStripMenuItem_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(241, 6);
            // 
            // closeTabToolStripMenuItem
            // 
            this.closeTabToolStripMenuItem.Name = "closeTabToolStripMenuItem";
            this.closeTabToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.W)));
            this.closeTabToolStripMenuItem.Size = new System.Drawing.Size(244, 26);
            this.closeTabToolStripMenuItem.Text = "Close tab";
            this.closeTabToolStripMenuItem.Click += new System.EventHandler(this.closeTabToolStripMenuItem_Click);
            // 
            // toolStripMenuItem3
            // 
            this.toolStripMenuItem3.Name = "toolStripMenuItem3";
            this.toolStripMenuItem3.Size = new System.Drawing.Size(241, 6);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(244, 26);
            this.exitToolStripMenuItem.Text = "E&xit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // kitchenMenuItem
            // 
            this.kitchenMenuItem.Name = "kitchenMenuItem";
            this.kitchenMenuItem.Size = new System.Drawing.Size(76, 24);
            this.kitchenMenuItem.Text = "&Kitchen";
            // 
            // runToolStripMenuItem
            // 
            this.runToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.runScriptToolStripMenuItem,
            this.debugToolStripMenuItem,
            this.toolStripMenuItem4,
            this.fulldescrOutputToolStripMenuItem});
            this.runToolStripMenuItem.Name = "runToolStripMenuItem";
            this.runToolStripMenuItem.Size = new System.Drawing.Size(49, 24);
            this.runToolStripMenuItem.Text = "&Run";
            // 
            // runScriptToolStripMenuItem
            // 
            this.runScriptToolStripMenuItem.Name = "runScriptToolStripMenuItem";
            this.runScriptToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F5;
            this.runScriptToolStripMenuItem.Size = new System.Drawing.Size(196, 26);
            this.runScriptToolStripMenuItem.Text = "&Run";
            this.runScriptToolStripMenuItem.Click += new System.EventHandler(this.button1_Click);
            // 
            // debugToolStripMenuItem
            // 
            this.debugToolStripMenuItem.Name = "debugToolStripMenuItem";
            this.debugToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F5)));
            this.debugToolStripMenuItem.Size = new System.Drawing.Size(196, 26);
            this.debugToolStripMenuItem.Text = "&Debug";
            this.debugToolStripMenuItem.Click += new System.EventHandler(this.debugToolStripMenuItem_Click);
            // 
            // toolStripMenuItem4
            // 
            this.toolStripMenuItem4.Name = "toolStripMenuItem4";
            this.toolStripMenuItem4.Size = new System.Drawing.Size(193, 6);
            // 
            // fulldescrOutputToolStripMenuItem
            // 
            this.fulldescrOutputToolStripMenuItem.CheckOnClick = true;
            this.fulldescrOutputToolStripMenuItem.Name = "fulldescrOutputToolStripMenuItem";
            this.fulldescrOutputToolStripMenuItem.Size = new System.Drawing.Size(196, 26);
            this.fulldescrOutputToolStripMenuItem.Text = "&Full descr()";
            // 
            // importToolStripMenuItem
            // 
            this.importToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.autodeskRevitDBToolStripMenuItem,
            this.autodeskRevitDBArchitectureToolStripMenuItem,
            this.autodeskRevitUIToolStripMenuItem,
            this.autodeskRevitUISelectionToolStripMenuItem,
            this.autodeskRevitCreationToolStripMenuItem});
            this.importToolStripMenuItem.Name = "importToolStripMenuItem";
            this.importToolStripMenuItem.Size = new System.Drawing.Size(71, 24);
            this.importToolStripMenuItem.Text = "&Import";
            // 
            // autodeskRevitDBToolStripMenuItem
            // 
            this.autodeskRevitDBToolStripMenuItem.Checked = true;
            this.autodeskRevitDBToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.autodeskRevitDBToolStripMenuItem.Enabled = false;
            this.autodeskRevitDBToolStripMenuItem.Name = "autodeskRevitDBToolStripMenuItem";
            this.autodeskRevitDBToolStripMenuItem.Size = new System.Drawing.Size(314, 26);
            this.autodeskRevitDBToolStripMenuItem.Text = "Autodesk.Revit.DB";
            // 
            // autodeskRevitDBArchitectureToolStripMenuItem
            // 
            this.autodeskRevitDBArchitectureToolStripMenuItem.Checked = true;
            this.autodeskRevitDBArchitectureToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.autodeskRevitDBArchitectureToolStripMenuItem.Enabled = false;
            this.autodeskRevitDBArchitectureToolStripMenuItem.Name = "autodeskRevitDBArchitectureToolStripMenuItem";
            this.autodeskRevitDBArchitectureToolStripMenuItem.Size = new System.Drawing.Size(314, 26);
            this.autodeskRevitDBArchitectureToolStripMenuItem.Text = "Autodesk.Revit.DB.Architecture";
            // 
            // autodeskRevitUIToolStripMenuItem
            // 
            this.autodeskRevitUIToolStripMenuItem.Checked = true;
            this.autodeskRevitUIToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.autodeskRevitUIToolStripMenuItem.Enabled = false;
            this.autodeskRevitUIToolStripMenuItem.Name = "autodeskRevitUIToolStripMenuItem";
            this.autodeskRevitUIToolStripMenuItem.Size = new System.Drawing.Size(314, 26);
            this.autodeskRevitUIToolStripMenuItem.Text = "Autodesk.Revit.UI";
            // 
            // autodeskRevitUISelectionToolStripMenuItem
            // 
            this.autodeskRevitUISelectionToolStripMenuItem.Checked = true;
            this.autodeskRevitUISelectionToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.autodeskRevitUISelectionToolStripMenuItem.Enabled = false;
            this.autodeskRevitUISelectionToolStripMenuItem.Name = "autodeskRevitUISelectionToolStripMenuItem";
            this.autodeskRevitUISelectionToolStripMenuItem.Size = new System.Drawing.Size(314, 26);
            this.autodeskRevitUISelectionToolStripMenuItem.Text = "Autodesk.Revit.UI.Selection";
            // 
            // autodeskRevitCreationToolStripMenuItem
            // 
            this.autodeskRevitCreationToolStripMenuItem.Checked = true;
            this.autodeskRevitCreationToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.autodeskRevitCreationToolStripMenuItem.Enabled = false;
            this.autodeskRevitCreationToolStripMenuItem.Name = "autodeskRevitCreationToolStripMenuItem";
            this.autodeskRevitCreationToolStripMenuItem.Size = new System.Drawing.Size(314, 26);
            this.autodeskRevitCreationToolStripMenuItem.Text = "Autodesk.Revit.Creation";
            // 
            // viewToolStripMenuItem
            // 
            this.viewToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.logOutputToolStripMenuItem,
            this.toolStripMenuItem6,
            this.clearLogOutputToolStripMenuItem,
            this.viewLogToolStripMenuItem,
            this.toolStripMenuItem7,
            this.configurationToolStripMenuItem});
            this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
            this.viewToolStripMenuItem.Size = new System.Drawing.Size(56, 24);
            this.viewToolStripMenuItem.Text = "&View";
            // 
            // logOutputToolStripMenuItem
            // 
            this.logOutputToolStripMenuItem.Checked = true;
            this.logOutputToolStripMenuItem.CheckOnClick = true;
            this.logOutputToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.logOutputToolStripMenuItem.Name = "logOutputToolStripMenuItem";
            this.logOutputToolStripMenuItem.Size = new System.Drawing.Size(203, 26);
            this.logOutputToolStripMenuItem.Text = "Log output";
            this.logOutputToolStripMenuItem.Click += new System.EventHandler(this.logOutputToolStripMenuItem_Click);
            // 
            // toolStripMenuItem6
            // 
            this.toolStripMenuItem6.Name = "toolStripMenuItem6";
            this.toolStripMenuItem6.Size = new System.Drawing.Size(200, 6);
            // 
            // clearLogOutputToolStripMenuItem
            // 
            this.clearLogOutputToolStripMenuItem.Name = "clearLogOutputToolStripMenuItem";
            this.clearLogOutputToolStripMenuItem.Size = new System.Drawing.Size(203, 26);
            this.clearLogOutputToolStripMenuItem.Text = "&Clear log output";
            this.clearLogOutputToolStripMenuItem.Click += new System.EventHandler(this.clearLogOutputToolStripMenuItem_Click);
            // 
            // viewLogToolStripMenuItem
            // 
            this.viewLogToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.zToolStripMenuItem,
            this.currentToolStripMenuItem});
            this.viewLogToolStripMenuItem.Name = "viewLogToolStripMenuItem";
            this.viewLogToolStripMenuItem.Size = new System.Drawing.Size(203, 26);
            this.viewLogToolStripMenuItem.Text = "&View log";
            // 
            // zToolStripMenuItem
            // 
            this.zToolStripMenuItem.Name = "zToolStripMenuItem";
            this.zToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.D1)));
            this.zToolStripMenuItem.Size = new System.Drawing.Size(244, 26);
            this.zToolStripMenuItem.Text = "&All";
            this.zToolStripMenuItem.Click += new System.EventHandler(this.zToolStripMenuItem_Click);
            // 
            // currentToolStripMenuItem
            // 
            this.currentToolStripMenuItem.Name = "currentToolStripMenuItem";
            this.currentToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.D2)));
            this.currentToolStripMenuItem.Size = new System.Drawing.Size(244, 26);
            this.currentToolStripMenuItem.Text = "&Current recipe";
            this.currentToolStripMenuItem.Click += new System.EventHandler(this.currentToolStripMenuItem_Click);
            // 
            // toolStripMenuItem7
            // 
            this.toolStripMenuItem7.Name = "toolStripMenuItem7";
            this.toolStripMenuItem7.Size = new System.Drawing.Size(200, 6);
            // 
            // configurationToolStripMenuItem
            // 
            this.configurationToolStripMenuItem.Name = "configurationToolStripMenuItem";
            this.configurationToolStripMenuItem.Size = new System.Drawing.Size(203, 26);
            this.configurationToolStripMenuItem.Text = "&Configuration";
            this.configurationToolStripMenuItem.Click += new System.EventHandler(this.configurationToolStripMenuItem_Click);
            // 
            // TypeForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(805, 522);
            this.Controls.Add(this.MainContainer);
            this.Controls.Add(this.topMenu);
            this.MainMenuStrip = this.topMenu;
            this.Name = "TypeForm";
            this.ShowIcon = false;
            this.Text = "Luavit Editor";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.TypeForm_FormClosing);
            this.Load += new System.EventHandler(this.TypeForm_Load);
            this.Resize += new System.EventHandler(this.TypeForm_Resize);
            this.MainContainer.Panel1.ResumeLayout(false);
            this.MainContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.MainContainer)).EndInit();
            this.MainContainer.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.textLua)).EndInit();
            this.topMenu.ResumeLayout(false);
            this.topMenu.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.SplitContainer MainContainer;
        private System.Windows.Forms.MenuStrip topMenu;
        private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem runToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem runScriptToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem debugToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem newScriptToolStripMenuItem;
        private System.Windows.Forms.TabControl mainTab;
        private FastColoredTextBoxNS.FastColoredTextBox textLua;
        private System.Windows.Forms.ToolStripMenuItem saveScriptToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem openScriptToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem closeTabToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem3;
        private System.Windows.Forms.ToolStripMenuItem saveAsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem viewToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem clearLogOutputToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem5;
        private System.Windows.Forms.ToolStripMenuItem saveAsSnippetToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem importToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem autodeskRevitDBToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem autodeskRevitUIToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem autodeskRevitDBArchitectureToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem autodeskRevitUISelectionToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem autodeskRevitCreationToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem4;
        private System.Windows.Forms.ToolStripMenuItem fulldescrOutputToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem logOutputToolStripMenuItem;
        private System.Windows.Forms.ListView listLog;
        private System.Windows.Forms.ColumnHeader logTime;
        private System.Windows.Forms.ColumnHeader logProvider;
        private System.Windows.Forms.ColumnHeader logContent;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem6;
        private System.Windows.Forms.ToolStripMenuItem viewLogToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem zToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem currentToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem kitchenMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem7;
        private System.Windows.Forms.ToolStripMenuItem configurationToolStripMenuItem;
        //private FastColoredTextBoxNS.FastColoredTextBox textLua;
    }
}