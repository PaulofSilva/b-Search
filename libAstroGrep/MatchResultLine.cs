using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace libbSearch
{
    /// <summary>
    /// Contains the information for a match within a given line.
    /// </summary>
    /// <remarks>
    /// bSearch File Searching Utility.
    /// Copyright (C) 2006 BigLevel Lda.
    /// 
    /// The author may be contacted at:
    /// suporte@bigLevel.pt or comercial@biglevel.pt
    /// </remarks>

    public class MatchResultLine
    {
        /// <summary>Current line</summary>
        public string Line { get; set; }

        /// <summary>Current line number</summary>
        public int LineNumber { get; set; }

        /// <summary>Current column number</summary>
        public int ColumnNumber { get; set; }

        /// <summary>Determines if this line has a match within it</summary>
        public bool HasMatch { get; set; }

        /// <summary>List of line matches</summary>
        public List<MatchResultLineMatch> Matches { get; set; }

        /// <summary>
        /// Initializes this class.
        /// </summary>

        public MatchResultLine()
        {
            Line = string.Empty;
            LineNumber = 1;
            ColumnNumber = 1;
            HasMatch = false;
            Matches = new List<MatchResultLineMatch>();
        }
    }
}
