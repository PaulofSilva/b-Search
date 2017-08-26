using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bSearch.Common
{
   /// <summary>
   /// Contains common application locations (data folder, log file, etc.)
   /// </summary>
   /// <remarks>
   ///   bSearch File Searching Utility.
   ///   Copyright (C) 2006 BigLevel Lda.
   /// 
   ///   The author may be contacted at:
   ///   suporte@bigLevel.pt or comercial@biglevel.pt
   /// </remarks>

   public sealed class ApplicationPaths
   {
      /// <summary>
      /// The data storage location.
      /// </summary>
      public static string DataFolder
      {
         get
         {
            if (ProductInformation.IsPortable)
               return Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
            else
               return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), ProductInformation.ApplicationName);
         }
      }

      /// <summary>The full path to the cache directory</summary>
      public static string CacheDirectory = Path.Combine(DataFolder, "Cache");

      /// <summary>The full path to the log directory</summary>
      public static string LogDirectory = Path.Combine(DataFolder, "Log");
      /// <summary>The full path to the log file</summary>
      public static string LogFile = Path.Combine(LogDirectory, ProductInformation.ApplicationName + ".log");
      /// <summary>The full path to the log archive file</summary>
      public static string LogArchiveFile = Path.Combine(LogDirectory, ProductInformation.ApplicationName + ".{#}.log");

      /// <summary>The full path to the executing assembly's folder</summary>
      public static string ExecutionFolder = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
      /// <summary>The full path to the executing assembly</summary>
      public static string ExecutingAssembly = Path.GetFullPath(System.Reflection.Assembly.GetEntryAssembly().Location);
   }
}
