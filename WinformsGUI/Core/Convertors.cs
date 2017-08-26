using System;
using System.Collections.Generic;
using System.Text;

using bSearch.Common;

namespace bSearch.Core
{
    /// <summary>
    /// Contains common methods to convert to string/from string.
    /// </summary>
    /// <remarks>
    ///   bSearch File Searching Utility. 
    ///   Copyright (C) 2006 BigLevel Lda.
    /// 
    ///   The author may be contacted at:
    ///   suporte@bigLevel.pt or comercial@biglevel.pt
    /// </remarks>
    /// <history>
    /// [Paulo_Silva]      17/06/2017  Initial
    /// </history>
    public class Convertors
    {
        /// <summary>
        /// Retrieves all the ComboBox entries as a string.
        /// </summary>
        /// <param name="combo">ComboBox</param>
        /// <returns>string of entries</returns>

        public static string GetComboBoxEntriesAsString(System.Windows.Forms.ComboBox combo)
        {
            string[] entries = new string[combo.Items.Count];

            for (int i = 0; i < combo.Items.Count; i++)
            {
                entries[i] = combo.Items[i].ToString();
            }

            return string.Join(Constants.SEARCH_ENTRIES_SEPARATOR, entries);
        }

        /// <summary>
        /// Retrieves the values as an array of strings.
        /// </summary>
        /// <param name="values">ComboBox values as a string</param>
        /// <returns>Array of strings</returns>

        public static string[] GetComboBoxEntriesFromString(string values)
        {
            string[] entries = Utils.SplitByString(values, Constants.SEARCH_ENTRIES_SEPARATOR);

            return entries;
        }

        /// <summary>
        /// Converts a Color to a string.
        /// </summary>
        /// <param name="color">Color</param>
        /// <returns>color values as a string</returns>

        public static string ConvertColorToString(System.Drawing.Color color)
        {
            return string.Format("{0}{4}{1}{4}{2}{4}{3}", color.R.ToString(), color.G.ToString(), color.B.ToString(), color.A.ToString(), Constants.COLOR_SEPARATOR);
        }

        /// <summary>
        /// Converts a string to a Color.
        /// </summary>
        /// <param name="color">color values as a string</param>
        /// <returns>Color</returns>

        public static System.Drawing.Color ConvertStringToColor(string color)
        {
            string[] rgba = color.Split(char.Parse(Constants.COLOR_SEPARATOR));

            return System.Drawing.Color.FromArgb(byte.Parse(rgba[3]), byte.Parse(rgba[0]), byte.Parse(rgba[1]), byte.Parse(rgba[2]));
        }

        /// <summary>
        /// Converts a string to a SolidColorBrush.
        /// </summary>
        /// <param name="color">color values as a string</param>
        /// <returns>System.Windows.Media.SolidColorBrush</returns>

        public static System.Windows.Media.SolidColorBrush ConvertStringToSolidColorBrush(string color)
        {
            System.Drawing.Color dColor = ConvertStringToColor(color);

            return new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(dColor.R, dColor.G, dColor.B));
        }

        /// <summary>
        /// Converts a font to a string.
        /// </summary>
        /// <param name="font">Font</param>
        /// <returns>font values as a string</returns>

        public static string ConvertFontToString(System.Drawing.Font font)
        {
            return string.Format("{0}{3}{1}{3}{2}", font.Name, font.Size.ToString(System.Globalization.CultureInfo.InvariantCulture), font.Style.ToString(), Constants.FONT_SEPARATOR);
        }

        /// <summary>
        /// Converts a string to a Font.
        /// </summary>
        /// <param name="font">font values as a string</param>
        /// <returns>Font</returns>

        public static System.Drawing.Font ConvertStringToFont(string font)
        {
            string[] fontValues = Utils.SplitByString(font, Constants.FONT_SEPARATOR);

            return new System.Drawing.Font(fontValues[0], float.Parse(fontValues[1], System.Globalization.CultureInfo.InvariantCulture), (System.Drawing.FontStyle)Enum.Parse(typeof(System.Drawing.FontStyle), fontValues[2], true), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        }

        /// <summary>
        /// Converts given file size in bytes as string to the display type as a string.
        /// </summary>
        /// <param name="bytes">File size in bytes</param>
        /// <param name="displayType">byte,kb,mb,gb</param>
        /// <returns>file size in given display type</returns>

        public static string ConvertFileSizeForDisplay(string bytes, string displayType)
        {
            // convert bytes value to selected display size
            long value = long.Parse(bytes);
            switch (displayType.ToLower())
            {
                case "byte":
                    break;
                case "kb":
                    value = value / 1024;
                    break;
                case "mb":
                    value = value / (1024 * 1024);
                    break;
                case "gb":
                    value = value / (1024 * 1024 * 1024);
                    break;
            }

            return value.ToString();
        }

        /// <summary>
        /// Converts given file size to long for use in comparison of file sizes.
        /// </summary>
        /// <param name="textValue">TextBox value entered by user</param>
        /// <param name="sizeType">The selected size type (byte,kb,mb,gb)</param>
        /// <param name="defaultValue">The default value</param>
        /// <returns>long representing number of bytes user selected</returns>

        public static long ConvertFileSizeFromDisplay(string textValue, string sizeType, long defaultValue)
        {
            long retVal = defaultValue;

            double size;
            if (double.TryParse(textValue, out size))
            {
                switch (sizeType.ToLower())
                {
                    case "byte":
                        break;
                    case "kb":
                        size = size * 1024;
                        break;
                    case "mb":
                        size = size * 1024 * 1024;
                        break;
                    case "gb":
                        size = size * 1024 * 1024 * 1024;
                        break;
                }

                retVal = (long)size;
            }

            return retVal;
        }
    }
}
