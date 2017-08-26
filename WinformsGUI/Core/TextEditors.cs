using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

using bSearch.Common;
using bSearch.Common.Logging;
using bSearch.Windows;

namespace bSearch.Core
{
    /// <summary>
    /// Helper class to handle managing the text editors.
    /// </summary>
    /// <remarks>
    ///   bSearch File Searching Utility.
    ///   Copyright (C) 2006 BigLevel Lda.
    /// 
    ///   The author may be contacted at:
    ///   suporte@bigLevel.pt or comercial@biglevel.pt
    /// </remarks>
    public class TextEditors
    {
        private static TextEditor[] __TextEditors;
        private const string DELIMETER = "|;;|";

        /// <summary>
        /// Open a file with a user defined text editor/executable.
        /// </summary>
        /// <param name="opener">TextEditorOpener object containing the information necessary to edit a file.</param>

        public static void Open(TextEditorOpener opener)
        {
            if (opener != null && opener.HasValue())
            {
                try
                {
                    // pick the correct editor to use
                    System.IO.FileInfo file = new System.IO.FileInfo(opener.Path);
                    TextEditor editorToUse = null;

                    // find extension match
                    if (__TextEditors != null)
                    {
                        foreach (TextEditor editor in __TextEditors)
                        {
                            // handle multiple types for one editor
                            string[] types = new string[1] { editor.FileType };
                            if (editor.FileType.Contains(Constants.TEXT_EDITOR_TYPE_SEPARATOR))
                            {
                                types = editor.FileType.Split(Constants.TEXT_EDITOR_TYPE_SEPARATOR.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                            }

                            // loop through all types defined for this editor
                            foreach (string type in types)
                            {
                                string currentType = type;

                                // add missing start . if file type has it and the user didn't add it.
                                if (currentType != Constants.ALL_FILE_TYPES && !currentType.StartsWith(".") && file.Extension.StartsWith("."))
                                    currentType = string.Format(".{0}", currentType);

                                if (currentType.IndexOf(file.Extension, StringComparison.OrdinalIgnoreCase) > -1)
                                {
                                    // use this editor
                                    editorToUse = editor;
                                    break;
                                }
                            }

                            if (editorToUse != null)
                                break;
                        }

                        // try finding default for all types (*)
                        if (editorToUse == null)
                        {
                            foreach (TextEditor editor in __TextEditors)
                            {
                                if (editor.FileType.Equals(Constants.ALL_FILE_TYPES))
                                {
                                    // use this editor
                                    editorToUse = editor;
                                    break;
                                }
                            }
                        }

                        if (editorToUse == null)
                        {
                            // since nothing defined, just use default app associated with file type
                            OpenWithDefault(opener.Path);
                        }
                        else
                        {
                            // adjust column if tab size is set
                            if (editorToUse.TabSize > 0 && opener.ColumnNumber > 0 && !string.IsNullOrEmpty(opener.LineText))
                            {
                                // count how many tabs before found hit column index
                                int count = 0;
                                for (int i = opener.ColumnNumber - 1; i >= 0; i--)
                                {
                                    if (opener.LineText[i] == '\t')
                                    {
                                        count++;
                                    }
                                }

                                opener.ColumnNumber += ((count * editorToUse.TabSize) - count);
                            }

                            LaunchEditor(editorToUse, opener.Path, opener.LineNumber, opener.ColumnNumber, string.Empty);
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogClient.Instance.Logger.Error("Unable to open text editor for file {0} at line {1}, column {2}, with text {3} and message {4}", opener.Path, opener.LineNumber, opener.ColumnNumber, opener.LineText, ex.Message);

                    MessageBox.Show(String.Format(Language.GetGenericText("TextEditorsErrorGeneric"), opener.Path, ex.Message),
                          ProductInformation.ApplicationName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
        }

        /// <summary>
        /// Opens the given file using the default associated application.
        /// </summary>
        /// <param name="path">Full path to file</param>

        public static void OpenWithDefault(string path)
        {
            System.Diagnostics.Process.Start(path);
        }

        /// <summary>
        /// Loads the user specified text editors.
        /// </summary>

        public static void Load()
        {
            string editorsString = bSearch.Core.GeneralSettings.TextEditors;

            if (editorsString.Length > 0)
            {
                //parse string for each editor
                string[] editors = Utils.SplitByString(editorsString, DELIMETER);
                if (editors.Length > 0)
                {
                    __TextEditors = new TextEditor[editors.Length];

                    for (int i = 0; i < editors.Length; i++)
                    {
                        //parse each editor for class properties
                        __TextEditors[i] = TextEditor.FromString(editors[i]);
                    }
                }
            }
            else
            {
                __TextEditors = Windows.Legacy.ConvertTextEditors();
                Save(__TextEditors);
            }
        }

        /// <summary>
        /// Get the text editors that were loaded.
        /// </summary>
        /// <returns>Array of TextEditor objects</returns>

        public static TextEditor[] GetAll()
        {
            return __TextEditors;
        }

        /// <summary>
        /// Saves the given Array of TextEditor objects.
        /// </summary>
        /// <param name="editors">Array of TextEditor objects</param>

        public static void Save(TextEditor[] editors)
        {
            if (editors != null)
            {
                System.Text.StringBuilder builder = new System.Text.StringBuilder(editors.Length);
                __TextEditors = new TextEditor[editors.Length];
                __TextEditors = editors;

                foreach (TextEditor editor in editors)
                {
                    if (builder.Length > 0)
                    {
                        builder.Append(DELIMETER);
                    }

                    builder.Append(editor.ToString());
                }

                bSearch.Core.GeneralSettings.TextEditors = builder.ToString();
            }
            else
            {
                __TextEditors = null;
                bSearch.Core.GeneralSettings.TextEditors = string.Empty;
            }

            bSearch.Core.GeneralSettings.Save();
        }

        #region Private Methods

        /// <summary>
        /// Open the defined editor for a file that the user has double clicked on.
        /// </summary>
        /// <param name="textEditor">Text editor object reference</param>
        /// <param name="path">Fully qualified file path</param>
        /// <param name="line">Line number</param>
        /// <param name="column">Column position</param>
        /// <param name="searchText">Current search text</param>

        private static void LaunchEditor(TextEditor textEditor, string path, int line, int column, string searchText)
        {
            try
            {
                if (string.IsNullOrEmpty(textEditor.Editor))
                {
                    OpenWithDefault(path);
                }
                else if (textEditor.Arguments.IndexOf("%1") == -1)
                {
                    // no file argument specified
                    MessageBox.Show(Language.GetGenericText("TextEditorsErrorNoCmdLineForFile"),
                       ProductInformation.ApplicationName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    // replace
                    //  %1 with filename 
                    //  %2 with line number
                    //  %3 with column
                    //  %4 with search text
                    string args = textEditor.Arguments;
                    if (textEditor.UseQuotesAroundFileName)
                    {
                        path = "\"" + path + "\"";
                    }
                    args = args.Replace("%1", path);
                    args = args.Replace("%2", line.ToString());
                    args = args.Replace("%3", column.ToString());
                    args = args.Replace("%4", searchText);

                    System.Diagnostics.Process.Start(textEditor.Editor, args);
                }
            }
            catch (Exception ex)
            {
                LogClient.Instance.Logger.Error("Unable to open text editor for editor {0}, file {1} at line {2}, column {3}, with message {4}", textEditor.ToString(), path, line, column, ex.Message);

                MessageBox.Show(String.Format(Language.GetGenericText("TextEditorsErrorGeneric"), path, ex.Message),
                   ProductInformation.ApplicationName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        #endregion
    }
}
