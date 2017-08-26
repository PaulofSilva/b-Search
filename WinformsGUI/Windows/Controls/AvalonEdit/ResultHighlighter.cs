using System;
using System.Linq;
using System.Windows.Media;

using libbSearch;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Rendering;

namespace bSearch.Windows.Controls
{
    /// <summary>
    /// Handles highlight for a single file/MatchResult.
    /// </summary>
    /// <remarks>
    ///   bSearch File Searching Utility. 
    ///   Copyright (C) 2006 BigLevel Lda.
    /// 
    ///   The author may be contacted at:
    ///   suporte@bigLevel.pt or comercial@biglevel.pt
    /// </remarks>

    public class ResultHighlighter : DocumentColorizingTransformer
    {
        private MatchResult match;
        private bool removeWhiteSpace = false;
        private bool showingFullFile = false;
        private SolidColorBrush matchForeground = new SolidColorBrush(Colors.White);
        private SolidColorBrush matchBackground = new SolidColorBrush(Color.FromRgb(251, 127, 6));
        private SolidColorBrush nonmatchForeground = new SolidColorBrush(Color.FromRgb(192, 192, 192));

        /// <summary>
        /// Creates an instance of this class.
        /// </summary>
        /// <param name="match">The current MatchResult</param>
        /// <param name="removeWhiteSpace">Determines if leading white space was removed</param>

        public ResultHighlighter(MatchResult match, bool removeWhiteSpace)
        {
            this.match = match;
            this.removeWhiteSpace = removeWhiteSpace;
        }

        /// <summary>
        /// Creates an instance of this class.
        /// </summary>
        /// <param name="match">The current MatchResult</param>
        /// <param name="removeWhiteSpace">Determines if leading white space was removed</param>
        /// <param name="showingFullFile">Determines if showing full file contents or just matches</param>

        public ResultHighlighter(MatchResult match, bool removeWhiteSpace, bool showingFullFile)
            : this(match, removeWhiteSpace)
        {
            this.showingFullFile = showingFullFile;
        }

        /// <summary>
        /// The match's foreground color.
        /// </summary>
        public SolidColorBrush MatchForeground
        {
            get { return matchForeground; }
            set { matchForeground = value; }
        }

        /// <summary>
        /// The match's background color.
        /// </summary>
        public SolidColorBrush MatchBackground
        {
            get { return matchBackground; }
            set { matchBackground = value; }
        }

        /// <summary>
        /// The non-match's foreground color.
        /// </summary>
        public SolidColorBrush NonMatchForeground
        {
            get { return nonmatchForeground; }
            set { nonmatchForeground = value; }
        }

        /// <summary>
        /// Applies the specified colors to the given line.
        /// </summary>
        /// <param name="line">Current DocumentLine from AvalonEdit</param>

        protected override void ColorizeLine(DocumentLine line)
        {
            int lineStartOffset = line.Offset;
            string text = CurrentContext.Document.GetText(line);
            if (match == null || match.Matches == null || match.Matches.Count == 0 || string.IsNullOrEmpty(text))
                return;

            int lineNumber = line.LineNumber; // 1 based

            // lines in grep are 0 based array
            MatchResultLine matchLine = null;
            if (showingFullFile)
            {
                matchLine = (from m in match.Matches where m.LineNumber == lineNumber select m).FirstOrDefault();
            }
            else
            {
                matchLine = lineNumber - 1 < match.Matches.Count ? match.Matches[lineNumber - 1] : null;
            }

            string contents = matchLine != null ? matchLine.Line : string.Empty;
            bool isHit = matchLine != null ? matchLine.HasMatch : false;

            try
            {
                if (isHit && !string.IsNullOrEmpty(contents))
                {
                    int trimOffset = 0;
                    if (removeWhiteSpace)
                    {
                        trimOffset = contents.Length - contents.TrimStart().Length;
                    }

                    for (int i = 0; i < matchLine.Matches.Count; i++)
                    {
                        int startPosition = matchLine.Matches[i].StartPosition;
                        int length = matchLine.Matches[i].Length;

                        base.ChangeLinePart(
                                  lineStartOffset + (startPosition - trimOffset), // startOffset
                                  lineStartOffset + (startPosition - trimOffset) + length, // endOffset
                                  (VisualLineElement element) =>
                                  {
                                      // highlight match
                                      element.TextRunProperties.SetForegroundBrush(MatchForeground);
                                      element.TextRunProperties.SetBackgroundBrush(MatchBackground);
                                  });
                    }
                }
                else if (!isHit && !showingFullFile)
                {
                    base.ChangeLinePart(
                       lineStartOffset, // startOffset
                       lineStartOffset + line.Length, // endOffset
                       (VisualLineElement element) =>
                       {
                           // all non-matched lines are grayed out
                           element.TextRunProperties.SetForegroundBrush(NonMatchForeground);
                       });
                }
            }
            catch
            { }
        }
    }
}
