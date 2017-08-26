using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bSearch.Windows.Controls
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    ///   bSearch File Searching Utility.
    ///   Copyright (C) 2006 BigLevel Lda.  
    /// 
    ///   The author may be contacted at:
    ///   suporte@bigLevel.pt or comercial@biglevel.pt
    /// </remarks>

    public class LineNumber
    {
        /// <summary>Line number</summary>
        public int Number { get; set; }

        /// <summary>Determines if current line has a match</summary>
        public bool HasMatch { get; set; }

        /// <summary>The full file path</summary>
        public string FileFullName { get; set; }

        /// <summary>Column number to first match</summary>
        public int ColumnNumber { get; set; }

        /// <summary>
        /// Creates an instance of this class.
        /// </summary>
        public LineNumber()
        {
            Number = -1;
            HasMatch = false;
            FileFullName = string.Empty;
            ColumnNumber = 1;
        }
    }
}
