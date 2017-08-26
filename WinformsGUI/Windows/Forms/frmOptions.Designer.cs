using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace bSearch.Windows.Forms
{
   public partial class frmOptions
   {
      /// <summary>
      /// Required designer variable.
      /// </summary>
      private System.ComponentModel.Container components = null;

      /// <summary>
      /// Clean up any resources being used.
      /// </summary>
      protected override void Dispose(bool disposing)
      {
         if (disposing)
         {
            if (components != null)
            {
               components.Dispose();
            }
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
            this.tbcOptions = new System.Windows.Forms.TabControl();
            this.tabGeneral = new System.Windows.Forms.TabPage();
            this.chkLabelColor = new System.Windows.Forms.CheckBox();
            this.chkSaveExclusionsPosition = new System.Windows.Forms.CheckBox();
            this.chkSaveMessagesPosition = new System.Windows.Forms.CheckBox();
            this.chkSaveSearchOptions = new System.Windows.Forms.CheckBox();
            this.chkShowExclusionErrorMessage = new System.Windows.Forms.CheckBox();
            this.ShortcutGroup = new System.Windows.Forms.GroupBox();
            this.chkStartMenuShortcut = new System.Windows.Forms.CheckBox();
            this.chkDesktopShortcut = new System.Windows.Forms.CheckBox();
            this.chkRightClickOption = new System.Windows.Forms.CheckBox();
            this.LanguageGroup = new System.Windows.Forms.GroupBox();
            this.cboLanguage = new System.Windows.Forms.ComboBox();
            this.cboPathMRUCount = new System.Windows.Forms.ComboBox();
            this.lblStoredPaths = new System.Windows.Forms.Label();
            this.tabFileEncoding = new System.Windows.Forms.TabPage();
            this.btnCacheClear = new System.Windows.Forms.Button();
            this.chkUseEncodingCache = new System.Windows.Forms.CheckBox();
            this.cboPerformance = new System.Windows.Forms.ComboBox();
            this.lblPerformance = new System.Windows.Forms.Label();
            this.chkDetectFileEncoding = new System.Windows.Forms.CheckBox();
            this.btnFileEncodingDelete = new System.Windows.Forms.Button();
            this.btnFileEncodingEdit = new System.Windows.Forms.Button();
            this.btnFileEncodingAdd = new System.Windows.Forms.Button();
            this.lstFiles = new System.Windows.Forms.ListView();
            this.clhEnabled = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.clhFile = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.clhEncoding = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.tabTextEditors = new System.Windows.Forms.TabPage();
            this.btnEdit = new System.Windows.Forms.Button();
            this.btnRemove = new System.Windows.Forms.Button();
            this.btnAdd = new System.Windows.Forms.Button();
            this.TextEditorsList = new System.Windows.Forms.ListView();
            this.ColumnType = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ColumnEditor = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ColumnArguments = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ColumnTabSize = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.tabResults = new System.Windows.Forms.TabPage();
            this.pnlResultsPreview = new System.Windows.Forms.Panel();
            this.lblResultPreview = new System.Windows.Forms.Label();
            this.rtxtResultsPreview = new System.Windows.Forms.RichTextBox();
            this.grpFileList = new System.Windows.Forms.GroupBox();
            this.lblFileCurrentFont = new System.Windows.Forms.Label();
            this.btnFileFindFont = new System.Windows.Forms.Button();
            this.grpResultWindow = new System.Windows.Forms.GroupBox();
            this.btnResultsContextForeColor = new bSearch.Windows.Controls.ColorButton();
            this.lblResultsContextForeColor = new System.Windows.Forms.Label();
            this.lblCurrentFont = new System.Windows.Forms.Label();
            this.btnFindFont = new System.Windows.Forms.Button();
            this.btnResultsWindowBackColor = new bSearch.Windows.Controls.ColorButton();
            this.btnResultsWindowForeColor = new bSearch.Windows.Controls.ColorButton();
            this.lblResultsWindowBack = new System.Windows.Forms.Label();
            this.lblResultsWindowFore = new System.Windows.Forms.Label();
            this.grpResultMatch = new System.Windows.Forms.GroupBox();
            this.BackColorButton = new bSearch.Windows.Controls.ColorButton();
            this.ForeColorButton = new bSearch.Windows.Controls.ColorButton();
            this.BackColorLabel = new System.Windows.Forms.Label();
            this.ForeColorLabel = new System.Windows.Forms.Label();
            this.tabPlugins = new System.Windows.Forms.TabPage();
            this.btnDown = new System.Windows.Forms.Button();
            this.btnUp = new System.Windows.Forms.Button();
            this.PluginDetailsGroup = new System.Windows.Forms.GroupBox();
            this.lblPluginDescription = new System.Windows.Forms.Label();
            this.lblPluginAuthor = new System.Windows.Forms.Label();
            this.lblPluginVersion = new System.Windows.Forms.Label();
            this.lblPluginName = new System.Windows.Forms.Label();
            this.lblPluginDetailAuthor = new System.Windows.Forms.Label();
            this.lblPluginDetailVersion = new System.Windows.Forms.Label();
            this.lblPluginDetailName = new System.Windows.Forms.Label();
            this.PluginsList = new System.Windows.Forms.ListView();
            this.PluginsColumnEnabled = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.PluginsColumnName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.PluginsColumnExt = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.cboServer = new System.Windows.Forms.ComboBox();
            this.cboDataBase = new System.Windows.Forms.ComboBox();
            this.btnTest = new System.Windows.Forms.Button();
            this.lblDataBase = new System.Windows.Forms.Label();
            this.lblPassword = new System.Windows.Forms.Label();
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.lblLogin = new System.Windows.Forms.Label();
            this.txtLogin = new System.Windows.Forms.TextBox();
            this.chkIntegritySecurity = new System.Windows.Forms.CheckBox();
            this.lblServer = new System.Windows.Forms.Label();
            this.chkUseDataBase = new System.Windows.Forms.CheckBox();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.tbcOptions.SuspendLayout();
            this.tabGeneral.SuspendLayout();
            this.ShortcutGroup.SuspendLayout();
            this.LanguageGroup.SuspendLayout();
            this.tabFileEncoding.SuspendLayout();
            this.tabTextEditors.SuspendLayout();
            this.tabResults.SuspendLayout();
            this.pnlResultsPreview.SuspendLayout();
            this.grpFileList.SuspendLayout();
            this.grpResultWindow.SuspendLayout();
            this.grpResultMatch.SuspendLayout();
            this.tabPlugins.SuspendLayout();
            this.PluginDetailsGroup.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tbcOptions
            // 
            this.tbcOptions.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbcOptions.Controls.Add(this.tabGeneral);
            this.tbcOptions.Controls.Add(this.tabFileEncoding);
            this.tbcOptions.Controls.Add(this.tabTextEditors);
            this.tbcOptions.Controls.Add(this.tabResults);
            this.tbcOptions.Controls.Add(this.tabPlugins);
            this.tbcOptions.Controls.Add(this.tabPage1);
            this.tbcOptions.Location = new System.Drawing.Point(10, 10);
            this.tbcOptions.Margin = new System.Windows.Forms.Padding(4);
            this.tbcOptions.Name = "tbcOptions";
            this.tbcOptions.SelectedIndex = 0;
            this.tbcOptions.Size = new System.Drawing.Size(701, 488);
            this.tbcOptions.TabIndex = 0;
            this.tbcOptions.SelectedIndexChanged += new System.EventHandler(this.tbcOptions_SelectedIndexChanged);
            // 
            // tabGeneral
            // 
            this.tabGeneral.Controls.Add(this.chkLabelColor);
            this.tabGeneral.Controls.Add(this.chkSaveExclusionsPosition);
            this.tabGeneral.Controls.Add(this.chkSaveMessagesPosition);
            this.tabGeneral.Controls.Add(this.chkSaveSearchOptions);
            this.tabGeneral.Controls.Add(this.chkShowExclusionErrorMessage);
            this.tabGeneral.Controls.Add(this.ShortcutGroup);
            this.tabGeneral.Controls.Add(this.LanguageGroup);
            this.tabGeneral.Controls.Add(this.cboPathMRUCount);
            this.tabGeneral.Controls.Add(this.lblStoredPaths);
            this.tabGeneral.Location = new System.Drawing.Point(4, 27);
            this.tabGeneral.Margin = new System.Windows.Forms.Padding(4);
            this.tabGeneral.Name = "tabGeneral";
            this.tabGeneral.Size = new System.Drawing.Size(693, 457);
            this.tabGeneral.TabIndex = 0;
            this.tabGeneral.Text = "General";
            this.tabGeneral.UseVisualStyleBackColor = true;
            // 
            // chkLabelColor
            // 
            this.chkLabelColor.AutoSize = true;
            this.chkLabelColor.Location = new System.Drawing.Point(10, 375);
            this.chkLabelColor.Margin = new System.Windows.Forms.Padding(4);
            this.chkLabelColor.Name = "chkLabelColor";
            this.chkLabelColor.Size = new System.Drawing.Size(231, 22);
            this.chkLabelColor.TabIndex = 40;
            this.chkLabelColor.Text = "Use accent color on &headings.";
            this.chkLabelColor.UseVisualStyleBackColor = true;
            // 
            // chkSaveExclusionsPosition
            // 
            this.chkSaveExclusionsPosition.AutoSize = true;
            this.chkSaveExclusionsPosition.Location = new System.Drawing.Point(10, 345);
            this.chkSaveExclusionsPosition.Margin = new System.Windows.Forms.Padding(4);
            this.chkSaveExclusionsPosition.Name = "chkSaveExclusionsPosition";
            this.chkSaveExclusionsPosition.Size = new System.Drawing.Size(247, 22);
            this.chkSaveExclusionsPosition.TabIndex = 39;
            this.chkSaveExclusionsPosition.Text = "Save exclusions window position";
            this.chkSaveExclusionsPosition.UseVisualStyleBackColor = true;
            // 
            // chkSaveMessagesPosition
            // 
            this.chkSaveMessagesPosition.AutoSize = true;
            this.chkSaveMessagesPosition.Location = new System.Drawing.Point(10, 315);
            this.chkSaveMessagesPosition.Margin = new System.Windows.Forms.Padding(4);
            this.chkSaveMessagesPosition.Name = "chkSaveMessagesPosition";
            this.chkSaveMessagesPosition.Size = new System.Drawing.Size(246, 22);
            this.chkSaveMessagesPosition.TabIndex = 38;
            this.chkSaveMessagesPosition.Text = "Save messages window position";
            this.chkSaveMessagesPosition.UseVisualStyleBackColor = true;
            // 
            // chkSaveSearchOptions
            // 
            this.chkSaveSearchOptions.AutoSize = true;
            this.chkSaveSearchOptions.Location = new System.Drawing.Point(10, 285);
            this.chkSaveSearchOptions.Margin = new System.Windows.Forms.Padding(4);
            this.chkSaveSearchOptions.Name = "chkSaveSearchOptions";
            this.chkSaveSearchOptions.Size = new System.Drawing.Size(212, 22);
            this.chkSaveSearchOptions.TabIndex = 37;
            this.chkSaveSearchOptions.Text = "Save search options on exit";
            this.chkSaveSearchOptions.UseVisualStyleBackColor = true;
            // 
            // chkShowExclusionErrorMessage
            // 
            this.chkShowExclusionErrorMessage.CheckAlign = System.Drawing.ContentAlignment.TopLeft;
            this.chkShowExclusionErrorMessage.Location = new System.Drawing.Point(10, 405);
            this.chkShowExclusionErrorMessage.Margin = new System.Windows.Forms.Padding(4);
            this.chkShowExclusionErrorMessage.Name = "chkShowExclusionErrorMessage";
            this.chkShowExclusionErrorMessage.Size = new System.Drawing.Size(671, 45);
            this.chkShowExclusionErrorMessage.TabIndex = 36;
            this.chkShowExclusionErrorMessage.Text = "Show a &prompt when a search yields items being excluded or an error occurs";
            this.chkShowExclusionErrorMessage.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.chkShowExclusionErrorMessage.UseVisualStyleBackColor = true;
            // 
            // ShortcutGroup
            // 
            this.ShortcutGroup.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ShortcutGroup.Controls.Add(this.chkStartMenuShortcut);
            this.ShortcutGroup.Controls.Add(this.chkDesktopShortcut);
            this.ShortcutGroup.Controls.Add(this.chkRightClickOption);
            this.ShortcutGroup.Location = new System.Drawing.Point(10, 55);
            this.ShortcutGroup.Margin = new System.Windows.Forms.Padding(4);
            this.ShortcutGroup.Name = "ShortcutGroup";
            this.ShortcutGroup.Padding = new System.Windows.Forms.Padding(4);
            this.ShortcutGroup.Size = new System.Drawing.Size(671, 130);
            this.ShortcutGroup.TabIndex = 35;
            this.ShortcutGroup.TabStop = false;
            this.ShortcutGroup.Text = "Shortcuts";
            // 
            // chkStartMenuShortcut
            // 
            this.chkStartMenuShortcut.AutoSize = true;
            this.chkStartMenuShortcut.BackColor = System.Drawing.Color.Transparent;
            this.chkStartMenuShortcut.Location = new System.Drawing.Point(8, 90);
            this.chkStartMenuShortcut.Margin = new System.Windows.Forms.Padding(4);
            this.chkStartMenuShortcut.Name = "chkStartMenuShortcut";
            this.chkStartMenuShortcut.Size = new System.Drawing.Size(162, 22);
            this.chkStartMenuShortcut.TabIndex = 29;
            this.chkStartMenuShortcut.Text = "Start Menu Shortcut";
            this.chkStartMenuShortcut.UseVisualStyleBackColor = false;
            // 
            // chkDesktopShortcut
            // 
            this.chkDesktopShortcut.AutoSize = true;
            this.chkDesktopShortcut.BackColor = System.Drawing.Color.Transparent;
            this.chkDesktopShortcut.Location = new System.Drawing.Point(8, 60);
            this.chkDesktopShortcut.Margin = new System.Windows.Forms.Padding(4);
            this.chkDesktopShortcut.Name = "chkDesktopShortcut";
            this.chkDesktopShortcut.Size = new System.Drawing.Size(146, 22);
            this.chkDesktopShortcut.TabIndex = 28;
            this.chkDesktopShortcut.Text = "Desktop Shortcut";
            this.chkDesktopShortcut.UseVisualStyleBackColor = false;
            // 
            // chkRightClickOption
            // 
            this.chkRightClickOption.AutoSize = true;
            this.chkRightClickOption.BackColor = System.Drawing.Color.Transparent;
            this.chkRightClickOption.Location = new System.Drawing.Point(8, 30);
            this.chkRightClickOption.Margin = new System.Windows.Forms.Padding(4);
            this.chkRightClickOption.Name = "chkRightClickOption";
            this.chkRightClickOption.Size = new System.Drawing.Size(234, 22);
            this.chkRightClickOption.TabIndex = 20;
            this.chkRightClickOption.Text = "Set right-click option on folders";
            this.chkRightClickOption.UseVisualStyleBackColor = false;
            // 
            // LanguageGroup
            // 
            this.LanguageGroup.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.LanguageGroup.Controls.Add(this.cboLanguage);
            this.LanguageGroup.Location = new System.Drawing.Point(10, 192);
            this.LanguageGroup.Margin = new System.Windows.Forms.Padding(4);
            this.LanguageGroup.Name = "LanguageGroup";
            this.LanguageGroup.Padding = new System.Windows.Forms.Padding(4);
            this.LanguageGroup.Size = new System.Drawing.Size(671, 75);
            this.LanguageGroup.TabIndex = 33;
            this.LanguageGroup.TabStop = false;
            this.LanguageGroup.Text = "Language";
            // 
            // cboLanguage
            // 
            this.cboLanguage.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboLanguage.Location = new System.Drawing.Point(20, 30);
            this.cboLanguage.Margin = new System.Windows.Forms.Padding(4);
            this.cboLanguage.Name = "cboLanguage";
            this.cboLanguage.Size = new System.Drawing.Size(179, 26);
            this.cboLanguage.TabIndex = 23;
            // 
            // cboPathMRUCount
            // 
            this.cboPathMRUCount.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboPathMRUCount.Items.AddRange(new object[] {
            "1",
            "2",
            "3",
            "4",
            "5",
            "6",
            "7",
            "8",
            "9",
            "10",
            "11",
            "12",
            "13",
            "14",
            "15",
            "16",
            "17",
            "18",
            "19",
            "20",
            "21",
            "22",
            "23",
            "24",
            "25"});
            this.cboPathMRUCount.Location = new System.Drawing.Point(10, 15);
            this.cboPathMRUCount.Margin = new System.Windows.Forms.Padding(4);
            this.cboPathMRUCount.Name = "cboPathMRUCount";
            this.cboPathMRUCount.Size = new System.Drawing.Size(69, 26);
            this.cboPathMRUCount.TabIndex = 31;
            // 
            // lblStoredPaths
            // 
            this.lblStoredPaths.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblStoredPaths.BackColor = System.Drawing.Color.Transparent;
            this.lblStoredPaths.Cursor = System.Windows.Forms.Cursors.Default;
            this.lblStoredPaths.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblStoredPaths.Location = new System.Drawing.Point(100, 15);
            this.lblStoredPaths.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblStoredPaths.Name = "lblStoredPaths";
            this.lblStoredPaths.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.lblStoredPaths.Size = new System.Drawing.Size(581, 26);
            this.lblStoredPaths.TabIndex = 32;
            this.lblStoredPaths.Text = "Number of most recently used paths to store";
            this.lblStoredPaths.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // tabFileEncoding
            // 
            this.tabFileEncoding.Controls.Add(this.btnCacheClear);
            this.tabFileEncoding.Controls.Add(this.chkUseEncodingCache);
            this.tabFileEncoding.Controls.Add(this.cboPerformance);
            this.tabFileEncoding.Controls.Add(this.lblPerformance);
            this.tabFileEncoding.Controls.Add(this.chkDetectFileEncoding);
            this.tabFileEncoding.Controls.Add(this.btnFileEncodingDelete);
            this.tabFileEncoding.Controls.Add(this.btnFileEncodingEdit);
            this.tabFileEncoding.Controls.Add(this.btnFileEncodingAdd);
            this.tabFileEncoding.Controls.Add(this.lstFiles);
            this.tabFileEncoding.Location = new System.Drawing.Point(4, 27);
            this.tabFileEncoding.Margin = new System.Windows.Forms.Padding(4);
            this.tabFileEncoding.Name = "tabFileEncoding";
            this.tabFileEncoding.Size = new System.Drawing.Size(693, 457);
            this.tabFileEncoding.TabIndex = 4;
            this.tabFileEncoding.Text = "File Encoding";
            this.tabFileEncoding.UseVisualStyleBackColor = true;
            // 
            // btnCacheClear
            // 
            this.btnCacheClear.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCacheClear.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnCacheClear.Location = new System.Drawing.Point(519, 79);
            this.btnCacheClear.Margin = new System.Windows.Forms.Padding(4);
            this.btnCacheClear.Name = "btnCacheClear";
            this.btnCacheClear.Size = new System.Drawing.Size(162, 31);
            this.btnCacheClear.TabIndex = 43;
            this.btnCacheClear.Text = "Clear Cache";
            this.btnCacheClear.UseVisualStyleBackColor = true;
            this.btnCacheClear.Click += new System.EventHandler(this.btnCacheClear_Click);
            // 
            // chkUseEncodingCache
            // 
            this.chkUseEncodingCache.AutoSize = true;
            this.chkUseEncodingCache.Location = new System.Drawing.Point(10, 84);
            this.chkUseEncodingCache.Margin = new System.Windows.Forms.Padding(4);
            this.chkUseEncodingCache.Name = "chkUseEncodingCache";
            this.chkUseEncodingCache.Size = new System.Drawing.Size(277, 22);
            this.chkUseEncodingCache.TabIndex = 42;
            this.chkUseEncodingCache.Text = "Enable cache for detected encodings.";
            this.chkUseEncodingCache.UseVisualStyleBackColor = true;
            // 
            // cboPerformance
            // 
            this.cboPerformance.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboPerformance.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.cboPerformance.FormattingEnabled = true;
            this.cboPerformance.Location = new System.Drawing.Point(126, 45);
            this.cboPerformance.Margin = new System.Windows.Forms.Padding(4);
            this.cboPerformance.Name = "cboPerformance";
            this.cboPerformance.Size = new System.Drawing.Size(150, 26);
            this.cboPerformance.TabIndex = 41;
            // 
            // lblPerformance
            // 
            this.lblPerformance.AutoSize = true;
            this.lblPerformance.Location = new System.Drawing.Point(6, 50);
            this.lblPerformance.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblPerformance.Name = "lblPerformance";
            this.lblPerformance.Size = new System.Drawing.Size(94, 18);
            this.lblPerformance.TabIndex = 40;
            this.lblPerformance.Text = "Performance";
            // 
            // chkDetectFileEncoding
            // 
            this.chkDetectFileEncoding.AutoSize = true;
            this.chkDetectFileEncoding.Location = new System.Drawing.Point(10, 12);
            this.chkDetectFileEncoding.Margin = new System.Windows.Forms.Padding(4);
            this.chkDetectFileEncoding.Name = "chkDetectFileEncoding";
            this.chkDetectFileEncoding.Size = new System.Drawing.Size(163, 22);
            this.chkDetectFileEncoding.TabIndex = 39;
            this.chkDetectFileEncoding.Text = "Detect file encoding.";
            this.chkDetectFileEncoding.UseVisualStyleBackColor = true;
            this.chkDetectFileEncoding.CheckedChanged += new System.EventHandler(this.chkDetectFileEncoding_CheckedChanged);
            // 
            // btnFileEncodingDelete
            // 
            this.btnFileEncodingDelete.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnFileEncodingDelete.Location = new System.Drawing.Point(250, 402);
            this.btnFileEncodingDelete.Margin = new System.Windows.Forms.Padding(4);
            this.btnFileEncodingDelete.Name = "btnFileEncodingDelete";
            this.btnFileEncodingDelete.Size = new System.Drawing.Size(112, 31);
            this.btnFileEncodingDelete.TabIndex = 4;
            this.btnFileEncodingDelete.Text = "&Delete";
            this.btnFileEncodingDelete.UseVisualStyleBackColor = true;
            this.btnFileEncodingDelete.Click += new System.EventHandler(this.btnFileEncodingDelete_Click);
            // 
            // btnFileEncodingEdit
            // 
            this.btnFileEncodingEdit.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnFileEncodingEdit.Location = new System.Drawing.Point(130, 402);
            this.btnFileEncodingEdit.Margin = new System.Windows.Forms.Padding(4);
            this.btnFileEncodingEdit.Name = "btnFileEncodingEdit";
            this.btnFileEncodingEdit.Size = new System.Drawing.Size(112, 31);
            this.btnFileEncodingEdit.TabIndex = 3;
            this.btnFileEncodingEdit.Text = "&Edit...";
            this.btnFileEncodingEdit.UseVisualStyleBackColor = true;
            this.btnFileEncodingEdit.Click += new System.EventHandler(this.btnFileEncodingEdit_Click);
            // 
            // btnFileEncodingAdd
            // 
            this.btnFileEncodingAdd.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnFileEncodingAdd.Location = new System.Drawing.Point(10, 402);
            this.btnFileEncodingAdd.Margin = new System.Windows.Forms.Padding(4);
            this.btnFileEncodingAdd.Name = "btnFileEncodingAdd";
            this.btnFileEncodingAdd.Size = new System.Drawing.Size(112, 31);
            this.btnFileEncodingAdd.TabIndex = 2;
            this.btnFileEncodingAdd.Text = "&Add...";
            this.btnFileEncodingAdd.UseVisualStyleBackColor = true;
            this.btnFileEncodingAdd.Click += new System.EventHandler(this.btnFileEncodingAdd_Click);
            // 
            // lstFiles
            // 
            this.lstFiles.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lstFiles.CheckBoxes = true;
            this.lstFiles.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.clhEnabled,
            this.clhFile,
            this.clhEncoding});
            this.lstFiles.FullRowSelect = true;
            this.lstFiles.HideSelection = false;
            this.lstFiles.Location = new System.Drawing.Point(10, 122);
            this.lstFiles.Margin = new System.Windows.Forms.Padding(4);
            this.lstFiles.Name = "lstFiles";
            this.lstFiles.Size = new System.Drawing.Size(670, 264);
            this.lstFiles.TabIndex = 1;
            this.lstFiles.UseCompatibleStateImageBehavior = false;
            this.lstFiles.View = System.Windows.Forms.View.Details;
            this.lstFiles.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.lstFiles_ColumnClick);
            this.lstFiles.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.lstFiles_ItemCheck);
            this.lstFiles.SelectedIndexChanged += new System.EventHandler(this.lstFiles_SelectedIndexChanged);
            this.lstFiles.DoubleClick += new System.EventHandler(this.lstFiles_DoubleClick);
            this.lstFiles.KeyDown += new System.Windows.Forms.KeyEventHandler(this.lstFiles_KeyDown);
            this.lstFiles.MouseDown += new System.Windows.Forms.MouseEventHandler(this.lstFiles_MouseDown);
            this.lstFiles.MouseUp += new System.Windows.Forms.MouseEventHandler(this.lstFiles_MouseUp);
            // 
            // clhEnabled
            // 
            this.clhEnabled.Text = "Enabled";
            this.clhEnabled.Width = 69;
            // 
            // clhFile
            // 
            this.clhFile.Text = "File";
            this.clhFile.Width = 320;
            // 
            // clhEncoding
            // 
            this.clhEncoding.Text = "Encoding";
            this.clhEncoding.Width = 132;
            // 
            // tabTextEditors
            // 
            this.tabTextEditors.Controls.Add(this.btnEdit);
            this.tabTextEditors.Controls.Add(this.btnRemove);
            this.tabTextEditors.Controls.Add(this.btnAdd);
            this.tabTextEditors.Controls.Add(this.TextEditorsList);
            this.tabTextEditors.Location = new System.Drawing.Point(4, 27);
            this.tabTextEditors.Margin = new System.Windows.Forms.Padding(4);
            this.tabTextEditors.Name = "tabTextEditors";
            this.tabTextEditors.Size = new System.Drawing.Size(693, 457);
            this.tabTextEditors.TabIndex = 1;
            this.tabTextEditors.Text = "Text Editors";
            this.tabTextEditors.UseVisualStyleBackColor = true;
            // 
            // btnEdit
            // 
            this.btnEdit.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnEdit.Location = new System.Drawing.Point(130, 402);
            this.btnEdit.Margin = new System.Windows.Forms.Padding(4);
            this.btnEdit.Name = "btnEdit";
            this.btnEdit.Size = new System.Drawing.Size(112, 31);
            this.btnEdit.TabIndex = 16;
            this.btnEdit.Text = "&Edit...";
            this.btnEdit.Click += new System.EventHandler(this.btnEdit_Click);
            // 
            // btnRemove
            // 
            this.btnRemove.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnRemove.Location = new System.Drawing.Point(250, 402);
            this.btnRemove.Margin = new System.Windows.Forms.Padding(4);
            this.btnRemove.Name = "btnRemove";
            this.btnRemove.Size = new System.Drawing.Size(112, 31);
            this.btnRemove.TabIndex = 15;
            this.btnRemove.Text = "&Delete";
            this.btnRemove.Click += new System.EventHandler(this.btnRemove_Click);
            // 
            // btnAdd
            // 
            this.btnAdd.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnAdd.Location = new System.Drawing.Point(10, 402);
            this.btnAdd.Margin = new System.Windows.Forms.Padding(4);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(112, 31);
            this.btnAdd.TabIndex = 14;
            this.btnAdd.Text = "&Add...";
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // TextEditorsList
            // 
            this.TextEditorsList.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TextEditorsList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.ColumnType,
            this.ColumnEditor,
            this.ColumnArguments,
            this.ColumnTabSize});
            this.TextEditorsList.FullRowSelect = true;
            this.TextEditorsList.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.TextEditorsList.HideSelection = false;
            this.TextEditorsList.Location = new System.Drawing.Point(10, 10);
            this.TextEditorsList.Margin = new System.Windows.Forms.Padding(4);
            this.TextEditorsList.MultiSelect = false;
            this.TextEditorsList.Name = "TextEditorsList";
            this.TextEditorsList.Size = new System.Drawing.Size(670, 376);
            this.TextEditorsList.TabIndex = 13;
            this.TextEditorsList.UseCompatibleStateImageBehavior = false;
            this.TextEditorsList.View = System.Windows.Forms.View.Details;
            this.TextEditorsList.SelectedIndexChanged += new System.EventHandler(this.TextEditorsList_SelectedIndexChanged);
            this.TextEditorsList.DoubleClick += new System.EventHandler(this.TextEditorsList_DoubleClick);
            // 
            // ColumnType
            // 
            this.ColumnType.Text = "File Type";
            this.ColumnType.Width = 100;
            // 
            // ColumnEditor
            // 
            this.ColumnEditor.Text = "Text Editor";
            this.ColumnEditor.Width = 240;
            // 
            // ColumnArguments
            // 
            this.ColumnArguments.Text = "Command Line";
            this.ColumnArguments.Width = 113;
            // 
            // ColumnTabSize
            // 
            this.ColumnTabSize.Text = "Tab Size";
            this.ColumnTabSize.Width = 75;
            // 
            // tabResults
            // 
            this.tabResults.Controls.Add(this.pnlResultsPreview);
            this.tabResults.Controls.Add(this.grpFileList);
            this.tabResults.Controls.Add(this.grpResultWindow);
            this.tabResults.Controls.Add(this.grpResultMatch);
            this.tabResults.Location = new System.Drawing.Point(4, 27);
            this.tabResults.Margin = new System.Windows.Forms.Padding(4);
            this.tabResults.Name = "tabResults";
            this.tabResults.Size = new System.Drawing.Size(693, 457);
            this.tabResults.TabIndex = 2;
            this.tabResults.Text = "Results";
            this.tabResults.UseVisualStyleBackColor = true;
            // 
            // pnlResultsPreview
            // 
            this.pnlResultsPreview.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlResultsPreview.Controls.Add(this.lblResultPreview);
            this.pnlResultsPreview.Controls.Add(this.rtxtResultsPreview);
            this.pnlResultsPreview.Location = new System.Drawing.Point(10, 360);
            this.pnlResultsPreview.Margin = new System.Windows.Forms.Padding(4);
            this.pnlResultsPreview.Name = "pnlResultsPreview";
            this.pnlResultsPreview.Size = new System.Drawing.Size(671, 76);
            this.pnlResultsPreview.TabIndex = 28;
            // 
            // lblResultPreview
            // 
            this.lblResultPreview.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblResultPreview.Location = new System.Drawing.Point(0, 0);
            this.lblResultPreview.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblResultPreview.Name = "lblResultPreview";
            this.lblResultPreview.Size = new System.Drawing.Size(671, 20);
            this.lblResultPreview.TabIndex = 28;
            this.lblResultPreview.Text = "Results Preview";
            // 
            // rtxtResultsPreview
            // 
            this.rtxtResultsPreview.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.rtxtResultsPreview.Location = new System.Drawing.Point(0, 26);
            this.rtxtResultsPreview.Margin = new System.Windows.Forms.Padding(4);
            this.rtxtResultsPreview.Name = "rtxtResultsPreview";
            this.rtxtResultsPreview.ReadOnly = true;
            this.rtxtResultsPreview.Size = new System.Drawing.Size(670, 49);
            this.rtxtResultsPreview.TabIndex = 27;
            this.rtxtResultsPreview.Text = "(21)  Example results line and, match, displayed";
            // 
            // grpFileList
            // 
            this.grpFileList.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpFileList.Controls.Add(this.lblFileCurrentFont);
            this.grpFileList.Controls.Add(this.btnFileFindFont);
            this.grpFileList.Location = new System.Drawing.Point(10, 4);
            this.grpFileList.Margin = new System.Windows.Forms.Padding(4);
            this.grpFileList.Name = "grpFileList";
            this.grpFileList.Padding = new System.Windows.Forms.Padding(4);
            this.grpFileList.Size = new System.Drawing.Size(671, 82);
            this.grpFileList.TabIndex = 27;
            this.grpFileList.TabStop = false;
            this.grpFileList.Text = "File List";
            // 
            // lblFileCurrentFont
            // 
            this.lblFileCurrentFont.AutoSize = true;
            this.lblFileCurrentFont.Location = new System.Drawing.Point(10, 42);
            this.lblFileCurrentFont.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblFileCurrentFont.Name = "lblFileCurrentFont";
            this.lblFileCurrentFont.Size = new System.Drawing.Size(91, 18);
            this.lblFileCurrentFont.TabIndex = 1;
            this.lblFileCurrentFont.Text = "Current Font";
            // 
            // btnFileFindFont
            // 
            this.btnFileFindFont.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnFileFindFont.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnFileFindFont.Location = new System.Drawing.Point(495, 36);
            this.btnFileFindFont.Margin = new System.Windows.Forms.Padding(4);
            this.btnFileFindFont.Name = "btnFileFindFont";
            this.btnFileFindFont.Size = new System.Drawing.Size(150, 31);
            this.btnFileFindFont.TabIndex = 0;
            this.btnFileFindFont.Text = "Find Font";
            this.btnFileFindFont.UseVisualStyleBackColor = true;
            this.btnFileFindFont.Click += new System.EventHandler(this.btnFileFindFont_Click);
            // 
            // grpResultWindow
            // 
            this.grpResultWindow.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpResultWindow.Controls.Add(this.btnResultsContextForeColor);
            this.grpResultWindow.Controls.Add(this.lblResultsContextForeColor);
            this.grpResultWindow.Controls.Add(this.lblCurrentFont);
            this.grpResultWindow.Controls.Add(this.btnFindFont);
            this.grpResultWindow.Controls.Add(this.btnResultsWindowBackColor);
            this.grpResultWindow.Controls.Add(this.btnResultsWindowForeColor);
            this.grpResultWindow.Controls.Add(this.lblResultsWindowBack);
            this.grpResultWindow.Controls.Add(this.lblResultsWindowFore);
            this.grpResultWindow.Location = new System.Drawing.Point(10, 175);
            this.grpResultWindow.Margin = new System.Windows.Forms.Padding(4);
            this.grpResultWindow.Name = "grpResultWindow";
            this.grpResultWindow.Padding = new System.Windows.Forms.Padding(4);
            this.grpResultWindow.Size = new System.Drawing.Size(671, 178);
            this.grpResultWindow.TabIndex = 24;
            this.grpResultWindow.TabStop = false;
            this.grpResultWindow.Text = "Results Window";
            // 
            // btnResultsContextForeColor
            // 
            this.btnResultsContextForeColor.ForeColor = System.Drawing.Color.Silver;
            this.btnResultsContextForeColor.Location = new System.Drawing.Point(180, 82);
            this.btnResultsContextForeColor.Margin = new System.Windows.Forms.Padding(4);
            this.btnResultsContextForeColor.Name = "btnResultsContextForeColor";
            this.btnResultsContextForeColor.SelectedColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.btnResultsContextForeColor.Size = new System.Drawing.Size(94, 29);
            this.btnResultsContextForeColor.TabIndex = 26;
            // 
            // lblResultsContextForeColor
            // 
            this.lblResultsContextForeColor.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.lblResultsContextForeColor.Location = new System.Drawing.Point(10, 80);
            this.lblResultsContextForeColor.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblResultsContextForeColor.Name = "lblResultsContextForeColor";
            this.lblResultsContextForeColor.Size = new System.Drawing.Size(170, 34);
            this.lblResultsContextForeColor.TabIndex = 25;
            this.lblResultsContextForeColor.Text = "Context Fore Color";
            this.lblResultsContextForeColor.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblCurrentFont
            // 
            this.lblCurrentFont.AutoSize = true;
            this.lblCurrentFont.Location = new System.Drawing.Point(10, 138);
            this.lblCurrentFont.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblCurrentFont.Name = "lblCurrentFont";
            this.lblCurrentFont.Size = new System.Drawing.Size(91, 18);
            this.lblCurrentFont.TabIndex = 24;
            this.lblCurrentFont.Text = "Current Font";
            // 
            // btnFindFont
            // 
            this.btnFindFont.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnFindFont.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnFindFont.Location = new System.Drawing.Point(495, 131);
            this.btnFindFont.Margin = new System.Windows.Forms.Padding(4);
            this.btnFindFont.Name = "btnFindFont";
            this.btnFindFont.Size = new System.Drawing.Size(150, 31);
            this.btnFindFont.TabIndex = 23;
            this.btnFindFont.Text = "&Find Font";
            this.btnFindFont.UseVisualStyleBackColor = true;
            this.btnFindFont.Click += new System.EventHandler(this.btnFindFont_Click);
            // 
            // btnResultsWindowBackColor
            // 
            this.btnResultsWindowBackColor.Location = new System.Drawing.Point(551, 30);
            this.btnResultsWindowBackColor.Margin = new System.Windows.Forms.Padding(4);
            this.btnResultsWindowBackColor.Name = "btnResultsWindowBackColor";
            this.btnResultsWindowBackColor.SelectedColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.btnResultsWindowBackColor.Size = new System.Drawing.Size(94, 29);
            this.btnResultsWindowBackColor.TabIndex = 22;
            // 
            // btnResultsWindowForeColor
            // 
            this.btnResultsWindowForeColor.Location = new System.Drawing.Point(180, 30);
            this.btnResultsWindowForeColor.Margin = new System.Windows.Forms.Padding(4);
            this.btnResultsWindowForeColor.Name = "btnResultsWindowForeColor";
            this.btnResultsWindowForeColor.SelectedColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.btnResultsWindowForeColor.Size = new System.Drawing.Size(94, 29);
            this.btnResultsWindowForeColor.TabIndex = 21;
            // 
            // lblResultsWindowBack
            // 
            this.lblResultsWindowBack.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.lblResultsWindowBack.Location = new System.Drawing.Point(381, 28);
            this.lblResultsWindowBack.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblResultsWindowBack.Name = "lblResultsWindowBack";
            this.lblResultsWindowBack.Size = new System.Drawing.Size(170, 34);
            this.lblResultsWindowBack.TabIndex = 20;
            this.lblResultsWindowBack.Text = "Back Color";
            this.lblResultsWindowBack.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblResultsWindowFore
            // 
            this.lblResultsWindowFore.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.lblResultsWindowFore.Location = new System.Drawing.Point(10, 28);
            this.lblResultsWindowFore.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblResultsWindowFore.Name = "lblResultsWindowFore";
            this.lblResultsWindowFore.Size = new System.Drawing.Size(170, 34);
            this.lblResultsWindowFore.TabIndex = 19;
            this.lblResultsWindowFore.Text = "Fore Color";
            this.lblResultsWindowFore.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // grpResultMatch
            // 
            this.grpResultMatch.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpResultMatch.Controls.Add(this.BackColorButton);
            this.grpResultMatch.Controls.Add(this.ForeColorButton);
            this.grpResultMatch.Controls.Add(this.BackColorLabel);
            this.grpResultMatch.Controls.Add(this.ForeColorLabel);
            this.grpResultMatch.Location = new System.Drawing.Point(10, 98);
            this.grpResultMatch.Margin = new System.Windows.Forms.Padding(4);
            this.grpResultMatch.Name = "grpResultMatch";
            this.grpResultMatch.Padding = new System.Windows.Forms.Padding(4);
            this.grpResultMatch.Size = new System.Drawing.Size(671, 70);
            this.grpResultMatch.TabIndex = 23;
            this.grpResultMatch.TabStop = false;
            this.grpResultMatch.Text = "Results Match";
            // 
            // BackColorButton
            // 
            this.BackColorButton.Location = new System.Drawing.Point(551, 30);
            this.BackColorButton.Margin = new System.Windows.Forms.Padding(4);
            this.BackColorButton.Name = "BackColorButton";
            this.BackColorButton.SelectedColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.BackColorButton.Size = new System.Drawing.Size(94, 29);
            this.BackColorButton.TabIndex = 18;
            // 
            // ForeColorButton
            // 
            this.ForeColorButton.Location = new System.Drawing.Point(180, 30);
            this.ForeColorButton.Margin = new System.Windows.Forms.Padding(4);
            this.ForeColorButton.Name = "ForeColorButton";
            this.ForeColorButton.SelectedColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.ForeColorButton.Size = new System.Drawing.Size(94, 29);
            this.ForeColorButton.TabIndex = 17;
            // 
            // BackColorLabel
            // 
            this.BackColorLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.BackColorLabel.Location = new System.Drawing.Point(381, 28);
            this.BackColorLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.BackColorLabel.Name = "BackColorLabel";
            this.BackColorLabel.Size = new System.Drawing.Size(170, 34);
            this.BackColorLabel.TabIndex = 16;
            this.BackColorLabel.Text = "Back Color";
            this.BackColorLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // ForeColorLabel
            // 
            this.ForeColorLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.ForeColorLabel.Location = new System.Drawing.Point(10, 28);
            this.ForeColorLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.ForeColorLabel.Name = "ForeColorLabel";
            this.ForeColorLabel.Size = new System.Drawing.Size(170, 34);
            this.ForeColorLabel.TabIndex = 15;
            this.ForeColorLabel.Text = "Fore Color";
            this.ForeColorLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // tabPlugins
            // 
            this.tabPlugins.Controls.Add(this.btnDown);
            this.tabPlugins.Controls.Add(this.btnUp);
            this.tabPlugins.Controls.Add(this.PluginDetailsGroup);
            this.tabPlugins.Controls.Add(this.PluginsList);
            this.tabPlugins.Location = new System.Drawing.Point(4, 27);
            this.tabPlugins.Margin = new System.Windows.Forms.Padding(4);
            this.tabPlugins.Name = "tabPlugins";
            this.tabPlugins.Size = new System.Drawing.Size(693, 457);
            this.tabPlugins.TabIndex = 3;
            this.tabPlugins.Text = "Plugins";
            this.tabPlugins.UseVisualStyleBackColor = true;
            // 
            // btnDown
            // 
            this.btnDown.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnDown.Location = new System.Drawing.Point(639, 155);
            this.btnDown.Margin = new System.Windows.Forms.Padding(4);
            this.btnDown.Name = "btnDown";
            this.btnDown.Padding = new System.Windows.Forms.Padding(0, 5, 0, 0);
            this.btnDown.Size = new System.Drawing.Size(42, 35);
            this.btnDown.TabIndex = 5;
            this.btnDown.Text = "▼";
            this.btnDown.Click += new System.EventHandler(this.btnDown_Click);
            // 
            // btnUp
            // 
            this.btnUp.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnUp.Location = new System.Drawing.Point(639, 92);
            this.btnUp.Margin = new System.Windows.Forms.Padding(4);
            this.btnUp.Name = "btnUp";
            this.btnUp.Padding = new System.Windows.Forms.Padding(0, 2, 0, 0);
            this.btnUp.Size = new System.Drawing.Size(42, 35);
            this.btnUp.TabIndex = 4;
            this.btnUp.Text = "▲";
            this.btnUp.Click += new System.EventHandler(this.btnUp_Click);
            // 
            // PluginDetailsGroup
            // 
            this.PluginDetailsGroup.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.PluginDetailsGroup.Controls.Add(this.lblPluginDescription);
            this.PluginDetailsGroup.Controls.Add(this.lblPluginAuthor);
            this.PluginDetailsGroup.Controls.Add(this.lblPluginVersion);
            this.PluginDetailsGroup.Controls.Add(this.lblPluginName);
            this.PluginDetailsGroup.Controls.Add(this.lblPluginDetailAuthor);
            this.PluginDetailsGroup.Controls.Add(this.lblPluginDetailVersion);
            this.PluginDetailsGroup.Controls.Add(this.lblPluginDetailName);
            this.PluginDetailsGroup.Location = new System.Drawing.Point(10, 290);
            this.PluginDetailsGroup.Margin = new System.Windows.Forms.Padding(4);
            this.PluginDetailsGroup.Name = "PluginDetailsGroup";
            this.PluginDetailsGroup.Padding = new System.Windows.Forms.Padding(4);
            this.PluginDetailsGroup.Size = new System.Drawing.Size(671, 150);
            this.PluginDetailsGroup.TabIndex = 3;
            this.PluginDetailsGroup.TabStop = false;
            this.PluginDetailsGroup.Text = "Plugin Details";
            // 
            // lblPluginDescription
            // 
            this.lblPluginDescription.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblPluginDescription.Location = new System.Drawing.Point(340, 30);
            this.lblPluginDescription.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblPluginDescription.Name = "lblPluginDescription";
            this.lblPluginDescription.Size = new System.Drawing.Size(321, 110);
            this.lblPluginDescription.TabIndex = 3;
            // 
            // lblPluginAuthor
            // 
            this.lblPluginAuthor.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblPluginAuthor.Location = new System.Drawing.Point(120, 110);
            this.lblPluginAuthor.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblPluginAuthor.Name = "lblPluginAuthor";
            this.lblPluginAuthor.Size = new System.Drawing.Size(210, 29);
            this.lblPluginAuthor.TabIndex = 2;
            // 
            // lblPluginVersion
            // 
            this.lblPluginVersion.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblPluginVersion.Location = new System.Drawing.Point(120, 70);
            this.lblPluginVersion.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblPluginVersion.Name = "lblPluginVersion";
            this.lblPluginVersion.Size = new System.Drawing.Size(210, 29);
            this.lblPluginVersion.TabIndex = 6;
            // 
            // lblPluginName
            // 
            this.lblPluginName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblPluginName.Location = new System.Drawing.Point(120, 30);
            this.lblPluginName.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblPluginName.Name = "lblPluginName";
            this.lblPluginName.Size = new System.Drawing.Size(210, 29);
            this.lblPluginName.TabIndex = 5;
            // 
            // lblPluginDetailAuthor
            // 
            this.lblPluginDetailAuthor.Location = new System.Drawing.Point(20, 110);
            this.lblPluginDetailAuthor.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblPluginDetailAuthor.Name = "lblPluginDetailAuthor";
            this.lblPluginDetailAuthor.Size = new System.Drawing.Size(100, 29);
            this.lblPluginDetailAuthor.TabIndex = 7;
            this.lblPluginDetailAuthor.Text = "Author:";
            // 
            // lblPluginDetailVersion
            // 
            this.lblPluginDetailVersion.Location = new System.Drawing.Point(20, 70);
            this.lblPluginDetailVersion.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblPluginDetailVersion.Name = "lblPluginDetailVersion";
            this.lblPluginDetailVersion.Size = new System.Drawing.Size(100, 29);
            this.lblPluginDetailVersion.TabIndex = 1;
            this.lblPluginDetailVersion.Text = "Version:";
            // 
            // lblPluginDetailName
            // 
            this.lblPluginDetailName.Location = new System.Drawing.Point(20, 30);
            this.lblPluginDetailName.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblPluginDetailName.Name = "lblPluginDetailName";
            this.lblPluginDetailName.Size = new System.Drawing.Size(100, 29);
            this.lblPluginDetailName.TabIndex = 0;
            this.lblPluginDetailName.Text = "Name:";
            // 
            // PluginsList
            // 
            this.PluginsList.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.PluginsList.CheckBoxes = true;
            this.PluginsList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.PluginsColumnEnabled,
            this.PluginsColumnName,
            this.PluginsColumnExt});
            this.PluginsList.FullRowSelect = true;
            this.PluginsList.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.PluginsList.HideSelection = false;
            this.PluginsList.Location = new System.Drawing.Point(10, 10);
            this.PluginsList.Margin = new System.Windows.Forms.Padding(4);
            this.PluginsList.MultiSelect = false;
            this.PluginsList.Name = "PluginsList";
            this.PluginsList.Size = new System.Drawing.Size(614, 269);
            this.PluginsList.TabIndex = 2;
            this.PluginsList.UseCompatibleStateImageBehavior = false;
            this.PluginsList.View = System.Windows.Forms.View.Details;
            this.PluginsList.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.PluginsList_ItemCheck);
            this.PluginsList.SelectedIndexChanged += new System.EventHandler(this.PluginsList_SelectedIndexChanged);
            // 
            // PluginsColumnEnabled
            // 
            this.PluginsColumnEnabled.Text = "Enabled";
            this.PluginsColumnEnabled.Width = 72;
            // 
            // PluginsColumnName
            // 
            this.PluginsColumnName.Text = "Name";
            this.PluginsColumnName.Width = 246;
            // 
            // PluginsColumnExt
            // 
            this.PluginsColumnExt.Text = "Extensions";
            this.PluginsColumnExt.Width = 134;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.cboServer);
            this.tabPage1.Controls.Add(this.cboDataBase);
            this.tabPage1.Controls.Add(this.btnTest);
            this.tabPage1.Controls.Add(this.lblDataBase);
            this.tabPage1.Controls.Add(this.lblPassword);
            this.tabPage1.Controls.Add(this.txtPassword);
            this.tabPage1.Controls.Add(this.lblLogin);
            this.tabPage1.Controls.Add(this.txtLogin);
            this.tabPage1.Controls.Add(this.chkIntegritySecurity);
            this.tabPage1.Controls.Add(this.lblServer);
            this.tabPage1.Controls.Add(this.chkUseDataBase);
            this.tabPage1.Location = new System.Drawing.Point(4, 27);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(693, 457);
            this.tabPage1.TabIndex = 5;
            this.tabPage1.Text = "Server";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // cboServer
            // 
            this.cboServer.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cboServer.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cboServer.Location = new System.Drawing.Point(258, 89);
            this.cboServer.Margin = new System.Windows.Forms.Padding(4);
            this.cboServer.Name = "cboServer";
            this.cboServer.Size = new System.Drawing.Size(302, 26);
            this.cboServer.TabIndex = 52;
            this.cboServer.DropDown += new System.EventHandler(this.cboServer_DropDown);
            // 
            // cboDataBase
            // 
            this.cboDataBase.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cboDataBase.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cboDataBase.Location = new System.Drawing.Point(258, 311);
            this.cboDataBase.Margin = new System.Windows.Forms.Padding(4);
            this.cboDataBase.Name = "cboDataBase";
            this.cboDataBase.Size = new System.Drawing.Size(302, 26);
            this.cboDataBase.TabIndex = 51;
            this.cboDataBase.DropDown += new System.EventHandler(this.cboDataBase_DropDown);
            // 
            // btnTest
            // 
            this.btnTest.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnTest.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnTest.Location = new System.Drawing.Point(569, 310);
            this.btnTest.Margin = new System.Windows.Forms.Padding(4);
            this.btnTest.Name = "btnTest";
            this.btnTest.Size = new System.Drawing.Size(37, 29);
            this.btnTest.TabIndex = 48;
            this.btnTest.Text = "Test";
            this.btnTest.UseVisualStyleBackColor = true;
            this.btnTest.Click += new System.EventHandler(this.btnTest_Click);
            // 
            // lblDataBase
            // 
            this.lblDataBase.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblDataBase.BackColor = System.Drawing.Color.Transparent;
            this.lblDataBase.Cursor = System.Windows.Forms.Cursors.Default;
            this.lblDataBase.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblDataBase.Location = new System.Drawing.Point(74, 311);
            this.lblDataBase.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblDataBase.Name = "lblDataBase";
            this.lblDataBase.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.lblDataBase.Size = new System.Drawing.Size(176, 26);
            this.lblDataBase.TabIndex = 47;
            this.lblDataBase.Text = "Database";
            this.lblDataBase.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblPassword
            // 
            this.lblPassword.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblPassword.BackColor = System.Drawing.Color.Transparent;
            this.lblPassword.Cursor = System.Windows.Forms.Cursors.Default;
            this.lblPassword.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblPassword.Location = new System.Drawing.Point(74, 239);
            this.lblPassword.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblPassword.Name = "lblPassword";
            this.lblPassword.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.lblPassword.Size = new System.Drawing.Size(176, 26);
            this.lblPassword.TabIndex = 45;
            this.lblPassword.Text = "Password";
            this.lblPassword.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtPassword
            // 
            this.txtPassword.Location = new System.Drawing.Point(258, 239);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.PasswordChar = '*';
            this.txtPassword.Size = new System.Drawing.Size(302, 24);
            this.txtPassword.TabIndex = 44;
            // 
            // lblLogin
            // 
            this.lblLogin.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblLogin.BackColor = System.Drawing.Color.Transparent;
            this.lblLogin.Cursor = System.Windows.Forms.Cursors.Default;
            this.lblLogin.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblLogin.Location = new System.Drawing.Point(74, 209);
            this.lblLogin.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblLogin.Name = "lblLogin";
            this.lblLogin.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.lblLogin.Size = new System.Drawing.Size(176, 26);
            this.lblLogin.TabIndex = 43;
            this.lblLogin.Text = "Login";
            this.lblLogin.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtLogin
            // 
            this.txtLogin.Location = new System.Drawing.Point(258, 209);
            this.txtLogin.Name = "txtLogin";
            this.txtLogin.Size = new System.Drawing.Size(302, 24);
            this.txtLogin.TabIndex = 42;
            // 
            // chkIntegritySecurity
            // 
            this.chkIntegritySecurity.AutoSize = true;
            this.chkIntegritySecurity.Location = new System.Drawing.Point(258, 135);
            this.chkIntegritySecurity.Margin = new System.Windows.Forms.Padding(4);
            this.chkIntegritySecurity.Name = "chkIntegritySecurity";
            this.chkIntegritySecurity.Size = new System.Drawing.Size(166, 22);
            this.chkIntegritySecurity.TabIndex = 41;
            this.chkIntegritySecurity.Text = "Use integrity security";
            this.chkIntegritySecurity.UseVisualStyleBackColor = true;
            // 
            // lblServer
            // 
            this.lblServer.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblServer.BackColor = System.Drawing.Color.Transparent;
            this.lblServer.Cursor = System.Windows.Forms.Cursors.Default;
            this.lblServer.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblServer.Location = new System.Drawing.Point(7, 89);
            this.lblServer.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblServer.Name = "lblServer";
            this.lblServer.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.lblServer.Size = new System.Drawing.Size(243, 26);
            this.lblServer.TabIndex = 40;
            this.lblServer.Text = "Server name or IP adress";
            this.lblServer.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // chkUseDataBase
            // 
            this.chkUseDataBase.AutoSize = true;
            this.chkUseDataBase.Location = new System.Drawing.Point(22, 23);
            this.chkUseDataBase.Margin = new System.Windows.Forms.Padding(4);
            this.chkUseDataBase.Name = "chkUseDataBase";
            this.chkUseDataBase.Size = new System.Drawing.Size(288, 22);
            this.chkUseDataBase.TabIndex = 39;
            this.chkUseDataBase.Text = "Save search results on Data Base SQL";
            this.chkUseDataBase.UseVisualStyleBackColor = true;
            this.chkUseDataBase.CheckedChanged += new System.EventHandler(this.chkUseDataBase_CheckedChanged);
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnOK.Location = new System.Drawing.Point(462, 508);
            this.btnOK.Margin = new System.Windows.Forms.Padding(4);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(112, 31);
            this.btnOK.TabIndex = 1;
            this.btnOK.Text = "&OK";
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnCancel.Location = new System.Drawing.Point(594, 508);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(4);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(112, 31);
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "&Cancel";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // frmOptions
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(120F, 120F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(719, 554);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.tbcOptions);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmOptions";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Options";
            this.Load += new System.EventHandler(this.frmOptions_Load);
            this.tbcOptions.ResumeLayout(false);
            this.tabGeneral.ResumeLayout(false);
            this.tabGeneral.PerformLayout();
            this.ShortcutGroup.ResumeLayout(false);
            this.ShortcutGroup.PerformLayout();
            this.LanguageGroup.ResumeLayout(false);
            this.tabFileEncoding.ResumeLayout(false);
            this.tabFileEncoding.PerformLayout();
            this.tabTextEditors.ResumeLayout(false);
            this.tabResults.ResumeLayout(false);
            this.pnlResultsPreview.ResumeLayout(false);
            this.grpFileList.ResumeLayout(false);
            this.grpFileList.PerformLayout();
            this.grpResultWindow.ResumeLayout(false);
            this.grpResultWindow.PerformLayout();
            this.grpResultMatch.ResumeLayout(false);
            this.tabPlugins.ResumeLayout(false);
            this.PluginDetailsGroup.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.ResumeLayout(false);

      }
      #endregion

      private System.Windows.Forms.TabControl tbcOptions;
      private System.Windows.Forms.Button btnOK;
      private System.Windows.Forms.Button btnCancel;
      private System.Windows.Forms.TabPage tabGeneral;
      private System.Windows.Forms.TabPage tabTextEditors;
      private System.Windows.Forms.TabPage tabResults;
      private System.Windows.Forms.TabPage tabPlugins;
      private System.Windows.Forms.GroupBox grpResultWindow;
      private bSearch.Windows.Controls.ColorButton btnResultsWindowBackColor;
      private bSearch.Windows.Controls.ColorButton btnResultsWindowForeColor;
      private System.Windows.Forms.Label lblResultsWindowBack;
      private System.Windows.Forms.Label lblResultsWindowFore;
      private System.Windows.Forms.GroupBox grpResultMatch;
      private bSearch.Windows.Controls.ColorButton BackColorButton;
      private bSearch.Windows.Controls.ColorButton ForeColorButton;
      private System.Windows.Forms.Label BackColorLabel;
      private System.Windows.Forms.Label ForeColorLabel;
      private System.Windows.Forms.GroupBox LanguageGroup;
      private System.Windows.Forms.ComboBox cboLanguage;
      private System.Windows.Forms.ComboBox cboPathMRUCount;
      private System.Windows.Forms.Label lblStoredPaths;
      private System.Windows.Forms.GroupBox PluginDetailsGroup;
      private System.Windows.Forms.Label lblPluginDetailAuthor;
      private System.Windows.Forms.Label lblPluginVersion;
      private System.Windows.Forms.Label lblPluginName;
      private System.Windows.Forms.Label lblPluginDescription;
      private System.Windows.Forms.Label lblPluginAuthor;
      private System.Windows.Forms.Label lblPluginDetailVersion;
      private System.Windows.Forms.Label lblPluginDetailName;
      private System.Windows.Forms.ListView PluginsList;
      private System.Windows.Forms.ColumnHeader PluginsColumnEnabled;
      private System.Windows.Forms.ColumnHeader PluginsColumnName;
      private System.Windows.Forms.ColumnHeader PluginsColumnExt;
      private System.Windows.Forms.Button btnEdit;
      private System.Windows.Forms.Button btnRemove;
      private System.Windows.Forms.Button btnAdd;
      private System.Windows.Forms.ListView TextEditorsList;
      private System.Windows.Forms.ColumnHeader ColumnType;
      private System.Windows.Forms.ColumnHeader ColumnEditor;
      private System.Windows.Forms.ColumnHeader ColumnArguments;
      private System.Windows.Forms.GroupBox ShortcutGroup;
      private System.Windows.Forms.CheckBox chkStartMenuShortcut;
      private System.Windows.Forms.CheckBox chkDesktopShortcut;
      private System.Windows.Forms.CheckBox chkRightClickOption;
      private Label lblCurrentFont;
      private Button btnFindFont;
      private CheckBox chkShowExclusionErrorMessage;
      private CheckBox chkSaveSearchOptions;
      private GroupBox grpFileList;
      private Label lblFileCurrentFont;
      private Button btnFileFindFont;
      private ColumnHeader ColumnTabSize;
      private Button btnDown;
      private Button btnUp;
      private TabPage tabFileEncoding;
      private Button btnFileEncodingDelete;
      private Button btnFileEncodingEdit;
      private Button btnFileEncodingAdd;
      private ListView lstFiles;
      private ColumnHeader clhEnabled;
      private ColumnHeader clhFile;
      private ColumnHeader clhEncoding;
      private CheckBox chkDetectFileEncoding;
      private CheckBox chkSaveMessagesPosition;
      private Controls.ColorButton btnResultsContextForeColor;
      private Label lblResultsContextForeColor;
      private ComboBox cboPerformance;
      private Label lblPerformance;
      private CheckBox chkUseEncodingCache;
      private Button btnCacheClear;
      private CheckBox chkSaveExclusionsPosition;
      private CheckBox chkLabelColor;
      private Panel pnlResultsPreview;
      private Label lblResultPreview;
      private RichTextBox rtxtResultsPreview;
      private TabPage tabPage1;
      private Button btnTest;
      private Label lblDataBase;
      private Label lblPassword;
      private TextBox txtPassword;
      private Label lblLogin;
      private TextBox txtLogin;
      private CheckBox chkIntegritySecurity;
      private Label lblServer;
      private CheckBox chkUseDataBase;
      private ComboBox cboDataBase;
      private ComboBox cboServer;
   }
}
