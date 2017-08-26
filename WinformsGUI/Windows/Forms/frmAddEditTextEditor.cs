using System;
using System.Collections.Generic;
using System.Windows.Forms;

using bSearch.Common;

namespace bSearch.Windows.Forms
{
    /// <summary>
    /// Used to Add/Edit a Text Editor for a specified file type.
    /// </summary>
    /// <remarks>
    ///   bSearch File Searching Utility.
    ///   Copyright (C) 2006 BigLevel Lda.
    ///   
    ///   The author may be contacted at:
    ///   suporte@bigLevel.pt or comercial@biglevel.pt
    /// </remarks>
    /// <history>

    public partial class frmAddEditTextEditor : BaseForm
    {
        #region Declarations
        private bool __Add = true;
        private bool __AllTypesDefined = false;
        private string __OriginalFileType = string.Empty;
        private TextEditor __Editor = null;
        private List<string> __ExistingFileTypes = null;
        #endregion

        /// <summary>
        /// Creates an instance of the frmAddEditTextEditor class.
        /// </summary>

        public frmAddEditTextEditor()
        {
            InitializeComponent();
        }

        #region Properties

        /// <summary>
        /// Gets/Sets the current TextEditor.
        /// </summary>

        public TextEditor Editor
        {
            get { return __Editor; }
            set
            {
                __Editor = value;
                __OriginalFileType = value.FileType;
            }
        }

        /// <summary>
        /// Determines whether the control is in Addition mode.
        /// </summary>

        public bool IsAdd
        {
            set { __Add = value; }
        }

        /// <summary>
        /// Determines whether the All File Types has already been used.
        /// </summary>

        public bool IsAllTypesDefined
        {
            set
            {
                __AllTypesDefined = value;
                if (__AllTypesDefined)
                    // one is defined so don't display
                    lblAllTypesMessage.Visible = false;
                else
                    // not defined so display message
                    lblAllTypesMessage.Visible = true;
            }
        }

        /// <summary>
        /// Contains the current file types defined.
        /// </summary>

        public List<string> ExistingFileTypes
        {
            get { return __ExistingFileTypes; }
            set { __ExistingFileTypes = value; }
        }
        #endregion

        #region Events
        /// <summary>
        /// Setup the form for display.
        /// </summary>
        /// <param name="sender">system parameter</param>
        /// <param name="e">system parameter</param>

        private void frmAddEditTextEditor_Load(object sender, System.EventArgs e)
        {
            if (!__Add)
            {
                // load values into text boxes
                txtFileType.Text = Editor.FileType;
                txtTextEditorLocation.Text = Editor.Editor;
                txtCmdLineArgs.Text = Editor.Arguments;
                chkUseQuotesAroundFileName.Checked = Editor.UseQuotesAroundFileName;
                updwnTabSize.Value = Editor.TabSize;
            }

            lblCmdOptions.Text = "Command Line Optons:\r\n" +
                                 "  %1 - File\r\n" +
                                 "  %2 - Line Number\r\n" +
                                 "  %3 - Column\r\n" +
                                 "  %4 - Searched Text";

            //Language.GenerateXml(this, Application.StartupPath + "\\" + this.Name + ".xml");
            Language.ProcessForm(this, HoverTips);

            UpdateCmdLinePreview();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="findFileType"></param>
        /// <param name="searchFileType"></param>
        /// <returns></returns>
        private bool FindType(string findFileType, string searchFileType)
        {
            bool foundType = false;

            if (searchFileType.Contains(Constants.TEXT_EDITOR_TYPE_SEPARATOR))
            {
                foreach (string val in searchFileType.Split(Constants.TEXT_EDITOR_TYPE_SEPARATOR.ToCharArray(), StringSplitOptions.RemoveEmptyEntries))
                {
                    if (IsFileTypeEqual(findFileType, val))
                    {
                        foundType = true;
                        break;
                    }
                }
            }
            else if (IsFileTypeEqual(findFileType, searchFileType))
            {
                foundType = true;
            }

            return foundType;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type1"></param>
        /// <param name="type2"></param>
        /// <returns></returns>
        private bool IsFileTypeEqual(string type1, string type2)
        {
            if (type1.StartsWith(".") && !type2.StartsWith("."))
            {
                if (type1.ToLower().Equals("." + type2.ToLower()))
                {
                    return true;
                }
            }
            else if (!type1.StartsWith(".") && type2.StartsWith("."))
            {
                if (("." + type1.ToLower()).Equals(type2.ToLower()))
                {
                    return true;
                }
            }
            else if (type1.ToLower().Equals(type2.ToLower()))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Save the user selected items and close the form.
        /// </summary>
        /// <param name="sender">system parameter</param>
        /// <param name="e">system parameter</param>

        private void btnOK_Click(object sender, System.EventArgs e)
        {
            // validate not an existing file type
            bool exists = false;
            if (ExistingFileTypes != null)
            {
                if (__Add || !__OriginalFileType.Equals(txtFileType.Text))
                {
                    foreach (string fileType in ExistingFileTypes)
                    {
                        if (fileType.Contains(Constants.TEXT_EDITOR_TYPE_SEPARATOR))
                        {
                            foreach (string val in fileType.Split(Constants.TEXT_EDITOR_TYPE_SEPARATOR.ToCharArray(), StringSplitOptions.RemoveEmptyEntries))
                            {
                                if (FindType(val, txtFileType.Text))
                                {
                                    exists = true;
                                    break;
                                }
                            }
                        }
                        else if (FindType(fileType, txtFileType.Text))
                        {
                            exists = true;
                            break;
                        }
                    }

                    if (exists)
                    {
                        this.DialogResult = DialogResult.None;
                        MessageBox.Show(this, Language.GetGenericText("TextEditorsErrorFileTypeExists"),
                           ProductInformation.ApplicationName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                }
            }

            // validate cmdline has at least %1
            if (txtCmdLineArgs.Text.IndexOf("%1") == -1)
            {
                txtCmdLineArgs.Text = "%1";
            }

            // load values from text boxes
            __Editor = new TextEditor(txtFileType.Text, txtTextEditorLocation.Text, txtCmdLineArgs.Text, Convert.ToInt32(updwnTabSize.Value), chkUseQuotesAroundFileName.Checked);

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        /// <summary>
        /// Close the form.
        /// </summary>
        /// <param name="sender">system parameter</param>
        /// <param name="e">system parameter</param>

        private void btnCancel_Click(object sender, System.EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Allow selection of an executable file.
        /// </summary>
        /// <param name="sender">system parameter</param>
        /// <param name="e">system parameter</param>

        private void btnBrowse_Click(object sender, System.EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "Executables (*.exe)|*.exe|All Files (*.*)|*.*";
            dlg.Title = Language.GetGenericText("TextEditorsBrowseTitle");
            dlg.Multiselect = false;

            if (dlg.ShowDialog(this) == DialogResult.OK)
            {
                txtTextEditorLocation.Text = dlg.FileName;
                UpdateCmdLinePreview();
            }

            dlg.Dispose();
        }

        /// <summary>
        /// Update the preview display.
        /// </summary>
        /// <param name="sender">system parameter</param>
        /// <param name="e">system parameter</param>

        private void txtCmdLineArgs_TextChanged(object sender, System.EventArgs e)
        {
            UpdateCmdLinePreview();
        }

        /// <summary>
        /// Update the preview display.
        /// </summary>
        /// <param name="sender">system parameter</param>
        /// <param name="e">system parameter</param>

        private void txtTextEditorLocation_TextChanged(object sender, EventArgs e)
        {
            UpdateCmdLinePreview();
        }

        /// <summary>
        /// Update the preview display.
        /// </summary>
        /// <param name="sender">system parameter</param>
        /// <param name="e">system parameter</param>

        private void chkUseQuotesAroundFileName_CheckedChanged(object sender, EventArgs e)
        {
            UpdateCmdLinePreview();
        }

        /// <summary>
        /// Checks to make sure the all file types (*) is used only once
        /// </summary>
        /// <param name="sender">system parameter</param>
        /// <param name="e">system parameter</param>

        private void txtFileType_TextChanged(object sender, System.EventArgs e)
        {
            if (__AllTypesDefined && !__OriginalFileType.Equals(Constants.ALL_FILE_TYPES))
            {
                if (txtFileType.Text.Equals(Constants.ALL_FILE_TYPES))
                {
                    MessageBox.Show(Language.GetGenericText("TextEditorsAllTypesDefined"), ProductInformation.ApplicationName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    txtFileType.Text = string.Empty;
                }
            }
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Returns a preview of what the command line will look like to open a file
        /// </summary>
        /// <returns>Preview of command line</returns>

        private void UpdateCmdLinePreview()
        {
            string previewText = Language.GetControlText(lblCmdOptionsView);
            string editor = string.Empty;
            string editorPath = txtTextEditorLocation.Text;
            string args = txtCmdLineArgs.Text;

            try
            {
                if (!string.IsNullOrEmpty(editorPath))
                {
                    editor = System.IO.Path.GetFileName(editorPath);
                }
            }
            catch
            {
                editor = string.Empty;
            }

            string path = @"c:\file path\filename.txt";
            if (chkUseQuotesAroundFileName.Checked)
            {
                path = "\"" + path + "\"";
            }
            args = args.Replace("%1", path);
            args = args.Replace("%2", "450");
            args = args.Replace("%3", "11");
            args = args.Replace("%4", "searched text");

            lblCmdOptionsView.Text = string.Format(previewText, editor, args);
        }
        #endregion
    }
}
