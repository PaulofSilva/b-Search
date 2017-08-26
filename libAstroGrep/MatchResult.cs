using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace libbSearch
{
    /// <summary>
    /// Contains all the information for all the matches within a file.
    /// </summary>
    /// <remarks>
    /// bSearch File Searching Utility.
    /// Copyright (C) 2006 BigLevel Lda.
    /// 
    /// The author may be contacted at:
    /// suporte@bigLevel.pt or comercial@biglevel.pt
    /// </remarks>

    public class MatchResult
    {
        private FileInfo file = null;
        private Encoding detectedEncoding = null;
        private List<MatchResultLine> matches = new List<MatchResultLine>();
        private bool fromPlugin = false;

        /// <summary>
        /// Gets/Sets the current FileInfo for this MatchResult.
        /// </summary>

        public FileInfo File
        {
            get { return file; }
            set { file = value; }
        }

        /// <summary>
        /// Gets/Sets the Index of the hit in the collection
        /// </summary>

        public int Index { get; set; }

        /// <summary>
        /// Gets the total hit count in the object
        /// </summary>

        public int HitCount { get; private set; }

        /// <summary>
        /// Gets/Sets the detected file encoding.
        /// </summary>

        public Encoding DetectedEncoding
        {
            get { return detectedEncoding; }
            set { detectedEncoding = value; }
        }

        /// <summary>
        /// Gets/Sets all the MatchResultLines for this MatchResult.
        /// </summary>
        public List<MatchResultLine> Matches
        {
            get { return matches; }
            set { matches = value; }
        }

        /// <summary>
        /// Gets/Sets whether this MatchResult is from a plugin.
        /// </summary>
        public bool FromPlugin
        {
            get { return fromPlugin; }
            set { fromPlugin = value; }
        }

        /// <summary>
        /// Initializes this MatchResult with the current FileInfo.
        /// </summary>
        /// <param name="file">Current FileInfo</param>

        public MatchResult(FileInfo file)
        {
            this.File = file;
            HitCount = 0;
        }

        /// <summary>
        /// Updates the total hit count
        /// </summary>

        public void SetHitCount()
        {
            SetHitCount(1);
        }

        /// <summary>
        /// Updates the total hit count
        /// </summary>
        /// <param name="count">Value to add count to total</param>

        public void SetHitCount(int count)
        {
            HitCount += count;
        }

        /// <summary>
        /// Retrieves the first MatchResultLine from the MatchResult list.
        /// </summary>
        /// <returns>First MatchResultLine that contains a match, otherwise null</returns>

        public MatchResultLine GetFirstMatch()
        {
            return (from m in matches where m.HasMatch select m).FirstOrDefault();
        }
    }
}
