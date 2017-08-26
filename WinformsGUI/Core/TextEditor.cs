using System;

using bSearch.Common;

namespace bSearch
{
    /// <summary>
    /// Represents a Text Editor application.
    /// </summary>
    /// <remarks>
    ///   bSearch File Searching Utility.
    ///   Copyright (C) 2006 BigLevel Lda.
    /// 
    ///   The author may be contacted at:
    ///   suporte@bigLevel.pt or comercial@biglevel.pt
    /// </remarks>

    public class TextEditor
    {
        #region Declarations
        private const string DELIMETER = "|@@|";

        private string fileType = string.Empty;
        private string editor = string.Empty;
        private string arguments = string.Empty;
        private int tabSize = 0;
        private bool useQuotesAroundFileName = true;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the TextEditor class.
        /// </summary>
        public TextEditor()
        {

        }

        /// <summary>
        /// Initializes a new instance of the TextEditor class.
        /// </summary>
        /// <param name="editorPath">Text Editor Path</param>
        /// <param name="editorArgs">Text Editor Command Line Arguments</param>

        public TextEditor(string editorPath, string editorArgs)
        {
            editor = editorPath;
            arguments = editorArgs;
        }

        /// <summary>
        /// Initializes a new instance of the TextEditor class.
        /// </summary>
        /// <param name="fileType">Text Editor File Type</param>
        /// <param name="editorPath">Text Editor Path</param>
        /// <param name="editorArgs">Text Editor Command Line Arguments</param>

        public TextEditor(string fileType, string editorPath, string editorArgs)
            : this(editorPath, editorArgs)
        {
            this.fileType = fileType;
        }

        /// <summary>
        /// Initializes a new instance of the TextEditor class.
        /// </summary>
        /// <param name="fileType">Text Editor File Type</param>
        /// <param name="editorPath">Text Editor Path</param>
        /// <param name="editorArgs">Text Editor Command Line Arguments</param>
        /// <param name="tabSize">Tab Size for Text Editor</param>

        public TextEditor(string fileType, string editorPath, string editorArgs, int tabSize)
            : this(fileType, editorPath, editorArgs)
        {
            this.tabSize = tabSize;
        }

        /// <summary>
        /// Initializes a new instance of the TextEditor class.
        /// </summary>
        /// <param name="fileType">Text Editor File Type</param>
        /// <param name="editorPath">Text Editor Path</param>
        /// <param name="editorArgs">Text Editor Command Line Arguments</param>
        /// <param name="tabSize">Tab Size for Text Editor</param>
        /// <param name="useQuotesAroundFileName">Use quotes around file name</param>

        public TextEditor(string fileType, string editorPath, string editorArgs, int tabSize, bool useQuotesAroundFileName)
            : this(fileType, editorPath, editorArgs, tabSize)
        {
            this.useQuotesAroundFileName = useQuotesAroundFileName;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Contains the file type.
        /// </summary>

        public string FileType
        {
            get { return fileType; }
            set { fileType = value; }
        }

        /// <summary>
        /// Contains the location.
        /// </summary>

        public string Editor
        {
            get { return editor; }
            set { editor = value; }
        }

        /// <summary>
        /// Contains the command line arguments.
        /// </summary>

        public string Arguments
        {
            get { return arguments; }
            set { arguments = value; }
        }

        /// <summary>
        /// Contains the editor's tab size.
        /// </summary>

        public int TabSize
        {
            get { return tabSize; }
            set { tabSize = value; }
        }

        /// <summary>
        /// Determines whether to wrap the file name with quotes.
        /// </summary>

        public bool UseQuotesAroundFileName
        {
            get { return useQuotesAroundFileName; }
            set { useQuotesAroundFileName = value; }
        }
        #endregion

        #region public Methods
        /// <summary>
        /// Gets the string representation of this class.
        /// </summary>
        /// <returns></returns>

        public override string ToString()
        {
            return string.Format("{1}{0}{2}{0}{3}{0}{4}{0}{5}", DELIMETER, editor, arguments, fileType, tabSize, useQuotesAroundFileName);
        }

        /// <summary>
        /// Translates the given string representation into the class/s properties.
        /// </summary>
        /// <param name="classAsString">The string representation of this class</param>

        public static TextEditor FromString(string classAsString)
        {
            TextEditor editor = new TextEditor();

            if (classAsString.Length > 0 && classAsString.IndexOf(DELIMETER) > -1)
            {
                string[] values = Utils.SplitByString(classAsString, DELIMETER);

                if (values.Length >= 3)
                {
                    editor.Editor = values[0];
                    editor.Arguments = values[1];
                    editor.FileType = values[2];
                }

                if (values.Length >= 4)
                {
                    int size = 0;
                    if (int.TryParse(values[3], out size))
                    {
                        editor.TabSize = size;
                    }
                }

                if (values.Length >= 5)
                {
                    bool useQuotes = true;
                    if (bool.TryParse(values[4], out useQuotes))
                    {
                        editor.UseQuotesAroundFileName = useQuotes;
                    }
                }
            }

            return editor;
        }
        #endregion
    }
}
