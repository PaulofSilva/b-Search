using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.Windows.Forms;

using bSearch.Common;
using bSearch.Common.Logging;
using bSearch.Output;
using libbSearch;

namespace bSearch.Windows.Forms
{
    /// <summary>
    /// Print type selection form
    /// </summary>
    /// <remarks>
    ///   bSearch File Searching Utility.
    ///   Copyright (C) 2006 BigLevel Lda.
    ///   
    ///   The author may be contacted at:
    ///   suporte@bigLevel.pt or comercial@biglevel.pt
    /// </remarks>

    public partial class frmPrint : BaseForm
    {
        #region Declarations

        private PrintDocument printDocument = new PrintDocument();
        private string currentContent = string.Empty;
        private int currentCharIndex = 0;

        private Font printFont;
        private Icon previewIcon;
        private MatchResultsExport.ExportSettings settings;

        #endregion


        /// <summary>
        /// Creates an instance of this class setting its private objects
        /// </summary>
        /// <param name="settings">Export settings</param>
        /// <param name="font">Font to use during printing</param>
        /// <param name="icon">The icon to use for print preview dialog</param>

        public frmPrint(MatchResultsExport.ExportSettings settings, Font font, Icon icon)
        {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            this.settings = settings;
            this.printFont = font;
            this.previewIcon = icon;

            printDocument.PrintPage += pdoc_PrintPage;
        }

        /// <summary>
        /// Form Load Event
        /// </summary>
        /// <param name="sender">system parm</param>
        /// <param name="e">system parm</param>

        private void frmPrint_Load(object sender, EventArgs e)
        {
            //Language.GenerateXml(this, Application.StartupPath + "\\" + this.Name + ".xml");
            Language.ProcessForm(this);

            // load the list of types to print
            lstPrintTypes.Items.Add(Language.GetGenericText("PrintTypeSelected"));
            lstPrintTypes.Items.Add(Language.GetGenericText("PrintTypeAll"));
            lstPrintTypes.Items.Add(Language.GetGenericText("PrintTypeFile"));

            // Set the first item as selected
            lstPrintTypes.SelectedIndex = 0;

            // Set the default document settings
            printDocument.DefaultPageSettings.Margins.Left = 25;
            printDocument.DefaultPageSettings.Margins.Top = 25;
            printDocument.DefaultPageSettings.Margins.Bottom = 25;
            printDocument.DefaultPageSettings.Margins.Right = 25;
        }

        #region Control Events
        /// <summary>
        /// Print Button Event
        /// </summary>
        /// <param name="sender">system parm</param>
        /// <param name="e">system parm</param>

        private void cmdPrint_Click(object sender, EventArgs e)
        {
            try
            {
                PrintDialog dialog = new PrintDialog();
                SetDocument();
                dialog.Document = printDocument;

                if (dialog.ShowDialog(this) == DialogResult.OK)
                    printDocument.Print();
            }
            catch (Exception ex)
            {
                LogClient.Instance.Logger.Error("Print Error: {0}", LogClient.GetAllExceptions(ex));

                MessageBox.Show(Language.GetGenericText("PrintErrorPrint"),
                   ProductInformation.ApplicationName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Print Preview Button Event
        /// </summary>
        /// <param name="sender">system parm</param>
        /// <param name="e">system parm</param>

        private void cmdPreview_Click(object sender, EventArgs e)
        {
            try
            {
                PrintPreviewDialog ppd = new PrintPreviewDialog();
                SetDocument();
                ppd.Document = printDocument;

                // Set properties of preview dialog
                ppd.StartPosition = FormStartPosition.Manual;
                Rectangle defaultBounds = new Rectangle(Owner.Left + 50, Owner.Top + 50, Math.Max(Owner.Width - 100, 640), Math.Max(Owner.Height - 100, 480));
                ppd.Bounds = defaultBounds;
                ppd.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
                ppd.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
                ppd.UseAntiAlias = true;
                ppd.FormBorderStyle = FormBorderStyle.Sizable;
                ppd.Icon = previewIcon;
                ppd.Text = Language.GetControlText(cmdPreview).Replace("&", string.Empty);

                // set initial zoom level to 100%
                ppd.PrintPreviewControl.Zoom = 1.0;

                ppd.ShowDialog(this);
            }
            catch (Exception ex)
            {
                LogClient.Instance.Logger.Error("Print Preview Error: {0}", LogClient.GetAllExceptions(ex));

                MessageBox.Show(Language.GetGenericText("PrintErrorPreview"),
                   ProductInformation.ApplicationName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Page Setup Button Event
        /// </summary>
        /// <param name="sender">system parm</param>
        /// <param name="e">system parm</param>

        private void cmdPageSetup_Click(object sender, EventArgs e)
        {
            PageSetupDialog psd = new PageSetupDialog();

            try
            {
                psd.Document = printDocument;
                psd.PageSettings = printDocument.DefaultPageSettings;

                if (psd.ShowDialog(this) == DialogResult.OK)
                    printDocument.DefaultPageSettings = psd.PageSettings;
            }
            catch (Exception ex)
            {
                LogClient.Instance.Logger.Error("Print Page Setup Error: {0}", LogClient.GetAllExceptions(ex));

                MessageBox.Show(Language.GetGenericText("PrintErrorPageSettings"),
                   ProductInformation.ApplicationName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Cancel Button Event
        /// </summary>
        /// <param name="sender">system parm</param>
        /// <param name="e">system parm</param>

        private void cmdCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        #endregion

        #region PrintDocument Events
        /// <summary>
        /// PrintPage Event
        /// </summary>
        /// <param name="sender">system parm</param>
        /// <param name="e">system parm</param>
        /// <remarks>
        ///   PrintPage is the foundational printing event. This event gets fired for every 
        ///   page that will be printed. You could also handle the BeginPrint and EndPrint
        ///   events for more control.
        ///   
        ///   The following is very 
        ///   fast and useful for plain text as MeasureString calculates the text that
        ///   can be fitted on an entire page. This is not that useful, however, for 
        ///   formatted text. In that case you would want to have word-level (vs page-level)
        ///   control, which is more complicated.
        /// </remarks>

        private void pdoc_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            int intPrintAreaHeight;
            int intPrintAreaWidth;
            int marginLeft;
            int marginTop;

            // Initialize local variables that contain the bounds of the printing 
            // area rectangle.
            intPrintAreaHeight = printDocument.DefaultPageSettings.PaperSize.Height - printDocument.DefaultPageSettings.Margins.Top - printDocument.DefaultPageSettings.Margins.Bottom;
            intPrintAreaWidth = printDocument.DefaultPageSettings.PaperSize.Width - printDocument.DefaultPageSettings.Margins.Left - printDocument.DefaultPageSettings.Margins.Right;

            // Initialize local variables to hold margin values that will serve
            // as the X and Y coordinates for the upper left corner of the printing 
            // area rectangle.
            marginLeft = printDocument.DefaultPageSettings.Margins.Left; // X coordinate
            marginTop = printDocument.DefaultPageSettings.Margins.Top; // Y coordinate

            // If the user selected Landscape mode, swap the printing area height 
            // and width.
            if (printDocument.DefaultPageSettings.Landscape)
            {
                int intTemp;
                intTemp = intPrintAreaHeight;
                intPrintAreaHeight = intPrintAreaWidth;
                intPrintAreaWidth = intTemp;
            }

            // Calculate the total number of lines in the document based on the height of
            // the printing area and the height of the font.
            int intLineCount = Convert.ToInt32(intPrintAreaHeight / printFont.Height);

            // Initialize the rectangle structure that defines the printing area.
            RectangleF rectPrintingArea = new RectangleF(marginLeft, marginTop, intPrintAreaWidth, intPrintAreaHeight);

            // Instantiate the StringFormat class, which encapsulates text layout 
            // information (such as alignment and line spacing), display manipulations 
            // (such as ellipsis insertion and national digit substitution) and OpenType 
            // features. Use of StringFormat causes MeasureString and DrawString to use
            // only an integer number of lines when printing each page, ignoring partial
            // lines that would otherwise likely be printed if the number of lines per 
            // page do not divide up cleanly for each page (which is usually the case).
            // See further discussion in the SDK documentation about StringFormatFlags.
            StringFormat fmt = new StringFormat(StringFormatFlags.LineLimit);

            // Call MeasureString to determine the number of characters that will fit in
            // the printing area rectangle. The CharFitted Int32 is passed ByRef and used
            // later when calculating intCurrentChar and thus HasMorePages. LinesFilled 
            // is not needed for this sample but must be passed when passing CharsFitted.
            // Mid is used to pass the segment of remaining text left off from the 
            // previous page of printing (recall that intCurrentChar was declared as 
            // static.
            int intLinesFilled;
            int intCharsFitted;
            e.Graphics.MeasureString(currentContent.Substring(currentCharIndex), printFont,
               new SizeF(intPrintAreaWidth, intPrintAreaHeight), fmt,
               out intCharsFitted, out intLinesFilled);

            // Print the text to the page.
            e.Graphics.DrawString(currentContent.Substring(currentCharIndex), printFont,
               Brushes.Black, rectPrintingArea, fmt);

            // Advance the current char to the last char printed on this page. As 
            // intCurrentChar is a static variable, its value can be used for the next
            // page to be printed. It is advanced by 1 and passed to Mid() to print the
            // next page (see above in MeasureString()).
            currentCharIndex += intCharsFitted;

            // HasMorePages tells the printing module whether another PrintPage event
            // should be fired.
            if (currentCharIndex < currentContent.Length)
            {
                e.HasMorePages = true;
            }
            else
            {
                e.HasMorePages = false;
                // You must explicitly reset currentCharCount as it is static.
                currentCharIndex = 0;
            }
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Set the document to print
        /// </summary>

        private void SetDocument()
        {
            try
            {
                switch (lstPrintTypes.SelectedIndex)
                {
                    case 0:
                        currentContent = OutputResults(MatchResultsExport.PrintSelected);
                        break;
                    case 1:
                        currentContent = OutputResults(MatchResultsExport.PrintAll);
                        break;
                    case 2:
                        currentContent = OutputResults(MatchResultsExport.PrintFileList);
                        break;
                    default:
                        currentContent = string.Format(Language.GetGenericText("PrintErrorDocument"), "invalid selection"); ;
                        break;
                }
            }
            catch (Exception ex)
            {
                LogClient.Instance.Logger.Error("Print SetDocument Error: {0}", LogClient.GetAllExceptions(ex));

                // display error to user in document if an error occurred trying to generate
                // the document for printing
                currentContent = string.Format(Language.GetGenericText("PrintErrorDocument"), ex.Message);
            }
        }

        /// <summary>
        /// Output results using given export delegate.
        /// </summary>
        /// <param name="outputter">Print delegate</param>
        /// <returns></returns>

        private string OutputResults(MatchResultsExport.PrintDelegate outputter)
        {
            return outputter(settings);
        }
        #endregion
    }
}
