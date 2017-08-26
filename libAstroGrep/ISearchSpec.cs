using System.Collections.Generic;

using libbSearch.EncodingDetection;

namespace libbSearch
{
   /// <summary>
   /// ISearchSpec interface to Grep.
   /// </summary>
   /// <remarks>
   /// bSearch File Searching Utility.
   /// Copyright (C) 2006 BigLevel Lda.
   /// 
   /// The author may be contacted at:
   /// suporte@bigLevel.pt or comercial@biglevel.pt
   /// </remarks>

   public interface ISearchSpec
   {
      /// <summary>Array of start directories</summary>
      string[] StartDirectories { get; }

      /// <summary>Full file paths to files that will be searched (if defined, StartDirectories ignored, can be used for Search within Results)</summary>
      string[] StartFilePaths { get; set; }

      /// <summary>Use of directory recursion for grep</summary>
      bool SearchInSubfolders { get; }

      /// <summary>Use of regular expressions for grep</summary>
      bool UseRegularExpressions { get; }

      /// <summary>Use of a case sensitive grep</summary>
      bool UseCaseSensitivity { get; }

      /// <summary>Use of a whole word match grep</summary>
      bool UseWholeWordMatching { get; }

      /// <summary>Use of negation of the grep results</summary>
      bool UseNegation { get; }

      /// <summary>The number of context lines included in grep results</summary>
      int ContextLines { get; }

      /// <summary>The search text</summary>
      string SearchText { get; }

      /// <summary>Whether to return only file names for grep results</summary>
      bool ReturnOnlyFileNames { get; }

      /// <summary>Sets list of FileEncoding objects to force encoding of certain files selected by user</summary>
      List<FileEncoding> FileEncodings { get; }

      /// <summary>Sets encoding options used when detecting encodings</summary>
      EncodingOptions EncodingDetectionOptions { get; }

      /// <summary>The FileFilter</summary>
      string FileFilter { get; }

      /// <summary>
      /// List of FilterItems that will filter out files/directories based on user inputted options.
      /// </summary>
      /// <remarks>
      /// Examples are Files that are readonly, binary, or the name contains certain text.
      /// Directories that are created after a certain date, or marked as system.
      /// </remarks>
      List<FilterItem> FilterItems { get; }
   }
}