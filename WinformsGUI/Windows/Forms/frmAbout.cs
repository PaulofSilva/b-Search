using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text;
using System.Windows.Forms;

using bSearch.Common;
using bSearch.Core;

namespace bSearch.Windows.Forms
{
    /// <summary>
    /// About Dialog
    /// </summary>
    /// <remarks>
    ///   bSearch File Searching Utility.
    ///   Copyright (C) 2006 BigLevel Lda.
    /// 
    ///   The author may be contacted at:
    ///   suporte@bigLevel.pt or comercial@biglevel.pt
    /// </remarks>

    public partial class frmAbout : BaseForm
    {
        /// <summary>
        /// Creates an instance of the frmAbout class.
        /// </summary>

        public frmAbout()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Opens the systems default browser and displays the web link
        /// </summary>
        /// <param name="sender">System parm</param>
        /// <param name="e">System parm</param>

        private void LicenseLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(e.Link.LinkData.ToString());
        }

        /// <summary>
        /// Opens the systems default browser and displays the web link
        /// </summary>
        /// <param name="sender">System parm</param>
        /// <param name="e">System parm</param>

        private void lnkHomePage_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(e.Link.LinkData.ToString());
        }

        /// <summary>
        /// Load values for form
        /// </summary>
        /// <param name="sender">System parm</param>
        /// <param name="e">System parm</param>

        private void frmAbout_Load(object sender, System.EventArgs e)
        {
            //Language.GenerateXml(Me, Application.StartupPath & "\" & Me.Name & ".xml")
            Language.ProcessForm(this);

            this.Text = string.Format(this.Text, ProductInformation.ApplicationName);
            lnkHomePage.Text = string.Format(lnkHomePage.Text, ProductInformation.ApplicationName);
            lnkHomePage.Links.Add(0, lnkHomePage.Text.Length, "http://bsoftware.pt/");
            LicenseLinkLabel.Links.Add(0, LicenseLinkLabel.Text.Length, "http://www.bsoftware.pt/lic.html");

            CopyrightLabel.Text = string.Format(CopyrightLabel.Text, DateTime.Now.Year.ToString());
            lblProductName.Text = ProductInformation.ApplicationName;
            lblProductVersion.Text = string.Format("{0}{1}", ProductInformation.ApplicationVersion.ToString(3), ProductInformation.IsPortable ? " (Portable)" : string.Empty);
        }

        /// <summary>
        /// Process Escape and Enter keys for this form.
        /// </summary>
        /// <param name="keyData">Current key data</param>
        /// <returns>true if processed, false otherwise</returns>

        protected override bool ProcessDialogKey(Keys keyData)
        {
            if (Form.ModifierKeys == Keys.None && keyData == Keys.Escape)
            {
                this.Close();
                return true;
            }
            else if (Form.ModifierKeys == Keys.None && keyData == Keys.Enter)
            {
                return true;
            }

            return base.ProcessDialogKey(keyData);
        }
    }
}
