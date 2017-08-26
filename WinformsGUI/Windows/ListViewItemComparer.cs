using System;
using System.Collections;
using System.Windows.Forms;

namespace bSearch.Windows
{
    /// <summary>
    /// Used for sorting of list view columns
    /// </summary>
    /// <remarks>
    ///   bSearch File Searching Utility.
    ///   Copyright (C) 2006 BigLevel Lda.
    /// 
    ///   The author may be contacted at:
    ///   suporte@bigLevel.pt or comercial@biglevel.pt
    /// </remarks>

    internal class ListViewItemComparer : IComparer
    {
        private int col;
        private SortOrder order;

        /// <summary>
        /// Initializes a new instance of the ListViewItemComparer class.
        /// </summary>

        public ListViewItemComparer()
        {
            col = 0;
            order = SortOrder.Ascending;
        }

        /// <summary>
        /// Initializes a new instance of the ListViewItemComparer class.
        /// </summary>
        /// <param name="column">Column to sort</param>
        /// <param name="sort">Sort Order</param>

        public ListViewItemComparer(int column, SortOrder sort)
        {
            col = column;
            order = sort;
        }

        /// <summary>
        /// Handles the comparison of the current column of ListViewItems.
        /// </summary>
        /// <param name="x">Value 1</param>
        /// <param name="y">Value 2</param>
        /// <returns>The resultant comparison of the given values based on Sort Order.</returns>

        public int Compare(object x, object y)
        {
            //Implements System.Collections.IComparer.Compare

            int _returnVal;

            // Determine whether the type being compared is a date type.
            try
            {
                if (col == Constants.COLUMN_INDEX_COUNT)
                {
                    // Parse the two objects passed as a parameter as a int.
                    int firstInt = int.Parse(((ListViewItem)x).SubItems[col].Text);
                    int secondInt = int.Parse(((ListViewItem)y).SubItems[col].Text);

                    // Compare the two integers.
                    if (firstInt < secondInt)
                        _returnVal = -1;
                    else if (firstInt > secondInt)
                        _returnVal = 1;
                    else
                        _returnVal = 0;
                }
                else if (col == Constants.COLUMN_INDEX_SIZE)
                {
                    // Parse the two objects passed as a parameter as a long.
                    long firstInt = Convert.ToInt64(((ListViewItem)x).SubItems[col].Tag);
                    long secondInt = Convert.ToInt64(((ListViewItem)y).SubItems[col].Tag);

                    // Compare the two integers.
                    if (firstInt < secondInt)
                        _returnVal = -1;
                    else if (firstInt > secondInt)
                        _returnVal = 1;
                    else
                        _returnVal = 0;
                }
                else if (col == Constants.COLUMN_INDEX_DATE)
                {
                    // Parse the two objects passed as a parameter as a DateTime.
                    System.DateTime firstDate = DateTime.Parse(((ListViewItem)x).SubItems[col].Text);

                    System.DateTime secondDate = DateTime.Parse(((ListViewItem)y).SubItems[col].Text);

                    // Compare the two dates.
                    _returnVal = DateTime.Compare(firstDate, secondDate);

                    // If neither compared object has a valid date format, 
                    // compare as a string.
                }
                else
                {
                    // Compare the two items as a string.
                    _returnVal = string.Compare(((ListViewItem)x).SubItems[col].Text, ((ListViewItem)y).SubItems[col].Text);
                }
            }
            catch
            {
                // Compare the two items as a string.
                _returnVal = string.Compare(((ListViewItem)x).SubItems[col].Text, ((ListViewItem)y).SubItems[col].Text);
            }

            // Determine whether the sort order is descending.
            if (order == SortOrder.Descending)
            {
                // Invert the value returned by String.Compare.
                _returnVal *= -1;
            }

            return _returnVal;
        }
    }
}