using System;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

using bSearch.Core;
using bSearch.Common.Logging;
using libbSearch;

namespace bSearch.Output
{
    /// <summary>
    /// Helper class when outputing results to HTML.
    /// </summary>
    /// <remarks>
    /// bSearch File Searching Utility.
    /// Copyright (C) 2006 BigLevel Lda.
    /// 
    /// The author may be contacted at:
    /// suporte@bigLevel.pt or comercial@biglevel.pt
    /// </remarks>

    public class HTMLHelper
    {
        private HTMLHelper()
        { }

        /// <summary>
        /// Retrieves a given file's contents from the embed resource.
        /// </summary>
        /// <param name="fileName">string containing resource file to retrieve.</param>

        public static string GetContents(string fileName)
        {
            try
            {
                System.Reflection.Assembly _assembly = System.Reflection.Assembly.GetExecutingAssembly();
                string _name = _assembly.GetName().Name;
                string _contents = string.Empty;

                Stream _stream = _assembly.GetManifestResourceStream(string.Format("{0}.Output.{1}", _name, fileName));

                if (_stream != null)
                {
                    using (StreamReader _reader = new StreamReader(_stream))
                    {
                        _contents = _reader.ReadToEnd();
                    }
                    _stream.Close();
                }
                _assembly = null;

                return _contents;
            }
            catch (Exception ex)
            {
                LogClient.Instance.Logger.Error("Unable to load html file contents from resource at {0} with message {1}", fileName, ex.Message);
            }

            return string.Empty;
        }

        /// <summary>
        /// Returns the given line with the search text highlighted.
        /// </summary>
        /// <param name="line">Line to check</param>
        /// <param name="grep">Grep Object containing options</param>
        /// <returns>Line with search text highlighted</returns>

        public static string GetHighlightLine(string line, Grep grep)
        {
            string newLine = string.Empty;

            if (!string.IsNullOrEmpty(grep.SearchSpec.SearchText))
            {
                if (grep.SearchSpec.UseRegularExpressions)
                    newLine = HighlightRegEx(line, grep);
                else
                    newLine = HighlightNormal(line, grep);
            }

            return newLine + "<br />";
        }

        /// <summary>
        /// Replaces all the css holders in the given text.
        /// </summary>
        /// <param name="css">Text containing holders</param>
        /// <returns>Text with holders replaced</returns>

        public static string ReplaceCssHolders(string css)
        {
            css = css.Replace("%%resultback%%", System.Drawing.ColorTranslator.ToHtml(Convertors.ConvertStringToColor(bSearch.Core.GeneralSettings.ResultsBackColor)));
            css = css.Replace("%%resultfore%%", System.Drawing.ColorTranslator.ToHtml(Convertors.ConvertStringToColor(bSearch.Core.GeneralSettings.ResultsForeColor)));
            css = css.Replace("%%highlightfore%%", System.Drawing.ColorTranslator.ToHtml(Convertors.ConvertStringToColor(bSearch.Core.GeneralSettings.HighlightForeColor)));
            css = css.Replace("%%highlightback%%", System.Drawing.ColorTranslator.ToHtml(Convertors.ConvertStringToColor(bSearch.Core.GeneralSettings.HighlightBackColor)));

            return css;
        }

        /// <summary>
        /// Replaces all the search option holders in the given text.
        /// </summary>
        /// <param name="text">Text containing holders</param>
        /// <param name="grep">Grep object containing settings</param>
        /// <param name="totalHits">Number of total hits</param>
        /// <param name="showLineNumbers">Determines whether to show line numbers</param>
        /// <param name="removeLeadingWhiteSpace">Determines whether to remove leading white space</param>
        /// <returns>Text with holders replaced</returns>

        public static string ReplaceSearchOptions(string text, Grep grep, int totalHits, bool showLineNumbers, bool removeLeadingWhiteSpace)
        {
            var spec = grep.SearchSpec;

            string searchPaths = string.Empty;
            if (spec.StartFilePaths != null && spec.StartFilePaths.Length > 0)
            {
                searchPaths = string.Join(", ", spec.StartFilePaths);
            }
            else
            {
                searchPaths = string.Join(", ", spec.StartDirectories);
            }

            text = text.Replace("%%totalhits%%", totalHits.ToString());
            text = text.Replace("%%year%%", DateTime.Now.Year.ToString());
            text = text.Replace("%%searchpaths%%", "Search Path(s): " + searchPaths);
            text = text.Replace("%%filetypes%%", "File Types: " + grep.SearchSpec.FileFilter);
            text = text.Replace("%%regex%%", "Regular Expressions: " + spec.UseRegularExpressions);
            text = text.Replace("%%casesen%%", "Case Sensitive: " + spec.UseCaseSensitivity);
            text = text.Replace("%%wholeword%%", "Whole Word: " + spec.UseWholeWordMatching);
            text = text.Replace("%%recurse%%", "Subfolders: " + spec.SearchInSubfolders);
            text = text.Replace("%%filenameonly%%", "Show File Names Only: " + spec.ReturnOnlyFileNames);
            text = text.Replace("%%negation%%", "Negation: " + spec.UseNegation);
            text = text.Replace("%%linenumbers%%", "Line Numbers: " + showLineNumbers);
            text = text.Replace("%%removeleadingwhitespace%%", "Remove Leading White Space: " + removeLeadingWhiteSpace);
            text = text.Replace("%%contextlines%%", "Context Lines: " + spec.ContextLines);

            // filter items
            StringBuilder filterBuilder = new StringBuilder();
            if (grep.SearchSpec.FilterItems != null)
            {
                filterBuilder.Append("Exclusions:<br/>");
                foreach (FilterItem item in grep.SearchSpec.FilterItems)
                {
                    string option = item.ValueOption.ToString();
                    if (item.ValueOption == FilterType.ValueOptions.None)
                    {
                        option = string.Empty;
                    }
                    filterBuilder.AppendFormat("<span class=\"indent\">{0} -> {1}: {2} {3} {4}</span><br/>",
                       item.FilterType.Category,
                       item.FilterType.SubCategory,
                       item.Value,
                       option,
                       !string.IsNullOrEmpty(option) && item.ValueIgnoreCase ? " (ignore case)" : string.Empty
                    );
                }
            }
            text = text.Replace("%%exclusions%%", filterBuilder.ToString());

            // %%searchmessage%%
            string searchMessage = string.Empty;
            if (!string.IsNullOrEmpty(grep.SearchSpec.SearchText))
            {
                if (grep.SearchSpec.ReturnOnlyFileNames)
                {
                    searchMessage = string.Format("{0} was {1}found in {2} file{3}", spec.SearchText, spec.UseNegation ? "not " : "", grep.MatchResults.Count, grep.MatchResults.Count > 1 ? "s" : "");
                }
                else
                {
                    searchMessage = string.Format("{0} was found {1} time{2} in {3} file{4}", spec.SearchText, totalHits, totalHits > 1 ? "s" : "", grep.MatchResults.Count, grep.MatchResults.Count > 1 ? "s" : "");
                }
            }
            text = text.Replace("%%searchmessage%%", searchMessage);

            return text;
        }

        #region Private Methods

        /// <summary>
        /// Returns the given line with the search text highlighted.
        /// </summary>
        /// <param name="line">Line to check</param>
        /// <param name="grep">Grep Object containing options</param>
        /// <returns>Line with search text highlighted</returns>

        private static string HighlightNormal(string line, Grep grep)
        {
            StringBuilder newLine = new StringBuilder();
            string searchText = grep.SearchSpec.SearchText;
            string tempLine = line;

            // attempt to locate the text in the line
            int pos = tempLine.IndexOf(searchText, grep.SearchSpec.UseCaseSensitivity ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase);

            if (pos > -1)
            {
                while (pos > -1)
                {
                    //retrieve parts of text
                    var begin = tempLine.Substring(0, pos);
                    var text = tempLine.Substring(pos, searchText.Length);
                    var end = tempLine.Substring(pos + searchText.Length);

                    newLine.Append(WebUtility.HtmlEncode(begin));

                    // do a check to see if begin and end are valid for wholeword searches
                    bool highlight = true;
                    if (grep.SearchSpec.UseWholeWordMatching)
                    {
                        highlight = Grep.WholeWordOnly(begin, end);
                    }

                    // set highlight color for searched text
                    if (highlight)
                    {
                        newLine.AppendFormat("<span class=\"searchtext\">{0}</span>", WebUtility.HtmlEncode(text));
                    }
                    else
                    {
                        newLine.Append(WebUtility.HtmlEncode(text));
                    }

                    // Check remaining string for other hits in same line
                    pos = end.IndexOf(searchText, grep.SearchSpec.UseCaseSensitivity ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase);

                    // set default color for end, if no more hits in line
                    tempLine = end;
                    if (pos < 0)
                    {
                        newLine.Append(WebUtility.HtmlEncode(end));
                    }
                }
            }
            else
            {
                newLine.Append(WebUtility.HtmlEncode(tempLine));
            }

            return newLine.ToString();
        }

        /// <summary>
        /// Returns the given line with the search text highlighted.
        /// </summary>
        /// <param name="line">Line to check</param>
        /// <param name="grep">Grep Object containing options</param>
        /// <returns>Line with search text highlighted</returns>

        private static string HighlightRegEx(string line, Grep grep)
        {
            string tempString = string.Empty;
            int lastPos = 0;
            Regex regEx;
            MatchCollection matchCollection;
            Match match;
            StringBuilder newLine = new StringBuilder();

            // find all reg ex matches in line
            string pattern = string.Format("{0}{1}{0}", grep.SearchSpec.UseWholeWordMatching ? "\\b" : string.Empty, grep.SearchSpec.SearchText);
            RegexOptions options = grep.SearchSpec.UseCaseSensitivity ? RegexOptions.None : RegexOptions.IgnoreCase;
            regEx = new Regex(pattern, options);
            matchCollection = regEx.Matches(line);

            // loop through the matches
            for (int i = 0; i < matchCollection.Count; i++)
            {
                match = matchCollection[i];

                // check for empty string to prevent assigning nothing to selection text preventing
                //  a system beep
                tempString = line.Substring(lastPos, match.Index - lastPos);
                if (!tempString.Equals(string.Empty))
                {
                    newLine.Append(WebUtility.HtmlEncode(tempString));
                }

                // set the hit text
                newLine.AppendFormat("<span class=\"searchtext\">{0}</span>", WebUtility.HtmlEncode(line.Substring(match.Index, match.Length)));

                // set the end text
                if (i + 1 >= matchCollection.Count)
                {
                    // no more hits so just set the rest
                    newLine.Append(WebUtility.HtmlEncode(line.Substring(match.Index + match.Length)));

                    lastPos = match.Index + match.Length;
                }
                else
                {
                    // another hit so just set inbetween
                    newLine.Append(WebUtility.HtmlEncode(line.Substring(match.Index + match.Length, matchCollection[i + 1].Index - (match.Index + match.Length))));
                    lastPos = matchCollection[i + 1].Index;
                }
            }

            if (matchCollection.Count == 0)
            {
                // no match, just a context line
                newLine.Append(WebUtility.HtmlEncode(line));
            }

            return newLine.ToString();
        }
        #endregion
    }
}
