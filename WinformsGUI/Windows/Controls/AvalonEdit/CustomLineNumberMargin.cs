using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.TextFormatting;

using ICSharpCode.AvalonEdit.Editing;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Rendering;
using ICSharpCode.AvalonEdit.Utils;

using libbSearch;

namespace bSearch.Windows.Controls
{
    /// <summary>
    /// Handles custom line numbering in margin when given specific line numbers.
    /// </summary>
    /// <remarks>
    /// bSearch File Searching Utility.
    /// Copyright (C) 2006 BigLevel Lda.
    /// 
    /// The author may be contacted at:
    /// suporte@bigLevel.pt or comercial@biglevel.pt
    /// 
    /// 
    /// Design Note:
    /// If List is equal to number of total lines, then use each specified, otherwise
    /// use only given LineNumbers that match to the given line number displayed.
    /// 
    /// </remarks>

    public class CustomLineNumberMargin : LineNumberMargin
    {
        /// <summary>
        /// Current list of line numbers to display.
        /// </summary>
        public List<LineNumber> LineNumbers { get; set; }

        /// <summary>
        /// Overrides to calculate size based on current line number max length.
        /// </summary>
        /// <param name="availableSize">available size</param>
        /// <returns>Size value of margin width</returns>
        /// <remarks>Defaults to 2</remarks>

        protected override System.Windows.Size MeasureOverride(System.Windows.Size availableSize)
        {
            int numberLength = 2;
            if (LineNumbers != null && LineNumbers.Count == this.Document.LineCount)
            {
                var list = (from n in LineNumbers where n.Number > -1 select n.Number);
                if (list != null && list.Count() > 0)
                {
                    numberLength = list.Max().ToString().Length;
                }
            }
            else
            {
                numberLength = maxLineNumberLength;
            }

            if (numberLength < 2)
            {
                numberLength = 2;
            }

            typeface = CreateTypeface(this);
            emSize = (double)GetValue(TextBlock.FontSizeProperty);

            FormattedText text = CreateFormattedText(
               this,
               new string('9', numberLength),
               typeface,
               emSize,
               (Brush)GetValue(Control.ForegroundProperty)
            );
            return new Size(text.Width, 0);
        }

        /// <summary>
        /// Renders the current line numbers to the margin.
        /// </summary>
        /// <param name="drawingContext">The current drawing context used to write line numbers</param>

        protected override void OnRender(System.Windows.Media.DrawingContext drawingContext)
        {
            TextView textView = this.TextView;
            Size renderSize = this.RenderSize;
            if (textView != null && textView.VisualLinesValid)
            {
                var foreground = (Brush)GetValue(Control.ForegroundProperty);  // non-match line
                var matchForeground = (Brush)GetValue(Control.BackgroundProperty); // match line

                foreach (VisualLine line in textView.VisualLines)
                {
                    int lineNumber = line.FirstDocumentLine.LineNumber;
                    bool isMatch = false;
                    if (LineNumbers != null)
                    {
                        // all line numbers are specified
                        if (LineNumbers.Count == this.Document.LineCount)
                        {
                            lineNumber = -1;

                            if (line.FirstDocumentLine.LineNumber - 1 < LineNumbers.Count)
                            {
                                lineNumber = LineNumbers[line.FirstDocumentLine.LineNumber - 1].Number;
                                isMatch = LineNumbers[line.FirstDocumentLine.LineNumber - 1].HasMatch;
                            }
                        }
                        else
                        {
                            // look for only a given line number since not all are specified
                            LineNumber lineNum = (from n in LineNumbers where n.Number == line.FirstDocumentLine.LineNumber select n).FirstOrDefault();
                            if (lineNum != null)
                            {
                                lineNumber = lineNum.Number;
                                isMatch = lineNum.HasMatch;
                            }
                        }
                    }

                    FormattedText text = CreateFormattedText(
                       this,
                       lineNumber > -1
                          ? lineNumber.ToString(CultureInfo.CurrentCulture)
                          : string.Empty,
                          typeface, emSize, isMatch ? matchForeground : foreground
                    );
                    drawingContext.DrawText(text, new Point(renderSize.Width - text.Width,
                                                            line.VisualTop - textView.VerticalOffset));
                }
            }
        }

        /// <summary>
        /// Copied from source code of AvalonEdit in order to use in custom implemenation.
        /// </summary>
        /// <param name="element">Current framework element</param>
        /// <param name="text">Current text</param>
        /// <param name="typeface">Current typeface</param>
        /// <param name="emSize">Requested size</param>
        /// <param name="foreground">Current foreground brush</param>
        /// <returns>FormattedText for framework element</returns>

        private FormattedText CreateFormattedText(FrameworkElement element, string text, Typeface typeface, double? emSize, Brush foreground)
        {
            return new FormattedText(
               text,
               CultureInfo.CurrentCulture,
               FlowDirection.LeftToRight,
               typeface,
               emSize.Value,
               foreground,
               null,
               TextOptions.GetTextFormattingMode(element)
            );
        }

        /// <summary>
        /// Copied from source code of AvalonEdit in order to use in custom implemenation.
        /// </summary>
        /// <param name="fe">Current framework element</param>
        /// <returns>Typeface based on current element</returns>

        private Typeface CreateTypeface(FrameworkElement fe)
        {
            return new Typeface((FontFamily)fe.GetValue(TextBlock.FontFamilyProperty),
                                (FontStyle)fe.GetValue(TextBlock.FontStyleProperty),
                                (FontWeight)fe.GetValue(TextBlock.FontWeightProperty),
                                (FontStretch)fe.GetValue(TextBlock.FontStretchProperty));
        }
    }
}
