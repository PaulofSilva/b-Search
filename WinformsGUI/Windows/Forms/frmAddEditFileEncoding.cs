using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using bSearch.Common;
using libbSearch;
using libbSearch.EncodingDetection;

namespace bSearch.Windows.Forms
{
    /// <summary>
    /// Used to edit a single file encoding item.
    /// </summary>
    /// <remarks>
    ///   bSearch File Searching Utility. 
    ///   Copyright (C) 2006 BigLevel Lda.
    /// 
    ///   The author may be contacted at:
    ///   suporte@bigLevel.pt or comercial@biglevel.pt
    /// </remarks>
    /// <history>
    /// [Paulo_Silva]      02/09/2015	CHG: 92, support for specific file encodings
    /// </history>
    public partial class frmAddEditFileEncoding : BaseForm
    {
        /// <summary>
        /// The currently selected FileEncoding.
        /// </summary>
        public FileEncoding SelectedFileEncoding
        {
            get;
            set;
        }

        /// <summary>
        /// Initialize the form.
        /// </summary>

        public frmAddEditFileEncoding()
        {
            InitializeComponent();

            // load combo box with available encodings
            cboEncodings.DisplayMember = "DisplayName";
            cboEncodings.ValueMember = "CodePage";
            cboEncodings.DataSource = Encoding.GetEncodings();
        }

        /// <summary>
        /// Setup the form with language and selected values.
        /// </summary>
        /// <param name="sender">system parameter</param>
        /// <param name="e">system parameter</param>

        private void frmAddEditForceEncodingFile_Load(object sender, EventArgs e)
        {
            Language.ProcessForm(this, toolTip1);

            if (SelectedFileEncoding != null)
            {
                txtFile.Text = SelectedFileEncoding.FilePath;
                cboEncodings.SelectedValue = SelectedFileEncoding.CodePage;
            }
        }

        /// <summary>
        /// Handle the browse file click event.
        /// </summary>
        /// <param name="sender">system parameter</param>
        /// <param name="e">system parameter</param>

        private void picBrowse_Click(object sender, EventArgs e)
        {
            var dlg = new OpenFileDialog();
            dlg.Multiselect = false;

            // set initial directory if valid
            if (System.IO.File.Exists(txtFile.Text))
            {
                dlg.FileName = txtFile.Text;
            }

            // display dialog and setup path if selected
            if (dlg.ShowDialog(this) == DialogResult.OK)
            {
                txtFile.Text = dlg.FileName;
            }
        }

        /// <summary>
        /// Handle the OK click event (verify and set entry).
        /// </summary>
        /// <param name="sender">system parameter</param>
        /// <param name="e">system parameter</param>

        private void btnOK_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.None;

            var file = VerifyInterface();
            if (file != null)
            {
                SelectedFileEncoding = file;

                this.DialogResult = System.Windows.Forms.DialogResult.OK;
                this.Close();
            }
        }

        /// <summary>
        /// Handle the Cancel click event.
        /// </summary>
        /// <param name="sender">system parameter</param>
        /// <param name="e">system parameter</param>

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }

        /// <summary>
        /// Verify the interface values are valid.
        /// </summary>
        /// <returns>FileEncoding object based on inputs, null if not valid (displays message to user)</returns>

        private FileEncoding VerifyInterface()
        {
            if (string.IsNullOrEmpty(txtFile.Text) || !System.IO.File.Exists(txtFile.Text))
            {
                MessageBox.Show(this, "Please select a valid file.", ProductInformation.ApplicationName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return null;
            }

            if (cboEncodings.SelectedItem == null)
            {
                MessageBox.Show(this, "Please select an encoding.", ProductInformation.ApplicationName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return null;
            }

            FileEncoding file = new FileEncoding();
            file.Enabled = true;
            file.FilePath = txtFile.Text;
            file.CodePage = (int)cboEncodings.SelectedValue;

            return file;
        }
    }
}
