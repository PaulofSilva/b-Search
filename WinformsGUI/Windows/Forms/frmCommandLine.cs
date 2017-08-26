using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace bSearch.Windows.Forms
{
    /// <summary>
    /// Displays the command line options for this program.
    /// </summary>
    /// <remarks>
    ///   bSearch File Searching Utility.
    ///   Copyright (C) 2006 BigLevel Lda.
    ///   
    ///   The author may be contacted at:
    ///   suporte@bigLevel.pt or comercial@biglevel.pt
    /// </remarks>
    /// <history>
    /// [Paulo_Silva]	   09/26/2012	ADD: 3572487, display command line flags
    /// </history>
    public partial class frmCommandLine : BaseForm
    {
        /// <summary>
        /// Creates an instance of the frmCommandLine class.
        /// </summary>

        public frmCommandLine()
        {
            InitializeComponent();
            API.ListViewExtensions.SetTheme(lstArguments);
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
        }

        /// <summary>
        /// Form Load Event
        /// </summary>
        /// <param name="sender">system parm</param>
        /// <param name="e">system parm</param>

        private void frmCommandLine_Load(object sender, EventArgs e)
        {
            //Language.GenerateXml(this, Application.StartupPath + "\\" + this.Name + ".xml");
            Language.ProcessForm(this);

            lstArguments.Items.Add(new ListViewItem(new string[] { "/spath=\"value\"", "Start Path" }));
            lstArguments.Items.Add(new ListViewItem(new string[] { "/stypes=\"value\"", "File Types" }));
            lstArguments.Items.Add(new ListViewItem(new string[] { "/stext=\"value\"", "Search Text" }));
            lstArguments.Items.Add(new ListViewItem(new string[] { "/local", "Store config files in local directory" }));
            lstArguments.Items.Add(new ListViewItem(new string[] { "/e", "Use regular expressions" }));
            lstArguments.Items.Add(new ListViewItem(new string[] { "/c", "Case sensitive" }));
            lstArguments.Items.Add(new ListViewItem(new string[] { "/w", "Whole Word" }));
            lstArguments.Items.Add(new ListViewItem(new string[] { "/r", "Recursive search (search subfolders)" }));
            lstArguments.Items.Add(new ListViewItem(new string[] { "/n", "Negation" }));
            lstArguments.Items.Add(new ListViewItem(new string[] { "/l", "Line numbers" }));
            lstArguments.Items.Add(new ListViewItem(new string[] { "/f", "File names only" }));
            lstArguments.Items.Add(new ListViewItem(new string[] { "/cl=\"value\"", "Number of context lines" }));
            lstArguments.Items.Add(new ListViewItem(new string[] { "/sh", "Skip hidden files and folders" }));
            lstArguments.Items.Add(new ListViewItem(new string[] { "/ss", "Skip system files and folders" }));
            lstArguments.Items.Add(new ListViewItem(new string[] { "/shf", "Skip hidden files" }));
            lstArguments.Items.Add(new ListViewItem(new string[] { "/shd", "Skip hidden folders" }));
            lstArguments.Items.Add(new ListViewItem(new string[] { "/ssf", "Skip system files" }));
            lstArguments.Items.Add(new ListViewItem(new string[] { "/ssd", "Skip system folders" }));
            lstArguments.Items.Add(new ListViewItem(new string[] { "/srf", "Skip ReadOnly files" }));
            lstArguments.Items.Add(new ListViewItem(new string[] { "/s", "Start searching immediately" }));
            lstArguments.Items.Add(new ListViewItem(new string[] { "/opath=\"value\"", "Save results to path (/s implied)" }));
            lstArguments.Items.Add(new ListViewItem(new string[] { "/otype=\"value\"", "Save results type (json,html,xml,txt)" }));
            lstArguments.Items.Add(new ListViewItem(new string[] { "/exit", "Exit application after search" }));
            lstArguments.Items.Add(new ListViewItem(new string[] { "/dmf=\"operator|value\"", "Date modified file (=,!=,>,<,>=,<=|MM/DD/YYYY)" }));
            lstArguments.Items.Add(new ListViewItem(new string[] { "/dmd=\"operator|value\"", "Date modified directory (=,!=,>,<,>=,<=|MM/DD/YYYY)" }));
            lstArguments.Items.Add(new ListViewItem(new string[] { "/dcf=\"operator|value\"", "Date created file (=,!=,>,<,>=,<=|MM/DD/YYYY)" }));
            lstArguments.Items.Add(new ListViewItem(new string[] { "/dcd=\"operator|value\"", "Date created directory (=,!=,>,<,>=,<=|MM/DD/YYYY)" }));
            lstArguments.Items.Add(new ListViewItem(new string[] { "/minfsize=\"operator|value\"", "Minimum file size (=,!=,>,<,>=,<=|bytes)" }));
            lstArguments.Items.Add(new ListViewItem(new string[] { "/maxfsize=\"operator|value\"", "Maximum file size (=,!=,>,<,>=,<=|bytes)" }));
            lstArguments.Items.Add(new ListViewItem(new string[] { "/minfc=\"value\"", "Minimum file count" }));
        }

        /// <summary>
        /// Closes this form.
        /// </summary>
        /// <param name="sender">system parameter</param>
        /// <param name="e">system parameter</param>

        private void btnOK_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
