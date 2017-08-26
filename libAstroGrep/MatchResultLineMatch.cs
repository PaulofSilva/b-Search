using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace libbSearch
{
    /// <summary>
    /// Contains the information for an individual match within the a given line.
    /// </summary>
    /// <remarks>
    /// bSearch File Searching Utility.
    /// Copyright (C) 2006 BigLevel Lda.
    /// 
    /// The author may be contacted at:
    /// suporte@bigLevel.pt or comercial@biglevel.pt
    /// </remarks>

    public class MatchResultLineMatch
    {
        /// <summary>Start position of found match in this line</summary>
        public int StartPosition { get; set; }

        /// <summary>Length of found match in this line</summary>
        public int Length { get; set; }

        /// <summary>
        /// Creates an instance of a MatchResultLineMatch.
        /// </summary>
        public MatchResultLineMatch()
        {
            StartPosition = -1;
            Length = 0;
        }

        /// <summary>
        /// Creates an instance of a MatchResultLineMatch with a given start position and length.
        /// </summary>
        /// <param name="startPosition">Start position of found match in this line</param>
        /// <param name="length">Length of found match in this line</param>
        public MatchResultLineMatch(int startPosition, int length)
        {
            StartPosition = startPosition;
            Length = length;
        }
    }
}
