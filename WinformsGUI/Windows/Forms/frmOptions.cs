using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

using bSearch.Common;
using bSearch.Core;
using libbSearch;
using libbSearch.EncodingDetection;
using libbSearch.Plugin;
using System.Data.SqlClient;
using System.Data;
using System.Data.Sql;


namespace bSearch.Windows.Forms
{
    /// <summary>
    /// Used to display the options dialog.
    /// </summary>
    /// <remarks>
    /// bSearch File Searching Utility.
    /// Copyright (C) 2006 BigLevel Lda.
    /// 
    /// The author may be contacted at:
    /// suporte@bigLevel.pt or comercial@biglevel.pt
    /// </remarks>

    public partial class frmOptions : BaseForm
    {
        private bool __LanguageChange = false;
        private bool __RightClickEnabled = false;
        private bool __RightClickUpdate = false;
        private bool __IsAdmin = API.UACHelper.HasAdminPrivileges();
        private Font __FileFont = Convertors.ConvertStringToFont(GeneralSettings.FilePanelFont);
        private bool inhibitFileEncodingAutoCheck;

        /// <summary>
        /// Creates a new instance of the frmOptions class.
        /// </summary>

        public frmOptions()
        {
            InitializeComponent();

            __RightClickEnabled = Shortcuts.IsSearchOption();

            ForeColorButton.ColorChange += new bSearch.Windows.Controls.ColorButton.ColorChangeHandler(NewColor);
            BackColorButton.ColorChange += new bSearch.Windows.Controls.ColorButton.ColorChangeHandler(NewColor);
            btnResultsWindowForeColor.ColorChange += new bSearch.Windows.Controls.ColorButton.ColorChangeHandler(NewColor);
            btnResultsWindowBackColor.ColorChange += new bSearch.Windows.Controls.ColorButton.ColorChangeHandler(NewColor);
            btnResultsContextForeColor.ColorChange += new bSearch.Windows.Controls.ColorButton.ColorChangeHandler(NewColor);
            chkRightClickOption.CheckedChanged += new EventHandler(chkRightClickOption_CheckedChanged);

            // get mainform's image list to use here for up/down buttons
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMain));
            System.Windows.Forms.ImageList ListViewImageList = new ImageList();
            ListViewImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("ListViewImageList.ImageStream")));
            ListViewImageList.TransparentColor = System.Drawing.Color.Transparent;
            ListViewImageList.Images.SetKeyName(0, "");
            ListViewImageList.Images.SetKeyName(1, "");

            btnUp.ImageList = ListViewImageList;
            btnUp.ImageIndex = 0;
            btnDown.ImageList = ListViewImageList;
            btnDown.ImageIndex = 1;

            API.ListViewExtensions.SetTheme(lstFiles);
            API.ListViewExtensions.SetTheme(TextEditorsList);
            API.ListViewExtensions.SetTheme(PluginsList);
        }

        /// <summary>
        /// Checks to see if the language w changed.
        /// </summary>

        public bool IsLanguageChange
        {
            get { return __LanguageChange; }
        }

        /// <summary>
        /// Handles setting the user specified options into the correct controls for display.
        /// </summary>
        /// <param name="sender">System parameter</param>
        /// <param name="e">System parameter</param>

        private void frmOptions_Load(object sender, System.EventArgs e)
        {
            cboPathMRUCount.SelectedIndex = GeneralSettings.MaximumMRUPaths - 1;
            chkRightClickOption.Checked = Shortcuts.IsSearchOption();
            if (Registry.IsInstaller())
            {
                chkDesktopShortcut.Visible = false;
                chkStartMenuShortcut.Visible = false;
            }
            else
            {
                chkDesktopShortcut.Checked = Shortcuts.IsDesktopShortcut();
                chkStartMenuShortcut.Checked = Shortcuts.IsStartMenuShortcut();
            }
            chkShowExclusionErrorMessage.Checked = GeneralSettings.ShowExclusionErrorMessage;
            chkSaveSearchOptions.Checked = GeneralSettings.SaveSearchOptionsOnExit;
            chkDetectFileEncoding.Checked = GeneralSettings.DetectFileEncoding;
            chkUseEncodingCache.Checked = GeneralSettings.UseEncodingCache;
            chkSaveMessagesPosition.Checked = GeneralSettings.LogDisplaySavePosition;
            chkSaveExclusionsPosition.Checked = GeneralSettings.ExclusionsDisplaySavePosition;
            chkLabelColor.Checked = GeneralSettings.UsebSearchAccentColor;

            // ColorButton init
            ForeColorButton.SelectedColor = Convertors.ConvertStringToColor(GeneralSettings.HighlightForeColor);
            BackColorButton.SelectedColor = Convertors.ConvertStringToColor(GeneralSettings.HighlightBackColor);
            btnResultsWindowForeColor.SelectedColor = Convertors.ConvertStringToColor(GeneralSettings.ResultsForeColor);
            btnResultsWindowBackColor.SelectedColor = Convertors.ConvertStringToColor(GeneralSettings.ResultsBackColor);
            btnResultsContextForeColor.SelectedColor = Convertors.ConvertStringToColor(GeneralSettings.ResultsContextForeColor);

            // results font
            rtxtResultsPreview.Font = Convertors.ConvertStringToFont(GeneralSettings.ResultsFont);
            DisplayFont(rtxtResultsPreview.Font, lblCurrentFont);

            // file list font
            DisplayFont(__FileFont, lblFileCurrentFont);

            tbcOptions.SelectedTab = tabGeneral;

            LoadEditors(TextEditors.GetAll());
            LoadPlugins();
            LoadFileEncodings();

            //Language.GenerateXml(this, Application.StartupPath + "\\" + this.Name + ".xml");
            Language.ProcessForm(this);
            Language.LoadComboBox(cboLanguage);

            // set the user selected language
            if (cboLanguage.Items.Count > 0)
            {
                foreach (object oItem in cboLanguage.Items)
                {
                    LanguageItem item = (LanguageItem)oItem;
                    if (item.Culture.Equals(GeneralSettings.Language, StringComparison.OrdinalIgnoreCase))
                    {
                        cboLanguage.SelectedItem = item;
                        break;
                    }
                }
            }

            // setup the performance drop down list
            List<EncodingPerformance> performanceValues = new List<EncodingPerformance>();
            Array values = Enum.GetValues(typeof(EncodingOptions.Performance));
            foreach (EncodingOptions.Performance val in values)
            {
                performanceValues.Add(new EncodingPerformance() { Name = Language.GetGenericText(string.Format("FileEncoding.Performance.{0}", Enum.GetName(typeof(EncodingOptions.Performance), val))), Value = (int)val });
            }
            cboPerformance.DisplayMember = "Name";
            cboPerformance.ValueMember = "Value";
            cboPerformance.DataSource = performanceValues;
            cboPerformance.SelectedValue = GeneralSettings.EncodingPerformance;
            chkDetectFileEncoding_CheckedChanged(null, null);

            // set column text
            TextEditorsList.Columns[0].Text = Language.GetGenericText("TextEditorsColumnFileType");
            TextEditorsList.Columns[1].Text = Language.GetGenericText("TextEditorsColumnLocation");
            TextEditorsList.Columns[2].Text = Language.GetGenericText("TextEditorsColumnCmdLine");
            TextEditorsList.Columns[3].Text = Language.GetGenericText("TextEditorsColumnTabSize");
            PluginsList.Columns[0].Text = Language.GetGenericText("PluginsColumnEnabled");
            PluginsList.Columns[1].Text = Language.GetGenericText("PluginsColumnName");
            PluginsList.Columns[2].Text = Language.GetGenericText("PluginsColumnExtensions");
            lstFiles.Columns[0].Text = Language.GetGenericText("FileEncoding.Enabled", "Enabled");
            lstFiles.Columns[1].Text = Language.GetGenericText("FileEncoding.FilePath", "File Path");
            lstFiles.Columns[2].Text = Language.GetGenericText("FileEncoding.Encoding", "Encoding");

            // set column widths
            TextEditorsList.Columns[0].Width = Constants.OPTIONS_TEXT_EDITOR_COLUMN_0_WIDTH * GeneralSettings.WindowsDPIPerCentSetting / 100;
            TextEditorsList.Columns[1].Width = Constants.OPTIONS_TEXT_EDITOR_COLUMN_1_WIDTH * GeneralSettings.WindowsDPIPerCentSetting / 100;
            TextEditorsList.Columns[2].Width = Constants.OPTIONS_TEXT_EDITOR_COLUMN_2_WIDTH * GeneralSettings.WindowsDPIPerCentSetting / 100;
            TextEditorsList.Columns[3].Width = Constants.OPTIONS_TEXT_EDITOR_COLUMN_3_WIDTH * GeneralSettings.WindowsDPIPerCentSetting / 100;
            PluginsList.Columns[0].Width = Constants.OPTIONS_PLUGINS_COLUMN_0_WIDTH * GeneralSettings.WindowsDPIPerCentSetting / 100;
            PluginsList.Columns[1].Width = Constants.OPTIONS_PLUGINS_COLUMN_1_WIDTH * GeneralSettings.WindowsDPIPerCentSetting / 100;
            PluginsList.Columns[2].Width = Constants.OPTIONS_PLUGINS_COLUMN_2_WIDTH * GeneralSettings.WindowsDPIPerCentSetting / 100;
            lstFiles.Columns[0].Width = Constants.OPTIONS_FILES_COLUMN_0_WIDTH * GeneralSettings.WindowsDPIPerCentSetting / 100;
            lstFiles.Columns[1].Width = Constants.OPTIONS_FILES_COLUMN_1_WIDTH * GeneralSettings.WindowsDPIPerCentSetting / 100;
            lstFiles.Columns[2].Width = Constants.OPTIONS_FILES_COLUMN_2_WIDTH * GeneralSettings.WindowsDPIPerCentSetting / 100;

            chkUseDataBase.Checked = GeneralSettings.UseDataBase;
            chkIntegritySecurity.Checked = GeneralSettings.UseIntegritySecurity;
            cboDataBase.Text = GeneralSettings.Database;
            cboServer.Text = GeneralSettings.Server;
            txtLogin.Text = GeneralSettings.Login;
            txtPassword.Text = GeneralSettings.Password;

            if (chkUseDataBase.Checked)
            {
                chkIntegritySecurity.Enabled = true;
                cboDataBase.Enabled = true;
                cboServer.Enabled = true;
                txtLogin.Enabled = true;
                txtPassword.Enabled = true;
            }
            else
            {
                chkIntegritySecurity.Enabled = false;
                cboDataBase.Enabled = false;
                cboServer.Enabled = false;
                txtLogin.Enabled = false;
                txtPassword.Enabled = false;
            }
        }

        #region Private Methods
        /// <summary>
        /// Update the results preview.
        /// </summary>

        private void UpdateResultsPreview()
        {
            string PREVIEW_TEXT = Language.GetGenericText("ResultsPreviewText");
            string PREVIEW_SPACER_TEXT = Language.GetGenericText("ResultsPreviewSpacerText");
            string PREVIEW_MATCH_TEXT = Language.GetGenericText("ResultsPreviewTextMatch");

            string _textToSearch = string.Empty;
            string _searchText = PREVIEW_MATCH_TEXT;
            string _tempLine = string.Empty;

            string _begin = string.Empty;
            string _text = string.Empty;
            string _end = string.Empty;
            int _pos = 0;
            bool _highlight = false;

            if (tbcOptions.SelectedTab == tabResults)
            {
                // Clear the contents
                rtxtResultsPreview.Text = string.Empty;
                rtxtResultsPreview.ForeColor = btnResultsWindowForeColor.SelectedColor;
                rtxtResultsPreview.BackColor = btnResultsWindowBackColor.SelectedColor;

                // Retrieve hit text
                _textToSearch = PREVIEW_TEXT;

                _textToSearch = string.Format("{0}{1}", PREVIEW_SPACER_TEXT, _textToSearch);

                // Set default font
                rtxtResultsPreview.SelectionFont = rtxtResultsPreview.Font;

                _tempLine = _textToSearch;

                // attempt to locate the text in the line
                _pos = _tempLine.IndexOf(_searchText);

                if (_pos > -1)
                {
                    do
                    {
                        _highlight = false;

                        //
                        // retrieve parts of text
                        _begin = _tempLine.Substring(0, _pos);
                        _text = _tempLine.Substring(_pos, _searchText.Length);
                        _end = _tempLine.Substring(_pos + _searchText.Length);

                        // set default color for starting text
                        rtxtResultsPreview.SelectionColor = btnResultsWindowForeColor.SelectedColor;
                        rtxtResultsPreview.SelectionBackColor = btnResultsWindowBackColor.SelectedColor;
                        rtxtResultsPreview.SelectedText = _begin;

                        // do a check to see if begin and end are valid for wholeword searches
                        _highlight = true;

                        // set highlight color for searched text
                        if (_highlight)
                        {
                            rtxtResultsPreview.SelectionColor = ForeColorButton.SelectedColor;
                            rtxtResultsPreview.SelectionBackColor = BackColorButton.SelectedColor;
                        }
                        rtxtResultsPreview.SelectedText = _text;

                        // Check remaining string for other hits in same line
                        _pos = _end.IndexOf(_searchText);

                        // set default color for end, if no more hits in line
                        _tempLine = _end;
                        if (_pos < 0)
                        {
                            rtxtResultsPreview.SelectionColor = btnResultsWindowForeColor.SelectedColor;
                            rtxtResultsPreview.SelectionBackColor = btnResultsWindowBackColor.SelectedColor;
                            if (_end.Length > 0)
                            {
                                rtxtResultsPreview.SelectedText = _end;
                            }
                        }

                    } while (_pos > -1);
                }
                else
                {
                    // set default color, no search text found
                    rtxtResultsPreview.SelectionColor = btnResultsWindowForeColor.SelectedColor;
                    rtxtResultsPreview.SelectionBackColor = btnResultsWindowBackColor.SelectedColor;
                    rtxtResultsPreview.SelectedText = _textToSearch;
                }
            }
        }

        /// <summary>
        /// Handle when a new color has been selected.
        /// </summary>
        /// <param name="newColor">new Color</param>

        private void NewColor(Color newColor)
        {
            UpdateResultsPreview();
        }

        /// <summary>
        /// Loads the given text editors.
        /// </summary>
        /// <param name="editors">TextEditor array, can be nothing</param>

        private void LoadEditors(TextEditor[] editors)
        {
            if (editors != null)
            {
                TextEditorsList.BeginUpdate();
                foreach (TextEditor editor in editors)
                {
                    ListViewItem item = new ListViewItem();
                    item.Text = editor.FileType;
                    item.SubItems.Add(editor.Editor);
                    item.SubItems.Add(editor.Arguments);
                    item.SubItems.Add(editor.TabSize.ToString());
                    item.Selected = true;
                    item.Tag = editor;
                    TextEditorsList.Items.Add(item);
                }
                TextEditorsList.EndUpdate();
            }
        }

        /// <summary>
        /// Saves the defined text editors.
        /// </summary>

        private void SaveEditors()
        {
            if (TextEditorsList.Items.Count == 0)
            {
                TextEditors.Save(null);
                return;
            }

            TextEditor[] editors = new TextEditor[TextEditorsList.Items.Count];
            int index = 0;
            foreach (ListViewItem item in TextEditorsList.Items)
            {
                editors[index] = item.Tag as TextEditor;
                index += 1;
            }

            TextEditors.Save(editors);
        }

        /// <summary>
        /// Sets the TextEditor's button states depending on if one is selected.
        /// </summary>

        private void SetTextEditorsButtonState()
        {
            if (TextEditorsList.SelectedItems.Count > 0)
            {
                btnRemove.Enabled = true;
                btnEdit.Enabled = true;
            }
            else
            {
                btnRemove.Enabled = false;
                btnEdit.Enabled = false;
            }
        }

        /// <summary>
        /// Determines if a text editor is defined for all file types.
        /// </summary>
        /// <returns>Returns true if an all file types text editor is defined, false otherwise</returns>

        private bool IsAllTypesDefined()
        {
            foreach (ListViewItem item in TextEditorsList.Items)
            {
                if (item.Text.Equals("*"))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Retrieves an array of file types currently defined.
        /// </summary>
        /// <returns>String array of file types</returns>

        private List<string> GetExistingFileTypes()
        {
            List<string> types = new List<string>();

            for (int i = 0; i < TextEditorsList.Items.Count; i++)
            {
                string value = TextEditorsList.Items[i].Text;
                if (value.Contains(Constants.TEXT_EDITOR_TYPE_SEPARATOR))
                {
                    foreach (string val in value.Split(Constants.TEXT_EDITOR_TYPE_SEPARATOR.ToCharArray(), StringSplitOptions.RemoveEmptyEntries))
                    {
                        types.Add(val.ToLower());
                    }
                }
                else
                {
                    types.Add(value.ToLower());
                }
            }

            return types;
        }

        /// <summary>
        /// Displays the given font as a string on the given label.
        /// </summary>
        /// <param name="fnt">Font to display</param>
        /// <param name="lbl">Label to show font</param>

        private void DisplayFont(Font fnt, Label lbl)
        {
            lbl.Text = string.Format("{0} / {1} / {2}", fnt.Name, fnt.SizeInPoints, fnt.Style.ToString());
        }
        #endregion

        #region Control Events
        /// <summary>
        /// Handles saving the user specified options.
        /// </summary>
        /// <param name="sender">System parameter</param>
        /// <param name="e">System parameter</param>

        private void btnOK_Click(object sender, System.EventArgs e)
        {
            // Store the values in the globals
            GeneralSettings.MaximumMRUPaths = cboPathMRUCount.SelectedIndex + 1;
            GeneralSettings.HighlightForeColor = Convertors.ConvertColorToString(ForeColorButton.SelectedColor);
            GeneralSettings.HighlightBackColor = Convertors.ConvertColorToString(BackColorButton.SelectedColor);
            GeneralSettings.ResultsForeColor = Convertors.ConvertColorToString(btnResultsWindowForeColor.SelectedColor);
            GeneralSettings.ResultsBackColor = Convertors.ConvertColorToString(btnResultsWindowBackColor.SelectedColor);
            GeneralSettings.ResultsContextForeColor = Convertors.ConvertColorToString(btnResultsContextForeColor.SelectedColor);
            GeneralSettings.ResultsFont = Convertors.ConvertFontToString(rtxtResultsPreview.Font);
            GeneralSettings.ShowExclusionErrorMessage = chkShowExclusionErrorMessage.Checked;
            GeneralSettings.SaveSearchOptionsOnExit = chkSaveSearchOptions.Checked;
            GeneralSettings.FilePanelFont = Convertors.ConvertFontToString(__FileFont);
            GeneralSettings.DetectFileEncoding = chkDetectFileEncoding.Checked;
            GeneralSettings.EncodingPerformance = (int)cboPerformance.SelectedValue;
            GeneralSettings.UseEncodingCache = chkUseEncodingCache.Checked;
            GeneralSettings.LogDisplaySavePosition = chkSaveMessagesPosition.Checked;
            GeneralSettings.ExclusionsDisplaySavePosition = chkSaveExclusionsPosition.Checked;
            GeneralSettings.UsebSearchAccentColor = chkLabelColor.Checked;

            // set Server SQL information
            GeneralSettings.UseDataBase = chkUseDataBase.Checked;
            GeneralSettings.UseIntegritySecurity = chkIntegritySecurity.Checked;
            GeneralSettings.Login = txtLogin.Text;
            GeneralSettings.Password = txtPassword.Text;
            GeneralSettings.Server = cboServer.Text;
            GeneralSettings.Database = cboDataBase.Text;

            // set default log display positions if save position is disabled
            if (!GeneralSettings.LogDisplaySavePosition)
            {
                // set log display window and column positions to default values
                GeneralSettingsReset.LogDisplaySetDefaultPositions();
            }

            // set default exclusions display positions if save position is disabled
            if (!GeneralSettings.ExclusionsDisplaySavePosition)
            {
                // set exclusions display window and column positions to default values
                GeneralSettingsReset.ExclusionsDisplaySetDefaultPositions();
            }

            // Only load new language on a change
            LanguageItem item = (LanguageItem)cboLanguage.SelectedItem;
            if (!GeneralSettings.Language.Equals(item.Culture))
            {
                GeneralSettings.Language = item.Culture;
                Language.Load(GeneralSettings.Language);
                __LanguageChange = true;
            }

            // set shortcuts
            if (!Registry.IsInstaller())
            {
                Shortcuts.SetDesktopShortcut(chkDesktopShortcut.Checked);
                Shortcuts.SetStartMenuShortcut(chkStartMenuShortcut.Checked);
            }

            SaveEditors();

            SaveFileEncodings();

            Core.PluginManager.Save();

            // handle right click search change
            if (__RightClickUpdate)
            {
                try
                {
                    string path = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Application.ExecutablePath), "bSearch.AdminProcess.exe");
                    string agPath = string.Format("\"{0}\"", Application.ExecutablePath);
                    string explorerText = string.Format("\"{0}\"", Language.GetGenericText("SearchExplorerItem"));
                    string args = string.Format("\"{0}\" {1} {2}", chkRightClickOption.Checked.ToString(), agPath, explorerText);

                    API.UACHelper.AttemptPrivilegeEscalation(path, args, false);
                }
                catch (Exception ex)
                {
                    // TODO
                    System.Diagnostics.Debug.WriteLine(ex.Message);
                }
            }

            this.Close();
        }

        /// <summary>
        /// Closes the form
        /// </summary>
        /// <param name="sender">System parameter</param>
        /// <param name="e">System parameter</param>

        private void btnCancel_Click(object sender, System.EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Add a new text editor.
        /// </summary>
        /// <param name="sender">system parameter</param>
        /// <param name="e">system parameter</param>

        private void btnAdd_Click(object sender, System.EventArgs e)
        {
            if (tbcOptions.SelectedTab == tabTextEditors)
            {
                frmAddEditTextEditor dlg = new frmAddEditTextEditor();
                dlg.IsAdd = true;
                dlg.IsAllTypesDefined = IsAllTypesDefined();
                dlg.ExistingFileTypes = GetExistingFileTypes();

                if (dlg.ShowDialog(this) == DialogResult.OK)
                {
                    // create new entry
                    ListViewItem item = new ListViewItem();
                    item.Tag = dlg.Editor;
                    item.Text = dlg.Editor.FileType;
                    item.SubItems.Add(dlg.Editor.Editor);
                    item.SubItems.Add(dlg.Editor.Arguments);
                    item.SubItems.Add(dlg.Editor.TabSize.ToString());
                    TextEditorsList.Items.Add(item);

                    SetTextEditorsButtonState();
                }
            }

            this.DialogResult = DialogResult.None;
        }

        /// <summary>
        /// Edit the selected text editor.
        /// </summary>
        /// <param name="sender">system parameter</param>
        /// <param name="e">system parameter</param>

        private void btnEdit_Click(object sender, System.EventArgs e)
        {
            if (tbcOptions.SelectedTab == tabTextEditors)
            {
                if (TextEditorsList.SelectedItems.Count > 0)
                {
                    ListViewItem item = TextEditorsList.SelectedItems[0];
                    frmAddEditTextEditor dlg = new frmAddEditTextEditor();

                    // set values
                    dlg.IsAdd = false;
                    dlg.IsAllTypesDefined = IsAllTypesDefined();
                    dlg.Editor = item.Tag as TextEditor;
                    dlg.ExistingFileTypes = GetExistingFileTypes();

                    if (dlg.ShowDialog(this) == DialogResult.OK)
                    {
                        // get values
                        TextEditorsList.SelectedItems[0].Tag = dlg.Editor;
                        TextEditorsList.SelectedItems[0].Text = dlg.Editor.FileType;
                        TextEditorsList.SelectedItems[0].SubItems[1].Text = dlg.Editor.Editor;
                        TextEditorsList.SelectedItems[0].SubItems[2].Text = dlg.Editor.Arguments;
                        TextEditorsList.SelectedItems[0].SubItems[3].Text = dlg.Editor.TabSize.ToString();
                    }

                    SetTextEditorsButtonState();
                }
            }

            this.DialogResult = DialogResult.None;
        }

        /// <summary>
        /// Delete the selected text editor.
        /// </summary>
        /// <param name="sender">system parameter</param>
        /// <param name="e">system parameter</param>

        private void btnRemove_Click(object sender, System.EventArgs e)
        {
            if (tbcOptions.SelectedTab == tabTextEditors)
            {
                // remove
                if (TextEditorsList.SelectedItems.Count > 0)
                {
                    TextEditorsList.Items.Remove(TextEditorsList.SelectedItems[0]);
                    SetTextEditorsButtonState();
                }
            }

            this.DialogResult = DialogResult.None;
        }

        /// <summary>
        /// Update the text editor button states.
        /// </summary>
        /// <param name="sender">system parameter</param>
        /// <param name="e">system parameter</param>

        private void TextEditorsList_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            SetTextEditorsButtonState();
        }

        /// <summary>
        /// Edit the selected text editor entry.
        /// </summary>
        /// <param name="sender">system parameter</param>
        /// <param name="e">system parameter</param>

        private void TextEditorsList_DoubleClick(object sender, EventArgs e)
        {
            Point clientPoint = TextEditorsList.PointToClient(Control.MousePosition);
            ListViewItem item = TextEditorsList.GetItemAt(clientPoint.X, clientPoint.Y);

            if (item != null)
            {
                item.Selected = true;
                btnEdit_Click(null, null);
            }
        }

        /// <summary>
        /// Setup tab pages when selected.
        /// </summary>
        /// <param name="sender">system parameter</param>
        /// <param name="e">system parameter</param>

        private void tbcOptions_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            if (tbcOptions.SelectedTab == tabResults)
            {
                UpdateResultsPreview();
            }
            else if (tbcOptions.SelectedTab == tabPlugins)
            {
                if (PluginsList.Items.Count > 0)
                {
                    PluginsList.Items[0].Selected = true;
                }
            }
        }


        /// <summary>
        /// Show font selection dialog.
        /// </summary>
        /// <param name="sender">system parameter</param>
        /// <param name="e">system parameter</param>

        private void btnFindFont_Click(object sender, EventArgs e)
        {
            var dlg = new FontDialog()
            {
                ShowColor = false,
                ShowEffects = false,
                ShowHelp = false,
                Font = rtxtResultsPreview.Font
            };

            var result = dlg.ShowDialog(this);
            if (result == DialogResult.OK)
            {
                DisplayFont(dlg.Font, lblCurrentFont);
                rtxtResultsPreview.Font = dlg.Font;
            }
        }

        /// <summary>
        /// Show font selection dialog.
        /// </summary>
        /// <param name="sender">system parameter</param>
        /// <param name="e">system parameter</param>

        private void btnFileFindFont_Click(object sender, EventArgs e)
        {
            var dlg = new FontDialog()
            {
                ShowColor = false,
                ShowEffects = false,
                ShowHelp = false,
                Font = __FileFont
            };

            var result = dlg.ShowDialog(this);
            if (result == DialogResult.OK)
            {
                DisplayFont(dlg.Font, lblFileCurrentFont);
                __FileFont = dlg.Font;
            }
        }

        /// <summary>
        /// Handle change to the right click checkbox.
        /// </summary>
        /// <param name="sender">system parameter</param>
        /// <param name="e">system parameter</param>

        private void chkRightClickOption_CheckedChanged(object sender, EventArgs e)
        {
            if (chkRightClickOption.Checked != __RightClickEnabled)
            {
                if (!__IsAdmin)
                {
                    API.UACHelper.AddShieldToButton(btnOK);
                }
                __RightClickUpdate = true;
            }
            else
            {
                if (!__IsAdmin)
                {
                    API.UACHelper.RemoveShieldFromButton(btnOK);
                }
                __RightClickUpdate = false;
            }
        }

        /// <summary>
        /// Update results preview display when remove leading white space checkbox changes.
        /// </summary>
        /// <param name="sender">system parameter</param>
        /// <param name="e">system parameter</param>

        private void chkRemoveLeadingWhiteSpace_CheckedChanged(object sender, EventArgs e)
        {
            UpdateResultsPreview();
        }

        private void btnGeneralFindFont_Click(object sender, EventArgs e)
        {

        }

        #endregion



        #region ServerSQL Methods

        private DataTable GetInstances()
        {
            SqlDataSourceEnumerator instance = SqlDataSourceEnumerator.Instance;
            DataTable dtInstancesList = new DataTable();
            dtInstancesList = instance.GetDataSources().Select("", "ServerName ASC").CopyToDataTable();

            return dtInstancesList;
        }


        private List<string> GetDatabaseList()
        {
            List<string> list = new List<string>();

            string conString = "";

            if (this.chkIntegritySecurity.Checked)
            {
                conString = "Data Source = " + this.cboServer.Text + ";Initial Catalog=" + "master" + ";Integrated Security=True";

            }
            else
            {

                conString = "Data Source=" + this.cboServer.Text + ";Initial Catalog=" + "master" +
                    ";Persist Security Info=True;User ID=" + this.txtLogin.Text + ";Password=" + this.txtPassword.Text;
            }

            // Open connection to the database

            using (SqlConnection con = new SqlConnection(conString))
            {
                con.Open();

                // Set up a command with the given query and associate
                // this with the current connection.
                using (SqlCommand cmd = new SqlCommand("SELECT name from sys.databases WHERE UPPER([name]) NOT IN ('MASTER', 'MODEL', 'TEMPDB', 'MSDB')", con))
                {
                    using (IDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            list.Add(dr[0].ToString());
                        }
                    }
                }
            }
            return list;

        }


        private void TestConnection()
        {

            string conString = "";

            if (this.chkIntegritySecurity.Checked)
            {
                conString = "Data Source = " + this.cboServer.Text + ";Initial Catalog=" + "master" + ";Integrated Security=True";

            }
            else
            {

                conString = "Data Source=" + this.cboServer.Text + ";Initial Catalog=" + "master" +
                    ";Persist Security Info=True;User ID=" + this.txtLogin.Text + ";Password=" + this.txtPassword.Text;
            }

            // Open connection to the database
            using (SqlConnection con = new SqlConnection(conString))
            {
                con.Open();

                if (con.State == ConnectionState.Open) // if connection.Open was successful
                {
                    MessageBox.Show("You have been successfully connected to the database!");
                }
                else
                {
                    MessageBox.Show("Connection failed.");
                }

            }

        }


        #endregion

        #region Plugin Methods
        /// <summary>
        /// Load the plugins from the manager to the listview.
        /// </summary>
        /// <param name="selectedIndex">Set to index to show selected</param>

        private void LoadPlugins(int selectedIndex = -1)
        {
            PluginsList.Items.Clear();
            ListViewItem item;

            for (int i = 0; i < Core.PluginManager.Items.Count; i++)
            {
                item = new ListViewItem();
                item.Checked = Core.PluginManager.Items[i].Enabled;
                item.SubItems.Add(Core.PluginManager.Items[i].Plugin.Name);
                item.SubItems.Add(Core.PluginManager.Items[i].Plugin.Extensions);
                if (selectedIndex > -1 && selectedIndex == i)
                {
                    item.Selected = true;
                }
                PluginsList.Items.Add(item);
            }
        }

        /// <summary>
        /// Display the selected plugin details.
        /// </summary>
        /// <param name="sender">system parameter</param>
        /// <param name="e">system parameter</param>

        private void PluginsList_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            if (PluginsList.SelectedItems.Count > 0)
                LoadPluginDetails(Core.PluginManager.Items[PluginsList.SelectedItems[0].Index].Plugin);
            else
                ClearPluginDetails();
        }

        /// <summary>
        /// Enable or disable the selected plugin.
        /// </summary>
        /// <param name="sender">system parameter</param>
        /// <param name="e">system parameter</param>

        private void PluginsList_ItemCheck(object sender, System.Windows.Forms.ItemCheckEventArgs e)
        {
            if (e.Index > -1 && e.Index < PluginsList.Items.Count)
            {
                PluginsList.Items[e.Index].Selected = true;
                if (e.NewValue == CheckState.Checked)
                    Core.PluginManager.Items[e.Index].Enabled = true;
                else
                    Core.PluginManager.Items[e.Index].Enabled = false;
            }
        }

        /// <summary>
        /// Clear the plugin details.
        /// </summary>

        private void ClearPluginDetails()
        {
            lblPluginName.Text = string.Empty;
            lblPluginVersion.Text = string.Empty;
            lblPluginAuthor.Text = string.Empty;
            lblPluginDescription.Text = string.Empty;
        }

        /// <summary>
        /// Display the plugin details.
        /// </summary>
        /// <param name="plugin">IbSearchPlugin to load</param>

        private void LoadPluginDetails(IbSearchPlugin plugin)
        {
            lblPluginName.Text = plugin.Name;
            lblPluginVersion.Text = plugin.Version;
            lblPluginAuthor.Text = plugin.Author;
            lblPluginDescription.Text = plugin.Description;
        }

        /// <summary>
        /// Moves the selected plugin up in the list.
        /// </summary>
        /// <param name="sender">system parameter</param>
        /// <param name="e">system parameter</param>

        private void btnUp_Click(object sender, EventArgs e)
        {
            // move selected plugin up in list
            if (PluginsList.SelectedItems.Count > 0 && PluginsList.SelectedItems[0].Index != 0)
            {
                Core.PluginManager.Items.Reverse(PluginsList.SelectedItems[0].Index - 1, 2);
                LoadPlugins(PluginsList.SelectedItems[0].Index - 1);
            }
        }

        /// <summary>
        /// Moves the selected plugin down in the list.
        /// </summary>
        /// <param name="sender">system parameter</param>
        /// <param name="e">system parameter</param>

        private void btnDown_Click(object sender, EventArgs e)
        {
            // move selected plugin down in list
            if (PluginsList.SelectedItems.Count > 0 && PluginsList.SelectedItems[0].Index != (PluginsList.Items.Count - 1))
            {
                Core.PluginManager.Items.Reverse(PluginsList.SelectedItems[0].Index, 2);
                LoadPlugins(PluginsList.SelectedItems[0].Index + 1);
            }
        }
        #endregion

        #region File Encoding Methods

        private void LoadFileEncodings()
        {
            var fileEncodings = FileEncoding.ConvertStringToFileEncodings(GeneralSettings.FileEncodings);
            if (fileEncodings != null && fileEncodings.Count > 0)
            {
                lstFiles.BeginUpdate();

                foreach (var file in fileEncodings)
                {
                    var item = GetFileEncodingListViewItem(file);
                    lstFiles.Items.Add(item);
                }

                lstFiles.EndUpdate();
            }

            SetFileEncodingButtonState();
        }

        private void SaveFileEncodings()
        {
            string encodings = string.Empty;

            if (lstFiles.Items.Count > 0)
            {
                var fileEncodings = new List<FileEncoding>();
                foreach (ListViewItem item in lstFiles.Items)
                {
                    var encoding = item.Tag as FileEncoding;
                    encoding.Enabled = item.Checked;
                    fileEncodings.Add(encoding);
                }

                encodings = FileEncoding.ConvertFileEncodingsToString(fileEncodings);
            }

            GeneralSettings.FileEncodings = encodings;
        }

        private void btnFileEncodingAdd_Click(object sender, EventArgs e)
        {
            var dialog = new frmAddEditFileEncoding();
            if (dialog.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
            {
                lstFiles.Items.Add(GetFileEncodingListViewItem(dialog.SelectedFileEncoding));

                SetFileEncodingButtonState();
            }

            this.DialogResult = System.Windows.Forms.DialogResult.None;
        }

        private void btnFileEncodingEdit_Click(object sender, EventArgs e)
        {
            if (lstFiles.SelectedItems.Count > 0)
            {
                // get currently selected exclusion
                var item = lstFiles.SelectedItems[0].Tag as FileEncoding;
                item.Enabled = lstFiles.SelectedItems[0].Checked;

                var dialog = new frmAddEditFileEncoding();
                dialog.SelectedFileEncoding = item;
                if (dialog.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
                {
                    item = dialog.SelectedFileEncoding;
                    var listItem = GetFileEncodingListViewItem(item);
                    lstFiles.SelectedItems[0].Checked = item.Enabled;
                    lstFiles.SelectedItems[0].Tag = item;

                    lstFiles.SelectedItems[0].SubItems[1].Text = listItem.SubItems[1].Text;
                    lstFiles.SelectedItems[0].SubItems[2].Text = listItem.SubItems[2].Text;

                    SetFileEncodingButtonState();
                }
            }

            this.DialogResult = System.Windows.Forms.DialogResult.None;
        }

        private void btnFileEncodingDelete_Click(object sender, EventArgs e)
        {
            // remove
            if (lstFiles.SelectedItems.Count > 0)
            {
                foreach (ListViewItem item in lstFiles.SelectedItems)
                {
                    lstFiles.Items.Remove(item);
                }
                SetFileEncodingButtonState();
            }

            this.DialogResult = DialogResult.None;
        }

        /// <summary>
        /// Sets the FileEncoding's button states depending on if one is selected.
        /// </summary>

        private void SetFileEncodingButtonState()
        {
            if (lstFiles.SelectedItems.Count > 0)
            {
                btnFileEncodingDelete.Enabled = true;
                btnFileEncodingEdit.Enabled = true;
            }
            else
            {
                btnFileEncodingDelete.Enabled = false;
                btnFileEncodingEdit.Enabled = false;
            }
        }

        /// <summary>
        /// Get the list view item from the given FileEncoding object.
        /// </summary>
        /// <param name="item">FileEncoding object</param>
        /// <returns>ListViewItem object</returns>

        private ListViewItem GetFileEncodingListViewItem(FileEncoding item)
        {
            ListViewItem listItem = new ListViewItem();
            listItem.Tag = item;
            listItem.Checked = item.Enabled;
            listItem.SubItems.Add(item.FilePath);
            listItem.SubItems.Add(System.Text.Encoding.GetEncoding(item.CodePage).EncodingName);

            return listItem;
        }

        /// <summary>
        /// Update the button states.
        /// </summary>
        /// <param name="sender">system parameter</param>
        /// <param name="e">system parameter</param>

        private void lstFiles_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            SetFileEncodingButtonState();
        }

        /// <summary>
        /// Edit the selected file entry.
        /// </summary>
        /// <param name="sender">system parameter</param>
        /// <param name="e">system parameter</param>

        private void lstFiles_DoubleClick(object sender, EventArgs e)
        {
            Point clientPoint = lstFiles.PointToClient(Control.MousePosition);
            ListViewItem item = lstFiles.GetItemAt(clientPoint.X, clientPoint.Y);

            if (item != null)
            {
                item.Selected = true;
                btnFileEncodingEdit_Click(null, null);
            }
        }

        /// <summary>
        /// Handles the key down event (supports ctrl-a, del).
        /// </summary>
        /// <param name="sender">system parameter</param>
        /// <param name="e">system parameter</param>

        private void lstFiles_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (e.KeyCode == Keys.A && e.Control) //ctrl+a  Select All
            {
                foreach (ListViewItem item in lstFiles.Items)
                {
                    item.Selected = true;
                }
            }

            if (e.KeyCode == Keys.Delete) //delete
            {
                btnFileEncodingDelete_Click(sender, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Handle not changing checked state of item when double clicking to edit it.
        /// </summary>
        /// <param name="sender">system parameter</param>
        /// <param name="e">system parameter</param>

        private void lstFiles_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            inhibitFileEncodingAutoCheck = true;
        }

        /// <summary>
        /// Handle not changing checked state of item when double clicking to edit it.
        /// </summary>
        /// <param name="sender">system parameter</param>
        /// <param name="e">system parameter</param>

        private void lstFiles_ItemCheck(object sender, System.Windows.Forms.ItemCheckEventArgs e)
        {
            if (inhibitFileEncodingAutoCheck)
            {
                e.NewValue = e.CurrentValue;
            }
        }

        /// <summary>
        /// Handle not changing checked state of item when double clicking to edit it.
        /// </summary>
        /// <param name="sender">system parameter</param>
        /// <param name="e">system parameter</param>

        private void lstFiles_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            inhibitFileEncodingAutoCheck = false;
        }

        /// <summary>
        /// Handles ListView Column Click event to allow Enabled column to toggle all checkboxes.
        /// </summary>
        /// <param name="sender">lstFiles listview</param>
        /// <param name="e">column click arguments</param>

        private void lstFiles_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            // enabled column
            if (e.Column == 0)
            {
                bool allChecked = (lstFiles.CheckedItems.Count == lstFiles.Items.Count);
                foreach (ListViewItem item in lstFiles.Items)
                {
                    item.Checked = !allChecked;
                }
            }
        }

        /// <summary>
        /// Enable/Disable settings based on if encoding detection is enabled.
        /// </summary>
        /// <param name="sender">system parameter</param>
        /// <param name="e">system parameter</param>

        private void chkDetectFileEncoding_CheckedChanged(object sender, EventArgs e)
        {
            lblPerformance.Enabled = cboPerformance.Enabled = chkUseEncodingCache.Enabled = btnCacheClear.Enabled = chkDetectFileEncoding.Checked;
        }

        /// <summary>
        /// Clears the encoding cache.
        /// </summary>
        /// <param name="sender">system parameter</param>
        /// <param name="e">system parameter</param>

        private void btnCacheClear_Click(object sender, EventArgs e)
        {
            libbSearch.EncodingDetection.Caching.EncodingCache.Instance.Clear(true);

            MessageBox.Show(this, Language.GetGenericText("FileEncoding.CacheCleared", "Cache cleared successfully."), ProductInformation.ApplicationName, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>
        /// Used to display encoding performance enum values.
        /// </summary>
        internal class EncodingPerformance
        {
            /// <summary>Display name of performance value</summary>
            public string Name { get; set; }

            /// <summary>Performance enum value</summary>
            public int Value { get; set; }
        }

        #endregion


        private void cboServer_DropDown(object sender, EventArgs e)
        {
            cboServer.DataSource = GetInstances();
            cboServer.DisplayMember = "ServerName";
            //cboServer.ValueMember = "ServerName";
            //cboServer.DropDownWidth = CalculateDropDownWidth(cboServer);

        }

        private void cboDataBase_DropDown(object sender, EventArgs e)
        {
            cboDataBase.DataSource = GetDatabaseList();
            cboDataBase.DisplayMember = "name";
            //cboDataBase.BindingContext = this.BindingContext;
            //cboDataBase.DropDownWidth = CalculateDropDownWidth(cboDataBase);

        }

        private void chkUseDataBase_CheckedChanged(object sender, EventArgs e)
        {
            if (chkUseDataBase.Checked)
            {
                chkIntegritySecurity.Enabled = true;
                cboDataBase.Enabled = true;
                cboServer.Enabled = true;
                txtLogin.Enabled = true;
                txtPassword.Enabled = true;
            }
            else
            {
                chkIntegritySecurity.Enabled = false;
                cboDataBase.Enabled = false;
                cboServer.Enabled = false;
                txtLogin.Enabled = false;
                txtPassword.Enabled = false;
            }
        }

        private void btnTest_Click(object sender, EventArgs e)
        {
            TestConnection();
        }

    }
}
