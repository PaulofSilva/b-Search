using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;

using libbSearch;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Rendering;

namespace bSearch.Windows.Controls
{
    /// <summary>
    /// Handles highlighting of all results when displayed.
    /// </summary>
    /// <remarks>
    ///   bSearch File Searching Utility. 
    ///   Copyright (C) 2006 BigLevel Lda.
    /// 
    ///   The author may be contacted at:
    ///   suporte@bigLevel.pt or comercial@biglevel.pt
    /// </remarks>
    /// <history>
    /// [Paulo_Silva]	   04/08/2015	ADD: switch from Rich Text Box to AvalonEdit
    /// </history>
    public class AllResultHighlighter : DocumentColorizingTransformer
    {
        private IList<MatchResult> matches;
        private bool removeWhiteSpace = false;
        private SolidColorBrush matchForeground = new SolidColorBrush(Colors.White);
        private SolidColorBrush matchBackground = new SolidColorBrush(Color.FromRgb(251, 127, 6));
        private SolidColorBrush nonmatchForeground = new SolidColorBrush(Color.FromRgb(192, 192, 192));

        /// <summary>
        /// Creates an instance of this class.
        /// </summary>
        /// <param name="matches">List of all matches</param>
        /// <param name="removeWhiteSpace">Determines if leading white space was removed</param>

        public AllResultHighlighter(IList<MatchResult> matches, bool removeWhiteSpace)
        {
            this.matches = matches;
            this.removeWhiteSpace = removeWhiteSpace;
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

            if (matches == null || matches.Count == 0 || string.IsNullOrEmpty(text))
                return;

            // find what type of line this is, either the file path or a result line
            bool isFileName = false;
            MatchResultLine matchLine = null;
            foreach (MatchResult result in matches)
            {
                if (result.File.FullName.Equals(text, StringComparison.OrdinalIgnoreCase))
                {
                    isFileName = true;
                    break;
                }
                else
                {
                    foreach (var matchResultLine in result.Matches)
                    {
                        string lineText = matchResultLine.Line;
                        if (removeWhiteSpace)
                        {
                            lineText = lineText.TrimStart();
                        }

                        if (lineText.Equals(text))
                        {
                            matchLine = matchResultLine;
                            break;
                        }
                    }
                }
            }

            try
            {
                if (isFileName)
                {
                    base.ChangeLinePart(
                       lineStartOffset, // startOffset
                       lineStartOffset + line.Length, // endOffset
                       (VisualLineElement element) =>
                       {
                           // bold current typeface for file name display
                           Typeface tf = element.TextRunProperties.Typeface;
                           var tfNew = new Typeface(tf.FontFamily, tf.Style, System.Windows.FontWeights.Bold, tf.Stretch);
                           element.TextRunProperties.SetTypeface(tfNew);
                       });
                }
                else
                {
                    if (matchLine != null && matchLine.HasMatch)
                    {
                        int trimOffset = 0;
                        if (removeWhiteSpace)
                        {
                            trimOffset = matchLine.Line.Length - matchLine.Line.TrimStart().Length;
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
                    else
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
            }
            catch
            { }
        }
    }
}
