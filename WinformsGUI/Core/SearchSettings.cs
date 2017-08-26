using System;
using System.IO;

using bSearch.Common;

namespace bSearch.Core
{
    /// <summary>
    /// Used to access the search option settings.
    /// </summary>
    /// <remarks>
    ///   bSearch File Searching Utility.
    ///   Copyright (C) 2006 BigLevel Lda.
    /// 
    ///   The author may be contacted at:
    ///   suporte@bigLevel.pt or comercial@biglevel.pt
    /// </remarks>
    /// <history>
    /// [Paulo_Silva]      18/06/2017  Created
    /// </history>
    public sealed class SearchSettings
    {
        // This class is fully static.
        private SearchSettings() { }

        #region Declarations
        private static SearchSettings __MySettings = null;

        private const string VERSION = "1.0";

        private bool regularExpressions = false;
        private bool caseSensitive = false;
        private bool wholeWord = false;
        private bool recurse = true;
        private bool fileNamesOnly = false;
        private bool negation = false;
        private bool lineNumbers = true;
        private int contextLines = 2;
        private bool showAllResultsAfterSearch = false;

        private string filterItems = Constants.DefaultFilterItems;

        #region Deprecated Property Declarations
        private bool skipHidden = false;
        private bool skipSystem = false;
        private string minFileSize = string.Empty;
        private string maxFileSize = string.Empty;
        private string minFileSizeType = "byte";
        private string maxFileSizeType = "byte";
        private string modifiedStart = string.Empty;
        private string modifiedEnd = string.Empty;
        private string exclusions = string.Empty;
        private int minimumFileCount = 0;
        #endregion

        #endregion

        /// <summary>
        /// Contains the static reference of this class.
        /// </summary>
        private static SearchSettings MySettings
        {
            get
            {
                if (__MySettings == null)
                {
                    __MySettings = new SearchSettings();
                    SettingsIO.Load(__MySettings, Location, VERSION);
                }
                return __MySettings;
            }
        }

        /// <summary>
        /// Gets the full location to the config file.
        /// </summary>
        static public string Location
        {
            get
            {
                return Path.Combine(ApplicationPaths.DataFolder, "bSearch.search.config");
            }
        }

        /// <summary>
        /// Save the search options.
        /// </summary>
        /// <returns>Returns true on success, false otherwise</returns>
        public static bool Save()
        {
            return SettingsIO.Save(MySettings, Location, VERSION);
        }

        /// <summary>
        /// Use regular expressions.
        /// </summary>
        static public bool UseRegularExpressions
        {
            get { return MySettings.regularExpressions; }
            set { MySettings.regularExpressions = value; }
        }

        /// <summary>
        /// Use case sensitivity.
        /// </summary>
        static public bool UseCaseSensitivity
        {
            get { return MySettings.caseSensitive; }
            set { MySettings.caseSensitive = value; }
        }

        /// <summary>
        /// Use whole word matching.
        /// </summary>
        static public bool UseWholeWordMatching
        {
            get { return MySettings.wholeWord; }
            set { MySettings.wholeWord = value; }
        }

        /// <summary>
        /// Use recursion to search sub directories.
        /// </summary>
        static public bool UseRecursion
        {
            get { return MySettings.recurse; }
            set { MySettings.recurse = value; }
        }

        /// <summary>
        /// Retrieve only file names.
        /// </summary>
        static public bool ReturnOnlyFileNames
        {
            get { return MySettings.fileNamesOnly; }
            set { MySettings.fileNamesOnly = value; }
        }

        /// <summary>
        /// Retrieve all files not containing search text.
        /// </summary>
        static public bool UseNegation
        {
            get { return MySettings.negation; }
            set { MySettings.negation = value; }
        }

        /// <summary>
        /// Include line numbers.
        /// </summary>
        static public bool IncludeLineNumbers
        {
            get { return MySettings.lineNumbers; }
            set { MySettings.lineNumbers = value; }
        }

        /// <summary>
        /// Number of context lines to display.
        /// </summary>
        static public int ContextLines
        {
            get { return MySettings.contextLines; }
            set { MySettings.contextLines = value; }
        }

        /// <summary>
        /// Search filter items.
        /// </summary>
        static public string FilterItems
        {
            get { return MySettings.filterItems; }
            set { MySettings.filterItems = value; }
        }

        /// <summary>
        /// Show all results after a search.
        /// </summary>
        static public bool ShowAllResultsAfterSearch
        {
            get { return MySettings.showAllResultsAfterSearch; }
            set { MySettings.showAllResultsAfterSearch = value; }
        }

        #region Deprecated Properties
        /// <summary>
        /// Skip hidden files and directories.
        /// </summary>
        static public bool SkipHidden
        {
            get { return MySettings.skipHidden; }
            set { MySettings.skipHidden = value; }
        }

        /// <summary>
        /// Skip system files and directories.
        /// </summary>
        static public bool SkipSystem
        {
            get { return MySettings.skipSystem; }
            set { MySettings.skipSystem = value; }
        }

        /// <summary>
        /// Minimum file size.
        /// </summary>
        static public string MinimumFileSize
        {
            get { return MySettings.minFileSize; }
            set { MySettings.minFileSize = value; }
        }

        /// <summary>
        /// Maximum file size.
        /// </summary>
        static public string MaximumFileSize
        {
            get { return MySettings.maxFileSize; }
            set { MySettings.maxFileSize = value; }
        }

        /// <summary>
        /// Minimum file size type.
        /// </summary>
        static public string MinimumFileSizeType
        {
            get { return MySettings.minFileSizeType; }
            set { MySettings.minFileSizeType = value; }
        }

        /// <summary>
        /// Maximum file size type.
        /// </summary>
        static public string MaximumFileSizeType
        {
            get { return MySettings.maxFileSizeType; }
            set { MySettings.maxFileSizeType = value; }
        }

        /// <summary>
        /// Modified date start.
        /// </summary>
        static public string ModifiedDateStart
        {
            get { return MySettings.modifiedStart; }
            set { MySettings.modifiedStart = value; }
        }

        /// <summary>
        /// Modified date end.
        /// </summary>
        static public string ModifiedDateEnd
        {
            get { return MySettings.modifiedEnd; }
            set { MySettings.modifiedEnd = value; }
        }

        /// <summary>
        /// Search exlusions.
        /// </summary>
        static public string Exclusions
        {
            get { return MySettings.exclusions; }
            set { MySettings.exclusions = value; }
        }

        /// <summary>
        /// Minimum File Count.
        /// </summary>
        static public int MinimumFileCount
        {
            get { return MySettings.minimumFileCount; }
            set { MySettings.minimumFileCount = value; }
        }
        #endregion
    }
}
