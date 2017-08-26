using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bSearch.Common
{
   /// <summary>
   /// Contains common application related information details (like Name, Version, etc.)
   /// </summary>
   /// <remarks>
   ///   b-Search File Searching Utility.
   ///   Copyright (C) 2006 BigLevel Lda.
   /// 
   ///   The author may be contacted at:
   ///   suporte@bigLevel.pt or comercial@biglevel.pt
   /// </remarks>
   /// <history>
   /// [Paulo_Silva]		17/06/2017	Initial, moved some from Core\Common to here
   /// </history>
   public sealed class ProductInformation
   {
      /// <summary>The application's display name</summary>
      public static string ApplicationName = "b-Search";

      /// <summary>
      /// The application's current version.
      /// </summary>
      public static Version ApplicationVersion
      {
         get
         {
            System.Reflection.Assembly _assembly = System.Reflection.Assembly.GetEntryAssembly();
            return _assembly.GetName().Version;
         }
      }

      /// <summary>The application's desired color</summary>
      public static Color ApplicationColor = Color.FromArgb(251, 127, 6);

      /// <summary>Determines if application is in portable mode</summary>
      public static bool IsPortable
      {
         get
         {
#if PORTABLE
            return true;
#else
            return false;
#endif
         }
      }

      /// <summary>The url to the help page</summary>
      public static string HelpUrl = "http://www.bsoftware.pt/";
      /// <summary>The url to the regular expressions help page</summary>
      public static string RegExHelpUrl = "https://msdn.microsoft.com/en-us/library/az24scfc.aspx";
      /// <summary>The url to the current version</summary>
      public static string VersionUrl = "http://www.bsoftware.pt";
      /// <summary>The url to the download page</summary>
      public static string DownloadUrl = "http://bsoftware.pt/download/";
      /// <summary>The url to the donation page</summary>
      public static string DonationUrl = "https://www.bsoftware.pt";
   }
}