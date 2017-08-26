using System;
using System.IO;
using System.Windows.Forms;

using bSearch.Common;
using bSearch.Common.Logging;
using bSearch.Windows;

namespace bSearch.Core
{
    /// <summary>
    /// Methods used to create/delete/verify shortcuts.
    /// </summary>
    /// <remarks>
    ///   bSearch File Searching Utility.
    ///   Copyright (C) 2006 BigLevel Lda.
    /// 
    ///   The author may be contacted at:
    ///   suporte@bigLevel.pt or comercial@biglevel.pt
    /// </remarks>

    public class Shortcuts
    {
        #region Public Methods

        /// <summary>
        /// Create or delete an application shortcut on the user's desktop.
        /// </summary>
        /// <param name="create">True to create shortcut, False to delete it</param>

        public static void SetDesktopShortcut(bool create)
        {
            CreateApplicationShortcut(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), create);
        }

        /// <summary>
        /// Create or delete an application shortcut on the user's start menu.
        /// </summary>
        /// <param name="create">True to create shortcut, False to delete it</param>

        public static void SetStartMenuShortcut(bool create)
        {
            CreateApplicationShortcut(Environment.GetFolderPath(Environment.SpecialFolder.Programs), create);
        }

        /// <summary>
        /// Checks to see if the desktop shortcut exists.
        /// </summary>
        /// <returns>Returns true if the shortcut exists, false otherwise</returns>

        public static bool IsDesktopShortcut()
        {
            return IsShortcut(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory));
        }

        /// <summary>
        /// Checks to see if the start menu shortcut exists.
        /// </summary>
        /// <returns>Returns true if the shortcut exists, false otherwise</returns>

        public static bool IsStartMenuShortcut()
        {
            return IsShortcut(Environment.GetFolderPath(Environment.SpecialFolder.Programs));
        }

        /// <summary>
        /// Checks to see if bSearch is a search option on right-click of folders
        /// </summary>
        /// <returns>True - set, False - not set</returns>

        public static bool IsSearchOption()
        {
            if (Legacy.CheckIfOldSearchOption())
            {
                Legacy.RemoveOldSearchOption();
            }

            Microsoft.Win32.RegistryKey _key = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(@"Directory\shell\bSearch", false);

            // key exists
            if (_key != null)
            {
                _key.Close();
                return true;
            }

            // key doesn't
            return false;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Creates a shortcut (lnk file) using for the application.
        /// </summary>
        /// <param name="location">Directory where the shortcut should be created.</param>
        /// <param name="create">True to create shortcut, False to delete it</param>

        private static void CreateApplicationShortcut(string location, bool create)
        {
            string path = System.IO.Path.Combine(location, string.Format("{0}.lnk", ProductInformation.ApplicationName));
            string oldPath = string.Format("{0}\\{1}.url", location, ProductInformation.ApplicationName);

            if (create)
            {
                //
                // Create shortcut
                //
                try
                {
                    using (API.ShellLink shortcut = new API.ShellLink())
                    {
                        shortcut.Target = Application.ExecutablePath;
                        shortcut.WorkingDirectory = System.IO.Path.GetDirectoryName(Application.ExecutablePath);
                        shortcut.Description = string.Empty;
                        shortcut.DisplayMode = API.ShellLink.LinkDisplayMode.edmNormal;
                        shortcut.Save(path);
                    }
                }
                catch (Exception ex)
                {
                    LogClient.Instance.Logger.Error("Unable to create shortcut at {0} with message {1}", location, ex.Message);
                }
            }
            else
            {
                //
                // Delete shortcut, if exists
                //
                try
                {
                    if (System.IO.File.Exists(path))
                        System.IO.File.Delete(path);

                    // delete old url if exists
                    if (System.IO.File.Exists(oldPath))
                        System.IO.File.Delete(oldPath);
                }
                catch (Exception ex)
                {
                    LogClient.Instance.Logger.Error("Unable to delete shortcut at {0} with message {1}", location, ex.Message);
                }
            }
        }

        /// <summary>
        /// Checks to see if a shortcut exists.
        /// </summary>
        /// <param name="location">Directory where the shortcut could be</param>
        /// <returns>Returns true if the shortcut exists, false otherwise</returns>

        private static bool IsShortcut(string location)
        {
            string path = Path.Combine(location, string.Format("{0}.lnk", ProductInformation.ApplicationName));
            string oldPath = string.Format("{0}\\{1}.url", location, ProductInformation.ApplicationName);

            if (File.Exists(path))
                return true;

            // check for older url based shortcut and create new one
            if (File.Exists(oldPath))
            {
                // delete
                CreateApplicationShortcut(location, false);

                // recreate
                CreateApplicationShortcut(location, true);

                return true;
            }

            return false;
        }
        #endregion
    }
}
