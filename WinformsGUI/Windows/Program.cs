using System;
using System.Windows.Forms;

using bSearch.Common;
using bSearch.Common.Logging;

namespace bSearch.Windows
{
    /// <summary>
    /// Startup for application
    /// </summary>
    /// <remarks>
    ///   bSearch File Searching Utility.
    ///   Copyright (C) 2006 BigLevel Lda.
    ///   
    ///   The author may be contacted at:
    ///   suporte@bigLevel.pt or comercial@biglevel.pt
    /// </remarks>

    public class Program
    {
        /// <summary>
        /// Starts the application
        /// </summary>
        /// <remarks>
        /// Enables visual styles for the controls if available.
        /// </remarks>

        [STAThread]
        static void Main()
        {
            try
            {
                Application.EnableVisualStyles();
                Application.DoEvents();

                // Parse command line, must be done before any use of user settings and form creation in case of help
                CommandLineProcessing.CommandLineArguments args = CommandLineProcessing.Process(Environment.GetCommandLineArgs());

                // needs to go after command line processing since StoreDataLocal determines log file location
                LogClient.Instance.Logger.Info("### STARTING {0}, version {1}{2} ###",
                   ProductInformation.ApplicationName,
                   ProductInformation.ApplicationVersion.ToString(3),
                   ProductInformation.IsPortable ? " (Portable)" : string.Empty);

                LogClient.Instance.Logger.Info("Operating System: {0}", Environment.OSVersion.VersionString);

                Legacy.ConvertLanguageValue();
                Language.Load(bSearch.Core.GeneralSettings.Language);

                if (args.AnyArguments && args.DisplayHelp)
                {
                    LogClient.Instance.Logger.Info("Displaying command line help window.");

                    // display command line options
                    Application.Run(new Windows.Forms.frmCommandLine());
                }
                else
                {
                    LogClient.Instance.Logger.Info("Displaying main search window.");

                    // display main form
                    Windows.Forms.frmMain mainForm = new bSearch.Windows.Forms.frmMain();
                    mainForm.CommandLineArgs = args;

                    //Application.AddMessageFilter(new ListViewMouseWheelMoveMessageFilter());
                    Application.Run(mainForm);
                }
            }
            catch (Exception ex)
            {
                LogClient.Instance.Logger.Fatal("Unhandled exception, exiting bSearch: {0}", LogClient.GetAllExceptions(ex));

                MessageBox.Show(string.Format("A critical error occurred and bSearch must be shutdown.  Please restart bSearch.\n({0})", ex.Message),
                   ProductInformation.ApplicationName, MessageBoxButtons.OK, MessageBoxIcon.Error);

                Application.Exit();
            }
        }
    }
}