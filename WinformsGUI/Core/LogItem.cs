using System;
using System.Collections.Generic;
using System.Text;

namespace bSearch.Core
{
    /// <summary>
    /// Class to contain a log item entry (status, exclusion, error).
    /// </summary>
    /// <remarks>
    ///   bSearch File Searching Utility.
    ///   Copyright (C) 2006 BigLevel Lda.
    /// 
    ///   The author may be contacted at:
    ///   suporte@bigLevel.pt or comercial@biglevel.pt
    /// </remarks>

    public class LogItem
    {
        /// <summary>
        /// Types of log item messages.
        /// </summary>
        public enum LogItemTypes
        {
            /// <summary>Status messages</summary>
            Status = 0,
            /// <summary>Exclusion messages</summary>
            Exclusion = 1,
            /// <summary>Error messages</summary>
            Error = 2
        }

        /// <summary>The item message type.</summary>
        public LogItemTypes ItemType { get; set; }

        /// <summary>The value of the message.</summary>
        public string Value { get; set; }

        /// <summary>Any extra details for the message.</summary>
        public string Details { get; set; }

        /// <summary>The message timestamp.</summary>
        public DateTime Date { get; private set; }

        /// <summary>
        /// Creates an instance of this class.
        /// </summary>
        /// <param name="type">Item Type</param>
        /// <param name="value">Value</param>
        public LogItem(LogItemTypes type, string value)
        {
            ItemType = type;
            Value = value;
            Details = string.Empty;
            Date = DateTime.Now;
        }

        /// <summary>
        /// Creates an instance of this class.
        /// </summary>
        /// <param name="type">Item Type</param>
        /// <param name="value">Value</param>
        /// <param name="details">Details</param>
        public LogItem(LogItemTypes type, string value, string details)
            : this(type, value)
        {
            Details = details;
        }

        /// <summary>
        /// String representation of this class (Type - Value).
        /// </summary>
        /// <returns>String showing Type - Value</returns>
        public override string ToString()
        {
            return string.Format("{0} - {1}", ItemType.ToString(), Value);
        }
    }
}
