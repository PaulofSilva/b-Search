using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace libbSearch.EncodingDetection
{
    /// <summary>
    /// Used to force a file to load with a certain Encoding.
    /// </summary>
    /// <remarks>
    /// bSearch File Searching Utility.
    /// Copyright (C) 2006 BigLevel Lda.
    /// 
    /// The author may be contacted at:
    /// suporte@bigLevel.pt or comercial@biglevel.pt
    /// </remarks>

    public class FileEncoding
    {
        private const char DELIMETER = '|';
        private const char LIST_DELIMETER = '<';

        /// <summary>
        /// Determines if file encoding is enabled or not.
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// Full file path.
        /// </summary>
        public string FilePath { get; set; }

        /// <summary>
        /// File encoding code page.
        /// </summary>
        public int CodePage { get; set; }

        /// <summary>
        /// Outputs this object to a string using the delimeter.
        /// </summary>
        /// <returns>string representation of this object</returns>

        public override string ToString()
        {
            return string.Format("{1}{0}{2}{0}{3}", DELIMETER, Enabled, FilePath, CodePage);
        }

        #region Public Static Methods

        /// <summary>
        /// Creates an instance of an FileEncoding object from a string.
        /// </summary>
        /// <param name="value">string to convert to object</param>
        /// <returns>FileEncoding object</returns>

        public static FileEncoding FromString(string value)
        {
            string[] values = value.Split(DELIMETER);

            var item = new FileEncoding();
            item.Enabled = Convert.ToBoolean(values[0]);
            item.FilePath = values[1];
            item.CodePage = Convert.ToInt32(values[2]);

            return item;
        }

        /// <summary>
        /// Converts a List of FileEncodings to a string.
        /// </summary>
        /// <param name="list">List of FileEncodings</param>
        /// <returns>string of FileEncodings</returns>

        public static string ConvertFileEncodingsToString(List<FileEncoding> list)
        {
            var builder = new System.Text.StringBuilder();

            if (list != null)
            {
                foreach (var item in list)
                {
                    if (builder.Length > 0)
                    {
                        builder.Append(LIST_DELIMETER);
                    }

                    builder.Append(item.ToString());
                }
            }

            return builder.ToString();
        }

        /// <summary>
        /// Converts the given string to a list of FileEncodings.
        /// </summary>
        /// <param name="value">string to convert</param>
        /// <returns>List of FileEncodings</returns>

        public static List<FileEncoding> ConvertStringToFileEncodings(string value)
        {
            var list = new List<FileEncoding>();

            if (!string.IsNullOrEmpty(value))
            {
                var values = value.Split(LIST_DELIMETER);

                foreach (string val in values)
                {
                    list.Add(FileEncoding.FromString(val));
                }
            }

            return list;
        }

        #endregion
    }
}
