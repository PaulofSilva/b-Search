using System;

namespace libbSearch.Plugin
{
   /// <summary>
   /// Interface definition for bSearch plugins.
   /// </summary>
   /// <remarks>
   ///   bSearch File Searching Utility.
   ///   Copyright (C) 2006 BigLevel Lda.
   /// 
   ///   The author may be contacted at:
   ///   suporte@bigLevel.pt or comercial@biglevel.pt
   /// </remarks>

   public interface IbSearchPlugin
   {
      /// <summary>
      /// Loads the plugin.
      /// </summary>
      /// <returns>Returns true if loaded, false otherwise.</returns>
      bool Load();

      /// <summary>
      /// Loads the plugin.
      /// </summary>
      /// <param name="visible">hide/show plugin information or 
      /// external application during grep process</param>
      /// <returns>Returns true if loaded, false otherwise.</returns>
      bool Load(bool visible);

      /// <summary>
      /// Unloads the plugin.
      /// </summary>
      void Unload();

      /// <summary>
      /// Method that performs grep.
      /// </summary>
      /// <param name="file">FileInfo containing current file</param>
      /// <param name="ex">Contains an Exception if one occurred</param>
      /// <returns>MatchResult containing valid match</returns>
      MatchResult Grep(System.IO.FileInfo file, ISearchSpec searchSpec, ref Exception ex);

      /// <summary>
      /// Determines if given file is supported by current plugin.
      /// </summary>
      /// <param name="file">Current FileInfo object</param>
      /// <returns>True if supported, False if not supported</returns>
      bool IsFileSupported(System.IO.FileInfo file);

      /// <summary>
      /// Gets whether plugin is available to use.
      /// </summary>
      bool IsAvailable { get; }

      /// <summary>
      /// Display of supported extensions of plugin.
      /// </summary>
      string Extensions { get; }

      /// <summary>
      /// Display name of plugin.
      /// </summary>
      string Name { get; }

      /// <summary>
      /// Version of plugin.
      /// </summary>
      string Version { get; }

      /// <summary>
      /// Author of plugin.
      /// </summary>
      string Author { get; }

      /// <summary>
      /// Description of plugin.
      /// </summary>
      string Description { get; }
   }
}