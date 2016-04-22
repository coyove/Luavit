using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Analysis;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.DB.Electrical;
using Autodesk.Revit.DB.ExtensibleStorage;
using Autodesk.Revit.DB.ExternalService;
using Autodesk.Revit.DB.Fabrication;
using Autodesk.Revit.DB.IFC;
using Autodesk.Revit.DB.Lighting;
using Autodesk.Revit.DB.Macros;
using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.DB.Plumbing;
using Autodesk.Revit.DB.PointClouds;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI.Mechanical;
using Autodesk.Revit.UI.Plumbing;
using Autodesk.Revit.Creation;
using Autodesk.Revit.UI;
using MoonSharp.Interpreter;
using System.IO;
using FastColoredTextBoxNS;
using MoonSharp.RemoteDebugger;
using System.Diagnostics;
using System.Text.RegularExpressions;
using MoonSharp.Interpreter.Loaders;
using System.Reflection;

namespace Luavit
{
    public partial class TypeForm : System.Windows.Forms.Form
    {
        public static IEnumerable<Type> GlobalTypesList;
        public static IEnumerable<Type> PrimaryTypesList;
        public UIApplication UIApp;
        public string curScriptName;
        public static string mainDirectory;
        public static string kitchenDirectory;

        private bool viewCurrentLogOnly = false;
        public List<LuavitLog> logs = new List<LuavitLog>();
        public List<LuavitLog> currentLogs;

        private RemoteDebuggerService remoteDebugger;
        private TextStyle keywordStyle = new TextStyle(Brushes.Brown, null, FontStyle.Bold);
        private Dictionary<string, Type> extTypesList = new Dictionary<string, Type>();

        public List<string> ignoreList = new List<string>()
        {
            "PreviewHwndHost",
            "PreviewControl",
        };

        public static List<string> OmittedMethods = new List<string>();
        public static List<string> OmittedProperties = new List<string>();
        public static string AutoPrepend = "";
        public static string AutoAppend = "";

        public TypeForm()
        {
            InitializeComponent();
        }

        private IEnumerable<Type> _EnumTypes(Type ty)
        {
            return from t in ty.Assembly.GetTypes()
                   where t.IsClass && t.Namespace == ty.Namespace
                   select t;
        }

        private void AddAutoComplete(FastColoredTextBox tb)
        {
            var popupMenu = new AutocompleteMenu(tb);
            popupMenu.SearchPattern = @"[\w\.]";

            var items = new List<AutocompleteItem>();

            foreach (var item in ((
                from m in typeof(Helper).GetMethods()
                where !OmittedMethods.Contains(m.Name)
                select "helper." + m.Name).Concat(
                from m in typeof(Maker).GetMethods()
                where !OmittedMethods.Contains(m.Name)
                select "make." + m.Name).Concat(
                from m in typeof(Utils).GetMethods()
                where !OmittedMethods.Contains(m.Name)
                select "utils." + m.Name)).Distinct().ToArray())

                items.Add(new MethodAutocompleteItem2(item));

            popupMenu.Items.SetAutocompleteItems(items);
        }

        private void TypeForm_Load(object sender, EventArgs e)
        {
            #region Paths
            TypeForm.mainDirectory = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal) + "\\Luavit";
            TypeForm.kitchenDirectory = TypeForm.mainDirectory + "\\Kitchen";

            System.IO.Directory.CreateDirectory(TypeForm.mainDirectory);
            System.IO.Directory.CreateDirectory(TypeForm.kitchenDirectory);

            Log("Main directory: " + TypeForm.mainDirectory);
            Log("Kitchen directory: " + TypeForm.kitchenDirectory);
            #endregion

            #region Register types
            PrimaryTypesList = 
                _EnumTypes(typeof(Autodesk.Revit.DB.Document)).Concat(
                _EnumTypes(typeof(Autodesk.Revit.UI.UIDocument))).Concat(
                _EnumTypes(typeof(Autodesk.Revit.UI.Selection.Selection))).Concat(
                _EnumTypes(typeof(Autodesk.Revit.DB.Architecture.Room))).Concat(
                _EnumTypes(typeof(Autodesk.Revit.Creation.Document)));

            foreach (var t in PrimaryTypesList)
            {
                if (!ignoreList.Contains(t.Name))
                {
                    UserData.RegisterType(t);
                    Log("Revit class '" + t.Name + "' registered.");
                }
            }

            extTypesList.Add("Autodesk.Revit.DB.Analysis", typeof(Autodesk.Revit.DB.Analysis.AnalysisDisplayColoredSurfaceSettings));
            extTypesList.Add("Autodesk.Revit.DB.Electrical", typeof(Autodesk.Revit.DB.Electrical.CableTray));
            extTypesList.Add("Autodesk.Revit.DB.ExtensibleStorage", typeof(Autodesk.Revit.DB.ExtensibleStorage.DataStorage));
            extTypesList.Add("Autodesk.Revit.DB.ExternalService", typeof(Autodesk.Revit.DB.ExternalService.ExternalService));
            extTypesList.Add("Autodesk.Revit.DB.Fabrication", typeof(Autodesk.Revit.DB.Fabrication.FabricationUtils));
            extTypesList.Add("Autodesk.Revit.DB.IFC", typeof(Autodesk.Revit.DB.IFC.IFCImportOptions));
            extTypesList.Add("Autodesk.Revit.DB.Lighting", typeof(Autodesk.Revit.DB.Lighting.BasicLossFactor));
            extTypesList.Add("Autodesk.Revit.DB.Macros", typeof(Autodesk.Revit.DB.Macros.DocumentEntryPoint));
            extTypesList.Add("Autodesk.Revit.DB.Mechanical", typeof(Autodesk.Revit.DB.Mechanical.Duct));
            extTypesList.Add("Autodesk.Revit.DB.Plumbing", typeof(Autodesk.Revit.DB.Plumbing.FluidTemperatureSetIterator));
            extTypesList.Add("Autodesk.Revit.DB.PointClouds", typeof(Autodesk.Revit.DB.PointClouds.PointCloudColorSettings));
            extTypesList.Add("Autodesk.Revit.DB.Structure", typeof(Autodesk.Revit.DB.Structure.AnalyticalConsistencyChecking));
            extTypesList.Add("Autodesk.Revit.UI.Mechanical", typeof(Autodesk.Revit.UI.Mechanical.DuctFittingAndAccessoryPressureDropUIData));
            extTypesList.Add("Autodesk.Revit.UI.Plumbing", typeof(Autodesk.Revit.UI.Plumbing.PipeFittingAndAccessoryPressureDropUIData));

            UserData.RegisterType<Helper>();
            UserData.RegisterType<Maker>();
            UserData.RegisterType<Utils>();
            UserData.RegisterType<HTTPClient>();
            UserData.RegisterType<System.Windows.Forms.Form>();
            UserData.RegisterType<TypeForm>();
            #endregion

            #region Editor
            textLua.Language = Language.Lua;
            textLua.AutoIndent = true;
            textLua.BracketsHighlightStrategy = BracketsHighlightStrategy.Strategy1;
            #endregion

            #region Menus
            var logMenu = new ContextMenu();
            logMenu.MenuItems.Add("Select all").Click += selectLogText_Click;
            logMenu.MenuItems.Add("-");
            logMenu.MenuItems.Add("Copy text").Click += copyLogText_Click;
            logMenu.MenuItems.Add("Pop to new tab").Click += popLogText_Click;
            listLog.ContextMenu = logMenu;

            foreach (var kv in extTypesList) {
                var typeMenu = new ToolStripMenuItem();
                typeMenu.Text = kv.Key;
                typeMenu.CheckOnClick = true;
                typeMenu.Click += importMenuItem_Click;
                importToolStripMenuItem.DropDownItems.Add(typeMenu);
            }

            LoadKitchenMenu();
            #endregion

            if (File.Exists(mainDirectory + "\\config.lua"))
                Config.LoadConfig(mainDirectory + "\\config.lua", this);

            NewScript();
            AutoResizeLogColumn();
        }

        private void LoadKitchenMenu()
        {
            kitchenMenuItem.DropDownItems.Clear();

            var menu = new ToolStripMenuItem();
            menu.Text = "Reload";
            menu.Click += (object sender, EventArgs e) => LoadKitchenMenu();
            kitchenMenuItem.DropDownItems.Add(menu);
            kitchenMenuItem.DropDownItems.Add(new ToolStripSeparator());

            ProcessDirectory(kitchenDirectory, kitchenMenuItem);
        }

        private void ProcessDirectory(string targetDirectory, ToolStripMenuItem menu)
        {
            string[] fileEntries = Directory.GetFiles(targetDirectory);
            foreach (string fileName in fileEntries)
            {
                var recipeMenu = new ToolStripMenuItem();
                recipeMenu.Text = Path.GetFileName(fileName);
                recipeMenu.Tag = fileName;
                recipeMenu.Click += kitchenRecipe_Click;
                menu.DropDownItems.Add(recipeMenu);
            }

            string[] subdirectoryEntries = Directory.GetDirectories(targetDirectory);
            foreach (string subdirectory in subdirectoryEntries)
            {
                var recipeMenu = new ToolStripMenuItem();
                recipeMenu.Text = subdirectory.Split('\\').Last();
                recipeMenu.Tag = subdirectory;
                ProcessDirectory(subdirectory, recipeMenu);
                menu.DropDownItems.Add(recipeMenu);
            }
        }

        private void kitchenRecipe_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem menu = sender as ToolStripMenuItem;
            NewScript((string)menu.Tag);
        }

        private void selectLogText_Click(object sender, EventArgs e)
        {
            listLog.BeginUpdate();
            try
            {
                for (int i = 0; i < listLog.VirtualListSize; i++)
                    listLog.SelectedIndices.Add(i);
            }
            finally
            {
                listLog.EndUpdate();
            }
        }

        private void copyLogText_Click(object sender, EventArgs e)
        {
            var text = "";
            for (var i = 0; i < listLog.SelectedIndices.Count; i++)
            {
                int idx = listLog.SelectedIndices[i];
                if (viewCurrentLogOnly)
                    text += currentLogs[idx].content + Environment.NewLine;
                else
                    text += logs[idx].content + Environment.NewLine;
            }
            
            Clipboard.SetText(text);
        }

        private void popLogText_Click(object sender, EventArgs e)
        {
            var text = "";
            for (var i = 0; i < listLog.SelectedIndices.Count; i++)
            {
                int idx = listLog.SelectedIndices[i];
                if (viewCurrentLogOnly)
                    text += currentLogs[idx].content + Environment.NewLine;
                else
                    text += logs[idx].content + Environment.NewLine;
            }
            NewScript("", text);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            RunScript(false);
        }

        public void RunScript(bool debug)
        {
            CheckImportMenu();
            Log("-");

            Script script = new Script();
            IEnumerable<Type> extTypes = null;

            foreach (var _item in importToolStripMenuItem.DropDownItems)
            {
                var item = _item as ToolStripMenuItem;
                if (item.Checked && item.Enabled)
                {
                    if (extTypes == null)
                        extTypes = (_EnumTypes(extTypesList[item.Text]));
                    else
                        extTypes = extTypes.Concat(_EnumTypes(extTypesList[item.Text]));
                }
            }

            if (extTypes != null)
            {
                foreach (var t in extTypes)
                {
                    if (!ignoreList.Contains(t.Name))
                    {
                        UserData.RegisterType(t);
                        Log("Ext Revit class '" + t.Name + "' registered.");
                    }
                }

                GlobalTypesList = PrimaryTypesList.Concat(extTypes).Distinct();
            } else
            {
                GlobalTypesList = PrimaryTypesList;
            }

            foreach (var t in GlobalTypesList.ToList())
            {
                if (!ignoreList.Contains(t.Name)) script.Globals.Set(t.Name, UserData.Create(t));
            }

            var helper = new Helper();
            helper.UIApp = UIApp;
            helper.MoonScript = script;

            var maker = new Maker();
            maker.TypeList = GlobalTypesList;

            script.Globals.Set("helper", UserData.Create(helper));
            script.Globals.Set("make", UserData.Create(maker));
            script.Globals.Set("utils", UserData.Create(new Utils()));

            script.Globals["log"] = (Func<object, int>)(s => Log(s, curScriptName));
            script.Globals["descr"] = (Func<object, int>)(o => Log(new Helper().Describe(o, fulldescrOutputToolStripMenuItem.Checked), curScriptName));
            script.Globals["uiapp"] = UIApp;
            script.Globals["uidoc"] = UIApp.ActiveUIDocument;
            script.Globals["doc"] = UIApp.ActiveUIDocument.Document;

            ((ScriptLoaderBase)script.Options.ScriptLoader).ModulePaths = new string[] { TypeForm.mainDirectory + "/?.lua" };

            var ctls = mainTab.SelectedTab.Controls;
            foreach (var c in ctls)
            {
                if (c is FastColoredTextBox)
                {
                    curScriptName = mainTab.SelectedTab.Text.Replace("*", "");
                    if (debug)
                    {
                        if (remoteDebugger == null)
                        {
                            remoteDebugger = new RemoteDebuggerService();
                            remoteDebugger.Attach(script, curScriptName, false);
                        }
                        Process.Start(remoteDebugger.HttpUrlStringLocalHost);
                    }

                    script.DoString(AutoPrepend + (c as FastColoredTextBox).Text + AutoAppend);
                    AutoResizeLogColumn();
                    break;
                }
            }            
        }

        public int Log(object text, string provider = "Luavit", string padding = "")
        {
            if (text is double || text is int || text is byte) text = text.ToString();
            
            if (text is string)
            {
                string t = text as string;

                foreach (var tmp in t.Split(new string[] { Environment.NewLine }, StringSplitOptions.None))
                    logs.Add(new LuavitLog(provider, padding + tmp));
            }
            else if (text.GetType().GetProperty("IntegerValue")?.GetValue(text) != null) 
            {
                var ty = text.GetType();
                logs.Add(new LuavitLog(provider, 
                    padding + ((int)ty.GetProperty("IntegerValue").GetValue(text)).ToString() + " (" + ty.Name + ")"));
            }
            else if (text is MoonSharp.Interpreter.Table)
            {
                var table = text as MoonSharp.Interpreter.Table;
                for(int i = 0; i < table.Keys.Count(); i++)
                {
                    var k = table.Keys.ElementAt(i);
                    var v = table.Values.ElementAt(i);
                    switch (table.Values.ElementAt(i).Type)
                    {
                        case DataType.Boolean:
                            // lstLog.Items.Add(padding + k.String + "=" + v.Boolean.ToString());
                            logs.Add(new LuavitLog(provider, padding + k.String + " = " + v.Boolean.ToString()));
                            break;
                        case DataType.ClrFunction:
                        case DataType.Function:
                            break;
                        case DataType.Nil:
                            logs.Add(new LuavitLog(provider, padding + k.String + " = nil"));
                            break;
                        case DataType.Number:
                            // lstLog.Items.Add(padding + k.String + "=" + v.Number.ToString());
                            logs.Add(new LuavitLog(provider, padding + k.String + " = " + v.Number.ToString()));
                            break;
                        case DataType.String:
                            // lstLog.Items.Add(padding + k.String + "=\"" + v.String + "\"");
                            logs.Add(new LuavitLog(provider, padding + k.String + " = \"" + v.String + "\""));
                            break;
                        case DataType.UserData:
                            // lstLog.Items.Add(padding + k.String + "=[UserData]");
                            logs.Add(new LuavitLog(provider, padding + k.String + " = [UserData]"));
                            break;
                        case DataType.Table:
                            // lstLog.Items.Add(padding + k.String + "={");
                            logs.Add(new LuavitLog(provider, padding + k.String + " = {"));
                            Log(v.Table, provider, padding + "    ");
                            // lstLog.Items.Add(padding + "}");
                            logs.Add(new LuavitLog(provider, padding + "}"));
                            break;
                        case DataType.Tuple:
                            // lstLog.Items.Add(padding + k.String + "=(" +
                            logs.Add(new LuavitLog(provider, padding + k.String + " = (" +
                                String.Join(", ", (from tuple in v.Tuple select tuple.ToPrintString()).ToArray()) + ")"));
                            break;
                        default:
                            // lstLog.Items.Add(padding + k.String + "=[Not Implemented]");
                            logs.Add(new LuavitLog(provider, padding + k.String + " = [Not Implemented]"));
                            break;
                    }
                }
                // lstLog.SelectedIndex = lstLog.Items.Count - 1;
            }

            viewCurrentLogOnly = false;
            listLog.VirtualListSize = logs.Count;
            listLog.Items[listLog.Items.Count - 1].EnsureVisible();
            return listLog.Items.Count;
        }

        private void debugToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RunScript(true);
        }

        private void textLua_TextChanged(object sender, TextChangedEventArgs e)
        {
            e.ChangedRange.ClearStyle(keywordStyle);
            e.ChangedRange.SetStyle(keywordStyle, @"\blog\b", RegexOptions.IgnoreCase);
            e.ChangedRange.SetStyle(keywordStyle, @"\bdescr\b", RegexOptions.IgnoreCase);
            e.ChangedRange.SetStyle(keywordStyle, @"\bmake\b", RegexOptions.IgnoreCase);
            e.ChangedRange.SetStyle(keywordStyle, @"\bhelper\b", RegexOptions.IgnoreCase);
            e.ChangedRange.SetStyle(keywordStyle, @"\butils\b", RegexOptions.IgnoreCase);

            var p = ((FastColoredTextBox)sender).Parent;
            if (!p.Text.Contains("*")) p.Text += "*";
        }

        private void rightLogMenu_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void editToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void TypeForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            for(int i = mainTab.TabPages.Count - 1; i >= 0; i--)
            {
                mainTab.SelectedIndex = i;
                if (!CloseCurrentTab())
                {
                    e.Cancel = true;
                    return;
                }
            }
        }

        private void newScriptToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NewScript();
        }

        public void NewScript(string fn = "", string content = "")
        {
            var txt = new FastColoredTextBox();
            txt.Language = Language.Lua;
            txt.AutoIndent = true;
            txt.BracketsHighlightStrategy = BracketsHighlightStrategy.Strategy1;
            txt.Font = textLua.Font;
            if (content != "") txt.Text = content;
            txt.TextChanged += textLua_TextChanged;
            txt.Dock = DockStyle.Fill;
            AddAutoComplete(txt);

            var filename = new Label();
            filename.Text = "newly";
            filename.Visible = false;

            var tab = new TabPage();
            tab.Controls.Add(txt);
            tab.Controls.Add(filename);

            if (fn != "")
            {
                txt.Text = System.IO.File.ReadAllText(fn);
                filename.Text = fn;
                tab.Text = Path.GetFileName(fn);
            }
            else {
                tab.Text = "Untitled";
            }

            mainTab.Controls.Add(tab);
            mainTab.SelectedIndex = mainTab.TabPages.Count - 1;

            CheckImportMenu();
        }

        private void saveScriptToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveCurrent();
        }

        public void SaveCurrent()
        {
            string filename = "";
            string content = "";

            foreach (var c in mainTab.SelectedTab.Controls)
            {
                if (c is FastColoredTextBox)
                {
                    content = ((c as FastColoredTextBox).Text);
                }

                if (c is Label)
                {
                    if ((c as Label).Text == "newly")
                    {
                        SaveFileDialog sfd = new SaveFileDialog();
                        sfd.Filter = "Lua script|*.lua";
                        sfd.Title = "Save lua script";
                        sfd.ShowDialog();

                        if (sfd.FileName != "")
                        {
                            filename = sfd.FileName;
                            (c as Label).Text = filename;
                        }
                    } else
                    {
                        filename = (c as Label).Text;
                    }
                }
            }

            if (filename != "")
            {
                mainTab.SelectedTab.Text = Path.GetFileName(filename);
                System.IO.File.WriteAllText(filename, content);
            }
        }

        private void openScriptToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog sfd = new OpenFileDialog();
            sfd.Filter = "Lua script|*.lua";
            sfd.Title = "Open lua script";
            sfd.ShowDialog();

            if (sfd.FileName != "")
            {
                NewScript(sfd.FileName);
            }
        }

        private void closeTabToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CloseCurrentTab(true);
        }

        public bool CloseCurrentTab(bool autoNew = false)
        {
            var tab = mainTab.SelectedTab;
            if (tab.Text.Contains("*"))
            {
                var result = MessageBox.Show(tab.Text + Environment.NewLine +
                    "Content not saved, save now?", "Luavit", MessageBoxButtons.YesNoCancel);
                if (result == DialogResult.Yes)
                {
                    SaveCurrent();
                    mainTab.TabPages.Remove(tab);
                } else if (result == DialogResult.No)
                {
                    mainTab.TabPages.Remove(tab);
                } else
                {
                    return false;
                }
            } else
            {
                mainTab.TabPages.Remove(tab);
            }
            if (autoNew && mainTab.TabPages.Count == 0) NewScript();
            return true;
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Lua script|*.lua|Txt file|*.txt";
            sfd.Title = "Save lua script as";
            sfd.ShowDialog();

            if (sfd.FileName != "")
            {
                foreach (var c in mainTab.SelectedTab.Controls)
                {
                    if (c is FastColoredTextBox)
                    {
                        System.IO.File.WriteAllText(sfd.FileName, (c as FastColoredTextBox).Text);
                    }

                    if (c is Label)
                    {
                        (c as Label).Text = sfd.FileName;
                    }
                }
                mainTab.SelectedTab.Text = Path.GetFileName(sfd.FileName);
            }
        }

        private void clearLogOutputToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ClearLog();
        }

        public void ClearLog()
        {
            logs.Clear();
            viewCurrentLogOnly = false;
            listLog.VirtualListSize = 0;
        }

        private void logOutputToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!logOutputToolStripMenuItem.Checked)
            {
                MainContainer.Panel2Collapsed = true;
                MainContainer.Panel2.Hide();
            }
            else {
                MainContainer.Panel2.Show();
                MainContainer.Panel2Collapsed = false;
            }
        }

        private void darkThemeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
        }

        private void saveAsSnippetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string filename = "";
            string content = "";
            Label lp = null;

            foreach (var c in mainTab.SelectedTab.Controls)
            {
                if (c is FastColoredTextBox)
                {
                    var ctl = (c as FastColoredTextBox);
                    Regex t = new Regex(@"\-\-\srecipe\((\S+?)\)", RegexOptions.Singleline);
                    var allMatches = t.Matches(ctl.Text);
                    content = ctl.Text;

                    if (allMatches.Count > 0)
                    {
                        filename = (allMatches[0].Groups[1].Value) + ".lua";
                    }
                } 
                else if (c is Label)
                {
                    lp = c as Label;
                }
            }

            if (filename != "" && lp != null)
            {
                mainTab.SelectedTab.Text = Path.GetFileName(filename);
                lp.Text = kitchenDirectory + "\\" + filename;
                var tmp = filename.Split('\\');
                if (tmp.Count() > 1)
                {
                    for (int i = 0; i < tmp.Count() - 1; i++)
                    {
                        System.IO.Directory.CreateDirectory(kitchenDirectory + "\\" + String.Join("\\", tmp.Take(i + 1)));
                    }
                }
                System.IO.File.WriteAllText(kitchenDirectory + "\\" + filename, content);
                Log("Saved to kitchen: " + filename);
            } else
            {
                Log("No recipe header found");
            }
        }

        private void importMenuItem_Click(object sender, EventArgs e)
        {
            foreach (var c in mainTab.SelectedTab.Controls)
            {
                if (c is FastColoredTextBox)
                {
                    var ctl = (c as FastColoredTextBox);
                    var text = ctl.Text;
                    var item = (sender as ToolStripMenuItem);
                    var find = item.Text.Replace(".", @"\.");

                    Regex t = new Regex(@"\-\-\simport\(" + find + @"\)", RegexOptions.Singleline);
                    var allMatches = t.Matches(text);

                    if (item.Checked)
                    {
                        if (allMatches.Count == 0)
                        {
                            ctl.Text = "-- import(" + item.Text + ")" + Environment.NewLine + ctl.Text;
                        }
                    }
                    else
                    {
                        if (allMatches.Count > 0)
                        {
                            ctl.Text = ctl.Text.Replace("-- import(" + item.Text + ")" + Environment.NewLine, "");
                        }
                    }
                }
            }
        }

        private void CheckImportMenu()
        {
            foreach (var c in mainTab.SelectedTab.Controls)
            {
                if (c is FastColoredTextBox)
                {
                    var ctl = (c as FastColoredTextBox);
                    var text = ctl.Text;

                    foreach (var _item in importToolStripMenuItem.DropDownItems)
                    {
                        var item = _item as ToolStripMenuItem;
                        var find = item.Text.Replace(".", @"\.");

                        Regex t = new Regex(@"\-\-\simport\(" + find + @"\)", RegexOptions.Singleline);
                        var allMatches = t.Matches(text);

                        if (item.Enabled)
                        {
                            item.Checked = (allMatches.Count > 0);   
                        }
                    }
                }
            }
        }

        private void mainTab_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (mainTab.SelectedTab != null )
            {
                CheckImportMenu();
                listLog.Refresh();
            }
        }

        private void TypeForm_Resize(object sender, EventArgs e)
        {
            AutoResizeLogColumn();
        }

        private void AutoResizeLogColumn()
        {
            listLog.AutoResizeColumn(0, ColumnHeaderAutoResizeStyle.ColumnContent);
            listLog.AutoResizeColumn(1, ColumnHeaderAutoResizeStyle.ColumnContent);
            listLog.Columns[listLog.Columns.Count - 1].Width = -2;
        }

        private void listLog_RetrieveVirtualItem(object sender, RetrieveVirtualItemEventArgs e)
        {
            ListViewItem lvi = new ListViewItem();

            if (viewCurrentLogOnly)
            {
                if (currentLogs != null)
                {
                    lvi.Text = currentLogs[e.ItemIndex].time;
                    lvi.SubItems.Add(currentLogs[e.ItemIndex].provider).ForeColor = System.Drawing.Color.Gray;
                    lvi.SubItems.Add(currentLogs[e.ItemIndex].content);
                }
            } else
            {
                lvi.Text = logs[e.ItemIndex].time;
                var p = logs[e.ItemIndex].provider;

                if (p == "Luavit")
                {
                    lvi.ForeColor = System.Drawing.Color.LightGray;
                    lvi.BackColor = System.Drawing.Color.Teal;
                } else if (p == mainTab.SelectedTab.Text.Replace("*", ""))
                {
                    lvi.ForeColor = System.Drawing.Color.Blue;
                } else
                {
                    lvi.ForeColor = System.Drawing.Color.Gray;
                }
                lvi.SubItems.Add(p);
                lvi.SubItems.Add(logs[e.ItemIndex].content);
            }

            if (lvi.SubItems[2].Text == "-")
            {
                lvi.BackColor = System.Drawing.Color.Black;
                lvi.ForeColor = System.Drawing.Color.White;
            }

            e.Item = lvi;
        }

        private void viewAllToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void zToolStripMenuItem_Click(object sender, EventArgs e)
        {
            viewCurrentLogOnly = false;

            listLog.VirtualListSize = logs.Count;
            listLog.Refresh();
        }

        private void currentToolStripMenuItem_Click(object sender, EventArgs e)
        {
            viewCurrentLogOnly = true;
            string cur = mainTab.SelectedTab.Text.Replace("*", "");
            currentLogs = (from l in logs where l.provider == cur select l).ToList();

            listLog.VirtualListSize = currentLogs.Count;
            listLog.Refresh();
        }

        private void listLog_SelectedIndexChanged(object sender, EventArgs e)
        {
            
        }

        private void mainTab_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Middle)
            {
                CloseCurrentTab(true);
            }
        }

        private void configurationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NewScript(mainDirectory + "\\config.lua");
        }
    }

    public class LuavitLog
    {
        public string time;
        public string provider;
        public string content;

        public LuavitLog(string p, string c)
        {
            time = DateTime.Now.ToString("HH:mm:ss");
            provider = p;
            content = c;
        }
    }

    public class MethodAutocompleteItem2 : MethodAutocompleteItem
    {
        string firstPart;
        string lastPart;

        public MethodAutocompleteItem2(string text)
            : base(text)
        {
            var i = text.LastIndexOf('.');
            if (i < 0)
                firstPart = text;
            else
            {
                firstPart = text.Substring(0, i);
                lastPart = text.Substring(i + 1);
            }
        }

        public override CompareResult Compare(string fragmentText)
        {
            int i = fragmentText.LastIndexOf('.');

            if (i < 0)
            {
                if (firstPart.StartsWith(fragmentText) && string.IsNullOrEmpty(lastPart))
                    return CompareResult.VisibleAndSelected;
                //if (firstPart.ToLower().Contains(fragmentText.ToLower()))
                //  return CompareResult.Visible;
            }
            else
            {
                var fragmentFirstPart = fragmentText.Substring(0, i);
                var fragmentLastPart = fragmentText.Substring(i + 1);


                if (firstPart != fragmentFirstPart)
                    return CompareResult.Hidden;

                if (lastPart != null && lastPart.StartsWith(fragmentLastPart))
                    return CompareResult.VisibleAndSelected;

                if (lastPart != null && lastPart.ToLower().Contains(fragmentLastPart.ToLower()))
                    return CompareResult.Visible;

            }

            return CompareResult.Hidden;
        }

        public override string GetTextForReplace()
        {
            if (lastPart == null)
                return firstPart;

            return firstPart + "." + lastPart;
        }

        public override string ToString()
        {
            if (lastPart == null)
                return firstPart;

            return lastPart;
        }
    }
}
