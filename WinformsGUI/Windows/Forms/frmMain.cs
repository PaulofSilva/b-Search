using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.IO;
using System.Data;
using System.Data.SqlClient;
using System.Data.Sql;
using System.Configuration;

using bSearch.Common;
using bSearch.Common.Logging;
using bSearch.Core;
using bSearch.Output;
using bSearch.Windows.Controls;
using libbSearch;
using libbSearch.EncodingDetection;

namespace bSearch.Windows.Forms
{
    /// <summary>
    /// Main Form
    /// </summary>
    /// <remarks>
    ///   b-Search File Searching Utility.
    ///   Copyright (C) 2006 BigLevel Lda.
    /// 
    ///   The author may be contacted at:
    ///   suporte@bigLevel.pt or comercial@biglevel.pt
    /// </remarks>

    public partial class frmMain : BaseForm
    {
        #region Declarations

        private int __SortColumn = -1;
        private Grep __Grep = null;
        private int __FileListHeight = GeneralSettings.DEFAULT_FILE_PANEL_HEIGHT;
        private readonly List<LogItem> LogItems = new List<LogItem>();
        private List<FilterItem> __FilterItems = new List<FilterItem>();
        private CommandLineProcessing.CommandLineArguments __CommandLineArgs = new CommandLineProcessing.CommandLineArguments();
        private long StartingTime = 0;

        private System.ComponentModel.IContainer components;

        #endregion

        #region Delegate Declarations

        private delegate void UpdateHitCountCallBack(MatchResult match);
        private delegate void SetSearchStateCallBack(bool enable);
        private delegate void UpdateStatusMessageCallBack(string message);
        private delegate void UpdateStatusCountCallBack(int count);
        private delegate void CalculateTotalCountCallBack();
        private delegate void ClearItemsCallBack();
        private delegate void AddToListCallBack(FileInfo file, int index);
        private delegate void DisplaySearchMessagesCallBack(LogItem.LogItemTypes? displayType);
        private delegate void DisplayExclusionErrorMessagesCallBack();
        private delegate void RestoreTaskBarProgressCallBack();
        private delegate void ShowAllResultsCallBack();

        #endregion

        /// <summary>
        /// Gets/Sets the command line arguments
        /// </summary>

        public CommandLineProcessing.CommandLineArguments CommandLineArgs
        {
            get { return __CommandLineArgs; }
            set { __CommandLineArgs = value; }
        }

        /// <summary>
        /// Creates an instance of the frmMain class.
        /// </summary>

        public frmMain()
        {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            API.ListViewExtensions.SetTheme(lstFileNames);
        }

        #region Form Events
        /// <summary>
        /// Form Load Event
        /// </summary>
        /// <param name="sender">System parm</param>
        /// <param name="e">System parm</param>

        private void frmMain_Load(object sender, System.EventArgs e)
        {
            // Load Windows DPI settings and set window and column positions and fonts to defaults if setting has changed
            LoadDPISettings();

            // set defaults
            txtContextLines.Maximum = Constants.MAX_CONTEXT_LINES;

            // Load language
            //Language.GenerateXml(this, Application.StartupPath + "\\" + this.Name + ".xml");
            Language.ProcessForm(this, this.toolTip1);

            // Load the general settings
            Legacy.ConvertGeneralSettings();
            LoadSettings();

            // Load the search settings
            Legacy.ConvertSearchSettings();
            int line = Core.SearchSettings.ContextLines;
            LoadSearchSettings();

            // Make sure in Mono to set the command-line path
            if (CommandLineArgs.AnyArguments && CommandLineArgs.IsValidStartPath)
            {
                cboFilePath.Text = CommandLineArgs.StartPath;
            }

            // Delete registry entry (if exist)
            Legacy.DeleteRegistry();

            // Load plugins
            PluginManager.Load();

            // set view state of controls
            LoadViewStates();

            // Handle any command line arguments
            ProcessCommandLine();

            // set focus to search text combobox
            cboSearchForText.Select();

            // Enlarge font without changing font family
            lblSearchHeading.Font = new Font(lblSearchHeading.Font.FontFamily, 12F);
            lblSearchOptions.Font = new Font(lblSearchOptions.Font.FontFamily, 12F);
            if (!GeneralSettings.UsebSearchAccentColor)
            {
                lblSearchHeading.ForeColor = Color.Black;
                lblSearchOptions.ForeColor = Color.Black;
            }

            MainMenu.Font = this.Font;
            stbStatus.Font = this.Font;

        }

        /// <summary>
        /// Closed Event - Save settings and exit
        /// </summary>
        /// <param name="sender">System parm</param>
        /// <param name="e">System parm</param>

        private void frmMain_Closed(object sender, EventArgs e)
        {
            SaveSettings();

            if (GeneralSettings.SaveSearchOptionsOnExit)
            {
                SaveSearchSettings();
            }

            if (textElementHost != null)
            {
                textElementHost.Dispose();
            }

            LogClient.Instance.Logger.Info("### STOPPING {0}, version {1}{2} ###",
               ProductInformation.ApplicationName,
               ProductInformation.ApplicationVersion.ToString(3),
               ProductInformation.IsPortable ? " (Portable)" : string.Empty);

            Application.Exit();
        }

        /// <summary>
        /// Allows Ctrl-F keyboard event to set focus to search text field.
        /// </summary>
        /// <param name="msg">system parameter for system message</param>
        /// <param name="keyData">system parameter for keys pressed</param>
        /// <returns>true if processed, false otherwise</returns>

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == (Keys.Control | Keys.F))
            {
                cboSearchForText.Focus();
                return true;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }
        #endregion

        #region Control Events

        /// <summary>
        /// Resize the comboboxes when the main search panel is resized.
        /// </summary>
        /// <param name="sender">system parm</param>
        /// <param name="e">system parm</param>
        /// <remarks>
        ///   This is a workaround for a bug in the .NET 2003 combobox control
        ///   when it is set to ComboBoxStyle.DropDown that will select the text
        ///   in the control when it is resized.
        ///   By temporarily changing the style to ComboBoxStyle.Simple and manually
        ///   setting the width, we can avoid this annoying feature.
        /// </remarks>

        private void pnlSearch_SizeChanged(object sender, EventArgs e)
        {
            string temp = cboFilePath.Text;
            cboFilePath.DropDownStyle = ComboBoxStyle.DropDownList;
            cboFilePath.DropDownStyle = ComboBoxStyle.DropDown;
            cboFilePath.Text = temp;

            temp = cboFileName.Text;
            cboFileName.DropDownStyle = ComboBoxStyle.Simple;
            cboFileName.DropDownStyle = ComboBoxStyle.DropDown;
            cboFileName.Text = temp;
            
            temp = cboSearchForText.Text;
            cboSearchForText.DropDownStyle = ComboBoxStyle.Simple;
            cboSearchForText.DropDownStyle = ComboBoxStyle.DropDown;
            cboSearchForText.Text = temp;
        }

        /// <summary>
        /// Handles drawing bottom border line.
        /// </summary>
        /// <param name="sender">system parameter</param>
        /// <param name="e">system parameter</param>

        private void lblSearchOptions_Paint(object sender, PaintEventArgs e)
        {
            var rect = lblSearchOptions.ClientRectangle;
            rect.Height -= 1;

            e.Graphics.DrawLine(new Pen(GeneralSettings.UsebSearchAccentColor ? ProductInformation.ApplicationColor : Color.Black) { Width = 2 }, rect.X, rect.Height, rect.Width, rect.Height);
        }

        /// <summary>
        /// Handles drawing bottom border line.
        /// </summary>
        /// <param name="sender">system parameter</param>
        /// <param name="e">system parameter</param>

        private void lblSearchHeading_Paint(object sender, PaintEventArgs e)
        {
            var rect = lblSearchHeading.ClientRectangle;
            rect.Height -= 1;

            e.Graphics.DrawLine(new Pen(GeneralSettings.UsebSearchAccentColor ? ProductInformation.ApplicationColor : Color.Black) { Width = 2 }, rect.X, rect.Height, rect.Width, rect.Height);
        }

        /// <summary>
        /// Show the Search Exclusions dialog
        /// </summary>
        /// <param name="sender">System parm</param>
        /// <param name="e">System parm</param>

        private void lnkExclusions_LinkClicked(object sender, System.Windows.Forms.LinkLabelLinkClickedEventArgs e)
        {
            var dlg = new frmExclusions(__FilterItems);
            if (dlg.ShowDialog(this) == DialogResult.OK)
            {
                __FilterItems = dlg.FilterItems;
            }
        }

        /// <summary>
        /// Resize drop down list if necessary
        /// </summary>
        /// <param name="sender">system parm</param>
        /// <param name="e">system parm</param>

        private void cboSearchForText_DropDown(object sender, EventArgs e)
        {
            cboSearchForText.DropDownWidth = CalculateDropDownWidth(cboSearchForText);
        }

        /// <summary>
        /// Resize drop down list if necessary
        /// </summary>
        /// <param name="sender">system parm</param>
        /// <param name="e">system parm</param>

        private void cboFilePath_DropDown(object sender, EventArgs e)
        {
            cboFilePath.DropDownWidth = CalculateDropDownWidth(cboFilePath);
        }

        /// <summary>
        /// Resize drop down list if necessary
        /// </summary>
        /// <param name="sender">system parm</param>
        /// <param name="e">system parm</param>

        private void cboFileName_DropDown(object sender, EventArgs e)
        {
            cboFileName.DropDownWidth = CalculateDropDownWidth(cboFileName);
        }

        /// <summary>
        /// Negation check event
        /// </summary>
        /// <param name="sender">system parm</param>
        /// <param name="e">system parm</param>

        private void chkNegation_CheckedChanged(object sender, EventArgs e)
        {
            chkFileNamesOnly.Checked = chkNegation.Checked;
            chkFileNamesOnly.Enabled = !chkNegation.Checked;
        }

        /// <summary>
        /// File Names Only Check Box Event
        /// </summary>
        /// <param name="sender">System parm</param>
        /// <param name="e">System parm</param>

        private void chkFileNamesOnly_CheckedChanged(object sender, EventArgs e)
        {
            if (chkFileNamesOnly.Checked)
            {
                txtContextLines.Enabled = false;
                lblContextLines.Enabled = false;
            }
            else
            {
                txtContextLines.Enabled = true;
                lblContextLines.Enabled = true;
            }
        }

        /// <summary>
        /// Cancel Button Event
        /// </summary>
        /// <param name="sender">System parm</param>
        /// <param name="e">System parm</param>

        private void btnCancel_Click(object sender, System.EventArgs e)
        {
            if (__Grep != null)
                __Grep.Abort();
        }

        /// <summary>
        /// Search Button Event
        /// </summary>
        /// <param name="sender">System parm</param>
        /// <param name="e">System parm</param>

        private void btnSearch_Click(object sender, System.EventArgs e)
        {
            if (!VerifyInterface())
                return;

            //string temp = "^.*(" + cboSearchForText.Text + "|" + cboSearchForText2.Text + ").*$"; 
            //"(.*" + cboSearchForText.Text + "*.*." + cboSearchForText2.Text + ".*)";
            //cboSearchForText.Text = temp;

            StartSearch(chkSearchInResults.Checked);

        }

        /// <summary>
        /// File Name List Double Click Event
        /// </summary>
        /// <param name="sender">System parm</param>
        /// <param name="e">System parm</param>

        private void lstFileNames_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && e.Clicks == 2)
            {
                // Make sure there is something to click on
                if (lstFileNames.SelectedItems.Count == 0)
                    return;

                var match = __Grep.RetrieveMatchResult(int.Parse(lstFileNames.SelectedItems[0].SubItems[Constants.COLUMN_INDEX_GREP_INDEX].Text));
                TextEditors.Open(TextEditorOpener.FromMatch(match, __Grep.SearchSpec.SearchText));
            }
        }

        /// <summary>
        /// File Name List Select Index Change Event
        /// </summary>
        /// <param name="sender">System parm</param>
        /// <param name="e">System parm</param>

        private void lstFileNames_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            if (lstFileNames.SelectedItems.Count == 1)
            {
                // retrieve hit object
                MatchResult match = __Grep.RetrieveMatchResult(int.Parse(lstFileNames.SelectedItems[0].SubItems[Constants.COLUMN_INDEX_GREP_INDEX].Text));

                SetStatusBarEncoding(match.DetectedEncoding != null ? match.DetectedEncoding.EncodingName : string.Empty);

                if (EntireFileMenuItem.Checked)
                {
                    ProcessFileForDisplay(match);
                }
                else
                {
                    ProcessMatchForDisplay(match);
                }
            }

            if (lstFileNames.SelectedItems.Count == 0)
            {
                SetStatusBarEncoding(string.Empty);
                txtHits.Clear();
            }

        }

        /// <summary>
        /// Allow sorting of list view columns
        /// </summary>
        /// <param name="sender">System parm</param>
        /// <param name="e">System parm</param>

        private void lstFileNames_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            int clearColIndex = -1;

            // set to wait cursor
            lstFileNames.Cursor = Cursors.WaitCursor;

            // Determine whether the column is the same as the last column clicked.
            if (e.Column != __SortColumn)
            {
                // Remove sort indicator
                if (__SortColumn != -1)
                {
                    clearColIndex = __SortColumn;
                }

                // Set the sort column to the new column.
                __SortColumn = e.Column;

                // Set the sort order to ascending by default.
                if (e.Column == Constants.COLUMN_INDEX_COUNT ||
                   e.Column == Constants.COLUMN_INDEX_SIZE ||
                   e.Column == Constants.COLUMN_INDEX_DATE)
                {
                    lstFileNames.Sorting = System.Windows.Forms.SortOrder.Descending;
                }
                else
                {
                    lstFileNames.Sorting = System.Windows.Forms.SortOrder.Ascending;
                }
            }
            else
            {
                // Determine what the last sort order was and change it.
                if (lstFileNames.Sorting == System.Windows.Forms.SortOrder.Ascending)
                    lstFileNames.Sorting = System.Windows.Forms.SortOrder.Descending;
                else
                    lstFileNames.Sorting = System.Windows.Forms.SortOrder.Ascending;
            }

            // Set the ListViewItemSorter property to a new ListViewItemComparer object.
            ListViewItemComparer comparer = new ListViewItemComparer(e.Column, lstFileNames.Sorting);
            lstFileNames.ListViewItemSorter = comparer;

            // Call the sort method to manually sort.
            lstFileNames.Sort();

            // Display sort image and highlight sort column
            Windows.API.ListViewExtensions.SetSortIcon(lstFileNames, e.Column, lstFileNames.Sorting);

            // Apply theming since sorting removes it.
            Windows.API.ListViewExtensions.SetTheme(lstFileNames);

            // restore to default cursor
            lstFileNames.Cursor = Cursors.Default;
        }

        /// <summary>
        /// Handles setting up the drag event for a selected file.
        /// </summary>
        /// <param name="sender">system parameter</param>
        /// <param name="e">system parameter</param>

        private void lstFileNames_ItemDrag(object sender, ItemDragEventArgs e)
        {
            var lst = sender as ListView;
            var paths = new List<string>();

            foreach (ListViewItem item in lst.SelectedItems)
            {
                var path = item.SubItems[Constants.COLUMN_INDEX_DIRECTORY].Text +
                 Path.DirectorySeparatorChar +
                 item.SubItems[Constants.COLUMN_INDEX_FILE].Text;

                if (File.Exists(path))
                {
                    paths.Add(path);
                }
            }

            var dataObject = new DataObject(DataFormats.FileDrop, paths.ToArray());
            lstFileNames.DoDragDrop(dataObject, DragDropEffects.Copy);
        }

        /// <summary>
        /// Handles setting the form's AcceptButton to btnSearch so that enter key defaults to btnSearch for rest of form.
        /// </summary>
        /// <param name="sender">system parameter</param>
        /// <param name="e">system parameter</param>

        private void lstFileNames_Leave(object sender, EventArgs e)
        {
            AcceptButton = btnSearch;
        }

        /// <summary>
        /// Handles setting the form's AcceptButton to null so that enter key can be processed by control.
        /// </summary>
        /// <param name="sender">system parameter</param>
        /// <param name="e">system parameter</param>

        private void lstFileNames_Enter(object sender, EventArgs e)
        {
            AcceptButton = null;
        }

        /// <summary>
        /// Allows selection of the search path Source.
        /// </summary>
        /// <param name="sender">system parameter</param>
        /// <param name="e">system parameter</param>

        private void picBrowse_Click(object sender, System.EventArgs e)
        {
            BrowseForFolder();
        }


        /// <summary>
        /// Allows selection of the search path Destination.
        /// </summary>
        /// <param name="sender">system parameter</param>
        /// <param name="e">system parameter</param>
        private void picBrowseDest_Click(object sender, EventArgs e)
        {
            BrowseForFolderDest();
        }

        /// <summary>
        /// Displays the exclusions messages.
        /// </summary>
        /// <param name="sender">system parameter</param>
        /// <param name="e">system parameter</param>

        private void sbFilterCountPanel_DoubleClick(object sender, EventArgs e)
        {
            DisplaySearchMessages(LogItem.LogItemTypes.Exclusion);
        }

        /// <summary>
        /// Displays the error messages.
        /// </summary>
        /// <param name="sender">system parameter</param>
        /// <param name="e">system parameter</param>

        private void sbErrorCountPanel_DoubleClick(object sender, EventArgs e)
        {
            DisplaySearchMessages(LogItem.LogItemTypes.Error);
        }

        /// <summary>
        /// Displays the status messages.
        /// </summary>
        /// <param name="sender">system parameter</param>
        /// <param name="e">system parameter</param>

        private void sbStatusPanel_DoubleClick(object sender, EventArgs e)
        {
            DisplaySearchMessages(LogItem.LogItemTypes.Status);
        }

        /// <summary>
        /// Handles the DragEnter event for the path combo box.
        /// </summary>
        /// <param name="sender">system parameter</param>
        /// <param name="e">system parameter</param>
        /// <remarks>
        /// Handles file drops only by putting the directory into the combo box.
        /// </remarks>

        private void cboFilePath_DragEnter(object sender, DragEventArgs e)
        {
            if ((e.AllowedEffect & DragDropEffects.Copy) == DragDropEffects.Copy)
            {
                if (e.Data.GetDataPresent(DataFormats.FileDrop))
                {
                    e.Effect = DragDropEffects.Copy;
                }
            }
        }

        /// <summary>
        /// Handles the DragDrop event for the path combo box.
        /// </summary>
        /// <param name="sender">system parameter</param>
        /// <param name="e">system parameter</param>
        /// <remarks>
        /// Handles file drops only by putting the directory into the combo box.
        /// </remarks>

        private void cboFilePath_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            if (files != null && files.Length > 0)
            {
                string path1 = files[0];
                if (Directory.Exists(path1))
                {
                    AddComboSelection(cboFilePath, path1);
                }
                else if (File.Exists(path1))
                {
                    FileInfo file = new FileInfo(path1);
                    AddComboSelection(cboFilePath, file.DirectoryName);
                }
            }
        }

        /// <summary>
        /// Hide tooltip for a ToolStripStatusLabel.
        /// </summary>
        /// <param name="sender">Current ToolStripStatusLabel</param>
        /// <param name="e">System parameter</param>

        private void CountPanel_MouseLeave(object sender, EventArgs e)
        {
            toolTip1.Hide(this);
        }

        /// <summary>
        /// Show tooltip for a ToolStripStatusLabel.
        /// </summary>
        /// <param name="sender">Current ToolStripStatusLabel</param>
        /// <param name="e">System parameter</param>

        private void CountPanel_MouseEnter(object sender, EventArgs e)
        {
            var label = (ToolStripStatusLabel)sender;
            label.ToolTipText = "";
            var newPoint = this.PointToClient(Cursor.Position);
            toolTip1.Show(Language.GetControlToolTipText(label), this, newPoint.X, newPoint.Y);
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Load the general settings values.
        /// </summary>

        private void LoadSettings()
        {
            //  Only load up to the desired number of paths.
            if (GeneralSettings.MaximumMRUPaths < 0 || GeneralSettings.MaximumMRUPaths > Constants.MAX_STORED_PATHS)
            {
                GeneralSettings.MaximumMRUPaths = Constants.MAX_STORED_PATHS;
            }

            LoadComboBoxEntry(cboFilePath, GeneralSettings.SearchStarts);
            LoadComboBoxEntry(cboFilePathDest, GeneralSettings.CopyDestination);
            LoadComboBoxEntry(cboFileName, GeneralSettings.SearchFilters);
            LoadComboBoxEntry(cboSearchForText, GeneralSettings.SearchTexts);
            LoadComboBoxEntry(cboSearchForText2, GeneralSettings.SearchTextsTwo);

            // Path
            if (cboFilePath.Items.Count > 0 && cboFilePath.Items.Count != 1)
            {
                cboFilePath.SelectedIndex = 0;

                SetWindowText();
            }

            // Filter
            if (cboFileName.Items.Count == 0)
            {
                // no entries so create defaults
                cboFileName.Items.AddRange(new object[] { "*.*","*.xml", "*.txt", "*.java", "*.htm, *.html", "*.jsp, *.asp", "*.js, *.inc", "*.htm, *.html, *.jsp, *.asp", "*.sql", "*.bas, *.cls, *.vb", "*.cs", "*.cpp, *.c, *.h", "*.asm" });
            }
            cboFileName.SelectedIndex = 0;

            // Search
            if (cboSearchForText.Items.Count > 0)
            {
                cboSearchForText.SelectedIndex = 0;
            }

            // Viewing options
            RemoveWhiteSpaceMenuItem.Checked = GeneralSettings.RemoveLeadingWhiteSpace;
            EntireFileMenuItem.Checked = GeneralSettings.ShowEntireFile;

            // File list columns
            lstFileNames.Font = Convertors.ConvertStringToFont(GeneralSettings.FilePanelFont);
            SetColumnsText();

            // Load the window settings
            LoadWindowSettings();

            // Load the text editor settings
            LoadTextEditorSettings();

            // Load the text editor
            TextEditors.Load();

            // Load data to gridview
            if (GeneralSettings.UseDataBase)
            {
                SetStatusBarTotalCountPick(TotalPick());
                GetData();
            }
            
        }

        /// <summary>
        /// Load the window settings.
        /// </summary>

        private void LoadWindowSettings()
        {
            int _state = GeneralSettings.WindowState;

            Rectangle defaultBounds = this.Bounds;

            // set the form start position method
            StartPosition = FormStartPosition.Manual;

            // set the form top position
            if (GeneralSettings.WindowTop != -1)
                Top = GeneralSettings.WindowTop;
            else
            {
                Top = (Screen.PrimaryScreen.Bounds.Height - Constants.WINDOW_HEIGHT * GeneralSettings.WindowsDPIPerCentSetting / 100) / 2;
                if (Top < 0) Top = 0;
            }

            // set the form left position
            if (GeneralSettings.WindowLeft != -1)
                Left = GeneralSettings.WindowLeft;
            else
            {
                Left = (Screen.PrimaryScreen.Bounds.Width - Constants.WINDOW_WIDTH * GeneralSettings.WindowsDPIPerCentSetting / 100) / 2;
                if (Left < 0) Left = 0;
            }

            // set the form width
            if (GeneralSettings.WindowWidth != -1)
                Width = GeneralSettings.WindowWidth;
            else
            {
                Width = Constants.WINDOW_WIDTH * GeneralSettings.WindowsDPIPerCentSetting / 100;
                if (Width > Screen.PrimaryScreen.Bounds.Width)
                    Width = Constants.WINDOW_WIDTH;
            }

            // set the form height
            if (GeneralSettings.WindowHeight != -1)
                Height = GeneralSettings.WindowHeight;
            else
            {
                Height = Constants.WINDOW_HEIGHT * GeneralSettings.WindowsDPIPerCentSetting / 100;
                if (Height > Screen.PrimaryScreen.Bounds.Height)
                    Height = Constants.WINDOW_HEIGHT;
            }

            // form can't find a screen to fit on, so reset to center screen on primary screen
            if (!Screen.AllScreens.Any(s => s.WorkingArea.IntersectsWith(this.Bounds)))
            {
                this.Bounds = defaultBounds;
            }

            if (_state != -1 && _state == (int)FormWindowState.Maximized)
            {
                WindowState = FormWindowState.Maximized;
            }

            // set the splitter position
            if (GeneralSettings.WindowSearchPanelWidth != -1)
                pnlSearch.Width = GeneralSettings.WindowSearchPanelWidth;

            // set the file panel height
            if (GeneralSettings.WindowFilePanelHeight != -1)
                lstFileNames.Height = GeneralSettings.WindowFilePanelHeight;
            else
                lstFileNames.Height = GeneralSettings.DEFAULT_FILE_PANEL_HEIGHT * GeneralSettings.WindowsDPIPerCentSetting / 100;

            // increase minimum default splitter position to avoid cutoff text with increasing DPI values
            splitLeftRight.MinSize = Constants.DEFAULT_SEARCH_PANEL_WIDTH * GeneralSettings.WindowsDPIPerCentSetting / 100;

            // adjust splitter position if neccessary
            if (pnlSearch.Width < splitLeftRight.MinSize)
            {
                pnlSearch.Width = splitLeftRight.MinSize;
            }

            // for windows xp and below, use the system render mode to make it look better
            if (!API.IsWindowsVistaOrLater)
            {
                MainMenu.RenderMode = ToolStripRenderMode.System;
            }

            // set file column widths from user settings

            if (GeneralSettings.WindowFileColumnNameWidth != -1)
                lstFileNames.Columns[Constants.COLUMN_INDEX_FILE].Width = GeneralSettings.WindowFileColumnNameWidth;
            else
                lstFileNames.Columns[Constants.COLUMN_INDEX_FILE].Width = Constants.COLUMN_WIDTH_FILE * GeneralSettings.WindowsDPIPerCentSetting / 100;

            if (GeneralSettings.WindowFileColumnLocationWidth != -1)
                lstFileNames.Columns[Constants.COLUMN_INDEX_DIRECTORY].Width = GeneralSettings.WindowFileColumnLocationWidth;
            else
                lstFileNames.Columns[Constants.COLUMN_INDEX_DIRECTORY].Width = Constants.COLUMN_WIDTH_DIRECTORY * GeneralSettings.WindowsDPIPerCentSetting / 100;

            if (GeneralSettings.WindowFileColumnFileExtWidth != -1)
                lstFileNames.Columns[Constants.COLUMN_INDEX_FILE_EXT].Width = GeneralSettings.WindowFileColumnFileExtWidth;
            else
                lstFileNames.Columns[Constants.COLUMN_INDEX_FILE_EXT].Width = Constants.COLUMN_WIDTH_FILE_EXT * GeneralSettings.WindowsDPIPerCentSetting / 100;

            if (GeneralSettings.WindowFileColumnDateWidth != -1)
                lstFileNames.Columns[Constants.COLUMN_INDEX_DATE].Width = GeneralSettings.WindowFileColumnDateWidth;
            else
                lstFileNames.Columns[Constants.COLUMN_INDEX_DATE].Width = Constants.COLUMN_WIDTH_DATE * GeneralSettings.WindowsDPIPerCentSetting / 100;

            if (GeneralSettings.WindowFileColumnSizeWidth != -1)
                lstFileNames.Columns[Constants.COLUMN_INDEX_SIZE].Width = GeneralSettings.WindowFileColumnSizeWidth;
            else
                lstFileNames.Columns[Constants.COLUMN_INDEX_SIZE].Width = Constants.COLUMN_WIDTH_SIZE * GeneralSettings.WindowsDPIPerCentSetting / 100;

            if (GeneralSettings.WindowFileColumnCountWidth != -1)
                lstFileNames.Columns[Constants.COLUMN_INDEX_COUNT].Width = GeneralSettings.WindowFileColumnCountWidth;
            else
                lstFileNames.Columns[Constants.COLUMN_INDEX_COUNT].Width = Constants.COLUMN_WIDTH_COUNT * GeneralSettings.WindowsDPIPerCentSetting / 100;
        }

        /// <summary>
        /// Load the texteditor settings.
        /// </summary>

        private void LoadTextEditorSettings()
        {
            var foreground = Convertors.ConvertStringToSolidColorBrush(GeneralSettings.ResultsForeColor);
            var background = Convertors.ConvertStringToSolidColorBrush(GeneralSettings.ResultsBackColor);
            var contextForeground = Convertors.ConvertStringToSolidColorBrush(GeneralSettings.ResultsContextForeColor);
            var font = Convertors.ConvertStringToFont(GeneralSettings.ResultsFont);

            txtHits.Foreground = foreground;
            txtHits.Background = background;
            txtHits.LineNumbersForeground = contextForeground;
            txtHits.LineNumbersMatchForeground = foreground;
            txtHits.FontFamily = new System.Windows.Media.FontFamily(font.FontFamily.Name);
            txtHits.FontSize = font.SizeInPoints * 96 / 72;
            txtHits.WordWrap = WordWrapMenuItem.Checked = GeneralSettings.ResultsWordWrap;

            // Sets texteditor scroll bar visibilty to windows system default
            txtHits.HorizontalScrollBarVisibility = System.Windows.Controls.ScrollBarVisibility.Auto;
            txtHits.VerticalScrollBarVisibility = System.Windows.Controls.ScrollBarVisibility.Auto;

            // Handles finding the current line under the mouse pointer during a double click to open the texteditor at that location
            txtHits.PreviewMouseDoubleClick += txtHits_PreviewMouseDoubleClick;

            // adjust texteditor font size
        }

        /// <summary>
        /// Save the general settings values.
        /// </summary>

        private void SaveSettings()
        {
            SaveWindowSettings();

            //save column widths
            GeneralSettings.WindowFileColumnNameWidth = lstFileNames.Columns[Constants.COLUMN_INDEX_FILE].Width;
            GeneralSettings.WindowFileColumnLocationWidth = lstFileNames.Columns[Constants.COLUMN_INDEX_DIRECTORY].Width;
            GeneralSettings.WindowFileColumnDateWidth = lstFileNames.Columns[Constants.COLUMN_INDEX_DATE].Width;
            GeneralSettings.WindowFileColumnCountWidth = lstFileNames.Columns[Constants.COLUMN_INDEX_COUNT].Width;
            GeneralSettings.WindowFileColumnSizeWidth = lstFileNames.Columns[Constants.COLUMN_INDEX_SIZE].Width;
            GeneralSettings.WindowFileColumnFileExtWidth = lstFileNames.Columns[Constants.COLUMN_INDEX_FILE_EXT].Width;

            //save divider panel positions
            GeneralSettings.WindowSearchPanelWidth = pnlSearch.Width;
            GeneralSettings.WindowFilePanelHeight = lstFileNames.Height;

            //save search comboboxes
            GeneralSettings.SearchStarts = Convertors.GetComboBoxEntriesAsString(cboFilePath);
            GeneralSettings.CopyDestination = Convertors.GetComboBoxEntriesAsString(cboFilePathDest);
            GeneralSettings.SearchFilters = Convertors.GetComboBoxEntriesAsString(cboFileName);
            GeneralSettings.SearchTexts = Convertors.GetComboBoxEntriesAsString(cboSearchForText);
            GeneralSettings.SearchTextsTwo = cboSearchForText2.Text;//Convertors.GetComboBoxEntriesAsString(cboSearchForText2);

            //save view options
            GeneralSettings.ResultsWordWrap = WordWrapMenuItem.Checked;
            GeneralSettings.RemoveLeadingWhiteSpace = RemoveWhiteSpaceMenuItem.Checked;
            GeneralSettings.ShowEntireFile = EntireFileMenuItem.Checked;

            GeneralSettings.Save();
        }

        /// <summary>
        /// Save the window settings in the config.
        /// </summary>

        private void SaveWindowSettings()
        {
            if (WindowState == FormWindowState.Normal)
            {
                GeneralSettings.WindowLeft = this.Left;
                GeneralSettings.WindowTop = this.Top;
                GeneralSettings.WindowWidth = this.Width;
                GeneralSettings.WindowHeight = this.Height;
                GeneralSettings.WindowState = (int)this.WindowState;
            }
            else
            {
                // just save the state, so that previous normal dimensions are valid
                GeneralSettings.WindowState = (int)this.WindowState;
            }
        }

        /// <summary>
        /// Loads the given System.Windows.Forms.ComboBox with the values.
        /// </summary>
        /// <param name="combo">System.Windows.Forms.ComboBoxy</param>
        /// <param name="values">string of the values to load</param>

        private void LoadComboBoxEntry(System.Windows.Forms.ComboBox combo, string values)
        {
            if (!values.Equals(string.Empty))
            {
                string[] items = Convertors.GetComboBoxEntriesFromString(values);

                if (items.Length > 0)
                {
                    int start = items.Length;
                    if (start > GeneralSettings.MaximumMRUPaths)
                    {
                        start = GeneralSettings.MaximumMRUPaths;
                    }

                    combo.BeginUpdate();
                    for (int i = start - 1; i > -1; i--)
                    {
                        AddComboSelection(combo, items[i]);
                    }
                    combo.EndUpdate();
                }
            }
        }

        /// <summary>
        /// Set the Common Search Settings on the form
        /// </summary>

        private void LoadSearchSettings()
        {
            chkRegularExpressions.Checked = Core.SearchSettings.UseRegularExpressions;
            chkCaseSensitive.Checked = Core.SearchSettings.UseCaseSensitivity;
            chkWholeWordOnly.Checked = Core.SearchSettings.UseWholeWordMatching;
            LineNumbersMenuItem.Checked = txtHits.ShowLineNumbers = Core.SearchSettings.IncludeLineNumbers;
            chkRecurse.Checked = Core.SearchSettings.UseRecursion;
            chkFileNamesOnly.Checked = Core.SearchSettings.ReturnOnlyFileNames;
            txtContextLines.Value = Core.SearchSettings.ContextLines;
            chkNegation.Checked = Core.SearchSettings.UseNegation;
            chkAllResultsAfterSearch.Checked = true; //Core.SearchSettings.ShowAllResultsAfterSearch;
            __FilterItems = FilterItem.ConvertStringToFilterItems(Core.SearchSettings.FilterItems);

        }

        /// <summary>
        /// Save the search options.
        /// </summary>

        private void SaveSearchSettings()
        {
            Core.SearchSettings.UseRegularExpressions = chkRegularExpressions.Checked;
            Core.SearchSettings.UseCaseSensitivity = chkCaseSensitive.Checked;
            Core.SearchSettings.UseWholeWordMatching = chkWholeWordOnly.Checked;
            Core.SearchSettings.IncludeLineNumbers = LineNumbersMenuItem.Checked;
            Core.SearchSettings.UseRecursion = chkRecurse.Checked;
            Core.SearchSettings.ReturnOnlyFileNames = chkFileNamesOnly.Checked;
            Core.SearchSettings.ContextLines = Convert.ToInt32(txtContextLines.Value);
            Core.SearchSettings.UseNegation = chkNegation.Checked;
            Core.SearchSettings.ShowAllResultsAfterSearch = chkAllResultsAfterSearch.Checked;
            Core.SearchSettings.FilterItems = FilterItem.ConvertFilterItemsToString(__FilterItems);

            Core.SearchSettings.Save();
        }

        /// <summary>
        /// Sets the file list's columns' text to the correct language.
        /// </summary>

        private void SetColumnsText()
        {
            if (lstFileNames.Columns.Count == 0)
            {
                lstFileNames.Columns.Add(Language.GetGenericText("ResultsColumnFile"), GeneralSettings.WindowFileColumnNameWidth, HorizontalAlignment.Left);
                lstFileNames.Columns.Add(Language.GetGenericText("ResultsColumnLocation"), GeneralSettings.WindowFileColumnLocationWidth, HorizontalAlignment.Left);
                lstFileNames.Columns.Add(Language.GetGenericText("ResultsColumnFileExt"), GeneralSettings.WindowFileColumnFileExtWidth, HorizontalAlignment.Left);
                lstFileNames.Columns.Add(Language.GetGenericText("ResultsColumnDate"), GeneralSettings.WindowFileColumnDateWidth, HorizontalAlignment.Left);
                lstFileNames.Columns.Add(Language.GetGenericText("ResultsColumnSize"), GeneralSettings.WindowFileColumnSizeWidth, HorizontalAlignment.Left);
                lstFileNames.Columns.Add(Language.GetGenericText("ResultsColumnCount"), GeneralSettings.WindowFileColumnCountWidth, HorizontalAlignment.Left);
            }
            else
            {
                lstFileNames.Columns[Constants.COLUMN_INDEX_FILE].Text = Language.GetGenericText("ResultsColumnFile");
                lstFileNames.Columns[Constants.COLUMN_INDEX_DIRECTORY].Text = Language.GetGenericText("ResultsColumnLocation");
                lstFileNames.Columns[Constants.COLUMN_INDEX_FILE_EXT].Text = Language.GetGenericText("ResultsColumnFileExt");
                lstFileNames.Columns[Constants.COLUMN_INDEX_SIZE].Text = Language.GetGenericText("ResultsColumnSize");
                lstFileNames.Columns[Constants.COLUMN_INDEX_DATE].Text = Language.GetGenericText("ResultsColumnDate");
                lstFileNames.Columns[Constants.COLUMN_INDEX_COUNT].Text = Language.GetGenericText("ResultsColumnCount");
            }

            AddContextMenuForResults();
        }

        /// <summary>
        /// Verify user selected options
        /// </summary>
        /// <returns>True - Verified, False - Otherwise</returns>

        private bool VerifyInterface()
        {
            try
            {
                int _lines = -1;
                if (!int.TryParse(txtContextLines.Text, out _lines) || _lines < 0 || _lines > Constants.MAX_CONTEXT_LINES)
                {
                    MessageBox.Show(String.Format(Language.GetGenericText("VerifyErrorContextLines"), 0, Constants.MAX_CONTEXT_LINES.ToString()),
                       ProductInformation.ApplicationName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return false;
                }

                if (cboFileName.Text.Trim().Equals(string.Empty))
                {
                    MessageBox.Show(Language.GetGenericText("VerifyErrorFileType"),
                       ProductInformation.ApplicationName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return false;
                }

                if (cboFilePath.Text.Trim().Equals(string.Empty))
                {
                    MessageBox.Show(Language.GetGenericText("VerifyErrorNoStartPath"),
                       ProductInformation.ApplicationName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return false;
                }

                string[] paths = cboFilePath.Text.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string path in paths)
                {
                    if (!System.IO.Directory.Exists(path))
                    {
                        MessageBox.Show(String.Format(Language.GetGenericText("VerifyErrorInvalidStartPath"), path),
                           ProductInformation.ApplicationName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return false;
                    }
                }

                if (chkSearchInResults.Checked && lstFileNames.Items.Count == 0)
                {
                    MessageBox.Show(Language.GetGenericText("VerifyErrorNoResultsToSearchIn"),
                       ProductInformation.ApplicationName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return false;
                }

                //if (cboSearchForText.Text.Trim().Equals(string.Empty))
                //{
                //    MessageBox.Show(Language.GetGenericText("VerifyErrorNoSearchText"),
                //       Constants.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                //    return false;
                //}

                if (chkRegularExpressions.Checked && !cboSearchForText.Text.Trim().Equals(string.Empty))
                {
                    // test reg ex
                    try
                    {
                        var reg = new Regex(cboSearchForText.Text, RegexOptions.IgnoreCase);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(String.Format(Language.GetGenericText("VerifyErrorInvalidRegEx"), ex.Message),
                           ProductInformation.ApplicationName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return false;
                    }
                }
            }
            catch
            {
                MessageBox.Show(Language.GetGenericText("VerifyErrorGeneric"),
                   ProductInformation.ApplicationName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Add an item to a combo box
        /// </summary>
        /// <param name="combo">Combo Box</param>
        /// <param name="item">Item to add</param>

        private static void AddComboSelection(ComboBox combo, string item)
        {
            if (item.Length > 0)
            {
                // If this path is already in the dropdown, remove it.
                if (combo.Items.Contains(item))
                {
                    combo.Items.Remove(item);
                }

                // Add this path as the first item in the dropdown.
                combo.Items.Insert(0, item);

                // The combo text gets cleared by the AddItem.
                combo.Text = item;

                // Only store as many paths as has been set in options.
                //if (combo.Items.Count > Common.NUM_STORED_PATHS)
                if (combo.Items.Count > GeneralSettings.MaximumMRUPaths)
                {
                    // Remove the last item in the list.
                    combo.Items.RemoveAt(combo.Items.Count - 1);
                }
            }
        }

        /// <summary>
        /// Highlight the searched text in the results
        /// </summary>
        /// <param name="match">MatchResult containing results</param>

        private void ProcessMatchForDisplay(MatchResult match)
        {
            if (match == null || match.Matches.Count == 0)
            {
                txtHits.Clear();
                txtHits.SyntaxHighlighting = null;
                txtHits.LineNumbers = null;
                return;
            }

            txtHits.Clear();
            txtHits.SyntaxHighlighting = null;
            txtHits.ScrollToHome();

            for (int i = txtHits.TextArea.TextView.LineTransformers.Count - 1; i >= 0; i--)
            {
                if (txtHits.TextArea.TextView.LineTransformers[i] is ResultHighlighter)
                    txtHits.TextArea.TextView.LineTransformers.RemoveAt(i);
                else if (txtHits.TextArea.TextView.LineTransformers[i] is AllResultHighlighter)
                    txtHits.TextArea.TextView.LineTransformers.RemoveAt(i);
            }
            var foreground = Convertors.ConvertStringToSolidColorBrush(GeneralSettings.HighlightForeColor);
            var background = Convertors.ConvertStringToSolidColorBrush(GeneralSettings.HighlightBackColor);
            var nonForeground = Convertors.ConvertStringToSolidColorBrush(GeneralSettings.ResultsContextForeColor);
            txtHits.TextArea.TextView.LineTransformers.Add(new ResultHighlighter(match, RemoveWhiteSpaceMenuItem.Checked)
            {
                MatchForeground = foreground,
                MatchBackground = background,
                NonMatchForeground = nonForeground
            });

            StringBuilder builder = new StringBuilder();
            var lineNumbers = new List<LineNumber>();
            var path = match.File.FullName;
            int max = match.Matches.Count;
            for (int i = 0; i < max; i++)
            {
                string line = match.Matches[i].Line;
                if (RemoveWhiteSpaceMenuItem.Checked)
                {
                    line = line.TrimStart();
                }
                builder.AppendLine(line);
                lineNumbers.Add(new LineNumber()
                {
                    Number = match.Matches[i].LineNumber,
                    HasMatch = match.Matches[i].HasMatch,
                    FileFullName = match.Matches[i].LineNumber > -1 || match.FromPlugin ? path : string.Empty,
                    ColumnNumber = match.Matches[i].ColumnNumber
                });
            }

            // the last result will have a hanging newline, so remove it.
            if (builder.Length > 0)
            {
                builder.Remove(builder.Length - Environment.NewLine.Length, Environment.NewLine.Length);
            }

            txtHits.LineNumbers = lineNumbers;
            txtHits.Text = builder.ToString();
        }

        /// <summary>
        /// Displays the entire selected file and uses a syntax highlighter when available for that file extension.
        /// </summary>
        /// <param name="match">Currently selected MatchResult</param>

        private void ProcessFileForDisplay(MatchResult match)
        {
            if (match == null || match.Matches.Count == 0)
            {
                txtHits.Clear();
                txtHits.LineNumbers = null;
                return;
            }

            if ((match.File.Length > 1024000 || FilterItem.IsBinaryFile(match.File)) &&
               MessageBox.Show(this, Language.GetGenericText("ResultsPreviewLargeBinaryFile"), ProductInformation.ApplicationName, MessageBoxButtons.YesNo,
               MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == System.Windows.Forms.DialogResult.No)
            {
                ProcessMatchForDisplay(match); // display just the results then and not the whole file.
                return;
            }

            txtHits.Clear();
            txtHits.LineNumbers = null;
            txtHits.ScrollToHome();

            var def = ICSharpCode.AvalonEdit.Highlighting.HighlightingManager.Instance.GetDefinitionByExtension(match.File.Extension);
            txtHits.SyntaxHighlighting = def;

            for (int i = txtHits.TextArea.TextView.LineTransformers.Count - 1; i >= 0; i--)
            {
                if (txtHits.TextArea.TextView.LineTransformers[i] is ResultHighlighter)
                    txtHits.TextArea.TextView.LineTransformers.RemoveAt(i);
                else if (txtHits.TextArea.TextView.LineTransformers[i] is AllResultHighlighter)
                    txtHits.TextArea.TextView.LineTransformers.RemoveAt(i);
            }
            var foreground = Convertors.ConvertStringToSolidColorBrush(GeneralSettings.HighlightForeColor);
            var background = Convertors.ConvertStringToSolidColorBrush(GeneralSettings.HighlightBackColor);
            var nonForeground = Convertors.ConvertStringToSolidColorBrush(GeneralSettings.ResultsContextForeColor);
            txtHits.TextArea.TextView.LineTransformers.Add(new ResultHighlighter(match, false, true)
            {
                MatchForeground = foreground,
                MatchBackground = background,
                NonMatchForeground = nonForeground
            });

            // convert MatchResultLine to LineNumber
            List<LineNumber> lineNumbers = new List<LineNumber>();
            foreach (MatchResultLine matchLine in match.Matches)
            {
                lineNumbers.Add(new LineNumber()
                {
                    ColumnNumber = matchLine.ColumnNumber,
                    Number = matchLine.LineNumber,
                    HasMatch = matchLine.HasMatch,
                    FileFullName = match.File.FullName
                });
            }

            txtHits.LineNumbers = lineNumbers;
            txtHits.Encoding = match.DetectedEncoding;
            txtHits.Load(match.File.FullName);
        }

        /// <summary>
        /// Handles generating the text for all matches.
        /// </summary>

        private void ProcessAllMatchesForDisplay()
        {
            txtHits.Dispatcher.Invoke(new Action(() =>
            {
                txtHits.Clear();
                txtHits.SyntaxHighlighting = null;
                txtHits.ScrollToHome();

                if (__Grep == null || __Grep.MatchResults.Count == 0)
                    return;

                for (int i = txtHits.TextArea.TextView.LineTransformers.Count - 1; i >= 0; i--)
                {
                    if (txtHits.TextArea.TextView.LineTransformers[i] is ResultHighlighter)
                        txtHits.TextArea.TextView.LineTransformers.RemoveAt(i);
                    else if (txtHits.TextArea.TextView.LineTransformers[i] is AllResultHighlighter)
                        txtHits.TextArea.TextView.LineTransformers.RemoveAt(i);
                }
                var foreground = Convertors.ConvertStringToSolidColorBrush(GeneralSettings.HighlightForeColor);
                var background = Convertors.ConvertStringToSolidColorBrush(GeneralSettings.HighlightBackColor);
                var nonForeground = Convertors.ConvertStringToSolidColorBrush(GeneralSettings.ResultsContextForeColor);
                txtHits.TextArea.TextView.LineTransformers.Add(new AllResultHighlighter(__Grep.MatchResults, RemoveWhiteSpaceMenuItem.Checked)
                {
                    MatchForeground = foreground,
                    MatchBackground = background,
                    NonMatchForeground = nonForeground
                });

                StringBuilder builder = new StringBuilder();
                var lineNumbers = new List<LineNumber>();

                int maxResults = __Grep.MatchResults.Count;
                for (int i = 0; i < maxResults; i++)
                {
                    var match = __Grep.MatchResults[i];
                    var path = match.File.FullName;
                    builder.AppendLine(match.File.FullName);
                    builder.AppendLine();
                    lineNumbers.Add(new LineNumber() { FileFullName = path });

                    //Invoke method insert into data base
                    if (GeneralSettings.UseDataBase)
                    {
                        InsertDataBase(path);

//                        if (string.IsNullOrEmpty(path))
//                        {
//                            MessageBox.Show("SERIAL=>> " + cboSearchForText.Text + "  NOT FOUND!!!",
//ProductInformation.ApplicationName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
//                        }
//                        else
//                        {
//                            InsertDataBase(path);
                        //}
                    }

                    lineNumbers.Add(new LineNumber());

                    int max = match.Matches.Count;
                    for (int j = 0; j < max; j++)
                    {
                        string line = match.Matches[j].Line;
                        if (RemoveWhiteSpaceMenuItem.Checked)
                        {
                            line = line.TrimStart();
                        }
                        builder.AppendLine(line);
                        lineNumbers.Add(new LineNumber()
                        {
                            Number = match.Matches[j].LineNumber,
                            HasMatch = match.Matches[j].HasMatch,
                            FileFullName = match.Matches[j].LineNumber > -1 ? path : string.Empty,
                            ColumnNumber = match.Matches[j].ColumnNumber
                        });
                    }

                    if (i + 1 < maxResults)
                    {
                        builder.AppendLine();
                        builder.AppendLine();
                        lineNumbers.Add(new LineNumber());
                        lineNumbers.Add(new LineNumber());
                    }
                }

                // the last result will have a hanging newline, so remove it.
                if (builder.Length > 0)
                {
                    builder.Remove(builder.Length - Environment.NewLine.Length, Environment.NewLine.Length);
                }

                txtHits.LineNumbers = lineNumbers;
                txtHits.Text = builder.ToString();
                //txtHitsb.Text = builder.ToString();

            }
        ));
            cboSearchForText.Focus();
        }

        /// <summary>
        /// Enable/Disable menu items (Thread safe)
        /// </summary>
        /// <param name="enable">True - enable menu items, False - disable</param>

        private void SetSearchState(bool enable)
        {
            if (this.InvokeRequired)
            {
                SetSearchStateCallBack _delegate = new SetSearchStateCallBack(SetSearchState);
                this.Invoke(_delegate, new Object[1] { enable });
                return;
            }

            FileMenu.Enabled = enable;
            EditMenu.Enabled = enable;
            ViewMenu.Enabled = enable;
            ToolsMenu.Enabled = enable;
            HelpMenu.Enabled = enable;

            btnSearch.Enabled = enable;
            btnCancel.Enabled = !enable;
            picBrowse.Enabled = enable;
            PanelOptionsContainer.Enabled = enable;

            cboFileName.Enabled = enable;
            cboFilePath.Enabled = enable;
            cboSearchForText.Enabled = enable;

            if (enable)
                btnSearch.Focus();
            else
                btnCancel.Focus();
        }

        /// <summary>
        /// Open Browser for Folder dialog
        /// </summary>

        private void BrowseForFolder()
        {
            VistaFolderBrowserDialog dlg = new VistaFolderBrowserDialog();
            if (!API.IsWindowsVistaOrLater)
            {
                dlg.Description = Language.GetGenericText("OpenFolderDescription");
                dlg.ShowNewFolderButton = false;
            }

            // set initial directory if valid
            if (System.IO.Directory.Exists(cboFilePath.Text))
            {
                dlg.SelectedPath = cboFilePath.Text;
            }

            // display dialog and setup path if selected
            if (dlg.ShowDialog(this) == DialogResult.OK)
            {
                AddComboSelection(cboFilePath, dlg.SelectedPath);
            }
        }

        /// <summary>
        /// Open Browser for Folder destination dialog
        /// </summary>

        private void BrowseForFolderDest()
        {
            VistaFolderBrowserDialog dlg = new VistaFolderBrowserDialog();
            if (!API.IsWindowsVistaOrLater)
            {
                dlg.Description = Language.GetGenericText("OpenFolderDescription");
                dlg.ShowNewFolderButton = false;
            }

            // set initial directory if valid
            if (System.IO.Directory.Exists(cboFilePathDest.Text))
            {
                dlg.SelectedPath = cboFilePathDest.Text;
            }

            // display dialog and setup path if selected
            if (dlg.ShowDialog(this) == DialogResult.OK)
            {
                AddComboSelection(cboFilePathDest, dlg.SelectedPath);
            }
        }

        /// <summary>
        /// Calculates the width of the drop down list of the given combo box
        /// </summary>
        /// <param name="combo">Combo box to base calculate from</param>
        /// <returns>Width of longest string in combo box items</returns>

        private int CalculateDropDownWidth(ComboBox combo)
        {
            const int EXTRA = 10;

            Graphics g = combo.CreateGraphics();
            int _max = combo.Width;
            string _itemValue = string.Empty;
            SizeF _size;

            foreach (object _item in combo.Items)
            {
                _itemValue = _item.ToString();
                _size = g.MeasureString(_itemValue, combo.Font);

                if (_size.Width > _max)
                    _max = Convert.ToInt32(_size.Width);
            }

            // keep original width if no item longer
            if (_max != combo.Width)
                _max += EXTRA;

            g.Dispose();

            return _max;
        }

        /// <summary>
        /// Truncate the given file's name if it is to long to fit in the given status bar
        /// </summary>
        /// <param name="file">FileInfo object to measure</param>
        /// <param name="status">StatusStrip to measure against</param>
        /// <returns>file name or truncated file name if to long</returns>

        private string TruncateFileName(System.IO.FileInfo file, StatusStrip status)
        {
            string name = file.FullName;
            const int EXTRA = 20;     //used for spacing of the sizer
            using (Graphics g = status.CreateGraphics())
            {
                int width = Convert.ToInt32(g.MeasureString(name, status.Font).Width);
                if (width >= (status.Width - EXTRA))
                {
                    // truncate to just the root name and the file name (for now)
                    name = file.Directory.Root.Name + @"...\" + file.Name;
                }
            }

            return name;
        }

        /// <summary>
        /// Processes any command line arguments
        /// </summary>

        private void ProcessCommandLine()
        {
            if (CommandLineArgs.AnyArguments)
            {
                if (CommandLineArgs.IsValidStartPath)
                    AddComboSelection(cboFilePath, CommandLineArgs.StartPath);

                if (CommandLineArgs.IsValidFileTypes)
                    AddComboSelection(cboFileName, CommandLineArgs.FileTypes);

                if (CommandLineArgs.IsValidSearchText)
                    AddComboSelection(cboSearchForText, CommandLineArgs.SearchText);

                // turn on option if specified (options default to last saved otherwise)
                if (CommandLineArgs.UseRegularExpressions)
                    chkRegularExpressions.Checked = true;
                if (CommandLineArgs.IsCaseSensitive)
                    chkCaseSensitive.Checked = true;
                if (CommandLineArgs.IsWholeWord)
                    chkWholeWordOnly.Checked = true;
                if (CommandLineArgs.UseRecursion)
                    chkRecurse.Checked = true;
                if (CommandLineArgs.IsFileNamesOnly)
                    chkFileNamesOnly.Checked = true;
                if (CommandLineArgs.IsNegation)
                    chkNegation.Checked = true;
                if (CommandLineArgs.UseLineNumbers)
                    LineNumbersMenuItem.Checked = true;
                if (CommandLineArgs.ContextLines > -1)
                    txtContextLines.Value = CommandLineArgs.ContextLines;

                if (CommandLineArgs.SkipHiddenFile)
                {
                    // check if exists, enable if necessary, or create them
                    var fiFile = GetFilterItemsByFilterType(new FilterType(FilterType.Categories.File, FilterType.SubCategories.Hidden));

                    if (fiFile == null || fiFile.Count == 0)
                    {
                        __FilterItems.Add(new FilterItem(new FilterType(FilterType.Categories.File, FilterType.SubCategories.Hidden),
                           string.Empty, FilterType.ValueOptions.None, false, true));
                    }
                    else if (!fiFile.First().Enabled)
                    {
                        int index = __FilterItems.IndexOf(fiFile.First());
                        if (index > -1)
                        {
                            __FilterItems[index].Enabled = true;
                        }
                    }
                }

                if (CommandLineArgs.SkipHiddenDirectory)
                {
                    // check if exists, enable if necessary, or create them
                    var fiDir = GetFilterItemsByFilterType(new FilterType(FilterType.Categories.Directory, FilterType.SubCategories.Hidden));

                    if (fiDir == null || fiDir.Count == 0)
                    {
                        __FilterItems.Add(new FilterItem(new FilterType(FilterType.Categories.Directory, FilterType.SubCategories.Hidden),
                           string.Empty, FilterType.ValueOptions.None, false, true));
                    }
                    else if (!fiDir.First().Enabled)
                    {
                        int index = __FilterItems.IndexOf(fiDir.First());
                        if (index > -1)
                        {
                            __FilterItems[index].Enabled = true;
                        }
                    }
                }
                if (CommandLineArgs.SkipSystemFile)
                {
                    // check if exists, enable if necessary, or create them
                    var fiFile = GetFilterItemsByFilterType(new FilterType(FilterType.Categories.File, FilterType.SubCategories.System));

                    if (fiFile == null || fiFile.Count == 0)
                    {
                        __FilterItems.Add(new FilterItem(new FilterType(FilterType.Categories.File, FilterType.SubCategories.System),
                           string.Empty, FilterType.ValueOptions.None, false, true));
                    }
                    else if (!fiFile.First().Enabled)
                    {
                        int index = __FilterItems.IndexOf(fiFile.First());
                        if (index > -1)
                        {
                            __FilterItems[index].Enabled = true;
                        }
                    }
                }

                if (CommandLineArgs.SkipSystemDirectory)
                {
                    // check if exists, enable if necessary, or create them
                    var fiDir = GetFilterItemsByFilterType(new FilterType(FilterType.Categories.Directory, FilterType.SubCategories.System));

                    if (fiDir == null || fiDir.Count == 0)
                    {
                        __FilterItems.Add(new FilterItem(new FilterType(FilterType.Categories.Directory, FilterType.SubCategories.System),
                           string.Empty, FilterType.ValueOptions.None, false, true));
                    }
                    else if (!fiDir.First().Enabled)
                    {
                        int index = __FilterItems.IndexOf(fiDir.First());
                        if (index > -1)
                        {
                            __FilterItems[index].Enabled = true;
                        }
                    }
                }

                if (CommandLineArgs.DateModifiedFile != null)
                {
                    // check if exists, enable if necessary/set value, or create it
                    var fiFile = GetFilterItemsByFilterType(new FilterType(FilterType.Categories.File, FilterType.SubCategories.DateModified));

                    bool foundExact = false;
                    if (fiFile != null && fiFile.Count > 0)
                    {
                        // more than one entry, so disable any that aren't less
                        foreach (var item in fiFile)
                        {
                            if (item.ValueOption == CommandLineArgs.DateModifiedFile.ValueOption && item.Value.Equals(CommandLineArgs.DateModifiedFile.Value.ToString()))
                            {
                                int index = __FilterItems.IndexOf(item);
                                if (index > -1)
                                {
                                    __FilterItems[index].Enabled = true;
                                    foundExact = true;
                                }
                            }
                            else
                            {
                                int index = __FilterItems.IndexOf(item);
                                if (index > -1)
                                {
                                    __FilterItems[index].Enabled = false;
                                }
                            }
                        }
                    }

                    if (fiFile == null || fiFile.Count == 0 || !foundExact)
                    {
                        __FilterItems.Add(new FilterItem(new FilterType(FilterType.Categories.File, FilterType.SubCategories.DateModified),
                           CommandLineArgs.DateModifiedFile.Value.ToString(), CommandLineArgs.DateModifiedFile.ValueOption, false, true));
                    }
                }

                if (CommandLineArgs.DateModifiedDirectory != null)
                {
                    // check if exists, enable if necessary/set value, or create it
                    var fiDir = GetFilterItemsByFilterType(new FilterType(FilterType.Categories.Directory, FilterType.SubCategories.DateModified));

                    bool foundExact = false;
                    if (fiDir != null && fiDir.Count > 0)
                    {
                        // more than one entry, so disable any that aren't less
                        foreach (var item in fiDir)
                        {
                            if (item.ValueOption == CommandLineArgs.DateModifiedDirectory.ValueOption && item.Value.Equals(CommandLineArgs.DateModifiedDirectory.Value.ToString()))
                            {
                                int index = __FilterItems.IndexOf(item);
                                if (index > -1)
                                {
                                    __FilterItems[index].Enabled = true;
                                    foundExact = true;
                                }
                            }
                            else
                            {
                                int index = __FilterItems.IndexOf(item);
                                if (index > -1)
                                {
                                    __FilterItems[index].Enabled = false;
                                }
                            }
                        }
                    }

                    if (fiDir == null || fiDir.Count == 0 || !foundExact)
                    {
                        __FilterItems.Add(new FilterItem(new FilterType(FilterType.Categories.Directory, FilterType.SubCategories.DateModified),
                           CommandLineArgs.DateModifiedDirectory.Value.ToString(), CommandLineArgs.DateModifiedDirectory.ValueOption, false, true));
                    }
                }

                if (CommandLineArgs.DateCreatedFile != null)
                {
                    // check if exists, enable if necessary/set value, or create it
                    var fiFile = GetFilterItemsByFilterType(new FilterType(FilterType.Categories.File, FilterType.SubCategories.DateCreated));

                    bool foundExact = false;
                    if (fiFile != null && fiFile.Count > 0)
                    {
                        // more than one entry, so disable any that aren't less
                        foreach (var item in fiFile)
                        {
                            if (item.ValueOption == CommandLineArgs.DateCreatedFile.ValueOption && item.Value.Equals(CommandLineArgs.DateCreatedFile.Value.ToString()))
                            {
                                int index = __FilterItems.IndexOf(item);
                                if (index > -1)
                                {
                                    __FilterItems[index].Enabled = true;
                                    foundExact = true;
                                }
                            }
                            else
                            {
                                int index = __FilterItems.IndexOf(item);
                                if (index > -1)
                                {
                                    __FilterItems[index].Enabled = false;
                                }
                            }
                        }
                    }

                    if (fiFile == null || fiFile.Count == 0 || !foundExact)
                    {
                        __FilterItems.Add(new FilterItem(new FilterType(FilterType.Categories.File, FilterType.SubCategories.DateCreated),
                           CommandLineArgs.DateCreatedFile.Value.ToString(), CommandLineArgs.DateCreatedFile.ValueOption, false, true));
                    }
                }

                if (CommandLineArgs.DateCreatedDirectory != null)
                {
                    // check if exists, enable if necessary/set value, or create it
                    var fiDir = GetFilterItemsByFilterType(new FilterType(FilterType.Categories.Directory, FilterType.SubCategories.DateCreated));

                    bool foundExact = false;
                    if (fiDir != null && fiDir.Count > 0)
                    {
                        // more than one entry, so disable any that aren't less
                        foreach (var item in fiDir)
                        {
                            if (item.ValueOption == CommandLineArgs.DateCreatedDirectory.ValueOption && item.Value.Equals(CommandLineArgs.DateCreatedDirectory.Value.ToString()))
                            {
                                int index = __FilterItems.IndexOf(item);
                                if (index > -1)
                                {
                                    __FilterItems[index].Enabled = true;
                                    foundExact = true;
                                }
                            }
                            else
                            {
                                int index = __FilterItems.IndexOf(item);
                                if (index > -1)
                                {
                                    __FilterItems[index].Enabled = false;
                                }
                            }
                        }
                    }

                    if (fiDir == null || fiDir.Count == 0 || !foundExact)
                    {
                        __FilterItems.Add(new FilterItem(new FilterType(FilterType.Categories.Directory, FilterType.SubCategories.DateCreated),
                           CommandLineArgs.DateCreatedDirectory.Value.ToString(), CommandLineArgs.DateCreatedDirectory.ValueOption, false, true));
                    }
                }

                if (CommandLineArgs.MinFileSize != null)
                {
                    // check if exists, enable if necessary/set value, or create it
                    var fiFile = GetFilterItemsByFilterType(new FilterType(FilterType.Categories.File, FilterType.SubCategories.Size));

                    bool foundExact = false;
                    if (fiFile != null && fiFile.Count > 0)
                    {
                        // more than one entry, so disable any that aren't less
                        foreach (var item in fiFile)
                        {
                            if (item.ValueOption == CommandLineArgs.MinFileSize.ValueOption && item.Value.Equals(CommandLineArgs.MinFileSize.Value.ToString()))
                            {
                                int index = __FilterItems.IndexOf(item);
                                if (index > -1)
                                {
                                    __FilterItems[index].Enabled = true;
                                    foundExact = true;
                                }
                            }
                            else
                            {
                                int index = __FilterItems.IndexOf(item);
                                if (index > -1)
                                {
                                    __FilterItems[index].Enabled = false;
                                }
                            }
                        }
                    }

                    if (fiFile == null || fiFile.Count == 0 || !foundExact)
                    {
                        __FilterItems.Add(new FilterItem(new FilterType(FilterType.Categories.File, FilterType.SubCategories.Size),
                           CommandLineArgs.MinFileSize.Value.ToString(), CommandLineArgs.MinFileSize.ValueOption, false, "byte", true));
                    }
                }

                if (CommandLineArgs.MaxFileSize != null)
                {
                    // check if exists, enable if necessary/set value, or create it
                    var fiFile = GetFilterItemsByFilterType(new FilterType(FilterType.Categories.File, FilterType.SubCategories.Size));

                    bool foundExact = false;
                    if (fiFile != null && fiFile.Count > 0)
                    {
                        // more than one entry, so disable any that aren't less
                        foreach (var item in fiFile)
                        {
                            if (item.ValueOption == CommandLineArgs.MaxFileSize.ValueOption && item.Value.Equals(CommandLineArgs.MaxFileSize.Value.ToString()))
                            {
                                int index = __FilterItems.IndexOf(item);
                                if (index > -1)
                                {
                                    __FilterItems[index].Enabled = true;
                                    foundExact = true;
                                }
                            }
                            else
                            {
                                int index = __FilterItems.IndexOf(item);
                                if (index > -1)
                                {
                                    __FilterItems[index].Enabled = false;
                                }
                            }
                        }
                    }

                    if (fiFile == null || fiFile.Count == 0 || !foundExact)
                    {
                        __FilterItems.Add(new FilterItem(new FilterType(FilterType.Categories.File, FilterType.SubCategories.Size),
                           CommandLineArgs.MaxFileSize.Value.ToString(), CommandLineArgs.MaxFileSize.ValueOption, false, "byte", true));
                    }
                }

                if (CommandLineArgs.MinFileCount > 0)
                {
                    // check if exists, enable if necessary/set value, or create it
                    var fiFile = GetFilterItemsByFilterType(new FilterType(FilterType.Categories.File, FilterType.SubCategories.MinimumHitCount));

                    if (fiFile == null || fiFile.Count == 0)
                    {
                        __FilterItems.Add(new FilterItem(new FilterType(FilterType.Categories.File, FilterType.SubCategories.MinimumHitCount),
                           CommandLineArgs.MinFileCount.ToString(), FilterType.ValueOptions.None, false, true));
                    }
                    else
                    {
                        int index = __FilterItems.IndexOf(fiFile.First());
                        if (index > -1)
                        {
                            __FilterItems[index].Enabled = true;
                            __FilterItems[index].Value = CommandLineArgs.MinFileCount.ToString();
                        }
                    }
                }

                if (CommandLineArgs.ReadOnlyFile)
                {
                    // check if exists, enable if necessary/set value, or create it
                    var fiFile = GetFilterItemsByFilterType(new FilterType(FilterType.Categories.File, FilterType.SubCategories.ReadOnly));

                    if (fiFile == null || fiFile.Count == 0)
                    {
                        __FilterItems.Add(new FilterItem(new FilterType(FilterType.Categories.File, FilterType.SubCategories.ReadOnly),
                           string.Empty, FilterType.ValueOptions.None, false, true));
                    }
                    else
                    {
                        int index = __FilterItems.IndexOf(fiFile.First());
                        if (index > -1)
                        {
                            __FilterItems[index].Enabled = true;
                        }
                    }
                }

                // keep last to ensure all options are set before a search begins
                if (CommandLineArgs.StartSearch)
                {
                    btnSearch_Click(null, null);
                    this.Show();
                    this.Refresh();
                }
            }
        }

        /// <summary>
        /// Gets a list of FilterItems that are the given FilterType.
        /// </summary>
        /// <param name="ft">Desired FilterType</param>
        /// <returns>List of FilterItems</returns>

        private List<FilterItem> GetFilterItemsByFilterType(FilterType ft)
        {
            return __FilterItems.FindAll(f => f.FilterType == ft);
        }

        /// <summary>
        /// Set the view states of the controls.
        /// </summary>

        private void LoadViewStates()
        {
            // set view state of results preview
            __FileListHeight = GeneralSettings.WindowFilePanelHeight;
        }
        /// <summary>
        /// Load Windows DPI settings and set window and column positions and fonts to defaults if setting has changed.
        /// </summary>

        private void LoadDPISettings()
        {
            using (var graphics = CreateGraphics())
            {
                // get Windows DPI percent scale setting
                int dpi_percent = API.GetCurrentDPISettingPerCent(graphics);

                // set window and column positions and fonts to defaults if DPI percent scale setting has changed
                if (dpi_percent != GeneralSettings.WindowsDPIPerCentSetting)
                {
                    // set default window and column positions
                    GeneralSettingsReset.WindowSetDefaultPositions();
                    GeneralSettingsReset.WindowFileSetDefaultPositions();
                    GeneralSettingsReset.LogDisplaySetDefaultPositions();
                    GeneralSettingsReset.ExclusionsDisplaySetDefaultPositions();

                    // if first start for version with this variable
                    if (GeneralSettings.SetDefaultFonts)
                    {
                        // set default fonts
                        GeneralSettings.FilePanelFont = GeneralSettings.FilePanelFontDefault;
                        GeneralSettings.ResultsFont = GeneralSettings.ResultsFontDefault;
                    }
                }

                // disable after first start
                GeneralSettings.SetDefaultFonts = false;

                // store setting in global variables
                GeneralSettings.WindowsDPIPerCentSetting = dpi_percent;
            }
        }

        #endregion

        #region Menu Events
        /// <summary>
        /// Enables file menu items when necessary.
        /// </summary>
        /// <param name="sender">System parm</param>
        /// <param name="e">System parm</param>

        private void FileMenu_DropDownOpening(object sender, EventArgs e)
        {
            SaveResultsMenuItem.Enabled = PrintResultsMenuItem.Enabled = lstFileNames.Items.Count > 0;
        }

        /// <summary>
        /// Create a new search window.
        /// </summary>
        /// <param name="sender">system parameter</param>
        /// <param name="e">system parameter</param>

        private void NewWindowMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(ApplicationPaths.ExecutingAssembly);
        }

        /// <summary>
        /// Select a folder for the search path.
        /// </summary>
        /// <param name="sender">system parameter</param>
        /// <param name="e">system parameter</param>

        private void SelectPathMenuItem_Click(object sender, System.EventArgs e)
        {
            BrowseForFolder();
        }

        /// <summary>
        /// Save the results to a file
        /// </summary>
        /// <param name="sender">System parm</param>
        /// <param name="e">System parm</param>

        private void SaveResultsMenuItem_Click(object sender, System.EventArgs e)
        {
            // only show dialog if information to save
            if (lstFileNames.Items.Count <= 0)
            {
                MessageBox.Show(Language.GetGenericText("SaveNoResults"), ProductInformation.ApplicationName, MessageBoxButtons.OK,
                                MessageBoxIcon.Information);
                return;
            }

            var dlg = new SaveFileDialog
                         {
                             CheckPathExists = true,
                             AddExtension = true,
                             Title = Language.GetGenericText("SaveDialogTitle"),
                             Filter = "Text (*.txt)|*.txt|HTML (*.html)|*.html|XML (*.xml)|*.xml|JSON (*.json)|*.json"
                         };


            if (dlg.ShowDialog(this) == DialogResult.OK)
            {
                switch (dlg.FilterIndex)
                {
                    case 1:
                        // Save to text
                        OutputResults(dlg.FileName, MatchResultsExport.SaveResultsAsText);
                        break;
                    case 2:
                        // Save to html
                        OutputResults(dlg.FileName, MatchResultsExport.SaveResultsAsHTML);
                        break;
                    case 3:
                        // Save to xml
                        OutputResults(dlg.FileName, MatchResultsExport.SaveResultsAsXML);
                        break;
                    case 4:
                        // Save to json
                        OutputResults(dlg.FileName, MatchResultsExport.SaveResultsAsJSON);
                        break;
                }
            }
        }

        /// <summary>
        /// Output results using given export delegate.
        /// </summary>
        /// <param name="filename">Current filename</param>
        /// <param name="outputter">File delegate</param>

        private void OutputResults(string filename, MatchResultsExport.FileDelegate outputter)
        {
            SetStatusBarMessage(String.Format(Language.GetGenericText("SaveSaving"), filename));

            try
            {
                // get all grep indexes
                IEnumerable<ListViewItem> lv = lstFileNames.Items.Cast<ListViewItem>();
                var indexes = (from i in lv select int.Parse(i.SubItems[Constants.COLUMN_INDEX_GREP_INDEX].Text)).ToList();

                MatchResultsExport.ExportSettings settings = new MatchResultsExport.ExportSettings()
                {
                    Path = filename,
                    Grep = __Grep,
                    GrepIndexes = indexes,
                    ShowLineNumbers = LineNumbersMenuItem.Checked,
                    RemoveLeadingWhiteSpace = RemoveWhiteSpaceMenuItem.Checked
                };
                outputter(settings);
                SetStatusBarMessage(Language.GetGenericText("SaveSaved"));
            }
            catch (Exception ex)
            {
                LogClient.Instance.Logger.Error("Save Results Error: {0}", LogClient.GetAllExceptions(ex));

                MessageBox.Show(
                    String.Format(Language.GetGenericText("SaveError"), ex),
                    ProductInformation.ApplicationName,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Show Print Dialog
        /// </summary>
        /// <param name="sender">system parm</param>
        /// <param name="e">system parm</param>

        private void PrintResultsMenuItem_Click(object sender, EventArgs e)
        {
            if (lstFileNames.Items.Count > 0)
            {
                // get all grep indexes
                IEnumerable<ListViewItem> lv = lstFileNames.Items.Cast<ListViewItem>();
                var indexes = (from i in lv where i.Selected select int.Parse(i.SubItems[Constants.COLUMN_INDEX_GREP_INDEX].Text)).ToList();

                MatchResultsExport.ExportSettings settings = new MatchResultsExport.ExportSettings()
                {
                    Grep = __Grep,
                    GrepIndexes = indexes,
                    ShowLineNumbers = LineNumbersMenuItem.Checked,
                    RemoveLeadingWhiteSpace = RemoveWhiteSpaceMenuItem.Checked
                };

                using (var printForm = new frmPrint(settings, Convertors.ConvertStringToFont(GeneralSettings.ResultsFont), Icon))
                {
                    printForm.ShowDialog(this);
                }
            }
            else
                MessageBox.Show(Language.GetGenericText("PrintNoResults"), ProductInformation.ApplicationName, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>
        /// Closes the program.
        /// </summary>
        /// <param name="sender">System parm</param>
        /// <param name="e">System parm</param>

        private void ExitMenuItem_Click(object sender, System.EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Enables edit menu items when necessary.
        /// </summary>
        /// <param name="sender">System parm</param>
        /// <param name="e">System parm</param>

        private void EditMenu_DropDownOpening(object sender, EventArgs e)
        {
            SelectAllMenuItem.Enabled = OpenSelectedMenuItem.Enabled = lstFileNames.Items.Count > 0;
        }

        /// <summary>
        /// Menu Select All Event
        /// </summary>
        /// <param name="sender">System parm</param>
        /// <param name="e">System parm</param>

        private void SelectAllMenuItem_Click(object sender, System.EventArgs e)
        {
            SelectAllListItems();
        }

        /// <summary>
        /// Enables view menu items when necessary.
        /// </summary>
        /// <param name="sender">system parameter</param>
        /// <param name="e">system parameter</param>

        private void ViewMenu_DropDownOpening(object sender, EventArgs e)
        {
            StatusMessageMenuItem.Enabled = GetLogItemsCountByType(LogItem.LogItemTypes.Status) > 0;
            ExclusionMessageMenuItem.Enabled = GetLogItemsCountByType(LogItem.LogItemTypes.Exclusion) > 0;
            ErrorMessageMenuItem.Enabled = GetLogItemsCountByType(LogItem.LogItemTypes.Error) > 0;
            AllMessageMenuItem.Enabled = AnyLogItems();
        }

        /// <summary>
        /// View status messages.
        /// </summary>
        /// <param name="sender">system parameter</param>
        /// <param name="e">system parameter</param>

        private void StatusMessageMenuItem_Click(object sender, System.EventArgs e)
        {
            DisplaySearchMessages(LogItem.LogItemTypes.Status);
        }

        /// <summary>
        /// View exclusion messages.
        /// </summary>
        /// <param name="sender">system parameter</param>
        /// <param name="e">system parameter</param>

        private void ExclusionMessageMenuItem_Click(object sender, System.EventArgs e)
        {
            DisplaySearchMessages(LogItem.LogItemTypes.Exclusion);
        }

        /// <summary>
        /// View error messages.
        /// </summary>
        /// <param name="sender">system parameter</param>
        /// <param name="e">system parameter</param>

        private void ErrorMessageMenuItem_Click(object sender, System.EventArgs e)
        {
            DisplaySearchMessages(LogItem.LogItemTypes.Error);
        }

        /// <summary>
        /// View all messages.
        /// </summary>
        /// <param name="sender">system parameter</param>
        /// <param name="e">system parameter</param>

        private void AllMessageMenuItem_Click(object sender, System.EventArgs e)
        {
            DisplaySearchMessages(null);
        }

        /// <summary>
        /// Zoom in view by 1.
        /// </summary>
        /// <param name="sender">system parameter</param>
        /// <param name="e">system parameter</param>

        private void ZoomInMenuItem_Click(object sender, EventArgs e)
        {
            txtHits.ZoomIn();
        }

        /// <summary>
        /// Zoom out view by 1.
        /// </summary>
        /// <param name="sender">system parameter</param>
        /// <param name="e">system parameter</param>

        private void ZoomOutMenuItem_Click(object sender, EventArgs e)
        {
            txtHits.ZoomOut();
        }

        /// <summary>
        /// Reset zoom level to initial value.
        /// </summary>
        /// <param name="sender">system parameter</param>
        /// <param name="e">system parameter</param>

        private void ZoomRestoreMenuItem_Click(object sender, EventArgs e)
        {
            txtHits.ZoomReset();
        }

        /// <summary>
        /// Option to toggle line numbers being displayed.
        /// </summary>
        /// <param name="sender">system parameter</param>
        /// <param name="e">system parameter</param>

        private void LineNumbersMenuItem_Click(object sender, System.EventArgs e)
        {
            LineNumbersMenuItem.Checked = !LineNumbersMenuItem.Checked;

            txtHits.ShowLineNumbers = LineNumbersMenuItem.Checked;
        }

        /// <summary>
        /// Option to toggle word wrapping.
        /// </summary>
        /// <param name="sender">system parameter</param>
        /// <param name="e">system parameter</param>

        private void WordWrapMenuItem_Click(object sender, System.EventArgs e)
        {
            WordWrapMenuItem.Checked = !WordWrapMenuItem.Checked;

            txtHits.WordWrap = WordWrapMenuItem.Checked;
        }

        /// <summary>
        /// Option to remove leading white space from each line.
        /// </summary>
        /// <param name="sender">system parameter</param>
        /// <param name="e">system parameter</param>

        private void RemoveWhiteSpaceMenuItem_Click(object sender, System.EventArgs e)
        {
            RemoveWhiteSpaceMenuItem.Checked = !RemoveWhiteSpaceMenuItem.Checked;

            if (lstFileNames.SelectedItems.Count > 0)
            {
                lstFileNames_SelectedIndexChanged(null, null);
            }
            else if (txtHits.Text.Length > 0)
            {
                ProcessAllMatchesForDisplay();
            }
        }

        /// <summary>
        /// Option to view entire file contents.
        /// </summary>
        /// <param name="sender">system parameter</param>
        /// <param name="e">system parameter</param>

        private void EntireFileMenuItem_Click(object sender, System.EventArgs e)
        {
            EntireFileMenuItem.Checked = !EntireFileMenuItem.Checked;

            lstFileNames_SelectedIndexChanged(null, null);
        }

        /// <summary>
        /// View all results.
        /// </summary>
        /// <param name="sender">system parameter</param>
        /// <param name="e">system parameter</param>

        private void AllResultsMenuItem_Click(object sender, System.EventArgs e)
        {
            foreach (ListViewItem item in lstFileNames.Items)
            {
                item.Selected = false;
            }

            ProcessAllMatchesForDisplay();
        }

        /// <summary>
        /// Open Selected Files Event
        /// </summary>
        /// <param name="sender">System parm</param>
        /// <param name="e">System parm</param>

        private void OpenSelectedMenuItem_Click(object sender, System.EventArgs e)
        {
            for (int i = 0; i < lstFileNames.SelectedItems.Count; i++)
            {
                var match = __Grep.RetrieveMatchResult(int.Parse(lstFileNames.SelectedItems[i].SubItems[Constants.COLUMN_INDEX_GREP_INDEX].Text));
                TextEditors.Open(TextEditorOpener.FromMatch(match, __Grep.SearchSpec.SearchText));
            }
        }

        /// <summary>
        /// Clear MRU Event
        /// </summary>
        /// <param name="sender">System parm</param>
        /// <param name="e">System parm</param>

        private void ClearMRUAllMenuItem_Click(object sender, System.EventArgs e)
        {
            cboFilePath.Items.Clear();
            cboFilePathDest.Items.Clear();
            cboFileName.Items.Clear();
            cboSearchForText.Items.Clear();
            cboSearchForText2.Items.Clear();

            GeneralSettings.SearchStarts = string.Empty;
            GeneralSettings.CopyDestination = string.Empty;
            GeneralSettings.SearchFilters = string.Empty;
            GeneralSettings.SearchTexts = string.Empty;
            GeneralSettings.SearchTextsTwo = string.Empty;
            GeneralSettings.Save();
        }

        /// <summary>
        /// Clear Search Paths MRU event.
        /// </summary>
        /// <param name="sender">system parameter</param>
        /// <param name="e">system parameter</param>

        private void ClearMRUPathsMenuItem_Click(object sender, EventArgs e)
        {
            cboFilePath.Items.Clear();

            GeneralSettings.SearchStarts = string.Empty;
            GeneralSettings.CopyDestination = string.Empty;
            GeneralSettings.Save();
        }

        /// <summary>
        /// Clear File Types MRU event.
        /// </summary>
        /// <param name="sender">system parameter</param>
        /// <param name="e">system parameter</param>

        private void ClearMRUTypesMenuItem_Click(object sender, EventArgs e)
        {
            cboFileName.Items.Clear();

            GeneralSettings.SearchFilters = string.Empty;
            GeneralSettings.Save();
        }

        /// <summary>
        /// Clear Search Text MRU event.
        /// </summary>
        /// <param name="sender">system parameter</param>
        /// <param name="e">system parameter</param>

        private void ClearMRUTextsMenuItem_Click(object sender, EventArgs e)
        {
            cboSearchForText.Items.Clear();

            GeneralSettings.SearchTexts = string.Empty;
            GeneralSettings.SearchTextsTwo = string.Empty;
            GeneralSettings.Save();
        }

        /// <summary>
        /// Save Search Settings Event
        /// </summary>
        /// <param name="sender">system parm</param>
        /// <param name="e">system parm</param>

        private void SaveSearchOptionsMenuItem_Click(object sender, System.EventArgs e)
        {
            if (VerifyInterface())
            {
                SaveSearchSettings();
            }
        }

        /// <summary>
        /// Menu Options Event
        /// </summary>
        /// <param name="sender">System parm</param>
        /// <param name="e">System parm</param>

        private void OptionsMenuItem_Click(object sender, System.EventArgs e)
        {
            var optionsForm = new frmOptions();
            var previousUseEncodingCache = GeneralSettings.UseEncodingCache;
            var previousEncodingPerformance = GeneralSettings.EncodingPerformance;

            if (optionsForm.ShowDialog(this) == DialogResult.OK)
            {
                //update combobox lengths
                while (cboFilePath.Items.Count > GeneralSettings.MaximumMRUPaths)
                    cboFilePath.Items.RemoveAt(cboFilePath.Items.Count - 1);
                while (cboFileName.Items.Count > GeneralSettings.MaximumMRUPaths)
                    cboFileName.Items.RemoveAt(cboFileName.Items.Count - 1);
                while (cboSearchForText.Items.Count > GeneralSettings.MaximumMRUPaths)
                    cboSearchForText.Items.RemoveAt(cboSearchForText.Items.Count - 1);

                // load new language if necessary
                if (optionsForm.IsLanguageChange)
                {
                    Language.ProcessForm(this, this.toolTip1);

                    SetColumnsText();
                    SetWindowText();

                    // clear statusbar text
                    SetStatusBarMessage(string.Empty);
                    CalculateTotalCount();
                }

                // reload the text editor settings
                LoadTextEditorSettings();

                lstFileNames.Font = Convertors.ConvertStringToFont(GeneralSettings.FilePanelFont);

                if (GeneralSettings.UsebSearchAccentColor)
                {
                    lblSearchHeading.ForeColor = ProductInformation.ApplicationColor;
                    lblSearchOptions.ForeColor = ProductInformation.ApplicationColor;
                }
                else
                {
                    lblSearchHeading.ForeColor = Color.Black;
                    lblSearchOptions.ForeColor = Color.Black;
                }

                // update current display
                if (lstFileNames.SelectedItems.Count > 0)
                {
                    lstFileNames_SelectedIndexChanged(null, null);
                }
                else if (txtHits.Text.Length > 0)
                {
                    ProcessAllMatchesForDisplay();
                }
            }
        }

        /// <summary>
        /// Shows help file (.chm) to user.
        /// </summary>
        /// <param name="sender">system parameter</param>
        /// <param name="e">system parameter</param>

        private void ViewHelpMenuItem_Click(object sender, EventArgs e)
        {
            //TODO: support currently selected language help file (b-Search-Help-en-us.chm, b-Search-Help-da-dk.chm, etc.)
            //Help.ShowHelp(this, Path.Combine(Constants.ProductLocation, "b-Search-Help.chm"));

            System.Diagnostics.Process.Start(ProductInformation.HelpUrl);
        }

        /// <summary>
        /// Shows regular expression help to user.
        /// </summary>
        /// /// <param name="sender">system parameter</param>
        /// <param name="e">system parameter</param>

        private void ViewRegExHelpMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(ProductInformation.RegExHelpUrl);
        }

        /// <summary>
        /// Open folder container log file(s).
        /// </summary>
        /// <param name="sender">system parameter</param>
        /// <param name="e">system parameter</param>

        private void LogFileMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(ApplicationPaths.LogFile);
        }

        /// <summary>
        /// Displays update check window to user.
        /// </summary>
        /// <param name="sender">system parameter</param>
        /// <param name="e">system parameter</param>

        private void CheckForUpdateMenuItem_Click(object sender, EventArgs e)
        {
            var dlg = new frmCheckForUpdateTemp();
            dlg.ShowDialog(this);
        }

        /// <summary>
        /// Menu About Event
        /// </summary>
        /// <param name="sender">System parm</param>
        /// <param name="e">System parm</param>

        private void AboutMenuItem_Click(object sender, EventArgs e)
        {
            var dlg = new frmAbout();
            dlg.ShowDialog(this);
        }

        /// <summary>
        /// Sends all selected items from the file list to the clipboard
        /// </summary>
        /// <param name="sender">system parameter</param>
        /// <param name="e">system parameter</param>

        private void CopyMenuItem_Click(object sender, System.EventArgs e)
        {
            if (lstFileNames.SelectedItems.Count <= 0)
                return;

            System.Text.StringBuilder data = new System.Text.StringBuilder();
            try
            {
                foreach (ListViewItem lvi in lstFileNames.SelectedItems)
                {
                    data.Append(lvi.Text);

                    // skip first and last columns (filename, sort order)
                    for (int i = 0; i < lvi.SubItems.Count; i++)
                    {
                        if (i != 0 && i != lvi.SubItems.Count - 1)
                        {
                            var subLvi = lvi.SubItems[i];
                            data.Append(", ");
                            data.Append(subLvi.Text);
                        }
                    }
                    data.Append(Environment.NewLine);
                }
                Clipboard.SetDataObject(data.ToString());
            }
            catch (Exception ex)
            {
                MessageBox.Show("Exception: " + ex.Message, "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Sends all selected items from the file list to the clipboard
        /// </summary>
        /// <param name="sender">system parameter</param>
        /// <param name="e">system parameter</param>

        private void CopyNameMenuItem_Click(object sender, System.EventArgs e)
        {
            if (lstFileNames.SelectedItems.Count <= 0)
                return;

            System.Text.StringBuilder data = new System.Text.StringBuilder();
            try
            {
                foreach (ListViewItem lvi in lstFileNames.SelectedItems)
                {
                    data.Append(lvi.Text);
                    data.Append(Environment.NewLine);
                }
                Clipboard.SetDataObject(data.ToString());
            }
            catch (Exception ex)
            {
                MessageBox.Show("Exception: " + ex.Message, "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Sends all selected items from the file list to the clipboard
        /// </summary>
        /// <param name="sender">system parameter</param>
        /// <param name="e">system parameter</param>

        private void CopyLocatedInMenuItem_Click(object sender, System.EventArgs e)
        {
            if (lstFileNames.SelectedItems.Count <= 0)
                return;

            System.Text.StringBuilder data = new System.Text.StringBuilder();
            try
            {
                foreach (ListViewItem lvi in lstFileNames.SelectedItems)
                {
                    data.Append(lvi.SubItems[1].Text);
                    data.Append(Environment.NewLine);
                }
                Clipboard.SetDataObject(data.ToString());
            }
            catch (Exception ex)
            {
                MessageBox.Show("Exception: " + ex.Message, "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Sends all selected items from the file list to the clipboard
        /// </summary>
        /// <param name="sender">system parameter</param>
        /// <param name="e">system parameter</param>

        private void CopyLocatedInAndNameMenuItem_Click(object sender, System.EventArgs e)
        {
            if (lstFileNames.SelectedItems.Count <= 0)
                return;

            System.Text.StringBuilder data = new System.Text.StringBuilder();
            try
            {
                foreach (ListViewItem lvi in lstFileNames.SelectedItems)
                {
                    data.AppendFormat("{0}{1}{2}", lvi.SubItems[1].Text, Path.DirectorySeparatorChar.ToString(), lvi.Text);
                    data.Append(Environment.NewLine);
                }
                Clipboard.SetDataObject(data.ToString());
            }
            catch (Exception ex)
            {
                MessageBox.Show("Exception: " + ex.Message, "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Need certain keyboard events on the lstFileNames.
        /// </summary>
        /// <param name="sender">system parameter</param>
        /// <param name="e">system parameter</param>

        private void lstFileNames_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            //ctrl+c  Copy to clipboard
            if (e.KeyCode == Keys.C && e.Control)
            {
                CopyMenuItem_Click(sender, EventArgs.Empty);
            }

            //ctrl+a  Select All
            if (e.KeyCode == Keys.A && e.Control)
            {
                SelectAllListItems();
            }

            //enter Edit selected file
            if (e.KeyCode == Keys.Enter)
            {
                OpenSelectedMenuItem_Click(null, null);
            }

            // ** I think the delete key is a bad idea, because its too easy to modify the results by mistake.
            //delete Delete selected from list
            //if (e.KeyCode == Keys.Delete)
            //{
            //   DeleteMenuItem_Click(sender, EventArgs.Empty);
            //}
        }

        /// <summary>
        /// Context Menu item for opening selected files
        /// </summary>
        /// <param name="sender">system parameter</param>
        /// <param name="e">system parameter</param>

        private void OpenMenuItem_Click(object sender, System.EventArgs e)
        {
            OpenSelectedMenuItem_Click(sender, e);
        }

        /// <summary>
        /// Context Menu item for opening selected file's Directory
        /// </summary>
        /// <param name="sender">system parameter</param>
        /// <param name="e">system parameter</param>

        private void OpenFolderMenuItem_Click(object sender, System.EventArgs e)
        {
            try
            {
                foreach (ListViewItem lvi in lstFileNames.SelectedItems)
                {
                    string filename = Path.Combine(lvi.SubItems[Constants.COLUMN_INDEX_DIRECTORY].Text, lvi.SubItems[Constants.COLUMN_INDEX_FILE].Text);
                    if (File.Exists(filename))
                    {
                        // explorer argument format: "/select, " + filename;
                        string fileSelect = string.Format("/select, \"{0}\"", filename);
                        System.Diagnostics.Process.Start("Explorer.exe", fileSelect);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Exception: " + ex.Message, "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Context Menu item for opening selected file with associated application.
        /// </summary>
        /// <param name="sender">system parameter</param>
        /// <param name="e">system parameter</param>

        private void OpenWithAssociatedApp_Click(object sender, System.EventArgs e)
        {
            try
            {
                string path;
                MatchResult hit;

                for (int i = 0; i < lstFileNames.SelectedItems.Count; i++)
                {
                    // retrieve hit object
                    hit = __Grep.RetrieveMatchResult(int.Parse(lstFileNames.SelectedItems[i].SubItems[Constants.COLUMN_INDEX_GREP_INDEX].Text));

                    // retrieve the filename
                    path = hit.File.FullName;

                    TextEditors.OpenWithDefault(path);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Exception: " + ex.Message, "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Context Menu Item for deleting items from the list.
        /// </summary>
        /// <param name="sender">system parameter</param>
        /// <param name="e">system parameter</param>

        private void DeleteMenuItem_Click(object sender, System.EventArgs e)
        {
            if (lstFileNames.SelectedItems.Count <= 0)
                return;

            try
            {
                lstFileNames.BeginUpdate();
                while (lstFileNames.SelectedItems.Count > 0)
                {
                    lstFileNames.SelectedItems[0].Remove();
                }
                lstFileNames.EndUpdate();

                // update counts
                CalculateTotalCount();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Exception: " + ex.Message, "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Handles deleting the selected files by using the recycle bin.
        /// </summary>
        /// <param name="sender">system parameter</param>
        /// <param name="e">system parameter</param>

        private void FileDeleteMenuItem_Click(object sender, EventArgs e)
        {
            Dictionary<string, string> files = new Dictionary<string, string>();

            for (int i = 0; i < lstFileNames.SelectedItems.Count; i++)
            {
                // retrieve hit object
                var hit = __Grep.RetrieveMatchResult(int.Parse(lstFileNames.SelectedItems[i].SubItems[Constants.COLUMN_INDEX_GREP_INDEX].Text));
                var index = lstFileNames.SelectedItems[i].SubItems[Constants.COLUMN_INDEX_GREP_INDEX].Text;

                files.Add(index, hit.File.FullName);
            }

            lstFileNames.BeginUpdate();
            foreach (var file in files)
            {
                // recycle bin delete
                API.FileDeletion.Delete(file.Value);

                // remove from list
                ListViewItem[] items = lstFileNames.Items.Find(file.Key, false);
                if (items != null && items.Length == 1)
                {
                    items[0].Remove();
                }
            }
            lstFileNames.EndUpdate();

            // clear results area since all selected files are removed
            if (files.Count > 0)
            {
                txtHits.Clear();
                CalculateTotalCount();
            }
        }

        /// <summary>
        /// Handles copying the selected files to the clipboard.
        /// </summary>
        /// <param name="sender">system parameter</param>
        /// <param name="e">system parameter</param>

        private void FileCopyMenuItem_Click(object sender, EventArgs e)
        {
            System.Collections.Specialized.StringCollection files = new System.Collections.Specialized.StringCollection();

            for (int i = 0; i < lstFileNames.SelectedItems.Count; i++)
            {
                // retrieve hit object
                var hit = __Grep.RetrieveMatchResult(int.Parse(lstFileNames.SelectedItems[i].SubItems[Constants.COLUMN_INDEX_GREP_INDEX].Text));

                // retrieve the filename
                files.Add(hit.File.FullName);
            }

            if (files.Count > 0)
            {
                Clipboard.SetFileDropList(files);
            }
        }

        /// <summary>
        /// Handle donate menu item selection.
        /// </summary>
        /// <param name="sender">system parameter</param>
        /// <param name="e">system parameter</param>

        private void donateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(ProductInformation.DonationUrl);
        }

        #endregion

        #region Grep Events
        /// <summary>
        /// Handles the Grep object's SearchingFile event
        /// </summary>
        /// <param name="file">FileInfo object containg currently being search file</param>

        private void ReceiveSearchingFile(System.IO.FileInfo file)
        {
            const string languageLookupText = "SearchSearching";
            LogItems.Add(new LogItem(LogItem.LogItemTypes.Status, languageLookupText, file.FullName));

            SetStatusBarMessage(string.Format(Language.GetGenericText(languageLookupText), TruncateFileName(file, stbStatus)));
        }

        /// <summary>
        /// Handles the Grep object's SearchingFileByPlugin event
        /// </summary>
        /// <param name="pluginName">Name of plugin currently searching file</param>

        private void ReceiveSearchingFileByPlugin(string pluginName)
        {
            const string languageLookupText = "SearchSearchingByPlugin";
            LogItems.Add(new LogItem(LogItem.LogItemTypes.Status, languageLookupText, pluginName));

            SetStatusBarMessage(string.Format(Language.GetGenericText(languageLookupText), pluginName));
        }

        /// <summary>
        /// Handles the Grep object's FileEncodingDetected event
        /// </summary>
        /// <param name="file">FileInfo object containg currently being search file</param>
        /// <param name="encoding">File's detected System.Text.Encoding</param>
        /// <param name="usedEncoder">The used encoder's name</param>

        private void ReceiveFileEncodingDetected(FileInfo file, System.Text.Encoding encoding, string usedEncoder)
        {
            const string languageLookupText = "SearchSearching";

            // find LogItem by SearchSearching and file.FullName
            // update details to include encoding
            foreach (LogItem item in LogItems)
            {
                if (item.Value == languageLookupText &&
                   item.Details == file.FullName)
                {
                    item.Details = string.Format("{0}||{1} [{2}]", file.FullName, encoding.EncodingName, usedEncoder);
                }
            }
        }

        /// <summary>
        /// A file has been detected to contain the searching text
        /// </summary>
        /// <param name="file">File detected to contain searching text</param>
        /// <param name="index">Position in GrepCollection</param>

        private void ReceiveFileHit(System.IO.FileInfo file, int index)
        {
            AddHitToList(file, index);
            CalculateTotalCount();
        }

        /// <summary>
        /// A line has been detected to contain the searching text
        /// </summary>
        /// <param name="match">The MatchResult that contains the line</param>
        /// <param name="index">The position in the HitObject's line collection</param>

        private void ReceiveLineHit(MatchResult match, int index)
        {
            UpdateHitCount(match);
            CalculateTotalCount();
        }

        /// <summary>
        /// Receives the search error event when a file search causes an uncatchable error
        /// </summary>
        /// <param name="file">FileInfo object of error file</param>
        /// <param name="ex">Exception</param>

        private void ReceiveSearchError(System.IO.FileInfo file, Exception ex)
        {
            string languageLookupText = "SearchGenericError";

            if (file == null)
            {
                LogItems.Add(new LogItem(LogItem.LogItemTypes.Error, languageLookupText, string.Format("{0}||{1}", string.Empty, ex.Message)));
                LogClient.Instance.Logger.Error("Search error from grep: {0}", LogClient.GetAllExceptions(ex));
            }
            else
            {
                languageLookupText = "SearchFileError";
                LogItems.Add(new LogItem(LogItem.LogItemTypes.Error, languageLookupText, string.Format("{0}||{1}", file.FullName, ex.Message)));
                LogClient.Instance.Logger.Error("Search error from grep for file {0}: {1}", file.FullName, LogClient.GetAllExceptions(ex));
            }

            SetStatusBarErrorCount(GetLogItemsCountByType(LogItem.LogItemTypes.Error));
        }

        /// <summary>
        /// Receives the search cancel event when the grep has been cancelled.
        /// </summary>

        private void ReceiveSearchCancel()
        {
            LogStopSearchMessage("cancel");

            RestoreTaskBarProgress();

            const string languageLookupText = "SearchCancelled";
            LogItems.Add(new LogItem(LogItem.LogItemTypes.Status, languageLookupText));

            SetStatusBarMessage(Language.GetGenericText(languageLookupText));
            SetSearchState(true);
            CalculateTotalCount();

            CheckShowAllResults();

            //ShowExclusionsErrorMessageBox();
        }

        /// <summary>
        /// Receives the search complete event when the grep has completed.
        /// </summary>

        private void ReceiveSearchComplete()
        {
            LogStopSearchMessage("finished");

            RestoreTaskBarProgress();

            const string languageLookupText = "SearchFinished";
            LogItems.Add(new LogItem(LogItem.LogItemTypes.Status, languageLookupText));

            SetStatusBarMessage(Language.GetGenericText(languageLookupText));
            CalculateTotalCount();
            SetSearchState(true);

            CheckShowAllResults();

            ShowExclusionsErrorMessageBox();

            if (CommandLineArgs.AnyArguments)
            {
                if (!string.IsNullOrEmpty(CommandLineArgs.OutputPath))
                {

                    MatchResultsExport.FileDelegate del = MatchResultsExport.SaveResultsAsText;
                    switch (CommandLineArgs.OutputType)
                    {
                        case "xml":
                            del = MatchResultsExport.SaveResultsAsXML;
                            break;

                        case "json":
                            del = MatchResultsExport.SaveResultsAsJSON;
                            break;

                        case "html":
                            del = MatchResultsExport.SaveResultsAsHTML;
                            break;

                        default:
                        case "txt":
                            del = MatchResultsExport.SaveResultsAsText;
                            break;
                    }

                    OutputResults(CommandLineArgs.OutputPath, del);
                }

                if (CommandLineArgs.ExitAfterSearch)
                {
                    this.Close();
                }
            }
        }

        /// <summary>
        /// Checks whether to show all results after a search (thread safe).
        /// </summary>

        private void CheckShowAllResults()
        {
            if (chkAllResultsAfterSearch.InvokeRequired)
            {
                ShowAllResultsCallBack del = CheckShowAllResults;
                chkAllResultsAfterSearch.Invoke(del);
                return;
            }

            if (chkAllResultsAfterSearch.Checked)
            {
                ProcessAllMatchesForDisplay();
            }
        }

        /// <summary>
        /// Handles the Grep object's FileFiltered event
        /// </summary>
        /// <param name="file">FileInfo object containg currently being search file</param>
        /// <param name="filterItem">FilterItem causing the filtering</param>
        /// <param name="filterValue">Value that caused the filtering</param>

        private void ReceiveFileFiltered(System.IO.FileInfo file, FilterItem filterItem, string filterValue)
        {
            LogItems.Add(new LogItem(LogItem.LogItemTypes.Exclusion, file.FullName, string.Format("{0}~~{1}", filterItem.ToString(), filterValue)));
            SetStatusBarFilterCount(GetLogItemsCountByType(LogItem.LogItemTypes.Exclusion));
        }

        /// <summary>
        /// Handles the Grep object's DirectoryFiltered event
        /// </summary>
        /// <param name="dir">DirectoryInfo object containg currently being searched directory</param>
        /// <param name="filterItem">FilterItem causing the filtering</param>
        /// <param name="filterValue">Value that caused the filtering</param>

        private void ReceiveDirectoryFiltered(System.IO.DirectoryInfo dir, FilterItem filterItem, string filterValue)
        {
            LogItems.Add(new LogItem(LogItem.LogItemTypes.Exclusion, dir.FullName, string.Format("{0}~~{1}", filterItem.ToString(), filterValue)));
            SetStatusBarFilterCount(GetLogItemsCountByType(LogItem.LogItemTypes.Exclusion));
        }

        /// <summary>
        /// Used to display search messages (like exclusions, errors) to the user.
        /// </summary>
        /// <param name="displayType">The type of message to display</param>

        private void DisplaySearchMessages(LogItem.LogItemTypes? displayType)
        {
            if (this.InvokeRequired)
            {
                DisplaySearchMessagesCallBack del = new DisplaySearchMessagesCallBack(DisplaySearchMessages);
                this.Invoke(del, new object[1] { displayType });
                return;
            }

            if ((!displayType.HasValue && AnyLogItems()) || (displayType.HasValue && GetLogItemsCountByType(displayType.Value) > 0))
            {
                using (var frm = new frmLogDisplay())
                {
                    frm.LogItems = LogItems.ToList();

                    frm.DefaultFilterType = displayType;
                    frm.StartPosition = FormStartPosition.Manual;

                    Rectangle defaultBounds = new Rectangle(this.Left + 20, this.Bottom - frm.Height - stbStatus.Height - 20, this.Width - 40, frm.Height);

                    int width = GeneralSettings.LogDisplaySavePosition && GeneralSettings.LogDisplayWidth != -1 ? GeneralSettings.LogDisplayWidth : defaultBounds.Width;
                    int height = GeneralSettings.LogDisplaySavePosition && GeneralSettings.LogDisplayHeight != -1 ? GeneralSettings.LogDisplayHeight : defaultBounds.Height;
                    int left = GeneralSettings.LogDisplaySavePosition && GeneralSettings.LogDisplayLeft != -1 ? GeneralSettings.LogDisplayLeft : defaultBounds.X;
                    int top = GeneralSettings.LogDisplaySavePosition && GeneralSettings.LogDisplayTop != -1 ? GeneralSettings.LogDisplayTop : defaultBounds.Y;

                    frm.Bounds = new Rectangle(left, top, width, height);

                    // form can't find a screen to fit on, so reset to default on primary screen
                    if (!Screen.AllScreens.Any(s => s.WorkingArea.IntersectsWith(frm.Bounds)))
                    {
                        frm.Bounds = defaultBounds;
                    }

                    frm.ShowDialog(this);
                }
            }
        }

        /// <summary>
        /// Updates the count column (Thread safe)
        /// </summary>
        /// <param name="match">MatchResult that contains updated information</param>

        private void UpdateHitCount(MatchResult match)
        {
            // Makes this a thread safe operation
            if (lstFileNames.InvokeRequired)
            {
                UpdateHitCountCallBack del = new UpdateHitCountCallBack(UpdateHitCount);
                lstFileNames.Invoke(del, new object[1] { match });
                return;
            }

            // find correct item to update
            foreach (ListViewItem item in lstFileNames.Items)
            {
                if (int.Parse(item.SubItems[Constants.COLUMN_INDEX_GREP_INDEX].Text) == match.Index)
                {
                    item.SubItems[Constants.COLUMN_INDEX_COUNT].Text = match.HitCount.ToString();
                    break;
                }
            }
        }

        /// <summary>
        /// Set the status bar message text. (Thread Safe)
        /// </summary>
        /// <param name="message">Text to display</param>

        private void SetStatusBarMessage(string message)
        {
            if (stbStatus.InvokeRequired)
            {
                UpdateStatusMessageCallBack _delegate = SetStatusBarMessage;
                stbStatus.Invoke(_delegate, new object[1] { message });
                return;
            }

            sbStatusPanel.Text = message;
        }

        /// <summary>
        /// Set the status bar text for the encoding name. (Thread Safe)
        /// </summary>
        /// <param name="encodingName">Encoding display name</param>

        private void SetStatusBarEncoding(string encodingName)
        {
            if (stbStatus.InvokeRequired)
            {
                UpdateStatusMessageCallBack _delegate = new UpdateStatusMessageCallBack(SetStatusBarEncoding);
                stbStatus.Invoke(_delegate, new object[1] { encodingName });
                return;
            }

            sbEncodingPanel.Text = encodingName;

            // setup borders depending on value
            if (string.IsNullOrEmpty(encodingName))
            {
                sbEncodingPanel.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Right;
            }
            else
            {
                sbEncodingPanel.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right)));
            }
        }

        /// <summary>
        /// Set the status bar text for the total count. (Thread Safe)
        /// </summary>
        /// <param name="count">Total number of hits</param>

        private void SetStatusBarTotalCount(int count)
        {
            if (stbStatus.InvokeRequired)
            {
                UpdateStatusCountCallBack _delegate = new UpdateStatusCountCallBack(SetStatusBarTotalCount);
                stbStatus.Invoke(_delegate, new object[1] { count });
                return;
            }

            sbTotalCountPanel.Text = string.Format(Language.GetGenericText("ResultsStatusTotalCount"), count);
        }

        /// <summary>
        /// Set the status bar text for the total count. (Thread Safe)
        /// </summary>
        /// <param name="count">Total number of hits</param>

        private void SetStatusBarTotalCountPick(int count)
        {
            if (stbStatus.InvokeRequired)
            {               
                UpdateStatusCountCallBack _delegate = new UpdateStatusCountCallBack(SetStatusBarTotalCountPick);
                stbStatus.Invoke(_delegate, new object[1] { count });
                return;
            }

            sbTotalCountPanelPick.Text = string.Format(Language.GetGenericText("ResultsStatusTotalPickCount"), count);
        }

        public int TotalPick()
        {
            string conString = "";
            int result = 0;
            string myQuery = "SELECT isnull(count(*),0) " +
                             " FROM u_result_file WHERE convert(DATE,ousrdata, 110) = convert(DATE,GETDATE(), 110) ";

            if (GeneralSettings.UseIntegritySecurity)
            {
                conString = "Data Source = " + GeneralSettings.Server + ";Initial Catalog=" + GeneralSettings.Database + ";Integrated Security=True";
            }
            else
            {
                conString = "Data Source=" + GeneralSettings.Server + ";Initial Catalog=" + GeneralSettings.Database +
                    ";Persist Security Info=True;User ID=" + GeneralSettings.Login + ";Password=" + GeneralSettings.Password;
            }


            using (SqlConnection con = new SqlConnection(conString))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand(myQuery, con))
                {
                    
                    //System.NullReferenceException occurs when their is no data/result
                    int getValue = (int)cmd.ExecuteScalar();
                    if (getValue > 0)
                    {
                        result = getValue;
                    }
                }
                con.Close();
            }
            return result;
        }

        /// <summary>
        /// Set the status bar text for the file count. (Thread Safe)
        /// </summary>
        /// <param name="count">Total number of files</param>

        private void SetStatusBarFileCount(int count)
        {
            if (stbStatus.InvokeRequired)
            {
                UpdateStatusCountCallBack _delegate = new UpdateStatusCountCallBack(SetStatusBarFileCount);
                stbStatus.Invoke(_delegate, new object[1] { count });
                return;
            }

            sbFileCountPanel.Text = string.Format(Language.GetGenericText("ResultsStatusFileCount"), count);
        }

        /// <summary>
        /// Set the status bar text for the filter count. (Thread Safe)
        /// </summary>
        /// <param name="count">Total number of filtered items</param>

        private void SetStatusBarFilterCount(int count)
        {
            if (stbStatus.InvokeRequired)
            {
                UpdateStatusCountCallBack _delegate = new UpdateStatusCountCallBack(SetStatusBarFilterCount);
                stbStatus.Invoke(_delegate, new object[1] { count });
                return;
            }

            sbFilterCountPanel.Text = string.Format(Language.GetGenericText("ResultsStatusFilterCount"), count);
            sbFilterCountPanel.BackColor = count > 0 ? Color.Yellow : SystemColors.Control;
        }

        /// <summary>
        /// Set the status bar text for the error count. (Thread Safe)
        /// </summary>
        /// <param name="count">Total number of errors</param>

        private void SetStatusBarErrorCount(int count)
        {
            if (stbStatus.InvokeRequired)
            {
                UpdateStatusCountCallBack _delegate = new UpdateStatusCountCallBack(SetStatusBarErrorCount);
                stbStatus.Invoke(_delegate, new object[1] { count });
                return;
            }

            sbErrorCountPanel.Text = string.Format(Language.GetGenericText("ResultsStatusErrorCount"), count);
            sbErrorCountPanel.BackColor = count > 0 ? Color.Red : SystemColors.Control;
        }

        /// <summary>
        /// Calculate and set the total number of hits for the current search. (Thread Safe)
        /// </summary>

        private void CalculateTotalCount()
        {
            if (lstFileNames.InvokeRequired)
            {
                CalculateTotalCountCallBack _delegate = new CalculateTotalCountCallBack(CalculateTotalCount);
                lstFileNames.Invoke(_delegate);
                return;
            }

            //update total hit count
            int total = 0;
            int single = 0;

            foreach (ListViewItem item in lstFileNames.Items)
            {
                single = int.Parse(item.SubItems[Constants.COLUMN_INDEX_COUNT].Text);
                total += single;
            }

            SetStatusBarTotalCount(total);
            SetStatusBarFileCount(lstFileNames.Items.Count);
            SetStatusBarFilterCount(GetLogItemsCountByType(LogItem.LogItemTypes.Exclusion));
            SetStatusBarErrorCount(GetLogItemsCountByType(LogItem.LogItemTypes.Error));

            if (GeneralSettings.UseDataBase) {

                SetStatusBarTotalCountPick(TotalPick());
            }
        }


        /// <summary>
        /// Start the searching
        /// </summary>
        /// <param name="searchWithInResults">true for searching within current results, false starts a new search</param>

        private void StartSearch(bool searchWithInResults)
        {
            try
            {
                // generate search specification from user interface
                var searchSpec = GetSearchSpecFromUI(searchWithInResults, cboSearchForText.Text);

                // update combo selections
                AddComboSelection(cboSearchForText, cboSearchForText.Text);
                AddComboSelection(cboFileName, cboFileName.Text);
                AddComboSelection(cboFilePath, cboFilePath.Text.Trim());

                SetWindowText();

                // disable gui
                SetSearchState(false);

                // reset display
                LogItems.Clear();
                SetStatusBarMessage(string.Empty);
                SetStatusBarEncoding(string.Empty);
                SetStatusBarTotalCount(0);
                SetStatusBarFileCount(0);
                SetStatusBarFilterCount(0);
                SetStatusBarErrorCount(0);

                ClearItems();
                txtHits.LineNumbers = null;
                txtHits.Clear();


                // create new grep instance
                __Grep = new Grep(searchSpec);

                // add plugins
                __Grep.Plugins = Core.PluginManager.Items;

                // attach events
                __Grep.FileHit += ReceiveFileHit;
                __Grep.LineHit += ReceiveLineHit;
                __Grep.SearchCancel += ReceiveSearchCancel;
                __Grep.SearchComplete += ReceiveSearchComplete;
                __Grep.SearchError += ReceiveSearchError;
                __Grep.SearchingFile += ReceiveSearchingFile;
                __Grep.FileFiltered += ReceiveFileFiltered;
                __Grep.DirectoryFiltered += ReceiveDirectoryFiltered;
                __Grep.SearchingFileByPlugin += ReceiveSearchingFileByPlugin;
                __Grep.FileEncodingDetected += ReceiveFileEncodingDetected;

                API.TaskbarProgress.SetState(this.Handle, API.TaskbarProgress.TaskbarStates.Indeterminate);
                LogItems.Add(new LogItem(LogItem.LogItemTypes.Status, "SearchStarted"));
                LogStartSearchMessage(searchSpec);
                StartingTime = Stopwatch.GetTimestamp();

                __Grep.BeginExecute();
            }
            catch (Exception ex)
            {
                LogStopSearchMessage("error");

                RestoreTaskBarProgress();
                LogItems.Add(new LogItem(LogItem.LogItemTypes.Error, "SearchGenericError", string.Format("{0}||{1}", string.Empty, ex.Message)));
                LogClient.Instance.Logger.Error("Unhandled search error: {0}", LogClient.GetAllExceptions(ex));

                string message = string.Format(Language.GetGenericText("SearchGenericError"), ex.Message);
                MessageBox.Show(this, message, ProductInformation.ApplicationName, MessageBoxButtons.OK, MessageBoxIcon.Error);

                SetStatusBarMessage(message);
                SetSearchState(true);
                CalculateTotalCount();
            }
        }

        /// <summary>
        /// Start the searching
        /// </summary>
        /// <param name="searchWithInResults">searching within current results</param>

        private void StartSearchCurrentResults()
        {
            try
            {
                // generate search specification from user interface
                var searchSpec = GetSearchSpecFromUI(true, cboSearchForText2.Text);

                // update combo selections
                AddComboSelection(cboSearchForText2, cboSearchForText2.Text);
                AddComboSelection(cboFileName, cboFileName.Text);
                AddComboSelection(cboFilePath, cboFilePath.Text.Trim());

                SetWindowText();

                // disable gui
                SetSearchState(false);

                // reset display
                LogItems.Clear();
                SetStatusBarMessage(string.Empty);
                SetStatusBarEncoding(string.Empty);
                SetStatusBarTotalCount(0);
                SetStatusBarFileCount(0);
                SetStatusBarFilterCount(0);
                SetStatusBarErrorCount(0);

                ClearItems();
                txtHits.LineNumbers = null;
                txtHits.Clear();

                // create new grep instance
                __Grep = new Grep(searchSpec);

                // add plugins
                __Grep.Plugins = Core.PluginManager.Items;

                // attach events
                __Grep.FileHit += ReceiveFileHit;
                __Grep.LineHit += ReceiveLineHit;
                __Grep.SearchCancel += ReceiveSearchCancel;
                __Grep.SearchComplete += ReceiveSearchComplete;
                __Grep.SearchError += ReceiveSearchError;
                __Grep.SearchingFile += ReceiveSearchingFile;
                __Grep.FileFiltered += ReceiveFileFiltered;
                __Grep.DirectoryFiltered += ReceiveDirectoryFiltered;
                __Grep.SearchingFileByPlugin += ReceiveSearchingFileByPlugin;
                __Grep.FileEncodingDetected += ReceiveFileEncodingDetected;

                API.TaskbarProgress.SetState(this.Handle, API.TaskbarProgress.TaskbarStates.Indeterminate);
                LogItems.Add(new LogItem(LogItem.LogItemTypes.Status, "SearchStarted"));
                LogStartSearchMessage(searchSpec);
                StartingTime = Stopwatch.GetTimestamp();

                __Grep.BeginExecute();
            }
            catch (Exception ex)
            {
                LogStopSearchMessage("error");

                RestoreTaskBarProgress();
                LogItems.Add(new LogItem(LogItem.LogItemTypes.Error, "SearchGenericError", string.Format("{0}||{1}", string.Empty, ex.Message)));
                LogClient.Instance.Logger.Error("Unhandled search error: {0}", LogClient.GetAllExceptions(ex));

                string message = string.Format(Language.GetGenericText("SearchGenericError"), ex.Message);
                MessageBox.Show(this, message, ProductInformation.ApplicationName, MessageBoxButtons.OK, MessageBoxIcon.Error);

                SetStatusBarMessage(message);
                SetSearchState(true);
                CalculateTotalCount();
            }
        }

        /// <summary>
        /// Logs a start search message to log file.
        /// </summary>
        /// <param name="searchSpec">Current search specification</param>

        private void LogStartSearchMessage(ISearchSpec searchSpec)
        {
            StringBuilder searchTextOptions = new StringBuilder();
            LogSearchOptionHelper(searchTextOptions, searchSpec.UseRegularExpressions, "regex");
            LogSearchOptionHelper(searchTextOptions, searchSpec.UseCaseSensitivity, "case sensitive");
            LogSearchOptionHelper(searchTextOptions, searchSpec.UseWholeWordMatching, "whole word");
            LogSearchOptionHelper(searchTextOptions, searchSpec.UseNegation, "negation");
            LogSearchOptionHelper(searchTextOptions, searchSpec.ReturnOnlyFileNames, "only file names");
            LogSearchOptionHelper(searchTextOptions, searchSpec.ContextLines > 0, string.Format("{0} context lines", searchSpec.ContextLines));

            if (searchTextOptions.Length > 0)
            {
                searchTextOptions.Insert(0, "[");
                searchTextOptions.Append("]");
            }

            StringBuilder fileEncoding = new StringBuilder();
            if (searchSpec.EncodingDetectionOptions.DetectFileEncoding)
            {
                fileEncoding.Append("[");
                fileEncoding.Append("detect encoding");
                fileEncoding.AppendFormat(", performance set at {0}", Enum.GetName(typeof(EncodingOptions.Performance), GeneralSettings.EncodingPerformance).ToLower());

                if (searchSpec.EncodingDetectionOptions.UseEncodingCache)
                {
                    fileEncoding.Append(", cache enabled");
                }

                fileEncoding.Append("]");
            }

            LogClient.Instance.Logger.Info("Search started in '{0}'{1} against {2}{3} for {4}{5}",
               searchSpec.StartFilePaths != null && searchSpec.StartFilePaths.Length > 0 ? string.Join(", ", searchSpec.StartFilePaths) : string.Join(", ", searchSpec.StartDirectories),
               searchSpec.SearchInSubfolders ? "[include sub folders]" : "",
               searchSpec.FileFilter,
               fileEncoding.ToString(),
               searchSpec.SearchText,
               searchTextOptions.ToString());
        }

        /// <summary>
        /// Handles formatting of log search option.
        /// </summary>
        /// <param name="optionBuilder">Current StringBuilder</param>
        /// <param name="option">if option is enabled</param>
        /// <param name="displayText">text to display for option</param>

        private void LogSearchOptionHelper(StringBuilder optionBuilder, bool option, string displayText)
        {
            if (option)
            {
                if (optionBuilder.Length > 0)
                    optionBuilder.Append(", ");

                optionBuilder.Append(displayText);
            }
        }

        /// <summary>
        /// Logs a stop search message to log file.
        /// </summary>
        /// <param name="stopType">Stopping type for current log entry</param>

        private void LogStopSearchMessage(string stopType)
        {
            long endingTime = Stopwatch.GetTimestamp();
            long elapsedTime = endingTime - StartingTime;
            double elapsedSeconds = elapsedTime * (1.0 / Stopwatch.Frequency);
            string stopTypeMessage = !string.IsNullOrEmpty(stopType) ? string.Format(" ({0})", stopType) : string.Empty;

            LogClient.Instance.Logger.Info("Search stopped{0}, taking {1} seconds.", stopTypeMessage, elapsedSeconds);
        }

        /// <summary>
        /// Clears the file list (Thread safe).
        /// </summary>

        private void ClearItems()
        {
            if (lstFileNames.InvokeRequired)
            {
                ClearItemsCallBack _delegate = ClearItems;
                lstFileNames.Invoke(_delegate);
                return;
            }

            lstFileNames.Items.Clear();
        }

        /// <summary>
        /// Sets the grep options
        /// </summary>
        /// <param name="searchWithInResults">is true when desired to search within current results list</param>

        private ISearchSpec GetSearchSpecFromUI(bool searchWithInResults, string search)
        {
            string path = cboFilePath.Text.Trim();

            List<string> filePaths = new List<string>();
            if (searchWithInResults)
            {
                // get currently listed file paths from ListView either by selection or all
                if (lstFileNames.SelectedItems != null && lstFileNames.SelectedItems.Count > 0)
                {
                    for (int i = 0; i < lstFileNames.SelectedItems.Count; i++)
                    {
                        filePaths.Add(Path.Combine(lstFileNames.SelectedItems[i].SubItems[Constants.COLUMN_INDEX_DIRECTORY].Text, lstFileNames.SelectedItems[i].SubItems[Constants.COLUMN_INDEX_FILE].Text));
                    }
                }
                else
                {
                    for (int i = 0; i < lstFileNames.Items.Count; i++)
                    {
                        filePaths.Add(Path.Combine(lstFileNames.Items[i].SubItems[Constants.COLUMN_INDEX_DIRECTORY].Text, lstFileNames.Items[i].SubItems[Constants.COLUMN_INDEX_FILE].Text));
                    }
                }
            }

            var spec = new SearchInterfaces.SearchSpec
            {
                UseCaseSensitivity = chkCaseSensitive.Checked,
                ContextLines = Convert.ToInt32(txtContextLines.Value),
                UseNegation = chkNegation.Checked,
                ReturnOnlyFileNames = chkFileNamesOnly.Checked,
                SearchInSubfolders = chkRecurse.Checked,
                UseRegularExpressions = chkRegularExpressions.Checked,
                UseWholeWordMatching = chkWholeWordOnly.Checked,
                SearchText = search, //cboSearchForText.Text,
                FileEncodings = FileEncoding.ConvertStringToFileEncodings(GeneralSettings.FileEncodings),
                EncodingDetectionOptions = new EncodingOptions()
                {
                    DetectFileEncoding = GeneralSettings.DetectFileEncoding,
                    PerformanceSetting = (EncodingOptions.Performance)GeneralSettings.EncodingPerformance,
                    UseEncodingCache = GeneralSettings.UseEncodingCache
                },
                FilterItems = __FilterItems.FindAll(delegate(FilterItem i) { return i.Enabled; })
            };


            // handle paths used in file filter entry, update path and fileName if fileName has a path in it
            string fileName = cboFileName.Text;
            int slashPos = fileName.LastIndexOf(Path.DirectorySeparatorChar.ToString());
            if (slashPos > -1)
            {
                fileName = fileName.Substring(slashPos + 1);
            }
            spec.FileFilter = fileName;

            // setup file paths if defined, otherwise setup directories
            if (filePaths != null && filePaths.Count > 0)
            {
                spec.StartFilePaths = filePaths.ToArray();
                spec.StartDirectories = null;
            }
            else
            {
                string[] paths = path.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);

                // fileName has a slash, so append the directory and get the file filter
                slashPos = spec.FileFilter.LastIndexOf(Path.DirectorySeparatorChar.ToString());
                if (slashPos > -1)
                {
                    // append to each starting directory
                    for (int i = 0; i < paths.Length; i++)
                    {
                        paths[i] += spec.FileFilter.Substring(0, slashPos);
                    }
                }
                spec.StartDirectories = paths;
                spec.StartFilePaths = null;
            }

            return spec;
        }

        /// <summary>
        /// Add a file hit to the listview (Thread safe).
        /// </summary>
        /// <param name="file">File to add</param>
        /// <param name="index">Position in GrepCollection</param>

        private void AddHitToList(FileInfo file, int index)
        {
            if (lstFileNames.InvokeRequired)
            {
                AddToListCallBack _delegate = AddHitToList;
                lstFileNames.Invoke(_delegate, new object[2] { file, index });
                return;
            }

            // don't add if it already exists
            foreach (ListViewItem item in lstFileNames.Items)
            {
                MatchResult hit = __Grep.RetrieveMatchResult(int.Parse(item.SubItems[Constants.COLUMN_INDEX_GREP_INDEX].Text));
                if (hit.File.FullName.Equals(file.FullName, StringComparison.InvariantCultureIgnoreCase))
                {
                    return;
                }
            }

            // Create the list item
            var listItem = new ListViewItem(file.Name);
            listItem.Name = index.ToString();
            listItem.ImageIndex = ListViewImageManager.GetImageIndex(file, ListViewImageList);
            listItem.SubItems.Add(file.DirectoryName);
            listItem.SubItems.Add(file.Extension);
            listItem.SubItems.Add(file.LastWriteTime.ToString());

            // add explorer style of file size for display but store file size in bytes for comparision
            ListViewItem.ListViewSubItem subItem = new ListViewItem.ListViewSubItem(listItem, API.StrFormatByteSize(file.Length));
            subItem.Tag = file.Length;
            listItem.SubItems.Add(subItem);

            listItem.SubItems.Add("0");

            // must be last
            listItem.SubItems.Add(index.ToString());

            // Add list item to listview
            lstFileNames.Items.Add(listItem);

            // clear it out
            listItem = null;
        }

        /// <summary>
        /// Select all the list items.
        /// </summary>

        private void SelectAllListItems()
        {
            foreach (ListViewItem lvi in lstFileNames.Items)
            {
                lvi.Selected = true;
            }
        }

        /// <summary>
        /// Show a message box to the user if they have exclusions or errors and how to view them.  Can be disabled
        /// via the setting in the Options dialog.
        /// </summary>

        private void ShowExclusionsErrorMessageBox()
        {
            if (this.InvokeRequired)
            {
                DisplayExclusionErrorMessagesCallBack _delegate = ShowExclusionsErrorMessageBox;
                this.Invoke(_delegate);
                return;
            }

            // only show if not disabled by user and either filter count or error count is greater than 0
            if (GeneralSettings.ShowExclusionErrorMessage &&
               (GetLogItemsCountByType(LogItem.LogItemTypes.Exclusion) > 0 ||
               GetLogItemsCountByType(LogItem.LogItemTypes.Error) > 0))
            {
                MessageBox.Show(this, Language.GetGenericText("ExclusionErrorMessageText"),
                   ProductInformation.ApplicationName, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        /// <summary>
        /// Setup the context menu for the results area.
        /// </summary>

        private void AddContextMenuForResults()
        {
            var menu = new System.Windows.Controls.ContextMenu();
            var item = new System.Windows.Controls.MenuItem();

            item.Click += openFile_Click;
            item.Header = Language.GetGenericText("ResultsContextMenu.OpenFileCurrentLine");
            item.IsEnabled = false;
            menu.Items.Add(item);

            menu.Items.Add(new System.Windows.Controls.Separator());

            item = new System.Windows.Controls.MenuItem();
            item.Click += copyItem_Click;
            item.Header = Language.GetGenericText("ResultsContextMenu.Copy");
            menu.Items.Add(item);

            item = new System.Windows.Controls.MenuItem();
            item.Header = Language.GetGenericText("ResultsContextMenu.SelectAll");
            item.Click += selectAllItem_Click;
            menu.Items.Add(item);

            txtHits.ContextMenu = menu;
            txtHits.ContextMenuOpening += menu_ContextMenuOpening;
        }

        /// <summary>
        /// Get TextEditorOpener for give position.
        /// </summary>
        /// <param name="position">Current TextViewPosition from results preview area</param>
        /// <returns>TextEditorOpener for current position</returns>

        private TextEditorOpener GetEditorAtLocation(ICSharpCode.AvalonEdit.TextViewPosition? position)
        {
            var opener = new TextEditorOpener();

            if (position.HasValue)
            {
                try
                {
                    string path = string.Empty;
                    int lineNumber = position.Value.Line;
                    int columnNumber = 1;

                    if (txtHits.LineNumbers != null && (lstFileNames.SelectedItems.Count == 0 || !EntireFileMenuItem.Checked))
                    {
                        // either all results or file's matches
                        var lineNumberHolder = txtHits.LineNumbers[position.Value.Line - 1];

                        path = lineNumberHolder.FileFullName;

                        if (!string.IsNullOrEmpty(path))
                        {
                            string line = string.Empty;
                            lineNumber = lineNumberHolder.Number > -1 ? lineNumberHolder.Number : 1;
                            columnNumber = lineNumberHolder.ColumnNumber;

                            var fileMatch = (from m in __Grep.MatchResults where m.File.FullName.Equals(path) select m).FirstOrDefault();
                            foreach (var matchLine in fileMatch.Matches)
                            {
                                if (matchLine.LineNumber == lineNumber)
                                {
                                    line = matchLine.Line;
                                    break;
                                }
                            }

                            opener = new TextEditorOpener(path, lineNumber, columnNumber, line, __Grep.SearchSpec.SearchText);
                        }
                    }
                    else if (lstFileNames.SelectedItems.Count > 0 && __Grep != null)
                    {
                        // full file
                        MatchResult result = __Grep.RetrieveMatchResult(Convert.ToInt32(lstFileNames.SelectedItems[0].SubItems[Constants.COLUMN_INDEX_GREP_INDEX].Text));
                        string line = string.Empty;
                        path = result.File.FullName;
                        MatchResultLine matchLine = (from m in result.Matches where m.LineNumber == position.Value.Line select m).FirstOrDefault();
                        if (matchLine != null && matchLine.LineNumber > -1)
                        {
                            line = matchLine.Line;
                            lineNumber = matchLine.LineNumber;
                            columnNumber = matchLine.ColumnNumber;
                        }

                        opener = new TextEditorOpener(path, lineNumber, columnNumber, line, __Grep.SearchSpec.SearchText);
                    }
                }
                catch { }
            }

            return opener;
        }

        /// <summary>
        /// Handles finding the current line under the mouse pointer during a double click to open the texteditor at that location.
        /// </summary>
        /// <param name="sender">system parameter</param>
        /// <param name="e">system parameter</param>

        private void txtHits_PreviewMouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.ChangedButton == System.Windows.Input.MouseButton.Left)
            {
                ICSharpCode.AvalonEdit.TextViewPosition? position = null;
                var mousePos = e.GetPosition(txtHits);
                if (mousePos != null)
                {
                    position = txtHits.GetPositionFromPoint(mousePos);
                }

                var opener = GetEditorAtLocation(position);
                if (opener.HasValue())
                {
                    e.Handled = true;
                    TextEditors.Open(opener);
                }
            }
        }

        /// <summary>
        /// Handles determining enabling the open file menuitem.
        /// </summary>
        /// <param name="sender">CustomTextEditor</param>
        /// <param name="e">system parameter</param>

        private void menu_ContextMenuOpening(object sender, System.Windows.Controls.ContextMenuEventArgs e)
        {
            // enable open at menu item
            var opener = GetEditorAtLocation(txtHits.GetPositionFromRightClickPoint());
            ((sender as TextEditorEx).ContextMenu.Items[0] as System.Windows.Controls.MenuItem).IsEnabled = opener.HasValue();
        }

        /// <summary>
        /// Handles finding the current line under the mouse pointer to open the texteditor at that location.
        /// </summary>
        /// <param name="sender">system parameter</param>
        /// <param name="e">system parameter</param>

        private void openFile_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var opener = GetEditorAtLocation(txtHits.GetPositionFromRightClickPoint());
            if (opener.HasValue())
            {
                TextEditors.Open(opener);
            }
        }

        /// <summary>
        /// Handles selecting all text in results preview area.
        /// </summary>
        /// <param name="sender">system parameter</param>
        /// <param name="e">system parameter</param>

        private void selectAllItem_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            txtHits.Focus();
            txtHits.SelectAll();
        }

        /// <summary>
        /// Handles setting clipboard data with currently selected text from results preview area.
        /// </summary>
        /// <param name="sender">system parameter</param>
        /// <param name="e">system parameter</param>

        private void copyItem_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            txtHits.Copy();
        }

        /// <summary>
        /// Gets the number of items in the LogItems list for a given type.
        /// </summary>
        /// <param name="type">LogItemType to determine count</param>
        /// <returns>0 if LogItems is null, count for type otherwise</returns>

        private int GetLogItemsCountByType(LogItem.LogItemTypes type)
        {
            if (LogItems == null || LogItems.Count == 0)
                return 0;

            return LogItems.FindAll(
                delegate(LogItem item)
                {
                    return item.ItemType == type;
                }
                ).Count;
        }

        /// <summary>
        /// Determines if any log items are present.
        /// </summary>
        /// <returns>true if log items are present, false otherwise</returns>

        private bool AnyLogItems()
        {
            return (LogItems != null ? LogItems.Count > 0 : false);
        }

        /// <summary>
        /// Sets the form's text to include the first entry of the search path's
        /// </summary>

        private void SetWindowText()
        {
            if (cboFilePath.Items.Count > 0)
            {
                this.Text = string.Format("{0} - {1}", cboFilePath.Items[0].ToString(), ProductInformation.ApplicationName);
            }
        }

        /// <summary>
        /// Restore the Win7+ TaskBar progress state to normal.
        /// </summary>

        private void RestoreTaskBarProgress()
        {
            if (this.InvokeRequired)
            {
                RestoreTaskBarProgressCallBack _delegate = RestoreTaskBarProgress;
                this.Invoke(_delegate);
                return;
            }

            try
            {
                API.TaskbarProgress.SetState(this.Handle, API.TaskbarProgress.TaskbarStates.NoProgress);
            }
            catch { }
        }
        #endregion

        /// <summary>
        /// Insert information about xml file into data base.
        /// </summary>
        private void InsertDataBase(string path)
        {

            FileInfo file = new FileInfo(path);
            string destDir = cboFilePathDest.Text;
            FileInfo destFile = new FileInfo(Path.Combine(destDir, file.Name));

            if (destFile.Exists)
            {
                if (file.LastWriteTime > destFile.LastWriteTime)
                {
                    // now you can safely overwrite it
                    file.CopyTo(destFile.FullName, true);
                }
            }
            else 
            {
                file.CopyTo(destFile.FullName, true);
            }

            string conString = string.Empty;
            string xmlFile = path;

            if (GeneralSettings.UseIntegritySecurity)
            {
                conString = "Data Source = " + GeneralSettings.Server + ";Initial Catalog=" + GeneralSettings.Database + ";Integrated Security=True";
            }
            else
            {
                conString = "Data Source=" + GeneralSettings.Server + ";Initial Catalog=" + GeneralSettings.Database +
                    ";Persist Security Info=True;User ID=" + GeneralSettings.Login + ";Password=" + GeneralSettings.Password;
            }

            // Open connection to the database
            using (SqlConnection con = new SqlConnection(conString))
            {
                // read xml and put into dataset
                DataSet ds = new DataSet();
                ds.ReadXml(xmlFile, XmlReadMode.InferSchema);


                //File.Delete(newFileName); // Delete the existing file if exists
                //File.Move(oldFileName, newFileName); // Rename the oldFileName into newFileName

                //rename the file
                File.Move(xmlFile, xmlFile + ".lido");
                

                DataTable dtResult = ds.Tables["result_file"];
                DataTable dtErrors = ds.Tables["error_no"];

                string wordChk = cboSearchForText2.Text.ToUpper();
                string find = "status = '" + wordChk + "'";
                DataRow[] foundRows = dtResult.Select(find);

                try
                {

                    if (foundRows[0].ItemArray[9].Equals(wordChk))
                    {
                        //MessageBox.Show("You have been a problem with with\n serial=>> " + foundRows[0].ItemArray[8] + "\n is =>> " + foundRows[0].ItemArray[9], "A T T E N T I O N",
                        //    MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);

                        MessageBox.Show("PROBLEM WITH SERIAL=>> " + foundRows[0].ItemArray[8] + " [ " + foundRows[0].ItemArray[9] + " ]",
   ProductInformation.ApplicationName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

                    }
                }
                catch (Exception)
                {
                    //MessageBox.Show("The word " + wordChk + " not found!!!");
                }

                con.Open();

                if (con.State == ConnectionState.Open) // if connection.Open was successful
                {
                    using (SqlBulkCopy bc = new SqlBulkCopy(conString))
                    {
                        bc.DestinationTableName = "u_result_file";
                        bc.ColumnMappings.Add("filename", "filename");
                        bc.ColumnMappings.Add("supplier", "supplier");
                        bc.ColumnMappings.Add("date", "date");
                        bc.ColumnMappings.Add("station_ID", "station_ID");
                        bc.ColumnMappings.Add("operator_ID", "operator_ID");
                        bc.ColumnMappings.Add("program", "program");
                        bc.ColumnMappings.Add("side_variant", "side_variant");
                        bc.ColumnMappings.Add("serial", "serial");
                        bc.ColumnMappings.Add("status", "status");
                        bc.ColumnMappings.Add("starttime", "starttime");
                        bc.WriteToServer(dtResult);
                    }

                }

                GetData();
                SetStatusBarTotalCountPick(TotalPick());
            }
        }


        private void GetData()
        {
            string conString = "";
            string selectCommand = "SELECT serial Serial, [Status] , operator_ID Operator, ousrdata AS Picking " +
                                   " FROM u_result_file WHERE convert(DATE,ousrdata, 110) = convert(DATE,GETDATE(), 110) ORDER BY Picking DESC";

            if (GeneralSettings.UseIntegritySecurity)
            {
                conString = "Data Source = " + GeneralSettings.Server + ";Initial Catalog=" + GeneralSettings.Database + ";Integrated Security=True";
            }
            else
            {
                conString = "Data Source=" + GeneralSettings.Server + ";Initial Catalog=" + GeneralSettings.Database +
                    ";Persist Security Info=True;User ID=" + GeneralSettings.Login + ";Password=" + GeneralSettings.Password;
            }


            using (SqlConnection con = new SqlConnection(conString))
            {

                con.Open();

                if (con.State == ConnectionState.Open) // if connection.Open was successful
                {
                    // Create a new data adapter based on the specified query.
                    SqlDataAdapter sda = new SqlDataAdapter(selectCommand, conString);


                    // Create a command builder to generate SQL update, insert, and
                    // delete commands based on selectCommand. These are used to
                    // update the database.
                    SqlCommandBuilder commandBuilder = new SqlCommandBuilder(sda);

                    // Populate a new data table and bind it to the BindingSource.
                    DataTable table = new DataTable();
                    table.Locale = System.Globalization.CultureInfo.InvariantCulture;
                    sda.Fill(table);

                    dataGridView1.DataSource = table;
                    InitializeDataGridView();

                }

            }

        }

        private void InitializeDataGridView()
        {
            // Initialize basic DataGridView properties.
            //dataGridView1.Dock = DockStyle.Fill;
            dataGridView1.BackgroundColor = Color.LightGray;
            dataGridView1.BorderStyle = BorderStyle.Fixed3D;

            // Set property values appropriate for read-only display and 
            // limited interactivity. 
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AllowUserToDeleteRows = false;
            dataGridView1.AllowUserToOrderColumns = true;
            dataGridView1.ReadOnly = true;
            //dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.MultiSelect = false;
            dataGridView1.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
            dataGridView1.AutoResizeColumns();
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;

            //dataGridView1.AllowUserToResizeColumns = false;
            //dataGridView1.ColumnHeadersHeightSizeMode =
            //    DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            //dataGridView1.AllowUserToResizeRows = false;
            //dataGridView1.RowHeadersWidthSizeMode =
            //    DataGridViewRowHeadersWidthSizeMode.DisableResizing;

            // Set the selection background color for all the cells.
            dataGridView1.DefaultCellStyle.SelectionBackColor = Color.White;
            dataGridView1.DefaultCellStyle.SelectionForeColor = Color.Black;

            // Set RowHeadersDefaultCellStyle.SelectionBackColor so that its default
            // value won't override DataGridView.DefaultCellStyle.SelectionBackColor.
            dataGridView1.RowHeadersDefaultCellStyle.SelectionBackColor = Color.Empty;

            // Set the background color for all rows and for alternating rows. 
            // The value for alternating rows overrides the value for all rows. 
            dataGridView1.RowsDefaultCellStyle.BackColor = Color.Bisque;
            dataGridView1.AlternatingRowsDefaultCellStyle.BackColor = Color.Beige;

            // Set the row and column header styles.
            dataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dataGridView1.ColumnHeadersDefaultCellStyle.BackColor = Color.Black;
            dataGridView1.RowHeadersDefaultCellStyle.BackColor = Color.Black;

            // Specify a larger font for the "Ratings" column. 
            using (Font font = new Font(
                dataGridView1.DefaultCellStyle.Font.FontFamily, 12, FontStyle.Bold))
            {
                dataGridView1.Columns[1].DefaultCellStyle.Font = font;
            }
            //dataGridView1.Columns[1].DefaultCellStyle.Font = new System.Drawing.Font("Tahoma", 12F, FontStyle.Bold);

            //dataGridView1.Columns[0].Width = 250;
            //dataGridView1.Columns[1].Width = 100;
            //dataGridView1.Columns[2].Width = 450;
            //dataGridView1.Columns[3].Width = 450;


        }


        private void cboFilePathDest_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            if (files != null && files.Length > 0)
            {
                string path1 = files[0];
                if (Directory.Exists(path1))
                {
                    AddComboSelection(cboFilePathDest, path1);
                }
                else if (File.Exists(path1))
                {
                    FileInfo file = new FileInfo(path1);
                    AddComboSelection(cboFilePathDest, file.DirectoryName);
                }
            }
        }

        private void cboFilePathDest_DragEnter(object sender, DragEventArgs e)
        {
            if ((e.AllowedEffect & DragDropEffects.Copy) == DragDropEffects.Copy)
            {
                if (e.Data.GetDataPresent(DataFormats.FileDrop))
                {
                    e.Effect = DragDropEffects.Copy;
                }
            }
        }

        private void cboFilePathDest_DropDown(object sender, EventArgs e)
        {
            cboFilePathDest.DropDownWidth = CalculateDropDownWidth(cboFilePathDest);
        }

    }
}
