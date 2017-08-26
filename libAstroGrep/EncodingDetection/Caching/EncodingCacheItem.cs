using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace libbSearch.EncodingDetection.Caching
{
   /// <summary>
   /// Encapsulates an encoding cache item.
   /// </summary>
   /// <remarks>
   ///   bSearch File Searching Utility.
   ///   Copyright (C) 2006 BigLevel Lda.
   /// 
   ///   The author may be contacted at:
   ///   suporte@bigLevel.pt or comercial@biglevel.pt
   /// </remarks>

   [Serializable]
   public class EncodingCacheItem
   {
      /// <summary>Defined Encoding code page value</summary>
      public int CodePage;

      /// <summary>Name of detector used</summary>
      public string DetectorName;

      /// <summary>The file size</summary>
      [OptionalField(VersionAdded = 2)]
      public long FileSize;
   }
}
