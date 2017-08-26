using System;
using System.Collections.Generic;
using System.Text;

namespace FolderSearchOption
{
    /// <summary>
    /// Handle setting right click option to search using bSearch (asks for Escalation)
    /// </summary>
    /// <remarks>
    ///   bSearch File Searching Utility.
    ///   Copyright (C) 2006 BigLevel Lda.
    /// 
    ///   The author may be contacted at:
    ///   suporte@bigLevel.pt or comercial@biglevel.pt
    /// </remarks>
    /// <history>
    /// [Paulo_Silva]      16/07/2017	Initial: handle UAC request for right click option
    /// </history>
    class Program
    {
        /// <summary>
        /// Run application with given arguments.
        /// </summary>
        /// <param name="args">Command Line Arguments</param>

        static void Main(string[] args)
        {
            // expecting:  \"True\" \"C:\\Code\\WinformsGUI\\bin\\Debug\\bSearch.EXE\" \"Search using {0}...\"
            if (args.Length > 0 && args.Length <= 3)
            {
                bool setOption = false;
                string path = string.Empty;
                string explorerText = "Search using {0}...";

                // setup values from args
                bool.TryParse(args[0], out setOption);
                path = args[1].Replace("\"", "");
                explorerText = args[2].Replace("\"", "");

                SetAsSearchOption(setOption, path, explorerText);
            }
        }

        /// <summary>
        /// Set registry entry to make application a right-click option on a foler
        /// </summary>
        /// <param name="setOption">True - Set registry value, False - remove registry value</param>
        /// <param name="path">Full path to bSearch.exe</param>
        /// <param name="explorerText">Text to be displayed in Explorer context menu</param>
        /// <history>
        /// [Paulo_Silva]	   16/06/2017	Created
        /// [Paulo_Silva]	   17/06/2017	use drive/directory instead of folder
        /// </history>
        public static void SetAsSearchOption(bool setOption, string path, string explorerText)
        {
            try
            {
                Microsoft.Win32.RegistryKey _key = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(@"Directory\shell", true);
                Microsoft.Win32.RegistryKey _bgKey = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(@"Directory\Background\shell", true);
                Microsoft.Win32.RegistryKey _driveKey = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(@"Drive\shell", true);
                Microsoft.Win32.RegistryKey _bSearchKey;
                Microsoft.Win32.RegistryKey _bSearchBGKey;
                Microsoft.Win32.RegistryKey _bSearchDriveKey;

                if (_key != null && _bgKey != null && _driveKey != null)
                {
                    if (setOption)
                    {
                        // create keys
                        _bSearchKey = _key.CreateSubKey("bSearch");
                        _bSearchBGKey = _bgKey.CreateSubKey("bSearch");
                        _bSearchDriveKey = _driveKey.CreateSubKey("bSearch");

                        if (_bSearchKey != null && _bSearchBGKey != null && _bSearchDriveKey != null)
                        {
                            _bSearchKey.SetValue("", String.Format(explorerText, "&bSearch"));
                            _bSearchBGKey.SetValue("", String.Format(explorerText, "&bSearch"));
                            _bSearchDriveKey.SetValue("", String.Format(explorerText, "&bSearch"));

                            // shows icon in Windows 7+
                            _bSearchKey.SetValue("Icon", string.Format("\"{0}\",0", path));
                            _bSearchBGKey.SetValue("Icon", string.Format("\"{0}\",0", path));
                            _bSearchDriveKey.SetValue("Icon", string.Format("\"{0}\",0", path));

                            Microsoft.Win32.RegistryKey _commandKey = _bSearchKey.CreateSubKey("command");
                            Microsoft.Win32.RegistryKey _commandBGKey = _bSearchBGKey.CreateSubKey("command");
                            Microsoft.Win32.RegistryKey _commandDriveKey = _bSearchDriveKey.CreateSubKey("command");
                            if (_commandKey != null && _commandBGKey != null && _commandDriveKey != null)
                            {
                                string keyValue = string.Format("\"{0}\" \"%L\"", path);
                                _commandKey.SetValue("", keyValue);
                                _commandDriveKey.SetValue("", keyValue);

                                // background needs %V
                                _commandBGKey.SetValue("", string.Format("\"{0}\" \"%V\"", path));
                            }
                        }
                    }
                    else
                    {
                        // remove keys
                        try
                        {
                            _key.DeleteSubKeyTree("bSearch");
                            _bgKey.DeleteSubKeyTree("bSearch");
                            _driveKey.DeleteSubKeyTree("bSearch");
                        }
                        catch { }
                    }
                }
            }
            catch { }
        }
    }
}
