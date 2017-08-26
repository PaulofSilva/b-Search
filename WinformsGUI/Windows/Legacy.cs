using System;

using bSearch.Common;
using bSearch.Core;

namespace bSearch.Windows
{
    /// <summary>
    /// Used to access legacy methods for conversion or removal.
    /// </summary>
    /// <remarks>
    ///   bSearch File Searching Utility.
    ///   Copyright (C) 2006 BigLevel Lda.
    /// 
    ///   The author may be contacted at:
    ///   suporte@bigLevel.pt or comercial@biglevel.pt
    /// </remarks>

    public class Legacy
    {
        /// <summary>
        /// Delete the registry settings for the legacy DEFAULT_EDITOR and EDITOR_ARG
        /// </summary>

        public static void DeleteSingleTextEditor()
        {
            try
            {
                Registry.DeleteStartupSetting("DEFAULT_EDITOR");
                Registry.DeleteStartupSetting("EDITOR_ARG");
            }
            catch { }
        }

        /// <summary>
        /// Checks for the Folder based Search option.
        /// </summary>
        /// <returns>True if found, False otherwise</returns>

        public static bool CheckIfOldSearchOption()
        {
            Microsoft.Win32.RegistryKey _key;
            _key = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(@"Folder\shell\bSearch", false);

            if (_key != null)
                return true;

            return false;
        }

        /// <summary>
        /// Removes the Folder based search option.
        /// </summary>

        public static void RemoveOldSearchOption()
        {
            Microsoft.Win32.RegistryKey _key = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(@"Folder\shell", true);

            if (_key != null)
            {
                try
                {
                    _key.DeleteSubKeyTree("bSearch");
                }
                catch { }
            }
        }

        /// <summary>
        /// Attempt to convert search options in registry to most recent style.
        /// </summary>
        /// <remarks>
        /// Removes any registry settings for search options if found and successfully converted.
        /// </remarks>

        public static void ConvertSearchSettings()
        {
            if (Registry.CheckStartupSetting("USE_REG_EXPRESSIONS"))
            {
                bSearch.Core.SearchSettings.UseRegularExpressions = Registry.GetStartupSetting("USE_REG_EXPRESSIONS", false);
                Registry.DeleteStartupSetting("USE_REG_EXPRESSIONS");
            }

            if (Registry.CheckStartupSetting("USE_CASE_SENSITIVE"))
            {
                bSearch.Core.SearchSettings.UseCaseSensitivity = Registry.GetStartupSetting("USE_CASE_SENSITIVE", false);
                Registry.DeleteStartupSetting("USE_CASE_SENSITIVE");
            }

            if (Registry.CheckStartupSetting("USE_WHOLE_WORD"))
            {
                bSearch.Core.SearchSettings.UseWholeWordMatching = Registry.GetStartupSetting("USE_WHOLE_WORD", false);
                Registry.DeleteStartupSetting("USE_WHOLE_WORD");
            }

            if (Registry.CheckStartupSetting("USE_LINE_NUMBERS"))
            {
                bSearch.Core.SearchSettings.IncludeLineNumbers = Registry.GetStartupSetting("USE_LINE_NUMBERS", true);
                Registry.DeleteStartupSetting("USE_LINE_NUMBERS");
            }

            if (Registry.CheckStartupSetting("USE_RECURSION"))
            {
                bSearch.Core.SearchSettings.UseRecursion = Registry.GetStartupSetting("USE_RECURSION", true);
                Registry.DeleteStartupSetting("USE_RECURSION");
            }

            if (Registry.CheckStartupSetting("SHOW_FILE_NAMES_ONLY"))
            {
                bSearch.Core.SearchSettings.ReturnOnlyFileNames = Registry.GetStartupSetting("SHOW_FILE_NAMES_ONLY", false);
                Registry.DeleteStartupSetting("SHOW_FILE_NAMES_ONLY");
            }

            if (Registry.CheckStartupSetting("USE_NEGATION"))
            {
                bSearch.Core.SearchSettings.UseNegation = Registry.GetStartupSetting("USE_NEGATION", false);
                Registry.DeleteStartupSetting("USE_NEGATION");
            }

            if (Registry.CheckStartupSetting("NUM_CONTEXT_LINES"))
            {
                int lines = Registry.GetStartupSetting("NUM_CONTEXT_LINES", 0);
                if (lines < 0 || lines > Constants.MAX_CONTEXT_LINES)
                    lines = 0;
                bSearch.Core.SearchSettings.ContextLines = lines;
                Registry.DeleteStartupSetting("NUM_CONTEXT_LINES");
            }

            var filterItems = new System.Collections.Generic.List<libbSearch.FilterItem>();

            // old list to new search option
            if (!string.IsNullOrEmpty(Core.GeneralSettings.ExtensionExcludeList))
            {
                var extensions = Core.GeneralSettings.ExtensionExcludeList.Split(';');

                foreach (var ext in extensions)
                {
                    libbSearch.FilterItem item = new libbSearch.FilterItem(new libbSearch.FilterType(libbSearch.FilterType.Categories.File, libbSearch.FilterType.SubCategories.Extension), ext, libbSearch.FilterType.ValueOptions.None, false, true);
                    filterItems.Add(item);
                }

                // set extension exclude to list to empty
                Core.GeneralSettings.ExtensionExcludeList = string.Empty;
            }

            // ExclusionItems to FilterItems
            if (!string.IsNullOrEmpty(Core.SearchSettings.Exclusions))
            {
                var exclusionItems = libbSearch.ExclusionItem.ConvertStringToExclusions(Core.SearchSettings.Exclusions);
                foreach (var oldItem in exclusionItems)
                {
                    libbSearch.FilterItem item = new libbSearch.FilterItem();
                    item.Enabled = oldItem.Enabled;
                    item.Value = oldItem.Value;
                    item.ValueIgnoreCase = oldItem.IgnoreCase;
                    switch (oldItem.Option)
                    {
                        case libbSearch.ExclusionItem.OptionsTypes.Contains:
                            item.ValueOption = libbSearch.FilterType.ValueOptions.Contains;
                            break;

                        case libbSearch.ExclusionItem.OptionsTypes.EndsWith:
                            item.ValueOption = libbSearch.FilterType.ValueOptions.EndsWith;
                            break;

                        case libbSearch.ExclusionItem.OptionsTypes.Equals:
                            item.ValueOption = libbSearch.FilterType.ValueOptions.Equals;
                            break;

                        case libbSearch.ExclusionItem.OptionsTypes.None:
                            item.ValueOption = libbSearch.FilterType.ValueOptions.None;
                            break;

                        case libbSearch.ExclusionItem.OptionsTypes.StartsWith:
                            item.ValueOption = libbSearch.FilterType.ValueOptions.StartsWith;
                            break;
                    }
                    switch (oldItem.Type)
                    {
                        case libbSearch.ExclusionItem.ExclusionTypes.DirectoryName:
                            item.FilterType = new libbSearch.FilterType(libbSearch.FilterType.Categories.Directory, libbSearch.FilterType.SubCategories.Name);
                            break;

                        case libbSearch.ExclusionItem.ExclusionTypes.DirectoryPath:
                            item.FilterType = new libbSearch.FilterType(libbSearch.FilterType.Categories.Directory, libbSearch.FilterType.SubCategories.Path);
                            break;

                        case libbSearch.ExclusionItem.ExclusionTypes.FileExtension:
                            item.FilterType = new libbSearch.FilterType(libbSearch.FilterType.Categories.File, libbSearch.FilterType.SubCategories.Extension);
                            break;

                        case libbSearch.ExclusionItem.ExclusionTypes.FileName:
                            item.FilterType = new libbSearch.FilterType(libbSearch.FilterType.Categories.File, libbSearch.FilterType.SubCategories.Name);
                            break;

                        case libbSearch.ExclusionItem.ExclusionTypes.FilePath:
                            item.FilterType = new libbSearch.FilterType(libbSearch.FilterType.Categories.File, libbSearch.FilterType.SubCategories.Path);
                            break;
                    }
                    filterItems.Add(item);
                }

                // set exclusions list to empty
                Core.SearchSettings.Exclusions = string.Empty;
            }

            if (Core.SearchSettings.MinimumFileCount > 0)
            {
                filterItems.Add(new libbSearch.FilterItem(new libbSearch.FilterType(libbSearch.FilterType.Categories.File, libbSearch.FilterType.SubCategories.MinimumHitCount),
                   Core.SearchSettings.MinimumFileCount.ToString(), libbSearch.FilterType.ValueOptions.None, false, true));

                Core.SearchSettings.MinimumFileCount = 0;
            }

            if (Core.SearchSettings.SkipHidden)
            {
                filterItems.Add(new libbSearch.FilterItem(new libbSearch.FilterType(libbSearch.FilterType.Categories.File, libbSearch.FilterType.SubCategories.Hidden),
                   string.Empty, libbSearch.FilterType.ValueOptions.None, false, true));

                filterItems.Add(new libbSearch.FilterItem(new libbSearch.FilterType(libbSearch.FilterType.Categories.Directory, libbSearch.FilterType.SubCategories.Hidden),
                   string.Empty, libbSearch.FilterType.ValueOptions.None, false, true));

                Core.SearchSettings.SkipHidden = false;
            }

            if (Core.SearchSettings.SkipSystem)
            {
                filterItems.Add(new libbSearch.FilterItem(new libbSearch.FilterType(libbSearch.FilterType.Categories.File, libbSearch.FilterType.SubCategories.System),
                   string.Empty, libbSearch.FilterType.ValueOptions.None, false, true));

                filterItems.Add(new libbSearch.FilterItem(new libbSearch.FilterType(libbSearch.FilterType.Categories.Directory, libbSearch.FilterType.SubCategories.System),
                   string.Empty, libbSearch.FilterType.ValueOptions.None, false, true));

                Core.SearchSettings.SkipSystem = false;
            }

            if (!string.IsNullOrEmpty(Core.SearchSettings.MinimumFileSize))
            {
                filterItems.Add(new libbSearch.FilterItem(new libbSearch.FilterType(libbSearch.FilterType.Categories.File, libbSearch.FilterType.SubCategories.Size),
                   Core.SearchSettings.MinimumFileSize, libbSearch.FilterType.ValueOptions.LessThan, false, Core.SearchSettings.MinimumFileSizeType, true));

                Core.SearchSettings.MinimumFileSize = string.Empty;
                Core.SearchSettings.MinimumFileSizeType = string.Empty;
            }

            if (!string.IsNullOrEmpty(Core.SearchSettings.MaximumFileSize))
            {
                filterItems.Add(new libbSearch.FilterItem(new libbSearch.FilterType(libbSearch.FilterType.Categories.File, libbSearch.FilterType.SubCategories.Size),
                   Core.SearchSettings.MaximumFileSize, libbSearch.FilterType.ValueOptions.GreaterThan, false, Core.SearchSettings.MaximumFileSizeType, true));

                Core.SearchSettings.MaximumFileSize = string.Empty;
                Core.SearchSettings.MaximumFileSizeType = string.Empty;
            }

            if (!string.IsNullOrEmpty(Core.SearchSettings.ModifiedDateStart))
            {
                filterItems.Add(new libbSearch.FilterItem(new libbSearch.FilterType(libbSearch.FilterType.Categories.File, libbSearch.FilterType.SubCategories.DateModified),
                   Core.SearchSettings.ModifiedDateStart, libbSearch.FilterType.ValueOptions.LessThan, false, true));

                Core.SearchSettings.ModifiedDateStart = string.Empty;
            }

            if (!string.IsNullOrEmpty(Core.SearchSettings.ModifiedDateEnd))
            {
                filterItems.Add(new libbSearch.FilterItem(new libbSearch.FilterType(libbSearch.FilterType.Categories.File, libbSearch.FilterType.SubCategories.DateModified),
                   Core.SearchSettings.ModifiedDateEnd, libbSearch.FilterType.ValueOptions.GreaterThan, false, true));

                Core.SearchSettings.ModifiedDateEnd = string.Empty;
            }

            // set filteritems list to new value
            if (filterItems.Count > 0)
            {
                Core.SearchSettings.FilterItems = libbSearch.FilterItem.ConvertFilterItemsToString(filterItems);
            }

            bSearch.Core.SearchSettings.Save();
        }

        /// <summary>
        /// Attempt to convert general settings in registry to most recent style.
        /// </summary>
        /// <remarks>
        /// Removes any registry settings for general settings if found and successfully converted.
        /// </remarks>

        public static void ConvertGeneralSettings()
        {
            // max mru
            if (Registry.CheckStartupSetting("MAX_STORED_PATHS"))
            {
                bSearch.Core.GeneralSettings.MaximumMRUPaths = Registry.GetStartupSetting("MAX_STORED_PATHS", 10);

                if (bSearch.Core.GeneralSettings.MaximumMRUPaths < 0 || bSearch.Core.GeneralSettings.MaximumMRUPaths > Constants.MAX_STORED_PATHS)
                    bSearch.Core.GeneralSettings.MaximumMRUPaths = Constants.MAX_STORED_PATHS;

                Registry.DeleteStartupSetting("MAX_STORED_PATHS");
            }

            // mru values
            ConvertMRUSettings();

            // window settings
            // column widths
            // splitter positions
            ConvertWindowSettings();

            // colors
            ConvertResultColors();

            // exclude list
            if (Registry.CheckStartupSetting("ExtensionExcludeList"))
            {
                bSearch.Core.GeneralSettings.ExtensionExcludeList = Registry.GetStartupSetting("ExtensionExcludeList", string.Empty);
                Registry.DeleteStartupSetting("ExtensionExcludeList");
            }

            // language
            if (Registry.CheckStartupSetting("Language"))
            {
                bSearch.Core.GeneralSettings.Language = Registry.GetStartupSetting("Language", Constants.DEFAULT_LANGUAGE);
                Registry.DeleteStartupSetting("Language");
            }

            bSearch.Core.GeneralSettings.Save();
        }

        /// <summary>
        /// Retrieve the registry values for the text editors and return a Text
        /// </summary>
        /// <returns>TextEditor array, null if values don't exist</returns>

        public static TextEditor[] ConvertTextEditors()
        {
            TextEditor[] editors = null;

            string mruValue = Registry.GetRegistrySetting("TextEditors", "MRUList", string.Empty);

            if (mruValue.Length > 0)
            {
                string display = string.Empty;
                string path = string.Empty;
                string args = string.Empty;
                int max = int.Parse(mruValue);

                editors = new TextEditor[max];

                for (int i = 1; i <= max; i++)
                {
                    path = Registry.GetRegistrySetting("TextEditors", i.ToString(), "-1");
                    args = Registry.GetRegistrySetting("TextEditors", i.ToString() + "_args", "-1");
                    display = Registry.GetRegistrySetting("TextEditors", i.ToString() + "_fileType", string.Empty);

                    if (!path.Equals("-1") && !args.Equals("-1"))
                        editors[i - 1] = new TextEditor(display, path, args);
                }

                Registry.DeleteRegistrySetting("TextEditors");
            }
            else
            {
                // try legacy, if exist, update and remove
                string editorPath = Registry.GetStartupSetting("DEFAULT_EDITOR");
                string editorArgs = Registry.GetStartupSetting("EDITOR_ARG");

                if (editorPath.Length > 0)
                {
                    TextEditor editor = new TextEditor();
                    editor.FileType = "*";
                    editor.Editor = editorPath;
                    editor.Arguments = editorArgs;

                    editors = new TextEditor[1];
                    editors[0] = editor;

                    // remove legacy from registry
                    DeleteSingleTextEditor();
                }
            }

            return editors;
        }

        /// <summary>
        /// Deletes all registry entries for this application.
        /// </summary>

        public static void DeleteRegistry()
        {
            Registry.DeleteAllRegistry();
        }

        /// <summary>
        /// Convert the old language value to the new culture based values.
        /// </summary>

        public static void ConvertLanguageValue()
        {
            // set language to installer selected
            string installerLanguage = Registry.GetInstallerLanguage();
            if (!string.IsNullOrEmpty(installerLanguage))
            {
                Core.GeneralSettings.Language = installerLanguage;
            }

            switch (Core.GeneralSettings.Language)
            {
                case "Español":
                case "1034":
                    Core.GeneralSettings.Language = "es-es";
                    break;

                case "Deutsch":
                case "1031":
                    Core.GeneralSettings.Language = "de-de";
                    break;

                case "Italiano":
                case "1040":
                    Core.GeneralSettings.Language = "it-it";
                    break;

                case "Danish":
                case "Dansk":
                case "1030":
                    Core.GeneralSettings.Language = "da-dk";
                    break;

                case "English":
                case "1033":
                    Core.GeneralSettings.Language = "en-us";
                    break;

                case "French":
                case "Français":
                case "1036":
                    Core.GeneralSettings.Language = "fr-fr";
                    break;

                case "Polski":
                case "1045":
                    Core.GeneralSettings.Language = "pl-pl";
                    break;
            }

            Core.GeneralSettings.Save();
        }

        #region Private Methods
        /// <summary>
        /// Convert and remove registry entries pertaining to MRU lists.
        /// </summary>

        private static void ConvertMRUSettings()
        {
            string _registryValue;
            string _indexNum;
            System.Text.StringBuilder sbPaths = new System.Text.StringBuilder(bSearch.Core.GeneralSettings.MaximumMRUPaths);
            System.Text.StringBuilder sbFilters = new System.Text.StringBuilder(bSearch.Core.GeneralSettings.MaximumMRUPaths);
            System.Text.StringBuilder sbSearches = new System.Text.StringBuilder(bSearch.Core.GeneralSettings.MaximumMRUPaths);

            //  Get the MRU Paths and add them to the path combobox.
            for (int i = 0; i < bSearch.Core.GeneralSettings.MaximumMRUPaths; i++)
            {
                _indexNum = i.ToString();

                //  Get the most recent start pathes
                _registryValue = Registry.GetStartupSetting("MRUPath" + _indexNum);

                //  Add the path to the path combobox.
                if (!_registryValue.Equals(string.Empty))
                {
                    if (sbPaths.Length > 0)
                        sbPaths.Append(Constants.SEARCH_ENTRIES_SEPARATOR);

                    sbPaths.Append(_registryValue);

                    Registry.DeleteStartupSetting("MRUPath" + _indexNum);
                }

                //  Get the most recent File filters
                _registryValue = Registry.GetStartupSetting("MRUFileName" + _indexNum);

                //  Add the file name to the path combobox.
                if (!_registryValue.Equals(string.Empty))
                {
                    if (sbFilters.Length > 0)
                        sbFilters.Append(Constants.SEARCH_ENTRIES_SEPARATOR);

                    sbFilters.Append(_registryValue);

                    Registry.DeleteStartupSetting("MRUFileName" + _indexNum);
                }

                //  Get the most recent search expressions
                _registryValue = Registry.GetStartupSetting("MRUExpression" + _indexNum);

                //  Add the search expression to the path combobox.
                if (!_registryValue.Equals(string.Empty))
                {
                    if (sbSearches.Length > 0)
                        sbSearches.Append(Constants.SEARCH_ENTRIES_SEPARATOR);

                    sbSearches.Append(_registryValue);

                    Registry.DeleteStartupSetting("MRUExpression" + _indexNum);
                }
            }

            if (sbPaths.Length > 0)
                bSearch.Core.GeneralSettings.SearchStarts = sbPaths.ToString();

            if (sbFilters.Length > 0)
                bSearch.Core.GeneralSettings.SearchFilters = sbFilters.ToString();

            if (sbSearches.Length > 0)
                bSearch.Core.GeneralSettings.SearchTexts = sbSearches.ToString();
        }

        /// <summary>
        /// Convert and remove registry entries pertaining to window settings.
        /// </summary>

        private static void ConvertWindowSettings()
        {
            if (Registry.CheckStartupSetting("POS_TOP"))
            {
                bSearch.Core.GeneralSettings.WindowTop = Registry.GetStartupSetting("POS_TOP", -1);
                Registry.DeleteStartupSetting("POS_TOP");
            }
            if (Registry.CheckStartupSetting("POS_LEFT"))
            {
                bSearch.Core.GeneralSettings.WindowLeft = Registry.GetStartupSetting("POS_LEFT", -1);
                Registry.DeleteStartupSetting("POS_LEFT");
            }
            if (Registry.CheckStartupSetting("POS_WIDTH"))
            {
                bSearch.Core.GeneralSettings.WindowWidth = Registry.GetStartupSetting("POS_WIDTH", -1);
                Registry.DeleteStartupSetting("POS_WIDTH");
            }
            if (Registry.CheckStartupSetting("POS_HEIGHT"))
            {
                bSearch.Core.GeneralSettings.WindowHeight = Registry.GetStartupSetting("POS_HEIGHT", -1);
                Registry.DeleteStartupSetting("POS_HEIGHT");
            }
            if (Registry.CheckStartupSetting("POS_STATE"))
            {
                bSearch.Core.GeneralSettings.WindowState = Registry.GetStartupSetting("POS_STATE", -1);
                Registry.DeleteStartupSetting("POS_STATE");
            }

            if (Registry.CheckStartupSetting("SEARCH_PANEL_WIDTH"))
            {
                bSearch.Core.GeneralSettings.WindowSearchPanelWidth = Registry.GetStartupSetting("SEARCH_PANEL_WIDTH", -1);
                Registry.DeleteStartupSetting("SEARCH_PANEL_WIDTH");
            }
            if (Registry.CheckStartupSetting("FILE_PANEL_HEIGHT"))
            {
                bSearch.Core.GeneralSettings.WindowFilePanelHeight = Registry.GetStartupSetting("FILE_PANEL_HEIGHT", -1);
                Registry.DeleteStartupSetting("FILE_PANEL_HEIGHT");
            }

            if (Registry.CheckStartupSetting("FILELIST_FILENAME_WIDTH"))
            {
                bSearch.Core.GeneralSettings.WindowFileColumnNameWidth = Registry.GetStartupSetting("FILELIST_FILENAME_WIDTH", 100);
                Registry.DeleteStartupSetting("FILELIST_FILENAME_WIDTH");
            }
            if (Registry.CheckStartupSetting("FILELIST_LOCATED_IN_WIDTH"))
            {
                bSearch.Core.GeneralSettings.WindowFileColumnLocationWidth = Registry.GetStartupSetting("FILELIST_LOCATED_IN_WIDTH", 200);
                Registry.DeleteStartupSetting("FILELIST_LOCATED_IN_WIDTH");
            }
            if (Registry.CheckStartupSetting("FILELIST_DATE_MODIFIED_WIDTH"))
            {
                bSearch.Core.GeneralSettings.WindowFileColumnDateWidth = Registry.GetStartupSetting("FILELIST_DATE_MODIFIED_WIDTH", 150);
                Registry.DeleteStartupSetting("FILELIST_DATE_MODIFIED_WIDTH");
            }
            if (Registry.CheckStartupSetting("FILELIST_COUNT_WIDTH"))
            {
                bSearch.Core.GeneralSettings.WindowFileColumnCountWidth = Registry.GetStartupSetting("FILELIST_COUNT_WIDTH", 60);
                Registry.DeleteStartupSetting("FILELIST_COUNT_WIDTH");
            }
        }

        /// <summary>
        /// Convert and remove registry entries pertaining to result colors.
        /// </summary>

        private static void ConvertResultColors()
        {
            if (Registry.CheckStartupSetting("HighlightForeColor"))
            {
                bSearch.Core.GeneralSettings.HighlightForeColor = Convertors.ConvertColorToString(Registry.GetStartupSetting("HighlightForeColor", ProductInformation.ApplicationColor));
                Registry.DeleteStartupSetting("HighlightForeColor");
            }
            if (Registry.CheckStartupSetting("HighlightBackColor"))
            {
                bSearch.Core.GeneralSettings.HighlightBackColor = Convertors.ConvertColorToString(Registry.GetStartupSetting("HighlightBackColor", System.Drawing.SystemColors.Window));
                Registry.DeleteStartupSetting("HighlightBackColor");
            }
            if (Registry.CheckStartupSetting("ResultsForeColor"))
            {
                bSearch.Core.GeneralSettings.ResultsForeColor = Convertors.ConvertColorToString(Registry.GetStartupSetting("ResultsForeColor", System.Drawing.SystemColors.WindowText));
                Registry.DeleteStartupSetting("ResultsForeColor");
            }
            if (Registry.CheckStartupSetting("ResultsBackColor"))
            {
                bSearch.Core.GeneralSettings.ResultsBackColor = Convertors.ConvertColorToString(Registry.GetStartupSetting("ResultsBackColor", System.Drawing.SystemColors.Window));
                Registry.DeleteStartupSetting("ResultsBackColor");
            }
        }
        #endregion
    }
}